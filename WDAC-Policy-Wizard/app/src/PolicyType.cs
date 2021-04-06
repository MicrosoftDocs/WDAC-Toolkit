// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;

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

            this.Log.AddInfoMsg("==== Policy Type Page Initialized ====");
        }

        /// <summary>
        /// Base policy radio button is selected. Sets the policy type to Base policy. 
        /// </summary>
        private void BasePolicy_Selected(object sender, EventArgs e)
        {
            this.suppPolicy_PictureBox.Tag = "Unselected";
            this.basePolicy_PictureBox.Tag = "Selected";

            // New base policy selected
            if (this._Policy._PolicyType != WDAC_Policy.PolicyType.BasePolicy)
                this._MainWindow.RedoFlowRequired = true; 

            this._Policy._PolicyType = WDAC_Policy.PolicyType.BasePolicy;
            this._MainWindow.Policy._PolicyType = this._Policy._PolicyType;

            // Update UI to reflect change
            basePolicy_PictureBox.Image = Properties.Resources.radio_on;
            suppPolicy_PictureBox.Image = Properties.Resources.radio_off;
            panelSupplName.Visible = false;
            this._MainWindow.ErrorOnPage = false; 
        }

        /// <summary>
        /// Supplemental policy radio button is selected. Sets the policy type to Supplemental. 
        /// Shows the supplemental policy panel. 
        /// </summary>
        private void SupplementalPolicy_Selected(object sender, EventArgs e)
        {
            this.suppPolicy_PictureBox.Tag = "Selected";
            this.basePolicy_PictureBox.Tag = "Unselected"; 

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
            panelSupplName.Visible = true;

            this._MainWindow.ErrorOnPage = true;
            this._MainWindow.ErrorMsg = "Select base policy to extend before continuing."; 
        }

        /// <summary>
        /// Opens the filedialog prompting the user to select the base policy to extend for the 
        /// supplemental policy. 
        /// </summary>
        private void button_Browse_Click(object sender, EventArgs e)
        {
            // Hide the validation panel
            basePolicyValidation_Panel.Visible = false; 

            // Open file dialog to get file or folder path
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Please select your exisiting policy XML file.";
            openFileDialog.CheckPathExists = true;
            //openFileDialog.DefaultExt = "xml";
            openFileDialog.Filter = "Policy File (*.xml)|*.xml"; 
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.BaseToSupplementPath = openFileDialog.FileName;
                this.textBoxBasePolicyPath.Text = openFileDialog.FileName;
                // Show right side of the text
                this.textBoxBasePolicyPath.SelectionStart = this.textBoxBasePolicyPath.TextLength - 1;
                this.textBoxBasePolicyPath.ScrollToCaret();

            }
            openFileDialog.Dispose();

            // User has modified the supplemental policy from original, force restart flow
            if(this._MainWindow.Policy.BaseToSupplementPath != this.BaseToSupplementPath)
                this._MainWindow.RedoFlowRequired = true;

            this._MainWindow.Policy.BaseToSupplementPath = this.BaseToSupplementPath;
            CheckPolicy_Recur(0); 
        }

        private void CheckPolicy_Recur(int count)
        {
            // Verification: does the chosen base policy allow supplemental policies
            this.Verified_Label.Visible = true;
            this.Verified_PictureBox.Visible = true;

            int isPolicyExtendableCode = isPolicyExtendable(this.BaseToSupplementPath);

            if (isPolicyExtendableCode == 0 && count < 2)
            {
                this.Verified_Label.Text = "This base policy allows supplemental policies.";
                this.Verified_PictureBox.Image = Properties.Resources.verified;
                this._MainWindow.ErrorOnPage = false;

                if(count == 1)
                {
                    DialogResult res = MessageBox.Show(String.Format("The Wizard has successfully added the Allow Supplemental Policy rule to {0}.", Path.GetFileName(this.BaseToSupplementPath)),
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
            }
            else if (isPolicyExtendableCode == 1 && count < 1)
            {
                this.Verified_Label.Text = "This base policy does not allow supplemental policies.";
                this.Verified_PictureBox.Image = Properties.Resources.not_extendable;
                this._MainWindow.ErrorOnPage = true;
                this._MainWindow.ErrorMsg = "Selected base policy does not allow supplemental policies.";

                // Prompt user if the Wizard should add the supplemental policy rule option to base policy
                DialogResult res = MessageBox.Show(String.Format("The base policy you have selected does not allow supplemental policies.\n\n" +
                    "Would you like the Wizard to add the Allow Supplemental Policy rule to {0}?", Path.GetFileName(this.BaseToSupplementPath)),
                    "Base Policy Issue", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    // Run command Set-RuleOption -Option 17, check isPolicyExtendable again
                    this._MainWindow.Log.AddInfoMsg("Attempting to convert the base policy to one that is extendable");
                    AddSupplementalOption(this.BaseToSupplementPath);
                    CheckPolicy_Recur(++count); 
                }
            }
            else
            {
                this.Verified_Label.Text = "This policy is not a base policy. Please select a base policy to supplement.";
                this.Verified_PictureBox.Image = Properties.Resources.not_extendable;
                this._MainWindow.ErrorOnPage = true;
                this._MainWindow.ErrorMsg = "Selected base policy is not a base policy.";
            }

            basePolicyValidation_Panel.Visible = true; 
        }

        /// <summary>
        /// Determines if the supplied xml GUID permits supplemental policies. i.e. extendable
        /// </summary>
        /// <param name="basePolPath">Path to the xml CI policy</param>
        /// <returns>Returns true if the GUID allows supplemental policies</returns>
        private int isPolicyExtendable(string basePolPath)
        {
            // Checks that this policy is 1) a base policy, 2) has the allow supplemental policy rule-option
            WDAC_Policy _BasePolicy = new WDAC_Policy();
            bool allowsSupplemental = false; 
            // Read File
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SiPolicy));
                StreamReader reader = new StreamReader(basePolPath);
                _BasePolicy.siPolicy = (SiPolicy)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception exp)
            {
                this._MainWindow.Log.AddErrorMsg("Reading the xml CI policy encountered the following error ", exp);
                // Prompt user for additional confirmation
                DialogResult res = MessageBox.Show("The Wizard is unable to read your base CI policy xml file. The policy XML appears to be corrupted.",
                    "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return 99;
            }

            this.Log.AddInfoMsg(String.Format("isPolicyExtendable -- Policy Type: {0}", _BasePolicy.siPolicy.PolicyType.ToString()));
            
            if(_BasePolicy.siPolicy.PolicyType.ToString().Contains("Supplemental"))
            {
                // Policy is not base -- not going to fix this case
                this.Log.AddInfoMsg("isPolicyExtendable -- returns error code 2");
                return 2;
            }

            foreach(var rule in _BasePolicy.siPolicy.Rules)
            {
                if(rule.Item.ToString().Contains("Supplemental"))
                {
                    allowsSupplemental = true;
                    this.Log.AddInfoMsg(String.Format("isPolicyExtendable -- {0}: True", rule.ToString())); 
                    break; 
                }
            }

            // if both allows supplemental policies, and this policy is not already a supplemental policy (ie. a base)
            if (allowsSupplemental)
            {
                this.Log.AddInfoMsg("isPolicyExtendable -- returns error code 0"); 
                return 0; 
            }
            else
            {
                // Policy does not have the supplemental rule option -- can fix this case
                this.Log.AddInfoMsg("isPolicyExtendable -- returns error code 1");
                return 1; 
            }
        }

        /// <summary>
        /// Resets the supplemental panel to its original state. 
        /// </summary>
        private void reset_panel()
       {
            this.BaseToSupplementPath = null; 
            this.textBoxBasePolicyPath.Text = "";
            this.Verified_Label.Visible = false;
            this.Verified_PictureBox.Visible = false;

            // Set default paths once, unless explicitly turned off in settings
            if (Properties.Settings.Default.useDefaultStrings)
            {
                string dateString = this._MainWindow.formatDate(false);
                this._Policy.SchemaPath = GetDefaultPath("Supplemental_Policy", 0);
                this._Policy.PolicyName = String.Format("{0}_{1}", "My Supplemental Policy", dateString);

                // These will trigger the textChange events
                this.textBoxSuppPath.Text = this._Policy.SchemaPath;
                this.textBox_PolicyName.Text = this._Policy.PolicyName;
                this._MainWindow.Policy.SchemaPath = this._Policy.SchemaPath;

                // Once the supp schema path is set, show panel to select base to supplement
                this.panelSuppl_Base.Visible = true; 

            }

            this._MainWindow.Policy._PolicyTemplate = this._Policy._PolicyTemplate;
        }

        private string GetDefaultPath(string policyTemplate, int nAttempts)
        {
            string dateString = this._MainWindow.formatDate(false);
            string proposedPath;

            if (nAttempts == 0)
                proposedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    String.Format("{0}{1}.xml", policyTemplate, dateString));
            else
                proposedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    String.Format("{0}{1}_{2}.xml", policyTemplate, dateString, nAttempts));

            if (File.Exists(proposedPath))
                return GetDefaultPath(policyTemplate, ++nAttempts);
            else
                return proposedPath;
        }

        private void button_BrowseSupp_Click(object sender, EventArgs e)
        {
            // Save dialog box pressed
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Your Supplemental Policy File";
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.Filter = "Policy Files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this._MainWindow.Policy.SchemaPath = saveFileDialog.FileName;
                this.textBoxSuppPath.Text = saveFileDialog.FileName;

                // Show right side of the text
                this.textBoxSuppPath.SelectionStart = this.textBoxSuppPath.TextLength - 1;
                this.textBoxSuppPath.ScrollToCaret();

                // Show panel if path is set
                this.panelSuppl_Base.Visible = true; 
            }

            saveFileDialog.Dispose();
        }

        private void AddSupplementalOption(string basePath)
        {
            Runspace policyRuleOptsRunspace = RunspaceFactory.CreateRunspace();
            policyRuleOptsRunspace.Open();
            Pipeline pipeline = policyRuleOptsRunspace.CreatePipeline();

            // Add Rule 17: Enabled:Allow Supplemental Policies
            pipeline.Commands.AddScript(String.Format("Set-RuleOption -FilePath \"{0}\" -Option 17", basePath));

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception e)
            {
                this.Log.AddErrorMsg("CreatePolicyRuleOptions() caught the following exception ", e);
            }
            policyRuleOptsRunspace.Close();
        }

        private void textBox_PolicyName_TextChanged(object sender, EventArgs e)
        {
            // Policy Friend Name
            this._MainWindow.Policy.PolicyName = textBox_PolicyName.Text;
        }


        private void multipleFormat_ButtonClick(object sender, EventArgs e)
        {
            // Show the multi-policy UI panel
            this.panel_MultiPolicy.Visible = true;

            // Set the setting to show this radio button selected next page load
            Properties.Settings.Default.showMultiplePolicyDefault = true; 


            // Just call into the events to reset the UI
            if (this.basePolicy_PictureBox.Tag.ToString().Contains("Unselected"))
            {
                SupplementalPolicy_Selected(sender, e);
            }
            else
            {
                BasePolicy_Selected(sender, e);
            }

            // Set policy format in Policy object
            this._MainWindow.Policy._Format = WDAC_Policy.Format.MultiPolicy;
            this.Log.AddInfoMsg("Setting WDAC Policy Format to " + this._MainWindow.Policy._Format.ToString());
        }

        private void singleFormat_ButtonClick(object sender, EventArgs e)
        {
            // UI changes - Hide the panel
            this.panel_MultiPolicy.Visible = false;
            this._MainWindow.ErrorOnPage = false;

            // Set the setting to show this radio button selected next page load
            Properties.Settings.Default.showMultiplePolicyDefault = false;

            // Set policy format in Policy object
            this._MainWindow.Policy._Format = WDAC_Policy.Format.Legacy;
            this.Log.AddInfoMsg("Setting WDAC Policy Format to " + this._MainWindow.Policy._Format.ToString());

            // Set policy type 
            this._MainWindow.Policy._PolicyType = WDAC_Policy.PolicyType.BasePolicy; 
        }

        private void PolicyType_Load(object sender, EventArgs e)
        {
            // On page load, check whether multiple or single policy format was chosen last time the page was loaded
            if(Properties.Settings.Default.showMultiplePolicyDefault)
            {
                this.radioButton_MultiplePolicy.Checked = true;
                this.radioButton_SinglePolicy.Checked = false;
            }
            else
            {
                this.radioButton_MultiplePolicy.Checked = false;
                this.radioButton_SinglePolicy.Checked = true; 
            }
        }

        private void label_LearnMore_Click(object sender, EventArgs e)
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
