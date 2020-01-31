// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using Squirrel;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using WDAC_Wizard.Properties;

namespace WDAC_Wizard
{

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
        public enum RuleLevel
        {
            None,             // Null Value for RuleLevel (used in RulesFromDrivers for signaling no fallback)
            Hash,             // Use only the file's hash in rules
            FileName,         // File name and Minimum Version specified
            PcaCertificate,   // Use the PCA certificate that issued the signer,
            Publisher,        // PCA+Publisher signer rules
            FilePublisher,    // Generate rules that tie filename and minimum version to a PCA/Publisher combo
            SignedVersion,    // [currently not supported] Minimum version tied to PCA Cert and from specific publisher (filename = *)
            FilePath,         // FilePath
            Folder            // Folder pathrule applied to each PE file in the folder
        }

        public enum RuleType { Allow, Deny };

        public RuleLevel Level { get; set; }
        public RuleType Type { get; set; }
        public string ReferenceFile { get; set; }
        public List<string> FileInfo { get; set; } //
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
            this.Type = RuleType.Allow;  // Allow by default to match the default state of the UI
            this.Level = RuleLevel.None;

            this.FileInfo = new List<string>();
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
        public PolicyCustomRules(string psVar, string ruleIndex, string refFile, RuleType _Type)
        {
            this.Type = RuleType.Allow;  // Allow by default to match the default state of the UI
            this.Level = RuleLevel.FilePath;
            this.ReferenceFile = refFile;
            this.PSVariable = psVar;
            this.RuleIndex = ruleIndex;
            this.ExceptionList = new List<RuleException>();
            this.FileInfo = new List<string>();
        }

        public void SetRuleLevel(RuleLevel ruleLevel)
        {
            this.Level = ruleLevel;
        }

        public void SetRuleType(RuleType ruleType)
        {
            this.Type = ruleType;
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

        public string GetLoggerDst()
        {
            DateTime sDate = DateTime.Now;
            string fileName = String.Format("/Log_{0}{1}_{2}{3}{4}", sDate.ToString("MM"), sDate.ToString("dd"),
               sDate.ToString("HH"), sDate.ToString("mm"), sDate.ToString("ss")) + ".txt";
            return fileName;
        }

        public void CloseLogger()
        {
            this.Log.Flush();
            this.Log.Close(); 
        }

        // Private

        private void AddBoilerPlate()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            this.AddInfoMsg(String.Format("WDAC Policy Wizard Version # {0}", versionInfo.FileVersion));
        }

    }

}


