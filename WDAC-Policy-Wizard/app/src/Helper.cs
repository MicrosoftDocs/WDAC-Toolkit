// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq; 
using System.Diagnostics;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Windows.Management.Deployment;
using Windows.ApplicationModel;
using System.Formats.Asn1;


namespace WDAC_Wizard
{
    internal static class Helper
    {
        // Unsupported Crypto OIDs
        // Oids and friendly names
        static Dictionary<string, string> UnsupportedOIDs = new Dictionary<string, string>
        {
            { "1.3.132.0.34", "ECC_P384" },
            {  "1.3.132.0.35", "ECC_P512" },
            {  "1.2.840.10045.3.1.7", "ECC_P256" },
            {  "1.2.840.10045.4.1", "ECDSA_SHA1" },
            {  "1.2.840.10045.4.3", "ECDSA_SPECIFIED" },
            {  "1.2.840.10045.4.3.2", "ECDSA_SHA256" },
            {  "1.2.840.10045.4.3.3", "ECDSA_SHA384" },
            {  "1.2.840.10045.4.3.4", "ECDSA_SH512" }
        };

        // Supported certificate extensions
        static List<string> SupportedCertificateExtensions = new List<string>
        {
            ".cer",
            ".crt",
            ".pem",
            ".pfx",
        };
        public enum BrowseFileType
        {
            Policy = 0,     // -Show .xml files
            EventLog = 1,   // -Show .evtx files
            PEFile = 2,     // -Show PE (.exe, .dll, .sys) files
            CsvFile = 3,    // -Show Csv file
            All = 4         // -Show . all files
        }

        static int UniquePolicyId = 0; 

        /// <summary>
        /// Converts a path like \Device\HarddiskVolume3\Windows\System32\wbem\WMIC.exe to "C\Windows\System32\wbem\WMIC.exe
        /// </summary>
        /// <param name="NTPath">Path containing \Device\HarddiskVolume3</param>
        /// <returns>A converted path like C:\Windows</returns>
        public static string GetDOSPath(string NTPath)
        {
            string windowsDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows); // e.g. "C:\\WINDOWS"
            const int WINDOWS_L = 7;
            string logicalDisk = windowsDir.Substring(0, windowsDir.Length - WINDOWS_L); // Gets the logical disk name string of the harddrive (e.g. "C:\\")

            // Regex replace to take the NT path and convert to DOS Path
            Regex regex = new Regex("\\\\[a-zA-Z]+\\\\[a-zA-Z]+[0-9]+\\\\", RegexOptions.IgnoreCase);
            Match match = regex.Match(NTPath);
            if (match.Success)
            {
                string dosPath = NTPath.Replace(match.Value, logicalDisk);
                // If useEnvVars is set, convert C:, C:\Windows and C:\Windows\System32 to 
                // %OSDIR%, %WINDIR% and %SYSTEM32%
                if(Properties.Settings.Default.useEnvVars)
                {
                    return GetEnvPath(dosPath);
                }
                else
                {
                    return dosPath;
                }                    
            }
            else
            {
                return NTPath;
            }
        }

        /// <summary>
        /// Helper function to call OpenFileDialog and multi-select files
        /// </summary>
        /// <param name="displayTitle">Title to display on the Open File Dialog UI</param>
        /// <param name="browseFileType">Enum of types of files supported by the Wizard for opening</param>
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
                openFileDialog.Filter = "App Control Policy Files (*.xml)|*.xml";
            }
            else if (browseFileType == BrowseFileType.EventLog)
            {
                openFileDialog.Filter = "Event Log Files (*.evtx)|*.evtx";
            }
            else if (browseFileType == BrowseFileType.CsvFile)
            {
                openFileDialog.Filter = "MDE AH CSV Files (*.csv)|*.csv";
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
        /// <param name="displayTitle">Title to display on the Open File Dialog UI</param>
        /// <param name="browseFileType">Enum of types of files supported by the Wizard for opening</param>
        /// <returns>Path to file if user selects Ok and file exists. String.Empty otherwise</returns>
        public static string BrowseForSingleFile(string displayTitle, BrowseFileType browseFileType)
        {
            // Open file dialog to get file or folder path
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = displayTitle;
            openFileDialog.CheckPathExists = true;

            if (browseFileType.Equals(BrowseFileType.PEFile))
            {
                openFileDialog.Filter = "Portable Executable Files (*.exe; *.dll; *.rll; *.bin)|*.EXE;*.DLL;*.RLL;*.BIN|" +
                "Script Files (*.ps1, *.bat, *.vbs, *.js)|*.PS1;*.BAT;*.VBS;*.JS|" +
                "System Files (*.sys, *.hxs, *.mui, *.lex, *.mof)|*.SYS;*.HXS;*.MUI;*.LEX;*.MOF|" +
                "All Binary and Script Files (*.exe, ...) |*.EXE;*.DLL;*.RLL;*.BIN;*.PS1;*.BAT;*.VBS;*.JS;*.SYS;*.HXS;*.MUI;*.LEX;*.MOF|" +
                "All files (*.*)|*.*";

                openFileDialog.FilterIndex = 4; // Display All Binary Files by default (everything)
            }
            else if (browseFileType.Equals(BrowseFileType.Policy))
            {
                openFileDialog.Filter = "App Control Policy Files (*.xml)|*.xml";
            }
            else if (browseFileType.Equals(BrowseFileType.CsvFile))
            {
                openFileDialog.Filter = "MDE AH CSV Files (*.csv)|*.csv";
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
        /// <param name="displayTitle">Title to display on the Open File Dialog UI</param>
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
        /// <param name="displayTitle">Title to display on the Open File Dialog UI</param>
        /// <param name="browseFileType">Enum of types of files supported by the Wizard for saving</param>
        /// <returns>Path to file if user selects Ok and file exists. String.Empty otherwise</returns>
        public static string SaveSingleFile(string displayTitle, BrowseFileType browseFileType)
        {
            string saveLocationPath = String.Empty;

            // Save dialog box pressed
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = displayTitle;
            saveFileDialog.CheckPathExists = true;

            if (browseFileType == BrowseFileType.Policy)
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

        /// <summary>
        /// Scans the input string folderPth and finds the filepath with the greatest _ID
        /// </summary>
        /// <param name="folderPth">Path to the folder where to scan for a unique file</param>
        /// <returns>String with the newest _ID filename. example) policy_44.xml</returns>
        public static string GetUniquePolicyPath(string folderPth)
        {
            string newUniquePath = Path.Combine(folderPth, $"policy_{UniquePolicyId++}.xml");

            // Continue incrementing UniquePolicyId until we find one that does not exist in the path
            while(Path.Exists(newUniquePath))
            {
                newUniquePath = Path.Combine(folderPth, $"policy_{UniquePolicyId++}.xml");
            }
            
            return newUniquePath;
        }

        /// <summary>
        /// Deserialize the xml policy on disk to SiPolicy
        /// </summary>
        /// <param name="xmlPath">Path to the xml file on disk</param>
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
                Logger.Log.AddErrorMsg("DeserializeXMLtoPolicy() caught the following error", exp);
                return null;
            }

            return siPolicy;
        }

        /// <summary>
        /// Deserialize the xml policy string to SiPolicy
        /// </summary>
        /// <param name="xmlPath">Path to the xml file on disk</param>
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
                Logger.Log.AddErrorMsg("DeserializeXMLStringtoPolicy() caught the following exception", exp);
                return null;
            }

            return siPolicy;
        }

        /// <summary>
        /// Serialize the SiPolicy object to XML file
        /// </summary>
        /// <param name="siPolicy">SiPolicy object</param>
        /// <param name="xmlPath">Path on disk to serialize the SiPolicy to</param>
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

        /// <summary>
        /// Formats the CN of the certificate subject
        /// </summary>
        /// <param name="publisher">Subject of the publisher containing all the CN=, L=, etc.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Checks for empty/null or "N/A" in the provided text
        /// </summary>
        /// <param name="text">Text string to verify</param>
        /// <returns></returns>
        public static bool IsValidText(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            else if (text == Properties.Resources.DefaultFileAttributeString)
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

        /// <summary>
        /// Checks for unsupported crypto oids (ECC, ECSDA)
        /// </summary>
        /// <returns>True, if unsupported crypto oids are parsed</returns>
        public static bool IsCryptoInvalid(X509Chain certChain)
        {
            foreach (var cert in certChain.ChainElements)
            {
                if (UnsupportedOIDs.ContainsKey(cert.Certificate.SignatureAlgorithm.Value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check that version has 4 parts (follows ww.xx.yy.zz format), and each part < 2^16
        /// </summary>
        /// <param name="version">Version string (e.g. 22.33.456.7890) </param>
        /// <returns>Bool stating whether the input string is a valid version</returns>
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
                    Logger.Log.AddErrorMsg("IsValidVersion() caught the following exception", e);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Concatenates the file version parts (major, minor, build and private)
        /// </summary>
        /// <param name="fileVersionInfo"></param>
        /// <returns>Major.Minor.Build.Private if file version exists. Null, otherwise.</returns>
        public static string ConcatFileVersion(FileVersionInfo fileVersionInfo)
        {
            if (fileVersionInfo.FileMajorPart == null
                || fileVersionInfo.FileMinorPart == null
                || fileVersionInfo.FileBuildPart == null
                || fileVersionInfo.FilePrivatePart == null
                || fileVersionInfo.FileVersion == null)
            {
                return null;
            }

            return String.Format("{0}.{1}.{2}.{3}", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart,
                                   fileVersionInfo.FileBuildPart, fileVersionInfo.FilePrivatePart);
        }

        /// <summary>
        /// Compares the min and max versions
        /// </summary>
        /// <param name="minVersion"></param>
        /// <param name="maxVersion"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Validates the custom path rule against the requirements for path rules in WDAC
        /// Updated 22H2: path rules now support multiple wildcards throughout the path
        /// </summary>
        /// <param name="customPath"></param>
        /// <returns>True if valid path rule in WDAC. False, otherwise.</returns>
        public static bool IsValidPathRule(string customPath)
        {
            // Check for at most 1 wildcard param (*)
            // This check was removed since WDAC path rules can support multiple wildcards

            // Check for macros (%OSDRIVE%, %WINDIR%, %SYSTEM32%)
            if (customPath.Contains("%"))
            {
                var macroParts = customPath.Split('%');
                if (macroParts.Length == 3)
                {
                    if (!(macroParts[1] == "OSDRIVE"
                        || macroParts[1] == "WINDIR"
                        || macroParts[1] == "SYSTEM32"))
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

        /// <summary>
        /// Checks for the presence of "?" or "*" wildcard characters in a custom path
        /// </summary>
        /// <param name="customPath">Returns the number of wildcards found</param>
        /// <returns></returns>
        public static int GetNumberofWildcards(string customPath)
        {
            int nWildCards = 0;
            foreach (char c in customPath)
            {
                if (c == '*')
                {
                    nWildCards++;
                }

                // Add 2 to wildcards to automatically trigger the validation on the CustomRuleConditions
                // If only 1 '?' it would pass the check
                else if (c == '?')
                {
                    nWildCards += 2;
                }
            }

            return nWildCards;
        }

        /// <summary>
        /// Maps the input to one of the supported macro paths in WDAC, if one exists.
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
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
        /// </summary>
        /// <param name="path">Directory path to verify user writeability</param>
        /// <returns>Bool representing user writeability. True if writeable, otherwise, false</returns>
        public static bool IsUserWriteable(string path)
        {
            // Try to create a subdir in the folderPath. If successful, write access is true. 
            // If an exception is hit, the path is likely not user-writeable 
            try
            {
                string testPath = Path.Combine(path, "testSubDir");
                Directory.CreateDirectory(testPath);
                Directory.Delete(testPath, true);

                return true;
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg("IsUserWriteable() caught the following exception", e);
                return false;
            }
        }

        /// <summary>
        /// Get the Installed Appx/MSIX Packages
        /// </summary>
        /// <param name="policyCustomRule"></param>
        /// <returns></returns>
        public static List<string> GetAppxPackages(string pfnSearchStr)
        {
            PackageManager packageManager = new PackageManager();
            IEnumerable<Package> packages;
            List<string> results = new List<string>();
            pfnSearchStr = pfnSearchStr.ToLowerInvariant();

            try
            {
                // Try to get packages installed for all system users;
                // Will fail if Wizard is not running elevated
                packages = packageManager.FindPackages();
            }
            catch
            {
                // fall back and get for the current user
                Logger.Log.AddWarningMsg("GetAppxPackages() fallback to user check");
                packages = packageManager.FindPackagesForUser("");
            }

            // Search all packages for pfnSearchString
            foreach (var package in packages)
            {
                string packageFamilyName = package.Id.FamilyName;
                if (packageFamilyName.ToLowerInvariant().Contains(pfnSearchStr))
                {
                    results.Add(packageFamilyName);
                }
            }

            return results;
        }



        /// <summary>
        /// Dump all of the package family names for the custom rules table
        /// </summary>
        /// <param name="policyCustomRule">PolicyCustomRule object</param>
        /// <returns>Comma-delimitted tring of packaged family names</returns>
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

            if (policyCustomRule.UsingCustomValues)
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
            if (String.IsNullOrEmpty(stringEku))
            {
                return null;
            }

            AsnWriter asn = new AsnWriter(AsnEncodingRules.DER);
            asn.WriteObjectIdentifier(stringEku);

            var ekuByteArray = asn.Encode();
            ekuByteArray[0] = 1;
            return BitConverter.ToString(ekuByteArray).Replace("-", "");
        }

        /// <summary>
        /// Converts a hash byte[] to hash hex string
        /// </summary>
        /// <param name="hashByte">Byte array of representing a hash</param>
        /// <returns>Hexidecimal, readable hash string</returns>
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
            // Null/empty catch
            if(String.IsNullOrEmpty(sHash))
            {
                return new byte[0];
            }

            // Odd length
            if(sHash.Length % 2 == 1)
            {
                return new byte[0];
            }

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
                Logger.Log.AddErrorMsg("GetWinVersion() caught the following exception", e);
            }
            return -1;
        }

        /// <summary>
        /// SKU check to see if cmdlets are on the system 
        /// /// </summary>
        internal static void LicenseCheck()
        {
            // Check that WDAC feature is compatible with system
            // Cmdlets are available on all builds 1909+. 
            // Pre-1909, Enterprise SKU only: https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/feature-availability
            int REQUIRED_V = 1909;
            string REQUIRED_ED = "Enterprise";

            Logger.Log.AddInfoMsg("--- Feature Compat Check ---");

            string edition = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CompositionEditionID", "").ToString();
            string prodName = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
            int releaseN = Convert.ToInt32(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", ""));

            if (releaseN >= REQUIRED_V)
            {
                Logger.Log.AddInfoMsg(String.Format("Release Id: {0} meets min build requirements.", releaseN));
            }
            else if (edition.Contains(REQUIRED_ED) || prodName.Contains(REQUIRED_ED))
            {
                Logger.Log.AddInfoMsg(String.Format("Edition/ProdName:{0}/{1} meets min build requirements.", edition, prodName));
            }
            else
            {
                // Device does not meet the versioning or SKU check
                Logger.Log.AddWarningMsg(String.Format("Incompatible Windows Build Detected!! BuildN={0}", releaseN));
                Logger.Log.AddWarningMsg(String.Format("Incompatible Windows Edition/Product Detected!! CompositionEditionID={0} and ProductName={1}", edition, prodName));
                DialogResult res = MessageBox.Show("The Policy Wizard has detected an incompatible version of Windows. " +
                    "The Wizard may not be able to successfully complete policy creation.",
                    "Incompatible Windows Product",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                if (res == DialogResult.OK)
                {
                    try
                    {
                        string webpage = "https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/feature-availability";
                        Process.Start(new ProcessStartInfo(webpage) { UseShellExecute = true });
                    }
                    catch(Exception ex)
                    {
                        Logger.Log.AddErrorMsg("Launching webpage for LicenseCheck encountered the following exception", ex);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the Temp folder where policy and log writing must occur
        /// </summary>
        /// <returns>Location of the WDACWizard Temp folder location</returns>
        public static string GetTempFolderPath()
        {
            // This runs before Logger is instantiated -- do not add logging here to avoid deref error
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
        /// Gets the path to the WDACWizard.exe executable and its parent directory
        /// </summary>
        /// <param name="exePath"></param>
        /// <returns></returns>
        internal static string GetExecutablePath(bool exePath)
        {
            string executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string folderPath = System.IO.Path.GetDirectoryName(executablePath);
            if (exePath)
            {
                return executablePath;
            }
            else
            {
                return folderPath;
            }
        }

        /// <summary>
        /// Gets the path of the documents folder. If documents folder path doesn't exist, fallsback to Desktop folder
        /// </summary>
        /// <returns>MyDocuments folder path. Else, Desktop path</returns>
        internal static string GetDocumentsFolder()
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); 
            string deskPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            if(Path.Exists(docPath))
            {
                return docPath;
            }
            else
            {
                return deskPath;
            }
        }

        /// <summary>
        /// Performs verification on the certificate file to determine whether this is a valid
        /// certificate file
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>True if a valid certificate. False, otherwise</returns>
        internal static bool IsValidCertificateFile(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                return false;
            }

            // Check the file extension is a supported cert ext, return if false
            if(!SupportedCertificateExtensions.Contains(Path.GetExtension(filePath).ToLower()))
            {
                return false; 
            }

            // Try to load the file as a certificate. If the load is successful, the certificate is valid, return true
            // Otherwise, return false
            try
            {
                X509Certificate2 cert = new X509Certificate2(filePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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


