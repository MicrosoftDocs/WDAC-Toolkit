using System;
using System.IO;
using System.Management.Automation;
using System.Collections.Generic; 
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.Diagnostics;

namespace WDAC_Wizard
{
    internal static class PSCmdlets
    {
        // Key already added PowerShell error HResult
        const int PSKEY_HRESULT = -2146233087;

        // Internal static instance of Runspace for PS pipelines
        internal static Runspace _Runspace {  get; set; }

        /// <summary>
        /// Creates Runspace and Pipeline while importing the ConfigCI module
        /// </summary>
        /// <returns></returns>
        internal static Pipeline CreatePipeline()
        {
            _Runspace =  RunspaceFactory.CreateRunspace();
            _Runspace.Open();
            Pipeline pipeline = _Runspace.CreatePipeline();
            pipeline.Commands.AddScript("Import-Module -SkipEditionCheck 'ConfigCI'");

            return pipeline;
        }

        /// <summary>
        /// Creates a dummy signer rule and policy to calculate the TBS hash for custom value signer rules
        /// </summary>
        /// <param name="customRule"></param>
        /// <returns></returns>
        internal static SiPolicy CreateSignerFromPS(PolicyCustomRules customRule)
        {
            string DUMMYPATH = Path.Combine(Helper.GetTempFolderPathRoot(), "DummySignersPolicy.xml");

            // Create runspace, pipeline and run script
            Pipeline pipeline = CreatePipeline();

            // Scan the file to extract the TBS hash (or hashes) for the signers
            pipeline.Commands.AddScript(String.Format("$DummyPcaRule += New-CIPolicyRule -Level PcaCertificate -DriverFilePath \"{0}\" -Fallback Hash", customRule.ReferenceFile));
            pipeline.Commands.AddScript(String.Format("New-CIPolicy -Rules $DummyPcaRule -FilePath \"{0}\"", DUMMYPATH));

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (CmdletInvocationException e) when (e.HResult == PSKEY_HRESULT)
            {
                // Catch the "An item with the same key has already been added" {System.Management.Automation.CmdletInvocationException}
                // Issue occurs on first powershell invocation - simply calling again fixes the issue
                // Github bugs 302, 362
                Logger.Log.AddWarningMsg("CreateSignerFromPS() caught CmdletInvocationException - An item with the same key has already been added");
                return CreateSignerFromPS(customRule);
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg("CreateSignerFromPS() caught the following exception", e);
                return null;
            }
            _Runspace.Dispose();

            // De-serialize the dummy policy to get the signer objects
            SiPolicy siPolicy = Helper.DeserializeXMLtoPolicy(DUMMYPATH);

            // Remove dummy file
            File.Delete(DUMMYPATH);

            return siPolicy;
        }

        /// <summary>
        /// Creates a list of hash rules to be used in the exceptions creation workflow. Calls the PS command to generate the hashes.
        /// </summary>
        /// <returns></returns>
        internal static object[] CreateHashRulesFromPS(PolicyCustomRules exceptRule)
        {
            // 1. Create a Deny Rule of the requested type (file attibutes, path, hash)
            // 2. Add the Deny Rule to the SiPolicy.FileRules section
            // 3. Return the ExceptDenyRule[] which contain the IDs of the Allow rules to add to the Signer.Exceptions

            string DUMMYPATH = Path.Combine(Helper.GetTempFolderPathRoot(), "DummyHashPolicy.xml");

            // Create runspace, pipeline and run script
            Pipeline pipeline = CreatePipeline();

            // Scan the file to extract the TBS hash (or hashes) for the signers
            string createRuleCmd = String.Format("$DummyHashRule += New-CIPolicyRule -Level Hash " +
                "-DriverFilePath \"{0}\"", exceptRule.ReferenceFile);

            if (exceptRule.Permission == PolicyCustomRules.RulePermission.Deny)
            {
                createRuleCmd += " -Deny";
            }
            pipeline.Commands.AddScript(createRuleCmd);
            pipeline.Commands.AddScript(String.Format("New-CIPolicy -Rules $DummyHashRule -FilePath \"{0}\"", DUMMYPATH));

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (CmdletInvocationException e) when (e.HResult == PSKEY_HRESULT)
            {
                // Catch the "An item with the same key has already been added" {System.Management.Automation.CmdletInvocationException}
                // Issue occurs on first powershell invocation - simply calling again fixes the issue
                // Github bugs 302, 362
                Logger.Log.AddWarningMsg("CreateHashRulesFromPS() caught CmdletInvocationException - An item with the same key has already been added");
                return CreateHashRulesFromPS(exceptRule);
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg("CreateHashRulesFromPS() caught the following exception:", e);
                return null;
            }
            _Runspace.Dispose();

            // De-serialize the dummy policy to get the signer objects
            SiPolicy tempSiPolicy = Helper.DeserializeXMLtoPolicy(DUMMYPATH);

            // Remove dummy file
            File.Delete(DUMMYPATH);

            return tempSiPolicy.FileRules;
        }

        /// <summary>
        /// Creates a WDAC policy for a signer rule to be used in signer rules
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="policyPath"></param>
        /// <returns></returns>
        internal static SiPolicy CreateSignerPolicyFromPS(PolicyCustomRules customRule, string policyPath)
        {
            // As a result of a bug in the ConfigCI New-CiPolicyRule variable output, the New-CIPolicyRule command
            // must be run in Powershell.exe (5.x) as opposed to PowerShell Core (7.x). Once the deserialization bug is 
            // fixed, the PS commands can be run in the runspace like all other commands
            
            // Scan the file to extract the TBS hash (or hashes for fallback) and, optionally, the CN for the signer rules
            string newPolicyRuleCmd = string.Empty;
            if (customRule.CheckboxCheckStates.checkBox1) // Publisher checkbox selected
            {
                newPolicyRuleCmd = String.Format("-NoProfile -Command \"$DummySignerRule = New-CIPolicyRule -Level Publisher -DriverFilePath \"{0}\" -Fallback Hash", customRule.ReferenceFile);
            }
            else // Publisher checkbox unselected - create a PCA rule
            {
                newPolicyRuleCmd = String.Format("-NoProfile -Command $DummySignerRule = New-CIPolicyRule -Level PcaCertificate -DriverFilePath \"{0}\" -Fallback Hash", customRule.ReferenceFile);
            }

            if (customRule.Permission == PolicyCustomRules.RulePermission.Deny)
            {
                newPolicyRuleCmd += " -Deny";
            }

            newPolicyRuleCmd += String.Format(", New-CIPolicy -Rules $DummySignerRule -FilePath \"{0}\"\"", policyPath);

            // Execute the Powershell (5.x) via Start Process
            var startInfo = new ProcessStartInfo
            {
                FileName = "powerShell.exe",
                Arguments = newPolicyRuleCmd,
                UseShellExecute = false,
                CreateNoWindow = true, // TODO: set to true after debugging
            };

            try
            {
                using (var process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    process.WaitForExit();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                }
            }
            catch (CmdletInvocationException e) when (e.HResult == PSKEY_HRESULT)
            {
                // Catch the "An item with the same key has already been added" {System.Management.Automation.CmdletInvocationException}
                // Issue occurs on first powershell invocation - simply calling again fixes the issue
                // Github bugs 302, 362
                Logger.Log.AddWarningMsg("CreateSignerPolicyFromPS() caught CmdletInvocationException - An item with the same key has already been added");
                return CreateSignerPolicyFromPS(customRule, policyPath);
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg("CreateSignerPolicyFromPS() caught the following exception", e);
                return null;
            }

            // De-serialize the dummy policy to get the signer objects
            SiPolicy siPolicy = Helper.DeserializeXMLtoPolicy(policyPath);
            return siPolicy;
        }

        /// <summary>
        /// Tries to add attributes like filename, publisher and version to non-custom publisher rules
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="signerSiPolicy"></param>
        internal static SiPolicy CreateHashPolicyFromPS(PolicyCustomRules customRule, string policyPath)
        {
            // Create runspace, pipeline and run script
            Pipeline pipeline = CreatePipeline();

            // Scan the file to extract the hashes for rules
            string newPolicyRuleCmd = String.Format("$DummyHashRule = New-CIPolicyRule -Level Hash -DriverFilePath \"{0}\"", customRule.ReferenceFile);
            if (customRule.Permission == PolicyCustomRules.RulePermission.Deny)
            {
                newPolicyRuleCmd += " -Deny";
            }

            pipeline.Commands.AddScript(newPolicyRuleCmd);
            pipeline.Commands.AddScript(String.Format("New-CIPolicy -Rules $DummyHashRule -FilePath \"{0}\"", policyPath));

            foreach (var cmd in pipeline.Commands)
            {
                Logger.Log.AddInfoMsg("Running the following commands: " + cmd);
            }

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg("CreateHashPolicyFromPS() encountered the following exception ", e);
                return null;
            }

            _Runspace.Dispose();

            // De-serialize the dummy policy to get the signer objects
            SiPolicy siPolicy = Helper.DeserializeXMLtoPolicy(policyPath);
            return siPolicy;
        }

        /// <summary>
        /// Tries to add attributes like filename, publisher and version to non-custom publisher rules
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="policyPath"></param>
        /// <param name="basePolicyToSupplementPath">Path to the base policy that the supplemental policy extends</param>
        /// <returns></returns>
        internal static SiPolicy CreateScannedPolicyFromPS(PolicyCustomRules customRule, string policyPath, string basePolicyToSupplementPath = null)
        {
            // Create runspace, pipeline and run script
            Pipeline pipeline = CreatePipeline();

            // Scan the file to extract the hashes for rules
            string newPolicyRuleCmd = String.Format("New-CIPolicy -ScanPath \"{0}\" -Level \"{1}\" -FilePath \"{2}\"",
                                                    customRule.ReferenceFile, customRule.Scan.Levels[0], policyPath);
            // Add fallback levels, if applicable
            if (customRule.Scan.Levels.Count > 1)
            {
                newPolicyRuleCmd += " -Fallback ";
                for (int i = 1; i < customRule.Scan.Levels.Count; i++)
                {
                    newPolicyRuleCmd += customRule.Scan.Levels[i] + ", ";
                }
                newPolicyRuleCmd = newPolicyRuleCmd.Substring(0, newPolicyRuleCmd.Length - 2); // trim trailing comma + whitespace
            }
            else
            {
                // By default, fall back to hash
                newPolicyRuleCmd += " -Fallback Hash";
            }

            // Add omit paths, if applicable
            if (customRule.Scan.OmitPaths.Count > 0)
            {
                newPolicyRuleCmd += " -OmitPaths ";
                for (int i = 0; i < customRule.Scan.OmitPaths.Count; i++)
                {
                    newPolicyRuleCmd += "\"" + customRule.Scan.OmitPaths[i] + "\", ";
                }
                newPolicyRuleCmd = newPolicyRuleCmd.Substring(0, newPolicyRuleCmd.Length - 2); // trim trailing comma + whitespace
            }

            // Handle User mode PEs, if applicable
            if (customRule.SigningScenarioCheckStates.umciEnabled)
            {
                newPolicyRuleCmd += " -UserPEs";
            }

            // Handle Deny rules, if applicable
            if (customRule.Permission == PolicyCustomRules.RulePermission.Deny)
            {
                newPolicyRuleCmd += " -Deny";
            }

            pipeline.Commands.AddScript(newPolicyRuleCmd);
            Logger.Log.AddInfoMsg("Running the following commands: " + newPolicyRuleCmd);

            // Set the supplemental-specific fields in the policy, if applicable
            if (!string.IsNullOrEmpty(basePolicyToSupplementPath))
            {
                string supplementalPolicyIdCmd = String.Format("Set-CIPolicyIdInfo -FilePath \"{0}\" -BasePolicyToSupplementPath \"{1}\"",
                                                                policyPath, basePolicyToSupplementPath);

                pipeline.Commands.AddScript(supplementalPolicyIdCmd);
                Logger.Log.AddInfoMsg("Running the following commands: " + supplementalPolicyIdCmd);
            }

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg("CreateScannedPolicyFromPS() encountered the following exception ", e);
                return null;
            }

            _Runspace.Dispose();

            // De-serialize the dummy policy to get the signer objects
            SiPolicy siPolicy = Helper.DeserializeXMLtoPolicy(policyPath);

            // Remove all the default policy rules
            // Do not set to null which causes invalid file at compilation time - Issue #218
            siPolicy.Rules = new RuleType[1];
            return siPolicy;
        }

        /// <summary>
        /// Runs the PS Set-CIPolicyIdInfo -Reset command to force the policy into multiple policy format
        /// </summary>
        /// <param name="path"></param>
        internal static void ResetGuidPs(string path)
        {
            // Create runspace, pipeline and runscript
            Pipeline pipeline = CreatePipeline();

            string resetCmd = String.Format("Set-CIPolicyIdInfo -ResetPolicyID \"{0}\"", path);

            pipeline.Commands.AddScript(resetCmd);
            Logger.Log.AddInfoMsg(String.Format("Running the following commands: {0}", resetCmd));

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg(String.Format("Exception encountered in ResetGuidPs(): {0}", e));
            }

            _Runspace.Dispose();
        }

        /// <summary>
        /// Method to convert the xml policy file into a binary CI policy file
        /// </summary>
        internal static string ConvertPolicyToBinary(string schemaPath)
        {
            // Operations: Converts the xml schema into a binary policy
            Logger.Log.AddInfoMsg("-- Converting to Binary --");

            // Create runspace, pipeline and runscript
            Pipeline pipeline = CreatePipeline();

            // If multi-policy format, use the {PolicyGUID}.cip format as defined in https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/deploy-multiple-windows-defender-application-control-policies#deploying-multiple-policies-locally
            string binaryFileName = string.Empty;
            string binPath = string.Empty; 
            SiPolicy finalSiPolicy = Helper.DeserializeXMLtoPolicy(schemaPath);

            if (finalSiPolicy != null)
            {
                if (finalSiPolicy.BasePolicyID != null)

                {
                    binaryFileName = String.Format("{0}.cip", finalSiPolicy.PolicyID);
                }
                else
                {
                    binaryFileName = "SiPolicy.p7b";
                }

                binPath = Path.Combine(Path.GetDirectoryName(schemaPath), binaryFileName);
                string binConvertCmd = String.Format("ConvertFrom-CIPolicy -XmlFilePath \"{0}\" -BinaryFilePath \"{1}\"",
                                                     schemaPath, binPath);

                pipeline.Commands.AddScript(binConvertCmd);
                Logger.Log.AddInfoMsg("Running the following commands: " + binConvertCmd);
            }

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg(String.Format("Exception encountered in ConvertPolicyToBinary(): {0}", exp));
            }

            _Runspace.Dispose();
            return binPath;
        }

        /// <summary>
        /// Runs the Merge-CIPolicy command given a list of input file paths and output file path
        /// </summary>
        /// <param name="policyPaths">List of input policy paths to merge into destPath</param>
        /// <param name="schemaPath">Filepath defined by Policy.SchemaPath</param>
        /// <param name="destPath">The final destination output path. OutputSchema.xml</param>
        internal static void MergePolicies(List<string> policyPaths, string schemaPath, string destPath)
        {
            // Create runspace, pipeline and runscript
            Pipeline pipeline = CreatePipeline();
            
            string mergeScript = "Merge-CIPolicy -PolicyPaths ";

            // Logging
            foreach (var policyPath in policyPaths)
            {
                mergeScript += String.Format("\"{0}\",", policyPath);
            }

            // Remove last comma and add outputFilePath
            mergeScript = mergeScript.Remove(mergeScript.Length - 1);
            mergeScript += String.Format(" -OutputFilePath \"{0}\"", schemaPath);

            Logger.Log.AddInfoMsg("Running the following Merge Commands: ");
            Logger.Log.AddInfoMsg(mergeScript);

            pipeline.Commands.AddScript(mergeScript);
            pipeline.Commands.Add("Out-String");

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
                // Make copy of the finished schema file
                File.Copy(schemaPath, destPath, true);
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg(String.Format("Exception encountered in MergeTemplatesPolicy(): {0}", e));
            }
            _Runspace.Dispose();
        }

        internal static void MergeCustomPolicies(List<string> customRulesPathList, int nCustomRules, string outputFilePath, string pathCustomValuePolicy)
        {
            string mergeScript = String.Empty;

            // Create runspace, pipeline and runscript
            Pipeline pipeline = CreatePipeline();

            if (customRulesPathList.Count > 0 || nCustomRules > 0)
            {
                // Add all the merge paths
                // First policy in the merge list of policies will determeine the output policy format
                // Since we set the format in ProcessCustomRules(), the customRulesPathList will be the correct format
                mergeScript = "Merge-CIPolicy -PolicyPaths ";
                foreach (string path in customRulesPathList)
                {
                    // If the xml was successfully generated in the previous step
                    if (File.Exists(path))
                    {
                        mergeScript += String.Format("\"{0}\",", path);
                    }
                    else
                    {
                        Logger.Log.AddErrorMsg("Wizard could not find " + path + ". Skipping merge step in MergeCustomRulesPolicy()");
                    }
                }

                // If there are custom value rules, merge in siPolicy from /Temp/Custom
                if (nCustomRules > 0 && File.Exists(pathCustomValuePolicy))
                {
                    mergeScript += String.Format("\"{0}\",", pathCustomValuePolicy);
                }

                // Remove last comma and add outputFilePath
                mergeScript = mergeScript.Remove(mergeScript.Length - 1);
                mergeScript += String.Format(" -OutputFilePath \"{0}\"", outputFilePath);

                pipeline.Commands.AddScript(mergeScript);
                pipeline.Commands.Add("Out-String");

                Logger.Log.AddInfoMsg(String.Format("Running the following commands: {0}", mergeScript));

                try
                {
                    Collection<PSObject> results = pipeline.Invoke();
                }
                catch (Exception e)
                {
                    Logger.Log.AddErrorMsg(String.Format("Exception encountered in MergeCustomRulesPolicy(): {0}", e));
                }
            }
            _Runspace.Dispose();
        }
    }
}
