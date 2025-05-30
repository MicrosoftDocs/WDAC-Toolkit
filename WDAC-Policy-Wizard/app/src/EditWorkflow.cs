// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Management.Automation.Runspaces;
using System.Diagnostics;

namespace WDAC_Wizard
{
    public partial class EditWorkflow : UserControl
    {
        public string EditPath { get; set; }
        private int NumberRules; 
        private MainWindow _MainWindow;
        private WDAC_Policy Policy;
        private Runspace runspace;
        private WorkflowType Workflow;
        private List<string> EventLogPaths; 
        const int PAD_X = 3;
        const int PAD_Y = 3;

        const string EVENT_LOG_POLICY_PATH = "EventLog_ConvertedTo_WDAC.xml"; 

        public enum WorkflowType
        {
            Edit = 0, 
            DeviceEventLog = 1, 
            ArbitraryEventLog = 2,
            AdvancedHunting = 3
        }

        public EditWorkflow(MainWindow pMainWindow)
        {
            InitializeComponent();

            _MainWindow = pMainWindow;
            _MainWindow.ErrorOnPage = true;
            _MainWindow.ErrorMsg = Properties.Resources.ChoosePolicyToEdit_Error;
            _MainWindow.RedoFlowRequired = false;
            Policy = _MainWindow.Policy; 
            Logger.Log.AddInfoMsg("==== Edit Workflow Page Initialized ====");
            Workflow = WorkflowType.Edit; // Edit xml is default in the UI
            EventLogPaths = new List<string>();
        }

        /// <summary>
        /// Sets the Edit Policy Path parameter of the policy object. Launches the OpenFileDialog so the user can 
        /// select the Policy they would like to edit. 
        /// </summary>
        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            // If user is changing the policy schema being edited, show message
            if(_MainWindow.PageList.Count > 1)
            {
                DialogResult res = MessageBox.Show("Modifying the current schema to edit will cause you to lose your progress. " +
                    "Are you sure you want to do this?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                {
                    _MainWindow.RedoFlowRequired = true;
                }
                else
                {
                    return;
                }
            }
                       
            Logger.Log.AddInfoMsg("Browsing for existing WDAC Policy on file.");
            string policyPath = String.Empty; 

            try
            {
                policyPath = Helper.BrowseForSingleFile(Properties.Resources.OpenXMLFileDialogTitle, Helper.BrowseFileType.Policy); 
                if (! String.IsNullOrEmpty(policyPath))
                {
                    textBoxPolicyPath.Text = policyPath;                   
                    EditPath = policyPath;

                    // Parse the policy for its information and display it
                    ParsePolicy(EditPath);
                    DisplayPolicyInfo(); 

                    _MainWindow.Policy.EditPolicyPath = EditPath;

                    // If user has returned to this page and updated the policy, must proceed to page 2
                    _MainWindow.RedoFlowRequired = true;

                    // Clear error flag to advance to the next page
                    _MainWindow.ErrorOnPage = false;
                    _MainWindow.DisplayInfoText(0);
                }
            }

            catch(Exception exp)
            {
                Logger.Log.AddErrorMsg("EditWorkflow Browse() encountered the following error ", exp); 
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
                Policy.siPolicy = Helper.DeserializeXMLtoPolicy(xmlPath); 
                _MainWindow.ErrorOnPage = false;

                // Set the policy format type for the policy creation step in MainForm.cs
                if(Policy.siPolicy.PolicyTypeID == Properties.Resources.LegacyPolicyID_GUID)
                {
                    _MainWindow.Policy._Format = WDAC_Policy.Format.Legacy;
                }
                else
                {
                    _MainWindow.Policy._Format = WDAC_Policy.Format.MultiPolicy;
                    
                    // Set the policy type (base, supplemental) to know which policy rule-options to set on ConfigTemplate_Control
                    if (Policy.siPolicy.PolicyType == global::PolicyType.SupplementalPolicy)
                    {
                        _MainWindow.Policy._PolicyType = WDAC_Policy.PolicyType.SupplementalPolicy; 
                    }
                    else
                    {
                        _MainWindow.Policy._PolicyType = WDAC_Policy.PolicyType.BasePolicy; 
                    }
                }
            }

            catch (Exception e)
            {
                // Log exception error and throw error to user
                DialogResult res = MessageBox.Show("The base policy you have selected cannot be parsed by the Wizard\n\n" +
                    "This is typically a result of a malformed policy.",
                    "Policy Parsing Issue", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Logger.Log.AddErrorMsg("ParsePolicy encountered the following error message" + e.ToString()); 
                _MainWindow.ErrorOnPage = true;
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
            if(Policy.siPolicy.Settings != null)
            {
                foreach (var setting in Policy.siPolicy.Settings)
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
            _MainWindow.Policy.PolicyName = textBox_PolicyName.Text;
        }

        /// <summary>
        /// Sets the new Policy ID setting. 
        /// </summary>
        private void TextBox_PolicyID_TextChanged(object sender, EventArgs e)
        {
            _MainWindow.Policy.PolicyID = textBox_PolicyID.Text;
        }

        /// <summary>
        /// Parses the event logs on file provided by the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParseLog_ButtonClick(object sender, EventArgs e)
        {
            string dspTitle = "Choose event logs to convert to policy";
            List<string> eventLogPaths = Helper.BrowseForMultiFiles(dspTitle, Helper.BrowseFileType.EventLog);

            if(eventLogPaths == null)
            {
                return; 
            }

            EventLogPaths = eventLogPaths;

            // Prep UI
            textBox_EventLogFilePath.Lines = eventLogPaths.ToArray(); 
            panel_Progress.Visible = true;
            panel_Progress.Location = buttonParseEventLog.Location; // Center the progress gif
            Workflow = WorkflowType.ArbitraryEventLog;

            // Clear error labels if applicable
            label_Error.Visible = false;
            eventLogParsing_Result_Panel.Visible = false;
            ahParsingLearnMore_Label.Visible = false;
            _MainWindow.DisplayInfoText(0);

            // Create background worker to display updates to UI
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }  
        }

        /// <summary>
        /// Parses the system event logs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParseSystemLog_ButtonClick(object sender, EventArgs e)
        {
            // Serialize the siPolicy to xml and display the name and ID to user. 
            // Afterwards, set the editPath to the temp location of the xml
            panel_Progress.Visible = true;
            panel_Progress.Location = buttonParseEventLog.Location; // Center the progress gif
            label_Progress.Text = "Event Viewer Log Parsing in Progress";
            Workflow = WorkflowType.DeviceEventLog;

            // Clear error labels if applicable
            label_Error.Visible = false;
            eventLogParsing_Result_Panel.Visible = false;
            ahParsingLearnMore_Label.Visible = false;
            _MainWindow.DisplayInfoText(0);

            // Create background worker to display updates to UI
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Runs the Advanced Hunting CSV parsing logic
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParseMDEAHLogs_ButtonClick(object sender, EventArgs e)
        {
            string dspTitle = "Select Advanced Hunting exported CSV files to convert to policy";
            List<string> eventLogPaths = Helper.BrowseForMultiFiles(dspTitle, Helper.BrowseFileType.CsvFile);

            if (eventLogPaths == null)
            {
                return;
            }

            EventLogPaths = eventLogPaths;

            // Prep UI
            textBox_AdvancedHuntingPaths.Lines = eventLogPaths.ToArray();
            panel_Progress.Visible = true;
            panel_Progress.Location = buttonParseEventLog.Location; // Center the progress gif
            Workflow = WorkflowType.AdvancedHunting;

            // Clear error labels if applicable
            label_Error.Visible = false;
            eventLogParsing_Result_Panel.Visible = false;
            ahParsingLearnMore_Label.Visible = false;
            _MainWindow.DisplayInfoText(0);

            // Create background worker to display updates to UI
            if (!backgroundWorker.IsBusy)
            {
                backgroundWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Parses the event logs provided by the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundWorker = sender as BackgroundWorker;
            if(Workflow == WorkflowType.ArbitraryEventLog)
            {
                _MainWindow.CiEvents = EventLog.ReadArbitraryEventLogs(EventLogPaths);
            }
            else if(Workflow == WorkflowType.AdvancedHunting)
            {
                // Process CSV file(s) as if they are Advanced Hunting and LogAnalytics

                List<CiEvent> aHuntingEvents = AdvancedHunting.ReadAdvancedHuntingCsvFiles(EventLogPaths);
                List<CiEvent> lAnalyticEvents = LogAnalytics.ReadLogAnalyticCsvFiles(EventLogPaths); 

                if(aHuntingEvents != null)
                {
                    _MainWindow.CiEvents = aHuntingEvents; 
                }

                if(lAnalyticEvents != null)
                {
                    if (_MainWindow.CiEvents == null)
                    {
                        _MainWindow.CiEvents = lAnalyticEvents; 
                    }
                    else
                    {
                        _MainWindow.CiEvents.AddRange(lAnalyticEvents);
                    }
                }

            }
            else
            {
                _MainWindow.CiEvents = EventLog.ReadSystemEventLogs();
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int progressPercent = e.ProgressPercentage;
            double completedRules = Math.Ceiling((double)progressPercent / (double)100 * (double)NumberRules); 
            string progress = String.Format("{0} / {1} Event Log events parsed", (int) completedRules, NumberRules); 

            label_Progress.Text = progress; 
        }

        /// <summary>
        /// Sets the UI after parsing the event logs finishes successfully or unsuccessfully
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Remove GIF // Update UI 
            panel_Progress.Visible = false;
            eventLogParsing_Result_Panel.Visible = true;
            eventLogParsing_Result_Panel.BringToFront(); 

            // Unsuccessful conversion
            if (e.Error != null)
            {
                Logger.Log.AddErrorMsg("ProcessPolicy() caught the following exception ", e.Error);
                parseResults_Label.Text = Properties.Resources.UnsuccessfulEventLogConversion; 
                parseresult_PictureBox.Image = Properties.Resources.not_extendable;
                _MainWindow.ErrorOnPage = true;
            }
            else if(_MainWindow.CiEvents == null
                    || _MainWindow.CiEvents.Count < 1)
            {
                // Show AH/LogAnalytics error messages
                if(Workflow == WorkflowType.AdvancedHunting)
                {
                    string errString = $"Advanced Hunting parsing returned: {AdvancedHunting.GetLastError()} " +
                                   $"\r\n\r\nLogAnalytics parsing returned {LogAnalytics.GetLastError()}";
                    Logger.Log.AddErrorMsg(errString);

                    _ = MessageBox.Show(errString,
                                        "Wizard Event Log Parsing Error",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);

                    parseResults_Label.Text = Properties.Resources.UnsuccessfulAdvancedHuntingLogConversion;
                    parseresult_PictureBox.Image = Properties.Resources.not_extendable;
                    _MainWindow.ErrorOnPage = true;
                    ahParsingLearnMore_Label.Visible = true;
                }

                // Show evtx error messages
                else
                {
                    Logger.Log.AddErrorMsg("Zero CiEvents were created.");
                    parseResults_Label.Text = Properties.Resources.UnsuccessfulEventLogConversionZeroEvents;
                    parseresult_PictureBox.Image = Properties.Resources.not_extendable;
                    _MainWindow.ErrorOnPage = true;
                }
            }
            else
            {
                parseResults_Label.Text = Properties.Resources.EventLogConversionSuccess;
                parseresult_PictureBox.Image = Properties.Resources.verified;
                _ = MessageBox.Show(Properties.Resources.EventLogConversionSuccess, 
                                    "Wizard Event Log Parsing Success", 
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                _MainWindow.ErrorOnPage = false;
            }

            Logger.Log.AddNewSeparationLine("Event Parsing Workflow -- DONE");
        }


        /// <summary>
        /// Opens the 'Create a policy from Event Logs' MS Docs page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_LearnMore_Click(object sender, EventArgs e)
        {
            try
            {
                string webpage = "https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control"
                    + "/audit-windows-defender-application-control-policies#create-a-windows-defender-application-control-policy-that-captures-audit-information-from-the-event-log";
                Process.Start(new ProcessStartInfo(webpage) { UseShellExecute = true });
            }
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg("Launching webpage for multipolicy link encountered the following error", exp);
            }
        }

        /// <summary>
        /// Sets the UI state for Edit XML workflow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditXML_RadioButton_Click(object sender, EventArgs e)
        {
            // User wants to edit xml file of an existing policy. Prepare the UI accordingly
            panel_Edit_XML.Visible = true;
            panel_EventLog_Conversion.Visible = false;

            _MainWindow.EditWorkflow = MainWindow.EditWorkflowType.Edit;

            // Bring edit xml panel to upper-right corner of page panel
            Point urPoint = new Point(PAD_X, PAD_Y);
            panel_Edit_XML.Location = urPoint;

            // Update error flag and message
            _MainWindow.ErrorMsg = Properties.Resources.ChoosePolicyToEdit_Error;
            if(String.IsNullOrEmpty(_MainWindow.Policy.EditPolicyPath))
            {
                _MainWindow.ErrorOnPage = true;
            }
            else
            {
                _MainWindow.ErrorOnPage = false;
            }

            // Update redo required flag
            if (_MainWindow.CiEvents != null && _MainWindow.CiEvents.Count > 0)
            {
                _MainWindow.RedoFlowRequired = true;
            }
        }

        /// <summary>
        /// Sets the UI state for event conversion workflow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventConversion_RadioButton_Click(object sender, EventArgs e)
        {
            // User wants to convert an event log to a WDAC policy. Prepare the UI accordingly
            panel_Edit_XML.Visible = false;
            panel_EventLog_Conversion.Visible = true;
            _MainWindow.EditWorkflow = MainWindow.EditWorkflowType.EventLog;

            // Bring edit xml panel to upper-right corner of page panel
            Point urPoint = new Point(PAD_X, PAD_Y);
            panel_EventLog_Conversion.Location = urPoint;

            // Update the error flag and message
            _MainWindow.ErrorMsg = Properties.Resources.ChooseEventLog_Error;
            if(_MainWindow.CiEvents == null || _MainWindow.CiEvents.Count < 1)
            {
                _MainWindow.ErrorOnPage = true; 
            }
            else
            {
                _MainWindow.ErrorOnPage = false;
            }

            // Update redo required flag
            if (!String.IsNullOrEmpty(_MainWindow.Policy.EditPolicyPath))
            {
                _MainWindow.RedoFlowRequired = true;
            }
        }

        /// <summary>
        /// Sets the save location (SchemaPath) for a file under edit 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonNewSaveLocation_Pressed(object sender, EventArgs e)
        {
            string saveLocation = Helper.SaveSingleFile(Properties.Resources.BrowseForXmlSaveLoc, Helper.BrowseFileType.Policy); 
            if(!String.IsNullOrEmpty(saveLocation))
            {
                textBoxSaveLocation.Text = saveLocation;
                textBoxSaveLocation.SelectionStart = textBoxSaveLocation.TextLength - 1;
                textBoxSaveLocation.ScrollToCaret();

                _MainWindow.Policy.SchemaPath = saveLocation; 
            }
        }

        /// <summary>
        /// Opens the WDAC Wizard docs page for parsing MDE Advanced Hunting events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AHLearnMoreLabel_Click(object sender, EventArgs e)
        {
            string webpage = "https://aka.ms/wdacWizardAHParsing";
            try
            {
                Process.Start(new ProcessStartInfo(webpage) { UseShellExecute = true });
            }
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg(String.Format("Launching {0} encountered the following error", webpage), exp);
            }
        }

        /// <summary>
        /// Form painting. Occurs on Form.Refresh, Load and Focus. 
        /// Used for UI element changes for Dark and Light Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditWorkflow_Validated(object sender, EventArgs e)
        {
            // Set Controls Color (e.g. Panels, Textboxes, Buttons)
            SetControlsColor();

            // Set UI for the 'buttonBrowse' Button
            SetButtonUI(buttonBrowse);

            // Set UI for the 'buttonNewSaveLocation' Button
            SetButtonUI(buttonNewSaveLocation);

            // Set UI for the 'buttonParseEventLog' Button
            SetButtonUI(buttonParseEventLog);

            // Set UI for the 'buttonParseLogFile' Button
            SetButtonUI(buttonParseLogFile);

            // Set UI for the 'buttonParseMDELog' Button
            SetButtonUI(buttonParseMDELog);

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
                    // Buttons
                    if (control is Button button
                        && (button.Tag == null || button.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        button.ForeColor = Color.White;
                        button.BackColor = Color.FromArgb(15, 15, 15);
                    }

                    // Panels
                    else if (control is Panel panel
                        && (panel.Tag == null || panel.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        panel.ForeColor = Color.White;
                        panel.BackColor = Color.FromArgb(15,15,15);
                    }

                    // Checkboxes
                    else if (control is TextBox textBox
                        && (textBox.Tag == null || textBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        textBox.ForeColor = Color.White;
                        textBox.BackColor = Color.FromArgb(15,15,15);
                    }

                    // Radio buttons
                    else if (control is RadioButton radioButton
                        && (radioButton.Tag == null || radioButton.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        radioButton.ForeColor = Color.White;
                        radioButton.BackColor = Color.FromArgb(15, 15, 15);
                    }
                }

                // Progress Panel Gif
                panel_Progress.BackColor = Color.Black; // Set to black so it appears in front of other controls
            }

            // Light Mode
            else
            {
                foreach (Control control in Controls)
                {
                    // Buttons
                    if (control is Button button
                        && (button.Tag == null || button.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        button.ForeColor = Color.Black;
                        button.BackColor = Color.White;
                    }

                    // Panels
                    else if (control is Panel panel
                        && (panel.Tag == null || panel.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        panel.ForeColor = Color.Black;
                        panel.BackColor = Color.White;
                    }

                    // Checkboxes
                    else if (control is TextBox textBox
                        && (textBox.Tag == null || textBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        textBox.ForeColor = Color.Black;
                        textBox.BackColor = Color.White; 
                    }

                    // Radio buttons
                    else if (control is RadioButton radioButton
                        && (radioButton.Tag == null || radioButton.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        radioButton.ForeColor = Color.Black;
                        radioButton.BackColor = Color.White; 
                    }
                }

                // Progress Panel Gif
                panel_Progress.BackColor = Color.White; 
            }
        }

        /// <summary>
        /// Sets the colors for the buttonBrowse Button which depends on the 
        /// state of Light and Dark Mode
        /// </summary>
        public void SetButtonUI(Button button)
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                button.FlatAppearance.BorderColor = Color.DodgerBlue;
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(50,30,144,255);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(50,30,144,255);
                button.FlatStyle = FlatStyle.Flat;
                button.ForeColor = Color.DodgerBlue;
                button.BackColor = Color.Transparent;
            }

            // Light Mode
            else
            {
                button.FlatAppearance.BorderColor = Color.Black;
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(50,30,144,255);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(50,30,144,255);
                button.FlatStyle = FlatStyle.Flat;
                button.ForeColor = Color.Black;
                button.BackColor = Color.WhiteSmoke;
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
                        textBox.BackColor = Color.FromArgb(15,15,15);
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
    }
}
