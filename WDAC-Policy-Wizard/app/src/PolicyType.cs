// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Drawing; 
using System.Diagnostics;

namespace WDAC_Wizard
{
    public partial class PolicyType : UserControl
    {
        public string BaseToSupplementPath { get; set; } // Path to the supplemental policy on disk

        private MainWindow _MainWindow;
        private WDAC_Policy _Policy;

        public PolicyType(MainWindow pMainWindow)
        {
            InitializeComponent();
            _MainWindow = pMainWindow;
            _Policy = pMainWindow.Policy;

            _MainWindow.ErrorOnPage = false;
            _MainWindow.RedoFlowRequired = false;

            Logger.Log.AddInfoMsg("==== Policy Type Page Initialized ====");
        }

        /// <summary>
        /// Base policy radio button is selected. Sets the policy type to Base policy. 
        /// </summary>
        private void BasePolicy_Selected(object sender, EventArgs e)
        {
            suppPolicy_PictureBox.Tag = "Unselected";
            basePolicy_PictureBox.Tag = "Selected";

            // New base policy selected
            if (_Policy._PolicyType != WDAC_Policy.PolicyType.BasePolicy)
                _MainWindow.RedoFlowRequired = true;

            _Policy._PolicyType = WDAC_Policy.PolicyType.BasePolicy;
            _MainWindow.Policy._PolicyType = _Policy._PolicyType;

            // Update UI to reflect change
            basePolicy_PictureBox.Image = Properties.Resources.radio_on;
            suppPolicy_PictureBox.Image = Properties.Resources.radio_off;
            panelSupplName.Visible = false;
            _MainWindow.ErrorOnPage = false;
        }

        /// <summary>
        /// Supplemental policy radio button is selected. Sets the policy type to Supplemental. 
        /// Shows the supplemental policy panel. 
        /// </summary>
        private void SupplementalPolicy_Selected(object sender, EventArgs e)
        {
            suppPolicy_PictureBox.Tag = "Selected";
            basePolicy_PictureBox.Tag = "Unselected";

            // Require >= 1903 for multiple policy formats - show UI notification 
            if (Helper.GetWinVersion() < 1903)
            {
                MessageBox.Show("The multiple policy format will not work on pre-1903 systems",
                                "Multiple Policy Format Attention",
                                MessageBoxButtons.OK, 
                                MessageBoxIcon.Information);
            }
                
            // Supplemental policy selected
            if (_Policy._PolicyType != WDAC_Policy.PolicyType.SupplementalPolicy)
            {
                _MainWindow.RedoFlowRequired = true;   
            }

            _Policy._PolicyType = WDAC_Policy.PolicyType.SupplementalPolicy;
            _MainWindow.Policy._PolicyType = _Policy._PolicyType;

            // Update UI to reflect change
            suppPolicy_PictureBox.Image = Properties.Resources.radio_on;
            basePolicy_PictureBox.Image = Properties.Resources.radio_off;

            // Show supplemental policy panel to allow user to build against a policy
            Reset_panel();
            panelSupplName.Visible = true;

            _MainWindow.ErrorOnPage = true;
            _MainWindow.ErrorMsg = "Select base policy to extend before continuing.";
        }

        /// <summary>
        /// Opens the filedialog prompting the user to select the base policy to extend for the 
        /// supplemental policy. 
        /// </summary>
        private void Button_Browse_Click(object sender, EventArgs e)
        {
            // Verify that a Base Policy ID/GUID is not in progress or finished
            // and confirm the user would rather continue with adding a path instead
            if (_Policy.BasePolicyId != Guid.Empty
                || !textBoxBasePolicyID.Text.Contains(Properties.Resources.ExampleBasePolicyId))
            {
                DialogResult res = MessageBox.Show(
                    "Adding a Base Policy by path will remove the ID entered.\r\nAre you sure you want to continue?",
                    "App Control Wizard",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (res != DialogResult.Yes)
                {
                    return;
                }

                _Policy.BasePolicyId = Guid.Empty;
                TextBoxBasePolicyID_Reformat();

                // Log state
                Logger.Log.AddInfoMsg("New supplemental policy flow. Clearing Policy ID and electing for base path");
            }

            // Hide the validation panel
            basePolicyValidation_Panel.Visible = false;

            // Open file dialog to get file or folder path
            string policyPath = Helper.BrowseForSingleFile(Properties.Resources.OpenXMLFileDialogTitle, Helper.BrowseFileType.Policy);

            if (String.IsNullOrEmpty(policyPath))
            {
                return;
            }

            BaseToSupplementPath = policyPath;
            textBoxBasePolicyPath.Text = policyPath;

            // Show right side of the text
            if (textBoxBasePolicyPath.TextLength > 0)
            {
                textBoxBasePolicyPath.SelectionStart = textBoxBasePolicyPath.TextLength - 1;
                textBoxBasePolicyPath.ScrollToCaret();
            }

            // User has modified the supplemental policy from original, force restart flow
            if (_MainWindow.Policy.BaseToSupplementPath != BaseToSupplementPath)
            {
                _MainWindow.RedoFlowRequired = true;
            }

            _MainWindow.Policy.BaseToSupplementPath = BaseToSupplementPath;
            CheckPolicy_Recur(0);
        }

        /// <summary>
        /// Recurrsively checks the base policy to determine if the base allows for supplemental policies. If not, 
        /// the Wizard will prompt the user to modify the base to add the supplemental option
        /// </summary>
        private void CheckPolicy_Recur(int count)
        {
            // Verification: does the chosen base policy allow supplemental policies
            Verified_Label.Visible = true;
            Verified_PictureBox.Visible = true;

            int IsPolicyExtendableCode = IsPolicyExtendable(BaseToSupplementPath);

            if (IsPolicyExtendableCode == 0 && count < 2)
            {
                Verified_Label.Text = "This base policy allows supplemental policies.";
                Verified_PictureBox.Image = Properties.Resources.verified;
                _MainWindow.ErrorOnPage = false;

                if (count == 1)
                {
                    DialogResult res = MessageBox.Show(String.Format("The Wizard has successfully added the Allow Supplemental Policy rule to {0}.", Path.GetFileName(BaseToSupplementPath)),
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
                }
            }
            else if (IsPolicyExtendableCode == 1 && count < 1)
            {
                Verified_Label.Text = "This base policy does not allow supplemental policies.";
                Verified_PictureBox.Image = Properties.Resources.not_extendable;
                _MainWindow.ErrorOnPage = true;
                _MainWindow.ErrorMsg = "Selected base policy does not allow supplemental policies.";

                // Prompt user if the Wizard should add the supplemental policy rule option to base policy
                DialogResult res = MessageBox.Show(String.Format("The base policy you have selected does not allow supplemental policies.\n\n" +
                    "Would you like the Wizard to add the Allow Supplemental Policy rule to {0}?", Path.GetFileName(BaseToSupplementPath)),
                    "Base Policy Issue", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    // Run command Set-RuleOption -Option 17, check IsPolicyExtendable again
                    Logger.Log.AddInfoMsg("Attempting to convert the base policy to one that is extendable");
                    bool success = AddSupplementalOption(BaseToSupplementPath);

                    // If adding supplemental option was unsuccessful for any reason
                    if (!success)
                    {
                        DialogResult _res = MessageBox.Show(String.Format("The Wizard was unable to add the 'Allow Supplemental Policy Option' to {0}.", Path.GetFileName(BaseToSupplementPath)),
                            "Something went wrong", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _MainWindow.ErrorOnPage = true;
                        _MainWindow.ErrorMsg = "The Wizard was unable to add the 'Allow Supplemental Policy Option'.";

                        return;
                    }

                    CheckPolicy_Recur(++count);
                }
            }
            else
            {
                Verified_Label.Text = "This policy does not support supplemental policies. Please select a multi-policy format base policy to supplement.";
                Verified_PictureBox.Image = Properties.Resources.not_extendable;
                _MainWindow.ErrorOnPage = true;
                _MainWindow.ErrorMsg = "Selected base policy is not a base policy.";
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
                Logger.Log.AddErrorMsg("Reading the xml CI policy encountered the following error ", exp);
                // Prompt user for additional confirmation
                DialogResult res = MessageBox.Show("The Wizard is unable to read your base CI policy xml file. The policy XML appears to be corrupted.",
                    "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return 99;
            }

            // Catch null/bad policies
            if (policyToSupplement.siPolicy == null)
            {
                // Prompt user for additional confirmation
                DialogResult res = MessageBox.Show("The Wizard is unable to read your base CI policy xml file. The policy XML appears to be corrupted.",
                    "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return 99;
            }

            Logger.Log.AddInfoMsg(String.Format("IsPolicyExtendable -- Policy Type: {0}", policyToSupplement.siPolicy.PolicyType.ToString()));

            if (policyToSupplement.siPolicy.PolicyType.ToString().Contains("Supplemental"))
            {
                // Policy is not base -- not going to fix this case
                Logger.Log.AddInfoMsg("IsPolicyExtendable -- returns error code 2 (is a supplemental policy)");
                return 2;
            }

            // Policy is not multi-policy format -- not going to fix this case
            if (String.IsNullOrEmpty(policyToSupplement.siPolicy.PolicyID)
                || policyToSupplement.siPolicy.PolicyID.Contains(Properties.Resources.LegacyPolicyID_GUID))
            {
                Logger.Log.AddInfoMsg("IsPolicyExtendable -- returns error code 3 (legacy GUID)");
                return 3;
            }

            if (policyToSupplement.HasRuleType(OptionType.EnabledAllowSupplementalPolicies))
            {
                allowsSupplemental = true;
            }

            // if both allows supplemental policies, and this policy is not already a supplemental policy (ie. a base)
            if (allowsSupplemental)
            {
                Logger.Log.AddInfoMsg("IsPolicyExtendable -- returns error code 0 (allows supplemental)");
                return 0;
            }
            else
            {
                // Policy does not have the supplemental rule option -- can fix this case
                Logger.Log.AddInfoMsg("IsPolicyExtendable -- returns error code 1 (multi-base setting supplemental)");
                return 1;
            }
        }

        /// <summary>
        /// Resets the supplemental panel to its original state. 
        /// </summary>
        private void Reset_panel()
        {
            BaseToSupplementPath = null;
            textBoxBasePolicyPath.Text = "";
            Verified_Label.Visible = false;
            Verified_PictureBox.Visible = false;

            // Set default paths once, unless explicitly turned off in settings
            if (Properties.Settings.Default.useDefaultStrings)
            {
                _Policy.SchemaPath = GetDefaultPath("Supplemental_Policy", 0);
                _Policy.PolicyName = String.Format("{0}_{1}", "My Supplemental Policy", Helper.GetFormattedDate());

                // These will trigger the textChange events
                textBoxSuppPath.Text = _Policy.SchemaPath;
                // Show right side of the text
                if (textBoxSuppPath.TextLength > 0)
                {
                    textBoxSuppPath.SelectionStart = textBoxSuppPath.TextLength - 1;
                    textBoxSuppPath.ScrollToCaret();
                }

                textBox_PolicyName.Text = _Policy.PolicyName;
                _MainWindow.Policy.SchemaPath = _Policy.SchemaPath;

                // Once the supp schema path is set, show panel to select base to supplement
                panelSuppl_Base.Visible = true;
            }

            _MainWindow.Policy._PolicyTemplate = _Policy._PolicyTemplate;
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

            _MainWindow.Policy.SchemaPath = policyPath;
            textBoxSuppPath.Text = policyPath;

            // Show right side of the text
            if (textBoxSuppPath.TextLength > 0)
            {
                textBoxSuppPath.SelectionStart = textBoxSuppPath.TextLength - 1;
                textBoxSuppPath.ScrollToCaret();
            }

            // Show panel if path is set
            panelSuppl_Base.Visible = true;
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
                existingRules[existingRules.Length - 1] = allowSupplementalsRule;
                tempSiPolicy.Rules = existingRules;

                // Check if path is user-writeable. If it is not, copy to documents and update path to BasePolicyToSupplement
                if (!Helper.IsUserWriteable(Path.GetDirectoryName(basePath)))
                {
                    basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.GetFileName(basePath));
                    BaseToSupplementPath = basePath;
                }

                // Serialize the policy back to xml policy format
                Helper.SerializePolicytoXML(tempSiPolicy, basePath);

                return true;
            }
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg("SetPolicyRuleOptions() caught the following exception ", exp);
                return false;
            }
        }

        // <summary>
        /// Policy name has been modified by the user. Update the policy name
        /// </summary>
        private void TextBox_PolicyName_TextChanged(object sender, EventArgs e)
        {
            // Policy Friend Name
            _MainWindow.Policy.PolicyName = textBox_PolicyName.Text;
        }

        // <summary>
        /// User is creating a multi-policy formated policy. Update the UI to reflect the desired state
        /// </summary>
        private void MultipleFormat_ButtonClick(object sender, EventArgs e)
        {
            // Show the multi-policy UI panel
            panel_MultiPolicy.Visible = true;

            // UI - hide the AppId Tagging policy panel
            appIdPolicy_Panel.Visible = false;

            // Set the setting to show this radio button selected next page load
            Properties.Settings.Default.showMultiplePolicyDefault = true;

            // Just call into the events to reset the UI
            if (basePolicy_PictureBox.Tag.ToString().Contains("Unselected"))
            {
                SupplementalPolicy_Selected(sender, e);
            }
            else
            {
                BasePolicy_Selected(sender, e);
            }

            // Set policy format in Policy object
            _MainWindow.Policy._Format = WDAC_Policy.Format.MultiPolicy;
            Logger.Log.AddInfoMsg("Setting WDAC Policy Format to " + _MainWindow.Policy._Format.ToString());

            // Set checkbox UI states for Single Policy and AppIdTagging
            multiPolicyCheckbox.Image = Properties.Resources.radio_on;
            singlePolicyCheckbox.Image = Properties.Resources.radio_off;
            appIdPolicyCheckbox.Image = Properties.Resources.radio_off;
        }

        // <summary>
        /// User is creating a single-policy formated policy. Update the UI to reflect the desired state and hide the multipolicy panel
        /// </summary>
        private void SingleFormat_ButtonClick(object sender, EventArgs e)
        {
            // UI changes - Hide the panel
            panel_MultiPolicy.Visible = false;
            _MainWindow.ErrorOnPage = false;

            // UI - hide the AppId Tagging policy panel
            appIdPolicy_Panel.Visible = false;

            // Set the setting to show this radio button selected next page load
            Properties.Settings.Default.showMultiplePolicyDefault = false;

            // Set policy format in Policy object
            _MainWindow.Policy._Format = WDAC_Policy.Format.Legacy;
            Logger.Log.AddInfoMsg("Setting WDAC Policy Format to " + _MainWindow.Policy._Format.ToString());

            // Set policy type 
            _MainWindow.Policy._PolicyType = WDAC_Policy.PolicyType.BasePolicy;

            // Set checkbox UI states for Multi Policy and AppIdTagging
            multiPolicyCheckbox.Image = Properties.Resources.radio_off;
            singlePolicyCheckbox.Image = Properties.Resources.radio_on;
            appIdPolicyCheckbox.Image = Properties.Resources.radio_off;
        }

        /// <summary>
        /// Creating an AppId Tagging Policy. Update the UI to reflect the desired state and hid the multi policy panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppIdPolicy_Click(object sender, EventArgs e)
        {
            // UI changes - Hide the multiple policy panel
            panel_MultiPolicy.Visible = false;
            _MainWindow.ErrorOnPage = false;

            // UI - show the AppId Tagging policy panel
            appIdPolicy_Panel.Visible = true;
            appIdPolicy_Panel.Location = panel_MultiPolicy.Location;

            // Set the setting to show this radio button selected next page load
            Properties.Settings.Default.showMultiplePolicyDefault = true;

            // Set policy format in Policy object
            _MainWindow.Policy._Format = WDAC_Policy.Format.None;
            Logger.Log.AddInfoMsg("Setting WDAC Policy Format to AppIdTagging");

            // Set policy type to AppId Tagging
            _MainWindow.Policy._PolicyType = WDAC_Policy.PolicyType.AppIdTaggingPolicy;

            // Set checkbox UI states for Multi Policy and AppIdTagging
            multiPolicyCheckbox.Image = Properties.Resources.radio_off;
            singlePolicyCheckbox.Image = Properties.Resources.radio_off;
            appIdPolicyCheckbox.Image = Properties.Resources.radio_on;

            // Set Default Policy Name, File location, if applicable
            if(Properties.Settings.Default.useDefaultStrings)
            {
                SetAppIdPolicyDefaultValues(); 
            }
        }

        // <summary>
        /// Method fires on page load and sets the default state of the UI determined by last time the page was loaded
        /// </summary>
        private void PolicyType_Load(object sender, EventArgs e)
        {
            // On page load, check whether multiple or single policy format was chosen last time the page was loaded
            if (Properties.Settings.Default.showMultiplePolicyDefault)
            {
                multiPolicyCheckbox.Image = Properties.Resources.radio_on;
                singlePolicyCheckbox.Image = Properties.Resources.radio_off;
                appIdPolicyCheckbox.Image = Properties.Resources.radio_off;

                // Set multiple policy format
                _MainWindow.Policy._Format = WDAC_Policy.Format.MultiPolicy;
            }
            else
            {
                multiPolicyCheckbox.Image = Properties.Resources.radio_off;
                singlePolicyCheckbox.Image = Properties.Resources.radio_on;
                appIdPolicyCheckbox.Image = Properties.Resources.radio_off;

                // Set legacy/singly policy format
                _MainWindow.Policy._Format = WDAC_Policy.Format.Legacy; 
            }

            Logger.Log.AddInfoMsg("PolicyType Page Load. Setting Policy Format to " + _MainWindow.Policy._Format.ToString());

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
                Process.Start(new ProcessStartInfo(webpage) { UseShellExecute = true });
            }
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg("Launching webpage for multipolicy link encountered the following error: ", exp);
            }
        }

        /// <summary>
        /// Opens the AppId Tagging Guide online docs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_LearnMoreAppId_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(Properties.Resources.AppIdTaggingDocsLink) { UseShellExecute = true });
            }
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg("Launching webpage for AppIdTagging policies encountered the following error: ", exp);
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
            if (Guid.TryParse(textBoxBasePolicyID.Text, out Guid result))
            {
                _Policy.BasePolicyId = result;
                _MainWindow.ErrorOnPage = false;

                // Log state
                Logger.Log.AddInfoMsg(String.Format("New supplemental policy flow. Valid policy ID entered: {0}", result.ToString()));
            }
            else
            {
                _MainWindow.ErrorOnPage = true;
                _MainWindow.ErrorMsg = Properties.Resources.InvalidBasePolicyId;
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
            if (!String.IsNullOrEmpty(_Policy.BaseToSupplementPath))
            {
                DialogResult res = MessageBox.Show(
                    String.Format("Adding a Base Policy ID will remove the following XML path: {0}. " +
                    "\r\nAre you sure you want to continue?", _Policy.BaseToSupplementPath),
                    "App Control Wizard",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (res != DialogResult.Yes)
                {
                    return;
                }

                // Otherwise, clear the base policy path text and objects
                BaseToSupplementPath = String.Empty;
                _Policy.BaseToSupplementPath = String.Empty;
                textBoxBasePolicyPath.Clear();

                // Hide the validation panel
                basePolicyValidation_Panel.Visible = false;

                // Log state
                Logger.Log.AddInfoMsg("New supplemental policy flow. Clearing XML path and electing for base policy ID");
            }

            // Clear the example text, if applicable
            // Set the text color to black for the user
            if (textBoxBasePolicyID.Text.Contains(Properties.Resources.ExampleBasePolicyId))
            {
                textBoxBasePolicyID.Clear();
                textBoxBasePolicyID.ForeColor = Color.Black;
            }
        }

        /// <summary>
        /// Resets the text to the original eg. {} text and color of the text
        /// </summary>
        private void TextBoxBasePolicyID_Reformat()
        {
            // Reset the text and font to original
            textBoxBasePolicyID.Text = Properties.Resources.ExampleBasePolicyId;
            textBoxBasePolicyID.ForeColor = SystemColors.WindowFrame;
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

            if (String.IsNullOrEmpty(textBoxBasePolicyID.Text))
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
            Setbutton_BrowseUI(button_Browse);

            // Set UI for the 'button_Browse_Supp' Button
            Setbutton_BrowseUI(button_Browse_Supp);

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
                foreach (Control control in Controls)
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
                foreach (Control control in Controls)
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
        public void Setbutton_BrowseUI(Button button)
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                button.FlatAppearance.BorderColor = Color.DodgerBlue;
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                button.FlatStyle = FlatStyle.Flat;
                button.ForeColor = Color.DodgerBlue;
                button.BackColor = Color.Transparent;
            }

            // Light Mode
            else
            {
                button.FlatAppearance.BorderColor = Color.Black;
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                button.FlatStyle = FlatStyle.Flat;
                button.ForeColor = Color.Black;
                button.BackColor = Color.WhiteSmoke;
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

        /// <summary>
        /// Updates the Policy Name of the AppId Tagging Policy when the textbox text changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppIdPolicyNameTextbox_TextChanged(object sender, EventArgs e)
        {
            _Policy.PolicyName = appIdPolicyName_Textbox.Text;
            _MainWindow.Policy.PolicyName = _Policy.PolicyName;
        }

        /// <summary>
        /// Updates the Policy path (schema path_ of the AppId Tagging Policy when the textbox text changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppIdPolicyLocation_Click(object sender, EventArgs e)
        {
            // Save dialog box pressed
            string policyPath = Helper.SaveSingleFile(Properties.Resources.SaveXMLFileDialogTitle, Helper.BrowseFileType.Policy);

            // If cancel button is selected by user, or path does not exist prevent unhandled error
            if (String.IsNullOrEmpty(policyPath))
            {
                return;
            }

            _MainWindow.Policy.SchemaPath = policyPath;
            appIdPolicyLocation_Textbox.Text = policyPath;

            // Show right side of the text
            if (appIdPolicyLocation_Textbox.TextLength > 55)
            {
                appIdPolicyLocation_Textbox.SelectionStart = appIdPolicyLocation_Textbox.TextLength - 1;
                appIdPolicyLocation_Textbox.ScrollToCaret();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetAppIdPolicyDefaultValues()
        {
            // Set default paths once, unless explicitly turned off in settings
            _Policy.SchemaPath = GetDefaultPath("AppIDTagging_Policy", 0);
            _Policy.PolicyName = String.Format("{0}_{1}", "My AppID Tagging Policy", Helper.GetFormattedDate());

            // These will trigger the textChange events
            appIdPolicyName_Textbox.Text = _Policy.PolicyName;
            appIdPolicyLocation_Textbox.Text = _Policy.SchemaPath;

            // Show right side of the text
            if (appIdPolicyLocation_Textbox.TextLength > 0)
            {
                appIdPolicyLocation_Textbox.SelectionStart = appIdPolicyLocation_Textbox.TextLength - 1;
                appIdPolicyLocation_Textbox.ScrollToCaret();
            }

            _MainWindow.Policy.SchemaPath = _Policy.SchemaPath;
            _MainWindow.Policy.PolicyName = _Policy.PolicyName; 
        }
        
    }
}
