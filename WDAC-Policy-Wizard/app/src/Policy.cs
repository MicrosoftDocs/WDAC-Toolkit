// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace WDAC_Wizard
{
    /// <summary>
    /// The local SiPolicy class for all policy manipulation
    /// </summary>
    public class WDAC_Policy
    {
        /// <summary>
        /// Enum to handle the type of policy being created/manipulated, e.g. Base, Supplemental, Edit or Merge workflows
        /// </summary>
        public enum PolicyType
        {
            None, 
            BasePolicy, 
            SupplementalPolicy, 
            AppIdTaggingPolicy
        }

        /// <summary>
        /// The base policy format. Can be either legacy (single policy) or multiple policy (19H1+)
        /// </summary>
        public enum Format
        {
            None,
            Legacy,
            MultiPolicy
        }

        /// <summary>
        /// Tracks the workflow the user has selected. New policy, editing policy or merging policies
        /// </summary>
        public enum Workflow
        {
            None,
            New,
            Edit,
            Merge
        }

        /// <summary>
        /// The template being leveraged in the new single or multiple policy creation workflow
        /// </summary>
        public enum NewPolicyTemplate
        {
            None, 
            AllowMicrosoft,
            WindowsWorks, 
            SignedReputable, 
        }

        // Policy Properties
        public PolicyType _PolicyType { get; set; }
        public NewPolicyTemplate _PolicyTemplate { get; set; }
        public Workflow PolicyWorkflow { get; set; }
        public Format _Format { get; set; }
        public List<RuleType> PolicyRuleOptions;

        public string PolicyName { get; set; }          // User entered friendly name for policy
        public string PolicyID { get; set; }
        public bool EnableHVCI { get; set; }            // Configure hypervisor code integrity (HVCI)?
        public bool EnableAudit { get; set; }           // Turn on audit mode? 
        public string VersionNumber { get; set; }       // Policy version. By default, 10.0.0.0.

        // Policy Settings
        public bool UseUserModeBlocks { get; set; }
        public bool UseKernelModeBlocks { get; set; }

        // Paths:
        public string SchemaPath { get; set; }          // Path to final xml file on disk
        public string TemplatePath { get; set; }        // ReadOnly Path to template policy - TODO: make const
        public string EditPolicyPath { get; set; }      // Path to the policy we are editing. Used for parsing.
        public string BinPath { get; set;  }

        public List<string> PoliciesToMerge { get; set; }

        // Supplemental policy objs:
        public string BaseToSupplementPath { get; set; } // Path to base policy to supplement, if applicable
        public Guid BasePolicyId { get; set; }           // Id of the base policy the supplemental policy will expand

        // Datastructs for signing rules (and exceptions)
        public List<PolicyEKUs> EKUs { get; set; }
        public Dictionary<string, PolicyFileRules> FileRules { get; set; }
        public Dictionary<string, PolicySigners> Signers { get; set; }
        public List<PolicyUpdateSigners> UpdateSigners { get; set; }
        public List<PolicySupplementalSigners> SupplementalSigners { get; set; }
        public List<PolicyCISigners> CISigners { get; set; }
        public List<PolicySigningScenarios> SigningScenarios { get; set; }
        public List<PolicySettings> PolicySettings { get; set; }
        public Dictionary<string, Dictionary<string, string>> ConfigRules { get; set; }

        public List<PolicyCustomRules> CustomRules { get; set; }

        public SiPolicy siPolicy;

        public WDAC_Policy()
        {
            siPolicy = null; 
            PolicyRuleOptions = new List<RuleType>();

            EnableHVCI = false;
            EnableAudit = true;

            EKUs = new List<PolicyEKUs>();
            FileRules = new Dictionary<string, PolicyFileRules>();
            Signers = new Dictionary<string, PolicySigners>();//<PolicySigners>();
            SigningScenarios = new List<PolicySigningScenarios>();
            UpdateSigners = new List<PolicyUpdateSigners>();
            SupplementalSigners = new List<PolicySupplementalSigners>();
            CISigners = new List<PolicyCISigners>();
            PolicySettings = new List<PolicySettings>();
            CustomRules = new List<PolicyCustomRules>();
            PoliciesToMerge = new List<string>(); 

            VersionNumber = "10.0.0.0"; // Default policy version when calling the New-CIPolicy cmdlet
            PolicyID = Helper.GetFormattedDate();

            UseKernelModeBlocks = false;
            UseUserModeBlocks = false; 
        }

        /// <summary>
        /// Helper function to update the version number on a policy in edit. Will roll the version beginning with the LSB
        /// </summary>
        public string UpdateVersion()
        {
            int[] versionIdx = siPolicy.VersionEx.Split('.').Select(n => Convert.ToInt32(n)).ToArray(); 
            for (int i = versionIdx.Length-1; i > 0; i--)
            {
                if (versionIdx[i] >= UInt16.MaxValue)
                {
                    versionIdx[i] = 0;
                    versionIdx[i - 1]++;
                }
                else
                { 
                    versionIdx[i]++;
                    break;  
                }
            }

            // 65535.65535.65535.65535 will roll to 0.0.0.0
            if(versionIdx[0] > UInt16.MaxValue)
            {
                versionIdx[0] = 0; 
            }

            // Convert int[] --> this.VersionNumber string
            VersionNumber = ""; // reset string 
            foreach(var vIdx in versionIdx)
            {
                VersionNumber += String.Format("{0}.", vIdx.ToString());
            } 
            VersionNumber = VersionNumber.Substring(0, VersionNumber.Length - 1); //remove trailing period

            return VersionNumber; 
        }

        /// <summary>
        /// Determines whether the policy file contains a version number and position.
        /// </summary>
        /// <returns>Position of the _v_ in the filename. Returns 0 if the filename does not contain version number.</returns>
        public int EditPathContainsVersionInfo()
        {
            // Min length based on min version (0.0.0.0)
            int minFileNameLen = 7; 
            if (EditPolicyPath == null || EditPolicyPath.Length < minFileNameLen)
            {
                return 0;
            }

            // Find last instance of "_v" substring 
            string fileName = Path.GetFileNameWithoutExtension(EditPolicyPath);
            int index = fileName.LastIndexOf("_v"); 
            if (index < 0)
            {
                return 0; 
            }

            // Assert 3 dots to denote version
            var parts = fileName.Substring(index).Split('.');
            if(parts.Length < 4)
            {
                // Fewer than 3 version fields
                return 0;
            }

            // Return the index pos + length of dir
            return index + Path.GetDirectoryName(EditPolicyPath).Length + 1; 
        }

        /// <summary>
        /// Checks if a given rule option is already specified in the Policy
        /// </summary>
        /// <param name="targetRuleOption">Rule OptionType to query the Policy object for</param>
        /// <returns></returns>
        public bool HasRuleOption(OptionType targetRuleOption)
        {
            foreach(var ruleOption in PolicyRuleOptions)
            {
                if(ruleOption.Item == targetRuleOption)
                {
                    return true; 
                }
            }

            return false; 
        }

        /// <summary>
        /// Removes a given rule option if it exists in the Policy
        /// </summary>
        /// <param name="targetRuleOption"></param>
        public void RemoveRuleOption(OptionType targetRuleOption)
        {
            List<RuleType> tempRuleOptions = PolicyRuleOptions; 
            for (int i = 0; i < tempRuleOptions.Count; i++)
            {
                if (tempRuleOptions[i].Item == OptionType.EnabledAllowSupplementalPolicies)
                {
                    tempRuleOptions.RemoveAt(i);
                    break;
                }
            }

            PolicyRuleOptions = tempRuleOptions; 
        }

        /// <summary>
        /// Checks if a given rule option is already specified in the Policy
        /// </summary>
        /// <param name="targetRuleOption">Rule OptionType to query the Policy object for</param>
        /// <returns></returns>
        public bool HasRuleType(OptionType targetRuleOption)
        {
            foreach (var ruleOption in siPolicy.Rules)
            {
                if (ruleOption.Item == targetRuleOption)
                {
                    return true;
                }
            }

            return false;
        }

        internal static void Something()
        {

        }
    }

    /// <summary>
    /// Custom COM object class to manipulate during rule creation
    /// </summary>
    public class COM
    {
        const string COMVALUENAME = "EnterpriseDefinedClsId";

        public enum ProviderType
        {
            None,
            PowerShell,
            WSH,
            IE,
            VBA,
            MSI,
            AllHostIds
        }

        public string Guid { get; set; }
        public ProviderType Provider { get; set; }

        public bool ValueItem { get; set; }
        public string ValueName { get; }

        public COM()
        {
            Provider = ProviderType.None;
            ValueName = COMVALUENAME;
        }

        /// <summary>
        /// Checks whether the COM Guid is valid. Returns true if "All Keys" or custom Guid is properly formed with or without {}
        /// </summary>
        /// <returns>True/False</returns>
        public bool IsValidRule()
        {
            // Possible solution 1: All Keys
            if(Guid.Equals(Properties.Resources.ComObjectAllKeys))
            {
                return true; 
            }

            // Possible solution 2: Valid GUID Format
            Guid _guid = System.Guid.NewGuid(); 
            return System.Guid.TryParse(Guid, out _guid); 
        }
    }

    /// <summary>
    /// Custom AppID class to manipulate during rule creation
    /// </summary>
    public class AppID
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public bool IsValidTag()
        {
            return !String.IsNullOrEmpty(Value)
                    && !String.IsNullOrEmpty(Key); 
        }
    }

    public class FolderScan
    {
        // Rule Levels. The first in the list will be passed in as the -Level
        // The rest will be passed in as -Fallback
        public List<string> Levels { get; set; }
        
        // Struct to store the paths to omit while scanning
        public List<string> OmitPaths { get; set; }

        public FolderScan()
        {
            Levels = new List<string>();
            OmitPaths = new List<string>(); 
        }
    }

    public class WDACSigner
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string CertOemID { get; set; }
        public string CommonName { get; set; }
        public List<string> FileAttribRefs { get; set; }

        public WDACSigner()
        {
            FileAttribRefs = new List<string>();
        }
    }

    public class DriverFile
    {
        public string Path { get; set; }
        public bool isKernel { get; set; }
        public bool isPE { get; set; }

        public DriverFile(string _path, bool _isKernel, bool _isPE)
        {
            Path = _path;
            isKernel = _isKernel;
            isPE = _isPE;
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
            Signers = new List<string>();
            FileRules = new List<string>();
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
            Exceptions = exceptionList;
        }

        public void AddFileAttribute(string ruleID)
        {
            FileAttributes.Add(ruleID); // Add ruleID to File Attributes list
        }

        public PolicySigners()
        {
            Exceptions = new List<string>();
            FileAttributes = new List<string>();
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
            if (String.IsNullOrEmpty(Hash) && String.IsNullOrEmpty(FilePath))
            {
                _RuleType = RuleType.FileName;
            }
            else if (String.IsNullOrEmpty(Hash) && String.IsNullOrEmpty(FileName))
            {
                _RuleType = RuleType.FilePath;
            }
            else
            {
                _RuleType = RuleType.Hash;
            }
        }

        public RuleType GetRuleType()
        {
            return _RuleType;
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
            Hashes = new List<string>();
            PackageFamilyNames = new List<string>();
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
            FolderScan,
            Certificate, 
            AppIDTag
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
        public Dictionary<string, string> FileInfo { get; set; }    // FileInfo dict containing PE metadata and sig CNs
        public bool SupportedCrypto { get; set; }                   // Boolean flag indicating whether the crypto detected is supported in WDAC
        public string PSVariable { get; set; }
        public string VersionNumber { get; set; }
        public string RuleIndex { get; set; }                       // Index of return struct in Get-SystemDriver cmdlet
        public int RowNumber { get; set; }                         // Index of the row in the datagrid

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

        // AppID Tags
        public AppID AppIDTag {get;set;}

        // Folder Scan
        public FolderScan Scan { get; set; }

        // Constructors
        public PolicyCustomRules()
        {
            Type = RuleType.None;
            Level = RuleLevel.None;
            Permission = RulePermission.Allow; // Allow by default to match the default state of the UI

            FileInfo = new Dictionary<string, string>();
            SupportedCrypto = true;
            ExceptionList = new List<PolicyCustomRules>();
            FolderContents = new List<string>();

            UsingCustomValues = false;
            CustomValues = new CustomValue();
            PackagedFamilyNames = new List<string>();

            // Set checkbox states
            CheckboxCheckStates = new CheckboxStates();
            CheckboxCheckStates.checkBox0 = false;
            CheckboxCheckStates.checkBox1 = false;
            CheckboxCheckStates.checkBox2 = false;
            CheckboxCheckStates.checkBox3 = false;
            CheckboxCheckStates.checkBox4 = false;

            // Set signing scenario states
            SigningScenarioCheckStates = new SigningScenarioStates();
            SigningScenarioCheckStates.umciEnabled = true;
            SigningScenarioCheckStates.kmciEnabled = false;

            COMObject = new COM();
            Scan = new FolderScan();
            AppIDTag = new AppID(); 
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
            Permission = RulePermission.Allow;  // Allow by default to match the default state of the UI
            Level = RuleLevel.FilePath;
            ReferenceFile = refFile;
            PSVariable = psVar;
            RuleIndex = ruleIndex;
            ExceptionList = new List<PolicyCustomRules>();
            FileInfo = new Dictionary<string, string>();

            UsingCustomValues = false;
        }

        public void SetRuleLevel(RuleLevel ruleLevel)
        {
            Level = ruleLevel;
        }

        public void SetRuleType(RuleType ruleType)
        {
            Type = ruleType;
        }

        public void SetRulePermission(RulePermission rulePermission)
        {
            Permission = rulePermission;
        }

        /// <summary>
        /// Returns code signing rule level: Hash, FileName, FilePath, Publisher, PcaCertificate, FilePublisher
        /// </summary>
        public RuleLevel GetRuleLevel()
        {
            return Level;
        }

        /// <summary>
        /// Returns type Publisher, File/folder path, File Attributes, PFN or Hash
        /// </summary>
        public RuleType GetRuleType()
        {
            return Type;
        }

        /// <summary>
        /// Returns Allow or Deny rule type
        /// </summary>
        public RulePermission GetRulePermission()
        {
            return Permission;
        }

        // Methods
        public void AddException(RuleType type, RuleLevel level, Dictionary<string, string> fileInfo, string refFile)
        {
            PolicyCustomRules ruleException = new PolicyCustomRules();
            ruleException.Type = type;
            ruleException.Level = level;
            ruleException.FileInfo = fileInfo;
            ruleException.ReferenceFile = refFile;

            ExceptionList.Add(ruleException);
        }

        /// <summary>
        /// Adds the PolicyCustomRule exception to the ExceptionsList list
        /// </summary>
        /// <param name="ruleException"></param>
        public void AddException(PolicyCustomRules ruleException)
        {
            ExceptionList.Add(ruleException);
        }

        /// <summary>
        /// Returns true if at least one checkbox is checked. False, otherwise. 
        /// </summary>
        /// <returns></returns>
        public bool IsAnyBoxChecked()
        {
            return CheckboxCheckStates.checkBox0 || CheckboxCheckStates.checkBox1
                || CheckboxCheckStates.checkBox2 || CheckboxCheckStates.checkBox3
                || CheckboxCheckStates.checkBox4;
        }
    }
}
