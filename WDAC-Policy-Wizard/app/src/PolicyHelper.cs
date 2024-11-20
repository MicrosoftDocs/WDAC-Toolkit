using System;
using System.Collections.Generic;
using System.Data;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Text.RegularExpressions;
using Windows.Networking.NetworkOperators;

namespace WDAC_Wizard
{
    internal static class PolicyHelper
    {
        /*
         * 
         *  CIPolicy Object Helper Methods
         * 
         */

        // Counts of Allow, Deny, FileAttrib and Signer rules created to pipe into IDs
        static internal int cFileAllowRules = 0;
        static internal int cFileDenyRules = 0;
        static internal int cFileAttribRules = 0;
        static internal int cEKURules = 0;
        static internal int cFileExceptions = 0;
        static internal int cSigners = 0;

        // Signing Scenario Constants
        const int KMCISCN = 131;
        const int UMCISCN = 12;

        /// <summary>
        /// Creates an Allow rule based on a provided hash string
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        internal static SiPolicy CreateAllowHashRule(SiPolicy siPolicy, PolicyCustomRules customRule = null, Allow allow = null, bool isException = false)
        {
            if (allow != null)
            {
                Allow allowRule = new Allow();
                allowRule.Hash = allow.Hash;
                allowRule.FriendlyName = allow.FriendlyName;

                if (!isException)
                {
                    allowRule.ID = String.Format("ID_ALLOW_HASH_{0}", cFileAllowRules++);
                }
                else
                {
                    allowRule.ID = String.Format("ID_ALLOW_EX_{0}", cFileExceptions++);
                }

                // Add the deny rule to FileRules and FileRuleRef section with Windows Signing Scenario
                siPolicy = AddAllowRule(allowRule, siPolicy, customRule.SigningScenarioCheckStates, isException);
            }

            if (customRule != null)
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
        internal static SiPolicy CreateDenyHashRule(SiPolicy siPolicy, PolicyCustomRules customRule = null, Deny deny = null, bool isException = false)
        {
            if (deny != null)
            {
                Deny denyRule = new Deny();
                denyRule.Hash = deny.Hash;
                denyRule.FriendlyName = deny.FriendlyName;
                if (!isException)
                {
                    denyRule.ID = String.Format("ID_DENY_HASH_{0}", cFileDenyRules++);
                }
                else
                {
                    denyRule.ID = String.Format("ID_DENY_EX_{0}", cFileExceptions++);
                }

                // Add the deny rule to FileRules and FileRuleRef section with Windows Signing Scenario
                siPolicy = AddDenyRule(denyRule, siPolicy, customRule.SigningScenarioCheckStates, isException);
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
        internal static SiPolicy CreateHashFallbackRules(SiPolicy hashSiPolicy, SiPolicy siPolicy, PolicyCustomRules customRule)
        {
            foreach (object hashRule in hashSiPolicy.FileRules)
            {
                if (hashRule.GetType() == typeof(Allow))
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
        internal static SiPolicy CreateAllowPathRule(PolicyCustomRules customRule, SiPolicy siPolicy, bool isException = false)
        {
            Allow allowRule = new Allow();
            string path;

            if (!isException)
            {
                allowRule.ID = String.Format("ID_ALLOW_PATH_{0}", cFileAllowRules++);
                path = customRule.CustomValues.Path;
            }
            else
            {
                allowRule.ID = String.Format("ID_ALLOW_EX_{0}", cFileExceptions++);
                path = customRule.ReferenceFile;
            }

            // If using env variables, convert the path to a macro
            if (Properties.Settings.Default.useEnvVars)
            {
                allowRule.FilePath = Helper.GetEnvPath(path);
            }
            else
            {
                allowRule.FilePath = path;
            }

            allowRule.FriendlyName = String.Format("Allow by path: {0}", allowRule.FilePath);

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
        internal static SiPolicy CreateDenyPathRule(PolicyCustomRules customRule, SiPolicy siPolicy, bool isException = false)
        {
            Deny denyRule = new Deny();
            string path;

            if (!isException)
            {
                denyRule.ID = String.Format("ID_DENY_PATH_{0}", cFileDenyRules++);
                path = customRule.CustomValues.Path;
            }
            else
            {
                denyRule.ID = String.Format("ID_DENY_EX_{0}", cFileExceptions++);
                path = customRule.ReferenceFile;
            }

            // If using env variables, convert the path to a macro
            if (Properties.Settings.Default.useEnvVars)
            {
                denyRule.FilePath = Helper.GetEnvPath(path);
            }
            else
            {
                denyRule.FilePath = path;
            }

            denyRule.FriendlyName = String.Format("Deny by path: {0}", denyRule.FilePath);

            // Add the deny rule to FileRules and FileRuleRef section with Windows Signing Scenario
            siPolicy = AddDenyRule(denyRule, siPolicy, customRule.SigningScenarioCheckStates, isException);

            return siPolicy;
        }

        /// <summary>
        /// Handles creating and adding a File Publisher signer rule with user-defined custom values
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        internal static SiPolicy CreateFilePublisherRule(PolicyCustomRules customRule, SiPolicy siPolicy)
        {
            // Still need to run the PS cmd to generate the TBS hash for the signer(s)
            SiPolicy tempSiPolicy = PSCmdlets.CreatePolicyRuleFromPS(customRule);

            if (tempSiPolicy == null)
            {
                return siPolicy;
            }

            Signer[] signers = tempSiPolicy.Signers;
            if (signers == null || signers.Length == 0)
            {
                // Failed to create signer rules. Fallback to hash rules
                if (tempSiPolicy.FileRules.Length > 0)
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
            fileAttrib.ID = "ID_FILEATTRIB_F_" + cFileAttribRules++;

            // Set the fileattribute fields based on the checkbox states
            if (customRule.CheckboxCheckStates.checkBox4)
            {
                fileAttrib.MinimumFileVersion = customRule.CustomValues.MinVersion.Trim();
            }

            if (customRule.CheckboxCheckStates.checkBox4
                && customRule.CustomValues.MaxVersion != null
                && customRule.CustomValues.MaxVersion != "*")
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

            // Issue #210 - the WDAC policy compiler will complain that version info without one of product, filename, etc.
            // is not a valid rule. Add Filename="*" like the SignedVersion PS cmd does
            if((fileAttrib.MinimumFileVersion != null || fileAttrib.MaximumFileVersion !=null)
                && (fileAttrib.FileName == null && fileAttrib.ProductName == null))
            {
                fileAttrib.FileName = "*";
            }

            // Add FileAttrib references
            signers = AddFileAttribToSigners(fileAttrib, signers);
            siPolicy = AddSiPolicyFileAttrib(fileAttrib, siPolicy);

            // Add signer references
            siPolicy = AddSiPolicySigner(signers, siPolicy, customRule.Permission, customRule.SigningScenarioCheckStates);

            // Process exceptions
            if (customRule.ExceptionList.Count > 0)
            {
                if (customRule.Permission == PolicyCustomRules.RulePermission.Allow)
                {
                    // Create except deny rules to add to allowed signers
                    ExceptDenyRule[] exceptDenyRules = CreateExceptDenyRules(customRule.ExceptionList, siPolicy);
                    siPolicy = AddExceptionsToAllowSigners(exceptDenyRules, siPolicy, customRule.SigningScenarioCheckStates);
                }
                else
                {
                    // Create except Allowrules
                    ExceptAllowRule[] exceptAllowRules = CreateExceptAllowRules(customRule.ExceptionList, siPolicy);
                    siPolicy = AddExceptionsToDeniedSigners(exceptAllowRules, siPolicy, customRule.SigningScenarioCheckStates);
                }
            }
            return siPolicy;
        }

        /// <summary>
        /// Creates an Allow File Attribute rule based on Original Filename, Description, Product and Internal filename
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        internal static SiPolicy CreateAllowFileAttributeRule(PolicyCustomRules customRule, SiPolicy siPolicy, bool isException = false)
        {
            Allow allowRule = new Allow();
            string friendlyName = "Allow files based on file attributes: ";

            // Add only the checked attributes
            // Original filename
            if (customRule.CheckboxCheckStates.checkBox0)
            {
                allowRule.FileName = customRule.CustomValues.FileName;
                friendlyName += allowRule.FileName + " and ";
            }

            // Description
            if (customRule.CheckboxCheckStates.checkBox1)
            {
                allowRule.FileDescription = customRule.CustomValues.Description;
                friendlyName += allowRule.FileDescription + " and ";
            }

            // Product name
            if (customRule.CheckboxCheckStates.checkBox2)
            {
                allowRule.ProductName = customRule.CustomValues.ProductName;
                friendlyName += allowRule.ProductName + " and ";
            }

            // Internal name
            if (customRule.CheckboxCheckStates.checkBox3)
            {
                allowRule.InternalName = customRule.CustomValues.InternalName;
                friendlyName += allowRule.InternalName + " and ";
            }

            // Min Version
            if (customRule.CheckboxCheckStates.checkBox4 && isException)
            {
                allowRule.MinimumFileVersion = customRule.CustomValues.MinVersion;
                friendlyName += allowRule.MinimumFileVersion + " and ";
            }

            allowRule.FriendlyName = friendlyName.Substring(0, friendlyName.Length - 5);

            if (!isException)
            {
                allowRule.ID = String.Format("ID_ALLOW_A_{0}", cFileAllowRules++);
            }
            else
            {
                allowRule.ID = String.Format("ID_ALLOW_EX_{0}", cFileExceptions++);
            }

            // Issue #210 - the WDAC policy compiler will complain that version info without one of product, filename, etc.
            // is not a valid rule. Add Filename="*" like the SignedVersion PS cmd does
            if (allowRule.MinimumFileVersion != null
                && (allowRule.FileName == null
                    && allowRule.InternalName == null
                    && allowRule.ProductName == null
                    && allowRule.FileDescription == null))
            {
                allowRule.FileName = "*";
            }

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
        internal static SiPolicy CreateDenyFileAttributeRule(PolicyCustomRules customRule, SiPolicy siPolicy, bool isException = false)
        {
            Deny denyRule = new Deny();
            string friendlyName = "Deny files based on file attributes: ";

            // Add only the checked attributes
            // Original filename
            if (customRule.CheckboxCheckStates.checkBox0)
            {
                denyRule.FileName = customRule.CustomValues.FileName;
                friendlyName += denyRule.FileName + " and ";
            }

            // File description
            if (customRule.CheckboxCheckStates.checkBox1)
            {
                denyRule.FileDescription = customRule.CustomValues.Description;
                friendlyName += denyRule.FileDescription + " and ";
            }

            // Product name
            if (customRule.CheckboxCheckStates.checkBox2)
            {
                denyRule.ProductName = customRule.CustomValues.ProductName;
                friendlyName += denyRule.ProductName + " and ";
            }

            // Internal name
            if (customRule.CheckboxCheckStates.checkBox3)
            {
                denyRule.InternalName = customRule.CustomValues.InternalName;
                friendlyName += denyRule.InternalName + " and ";
            }

            // Min Version
            if (customRule.CheckboxCheckStates.checkBox4 && isException)
            {
                denyRule.MinimumFileVersion = customRule.CustomValues.MinVersion;
                friendlyName += denyRule.MinimumFileVersion + " and ";
            }

            denyRule.FriendlyName = friendlyName.Substring(0, friendlyName.Length - 5);

            if (!isException)
            {
                denyRule.ID = String.Format("ID_DENY_D_{0}", cFileDenyRules++);
            }
            else
            {
                denyRule.ID = String.Format("ID_DENY_EX_{0}", cFileExceptions++);
            }

            // Issue #210 - the WDAC policy compiler will complain that version info without one of product, filename, etc.
            // is not a valid rule. Add Filename="*" like the SignedVersion PS cmd does
            if (denyRule.MinimumFileVersion != null
                && (denyRule.FileName == null
                    && denyRule.InternalName == null
                    && denyRule.ProductName == null
                    && denyRule.FileDescription == null))
            {
                denyRule.FileName = "*";
            }

            // Add the deny rule to FileRules and FileRuleRef section with Windows Signing Scenario
            siPolicy = AddDenyRule(denyRule, siPolicy, customRule.SigningScenarioCheckStates, isException);

            return siPolicy;
        }

        /// <summary>
        /// Creates a File Attribute rule from PE header defined data
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        internal static SiPolicy CreateNonCustomFileAttributeRule(PolicyCustomRules customRule, SiPolicy siPolicy)
        {
            // Format like a custom rule and pass into the custom rule handler methods
            customRule.CustomValues.FileName = customRule.FileInfo["OriginalFilename"];
            customRule.CustomValues.Description = customRule.FileInfo["FileDescription"];
            customRule.CustomValues.ProductName = customRule.FileInfo["ProductName"];
            customRule.CustomValues.InternalName = customRule.FileInfo["InternalName"];

            if (customRule.Permission == PolicyCustomRules.RulePermission.Allow)
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
        internal static SiPolicy CreateNonCustomFilePathRule(PolicyCustomRules customRule, SiPolicy siPolicy)
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
        internal static SiPolicy CreatePFNRule(PolicyCustomRules customRule, SiPolicy siPolicy)
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
                    allowRule.MinimumFileVersion = Properties.Resources.DefaultVersionString;
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
                    denyRule.PackageVersion = Properties.Resources.MaxVersion;
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
        internal static SiPolicy CreateComRule(SiPolicy siPolicy, List<COM> comObjectList)
        {
            if (comObjectList == null || comObjectList.Count == 0)
            {
                return siPolicy;
            }

            foreach (COM customComObj in comObjectList)
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
        internal static SiPolicy AddComRule(SiPolicy siPolicy, Setting comObject)
        {
            if (siPolicy.Settings != null)
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
        internal static SiPolicy AddAllowRule(Allow allowRule, SiPolicy siPolicy, PolicyCustomRules.SigningScenarioStates scenarioStates, bool isException = false)
        {
            // Copy and replace the FileRules obj[] in siPolicy
            // FileRules always initalized - no need to check if null
            object[] fileRulesCopy = siPolicy.FileRules;
            Array.Resize(ref fileRulesCopy, fileRulesCopy.Length + 1);
            fileRulesCopy[fileRulesCopy.Length - 1] = allowRule;
            siPolicy.FileRules = fileRulesCopy;

            // Add the filerule reference to signing scenario
            // Skip if this is an exception
            if (!isException)
            {
                siPolicy = AddFileRulesRef(allowRule.ID, siPolicy, scenarioStates);
            }

            return siPolicy;
        }

        /// <summary>
        /// Handles adding the new Allow Rule object to the provided siPolicy
        /// </summary>
        /// <param name="allowRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        internal static SiPolicy AddDenyRule(Deny denyRule, SiPolicy siPolicy, PolicyCustomRules.SigningScenarioStates scenarioStates, bool isException = false)
        {
            // Copy and replace the FileRules obj[] in siPolicy
            // FileRules always initalized - no need to check if null
            object[] fileRulesCopy = siPolicy.FileRules;
            Array.Resize(ref fileRulesCopy, fileRulesCopy.Length + 1);
            fileRulesCopy[fileRulesCopy.Length - 1] = denyRule;
            siPolicy.FileRules = fileRulesCopy;

            // Add the filerule reference
            if (!isException)
            {
                siPolicy = AddFileRulesRef(denyRule.ID, siPolicy, scenarioStates);
            }

            return siPolicy;
        }

        /// <summary>
        /// Adds an SiPolicy FileAttrib to the policy
        /// </summary>
        /// <param name="fileAttrib"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        internal static SiPolicy AddSiPolicyFileAttrib(FileAttrib fileAttrib, SiPolicy siPolicy)
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
        internal static SiPolicy AddSiPolicyEKUs(EKU eku, SiPolicy siPolicy)
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
        internal static Signer[] AddFileAttribToSigners(FileAttrib fileAttrib, Signer[] signers)
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
        internal static Signer[] SetSignersPublishers(Signer[] signers, PolicyCustomRules customRule)
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
        internal static Signer[] SetSignersEKUs(Signer[] signers, EKU eku)
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
        internal static SiPolicy AddFileRulesRef(string ruleID, SiPolicy siPolicy, PolicyCustomRules.SigningScenarioStates scenarioStates)
        {
            // Copy and replace the FileRulesRef section to add to Signing Scenarios
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

            return siPolicy;
        }

        /// <summary>
        /// Adds an Allow Signer or Deny Signer to the siPolicy signing scenario
        /// </summary>
        /// <param name="signers"></param>
        /// <param name="siPolicy"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        internal static SiPolicy AddSiPolicySigner(Signer[] signers, SiPolicy siPolicy, PolicyCustomRules.RulePermission action,
            PolicyCustomRules.SigningScenarioStates scenarioStates)
        {
            // Copy the SiPolicy signer object and add the signer param to the field
            Signer[] signersCopy = siPolicy.Signers;
            Array.Resize(ref signersCopy, signersCopy.Length + signers.Length);
            for (int i = 0; i < signers.Length; i++)
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
                    for (int j = 0; j < siPolicy.SigningScenarios.Length; j++)
                    {
                        if (siPolicy.SigningScenarios[j].Value == KMCISCN && scenarioStates.kmciEnabled) // Kernel mode (131)
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
        internal static ExceptAllowRule[] CreateExceptAllowRules(List<PolicyCustomRules> exceptionsList, SiPolicy siPolicy)
        {
            // 1. Create an Allow Rule of the requested type (file attibutes, path, hash)
            // 2. Add the Allow Rule to the SiPolicy.FileRules section
            // 3. Return the ExceptAllowRule[] which contain the IDs of the Allow rules to add to the Signer.Exceptions

            ExceptAllowRule[] exceptAllowRules = new ExceptAllowRule[exceptionsList.Count];
            int i = 0;

            foreach (var exceptAllowRule in exceptionsList)
            {
                switch (exceptAllowRule.Type)
                {
                    case PolicyCustomRules.RuleType.FileAttributes:
                        {
                            exceptAllowRules[i] = new ExceptAllowRule();
                            exceptAllowRules[i++].AllowRuleID = String.Format("ID_ALLOW_EX_{0}", cFileExceptions); // Don't increment cFileExceptions
                            siPolicy = CreateAllowFileAttributeRule(exceptAllowRule, siPolicy, true);
                            break;
                        }

                    case PolicyCustomRules.RuleType.FilePath:
                    case PolicyCustomRules.RuleType.FolderPath:
                        {
                            exceptAllowRules[i] = new ExceptAllowRule();
                            exceptAllowRules[i++].AllowRuleID = String.Format("ID_ALLOW_EX_{0}", cFileExceptions); // Don't increment cFileExceptions
                            siPolicy = CreateAllowPathRule(exceptAllowRule, siPolicy, true);
                            break;
                        }

                    case PolicyCustomRules.RuleType.Hash:
                        {
                            object[] hashRules = PSCmdlets.CreatePolicyRuleFromPS(exceptAllowRule).FileRules;
                            Array.Resize(ref exceptAllowRules, exceptAllowRules.Length + hashRules.Length - 1); // re-size the exceptDenyRules id to accomodate (likely) 3 more hash rules
                            foreach (object hashRule in hashRules)
                            {
                                exceptAllowRules[i] = new ExceptAllowRule();
                                exceptAllowRules[i++].AllowRuleID = String.Format("ID_ALLOW_EX_{0}", cFileExceptions); // Don't increment cFileExceptions
                                siPolicy = CreateAllowHashRule(siPolicy, exceptAllowRule, (Allow)hashRule, true);
                            }
                            break;
                        }
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
        internal static ExceptDenyRule[] CreateExceptDenyRules(List<PolicyCustomRules> exceptionsList, SiPolicy siPolicy)
        {
            // 1. Create a Deny Rule of the requested type (file attibutes, path, hash)
            // 2. Add the Deny Rule to the SiPolicy.FileRules section
            // 3. Return the ExceptDenyRule[] which contain the IDs of the Allow rules to add to the Signer.Exceptions

            ExceptDenyRule[] exceptDenyRules = new ExceptDenyRule[exceptionsList.Count];
            int i = 0;

            foreach (var exceptDenyRule in exceptionsList)
            {
                switch (exceptDenyRule.Type)
                {
                    case PolicyCustomRules.RuleType.FileAttributes:
                        {
                            exceptDenyRules[i] = new ExceptDenyRule();
                            exceptDenyRules[i++].DenyRuleID = String.Format("ID_DENY_EX_{0}", cFileExceptions); // Don't increment cFileExceptions
                            siPolicy = CreateDenyFileAttributeRule(exceptDenyRule, siPolicy, true);
                            break;
                        }

                    case PolicyCustomRules.RuleType.FilePath:
                    case PolicyCustomRules.RuleType.FolderPath:
                        {
                            exceptDenyRules[i] = new ExceptDenyRule();
                            exceptDenyRules[i++].DenyRuleID = String.Format("ID_DENY_EX_{0}", cFileExceptions); // Don't increment cFileExceptions
                            siPolicy = CreateDenyPathRule(exceptDenyRule, siPolicy, true);
                            break;
                        }

                    case PolicyCustomRules.RuleType.Hash:
                        {
                            object[] hashRules = PSCmdlets.CreatePolicyRuleFromPS(exceptDenyRule).FileRules;
                            Array.Resize(ref exceptDenyRules, exceptDenyRules.Length + hashRules.Length - 1); // re-size the exceptDenyRules id to accomodate (likely) 3 more hash rules
                            foreach (object hashRule in hashRules)
                            {
                                exceptDenyRules[i] = new ExceptDenyRule();
                                exceptDenyRules[i++].DenyRuleID = String.Format("ID_DENY_EX_{0}", cFileExceptions); // Don't increment cFileExceptions
                                siPolicy = CreateDenyHashRule(siPolicy, exceptDenyRule, (Deny)hashRule, true);
                            }
                            break;
                        }
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
        internal static SiPolicy AddExceptionsToAllowSigners(ExceptDenyRule[] exceptDenyRules, SiPolicy siPolicy, PolicyCustomRules.SigningScenarioStates scenarioStates)
        {
            // Add ExceptDenyRule IDs to signing scenarios
            for (int i = 0; i < siPolicy.SigningScenarios.Length; i++)
            {
                // Kernel mode signers (131)
                if (siPolicy.SigningScenarios[i].Value == KMCISCN && scenarioStates.kmciEnabled)
                {
                    for (int j = 0; j < siPolicy.SigningScenarios[i].ProductSigners.AllowedSigners.AllowedSigner.Length; j++)
                    {
                        siPolicy.SigningScenarios[i].ProductSigners.AllowedSigners.AllowedSigner[j].ExceptDenyRule = exceptDenyRules;
                    }
                }

                // User mode signers (12)
                if (siPolicy.SigningScenarios[i].Value == UMCISCN && scenarioStates.umciEnabled)
                {
                    for (int j = 0; j < siPolicy.SigningScenarios[i].ProductSigners.AllowedSigners.AllowedSigner.Length; j++)
                    {
                        siPolicy.SigningScenarios[i].ProductSigners.AllowedSigners.AllowedSigner[j].ExceptDenyRule = exceptDenyRules;
                    }
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
        internal static SiPolicy AddExceptionsToDeniedSigners(ExceptAllowRule[] exceptAllowRules, SiPolicy siPolicy, PolicyCustomRules.SigningScenarioStates scenarioStates)
        {
            // Add ExceptAllowRule IDs to signing scenarios
            for (int i = 0; i < siPolicy.SigningScenarios.Length; i++)
            {
                // Kernel mode signers (131)
                if (siPolicy.SigningScenarios[i].Value == KMCISCN && scenarioStates.kmciEnabled)
                {
                    for (int j = 0; j < siPolicy.SigningScenarios[i].ProductSigners.DeniedSigners.DeniedSigner.Length; j++)
                    {
                        siPolicy.SigningScenarios[i].ProductSigners.DeniedSigners.DeniedSigner[j].ExceptAllowRule = exceptAllowRules;
                    }
                }

                // User mode signers (12)
                if (siPolicy.SigningScenarios[i].Value == UMCISCN && scenarioStates.umciEnabled)
                {
                    for (int j = 0; j < siPolicy.SigningScenarios[i].ProductSigners.DeniedSigners.DeniedSigner.Length; j++)
                    {
                        siPolicy.SigningScenarios[i].ProductSigners.DeniedSigners.DeniedSigner[j].ExceptAllowRule = exceptAllowRules;
                    }
                }
            }

            return siPolicy;
        }

        /// <summary>
        /// Tries to add attributes like filename, publisher and version to non-custom publisher rules
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="signerSiPolicy"></param>
        internal static SiPolicy AddSignerRuleAttributes(PolicyCustomRules customRule, SiPolicy signerSiPolicy)
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

            // If none of the extra attributes are to be added and null exceptions, skip creating a FileAttrib rule
            if (customRule.CheckboxCheckStates.checkBox2        // Product name
                || customRule.CheckboxCheckStates.checkBox3     // Original filename
                || customRule.CheckboxCheckStates.checkBox4)    // Version
            {
                // Create new FileAttrib object to link to signers
                FileAttrib fileAttrib = CreateFileAttributeFromCustomRule(customRule);

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
            }
            else
            {
                // Add signer references
                siPolicy = AddSiPolicySigner(signers, siPolicy, customRule.Permission, customRule.SigningScenarioCheckStates);
            }

            // Process exceptions
            if (customRule.ExceptionList.Count > 0)
            {
                if (customRule.Permission == PolicyCustomRules.RulePermission.Allow)
                {
                    // Create except deny rules to add to allowed signers
                    ExceptDenyRule[] exceptDenyRules = CreateExceptDenyRules(customRule.ExceptionList, siPolicy);
                    siPolicy = AddExceptionsToAllowSigners(exceptDenyRules, siPolicy, customRule.SigningScenarioCheckStates);
                }
                else
                {
                    // Create except Allowrules
                    ExceptAllowRule[] exceptAllowRules = CreateExceptAllowRules(customRule.ExceptionList, siPolicy);
                    siPolicy = AddExceptionsToDeniedSigners(exceptAllowRules, siPolicy, customRule.SigningScenarioCheckStates);
                }
            }

            return siPolicy;
        }

        /// <summary>
        /// Creates an object from the FileAttrib class
        /// </summary>
        /// <param name="customRule"></param>
        /// <returns></returns>
        internal static FileAttrib CreateFileAttributeFromCustomRule(PolicyCustomRules customRule)
        {
            FileAttrib fileAttrib = new FileAttrib();
            fileAttrib.ID = "ID_FILEATTRIB_F_" + cFileAttribRules++;

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

            // Issue #210 - the WDAC policy compiler will complain that version info without one of product, filename, etc.
            // is not a valid rule. Add Filename="*" like the SignedVersion PS cmd does
            if ((fileAttrib.MinimumFileVersion != null || fileAttrib.MaximumFileVersion != null)
                && (fileAttrib.FileName == null && fileAttrib.ProductName == null))
            {
                fileAttrib.FileName = "*";
            }

            fileAttrib.FriendlyName = friendlyName.Substring(0, friendlyName.Length - 5); // remove trailing " and "
            return fileAttrib;
        }

        /// <summary>
        /// Returns a new SiPolicy Setting array with custom Policy Name and Policy ID
        /// </summary>
        /// <param name="siPolicy"></param>
        /// <param name="policyName"></param>
        /// <param name="policyId"></param>
        /// <returns></returns>
        internal static Setting[] SetPolicyInfo(Setting[] existingSettings, string policyName, string policyId)
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
                foreach (var setting in existingSettings)
                {
                    if (!(setting.Provider == "PolicyInfo"
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
        internal static SiPolicy AddSiPolicyCiSigner(Signer[] signers, SiPolicy siPolicy)
        {
            // Populate the ciSignerIds from the signers array
            CiSigner[] newCiSigners = new CiSigner[signers.Length];

            for (int i = 0; i < signers.Length; i++)
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

        /// <summary>
        /// Creates the PolicyInfo.Information.Name Setting
        /// </summary>
        /// <param name="policyName">Name string for the PolicyName Setting</param>
        /// <returns></returns>
        internal static Setting CreatePolicyNameSetting(string policyName)
        {
            Setting settingName = new Setting();
            settingName.Provider = "PolicyInfo";
            settingName.Key = "Information";
            settingName.ValueName = "Name";
            settingName.Value = new SettingValueType();
            settingName.Value.Item = String.IsNullOrEmpty(policyName) ? String.Empty : policyName;
            return settingName;
        }

        /// <summary>
        /// Creates the PolicyInfo.Information.ID Setting
        /// </summary>
        /// <param name="policyId"></param>
        /// <returns></returns>
        internal static Setting CreatePolicyIdSetting(string policyId)
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
        internal static bool PolicyHasRule(List<RuleType> siPolicyRuleOptions, OptionType targetRuleOption)
        {
            if (siPolicyRuleOptions == null)
            {
                return false;
            }

            // Check each rule option for the target rule option
            foreach (var ruleOption in siPolicyRuleOptions)
            {
                if (ruleOption.Item == targetRuleOption)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes duplicate Rule-Option values and returns only unique Rule-Option instances
        /// </summary>
        /// <param name="duplicateRuleTypes"></param>
        /// <returns></returns>
        internal static List<RuleType> DeDuplicateRuleOptions(List<RuleType> duplicateRuleTypes)
        {
            // Trivial case where null or only 1 rule-option
            if (duplicateRuleTypes == null || duplicateRuleTypes.Count < 2)
            {
                return duplicateRuleTypes;
            }

            List<RuleType> deduplicatedRuleOptions = new List<RuleType>();
            Dictionary<OptionType, int> keyValuePairs = new Dictionary<OptionType, int>();

            // Dedupe using Dictionary keys
            foreach (var ruleOption in duplicateRuleTypes)
            {
                if (keyValuePairs.ContainsKey(ruleOption.Item))
                {
                    continue;
                }
                keyValuePairs[ruleOption.Item] = 1;
            }

            // Set deduplicated rule-options to the dictionary keys to guarantee unique entries
            foreach (OptionType item in keyValuePairs.Keys)
            {
                RuleType ruleOption = new RuleType();
                ruleOption.Item = item;

                deduplicatedRuleOptions.Add(ruleOption);
            }

            return deduplicatedRuleOptions;

        }

        /// <summary>
        /// Updates all the non-unique Signer, FileAttrib and Allow/Deny rules generated by the cmdlets
        /// </summary>
        /// <param name="siPolicy"></param>
        /// <param name="ruleType"></param>
        /// <returns></returns>
        internal static SiPolicy RemapIDs(SiPolicy siPolicy, PolicyCustomRules.RuleType ruleType)
        {
            if(ruleType == PolicyCustomRules.RuleType.Hash)
            {
                // Remap the FileRules IDs to ensure they're unique
                siPolicy = FormatFileRuleIDs(siPolicy); 
            }

            if(ruleType == PolicyCustomRules.RuleType.Publisher)
            {
                // Remap the Signer Rule IDs (Signer, CiSigners)
                siPolicy = FormatSignerRuleIDs(siPolicy);
                siPolicy = FormatFileRuleIDs(siPolicy);
            }

            if(ruleType == PolicyCustomRules.RuleType.Certificate)
            {
                // Remap the Signer Rule IDs (Signer, CiSigners)
                siPolicy = FormatSignerRuleIDs(siPolicy);
            }
          
            return siPolicy;
        }

        /// <summary>
        /// Remaps the FileRule and Signer IDs
        /// </summary>
        /// <param name="customRulesPolicy"></param>
        /// <param name="existingPolicy"></param>
        /// <returns></returns>
        internal static SiPolicy RemapIDs(SiPolicy customRulesPolicy, SiPolicy existingPolicy)
        {
            // File Rules
            customRulesPolicy = FormatFileRuleIDsAgainstExisting(customRulesPolicy, existingPolicy);

            // Signer Rules
            customRulesPolicy = FormatSignerRuleIDsAgainstExsiting(customRulesPolicy, existingPolicy);

            return customRulesPolicy; 
        }

        /// <summary>
        /// Formats the Rule IDs in cases of long runs of "_0"s
        /// </summary>
        /// <param name="siPolicy">SiPolicy object with unformatted ruleIDs</param>
        /// <returns>SiPolicy object with formatted rule IDs</returns>
        internal static SiPolicy FormatFileRuleIDs(SiPolicy siPolicy)
        {
            // Break out if null siPolicy provided or FileRules and Signers are empty
            if (siPolicy == null 
                || siPolicy.FileRules == null
                ||siPolicy.FileRules.Length == 0)
            {
                return siPolicy;
            }

            // Dictionary containing the mapping between old and new ids
            Dictionary<string, string> idMapping = new Dictionary<string, string>();

            // Check all FileRules
            object[] fileRules = siPolicy.FileRules;

            // First pass: get all IDs --> existing IDs and non-conformant IDs --> idMapping
            foreach (var fileRule in fileRules)
            {
                // Allow Rules
                if (fileRule.GetType() == typeof(Allow))
                {
                    idMapping[((Allow)fileRule).ID] = $"ID_ALLOW_A_{cFileAllowRules++}";
                    ((Allow)fileRule).ID = idMapping[((Allow)fileRule).ID];
                }

                // Deny Rules
                else if (fileRule.GetType() == typeof(Deny))
                {
                    idMapping[((Deny)fileRule).ID] = $"ID_DENY_D_{cFileDenyRules++}";
                    ((Deny)fileRule).ID = idMapping[((Deny)fileRule).ID];
                }

                // File Attribute Rules
                else if (fileRule.GetType() == typeof(FileAttrib))
                {
                    idMapping[((FileAttrib)fileRule).ID] = $"ID_FILEATTRIB_F_{cFileAttribRules++}";
                    ((FileAttrib)fileRule).ID = idMapping[((FileAttrib)fileRule).ID];
                }
            }

            siPolicy.FileRules = fileRules;

            return UpdateSiPolicyFileRuleIDs(siPolicy, idMapping);
        }

        /// <summary>
        /// Checks all FileRule IDs in custom rule policy against an existing policy. 
        /// Creates unique IDs for custom rules if ID is found in existing policy
        /// </summary>
        /// <param name="customRulesPolicy"></param>
        /// <param name="existingPolicy"></param>
        /// <returns></returns>
        internal static SiPolicy FormatFileRuleIDsAgainstExisting(SiPolicy customRulesPolicy, SiPolicy existingPolicy)
        {
            // Break out if null siPolicy provided or FileRules and Signers are empty
            if (customRulesPolicy == null 
                || customRulesPolicy.FileRules == null
                || customRulesPolicy.FileRules.Length == 0)
            {
                return customRulesPolicy;
            }

            // Break early if existingPolicy does not have FileRules
            if (existingPolicy == null
                || existingPolicy.FileRules == null
                || existingPolicy.FileRules.Length == 0)
            {
                return customRulesPolicy;
            }

            // Dictionary containing the mapping between old and new ids
            Dictionary<string, string> idMapping = new Dictionary<string, string>();
            string newID = string.Empty;

            // Dictionary containing exitingPolicy's FileRule IDs
            Dictionary<string, string> existingFileRuleIDs = new Dictionary<string, string>();

            // Get existingPolicy FileRule IDs
            foreach(var fileRule in existingPolicy.FileRules)
            {
                // Allow Rules
                if (fileRule.GetType() == typeof(Allow))
                {
                    existingFileRuleIDs.Add(((Allow)fileRule).ID, ""); 
                }

                // Deny Rules
                else if (fileRule.GetType() == typeof(Deny))
                {
                    existingFileRuleIDs.Add(((Deny)fileRule).ID, "");
                }

                // File Attribute Rules
                else if (fileRule.GetType() == typeof(FileAttrib))
                {
                    existingFileRuleIDs.Add(((FileAttrib)fileRule).ID, "");
                }
            }

            // Check all custom FileRules IDs
            foreach (var fileRule in customRulesPolicy.FileRules)
            {
                // Allow Rules
                if (fileRule.GetType() == typeof(Allow))
                {
                    // If existingPolicy shares ID with customRulesPolicy, find a new value
                    // for existingPolicy FileRule

                    if (existingFileRuleIDs.ContainsKey(((Allow)fileRule).ID))
                    {
                        newID = $"ID_ALLOW_A_{cFileAllowRules++}";

                        // Keep updating newID by incrementing the count of AllowRules
                        // until existingFileRules does not share same ID
                        while (existingFileRuleIDs.ContainsKey(newID))
                        {
                            newID = $"ID_ALLOW_A_{cFileAllowRules++}";
                        }

                        // Old ID, new ID
                        idMapping.Add(((Allow)fileRule).ID, newID);
                        ((Allow)fileRule).ID = newID;
                    }
                }

                // Deny Rules
                if (fileRule.GetType() == typeof(Deny))
                {
                    // If existingPolicy shares ID with customRulesPolicy, find a new value
                    // for existingPolicy FileRule

                    if (existingFileRuleIDs.ContainsKey(((Deny)fileRule).ID))
                    {
                        newID = $"ID_DENY_D_{cFileDenyRules++}";

                        // Keep updating newID by incrementing the count of DenyRules
                        // until existingFileRules does not share same ID
                        while (existingFileRuleIDs.ContainsKey(newID))
                        {
                            newID = $"ID_DENY_D_{cFileDenyRules++}";
                        }

                        // Old ID, new ID
                        idMapping.Add(((Deny)fileRule).ID, newID);
                        ((Deny)fileRule).ID = newID;
                    }
                }

                // File Attribute Rules
                if (fileRule.GetType() == typeof(FileAttrib))
                {
                    // If existingPolicy shares ID with customRulesPolicy, find a new value
                    // for existingPolicy FileRule

                    if (existingFileRuleIDs.ContainsKey(((FileAttrib)fileRule).ID))
                    {
                        newID = $"ID_FILEATTRIB_F_{cFileAttribRules++}";

                        // Keep updating newID by incrementing the count of FileAttrib
                        // until existingFileRules does not share same ID
                        while (existingFileRuleIDs.ContainsKey(newID))
                        {
                            newID = $"ID_FILEATTRIB_F_{cFileAttribRules++}";
                        }

                        // Old ID, new ID
                        idMapping.Add(((FileAttrib)fileRule).ID, newID);
                        ((FileAttrib)fileRule).ID = newID;
                    }
                }
            }

            return UpdateSiPolicyFileRuleIDs(customRulesPolicy, idMapping);
        }

        /// <summary>
        /// Checks all FileRule IDs in custom rule policy against an existing policy. 
        /// Creates unique IDs for custom rules if ID is found in existing policy
        /// </summary>
        /// <param name="customRulesPolicy"></param>
        /// <param name="existingPolicy"></param>
        /// <returns></returns>
        internal static SiPolicy FormatSignerRuleIDsAgainstExsiting(SiPolicy customRulesPolicy, SiPolicy existingPolicy)
        {
            // Break out if null siPolicy provided or FileRules and Signers are empty
            if (customRulesPolicy == null
                || customRulesPolicy.FileRules == null
                || customRulesPolicy.FileRules.Length == 0)
            {
                return customRulesPolicy;
            }

            // Break early if existingPolicy does not have FileRules
            if (existingPolicy == null
                || existingPolicy.FileRules == null
                || existingPolicy.FileRules.Length == 0)
            {
                return customRulesPolicy;
            }

            // Dictionary containing the mapping between old and new ids
            Dictionary<string, string> idMapping = new Dictionary<string, string>();
            string newID = string.Empty;

            // Dictionary containing exitingPolicy's FileRule IDs
            Dictionary<string, string> existingSignerIDs = new Dictionary<string, string>();

            // Get all exiting signer IDs
            foreach (var signer in existingPolicy.Signers)
            {
                existingSignerIDs.Add(signer.ID,"");
            }

            // Check all custom Signer IDs
            foreach (var signer in customRulesPolicy.Signers)
            {
                // If existingPolicy shares ID with customRulesPolicy, find a new value
                // for existingPolicy FileRule

                if (existingSignerIDs.ContainsKey(signer.ID))
                {
                    newID = $"ID_SIGNER_S_{cSigners++}";

                    // Keep updating newID by incrementing the count of AllowRules
                    // until existingFileRules does not share same ID
                    while (existingSignerIDs.ContainsKey(newID))
                    {
                        newID = $"ID_SIGNER_S_{cSigners++}";
                    }

                    // Old ID, new ID
                    idMapping.Add(signer.ID, newID);
                    signer.ID = newID; 
                }
            }

            return UpdateSiPolicySignerIDs(customRulesPolicy, idMapping);            
        }

        /// <summary>
        /// Formats the Rule IDs in cases of long runs of "_0"s
        /// </summary>
        /// <param name="siPolicy">SiPolicy object with unformatted ruleIDs</param>
        /// <returns>SiPolicy object with formatted rule IDs</returns>
        internal static SiPolicy FormatSignerRuleIDs(SiPolicy siPolicy)
        {
            // Break out if null siPolicy provided or FileRules and Signers are empty
            if (siPolicy == null || siPolicy.Signers == null)
            {
                return siPolicy;
            }

            // Dictionary containing the mapping between old and new ids
            Dictionary<string, string> idMapping = new Dictionary<string, string>();

            // Check all Signers
            Signer[] signers = siPolicy.Signers;

            // Remap all existing IDs to conformant IDs to ensure uniqueness
            // Ex. generic ID_SIGNER_S_1 --> ID_SIGNER_S_234
            foreach (var signer in signers)
            {
                idMapping.Add(signer.ID, $"ID_SIGNER_S_{cSigners++}");
                signer.ID = idMapping[signer.ID];
            }

            siPolicy.Signers = signers;

            return UpdateSiPolicySignerIDs(siPolicy, idMapping);
        }

        /// <summary>
        /// Handles updating Signer ID in Signing Scenarios, CiSigners, UpdateSigners, SupplementalSigners, etc.
        /// </summary>
        /// <param name="siPolicy"></param>
        /// <param name="idMapping"></param>
        /// <returns></returns>
        internal static SiPolicy UpdateSiPolicySignerIDs(SiPolicy siPolicy, Dictionary<string, string> idMapping)
        {
            // Signing Scenarios Product Signers
            SigningScenario[] tempSigningScenario = siPolicy.SigningScenarios;
            foreach (SigningScenario signingScn in tempSigningScenario)
            {
                // Allowed Signers
                if (signingScn.ProductSigners.AllowedSigners != null)
                {
                    for (int i = 0; i < signingScn.ProductSigners.AllowedSigners.AllowedSigner.Length; i++)
                    {
                        string id = signingScn.ProductSigners.AllowedSigners.AllowedSigner[i].SignerId;

                        // Remap IDs in Allowed Signer section
                        if (idMapping.ContainsKey(id))
                        {
                            signingScn.ProductSigners.AllowedSigners.AllowedSigner[i].SignerId = idMapping[id];
                            Logger.Log.AddInfoMsg($"SignerID alread exists. Replacing SignerID: {id} with {idMapping[id]}");
                        }
                    }
                }

                // Denied Signers
                if (signingScn.ProductSigners.DeniedSigners != null)
                {
                    for (int i = 0; i < signingScn.ProductSigners.DeniedSigners.DeniedSigner.Length; i++)
                    {
                        string id = signingScn.ProductSigners.DeniedSigners.DeniedSigner[i].SignerId;

                        // Remap IDs in Denied Signer section
                        if (idMapping.ContainsKey(id))
                        {
                            signingScn.ProductSigners.DeniedSigners.DeniedSigner[i].SignerId = idMapping[id];
                            Logger.Log.AddInfoMsg($"SignerID alread exists. Replacing SignerID: {id} with {idMapping[id]}");
                        }
                    }
                }
            }
            siPolicy.SigningScenarios = tempSigningScenario;

            // CiSigners
            CiSigner[] tempCiSigners = siPolicy.CiSigners;
            if (tempCiSigners != null)
            {
                for (int i = 0; i < tempCiSigners.Length; i++)
                {
                    string id = tempCiSigners[i].SignerId;

                    // Remap IDs in Ci Signer section
                    if (idMapping.ContainsKey(id))
                    {
                        tempCiSigners[i].SignerId = idMapping[id];
                        Logger.Log.AddInfoMsg($"SignerID alread exists. Replacing SignerID: {id} with {idMapping[id]}");
                    }
                }
                siPolicy.CiSigners = tempCiSigners;
            }

            // UpdateSigners
            UpdatePolicySigner[] tempPolicySigners = siPolicy.UpdatePolicySigners;
            if (tempPolicySigners != null)
            {
                for (int i = 0; i < tempPolicySigners.Length; i++)
                {
                    string id = tempPolicySigners[i].SignerId;

                    // Remap IDs in Ci Signer section
                    if (idMapping.ContainsKey(id))
                    {
                        tempPolicySigners[i].SignerId = idMapping[id];
                        Logger.Log.AddInfoMsg($"SignerID alread exists. Replacing SignerID: {id} with {idMapping[id]}");
                    }
                }
                siPolicy.UpdatePolicySigners = tempPolicySigners;
            }

            // Supplemental Signers
            SupplementalPolicySigner[] tempSupplementalSigners = siPolicy.SupplementalPolicySigners;
            if (tempSupplementalSigners != null)
            {
                for (int i = 0; i < tempSupplementalSigners.Length; i++)
                {
                    string id = tempSupplementalSigners[i].SignerId;

                    // Remap IDs in Ci Signer section
                    if (idMapping.ContainsKey(id))
                    {
                        tempSupplementalSigners[i].SignerId = idMapping[id];
                        Logger.Log.AddInfoMsg($"SignerID alread exists. Replacing SignerID: {id} with {idMapping[id]}");
                    }
                }
                siPolicy.SupplementalPolicySigners = tempSupplementalSigners;
            }

            return siPolicy;
        }

        /// <summary>
        /// Handles updating FileRule IDs in SigningScenarios, Signers, etc. 
        /// </summary>
        /// <param name="siPolicy"></param>
        /// <param name="idMapping"></param>
        /// <returns></returns>
        internal static SiPolicy UpdateSiPolicyFileRuleIDs(SiPolicy siPolicy, Dictionary<string, string> idMapping)
        {
            // Update References in Signing Scenarios
            SigningScenario[] tempSigningScenario = siPolicy.SigningScenarios;
            foreach (SigningScenario signingScn in tempSigningScenario)
            {
                if (signingScn.ProductSigners.FileRulesRef != null)
                {
                    for (int i = 0; i < signingScn.ProductSigners.FileRulesRef.FileRuleRef.Length; i++)
                    {
                        string id = signingScn.ProductSigners.FileRulesRef.FileRuleRef[i].RuleID;

                        // Remap IDs in FileRuleRef section
                        if (idMapping.ContainsKey(id))
                        {
                            signingScn.ProductSigners.FileRulesRef.FileRuleRef[i].RuleID = idMapping[id];
                            Logger.Log.AddInfoMsg($"Replacing RuleID: {id} with {idMapping[id]}");
                        }
                    }

                    // Check AllowedSigner.ExceptDenyRules that must be remapped
                    // Root cause of Issue #384
                    if (signingScn.ProductSigners.AllowedSigners != null)
                    {
                        for (int i = 0; i < signingScn.ProductSigners.AllowedSigners.AllowedSigner.Length; i++)
                        {
                            if (signingScn.ProductSigners.AllowedSigners.AllowedSigner[i].ExceptDenyRule == null)
                                continue;

                            for (int j = 0; j < signingScn.ProductSigners.AllowedSigners.AllowedSigner[i].ExceptDenyRule.Length; j++)
                            {
                                string id = signingScn.ProductSigners.AllowedSigners.AllowedSigner[i].ExceptDenyRule[j].DenyRuleID;

                                // Remap IDs in DenyRuleID section
                                if (idMapping.ContainsKey(id))
                                {
                                    signingScn.ProductSigners.AllowedSigners.AllowedSigner[i].ExceptDenyRule[j].DenyRuleID = idMapping[id];
                                    Logger.Log.AddInfoMsg($"Replacing RuleID: {id} with {idMapping[id]}");
                                }
                            }
                        }
                    }

                    // Check DeniedSigner.ExceptAllowRules that must be remapped
                    if (signingScn.ProductSigners.DeniedSigners != null)
                    {
                        for (int i = 0; i < signingScn.ProductSigners.DeniedSigners.DeniedSigner.Length; i++)
                        {
                            if (signingScn.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule == null)
                                continue;

                            for (int j = 0; j < signingScn.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule.Length; j++)
                            {
                                string id = signingScn.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule[j].AllowRuleID;

                                // Remap IDs in DenyRuleID section
                                if (idMapping.ContainsKey(id))
                                {
                                    signingScn.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule[j].AllowRuleID = idMapping[id];
                                    Logger.Log.AddInfoMsg($"Replacing RuleID: {id} with {idMapping[id]}");
                                }
                            }
                        }
                    }
                }
            }

            siPolicy.SigningScenarios = tempSigningScenario;

            // Check all FileAttribRef in Signers
            Signer[] tempSigners = siPolicy.Signers;

            if (tempSigners != null)
            {
                foreach (Signer tempSigner in tempSigners)
                {
                    if (tempSigner.FileAttribRef == null)
                        continue;

                    foreach (FileAttribRef fileAttribRef in tempSigner.FileAttribRef)
                    {
                        string id = fileAttribRef.RuleID;

                        // Remap IDs in FileRuleRef section
                        if (idMapping.ContainsKey(id))
                        {
                            fileAttribRef.RuleID = idMapping[id];
                            Logger.Log.AddInfoMsg($"Replacing RuleID: {id} with {idMapping[id]}");
                        }
                    }
                }
                siPolicy.Signers = tempSigners;
            }


            return siPolicy;
        }


        /// <summary>
        /// Handles the merge operation between two SiPolicy objects. Merges all FileRules, Signers, SigningScenarios and references
        /// </summary>
        /// <param name="tempPolicy"></param>
        /// <param name="resultantPolicy"></param>
        /// <param name="ruleType"></param>
        /// <returns></returns>
        public static SiPolicy MergePolicies(SiPolicy tempPolicy, SiPolicy resultantPolicy)
        {
            // Handle nulls
            if(resultantPolicy == null)
            {
                return tempPolicy;
            }

            if(tempPolicy == null)
            {
                return resultantPolicy;
            }

            // Handle Signing Scenario (AllowedSigners, DeniedSigners and FileRuleRefs)
            resultantPolicy.SigningScenarios = MergeSigningScenario(tempPolicy.SigningScenarios, resultantPolicy.SigningScenarios);

            // Handle Signers
            resultantPolicy.Signers = MergeSigners(tempPolicy.Signers, resultantPolicy.Signers);   

            // Handle File Rules
            resultantPolicy.FileRules = MergeFileRules(tempPolicy.FileRules, resultantPolicy.FileRules);

            // Handle CiSigners
            if (tempPolicy.CiSigners != null && tempPolicy.CiSigners.Length > 0)
            {
                resultantPolicy.CiSigners = MergeCiSigners(tempPolicy.CiSigners, resultantPolicy.CiSigners);
            }

            // Handle EKUs
            if(tempPolicy.EKUs != null && tempPolicy.EKUs.Length > 0)
            {
                resultantPolicy.EKUs = MergeEKUs(tempPolicy.EKUs, resultantPolicy.EKUs);
            }
            
            return resultantPolicy;
        }

        /// <summary>
        /// Handles the merge operation between a new SigningScenario and an existing SigningScenario struct
        /// </summary>
        /// <param name="newSigningScenarios"></param>
        /// <param name="resultSigningScenarios"></param>
        /// <returns></returns>
        static SigningScenario[] MergeSigningScenario(SigningScenario[] newSigningScenarios, SigningScenario[] resultSigningScenarios)
        {
            // Check each SigningScenario's ProductSigners and copies references as needed
            for (int i = 0; i < newSigningScenarios.Length; i++)
            {
                for (int j = 0; j < resultSigningScenarios.Length; j++)
                { 
                    // Kernel mode (131)
                    if (newSigningScenarios[i].Value == KMCISCN
                        && resultSigningScenarios[j].Value == KMCISCN)
                    {
                        resultSigningScenarios[j].ProductSigners = MergeProductSigners(newSigningScenarios[i].ProductSigners,
                                                                                                   resultSigningScenarios[j].ProductSigners);
                    }


                    // User mode (12)
                    if (newSigningScenarios[i].Value == UMCISCN
                        && resultSigningScenarios[j].Value == UMCISCN)
                    {
                        resultSigningScenarios[j].ProductSigners = MergeProductSigners(newSigningScenarios[i].ProductSigners,
                                                                                                   resultSigningScenarios[j].ProductSigners);
                    }
                }
            }

            return resultSigningScenarios;
        }


        /// <summary>
        /// Handles the merge operation between a new ProductSigner and an existing ProductSigner struct
        /// </summary>
        /// <param name="newProductSigners"></param>
        /// <param name="resultProductSigners"></param>
        /// <returns></returns>
        static ProductSigners MergeProductSigners(ProductSigners newProductSigners, ProductSigners resultProductSigners)
        {
            // Short circuit if nothing from the new sipolicy
            if (newProductSigners == null)
            {
                return resultProductSigners;
            }

            // Allowed Signers
            if (newProductSigners.AllowedSigners != null)
            {
                // Short Circuit if current AllowedSigner is null
                if (resultProductSigners.AllowedSigners == null)
                {
                    resultProductSigners.AllowedSigners = newProductSigners.AllowedSigners;
                }
                else // new and existing AllowedSigners
                {
                    int copySize = newProductSigners.AllowedSigners.AllowedSigner.Length
                                                    + resultProductSigners.AllowedSigners.AllowedSigner.Length;
                    AllowedSigner[] allowedSignersCopy = new AllowedSigner[copySize];

                    int newAllowedSignersLen = newProductSigners.AllowedSigners.AllowedSigner.Length;

                    // New AllowedSigners
                    for (int i = 0; i < newAllowedSignersLen; i++)
                    {
                        allowedSignersCopy[i] = newProductSigners.AllowedSigners.AllowedSigner[i];
                    }

                    // Existing AllowedSigners
                    for (int i = 0; i < resultProductSigners.AllowedSigners.AllowedSigner.Length; i++)
                    {
                        // Offset the index to length of new Prod signers to not overwrite entries
                        allowedSignersCopy[i + newAllowedSignersLen] = resultProductSigners.AllowedSigners.AllowedSigner[i];
                    }


                    resultProductSigners.AllowedSigners.AllowedSigner = allowedSignersCopy;
                }

            }

            // Denied Signers
            if (newProductSigners.DeniedSigners != null)
            {
                // Short Circuit if current DeniedSigners is null
                if (resultProductSigners.DeniedSigners == null)
                {
                    resultProductSigners.DeniedSigners = newProductSigners.DeniedSigners;
                }
                else // new and existing DeniedSigners
                {
                    int copySize = newProductSigners.DeniedSigners.DeniedSigner.Length
                                                    + resultProductSigners.DeniedSigners.DeniedSigner.Length;
                    DeniedSigner[] deniedSignersCopy = new DeniedSigner[copySize];

                    int newDeniedSignersLen = newProductSigners.DeniedSigners.DeniedSigner.Length;

                    // New DeniedSigners
                    for (int i = 0; i < newDeniedSignersLen; i++)
                    {
                        deniedSignersCopy[i] = newProductSigners.DeniedSigners.DeniedSigner[i];
                    }

                    // Existing AllowedSigners
                    for (int i = 0; i < resultProductSigners.DeniedSigners.DeniedSigner.Length; i++)
                    {
                        // Offset the index to length of new Prod signers to not overwrite entries
                        deniedSignersCopy[i + newDeniedSignersLen] = resultProductSigners.DeniedSigners.DeniedSigner[i];
                    }

                    resultProductSigners.DeniedSigners.DeniedSigner = deniedSignersCopy;
                }
            }
            // FileRulesRef
            if (newProductSigners.FileRulesRef != null)
            {
                // Short Circuit if current DeniedSigners is null
                if (resultProductSigners.FileRulesRef == null)
                {
                    resultProductSigners.FileRulesRef = newProductSigners.FileRulesRef;
                }
                else // new and existing DeniedSigners
                {
                    int copySize = newProductSigners.FileRulesRef.FileRuleRef.Length
                                                    + resultProductSigners.FileRulesRef.FileRuleRef.Length;
                    FileRuleRef[] fileRuleRefCopy = new FileRuleRef[copySize];

                    int newFileRuleRefLen = newProductSigners.FileRulesRef.FileRuleRef.Length;

                    // New DeniedSigners
                    for (int i = 0; i < newFileRuleRefLen; i++)
                    {
                        fileRuleRefCopy[i] = newProductSigners.FileRulesRef.FileRuleRef[i];
                    }

                    // Existing AllowedSigners
                    for (int i = 0; i < resultProductSigners.FileRulesRef.FileRuleRef.Length; i++)
                    {
                        // Offset the index to length of new Prod signers to not overwrite entries
                        fileRuleRefCopy[i + newFileRuleRefLen] = resultProductSigners.FileRulesRef.FileRuleRef[i];
                    }


                    resultProductSigners.FileRulesRef.FileRuleRef = fileRuleRefCopy;
                }
            }

            return resultProductSigners;
        }


        /// <summary>
        /// Handles the merge operation between a new Signer[] and an existing Signer[] struct
        /// </summary>
        /// <param name="newProductSigners"></param>
        /// <param name="resultProductSigners"></param>
        /// <returns></returns>
        static Signer[] MergeSigners(Signer[] newSigners, Signer[] resultSigners)
        {
            // Short circuit if nothing from the new sipolicy
            if (newSigners == null || newSigners.Length == 0)
            {
                return resultSigners;
            }

            // Return newSigners if resultSigners is empty/null
            if (resultSigners == null || resultSigners.Length == 0)
            {
                return newSigners;
            }
   
            int copySize = newSigners.Length + resultSigners.Length;
            Signer[] signersCopy = new Signer[copySize];
                                    
            // New DeniedSigners
            for (int i = 0; i < newSigners.Length; i++)
            {
                signersCopy[i] = newSigners[i];
            }
            
            // Existing AllowedSigners
            for (int i = 0; i < resultSigners.Length; i++)
            {
                // Offset the index to length of new Prod signers to not overwrite entries
                signersCopy[i + newSigners.Length] = resultSigners[i];
            }
            
            return signersCopy;
        }


        /// <summary>
        /// Handles the merge operation between a new Signer[] and an existing Signer[] struct
        /// </summary>
        /// <param name="newProductSigners"></param>
        /// <param name="resultProductSigners"></param>
        /// <returns></returns>
        static Object[] MergeFileRules(Object[] newFileRules, Object[] resultFileRules)
        {
            // Short circuit if nothing from the new sipolicy
            if (newFileRules == null || newFileRules.Length == 0)
            {
                return resultFileRules;
            }

            // Return newFileRules if resultFileRules is empty/null
            if (resultFileRules == null || resultFileRules.Length == 0)
            {
                return newFileRules;
            }

            int copySize = newFileRules.Length + resultFileRules.Length;
            Object[] fileRulesCopy = new Object[copySize];

            // New DeniedSigners
            for (int i = 0; i < newFileRules.Length; i++)
            {
                fileRulesCopy[i] = newFileRules[i];
            }

            // Existing AllowedSigners
            for (int i = 0; i < resultFileRules.Length; i++)
            {
                // Offset the index to length of new Prod signers to not overwrite entries
                fileRulesCopy[i + newFileRules.Length] = resultFileRules[i];
            }

            return fileRulesCopy;
        }

        /// <summary>
        /// Handles the merge operation between a new Signer[] and an existing Signer[] struct
        /// </summary>
        /// <param name="newProductSigners"></param>
        /// <param name="resultProductSigners"></param>
        /// <returns></returns>
        static CiSigner[] MergeCiSigners(CiSigner[] newCiSigner, CiSigner[] resultCiSigner)
        {
            // Short circuit if nothing from the new sipolicy
            if (newCiSigner == null || newCiSigner.Length == 0)
            {
                return resultCiSigner;
            }

            // Return newFileRules if resultFileRules is empty/null
            if (resultCiSigner == null || resultCiSigner.Length == 0)
            {
                return newCiSigner;
            }

            int copySize = newCiSigner.Length + resultCiSigner.Length;
            CiSigner[] ciSignerCopy = new CiSigner[copySize];

            // New CiSigner
            for (int i = 0; i < newCiSigner.Length; i++)
            {
                ciSignerCopy[i] = newCiSigner[i];
            }

            // Existing AllowedSigners
            for (int i = 0; i < resultCiSigner.Length; i++)
            {
                // Offset the index to length of new Prod signers to not overwrite entries
                ciSignerCopy[i + newCiSigner.Length] = resultCiSigner[i];
            }

            return ciSignerCopy;
        }


        /// <summary>
        /// Handles the merge operation between a new EKU[] and an existing EKU[] struct
        /// </summary>
        /// <param name="newEKU"></param>
        /// <param name="resultEKU"></param>
        /// <returns></returns>
        static EKU[] MergeEKUs(EKU[] newEKU, EKU[] resultEKU)
        {
            // Short circuit if nothing from the new sipolicy
            if (newEKU == null || newEKU.Length == 0)
            {
                return resultEKU;
            }

            // Return newFileRules if resultFileRules is empty/null
            if (resultEKU == null || resultEKU.Length == 0)
            {
                return newEKU;
            }

            int copySize = newEKU.Length + resultEKU.Length;
            EKU[] ekuCopy = new EKU[copySize];

            // New EKUs
            for (int i = 0; i < newEKU.Length; i++)
            {
                ekuCopy[i] = newEKU[i];
            }

            // Existing EKUs
            for (int i = 0; i < resultEKU.Length; i++)
            {
                // Offset the index to length of new EKUs to not overwrite entries
                ekuCopy[i + newEKU.Length] = resultEKU[i];
            }

            return ekuCopy;
        }

        /// <summary>
        /// Resets the Policy ID for base and supplemental policies and BasePolicy ID, for base policies
        /// </summary>
        /// <param name="policy"></param>
        public static void ResetPolicyGuid(SiPolicy policy)
        {
            // Catch null and Legacy policies
            if (policy == null || policy.BasePolicyID == null)
            {
                return;
            }

            // Base
            if (policy.BasePolicyID == policy.PolicyID)
            {
                policy.BasePolicyID = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
                policy.PolicyID = policy.BasePolicyID;
            }

            // Supplemental
            else
            {
                policy.PolicyID = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
            }
        }
    }
}
