// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;


namespace WDAC_Wizard
{
    public class WDAC_Policy
    {
        public enum PolicyType
        {
            None, 
            BasePolicy, 
            SupplementalPolicy, 
            Edit, 
            Merge
        }

        public enum NewPolicyTemplate
        {
            None, 
            AllowMicrosoft,
            WindowsWorks, 
            SignedReputable, 
        }

        public enum Format
        {
            None,
            Legacy, 
            MultiPolicy
        }

        // Policy Properties
        public PolicyType _PolicyType { get; set; }
        public NewPolicyTemplate _PolicyTemplate { get; set; }
        public Format _Format { get; set; }

        public string PolicyName { get; set; }          // User entered friendly name for policy
        public bool EnableHVCI { get; set; }            // Configure hypervisor code integrity (HVCI)?
        public bool EnableAudit { get; set; }           // Turn on audit mode? 
        public string VersionNumber { get; set; }       // Policy version. By default, 10.0.0.0.

        // Paths:
        public string SchemaPath { get; set; }          // Path to final xml file on disk
        public string TemplatePath { get; set; }        // ReadOnly Path to template policy - TODO: make const
        public string BaseToSupplementPath { get; set; } // Path to base policy to supplement, if applicable
        public string EditPolicyPath { get; set; }      // Path to the policy we are editing. Used for parsing.


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
            this.siPolicy = null; 

            this._PolicyTemplate = NewPolicyTemplate.None;
            this._PolicyType = PolicyType.None;
            this._Format = Format.None;

            this.EnableHVCI = false;
            this.EnableAudit = true;

            this.EKUs = new List<PolicyEKUs>();
            this.FileRules = new Dictionary<string, PolicyFileRules>();
            this.Signers = new Dictionary<string, PolicySigners>();//<PolicySigners>();
            this.SigningScenarios = new List<PolicySigningScenarios>();
            this.UpdateSigners = new List<PolicyUpdateSigners>();
            this.SupplementalSigners = new List<PolicySupplementalSigners>();
            this.CISigners = new List<PolicyCISigners>();
            this.PolicySettings = new List<PolicySettings>();
            this.CustomRules = new List<PolicyCustomRules>();

            this.VersionNumber = "10.0.0.0"; // Default policy version when calling the New-CIPolicy cmdlet
            
        }

        public string UpdateVersion()
        {
            int[] versionIdx = this.VersionNumber.Split('.').Select(n => Convert.ToInt32(n)).ToArray();
            for (int i = versionIdx.Length-1; i > 0; i--)
            {
                if (versionIdx[i] >= 9)
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
            // Convert int[] --> this.VersionNumber string
            this.VersionNumber = ""; // reset string 
            foreach(var vIdx in versionIdx)
                this.VersionNumber += String.Format("{0}.", vIdx.ToString());
            this.VersionNumber = this.VersionNumber.Substring(0, this.VersionNumber.Length - 1); //remove trailing period

            return this.VersionNumber; 
        }

        public int GetWinVersion()
        {
            try
            {
                return Convert.ToInt32(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", ""));
            } 
            catch(Exception e)
            {

            }
            return -1;  
        }

    }
}
