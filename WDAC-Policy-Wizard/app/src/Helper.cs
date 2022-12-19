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
using System.Xml.Serialization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Win32;
using System.Diagnostics.Eventing.Reader;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.Windows.Forms;

namespace WDAC_Wizard
{

    internal static class Helper
    {
        const int AUDIT_PE = 3076;
        const int BLOCK_PE = 3077;
        const int AUDIT_KERNEL = 3067;
        const int BLOCK_KERNEL = 3068;
        const int AUDIT_SCRIPT = 8028;
        const int BLOCK_SCRIPT = 8029;

        const string AUDIT_POLICY_PATH = "AuditEvents_Policy.xml";
        const string AUDIT_LOG_PATH = "AuditEvents_Log.txt";

        // Signing Scenario Constants
        const int KMCISCN = 131;
        const int UMCISCN = 12; 
        public enum BrowseFileType
        {
            Policy = 0,     // -Show .xml files
            EventLog = 1,   // -Show .evtx files
            PEFile = 2,     // -Show PE (.exe, .dll, .sys) files
            All = 3         // -Show . all files
        }

        public static string GetDOSPath(string NTPath)
        {
            string windowsDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            const int WINDOWS_L = 7;
            string logicalDisk = windowsDir.Substring(0, windowsDir.Length - WINDOWS_L); // Gets the logical disk name string of the harddrive

            // Regex replace to take the NT path and convert to DOS Path

            string pattern = @"\\\\[a-zA-Z]+\\\\[a-zA-Z]+[0-9]+\\\\";

            Regex regex = new Regex("\\\\[a-zA-Z]+\\\\[a-zA-Z]+[0-9]+\\\\", RegexOptions.IgnoreCase);
            Match match = regex.Match(NTPath);
            if (match.Success)
            {
                string dosPath = NTPath.Replace(match.Value, logicalDisk);
                return dosPath;
            }
            else
            {
                return NTPath;
            }
        }

        /// <summary>
        /// Helper function to call OpenFileDialog and multi-select files
        /// </summary>
        /// <param name="displayTitle"></param>
        /// <param name="browseFileType"></param>
        /// <returns>String list of file paths if paths found and user clicks OK. Null otherwise</returns>
        public static List<string> BrowseForMultiFiles(string displayTitle, BrowseFileType browseFileType)
        {
            List<string> policyPaths = new List<string>();

            // Open file dialog to get file or folder path
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = displayTitle;
            openFileDialog.CheckPathExists = true;

            if (browseFileType == BrowseFileType.Policy)
            {
                openFileDialog.Filter = "WDAC Policy Files (*.xml)|*.xml";
            }
            else if (browseFileType == BrowseFileType.EventLog)
            {
                openFileDialog.Filter = "Event Log Files (*.evtx)|*.evtx";
            }
            else
            {
                openFileDialog.Filter = "All Files (*.)|*.";
            }
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                policyPaths = openFileDialog.FileNames.ToList();
                openFileDialog.Dispose();

                return policyPaths;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Browse for single file path using OpenFileDialog
        /// </summary>
        /// <param name="displayTitle"></param>
        /// <param name="browseFile"></param>
        /// <returns>Path to file if user selects Ok and file exists. String.Empty otherwise</returns>
        public static string BrowseForSingleFile(string displayTitle, BrowseFileType browseFile)
        {
            // Open file dialog to get file or folder path
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = displayTitle;
            openFileDialog.CheckPathExists = true;

            if (browseFile.Equals(BrowseFileType.PEFile))
            {
                openFileDialog.Filter = "Portable Executable Files (*.exe; *.dll; *.rll; *.bin)|*.EXE;*.DLL;*.RLL;*.BIN|" +
                "Script Files (*.ps1, *.bat, *.vbs, *.js)|*.PS1;*.BAT;*.VBS;*.JS|" +
                "System Files (*.sys, *.hxs, *.mui, *.lex, *.mof)|*.SYS;*.HXS;*.MUI;*.LEX;*.MOF|" +
                "All Binary and Script Files (*.exe, ...) |*.EXE;*.DLL;*.RLL;*.BIN;*.PS1;*.BAT;*.VBS;*.JS;*.SYS;*.HXS;*.MUI;*.LEX;*.MOF|" +
                "All files (*.*)|*.*";

                openFileDialog.FilterIndex = 4; // Display All Binary Files by default (everything)
            }
            else if (browseFile.Equals(BrowseFileType.Policy))
            {
                openFileDialog.Filter = "WDAC Policy Files (*.xml)|*.xml";
            }
            else
            {
                openFileDialog.Filter = "All Files (*.)|*.";
            }
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                openFileDialog.Dispose();
            }

            return openFileDialog.FileName;
        }

        /// <summary>
        /// Browse for foler path using the FolderBrowserDialog
        /// </summary>
        /// <param name="displayTitle"></param>
        /// <returns></returns>
        public static string GetFolderPath(string displayTitle)
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            openFolderDialog.Description = displayTitle;

            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                openFolderDialog.Dispose();
                return openFolderDialog.SelectedPath;
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// Open SaveFileDialog for a single file
        /// </summary>
        /// <returns>>Path to file if user selects Ok and file exists. String.Empty otherwise</returns>
        public static string SaveSingleFile(string displayTitle, BrowseFileType browseFile)
        {
            string saveLocationPath = String.Empty;

            // Save dialog box pressed
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = displayTitle;
            saveFileDialog.CheckPathExists = true;

            if (browseFile == BrowseFileType.Policy)
            {
                saveFileDialog.Filter = "Policy Files (*.xml)|*.xml";
            }
            else
            {
                saveFileDialog.Filter = "All Files (*.)|*.";
            }
            saveFileDialog.RestoreDirectory = true;

            saveFileDialog.ShowDialog();
            saveLocationPath = saveFileDialog.FileName;
            saveFileDialog.Dispose();

            return saveLocationPath;
        }

        //
        // Summary:
        //     Scans the input string folderPth and finds the filepath with the greatest _ID. 
        //      
        // Returns:
        //     String with the newest _ID filename. example) policy_44.xml 
        public static string GetUniquePolicyPath(string folderPth)
        {
            string newUniquePath = "";
            int NewestID = -1;
            int Start, End;

            DirectoryInfo dir = new DirectoryInfo(folderPth);

            foreach (var file in dir.GetFiles("*.xml"))
            {
                Start = file.Name.IndexOf("policy_") + 7;
                End = file.Name.IndexOf(".xml");

                // If Start indexof returns -1, 
                if (Start == 6)
                {
                    continue;
                }

                int ID = Convert.ToInt32(file.Name.Substring(Start, End - Start));

                if (ID > NewestID)
                {
                    NewestID = ID;
                }
            }

            if (NewestID < 0)
            {
                newUniquePath = Path.Combine(folderPth, "policy_0.xml"); //first temp policy being created
            }
            else
            {
                newUniquePath = Path.Combine(folderPth, String.Format("policy_{0}.xml", NewestID + 1));
            }

            return newUniquePath;
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

        /// <summary>
        /// Deserialize the xml policy string to SiPolicy
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns>SiPolicy object</returns>
        public static SiPolicy DeserializeXMLStringtoPolicy(string xmlContents)
        {
            SiPolicy siPolicy;

            try
            {
                var stream = new MemoryStream();
                var writer = new StreamWriter(stream);
                writer.Write(xmlContents);
                writer.Flush();
                stream.Position = 0;

                XmlSerializer serializer = new XmlSerializer(typeof(SiPolicy));
                StreamReader reader = new StreamReader(stream);
                siPolicy = (SiPolicy)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception exp)
            {
                return null;
            }

            return siPolicy;
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

        public static bool IsValidText(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            else if(text == Properties.Resources.DefaultFileAttributeString)
            {
                return false; 
            }
            else
            {
                return true; 
            }
        }

        /// <summary>
        /// Format the custom published input from the user
        /// </summary>
        /// <param name="certSubjectName"></param>
        /// <returns></returns>
        public static string FormatSubjectName(string certSubjectName)
        {
            // Remove unwanted info from the subject name (C= onwards)
            int country_idx = certSubjectName.IndexOf("C=");
            if (country_idx > 1)
            {
                int comma_idx = certSubjectName.IndexOf(',', country_idx);
                if (comma_idx > 1)
                {
                    certSubjectName = certSubjectName.Substring(0, comma_idx);
                }
            }

            return certSubjectName;
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

        public static Dictionary<string, string> ParsePSOutput(Collection<PSObject> results)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();

            // Convert results to something parseable
            StringBuilder sBuilder = new StringBuilder();
            foreach (PSObject psObject in results)
            {
                sBuilder.AppendLine(psObject.ToString());
            }

            // Parse the SystemDriver cmdlet output for the scanPath only
            string scriptOutput = sBuilder.ToString();
            var packages = scriptOutput.Split(':');
            int OFFSET = 21;

            try
            {
                foreach (var package in packages)
                {
                    if (package.Contains("\r\nPublisher       "))
                    {
                        string pkgName = package.Substring(1, package.Length - OFFSET);
                        if (!output.ContainsKey(pkgName))
                        {
                            output[pkgName] = "";
                        }
                    }
                }
            }
            catch (Exception exp)
            {

            }

            return output;
        }

        // Dump all of the package family names for the custom rules table
        public static string GetListofPackages(PolicyCustomRules policyCustomRule)
        {
            string output = String.Empty;

            if (policyCustomRule.PackagedFamilyNames == null)
            {
                return String.Empty;
            }
            if (policyCustomRule.PackagedFamilyNames.Count == 0)
            {
                return String.Empty;
            }

            if(policyCustomRule.UsingCustomValues)
            {
                foreach (var package in policyCustomRule.CustomValues.PackageFamilyNames)
                {
                    output += String.Format("{0}, ", package);
                }
            }
            else
            {
                foreach (var package in policyCustomRule.PackagedFamilyNames)
                {
                    output += String.Format("{0}, ", package);
                }
            }

            output = output.Substring(0, output.Length - 2); // Trim off trailing whitespace and comma
            return output;
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
        /// Converts a hash byte[] to hash hex string
        /// </summary>
        /// <param name="hashByte"></param>
        /// <returns></returns>
        public static string ConvertHash(byte[] hashByte)
        {
            if (hashByte == null)
            {
                return string.Empty;
            }

            string hashstring = string.Empty;
            for (int i = 0; i < hashByte.Length; i++)
            {
                hashstring += hashByte[i].ToString("X");
            }

            return hashstring;
        }

        /// <summary>
        /// Takes in an AppLocker hash string value and returns an SiPolicy hash
        /// </summary>
        /// <param name="sHash"></param>
        /// <returns></returns>
        public static byte[] ConvertHashStringToByte(string sHash)
        {
            byte[] bHash = new byte[sHash.Length / 2];
            int _base = 16;
            string sValue;  //chunk into 2's

            for (int i = 0; i < sHash.Length; i += 2)
            {
                sValue = "" + sHash[i] + sHash[i + 1];
                bHash[i / 2] = Convert.ToByte(sValue, _base);
            }

            return bHash;
        }

        /// <summary>
        /// Helper function to get the Windows version (e.g. 1903) to determine whether certain features are supported on this system.
        /// </summary>
        public static int GetWinVersion()
        {
            try
            {
                return Convert.ToInt32(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", ""));
            }
            catch (Exception e)
            {

            }
            return -1;
        }

        /// <summary>
        /// Returns the Temp folder where policy and log writing must occur
        /// </summary>
        /// <returns>Location of the WDACWizard Temp folder location</returns>
        public static string GetTempFolderPath()
        {
            string tempFolder = Path.Combine("WDACWizard", "Temp", GetFormattedDateTime());
            return Path.Combine(Path.GetTempPath(), tempFolder);
        }

        /// <summary>
        /// Returns the Temp folder where policy and log writing must occur
        /// </summary>
        /// <returns>Location of the WDACWizard Temp folder location</returns>
        public static string GetTempFolderPathRoot()
        {
            return Path.Combine(Path.GetTempPath(), "WDACWizard");
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

        /*
         * 
         *  CIPolicy Object Helper Methods
         * 
         */

        // Counts of file rules created to pipe into IDs
        static public int cFileAllowRules = 0;
        static public int cFileDenyRules = 0;
        static public int cFileAttribRules = 0;
        static public int cEKURules= 0;

        /// <summary>
        /// Creates an Allow rule based on a provided hash string
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy CreateAllowHashRule(SiPolicy siPolicy, PolicyCustomRules customRule = null, Allow allow=null)
        {
            if(allow != null)
            {
                Allow allowRule = new Allow();
                allowRule.Hash = allow.Hash;
                allowRule.FriendlyName = allow.FriendlyName;
                allowRule.ID = String.Format("ID_ALLOW_HASH_{0}", cFileAllowRules);
                cFileAllowRules++;

                // Add the deny rule to FileRules and FileRuleRef section with Windows Signing Scenario
                siPolicy = AddAllowRule(allowRule, siPolicy, customRule.SigningScenarioCheckStates);
            }

            if(customRule != null)
            {
                // Iterate through the hashes
                foreach (var hash in customRule.CustomValues.Hashes)
                {
                    Allow allowRule = new Allow();

                    allowRule.Hash = Helper.ConvertHashStringToByte(hash);
                    allowRule.FriendlyName = String.Format("Allow hash: {0}", hash);
                    allowRule.ID = String.Format("ID_ALLOW_HASH_{0}", cFileAllowRules);
                    cFileAllowRules++;

                    // Add the Allow rule to FileRules and FileRuleRef section with Windows Signing Scenario
                    siPolicy = AddAllowRule(allowRule, siPolicy, customRule.SigningScenarioCheckStates);
                }
            }
            
            return siPolicy;
        }

        /// <summary>
        /// Creates a Deny rule based on a provided hash string
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy CreateDenyHashRule(SiPolicy siPolicy, PolicyCustomRules customRule = null, Deny deny=null)
        {
            if(deny != null)
            {
                Deny denyRule = new Deny();
                denyRule.Hash = deny.Hash;
                denyRule.FriendlyName = deny.FriendlyName;
                denyRule.ID = String.Format("ID_DENY_HASH_{0}", cFileDenyRules);
                cFileDenyRules++;

                // Add the deny rule to FileRules and FileRuleRef section with Windows Signing Scenario
                siPolicy = AddDenyRule(denyRule, siPolicy, customRule.SigningScenarioCheckStates);
            }

            if (customRule != null)
            {
                // Iterate through the hashes
                foreach (var hash in customRule.CustomValues.Hashes)
                {
                    Deny denyRule = new Deny();
                    denyRule.Hash = Helper.ConvertHashStringToByte(hash);
                    denyRule.FriendlyName = String.Format("Deny hash: {0}", hash);
                    denyRule.ID = String.Format("ID_DENY_HASH_{0}", cFileDenyRules);
                    cFileDenyRules++;

                    // Add the deny rule to FileRules and FileRuleRef section with Windows Signing Scenario
                    siPolicy = AddDenyRule(denyRule, siPolicy, customRule.SigningScenarioCheckStates);
                }
            }

            return siPolicy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hashSiPolicy"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy CreateHashFallbackRules(SiPolicy hashSiPolicy, SiPolicy siPolicy, PolicyCustomRules customRule)
        {
            foreach(object hashRule in hashSiPolicy.FileRules)
            {
                if(hashRule.GetType() == typeof(Allow))
                {
                    siPolicy = CreateAllowHashRule(siPolicy, customRule, (Allow)hashRule);
                }
                else
                {
                    siPolicy = CreateDenyHashRule(siPolicy, customRule, (Deny)hashRule);
                }
            }

            return siPolicy; 
        }

        /// <summary>
        /// Creates an Allow rule based on a filepath
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy CreateAllowPathRule(PolicyCustomRules customRule, SiPolicy siPolicy, bool isException=false)
        {
            Allow allowRule = new Allow();

            if (Properties.Settings.Default.useEnvVars)
            {
                allowRule.FilePath = Helper.GetEnvPath(customRule.CustomValues.Path);
            }
            else
            {
                allowRule.FilePath = customRule.CustomValues.Path;
            }

            allowRule.FriendlyName = String.Format("Allow by path: {0}", allowRule.FilePath);
            allowRule.ID = String.Format("ID_ALLOW_PATH_{0}", cFileAllowRules);
            cFileAllowRules++;

            // Add the Allow rule to FileRules and FileRuleRef section with Windows Signing Scenario
            siPolicy = AddAllowRule(allowRule, siPolicy, customRule.SigningScenarioCheckStates, isException);
            return siPolicy;
        }

        /// <summary>
        ///  Creates a Deny rule based on a filepath
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy CreateDenyPathRule(PolicyCustomRules customRule, SiPolicy siPolicy)
        {
            Deny denyRule = new Deny();

            if (Properties.Settings.Default.useEnvVars)
            {
                denyRule.FilePath = Helper.GetEnvPath(customRule.CustomValues.Path);
            }
            else
            {
                denyRule.FilePath = customRule.CustomValues.Path;
            }
            denyRule.FriendlyName = String.Format("Deny by path: {0}", denyRule.FilePath);
            denyRule.ID = String.Format("ID_DENY_PATH_{0}", cFileDenyRules);
            cFileDenyRules++;

            // Add the deny rule to FileRules and FileRuleRef section with Windows Signing Scenario
            siPolicy = AddDenyRule(denyRule, siPolicy, customRule.SigningScenarioCheckStates);

            return siPolicy;
        }

        /// <summary>
        /// Handles creating and adding a File Publisher signer rule with user-defined custom values
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy CreateFilePublisherRule(PolicyCustomRules customRule, SiPolicy siPolicy)
        {
            // Still need to run the PS cmd to generate the TBS hash for the signer(s)
            SiPolicy tempSiPolicy = CreateSignerFromPS(customRule);

            if(tempSiPolicy == null)
            {
                return siPolicy;
            }

            Signer[] signers = tempSiPolicy.Signers; 
            if (signers == null || signers.Length == 0)
            {
                // Failed to create signer rules. Fallback to hash rules
                if(tempSiPolicy.FileRules.Length > 0)
                {
                    siPolicy = CreateHashFallbackRules(tempSiPolicy, siPolicy, customRule);
                }

                return siPolicy; 
            }

            // Handle the custom publisher fields on the signer 
            if (customRule.CheckboxCheckStates.checkBox1)
            {
                signers = SetSignersPublishers(signers, customRule);
            }

            // Handle the Custom EKU fields on the signer  
            if (!String.IsNullOrEmpty(customRule.EKUEncoded))
            {
                EKU eku = new EKU();
                eku.ID = "ID_EKU_A_" + cEKURules++;
                eku.FriendlyName = customRule.EKUFriendly;
                eku.Value = Helper.ConvertHashStringToByte(customRule.EKUEncoded);

                signers = SetSignersEKUs(signers, eku);
                siPolicy = AddSiPolicyEKUs(eku, siPolicy); 
            }

            // Create new FileAttrib object to link to signer
            FileAttrib fileAttrib = new FileAttrib();
            fileAttrib.ID = "ID_FILEATTRIB_A_" + cFileAttribRules++;

            // Set the fileattribute fields based on the checkbox states
            if (customRule.CheckboxCheckStates.checkBox4)
            {
                fileAttrib.MinimumFileVersion = customRule.CustomValues.MinVersion.Trim();
            }

            if (customRule.CheckboxCheckStates.checkBox4 && 
                customRule.CustomValues.MaxVersion != null && customRule.CustomValues.MaxVersion != "*")
            {
                fileAttrib.MaximumFileVersion = customRule.CustomValues.MaxVersion.Trim(); 
            }

            if (customRule.CheckboxCheckStates.checkBox3)
            {
                fileAttrib.FileName = customRule.CustomValues.FileName;
            }

            if (customRule.CheckboxCheckStates.checkBox2)
            {
                fileAttrib.ProductName = customRule.CustomValues.ProductName;
            }

            // Add FileAttrib references
            signers = AddFileAttribToSigners(fileAttrib, signers);
            siPolicy = AddSiPolicyFileAttrib(fileAttrib, siPolicy);

            // Add signer references
            siPolicy = AddSiPolicySigner(signers, siPolicy, customRule.Permission, customRule.SigningScenarioCheckStates);

            return siPolicy;            
        }

        /// <summary>
        /// Creates an Allow File Attribute rule based on Original Filename, Description, Product and Internal filename
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy CreateAllowFileAttributeRule(PolicyCustomRules customRule, SiPolicy siPolicy, bool isException=false)
        {
            Allow allowRule = new Allow();
            string friendlyName = "Allow files based on file attributes: "; 

            // Add only the checked attributes
            if (customRule.CheckboxCheckStates.checkBox0)
            {
                allowRule.FileName = customRule.CustomValues.FileName;
                friendlyName += allowRule.FileName + " and "; 
            }
            
            if(customRule.CheckboxCheckStates.checkBox1)
            {
                allowRule.FileDescription = customRule.CustomValues.Description;
                friendlyName += allowRule.FileDescription + " and ";
            }
            
            if(customRule.CheckboxCheckStates.checkBox2)
            {
                allowRule.ProductName = customRule.CustomValues.ProductName;
                friendlyName += allowRule.ProductName + " and ";
            }
            
            if(customRule.CheckboxCheckStates.checkBox3)
            {
                allowRule.InternalName = customRule.CustomValues.InternalName;
                friendlyName += allowRule.InternalName + " and ";
            }

            if(customRule.CheckboxCheckStates.checkBox4 && isException)
            {
                allowRule.MinimumFileVersion = customRule.CustomValues.MinVersion;
                friendlyName += allowRule.MinimumFileVersion + " and ";
            }

            allowRule.FriendlyName = friendlyName.Substring(0, friendlyName.Length - 5); 
            allowRule.ID = String.Format("ID_ALLOW_A_{0}", cFileAllowRules++);

            // Add the Allow rule to FileRules and FileRuleRef section with Windows Signing Scenario
            siPolicy = AddAllowRule(allowRule, siPolicy, customRule.SigningScenarioCheckStates, isException);
                        
            return siPolicy;
        }

        /// <summary>
        /// Creates a Deny File Attribute rule based on Original Filename, Description, Product and Internal filename
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy CreateDenyFileAttributeRule(PolicyCustomRules customRule, SiPolicy siPolicy)
        {
            Deny denyRule = new Deny();
            string friendlyName = "Allow files based on file attributes: ";

            // Add only the checked attributes
            if (customRule.CheckboxCheckStates.checkBox0)
            {
                denyRule.FileName = customRule.CustomValues.FileName;
                friendlyName += denyRule.FileName + " and ";
            }

            if (customRule.CheckboxCheckStates.checkBox1)
            {
                denyRule.FileDescription = customRule.CustomValues.Description;
                friendlyName += denyRule.FileDescription + " and ";
            }

            if (customRule.CheckboxCheckStates.checkBox2)
            {
                denyRule.ProductName = customRule.CustomValues.ProductName;
                friendlyName += denyRule.ProductName + " and ";
            }

            if (customRule.CheckboxCheckStates.checkBox3)
            {
                denyRule.InternalName = customRule.CustomValues.InternalName;
                friendlyName += denyRule.InternalName + " and ";
            }


            denyRule.FriendlyName = friendlyName.Substring(0, friendlyName.Length - 5);
            denyRule.ID = String.Format("ID_DENY_A_{0}", cFileDenyRules++);

            // Add the deny rule to FileRules and FileRuleRef section with Windows Signing Scenario
            siPolicy = AddDenyRule(denyRule, siPolicy, customRule.SigningScenarioCheckStates);

            return siPolicy;
        }

        /// <summary>
        /// Creates a File Attribute rule from PE header defined data
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy CreateNonCustomFileAttributeRule(PolicyCustomRules customRule, SiPolicy siPolicy)
        {
            // Format like a custom rule and pass into the custom rule handler methods
            customRule.CustomValues.FileName = customRule.FileInfo["OriginalFilename"];
            customRule.CustomValues.Description = customRule.FileInfo["FileDescription"];
            customRule.CustomValues.ProductName = customRule.FileInfo["ProductName"];
            customRule.CustomValues.InternalName = customRule.FileInfo["InternalName"];

            if(customRule.Permission == PolicyCustomRules.RulePermission.Allow)
            {
                return CreateAllowFileAttributeRule(customRule, siPolicy); 
            }
            else
            {
                return CreateDenyFileAttributeRule(customRule, siPolicy);
            }
        }

        /// <summary>
        /// Creates a File Attribute rule from the OpenFile/OpenFolder returned path defined data
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy CreateNonCustomFilePathRule(PolicyCustomRules customRule, SiPolicy siPolicy)
        {
            // Format like a custom rule and pass into the custom rule handler methods
            customRule.CustomValues.Path = customRule.ReferenceFile; 

            if (customRule.Permission == PolicyCustomRules.RulePermission.Allow)
            {
                return CreateAllowPathRule(customRule, siPolicy);
            }
            else
            {
                return CreateDenyPathRule(customRule, siPolicy);
            }
        }

        /// <summary>
        /// Handles the creation of PFN rules and addition to the SiPolicy object
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy CreatePFNRule(PolicyCustomRules customRule, SiPolicy siPolicy)
        {
            List<string> pfnNames = new List<string>();

            // Custom Value rules
            if (customRule.CustomValues.PackageFamilyNames.Count > 0)
            {
                pfnNames = customRule.CustomValues.PackageFamilyNames;
            }
            else
            {
                // System generated PFN names
                pfnNames = customRule.PackagedFamilyNames;
            }

            // Iterate through the PFNs supplied and create a PFN rule per PFN
            if (customRule.Permission == PolicyCustomRules.RulePermission.Allow)
            {
                foreach (var pfn in pfnNames)
                {
                    Allow allowRule = new Allow();
                    allowRule.PackageFamilyName = pfn;
                    allowRule.MinimumFileVersion = Properties.Resources.DefaultPFNVersion;
                    allowRule.FriendlyName = String.Format("Allow packaged app by Package Family Name (PFN): {0}", pfn);
                    allowRule.ID = String.Format("ID_ALLOW_PFN_{0}", cFileAllowRules);
                    cFileAllowRules++;

                    // Add the Allow rule to FileRules and FileRuleRef section with Windows Signing Scenario
                    siPolicy = AddAllowRule(allowRule, siPolicy, customRule.SigningScenarioCheckStates);
                }
            }
            else
            {
                foreach (var pfn in pfnNames)
                {
                    Deny denyRule = new Deny();
                    denyRule.PackageFamilyName = pfn;
                    denyRule.MinimumFileVersion = Properties.Resources.DefaultPFNVersion;
                    denyRule.FriendlyName = String.Format("Deny packaged app by Package Family Name (PFN): {0}", pfn);
                    denyRule.ID = String.Format("ID_DENY_PFN_{0}", cFileDenyRules);
                    cFileDenyRules++;

                    // Add the Allow rule to FileRules and FileRuleRef section with Windows Signing Scenario
                    siPolicy = AddDenyRule(denyRule, siPolicy, customRule.SigningScenarioCheckStates);
                }
            }
            return siPolicy; 
        }

        /// <summary>
        /// Creates a COM object instance and adds to the provided WDAC SiPolicy object
        /// </summary>
        /// <param name="siPolicy">SiPolicy object to manipulate and add the COM objects to</param>
        /// <param name="comObjectList">List of COM object instances to parse and add to siPolicy</param>
        /// <returns></returns>
        public static SiPolicy CreateComRule(SiPolicy siPolicy, List<COM> comObjectList)
        {
            if(comObjectList == null || comObjectList.Count == 0)
            {
                return siPolicy; 
            }

            foreach(COM customComObj in comObjectList)
            {
                Setting comObject = new Setting();
                comObject.Key = customComObj.Guid;
                comObject.Provider = customComObj.Provider.ToString();
                comObject.ValueName = customComObj.ValueName;
                comObject.Value = new SettingValueType();
                comObject.Value.Item = customComObj.ValueItem;

                siPolicy = AddComRule(siPolicy, comObject);
            }

            return siPolicy; 
        }

        /// <summary>
        /// Adds a COM object as a SiPolicy Setting. Returns SiPolicy with the Setting. 
        /// </summary>
        /// <param name="siPolicy">SiPolicy object to add the COM object Setting to</param>
        /// <param name="comObject">COM object SiPolicy Setting which is added to the SiPolicy object</param>
        /// <returns></returns>
        private static SiPolicy AddComRule(SiPolicy siPolicy, Setting comObject)
        {
            if(siPolicy.Settings != null)
            {
                Setting[] settingsCopy = siPolicy.Settings;
                Array.Resize(ref settingsCopy, settingsCopy.Length + 1);
                settingsCopy[settingsCopy.Length - 1] = comObject;

                siPolicy.Settings = settingsCopy;
            }
            else
            {
                siPolicy.Settings = new Setting[1];
                siPolicy.Settings[0] = comObject; 
            }
            
            return siPolicy; 
        }

        /// <summary>
        /// Handles adding the new Allow Rule object to the provided siPolicy
        /// </summary>
        /// <param name="allowRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        private static SiPolicy AddAllowRule(Allow allowRule, SiPolicy siPolicy, PolicyCustomRules.SigningScenarioStates scenarioStates, bool isException = false)
        {
            // Copy and replace the FileRules obj[] in siPolicy
            // FileRules always initalized - no need to check if null
            object[] fileRulesCopy = siPolicy.FileRules;
            Array.Resize(ref fileRulesCopy, fileRulesCopy.Length + 1);
            fileRulesCopy[fileRulesCopy.Length - 1] = allowRule;
            siPolicy.FileRules = fileRulesCopy;

            // Add the filerule reference
            siPolicy = AddFileRulesRef(allowRule.ID, siPolicy, scenarioStates, isException);

            return siPolicy;
        }

        /// <summary>
        /// Handles adding the new Allow Rule object to the provided siPolicy
        /// </summary>
        /// <param name="allowRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        private static SiPolicy AddDenyRule(Deny denyRule, SiPolicy siPolicy, PolicyCustomRules.SigningScenarioStates scenarioStates, bool isException = false)
        {
            // Copy and replace the FileRules obj[] in siPolicy
            // FileRules always initalized - no need to check if null
            object[] fileRulesCopy = siPolicy.FileRules;
            Array.Resize(ref fileRulesCopy, fileRulesCopy.Length + 1);
            fileRulesCopy[fileRulesCopy.Length - 1] = denyRule;
            siPolicy.FileRules = fileRulesCopy;

            // Add the filerule reference
            siPolicy = AddFileRulesRef(denyRule.ID, siPolicy, scenarioStates, isException);

            return siPolicy;
        }

        /// <summary>
        /// Adds an SiPolicy FileAttrib to the policy
        /// </summary>
        /// <param name="fileAttrib"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        private static SiPolicy AddSiPolicyFileAttrib(FileAttrib fileAttrib, SiPolicy siPolicy)
        {
            // Copy and replace FileRules section in SiPolicy
            // FileRules always initalized - no need to check if null
            object[] fileRulesCopy = siPolicy.FileRules;
            Array.Resize(ref fileRulesCopy, fileRulesCopy.Length + 1);
            fileRulesCopy[fileRulesCopy.Length - 1] = fileAttrib;
            siPolicy.FileRules = fileRulesCopy;

            return siPolicy;
        }

        /// <summary>
        /// Adds an EKU to the siPolicy object
        /// </summary>
        /// <param name="eku"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        private static SiPolicy AddSiPolicyEKUs(EKU eku, SiPolicy siPolicy)
        {
            EKU[] ekuCopy = siPolicy.EKUs;
            Array.Resize(ref ekuCopy, ekuCopy.Length + 1);
            ekuCopy[ekuCopy.Length - 1] = eku;
            siPolicy.EKUs = ekuCopy;
            
            return siPolicy; 
        }

        /// <summary>
        /// Adds the FileAttrib object to the signers[] object
        /// </summary>
        /// <param name="fileAttrib"></param>
        /// <param name="signers"></param>
        /// <param name="customRule"></param>
        /// <returns></returns>
        private static Signer[] AddFileAttribToSigners(FileAttrib fileAttrib, Signer[] signers)
        {
            for (int i = 0; i < signers.Length; i++)
            {
                // Link the new FileAttrib object back to the signer
                FileAttribRef fileAttribRef = new FileAttribRef();
                fileAttribRef.RuleID = fileAttrib.ID;

                signers[i].FileAttribRef = new FileAttribRef[1];
                signers[i].FileAttribRef[0] = fileAttribRef; 
            }

            return signers; 
        }

        /// <summary>
        /// Sets the Publisher field on the Signers object
        /// </summary>
        /// <param name="signers"></param>
        /// <param name="customRule"></param>
        /// <returns></returns>
        private static Signer[] SetSignersPublishers(Signer[] signers, PolicyCustomRules customRule)
        {
            for (int i = 0; i < signers.Length; i++)
            {
                // Create new CertPublisher object and add CertPublisher field
                CertPublisher cPub = new CertPublisher();
                cPub.Value = customRule.CustomValues.Publisher;
                signers[i].CertPublisher = cPub; 
            }

            return signers;
        }


        /// <summary>
        /// Sets the EKU field on the Signers object
        /// </summary>
        /// <param name="signers"></param>
        /// <param name="customRule"></param>
        /// <returns></returns>
        private static Signer[] SetSignersEKUs(Signer[] signers, EKU eku)
        {
            for (int i = 0; i < signers.Length; i++)
            {
                // Create new CertEKU[]
                // TODO support >1 EKUS in the future
                CertEKU[] certEKUs = new CertEKU[1];
                certEKUs[0] = new CertEKU();
                certEKUs[0].ID = eku.ID; 
                signers[i].CertEKU = certEKUs; 
            }

            return signers;
        }

        /// <summary>
        /// Creates a new FileRuleRef and adds the rule ID
        /// </summary>
        /// <param name="ruleID"></param>
        /// <param name="siPolicy"></param>
        /// <param name="isException"></param>
        /// <returns></returns>
        private static SiPolicy AddFileRulesRef(string ruleID, SiPolicy siPolicy, PolicyCustomRules.SigningScenarioStates scenarioStates, bool isException = false)
        {
            // Copy and replace the FileRulesRef section to add to Signing Scenarios
            // If this is an exception, don't add to FileRulesRef section
            if (!isException)
            {
                FileRulesRef refCopy = new FileRulesRef();

                for (int i = 0; i < siPolicy.SigningScenarios.Length; i++)
                {
                    // Kernel mode (131)
                    if (siPolicy.SigningScenarios[i].Value == KMCISCN && scenarioStates.kmciEnabled)
                    {

                        if (siPolicy.SigningScenarios[i].ProductSigners.FileRulesRef == null)
                        {
                            refCopy.FileRuleRef = new FileRuleRef[1];
                            refCopy.FileRuleRef[0] = new FileRuleRef();
                            refCopy.FileRuleRef[0].RuleID = ruleID;
                        }
                        else
                        {
                            refCopy.FileRuleRef = new FileRuleRef[siPolicy.SigningScenarios[i].ProductSigners.FileRulesRef.FileRuleRef.Length + 1];
                            for (int j = 0; j < refCopy.FileRuleRef.Length - 1; j++)
                            {
                                refCopy.FileRuleRef[j] = siPolicy.SigningScenarios[i].ProductSigners.FileRulesRef.FileRuleRef[j];
                            }

                            refCopy.FileRuleRef[refCopy.FileRuleRef.Length - 1] = new FileRuleRef();
                            refCopy.FileRuleRef[refCopy.FileRuleRef.Length - 1].RuleID = ruleID;
                        }

                        siPolicy.SigningScenarios[i].ProductSigners.FileRulesRef = refCopy;
                    }

                    // User mode (12)
                    if (siPolicy.SigningScenarios[i].Value == UMCISCN && scenarioStates.umciEnabled)
                    {
                        if (siPolicy.SigningScenarios[i].ProductSigners.FileRulesRef == null)
                        {
                            refCopy.FileRuleRef = new FileRuleRef[1];
                            refCopy.FileRuleRef[0] = new FileRuleRef();
                            refCopy.FileRuleRef[0].RuleID = ruleID;
                        }
                        else
                        {
                            refCopy.FileRuleRef = new FileRuleRef[siPolicy.SigningScenarios[i].ProductSigners.FileRulesRef.FileRuleRef.Length + 1];
                            for (int j = 0; j < refCopy.FileRuleRef.Length - 1; j++)
                            {
                                refCopy.FileRuleRef[j] = siPolicy.SigningScenarios[i].ProductSigners.FileRulesRef.FileRuleRef[j];
                            }

                            refCopy.FileRuleRef[refCopy.FileRuleRef.Length - 1] = new FileRuleRef();
                            refCopy.FileRuleRef[refCopy.FileRuleRef.Length - 1].RuleID = ruleID;
                        }

                        siPolicy.SigningScenarios[i].ProductSigners.FileRulesRef = refCopy;
                    }
                }
            }

            return siPolicy;
        }

        /// <summary>
        /// Adds an Allow Signer or Deny Signer to the siPolicy signing scenario
        /// </summary>
        /// <param name="signers"></param>
        /// <param name="siPolicy"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private static SiPolicy AddSiPolicySigner(Signer[] signers, SiPolicy siPolicy, PolicyCustomRules.RulePermission action, 
            PolicyCustomRules.SigningScenarioStates scenarioStates)
        {
            // Copy the SiPolicy signer object and add the signer param to the field
            Signer[] signersCopy = siPolicy.Signers;
            Array.Resize(ref signersCopy, signersCopy.Length + signers.Length); 
            for (int i=0; i < signers.Length; i++)
            {
                signersCopy[siPolicy.Signers.Length + i] = signers[i];
            }

            siPolicy.Signers = signersCopy;

            if (action == PolicyCustomRules.RulePermission.Allow)
            {
                // Create an AllowedSigner object to add to the SiPolicy ProductSigners section
                for (int i = 0; i < signers.Length; i++)
                {
                    AllowedSigner allowedSigner = new AllowedSigner();
                    allowedSigner.SignerId = signers[i].ID;                    

                    // Iterate through all SigningScenarios
                    for(int j = 0; j < siPolicy.SigningScenarios.Length; j++)
                    {
                        if(siPolicy.SigningScenarios[j].Value == KMCISCN && scenarioStates.kmciEnabled) // Kernel mode (131)
                        {
                            // Copy and replace
                            if (siPolicy.SigningScenarios[j].ProductSigners.AllowedSigners == null)
                            {
                                siPolicy.SigningScenarios[j].ProductSigners.AllowedSigners = new AllowedSigners();
                                siPolicy.SigningScenarios[j].ProductSigners.AllowedSigners.AllowedSigner = new AllowedSigner[1];
                                siPolicy.SigningScenarios[j].ProductSigners.AllowedSigners.AllowedSigner[0] = allowedSigner;
                            }
                            else
                            {
                                AllowedSigner[] allowedSignersCopy = siPolicy.SigningScenarios[j].ProductSigners.AllowedSigners.AllowedSigner;
                                Array.Resize(ref allowedSignersCopy, allowedSignersCopy.Length + 1);
                                allowedSignersCopy[allowedSignersCopy.Length - 1] = allowedSigner;

                                siPolicy.SigningScenarios[j].ProductSigners.AllowedSigners.AllowedSigner = allowedSignersCopy;
                            }
                        }

                        if (siPolicy.SigningScenarios[j].Value == UMCISCN && scenarioStates.umciEnabled) // User mode (12)
                        {
                            // Copy and replace
                            if (siPolicy.SigningScenarios[j].ProductSigners.AllowedSigners == null)
                            {
                                siPolicy.SigningScenarios[j].ProductSigners.AllowedSigners = new AllowedSigners();
                                siPolicy.SigningScenarios[j].ProductSigners.AllowedSigners.AllowedSigner = new AllowedSigner[1];
                                siPolicy.SigningScenarios[j].ProductSigners.AllowedSigners.AllowedSigner[0] = allowedSigner;
                            }
                            else
                            {
                                AllowedSigner[] allowedSignersCopy = siPolicy.SigningScenarios[j].ProductSigners.AllowedSigners.AllowedSigner;
                                Array.Resize(ref allowedSignersCopy, allowedSignersCopy.Length + 1);
                                allowedSignersCopy[allowedSignersCopy.Length - 1] = allowedSigner;

                                siPolicy.SigningScenarios[j].ProductSigners.AllowedSigners.AllowedSigner = allowedSignersCopy;
                            }
                        }
                    }
                }
            }
            else
            {
                // Create a DeniedSigner object to add to the SiPolicy ProductSigners section
                for (int i = 0; i < signers.Length; i++)
                {
                    DeniedSigner deniedSigner = new DeniedSigner();
                    deniedSigner.SignerId = signers[i].ID;

                    // Iterate through all SigningScenarios
                    for (int j = 0; j < siPolicy.SigningScenarios.Length; j++)
                    {
                        if (siPolicy.SigningScenarios[j].Value == KMCISCN && scenarioStates.kmciEnabled) // Kernel mode (131)
                        {
                            // Copy and replace
                            if (siPolicy.SigningScenarios[j].ProductSigners.DeniedSigners == null)
                            {
                                siPolicy.SigningScenarios[j].ProductSigners.DeniedSigners = new DeniedSigners();
                                siPolicy.SigningScenarios[j].ProductSigners.DeniedSigners.DeniedSigner = new DeniedSigner[1];
                                siPolicy.SigningScenarios[j].ProductSigners.DeniedSigners.DeniedSigner[0] = deniedSigner;
                            }
                            else
                            {
                                DeniedSigner[] deniedSignersCopy = siPolicy.SigningScenarios[j].ProductSigners.DeniedSigners.DeniedSigner;
                                Array.Resize(ref deniedSignersCopy, deniedSignersCopy.Length + 1);
                                deniedSignersCopy[deniedSignersCopy.Length - 1] = deniedSigner;

                                siPolicy.SigningScenarios[j].ProductSigners.DeniedSigners.DeniedSigner = deniedSignersCopy;
                            }
                        }

                        if (siPolicy.SigningScenarios[j].Value == UMCISCN && scenarioStates.umciEnabled) // User mode (12)
                        {
                            // Copy and replace
                            if (siPolicy.SigningScenarios[j].ProductSigners.DeniedSigners == null)
                            {
                                siPolicy.SigningScenarios[j].ProductSigners.DeniedSigners = new DeniedSigners();
                                siPolicy.SigningScenarios[j].ProductSigners.DeniedSigners.DeniedSigner = new DeniedSigner[1];
                                siPolicy.SigningScenarios[j].ProductSigners.DeniedSigners.DeniedSigner[0] = deniedSigner;
                            }
                            else
                            {
                                DeniedSigner[] deniedSignersCopy = siPolicy.SigningScenarios[j].ProductSigners.DeniedSigners.DeniedSigner;
                                Array.Resize(ref deniedSignersCopy, deniedSignersCopy.Length + 1);
                                deniedSignersCopy[deniedSignersCopy.Length - 1] = deniedSigner;

                                siPolicy.SigningScenarios[j].ProductSigners.DeniedSigners.DeniedSigner = deniedSignersCopy;
                            }
                        }
                    }
                }
            }

            return siPolicy; 
        }

        /// <summary>
        /// Creates exception rules of type ALLOW
        /// </summary>
        /// <param name="exceptionsList"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        static private ExceptAllowRule[] CreateExceptAllowRules(List<PolicyCustomRules> exceptionsList, SiPolicy siPolicy)
        {
            ExceptAllowRule[] exceptAllowRules = new ExceptAllowRule[exceptionsList.Count];
            int i = 0; 

            foreach(var exceptAllowRule in exceptionsList)
            {
                if(exceptAllowRule.Type == PolicyCustomRules.RuleType.FileAttributes)
                {
                    siPolicy = CreateAllowFileAttributeRule(exceptAllowRule, siPolicy, true);
                }
                else if(exceptAllowRule.Type == PolicyCustomRules.RuleType.FilePath || exceptAllowRule.Type == PolicyCustomRules.RuleType.FolderPath)
                {
                    exceptAllowRules[i++].AllowRuleID = String.Format("ID_ALLOW_PATH_{0}", cFileAllowRules);
                    siPolicy = CreateAllowPathRule(exceptAllowRule, siPolicy, true);                    
                }
                else
                {
                    // Hash rule
                    // Create
                    // CreateAllowHashRule(siPolicy, null, allow)
                    // Add
                }
            }

            return exceptAllowRules; 
        }

        /// <summary>
        /// Creates exception rules of type DENY
        /// </summary>
        /// <param name="exceptionsList"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        static private ExceptDenyRule[] CreateExceptDenyRules(List<PolicyCustomRules> exceptionsList, SiPolicy siPolicy)
        {
            ExceptDenyRule[] exceptDenyRules = new ExceptDenyRule[exceptionsList.Count];
            int i = 0;

            foreach (var exceptAllowRule in exceptionsList)
            {
                if (exceptAllowRule.Type == PolicyCustomRules.RuleType.FileAttributes)
                {
                    siPolicy = CreateAllowFileAttributeRule(exceptAllowRule, siPolicy, true);
                }
                else if (exceptAllowRule.Type == PolicyCustomRules.RuleType.FilePath || exceptAllowRule.Type == PolicyCustomRules.RuleType.FolderPath)
                {
                    exceptDenyRules[i++].DenyRuleID = String.Format("ID_ALLOW_PATH_{0}", cFileAllowRules);
                    siPolicy = CreateAllowPathRule(exceptAllowRule, siPolicy, true);
                }
                else
                {
                    // Hash rule
                    // Create
                    // CreateAllowHashRule(siPolicy, null, allow)
                    // Add
                }
            }

            return exceptDenyRules;
        }

        /// <summary>
        /// Adds the exceptions list to the policy's allowed signers
        /// </summary>
        /// <param name="exceptDenyRules"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        static private SiPolicy AddExceptionsToAllowSigners(ExceptDenyRule[] exceptDenyRules, SiPolicy siPolicy)
        {
            // Add ExceptDenyRule IDs to signing scenarios
            for(int i = 0; i <= siPolicy.SigningScenarios.Length; i++)
            {
                for (int j = 0; j <= siPolicy.SigningScenarios[i].ProductSigners.AllowedSigners.AllowedSigner.Length; j++)
                {
                    siPolicy.SigningScenarios[i].ProductSigners.AllowedSigners.AllowedSigner[j].ExceptDenyRule = exceptDenyRules;
                }
            }

            return siPolicy; 
        }

        /// <summary>
        /// Adds exception list to the policy's denied signers
        /// </summary>
        /// <param name="exceptAllowRules"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        static private SiPolicy AddExceptionsToDeniedSigners(ExceptAllowRule[] exceptAllowRules, SiPolicy siPolicy)
        {
            // Add ExceptAllowRule IDs to signing scenarios
            for (int i = 0; i <= siPolicy.SigningScenarios.Length; i++)
            {
                for (int j = 0; j <= siPolicy.SigningScenarios[i].ProductSigners.DeniedSigners.DeniedSigner.Length; j++)
                {
                    siPolicy.SigningScenarios[i].ProductSigners.DeniedSigners.DeniedSigner[j].ExceptAllowRule = exceptAllowRules;
                }
            }

            return siPolicy;
        }

        /// <summary>
        /// Creates a dummy signer rule and policy to calculate the TBS hash for custom value signer rules
        /// </summary>
        /// <param name="customRule"></param>
        /// <returns></returns>
        static private SiPolicy CreateSignerFromPS(PolicyCustomRules customRule)
        {
            string DUMMYPATH = Path.Combine(GetTempFolderPathRoot(), "DummySignersPolicy.xml");

            // Create runspace, pipeline and run script
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();

            // Scan the file to extract the TBS hash (or hashes) for the signers
            pipeline.Commands.AddScript(String.Format("$DummyPcaRule += New-CIPolicyRule -Level PcaCertificate -DriverFilePath \"{0}\" -Fallback Hash", customRule.ReferenceFile));
            pipeline.Commands.AddScript(String.Format("New-CIPolicy -Rules $DummyPcaRule -FilePath \"{0}\"", DUMMYPATH));

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception e)
            {
                return null; 
            }
            runspace.Dispose(); 

            // De-serialize the dummy policy to get the signer objects
            SiPolicy siPolicy = DeserializeXMLtoPolicy(DUMMYPATH);

            // Remove dummy file
            File.Delete(DUMMYPATH);

            return siPolicy; 
        }

        /// <summary>
        /// Creates a WDAC policy for a signer rule to be used in signer rules
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="policyPath"></param>
        /// <returns></returns>
        public static SiPolicy CreateSignerPolicyFromPS(PolicyCustomRules customRule, string policyPath)
        {
            // Create runspace, pipeline and run script
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();

            // Scan the file to extract the TBS hash (or hashes for fallback) and, optionally, the CN for the signer rules
            string newPolicyRuleCmd = string.Empty; 
            if(customRule.CheckboxCheckStates.checkBox1)
            {
                newPolicyRuleCmd = String.Format("$DummySignerRule = New-CIPolicyRule -Level Publisher -DriverFilePath \"{0}\" -Fallback Hash", customRule.ReferenceFile);
            }
            else
            {
                newPolicyRuleCmd = String.Format("$DummySignerRule = New-CIPolicyRule -Level PcaCertificate -DriverFilePath \"{0}\" -Fallback Hash", customRule.ReferenceFile);
            }

            if(customRule.Permission == PolicyCustomRules.RulePermission.Deny)
            {
                newPolicyRuleCmd += " -Deny"; 
            }

            pipeline.Commands.AddScript(newPolicyRuleCmd);
            pipeline.Commands.AddScript(String.Format("New-CIPolicy -Rules $DummySignerRule -FilePath \"{0}\"", policyPath));

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception e)
            {
                return null;
            }

            runspace.Dispose();

            // De-serialize the dummy policy to get the signer objects
            SiPolicy siPolicy = DeserializeXMLtoPolicy(policyPath);
            return siPolicy;
        }

        /// <summary>
        /// Tries to add attributes like filename, publisher and version to non-custom publisher rules
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="signerSiPolicy"></param>
        public static SiPolicy AddSignerRuleAttributes(PolicyCustomRules customRule, SiPolicy signerSiPolicy)
        {
            // Get signers and check if Wizard fell back to hash rules
            Signer[] signers = signerSiPolicy.Signers;
            if (signers == null || signers.Length == 0)
            {
                // Failed to create signer rules and fellback to hash rules. There are no signers to which to add file attributes
                return signerSiPolicy;
            }

            // Serialize a new policy so signing scenarios are not pre-filled resulting in duplicate signer references
            SiPolicy siPolicy = Helper.DeserializeXMLStringtoPolicy(Properties.Resources.EmptyWDAC);

            // If none of the extra attributes are to be added, skip creating a FileAttrib rule
            if (!(customRule.CheckboxCheckStates.checkBox2 || customRule.CheckboxCheckStates.checkBox3 || customRule.CheckboxCheckStates.checkBox4))
            {
                // Add signer references
                siPolicy = AddSiPolicySigner(signers, siPolicy, customRule.Permission, customRule.SigningScenarioCheckStates);
                return siPolicy;
            }

            // Create new FileAttrib object to link to signers
            FileAttrib fileAttrib = new FileAttrib();
            fileAttrib.ID = "ID_FILEATTRIB_A_" + cFileAttribRules++;
            
            string friendlyName = customRule.Permission.ToString() + " files based on file attributes: ";

            // Set the fileattribute fields based on the checkbox states
            // Version
            if (customRule.CheckboxCheckStates.checkBox4)
            {
                fileAttrib.MinimumFileVersion = customRule.FileInfo["FileVersion"];
                friendlyName += fileAttrib.MinimumFileVersion + " and ";
            }

            // Original Filename
            if (customRule.CheckboxCheckStates.checkBox3)
            {
                fileAttrib.FileName = customRule.FileInfo["OriginalFilename"];
                friendlyName += fileAttrib.FileName + " and ";
            }

            // Product name
            if (customRule.CheckboxCheckStates.checkBox2)
            {
                fileAttrib.ProductName = customRule.FileInfo["ProductName"];
                friendlyName += fileAttrib.ProductName + " and ";
            }

            fileAttrib.FriendlyName = friendlyName.Substring(0, friendlyName.Length - 5); // remove trailing " and "

            // Handle the Custom EKU fields on the signer  
            if (!String.IsNullOrEmpty(customRule.EKUEncoded))
            {
                EKU eku = new EKU();
                eku.ID = "ID_EKU_A_" + cEKURules++;
                eku.FriendlyName = customRule.EKUFriendly;
                eku.Value = Helper.ConvertHashStringToByte(customRule.EKUEncoded);

                signers = SetSignersEKUs(signers, eku);
                signerSiPolicy = AddSiPolicyEKUs(eku, signerSiPolicy);
            }


            // Add FileAttrib references
            signers = AddFileAttribToSigners(fileAttrib, signers);
            siPolicy = AddSiPolicyFileAttrib(fileAttrib, siPolicy);

            // Add signer references
            siPolicy = AddSiPolicySigner(signers, siPolicy, customRule.Permission, customRule.SigningScenarioCheckStates);

            // Add CiSigner - Github Issue #161
            // Usermode rule you are creating a rule for, you need to add signer to cisigners.
            // Kernel mode rule, don't add signer to cisigners
            // If you don't know always add to cisigners.
            if (customRule.SigningScenarioCheckStates.umciEnabled)
            {
                siPolicy = AddSiPolicyCiSigner(signers, siPolicy);
            }

            // Process exceptions
            if (customRule.ExceptionList.Count > 0)
            {
                if(customRule.Permission == PolicyCustomRules.RulePermission.Allow)
                {
                    // Create except deny rules to add to allowed signers
                    ExceptDenyRule[] exceptDenyRules = CreateExceptDenyRules(customRule.ExceptionList, siPolicy);
                    siPolicy = AddExceptionsToAllowSigners(exceptDenyRules, siPolicy);
                }
                else
                {
                    // Create except Allowrules
                    ExceptAllowRule[] exceptAllowRules = CreateExceptAllowRules(customRule.ExceptionList, siPolicy);
                    siPolicy = AddExceptionsToDeniedSigners(exceptAllowRules, siPolicy);
                }
            }

            return siPolicy;
        }

        /// <summary>
        /// Returns a new SiPolicy Setting array with custom Policy Name and Policy ID
        /// </summary>
        /// <param name="siPolicy"></param>
        /// <param name="policyName"></param>
        /// <param name="policyId"></param>
        /// <returns></returns>
        public static Setting[] SetPolicyInfo(Setting[] existingSettings, string policyName, string policyId)
        {
            // If the policy does not have a Settings element, trivial addition case
            if (existingSettings == null)
            {
                Setting[] newSettings = new Setting[2];
                newSettings[0] = CreatePolicyNameSetting(policyName);
                newSettings[1] = CreatePolicyIdSetting(policyId);
                return newSettings; 
            }
            // If there are Policy Id and/or Policy Name and/or COM objects, prepend new Policy ID and Name
            else
            {
                List<Setting> settingList = new List<Setting>();
                settingList.Add(CreatePolicyNameSetting(policyName));
                settingList.Add(CreatePolicyIdSetting(policyId));

                // Also append all other non-PolicyInfo provider settings i.e. COM objects
                foreach(var setting in existingSettings)
                {
                    if(!(setting.Provider == "PolicyInfo" 
                        && (setting.ValueName == "Name" || setting.ValueName == "Id"))) 
                    {
                        settingList.Add(setting); 
                    }
                }

                return settingList.ToArray(); 
            }
        }

        /// Adds the signer ID to the CiSigners Section for user mode rules so that enterprise signers can be passed
        /// </summary>
        /// <param name="signers"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy AddSiPolicyCiSigner(Signer[] signers, SiPolicy siPolicy)
        {
            // Populate the ciSignerIds from the signers array
            CiSigner[] newCiSigners = new CiSigner[signers.Length]; 

            for(int i= 0; i< signers.Length; i++)
            {
                newCiSigners[i] = new CiSigner();
                newCiSigners[i].SignerId = signers[i].ID; 
            }

            // Copy the SiPolicy signer object and add the array of CI Signer IDs
            CiSigner[] cisignersCopy = siPolicy.CiSigners;
            Array.Resize(ref cisignersCopy, cisignersCopy.Length + newCiSigners.Length);
            for (int i = 0; i < signers.Length; i++)
            {
                cisignersCopy[siPolicy.CiSigners.Length + i] = newCiSigners[i];
            }

            siPolicy.CiSigners = cisignersCopy;
            return siPolicy; 
        }

        public static Setting CreatePolicyNameSetting(string policyName)
        {
            Setting settingName = new Setting();
            settingName.Provider = "PolicyInfo";
            settingName.Key = "Information";
            settingName.ValueName = "Name";
            settingName.Value = new SettingValueType();
            settingName.Value.Item = String.IsNullOrEmpty(policyName) ? String.Empty : policyName;
            return settingName; 
        }

        public static Setting CreatePolicyIdSetting(string policyId)
        {
            Setting settingID = new Setting();
            settingID.Provider = "PolicyInfo";
            settingID.Key = "Information";
            settingID.ValueName = "Id";
            settingID.Value = new SettingValueType();
            settingID.Value.Item = String.IsNullOrEmpty(policyId) ? String.Empty : policyId;
            return settingID;
        }

        /// <summary>
        /// Checks the SiPolicy to see if a particular OptionType is set
        /// </summary>
        /// <param name="siPolicy"></param>
        /// <param name="optionType"></param>
        /// <returns></returns>
        public static bool PolicyHasRule(List<RuleType> siPolicyRuleOptions, OptionType targetRuleOption)
        {
            if(siPolicyRuleOptions == null)
            {
                return false; 
            }

            // Check each rule option for the target rule option
            foreach(var ruleOption in siPolicyRuleOptions)
            {
                if(ruleOption.Item == targetRuleOption)
                {
                    return true; 
                }
            }

            return false;
        }

        // End of SiPolicy Helper methods

    } // End of static class Helper

    public class WDACSigner
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string CertOemID { get; set; }
        public string CommonName { get; set; }
        public List<string> FileAttribRefs { get; set; }

        public WDACSigner()
        {
            this.FileAttribRefs = new List<string>();
        }
    }
    
    public class DriverFile
    {
        public string Path { get; set; }
        public bool isKernel { get; set; }
        public bool isPE { get; set; }

        public DriverFile(string _path, bool _isKernel, bool _isPE)
        {
            this.Path = _path;
            this.isKernel = _isKernel;
            this.isPE = _isPE; 
        }
    }

    // Class for Policy xml Settings
    public class PolicySettings
    {
        public string Provider { get; set; }
        public string Key { get; set; }
        public string ValueName { get; set; }
        public bool ValBool { get; set; }
        public string ValString { get; set; }
    }

    public class PolicyCISigners
    {
        public string SignerId { get; set; }
    }

    public class PolicySupplementalSigners
    {
        public string SignerId { get; set; }
    }

    public class PolicyUpdateSigners
    {
        public string SignerId { get; set; }
    }

    public class PolicySigningScenarios
    {
        // Attributes
        public string Value { get; set; }
        public string ID { get; set; }
        public string FriendlyName { get; set; }

        /// <summary>
        /// List of string SignerIDs to lookup in Policy.Signers Dict
        /// </summary>
        public List<string> Signers { get; set; }

        /// <summary>
        /// List of string rule IDS to lookup in Policy.FileRules Dict
        /// </summary>
        public List<string> FileRules { get; set; }


        public PolicySigningScenarios()
        {
            this.Signers = new List<string>();
            this.FileRules = new List<string>();
        }


    }

    public class PolicySigners
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string CertID { get; set; }
        public string CertPub { get; set; }

        /// <summary>
        /// Signer action: "Allow" or "Deny"
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// List of IDs in the exception attribute of signers.
        /// </summary>
        public List<string> Exceptions { get; set; }

        /// <summary>
        /// List of Rule IDs which reference IDs in FileRules.
        /// </summary>
        public List<string> FileAttributes { get; set; }

        public void AddException(List<string> exceptionList)
        {
            this.Exceptions = exceptionList; 
        }

        public void AddFileAttribute(string ruleID)
        {
            this.FileAttributes.Add(ruleID); // Add ruleID to File Attributes list
        }

        public PolicySigners()
        {
            this.Exceptions = new List<string>();
            this.FileAttributes = new List<string>(); 
        }
    }

    public class PolicyEKUs
    {
        public string ID { get; set; }
        public string Value { get; set; }
        public string FriendlyName { get; set; }
    }

    public class PolicyFileRules
    {
        public enum RuleType
        {
            FileName,   // -Level FileName
            FilePath,   // -Level FilePath
            Hash        // -Level Hash
        }

        public enum FileNameLevel
        {
            None,
            OriginalFileName,
            InternalName,
            FileDescription,
            ProductName,
            PackageFamilyName,
            FilePath
        }

        public string Action { get; set; } //Either Deny or Allow
        public string ID { get; set; }
        public string FriendlyName { get; set; }
        public string FileName { get; set; }
        public string MinimumFileVersion { get; set; }
        public string Hash { get; set; }
        public string FilePath { get; set; }
        public RuleType _RuleType { get; set; }

        public void SetRuleType()
        {
            if (String.IsNullOrEmpty(this.Hash) && String.IsNullOrEmpty(this.FilePath))
            {
                this._RuleType = RuleType.FileName;
            }
            else if (String.IsNullOrEmpty(this.Hash) && String.IsNullOrEmpty(this.FileName))
            {
                this._RuleType = RuleType.FilePath;
            }
            else
            {
                this._RuleType = RuleType.Hash;
            }
        }

        public RuleType GetRuleType()
        {
            return this._RuleType; 
        }

    }

    // Custom Values object to organize custom values in Custom Rules object
    public class CustomValue
    {
        public string Publisher;
        public string MinVersion;
        public string MaxVersion;
        public string FileName;
        public string ProductName;
        public string Description;
        public string InternalName;
        public string Path;
        public List<string> PackageFamilyNames; 
        public List<string> Hashes; 

        public CustomValue()
        {
            this.Hashes = new List<string>();
            this.PackageFamilyNames = new List<string>();
        }
    }

    public class PolicyCustomRules
    {
        public enum RuleType
        {
            None,
            Publisher,
            FileAttributes,   // RuleLevel set to "FileName", gives way to additional switch "SpecificFileNameLevel"
            PackagedApp,
            FilePath,
            FolderPath,
            Hash, 
            Com, 
            FolderScan
        }

        public enum RuleLevel
        {
            None,             // Null Value for RuleLevel (used in RulesFromDrivers for signaling no fallback)
            Hash,             // Use only the file's hash in rules
            FileName,         // File name and Minimum Version specified
            RootCertificate,  // Use the Root CA certificate (top-level)
            PcaCertificate,   // Use the PCA certificate that issued the signer,
            Publisher,        // PCA+Publisher signer rules
            FilePublisher,    // Generate rules that tie filename and minimum version to a PCA/Publisher combo
            SignedVersion,    // Minimum version tied to PCA Cert and from specific publisher (filename = *)
            FilePath,         // FilePath
            Folder,           // Folder pathrule applied to each PE file in the folder
            InternalName,
            ProductName,
            FileDescription,
            OriginalFileName,
            PackagedFamilyName // Packaged app rule
        }

        public struct CheckboxStates
        {
            public bool checkBox0;
            public bool checkBox1;
            public bool checkBox2;
            public bool checkBox3;
            public bool checkBox4;
        }

        public CheckboxStates CheckboxCheckStates;

        public struct SigningScenarioStates
        {
            public bool umciEnabled;
            public bool kmciEnabled;
        }

        public SigningScenarioStates SigningScenarioCheckStates;



        public enum RulePermission { Allow, Deny };

        // enums: 
        public RuleLevel Level { get; set; }
        public RuleType Type { get; set; }
        public RulePermission Permission { get; set; }

        // Variables:
        public string ReferenceFile { get; set; }
        public Dictionary<string, string> FileInfo { get; set; } //
        public string PSVariable { get; set; }
        public string VersionNumber { get; set; }
        public string RuleIndex { get; set; } // Index of return struct in Get-SystemDriver cmdlet
        public int RowNumber { get; set;  }     // Index of the row in the datagrid

        // Custom values
        public bool UsingCustomValues { get; set; }
        public CustomValue CustomValues { get; set; }
        public List<string> PackagedFamilyNames { get; set; }

        // EKU Attributes
        public string EKUFriendly { get; set; }
        public string EKUEncoded { get; set; }

        // Filepath params
        public List<string> FolderContents { get; set; }

        // Exception Params -- currently not supporting
        public List<PolicyCustomRules> ExceptionList { get; set; }

        // COM Object
        public COM COMObject { get; set; }

        // Folder Scan
        public FolderScan Scan { get; set; }

        // Constructors
        public PolicyCustomRules()
        {
            this.Type = RuleType.None;  
            this.Level = RuleLevel.None;
            this.Permission = RulePermission.Allow; // Allow by default to match the default state of the UI

            this.FileInfo = new Dictionary<string, string>();
            this.ExceptionList = new List<PolicyCustomRules>();
            this.FolderContents = new List<string>();

            this.UsingCustomValues = false;
            this.CustomValues = new CustomValue();
            this.PackagedFamilyNames = new List<string>();

            // Set checkbox states
            this.CheckboxCheckStates = new CheckboxStates();
            this.CheckboxCheckStates.checkBox0 = false;
            this.CheckboxCheckStates.checkBox1 = false;
            this.CheckboxCheckStates.checkBox2 = false;
            this.CheckboxCheckStates.checkBox3 = false;
            this.CheckboxCheckStates.checkBox4 = false;

            // Set signing scenario states
            this.SigningScenarioCheckStates = new SigningScenarioStates();
            this.SigningScenarioCheckStates.umciEnabled = true;
            this.SigningScenarioCheckStates.kmciEnabled = false;

            this.COMObject = new COM();
            this.Scan = new FolderScan(); 
        }

        /// <summary>
        /// Creates a FilePath CustomRule after creating a PowerShell variable
        /// </summary>
        /// <param name="psVar">XmlNode holding the rule</param>
        /// <param name="ruleIndex">Is it a FileRule or a SignerRule</param>
        /// <param name="refFile">The document which contains node.</param>
        /// <param name="_Type">is the file a user mode file?</param>
        /// 
        public PolicyCustomRules(string psVar, string ruleIndex, string refFile, RulePermission _Permission)
        {
            this.Permission = RulePermission.Allow;  // Allow by default to match the default state of the UI
            this.Level = RuleLevel.FilePath;
            this.ReferenceFile = refFile;
            this.PSVariable = psVar;
            this.RuleIndex = ruleIndex;
            this.ExceptionList = new List<PolicyCustomRules>();
            this.FileInfo = new Dictionary<string, string>();

            this.UsingCustomValues = false;
        }

        public void SetRuleLevel(RuleLevel ruleLevel)
        {
            this.Level = ruleLevel;
        }

        public void SetRuleType(RuleType ruleType)
        {
            this.Type = ruleType;
        }

        public void SetRulePermission(RulePermission rulePermission)
        {
            this.Permission = rulePermission; 
        }

        /// <summary>
        /// Returns code signing rule level: Hash, FileName, FilePath, Publisher, PcaCertificate, FilePublisher
        /// </summary>
        public RuleLevel GetRuleLevel()
        {
            return this.Level;
        }

        /// <summary>
        /// Returns type Publisher, File/folder path, File Attributes, PFN or Hash
        /// </summary>
        public RuleType GetRuleType()
        {
            return this.Type;
        }

        /// <summary>
        /// Returns Allow or Deny rule type
        /// </summary>
        public RulePermission GetRulePermission()
        {
            return this.Permission; 
        }

        // Methods
        public void AddException(PolicyCustomRules.RuleType type, PolicyCustomRules.RuleLevel level, Dictionary<string,string> fileInfo, string refFile)
        {
            PolicyCustomRules ruleException = new PolicyCustomRules();
            ruleException.Type = type;
            ruleException.Level = level;
            ruleException.FileInfo = fileInfo;
            ruleException.ReferenceFile = refFile;

            this.ExceptionList.Add(ruleException);
        }

        public void AddException(PolicyCustomRules ruleException)
        {
            if(ruleException.Type != PolicyCustomRules.RuleType.None 
                || ruleException.Level != PolicyCustomRules.RuleLevel.None
                || ruleException.FileInfo != null 
                || ruleException.ReferenceFile!= null)
            {
                this.ExceptionList.Add(ruleException);
            }
            else
            {
                // Log error or something
            }
        }
    }

    public class Logger
    {
        public StreamWriter Log;
        public string FileName;

        // Singleton pattern here we only allow one instance of the class. 

        public Logger(string _FolderName)
        {
            string fileName = GetLoggerDst();
            this.FileName = _FolderName + fileName;

            if (!File.Exists(this.FileName))
            {
                this.Log = new StreamWriter(this.FileName);
            }

            this.Log.AutoFlush = true;
            this.AddBoilerPlate(); 
        }

        public void AddInfoMsg(string info)
        {
            string msg = String.Format("{0} [INFO]: {1}", Helper.GetFormattedDateTime(), info);
            this.Log.WriteLine(msg);
        }
        public void AddErrorMsg(string error)
        {
            string msg = String.Format("{0} [ERROR]: {1}", Helper.GetFormattedDateTime(), error);
            this.Log.WriteLine(msg);
        }

        public void AddErrorMsg(string error, Exception e)
        {
            string msg = String.Format("{0} [ERROR]: {1}: {2}", Helper.GetFormattedDateTime(), error, e.ToString());
            this.Log.WriteLine(msg);
        }

        public void AddErrorMsg(string error, Exception e, int lineN)
        {
            string msg = String.Format("{0} [ERROR] at line {1}. \r\n {2}: {3}", Helper.GetFormattedDateTime(), lineN, error, e.ToString());
            this.Log.WriteLine(msg);
        }

        public void AddWarningMsg(string warning)
        {
            string msg = String.Format("{0} [WARNING]: {1}", Helper.GetFormattedDateTime(), warning);
            this.Log.WriteLine(msg);
        }

        public void AddNewSeparationLine(string subTitle)
        {
            string[] msg = new string[3];
            msg[0] = String.Format("{0} [INFO]: **********************************************************************", Helper.GetFormattedDateTime());
            msg[1] = String.Format("{0} [INFO]: {1}", Helper.GetFormattedDateTime(), subTitle);
            msg[2] = String.Format("{0} [INFO]: **********************************************************************", Helper.GetFormattedDateTime());

            foreach(var line in msg)
            {
                this.Log.WriteLine(line);
            }
        }

        /// <summary>
        /// Sets the name for the log file based on date and time
        /// </summary>
        /// <returns></returns>
        public string GetLoggerDst()
        {
            string fileName = String.Format("/Log_{0}_{1}.txt", Helper.GetFormattedDate(), Helper.GetFormattedTime());
            return fileName;
        }

        /// <summary>
        /// Closes the Log StreamWriter object
        /// </summary>
        public void CloseLogger()
        {
            //this.Log.Flush();
            this.Log.Close(); 
        }

        /// <summary>
        /// Adds information on the Wizard and OS like their versions
        /// </summary>
        private void AddBoilerPlate()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.AddInfoMsg(String.Format("WDAC Policy Wizard Version # {0}", versionInfo.FileVersion));
            this.AddInfoMsg(String.Format("Session ID: {0}", GetInstallTime())); 
        }

        private string GetInstallTime()
        {
            RegistryHive rootNode = RegistryHive.LocalMachine;
            RegistryView registryView = RegistryView.Registry64;
            RegistryKey root = RegistryKey.OpenBaseKey(rootNode, registryView);
            RegistryKey registryKey = root.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            RegistryValueKind subKeyValueKind = registryKey.GetValueKind("InstallTime");
            object subKeyValue = null;
            subKeyValue = registryKey.GetValue("InstallTime");

            return subKeyValue.ToString(); /*

            int dword = (int)subKeyValue;
            string valueAsStr = true ? Convert.ToString(dword, 16).ToUpper() : dword.ToString();

            string inst =  Convert.ToString(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "BuildLab", ""));
            return valueAsStr; */
        }
    }
}


