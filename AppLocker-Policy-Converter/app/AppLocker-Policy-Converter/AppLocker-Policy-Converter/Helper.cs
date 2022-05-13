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
        /// Converts an AppLocker FilePathRuleType to an SiPolicy publisher rule. Adds the new rule to the provided policy
        /// </summary>
        /// <param name="filePubRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy ConvertFilePublisherRule(FilePublisherRuleType filePubRule, SiPolicy siPolicy)
        {
            string action = (String)filePubRule.Action.ToString();
            string productName = filePubRule.Conditions.FilePublisherCondition.ProductName;
            string minVersion = filePubRule.Conditions.FilePublisherCondition.BinaryVersionRange.LowSection;
            string maxVersion = filePubRule.Conditions.FilePublisherCondition.BinaryVersionRange.HighSection;

            // Create new signer object
            Signer signer = new Signer();
            signer.Name = filePubRule.Name;

            CertPublisher cPub = new CertPublisher();
            cPub.Value = ExtractPublisher(filePubRule.Conditions.FilePublisherCondition.PublisherName);
            signer.CertPublisher = cPub;

            string signerId = "ID_SIGNER_" + cFilePublisherRules;

            // Create new FileAttrib object to link to signer
            FileAttrib fileAttrib = new FileAttrib(); 
            fileAttrib.FileName = filePubRule.Conditions.FilePublisherCondition.BinaryName;

            // Do not blindly set versions == "*"
            // This is okay to do for Original Filenames
            if(minVersion != "*")
            {
                fileAttrib.MinimumFileVersion = minVersion;
            }
            if(maxVersion != "*")
            {
                fileAttrib.MaximumFileVersion = maxVersion;
            }
            if(productName != "*" || !String.IsNullOrEmpty(productName))
            {
                fileAttrib.ProductName = productName; 
            }

            fileAttrib.ID = "ID_FILEATTRIB_" + cFileAttribRules;

            // Link the new FileAttrib object back to the signer
            FileAttribRef fileAttribRef = new FileAttribRef();
            fileAttribRef.RuleID = fileAttrib.ID;
            signer.FileAttribRef = new FileAttribRef[1];
            signer.FileAttribRef[0] = fileAttribRef;

            cFileAttribRules++;

            // Add FileAttrib to SiPolicy.FileRules
            // siPolicy.FileRules.Append(

            // Link the signer to AllowedSigner/DeniedSigner objs
            if (action == "Allow")
            {
                AllowedSigner allowedSigner = new AllowedSigner();
                allowedSigner.SignerId = signerId;

                // Copy and replace
                int cAllowedSigners = siPolicy.SigningScenarios[0].ProductSigners.AllowedSigners.AllowedSigner.Length;
                AllowedSigner[] allowedSigners = new AllowedSigner[cAllowedSigners + 1];

                for(int i = 0; i< cAllowedSigners; i++)
                {
                    allowedSigners[i] = siPolicy.SigningScenarios[0].ProductSigners.AllowedSigners.AllowedSigner[i];
                }

                allowedSigners[cAllowedSigners + 1] = allowedSigner;
            }
            else
            {
                DeniedSigner deniedSigner = new DeniedSigner();
                deniedSigner.SignerId = signerId;
            }
            cFilePublisherRules++;
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
                    allowRule.FriendlyName = fileHashRule.Name;
                    string algo = fileHashRule.Conditions.FileHashCondition[0].Type.ToString(); //e.g. Type = SHA256
                    allowRule.ID = String.Format("ID_ALLOW_B_{0}_{1}", cFileHashRules, algo);

                    // Add the Allow rule to FileRules and FileRuleRef section with Windows Signing Scenario
                    siPolicy = AddSiPolicyAllowRule(allowRule, siPolicy);
                }
                else
                {
                    Deny denyRule = new Deny();
                    denyRule.Hash = ConvertHashStringToByte(fileHash.Data);
                    denyRule.FriendlyName = fileHashRule.Name;
                    string algo = fileHashRule.Conditions.FileHashCondition[0].Type.ToString(); //Type = SHA256
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

            if(siPolicy.FileRules == null)
            {
                siPolicy.FileRules = new object[1]; 
            }

            string wdacPathRule = MakeValidPathRule(filePathRule.Conditions.FilePathCondition.Path);

            // Unable to convert to valid WDAC path rule, return
            if(String.IsNullOrEmpty(wdacPathRule))
            {
                return siPolicy; 
            }

            if (action == "Allow")
            {
                Allow allowRule = new Allow();
                allowRule.FilePath = wdacPathRule; 
                allowRule.FriendlyName = filePathRule.Description;
                allowRule.ID = "ID_ALLOW_C_" + cFilePathRules.ToString();

                // Add the Allow rule to FileRules and FileRuleRef section with Windows Signing Scenario
                siPolicy = AddSiPolicyAllowRule(allowRule, siPolicy);
            }
            else
            {
                Deny denyRule = new Deny();
                denyRule.FilePath = wdacPathRule; 
                denyRule.FriendlyName = filePathRule.Description;
                denyRule.ID = "ID_DENY_C_" + cFilePathRules.ToString();

                // Add the Deny rule to FileRules and FileRuleRef section with Windows Signing Scenario
                siPolicy = AddSiPolicyDenyRule(denyRule, siPolicy);
            }

            cFilePathRules++;
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

        /// <summary>
        /// Takes in an AppLocker hash string value and returns an SiPolicy hash
        /// </summary>
        /// <param name="sHash"></param>
        /// <returns></returns>
        public static byte[] ConvertHashStringToByte(string sHash)
        {
            sHash = sHash.Substring(2); // Trim the first 0x off the string
            byte[] bHash = new byte[sHash.Length];
            int _base = 16; 

            for (int i = 0; i < sHash.Length; i++)
            {
                bHash[i] = Convert.ToByte(sHash[16], _base);
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

            // WDAC only supports a handful of macros
            if (appLockerPathRule.Contains("*") && appLockerPathRule.Contains("%"))
            {
                Console.WriteLine("");
                Console.WriteLine(String.Format("WARNING: AppLocker Path Rule {0} is not a valid WDAC Path Rule.", appLockerPathRule));
                return null;
            }

            if(appLockerPathRule.Contains("%"))
            {
                var macroParts = appLockerPathRule.Split("%"); 
                wdacPathRule = macroParts[0] + macroParts[2]; // Keep only the outside edges

                Console.WriteLine("");
                Console.WriteLine(String.Format("WARNING: AppLocker Path Rule {0} is not a valid WDAC Path Rule.", appLockerPathRule));
                Console.WriteLine(String.Format("Replacing with the following Path Rule: {0}", wdacPathRule));

                return wdacPathRule; 
            }

            int cWildcards = appLockerPathRule.Count(f => (f == '*'));

            // Assert the only position is front or back
            if(cWildcards == 1)
            {
                int idx = appLockerPathRule.IndexOf('*'); 
                if (idx == 0 || idx == appLockerPathRule.Length-1)
                {
                    // This is a valid position for the 1 wildcard
                }
                else
                {
                    var parts = appLockerPathRule.Split('*');

                    foreach (string subString in parts)
                        wdacPathRule += subString;

                    Console.WriteLine("");
                    Console.WriteLine(String.Format("WARNING: AppLocker Path Rule {0} is not a valid WDAC Path Rule.", appLockerPathRule));
                    Console.WriteLine(String.Format("Replacing with the following Path Rule: {0}", wdacPathRule));

                    return wdacPathRule;
                }
            }

            // WDAC supports at most 1 wildcard * in the path
            if (cWildcards > 1)
            {
                var parts = appLockerPathRule.Split('*');
                if (parts.Length > 2)
                {
                    foreach (string subString in parts)
                        wdacPathRule += subString;

                    // Keep the leading wildcard
                    if(String.IsNullOrEmpty(parts[0]))
                    {
                        wdacPathRule = "*" + wdacPathRule; 
                    }
                    else 
                    {
                        // Else, keep the trailing the wildcard
                        wdacPathRule += "*"; 
                    }

                    Console.WriteLine("");
                    Console.WriteLine(String.Format("WARNING: AppLocker Path Rule {0} is not a valid WDAC Path Rule.", appLockerPathRule));
                    Console.WriteLine(String.Format("Replacing with the following Path Rule: {0}", wdacPathRule));

                    return wdacPathRule; 
                }
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
            string formattedPub = pubParts[0];

            formattedPub = formattedPub.Split('=')[1]; 

            // Remove any prepended whitespace
            char[] charsToTrim = { ' ', '\'' };
            formattedPub = formattedPub.Trim(charsToTrim);

            return formattedPub;
        }

        private static SiPolicy AddSiPolicyAllowRule(Allow allowRule, SiPolicy siPolicy)
        {
            // Copy and replace the FileRules obj[] in siPolicy
            object[] fileRulesCopy = new object[siPolicy.FileRules.Length + 1];
            for (int i = 0; i < fileRulesCopy.Length - 1; i++)
            {
                fileRulesCopy[i] = siPolicy.FileRules[i];
            }

            fileRulesCopy[fileRulesCopy.Length - 1] = allowRule;
            siPolicy.FileRules = fileRulesCopy;

            // Copy and replace the FileRulesRef section to add to Signing Scenarios
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

                refCopy.FileRuleRef[fileRulesCopy.Length - 1] = new FileRuleRef();
                refCopy.FileRuleRef[fileRulesCopy.Length - 1].RuleID = allowRule.ID;
            }

            siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef = refCopy;
            return siPolicy; 
        }


        private static SiPolicy AddSiPolicyDenyRule(Deny denyRule, SiPolicy siPolicy)
        {
            // Copy and replace the FileRules obj[] in siPolicy
            object[] fileRulesCopy = new object[siPolicy.FileRules.Length + 1];
            for (int i = 0; i < fileRulesCopy.Length - 1; i++)
            {
                fileRulesCopy[i] = siPolicy.FileRules[i];
            }

            fileRulesCopy[fileRulesCopy.Length - 1] = denyRule;
            siPolicy.FileRules = fileRulesCopy;

            // Copy and replace the FileRulesRef section to add to Signing Scenarios
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

                refCopy.FileRuleRef[fileRulesCopy.Length - 1] = new FileRuleRef();
                refCopy.FileRuleRef[fileRulesCopy.Length - 1].RuleID = denyRule.ID;
            }

            siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef = refCopy;
            return siPolicy; 
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

       
    }
}