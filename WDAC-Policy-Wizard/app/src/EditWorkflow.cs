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
                    this._MainWindow.RedoFlowRequired = true;
                else
                    return;
            }
                       
            this.Log.AddInfoMsg("Browsing for existing WDAC Policy on file.");

            // Save dialog box pressed
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse for existing WDAC Policy File";
            openFileDialog.CheckPathExists = true;
            openFileDialog.DefaultExt = "xml";
            openFileDialog.Filter = "Policy Files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxPolicyPath.Text = openFileDialog.FileName;                   
                    this.EditPath = openFileDialog.FileName;

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
            openFileDialog.Dispose();
        }

        /// <summary>
        /// Parses the Edit Policy to read in the Policy Settings (end of policy), specifically, policy name and ID. 
        /// </summary>
        private void ParsePolicy(string xmlPath)
        {
            // Serialize the policy into the policy object
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SiPolicy));
                StreamReader reader = new StreamReader(xmlPath);
                this.Policy.siPolicy = (SiPolicy)serializer.Deserialize(reader);
                reader.Close();
                this._MainWindow.ErrorOnPage = false;

                // Set the policy format type for the policy creation step in MainForm.cs
                if(this.Policy.siPolicy.BasePolicyID != null)
                {
                    this._MainWindow.Policy._Format = WDAC_Policy.Format.MultiPolicy;
                }
                else
                {
                    this._MainWindow.Policy._Format = WDAC_Policy.Format.Legacy; 
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
        private void textBox_PolicyName_TextChanged(object sender, EventArgs e)
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
            if (this.runspace == null)
            {
                this.runspace = RunspaceFactory.CreateRunspace();
                this.runspace.Open();
            }

            string dspTitle = "Choose event logs to convert to policy";
            List<string> eventLogPaths = Helper.BrowseForMultiFiles(dspTitle, Helper.BrowseFileType.EventLog);

            this.DriverFiles = Helper.ReadArbitraryEventLogs(eventLogPaths);
            this.panel_Progress.Visible = true;
            // TODO: handle 0 case
            this.NumberRules = this.DriverFiles.Count;

            // Create background worker to display updates to UI
            if (!this.backgroundWorker.IsBusy)
            {
                this.backgroundWorker.RunWorkerAsync();
            }  
        }

        private void button_ParseEventLog_Click(object sender, EventArgs e)
        {
            SiPolicy siPolicy = Helper.ReadMachineEventLogs(this._MainWindow.TempFolderPath);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.backgroundWorker = sender as BackgroundWorker;

            bool success = ConvertDriverFilestoXml(this.DriverFiles);
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int progressPercent = e.ProgressPercentage;
            double completedRules = Math.Ceiling((double)progressPercent / (double)100 * (double)this.NumberRules); 
            string progress = String.Format("{0} / {1} Rules Created", (int) completedRules, this.NumberRules); 

            label_Progress.Text = progress; 

        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                this.Log.AddErrorMsg("ProcessPolicy() caught the following exception ", e.Error);
            }
            else
            {
                // Remove GIF // Update UI 
                this.panel_Progress.Visible = false; 

            }

            this.Log.AddNewSeparationLine("Event Parsing Workflow -- DONE");

        }

        private void Button_ParseEventLog_Click(object sender, EventArgs e)
        {
            SiPolicy siPolicy = Helper.ReadMachineEventLogs(this._MainWindow.TempFolderPath, "Publisher"); 
        }

        public string CreateCustomRuleScript(PolicyCustomRules customRule)
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
            string policyScript = string.Empty;

            if (this.Policy._Format == WDAC_Policy.Format.MultiPolicy)
                policyScript = String.Format("New-CIPolicy -MultiplePolicyFormat -FilePath \"{0}\" -Rules $Rule_{1}", tempPolicyPath, customRule.PSVariable);

            else
                policyScript = String.Format("New-CIPolicy -FilePath \"{0}\" -Rules $Rule_{1}", tempPolicyPath, customRule.PSVariable);

            return policyScript;
        }

        private bool ConvertDriverFilestoXml(List<DriverFile> driverFiles)
        {
            bool wasSuccessful = true;
            int progressVal = 0; 
            List<string> policyPaths = new List<string>();

            for (int i = 0; i < 5; i++)// driverFiles.Count(); i++)
            {
                var file = driverFiles[i];
                string tmpPolicyPath = Helper.GetUniquePolicyPath(this._MainWindow.TempFolderPath);
                PolicyCustomRules customRule = new PolicyCustomRules();
                customRule.ReferenceFile = file.Path;
                customRule.Type = PolicyCustomRules.RuleType.Publisher;
                customRule.Level = PolicyCustomRules.RuleLevel.Publisher;
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


            string mergeScript = "Merge-CIPolicy -PolicyPaths ";
            string outputFilePath = Path.Combine(this._MainWindow.TempFolderPath, "Audit_Merge_Policy.xml");
            foreach (var path in policyPaths)
            {
                mergeScript += String.Format("\"{0}\",", path);
            }

            // Remove last comma and add outputFilePath
            mergeScript = mergeScript.Remove(mergeScript.Length - 1);
            mergeScript += String.Format(" -OutputFilePath \"{0}\"", outputFilePath);
            Pipeline mergePipeline = this.runspace.CreatePipeline();

            mergePipeline.Commands.AddScript(mergeScript);
            mergePipeline.Commands.Add("Out-String");
            mergePipeline.Invoke();
            mergePipeline.Dispose();

            return wasSuccessful; 
        }
    }
}
