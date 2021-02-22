// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using System.Security; 
using System.Security.Permissions; 

using Microsoft.Win32;
using WDAC_Wizard.src;

namespace WDAC_Wizard
{
    public partial class MainWindow : Form
    {
        // MainWindow Specific Private Properties 
        private bool ConfigInProcess;                    // Has a user selected one of the following views: New, Audit, Edit, Merge
        private int CurrentPage;                         // Which user control is sitting on top of MainWindow 
        public byte view { get; set; }                   // 1 - New policy ,, 2 - Edit policy,, 3 - Merge policy
        public bool ErrorOnPage { get; set; }            // Flag blocking proceeding to next user control. 
        public string ErrorMsg { get; set; }             // Accompanying message error description for ErrorOnPage flag 
        public bool RedoFlowRequired { get; set; }       // Flag which prohibts user from making changes on page 1 then skipping back to page 4, for instance
        public bool CustomRuleinProgress { get; set; }   // Flag set when user has kicked off the custom rule procedure. Ensures user does not go to build page without accidentally creating the rule

        public Logger Log { get; set; }
        public List<string> PageList;
        public WDAC_Policy Policy { get; set; }
        // Runspace param to access all PS Variables and eliminate overhead opening each time
        private Runspace runspace;
        private int RulesNumber;
        public string TempFolderPath { get; set; }
        public string ExeFolderPath { get; set; }

        // Edit Workflow datastructs
        private BuildPage _BuildPage;

        public MainWindow()
        {
            this.TempFolderPath = CreateTempFolder();
            this.Log = new Logger(this.TempFolderPath); 
            
            InitializeComponent();

            // Init MainWindow params
            this.ConfigInProcess = false;
            this.CurrentPage = 0;
            this.RulesNumber = 0;

            this.Policy = new WDAC_Policy();
            this.PageList = new List<string>();
            this.ExeFolderPath = GetExecutablePath(false);

            this.CustomRuleinProgress = false; 

            // Check for configci cmdlet availability
            LicenseCheck(); 

        }

        // ###############
        // HEADER CONTROLS
        // ###############

        /// <summary>
        /// New policy button selected: User can select either base or suppl policy,
        /// a template policy then customize policy rules and file rules.
        /// </summary>
        /// 
        private void button_New_Click(object sender, EventArgs e)
        {
            if (!this.ConfigInProcess)
            {
                this.Log.AddNewSeparationLine("Workflow -- New Policy Selected");
                this.view = 1; 
                this.CurrentPage = 1;
                this.ConfigInProcess = true;
                this.RedoFlowRequired = false; 
                this.Policy._PolicyType = WDAC_Policy.PolicyType.BasePolicy; // Set by default to match the UI default view

                pageController(sender, e); 
                button_Next.Visible = true; 
            }

            else
            {
                // Working on other workflow - do you want to leave?
                if (wantToAbandonWork())
                {
                    display_info_text(0);
                    this.ConfigInProcess = false;
                    button_New_Click(sender, e);
                }
            }
        }

        
        /// <summary>
        /// Edit policy button selected: User can load a pre-exisiting policy on disk and 
        /// view and reconfigure its rules and settings. 
        /// </summary>
        private void button_Edit_Click(object sender, EventArgs e)
        {
            // Edit Policy Button:
           
            if (!this.ConfigInProcess)
            {
                this.Log.AddNewSeparationLine("Workflow -- Edit Policy Selected");
                this.view = 2;
                this.CurrentPage = 1; 
                this.ConfigInProcess = true;
                this.Policy._PolicyType = WDAC_Policy.PolicyType.Edit; 

                pageController(sender, e);
                button_Next.Visible = true;
            }

            else
            {
                // Working on other workflow - do you want to leave?
                // If so, set the ConfigInProcess flag to false
                if (wantToAbandonWork())
                {
                    display_info_text(0);
                    this.ConfigInProcess = false;
                    button_Edit_Click(sender, e);
                }
            }
        }


        /// <summary>
        /// Merge policy button selected: User must select two policies on disk to merge into 
        /// one single policy where the new policy is the intersection of the former two. 
        /// </summary>
        private void button_Merge_Click(object sender, EventArgs e)
        {
            if (!this.ConfigInProcess)
            {
                this.Log.AddNewSeparationLine("Workflow -- Merge Policies Selected");
                this.view = 3;
                this.CurrentPage = 1;
                this.ConfigInProcess = true;
                this.Policy._PolicyType = WDAC_Policy.PolicyType.Merge;

                pageController(sender, e);
                button_Next.Visible = true;
            }

            else
            {
                // Working on other workflow - do you want to leave?
                // If so, set the ConfigInProcess flag to false
                if (wantToAbandonWork())
                {
                    display_info_text(0);
                    this.ConfigInProcess = false;
                    button_Merge_Click(sender, e);
                }
            }
        }

        // #####################
        // CONTROL PANEL CONTROLS
        // #####################

        /// <summary>
        /// Home button on the left hand navigation panel. Resets the state of the application and brings user to the MainWindow form.  
        /// </summary>
        private void home_Button_Click(object sender, EventArgs e)
        {
            this.button_Next.Visible = false;

            // Reset flags
            this.ConfigInProcess = false;
            this.CustomRuleinProgress = false; 
            this.view = 0;
            this.CurrentPage = 0;
            this.Log = new Logger(this.TempFolderPath);

            RemoveControls(); 
            ShowControlPanel(sender, e);
            
        }
        /// <summary>
        /// Settings button on the left hand navigation panel. Loads the Settings UserControl. 
        /// </summary>
        private void settings_Button_Click(object sender, EventArgs e)
        {
            this.Log.AddInfoMsg("Workflow -- Settings Button Selected");
            this.button_Next.Visible = false;
            this.CurrentPage = 99; 

            var _SettingsPage = new SettingsPage(this);
            _SettingsPage.Name = "SettingsPage"; 
            this.Controls.Add(_SettingsPage);
            this.PageList.Add(_SettingsPage.Name); 
            _SettingsPage.BringToFront();
            _SettingsPage.Focus();

            ShowControlPanel(sender, e);
        }

        /// <summary>
        /// Controls the PageController method when the user presses the Next button.
        /// If there is an ErrorOnPage flag, the page is not advanced. 
        /// </summary>
        private void button_Next_Click(object sender, EventArgs e)
        {
            if (!this.ErrorOnPage)
            {
                // Check that a custom rule is not in progress
                if(this.CustomRuleinProgress)
                {
                    DialogResult res = MessageBox.Show("Do you want to create this custom rule before building your WDAC policy?", 
                        "Confirmation", 
                        MessageBoxButtons.YesNo, 
                        MessageBoxIcon.Question);

                    if (res == DialogResult.Yes)
                        return;
                }

                this.CurrentPage++;
                pageController(sender, e);
            }
                
            else
                display_info_text(99); 
        }

        private void PageNButton_Click(object sender, EventArgs e)
        {
            // Get the button name, extract the number of page user wants, set to CurrPage
            string buttonName = ((Button)sender).Name;
            int desiredPage = Convert.ToInt32(buttonName.Substring(4,1)); //eg. Page4_Button --> 4

            if (this.CurrentPage == desiredPage)
                return; 
            this.CurrentPage = desiredPage;

            // If settings page was previous, re-enable Next button
            this.button_Next.Visible = true; 
            pageController(sender, e);
        }


        /// <summary>
        /// Controller mechanism to determine which UserControls to place ontop of the MainWindow WinForm.
        /// Method called by the Next and Back button.
        /// </summary>
        public void pageController(object sender, EventArgs e)
        {
            display_info_text(0);
            
            //RemoveControls(); 

            // Get pertitent workflow
            switch (this.CurrentPage)
            {
                // Home page
                case 0:
                    break;

                // 1st page
                case 1:
                    switch (this.Policy._PolicyType)
                    {
                        case WDAC_Policy.PolicyType.BasePolicy:
                            // New view - load choose base vs supplemental
                            string pageKey = "PolicyTypePage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, launch instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                var _PolicyTypePage = new PolicyType(this);
                                _PolicyTypePage.Name = pageKey;
                                this.PageList.Add(_PolicyTypePage.Name);
                                this.Controls.Add(_PolicyTypePage);
                                _PolicyTypePage.BringToFront();
                                _PolicyTypePage.Focus();
                            }

                            ShowControlPanel(sender, e);
                            break;

                        case WDAC_Policy.PolicyType.SupplementalPolicy:
                            // New view - load choose base vs supplemental
                            pageKey = "PolicyTypePage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, launch instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                var _PolicyTypePage = new PolicyType(this);
                                _PolicyTypePage.Name = pageKey;
                                this.PageList.Add(_PolicyTypePage.Name);
                                this.Controls.Add(_PolicyTypePage);
                                _PolicyTypePage.BringToFront();
                                _PolicyTypePage.Focus();
                            }

                            ShowControlPanel(sender, e);
                            break;


                        case WDAC_Policy.PolicyType.Edit:
                            // view & edit mode
                            // Show policy rules UI
                            pageKey = "EditWorkflowPage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, launch instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                var _EditWorkflow = new EditWorkflow(this);
                                _EditWorkflow.Name = pageKey;
                                this.PageList.Add(_EditWorkflow.Name);
                                this.Controls.Add(_EditWorkflow);
                                _EditWorkflow.BringToFront();
                                _EditWorkflow.Focus();
                            }

                            ShowControlPanel(sender, e);
                            break;

                        case WDAC_Policy.PolicyType.Merge:
                            // Merge Mode
                            pageKey = "MergePage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, launch instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                var _MergePage = new PolicyMerge_Control(this);
                                _MergePage.Name = pageKey;
                                this.PageList.Add(_MergePage.Name);
                                this.Controls.Add(_MergePage);
                                _MergePage.BringToFront();
                                _MergePage.Focus();
                            }

                            ShowControlPanel(sender, e);
                            break; 
                    }
                    break; // end of 1st page

                
                case 2: // 2nd Page
                    switch (this.Policy._PolicyType)
                    {
                        case WDAC_Policy.PolicyType.BasePolicy:
                            // New BASE policy - template page
                            string pageKey = "TemplatePage"; 
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, launch instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                var _NewPolicyPage = new TemplatePage(this);
                                _NewPolicyPage.Name = pageKey;
                                this.PageList.Add(_NewPolicyPage.Name);
                                this.Controls.Add(_NewPolicyPage);
                                _NewPolicyPage.BringToFront();
                                _NewPolicyPage.Focus();
                            }
                            
                            ShowControlPanel(sender, e);
                            break;

                        case WDAC_Policy.PolicyType.SupplementalPolicy:
                            
                            // New SUPPLEMENTAL policy -> policy rules. Do not present a template page
                            pageKey = "SupConfigTemplatePage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, show instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                var _ConfigTemplateControl = new ConfigTemplate_Control(this);
                                _ConfigTemplateControl.Name = pageKey;
                                this.PageList.Add(_ConfigTemplateControl.Name);
                                this.Controls.Add(_ConfigTemplateControl);
                                _ConfigTemplateControl.BringToFront();
                                _ConfigTemplateControl.Focus();
                            }
                            ShowControlPanel(sender, e);
                            break;

                        case WDAC_Policy.PolicyType.Edit:

                            // Edit Mode
                            pageKey = "EditPolicyRulesPage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, launch instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                var _RulesPage = new ConfigTemplate_Control(this);
                                _RulesPage.Name = pageKey;
                                this.PageList.Add(_RulesPage.Name);
                                this.Controls.Add(_RulesPage);
                                _RulesPage.BringToFront();
                                _RulesPage.Focus();
                            }

                            ShowControlPanel(sender, e);
                            break;

                        case WDAC_Policy.PolicyType.Merge:

                            button_Next.Visible = false;

                            pageKey = "BuildPage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, show instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                this._BuildPage = new BuildPage(this);
                                this._BuildPage.Name = pageKey;
                                this.PageList.Add(this._BuildPage.Name);
                                this.Controls.Add(this._BuildPage);
                                this._BuildPage.BringToFront();
                                this._BuildPage.Focus();
                            }
                            ShowControlPanel(sender, e);
                            ProcessPolicy();
                            break; 


                        case WDAC_Policy.PolicyType.None:
                            display_info_text(98);
                            this.CurrentPage--;
                            break;
                    }
                    break;
                        

                // 3rd Page
                case 3:
                    switch (this.Policy._PolicyType)
                    {
                        case WDAC_Policy.PolicyType.BasePolicy:  // New BASE policy - rules page

                            string pageKey = "ConfigTemplatePage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, show instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                var _ConfigTemplateControl = new ConfigTemplate_Control(this);
                                _ConfigTemplateControl.Name = pageKey; 
                                this.PageList.Add(_ConfigTemplateControl.Name);
                                this.Controls.Add(_ConfigTemplateControl);
                                _ConfigTemplateControl.BringToFront();
                                _ConfigTemplateControl.Focus();
                            }

                            ShowControlPanel(sender, e);
                            break;

                        case WDAC_Policy.PolicyType.SupplementalPolicy: // New SUPPLEMENTAL policy

                            pageKey = "SupSigningRulesPage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, show instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                var _SigningSupplementalRules = new SigningRules_Control(this);
                                _SigningSupplementalRules.Name = pageKey;
                                this.PageList.Add(_SigningSupplementalRules.Name);
                                this.Controls.Add(_SigningSupplementalRules);
                                _SigningSupplementalRules.BringToFront();
                                _SigningSupplementalRules.Focus();
                            }

                            ShowControlPanel(sender, e);
                            break;

                        case WDAC_Policy.PolicyType.Edit:

                            // Edit & view mode
                            pageKey = "SigningRulesPage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, show instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                var _SigningRules_Control = new SigningRules_Control(this);
                                _SigningRules_Control.Name = pageKey;
                                this.PageList.Add(_SigningRules_Control.Name);
                                this.Controls.Add(_SigningRules_Control);
                                _SigningRules_Control.BringToFront();
                                _SigningRules_Control.Focus();
                            }

                            ShowControlPanel(sender, e);
                            break;
                    }
                    break;

                // 4th Page
                case 4:
                    switch (this.Policy._PolicyType)
                    {
                        case WDAC_Policy.PolicyType.BasePolicy:
                            // New BASE policy - Add file rules page
                            string pageKey = "SigningRulesPage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, show instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                var _SigningRules = new SigningRules_Control(this);
                                _SigningRules.Name = pageKey; 
                                this.PageList.Add(_SigningRules.Name);
                                this.Controls.Add(_SigningRules);
                                _SigningRules.BringToFront();
                                _SigningRules.Focus();
                            }

                            ShowControlPanel(sender, e);
                            break;

                        case WDAC_Policy.PolicyType.SupplementalPolicy:
                            // New SUPPLEMENTAL policy -- build out policy

                            pageKey = "BuildPage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, show instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                this._BuildPage = new BuildPage(this);
                                this._BuildPage.Name = pageKey;
                                this.PageList.Add(this._BuildPage.Name);
                                this.Controls.Add(this._BuildPage);
                                this._BuildPage.BringToFront();
                                this._BuildPage.Focus();
                                button_Next.Visible = false;
                            }
                            ShowControlPanel(sender, e);
                            ProcessPolicy();

                            break;

                        case WDAC_Policy.PolicyType.Edit:
                            // Edit & view mode

                            button_Next.Visible = false;

                            pageKey = "BuildPage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, show instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                this._BuildPage = new BuildPage(this);
                                this._BuildPage.Name = pageKey;
                                this.PageList.Add(this._BuildPage.Name);
                                this.Controls.Add(this._BuildPage);
                                this._BuildPage.BringToFront();
                                this._BuildPage.Focus();
                            }
                            ShowControlPanel(sender, e);
                            ProcessPolicy();

                            break;
                    }
                    break;

                // 5th Page
                case 5:
                    switch (this.Policy._PolicyType)
                    {
                        case WDAC_Policy.PolicyType.BasePolicy:
                            // Build out the policy - begin PS execution
                            
                            button_Next.Visible = false;

                            string pageKey = "BuildPage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, show instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                this._BuildPage = new BuildPage(this);
                                this._BuildPage.Name = pageKey;
                                this.PageList.Add(this._BuildPage.Name);
                                this.Controls.Add(this._BuildPage);
                                this._BuildPage.BringToFront();
                                this._BuildPage.Focus();
                            }

                            ShowControlPanel(sender, e);
                            ProcessPolicy();
                            break;

                    }
                    break;
            }
        }

        /// <summary>
        /// Begins executing the worer thread and creating the policy created by users. 
        /// Creates the temp directory path to write all of the temp data.  
        /// </summary>
        private void ProcessPolicy()
        {
            // Create folder for temp intermediate policies
            try
            {
                System.IO.Directory.CreateDirectory(this.TempFolderPath); // Create new temp folder
            }
            catch (Exception e)
            {
                this.Log.AddErrorMsg("Process Policy() caught the following exception ", e);
            }
            System.IO.Directory.CreateDirectory(this.TempFolderPath);
           
            this.runspace = RunspaceFactory.CreateRunspace();
            this.runspace.Open();

            this.Log.AddNewSeparationLine("Workflow -- Building Policy Underway"); 

            // Write all policy, file and signer rules to xml files:
            if (!backgroundWorker1.IsBusy)
                backgroundWorker1.RunWorkerAsync();
        }

        /// <summary>
        /// Event handler where the time-consuming work of creating the policies is accomplished. 
        /// No UI changes should be performed in this method. 
        /// </summary>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string MERGEPATH = System.IO.Path.Combine(this.TempFolderPath, "FinalPolicy.xml");
            
            if(this.Policy._PolicyType != WDAC_Policy.PolicyType.Merge)
            {
                // Handle Policy Rule-Options
                CreatePolicyRuleOptions(worker);

                // Handle custom rules:
                //  1. Create all of the $Rule objects running New-CIPolicyRule
                //  2. Create a unique CI policy per custom rule by running New-CIPolicy
                List<string> customRulesPathList = ProcessCustomRules(worker);
                // Merge policies - all custom ones and the template and/or the base (if this is a supplemental)
                // For some reason, Merge-CIPolicy -Rules <Rule[]> is not trivial - use -PolicyPaths instead
                MergeCustomRulesPolicy(customRulesPathList, MERGEPATH, worker);
            }
                      

            // Merge all of the unique CI policies with template and/or base policy:
            MergeTemplatesPolicy(MERGEPATH, worker);

            // Set additional parameters, for instance, policy name, GUIDs, version, etc
            SetAdditionalParameters(worker);

            // Convert the policy from XML to Bin -- v2 
            if (Properties.Settings.Default.convertPolicyToBinary)
            {
                ConvertPolicyToBinary();
            }

            worker.ReportProgress(100);
        }

        /// <summary>
        /// Event handler where the progress of the worker is updated when worker.ReportProgress is 
        /// called. Handle UI updates to BuildPage class through public method 'UpdateProgressBar'.
        /// </summary>
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string process = "";
            int progressPercent = e.ProgressPercentage;
            if (progressPercent <= 10)
                process = "Building policy rules ...";
            else if (progressPercent <= 70)
                process = "Configuring custom policy signing rules ...";
            else if (progressPercent <= 80)
                process = "Building policy signing rules ...";
            else if (progressPercent <= 85)
                process = "Merging custom rules policies ...";
            else if (progressPercent <= 95)
                process = "Merging custom rules with template policies ...";
            else
                process = "Setting additional parameters ..."; 

            this._BuildPage.UpdateProgressBar(e.ProgressPercentage, process);
        }

        /// <summary>
        /// This event handler deals with the results of the background operation. Handles both
        /// successful and unsuccessful results. Successful results in a call to the BuildPage 
        /// ShowFinishMsg method.
        /// </summary>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.Log.AddErrorMsg("ProcessPolicy() caught the following exception ", e.Error);
                this._BuildPage.ShowError();
            }
            else
            {
                this._BuildPage.ShowFinishMsg(this.Policy.SchemaPath);
                this._BuildPage.UpdateProgressBar(100, " ");
            }

            this.Log.AddNewSeparationLine("Workflow -- DONE"); 

            // Upload log file if customer consents
            if (Properties.Settings.Default.allowTelemetry)
            {
                this.Log.UploadLog();
            }
        }

        /// <summary>
        /// Create the policy rule-option pairings the user has set and creates a seperate CI policy just for the pairings. 
        /// Also removes all of the rule-options for the template policy such that there is no merge conflict. 
        /// </summary>
        public void CreatePolicyRuleOptions(BackgroundWorker worker)
        {
            // Set-RuleOption: https://docs.microsoft.com/en-us/powershell/module/configci/set-ruleoption?view=win10-ps
            // Inputs: -FilePath <string> required existing, -Option <Int32>: indices of rules options

            this.Log.AddInfoMsg("--- Create Policy Rule Options --- ");

            // Rules - read in the ConfigRules dictionary
            Dictionary<string, Dictionary<string, string>>.KeyCollection keys = this.Policy.ConfigRules.Keys;
            Runspace policyRuleOptsRunspace = RunspaceFactory.CreateRunspace();
            policyRuleOptsRunspace.Open();
            Pipeline pipeline = policyRuleOptsRunspace.CreatePipeline();

            // Make a copy of the Template policy and REMOVE ALL of its rule options
            // That way we ensure we take the set rule-options from ConfigRules
            try
            {
                string templateCopyPath = Path.Combine(this.TempFolderPath, "TemplateCopy.xml");
                File.Copy(this.Policy.TemplatePath, templateCopyPath, true);
                this.Policy.TemplatePath = templateCopyPath;

            }
            catch (Exception e)
            {
                this.Log.AddErrorMsg("Create Policy Rule Options -- copying Template Policy encountered the following error ", e); 
            }
            
            int N_Rules = 19; 
            for(int i = 0; i <= N_Rules; i++)
            {
                pipeline.Commands.AddScript(String.Format("Set-RuleOption -FilePath \"{0}\" -Option {1} -Delete ", this.Policy.TemplatePath, i));
            }
                            

            foreach (string key in keys)
            {
                // Skip the unsupported rules -- they will throw an error when converting to .bin policy files
                if (this.Policy.ConfigRules[key]["Supported"] == "False")
                    continue; 

                string ruleVal = this.Policy.ConfigRules[key]["CurrentValue"];
                string allowedText = this.Policy.ConfigRules[key]["AllowedValue"];
                string ruleOptNum = this.Policy.ConfigRules[key]["RuleNumber"];

                if (ruleVal == allowedText && !String.IsNullOrEmpty(ruleOptNum)) //Value == xml allowable output - write
                {
                    pipeline.Commands.AddScript(String.Format("Set-RuleOption -FilePath \"{0}\" -Option {1} ",
                        this.Policy.TemplatePath, ruleOptNum));
                    this.Log.AddInfoMsg(String.Format("Adding rule-option pair: {0}:{1}", key, ruleVal));
                }
                //else skip - invalid value eg)  Disable:UMCI  so omitting it from setting accomplishes this
            }

            // Assert supplemental policies and legacy policies cannot have the Supplemental (rule #17) option
            if (this.Policy._Format == WDAC_Policy.Format.Legacy)
            {
                pipeline.Commands.AddScript(String.Format("Set-RuleOption -FilePath \"{0}\" -Option 17 -Delete", this.Policy.TemplatePath));
            }

            // Assert unsigned CI policy (rule #6) - fixes issues with converting to binary where the policy is unsigned
            if (Properties.Settings.Default.convertPolicyToBinary)
            {
                pipeline.Commands.AddScript(String.Format("Set-RuleOption -FilePath \"{0}\" -Option 6", this.Policy.TemplatePath));
            }

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception e)
            {
                this.Log.AddErrorMsg("CreatePolicyRuleOptions() caught the following exception ", e);
            }

            policyRuleOptsRunspace.Close();
            worker.ReportProgress(10);
        }


        /// <summary>
        /// Creates the driver file objects in PowerShell. This should only be run when attempting to bulk add all driver objects, 
        /// for instance running Get-SystemDrivers 
        /// </summary>
        public void CreateDriverFiles(BackgroundWorker worker)
        {
            this.Log.AddInfoMsg("--- Create Driver Files ---");

            PolicyCustomRules NewFileRule = new PolicyCustomRules();
            List<PolicyCustomRules> CustomRulesCopy = this.Policy.CustomRules;
            int nRules = CustomRulesCopy.Count;

            // Get number of operations (purely for update worker):
            int nOps = 0;
            int completedOps = 0;

            foreach (var CustomRule in CustomRulesCopy)
            {
                if (CustomRule.GetRuleLevel() == PolicyCustomRules.RuleLevel.Folder)
                    nOps += CustomRule.FolderContents.Count;
                else
                    nOps++;
            }

            this.Log.AddInfoMsg(String.Format("Number of driver file ops: {0}", nOps.ToString()));

            // Iterate through each CustomRule and call Get-SystemDriver cmdlet (pass through in GetSystemInfo()
            for (int i = 0; i < nRules; i++)
            {
                var CustomRule = this.Policy.CustomRules[i];
                if (CustomRule.GetRuleLevel() == PolicyCustomRules.RuleLevel.Folder)
                {
                    foreach (string filePath in CustomRule.FolderContents)
                    {
                        // Create a new rule foreach file in folder
                        Dictionary<string, string> SystemFileInfo = GetSystemInfo(filePath);
                        NewFileRule = new PolicyCustomRules(SystemFileInfo["PSVar"], SystemFileInfo["RuleIndex"], 
                            filePath, CustomRule.GetRulePermission());
                        this.Policy.CustomRules.Add(NewFileRule);

                        // Update status:
                        completedOps++;
                        double prog = Math.Ceiling((double)completedOps / (double)nOps * (double)60);
                        worker.ReportProgress((int)prog + 10); // Assumes this operation represents 60% of time requirements
                    }
                }

                else
                {
                    string refFilePath = CustomRule.ReferenceFile;
                    Dictionary<string, string> SystemInfo = GetSystemInfo(refFilePath);

                    // Only params we need are the PS variable and its Rule Index from the Dict
                    CustomRule.PSVariable = SystemInfo["PSVar"];
                    CustomRule.RuleIndex = SystemInfo["RuleIndex"];

                    // Update status:
                    completedOps++;
                    double prog = Math.Ceiling((double)completedOps / (double)nOps * (double)60);
                    worker.ReportProgress((int)prog + 10); // Assumes this operation represents 60% of time requirements

                }
            }
        }


        /// <summary>
        /// Processes all of the custom rules defined by user. 
        /// </summary>
        public List<string> ProcessCustomRules(BackgroundWorker worker)
        {
            List<string> customRulesPathList = new List<string>();
            int nCustomRules = this.Policy.CustomRules.Count;
            int progressVal = 0;

            // Iterate through all of the custom rules and update the progress bar    
            for (int i = 0; i < nCustomRules; i++)
            {
                var customRule = this.Policy.CustomRules[i];
                customRule.PSVariable = i.ToString(); 
                string tmpPolicyPath = getUniquePolicyPath(this.TempFolderPath);
                customRulesPathList.Add(tmpPolicyPath); 

                string ruleScript = createCustomRuleScript(customRule);
                string policyScript = createPolicyScript(customRule, tmpPolicyPath); 

                // Add script to pipeline and run PS command
                Pipeline pipeline = this.runspace.CreatePipeline();
                pipeline.Commands.AddScript(ruleScript);
                pipeline.Commands.AddScript(policyScript);
                this.Log.AddInfoMsg(String.Format("Running the following commands: {0}", ruleScript));
                this.Log.AddInfoMsg(String.Format("Running the following commands: {0}", policyScript));

                // Update progress bar per completion of custom rule created
                progressVal = 10 + i * 70 / nCustomRules; 
                worker.ReportProgress(progressVal); //Assumes the operations involved with this step take about 70% -- probably should be a little higher

                try
                {
                    Collection<PSObject> results = pipeline.Invoke();
                }
                catch (Exception e)
                {
                    this.Log.AddErrorMsg("CreatePolicyFileRuleOptions() caught the following exception ", e);
                }
            }

            //TODO: results check ensuring 
            runspace.Dispose();
            return customRulesPathList;
        }

        public string createCustomRuleScript(PolicyCustomRules customRule)
        {
            string customRuleScript = string.Empty;

            // Create new CI Rule: https://docs.microsoft.com/en-us/powershell/module/configci/new-cipolicyrule
            switch (customRule.GetRuleType())
            {
                case PolicyCustomRules.RuleType.Publisher:
                    {
                        customRuleScript = String.Format("$Rule_{0} = New-CIPolicyRule -Level {1} -DriverFilePath \'{2}\'" + 
                            " -Fallback Hash", customRule.PSVariable, customRule.GetRuleLevel(), customRule.ReferenceFile);
                    }
                    break;

                case PolicyCustomRules.RuleType.FilePath:
                    {
                        customRuleScript = String.Format("$Rule_{0} = New-CIPolicyRule -Level FilePath -DriverFilePath \"{1}\"" +
                            " -Fallback Hash -UserWriteablePaths", customRule.PSVariable, customRule.ReferenceFile); // -UserWriteablePaths allows all paths (userWriteable and non) to be added as filepath rules
                    }
                    break;

                case PolicyCustomRules.RuleType.Folder:
                    {
                        // Check if part of the folder path can be replaced with an env variable eg. %OSDRIVE% == "C:\"
                        if (customRule.GetRuleType() == PolicyCustomRules.RuleType.FilePath &&
                            Properties.Settings.Default.useEnvVars && customRule.isEnvVar())
                            customRuleScript = String.Format("$Rule_{0} = New-CIPolicyRule -FilePathRule \"{1}\"", customRule.PSVariable, customRule.GetEnvVar());
                        else
                            customRuleScript = String.Format("$Rule_{0} = New-CIPolicyRule -FilePathRule \"{1}\"", customRule.PSVariable, customRule.ReferenceFile);
                    }
                    break;

                case PolicyCustomRules.RuleType.FileAttributes:
                    {
                        customRuleScript = String.Format("$Rule_{0} = New-CIPolicyRule -Level FileName -SpecificFileNameLevel {1} -DriverFilePath \"{2}\" " +
                            "-Fallback Hash", customRule.PSVariable, customRule.GetRuleLevel(), customRule.ReferenceFile);
                    }
                    break;

                case PolicyCustomRules.RuleType.Hash:
                    {
                        customRuleScript = String.Format("$Rule_{0} = New-CIPolicyRule -Level {1} -DriverFilePath \"{2}\" ", customRule.PSVariable, customRule.GetRuleLevel(), customRule.ReferenceFile);
                    }
                    break;
            }

            // If this is a deny rule, append the Deny switch
            if (customRule.GetRulePermission() == PolicyCustomRules.RulePermission.Deny)
                customRuleScript += " -Deny";

            return customRuleScript; 
        }

        /// <summary>
        /// Creates a unique CI Policy file per custom rule defined in the SigningRules_Control. Writes to a unique filepath.
        /// </summary>
        public string createPolicyScript(PolicyCustomRules customRule, string tempPolicyPath)
        {
            string policyScript = string.Empty;

            if (this.Policy._Format == WDAC_Policy.Format.MultiPolicy)
                policyScript = String.Format("New-CIPolicy -MultiplePolicyFormat -FilePath \"{0}\" -Rules $Rule_{1}", tempPolicyPath, customRule.PSVariable);

            else
                policyScript = String.Format("New-CIPolicy -FilePath \"{0}\" -Rules $Rule_{1}", tempPolicyPath, customRule.PSVariable);

            return policyScript; 
        }

        
        /// <summary>
        /// Merges all of the CI Policies created for each custom rule. Writes the merged policy into one OutputFilePath (input)
        /// </summary>
        public void MergeCustomRulesPolicy(List<string> customRulesPathList, string outputFilePath, BackgroundWorker worker)
        {
            this.Log.AddInfoMsg("--- Merge Custom Rules Policy ---");

            string mergeScript = String.Empty;
            if (customRulesPathList.Count > 0)
            {
                // Add all the merge paths
                // First policy in the merge list of policies will determeine the output policy format
                // Since we set the format in ProcessCustomRules(), the customRulesPathList will be the correct format
                mergeScript = "Merge-CIPolicy -PolicyPaths ";
                foreach (string path in customRulesPathList)
                    mergeScript += String.Format("\"{0}\",", path);

                // Remove last comma and add outputFilePath
                mergeScript = mergeScript.Remove(mergeScript.Length - 1);
                mergeScript += String.Format(" -OutputFilePath \"{0}\"", outputFilePath);
            }

            // Create runspace, pipeline and runscript
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(mergeScript);
            pipeline.Commands.Add("Out-String");

            this.Log.AddInfoMsg(String.Format("Running the following commands: {0}", mergeScript));

            Collection<PSObject> results = pipeline.Invoke();

            runspace.Dispose();
            worker.ReportProgress(85);
        }

        /// <summary>
        /// Merges the template policy, supplemental policy, and/or custom rules policy into the user's desired output path
        /// </summary>
        public void MergeTemplatesPolicy(string customRulesMergePath, BackgroundWorker worker)
        {
            // Template policy @ this.TemplatePath, Supplemental policy @ this.BaseToSupplementPath
            // Operations: Merge template (always applicable) with suppleme (if applicable) with 
            //             merge results from MergeCustomRulesPolicy (if applicable) into policy 
            //             defined by user: Path: this.SchemaPath

            // If Edit mode is selected, this.SchemaPath is null while this.TemplatePath points to the .xml file 
            // the user would like to edit. Since we don't explicitly prompt the user for a new path. copy the TemplatePath
            // and append '_Edit' to the file path.
            if (this.Policy._PolicyType == WDAC_Policy.PolicyType.Edit)
            {
                // Check if _v10.0.x.y is already in string ie. editing the output of an editing workflow
                if(this.Policy.EditPathContainsVersionInfo())
                {
                    int sOFFSET = 14;
                    this.Policy.SchemaPath = String.Format("{0}_v{1}.xml", this.Policy.EditPolicyPath.Substring(0,
                        this.Policy.EditPolicyPath.Length - sOFFSET),this.Policy.UpdateVersion());
                }

                else
                {
                    this.Policy.SchemaPath = String.Format("{0}_v{1}.xml", this.Policy.EditPolicyPath.Substring(
                    0, this.Policy.EditPolicyPath.Length - 4), this.Policy.UpdateVersion());
                }
            }

            // Check if user-writeable. If it is not, default to MyDocuments
            if(!WriteAccess(Path.GetDirectoryName(this.Policy.SchemaPath)))
            {
                this.Policy.SchemaPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Path.GetFileName(this.Policy.SchemaPath)); 
            }

           this.Log.AddInfoMsg("--- Merge Templates Policy ---");
            string DEST_PATH = System.IO.Path.Combine(this.TempFolderPath, "OutputSchema.xml"); //this.TempFolderPath + @"\OutputSchema.xml";

            List<string> policyPaths = new List<string>();
            string mergeScript = "Merge-CIPolicy -PolicyPaths ";

            // First merged policy will define the type of the output. eg) if merging a legcy with multi --> output=legacy, vice-versa
            // Again, the policy at customRulesMergePath will be the correct format and will determine the output format as its first in the command
            // TODO: what if CustomRules is empty ??
            if (this.Policy.CustomRules.Count > 0)
            {
                policyPaths.Add(customRulesMergePath);
            }

            if (this.Policy.TemplatePath != null)
            {
                policyPaths.Add(this.Policy.TemplatePath);
            }

            if (this.Policy.PoliciesToMerge.Count > 0)
            {
                foreach(var path in this.Policy.PoliciesToMerge)
                {
                    policyPaths.Add(path);
                }
            }
                
            // Merge-CIPolicy command requires at MIN 1 valid input policy:
            if (policyPaths.Count < 1)
            {
                this.Log.AddErrorMsg("MergeTemplatesPolicy() encountered the following error: Unable to locate any policies to merge");
            }
                

            foreach (var policyPath in policyPaths)
            {
                mergeScript += String.Format("\"{0}\",", policyPath);
            }

            // Remove last comma and add outputFilePath
            // TODO: check if this.Policy.SchemaPath is user-writeable
            mergeScript = mergeScript.Remove(mergeScript.Length - 1);
            mergeScript += String.Format(" -OutputFilePath \"{0}\"", this.Policy.SchemaPath);

            this.Log.AddInfoMsg("Running the following Merge Commands: ");
            this.Log.AddInfoMsg(mergeScript); 

            // Create runspace, pipeline and runscript
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(mergeScript);
            pipeline.Commands.Add("Out-String");
            try
            {
                Collection<PSObject> results = pipeline.Invoke();
                // Make copy of the finished schema file
                File.Copy(this.Policy.SchemaPath, DEST_PATH);
            }
            catch (Exception e)
            {
                Console.WriteLine(String.Format("Exception encountered: {0}", e));
            }
            runspace.Dispose();

            //TODO: check output
            worker.ReportProgress(95);

        }

        /// <summary>
        /// Sets the additonal parameters at the end of a policy: GUIDs, versions, etc
        /// </summary>
        void SetAdditionalParameters(BackgroundWorker worker)
        {
            // Operations: Set HVCI options, policy version, policy ID values, GUIDs:
            this.Log.AddInfoMsg("-- Set Additional Parameters --");

            // Create runspace, pipeline and runscript
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();

            // IF the policy is multi format ONLY, set policy info, and reset the guids
            if (this.Policy._Format == WDAC_Policy.Format.MultiPolicy)
            {
                // Set policy info - ID, Name
                string setIdInfoCmd = String.Format("Set-CIPolicyIdInfo -FilePath \"{0}\" -PolicyID \"{1}\" -PolicyName \"{2}\"", this.Policy.SchemaPath, this.Policy.PolicyID, this.Policy.PolicyName);

                // Reset the GUIDs s.t. does not mirror the policy GUID 
                string resetGuidsCmd = String.Format("Set-CIPolicyIdInfo -FilePath \"{0}\" -ResetPolicyID", this.Policy.SchemaPath);

                pipeline.Commands.AddScript(setIdInfoCmd);
                pipeline.Commands.AddScript(resetGuidsCmd);
            }

            
            if (this.Policy.EnableHVCI)
            {
                // Set the HVCI value at the end of the xml document
                string setHVCIOptsCmd = String.Format("Set-HVCIOptions -Enabled -FilePath \"{0}\"", this.Policy.SchemaPath);
                pipeline.Commands.AddScript(setHVCIOptsCmd);
            }

            // If supplemental policy, set the Base policy guid
            if (this.Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy)
            {
                string setBaseGuidCmd = String.Format("Set-CIPolicyIdInfo -FilePath \"{0}\" -BasePolicyToSupplementPath \"{1}\"", 
                    this.Policy.SchemaPath, this.Policy.BaseToSupplementPath);
                pipeline.Commands.AddScript(setBaseGuidCmd);
            }

            // Update the version number on the edited policies. If not specified, version defaults to 10.0.0.0
            if (this.Policy._PolicyType == WDAC_Policy.PolicyType.Edit)
            {
                string updateVersionCmd = String.Format("Set-CIPolicyVersion -FilePath \"{0}\" -Version \"{1}\"", this.Policy.SchemaPath, this.Policy.VersionNumber);
                pipeline.Commands.AddScript(updateVersionCmd);
            }

            if (pipeline.Commands.Count > 0)
            {
                this.Log.AddInfoMsg("Running the following Add Params Commands: ");
                foreach (Command command in pipeline.Commands)
                {
                    this.Log.AddInfoMsg(command.ToString());
                }

                try
                {
                    Collection<PSObject> results = pipeline.Invoke();
                }
                catch (Exception e)
                {
                    this.Log.AddErrorMsg(String.Format("Exception encountered in SetAdditionalParameters(): {0}", e));
                }
            }
            
            runspace.Dispose();
        }

        /// <summary>
        /// Method to convert the xml policy file into a binary CI policy file
        /// </summary>
        public bool ConvertPolicyToBinary()
        {
            // Operations: Converts the xml schema into a binary policy
            this.Log.AddInfoMsg("-- Converting to Binary --");

            // Create runspace, pipeline and runscript
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();

            string binaryFilePath = Path.Combine(Path.GetDirectoryName(this.Policy.SchemaPath), Path.GetFileNameWithoutExtension(this.Policy.SchemaPath)) + ".bin"; //stripped the path remove the .xml --> .bin

            string binConvertCmd = String.Format("ConvertFrom-CIPolicy -XmlFilePath \"{0}\" -BinaryFilePath \"{1}\"",
                this.Policy.SchemaPath, binaryFilePath);

            pipeline.Commands.AddScript(binConvertCmd);

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
                return true;
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg(String.Format("Exception encountered in ConvertPolicyToBinary(): {0}", exp));
                return false; 
            }
        }

        //
        // Summary:
        //     Calls the Get-SystemDriver commandlet and parses the output for the driver variable
        //     Requires scanPath input, 
        //      $Files_{0} = Get-SystemDriver -ScanPath '{1}' -UserPEs -PathToCatroot ' '
        //      
        // Returns:
        //     A dictionary of Get-SystemDriver PS command output
        public Dictionary<string, string> GetSystemInfo(string scanPath)
        {
            this.Log.AddInfoMsg("-- Get System Info --");

            Dictionary<string, string> systemInfo = new Dictionary<string, string>();
            string scanFolderPath = scanPath.Substring(0, scanPath.LastIndexOf(@"\"));
            string fileName = scanPath.Substring(scanPath.LastIndexOf(@"\"));
            
            // Command will omit sub folders (recurssively) to optimize perf
            string sysInfoScript = String.Format("$Files_{0} = Get-SystemDriver -ScanPath \"{1}\" -UserPEs -PathToCatroot ' ' -OmitPaths \"{2}\"", 
                this.RulesNumber, scanFolderPath, GetListSubFolders(scanFolderPath));
            this.Log.AddInfoMsg(sysInfoScript);

            // Create the PS pipeline
            Pipeline pipeline = this.runspace.CreatePipeline();
            pipeline.Commands.AddScript(sysInfoScript);
            pipeline.Commands.AddScript(String.Format("$Files_{0}", this.RulesNumber));
            pipeline.Commands.Add("Out-String");

            // Run the pipeline and cast results into a stringbuilder obj
            Collection<PSObject> results = pipeline.Invoke();
            StringBuilder sBuilder = new StringBuilder();
            foreach (PSObject psObject in results)
                sBuilder.AppendLine(psObject.ToString());

            // Parse the SystemDriver cmdlet output for the scanPath only
            string scriptOutput = sBuilder.ToString();
            systemInfo = ParseSystemInfo(scriptOutput, fileName);
            systemInfo.Add("PSVar", this.RulesNumber.ToString());
            this.RulesNumber++;

            // Log the parsing results 
            Dictionary<string, string>.KeyCollection keys = systemInfo.Keys;
            foreach (string key in keys)
                this.Log.AddInfoMsg(String.Format("{0} - {1}", key, systemInfo[key])); 

            return systemInfo;
        }

        //
        // Summary:
        //     Takes input of Powershell Get-SystemDriver ouput and parses the output for the desired object. 
        //      
        // Returns:
        //     Returns dictionary of Powershell Get-SystemDriver output
        private Dictionary<string, string> ParseSystemInfo(string scriptOutput, string scanPath)
        {
            // Find area we want to scan:
            var dictStarts = new List<int>();
            var searchPath = new List<int>();
            Dictionary<string, string> Dict = new Dictionary<string, string>();
            int ruleIndex = 0;

            try
            {
                for (int i = scriptOutput.IndexOf("FilePath"); i > -1; i = scriptOutput.IndexOf("FilePath", i + 1))
                    dictStarts.Add(i);
                dictStarts.Add(scriptOutput.Length);

                for (int i = 0; i < dictStarts.Count - 1; i++)
                {
                    string searchString = scriptOutput.Substring(dictStarts[i], dictStarts[i + 1] - dictStarts[i] - 1);
                    if (searchString.Contains(scanPath))
                    {
                        searchPath.Add(dictStarts[i]);
                        searchPath.Add(dictStarts[i + 1]);
                        break;
                    }
                    ruleIndex++;
                }

                if (searchPath.Count == 0)
                    return Dict;

                // Find indices of colons - omit cases where its part of the PE path
                scriptOutput = scriptOutput.Substring(searchPath[0], searchPath[1] - searchPath[0]);
                var colonIndx = new List<int>();
                var startIndx = new List<int>();
                startIndx.Add(-2);
                for (int i = scriptOutput.IndexOf("\r\n"); i > -1; i = scriptOutput.IndexOf("\r\n", i + 1))
                {
                    // Handle exceptions of file locations eg. "C:\\" or Prod name is a website eg. https://www. 
                    // In either case we do not want to catch these colons
                    //if (scriptOutput[i + 1].ToString() != @"\" && scriptOutput[i + 1].ToString() != "/")
                    startIndx.Add(i);
                }

                for (int i = 0; i < startIndx.Count - 2; i++)
                {
                    int START = startIndx[i] + 2;
                    int END = startIndx[i + 1];
                    string subString = scriptOutput.Substring(START, END - START);

                    int endofKey = subString.IndexOf(":");
                    // If unable to locate key-value pairing, finished parsing
                    if (endofKey < 0)
                        break;
                    string key = subString.Substring(0, endofKey);
                    string value = subString.Substring(endofKey + 1);
                    Dict.Add(RemoveWhitespace(key), value);
                }

                Dict.Add("RuleIndex", Convert.ToString(ruleIndex));
            }
            catch(Exception e)
            {
                var st = new StackTrace(e, true);
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                int line = frame.GetFileLineNumber();
                this.Log.AddErrorMsg("Parse System Info () encountered the following error ", e, line);
            }

            
            return Dict;
        }

        //
        // Summary:
        //     Removes the whitespace of an input string. Important for dict keys
        //      
        // Returns:
        //     String without whitespace. Eg) "key name   " --> "keyname" 
        public static string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }

        public string GetListSubFolders(string folderPath)
        {
            var subFolderList = Directory.GetDirectories(folderPath);
            if (subFolderList.Length > 0)
            {
                string subfolders = String.Empty;
                foreach (var folder in subFolderList)
                    subfolders += String.Format("'{0}', ", folder);

                this.Log.AddInfoMsg(String.Format("Found the following subfolders for {0} -- {1}", folderPath, subfolders));
                return subfolders.Substring(0, subfolders.Length - 2); //trim the trailing comma and space
            }
            else
                return "' '"; 
            
        }

        //
        // Summary:
        //     Scans the input string folderPth and finds the filepath with the greatest _ID. 
        //      
        // Returns:
        //     String with the newest _ID filename. example) policy_44.xml 
        private string getUniquePolicyPath(string folderPth)
        {
            string newUniquePath = "";
            int NewestID = -1;
            int Start, End;

            DirectoryInfo dir = new DirectoryInfo(folderPth);

            foreach (var file in dir.GetFiles("*.xml"))
            {
                this.Log.AddInfoMsg(String.Format("Found xml file, {0}", file.Name)); 
                Start = file.Name.IndexOf("policy_") + 7;
                End = file.Name.IndexOf(".xml");

                // If Start indexof returns -1, 
                if (Start == 6)
                {
                    continue; 
                }

                int ID = Convert.ToInt32(file.Name.Substring(Start, End - Start));

                if (ID > NewestID)
                    NewestID = ID;
            }

            if (NewestID < 0)
            {
                newUniquePath = System.IO.Path.Combine(folderPth, "policy_0.xml"); //first temp policy being created
            }
            else
            {
                newUniquePath = System.IO.Path.Combine(folderPth, String.Format("policy_{0}.xml", NewestID + 1));
            }

            this.Log.AddInfoMsg(String.Format("Unique Policy Path returned: {0}", newUniquePath));
            return newUniquePath;
        }

        
        /// <summary>
        /// Retrieves the current date and formats it in descending order.
        /// </summary>
        /// <param name="includeTime">Bool indicating whether to include the time string (hour+min) in return</param>
        /// <returns>Returns Date string with format: MM-dd-yy_HH-mm [24 hour time] of MM-dd-yy</returns>
        public string formatDate(bool includeTime=true)
        {
            DateTime sDate = DateTime.Now;
            if(includeTime)
                return String.Format("{0}{1}{2}_{3}{4}", sDate.ToString("MM"), sDate.ToString("dd"),
                    sDate.ToString("yy"), sDate.ToString("HH"), sDate.ToString("mm"));
            else
                return String.Format("{0}{1}{2}", sDate.ToString("MM"), sDate.ToString("dd"),
                    sDate.ToString("yy"));
        }

        // UI helper functions

        //
        // Summary:
        //     Prompts the user to confirm if they want to abandon progress
        //      
        // Returns:
        //     Bool - true: abandon work, false - cancel abandon work 
        private bool wantToAbandonWork()
        {
            this.Log.AddWarningMsg("Abandon Work Entered.");
            DialogResult res = MessageBox.Show("Are you sure you want to abandon your progress?", 
                "Confirmation", 
                MessageBoxButtons.OKCancel, 
                MessageBoxIcon.Information);

            if (res == DialogResult.OK)
            {
                this.Log.AddWarningMsg("Abandon Work returned OK.");
                return true;
            }
            else if (res == DialogResult.Cancel)
            {
                this.Log.AddWarningMsg("Abandon Work returned Cancel.");
                return false;
            }
            else
            {
                this.Log.AddWarningMsg("Abandon Work returned Other.");
                return false;
            }
                
        }

        //
        // Summary:
        //     Displays the left-most control panel with home, settings buttons and progress pages. 
        //     Displays the highlighting panel to show users current page.
        //      
        // Returns:
        //     None. 
        private void ShowControlPanel(object sender, EventArgs e)
        {
            Controls.Add(control_Panel);
            control_Panel.BringToFront();
            control_Panel.Focus();

            this.Controls.Add(button_Next);
            button_Next.BringToFront();
            button_Next.Focus(); 

            // Set highlight panel location
            int X_OFFSET = 15;
            int Y_OFFSET = 5;
            try
            {
                var sideButton = (Button)sender; 
                controlHighlight_Panel.Location = new System.Drawing.Point(sideButton.Location.X - X_OFFSET, sideButton.Location.Y + Y_OFFSET);
            }
            catch(Exception exc)
            {
                controlHighlight_Panel.Location = new System.Drawing.Point(this.home_Button.Location.X - X_OFFSET, this.home_Button.Location.Y + Y_OFFSET);
            }

            // Enable Settings Button -- if on building page, it will be disabled below
            this.settings_Button.Enabled = true; 

            // Set link text
            switch (this.Policy._PolicyType)
            {
                case WDAC_Policy.PolicyType.BasePolicy: // Policy creator
                    this.workflow_Label.Visible = true;
                    this.page1_Button.Visible = true;
                    this.page2_Button.Visible = true;
                    this.page3_Button.Visible = true;
                    this.page4_Button.Visible = true;
                    this.page5_Button.Visible = true;
                    this.workflow_Label.Text = "Policy Creator";
                    this.page1_Button.Text = "Policy Type"; 
                    this.page2_Button.Text = "Policy Template"; 
                    this.page3_Button.Text = "Policy Rules"; 
                    this.page4_Button.Text = "Signing Rules";
                    this.page5_Button.Text = "Creating Policy"; 
                    break;

                case WDAC_Policy.PolicyType.SupplementalPolicy: // Policy creator
                    this.workflow_Label.Visible = true;
                    this.page1_Button.Visible = true;
                    this.page2_Button.Visible = true;
                    this.page3_Button.Visible = true;
                    this.page4_Button.Visible = true;
                    this.page5_Button.Visible = false;

                    this.workflow_Label.Text = "Policy Creator";
                    this.page1_Button.Text = "Policy Type";
                    this.page2_Button.Text = "Policy Rules";
                    this.page3_Button.Text = "Signing Rules";
                    this.page4_Button.Text = "Creating Policy";
                    break;

                case WDAC_Policy.PolicyType.Edit: // policy editor
                    this.workflow_Label.Visible = true;
                    this.page1_Button.Visible = true;
                    this.page2_Button.Visible = true;
                    this.page3_Button.Visible = true;
                    this.page4_Button.Visible = true;
                    this.page5_Button.Visible = false;

                    this.workflow_Label.Text = "Policy Editor";
                    this.page1_Button.Text = "Select Policy";
                    this.page2_Button.Text = "Policy Rules";
                    this.page3_Button.Text = "Signing Rules";
                    this.page4_Button.Text = "Creating Policy";
                    break;

                case WDAC_Policy.PolicyType.Merge: // merger
                    this.workflow_Label.Visible = true;
                    this.page1_Button.Visible = true;
                    this.page2_Button.Visible = true;
                    this.page3_Button.Visible = false;
                    this.page4_Button.Visible = false;
                    this.page5_Button.Visible = false;

                    this.workflow_Label.Text = "Policy Merger";
                    this.page1_Button.Text = "Select Policies";
                    this.page2_Button.Text = "Creating Policy";

                    break;

                case WDAC_Policy.PolicyType.None: // not yet set (first page) -- default to view 
                    
                    switch(this.view)
                    {
                        case 1: // New policy
                            this.workflow_Label.Visible = true;
                            this.page1_Button.Visible = true;
                            this.page2_Button.Visible = false;
                            this.page3_Button.Visible = false;
                            this.page4_Button.Visible = false;
                            this.page5_Button.Visible = false;
                            this.workflow_Label.Text = "Policy Creator";
                            this.page1_Button.Text = "Policy Type";
                            break;

                        case 2: // edit policy
                            this.workflow_Label.Visible = true;
                            this.page1_Button.Visible = true;
                            this.page2_Button.Visible = true;
                            this.page3_Button.Visible = true;
                            this.page4_Button.Visible = true;
                            this.workflow_Label.Text = "Policy Editor";
                            this.page1_Button.Text = "Select Policy";
                            this.page2_Button.Text = "Policy Rules";
                            this.page3_Button.Text = "Signing Rules";
                            this.page4_Button.Text = "Creating Policy";
                            break;


                        case 3: // Merge workflow

                            break; 
                    }
                    break; 


                default:
                    this.workflow_Label.Visible = false;
                    this.page1_Button.Visible = false;
                    this.page2_Button.Visible = false;
                    this.page3_Button.Visible = false;
                    this.page4_Button.Visible = false;
                    this.page5_Button.Visible = false;
                    break; 
            }

            // Lazy implementation of highlighting current page in workflow
            switch(this.CurrentPage)
            {
                case 1:
                    this.page1_Button.Enabled = true;
                    this.page2_Button.Enabled = false;
                    this.page3_Button.Enabled = false;
                    this.page4_Button.Enabled = false;
                    this.page5_Button.Enabled = false;
                    controlHighlight_Panel.Location = new System.Drawing.Point(this.page1_Button.Location.X - X_OFFSET, this.page1_Button.Location.Y + Y_OFFSET);
                    break;

                case 2:
                    if(this.view == 3)
                    {
                        // Building page
                        this.page1_Button.Enabled = false;
                        this.page2_Button.Enabled = false;
                        this.page3_Button.Enabled = false;
                        this.page4_Button.Enabled = false;
                        this.page5_Button.Enabled = false;

                        this.settings_Button.Enabled = false; 
                    }
                    else
                    {
                        this.page1_Button.Enabled = true;
                        this.page2_Button.Enabled = true;
                        this.page3_Button.Enabled = false;
                        this.page4_Button.Enabled = false;
                        this.page5_Button.Enabled = false;
                    }
                    controlHighlight_Panel.Location = new System.Drawing.Point(this.page2_Button.Location.X - X_OFFSET, this.page2_Button.Location.Y + Y_OFFSET);
                    break;

                case 3:
                    this.page1_Button.Enabled = true;
                    this.page2_Button.Enabled = true;
                    this.page3_Button.Enabled = true;
                    this.page4_Button.Enabled = false;
                    this.page5_Button.Enabled = false;
                    controlHighlight_Panel.Location = new System.Drawing.Point(this.page3_Button.Location.X - X_OFFSET, this.page3_Button.Location.Y + Y_OFFSET);
                    break;

                case 4:
                    if(this.view == 2)
                    {
                        // Building page
                        this.page1_Button.Enabled = false;
                        this.page2_Button.Enabled = false;
                        this.page3_Button.Enabled = false;
                        this.page4_Button.Enabled = false;

                        this.settings_Button.Enabled = false;
                    }
                    else
                    {
                        this.page1_Button.Enabled = true;
                        this.page2_Button.Enabled = true;
                        this.page3_Button.Enabled = true;
                        this.page4_Button.Enabled = true;
                        this.page5_Button.Enabled = false;
                    }
                    
                    controlHighlight_Panel.Location = new System.Drawing.Point(this.page4_Button.Location.X - X_OFFSET, this.page4_Button.Location.Y + Y_OFFSET);
                    break;

                case 5:
                    // Building page

                    this.page1_Button.Enabled = false;
                    this.page2_Button.Enabled = false;
                    this.page3_Button.Enabled = false;
                    this.page4_Button.Enabled = false;
                    this.page5_Button.Enabled = false;

                    this.settings_Button.Enabled = false;

                    controlHighlight_Panel.Location = new System.Drawing.Point(this.page5_Button.Location.X - X_OFFSET, this.page5_Button.Location.Y + Y_OFFSET);
                    break;
            }

        }

        //
        // Summary:
        //     Empties this.Controls based on contents of PageList. 
        //      
        // Returns:
        //     None.
        private void RemoveControls()
        {
            // Empty this.PageList so that we can start a new workflow after policy build
            foreach (var page in this.PageList)
                this.Controls.RemoveByKey(page);
            this.PageList.Clear(); 
        }

        
        /// <summary>
        /// Public method to set the text and visibility of the info text label at bottom-left of the form or user control.
        /// </summary>
        public void display_info_text(int infoN)
        {
            label_Info.Visible = true;
            label_Info.ForeColor = Color.DeepSkyBlue;

            switch (infoN)
            {
                case 0:
                    // Reset label
                    label_Info.Text = " ";
                    break;

                case 1:
                    label_Info.Text = "Windows S Mode Mode does the following";
                    break;

                case 2:
                    label_Info.Text = "Signed and Reputable Mode does the following";
                    break;

                case 3:
                    label_Info.Text = "Audit Only Mode does the following";
                    break;

                
                case 97:
                    label_Info.Text = "Please select an exisiting policy .xml file before continuing.";
                    label_Info.ForeColor = Color.Red;
                    this.Log.AddWarningMsg("Page controller triggered without selecting existing policy .xml file"); 
                    break;

                case 98:
                    label_Info.Text = "Please select one of the options before continuing.";
                    label_Info.ForeColor = Color.Red;
                    this.Log.AddWarningMsg("Page controller triggered without selecting a policy template");
                    break;

                case 99:

                    label_Info.Text = this.ErrorMsg;
                    label_Info.ForeColor = Color.Red;
                    this.Log.AddErrorMsg(this.ErrorMsg);
                        break;

                default:
                    label_Info.Text = " ";
                    break;

            }

            label_Info.Focus();
            label_Info.BringToFront();             
            
            Timer settingsUpdateNotificationTimer = new Timer();
            settingsUpdateNotificationTimer.Interval = (5000); //3 secs
            settingsUpdateNotificationTimer.Tick += new EventHandler(SettingUpdateTimer_Tick);
            settingsUpdateNotificationTimer.Start();

        }

        public void ResetWorkflow(object sender, EventArgs e)
        {
            this.home_Button_Click(sender, e); 
        }

        //
        // Summary:
        //     Checks for updates on the startup of the application. Currently set to grab updates from the scratch location. 
        //      
        // Returns:
        //     None.
        /*private async Task CheckForUpdates()
        {
            this.Log.AddInfoMsg("Checking for Updates -- STARTED"); 
            // TODO: point the update manager to github
            using (var manager = new UpdateManager(@"\\scratch2\scratch\jogeurte\WDACWizard\Releases\"))
            {
                await manager.UpdateApp();
                this.Log.AddInfoMsg("Checking for Updates -- UPDATING APP");
            }
            this.Log.AddInfoMsg("Checking for Updates -- FINISHED");
        }*/


        //
        // Summary:
        //     Creates the session folder. Contents such as the log, intermediate policies are stored here.  
        //      
        // Returns:
        //     Returns the folder path for the current app session.
        private string CreateTempFolder()
        {
            //AppData + WDAC Temp folder
            string tempFolder = Path.Combine("WDACWizard", "Temp", formatDate()); 
            string tempFolderPath = Path.Combine(Path.GetTempPath(), tempFolder); 

            if (!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            return tempFolderPath; 


        }

        /// <summary>
        /// Triggered upon application closing event. 
        /// /// </summary>
        private void FormClosing_Event(object sender, FormClosingEventArgs e)
        {
            this.Log.CloseLogger();
        }

        private string GetExecutablePath(bool exePath)
        {
            string executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string folderPath = System.IO.Path.GetDirectoryName(executablePath);
            if (exePath)
                return executablePath;
            else
                return folderPath; 
        }

        /// <summary>
        /// Check that the given directory is write-accessable by the user.  
        /// /// </summary>
        private bool WriteAccess(string folderPath)
        {
            // Try to create a subdir in the folderPath. If successful, write access is true. 
            // If an exception is hit, the path is likely not user-writeable 
            try
            {
                DirectoryInfo di = new DirectoryInfo(folderPath); 
                if(di.Exists)
                {
                    DirectoryInfo dis = new DirectoryInfo(Path.Combine(folderPath, "testSubDir"));
                    dis.Create();
                    dis.Delete(); 
                }

                return true; 
            }
            catch(Exception e)
            {
                this.Log.AddErrorMsg("WriteAccess() encountered the following exception: " + e); 
                return false; 
            }
        }

        // SKU check if cmdlets are available on the device 
        private void LicenseCheck()
        {
            // Check that WDAC feature is compatible with system
            // Cmdlets are available on all builds 1909+. 
            // Pre-1909, Enterprise SKU only: https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-defender-application-control/feature-availability

            int REQUIRED_V = 1909;
            string REQUIRED_ED = "Enterprise";
            int supt_flag;

            this.Log.AddInfoMsg("--- Feature Compat Check ---"); 

            string edition = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CompositionEditionID", "").ToString();
            string prodName = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "").ToString();
            int releaseN = Convert.ToInt32(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", ""));

            if (releaseN >= REQUIRED_V)
            {
                supt_flag = 1;
                this.Log.AddInfoMsg(String.Format("Release Id: {0} meets min build requirements.", releaseN));
            }
            else if (edition.Contains(REQUIRED_ED) || prodName.Contains(REQUIRED_ED))
            {
                supt_flag = 1;
                this.Log.AddInfoMsg(String.Format("Edition/ProdName:{0}/{1} meets min build requirements.", edition, prodName));
            }
            else
                supt_flag = 0; 

        
            if (supt_flag == 0) // edition or prod name not found in either reg key, n_ed_sup = 0, throw warn msg
            {
                this.Log.AddWarningMsg(String.Format("Incompatible Windows Build Detected!! BuildN={0}", releaseN));
                this.Log.AddWarningMsg(String.Format("Incompatible Windows Edition/Product Detected!! CompositionEditionID={0} and ProductName={1}", edition, prodName));
                DialogResult res = MessageBox.Show("The Policy Wizard has detected an incompatible version of Windows. " +
                    "The Wizard may not be able to successfully complete policy creation.",
                    "Incompatible Windows Product", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);

                if (res == DialogResult.OK)
                {
                    System.Diagnostics.Process.Start("https://docs.microsoft.com/en-us/windows/security/threat-protection/windows-defender-application-control/feature-availability");
                }
            }
        }

        public int getReleaseId()
        {
            return Convert.ToInt32(Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", ""));
        }

        private void SettingUpdateTimer_Tick(object sender, EventArgs e)
        {
            this.label_Info.Visible = false;
        }
    }

}
