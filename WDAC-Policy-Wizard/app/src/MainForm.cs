// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;
using System.Collections.ObjectModel;
using System.Management.Automation.Runspaces;
using Squirrel;
using System.Diagnostics;
using WDAC_Wizard.Properties;

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

        public Logger Log { get; set; }
        private List<string> PageList;
        public WDAC_Policy Policy { get; set; }
        // Runspace param to access all PS Variables and eliminate overhead opening each time
        private Runspace runspace;
        private int RulesNumber;
        public string TempFolderPath { get; set; }

        // Edit Workflow datastructs
        private BuildPage _BuildPage;

        public MainWindow()
        {
            this.TempFolderPath = CreateTempFolder();
            this.Log = new Logger(this.TempFolderPath); 
            
            InitializeComponent();
            CheckForUpdates();

            // Init MainWindow params
            this.ConfigInProcess = false;
            this.CurrentPage = 0;
            this.RulesNumber = 0;

            this.Policy = new WDAC_Policy();
            this.PageList = new List<string>(); 
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
                this.Log.AddInfoMsg("Workflow -- New Policy Selected");
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
                this.Log.AddInfoMsg("Workflow -- Edit Policy Selected");

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
            this.Policy.VersionNumber = "10.9.9.9";
            Console.WriteLine(this.Policy.UpdateVersion());

            this.Policy.VersionNumber = "10.0.0.5";
            Console.WriteLine(this.Policy.UpdateVersion()); 

            if (!this.ConfigInProcess)
            {
                this.view = 3;
                this.ConfigInProcess = true;
                this.CurrentPage = 1; 
                this.Log.AddInfoMsg("Workflow -- Merge Policy Selected");

                var _EditWorkflow = new EditWorkflow(this);
                _EditWorkflow.Name = "EditWorkflow";
                this.Controls.Add(_EditWorkflow);
                this.PageList.Add(_EditWorkflow.Name); 
                _EditWorkflow.BringToFront();
                _EditWorkflow.Focus();

                ShowControlPanel(sender, e);
                button_Next.Visible = true;
            }

            else
            {
                // Working on other workflow - do you want to leave?
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
            this.view = 0;
            this.CurrentPage = 0; 

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
                                this.Controls.Add(_PolicyTypePage);
                                this.PageList.Add(_PolicyTypePage.Name);
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
                                this.Controls.Add(_PolicyTypePage);
                                this.PageList.Add(_PolicyTypePage.Name);
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
                                this.Controls.Add(_EditWorkflow);
                                this.PageList.Add(_EditWorkflow.Name);
                                _EditWorkflow.BringToFront();
                                _EditWorkflow.Focus();
                            }

                            ShowControlPanel(sender, e);
                            break;
                    }
                    break;

                
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
                                this.Controls.Add(_NewPolicyPage);
                                this.PageList.Add(_NewPolicyPage.Name);
                                _NewPolicyPage.BringToFront();
                                _NewPolicyPage.Focus();
                            }
                            
                            ShowControlPanel(sender, e);
                            break;

                        case WDAC_Policy.PolicyType.SupplementalPolicy:
                            // New SUPPLEMENTAL policy
                            pageKey = "TemplatePage";
                            // Show policy rules UI
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, launch instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                var _NewSupplementalPage = new TemplatePage(this);
                                _NewSupplementalPage.Name = pageKey; 
                                this.Controls.Add(_NewSupplementalPage);
                                this.PageList.Add(_NewSupplementalPage.Name);
                                _NewSupplementalPage.BringToFront();
                                _NewSupplementalPage.Focus();
                            }
                            ShowControlPanel(sender, e);
                            break;

                        case WDAC_Policy.PolicyType.Edit:
                            // Edit & view mode
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
                                this.Controls.Add(_RulesPage);
                                this.PageList.Add(_RulesPage.Name);
                                _RulesPage.BringToFront();
                                _RulesPage.Focus();
                            }

                            ShowControlPanel(sender, e);
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
                                this.Controls.Add(_ConfigTemplateControl);
                                this.PageList.Add(_ConfigTemplateControl.Name);
                                _ConfigTemplateControl.BringToFront();
                                _ConfigTemplateControl.Focus();
                            }

                            ShowControlPanel(sender, e);
                            break;

                        case WDAC_Policy.PolicyType.SupplementalPolicy: // New SUPPLEMENTAL policy

                            pageKey = "ConfigTemplatePage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, show instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                var _ConfigTemplate = new ConfigTemplate_Control(this);
                                _ConfigTemplate.Name = pageKey; 
                                this.Controls.Add(_ConfigTemplate);
                                this.PageList.Add(_ConfigTemplate.Name);
                                _ConfigTemplate.BringToFront();
                                _ConfigTemplate.Focus();
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
                                this.Controls.Add(_SigningRules_Control);
                                this.PageList.Add(_SigningRules_Control.Name);
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
                                this.Controls.Add(_SigningRules);
                                this.PageList.Add(_SigningRules.Name);
                                _SigningRules.BringToFront();
                                _SigningRules.Focus();
                            }

                            ShowControlPanel(sender, e);
                            break;

                        case WDAC_Policy.PolicyType.SupplementalPolicy:
                            // New SUPPLEMENTAL policy
                            
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
                                this.Controls.Add(_SigningSupplementalRules);
                                this.PageList.Add(_SigningSupplementalRules.Name);
                                _SigningSupplementalRules.BringToFront();
                                _SigningSupplementalRules.Focus();
                            }

                            ShowControlPanel(sender, e);
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
                                this.Controls.Add(this._BuildPage);
                                this.PageList.Add(this._BuildPage.Name);
                                this._BuildPage.BringToFront();
                                this._BuildPage.Focus();
                            }

                            ProcessPolicy();
                            ShowControlPanel(sender, e);
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
                                this.Controls.Add(this._BuildPage);
                                this.PageList.Add(this._BuildPage.Name);
                                this._BuildPage.BringToFront();
                                this._BuildPage.Focus();
                            }

                            ShowControlPanel(sender, e);
                            ProcessPolicy();
                            break;

                        case WDAC_Policy.PolicyType.SupplementalPolicy:
                            // Build out the policy - begin PS execution
                            
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
                                this.Controls.Add(this._BuildPage);
                                this.PageList.Add(this._BuildPage.Name);
                                this._BuildPage.BringToFront();
                                this._BuildPage.Focus();
                                button_Next.Visible = false;
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
            //TODO: change this to program files - temp
            BackgroundWorker worker = sender as BackgroundWorker;
            string MERGEPATH = this.TempFolderPath + @"\FinalPolicy.xml";
            // Handle Policy Rule-Options
            CreatePolicyRuleOptions(worker);

            // Create the driver files
            CreateDriverFiles(worker);

            // Handle custom rules 
            List<string> customRulesPathList = CreatePolicyFileRules(worker);

            // Merge policies - all custom ones and the template and/or the base (if this is a supplemental)
            // For some reason, Merge-CIPolicy -Rules <Rule[]> is not trivial - use -PolicyPaths instead
            MergeCustomRulesPolicy(customRulesPathList, MERGEPATH, worker);

            // Merge with template and/or base:
            MergeTemplatesPolicy(MERGEPATH, worker);

            SetAdditionalParameters(worker);
            ConvertToBinary();
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
                process = "Configuring policy signing rules ...";
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
                pipeline.Commands.AddScript(String.Format("Set-RuleOption -FilePath {0} -Option {1} -Delete ",
                    this.Policy.TemplatePath, i));
            

            foreach (string key in keys)
            {
                // Skip the unsupported rules -- they will throw an error when converting to .bin policy files
                if (this.Policy.ConfigRules[key]["Supported"] == "False")
                    continue; 
                string ruleVal = this.Policy.ConfigRules[key]["CurrentValue"];
                string allowedText = this.Policy.ConfigRules[key]["AllowedValue"];
                if (ruleVal == allowedText) //Value == xml allowable output - write
                {
                    string ruleOptNum = this.Policy.ConfigRules[key]["RuleNumber"];
                    pipeline.Commands.AddScript(String.Format("Set-RuleOption -FilePath {0} -Option {1} ",
                        this.Policy.TemplatePath, ruleOptNum));
                    this.Log.AddInfoMsg(String.Format("Adding rule-option pair: {0}:{1}", key, ruleVal));
                }

                //else skip - invalid value eg)  Disable:UMCI  so omitting it from setting accomplishes this
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
                            filePath, CustomRule.GetRuleType());
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
        /// Creates a unique CI Policy file per custom rule defined in the SigningRules_Control. Writes to a unique filepath.
        /// </summary>
        public List<string> CreatePolicyFileRules(BackgroundWorker worker, bool auditMode = false)
        {
            // Input: directory path to temp appdata
            // Output: list<string> of temp policies .xml files written

            // Get the SigningRules_Control runspace which created the rules
            List<string> customRulesPathList = new List<string>();
            int nRules = this.Policy.CustomRules.Count;
            for (int i = 0; i < nRules; i++)
            {
                var CustomRule = this.Policy.CustomRules[i];
                // Skip if the Custom Rule is a Folder Fule -- we have already broken these out into individual file rules
                // at this point. 
                if (CustomRule.GetRuleLevel() == PolicyCustomRules.RuleLevel.Folder)
                    continue;

                String tempPolicyPath = getUniquePolicyPath(this.TempFolderPath);
                customRulesPathList.Add(tempPolicyPath);

                // Create new CI Rule: https://docs.microsoft.com/en-us/powershell/module/configci/new-cipolicyrule
                string createRuleScript;

                // if the rule is type filepath and settings
                if (CustomRule.GetRuleLevel() == PolicyCustomRules.RuleLevel.FilePath && 
                    Properties.Settings.Default.useEnvVars && CustomRule.isEnvVar())
                        createRuleScript = String.Format("$Rule_{0} = New-CIPolicyRule -FilePathRule {1}", CustomRule.PSVariable, CustomRule.GetEnvVar());
                else
                    createRuleScript = String.Format("$Rule_{0} = New-CIPolicyRule -Level {1} -DriverFiles $Files_{0}[{2}] " + 
                        "-Fallback Hash", CustomRule.PSVariable, CustomRule.GetRuleLevel(), CustomRule.RuleIndex);
                
                string createPolicyScript = ""; 
                // Create new CI Policy from rule: https://docs.microsoft.com/en-us/powershell/module/configci/new-cipolicy
                if (this.Policy.ConfigRules["Allow Supplemental Policies"]["CurrentValue"] 
                    == this.Policy.ConfigRules["Allow Supplemental Policies"]["AllowedValue"]) // if base allows supplemental, set "multiplepolicy format" switch
                    createPolicyScript = String.Format("New-CIPolicy -MultiplePolicyFormat -FilePath {0} -Rules $Rule_{1}", 
                        tempPolicyPath, CustomRule.PSVariable);
                
                else
                    createPolicyScript = String.Format("New-CIPolicy -FilePath {0} -Rules $Rule_{1}", tempPolicyPath, CustomRule.PSVariable);


                if (CustomRule.GetRuleType() == PolicyCustomRules.RuleType.Deny)  // Deny type rule
                    createPolicyScript += " -Deny";

                Pipeline pipeline = this.runspace.CreatePipeline();
                pipeline.Commands.AddScript(createRuleScript);
                pipeline.Commands.AddScript(createPolicyScript);
                this.Log.AddInfoMsg(String.Format("Running the following commands: {0}", createRuleScript));
                this.Log.AddInfoMsg(String.Format("Running the following commands: {0}", createPolicyScript));

                try
                {
                    Collection<PSObject> results = pipeline.Invoke();
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0} Exception caught", e);
                }
                worker.ReportProgress(70 + i / nRules * 10);
            }
            //TODO: results check ensuring 
            runspace.Dispose();
            return customRulesPathList;
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
                mergeScript = "Merge-CIPolicy -PolicyPaths ";
                foreach (string path in customRulesPathList)
                    mergeScript += String.Format("{0},", path);

                // Remove last comma and add outputFilePath
                mergeScript = mergeScript.Remove(mergeScript.Length - 1);
                mergeScript += String.Format(" -OutputFilePath {0}", outputFilePath);
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
            // Template policy @ this.TemplatePath, Supplemental policy @ this.SupplementalPath
            // Operations: Merge template (always applicable) with suppleme (if applicable) with 
            //             merge results from MergeCustomRulesPolicy (if applicable) into policy 
            //             defined by user: Path: this.SchemaPath

            // If Edit mode is selected, this.SchemaPath is null while this.TemplatePath points to the .xml file 
            // the user would like to edit. Since we don't explicitly prompt the user for a new path. copy the TemplatePath
            // and append '_Edit' to the file path.
            if (this.Policy._PolicyType == WDAC_Policy.PolicyType.Edit)
                this.Policy.SchemaPath = String.Format("{0}_v{1}.xml", this.Policy.EditPolicyPath.Substring(
                    0,this.Policy.EditPolicyPath.Length - 4), this.Policy.UpdateVersion());

            this.Log.AddInfoMsg("--- Merge Templates Policy ---");
            string DEST_PATH = this.TempFolderPath + @"\OutputSchema.xml";

            List<string> policyPaths = new List<string>();
            string mergeScript = "Merge-CIPolicy -PolicyPaths ";

            if (this.Policy.TemplatePath != null)
                policyPaths.Add(this.Policy.TemplatePath);

            if (this.Policy.SupplementalPath != null)
                policyPaths.Add(this.Policy.SupplementalPath);

            if (this.Policy.CustomRules.Count > 0)
                policyPaths.Add(customRulesMergePath);

            // Merge-CIPolicy command requires at MIN 1 valid input policy:
            if (policyPaths.Count < 1)
                return;

            foreach (var policyPath in policyPaths)
                mergeScript += String.Format("{0},", policyPath);

            // Remove last comma and add outputFilePath
            mergeScript = mergeScript.Remove(mergeScript.Length - 1);
            mergeScript += String.Format(" -OutputFilePath {0}", this.Policy.SchemaPath);

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

            string setIdInfoCmd = String.Format("Set-CIPolicyIdInfo -FilePath {0} -PolicyID {1} -PolicyName {2}", 
                this.Policy.SchemaPath, formatDate(), this.Policy.PolicyName);
            pipeline.Commands.AddScript(setIdInfoCmd);

            // Set the HVCI value at the end of the xml document
            string setHVCIOptsCmd = String.Format("Set-HVCIOptions -Enabled -FilePath {0}", this.Policy.SchemaPath);
            if (this.Policy.EnableHVCI)
                pipeline.Commands.AddScript(setHVCIOptsCmd);

            // Reset the GUIDs s.t. does not mirror the policy GUID 
            string resetGuidsCmd = String.Format("Set-CIPolicyIdInfo -FilePath {0} -ResetPolicyID", this.Policy.SchemaPath);
            pipeline.Commands.AddScript(resetGuidsCmd);

            // Update the version number on the edited policies. If not specified, version defaults to 10.0.0.0
            string updateVersionCmd = String.Format("Set-CIPolicyVersion -FilePath {0} -Version {1}", this.Policy.SchemaPath, this.Policy.VersionNumber);
            if (this.Policy._PolicyType == WDAC_Policy.PolicyType.Edit)
                pipeline.Commands.AddScript(updateVersionCmd); 

            Collection<PSObject> results = pipeline.Invoke();

            runspace.Dispose();
            worker.ReportProgress(100);

        }

        /// <summary>
        /// Method to convert the xml policy file into a binary CI policy file
        /// </summary>
        public bool ConvertToBinary()
        {
            // Operations: Converts the xml schema into a binary policy
            this.Log.AddInfoMsg("-- Converting to Binary --");

            // Create runspace, pipeline and runscript
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();
            string binaryFilePath = this.Policy.SchemaPath.Substring(0, this.Policy.SchemaPath.Length - 4) + ".bin"; //remove the .xml --> .bin
            string binConvertCmd = String.Format("ConvertFrom-CIPolicy -XmlFilePath {0} -BinaryFilePath {1}" ,
                this.Policy.SchemaPath, binaryFilePath);

            pipeline.Commands.AddScript(binConvertCmd);
            Collection<PSObject> results = pipeline.Invoke();
            return true; 
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

            // Parse the SystemDriver cmdlet output for the scanPath only
            Dictionary<string, string> systemInfo = new Dictionary<string, string>();
            string scanFolderPath = scanPath.Substring(0, scanPath.LastIndexOf(@"\"));
            string fileName = scanPath.Substring(scanPath.LastIndexOf(@"\"));
            string sysInfoScript = String.Format("$Files_{0} = Get-SystemDriver -ScanPath '{1}' -UserPEs -PathToCatroot ' ' -OmitPaths {2}", 
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

            // Parse the output string into a dictionary
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
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        public string GetListSubFolders(string folderPath)
        {
            var subFolderList = Directory.GetDirectories(folderPath);
            string subfolders = String.Empty;
            foreach (var folder in subFolderList)
                subfolders += String.Format("'{0}', ", folder);

            this.Log.AddInfoMsg(String.Format("Found the following subfolders for {0} -- {1}", folderPath, subfolders));
            return subfolders.Substring(0, subfolders.Length-2); //trim the trailing comma and space
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
                string _fileName = file.FullName;
                Start = _fileName.IndexOf("policy_") + 7;
                End = _fileName.IndexOf(".xml");
                // If Start indexof returns -1, 
                if (Start == 6)
                    continue; 

                int ID = Convert.ToInt32(_fileName.Substring(Start, End - Start));

                if (ID > NewestID)
                    NewestID = ID;
            }

            if (NewestID < 0)
                newUniquePath = folderPth + "/policy_0.xml"; //first temp policy being created
            else
                newUniquePath = folderPth + String.Format("/policy_{0}.xml", NewestID + 1);

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
            DialogResult res = MessageBox.Show("Are you sure you want to abandon your progress?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

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
            var sideButton = (Button)sender; 
            controlHighlight_Panel.Location = new Point(sideButton.Location.X - X_OFFSET, sideButton.Location.Y + Y_OFFSET);

            // Set link text
            switch (this.view)
            {
                case 1: // Policy creator
                    this.workflow_Button.Visible = true;
                    this.page1_Button.Visible = true;
                    this.page2_Button.Visible = true;
                    this.page3_Button.Visible = true;
                    this.page4_Button.Visible = true;
                    this.page5_Button.Visible = true;
                    this.workflow_Button.Text = "Policy Creator";
                    this.page1_Button.Text = "Policy Type"; 
                    this.page2_Button.Text = "Policy Template"; 
                    this.page3_Button.Text = "Policy Rules"; 
                    this.page4_Button.Text = "Signing Rules";
                    this.page5_Button.Text = "Creating Policy"; 
                    break;

                case 2: // policy editor
                    this.workflow_Button.Visible = true;
                    this.page1_Button.Visible = true;
                    this.page2_Button.Visible = true;
                    this.page3_Button.Visible = true;
                    this.page4_Button.Visible = true;
                    this.workflow_Button.Text = "Policy Editor";
                    this.page1_Button.Text = "Select Policy";
                    this.page2_Button.Text = "Policy Rules";
                    this.page3_Button.Text = "Signing Rules";
                    this.page4_Button.Text = "Creating Policy";
                    break;

                case 3: // merger

                    break;

                default:
                    this.workflow_Button.Visible = false;
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
                    controlHighlight_Panel.Location = new Point(this.page1_Button.Location.X - X_OFFSET, this.page1_Button.Location.Y + Y_OFFSET);
                    break;
                case 2:
                    this.page1_Button.Enabled = true;
                    this.page2_Button.Enabled = true;
                    this.page3_Button.Enabled = false;
                    this.page4_Button.Enabled = false;
                    this.page5_Button.Enabled = false;
                    controlHighlight_Panel.Location = new Point(this.page2_Button.Location.X - X_OFFSET, this.page2_Button.Location.Y + Y_OFFSET);
                    break;
                case 3:
                    this.page1_Button.Enabled = true;
                    this.page2_Button.Enabled = true;
                    this.page3_Button.Enabled = true;
                    this.page4_Button.Enabled = false;
                    this.page5_Button.Enabled = false;
                    controlHighlight_Panel.Location = new Point(this.page3_Button.Location.X - X_OFFSET, this.page3_Button.Location.Y + Y_OFFSET);
                    break;
                case 4:
                    this.page1_Button.Enabled = true;
                    this.page2_Button.Enabled = true;
                    this.page3_Button.Enabled = true;
                    this.page4_Button.Enabled = true;
                    this.page5_Button.Enabled = false;
                    controlHighlight_Panel.Location = new Point(this.page4_Button.Location.X - X_OFFSET, this.page4_Button.Location.Y + Y_OFFSET);
                    break;
                case 5:
                    this.page1_Button.Enabled = true;
                    this.page2_Button.Enabled = true;
                    this.page3_Button.Enabled = true;
                    this.page4_Button.Enabled = true;
                    this.page5_Button.Enabled = true;
                    controlHighlight_Panel.Location = new Point(this.page5_Button.Location.X - X_OFFSET, this.page5_Button.Location.Y + Y_OFFSET);
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
            //pictureBox_Info.Visible = true;
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
                    label_Info.Text = "Nights Watch Mode does the following";
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

        }

        //
        // Summary:
        //     Checks for updates on the startup of the application. Currently set to grab updates from the scratch location. 
        //      
        // Returns:
        //     None.
        private async Task CheckForUpdates()
        {
            this.Log.AddInfoMsg("Checking for Updates -- STARTED"); 
            // TODO: point the update manager to github
            using (var manager = new UpdateManager(@"\\scratch2\scratch\jogeurte\WDACWizard\Releases\"))
            {
                await manager.UpdateApp();
                this.Log.AddInfoMsg("Checking for Updates -- UPDATING APP");
            }
            this.Log.AddInfoMsg("Checking for Updates -- FINISHED");
        }


        //
        // Summary:
        //     Creates the session folder. Contents such as the log, intermediate policies are stored here.  
        //      
        // Returns:
        //     Returns the folder path for the current app session.
        private string CreateTempFolder()
        {
            //AppData + WDAC Temp folder
            string tempFolderPath = String.Format(@"{0}/WDACWizard/temp/{1}", 
                Environment.GetEnvironmentVariable("LocalAppData"), formatDate());
            
            if(!Directory.Exists(tempFolderPath))
                Directory.CreateDirectory(tempFolderPath);

            return tempFolderPath; 


        }

        /// <summary>
        /// Triggered upon application closing event. 
        /// /// </summary>
        private void FormClosing_Event(object sender, FormClosingEventArgs e)
        {
            this.Log.CloseLogger();// does this belong here? 
            // TODO: add Telemetry
        }
    }

}
