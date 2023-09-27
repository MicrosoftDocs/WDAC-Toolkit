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
        public bool CustomRuleinProgress { get; set; }   // Flag set when user has kicked off the custom rule procedure. Ensures user does not go to build page without accidentally creating the rule
        public bool BinaryFileCreated { get; set; }      // Flag set when the conversion to binary file (.cip/.p7b) was successful. False, otherwise.

        public Logger Log { get; set; }
        public List<string> PageList;
        public WDAC_Policy Policy { get; set; }
        public List<CiEvent> CiEvents { get; set; }

        // Runspace param to access all PS Variables and eliminate overhead opening each time
        private Runspace runspace;
        private int RulesNumber;
        private int nCustomValueRules;

        public string TempFolderPath { get; set; }
        public string ExeFolderPath { get; set; }

        // Edit Workflow datastructs
        private BuildPage _BuildPage;
        private SigningRules_Control _SigningRulesControl;
        public EditWorkflowType EditWorkflow;
        public SiPolicy EventLogPolicy; 

        public enum EditWorkflowType
        {
            Edit = 0,
            EventLog = 1,
        }

        public MainWindow()
        {
            this.TempFolderPath = CreateTempFolder();
            this.Log = new Logger(this.TempFolderPath); 
            
            InitializeComponent();

            // Init MainWindow params
            this.ConfigInProcess = false;
            this.CurrentPage = 0;
            this.RulesNumber = 0;
            this.nCustomValueRules = 0;

            this.Policy = new WDAC_Policy();
            this.CiEvents = new List<CiEvent>(); 
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
        private void Button_New_Click(object sender, EventArgs e)
        {
            if (!this.ConfigInProcess)
            {
                this.Log.AddNewSeparationLine("Workflow -- New Policy Selected");
                this.view = 1; 
                this.CurrentPage = 1;
                this.ConfigInProcess = true;
                this.RedoFlowRequired = false;
                this.Policy.PolicyWorkflow = WDAC_Policy.Workflow.New;
                this.Policy._PolicyType = WDAC_Policy.PolicyType.BasePolicy;

                PageController(sender, e); 
                button_Next.Visible = true; 

            }

            else
            {
                // Working on other workflow - do you want to leave?
                if (WantToAbandonWork())
                {
                    DisplayInfoText(0);
                    this.ConfigInProcess = false;
                    Button_New_Click(sender, e);
                }
            }
        }

        
        /// <summary>
        /// Edit policy button selected: User can load a pre-exisiting policy on disk and 
        /// view and reconfigure its rules and settings. 
        /// </summary>
        private void Button_Edit_Click(object sender, EventArgs e)
        {
            // Edit Policy Button:
           
            if (!this.ConfigInProcess)
            {
                this.Log.AddNewSeparationLine("Workflow -- Edit Policy Selected");
                this.view = 2;
                this.CurrentPage = 1; 
                this.ConfigInProcess = true;
                this.Policy.PolicyWorkflow = WDAC_Policy.Workflow.Edit;

                PageController(sender, e);
                button_Next.Visible = true;
            }

            else
            {
                // Working on other workflow - do you want to leave?
                // If so, set the ConfigInProcess flag to false
                if (WantToAbandonWork())
                {
                    DisplayInfoText(0);
                    this.ConfigInProcess = false;
                    Button_Edit_Click(sender, e);
                }
            }
        }


        /// <summary>
        /// Merge policy button selected: User must select two policies on disk to merge into 
        /// one single policy where the new policy is the intersection of the former two. 
        /// </summary>
        private void Button_Merge_Click(object sender, EventArgs e)
        {
            if (!this.ConfigInProcess)
            {
                this.Log.AddNewSeparationLine("Workflow -- Merge Policies Selected");
                this.view = 3;
                this.CurrentPage = 1;
                this.ConfigInProcess = true;
                this.Policy.PolicyWorkflow = WDAC_Policy.Workflow.Merge;

                PageController(sender, e);
                button_Next.Visible = true;
            }

            else
            {
                // Working on other workflow - do you want to leave?
                // If so, set the ConfigInProcess flag to false
                if (WantToAbandonWork())
                {
                    DisplayInfoText(0);
                    this.ConfigInProcess = false;
                    Button_Merge_Click(sender, e);
                }
            }
        }

        // #####################
        // CONTROL PANEL CONTROLS
        // #####################

        /// <summary>
        /// Home button on the left hand navigation panel. Resets the state of the application and brings user to the MainWindow form.  
        /// </summary>
        private void Home_Button_Click(object sender, EventArgs e)
        {
            this.button_Next.Visible = false;

            // If the CustomRules Panel is open, close it
            if (this.CustomRuleinProgress && this._SigningRulesControl != null)
            {
                DialogResult res = MessageBox.Show("Do you want to return home and abandon this custom rule?",
                        "Confirmation",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    // Close the CustomRuleConditionsPanel
                    this._SigningRulesControl.CloseCustomRulesPanel(); 
                }
                else
                {
                    // Do not return home
                    return; 
                }
            }

            // Re-initialize SiPolicy object
            this.Policy = new WDAC_Policy();

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
        private void Settings_Button_Click(object sender, EventArgs e)
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
        private void Button_Next_Click(object sender, EventArgs e)
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
                PageController(sender, e);
            }
                
            else
                DisplayInfoText(99); 
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            PageController(sender, e);
        }

        /// <summary>
        /// Controller mechanism to determine which UserControls to place ontop of the MainWindow WinForm.
        /// Method called by the Next and Back button.
        /// </summary>
        public void PageController(object sender, EventArgs e)
        {
            DisplayInfoText(0);
            
            //RemoveControls(); 

            // Get pertitent workflow
            switch (this.CurrentPage){
            // Home page
            case 0:
                break;

            // 1st page
            case 1:
                    // New Workflow
                    if (this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.New)
                    {
                        // New Base Policy Workflow
                        if(this.Policy._PolicyType == WDAC_Policy.PolicyType.BasePolicy)
                        {
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
                        }

                        else if(this.Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy)
                        {
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
                        }
                    }

                    // Edit Workflow
                    else if(this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.Edit)
                    {
                        // view & edit mode
                        // Show policy rules UI
                        string pageKey  = "EditWorkflowPage";
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
                    }

                    // Merge Workflow
                    else if(this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.Merge)
                    {
                        string pageKey  = "MergePage";
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
                    }

            break; // end of 1st page

            case 2: // 2nd Page

                    // New Workflow
                    if (this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.New)
                    {
                        // New Base Policy Workflow
                        if (this.Policy._PolicyType == WDAC_Policy.PolicyType.BasePolicy)
                        {
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
                        }

                        else if (this.Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy)
                        {
                            // New SUPPLEMENTAL policy -> policy rules. Do not present a template page
                            string pageKey  = "SupConfigTemplatePage";
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
                        }
                    }
                    // Edit Workflow
                    else if (this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.Edit)
                    {
                        // Edit Mode
                        string pageKey = "EditPolicyRulesPage";
                        if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, launch instance
                        {
                            Control[] _Pages = this.Controls.Find(pageKey, true);
                            _Pages[0].Show();
                            _Pages[0].BringToFront();
                            _Pages[0].Focus();
                        }
                        else
                        {
                            // CHECKS HERE IF EDIT FLOW OR AUDIT FLOW
                            if (this.EditWorkflow == EditWorkflowType.Edit)
                            {
                                var _RulesPage = new ConfigTemplate_Control(this);
                                _RulesPage.Name = pageKey;
                                this.PageList.Add(_RulesPage.Name);
                                this.Controls.Add(_RulesPage);
                                _RulesPage.BringToFront();
                                _RulesPage.Focus();
                            }
                            else
                            {
                                var _RulesPage = new EventLogRuleConfiguration(this);
                                _RulesPage.Name = pageKey;
                                this.PageList.Add(_RulesPage.Name);
                                this.Controls.Add(_RulesPage);
                                _RulesPage.BringToFront();
                                _RulesPage.Focus();
                            }
                        }

                        ShowControlPanel(sender, e);
                    }

                    // Merge Workflow
                    else if (this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.Merge)
                    {
                        button_Next.Visible = false;

                        string pageKey  = "BuildPage";
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
                    }

            break; // end of 2nd page
                        

            // 3rd Page
            case 3:
                    // New policy workflow
                    if (this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.New)
                    {
                        // New Base
                        if(this.Policy._PolicyType == WDAC_Policy.PolicyType.BasePolicy)
                        {
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
                        }

                        // New Supplemental
                        if(this.Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy)
                        {
                            string pageKey = "SupSigningRulesPage";
                            if (this.PageList.Contains(pageKey) && !this.RedoFlowRequired) //already been here, show instance
                            {
                                Control[] _Pages = this.Controls.Find(pageKey, true);
                                _Pages[0].Show();
                                _Pages[0].BringToFront();
                                _Pages[0].Focus();
                            }
                            else
                            {
                                this._SigningRulesControl = new SigningRules_Control(this);
                                this._SigningRulesControl.Name = pageKey;
                                this.PageList.Add(this._SigningRulesControl.Name);
                                this.Controls.Add(this._SigningRulesControl);
                                this._SigningRulesControl.BringToFront();
                                this._SigningRulesControl.Focus();
                            }

                            ShowControlPanel(sender, e);
                        }
                    }

                    // Edit policy workflow
                    else if(this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.Edit)
                    {
                        // Edit & view mode
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
                            // CHECKS HERE IF EDIT FLOW OR AUDIT FLOW
                            if (this.EditWorkflow == EditWorkflowType.Edit)
                            {
                                this._SigningRulesControl = new SigningRules_Control(this);
                                this._SigningRulesControl.Name = pageKey;
                                this.PageList.Add(this._SigningRulesControl.Name);
                                this.Controls.Add(this._SigningRulesControl);
                                this._SigningRulesControl.BringToFront();
                                this._SigningRulesControl.Focus();
                            }
                            else
                            {
                                // Go to build page and provide the SiPolicy object
                                pageKey = "Event Logs Build Page";
                                this._BuildPage = new BuildPage(this);
                                this._BuildPage.Name = pageKey;
                                this.PageList.Add(this._BuildPage.Name);
                                this.Controls.Add(this._BuildPage);
                                this._BuildPage.BringToFront();
                                this._BuildPage.Focus();
                                button_Next.Visible = false;
                                ProcessPolicy();
                            }
                        }

                        ShowControlPanel(sender, e);
                    }

                break;

            // 4th Page
            case 4:
                    // New policy workflow
                    if (this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.New)
                    {
                        // New Base
                        if (this.Policy._PolicyType == WDAC_Policy.PolicyType.BasePolicy)
                        {
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
                                this._SigningRulesControl = new SigningRules_Control(this);
                                this._SigningRulesControl.Name = pageKey;
                                this.PageList.Add(this._SigningRulesControl.Name);
                                this.Controls.Add(this._SigningRulesControl);
                                this._SigningRulesControl.BringToFront();
                                this._SigningRulesControl.Focus();
                            }

                            ShowControlPanel(sender, e);
                        }

                        // New Supplemental
                        if (this.Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy)
                        {
                            // New SUPPLEMENTAL policy -- build out policy

                            string pageKey  = "BuildPage";
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
                        }
                    }

                    // Edit policy workflow
                    else if (this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.Edit)
                    {
                        // Edit & view mode

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
                    }

                break;

            // 5th Page
            case 5:
                if(this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.New 
                    && this.Policy._PolicyType == WDAC_Policy.PolicyType.BasePolicy)
                {
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
                }
            break;

            } // End of Switch
        }

        /// <summary>
        /// Begins executing the worker thread and creating the policy created by users. 
        /// Creates the temp directory path to write all of the temp data.  
        /// </summary>
        private void ProcessPolicy()
        {
            // Short circuit policy building if using Event Log workflow
            if(this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.Edit && this.EditWorkflow == EditWorkflowType.EventLog)
            {
                // Save the path under the AppDataLocal\Temp\WDACWizard folder 
                string fileName = String.Format("EventLogPolicy_{0}.xml", Helper.GetFormattedDateTime());
                string pathToWrite = Path.Combine(this.TempFolderPath, fileName);

                try
                {
                    Helper.SerializePolicytoXML(this.EventLogPolicy, pathToWrite);
                }
                catch(Exception e)
                {
                    this.Log.AddErrorMsg("Event Log SiPolicy serialization failed with error: ", e);
                }

                // Close log file 
                this.Log.CloseLogger(); 

                // Build Page:
                this._BuildPage.UpdateProgressBar(100, "Finished completing event log conversion to WDAC Policy");
                this._BuildPage.ShowFinishMsg(pathToWrite);
                return;
            }

            // Create folder for temp intermediate policies
            try
            {
                System.IO.Directory.CreateDirectory(this.TempFolderPath); // Create new temp folder
            }
            catch (Exception e)
            {
                this.Log.AddErrorMsg("Process Policy() caught the following exception ", e);
            }
           
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
        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            string MERGEPATH = System.IO.Path.Combine(this.TempFolderPath, "Merged_CustomRules_Policy.xml");

            // Flag to skip policy conversion in the case of signed policy without PolicySigners section
            bool skipPolicyConversion = false; 
            
            if(this.Policy.PolicyWorkflow != WDAC_Policy.Workflow.Merge)
            {
                // Handle custom value rules: 
                ProcessCustomValueRules(worker);

                // Handle user created rules:
                List<string> customRulesPathList = ProcessSignerRules(worker);
                
                // Merge policies - all custom ones and the template and/or the base (if this is a supplemental)
                MergeCustomRulesPolicy(customRulesPathList, MERGEPATH, worker);
            }
                      
            // Merge all of the unique CI policies with template and/or base policy:
            MergeTemplatesPolicy(MERGEPATH, worker);

            if (this.Policy.PolicyWorkflow != WDAC_Policy.Workflow.Merge)
            {
                // Handle Policy Rule-Options
                SetPolicyRuleOptions(worker, ref skipPolicyConversion);

                // Handle COM rules
                CreateComObjectRules(worker);
            }

            // Set additional parameters,
            // for instance, policy name, GUIDs, version, etc
            SetAdditionalParameters(worker);

            // Convert the policy from XML to Binary file
            if (Properties.Settings.Default.convertPolicyToBinary
                && !skipPolicyConversion)
            {
                this.BinaryFileCreated = ConvertPolicyToBinary();
            }

            worker.ReportProgress(100);
        }

        /// <summary>
        /// Event handler where the progress of the worker is updated when worker.ReportProgress is 
        /// called. Handle UI updates to BuildPage class through public method 'UpdateProgressBar'.
        /// </summary>
        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string process = "";
            int progressPercent = e.ProgressPercentage;
            if (progressPercent <= 10)
                process = "Building policy rules ...";
            else if (progressPercent <= 70)
                process = "Configuring WDAC policy signer and file rules ...";
            else if (progressPercent <= 80)
                process = "Building custom policy file rules ...";
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
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // If error in workflow, log final exception
            if (e.Error != null)
            {
                this.Log.AddErrorMsg("ProcessPolicy() caught the following exception ", e.Error);
            }
            this.Log.AddNewSeparationLine("Workflow -- DONE");

            // Close the log file.At the end of the workflow
            this.Log.CloseLogger(); 

            // Check for error and show the error UI on the build page
            if (e.Error != null)
            {
                this._BuildPage.ShowError();
            }
            else
            {
                this._BuildPage.UpdateProgressBar(100, " ");

                // If we successfully converted the policy XML to binary, show both file paths
                // If conversion was unsuccessful, show only the XML path
                if (this.BinaryFileCreated)
                {
                    this._BuildPage.ShowFinishMsg(this.Policy.SchemaPath, this.Policy.BinPath);
                }
                else
                {
                    this._BuildPage.ShowFinishMsg(this.Policy.SchemaPath); 
                }
            }

            // Re-initialize SiPolicy object
            this.Policy = new WDAC_Policy();
        }

        /// <summary>
        /// Create the policy rule-option pairings the user has set and creates a seperate CI policy just for the pairings. 
        /// Also removes all of the rule-options for the template policy such that there is no merge conflict. 
        /// </summary>
        public void SetPolicyRuleOptions(BackgroundWorker worker, ref bool skipPolicyConversion)
        {
            this.Log.AddInfoMsg("--- Create Policy Rule Options --- ");

            //Dictionary<string, Dictionary<string, string>>.KeyCollection keys = this.Policy.ConfigRules.Keys;
            SiPolicy finalPolicy = new SiPolicy();

            // Make a copy of the Template policy and REMOVE ALL of its rule options
            // That way we ensure we take the set rule-options from ConfigRules
            try
            {
                finalPolicy = Helper.DeserializeXMLtoPolicy(this.Policy.SchemaPath);
                if(finalPolicy == null)
                {
                    return;
                }
            }
            catch (Exception e)
            {
                this.Log.AddErrorMsg("Create Policy Rule Options -- parsing OutputSchema.xml encountered the following error ", e); 
            }

            // Policy Rule Options derived from the final state of the rules page
            List<RuleType> ruleOptionsList = this.Policy.PolicyRuleOptions;

            // Assert supplemental policies and legacy policies cannot have the Supplemental (rule #17) option
            if (Policy.HasRuleOption(OptionType.EnabledAllowSupplementalPolicies)
                && (this.Policy._Format == WDAC_Policy.Format.Legacy || this.Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy))
            {
                for(int i=0; i < ruleOptionsList.Count; i++)
                {
                    if(ruleOptionsList[i].Item == OptionType.EnabledAllowSupplementalPolicies)
                    {
                        this.Log.AddInfoMsg("Removing EnabledAllowSupplementalPolicies (rule-option 17)"); 
                        ruleOptionsList.RemoveAt(i);
                        break;
                    }
                }
            }

            // Updated logic for Issue #262 - instead of forcing rule-option 6, skip binary conversion
            // if the policy has the signed CI policy option (rule #6) and lacks policy signers
            if (Properties.Settings.Default.convertPolicyToBinary 
                && !Policy.HasRuleOption(OptionType.EnabledUnsignedSystemIntegrityPolicy))
            {
                if(finalPolicy.UpdatePolicySigners == null 
                    || finalPolicy.UpdatePolicySigners.Length < 1)
                {
                    skipPolicyConversion = true; 
                }

                // Handle Supplemental Signers issue - must have supplemental signers if signed policy && policy
                // allows supplemental policies
                if(Policy.HasRuleOption(OptionType.EnabledAllowSupplementalPolicies))
                {
                    if (finalPolicy.SupplementalPolicySigners == null
                    || finalPolicy.SupplementalPolicySigners.Length < 1)
                    {
                        skipPolicyConversion = true;
                    }
                }
            }

            // De-duplicate the Rule Options list
            List<RuleType> dedupedRuleOptions = Helper.DeDuplicateRuleOptions(ruleOptionsList);

            // Convert from List<RuleType> to RuleType[]
            RuleType[] ruleOptions = new RuleType[dedupedRuleOptions.Count];
            for(int i = 0; i< ruleOptions.Length; i++)
            {
                ruleOptions[i] = dedupedRuleOptions[i];
                this.Log.AddInfoMsg("Adding " + dedupedRuleOptions[i].Item); 
            }

            try
            {
                finalPolicy.Rules = ruleOptions;
                Helper.SerializePolicytoXML(finalPolicy, this.Policy.SchemaPath);
            }
            catch (Exception e)
            {
                this.Log.AddErrorMsg("Create Policy Rule Options -- serializing finalPolicy object back to OutputSchema.xml encountered the following error ", e);
            }

            worker.ReportProgress(95);
        }

        public void CreateComObjectRules(BackgroundWorker worker)
        {
            // Iterate through all the custom rules for the COM object rules only
            List<COM> comObjectRules = new List<COM>();
            foreach(var rule in this.Policy.CustomRules)
            {
                if(rule.COMObject.Guid != null)
                {
                    comObjectRules.Add(rule.COMObject);
                    this.Log.AddInfoMsg(String.Format("Creating COM {0} Rule for Provider={1} and Key={2}",
                                                       rule.COMObject.ValueItem.ToString(),
                                                       rule.COMObject.Provider.ToString(),
                                                       rule.COMObject.Guid));
                }
            }

            // Return if no COM rules were created
            if(comObjectRules.Count == 0)
            {
                worker.ReportProgress(99); 
                return; 
            }

            // Manipulate the final policy on disk as oppposed to any temp policies
            SiPolicy policy = Helper.DeserializeXMLtoPolicy(this.Policy.SchemaPath);
            policy = Helper.CreateComRule(policy, comObjectRules);

            try
            {
                Helper.SerializePolicytoXML(policy, this.Policy.SchemaPath);
            }
            catch (Exception e)
            {
                this.Log.AddErrorMsg(String.Format("Exception encountered in CreateComObjectRules(): {0}", e));
            }

        }

        /// <summary>
        /// Sets the additonal parameters at the end of a policy: GUIDs, versions, etc
        /// </summary>
        void SetAdditionalParameters(BackgroundWorker worker)
        {
            // Operations:
            // 1. Set Policy GUIDs (PolicyID, BasePolicyID)
            // 2. Settings PolicyInfo.Name and PolicyInfo.Id
            // 3. VersionExpolicy 
            // 4. HVCI state
            // 5. Format IDs in the case of Issue #247

            this.Log.AddInfoMsg("-- Set Additional Parameters --");
            SiPolicy siPolicy = Helper.DeserializeXMLtoPolicy(this.Policy.SchemaPath); 

            if(siPolicy == null)
            {
                return;
            }

            // The only time we should be reseting the GUID is NEW multi-policy Base or Supplemental Policy
            if (this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.New)
            {
                // Set policy info - ID, Name
                // Serialize the policy back to XML so settings persist after GUID reset
                siPolicy.Settings = Helper.SetPolicyInfo(siPolicy.Settings, this.Policy.PolicyName, this.Policy.PolicyID);
                Helper.SerializePolicytoXML(siPolicy, this.Policy.SchemaPath); 

                if (this.Policy._PolicyType == WDAC_Policy.PolicyType.BasePolicy 
                    && this.Policy._Format == WDAC_Policy.Format.MultiPolicy)
                {
                    // Run Set-CIPolicyIdInfo -ResetPolicyID to force the policy into multiple policy format
                    ResetGuidPs(this.Policy.SchemaPath);

                    // Deserialize again since Wizard reset the policy on disk to multi-policy
                    siPolicy = Helper.DeserializeXMLtoPolicy(this.Policy.SchemaPath);

                    // Reset the GUIDs
                    siPolicy.BasePolicyID = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
                    siPolicy.PolicyID = siPolicy.BasePolicyID;

                    // Force SiPolicy PolicyType to BasePolicy
                    siPolicy.PolicyType = global::PolicyType.BasePolicy; 
                    siPolicy.PolicyTypeSpecified = true;

                    // Remove PolicyTypeID, if applicable
                    siPolicy.PolicyTypeID = null;
                }

                // If single policy/legacy policy, ensure the correct IDs are set on the policy
                else if (this.Policy._Format == WDAC_Policy.Format.Legacy)
                {
                    siPolicy.BasePolicyID = null;
                    siPolicy.PolicyID = null;
                    siPolicy.PolicyTypeSpecified = false; 
                    siPolicy.PolicyTypeID = Properties.Resources.LegacyPolicyID_GUID; 
                }

                // If supplemental policy, set the Base policy guid; reset the policy ID and set the policy name
                else if (this.Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy)
                {
                    // User provided path to base policy XML
                    if(this.Policy.BasePolicyId == Guid.Empty)
                    {
                        // Set BasePolicyID to match the Id of the linked base policy
                        siPolicy.BasePolicyID = Helper.DeserializeXMLtoPolicy(this.Policy.BaseToSupplementPath).BasePolicyID;
                    }

                    // User provided the GUID of the base policy only
                    else
                    {
                        string baseId = this.Policy.BasePolicyId.ToString().ToUpper();

                        // Set BasePolicyID to match the Id provided base policy ID
                        if (baseId.Contains("{"))
                        {
                            siPolicy.BasePolicyID = baseId; 
                        }
                        else
                        {
                            siPolicy.BasePolicyID = "{" + baseId + "}";
                        }
                    }

                    // Reset the PolicyID guid so it is unique
                    siPolicy.PolicyID = "{" + Guid.NewGuid().ToString().ToUpper() + "}";

                    // Force SiPolicy PolicyType to Supplemental
                    siPolicy.PolicyType = global::PolicyType.SupplementalPolicy;
                    siPolicy.PolicyTypeSpecified = true;

                    // Remove PolicyTypeID, if applicable
                    siPolicy.PolicyTypeID = null;
                }

                // Log information set for debugging
                this.Log.AddInfoMsg("Additional parameters set - BasePolicyID: " + siPolicy.BasePolicyID);
                this.Log.AddInfoMsg("Additional parameters set - PolicyID: " + siPolicy.PolicyID);

                // Log Policy settings set by the Wizard
                foreach(var setting in siPolicy.Settings)
                {
                    this.Log.AddInfoMsg(String.Format("Additional parameters set - Provider:{0} - Key:{1} - ValueName: {2} - Value:{3} ", 
                        setting.Provider, setting.Key, setting.ValueName, setting.Value.Item));
                }
            }

            // Update the version number on the edited policies. If not specified, version defaults to 10.0.0.0
            if (this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.Edit)
            {
                // Set policy info - ID, Name
                siPolicy.Settings = Helper.SetPolicyInfo(siPolicy.Settings, this.Policy.PolicyName, this.Policy.PolicyID); 
                siPolicy.VersionEx = this.Policy.VersionNumber;

                this.Log.AddInfoMsg("Additional parameters set - Info.PolicyName to " + siPolicy.Settings[0].Value.Item);
                this.Log.AddInfoMsg("Additional parameters set - Info.PolicyID to " + siPolicy.Settings[1].Value.Item);
                this.Log.AddInfoMsg("Additional parameters set - VersionEx " + siPolicy.VersionEx);
            }

            // Set HVCI state
            if (this.Policy.EnableHVCI)
            {
                // Enable HVCI since HVCI is not a Rule-Option
                siPolicy.HvciOptions = 1;
                this.Log.AddInfoMsg("Additional parameters set - HVCI set to 1");
            }
            else
            {
                siPolicy.HvciOptions = 0;
                this.Log.AddInfoMsg("Additional parameters set - HVCI set to 0");
            }

            // Lastly, re-format Allow/Deny, FileAttrib and Signer IDs, if applicable
            siPolicy = Helper.FormatFileRuleIDs(siPolicy);
            siPolicy = Helper.FormatSignerRuleIDs(siPolicy);

            try
            {
                Helper.SerializePolicytoXML(siPolicy, this.Policy.SchemaPath); 
            }
            catch (Exception e)
            {
                this.Log.AddErrorMsg(String.Format("Exception encountered in SetAdditionalParameters(): {0}", e));
            }
            
            runspace.Dispose();
        }

        /// <summary>
        /// Processes all of the non-custom signer rules defined by user. 
        /// </summary>
        public List<string> ProcessSignerRules(BackgroundWorker worker)
        {
            List<string> customRulesPathList = new List<string>();
            int nCustomRules = this.Policy.CustomRules.Count;
            int progressVal = 0;

            // Iterate through all of the custom rules and update the progress bar    
            for (int i = 0; i < nCustomRules; i++)
            {
                progressVal = 25 + i * 60 / nCustomRules;
                worker.ReportProgress(progressVal); //Assumes the operations involved with this step take about 70% -- probably should be a little higher

                var customRule = this.Policy.CustomRules[i];

                // Skip; already handled ALL custom value rules
                if(customRule.UsingCustomValues)
                {
                    continue;
                }

                // Skip the following rules that are handled by custom rules method -
                // File Attributes, PFN rules, file/folder path rules
                if (!(customRule.Type == PolicyCustomRules.RuleType.Publisher 
                      || customRule.Type == PolicyCustomRules.RuleType.Hash
                      || customRule.Type == PolicyCustomRules.RuleType.FolderScan))
                {
                    continue;
                }

                string tmpPolicyPath = Helper.GetUniquePolicyPath(this.TempFolderPath);

                // Create a single policy per rule using the Powershell cmdlets with Level=PCACertificate or Publisher
                // and add the additional attributes like OriginalFilename and version by serializing the
                // policy to reduce the complexity of the PS scripting
                if (customRule.Type == PolicyCustomRules.RuleType.Publisher)
                {
                    SiPolicy signerSiPolicy = Helper.CreateSignerPolicyFromPS(customRule, tmpPolicyPath);

                    if (signerSiPolicy != null)
                    {
                        signerSiPolicy = Helper.AddSignerRuleAttributes(customRule, signerSiPolicy);
                        Helper.SerializePolicytoXML(signerSiPolicy, tmpPolicyPath);
                        customRulesPathList.Add(tmpPolicyPath); // Successfully ran the PS commands; add path to list to merge at the end
                    }
                }
                
                // Hash Rules -- Invoke Powershell cmd to generate 
                if(customRule.Type == PolicyCustomRules.RuleType.Hash)
                {
                    SiPolicy signerSiPolicy = CreateHashPolicyFromPS(customRule, tmpPolicyPath);
                    if (signerSiPolicy != null)
                    {
                        Helper.SerializePolicytoXML(signerSiPolicy, tmpPolicyPath);
                        customRulesPathList.Add(tmpPolicyPath); // Successfully ran the PS commands; add path to list to merge at the end
                    }
                }

                // Folder Scan -- Invoke the New-CiPolicy PS cmd to generate a CI policy
                if(customRule.Type == PolicyCustomRules.RuleType.FolderScan)
                {
                    SiPolicy signerSiPolicy; 
                    if (this.Policy._PolicyType == WDAC_Policy.PolicyType.BasePolicy)
                    {
                        signerSiPolicy = CreateScannedPolicyFromPS(customRule, tmpPolicyPath);
                    }
                    else
                    {
                        signerSiPolicy = CreateScannedPolicyFromPS(customRule, tmpPolicyPath, this.Policy.BaseToSupplementPath);
                    }
                    
                    if (signerSiPolicy != null)
                    {
                        Helper.SerializePolicytoXML(signerSiPolicy, tmpPolicyPath);
                        customRulesPathList.Add(tmpPolicyPath); // Successfully ran the PS commands; add path to list to merge at the end
                    }
                }
            }

            //TODO: results check ensuring 
            runspace.Dispose();
            return customRulesPathList;
        }

        /// <summary>
        /// Processes all of the custom rules with user input fields
        /// </summary>
        /// <param name="worker"></param>
        public void ProcessCustomValueRules(BackgroundWorker worker)
        {
            SiPolicy siPolicyCustomValueRules = Helper.DeserializeXMLStringtoPolicy(Resources.EmptyWDAC);

            // Iterate through all of the custom rules and PFN rules and update the progress bar    
            foreach (var customRule in this.Policy.CustomRules)
            {
                if(customRule.UsingCustomValues)
                {
                    siPolicyCustomValueRules = HandleCustomValues(customRule, siPolicyCustomValueRules);
                    this.nCustomValueRules++;
                }
                else if(customRule.Type == PolicyCustomRules.RuleType.PackagedApp)
                {
                    siPolicyCustomValueRules = Helper.CreatePFNRule(customRule, siPolicyCustomValueRules);
                    this.nCustomValueRules++;
                }
                else if(customRule.Type == PolicyCustomRules.RuleType.FileAttributes)
                {
                    siPolicyCustomValueRules = Helper.CreateNonCustomFileAttributeRule(customRule, siPolicyCustomValueRules);
                    this.nCustomValueRules++;
                }
                else if(customRule.Type == PolicyCustomRules.RuleType.FilePath || customRule.Type == PolicyCustomRules.RuleType.FolderPath)
                {
                    siPolicyCustomValueRules = Helper.CreateNonCustomFilePathRule(customRule, siPolicyCustomValueRules);
                    this.nCustomValueRules++;
                }
            }

            // If we created custom value rules, write to temp folder to merge with other policies
            if(this.nCustomValueRules > 0)
            {
                Helper.SerializePolicytoXML(siPolicyCustomValueRules, System.IO.Path.Combine(this.TempFolderPath, "CustomValueRules.xml"));
            }

            worker.ReportProgress(25);
        }

        /// <summary>
        /// Processes all of the custom rules with custom values. E.g. custom version ranges, custom filenames, file paths
        /// </summary>
        public SiPolicy HandleCustomValues(PolicyCustomRules customRule, SiPolicy siPolicy)
        {
            if(customRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                siPolicy = Helper.CreateFilePublisherRule(customRule, siPolicy);
            }

            else if (customRule.Type == PolicyCustomRules.RuleType.FileAttributes)
            {
                if (customRule.Permission == PolicyCustomRules.RulePermission.Allow)
                {
                    siPolicy = Helper.CreateAllowFileAttributeRule(customRule, siPolicy);
                }
                else
                {
                    siPolicy = Helper.CreateDenyFileAttributeRule(customRule, siPolicy);
                }
            }

            else if(customRule.Type == PolicyCustomRules.RuleType.PackagedApp)
            {
                siPolicy = Helper.CreatePFNRule(customRule, siPolicy);
            }

            else if (customRule.Type == PolicyCustomRules.RuleType.FilePath || customRule.Type == PolicyCustomRules.RuleType.FolderPath)
            {
                if (customRule.Permission == PolicyCustomRules.RulePermission.Allow)
                {
                    siPolicy = Helper.CreateAllowPathRule(customRule, siPolicy);
                }
                else
                {
                    siPolicy = Helper.CreateDenyPathRule(customRule, siPolicy);
                }
            }

            else if (customRule.Type == PolicyCustomRules.RuleType.Hash)
            {
                if (customRule.Permission == PolicyCustomRules.RulePermission.Allow)
                {
                    siPolicy = Helper.CreateAllowHashRule(siPolicy, customRule);
                }
                else
                {
                    siPolicy = Helper.CreateDenyHashRule(siPolicy, customRule);
                }
            }

            return siPolicy;
        }

        /// <summary>
        /// Tries to add attributes like filename, publisher and version to non-custom publisher rules
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="signerSiPolicy"></param>
        public SiPolicy CreateHashPolicyFromPS(PolicyCustomRules customRule, string policyPath)
        {
            // Create runspace, pipeline and run script
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();

            // Scan the file to extract the hashes for rules
            string newPolicyRuleCmd = String.Format("$DummyHashRule = New-CIPolicyRule -Level Hash -DriverFilePath \"{0}\"", customRule.ReferenceFile);
            if (customRule.Permission == PolicyCustomRules.RulePermission.Deny)
            {
                newPolicyRuleCmd += " -Deny";
            }

            pipeline.Commands.AddScript(newPolicyRuleCmd);
            pipeline.Commands.AddScript(String.Format("New-CIPolicy -Rules $DummyHashRule -FilePath \"{0}\"", policyPath));

            foreach(var cmd in pipeline.Commands)
            {
                this.Log.AddInfoMsg("Running the following commands: " + cmd);
            }

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception e)
            {
                this.Log.AddErrorMsg("CreateHashPolicyFromPS() encountered the following exception ", e);
                return null;
            }

            runspace.Dispose();

            // De-serialize the dummy policy to get the signer objects
            SiPolicy siPolicy = Helper.DeserializeXMLtoPolicy(policyPath);
            return siPolicy;
        }

        /// <summary>
        /// Tries to add attributes like filename, publisher and version to non-custom publisher rules
        /// </summary>
        /// <param name="customRule"></param>
        /// <param name="policyPath"></param>
        /// <param name="basePolicyToSupplementPath">Path to the base policy that the supplemental policy extends</param>
        /// <returns></returns>
        public SiPolicy CreateScannedPolicyFromPS(PolicyCustomRules customRule, string policyPath, string basePolicyToSupplementPath= null)
        {
            // Create runspace, pipeline and run script
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();

            // Scan the file to extract the hashes for rules
            string newPolicyRuleCmd = String.Format("New-CIPolicy -ScanPath \"{0}\" -Level \"{1}\" -FilePath \"{2}\"", 
                                                    customRule.ReferenceFile, customRule.Scan.Levels[0], policyPath);
            // Add fallback levels, if applicable
            if(customRule.Scan.Levels.Count > 1)
            {
                newPolicyRuleCmd += " -Fallback ";
                for(int i=1; i <customRule.Scan.Levels.Count; i++)
                {
                    newPolicyRuleCmd += customRule.Scan.Levels[i] + ", "; 
                }
                newPolicyRuleCmd = newPolicyRuleCmd.Substring(0, newPolicyRuleCmd.Length - 2); // trim trailing comma + whitespace
            }
            else
            {
                // By default, fall back to hash
                newPolicyRuleCmd += " -Fallback Hash";
            }

            // Add omit paths, if applicable
            if (customRule.Scan.OmitPaths.Count > 0)
            {
                newPolicyRuleCmd += " -OmitPaths ";
                for (int i = 0; i < customRule.Scan.OmitPaths.Count; i++)
                {
                    newPolicyRuleCmd += "\"" + customRule.Scan.OmitPaths[i] + "\", ";
                }
                newPolicyRuleCmd = newPolicyRuleCmd.Substring(0, newPolicyRuleCmd.Length - 2); // trim trailing comma + whitespace
            }

            // Handle User mode PEs, if applicable
            if(customRule.SigningScenarioCheckStates.umciEnabled)
            {
                newPolicyRuleCmd += " -UserPEs";
            }

            // Handle Deny rules, if applicable
            if (customRule.Permission == PolicyCustomRules.RulePermission.Deny)
            {
                newPolicyRuleCmd += " -Deny";
            }

            pipeline.Commands.AddScript(newPolicyRuleCmd);
            this.Log.AddInfoMsg("Running the following commands: " + newPolicyRuleCmd);

            // Set the supplemental-specific fields in the policy, if applicable
            if (!string.IsNullOrEmpty(basePolicyToSupplementPath))
            {
                string supplementalPolicyIdCmd = String.Format("Set-CIPolicyIdInfo -FilePath \"{0}\" -BasePolicyToSupplementPath \"{1}\"",
                                                                policyPath, basePolicyToSupplementPath);

                pipeline.Commands.AddScript(supplementalPolicyIdCmd);
                this.Log.AddInfoMsg("Running the following commands: " + supplementalPolicyIdCmd);
            }

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception e)
            {
                this.Log.AddErrorMsg("CreateScannedPolicyFromPS() encountered the following exception ", e);
                return null;
            }

            runspace.Dispose();

            // De-serialize the dummy policy to get the signer objects
            SiPolicy siPolicy = Helper.DeserializeXMLtoPolicy(policyPath);

            // Remove all the default policy rules
            // Do not set to null which causes invalid file at compilation time - Issue #218
            
            siPolicy.Rules = new RuleType[1];

            return siPolicy;
        }

        /// <summary>
        /// Runs the PS Set-CIPolicyIdInfo -Reset command to force the policy into multiple policy format
        /// </summary>
        /// <param name="path"></param>
        public void ResetGuidPs(string path)
        {
            // Create runspace, pipeline and runscript
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();

            string resetCmd = String.Format("Set-CIPolicyIdInfo -ResetPolicyID \"{0}\"", path);

            pipeline.Commands.AddScript(resetCmd);
            this.Log.AddInfoMsg(String.Format("Running the following commands: {0}", resetCmd));

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
            }
            catch (Exception e)
            {
                this.Log.AddErrorMsg(String.Format("Exception encountered in ResetGuidPs(): {0}", e));
            }
        }

        /// <summary>
        /// Merges all of the CI Policies created for each custom rule. Writes the merged policy into one OutputFilePath (input)
        /// </summary>
        public void MergeCustomRulesPolicy(List<string> customRulesPathList, string outputFilePath, BackgroundWorker worker)
        {
            this.Log.AddInfoMsg("--- Merge Custom Rules Policy ---");

            string mergeScript = String.Empty;

            // Create runspace, pipeline and runscript
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();
            Pipeline pipeline = runspace.CreatePipeline();


            if (customRulesPathList.Count > 0 || this.nCustomValueRules > 0)
            {
                // Add all the merge paths
                // First policy in the merge list of policies will determeine the output policy format
                // Since we set the format in ProcessCustomRules(), the customRulesPathList will be the correct format
                mergeScript = "Merge-CIPolicy -PolicyPaths ";
                foreach (string path in customRulesPathList)
                {
                    // If the xml was successfully generated in the previous step
                    if (File.Exists(path))
                    {
                        mergeScript += String.Format("\"{0}\",", path);
                    }
                    else
                    {
                        this.Log.AddErrorMsg("Wizard could not find " + path + ". Skipping merge step in MergeCustomRulesPolicy()");
                    }
                }

                // If there are custom value rules, merge in siPolicy from /Temp/Custom
                if(this.nCustomValueRules > 0)
                {
                    mergeScript += String.Format("\"{0}\",", System.IO.Path.Combine(this.TempFolderPath, "CustomValueRules.xml"));
                }

                // Remove last comma and add outputFilePath
                mergeScript = mergeScript.Remove(mergeScript.Length - 1);
                mergeScript += String.Format(" -OutputFilePath \"{0}\"", outputFilePath);

                pipeline.Commands.AddScript(mergeScript);
                pipeline.Commands.Add("Out-String");

                this.Log.AddInfoMsg(String.Format("Running the following commands: {0}", mergeScript));

                try
                {
                    Collection<PSObject> results = pipeline.Invoke();
                }
                catch (Exception e)
                {
                    this.Log.AddErrorMsg(String.Format("Exception encountered in MergeCustomRulesPolicy(): {0}", e));
                }
            }
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

            // If Edit mode is selected, since the 'New Save Location' field is optional, the Policy.SchemaPath can either be set or null.  
            // If the field is null, set the save locaiton to the same location as the EditPolicyPath.
            // Otherwise, keep Policy.SchemaPath as is
            
            if (this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.Edit)
            {
                if(String.IsNullOrEmpty(this.Policy.SchemaPath))
                {
                    // Since we don't have a new save location, copy the TemplatePath
                    // and append '_Edit' to the file path.
                    // Check if _v10.0.x.y is already in string ie. editing the output of an editing workflow
                    int versionNumPos = this.Policy.EditPathContainsVersionInfo(); 
                    if (versionNumPos > 0)
                    {
                        string filePathWithoutVer = this.Policy.EditPolicyPath.Substring(0, versionNumPos);
                        this.Policy.SchemaPath = String.Format("{0}_v{1}.xml", filePathWithoutVer, this.Policy.UpdateVersion());
                    }
                    else
                    {
                        string filePathWithoutExt = Path.Combine(Path.GetDirectoryName(this.Policy.EditPolicyPath), 
                                                                 Path.GetFileNameWithoutExtension(this.Policy.EditPolicyPath));
                        this.Policy.SchemaPath = String.Format("{0}_v{1}.xml", filePathWithoutExt, this.Policy.UpdateVersion());
                    }
                }
                else
                {
                    this.Policy.UpdateVersion(); 
                }
            }

            // Check if the chosen path is user-writeable
            // If it is not, default to AppDataLocal\Temp\WDACWizard
            if(!Helper.IsUserWriteable(Path.GetDirectoryName(this.Policy.SchemaPath)))
            {
                this.Policy.SchemaPath = Path.Combine(this.TempFolderPath, Path.GetFileName(this.Policy.SchemaPath)); 
            }

            this.Log.AddInfoMsg("--- Merge Templates Policy ---");
            string DEST_PATH = System.IO.Path.Combine(this.TempFolderPath, "OutputSchema.xml"); //this.TempFolderPath + @"\OutputSchema.xml";

            List<string> policyPaths = new List<string>();
            string mergeScript = "Merge-CIPolicy -PolicyPaths ";

            // First merged policy will define the type of the output. eg) if merging a legcy with multi --> output=legacy, vice-versa
            // Again, the policy at customRulesMergePath will be the correct format and will determine the output format as its first in the command
            // TODO: what if CustomRules is empty ??

            // TemplatePath holds the structure of the policy under edit
            // If editing a policy, e.g. supplemental, have the TemplatePath define the structure

            if (this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.Edit)
            {
                // TemplatePath holds the structure of the policy under edit
                if (this.Policy.TemplatePath != null)
                {
                    policyPaths.Add(this.Policy.TemplatePath);
                }

                if (this.Policy.CustomRules.Count > 0 && File.Exists(customRulesMergePath))
                {
                    policyPaths.Add(customRulesMergePath);
                }
            }
            else
            {
                // Merge issue #87 is here - non zero custom rules
                if (this.Policy.CustomRules.Count > 0 && File.Exists(customRulesMergePath))
                {
                    policyPaths.Add(customRulesMergePath);
                }

                if (this.Policy.TemplatePath != null)
                {
                    policyPaths.Add(this.Policy.TemplatePath);
                }
            }
            
            // Added in the order based on their final order in the table
            if (this.Policy.PoliciesToMerge.Count > 0)
            {
                foreach(var path in this.Policy.PoliciesToMerge)
                {
                    policyPaths.Add(path);
                }
            }

            // Check whether the recommended block list rules are wanted in the output:
            if(this.Policy.UseUserModeBlocks)
            {
                string recommendedUsermodeBlockPath = System.IO.Path.Combine(this.ExeFolderPath, "Recommended_UserMode_Blocklist.xml");
                policyPaths.Add(recommendedUsermodeBlockPath);
            }

            if (this.Policy.UseKernelModeBlocks)
            {
                string recommendedDriverBlockPath = System.IO.Path.Combine(this.ExeFolderPath, "Recommended_Driver_Blocklist.xml"); ; 
                policyPaths.Add(recommendedDriverBlockPath); 
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
                File.Copy(this.Policy.SchemaPath, DEST_PATH, true);
            }
            catch (Exception e)
            {
                this.Log.AddErrorMsg(String.Format("Exception encountered in MergeTemplatesPolicy(): {0}", e));
            }
            runspace.Dispose();

            //TODO: check output
            worker.ReportProgress(90);
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

            // If multi-policy format, use the {PolicyGUID}.cip format as defined in https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/deploy-multiple-windows-defender-application-control-policies#deploying-multiple-policies-locally
            string binaryFileName = string.Empty;
            SiPolicy finalSiPolicy = Helper.DeserializeXMLtoPolicy(this.Policy.SchemaPath);

            if (finalSiPolicy != null)
            {
                if(finalSiPolicy.BasePolicyID != null)
                
                {
                    binaryFileName = String.Format("{0}.cip", finalSiPolicy.PolicyID);
                }
                else
                {
                    binaryFileName = "SiPolicy.p7b";
                }

                this.Policy.BinPath = Path.Combine(Path.GetDirectoryName(this.Policy.SchemaPath), binaryFileName);
                string binConvertCmd = String.Format("ConvertFrom-CIPolicy -XmlFilePath \"{0}\" -BinaryFilePath \"{1}\"", this.Policy.SchemaPath, this.Policy.BinPath);

                pipeline.Commands.AddScript(binConvertCmd);
            }           

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

        // UI helper functions

        /// <summary>
        /// Prompts the user to confirm if they want to abandon progress
        /// </summary>       
        private bool WantToAbandonWork()
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

        /// <summary>
        /// Displays the left-most control panel with home, settings buttons and progress pages. Displays the highlighting panel to show users current page.
        /// </summary> 
        private void ShowControlPanel(object sender, EventArgs e)
        {
            this.Controls.Add(this.control_Panel);
            this.control_Panel.BringToFront();
            this.control_Panel.Focus();

            this.Controls.Add(button_Next);
            button_Next.BringToFront();
            button_Next.Focus(); 

            // Set highlight panel location
            int X_OFFSET = 15;
            int Y_OFFSET = 5;
            try
            {
                var sideButton = (Button)sender; 
                this.controlHighlight_Panel.Location = new System.Drawing.Point(sideButton.Location.X - X_OFFSET, sideButton.Location.Y + Y_OFFSET);
            }
            catch(Exception exc)
            {
                this.controlHighlight_Panel.Location = new System.Drawing.Point(this.home_Button.Location.X - X_OFFSET, this.home_Button.Location.Y + Y_OFFSET);
            }

            // Enable Settings Button -- if on building page, it will be disabled below
            this.settings_Button.Enabled = true; 

            // Set link text
            if(this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.New)
            {
                if(this.Policy._PolicyType == WDAC_Policy.PolicyType.BasePolicy)
                {
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
                    this.page4_Button.Text = "File Rules";
                    this.page5_Button.Text = "Creating Policy";
                }

                else if (this.Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy)
                {
                    this.workflow_Label.Visible = true;
                    this.page1_Button.Visible = true;
                    this.page2_Button.Visible = true;
                    this.page3_Button.Visible = true;
                    this.page4_Button.Visible = true;
                    this.page5_Button.Visible = false;

                    this.workflow_Label.Text = "Policy Creator";
                    this.page1_Button.Text = "Policy Type";
                    this.page2_Button.Text = "Policy Rules";
                    this.page3_Button.Text = "File Rules";
                    this.page4_Button.Text = "Creating Policy";
                }
            }
            // Policy Editor
            else if (this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.Edit)
            {
                // Editing a policy but not creating one from event logs
                if(this.EditWorkflow == EditWorkflowType.Edit)
                {
                    this.workflow_Label.Visible = true;
                    this.page1_Button.Visible = true;
                    this.page2_Button.Visible = true;
                    this.page3_Button.Visible = true;
                    this.page4_Button.Visible = true;
                    this.page5_Button.Visible = false;

                    this.workflow_Label.Text = "Policy Editor";
                    this.page1_Button.Text = "Select Policy";
                    this.page2_Button.Text = "Policy Rules";
                    this.page3_Button.Text = "File Rules";

                    this.page4_Button.Text = "Creating Policy";
                }
                // Creating a policy from event log or advanced hunting
                else if(this.EditWorkflow == EditWorkflowType.EventLog)
                {
                    this.workflow_Label.Visible = true;
                    this.page1_Button.Visible = true;
                    this.page2_Button.Visible = true;
                    this.page3_Button.Visible = true;
                    this.page4_Button.Visible = false;
                    this.page5_Button.Visible = false;

                    this.workflow_Label.Text = "Policy Editor";
                    this.page1_Button.Text = "Select Policy";
                    this.page2_Button.Text = "File Rules";
                    this.page3_Button.Text = "Creating Policy";
                }
            }
            
            // Policy Merger
            else if(this.Policy.PolicyWorkflow == WDAC_Policy.Workflow.Merge)
            {
                this.workflow_Label.Visible = true;
                this.page1_Button.Visible = true;
                this.page2_Button.Visible = true;
                this.page3_Button.Visible = false;
                this.page4_Button.Visible = false;
                this.page5_Button.Visible = false;

                this.workflow_Label.Text = "Policy Merger";
                this.page1_Button.Text = "Select Policies";
                this.page2_Button.Text = "Creating Policy";
            }
            else
            {
                this.workflow_Label.Visible = false;
                this.page1_Button.Visible = false;
                this.page2_Button.Visible = false;
                this.page3_Button.Visible = false;
                this.page4_Button.Visible = false;
                this.page5_Button.Visible = false;
            }

            // Lazy implementation of highlighting current page in workflow
            switch(this.CurrentPage){
            case 1:
                this.page1_Button.Enabled = true;
                this.page2_Button.Enabled = false;
                this.page3_Button.Enabled = false;
                this.page4_Button.Enabled = false;
                this.page5_Button.Enabled = false;
                controlHighlight_Panel.Location = new System.Drawing.Point(this.page1_Button.Location.X - X_OFFSET, this.page1_Button.Location.Y + Y_OFFSET);
                break;

            case 2:
                if(this.view == 3) // Merge
                {
                    // Building page
                    this.page1_Button.Enabled = false;
                    this.page2_Button.Enabled = false;
                    this.page3_Button.Enabled = false;
                    this.page4_Button.Enabled = false;
                    this.page5_Button.Enabled = false;

                    this.settings_Button.Enabled = false; // disable settings button to prevent writing to flushed/closed logger
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

                if(this.view ==2 && this.EditWorkflow == EditWorkflowType.EventLog) // event log or AH building page
                {
                    this.page1_Button.Enabled = false;
                    this.page2_Button.Enabled = false;
                    this.page3_Button.Enabled = false;
                    this.page4_Button.Enabled = false;
                    this.page5_Button.Enabled = false;

                    this.settings_Button.Enabled = false; // disable settings button to prevent writing to flushed/closed logger
                }
                else
                {
                    this.page1_Button.Enabled = true;
                    this.page2_Button.Enabled = true;
                    this.page3_Button.Enabled = true;
                    this.page4_Button.Enabled = false;
                    this.page5_Button.Enabled = false;
                }
                controlHighlight_Panel.Location = new System.Drawing.Point(this.page3_Button.Location.X - X_OFFSET, this.page3_Button.Location.Y + Y_OFFSET);
                break;

            case 4:
                if(this.view == 2) // Edit
                {
                    // Building page
                    this.page1_Button.Enabled = false;
                    this.page2_Button.Enabled = false;
                    this.page3_Button.Enabled = false;
                    this.page4_Button.Enabled = false;
                    this.page5_Button.Enabled = false;

                    this.settings_Button.Enabled = false; // disable settings button to prevent writing to flushed/closed logger
                }
                else
                {
                    if(this.Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy) // build page for supplemental policies
                    {
                        this.page1_Button.Enabled = false;
                        this.page2_Button.Enabled = false;
                        this.page3_Button.Enabled = false;
                        this.page4_Button.Enabled = false;
                        this.page5_Button.Enabled = false;

                        this.settings_Button.Enabled = false; // disable settings button to prevent writing to flushed/closed logger
                    }

                    else
                    {
                        this.page1_Button.Enabled = true;
                        this.page2_Button.Enabled = true;
                        this.page3_Button.Enabled = true;
                        this.page4_Button.Enabled = true;
                        this.page5_Button.Enabled = false;
                    }
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

        /// <summary>
        /// Empties this.Controls based on contents of PageList.
        /// </summary> 
        private void RemoveControls()
        {
            // Empty this.PageList so that we can start a new workflow after policy build
            foreach (var page in this.PageList)
            {
                this.Controls.RemoveByKey(page);
            }
            this.PageList.Clear(); 
        }

        /// <summary>
        /// Public method to set the text and visibility of the info text label at bottom-left of the form or user control.
        /// </summary>
        public void DisplayInfoText(int infoN)
        {
            label_Info.Visible = true;
            label_Info.ForeColor = Color.DeepSkyBlue;

            switch (infoN){
            case 0:
                // Reset label
                label_Info.Text = " ";
                HideInfoLabel();
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
        }

        /// <summary>
        /// Sets the Control Panel and children UI elements
        /// depending on the state of Light or Dark mode
        /// </summary>
        public void SetControlPanelUI()
        {
            // Dark Mode
            if(Properties.Settings.Default.useDarkMode)
            {
                // Control panel color
                this.control_Panel.BackColor = Color.Black; 

                // Subcontrol elements like buttons and text
                foreach (Control control in this.control_Panel.Controls)
                {
                    if (control.Tag == null
                        || control.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        control.ForeColor = Color.White;
                    }
                }

                // Home and Settings images
                settings_Button.Image = Properties.Resources.white_gear;
                home_Button.Image = Properties.Resources.white_house; 
            }

            // Light Mode
            else
            {
                // Control Panel color
                this.control_Panel.BackColor = Color.FromArgb(230, 230, 230); 

                // Subcontrol elements like buttons and text
                foreach(Control control in this.control_Panel.Controls)
                {
                    if(control.Tag == null 
                        || control.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        control.ForeColor = Color.Black; 
                    }
                }

                // Home and Settings images
                settings_Button.Image = Properties.Resources.gear;
                home_Button.Image = Properties.Resources.house;
            }
        }

        /// <summary>
        /// Sets the colors for the Next Button which depends on the 
        /// state of Light and Dark Mode
        /// </summary>
        public void SetNextButtonUI()
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                button_Next.ForeColor = Color.DodgerBlue;
                button_Next.BackColor = Color.Black;
            }

            // Light Mode
            else
            {
                button_Next.ForeColor = Color.Black;
                button_Next.BackColor = Color.WhiteSmoke;
            }
        }

        /// <summary>
        /// Resets the logic flow to the home page by calling the Home Button Click method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ResetWorkflow(object sender, EventArgs e)
        {
            this.Home_Button_Click(sender, e); 
        }
    
        /// <summary>
        /// Creates the session folder. Contents such as the log, intermediate policies are stored here.
        /// </summary>
        private string CreateTempFolder()
        {
            //AppData + WDAC Temp folder
            string tempFolderPath = Helper.GetTempFolderPath(); 

            if (!Directory.Exists(tempFolderPath))
            {
                Directory.CreateDirectory(tempFolderPath);
            }

            return tempFolderPath; 
        }

        /// <summary>
        /// Triggered upon application closing event. 
        /// /// </summary>
        private void FormClosing_Event(object sender, FormClosingEventArgs e)
        {
            this.Log.CloseLogger();
        }

        /// <summary>
        /// Get exe/assembly path  
        /// /// </summary>
        private string GetExecutablePath(bool exePath)
        {
            string executablePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string folderPath = System.IO.Path.GetDirectoryName(executablePath);
            if (exePath)
            {
                return executablePath;
            }
            else
            {
                return folderPath;
            }
        }

        /// <summary>
        /// SKU check to see if cmdlets are on the system 
        /// /// </summary>
        private void LicenseCheck()
        {
            // Check that WDAC feature is compatible with system
            // Cmdlets are available on all builds 1909+. 
            // Pre-1909, Enterprise SKU only: https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/feature-availability

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
            {
                // Device does not meet the versioning or SKU check
                supt_flag = 0;
            }

            // edition or prod name not found in either reg key, n_ed_sup = 0, throw warn msg
            if (supt_flag == 0) 
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
                    System.Diagnostics.Process.Start("https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/feature-availability");
                }
            }
        }

        /// <summary>
        /// Hide info label on timer tick
        /// /// </summary>
        public void HideInfoLabel()
        {
            this.label_Info.Visible = false;
        }

        /// <summary>
        /// Runs on page load. Sets the colors for the UI elements 
        /// depending on the state of Light and Dark Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Load(object sender, EventArgs e)
        {
            // Set background color
            SetMainWindowColors(); 

            // Set UI for the Control Panel
            SetControlPanelUI();

            // Set UI for the 'Next' Button
            SetNextButtonUI(); 
        }

        /// <summary>
        /// Sets the Main Windows UI colors depending on the 
        /// state of Light and Dark Mode
        /// </summary>
        public void SetMainWindowColors()
        {
            // Iterate over all the controls and set the forecolor
            foreach(Control control in this.Controls)
            {
                if(control.GetType() != typeof(Panel)) // exclude the control panel
                {
                    if(control.Tag == null 
                        || control.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        if(Properties.Settings.Default.useDarkMode)
                        {
                            // Dark Mode
                            control.ForeColor = Color.White;
                        }
                        else
                        {
                            // Light Mode
                            control.ForeColor = Color.Black; 
                        }
                    }
                }
            }

            // Set the back color of the Main Window form
            if (Properties.Settings.Default.useDarkMode)
            {
                // Dark Mode
                BackColor = Color.FromArgb(15, 15, 15); 
            }
            else
            {
                // Light Mode
                BackColor = Color.White;
            }
        }

        /// <summary>
        /// Calls the Load() method on all existing pages to ensure they
        /// are Dark Mode and Light Mode compliant
        /// </summary>
        public void ReloadPreviousPages()
        {
            foreach(var pageKey in this.PageList)
            {
                Control[] _Pages = this.Controls.Find(pageKey, true);
                _Pages[0].Refresh(); 
            }
        }

    } // End of MainForm class
}
