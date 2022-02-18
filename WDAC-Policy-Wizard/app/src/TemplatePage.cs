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
using System.IO;
using System.Timers; 

namespace WDAC_Wizard
{
    public partial class TemplatePage : UserControl
    {
        // Properties to maintain the policy mode selected
        private MainWindow _MainWindow;
        private WDAC_Policy _Policy;
        private Logger Log; 
        public TemplatePage(MainWindow pMainWindow)
        {
            InitializeComponent();
            this._MainWindow = pMainWindow;
            this._MainWindow.ErrorOnPage = true;
            this._MainWindow.RedoFlowRequired = false; 
            this._MainWindow.ErrorMsg = "Please select a policy template before continuing."; 
            this._Policy = new WDAC_Policy();
            this.Log = this._MainWindow.Log;
            this.Log.AddInfoMsg("==== Template Page Initialized ===="); 
        }

        /// <summary>
        /// Allow Microsoft template policy selected. Most restrictive and highest level of security.  
        /// </summary>
        /// 
        private void AllowMsft_Button_Click(object sender, EventArgs e)
        {
            // Allow Microsoft Template chosen --> DefaultWindows_Enforced
            // User coming back to this page and changing template -- redo flow
            if (this._MainWindow.Policy._PolicyTemplate != WDAC_Policy.NewPolicyTemplate.None
                 && this._MainWindow.Policy.TemplatePath != null)
            {
                this._MainWindow.RedoFlowRequired = true;
            }
                
            this._Policy._PolicyTemplate = WDAC_Policy.NewPolicyTemplate.AllowMicrosoft;
            // Update UI
            uncheck_all();
            SetDefaultTextValues("AllowMicrosoft"); 
            allowMsft_Button.Tag = "toggle";
            allowMsft_Button.BackgroundImage = Properties.Resources.radio_on;
            this.Log.AddInfoMsg("New Base Template: Allow Microsoft selected");
        }

        /// <summary>
        /// Windows Works template policy selected. 2nd most restrictive and high level of security.  
        /// </summary>
        /// 
        private void WindowsWorks_Button_Click(object sender, EventArgs e)
        {
            // User coming back to this page and changing template -- redo flow
            if (this._MainWindow.Policy._PolicyTemplate != WDAC_Policy.NewPolicyTemplate.None
                 && this._MainWindow.Policy.TemplatePath != null)
            {
                this._MainWindow.RedoFlowRequired = true;
            }
                
            this._Policy._PolicyTemplate = WDAC_Policy.NewPolicyTemplate.WindowsWorks;
            // Update UI
            uncheck_all();
            SetDefaultTextValues("WindowsWorks");
            windowsWorks_Button.Tag = "toggle";
            windowsWorks_Button.BackgroundImage = Properties.Resources.radio_on;
            this.Log.AddInfoMsg("New Base Template: Windows Works selected");
        }

        /// <summary>
        /// Signed and Reputable template policy selected. Least restrictive and lowest level of security.  
        /// </summary>
        /// 
        private void signedReputable_Button_Click(object sender, EventArgs e)
        {
            //Policy Signed and Reputable (certificate & reputation signing)
            // User coming back to this page and changing template -- redo flow
            if (this._MainWindow.Policy._PolicyTemplate != WDAC_Policy.NewPolicyTemplate.None
                 && this._MainWindow.Policy.TemplatePath != null)
            {
                this._MainWindow.RedoFlowRequired = true;
            }
                
            this._Policy._PolicyTemplate = WDAC_Policy.NewPolicyTemplate.SignedReputable;
            
            // Update UI
            uncheck_all();
            SetDefaultTextValues("SignedReputable");
            signedReputable_Button.Tag = "toggle";
            signedReputable_Button.BackgroundImage = Properties.Resources.radio_on;
            this.Log.AddInfoMsg("New Base Template: Signed and Reputable selected");
        }

        /// <summary>
        /// Uncheck all of the tempalte policy options. Required since the buttons do not automatically 
        /// untoggle each other. Called each time a user toggles a template.  
        /// </summary>
        /// 
        private void uncheck_all()
        {
            // Show the text fields now that user has selected base policy template:
            this.policyInfoPanel.Visible = true;
            this._MainWindow.Display_info_text(0); 

            // Force other switch buttons off
            this.allowMsft_Button.Tag = "untoggle";
            this.windowsWorks_Button.Tag = "untoggle";
            this.signedReputable_Button.Tag = "untoggle";

            this.allowMsft_Button.BackgroundImage = Properties.Resources.radio_off;
            this.windowsWorks_Button.BackgroundImage = Properties.Resources.radio_off;
            this.signedReputable_Button.BackgroundImage = Properties.Resources.radio_off;

            // this._MainWindow.ErrorOnPage = false;
        }

        /// <summary>
        /// Sets the policy schema path string. Triggered when user wants to modify the save path for their policy: browse button press or PolicyPath TextBox text changed.  
        /// </summary>
        /// 
        private void textBoxPolicyPath_TextChanged(object sender, EventArgs e)
        {
            // Save dialog box pressed

            string policyPath = Helper.SaveSingleFile(Properties.Resources.SaveXMLFileDialogTitle, Helper.BrowseFileType.Policy); 

            textBoxPolicyPath.Text = policyPath;
            this._Policy.SchemaPath = policyPath;
            this._MainWindow.Policy.SchemaPath = this._Policy.SchemaPath;
            this.textBoxPolicyPath.SelectionStart = this.textBoxPolicyPath.TextLength - 1; 
            this.textBoxPolicyPath.ScrollToCaret(); 
            
            if(this._Policy.PolicyName != null)
            {
                this._MainWindow.ErrorOnPage = false;
            }
        }

        /// <summary>
        /// Sets the policy friendly name string. Triggered when user wants to modify the policy name: PolicyName TextBox text changed.  
        /// </summary>
        /// 
        private void textBoxPolicyName_TextChanged(object sender, EventArgs e)
        {
            // Policy Friend Name
            this._Policy.PolicyName = textBox_PolicyName.Text;
            this._MainWindow.Policy.PolicyName = this._Policy.PolicyName;

            if(this._Policy.SchemaPath != null)
                this._MainWindow.ErrorOnPage = false;
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
                string dateString = this._MainWindow.FormatDate(false);
                this._Policy.SchemaPath = GetDefaultPath(policyTemplate, 0); 
                this._Policy.PolicyName = String.Format("{0}_{1}", policyTemplate, dateString);

                // These will trigger the textChange events
                this.textBoxPolicyPath.Text = this._Policy.SchemaPath;
                this.textBox_PolicyName.Text = this._Policy.PolicyName;
                this._MainWindow.Policy.SchemaPath = this._Policy.SchemaPath;

                this._MainWindow.ErrorOnPage = false;
            }

            this._MainWindow.Policy._PolicyTemplate = this._Policy._PolicyTemplate;

            // Show right side of the text
            this.textBoxPolicyPath.SelectionStart = this.textBoxPolicyPath.TextLength - 1;
            this.textBoxPolicyPath.ScrollToCaret();
        }

        private string GetDefaultPath(string policyTemplate, int nAttempts)
        {
            string dateString = this._MainWindow.FormatDate(false);
            string proposedPath;

            if(nAttempts ==0)
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
                
        private void ISGLabel_Click(object sender, EventArgs e)
        {
            // ISG label clicked. Launch ISG webpage
            try
            {
                string webpage = "https://docs.microsoft.com/windows/security/threat-protection/" +
                    "windows-defender-application-control/use-windows-defender-application-control-with-" +
                    "intelligent-security-graph";
                System.Diagnostics.Process.Start(webpage);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("Launching webpage for ISG link encountered the following error", exp);
            }
        }


        /// <summary>
        /// Sets the back color to "highglight" for the checkbox picturebox when the user hovers the mouse over a checkbox.  
        /// </summary>
        /// <param name="sender">Sender is the picturebox control </param>
        private void MouseHover_Button(object sender, EventArgs e)
        {
            Color hoverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(230)))), ((int)(((byte)(253)))));
            PictureBox checkBox = ((PictureBox)sender);
            checkBox.BackColor = hoverBackColor;
        }

        /// <summary>
        /// Sets the back color to white for the checkbox picturebox when the user is no longer hovering the mouse over a checkbox.  
        /// </summary>
        /// <param name="sender">Sender is the picturebox control </param>
        private void MouseLeave_Button(object sender, EventArgs e)
        {
            PictureBox checkBox = ((PictureBox)sender);
            checkBox.BackColor = Color.White;
        }

        // Learn more about the template policies
        private void label3_Click(object sender, EventArgs e)
        {
            try
            {
                string webpage = "https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/example-wdac-base-policies";
                System.Diagnostics.Process.Start(webpage);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("Launching webpage for Windows Works template encountered the following error", exp);
            }
        }
    }
}
