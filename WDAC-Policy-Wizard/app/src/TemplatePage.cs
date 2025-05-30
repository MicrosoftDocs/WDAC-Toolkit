// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace WDAC_Wizard
{
    public partial class TemplatePage : UserControl
    {
        // Properties to maintain the policy mode selected
        private MainWindow _MainWindow;
        private WDAC_Policy _Policy;
        public TemplatePage(MainWindow pMainWindow)
        {
            InitializeComponent();
            _MainWindow = pMainWindow;
            _MainWindow.ErrorOnPage = true;
            _MainWindow.RedoFlowRequired = false;
            _MainWindow.ErrorMsg = "Please select a policy template before continuing.";
            _Policy = new WDAC_Policy();
            Logger.Log.AddInfoMsg("==== Template Page Initialized ====");
        }

        /// <summary>
        /// Allow Microsoft template policy selected. Most restrictive and highest level of security.  
        /// </summary>
        /// 
        private void AllowMsft_Button_Click(object sender, EventArgs e)
        {
            // Allow Microsoft Template chosen --> DefaultWindows_Enforced
            // User coming back to this page and changing template -- redo flow
            if (_MainWindow.Policy._PolicyTemplate != WDAC_Policy.NewPolicyTemplate.None
                 && _MainWindow.Policy.TemplatePath != null)
            {
                _MainWindow.RedoFlowRequired = true;
            }

            _Policy._PolicyTemplate = WDAC_Policy.NewPolicyTemplate.AllowMicrosoft;

            // Update UI
            Uncheck_all();
            SetDefaultTextValues("AllowMicrosoft");
            allowMsft_Button.Tag = "toggle";
            allowMsft_Button.Image = Properties.Resources.radio_on;
            Logger.Log.AddInfoMsg("New Base Template: Allow Microsoft selected");
        }

        /// <summary>
        /// Windows Works template policy selected. 2nd most restrictive and high level of security.  
        /// </summary>
        /// 
        private void WindowsWorks_Button_Click(object sender, EventArgs e)
        {
            // User coming back to this page and changing template -- redo flow
            if (_MainWindow.Policy._PolicyTemplate != WDAC_Policy.NewPolicyTemplate.None
                 && _MainWindow.Policy.TemplatePath != null)
            {
                _MainWindow.RedoFlowRequired = true;
            }

            _Policy._PolicyTemplate = WDAC_Policy.NewPolicyTemplate.WindowsWorks;
            // Update UI
            Uncheck_all();
            SetDefaultTextValues("WindowsWorks");
            windowsWorks_Button.Tag = "toggle";
            windowsWorks_Button.Image = Properties.Resources.radio_on;
            Logger.Log.AddInfoMsg("New Base Template: Windows Works selected");
        }

        /// <summary>
        /// Signed and Reputable template policy selected. Least restrictive and lowest level of security.  
        /// </summary>
        /// 
        private void SignedReputable_Button_Click(object sender, EventArgs e)
        {
            //Policy Signed and Reputable (certificate & reputation signing)
            // User coming back to this page and changing template -- redo flow
            if (_MainWindow.Policy._PolicyTemplate != WDAC_Policy.NewPolicyTemplate.None
                 && _MainWindow.Policy.TemplatePath != null)
            {
                _MainWindow.RedoFlowRequired = true;
            }

            _Policy._PolicyTemplate = WDAC_Policy.NewPolicyTemplate.SignedReputable;

            // Update UI
            Uncheck_all();
            SetDefaultTextValues("SignedReputable");
            signedReputable_Button.Tag = "toggle";
            signedReputable_Button.Image = Properties.Resources.radio_on;
            Logger.Log.AddInfoMsg("New Base Template: Signed and Reputable selected");
        }

        /// <summary>
        /// Uncheck all of the tempalte policy options. Required since the buttons do not automatically 
        /// untoggle each other. Called each time a user toggles a template.  
        /// </summary>
        private void Uncheck_all()
        {
            // Show the text fields now that user has selected base policy template:
            policyInfoPanel.Visible = true;
            _MainWindow.DisplayInfoText(0);

            // Force other switch buttons off
            allowMsft_Button.Tag = "untoggle";
            windowsWorks_Button.Tag = "untoggle";
            signedReputable_Button.Tag = "untoggle";

            allowMsft_Button.Image = Properties.Resources.radio_off;
            windowsWorks_Button.Image = Properties.Resources.radio_off;
            signedReputable_Button.Image = Properties.Resources.radio_off;
        }

        /// <summary>
        /// Sets the Policy Path save location for the XML file. Triggered on Browse button click 
        /// or TextBoxPolicyPath is double clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxPolicyPath_SetPath(object sender, EventArgs e)
        {
            // Save dialog box pressed or textbox double clicked
            string policyPath = Helper.SaveSingleFile(Properties.Resources.SaveXMLFileDialogTitle, Helper.BrowseFileType.Policy);

            // If cancel button is selected by user, or path does not exist prevent unhandled error
            if (String.IsNullOrEmpty(policyPath))
            {
                if (String.IsNullOrEmpty(_MainWindow.Policy.SchemaPath))
                {
                    _MainWindow.ErrorMsg = Properties.Resources.NullXmlPath;
                    _MainWindow.ErrorOnPage = true;
                }
                return;
            }

            textBoxPolicyPath.Text = policyPath;
            _Policy.SchemaPath = policyPath;
            _MainWindow.Policy.SchemaPath = _Policy.SchemaPath;
            _MainWindow.ErrorOnPage = false;

            // Scroll to the right-most side of the textbox
            if (textBoxPolicyPath.TextLength > 0)
            {
                textBoxPolicyPath.SelectionStart = textBoxPolicyPath.TextLength - 1;
                textBoxPolicyPath.ScrollToCaret();
            }
        }


        /// <summary>
        /// Sets the Policy Path save location for the XML file. Triggered when the user 
        /// changes the text in the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxPolicyPath_TextChanged(object sender, EventArgs e)
        {
            _Policy.SchemaPath = textBoxPolicyPath.Text;
            _MainWindow.Policy.SchemaPath = _Policy.SchemaPath;

            // Validate the Path
            if (String.IsNullOrWhiteSpace(_MainWindow.Policy.SchemaPath))
            {
                _MainWindow.ErrorMsg = Properties.Resources.NullXmlPath;
                _MainWindow.ErrorOnPage = true;
            }
            else
            {
                _MainWindow.ErrorOnPage = false;
            }
        }

        /// <summary>
        /// Sets the policy friendly name string. Triggered when user wants to modify the policy name: PolicyName TextBox text changed. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxPolicyName_TextChanged(object sender, EventArgs e)
        {
            // Policy Friendly Name
            _Policy.PolicyName = textBox_PolicyName.Text;
            _MainWindow.Policy.PolicyName = _Policy.PolicyName;
        }

        /// <summary>
        /// Sets a default save path location and policy name once a template is selected.  
        /// </summary>
        /// <param name="policyTemplate">String relating to the policy template selected. Appears in the friendly policy name</param>
        private void SetDefaultTextValues(string policyTemplate)
        {
            // Set default paths once, unless explicitly turned off in settings
            if (Properties.Settings.Default.useDefaultStrings)
            {
                _Policy.SchemaPath = GetDefaultPath(policyTemplate, 0);
                _Policy.PolicyName = String.Format("{0}_{1}", policyTemplate, Helper.GetFormattedDate());

                // These will trigger the textChange events
                textBoxPolicyPath.Text = _Policy.SchemaPath;
                textBox_PolicyName.Text = _Policy.PolicyName;
                _MainWindow.Policy.SchemaPath = _Policy.SchemaPath;

                _MainWindow.ErrorOnPage = false;
            }

            _MainWindow.Policy._PolicyTemplate = _Policy._PolicyTemplate;

            // Show right side of the text
            if (textBoxPolicyPath.TextLength > 0)
            {
                textBoxPolicyPath.SelectionStart = textBoxPolicyPath.TextLength - 1;
                textBoxPolicyPath.ScrollToCaret();
            }

        }

        /// <summary>
        /// Find a default path for the policy to be saved. 
        /// </summary>
        /// <param name="policyTemplate"></param>
        /// <param name="nAttempts"></param>
        /// <returns></returns>
        private string GetDefaultPath(string policyTemplate, int nAttempts)
        {
            string proposedPath;

            if (nAttempts == 0)
                proposedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    String.Format("{0}{1}.xml", policyTemplate, Helper.GetFormattedDate()));
            else
                proposedPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    String.Format("{0}{1}_{2}.xml", policyTemplate, Helper.GetFormattedDate(), nAttempts));

            if (File.Exists(proposedPath))
                return GetDefaultPath(policyTemplate, ++nAttempts);
            else
                return proposedPath;
        }

        /// <summary>
        /// User has clicked the ISG label to learn more. Launch webpage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ISGLabel_Click(object sender, EventArgs e)
        {
            // ISG label clicked. Launch ISG webpage
            try
            {
                string webpage = "https://docs.microsoft.com/windows/security/threat-protection/" +
                    "windows-defender-application-control/use-windows-defender-application-control-with-" +
                    "intelligent-security-graph";
                Process.Start(new ProcessStartInfo(webpage) { UseShellExecute = true });
            }
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg("Launching webpage for ISG link encountered the following error", exp);
            }
        }


        /// <summary>
        /// Sets the back color to "highglight" for the checkbox picturebox when the user hovers the mouse over a checkbox.  
        /// </summary>
        /// <param name="sender">Sender is the picturebox control </param>
        private void MouseHover_Button(object sender, EventArgs e)
        {
            PictureBox checkBox = ((PictureBox)sender);

            // If untoggled, show toggled button on hover. Undo on mouse leave event
            if (!checkBox.Tag.Equals("toggle"))
            {
                checkBox.Image = Properties.Resources.radio_on;
            }
        }

        /// <summary>
        /// Sets the back color to white for the checkbox picturebox when the user is no longer hovering the mouse over a checkbox.  
        /// </summary>
        /// <param name="sender">Sender is the picturebox control </param>
        private void MouseLeave_Button(object sender, EventArgs e)
        {
            PictureBox checkBox = ((PictureBox)sender);

            // Undo the image set by MouseHover_Button event
            // Image with untoggle tags need to be set back to untoggled
            if (!checkBox.Tag.Equals("toggle"))
            {
                checkBox.Image = Properties.Resources.radio_off;
            }
        }

        /// <summary>
        /// Learn more about the template policies
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_LearnMore_Click(object sender, EventArgs e)
        {
            try
            {
                string webpage = "https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/example-wdac-base-policies";
                Process.Start(new ProcessStartInfo(webpage) { UseShellExecute = true });
            }
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg("Launching webpage for Windows Works template encountered the following error", exp);
            }
        }

        /// <summary>
        /// Form painting. Occurs on Form.Refresh, Load and Focus. 
        /// Used for UI element changes for Dark and Light Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TemplatePage_Validated(object sender, EventArgs e)
        {
            // Set Controls Color (e.g. Panels)
            SetControlsColor();

            // Set UI for the 'button_Browse' Button
            Setbutton_BrowseUI();

            // Set Labels Color
            List<Label> labels = new List<Label>();
            GetLabelsRecursive(this, labels);
            SetLabelsColor(labels);

            // Set TextBoxes Color
            List<TextBox> textBoxes = new List<TextBox>();
            GetTextBoxesRecursive(this, textBoxes);
            SetTextBoxesColor(textBoxes);

            // Set PolicyType Form back color
            SetFormBackColor();

            // Set Template Policy Icons
            SetTemplateIconImages();
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
                    if (control is Panel panel
                        && (panel.Tag == null || panel.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        panel.ForeColor = Color.White;
                        panel.BackColor = Color.FromArgb(15, 15, 15);
                    }
                }
            }

            // Light Mode
            else
            {
                foreach (Control control in Controls)
                {
                    if (control is Panel panel
                        && (panel.Tag == null || panel.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        panel.ForeColor = Color.Black;
                        panel.BackColor = Color.White;
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
                button_Browse.FlatAppearance.BorderColor = Color.DodgerBlue;
                button_Browse.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                button_Browse.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                button_Browse.FlatStyle = FlatStyle.Flat;
                button_Browse.ForeColor = Color.DodgerBlue;
                button_Browse.BackColor = Color.Transparent;
            }

            // Light Mode
            else
            {
                button_Browse.FlatAppearance.BorderColor = Color.Black;
                button_Browse.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                button_Browse.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                button_Browse.FlatStyle = FlatStyle.Flat;
                button_Browse.ForeColor = Color.Black;
                button_Browse.BackColor = Color.WhiteSmoke;
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
        /// Sets the images for the policy template icons
        /// depending on Dark or Light Mode
        /// </summary>
        private void SetTemplateIconImages()
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                windowsPictureBox.Image = Properties.Resources.white_windows_logo;
                microsoftPictureBox.Image = Properties.Resources.white_windows_logo;
                reputablePictureBox.Image = Properties.Resources.white_shield;
            }

            // Light Mode
            else
            {
                windowsPictureBox.Image = Properties.Resources.windows_logo;
                microsoftPictureBox.Image = Properties.Resources.windows_logo;
                reputablePictureBox.Image = Properties.Resources.shield;
            }
        }
    }
}
