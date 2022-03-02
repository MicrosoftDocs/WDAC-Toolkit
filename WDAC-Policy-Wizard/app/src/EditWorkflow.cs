// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.Management.Automation.Runspaces;
using System.Management.Automation; 

namespace WDAC_Wizard
{
    public partial class EditWorkflow : UserControl
    {
        public string EditPath { get; set; }
        private int NumberRules; 
        private Logger Log; 
        private MainWindow _MainWindow;
        private WDAC_Policy Policy;
        private Runspace runspace;
        private List<DriverFile> DriverFiles;
        private WorkflowType Workflow;
        private RuleLevel SelectedLevel; 
        const int PAD_X = 3;
        const int PAD_Y = 3;

        const string EVENT_LOG_POLICY_PATH = "EventLog_ConvertedTo_WDAC.xml"; 

        private enum WorkflowType
        {
            Edit = 0, 
            DeviceEventLog = 1, 
            ArbitraryEventLog = 2
        }

        private enum RuleLevel
        {
            None = -1,
            RootCertificate = 0, 
            PCACertificate = 1, 
            Publisher = 2, 
            SignedVersion = 3, 
            FilePublisher = 4, 
            FileName = 5, 
            Hash = 6 
        }

        public EditWorkflow(MainWindow pMainWindow)
        {
            InitializeComponent();

            this._MainWindow = pMainWindow;
            this._MainWindow.ErrorOnPage = false;
            this._MainWindow.RedoFlowRequired = false;
            this._MainWindow.Policy._PolicyType = WDAC_Policy.PolicyType.Edit;
            this.Policy = this._MainWindow.Policy; 
            this.Log = this._MainWindow.Log;
            this.Log.AddInfoMsg("==== Edit Workflow Page Initialized ====");
            this.Workflow = WorkflowType.Edit; // Edit xml is default in the UI
            this.SelectedLevel = RuleLevel.None; 
        }

        /// <summary>
        /// Sets the Edit Policy Path parameter of the policy object. Launches the OpenFileDialog so the user can 
        /// select the Policy they would like to edit. 
        /// </summary>
        private void button_Create_Click(object sender, EventArgs e)
        {
            // If user is changing the policy schema being edited, show message
            if(this._MainWindow.PageList.Count > 1)
            {
                DialogResult res = MessageBox.Show("Modifying the current schema to edit will cause you to lose your progress." +
                    "Are you sure you want to do this?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                {
                    this._MainWindow.RedoFlowRequired = true;
                }
                else
                {
                    return;
                }
            }
                       
            this.Log.AddInfoMsg("Browsing for existing WDAC Policy on file.");
            string policyPath = String.Empty; 

            try
            {
                policyPath = Helper.BrowseForSingleFile(Properties.Resources.OpenXMLFileDialogTitle, Helper.BrowseFileType.Policy); 
                if (! String.IsNullOrEmpty(policyPath))
                {
                    textBoxPolicyPath.Text = policyPath;                   
                    this.EditPath = policyPath;

                    // Parse the policy for its information and display it
                    ParsePolicy(this.EditPath);
                    DisplayPolicyInfo(); 

                    this._MainWindow.Policy.EditPolicyPath = this.EditPath;

                    // If user has returned to this page and updated the policy, must proceed to page 2
                    this._MainWindow.RedoFlowRequired = true; 
                }
            }

            catch(Exception exp)
            {
                this.Log.AddErrorMsg("EditWorkflow Browse() encountered the following error ", exp); 
            }
        }

        /// <summary>
        /// Parses the Edit Policy to read in the Policy Settings (end of policy), specifically, policy name and ID. 
        /// </summary>
        private void ParsePolicy(string xmlPath)
        {
            // Serialize the policy into the policy object
            try
            {
                this.Policy.siPolicy = Helper.DeserializeXMLtoPolicy(xmlPath); 
                this._MainWindow.ErrorOnPage = false;

                // Set the policy type (base, supplemental) to know which policy rule-options to set on ConfigTemplate_Control
                // Set the policy format type for the policy creation step in MainForm.cs
                if(this.Policy.siPolicy.PolicyTypeID == Properties.Resources.LegacyPolicyID_GUID)
                {
                    this._MainWindow.Policy._Format = WDAC_Policy.Format.Legacy;
                }
                else
                {
                    this._MainWindow.Policy._Format = WDAC_Policy.Format.MultiPolicy;
                }
            }

            catch (Exception e)
            {
                // Log eexception error and throw error to user
                DialogResult res = MessageBox.Show("The base policy you have selected cannot be parsed by the Wizard\n\n" +
                    "This is typically a result of a malformed policy.",
                    "Policy Parsing Issue", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Log.AddErrorMsg("ParsePolicy encountered the following error message" + e.ToString()); 
                this._MainWindow.ErrorOnPage = true;
            }
        }

        /// <summary>
        /// Displays the Policy settings: policy name and ID, to the User to allow for modification. 
        /// </summary>
        private void DisplayPolicyInfo()
        {
            // Set the default text fields to N/A. Will overwrite if we find true settings
            textBox_PolicyID.Text =   "N/A"; 
            textBox_PolicyName.Text = "N/A"; 
            if(this.Policy.siPolicy.Settings != null)
            {
                foreach (var setting in this.Policy.siPolicy.Settings)
                {
                    if (setting.ValueName == "Name")
                    {
                        // Found the name of the policy
                        textBox_PolicyName.Text = setting.Value.Item.ToString();
                    }
                    else if (setting.ValueName == "Id")
                    {
                        // Found the name of the policy
                        textBox_PolicyID.Text = setting.Value.Item.ToString();
                    }
                    else
                    {
                        // Found another setting that we do not show to user
                        continue;
                    }
                }
            }

            policyInfoPanel.Visible = true;
        }

        /// <summary>
        /// Sets the new Policy Name setting. 
        /// </summary>
        private void TextBox_PolicyName_TextChanged(object sender, EventArgs e)
        {
            this._MainWindow.Policy.PolicyName = textBox_PolicyName.Text;
        }

        /// <summary>
        /// Sets the new Policy ID setting. 
        /// </summary>
        private void textBox_PolicyID_TextChanged(object sender, EventArgs e)
        {
            this._MainWindow.Policy.PolicyID = textBox_PolicyID.Text;
        }


        private void button_Parse_LogFile_Click(object sender, EventArgs e)
        {

            if (this.SelectedLevel == RuleLevel.None)
            {
                this.Log.AddWarningMsg("Selected Level is null. Level must be selected before Parse_LogEvent_Click");
                this.label_Error.Text = "Rule level must be selected before event log file parsing can begin";
                this.label_Error.Visible = true;
                this.label_Error.BringToFront();
                return; 
            }

            if (this.runspace == null)
            {
                this.runspace = RunspaceFactory.CreateRunspace();
                this.runspace.Open();
            }

            string dspTitle = "Choose event logs to convert to policy";
            List<string> eventLogPaths = Helper.BrowseForMultiFiles(dspTitle, Helper.BrowseFileType.EventLog);

            if(eventLogPaths == null)
            {
                return; 
            }

            // Prep UI
            this.textBox_EventLogFilePath.Text = eventLogPaths[0]; 
            this.panel_Progress.Visible = true;
            this.label_Error.Visible = false;
            this.eventLogParsing_Result_Panel.Visible = false;
            this.Workflow = WorkflowType.ArbitraryEventLog;


            this.DriverFiles = Helper.ReadArbitraryEventLogs(eventLogPaths);
                     
            // TODO: handle 0 case
            this.NumberRules = this.DriverFiles.Count;
            this.label_Progress.Text = String.Format("0 / {0} Rules from Event Log Created", this.NumberRules); 


            // Create background worker to display updates to UI
            if (!this.backgroundWorker.IsBusy)
            {
                this.backgroundWorker.RunWorkerAsync();
            }  
        }

        private void button_ParseEventLog_Click(object sender, EventArgs e)
        {
            // Serialize the siPolicy to xml and display the name and ID to user. 
            // Afterwards, set the editPath to the temp location of the xml

            if(this.SelectedLevel == RuleLevel.None)
            {
                this.Log.AddWarningMsg("Selected Level is null. Level must be selected before ParseEventLog_Click");
                this.label_Error.Text = "Rule level must be selected before event log parsing can begin";
                this.label_Error.Visible = true;
                this.label_Error.BringToFront(); 
                return; 
            }

            this.panel_Progress.Visible = true;
            this.label_Error.Visible = false;
            this.eventLogParsing_Result_Panel.Visible = false; 
            this.label_Progress.Text = "Event Viewer Log Conversion in Progress";
            this.Workflow = WorkflowType.DeviceEventLog; 

            // Create background worker to display updates to UI
            if (!this.backgroundWorker.IsBusy)
            {
                this.backgroundWorker.RunWorkerAsync();
            }

        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.backgroundWorker = sender as BackgroundWorker;
            string xmlPath = String.Empty; 
            if(this.Workflow == WorkflowType.ArbitraryEventLog)
            {
                List<string> policyPaths = ConvertDriverFilestoXml(this.DriverFiles);
                xmlPath = MergeAllPolicies(policyPaths); 
            }
            
            else if(this.Workflow == WorkflowType.DeviceEventLog)
            {
                SiPolicy siPolicy = Helper.ReadMachineEventLogs(this._MainWindow.TempFolderPath, this.SelectedLevel.ToString());

                xmlPath = Path.Combine(this._MainWindow.TempFolderPath, EVENT_LOG_POLICY_PATH);
                Helper.SerializePolicytoXML(siPolicy, xmlPath);
            }
            else
            {

            }

            // Set paths correctly so policy rules page can parse the policy
            this.EditPath = xmlPath;
            this._MainWindow.Policy.EditPolicyPath = this.EditPath;
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int progressPercent = e.ProgressPercentage;
            double completedRules = Math.Ceiling((double)progressPercent / (double)100 * (double)this.NumberRules); 
            string progress = String.Format("{0} / {1} Rules from Event Log Created", (int) completedRules, this.NumberRules); 

            label_Progress.Text = progress; 

        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Remove GIF // Update UI 
            this.panel_Progress.Visible = false;
            this.eventLogParsing_Result_Panel.Visible = true;
            this.eventLogParsing_Result_Panel.BringToFront(); 

            // Unsuccessful conversion
            if (e.Error != null)
            {
                this.Log.AddErrorMsg("ProcessPolicy() caught the following exception ", e.Error);
                this.parseResults_Label.Text = Properties.Resources.UnsuccessfulEventLogConversion; 
                this.parseresult_PictureBox.Image = Properties.Resources.not_extendable; 
            }
            else
            {
                this.parseResults_Label.Text = Properties.Resources.EventLogConversionSuccess;
                this.parseresult_PictureBox.Image = Properties.Resources.verified;
                DialogResult res = MessageBox.Show(Properties.Resources.EventLogConversionSuccess, "WDAC Wizard Event Log to WDAC Policy Conversion Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.Log.AddNewSeparationLine("Event Parsing Workflow -- DONE");

        }


        public string CreateCustomRuleScript(PolicyCustomRules customRule)
        {
            string customRuleScript = string.Empty;

            // Create new CI Rule: https://docs.microsoft.com/powershell/module/configci/new-cipolicyrule
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
                        if (customRule.GetRuleType() == PolicyCustomRules.RuleType.FilePath && Properties.Settings.Default.useEnvVars )
                            customRuleScript = String.Format("$Rule_{0} = New-CIPolicyRule -FilePathRule \"{1}\"", customRule.PSVariable, Helper.GetEnvPath(customRule.ReferenceFile));
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
            {
                customRuleScript += " -Deny";
            }
                
            return customRuleScript;
        }

        /// <summary>
        /// Creates a unique CI Policy file per custom rule defined in the SigningRules_Control. Writes to a unique filepath.
        /// </summary>
        public string CreatePolicyScript(PolicyCustomRules customRule, string tempPolicyPath)
        {
            string policyScript;

            if (this.Policy._Format == WDAC_Policy.Format.MultiPolicy)
            {
                policyScript = String.Format("New-CIPolicy -MultiplePolicyFormat -FilePath \"{0}\" -Rules $Rule_{1}", tempPolicyPath, customRule.PSVariable);
            }
                
            else
            {
                policyScript = String.Format("New-CIPolicy -FilePath \"{0}\" -Rules $Rule_{1}", tempPolicyPath, customRule.PSVariable);
            }
                
            return policyScript;
        }

        private List<string> ConvertDriverFilestoXml(List<DriverFile> driverFiles)
        {
            List<string> policyPaths = new List<string>();

            for (int i = 0; i < driverFiles.Count(); i++)
            {
                var file = driverFiles[i];
                string tmpPolicyPath = Helper.GetUniquePolicyPath(this._MainWindow.TempFolderPath);
                PolicyCustomRules customRule = new PolicyCustomRules();
                customRule.ReferenceFile = file.Path;

                switch ((int)this.SelectedLevel)
                {
                    case 0:
                        customRule.Type = PolicyCustomRules.RuleType.Publisher;
                        customRule.Level = PolicyCustomRules.RuleLevel.RootCertificate;
                        break;

                    case 1:
                        customRule.Type = PolicyCustomRules.RuleType.Publisher;
                        customRule.Level = PolicyCustomRules.RuleLevel.PcaCertificate;
                        break;

                    case 2:
                        customRule.Type = PolicyCustomRules.RuleType.Publisher;
                        customRule.Level = PolicyCustomRules.RuleLevel.Publisher;
                        break;

                    case 3:
                        customRule.Type = PolicyCustomRules.RuleType.Publisher;
                        customRule.Level = PolicyCustomRules.RuleLevel.SignedVersion;
                        break;

                    case 4:
                        customRule.Type = PolicyCustomRules.RuleType.Publisher;
                        customRule.Level = PolicyCustomRules.RuleLevel.FilePublisher;
                        break;

                    case 5:
                        customRule.Type = PolicyCustomRules.RuleType.FilePath;
                        customRule.Level = PolicyCustomRules.RuleLevel.FilePath;
                        break;

                    case 6:
                        customRule.Type = PolicyCustomRules.RuleType.Hash;
                        customRule.Level = PolicyCustomRules.RuleLevel.Hash;
                        break;
                }
                
                
                customRule.Permission = PolicyCustomRules.RulePermission.Allow;
                customRule.PSVariable = i.ToString();
                string ruleScript = CreateCustomRuleScript(customRule);
                string policyScript = CreatePolicyScript(customRule, tmpPolicyPath);

                // Add script to pipeline and run PS command
                Pipeline pipeline = this.runspace.CreatePipeline();
                pipeline.Commands.AddScript(ruleScript);
                pipeline.Commands.AddScript(policyScript);

                // Update progress bar per completion of custom rule created
                double prog = Math.Ceiling((double)i / (double)driverFiles.Count * (double)100);
                this.backgroundWorker.ReportProgress((int) prog);

                try
                {
                    Collection<PSObject> results = pipeline.Invoke();
                    policyPaths.Add(tmpPolicyPath);
                }
                catch (Exception exp)
                {
                    this.Log.AddErrorMsg("CreatePolicyFileRuleOptions() caught the following exception ", exp);
                }

                pipeline.Dispose(); 
            }

            return policyPaths; 
        }

        private string MergeAllPolicies(List<string> policyPaths)
        {
            string mergeScript = "Merge-CIPolicy -PolicyPaths ";
            string xmlPath = Path.Combine(this._MainWindow.TempFolderPath, EVENT_LOG_POLICY_PATH);

            foreach (var path in policyPaths)
            {
                mergeScript += String.Format("\"{0}\",", path);
            }

            // Remove last comma and add outputFilePath
            mergeScript = mergeScript.Remove(mergeScript.Length - 1);
            mergeScript += String.Format(" -OutputFilePath \"{0}\"", xmlPath);
            Pipeline mergePipeline = this.runspace.CreatePipeline();

            mergePipeline.Commands.AddScript(mergeScript);
            mergePipeline.Commands.Add("Out-String");
            mergePipeline.Invoke();
            mergePipeline.Dispose();

            return xmlPath;
        }

        private void Label_LearnMore_Click(object sender, EventArgs e)
        {
            // multi-policy info label clicked. Launch multi-policy info webpage
            try
            {
                string webpage = "https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control"
                    + "/audit-windows-defender-application-control-policies#create-a-windows-defender-application-control-policy-that-captures-audit-information-from-the-event-log";
                System.Diagnostics.Process.Start(webpage);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("Launching webpage for multipolicy link encountered the following error", exp);
            }
        }

        private void EditXML_RadioButton_Click(object sender, EventArgs e)
        {
            // User wants to edit xml file of an existing policy. Prepare the UI accordingly
            this.panel_Edit_XML.Visible = true;
            this.panel_EventLog_Conversion.Visible = false;

            // Bring edit xml panel to upper-right corner of page panel
            Point urPoint = new Point(PAD_X, PAD_Y);
            this.panel_Edit_XML.Location = urPoint; 
        }

        private void EventConversion_RadioButton_Click(object sender, EventArgs e)
        {
            // User wants to convert an event log to a WDAC policy. Prepare the UI accordingly
            this.panel_Edit_XML.Visible = false;
            this.panel_EventLog_Conversion.Visible = true;
            this.textBox_EventLog.Text = Properties.Resources.CILogEvtPath;


            // Bring edit xml panel to upper-right corner of page panel
            Point urPoint = new Point(PAD_X, PAD_Y);
            this.panel_EventLog_Conversion.Location = urPoint;
        }

        private void comboBox_Level_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Convert selected index to this.SelectedLevel
            switch(this.comboBox_Level.SelectedIndex)
            {
                case 0:
                    this.SelectedLevel = RuleLevel.RootCertificate;
                    break;

                case 1:
                    this.SelectedLevel = RuleLevel.PCACertificate;
                    break;

                case 2:
                    this.SelectedLevel = RuleLevel.Publisher;
                    break;

                case 3:
                    this.SelectedLevel = RuleLevel.SignedVersion;
                    break;

                case 4:
                    this.SelectedLevel = RuleLevel.FilePublisher;
                    break;

                case 5:
                    this.SelectedLevel = RuleLevel.FileName;
                    break;

                case 6:
                    this.SelectedLevel = RuleLevel.Hash;
                    break;
            }
        }
    }
}
