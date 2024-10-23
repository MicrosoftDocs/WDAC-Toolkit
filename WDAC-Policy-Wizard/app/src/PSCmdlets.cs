using System;
using System.IO;
using System.Management.Automation;
using System.Collections.Generic; 
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using System.Security.Policy;
using Markdig.Helpers;
using System.Linq;

namespace WDAC_Wizard
{
    internal static class PSCmdlets
    {
        // Key already added PowerShell error HResult
        const int PSKEY_HRESULT = -2146233087;

        // Name of PS Script to generate signer policy (workaround for New-CIPolicyRule bug)
        const string PS_POLICYRULE_FILENAME = "CreateSignerPolicy.ps1";

        // Name of PS Script to generate signer policy (workaround for New-CIPolicy -ScanPath bug)
        const string PS_SCAN_FILENAME = "CreateScannedPolicy.ps1";

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
        /// Creates a WDAC policy for a signer rule to be used in signer rules
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="policyPath"></param>
        /// <returns></returns>
        internal static SiPolicy CreatePolicyRuleFromPS(PolicyCustomRules customRule, string policyPath=null)
        {
            // As a result of a bug in the ConfigCI New-CiPolicyRule variable output, the New-CIPolicyRule command
            // must be run in Powershell.exe (5.x) as opposed to PowerShell Core (7.x). Once the deserialization bug is 
            // fixed, the PS commands can be run in the runspace like all other commands
            
            // Scan the file to extract the TBS hash (or hashes for fallback) and, optionally, the CN for the signer rules
            string level = string.Empty;
            string deny = "False";
            string ps1File = Path.Combine(Helper.GetExecutablePath(false), PS_POLICYRULE_FILENAME);
            string wizardPath = Helper.GetExecutablePath(true);

            // Set Rule Level
            if(customRule.Type == PolicyCustomRules.RuleType.Hash)
            {
                level = "Hash";
            }
            else
            {
                if (customRule.CheckboxCheckStates.checkBox1) // Publisher checkbox selected
                {
                    level = "Publisher";
                }
                else // Publisher checkbox unselected - create a PCA rule
                {
                    level = "PcaCertificate";
                }
            }

            // Set temp filepath if null
            if(policyPath == null)
            {
                policyPath = Path.Combine(Helper.GetTempFolderPathRoot(), "DummyHashPolicy.xml");
            }

            if (customRule.Permission == PolicyCustomRules.RulePermission.Deny)
            {
                deny = "True";
            }

            string newPolicyScriptCmd = $"-NoProfile -ExecutionPolicy ByPass -File \"{ps1File}\" -WdacBinPath \"{wizardPath}\" " +
                $"-DriverFilePath \"{customRule.ReferenceFile}\" -PolicyPath \"{policyPath}\" -Level {level} -Deny {deny}";

            Logger.Log.AddInfoMsg($"Running the following PS cmd in CreateSignerPolicyFromPS(): {newPolicyScriptCmd}");

            // Execute the Powershell (5.x) via Start Process
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = newPolicyScriptCmd,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Process process = new Process
            {
                StartInfo = startInfo
            };

            try
            {
                process.Start();
                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if(!string.IsNullOrEmpty(error))
                {
                    Logger.Log.AddErrorMsg($"CreatePolicyRuleFromPS() threw the following error: {error}");
                }
                
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg("CreatePolicyRuleFromPS() caught the following exception", e);
                return null;
            }

            // Remap the signer, file attribute and hash rule IDs to guarantee uniqueness
            // De-serialize the dummy policy to get the signer or file rule objects
            return PolicyHelper.RemapIDs(Helper.DeserializeXMLtoPolicy(policyPath), customRule.Type);
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
            // As a result of a bug in the ConfigCI New-CIPolicy -ScanPath in PWSH Core, the scan command
            // must be run in Powershell.exe (5.x) 

            string scanPath = customRule.ReferenceFile; 
            string level = customRule.Scan.Levels[0];
            string fallbacks = "Hash";
            string pathsToOmit = "";
            string deny = "False";
            string userPEs = "False";
            string ps1File = Path.Combine(Helper.GetExecutablePath(false), PS_SCAN_FILENAME);
            string wizardPath = Helper.GetExecutablePath(true);

            // Add fallback levels, if applicable
            if (customRule.Scan.Levels.Count > 1)
            {
                fallbacks = string.Join(",",customRule.Scan.Levels.Skip(1));
            }

            // Add paths to omit, if applicable
            if (customRule.Scan.OmitPaths.Count > 0)
            {
                pathsToOmit = string.Join(",", customRule.Scan.OmitPaths);
            }

            // Handle User mode PEs, if applicable
            if (customRule.SigningScenarioCheckStates.umciEnabled)
            {
                userPEs = "True";
            }

            // Allow vs Deny rule case
            if (customRule.Permission == PolicyCustomRules.RulePermission.Deny)
            {
                deny = "True";
            }

            string newPolicyScriptCmd = $"-NoProfile -ExecutionPolicy ByPass -File \"{ps1File}\" -ScanPath \"{scanPath}\" " +
                $"-PolicyPath \"{policyPath}\" -Level {level} -Fallback {fallbacks} -PathsToOmit \"{pathsToOmit}\"" +
                $" -Deny {deny} -UserPEs {userPEs}";

            Logger.Log.AddInfoMsg($"Running the following PS cmd in CreateScannedPolicyFromPS(): {newPolicyScriptCmd}");

            // Execute the Powershell (5.x) via Start Process
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = newPolicyScriptCmd,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Process process = new Process
            {
                StartInfo = startInfo
            };

            try
            {
                process.Start();
                process.WaitForExit();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Log.AddErrorMsg($"CreateScannedPolicyFromPS() threw the following error: {error}");
                }

            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg("CreateScannedPolicyFromPS() caught the following exception", e);
                return null;
            }

            // De-serialize the dummy policy to get the signer objects
            SiPolicy siPolicy = Helper.DeserializeXMLtoPolicy(policyPath);

            // Remove all the default policy rules
            // Do not set to null which causes invalid file at compilation time - Issue #218
            if (siPolicy != null)
            {
                siPolicy.Rules = new RuleType[1];
            }
            return siPolicy;
        }

        /// <summary>
        /// Runs the Add-SignerRule to generate Publisher rules from certificate files and 
        /// serializes the policy file into an SiPolicy object
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="policyPath"></param>
        /// <returns></returns>
        internal static SiPolicy AddSignerRuleFromPS(PolicyCustomRules customRule, string policyPath)
        {
            // Create runspace, pipeline and runscript
            Pipeline pipeline = CreatePipeline();

            string addSignerCmd = $"Add-SignerRule -CertificatePath \"{customRule.ReferenceFile}\" -FilePath \"{policyPath}\"";

            // Add -User if user mode
            if (customRule.SigningScenarioCheckStates.umciEnabled)
            {
                addSignerCmd += " -User";
            }

            // Add -Kernel if kernel mode
            if (customRule.SigningScenarioCheckStates.kmciEnabled)
            {
                addSignerCmd += " -Kernel";
            }

            // Append -Deny for deny rules
            if (customRule.Permission == PolicyCustomRules.RulePermission.Deny)
            {
                addSignerCmd += " -Deny";
            }

            pipeline.Commands.AddScript(addSignerCmd);
            Logger.Log.AddInfoMsg($"Running the following PS7 command: {addSignerCmd}");

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg($"Exception encountered in AddSignerRuleFromPS(): {e}");
            }

            _Runspace.Dispose();

            // De-serialize the dummy policy to get the signer objects
            SiPolicy siPolicy = Helper.DeserializeXMLtoPolicy(policyPath);

            // Remove all the default policy rules
            // Do not set to null which causes invalid file at compilation time - Issue #218
            if (siPolicy != null)
            {
                siPolicy.Rules = new RuleType[1];
            }

            return PolicyHelper.RemapIDs(siPolicy, customRule.Type);
        }

        /// <summary>
        /// Runs the PS Set-CIPolicyIdInfo -Reset command to force the policy into multiple policy format
        /// </summary>
        /// <param name="path"></param>
        internal static void ResetGuidPS(string path)
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
        internal static string CompilePolicyBinary(string xmlPath)
        {
            // Operations: Converts the xml schema into a binary policy
            Logger.Log.AddInfoMsg("-- Converting to Binary --");

            // Create runspace, pipeline and runscript
            Pipeline pipeline = CreatePipeline();

            // If multi-policy format, use the {PolicyGUID}.cip format as defined in https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/deploy-multiple-windows-defender-application-control-policies#deploying-multiple-policies-locally
            string binaryFileName = string.Empty;
            string binPath = string.Empty; 
            SiPolicy finalSiPolicy = Helper.DeserializeXMLtoPolicy(xmlPath);

            if (finalSiPolicy != null)
            {
                // Multi-policy format policies always have a base policy ID. 
                if (finalSiPolicy.BasePolicyID != null)
                {
                    binaryFileName = $"{finalSiPolicy.PolicyID}.cip";
                }
                else
                {
                    // Legacy policies do not. Set bin name to SiPolicy.p7b
                    binaryFileName = "SiPolicy.p7b";
                }

                binPath = Path.Combine(Path.GetDirectoryName(xmlPath), binaryFileName);
                string binConvertCmd = String.Format("ConvertFrom-CIPolicy -XmlFilePath \"{0}\" -BinaryFilePath \"{1}\"",
                                                     xmlPath, binPath);

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
        /// <param name="destPath">The final destination output path. OutputSchema.xml</param>
        internal static void MergePolicies(List<string> policyPaths, string destPath)
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
            mergeScript += String.Format(" -OutputFilePath \"{0}\"", destPath);

            Logger.Log.AddInfoMsg("Running the following Merge Commands: ");
            Logger.Log.AddInfoMsg(mergeScript);

            pipeline.Commands.AddScript(mergeScript);
            pipeline.Commands.Add("Out-String");

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
                // Make copy of the finished schema file
            }
            catch (Exception e)
            {
                Logger.Log.AddErrorMsg(String.Format("Exception encountered in MergeTemplatesPolicy(): {0}", e));
            }
            _Runspace.Dispose();
        }

        /// <summary>
        /// Runs the Merge-CIPolicy command for all the policies generated per custom rule
        /// </summary>
        /// <param name="customRulesPathList">List of custom policy paths to merge</param>
        /// <param name="nCustomRules">Number of custom rules generated</param>
        /// <param name="outputFilePath">Final output policy path</param>
        /// <param name="pathCustomValuePolicy">Policy containing the list of custom rules e.g. custom path, custom PFN</param>
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
