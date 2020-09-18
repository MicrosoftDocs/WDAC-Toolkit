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
using WDAC_Wizard.Properties;
using System.Xml;


namespace WDAC_Wizard
{
    public partial class PolicyType : UserControl
    {
        public string BaseToSupplementPath { get; set; } // Path to the supplemental policy on disk

        private MainWindow _MainWindow;
        private WDAC_Policy _Policy;
        private Logger Log; 

        public PolicyType(MainWindow pMainWindow)
        {
            InitializeComponent();
            this._MainWindow = pMainWindow;
            this._Policy = pMainWindow.Policy;
            this.Log = this._MainWindow.Log; 

            this._MainWindow.ErrorOnPage = false;
            this._MainWindow.RedoFlowRequired = false; 
        }

        /// <summary>
        /// Base policy radio button is selected. Sets the policy type to Base policy. 
        /// </summary>
        private void BasePolicy_Selected(object sender, EventArgs e)
        {
            // New base policy selected
            if (this._Policy._PolicyType != WDAC_Policy.PolicyType.BasePolicy)
                this._MainWindow.RedoFlowRequired = true; 

            this._Policy._PolicyType = WDAC_Policy.PolicyType.BasePolicy;
            this._MainWindow.Policy._PolicyType = this._Policy._PolicyType;

            // Update UI to reflect change
            basePolicy_PictureBox.Image = Properties.Resources.radio_on;
            suppPolicy_PictureBox.Image = Properties.Resources.radio_off;
            panelSupplementalPolicy.Visible = false;
            this._MainWindow.ErrorOnPage = false; 
        }

        /// <summary>
        /// Supplemental policy radio button is selected. Sets the policy type to Supplemental. 
        /// Shows the supplemental policy panel. 
        /// </summary>
        private void SupplementalPolicy_Selected(object sender, EventArgs e)
        {
            // Require >= 1903 for multiple policy formats - show UI notification 
            if (this._Policy.GetWinVersion() < 1903)
                MessageBox.Show("The multiple policy format will not work on pre-1903 systems","Multiple Policy Format Attention", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Supplemental policy selected
            if (this._Policy._PolicyType != WDAC_Policy.PolicyType.SupplementalPolicy)
                this._MainWindow.RedoFlowRequired = true;

            this._Policy._PolicyType = WDAC_Policy.PolicyType.SupplementalPolicy;
            this._MainWindow.Policy._PolicyType = this._Policy._PolicyType;

            // Update UI to reflect change
            suppPolicy_PictureBox.Image = Properties.Resources.radio_on;
            basePolicy_PictureBox.Image = Properties.Resources.radio_off; 
            // Show supplemental policy panel to allow user to build against a policy
            reset_panel();
            panelSupplementalPolicy.Visible = true;

            this._MainWindow.ErrorOnPage = true;
            this._MainWindow.ErrorMsg = "Select base policy to extend before continuing."; 
        }

        /// <summary>
        /// Opens the filedialog prompting the user to select the base policy to extend for the 
        /// supplemental policy. 
        /// </summary>
        private void button_Browse_Click(object sender, EventArgs e)
        {
            // Open file dialog to get file or folder path
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            openFileDialog.Title = "Please select your exisiting policy XML file.";
            openFileDialog.CheckPathExists = true;
            //openFileDialog.DefaultExt = "xml";
            openFileDialog.Filter = "Policy File (*.xml)|*.xml"; 
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.BaseToSupplementPath = openFileDialog.FileName;
                textBoxPolicyPath.Text = openFileDialog.FileName;
                
            }
            openFileDialog.Dispose();

            // User has modified the supplemental policy from original, force restart flow
            if(this._MainWindow.Policy.BaseToSupplementPath != this.BaseToSupplementPath)
                this._MainWindow.RedoFlowRequired = true;

            this._MainWindow.Policy.BaseToSupplementPath = this.BaseToSupplementPath;

            // Verification: does the chosen base policy allow supplemental policies
            this.Verified_Label.Visible = true;
            this.Verified_PictureBox.Visible = true; 

            if(isPolicyExtendable(this.BaseToSupplementPath))
            {
                this.Verified_Label.Text = "This base policy allows supplemental policies.";
                this.Verified_PictureBox.Image = Properties.Resources.verified;
                this._MainWindow.ErrorOnPage = false; 
            }

            else
            {
                this.Verified_Label.Text = "This base policy does not allow supplemental policies.";
                this.Verified_PictureBox.Image = Properties.Resources.not_extendable;
                this._MainWindow.ErrorOnPage = true;
                this._MainWindow.ErrorMsg = "Selected base policy does not allow supplemental policies.";
            }

        }

        /// <summary>
        /// Determines if the supplied xml GUID permits supplemental policies. i.e. extendable
        /// </summary>
        /// <param name="basePolPath">Path to the xml CI policy</param>
        /// <returns>Returns true if the GUID allows supplemental policies</returns>
        private bool isPolicyExtendable(string basePolPath)
        {
            // Checks that this policy is 1) a base policy, 2) has the allow supplemental policy rule-option
            bool isBase =false, allowsSupplemental = false;
            string basePolicyID = "8283AC0F", policyID = "36924F1C"; // init and assigned different guids so if not set for whatever reason do not get false positive of being a base
            try
            {
                XmlTextReader xmlReader = new XmlTextReader(basePolPath);
                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (xmlReader.Name)
                            {
                                case "Rules":
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
                                                    if (polRule[1] == "Allow Supplemental Policies")
                                                        allowsSupplemental = true; 
                                                }
                                                break;
                                            case XmlNodeType.EndElement:
                                                eoeCount++;
                                                break;
                                        }
                                    }
                                    break;

                                case "BasePolicyID":
                                    // HVCI on or off
                                    basePolicyID = xmlReader.ReadElementContentAsString();
                                    this.Log.AddInfoMsg(String.Format("Found Base Policy ID: {0}", basePolicyID));
                                    break;

                                case "PolicyID":
                                    // HVCI on or off
                                    policyID = xmlReader.ReadElementContentAsString();
                                    this.Log.AddInfoMsg(String.Format("Found Policy ID: {0}", policyID));
                                    break;

                            }
                            break;
                    }
                } //end of while
            }

            catch (Exception e)
            {
                this.Log.AddErrorMsg("isPolicyExtendable() encountered the following Exception: ", e);
            }

            this.Log.AddInfoMsg(String.Format("Policy ID allows supplemental: {0}", allowsSupplemental));

            if (basePolicyID == policyID)
                isBase = true;

            if (isBase && allowsSupplemental)   // if both allows supplemental policies, and this policy is not already a supplemental policy (ie. a base)
                return true;
            else
                return false; 
        }

        /// <summary>
        /// Resets the supplemental panel to its original state. 
        /// </summary>
        private void reset_panel()
       {
            this.BaseToSupplementPath = null; 
            this.textBoxPolicyPath.Text = "";
            this.Verified_Label.Visible = false;
            this.Verified_PictureBox.Visible = false;
        }

        /// <summary>
        /// Launches the multi-policy info webpage when the info label is clicked.  
        /// </summary>
        private void SupplementalInfoLabel_Click(object sender, EventArgs e)
        {
            // multi-policy info label clicked. Launch multi-policy info webpage
            try
            {
                string webpage = "https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-defender-application-control/" +
                    "deploy-multiple-windows-defender-application-control-policies";
                System.Diagnostics.Process.Start(webpage);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("Launching webpage for multipolicy link encountered the following error", exp);
            }
        }
    }
}
