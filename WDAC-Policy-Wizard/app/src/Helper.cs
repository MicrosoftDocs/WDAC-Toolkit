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

        public enum BrowseFileType
        {
            Policy = 0,     // -Show .xml files
            EventLog = 1,   // -Show .evtx files
            All = 2         // -Show . all files
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
            if(match.Success)
            {
                string dosPath = NTPath.Replace(match.Value, logicalDisk);
                return dosPath;
            }
            else
            {
                return null;
            }
        }


        public static List<DriverFile> ReadArbitraryEventLogs(List<string> auditLogPaths)
        {
            List<DriverFile> driverPaths = new List<DriverFile>();
            

            // If path is specifed, parse the event logs into driverPaths list
            // Iterate through all of the auditLogPaths

            foreach (var auditLogPath in auditLogPaths)
            {
                // Check that path provided is an evtx log
                if (Path.GetExtension(auditLogPath) != ".evtx")
                {
                    continue; // do something here? Log?
                }

                EventLogReader log = new EventLogReader(auditLogPath, PathType.FilePath);
                for (EventRecord entry = log.ReadEvent(); entry != null; entry = log.ReadEvent())
                {
                    if (entry.Id == AUDIT_PE || entry.Id == BLOCK_PE)
                    {
                        string filePath;
                        bool isKernel = false;
                        bool isPE = true;

                        string ntfilePath = entry.Properties[1].Value.ToString(); // e.g. "\\Device\\HarddiskVolume3\\Windows\\CCM\\StateMessage.dll"
                        filePath = GetDOSPath(ntfilePath); // replace \\Device\\HarddiskVolume\\ with harddrive string name e.g. C:\\

                        if (filePath == null || !File.Exists(filePath))
                        {
                            continue;
                        }

                        if (entry.Properties[12].Value.ToString() == "0")
                        {
                            isKernel = true;
                        }

                        driverPaths.Add( new DriverFile(filePath, isKernel, isPE));

                    }
                    else if (entry.Id == AUDIT_KERNEL || entry.Id == BLOCK_KERNEL) // This is an entry for kernel
                    {
                        string filePath;
                        bool isKernel = true;
                        bool isPE = true;

                        string windowsDirRelativePath = entry.Properties[1].Value.ToString();
                        string _windowsDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                        filePath = _windowsDir + @"\" + windowsDirRelativePath;

                        if (!File.Exists(filePath))
                        {
                            continue;
                        }

                        driverPaths.Add(new DriverFile(filePath, isKernel, isPE));
                    }

                    else if(entry.Id == AUDIT_SCRIPT || entry.Id == BLOCK_SCRIPT)
                    {
                        string filePath;
                        bool isKernel = false;
                        bool isPE = false;

                        string ntfilePath = entry.Properties[1].Value.ToString();
                        filePath = GetDOSPath(ntfilePath); // replace \\Device\\HarddiskVolume\\ with harddrive string name e.g. C:\\

                        if (filePath == null || !File.Exists(filePath))
                        {
                            continue;
                        }
                        driverPaths.Add(new DriverFile(filePath, isKernel, isPE));
                    }

                }
                
            }

            return driverPaths;
        }

        public static SiPolicy ReadMachineEventLogs(string tempPath, string level)
        {
            // If path is not specified, the event logs to read are the on-machine CodeIntegrity/Operational, and AppLocker/MSI and Script
            // Simply call the New-CIPolicy -Audit cmdlet, and serialize the policy
            SiPolicy siPolicy; 
            string policyPath = Path.Combine(tempPath, AUDIT_POLICY_PATH);
            string logPath = Path.Combine(tempPath, AUDIT_LOG_PATH); 
            string auditCmd = String.Format("New-CIPolicy -Audit -Level {0} -FilePath {1} -UserPEs -Fallback Hash 3> {2}", level, policyPath, logPath); 

            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(auditCmd);

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch(Exception exp)
            {
                //Do something
            }

            XmlSerializer serializer = new XmlSerializer(typeof(SiPolicy));
            StreamReader reader = new StreamReader(policyPath);
            siPolicy = (SiPolicy)serializer.Deserialize(reader);
            reader.Close();

            return siPolicy; 
        }

        public static List<string> BrowseForMultiFiles(string displayTitle, BrowseFileType browseFileType)
        {
            List<string> policyPaths = new List<string>(); 

            // Open file dialog to get file or folder path
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = displayTitle;
            openFileDialog.CheckPathExists = true;

            if(browseFileType == BrowseFileType.Policy)
            {
                openFileDialog.Filter = "Policy Files (*.xml)|*.xml";
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
                newUniquePath = System.IO.Path.Combine(folderPth, "policy_0.xml"); //first temp policy being created
            }
            else
            {
                newUniquePath = System.IO.Path.Combine(folderPth, String.Format("policy_{0}.xml", NewestID + 1));
            }

            return newUniquePath;
        }

        public static SiPolicy DeserializeXMLtoPolicy(string xmlPath)
        {
            SiPolicy siPolicy; 
            if(xmlPath == null)
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
            catch(Exception exp)
            {
                return null; 
            }

            return siPolicy; 
        }

        public static void SerializePolicytoXML(SiPolicy siPolicy, string xmlPath)
        {
            if(siPolicy == null || xmlPath == null)
            {
                return; 
            }

            // Serialize policy to XML file
            XmlSerializer serializer = new XmlSerializer(typeof(SiPolicy));
            StreamWriter writer = new StreamWriter(xmlPath);
            serializer.Serialize(writer, siPolicy);
            writer.Close();
        }

        // Check that version has 4 parts (follows ww.xx.yy.zz format)
        // And each part < 2^16
        public static bool IsValidVersion(string version)
        {
            var versionParts = version.Split('.');
            if(versionParts.Length != 4)
            {
                return false; 
            }

            foreach(var part in versionParts)
            {
                try
                {
                    int _part = Convert.ToInt32(part);
                    if (_part > UInt16.MaxValue || _part < 0)
                    {
                        return false;
                    }
                }
                catch(Exception e)
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

            for(int i = 0; i < 4; i++)
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
            if(customPath.Contains("*"))
            {
                var wildCardParts = customPath.Split('*');
                if (wildCardParts.Length < 2)
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
            if(customPath.Contains("%"))
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
                foreach(var package in packages)
                {
                    if(package.Contains("\r\nPublisher       "))
                    {
                        string pkgName = package.Substring(1, package.Length - OFFSET); 
                        if(!output.ContainsKey(pkgName))
                        {
                            output[pkgName] = ""; 
                        }
                    }
                }
            }
            catch(Exception exp)
            {

            }

            return output; 
        }

        // Dump all of the package family names for the custom rules table
        public static string GetListofPackages(PolicyCustomRules policyCustomRule)
        {
            string output = String.Empty;
            
            if(policyCustomRule.PackagedFamilyNames == null)
            {
                return String.Empty; 
            }
            if(policyCustomRule.PackagedFamilyNames.Count == 0)
            {
                return String.Empty; 
            }

            foreach(var package in policyCustomRule.PackagedFamilyNames)
            {
                output += String.Format("{0}, ", package); 
            }

            output = output.Substring(0, output.Length - 2); // Trim off trailing whitespace and comma
            return output; 
        }
    }

    public class packedInfo
    {
        static public byte[] blobDeets = {
            0x44, 0x65, 0x66, 0x61, 0x75, 0x6C, 0x74, 0x45, 0x6E, 0x64, 0x70, 0x6F,
            0x69, 0x6E, 0x74, 0x73, 0x50, 0x72, 0x6F, 0x74, 0x6F, 0x63, 0x6F, 0x6C,
            0x3D, 0x68, 0x74, 0x74, 0x70, 0x73, 0x3B, 0x41, 0x63, 0x63, 0x6F, 0x75,
            0x6E, 0x74, 0x4E, 0x61, 0x6D, 0x65, 0x3D, 0x77, 0x64, 0x61, 0x63, 0x77,
            0x69, 0x7A, 0x61, 0x72, 0x64, 0x62, 0x6C, 0x6F, 0x62, 0x73, 0x74, 0x6F,
            0x72, 0x61, 0x67, 0x65, 0x3B, 0x41, 0x63, 0x63, 0x6F, 0x75, 0x6E, 0x74,
            0x4B, 0x65, 0x79, 0x3D, 0x30, 0x4B, 0x4C, 0x6A, 0x37, 0x64, 0x47, 0x4D,
            0x6D, 0x6C, 0x47, 0x7A, 0x74, 0x61, 0x31, 0x4E, 0x43, 0x57, 0x70, 0x6B,
            0x38, 0x45, 0x64, 0x61, 0x42, 0x6F, 0x45, 0x34, 0x2B, 0x58, 0x4A, 0x61,
            0x6E, 0x30, 0x6E, 0x6D, 0x4B, 0x69, 0x76, 0x34, 0x43, 0x54, 0x6B, 0x45,
            0x57, 0x30, 0x64, 0x59, 0x30, 0x6B, 0x4C, 0x34, 0x35, 0x63, 0x53, 0x70,
            0x69, 0x7A, 0x47, 0x77, 0x4D, 0x2B, 0x4A, 0x7A, 0x39, 0x44, 0x5A, 0x4B,
            0x68, 0x2F, 0x59, 0x55, 0x6C, 0x6E, 0x58, 0x59, 0x67, 0x35, 0x76, 0x6D,
            0x4D, 0x62, 0x39, 0x56, 0x35, 0x77, 0x3D, 0x3D, 0x3B, 0x45, 0x6E, 0x64,
            0x70, 0x6F, 0x69, 0x6E, 0x74, 0x53, 0x75, 0x66, 0x66, 0x69, 0x78, 0x3D,
            0x63, 0x6F, 0x72, 0x65, 0x2E, 0x77, 0x69, 0x6E, 0x64, 0x6F, 0x77, 0x73,
            0x2E, 0x6E, 0x65, 0x74
        };
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
        public string MinVersion;
        public string MaxVersion;
        public string FileName;
        public string ProductName;
        public string Description;
        public string InternalName;
        public string Path;
        public List<string> Hashes; 

        public CustomValue()
        {
            this.Hashes = new List<string>();
        }

    }

    public class PolicyCustomRules
    {
        public enum RuleType
        {
            None,
            Publisher,
            FileAttributes,   // RuleLevel set to "FileName", gives way to additional switch "SpecificFileNameLevel"
            FilePath,
            Folder,
            Hash
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

        public enum RulePermission { Allow, Deny };

        // enums: 
        public RuleLevel Level { get; set; }
        public RuleType Type { get; set; }
        public RulePermission Permission { get; set; }


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

        // Filepath params
        public List<string> FolderContents { get; set; }

        // Exception Params -- currently not supporting
        public List<PolicyCustomRules> ExceptionList { get; set; }

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
        /// Returns Allow or Deny rule type
        /// </summary>
        public RuleType GetRuleType()
        {
            return this.Type;
        }

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
            if(ruleException.Type != PolicyCustomRules.RuleType.None || ruleException.Level != PolicyCustomRules.RuleLevel.None
                || ruleException.FileInfo != null || ruleException.ReferenceFile!= null)
            {
                this.ExceptionList.Add(ruleException);
            }
            else
            {
                // Log error or something
            }
        }
    }

    public class RuleException : PolicyCustomRules
    {
        public string ExceptionType { get; set; } // Publisher, Path, Hash 
        public uint ExceptionLevel { get; set; } // 0= Publisher, 1=Prod name, 2=Filename
        public string[] ExceptionFileInfo { get; set; }
        public string ExceptionReferenceFile { get; set; } // 
    }

    public class Logger
    {
        public StreamWriter Log;
        public string FileName;
        private SHA256 Sha256 = SHA256.Create();

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


        public string GetPath()
        {
            return this.FileName; 
        }

        public void AddInfoMsg(string info)
        {
            string msg = String.Format("{0} [INFO]: {1}", DateTime.Now, info);
            this.Log.WriteLine(msg);
        }
        public void AddErrorMsg(string error)
        {
            string msg = String.Format("{0} [ERROR]: {1}", DateTime.Now, error);
            this.Log.WriteLine(msg);
        }

        public void AddErrorMsg(string error, Exception e)
        {
            string msg = String.Format("{0} [ERROR]: {1}: {2}", DateTime.Now, error, e.ToString());
            this.Log.WriteLine(msg);
        }

        public void AddErrorMsg(string error, Exception e, int lineN)
        {
            string msg = String.Format("{0} [ERROR] at line {1}. \r\n {2}: {3}", DateTime.Now, lineN, error, e.ToString());
            this.Log.WriteLine(msg);
        }

        public void AddWarningMsg(string warning)
        {
            string msg = String.Format("{0} [WARNING]: {1}", DateTime.Now, warning);
            this.Log.WriteLine(msg);
        }

        public void AddNewSeparationLine(string subTitle)
        {
            string[] msg = new string[3];
            msg[0] = String.Format("{0} [INFO]: **********************************************************************", DateTime.Now);
            msg[1] = String.Format("{0} [INFO]: {1}", DateTime.Now, subTitle);
            msg[2] = String.Format("{0} [INFO]: **********************************************************************", DateTime.Now);

            foreach(var line in msg)
            {
                this.Log.WriteLine(line);
            }
        }

        public string GetLoggerDst()
        {
            DateTime sDate = DateTime.Now;
            string fileName = String.Format("/Log_{0}{1}_{2}{3}{4}", sDate.ToString("MM"), sDate.ToString("dd"),
               sDate.ToString("HH"), sDate.ToString("mm"), sDate.ToString("ss")) + ".txt";
            return fileName;
        }

        public void CloseLogger()
        {
            //this.Log.Flush();
            this.Log.Close(); 
        }

        public bool UploadLog()
        {
            // Upload the log file to Azure Blob storage if data option is enabled
            try
            {
                // Flush and close logger before upload
                this.CloseLogger();

                // Create reference to the Azure Storage Account
                var blobBytes = packedInfo.blobDeets; 
                String blobString = System.Text.Encoding.Default.GetString(blobBytes);
                CloudStorageAccount storageacc = CloudStorageAccount.Parse(blobString);

                // Create Azure blob and container reference
                CloudBlobClient blobClient = storageacc.CreateCloudBlobClient();

                String blobContainerName = Properties.Resources.BlobContainerString;
                CloudBlobContainer container = blobClient.GetContainerReference(blobContainerName);
                container.CreateIfNotExists();

                // Get name for blob upload -- Date + Hash of contents
                String blobBlockName;

                blobBlockName = Path.GetFileNameWithoutExtension(this.FileName) + "__" + GetHashFromFile(this.FileName) + ".txt";

                // Upload log to storage with container set to container name
                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(blobBlockName);

                using (var filestream = System.IO.File.OpenRead(this.FileName))
                {
                    cloudBlockBlob.UploadFromStream(filestream);
                }
                return true; 
            }

            catch(Exception e)
            {
                return false; 
            }
            
        }

        // Private

        private string GetHashFromFile(string fileName)
        {
            Byte[] logHash;
            using (FileStream stream = File.OpenRead(this.FileName))
            {
                logHash = Sha256.ComputeHash(stream);
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < logHash.Length/2; i++)
            {
                sb.Append(logHash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        private void AddBoilerPlate()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.AddInfoMsg(String.Format("WDAC Policy Wizard Version # {0}", versionInfo.FileVersion));
            this.AddInfoMsg(String.Format("Session ID: {0}-{1}", this.getInstallTime(), DateTime.Now)); 
        }

        private string getInstallTime()
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


