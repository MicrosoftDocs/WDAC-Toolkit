// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

namespace WDAC_Wizard
{
    partial class EditWorkflow
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.textBoxPolicyPath = new System.Windows.Forms.TextBox();
            this.policyInfoPanel = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonNewSaveLocation = new System.Windows.Forms.Button();
            this.textBox_PolicyName = new System.Windows.Forms.TextBox();
            this.textBoxSaveLocation = new System.Windows.Forms.TextBox();
            this.label_policyName = new System.Windows.Forms.Label();
            this.textBox_PolicyID = new System.Windows.Forms.TextBox();
            this.label_fileLocation = new System.Windows.Forms.Label();
            this.button_ParseEventLog = new System.Windows.Forms.Button();
            this.button_Parse_LogFile = new System.Windows.Forms.Button();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.panel_Progress = new System.Windows.Forms.Panel();
            this.pictureBox_Progress = new System.Windows.Forms.PictureBox();
            this.label_Progress = new System.Windows.Forms.Label();
            this.label_LearnMore = new System.Windows.Forms.Label();
            this.radioButton_EventConversion = new System.Windows.Forms.RadioButton();
            this.radioButton_EditXML = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.panel_Page = new System.Windows.Forms.Panel();
            this.panel_EventLog_Conversion = new System.Windows.Forms.Panel();
            this.button_Parse_MDE_AH_Logs = new System.Windows.Forms.Button();
            this.textBox_AdvancedHuntingPaths = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox_EventLog = new System.Windows.Forms.TextBox();
            this.eventLogParsing_Result_Panel = new System.Windows.Forms.Panel();
            this.ahParsingLearnMore_Label = new System.Windows.Forms.Label();
            this.parseresult_PictureBox = new System.Windows.Forms.PictureBox();
            this.parseResults_Label = new System.Windows.Forms.Label();
            this.textBox_EventLogFilePath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel_Edit_XML = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label_Error = new System.Windows.Forms.Label();
            this.policyInfoPanel.SuspendLayout();
            this.panel_Progress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Progress)).BeginInit();
            this.panel_Page.SuspendLayout();
            this.panel_EventLog_Conversion.SuspendLayout();
            this.eventLogParsing_Result_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.parseresult_PictureBox)).BeginInit();
            this.panel_Edit_XML.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(164, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(281, 29);
            this.label1.TabIndex = 48;
            this.label1.Text = "Edit Existing WDAC Policy";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(165, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(572, 21);
            this.label2.TabIndex = 108;
            this.label2.Text = "Browse for your policy on disk or create one from a code integrity event log.";
            // 
            // browseButton
            // 
            this.browseButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.MenuHighlight;
            this.browseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.browseButton.Font = new System.Drawing.Font("Tahoma", 9F);
            this.browseButton.ForeColor = System.Drawing.Color.DodgerBlue;
            this.browseButton.Location = new System.Drawing.Point(499, 21);
            this.browseButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(110, 28);
            this.browseButton.TabIndex = 109;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // textBoxPolicyPath
            // 
            this.textBoxPolicyPath.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBoxPolicyPath.Location = new System.Drawing.Point(17, 22);
            this.textBoxPolicyPath.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxPolicyPath.Name = "textBoxPolicyPath";
            this.textBoxPolicyPath.ReadOnly = true;
            this.textBoxPolicyPath.Size = new System.Drawing.Size(462, 26);
            this.textBoxPolicyPath.TabIndex = 1;
            // 
            // policyInfoPanel
            // 
            this.policyInfoPanel.Controls.Add(this.label7);
            this.policyInfoPanel.Controls.Add(this.label5);
            this.policyInfoPanel.Controls.Add(this.buttonNewSaveLocation);
            this.policyInfoPanel.Controls.Add(this.textBox_PolicyName);
            this.policyInfoPanel.Controls.Add(this.textBoxSaveLocation);
            this.policyInfoPanel.Controls.Add(this.label_policyName);
            this.policyInfoPanel.Controls.Add(this.textBox_PolicyID);
            this.policyInfoPanel.Controls.Add(this.label_fileLocation);
            this.policyInfoPanel.Location = new System.Drawing.Point(2, 64);
            this.policyInfoPanel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.policyInfoPanel.Name = "policyInfoPanel";
            this.policyInfoPanel.Size = new System.Drawing.Size(648, 189);
            this.policyInfoPanel.TabIndex = 111;
            this.policyInfoPanel.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(23, 134);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(222, 19);
            this.label7.TabIndex = 113;
            this.label7.Text = "New Save Location (optional):";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(16, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(338, 19);
            this.label5.TabIndex = 11;
            this.label5.Text = "Edit the policy name and ID, if you would like.\r\n";
            // 
            // buttonNewSaveLocation
            // 
            this.buttonNewSaveLocation.FlatAppearance.BorderColor = System.Drawing.SystemColors.MenuHighlight;
            this.buttonNewSaveLocation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonNewSaveLocation.Font = new System.Drawing.Font("Tahoma", 9F);
            this.buttonNewSaveLocation.ForeColor = System.Drawing.Color.DodgerBlue;
            this.buttonNewSaveLocation.Location = new System.Drawing.Point(502, 153);
            this.buttonNewSaveLocation.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonNewSaveLocation.Name = "buttonNewSaveLocation";
            this.buttonNewSaveLocation.Size = new System.Drawing.Size(110, 28);
            this.buttonNewSaveLocation.TabIndex = 114;
            this.buttonNewSaveLocation.Text = "Browse";
            this.buttonNewSaveLocation.UseVisualStyleBackColor = true;
            this.buttonNewSaveLocation.Click += new System.EventHandler(this.ButtonNewSaveLocation_Pressed);
            // 
            // textBox_PolicyName
            // 
            this.textBox_PolicyName.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_PolicyName.Location = new System.Drawing.Point(126, 42);
            this.textBox_PolicyName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_PolicyName.Name = "textBox_PolicyName";
            this.textBox_PolicyName.Size = new System.Drawing.Size(351, 26);
            this.textBox_PolicyName.TabIndex = 2;
            this.textBox_PolicyName.TextChanged += new System.EventHandler(this.TextBox_PolicyName_TextChanged);
            // 
            // textBoxSaveLocation
            // 
            this.textBoxSaveLocation.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBoxSaveLocation.Location = new System.Drawing.Point(20, 154);
            this.textBoxSaveLocation.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxSaveLocation.Name = "textBoxSaveLocation";
            this.textBoxSaveLocation.ReadOnly = true;
            this.textBoxSaveLocation.Size = new System.Drawing.Size(462, 26);
            this.textBoxSaveLocation.TabIndex = 112;
            this.textBoxSaveLocation.DoubleClick += new System.EventHandler(this.ButtonNewSaveLocation_Pressed);
            // 
            // label_policyName
            // 
            this.label_policyName.AutoSize = true;
            this.label_policyName.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label_policyName.ForeColor = System.Drawing.Color.Black;
            this.label_policyName.Location = new System.Drawing.Point(26, 44);
            this.label_policyName.Name = "label_policyName";
            this.label_policyName.Size = new System.Drawing.Size(92, 18);
            this.label_policyName.TabIndex = 8;
            this.label_policyName.Text = "Policy Name:";
            // 
            // textBox_PolicyID
            // 
            this.textBox_PolicyID.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_PolicyID.Location = new System.Drawing.Point(126, 77);
            this.textBox_PolicyID.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_PolicyID.Name = "textBox_PolicyID";
            this.textBox_PolicyID.Size = new System.Drawing.Size(351, 26);
            this.textBox_PolicyID.TabIndex = 3;
            this.textBox_PolicyID.TextChanged += new System.EventHandler(this.TextBox_PolicyID_TextChanged);
            // 
            // label_fileLocation
            // 
            this.label_fileLocation.AutoSize = true;
            this.label_fileLocation.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label_fileLocation.ForeColor = System.Drawing.Color.Black;
            this.label_fileLocation.Location = new System.Drawing.Point(26, 79);
            this.label_fileLocation.Name = "label_fileLocation";
            this.label_fileLocation.Size = new System.Drawing.Size(69, 18);
            this.label_fileLocation.TabIndex = 6;
            this.label_fileLocation.Text = "Policy ID:";
            // 
            // button_ParseEventLog
            // 
            this.button_ParseEventLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_ParseEventLog.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_ParseEventLog.Location = new System.Drawing.Point(343, 72);
            this.button_ParseEventLog.Name = "button_ParseEventLog";
            this.button_ParseEventLog.Size = new System.Drawing.Size(133, 27);
            this.button_ParseEventLog.TabIndex = 112;
            this.button_ParseEventLog.Text = "Parse Event Logs";
            this.button_ParseEventLog.UseVisualStyleBackColor = true;
            this.button_ParseEventLog.Click += new System.EventHandler(this.ParseSystemLog_ButtonClick);
            // 
            // button_Parse_LogFile
            // 
            this.button_Parse_LogFile.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.button_Parse_LogFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Parse_LogFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.button_Parse_LogFile.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_Parse_LogFile.Location = new System.Drawing.Point(343, 174);
            this.button_Parse_LogFile.Name = "button_Parse_LogFile";
            this.button_Parse_LogFile.Size = new System.Drawing.Size(133, 27);
            this.button_Parse_LogFile.TabIndex = 113;
            this.button_Parse_LogFile.Text = "Parse Log File(s)";
            this.button_Parse_LogFile.UseVisualStyleBackColor = true;
            this.button_Parse_LogFile.Click += new System.EventHandler(this.ParseLog_ButtonClick);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker_RunWorkerCompleted);
            // 
            // panel_Progress
            // 
            this.panel_Progress.Controls.Add(this.pictureBox_Progress);
            this.panel_Progress.Controls.Add(this.label_Progress);
            this.panel_Progress.Location = new System.Drawing.Point(533, 8);
            this.panel_Progress.Name = "panel_Progress";
            this.panel_Progress.Size = new System.Drawing.Size(280, 193);
            this.panel_Progress.TabIndex = 114;
            this.panel_Progress.Visible = false;
            // 
            // pictureBox_Progress
            // 
            this.pictureBox_Progress.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_Progress.Image = global::WDAC_Wizard.Properties.Resources.loading;
            this.pictureBox_Progress.InitialImage = global::WDAC_Wizard.Properties.Resources.loading;
            this.pictureBox_Progress.Location = new System.Drawing.Point(76, 48);
            this.pictureBox_Progress.Name = "pictureBox_Progress";
            this.pictureBox_Progress.Size = new System.Drawing.Size(128, 128);
            this.pictureBox_Progress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_Progress.TabIndex = 0;
            this.pictureBox_Progress.TabStop = false;
            this.pictureBox_Progress.Tag = "IgnoreDarkMode";
            // 
            // label_Progress
            // 
            this.label_Progress.AutoSize = true;
            this.label_Progress.BackColor = System.Drawing.Color.Transparent;
            this.label_Progress.Location = new System.Drawing.Point(15, 18);
            this.label_Progress.Name = "label_Progress";
            this.label_Progress.Size = new System.Drawing.Size(250, 17);
            this.label_Progress.TabIndex = 1;
            this.label_Progress.Tag = "IgnoreDarkMode";
            this.label_Progress.Text = "Parsing Rules from Event Log Created";
            this.label_Progress.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label_LearnMore
            // 
            this.label_LearnMore.AutoSize = true;
            this.label_LearnMore.BackColor = System.Drawing.Color.Transparent;
            this.label_LearnMore.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label_LearnMore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.label_LearnMore.Image = global::WDAC_Wizard.Properties.Resources.external_link_symbol_highlight;
            this.label_LearnMore.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label_LearnMore.Location = new System.Drawing.Point(166, 216);
            this.label_LearnMore.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_LearnMore.Name = "label_LearnMore";
            this.label_LearnMore.Size = new System.Drawing.Size(289, 18);
            this.label_LearnMore.TabIndex = 107;
            this.label_LearnMore.Tag = "IgnoreDarkMode";
            this.label_LearnMore.Text = "Learn more about event log conversion     ";
            this.label_LearnMore.Click += new System.EventHandler(this.Label_LearnMore_Click);
            // 
            // radioButton_EventConversion
            // 
            this.radioButton_EventConversion.AutoSize = true;
            this.radioButton_EventConversion.Font = new System.Drawing.Font("Tahoma", 10F);
            this.radioButton_EventConversion.Location = new System.Drawing.Point(169, 165);
            this.radioButton_EventConversion.Name = "radioButton_EventConversion";
            this.radioButton_EventConversion.Size = new System.Drawing.Size(300, 25);
            this.radioButton_EventConversion.TabIndex = 111;
            this.radioButton_EventConversion.Text = "Convert Event Log to a WDAC Policy";
            this.radioButton_EventConversion.UseVisualStyleBackColor = true;
            this.radioButton_EventConversion.Click += new System.EventHandler(this.EventConversion_RadioButton_Click);
            // 
            // radioButton_EditXML
            // 
            this.radioButton_EditXML.AutoSize = true;
            this.radioButton_EditXML.Checked = true;
            this.radioButton_EditXML.Font = new System.Drawing.Font("Tahoma", 10F);
            this.radioButton_EditXML.Location = new System.Drawing.Point(169, 121);
            this.radioButton_EditXML.Name = "radioButton_EditXML";
            this.radioButton_EditXML.Size = new System.Drawing.Size(174, 25);
            this.radioButton_EditXML.TabIndex = 110;
            this.radioButton_EditXML.TabStop = true;
            this.radioButton_EditXML.Text = "Edit Policy XML File";
            this.radioButton_EditXML.UseVisualStyleBackColor = true;
            this.radioButton_EditXML.Click += new System.EventHandler(this.EditXML_RadioButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label9.Location = new System.Drawing.Point(166, 193);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(620, 18);
            this.label9.TabIndex = 109;
            this.label9.Text = "Convert the device\'s Code Integrity event log or an arbitrary log file to a WDAC " +
    "policy XML file";
            // 
            // panel_Page
            // 
            this.panel_Page.Controls.Add(this.panel_EventLog_Conversion);
            this.panel_Page.Controls.Add(this.panel_Edit_XML);
            this.panel_Page.Location = new System.Drawing.Point(156, 254);
            this.panel_Page.Name = "panel_Page";
            this.panel_Page.Size = new System.Drawing.Size(950, 637);
            this.panel_Page.TabIndex = 115;
            // 
            // panel_EventLog_Conversion
            // 
            this.panel_EventLog_Conversion.Controls.Add(this.panel_Progress);
            this.panel_EventLog_Conversion.Controls.Add(this.button_Parse_MDE_AH_Logs);
            this.panel_EventLog_Conversion.Controls.Add(this.textBox_AdvancedHuntingPaths);
            this.panel_EventLog_Conversion.Controls.Add(this.label8);
            this.panel_EventLog_Conversion.Controls.Add(this.textBox_EventLog);
            this.panel_EventLog_Conversion.Controls.Add(this.eventLogParsing_Result_Panel);
            this.panel_EventLog_Conversion.Controls.Add(this.button_Parse_LogFile);
            this.panel_EventLog_Conversion.Controls.Add(this.textBox_EventLogFilePath);
            this.panel_EventLog_Conversion.Controls.Add(this.label4);
            this.panel_EventLog_Conversion.Controls.Add(this.label6);
            this.panel_EventLog_Conversion.Controls.Add(this.button_ParseEventLog);
            this.panel_EventLog_Conversion.Location = new System.Drawing.Point(3, 265);
            this.panel_EventLog_Conversion.Name = "panel_EventLog_Conversion";
            this.panel_EventLog_Conversion.Size = new System.Drawing.Size(856, 388);
            this.panel_EventLog_Conversion.TabIndex = 1;
            this.panel_EventLog_Conversion.Visible = false;
            // 
            // button_Parse_MDE_AH_Logs
            // 
            this.button_Parse_MDE_AH_Logs.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.button_Parse_MDE_AH_Logs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Parse_MDE_AH_Logs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.button_Parse_MDE_AH_Logs.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_Parse_MDE_AH_Logs.Location = new System.Drawing.Point(342, 273);
            this.button_Parse_MDE_AH_Logs.Name = "button_Parse_MDE_AH_Logs";
            this.button_Parse_MDE_AH_Logs.Size = new System.Drawing.Size(133, 27);
            this.button_Parse_MDE_AH_Logs.TabIndex = 124;
            this.button_Parse_MDE_AH_Logs.Text = "Parse Log File(s)";
            this.button_Parse_MDE_AH_Logs.UseVisualStyleBackColor = true;
            this.button_Parse_MDE_AH_Logs.Click += new System.EventHandler(this.ParseMDEAHLogs_ButtonClick);
            // 
            // textBox_AdvancedHuntingPaths
            // 
            this.textBox_AdvancedHuntingPaths.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_AdvancedHuntingPaths.Location = new System.Drawing.Point(22, 232);
            this.textBox_AdvancedHuntingPaths.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_AdvancedHuntingPaths.Multiline = true;
            this.textBox_AdvancedHuntingPaths.Name = "textBox_AdvancedHuntingPaths";
            this.textBox_AdvancedHuntingPaths.ReadOnly = true;
            this.textBox_AdvancedHuntingPaths.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_AdvancedHuntingPaths.Size = new System.Drawing.Size(453, 37);
            this.textBox_AdvancedHuntingPaths.TabIndex = 126;
            this.textBox_AdvancedHuntingPaths.Text = "Select MDE Advanced Hunting CSV Files";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(23, 210);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(349, 18);
            this.label8.TabIndex = 125;
            this.label8.Text = "Parse MDE Advanced Hunting Events to Policy";
            // 
            // textBox_EventLog
            // 
            this.textBox_EventLog.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_EventLog.Location = new System.Drawing.Point(23, 27);
            this.textBox_EventLog.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_EventLog.Multiline = true;
            this.textBox_EventLog.Name = "textBox_EventLog";
            this.textBox_EventLog.ReadOnly = true;
            this.textBox_EventLog.Size = new System.Drawing.Size(453, 43);
            this.textBox_EventLog.TabIndex = 123;
            this.textBox_EventLog.Text = "Microsoft-Windows-CodeIntegrity/Operational\r\nMicrosoft-Windows-Applocker/MSI and " +
    "Script";
            // 
            // eventLogParsing_Result_Panel
            // 
            this.eventLogParsing_Result_Panel.Controls.Add(this.ahParsingLearnMore_Label);
            this.eventLogParsing_Result_Panel.Controls.Add(this.parseresult_PictureBox);
            this.eventLogParsing_Result_Panel.Controls.Add(this.parseResults_Label);
            this.eventLogParsing_Result_Panel.Location = new System.Drawing.Point(17, 305);
            this.eventLogParsing_Result_Panel.Margin = new System.Windows.Forms.Padding(2);
            this.eventLogParsing_Result_Panel.Name = "eventLogParsing_Result_Panel";
            this.eventLogParsing_Result_Panel.Size = new System.Drawing.Size(817, 64);
            this.eventLogParsing_Result_Panel.TabIndex = 122;
            this.eventLogParsing_Result_Panel.Visible = false;
            // 
            // ahParsingLearnMore_Label
            // 
            this.ahParsingLearnMore_Label.AutoSize = true;
            this.ahParsingLearnMore_Label.BackColor = System.Drawing.Color.Transparent;
            this.ahParsingLearnMore_Label.Font = new System.Drawing.Font("Tahoma", 9F);
            this.ahParsingLearnMore_Label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.ahParsingLearnMore_Label.Image = global::WDAC_Wizard.Properties.Resources.external_link_symbol_highlight;
            this.ahParsingLearnMore_Label.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ahParsingLearnMore_Label.Location = new System.Drawing.Point(4, 41);
            this.ahParsingLearnMore_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ahParsingLearnMore_Label.Name = "ahParsingLearnMore_Label";
            this.ahParsingLearnMore_Label.Size = new System.Drawing.Size(349, 18);
            this.ahParsingLearnMore_Label.TabIndex = 127;
            this.ahParsingLearnMore_Label.Tag = "IgnoreDarkMode";
            this.ahParsingLearnMore_Label.Text = "Learn more about parsing Advanced Hunting logs    ";
            this.ahParsingLearnMore_Label.Visible = false;
            this.ahParsingLearnMore_Label.Click += new System.EventHandler(this.AHLearnMoreLabel_Click);
            // 
            // parseresult_PictureBox
            // 
            this.parseresult_PictureBox.Image = global::WDAC_Wizard.Properties.Resources.verified;
            this.parseresult_PictureBox.Location = new System.Drawing.Point(4, 5);
            this.parseresult_PictureBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.parseresult_PictureBox.Name = "parseresult_PictureBox";
            this.parseresult_PictureBox.Size = new System.Drawing.Size(23, 26);
            this.parseresult_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.parseresult_PictureBox.TabIndex = 94;
            this.parseresult_PictureBox.TabStop = false;
            // 
            // parseResults_Label
            // 
            this.parseResults_Label.AutoSize = true;
            this.parseResults_Label.Font = new System.Drawing.Font("Tahoma", 9F);
            this.parseResults_Label.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.parseResults_Label.Location = new System.Drawing.Point(37, 8);
            this.parseResults_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.parseResults_Label.Name = "parseResults_Label";
            this.parseResults_Label.Size = new System.Drawing.Size(223, 18);
            this.parseResults_Label.TabIndex = 16;
            this.parseResults_Label.Text = "Policy conversion was successful.";
            // 
            // textBox_EventLogFilePath
            // 
            this.textBox_EventLogFilePath.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_EventLogFilePath.Location = new System.Drawing.Point(23, 133);
            this.textBox_EventLogFilePath.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_EventLogFilePath.Multiline = true;
            this.textBox_EventLogFilePath.Name = "textBox_EventLogFilePath";
            this.textBox_EventLogFilePath.ReadOnly = true;
            this.textBox_EventLogFilePath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_EventLogFilePath.Size = new System.Drawing.Size(453, 37);
            this.textBox_EventLogFilePath.TabIndex = 118;
            this.textBox_EventLogFilePath.Text = "Select Event Log Files";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(23, 111);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(274, 18);
            this.label4.TabIndex = 117;
            this.label4.Text = "Parse Event Log evtx Files to Policy";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(23, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(432, 18);
            this.label6.TabIndex = 116;
            this.label6.Text = "Parse Event Logs from the system Event Viewer to Policy";
            // 
            // panel_Edit_XML
            // 
            this.panel_Edit_XML.Controls.Add(this.label3);
            this.panel_Edit_XML.Controls.Add(this.policyInfoPanel);
            this.panel_Edit_XML.Controls.Add(this.browseButton);
            this.panel_Edit_XML.Controls.Add(this.textBoxPolicyPath);
            this.panel_Edit_XML.Location = new System.Drawing.Point(3, 3);
            this.panel_Edit_XML.Name = "panel_Edit_XML";
            this.panel_Edit_XML.Size = new System.Drawing.Size(857, 256);
            this.panel_Edit_XML.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(20, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(143, 19);
            this.label3.TabIndex = 12;
            this.label3.Text = "Policy Path to Edit:";
            // 
            // label_Error
            // 
            this.label_Error.AutoSize = true;
            this.label_Error.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Error.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label_Error.Location = new System.Drawing.Point(163, 640);
            this.label_Error.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Error.Name = "label_Error";
            this.label_Error.Size = new System.Drawing.Size(620, 18);
            this.label_Error.TabIndex = 116;
            this.label_Error.Text = "Convert the device\'s Code Integrity event log or an arbitrary log file to a WDAC " +
    "policy XML file";
            this.label_Error.Visible = false;
            // 
            // EditWorkflow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label_LearnMore);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.radioButton_EventConversion);
            this.Controls.Add(this.radioButton_EditXML);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel_Page);
            this.Controls.Add(this.label_Error);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "EditWorkflow";
            this.Size = new System.Drawing.Size(1208, 894);
            this.Validated += new System.EventHandler(this.EditWorkflow_Validated);
            this.policyInfoPanel.ResumeLayout(false);
            this.policyInfoPanel.PerformLayout();
            this.panel_Progress.ResumeLayout(false);
            this.panel_Progress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Progress)).EndInit();
            this.panel_Page.ResumeLayout(false);
            this.panel_EventLog_Conversion.ResumeLayout(false);
            this.panel_EventLog_Conversion.PerformLayout();
            this.eventLogParsing_Result_Panel.ResumeLayout(false);
            this.eventLogParsing_Result_Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.parseresult_PictureBox)).EndInit();
            this.panel_Edit_XML.ResumeLayout(false);
            this.panel_Edit_XML.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox textBoxPolicyPath;
        private System.Windows.Forms.Panel policyInfoPanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_PolicyName;
        private System.Windows.Forms.Label label_policyName;
        private System.Windows.Forms.TextBox textBox_PolicyID;
        private System.Windows.Forms.Label label_fileLocation;
        private System.Windows.Forms.Button button_ParseEventLog;
        private System.Windows.Forms.Button button_Parse_LogFile;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.Panel panel_Progress;
        private System.Windows.Forms.Label label_Progress;
        private System.Windows.Forms.PictureBox pictureBox_Progress;
        private System.Windows.Forms.Label label_LearnMore;
        private System.Windows.Forms.RadioButton radioButton_EventConversion;
        private System.Windows.Forms.RadioButton radioButton_EditXML;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel_Page;
        private System.Windows.Forms.Panel panel_EventLog_Conversion;
        private System.Windows.Forms.Panel panel_Edit_XML;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_EventLogFilePath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label_Error;
        private System.Windows.Forms.Panel eventLogParsing_Result_Panel;
        private System.Windows.Forms.PictureBox parseresult_PictureBox;
        private System.Windows.Forms.Label parseResults_Label;
        private System.Windows.Forms.TextBox textBox_EventLog;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonNewSaveLocation;
        private System.Windows.Forms.TextBox textBoxSaveLocation;
        private System.Windows.Forms.Button button_Parse_MDE_AH_Logs;
        private System.Windows.Forms.TextBox textBox_AdvancedHuntingPaths;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label ahParsingLearnMore_Label;
    }
}
