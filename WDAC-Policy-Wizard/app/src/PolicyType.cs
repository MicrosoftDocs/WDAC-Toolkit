// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Drawing; 
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
            if (Helper.GetWinVersion() < 1903)
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
            Reset_panel();
            panelSupplName.Visible = true;

            this._MainWindow.ErrorOnPage = true;
            this._MainWindow.ErrorMsg = "Select base policy to extend before continuing."; 
        }

        /// <summary>
        /// Opens the filedialog prompting the user to select the base policy to extend for the 
        /// supplemental policy. 
        /// </summary>
        private void Button_Browse_Click(object sender, EventArgs e)
        {
            // Verify that a Base Policy ID/GUID is not in progress or finished
            // and confirm the user would rather continue with adding a path instead
            if (this._Policy.BasePolicyId != Guid.Empty 
                || !textBoxBasePolicyID.Text.Contains(Properties.Resources.ExampleBasePolicyId))
            {
                DialogResult res = MessageBox.Show(
                    "Adding a Base Policy by path will remove the ID entered.\r\nAre you sure you want to continue?",
                    "WDAC Wizard",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (res != DialogResult.Yes)
                {
                    return;
                }

                this._Policy.BasePolicyId = Guid.Empty; 
                TextBoxBasePolicyID_Reformat();

                // Log state
                this._MainWindow.Log.AddInfoMsg("New supplemental policy flow. Clearing Policy ID and electing for base path");
            }

            // Hide the validation panel
            basePolicyValidation_Panel.Visible = false;

            // Open file dialog to get file or folder path
            string policyPath = Helper.BrowseForSingleFile(Properties.Resources.OpenXMLFileDialogTitle, Helper.BrowseFileType.Policy);
  
            if (String.IsNullOrEmpty(policyPath))
            {
                return; 
            }

            this.BaseToSupplementPath = policyPath;
            this.textBoxBasePolicyPath.Text = policyPath;

            // Show right side of the text
            if(this.textBoxBasePolicyPath.TextLength > 0)
            {
                this.textBoxBasePolicyPath.SelectionStart = this.textBoxBasePolicyPath.TextLength - 1;
                this.textBoxBasePolicyPath.ScrollToCaret();
            }
            
            // User has modified the supplemental policy from original, force restart flow
            if (this._MainWindow.Policy.BaseToSupplementPath != this.BaseToSupplementPath)
            {
                this._MainWindow.RedoFlowRequired = true;
            }

            this._MainWindow.Policy.BaseToSupplementPath = this.BaseToSupplementPath;
            CheckPolicy_Recur(0); 
        }

        /// <summary>
        /// Recurrsively checks the base policy to determine if the base allows for supplemental policies. If not, 
        /// the Wizard will prompt the user to modify the base to add the supplemental option
        /// </summary>
        private void CheckPolicy_Recur(int count)
        {
            // Verification: does the chosen base policy allow supplemental policies
            this.Verified_Label.Visible = true;
            this.Verified_PictureBox.Visible = true;

            int IsPolicyExtendableCode = IsPolicyExtendable(this.BaseToSupplementPath);

            if (IsPolicyExtendableCode == 0 && count < 2)
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
            else if (IsPolicyExtendableCode == 1 && count < 1)
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
                    // Run command Set-RuleOption -Option 17, check IsPolicyExtendable again
                    this._MainWindow.Log.AddInfoMsg("Attempting to convert the base policy to one that is extendable");
                    bool success = AddSupplementalOption(this.BaseToSupplementPath);

                    // If adding supplemental option was unsuccessful for any reason
                    if(!success)
                    {
                        DialogResult _res = MessageBox.Show(String.Format("The Wizard was unable to add the 'Allow Supplemental Policy Option' to {0}.", Path.GetFileName(this.BaseToSupplementPath)),
                            "Something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this._MainWindow.ErrorOnPage = true;
                        this._MainWindow.ErrorMsg = "The Wizard was unable to add the 'Allow Supplemental Policy Option'.";

                        return; 
                    }

                    CheckPolicy_Recur(++count); 
                }
            }
            else
            {
                this.Verified_Label.Text = "This policy does not support supplemental policies. Please select a multi-policy format base policy to supplement.";
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
        private int IsPolicyExtendable(string basePolPath)
        {
            // Checks that this policy is
            // 1) a multi-policy format base policy
            // 2) has the allow supplemental policy rule-option
            WDAC_Policy policyToSupplement = new WDAC_Policy();
            bool allowsSupplemental = false; 
            // Read File
            try
            {
                policyToSupplement.siPolicy = Helper.DeserializeXMLtoPolicy(basePolPath);
            }
            catch (Exception exp)
            {
                this._MainWindow.Log.AddErrorMsg("Reading the xml CI policy encountered the following error ", exp);
                // Prompt user for additional confirmation
                DialogResult res = MessageBox.Show("The Wizard is unable to read your base CI policy xml file. The policy XML appears to be corrupted.",
                    "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return 99;
            }

            // Catch null/bad policies
            if(policyToSupplement.siPolicy == null)
            {
                // Prompt user for additional confirmation
                DialogResult res = MessageBox.Show("The Wizard is unable to read your base CI policy xml file. The policy XML appears to be corrupted.",
                    "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return 99;
            }

            this.Log.AddInfoMsg(String.Format("IsPolicyExtendable -- Policy Type: {0}", policyToSupplement.siPolicy.PolicyType.ToString()));
            
            if(policyToSupplement.siPolicy.PolicyType.ToString().Contains("Supplemental"))
            {
                // Policy is not base -- not going to fix this case
                this.Log.AddInfoMsg("IsPolicyExtendable -- returns error code 2 (is a supplemental policy)");
                return 2;
            }

            // Policy is not multi-policy format -- not going to fix this case
            if(String.IsNullOrEmpty(policyToSupplement.siPolicy.PolicyID)
                || policyToSupplement.siPolicy.PolicyID.Contains(Properties.Resources.LegacyPolicyID_GUID))
            {
                this.Log.AddInfoMsg("IsPolicyExtendable -- returns error code 3 (legacy GUID)");
                return 3;
            }

            if(policyToSupplement.HasRuleType(OptionType.EnabledAllowSupplementalPolicies))
            {
                allowsSupplemental = true;
            }

            // if both allows supplemental policies, and this policy is not already a supplemental policy (ie. a base)
            if (allowsSupplemental)
            {
                this.Log.AddInfoMsg("IsPolicyExtendable -- returns error code 0 (allows supplemental)"); 
                return 0; 
            }
            else
            {
                // Policy does not have the supplemental rule option -- can fix this case
                this.Log.AddInfoMsg("IsPolicyExtendable -- returns error code 1 (multi-base setting supplemental)");
                return 1; 
            }
        }

        /// <summary>
        /// Resets the supplemental panel to its original state. 
        /// </summary>
        private void Reset_panel()
       {
            this.BaseToSupplementPath = null; 
            this.textBoxBasePolicyPath.Text = "";
            this.Verified_Label.Visible = false;
            this.Verified_PictureBox.Visible = false;

            // Set default paths once, unless explicitly turned off in settings
            if (Properties.Settings.Default.useDefaultStrings)
            {
                this._Policy.SchemaPath = GetDefaultPath("Supplemental_Policy", 0);
                this._Policy.PolicyName = String.Format("{0}_{1}", "My Supplemental Policy", Helper.GetFormattedDate());

                // These will trigger the textChange events
                this.textBoxSuppPath.Text = this._Policy.SchemaPath;
                // Show right side of the text
                if(this.textBoxSuppPath.TextLength > 0)
                {
                    this.textBoxSuppPath.SelectionStart = this.textBoxSuppPath.TextLength - 1;
                    this.textBoxSuppPath.ScrollToCaret();
                }
                
                this.textBox_PolicyName.Text = this._Policy.PolicyName;
                this._MainWindow.Policy.SchemaPath = this._Policy.SchemaPath;

                // Once the supp schema path is set, show panel to select base to supplement
                this.panelSuppl_Base.Visible = true; 
            }

            this._MainWindow.Policy._PolicyTemplate = this._Policy._PolicyTemplate;
        }

        /// <summary>
        /// Determines a default path for the edited policy on disk. Determines if that path exits, and recurrsively determines a new path.   
        /// </summary>
        /// <returns> Returns a unique path to policy after editing it</returns>
        private string GetDefaultPath(string policyTemplate, int nAttempts)
        {
            string proposedPath;

            if (nAttempts == 0)
            {
                proposedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    String.Format("{0}{1}.xml", policyTemplate, Helper.GetFormattedDate()));
            }
            else
            {
                proposedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    String.Format("{0}{1}_{2}.xml", policyTemplate, Helper.GetFormattedDate(), nAttempts));
            }

            if (File.Exists(proposedPath))
            {
                return GetDefaultPath(policyTemplate, ++nAttempts);
            }
            else
            {
                return proposedPath;
            }       
        }

        /// <summary>
        /// Browse button clicked. Open the save dialog to allow user to select a path to save their policy 
        /// </summary>
        private void Button_BrowseSupp_Click(object sender, EventArgs e)
        {
            // Save dialog box pressed
            string policyPath = Helper.SaveSingleFile(Properties.Resources.SaveXMLFileDialogTitle, Helper.BrowseFileType.Policy);

            // If cancel button is selected by user, or path does not exist prevent unhandled error
            if (String.IsNullOrEmpty(policyPath))
            {
                return;
            }

            this._MainWindow.Policy.SchemaPath = policyPath;
            this.textBoxSuppPath.Text = policyPath;

            // Show right side of the text
            if(this.textBoxSuppPath.TextLength > 0)
            {
                this.textBoxSuppPath.SelectionStart = this.textBoxSuppPath.TextLength - 1;
                this.textBoxSuppPath.ScrollToCaret();
            }

            // Show panel if path is set
            this.panelSuppl_Base.Visible = true; 
        }

        /// <summary>
        /// Add the "Allow Supplemental" policy rule-option to a base policy on disk by de-serializing and re-serializing the policy
        /// </summary>
        private bool AddSupplementalOption(string basePath)
        {
            SiPolicy tempSiPolicy; 

            try
            {
                // Deserialize the policy into SiPolicy object
                tempSiPolicy = Helper.DeserializeXMLtoPolicy(basePath); 

                // Append allow supplemental rule to SiPolicy obj
                RuleType[] existingRules = tempSiPolicy.Rules;

                RuleType allowSupplementalsRule = new RuleType();
                allowSupplementalsRule.Item = OptionType.EnabledAllowSupplementalPolicies;
                Array.Resize(ref existingRules, existingRules.Length + 1);
                existingRules[existingRules.Length-1] = allowSupplementalsRule;
                tempSiPolicy.Rules = existingRules;

                // Check if path is user-writeable. If it is not, copy to documents and update path to BasePolicyToSupplement
                if (!Helper.IsUserWriteable(Path.GetDirectoryName(basePath)))
                {
                    basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.GetFileName(basePath));
                    this.BaseToSupplementPath = basePath; 
                }

                // Serialize the policy back to xml policy format
                Helper.SerializePolicytoXML(tempSiPolicy, basePath); 
                
                return true; 
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("SetPolicyRuleOptions() caught the following exception ", exp);
                return false; 
            }
        }

        // <summary>
        /// Policy name has been modified by the user. Update the policy name
        /// </summary>
        private void TextBox_PolicyName_TextChanged(object sender, EventArgs e)
        {
            // Policy Friend Name
            this._MainWindow.Policy.PolicyName = textBox_PolicyName.Text;
        }

        // <summary>
        /// User is creating a multi-policy formated policy. Update the UI to reflect the desired state
        /// </summary>
        private void MultipleFormat_ButtonClick(object sender, EventArgs e)
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

        // <summary>
        /// User is creating a single-policy formated policy. Update the UI to reflect the desired state and hide the multipolicy panel
        /// </summary>
        private void SingleFormat_ButtonClick(object sender, EventArgs e)
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

        // <summary>
        /// Method fires on page load and sets the default state of the UI determined by last time the page was loaded
        /// </summary>
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

        // <summary>
        /// User has clicked the "Learn More" label. Launch the multi-policy help online doc
        /// </summary>
        private void Label_LearnMore_Click(object sender, EventArgs e)
        {
            // multi-policy info label clicked. Launch multi-policy info webpage
            try
            {
                string webpage = "https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/" +
                    "deploy-multiple-windows-defender-application-control-policies";
                System.Diagnostics.Process.Start(webpage);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("Launching webpage for multipolicy link encountered the following error", exp);
            }
        }

        /// <summary>
        /// Fires when the base policy ID text entered changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxBasePolicyID_TextChanged(object sender, EventArgs e)
        {
            // Validate the text entered
            if(Guid.TryParse(textBoxBasePolicyID.Text, out Guid result))
            {
                this._Policy.BasePolicyId = result; 
                this._MainWindow.ErrorOnPage = false;

                // Log state
                this._MainWindow.Log.AddInfoMsg(String.Format("New supplemental policy flow. Valid policy ID entered: ", result.ToString()));
            }
            else
            {
                this._MainWindow.ErrorOnPage = true;
                this._MainWindow.ErrorMsg = Properties.Resources.InvalidBasePolicyId; 
            }
        }

        /// <summary>
        /// Textbox clicked. Clears the example text, if applicable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxBasePolicyID_Selected(object sender, EventArgs e)
        {
            // Check if we already have a base policy path and confirm the user would rather
            // continue with adding a GUID instead
            if (!String.IsNullOrEmpty(this._Policy.BaseToSupplementPath))
            {
                DialogResult res = MessageBox.Show(
                    String.Format("Adding a Base Policy ID will remove the following XML path: {0}. " +
                    "\r\nAre you sure you want to continue?", this._Policy.BaseToSupplementPath),
                    "WDAC Wizard",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (res != DialogResult.Yes)
                {
                    return;
                }

                // Otherwise, clear the base policy path text and objects
                this.BaseToSupplementPath = String.Empty;
                this._Policy.BaseToSupplementPath = String.Empty;
                textBoxBasePolicyPath.Clear();

                // Hide the validation panel
                basePolicyValidation_Panel.Visible = false;

                // Log state
                this._MainWindow.Log.AddInfoMsg("New supplemental policy flow. Clearing XML path and electing for base policy ID");
            }

            // Clear the example text, if applicable
            // Set the text color to black for the user
            if (textBoxBasePolicyID.Text.Contains(Properties.Resources.ExampleBasePolicyId))
            {
                textBoxBasePolicyID.Clear();
                textBoxBasePolicyID.ForeColor = System.Drawing.Color.Black;
            }
        }

        /// <summary>
        /// Resets the text to the original eg. {} text and color of the text
        /// </summary>
        private void TextBoxBasePolicyID_Reformat()
        {
            // Reset the text and font to original
            textBoxBasePolicyID.Text = Properties.Resources.ExampleBasePolicyId;
            textBoxBasePolicyID.ForeColor = System.Drawing.SystemColors.WindowFrame; 
        }

        /// <summary>
        /// Fires on form leaving event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxBasePolicyID_Leave(object sender, EventArgs e)
        {
            // If nothing was input, reformat the text box. 
            // Otherwise, leave it as is - good Id or in progress

            if(String.IsNullOrEmpty(textBoxBasePolicyID.Text))
            {
                TextBoxBasePolicyID_Reformat();
            }
        }


        /// <summary>
        /// Form painting. Occurs on Form.Refresh, Load and Focus. 
        /// Used for UI element changes for Dark and Light Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PolicyType_Validated(object sender, EventArgs e)
        {
            // Set Controls Color (e.g. Radio Buttons)
            SetControlsColor();

            // Set UI for the 'button_Browse' Button
            Setbutton_BrowseUI();

            // Set UI for the 'button_Browse_Supp' Button
            Setbutton_Browse_SuppUI();

            // Set Labels Color
            List<Label> labels = new List<Label>();
            GetLabelsRecursive(this, labels);
            SetLabelsColor(labels);

            // Set Textboxes Color
            List<TextBox> textBoxes = new List<TextBox>();
            GetTextBoxesRecursive(this, textBoxes);
            SetTextBoxesColor(textBoxes);

            // Set PolicyType Form back color
            SetFormBackColor();
        }

        /// <summary>
        /// Sets the color of the controls
        /// </summary>
        /// <param name="labels"></param>
        private void SetControlsColor()
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                foreach (Control control in this.Controls)
                {
                    if (control is RadioButton radioButton
                        && (radioButton.Tag == null || radioButton.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        radioButton.ForeColor = Color.White;
                        radioButton.BackColor = Color.FromArgb(15, 15, 15);
                    }
                }
            }

            // Light Mode
            else
            {
                foreach (Control control in this.Controls)
                {
                    if (control is RadioButton radioButton
                        && (radioButton.Tag == null || radioButton.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        radioButton.ForeColor = Color.Black;
                        radioButton.BackColor = Color.White;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the colors for the button_Browse Button which depends on the 
        /// state of Light and Dark Mode
        /// </summary>
        public void Setbutton_BrowseUI()
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                button_Browse.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
                button_Browse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
                button_Browse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
                button_Browse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                button_Browse.ForeColor = System.Drawing.Color.DodgerBlue;
                button_Browse.BackColor = System.Drawing.Color.Transparent;
            }

            // Light Mode
            else
            {
                button_Browse.FlatAppearance.BorderColor = System.Drawing.Color.Black;
                button_Browse.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
                button_Browse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
                button_Browse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                button_Browse.ForeColor = System.Drawing.Color.Black;
                button_Browse.BackColor = System.Drawing.Color.WhiteSmoke;
            }
        }

        /// <summary>
        /// Sets the colors for the button_Browse_Supp Button which depends on the 
        /// state of Light and Dark Mode
        /// </summary>
        public void Setbutton_Browse_SuppUI()
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                button_Browse_Supp.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
                button_Browse_Supp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
                button_Browse_Supp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
                button_Browse_Supp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                button_Browse_Supp.ForeColor = System.Drawing.Color.DodgerBlue;
                button_Browse_Supp.BackColor = System.Drawing.Color.Transparent;
            }

            // Light Mode
            else
            {
                button_Browse_Supp.FlatAppearance.BorderColor = System.Drawing.Color.Black;
                button_Browse_Supp.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
                button_Browse_Supp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
                button_Browse_Supp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                button_Browse_Supp.ForeColor = System.Drawing.Color.Black;
                button_Browse_Supp.BackColor = System.Drawing.Color.WhiteSmoke;
            }
        }
        
        /// <summary>
        /// Gets all of the labels on the form recursively
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="labels"></param>
        private void GetLabelsRecursive(Control parent, List<Label> labels)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Label label)
                {
                    labels.Add(label);
                }
                else
                {
                    GetLabelsRecursive(control, labels);
                }
            }
        }        

        /// <summary>
        /// Sets the color of the labels defined in the provided List
        /// </summary>
        /// <param name="labels"></param>
        private void SetLabelsColor(List<Label> labels)
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                foreach (Label label in labels)
                {
                    if (label.Tag == null || label.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        label.ForeColor = Color.White;
                        label.BackColor = Color.FromArgb(15, 15, 15);
                    }
                }
            }

            // Light Mode
            else
            {
                foreach (Label label in labels)
                {
                    if (label.Tag == null || label.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        label.ForeColor = Color.Black;
                        label.BackColor = Color.White;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all of the labels on the form recursively
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="labels"></param>
        private void GetTextBoxesRecursive(Control parent, List<TextBox> textBoxes)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBoxes.Add(textBox);
                }
                else
                {
                    GetTextBoxesRecursive(control, textBoxes);
                }
            }
        }

        /// <summary>
        /// Sets the color of the labels defined in the provided List
        /// </summary>
        /// <param name="labels"></param>
        private void SetTextBoxesColor(List<TextBox> textBoxes)
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                foreach (TextBox textBox in textBoxes)
                {
                    if (textBox.Tag == null || textBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        textBox.ForeColor = Color.White;
                        textBox.BackColor = Color.FromArgb(15, 15, 15);
                    }
                }
            }

            // Light Mode
            else
            {
                foreach (TextBox textBox in textBoxes)
                {
                    if (textBox.Tag == null || textBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        textBox.ForeColor = Color.Black;
                        textBox.BackColor = Color.White;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the Back Color of the form depending on the
        /// state of Dark and Light Mode
        /// </summary>
        private void SetFormBackColor()
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                BackColor = Color.FromArgb(15, 15, 15);
            }

            // Light Mode
            else
            {
                BackColor = Color.White;
            }
        }
    }
}
