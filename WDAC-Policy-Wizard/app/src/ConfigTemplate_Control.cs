// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using WDAC_Wizard.Properties;
using System.Xml.Serialization; 

namespace WDAC_Wizard
{
    public partial class ConfigTemplate_Control : UserControl
    {        
        public Logger Log { get; set; }
        public MainWindow _MainWindow;
        private WDAC_Policy Policy;

        public ConfigTemplate_Control(MainWindow pMainWindow)
        {
            InitializeComponent();

            
            this.Policy = pMainWindow.Policy; 
            this._MainWindow = pMainWindow;
            this.Log = this._MainWindow.Log;

            this._MainWindow.ErrorOnPage = false;
            this._MainWindow.RedoFlowRequired = false; // Nothing on this page will change the state of this

            this.Log.AddInfoMsg("==== Configuration Template Page Initialized ====");
        }

        /// <summary>
        /// Method is executed when the user control is loaded. Sets the default values for the
        /// buttons depending on the template policy, or the edited policy. 
        /// </summary>
        private void SetDefaultButtonVals(object sender, EventArgs e)
        {
            // **** This function is run on UI load  *** //

            this.Policy._PolicyType = this._MainWindow.Policy._PolicyType; 
            this.Policy._PolicyTemplate = this._MainWindow.Policy._PolicyTemplate; 
            this.Policy.EditPolicyPath = this._MainWindow.Policy.EditPolicyPath;

            this.Policy.ConfigRules = initRulesDict(); // If supplemental, need to disable the button

            // If unable to read the CI policy, fail gracefully and return to the home page
            if (!this.ReadSetRules(sender, e))
                return; 

            // Enable audit mode by default
            if(this.Policy._PolicyType == WDAC_Policy.PolicyType.BasePolicy)
            {
                this.Policy.ConfigRules["AuditMode"]["CurrentValue"] = "Enabled";
            }

            // Set HVCI option value
            if (this.Policy.EnableHVCI)
                this.Policy.ConfigRules["HVCI"]["CurrentValue"] = this.Policy.ConfigRules["HVCI"]["AllowedValue"]; 

            Dictionary<string, Dictionary<string, string>>.KeyCollection keys = this.Policy.ConfigRules.Keys;
            foreach (string key in keys)
            {
                // If unsupported, skip
                if (!Convert.ToBoolean(this.Policy.ConfigRules[key]["Supported"]))
                    continue;

                // Get the button (UI element) name to modify the state of the button
                string buttonName = this.Policy.ConfigRules[key]["ButtonMapping"];
                string labelName = "label_" + buttonName.Substring(buttonName.IndexOf('_') + 1); 

                // If the policy rule current value matches the allowed value, rule has been set
                if (this.Policy.ConfigRules[key]["CurrentValue"] == this.Policy.ConfigRules[key]["AllowedValue"]) 
                {
                    // Set button to toggled mode
                    this.Controls.Find(buttonName, true).FirstOrDefault().Tag = "toggle";
                    this.Controls.Find(buttonName, true).FirstOrDefault().BackgroundImage = Properties.Resources.toggle;
                }
                else
                {
                    // Set button to untoggled mode
                    this.Controls.Find(buttonName, true).FirstOrDefault().Tag = "untoggle";
                    this.Controls.Find(buttonName, true).FirstOrDefault().BackgroundImage = Properties.Resources.untoggle_old;
                }

                // Depending on the policy, e.g. supplementals, do not allow user to modify the state of some rule-options
                if (this.Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy ||  this.Policy.siPolicy.PolicyType == global::PolicyType.SupplementalPolicy)
                { 
                    switch(this.Policy.ConfigRules[key]["ValidSupplemental"])
                    {
                        case "True":
                            this.Controls.Find(buttonName, true).FirstOrDefault().Enabled = true;
                            this.Controls.Find(labelName, true).FirstOrDefault().Tag = "Enabled";
                            this.Controls.Find(labelName, true).FirstOrDefault().ForeColor = Color.Black;
                            break;

                        case "False":
                            this.Controls.Find(buttonName, true).FirstOrDefault().Enabled = false;
                            this.Controls.Find(labelName, true).FirstOrDefault().Tag = "Grayed";
                            this.Controls.Find(labelName, true).FirstOrDefault().ForeColor = Color.Gray;
                            break;

                        case "False-NoInherit":
                            this.Controls.Find(buttonName, true).FirstOrDefault().Enabled = false;
                            this.Controls.Find(labelName, true).FirstOrDefault().Tag = "Grayed-NoInherit";
                            this.Controls.Find(labelName, true).FirstOrDefault().ForeColor = Color.Gray;
                            break; 
                    }
                }
            }
        }

        /// <summary>
        /// Sets the state of the rule-option button and its state in the Policy object
        /// </summary>
        /// <param name="buttonName">String containing the name of the toggle button
        /// build operations</param>
        private void SetButtonVal(string buttonName)
        {
            // Have to cycle through all of the keys to find the button name value
            Dictionary<string, Dictionary<string, string>>.KeyCollection keys = this.Policy.ConfigRules.Keys;
            foreach (string key in keys)
            {
                if (this.Policy.ConfigRules[key]["ButtonMapping"] == buttonName) // button name match
                {
                    if (this.Controls.Find(buttonName, true).FirstOrDefault().Tag.ToString() == "untoggle")
                    {
                        // Set button to toggled mode
                        this.Controls.Find(buttonName, true).FirstOrDefault().Tag = "toggle";
                        this.Controls.Find(buttonName, true).FirstOrDefault().BackgroundImage = Properties.Resources.toggle;

                        this.Policy.ConfigRules[key]["CurrentValue"] = this.Policy.ConfigRules[key]["AllowedValue"]; 
                    }
                    else
                    {
                        // Set button to untoggled mode
                        this.Controls.Find(buttonName, true).FirstOrDefault().Tag = "untoggle";
                        this.Controls.Find(buttonName, true).FirstOrDefault().BackgroundImage = Properties.Resources.untoggle_old;

                        this.Policy.ConfigRules[key]["CurrentValue"] = GetOppositeOption(this.Policy.ConfigRules[key]["AllowedValue"]); 
                    }

                    this.Log.AddInfoMsg(String.Format("Rule-Option Setting Changed --- {0}: {1}", key, this.Policy.ConfigRules[key]["CurrentValue"]));
                    break; // break out of foreach, we found the button
                }
            } 
        }

        /// <summary>
        /// All of the mouse hovers over the rule-option labels trigger this method to set the 
        /// option description on the UI. 
        /// </summary>
        private void RuleLabel_Hover(object sender, EventArgs e)
        {
            label_Info.Visible = true;
            string _type = sender.GetType().Name;

            // If hovering on the user control (i.e. not a label, remove label Info text
            // All of the descriptions are stored in  Resources.resx
            if (_type == "ConfigTemplate_Control")
            {
                label_Info.Text = "";
                return;
            }

            if (((Label)sender).Tag.ToString() == "Grayed")
            {
                label_Info.Text = Resources.InvalidSupplementalRule_Info;
                return; 
            }

            if (((Label)sender).Tag.ToString() == "Grayed-NoInherit")
            {
                label_Info.Text = Resources.InvalidSupplementalRule_NoInherit_Info;
                return;
            }

            switch (((Label)sender).Text)
            {
                case "User Mode Code Integrity":
                    label_Info.Text = Resources.UMCI_Info;
                    break;

                case "Boot Menu Protection":
                    label_Info.Text = Resources.UnsupportedRule_Info;
                    break;

                case "Require WHQL":
                    label_Info.Text = Resources.WHQL_Info;
                    break;

                case "Inherit Default Policy":
                    label_Info.Text = Resources.UnsupportedRule_Info;
                    break;

                case "Unsigned System Integrity Policy":
                    label_Info.Text = Resources.UnsignedPolicy_Info;
                    break;

                case "Advanced Boot Options Menu":
                    label_Info.Text = Resources.AdvancedBootOpts_Info;
                    break;

                case "Disable Script Enforcement":
                    label_Info.Text = Resources.ScriptEnforcement_Info;
                    break;

                case "Enforce Store Applications":
                    label_Info.Text = Resources.StoreApps_Info;
                    break;

                case "Managed Installer":
                    label_Info.Text = Resources.ManagedInst_Info;
                    break;

                case "Intelligent Security Graph":
                    label_Info.Text = Resources.ISG_Info;
                    break;

                case "Update Policy without Rebooting":
                    label_Info.Text = Resources.NoReboot_Info;
                    break;

                case "Allow Supplemental Policies":
                    label_Info.Text = Resources.SuppPolicies_Info;
                    break;

                case "Hypervisor-protected Code Integrity    ":
                    label_Info.Text = Resources.HVCI_Info;
                    break;

                case "Disable Flight Signing":
                    label_Info.Text = Resources.FlightSigning_Info;
                    break;

                case "Allow Debug Policy Augmented":
                    label_Info.Text = Resources.UnsupportedRule_Info;
                    break;

                case "Require EV Signers":
                    label_Info.Text = Resources.EVSigners_Info;
                    break;

                case "Boot Audit on Failure":
                    label_Info.Text = Resources.BootAudit_Info;
                    break;

                case "Invalidate EAs on Reboot":
                    label_Info.Text = Resources.InvalidateEAs_Info;
                    break;

                case "Disable Runtime Filepath Rules":
                    label_Info.Text = Resources.RuntimeRules;
                    break;

                case "Dynamic Code Security":
                    label_Info.Text = Resources.DynamicSecurity_Info;
                    break;

                case "Treat Revoked as Unsigned":
                    label_Info.Text = Resources.RevokedAsUnsigned_Info;
                    break; 

                default:
                    label_Info.Text = "";
                    break; 
            }

            // Format the label to fit at the bottom of the page.
            // Set the cut location at the 85th percentile space location. 
            if(label_Info.Text.Length > 135)
            {
                string _tmp = label_Info.Text;
                var idx = new List<int>();
                for (int i = _tmp.IndexOf(' '); i > -1; i = _tmp.IndexOf(' ', i + 1))
                    idx.Add(i);
                int cutLoc = Convert.ToInt32(Math.Round(idx.Count * 0.85)); 
                label_Info.Text = _tmp.Substring(0,idx[cutLoc]) + "\r\n" + _tmp.Substring(idx[cutLoc]+1);
            }
            
        }

        /// <summary>
        /// Makes visible the advanced options panel. 
        /// </summary>
        private void AdvancedOptions_ButtonClick(object sender, EventArgs e)
        {
            // Flip the state of the advanced options panel and the text for the button label
            if (!panel_AdvancedOptions.Visible)
            {
                panel_AdvancedOptions.Visible = true;
                this.label_AdvancedOptions.Text = "- Advanced Options"; 
            }
            else
            {
                panel_AdvancedOptions.Visible = false;
                this.label_AdvancedOptions.Text = "+ Advanced Options"; 
            }
            this.Log.AddInfoMsg("Advanced options clicked.");
        }

        /// <summary>
        /// Parses the RulesDict xml document and extracts the AllowedValue, corresponding button name
        /// whether or not the rule is supported, and the option number. 
        /// </summary>
        /// <returns>Config rules dictionary which the fields from the RulesDict xml file</returns>
        private Dictionary<string, Dictionary<string, string>> initRulesDict()
        {
            // Wiring the button IDs to the corresponding rules
            // Default values and order can be found @: https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/select-types-of-rules-to-create#windows-defender-application-control-policy-rules
            string dictPath = System.IO.Path.Combine(this._MainWindow.ExeFolderPath,"RulesDict.xml");
            Dictionary<string, Dictionary<string, string>> rulesDict = new Dictionary<string, Dictionary<string, string>>();

            try
            {
                XmlTextReader xmlReader = new XmlTextReader(dictPath);
                while (xmlReader.Read())
                {
                    if(xmlReader.Name == "Rules")
                    {
                        while(xmlReader.Read())
                        {
                            if(xmlReader.NodeType == XmlNodeType.Element)
                            {
                                Dictionary<string, string> tempDict = new Dictionary<string, string>();
                                string ruleName = xmlReader.GetAttribute("Name");
                                tempDict.Add("AllowedValue", xmlReader.GetAttribute("AllowedValue"));
                                tempDict.Add("ButtonMapping", xmlReader.GetAttribute("ButtonMapping"));
                                tempDict.Add("CurrentValue", GetOppositeOption(tempDict["AllowedValue"]));
                                tempDict.Add("Supported", xmlReader.GetAttribute("Supported"));
                                tempDict.Add("RuleNumber", xmlReader.GetAttribute("RuleNumber"));
                                tempDict.Add("ValidSupplemental", xmlReader.GetAttribute("ValidSupplemental")); 
                                rulesDict.Add(ruleName, tempDict);
                            }
                        }
                    }         
                }
            }
            catch(Exception e)
            {
                this.Log.AddErrorMsg("Reading RulesDict.xml in initRulesDict() encountered the following error: ", e); 
            }
            return rulesDict; 
        }

        /// <summary>
        /// All of the button trigger events trigger this method. Casts the sender to get the button name and calls the SetButtonVal using the
        /// triggered button name. 
        /// </summary>
        private void Toggle_Button_Click(object sender, EventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            SetButtonVal(buttonName);

            this._MainWindow.Policy.ConfigRules = this.Policy.ConfigRules;
            this.Policy.EnableHVCI = this.Policy.ConfigRules["HVCI"]["CurrentValue"] == "Enabled";
            this._MainWindow.Policy.EnableHVCI = this.Policy.EnableHVCI; 
        }

        /// <summary>
        /// Parses the template or the policy that the user would like to edit. Sets all of the 
        /// CI Policy related fields from the xml document. 
        /// </summary>
        private bool ReadSetRules(object sender, EventArgs e)
        {
            // Read in the pre-set policy rules and HVCI option from either:
            // Template policy schema file IF NEW policy selected
            // Pre-existing base policy IF EDIT policy selected

            string xmlPathToRead = "";

            // If we are editing a policy, read the EditPolicyPath
            // We need to know whether we are editing a base or supplemental policy
            if (this.Policy._PolicyType == WDAC_Policy.PolicyType.Edit)
            {
                xmlPathToRead = this._MainWindow.Policy.EditPolicyPath;
            }
                
            // If we are supplementing a policy, we need to mirror the rule options of the base so they do not conflict
            else if (this.Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy)
            {
                xmlPathToRead = this._MainWindow.Policy.BaseToSupplementPath;
            }
            else
            {
                switch (this.Policy._PolicyTemplate)
                {
                    case WDAC_Policy.NewPolicyTemplate.WindowsWorks:
                        // Windows Works Mode 
                        xmlPathToRead = System.IO.Path.Combine(this._MainWindow.ExeFolderPath, "DefaultWindows_Audit.xml");
                        break;

                    case WDAC_Policy.NewPolicyTemplate.SignedReputable:
                        // Signed and Reputable Mode
                        xmlPathToRead = System.IO.Path.Combine(this._MainWindow.ExeFolderPath, "SignedReputable.xml");
                        break;

                    case WDAC_Policy.NewPolicyTemplate.AllowMicrosoft:
                        // Allow Microsoft mode
                        xmlPathToRead = System.IO.Path.Combine(this._MainWindow.ExeFolderPath, "AllowMicrosoft.xml");
                        break;
                }
            }
                
            this.Log.AddInfoMsg(String.Format("--- Reading Set Rules from {0} ---", xmlPathToRead));

            // Read File
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SiPolicy));
                StreamReader reader = new StreamReader(xmlPathToRead);
                this.Policy.siPolicy = (SiPolicy)serializer.Deserialize(reader);
                reader.Close();
            }
            catch(Exception exp)
            {
                this._MainWindow.Log.AddErrorMsg("Reading the xml CI policy encountered the following error ", exp);
                // Prompt user for additional confirmation
                DialogResult res = MessageBox.Show("The Wizard is unable to read your CI policy xml file. The policy XML is corrupted. ",
                    "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (res == DialogResult.OK)
                    this._MainWindow.ResetWorkflow(sender, e);
                return false; 
            }
            
            // Merge with configRules:
            foreach(var rule in this.Policy.siPolicy.Rules)
            {
                string value = ParseRule(rule.Item.ToString())[0]; 
                string name = ParseRule(rule.Item.ToString())[1];

                if (this.Policy.ConfigRules.ContainsKey(name))
                {
                    if(this.Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy  ||  this.Policy.siPolicy.PolicyType == global::PolicyType.SupplementalPolicy)
                    {
                        // If the policy rule is not a valid supplemental option AND should not be inherited from base, e.g. AllowSupplementals
                        // Set the value to not enabled (Get Opposite Value)
                        if(this.Policy.ConfigRules[name]["ValidSupplemental"] == "False-NoInherit")
                        {
                            this.Policy.ConfigRules[name]["CurrentValue"] = GetOppositeOption(value);
                        }
                        else
                        {
                            this.Policy.ConfigRules[name]["CurrentValue"] = value;
                        }
                    }
                    else
                    {
                        this.Policy.ConfigRules[name]["CurrentValue"] = value;
                    }
                }
            }

            this.Policy.EnableHVCI = this.Policy.siPolicy.HvciOptions > 0; 
            this._MainWindow.Policy.ConfigRules = this.Policy.ConfigRules;
            this._MainWindow.Policy.EnableHVCI = this.Policy.EnableHVCI;
            this._MainWindow.Policy.siPolicy = this.Policy.siPolicy;

            // Copy template to temp folder for reading and writing unless template already in temp folder (event log conversion)
            if(!xmlPathToRead.Contains(this._MainWindow.TempFolderPath))
            {
                string xmlTemplateToWrite = Path.Combine(this._MainWindow.TempFolderPath, Path.GetFileName(xmlPathToRead));
                File.Copy(xmlPathToRead, xmlTemplateToWrite, true);
                this._MainWindow.Policy.TemplatePath = xmlTemplateToWrite;
            }
            else
            {
                this._MainWindow.Policy.TemplatePath = xmlPathToRead; 
            }
            

            return true; 
        }

        /// <summary>
        /// Takes in the allowed value such as Enabled, Disabled, Allowed, Required, and returns the opposite value
        /// such that the option will not be set in the cmdlet. 
        /// </summary>
        /// <param name="allowedOption">Allowed values to set the options to in the Set-RuleOption cmdlet</param>
        /// <returns>String opposite of the allowed string values. Eg) Enabled returns Disabled</returns>
        private string GetOppositeOption(string allowedOption)
        {
            string oppOption = "";

            switch(allowedOption)
            {
                case "Enabled":
                    oppOption =  "Disabled";
                    break;

                case "Disabled":
                    oppOption =  "Enabled";
                    break;

                case "Required":
                    oppOption =  "Disabled";
                    break;

                case "Allowed":
                    oppOption = "Disabled";
                    break; 
            }

            return oppOption; 
        }

        private List<string> ParseRule(string rule)
        {
            List<string> parsedRule = new List<string>(); 
            if (rule.Contains("Enabled"))
            {
                int eop = 7;
                parsedRule.Add(rule.Substring(0, eop)); 
                parsedRule.Add(rule.Substring(eop));
            }

            else if (rule.Contains("Disabled"))
            {
                int eop = 8;
                parsedRule.Add(rule.Substring(0, eop));
                parsedRule.Add(rule.Substring(eop));
            }

            else if (rule.Contains("Allowed"))
            {
                int eop = 7;
                parsedRule.Add(rule.Substring(0, eop));
                parsedRule.Add(rule.Substring(eop));
            }

            else if (rule.Contains("Required"))
            {
                int eop = 8;
                parsedRule.Add(rule.Substring(0, eop));
                parsedRule.Add(rule.Substring(eop));
            }

            else
            {
                Log.AddErrorMsg("Rule Value not found for rule " + rule);
            }
            return parsedRule; 
        }

        /// <summary>
        /// Method to display the audit mode info text. 
        /// </summary>
        private void Display_Audit_Recommendation(object sender, EventArgs e)
        {
            label_Info.Text = "It is recommended to run new policies in audit mode before enforcement to determine the impacts of the policy.";
            label_Info.Visible = true; 
        }

        private void HVCILabel_Click(object sender, EventArgs e)
        {
            // Label for learn more about policy options clicked. Launch msft docs page. 
            try
            {
                string webpage = "https://docs.microsoft.com/windows/security/threat-protection/device-guard/enable-virtualization-based-protection-of-code-integrity";
                System.Diagnostics.Process.Start(webpage);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("Launching webpage for policy options link encountered the following error", exp);
            }
        }

        private void LabelPolicyOptions_Click(object sender, EventArgs e)
        {
            // Label for learn more about policy options clicked. Launch msft docs page. 
            try
            {
                string webpage = "https://docs.microsoft.com/windows/security/threat-protection/" +
                    "windows-defender-application-control/select-types-of-rules-to-create";
                System.Diagnostics.Process.Start(webpage);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("Launching webpage for policy options link encountered the following error", exp);
            }
        }

        private void AdvancedOptions_MouseHover(object sender, EventArgs e)
        {
            Label checkBox = ((Label)sender);
            checkBox.BackColor = Color.WhiteSmoke;
        }

        private void AdvancedOptions_MouseLeave(object sender, EventArgs e)
        {
            Label checkBox = ((Label)sender);
            checkBox.BackColor = Color.White;
        }
    }
}
