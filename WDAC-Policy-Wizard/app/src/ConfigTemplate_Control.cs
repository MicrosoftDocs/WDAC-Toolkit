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

namespace WDAC_Wizard
{
    public partial class ConfigTemplate_Control : UserControl
    {        
        public Logger Log { get; set; }
        public MainWindow _MainWindow;
        private WDAC_Policy _Policy; 

        public ConfigTemplate_Control(MainWindow pMainWindow)
        {
            InitializeComponent();

            this._Policy = new WDAC_Policy();
            this._MainWindow = pMainWindow;
            this.Log = this._MainWindow.Log;

            this._MainWindow.ErrorOnPage = false;
            this._MainWindow.RedoFlowRequired = false; // Nothing on this page will change the state of this
        }

        /// <summary>
        /// Method is executed when the user control is loaded. Sets the default values for the
        /// buttons depending on the template policy, or the edited policy. 
        /// </summary>
        private void SetDefaultButtonVals(object sender, EventArgs e)
        {
            // **** This function is run on UI load  *** //

            this._Policy._PolicyType = this._MainWindow.Policy._PolicyType;
            this._Policy._PolicyTemplate = this._MainWindow.Policy._PolicyTemplate; 
            this._Policy.EditPolicyPath = this._MainWindow.Policy.EditPolicyPath;

            this._Policy.ConfigRules = initRulesDict();
            readSetRules();

            Dictionary<string, Dictionary<string, string>>.KeyCollection keys = this._Policy.ConfigRules.Keys;
            foreach (string key in keys)
            {
                // If unsupported, skip
                if (!Convert.ToBoolean(this._Policy.ConfigRules[key]["Supported"]))
                    continue; 

                // If the policy rule current value matches the allowed value, rule has been set
                string buttonName = this._Policy.ConfigRules[key]["ButtonMapping"];
                if (this._Policy.ConfigRules[key]["CurrentValue"] == this._Policy.ConfigRules[key]["AllowedValue"]) 
                {
                    // Set button to toggled mode
                    this.Controls.Find(buttonName, true).FirstOrDefault().Tag = "toggle";
                    this.Controls.Find(buttonName, true).FirstOrDefault().BackgroundImage = Properties.Resources.toggle;
                }
                else
                {
                    // Set button to untoggled mode
                    this.Controls.Find(buttonName, true).FirstOrDefault().Tag = "untoggle";
                    this.Controls.Find(buttonName, true).FirstOrDefault().BackgroundImage = Properties.Resources.untoggle;
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
            Dictionary<string, Dictionary<string, string>>.KeyCollection keys = this._Policy.ConfigRules.Keys;
            foreach (string key in keys)
            {
                if (this._Policy.ConfigRules[key]["ButtonMapping"] == buttonName) // button name match
                {
                    if (this.Controls.Find(buttonName, true).FirstOrDefault().Tag.ToString() == "untoggle")
                    {
                        // Set button to toggled mode
                        this.Controls.Find(buttonName, true).FirstOrDefault().Tag = "toggle";
                        this.Controls.Find(buttonName, true).FirstOrDefault().BackgroundImage = Properties.Resources.toggle;

                        this._Policy.ConfigRules[key]["CurrentValue"] = this._Policy.ConfigRules[key]["AllowedValue"]; 
                    }
                    else
                    {
                        // Set button to untoggled mode
                        this.Controls.Find(buttonName, true).FirstOrDefault().Tag = "untoggle";
                        this.Controls.Find(buttonName, true).FirstOrDefault().BackgroundImage = Properties.Resources.untoggle;

                        this._Policy.ConfigRules[key]["CurrentValue"] = GetOppositeOption(this._Policy.ConfigRules[key]["AllowedValue"]); 
                    }
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
            }

            // Format the label to fit at the bottom of the page.
            // Set the cut location at the 75th percentile space location. 
            if(label_Info.Text.Length > 135)
            {
                string _tmp = label_Info.Text;
                var idx = new List<int>();
                for (int i = _tmp.IndexOf(' '); i > -1; i = _tmp.IndexOf(' ', i + 1))
                    idx.Add(i);
                int cutLoc = Convert.ToInt32(Math.Round(idx.Count * 0.75)); 
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
            // Default values and order can be found @: https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-defender-application-control/select-types-of-rules-to-create#windows-defender-application-control-policy-rules
            string dictPath = "./RulesDict.xml";
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

            this._MainWindow.Policy.ConfigRules = this._Policy.ConfigRules;
            this._Policy.EnableHVCI = this._Policy.ConfigRules["HVCI"]["CurrentValue"] == "Enabled";
            this._MainWindow.Policy.EnableHVCI = this._Policy.EnableHVCI; 

        }

        /// <summary>
        /// Parses the template or the policy that the user would like to edit. Sets all of the 
        /// CI Policy related fields from the xml document. 
        /// </summary>
        private void readSetRules()
        {
            // Read in the pre-set policy rules and HVCI option from either:
            // Template policy schema file IF NEW policy selected
            // Pre-existing base policy IF EDIT policy selected

            string xmlPathToRead = "";

            if(this._Policy._PolicyType == WDAC_Policy.PolicyType.Edit)
                xmlPathToRead = this._MainWindow.Policy.EditPolicyPath;
            
            else 
            {
                switch (this._Policy._PolicyTemplate)
                {
                    case WDAC_Policy.NewPolicyTemplate.WindowsWorks:
                        // Windows Works Mode 
                        xmlPathToRead = "./DefaultWindows_Audit.xml";
                        break;
                    
                    case WDAC_Policy.NewPolicyTemplate.NightsWatch:
                        // Signed and Reputable Mode
                        xmlPathToRead = "./NightsWatch.xml";
                        break;

                    case WDAC_Policy.NewPolicyTemplate.AllowMicrosoft:
                        // Allow Microsoft mode
                        xmlPathToRead = "./AllowMicrosoft.xml"; 
                        break;
                }
            }
                
            this.Log.AddInfoMsg(String.Format("--- Reading Set Rules from {0} ---", xmlPathToRead));

            Dictionary<string, string> policyRules = new Dictionary<string, string>();
            string hvci_Val = String.Empty;

            // Parsing the xml document
            try
            {
                XmlTextReader xmlReader = new XmlTextReader(xmlPathToRead);
                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (xmlReader.Name)
                            {
                                case "Rules":
                                    // Handle the policy rules - SUCCESS
                                    int eoeCount = 0;
                                    while (xmlReader.Read() && eoeCount < 3)
                                    {
                                        switch (xmlReader.NodeType)
                                        {
                                            case XmlNodeType.Element:
                                                eoeCount = 0;
                                                break;
                                            case XmlNodeType.Text:
                                                {
                                                    eoeCount = 0;

                                                    // Rule in this text - add to dictionary
                                                    string optionLine = xmlReader.Value;
                                                    string[] polRule = optionLine.Split(':');
                                                    policyRules[polRule[1]] = polRule[0];
                                                    this.Log.AddInfoMsg(String.Format("Found Pre-set Rule-Option Pair {0}:{1}", polRule[1], polRule[0]));
                                                }
                                                break;
                                            case XmlNodeType.EndElement:
                                                eoeCount++;
                                                break;
                                        }
                                    }
                                    break;

                                case "HvciOptions":
                                    // HVCI on or off
                                    hvci_Val = xmlReader.ReadElementContentAsString();
                                    this.Log.AddInfoMsg(String.Format("Found HVCI Value {0}", hvci_Val));
                                    break;

                            }
                            break;
                    }
                } //end of while

                xmlReader.Dispose(); 
            }

            catch(Exception e)
            {
                this.Log.AddErrorMsg("ReadSetRules() encountered the following Exception: ", e);
            }

            
            // Merge with configRules:
            Dictionary<string, string>.KeyCollection prKeys = new Dictionary<string, string>.KeyCollection(policyRules);
            foreach(string prKey in prKeys)
            {
                if (this._Policy.ConfigRules.ContainsKey(prKey))
                    this._Policy.ConfigRules[prKey]["CurrentValue"] = policyRules[prKey]; 
            }

            if(hvci_Val != String.Empty)
                this._Policy.EnableHVCI = Convert.ToUInt16(hvci_Val) > 0; // Convert to bool

            this._MainWindow.Policy.ConfigRules = this._Policy.ConfigRules;
            this._MainWindow.Policy.EnableHVCI = this._Policy.EnableHVCI;

            // Copy template to temp folder for reading and writing
            string xmlTemplateToWrite = Path.Combine(this._MainWindow.TempFolderPath, Path.GetFileName(xmlPathToRead));
            File.Copy(xmlPathToRead, xmlTemplateToWrite, true);
            this._MainWindow.Policy.TemplatePath = xmlTemplateToWrite; 
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

        /// <summary>
        /// Method to display the audit mode info text. 
        /// </summary>
        private void Display_Audit_Recommendation(object sender, EventArgs e)
        {
            label_Info.Text = "We recommend that you run all new policies in audit mode before enforcement to determine the impacts of the policies.";
            label_Info.Visible = true; 
        }

        private void HVCILabel_Click(object sender, EventArgs e)
        {

        }

        private void LabelPolicyOptions_Click(object sender, EventArgs e)
        {
            // Label for learn more about policy options clicked. Launch msft docs page. 
            try
            {
                string webpage = "https://docs.microsoft.com/en-us/windows/security/threat-protection/" +
                    "windows-defender-application-control/select-types-of-rules-to-create";
                System.Diagnostics.Process.Start(webpage);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("Launching webpage for policy options link encountered the following error", exp);
            }
        }
    }
}
