// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq; 
using System.Diagnostics;
using System.Collections; 
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Diagnostics.Eventing.Reader;
using System.Collections.ObjectModel;

namespace AppLocker_Policy_Converter
{

    internal static class Helper
    {

        public enum BrowseFileType
        {
            Policy = 0,     // -Show .xml files
            EventLog = 1,   // -Show .evtx files
            PEFile = 2,     // -Show PE (.exe, .dll, .sys) files
            All = 3         // -Show . all files
        }

        public static AppLockerPolicy SerializeAppLockerPolicy(string xmlPath)
        {
            AppLockerPolicy appLockerPolicy;
            if (xmlPath == null)
            {
                return null;
            }
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AppLockerPolicy));
                StreamReader reader = new StreamReader(xmlPath);
                appLockerPolicy = (AppLockerPolicy)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception exp)
            {
                return null;
            }

            return appLockerPolicy;

        }

        /// <summary>
        /// Deserialize the xml policy on disk to SiPolicy
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns>SiPolicy object</returns>
        public static SiPolicy DeserializeXMLtoPolicy(string xmlPath)
        {
            SiPolicy siPolicy;
            if (xmlPath == null)
            {
                return null;
            }
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SiPolicy));
                StreamReader reader = new StreamReader(xmlPath);
                siPolicy = (SiPolicy)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception exp)
            {
                return null;
            }

            return siPolicy;
        }

        public static void ConvertFilePublisherRule(FilePublisherRuleType filePubRule)
        {
            string action = (String)filePubRule.Action.ToString();
            string desc = filePubRule.Description;
            string binName = filePubRule.Conditions.FilePublisherCondition.BinaryName;
            string lowVersion = filePubRule.Conditions.FilePublisherCondition.BinaryVersionRange.LowSection;
            string highVerson = filePubRule.Conditions.FilePublisherCondition.BinaryVersionRange.HighSection;
            string product = filePubRule.Conditions.FilePublisherCondition.ProductName;
            string publisher = filePubRule.Conditions.FilePublisherCondition.PublisherName;
        }

        public static void ConvertFileHashRule(FileHashRuleType fileHashRule)
        {
            string action = fileHashRule.Action.ToString();
            string desc = fileHashRule.Description;

            // Iterate through these
            string algo = fileHashRule.Conditions.FileHashCondition[0].Type.ToString(); //Type == SHA256
            string hash = fileHashRule.Conditions.FileHashCondition[0].Data;

            string binName = fileHashRule.Conditions.FileHashCondition[0].SourceFileName;
        }

        public static void ConvertFilePathRule(FilePathRuleType filePathRule)
        {
            string action = (String)filePathRule.Action.ToString();
            string desc = filePathRule.Description;

            string path = filePathRule.Conditions.FilePathCondition.Path;
        }

        /// <summary>
        /// Serialize the SiPolicy object to XML file
        /// </summary>
        /// <param name="siPolicy">SiPolicy object</param>
        /// <param name="xmlPath">Path to serialize the SiPolicy to</param>
        public static void SerializePolicytoXML(SiPolicy siPolicy, string xmlPath)
        {
            if (siPolicy == null || xmlPath == null)
            {
                return;
            }

            // Serialize policy to XML file
            XmlSerializer serializer = new XmlSerializer(typeof(SiPolicy));
            StreamWriter writer = new StreamWriter(xmlPath);
            serializer.Serialize(writer, siPolicy);
            writer.Close();
        }

        // Check that publisher does not have multiple instances of '='
        // That could indicate more fields like C=, L=, S= have been provided
        // TODO: simply parse for CN only
        public static bool IsValidPublisher(string publisher)
        {
            if (String.IsNullOrEmpty(publisher))
            {
                return false;
            }

            var pubParts = publisher.Split('=');
            if (pubParts.Length > 2)
            {
                return false;
            }

            return true;
        }

        public static string FormatPublisherCN(string publisher)
        {
            string formattedPub;

            var pubParts = publisher.Split('=');
            if (pubParts.Length == 2)
            {
                // Ex) ["CN =", "   Contoso Corporation"]
                formattedPub = pubParts[1];
            }
            else
            {
                formattedPub = publisher;
            }

            // Remove any prepended whitespace
            char[] charsToTrim = { ' ', '\'' };
            formattedPub = formattedPub.Trim(charsToTrim);

            return formattedPub;
        }

        // Check that version has 4 parts (follows ww.xx.yy.zz format)
        // And each part < 2^16
        public static bool IsValidVersion(string version)
        {
            var versionParts = version.Split('.');
            if (versionParts.Length != 4)
            {
                return false;
            }

            foreach (var part in versionParts)
            {
                try
                {
                    int _part = Convert.ToInt32(part);
                    if (_part > UInt16.MaxValue || _part < 0)
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return true;
        }

        public static int CompareVersions(string minVersion, string maxVersion)
        {
            var minversionParts = minVersion.Split('.');
            var maxversionParts = maxVersion.Split('.');

            for (int i = 0; i < 4; i++)
            {
                int minVerPart = Convert.ToInt32(minversionParts[i]);
                int maxVerPart = Convert.ToInt32(maxversionParts[i]);

                if (minVerPart > maxVerPart)
                {
                    return -1;
                }
                else if (minVerPart < maxVerPart)
                {
                    return 1;
                }
            }

            return 0;
        }

        public static bool IsValidPathRule(string customPath)
        {
            // Check for at most 1 wildcard param (*)
            if (customPath.Contains("*"))
            {
                var wildCardParts = customPath.Split('*');
                if (wildCardParts.Length > 2)
                {
                    return false;
                }
                else
                {
                    // Start or end must be empty
                    if (String.IsNullOrEmpty(wildCardParts[0]) || String.IsNullOrEmpty(wildCardParts[1]))
                    {
                        // Continue - either side is empty
                    }
                    else
                    {
                        // wildcard in middle of path - not supported
                        return false;
                    }
                }
            }

            // Check for macros (%OSDRIVE%, %WINDIR%, %SYSTEM32%)
            if (customPath.Contains("%"))
            {
                var macroParts = customPath.Split('%');
                if (macroParts.Length == 3)
                {
                    if (macroParts[1] == "OSDRIVE" || macroParts[1] == "WINDIR" || macroParts[1] == "SYSTEM32")
                    {
                        // continue with rest of checks
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    // Too many or too few '%'
                    return false;
                }
            }
            return true;
        }

        public static string GetEnvPath(string _path)
        {
            // if the path contains one of the following environment variables -- return true as the cmdlets can replace it
            string sys = Environment.GetFolderPath(Environment.SpecialFolder.System).ToUpper();
            string win = Environment.GetFolderPath(Environment.SpecialFolder.Windows).ToUpper();
            string os = Path.GetPathRoot(Environment.SystemDirectory).ToUpper();

            string envPath = String.Empty;
            string upperPath = _path.ToUpper();

            if (upperPath.Contains(sys)) // C:/WINDOWS/system32/foo/bar --> %SYSTEM32%/foo/bar
            {
                envPath = "%SYSTEM32%" + _path.Substring(sys.Length);
                return envPath;
            }
            else if (upperPath.Contains(win)) // WINDIR
            {
                envPath = "%WINDIR%" + _path.Substring(win.Length);
                return envPath;
            }
            else if (upperPath.Contains(os)) // OSDRIVE
            {
                envPath = "%OSDRIVE%\\" + _path.Substring(os.Length);
                return envPath;
            }
            else
            {
                return _path; // otherwise, not an env variable we support
            }

        }

        /// <summary>
        /// Check that the given directory is write-accessable by the user.  
        /// /// </summary>
        public static bool IsUserWriteable(string path)
        {
            // Try to create a subdir in the folderPath. If successful, write access is true. 
            // If an exception is hit, the path is likely not user-writeable 
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                if (di.Exists)
                {
                    DirectoryInfo dis = new DirectoryInfo(Path.Combine(path, "testSubDir"));
                    dis.Create();
                    dis.Delete();
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }


        /// <summary>
        /// Convert OID/EKU data type to TLV triplet according to schema https://docs.microsoft.com/en-us/windows/win32/seccertenroll/about-object-identifier?redirectedfrom=MSDN
        /// </summary>
        /// <param name="oid"></param>
        /// <returns>Encoded TLV string containing the EKU</returns>
        public static string EKUValueToTLVEncoding(string stringEku)
        {
            List<string> encodedEku = new List<string>();

            // CONST
            const string N_EKU = "01"; // Only support 1 EKU at a time


            if (String.IsNullOrEmpty(stringEku))
            {
                return null;
            }

            var ekuParts = stringEku.Split('.');

            if (ekuParts == null || ekuParts.Length < 2)
            {
                return null;
            }

            // Ensure properly formatted oids provided
            foreach (var ekuPart in ekuParts)
            {
                try
                {
                    int.Parse(ekuPart);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Bad oid format. NAN values provided.");
                    return null;
                }
            }

            encodedEku.Add(FormatHexString(N_EKU));
            encodedEku.Add(FormatHexString(ekuParts.Length.ToString("X")));

            // The first two nodes of the OID are encoded onto a single byte. The first node is multiplied by the decimal 40 and the result is added to the value of the second node.
            int firstByteD = 40 * int.Parse(ekuParts[0]) + int.Parse(ekuParts[1]);
            string firstByteHex = FormatHexString(firstByteD.ToString("X"));

            encodedEku.Add(firstByteHex);

            // Convert the rest of the EKU/OID from pos 2 to end
            foreach (var ekuPart in ekuParts.Skip(2))
            {
                List<string> ekuOctet = GetEKUOctet(int.Parse(ekuPart));
                foreach (var octet in ekuOctet)
                {
                    encodedEku.Add(octet);
                }
            }

            return string.Join("", encodedEku);
        }

        public static List<string> GetEKUOctet(int node)
        {
            List<string> octet = new List<string>();
            const int MAX_EKU_VAL = 127;
            const int MSB = 0;
            const int IGNORE_POS = 9;

            if (node > MAX_EKU_VAL)
            {
                // Node values greater than or equal to 128 are encoded on multiple bytes.
                // Bit 7 of the leftmost byte is set to one.
                // Bits 0 through 6 of each byte contains the encoded value.
                string s = Convert.ToString(node, 2); //Convert to binary in a string

                int[] bits = s.PadLeft(16, '0') // Add 0's from left
                             .Select(c => int.Parse(c.ToString())) // convert each char to int
                             .ToArray();

                // MSB (bit 7 of the leftmost byte) set to 1
                // Ignoring bit 7 of the rightmost byte (ie set to 0)
                // Shifting the right nibble of the leftmost byte by 1 bit
                bits[MSB] = 1;
                bits[IGNORE_POS] = 0;

                bits[4] = bits[5];
                bits[5] = bits[6];
                bits[6] = bits[7];
                bits[7] = 0;

                // Convert left and right byte to hex to add to octet list
                List<string> decArr = DecodeBitArray(bits);
                octet.Add(decArr[0] + decArr[1]);
                octet.Add(decArr[2] + decArr[3]);
            }
            else
            {
                // Node values less than or equal to 127 are encoded on one byte.
                octet.Add(FormatHexString(node.ToString("X")));
            }

            return octet;
        }

        /// <summary>
        ///  Simple method to ensure hex values are size=2
        /// </summary>
        /// <param name="hex_in"></param>
        /// <returns></returns>
        public static string FormatHexString(string hex_in)
        {
            if (String.IsNullOrEmpty(hex_in))
            {
                return String.Empty;
            }
            if (hex_in.Length == 1)
            {
                return "0" + hex_in;
            }
            else
            {
                return hex_in;
            }
        }

        /// <summary>
        /// Convert bit array to hexidecimal values
        /// </summary>
        /// <param name="bits"></param>
        /// <returns></returns>
        public static List<string> DecodeBitArray(int[] bits)
        {
            List<string> octet = new List<string>();
            int val = 0;

            for (int i = 0; i < bits.Length; i++)
            {
                if (i % 4 == 0)
                {
                    val = 0;
                    val += 8 * bits[i];
                }
                else if (i % 4 == 1)
                {
                    val += 4 * bits[i];
                }
                else if (i % 4 == 2)
                {
                    val += 2 * bits[i];
                }
                else if (i % 4 == 3)
                {
                    val += 1 * bits[i];
                    octet.Add(val.ToString("X"));
                }
            }

            return octet;
        }

        /// <summary>
        /// Calls DateTime.UTCNow and formats to ISO 8601 (YYYY-MM-DD)
        /// </summary>
        /// <returns>DateTime string in format YYYY-MM-DD</returns>
        public static string GetFormattedDate()
        {
            // Get DateTime now in UTC
            // Format to ISO 8601 (YYYY-MM-DD)
            return DateTime.UtcNow.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Calls DateTime.UTCNow and formats to ISO 8601 (T[hh][mm][ss])
        /// </summary>
        /// <returns>DateTime string in format T[hh][mm][ss]</returns>
        public static string GetFormattedTime()
        {
            // Get DateTime now in UTC
            // Format to ISO 8601 (T[hh][mm][ss])
            return "T" + DateTime.UtcNow.ToString("HH-mm-ss");
        }

        /// <summary>
        /// Calls DateTime.UTCNow and formats to ISO 8601 (YYYY-MM-DD-T[hh][mm][ss])
        /// </summary>
        /// <returns>DateTime string in format YYYY-MM-DD-T[hh][mm][ss]</returns>
        public static string GetFormattedDateTime()
        {
            // Get DateTime now in UTC
            // Format to ISO 8601 (T[hh][mm][ss])
            return DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH-mm-ss");
        }


    }
}