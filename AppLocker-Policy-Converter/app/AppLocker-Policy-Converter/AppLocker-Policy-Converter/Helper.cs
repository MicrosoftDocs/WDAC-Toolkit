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
        // Counts of file rules created to pipe into IDs
        static public int cFilePublisherRules = 0;
        static public int cFileAttribRules = 0;
        static public int cFilePathRules = 0; 
        static public int cFileHashRules = 0;

        static public Dictionary<string, string> supportedMacros = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "OSDRIVE", "" }, { "WINDIR", "" }, { "SYSTEM32", "" } };

        static public Dictionary<string, string> convertibleMacros = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) {
            { "PROGRAMFILES",     @"%OSDRIVE%\Program Files" },
            { "PROGRAMFILES(X86)", @"%OSDRIVE%\Program Files (x86)" },
            { "PROGRAMDATA",      @"%OSDRIVE%\ProgramData" },
        };

        static public HashSet<string> userScopedMacros = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
            "APPDATA", "LOCALAPPDATA", "USERPROFILE", "TEMP", "HOMEPATH", "HOMEDRIVE"
        };

        // Warning and error msgs structs
        static private List<string> WarningMessages = new List<string>();
        static private List<string> ErrorMessages = new List<string>();

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
                using (StreamReader reader = new StreamReader(xmlPath))
                {
                    appLockerPolicy = (AppLockerPolicy)serializer.Deserialize(reader);
                }
            }
            catch (Exception exp)
            {
                throw new InvalidOperationException("There is an error in " + xmlPath, exp);
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
                using (StreamReader reader = new StreamReader(xmlPath))
                {
                    siPolicy = (SiPolicy)serializer.Deserialize(reader);
                }
            }
            catch (Exception exp)
            {
                throw new InvalidOperationException("There is an error in " + xmlPath, exp);
            }

            return siPolicy;
        }

        /// <summary>
        /// Deserialize the xml policy on disk to SiPolicy
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <returns>SiPolicy object</returns>
        public static SiPolicy DeserializeXMLStringtoPolicy(string xmlContents)
        {
            SiPolicy siPolicy;
            
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SiPolicy));
                using (StringReader reader = new StringReader(xmlContents))
                {
                    siPolicy = (SiPolicy)serializer.Deserialize(reader);
                }
            }
            catch (Exception exp)
            {
                throw new InvalidOperationException("There is an error in the provided XML contents", exp);
            }

            return siPolicy;
        }

        /// <summary>
        /// Converts an AppLocker FilePathRuleType to an SiPolicy publisher rule. Adds the new rule to the provided policy
        /// </summary>
        /// <param name="filePubRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy ConvertFilePublisherRule(FilePublisherRuleType filePubRule, SiPolicy siPolicy)
        {
            string publisherName = filePubRule.Conditions.FilePublisherCondition.PublisherName;

            // Microsoft Corporation signers need 2 rules - handle with a different method
            if(publisherName.Contains("Microsoft"))
            {
                return ConvertMSFTFilePublisherRule(filePubRule, siPolicy);
            }

            string action = (String)filePubRule.Action.ToString();
            string productName = filePubRule.Conditions.FilePublisherCondition.ProductName;
            string minVersion = filePubRule.Conditions.FilePublisherCondition.BinaryVersionRange.LowSection;
            string maxVersion = filePubRule.Conditions.FilePublisherCondition.BinaryVersionRange.HighSection;
            string fileName = filePubRule.Conditions.FilePublisherCondition.BinaryName;

            // Handle exceptions, if any
            List<object> exceptionsList = new List<object>();
            List<ExceptAllowRule> exceptAllowRulesList = new List<ExceptAllowRule>(); 
            List<ExceptDenyRule> exceptDenyRulesList = new List<ExceptDenyRule>(); 


            if(filePubRule.Exceptions != null)
            {
                (exceptAllowRulesList, exceptDenyRulesList, siPolicy) = CreateExceptions(filePubRule, siPolicy);
            }

            // Create new CertPublisher object and add CertPublisher field
            CertPublisher cPub = new CertPublisher();
            cPub.Value = ExtractPublisher(publisherName);

            // Create new Certificate Root object and add to CertRoot field
            CertRoot cRoot = new CertRoot();
            cRoot.Type = CertEnumType.Wellknown;
            byte[] arr = { 20 }; //Authroot
            cRoot.Value = arr;

            // Create new FileAttrib object to link to signer
            FileAttrib fileAttrib = new FileAttrib();
            fileAttrib.FileName = fileName;
            fileAttrib.ID = "ID_FILEATTRIB_A_" + cFileAttribRules;
            fileAttrib.FriendlyName = filePubRule.Name;

            // Do not blindly set versions == "*"
            // This is okay to do for Original Filenames
            if (minVersion != "*")
            {
                fileAttrib.MinimumFileVersion = minVersion;
            }
            if(maxVersion != "*")
            {
                fileAttrib.MaximumFileVersion = maxVersion;
            }
            if(!String.IsNullOrEmpty(productName) && productName != "*")
            {
                fileAttrib.ProductName = productName; 
            }

            // Add the FileAttributeReference to SiPolicy
            siPolicy = AddSiPolicyFileAttrib(fileAttrib, siPolicy);
            cFileAttribRules++;

            // Link the new FileAttrib object back to the signer
            FileAttribRef fileAttribRef = new FileAttribRef();
            fileAttribRef.RuleID = fileAttrib.ID;

            // Create new signer object
            Signer signer = new Signer();
            signer.Name = filePubRule.Name;
            signer.ID = "ID_SIGNER_A_" + cFilePublisherRules;
            signer.CertRoot = cRoot; 
            signer.CertPublisher = cPub;
            signer.FileAttribRef = new FileAttribRef[1];
            signer.FileAttribRef[0] = fileAttribRef;

            cFilePublisherRules++;

            if (action == "Allow")
            {
                // Add the allow signer to Signers and the product signers section with Windows Signing Scenario
                // Add to CiSigners section to indicate that this is a valid Enterprise signer
                if(exceptDenyRulesList.Count > 0)
                {
                    siPolicy = AddSiPolicyAllowSigner(signer, siPolicy, exceptDenyRulesList);
                }
                else
                {
                    siPolicy = AddSiPolicyAllowSigner(signer, siPolicy);
                }
                siPolicy = AddCiSigner(signer, siPolicy);
            }
            else
            {
                // Add the deny signer to Signers and the product signers section with Windows Signing Scenario
                if (exceptAllowRulesList.Count > 0)
                {
                    siPolicy = AddSiPolicyDenySigner(signer, siPolicy, exceptAllowRulesList);
                }
                else
                {
                    siPolicy = AddSiPolicyDenySigner(signer, siPolicy);
                }
                siPolicy = AddCiSigner(signer, siPolicy);
            }
            
            return siPolicy;
        }

        /// <summary>
        /// Converts an AppLocker FilePathRuleType to 2 SiPolicy Microsoft publisher rules (Wellknown root = 06,07). Adds the new rules to the provided policy
        /// </summary>
        /// <param name="filePubRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy ConvertMSFTFilePublisherRule(FilePublisherRuleType filePubRule, SiPolicy siPolicy)
        {
            string publisherName = filePubRule.Conditions.FilePublisherCondition.PublisherName;
            string action = (String)filePubRule.Action.ToString();
            string productName = filePubRule.Conditions.FilePublisherCondition.ProductName;
            string minVersion = filePubRule.Conditions.FilePublisherCondition.BinaryVersionRange.LowSection;
            string maxVersion = filePubRule.Conditions.FilePublisherCondition.BinaryVersionRange.HighSection;
            string fileName = filePubRule.Conditions.FilePublisherCondition.BinaryName;

            // Create new CertPublisher object and add CertPublisher field
            CertPublisher cPub = new CertPublisher();
            cPub.Value = ExtractPublisher(publisherName);

            // Create new 2 new cert root objects for Microsoft Wellknown Roots 06 and 07
            // Impossible to know from the AppLocker policy which root was used so trust both
            CertRoot cProdRoot = new CertRoot();
            cProdRoot.Type = CertEnumType.Wellknown;
            byte[] arr = { 06 }; // MS Root 2010
            cProdRoot.Value = arr;

            CertRoot cStdRoot = new CertRoot();
            cStdRoot.Type = CertEnumType.Wellknown;
            byte[] arr1 = { 07 }; // MS Root 2011
            cStdRoot.Value = arr1;

            // Create new FileAttrib object to link to signer
            FileAttrib fileAttrib = new FileAttrib();
            fileAttrib.FileName = fileName;
            fileAttrib.ID = "ID_FILEATTRIB_A_" + cFileAttribRules;
            fileAttrib.FriendlyName = filePubRule.Name;

            // Do not blindly set versions == "*"
            // This is okay to do for Original Filenames
            if (minVersion != "*")
            {
                fileAttrib.MinimumFileVersion = minVersion;
            }
            if (maxVersion != "*")
            {
                fileAttrib.MaximumFileVersion = maxVersion;
            }
            if (productName != "*" || !String.IsNullOrEmpty(productName))
            {
                fileAttrib.ProductName = productName;
            }

            // Add the FileAttributeReference to SiPolicy
            siPolicy = AddSiPolicyFileAttrib(fileAttrib, siPolicy);
            cFileAttribRules++;

            // Link the new FileAttrib object back to the signer
            FileAttribRef fileAttribRef = new FileAttribRef();
            fileAttribRef.RuleID = fileAttrib.ID;

            // Create new signer object for Prod root (Wellknown=06)
            Signer prodSigner = new Signer();
            prodSigner.Name = filePubRule.Name;
            prodSigner.ID = "ID_SIGNER_A_" + cFilePublisherRules;
            prodSigner.CertRoot = cProdRoot;
            prodSigner.CertPublisher = cPub;
            prodSigner.FileAttribRef = new FileAttribRef[1];
            prodSigner.FileAttribRef[0] = fileAttribRef;
            cFilePublisherRules++;

            // Create new signer object for Std root (Wellknown=07)
            Signer stdSigner = new Signer();
            stdSigner.Name = filePubRule.Name;
            stdSigner.ID = "ID_SIGNER_A_" + cFilePublisherRules;
            stdSigner.CertRoot = cStdRoot;
            stdSigner.CertPublisher = cPub;
            stdSigner.FileAttribRef = new FileAttribRef[1];
            stdSigner.FileAttribRef[0] = fileAttribRef;
            cFilePublisherRules++;

            if (action == "Allow")
            {
                // Add the allow signer to Signers and the product signers section with Windows Signing Scenario
                siPolicy = AddSiPolicyAllowSigner(prodSigner, siPolicy);
                siPolicy = AddSiPolicyAllowSigner(stdSigner, siPolicy);
            }
            else
            {
                // Add the deny signer to Signers and the product signers section with Windows Signing Scenario
                siPolicy = AddSiPolicyDenySigner(prodSigner, siPolicy);
                siPolicy = AddSiPolicyDenySigner(stdSigner, siPolicy);
            }

            return siPolicy;
        }


        /// <summary>
        /// Converts an AppLocker FileHashRuleType to an SiPolicy rule. Adds the new rule to the provided policy
        /// </summary>
        /// <param name="fileHashRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy ConvertFileHashRule(FileHashRuleType fileHashRule, SiPolicy siPolicy)
        {
            foreach(FileHashType fileHash in fileHashRule.Conditions.FileHashCondition)
            {
                string action = fileHashRule.Action.ToString();

                if (action == "Allow")
                {
                    Allow allowRule = new Allow();
                    allowRule.Hash = ConvertHashStringToByte(fileHash.Data);
                    // Use per-hash SourceFileName for FriendlyName if available, fall back to rule Name
                    allowRule.FriendlyName = !string.IsNullOrEmpty(fileHash.SourceFileName)
                        ? String.Format("{0} ({1})", fileHash.SourceFileName, fileHashRule.Name)
                        : fileHashRule.Name;
                    string algo = fileHash.Type.ToString(); //e.g. Type = SHA256
                    allowRule.ID = String.Format("ID_ALLOW_B_{0}_{1}", cFileHashRules, algo);

                    // Add the Allow rule to FileRules and FileRuleRef section with Windows Signing Scenario
                    siPolicy = AddSiPolicyAllowRule(allowRule, siPolicy);
                }
                else
                {
                    Deny denyRule = new Deny();
                    denyRule.Hash = ConvertHashStringToByte(fileHash.Data);
                    // Use per-hash SourceFileName for FriendlyName if available, fall back to rule Name
                    denyRule.FriendlyName = !string.IsNullOrEmpty(fileHash.SourceFileName)
                        ? String.Format("{0} ({1})", fileHash.SourceFileName, fileHashRule.Name)
                        : fileHashRule.Name;
                    string algo = fileHash.Type.ToString(); //Type = SHA256
                    denyRule.ID = String.Format("ID_DENY_B_{0}_{1}", cFileHashRules, algo);

                    // Add the deny rule to FileRules and FileRuleRef section with Windows Signing Scenario
                    siPolicy = AddSiPolicyDenyRule(denyRule, siPolicy);
                }

                cFileHashRules++;
            }

            return siPolicy;
        }

        /// <summary>
        /// Converts an AppLocker FilePathRuleType to an SiPolicy rule. Adds the new rule to the provided policy
        /// </summary>
        /// <param name="filePathRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy ConvertFilePathRule(FilePathRuleType filePathRule, SiPolicy siPolicy)
        {
            string action = (String)filePathRule.Action.ToString();
            string path = filePathRule.Conditions.FilePathCondition.Path;

            // Warn user that Allow "*" will not be processed. Rule collections like Scripts and Msi/Appx might have allow * 
            // while having a strict exe allowlist. This would result in an allow all WDAC policy with unintended consequences
            if(path == "*")
            {
                ErrorMessages.Add(String.Format("ERROR: <FilePathCondition Path=\"*\" /> from rule ID = {0} cannot be converted. " +
                    "ALLOW OR DENY \"*\" RULES MUST BE MANUALLY ADDED YOUR WDAC POLICY.", filePathRule.Id));
                return siPolicy; 
            }

            if (siPolicy.FileRules == null)
            {
                siPolicy.FileRules = new object[1]; 
            }

            // Validate the path rule confirms to the WDAC path rule format
            // Else, try to convert to a WDAC path rule
            string wdacPathRule = MakeValidPathRule(path);

            // Unable to convert to valid WDAC path rule, return
            if(String.IsNullOrEmpty(wdacPathRule))
            {
                return siPolicy; 
            }

            string friendlyName = filePathRule.Name;

            if (action == "Allow")
            {
                Allow allowRule = new Allow();
                allowRule.FilePath = wdacPathRule; 
                allowRule.FriendlyName = friendlyName;
                allowRule.ID = "ID_ALLOW_C_" + cFilePathRules.ToString();

                // Add the Allow rule to FileRules and FileRuleRef section with Windows Signing Scenario
                siPolicy = AddSiPolicyAllowRule(allowRule, siPolicy);
            }
            else
            {
                Deny denyRule = new Deny();
                denyRule.FilePath = wdacPathRule; 
                denyRule.FriendlyName = friendlyName;
                denyRule.ID = "ID_DENY_C_" + cFilePathRules.ToString();

                // Add the Deny rule to FileRules and FileRuleRef section with Windows Signing Scenario
                siPolicy = AddSiPolicyDenyRule(denyRule, siPolicy);
            }

            // Increment counter before processing exceptions to avoid ID collisions
            // between the parent rule and its exception rules
            cFilePathRules++;

            // Convert exceptions for Allow rules and explicitly skips exceptions on Deny rules with a warning.
            if(filePathRule.Exceptions != null)
            {
                siPolicy = CreateFilePathExceptions(filePathRule, siPolicy);
            }

            return siPolicy;
        }


        // Converts exceptions for Allow rules and skips Deny-rule exceptions.
        public static SiPolicy CreateFilePathExceptions(FilePathRuleType filePathRule, SiPolicy siPolicy)
        {
            string action = filePathRule.Action.ToString();

            if (filePathRule.Exceptions == null || filePathRule.Exceptions.Items == null || filePathRule.Exceptions.Items.Length == 0)
            {
                return siPolicy;
            }

            foreach (var exceptionItem in filePathRule.Exceptions.Items)
            {
                if (exceptionItem.GetType() == typeof(FilePathConditionType))
                {
                    FilePathConditionType exception = (FilePathConditionType)exceptionItem;
                    if (exception.Path == "*")
                    {
                        WarningMessages.Add(String.Format(
                            "WARNING: Skipping wildcard exception '*' from path rule '{0}' - " +
                            "a '*' exception cannot be safely converted to WDAC as it would generate a Deny-all rule. " +
                            "Handle this manually.", filePathRule.Name));
                        continue;
                    }
                    
                    string wdacPath = MakeValidPathRule(exception.Path);
                    if (String.IsNullOrEmpty(wdacPath))
                    {
                        WarningMessages.Add(String.Format("WARNING: Skipping path exception '{0}' - cannot be converted to a valid WDAC path rule.", exception.Path));
                        continue;
                    }

                    if (action == "Allow")
                    {
                        // Allow parent -> exception becomes Deny
                        WarningMessages.Add(String.Format(
                            "WARNING: Exception '{0}' from Allow path rule '{1}' has been converted to a WDAC Deny rule. " +
                            "In WDAC, Deny overrides any Allow rule - verify this matches the intended policy behavior.",
                            wdacPath, filePathRule.Name));
                        Deny deny = new Deny();
                        deny.FilePath = wdacPath;
                        deny.ID = "ID_DENY_C_" + cFilePathRules.ToString();
                        deny.FriendlyName = "Deny path - " + wdacPath;
                        siPolicy = AddSiPolicyDenyRule(deny, siPolicy);
                        cFilePathRules++;
                    }
                    else
                    {
                        // Deny parent path exceptions cannot be represented safely in WDAC:
                        // a matching Deny path rule still blocks even if a separate Allow path rule exists.
                        WarningMessages.Add(String.Format(
                            "WARNING: Skipping path exception '{0}' from deny path rule '{1}' - WDAC cannot safely represent exceptions on deny path rules. Handle this case manually.",
                            exception.Path, filePathRule.Name));
                    }
                }
                else if (exceptionItem.GetType() == typeof(FileHashConditionType))
                {
                    FileHashConditionType exception = (FileHashConditionType)exceptionItem;
                    if (exception.FileHash == null || exception.FileHash.Length == 0)
                    {
                        WarningMessages.Add(String.Format("WARNING: Skipping hash exception from path rule '{0}' - file hash condition is empty or malformed.",
                            filePathRule.Name));
                        continue;
                    }

                    foreach (var hashVal in exception.FileHash)
                    {
                        string hashDisplayName = !string.IsNullOrEmpty(hashVal.SourceFileName) ? hashVal.SourceFileName : hashVal.Data;
                        if (action == "Allow")
                        {
                            WarningMessages.Add(String.Format(
                                "WARNING: Skipping hash exception '{0}' from Allow path rule '{1}' - " +
                                "hash-based exceptions cannot be safely converted to WDAC. A Deny-by-hash rule in WDAC " +
                                "blocks the file globally regardless of path, which is broader than the original AppLocker " +
                                "exception scoped to the parent rule. Handle this manually.",
                                hashDisplayName,
                                filePathRule.Name));
                        }
                        else
                        {
                            WarningMessages.Add(String.Format(
                                "WARNING: Skipping hash exception '{0}' from deny path rule '{1}' - " +
                                "hash-based exceptions cannot be safely converted to WDAC because WDAC Deny rules override " +
                                "Allow rules, so an allow-by-hash exception would not take effect. Handle this manually.",
                                hashDisplayName,
                                filePathRule.Name));
                        }
                    }
                }
                else if (exceptionItem.GetType() == typeof(FilePublisherConditionType))
                {
                    FilePublisherConditionType exception = (FilePublisherConditionType)exceptionItem;
                    WarningMessages.Add(String.Format("WARNING: Skipping publisher exception '{0}' from path rule '{1}' - publisher conditions cannot be converted to WDAC path rule exceptions.",
                        exception.PublisherName, filePathRule.Name));
                }
            }

            return siPolicy;
        }

        public static (List<ExceptAllowRule>, List<ExceptDenyRule>, SiPolicy) CreateExceptions(FilePublisherRuleType filePubRule, SiPolicy siPolicy)
        {
            List <ExceptAllowRule> exceptAllowRules = new List<ExceptAllowRule>();
            List<ExceptDenyRule> exceptDenyRules = new List<ExceptDenyRule>();

            //
            ExceptAllowRule exceptAllowRule = new ExceptAllowRule();
            ExceptDenyRule exceptDenyRule = new ExceptDenyRule();

            string action = filePubRule.Action.ToString(); 

            foreach (var exceptionItem in filePubRule.Exceptions.Items)
            {
                if (exceptionItem.GetType() == typeof(FileHashConditionType))
                {
                    FileHashConditionType exception = (FileHashConditionType)exceptionItem;
                    foreach(var hashVal in exception.FileHash)
                    {
                        if(action == "Allow")
                        {
                            // Create a Deny rule to except an Allow rule
                            Deny deny = new Deny();
                            deny.Hash = ConvertHashStringToByte(hashVal.Data);
                            string algo = hashVal.Type.ToString(); //e.g. Type = SHA256
                            deny.ID = String.Format("ID_DENY_B_{0}_{1}", cFileHashRules, algo);
                            deny.FriendlyName = hashVal.SourceFileName; 

                            siPolicy = AddSiPolicyDenyRule(deny, siPolicy, true);
                            exceptDenyRule.DenyRuleID = deny.ID; 
                            exceptDenyRules.Add(exceptDenyRule);
                            cFileHashRules++;
                        }
                        else
                        {
                            Allow allow = new Allow();
                            allow.Hash = ConvertHashStringToByte(hashVal.Data);
                            string algo = hashVal.Type.ToString(); //e.g. Type = SHA256
                            allow.ID = String.Format("ID_ALLOW_B_{0}_{1}", cFileHashRules, algo);
                            allow.FriendlyName = hashVal.SourceFileName;

                            siPolicy = AddSiPolicyAllowRule(allow, siPolicy, true);
                            exceptAllowRule.AllowRuleID = allow.ID;
                            exceptAllowRules.Add(exceptAllowRule);
                            cFileHashRules++;
                        }
                    }

                }
                else if (exceptionItem.GetType() == typeof(FilePathConditionType))
                {
                    FilePathConditionType exception = (FilePathConditionType)exceptionItem;
                    if(action == "Allow")
                    {
                        // Create a Deny rule to except an Allow rule
                        Deny deny = new Deny();
                        string wdacPath = MakeValidPathRule(exception.Path);
                        if(String.IsNullOrEmpty(wdacPath))
                        {
                            // Skip exception, path rule cannot be converted
                            break;
                        }
                        deny.FilePath = wdacPath;
                        deny.ID = "ID_DENY_C_" + cFilePathRules.ToString();
                        deny.FriendlyName = "Deny path - " + exception.Path;

                        siPolicy = AddSiPolicyDenyRule(deny, siPolicy, true);
                        exceptDenyRule.DenyRuleID = deny.ID;
                        exceptDenyRules.Add(exceptDenyRule);
                        cFilePathRules++;
                    }
                    else
                    {
                        // Create an Allow rule
                        Allow allow = new Allow();
                        string wdacPath = MakeValidPathRule(exception.Path);
                        if (String.IsNullOrEmpty(wdacPath))
                        {
                            // Skip exception, path rule cannot be converted
                            break;
                        }
                        allow.FilePath = wdacPath;
                        allow.ID = "ID_ALLOW_C_" + cFilePathRules.ToString();
                        allow.FriendlyName = "Allow path - " + exception.Path;

                        siPolicy = AddSiPolicyAllowRule(allow, siPolicy, true);
                        exceptAllowRule.AllowRuleID = allow.ID;
                        exceptAllowRules.Add(exceptAllowRule);
                        cFilePathRules++;
                    }
                }
                else if (exceptionItem.GetType() == typeof(FilePublisherConditionType))
                {
                    FilePublisherConditionType exception = (FilePublisherConditionType)exceptionItem;
                    WarningMessages.Add(String.Format("WARNING: SKIPPING RULE EXCEPTION {0}. Id = {1}" +
                        "Publisher rules cannot be used to except publisher rules in WDAC.", exception.PublisherName, filePubRule.Id));
                }
            }

            return (exceptAllowRules, exceptDenyRules, siPolicy);
        }

        public static SiPolicy DeduplicateFileRules(SiPolicy siPolicy)
        {
            if (siPolicy?.FileRules == null)
                return siPolicy;
        
            // Only deduplicate rules referenced in FileRulesRef - never touch exception rules
            // referenced via AllowedSigner.ExceptDenyRule or DeniedSigner.ExceptAllowRule
            var referencedIDs = new HashSet<string>();
            if (siPolicy.SigningScenarios != null && 
                siPolicy.SigningScenarios.Length > 1 &&
                siPolicy.SigningScenarios[1]?.ProductSigners?.FileRulesRef?.FileRuleRef != null)
            {
                foreach (var ruleRef in siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef.FileRuleRef)
                    referencedIDs.Add(ruleRef.RuleID);
            }
        
            var seenAllowPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var seenDenyPaths  = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var keepRules      = new List<object>();
            var removedIDs     = new HashSet<string>();
        
            foreach (var rule in siPolicy.FileRules)
            {
                if (rule is Allow allow && !string.IsNullOrEmpty(allow.FilePath) && referencedIDs.Contains(allow.ID))
                {
                    if (!seenAllowPaths.Add(allow.FilePath))
                    {
                        WarningMessages.Add(String.Format(
                            "WARNING: Duplicate Allow path rule '{0}' (ID={1}) removed - " +
                            "this path was already added from another AppLocker rule collection.",
                            allow.FilePath, allow.ID));
                        removedIDs.Add(allow.ID);
                        continue;
                    }
                }
                else if (rule is Deny deny && !string.IsNullOrEmpty(deny.FilePath) && referencedIDs.Contains(deny.ID))
                {
                    if (!seenDenyPaths.Add(deny.FilePath))
                    {
                        WarningMessages.Add(String.Format(
                            "WARNING: Duplicate Deny path rule '{0}' (ID={1}) removed - " +
                            "this path was already added from another AppLocker rule collection.",
                            deny.FilePath, deny.ID));
                        removedIDs.Add(deny.ID);
                        continue;
                    }
                }
                keepRules.Add(rule);
            }
        
            siPolicy.FileRules = keepRules.ToArray();
        
            // Remove corresponding FileRuleRef entries for deduplicated rules
            if (removedIDs.Count > 0 &&
                siPolicy.SigningScenarios != null &&
                siPolicy.SigningScenarios.Length > 1 &&
                siPolicy.SigningScenarios[1]?.ProductSigners?.FileRulesRef?.FileRuleRef != null)
            {
                siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef.FileRuleRef =
                    siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef.FileRuleRef
                    .Where(r => !removedIDs.Contains(r.RuleID))
                    .ToArray();
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

            siPolicy = DeduplicateFileRules(siPolicy);

            // Serialize policy to XML file
            XmlSerializer serializer = new XmlSerializer(typeof(SiPolicy));
            StreamWriter writer = new StreamWriter(xmlPath);
            serializer.Serialize(writer, siPolicy);
            writer.Close();
        }

        /// <summary>
        /// Takes in an AppLocker hash string value and returns an SiPolicy hash
        /// </summary>
        /// <param name="sHash"></param>
        /// <returns></returns>
        public static byte[] ConvertHashStringToByte(string sHash)
        {
            sHash = sHash.Substring(2); // Trim the first "0x" off the string
            byte[] bHash = new byte[sHash.Length/2];
            int _base = 16;
            string sValue;  //chunk into 2's

            for(int i= 0; i < sHash.Length; i+=2)
            {
                sValue = "" + sHash[i] + sHash[i + 1];
                bHash[i / 2] = Convert.ToByte(sValue, _base);
            }

            return bHash; 
        }

        /// <summary>
        /// Converts an AppLocker FilePath rule to one that WDAC can handle. Returns null where constraints are broken.
        /// </summary>
        /// <param name="appLockerPathRule"></param>
        /// <returns></returns>
        public static string MakeValidPathRule(string appLockerPathRule)
        {
            string wdacPathRule = String.Empty; 

            // WDAC only supports a handful of macros - %OSDRIVE%, %WINDIR%, %SYSTEM32%
            if (appLockerPathRule.Contains("%"))
            {
                var macroParts = appLockerPathRule.Split("%");
                if (macroParts.Length != 3)
                {
                    ErrorMessages.Add(String.Format("ERROR: AppLocker Path Rule \"{0}\" is not a valid WDAC Path Rule.", appLockerPathRule));
                    return null;
                }

                // This macro is NOT supported in WDAC - try converting
                // Need to still check valid rules for wildcarding - goto wildcard check
                if (!supportedMacros.ContainsKey(macroParts[1]))
                {
                    string macroName = macroParts[1];

                    // Check if this is a per-user macro that cannot be safely auto-converted
                    if (userScopedMacros.Contains(macroName))
                    {
                        WarningMessages.Add(String.Format(
                            "WARNING: AppLocker Path Rule \"{0}\" uses per-user macro '%{1}%' which cannot be " +
                            "automatically converted. Please manually replace with an appropriate WDAC path rule "
                            + "(e.g. using %OSDRIVE%\\Users\\*\\... pattern).",
                            appLockerPathRule, macroName));
                        return null;
                    }

                    // Check if this macro can be deterministically mapped to a WDAC %OSDRIVE%-based path
                    if (convertibleMacros.TryGetValue(macroName, out string wdacMacroReplacement))
                    {
                        wdacPathRule = wdacMacroReplacement + macroParts[2];
                        WarningMessages.Add(String.Format(
                            "WARNING: AppLocker macro '%{0}%' is not supported in WDAC. " +
                            "Automatically converted \"{1}\" to \"{2}\".",
                            macroName, appLockerPathRule, wdacPathRule));
                    
                        // Run wildcard validation on the converted path before returning
                        int cConverted = wdacPathRule.Count(f => f == '*');
                        if (cConverted == 1)
                        {
                            int idx = wdacPathRule.IndexOf('*');
                            if (idx != 0 && idx != wdacPathRule.Length - 1)
                                WarningMessages.Add(String.Format("WARNING: AppLocker Path Rule \"{0}\" is a valid WDAC Path Rule on Windows 11 systems only.", wdacPathRule));
                        }
                        else if (cConverted > 1)
                        {
                            WarningMessages.Add(String.Format("WARNING: AppLocker Path Rule \"{0}\" is valid in WDAC on Windows 11 systems only.", wdacPathRule));
                        }
                    
                        return wdacPathRule;
                    }

                    // Unknown macro - fall back to wildcard stripping as before
                    if (String.IsNullOrEmpty(macroParts[0]))
                    {
                        if(macroParts[2] == @"\*")
                        {
                            // E.g. %UNKNOWNMACRO%\* would result in Path=*\* or just Path="*" which we do not want to create
                            ErrorMessages.Add(String.Format("ERROR: AppLocker Path Rule \"{0}\" is not a valid WDAC Path Rule.", appLockerPathRule));
                            return null;
                        }

                        wdacPathRule = "*" + macroParts[2];
                    }
                    else
                    {
                        // Keep only the outside edges
                        wdacPathRule = macroParts[0] + macroParts[2];
                    }

                    WarningMessages.Add(String.Format("WARNING: AppLocker Path Rule \"{0}\" is not a valid WDAC Path Rule. Replacing with the " +
                        "following Path Rule: \"{1}\"", appLockerPathRule, wdacPathRule));

                    return wdacPathRule;
                }
            }

            int cWildcards = appLockerPathRule.Count(f => (f == '*'));

            // Updated 22H2: wildcards are now supported in the middle of the path
            if(cWildcards == 1)
            {
                int idx = appLockerPathRule.IndexOf('*'); 
                if (idx == 0 || idx == appLockerPathRule.Length-1)
                {
                    // Wildcard is at the front or end of the path
                    // This is a valid position for the 1 wildcard
                    // E.g. *\Folder1\Folder2\Tool.exe
                    // E.g. %WINDIR%\Folder1\Folder2\Folder3\*
                }
                else
                {
                    // 1 Wildcard is in the middle of the path
                    // E.g. %WINDIR%\Folder1\*\FolderX\tool.dll -->
                    // *\FolderX\tool.dll
              
                    // Updated: Win11 22H2 and 21H2 now support this - show warning about support

                    WarningMessages.Add(String.Format("WARNING: AppLocker Path Rule \"{0}\" is a valid WDAC Path " +
                                        "Rule on Windows 11 systems only.",appLockerPathRule));
                }
            }

            // Multiple wildcards found
            // Now supported on Windows 11. Just warn the user about the support
            if (cWildcards > 1)
            {
                WarningMessages.Add(String.Format("WARNING: AppLocker Path Rule \"{0}\" is valid in WDAC on Windows 11 systems only.",
                                                    appLockerPathRule));
            }
            
            return appLockerPathRule; 
        }

        /// <summary>
        /// Parses the AppLocker PublisherName into one Publisher value for WDAC. If CN is present, returns CN, else, returns O value. 
        /// </summary>
        /// <param name="publisher"></param>
        /// <returns></returns>
        public static string ExtractPublisher(string publisher)
        {
            // Always grab the first value - it will be CN where exists, O otherwise (when O=CN)
            // Ex) ["O =", "   Contoso Corporation"]
            var pubParts = publisher.Split(',');
            if(pubParts.Length < 2)
            {
                return publisher;
            }

            string formattedPub = pubParts[0];

            formattedPub = formattedPub.Split('=')[1]; 

            // Remove any prepended whitespace
            char[] charsToTrim = { ' ', '\'' };
            formattedPub = formattedPub.Trim(charsToTrim);

            return formattedPub;
        }

        /// <summary>
        /// Handles adding the new Allow Rule object to the provided siPolicy
        /// </summary>
        /// <param name="allowRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        private static SiPolicy AddSiPolicyAllowRule(Allow allowRule, SiPolicy siPolicy,bool isException= false)
        {
            // Copy and replace the FileRules obj[] in siPolicy
            // FileRules always initalized - no need to check if null
            object[] fileRulesCopy = new object[siPolicy.FileRules.Length + 1];
            for (int i = 0; i < fileRulesCopy.Length - 1; i++)
            {
                fileRulesCopy[i] = siPolicy.FileRules[i];
            }

            fileRulesCopy[fileRulesCopy.Length - 1] = allowRule;
            siPolicy.FileRules = fileRulesCopy;

            // Copy and replace the FileRulesRef section to add to Signing Scenarios
            // If this is an exception, don't add to FileRulesRef section
            if(!isException)
            {
                FileRulesRef refCopy = new FileRulesRef();
                if (siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef == null)
                {
                    refCopy.FileRuleRef = new FileRuleRef[1];
                    refCopy.FileRuleRef[0] = new FileRuleRef();
                    refCopy.FileRuleRef[0].RuleID = allowRule.ID;
                }
                else
                {
                    refCopy.FileRuleRef = new FileRuleRef[siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef.FileRuleRef.Length + 1];
                    for (int i = 0; i < refCopy.FileRuleRef.Length - 1; i++)
                    {
                        refCopy.FileRuleRef[i] = siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef.FileRuleRef[i];
                    }

                    refCopy.FileRuleRef[refCopy.FileRuleRef.Length - 1] = new FileRuleRef();
                    refCopy.FileRuleRef[refCopy.FileRuleRef.Length - 1].RuleID = allowRule.ID;
                }

                siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef = refCopy;
            }
            
            return siPolicy; 
        }

        /// <summary>
        /// Handles adding the new Deny Rule object to the provided siPolicy
        /// </summary>
        /// <param name="denyRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        private static SiPolicy AddSiPolicyDenyRule(Deny denyRule, SiPolicy siPolicy, bool isException=false)
        {
            // Copy and replace the FileRules obj[] in siPolicy
            // FileRules always initalized - no need to check if null
            object[] fileRulesCopy = new object[siPolicy.FileRules.Length + 1];
            for (int i = 0; i < fileRulesCopy.Length - 1; i++)
            {
                fileRulesCopy[i] = siPolicy.FileRules[i];
            }

            fileRulesCopy[fileRulesCopy.Length - 1] = denyRule;
            siPolicy.FileRules = fileRulesCopy;

            // Copy and replace the FileRulesRef section to add to Signing Scenarios
            // If this is an exception, don't add to FileRulesRef section
            if(!isException)
            {
                FileRulesRef refCopy = new FileRulesRef();
                if (siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef == null)
                {
                    refCopy.FileRuleRef = new FileRuleRef[1];
                    refCopy.FileRuleRef[0] = new FileRuleRef();
                    refCopy.FileRuleRef[0].RuleID = denyRule.ID;
                }
                else
                {
                    refCopy.FileRuleRef = new FileRuleRef[siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef.FileRuleRef.Length + 1];
                    for (int i = 0; i < refCopy.FileRuleRef.Length - 1; i++)
                    {
                        refCopy.FileRuleRef[i] = siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef.FileRuleRef[i];
                    }

                    refCopy.FileRuleRef[refCopy.FileRuleRef.Length - 1] = new FileRuleRef();
                    refCopy.FileRuleRef[refCopy.FileRuleRef.Length - 1].RuleID = denyRule.ID;
                }

                siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef = refCopy;
            }
           
            return siPolicy; 
        }

        /// <summary>
        /// Handles adding the new AllowSignerobject to the provided siPolicy
        /// </summary>
        /// <param name="signer"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        private static SiPolicy AddSiPolicyAllowSigner(Signer signer, SiPolicy siPolicy, List<ExceptDenyRule> denyRuleIDs=null)
        {
            // Copy the SiPolicy signer object and add the signer param to the field
            Signer[] signersCopy = new Signer[siPolicy.Signers.Length + 1];
            for (int i = 0; i < signersCopy.Length - 1; i++)
            {
                signersCopy[i] = siPolicy.Signers[i];
            }

            signersCopy[signersCopy.Length - 1] = signer;
            siPolicy.Signers = signersCopy;

            // Create an AllowedSigner object to add to the SiPolicy ProductSigners section
            AllowedSigner allowedSigner = new AllowedSigner();
            allowedSigner.SignerId = signer.ID;

            // Add exception rules if applicable
            if(denyRuleIDs != null)
            {
                ExceptDenyRule[] denyRules = new ExceptDenyRule[denyRuleIDs.Count];
                for (int i = 0; i < denyRuleIDs.Count; i++)
                {
                    denyRules[i] = new ExceptDenyRule();
                    denyRules[i].DenyRuleID = denyRuleIDs[i].DenyRuleID;
                }
                allowedSigner.ExceptDenyRule = denyRules;
            }

            // Copy and replace
            if (siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners == null)
            {
                siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners = new AllowedSigners();
                siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners.AllowedSigner = new AllowedSigner[1];
                siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners.AllowedSigner[0] = allowedSigner; 
            }
            else
            {
                int cAllowedSigners = siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners.AllowedSigner.Length;
                AllowedSigner[] allowedSigners = new AllowedSigner[cAllowedSigners + 1];

                for (int i = 0; i < cAllowedSigners; i++)
                {
                    allowedSigners[i] = siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners.AllowedSigner[i];
                }

                allowedSigners[cAllowedSigners] = allowedSigner;
                siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners.AllowedSigner = allowedSigners;
            }
            return siPolicy;             
        }

        /// <summary>
        /// Handles adding the new DenySigner object to the provided siPolicy
        /// </summary>
        /// <param name="signer"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        private static SiPolicy AddSiPolicyDenySigner(Signer signer, SiPolicy siPolicy, List<ExceptAllowRule> allowRuleIDs = null)
        {
            // Copy the SiPolicy signer object and add the signer param to the field
            Signer[] signersCopy = new Signer[siPolicy.Signers.Length + 1];
            for (int i = 0; i < signersCopy.Length - 1; i++)
            {
                signersCopy[i] = siPolicy.Signers[i];
            }

            signersCopy[signersCopy.Length - 1] = signer;
            siPolicy.Signers = signersCopy;

            // Create an AllowedSigner object to add to the SiPolicy ProductSigners section
            DeniedSigner deniedSigner = new DeniedSigner();
            deniedSigner.SignerId = signer.ID;

            // Add exception allow rules
            if(allowRuleIDs != null)
            {
                ExceptAllowRule[] allowRules = new ExceptAllowRule[allowRuleIDs.Count];
                for(int i = 0; i < allowRuleIDs.Count; i++)
                {
                    allowRules[i] = new ExceptAllowRule();
                    allowRules[i].AllowRuleID = allowRuleIDs[i].AllowRuleID; 
                }
                deniedSigner.ExceptAllowRule = allowRules; 
            }

            // Copy and replace
            if (siPolicy.SigningScenarios[1].ProductSigners.DeniedSigners == null)
            {
                siPolicy.SigningScenarios[1].ProductSigners.DeniedSigners = new DeniedSigners();
                siPolicy.SigningScenarios[1].ProductSigners.DeniedSigners.DeniedSigner = new DeniedSigner[1];
                siPolicy.SigningScenarios[1].ProductSigners.DeniedSigners.DeniedSigner[0] = deniedSigner;
            }
            else
            {
                int cDeniedSigners = siPolicy.SigningScenarios[1].ProductSigners.DeniedSigners.DeniedSigner.Length;
                DeniedSigner[] deniedSigners = new DeniedSigner[cDeniedSigners + 1];

                for (int i = 0; i < cDeniedSigners; i++)
                {
                    deniedSigners[i] = siPolicy.SigningScenarios[1].ProductSigners.DeniedSigners.DeniedSigner[i];
                }

                deniedSigners[cDeniedSigners] = deniedSigner;
                siPolicy.SigningScenarios[1].ProductSigners.DeniedSigners.DeniedSigner = deniedSigners;
            }

            return siPolicy;
        }

        /// <summary>
        /// Handles adding the new FileAttribute object to the provided siPolicy
        /// </summary>
        /// <param name="fileAttrib"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>

        /// <summary>
        /// Adds a CiSigner object to the CiSigners section in the WDAC policy
        /// </summary>
        /// <param name="ciSigner"></param>
        /// <param name="siPolicy"></param>
        private static SiPolicy AddCiSigner(Signer signer, SiPolicy siPolicy)
        {
            // Add to the CiSigners section of the policy as well
            // Copy the SiPolicy signer object and add the signer param to the field
            CiSigner[] ciSignersCopy = new CiSigner[siPolicy.CiSigners.Length + 1];
            for (int i = 0; i < ciSignersCopy.Length - 1; i++)
            {
                ciSignersCopy[i] = siPolicy.CiSigners[i];
            }

            ciSignersCopy[ciSignersCopy.Length - 1] = new CiSigner();
            ciSignersCopy[ciSignersCopy.Length - 1].SignerId = signer.ID;
            siPolicy.CiSigners = ciSignersCopy;

            return siPolicy;
        }

        private static SiPolicy AddSiPolicyFileAttrib(FileAttrib fileAttrib, SiPolicy siPolicy)
        {
            // Copy and replace FileRules section in SiPolicy
            // FileRules always initalized - no need to check if null
            object[] fileRulesCopy = new object[siPolicy.FileRules.Length + 1];
            for (int i = 0; i < fileRulesCopy.Length - 1; i++)
            {
                fileRulesCopy[i] = siPolicy.FileRules[i];
            }

            fileRulesCopy[fileRulesCopy.Length - 1] = fileAttrib; 
            siPolicy.FileRules = fileRulesCopy;

            return siPolicy; 
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
        /// Dumps all the warning messages as well as the count into the console
        /// </summary>
        public static void DumpWarningMsgs()
        {
            if(WarningMessages.Count == 0)
            {
                return; 
            }

            Console.WriteLine(String.Format("\r\nPolicy Conversion finished with {0} WARNINGS: ", WarningMessages.Count)); 
            foreach(string warningMsg in WarningMessages)
            {
                Console.WriteLine(warningMsg);
            }
        }

        /// <summary>
        /// Dumps all the warning messages as well as the count into the console
        /// </summary>
        public static void DumpErrorMsgs()
        {
            if (ErrorMessages.Count == 0)
            {
                return;
            }

            Console.WriteLine(String.Format("\r\nPolicy Conversion finished with {0} ERRORS: ", ErrorMessages.Count));
            foreach (string errorMsg in ErrorMessages)
            {
                Console.WriteLine(errorMsg);
            }
        }
    }
}
