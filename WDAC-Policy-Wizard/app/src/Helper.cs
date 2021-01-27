// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.IO;
using System.Text; 
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.Azure; 
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob; 

namespace WDAC_Wizard
{
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
                this._RuleType = RuleType.FileName;
            else if (String.IsNullOrEmpty(this.Hash) && String.IsNullOrEmpty(this.FileName))
                this._RuleType = RuleType.FilePath;
            else
                this._RuleType = RuleType.Hash;         
        }

        public RuleType GetRuleType()
        {
            return this._RuleType; 
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
            PcaCertificate,   // Use the PCA certificate that issued the signer,
            Publisher,        // PCA+Publisher signer rules
            FilePublisher,    // Generate rules that tie filename and minimum version to a PCA/Publisher combo
            SignedVersion,    // Minimum version tied to PCA Cert and from specific publisher (filename = *)
            FilePath,         // FilePath
            Folder,           // Folder pathrule applied to each PE file in the folder
            InternalName,
            ProductName,
            FileDescription,
            OriginalFileName
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


        // Filepath params
        public List<string> FolderContents { get; set; }

        // Exception Params -- currently not supporting
        public List<RuleException> ExceptionList { get; set; }

        // Constructors
        public PolicyCustomRules()
        {
            this.Type = RuleType.None;  
            this.Level = RuleLevel.None;
            this.Permission = RulePermission.Allow; // Allow by default to match the default state of the UI

            this.FileInfo = new Dictionary<string, string>();
            this.ExceptionList = new List<RuleException>();
            this.FolderContents = new List<string>();
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
            this.ExceptionList = new List<RuleException>();
            this.FileInfo = new Dictionary<string, string>();
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
        public void AddException(string type, uint level, string[] fileInfo, string refFile)
        {
            RuleException ruleException = new RuleException();
            ruleException.ExceptionType = type;
            ruleException.ExceptionLevel = level;
            ruleException.ExceptionFileInfo = fileInfo;
            ruleException.ExceptionReferenceFile = refFile;

            this.ExceptionList.Add(ruleException);
        }

        public bool isEnvVar()
        {
            // if the path contains one of the following environment variables -- return true as the cmdlets can replace it
            if (this.ReferenceFile.Contains(Path.GetPathRoot(Environment.SystemDirectory)) || 
                this.ReferenceFile.Contains(Environment.GetFolderPath(Environment.SpecialFolder.Windows)) ||
                this.ReferenceFile.Contains(Environment.GetFolderPath(Environment.SpecialFolder.System)))
                return true;

            // otherwise, not an env variable we support
            else
                return false; 
        }

        public string GetEnvVar()
        {
            string sys = Environment.GetFolderPath(Environment.SpecialFolder.System);
            string win = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string os = Path.GetPathRoot(Environment.SystemDirectory);

            string retVal = String.Empty; 

            if (this.ReferenceFile.Contains(Environment.GetFolderPath(Environment.SpecialFolder.System))) //OSDRIVE/WINDOWS/system32
                retVal =  "%SYSTEM32%/" + this.ReferenceFile.Substring(Environment.GetFolderPath(Environment.SpecialFolder.System).Length); 

            else if (this.ReferenceFile.Contains(Environment.GetFolderPath(Environment.SpecialFolder.Windows))) //OSDRIVE/WINDOWS
                retVal = "%WINDIR%/" + this.ReferenceFile.Substring(Environment.GetFolderPath(Environment.SpecialFolder.Windows).Length);

            else if (this.ReferenceFile.Contains(Path.GetPathRoot(Environment.SystemDirectory))) // OSDRIVE
                retVal = "%OSDRIVE%/" + this.ReferenceFile.Substring(Path.GetPathRoot(Environment.SystemDirectory).Length);

            else
                retVal = "";

            return retVal;
        }
    }

    public class RuleException
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
                this.Log = new StreamWriter(this.FileName);

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
        }
    }
}


