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
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            buttonBrowse = new System.Windows.Forms.Button();
            textBoxPolicyPath = new System.Windows.Forms.TextBox();
            policyInfoPanel = new System.Windows.Forms.Panel();
            label7 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            buttonNewSaveLocation = new System.Windows.Forms.Button();
            textBox_PolicyName = new System.Windows.Forms.TextBox();
            textBoxSaveLocation = new System.Windows.Forms.TextBox();
            label_policyName = new System.Windows.Forms.Label();
            textBox_PolicyID = new System.Windows.Forms.TextBox();
            label_fileLocation = new System.Windows.Forms.Label();
            buttonParseEventLog = new System.Windows.Forms.Button();
            buttonParseLogFile = new System.Windows.Forms.Button();
            backgroundWorker = new System.ComponentModel.BackgroundWorker();
            panel_Progress = new System.Windows.Forms.Panel();
            pictureBox_Progress = new System.Windows.Forms.PictureBox();
            label_Progress = new System.Windows.Forms.Label();
            label_LearnMore = new System.Windows.Forms.Label();
            radioButton_EventConversion = new System.Windows.Forms.RadioButton();
            radioButton_EditXML = new System.Windows.Forms.RadioButton();
            label9 = new System.Windows.Forms.Label();
            panel_Page = new System.Windows.Forms.Panel();
            panel_EventLog_Conversion = new System.Windows.Forms.Panel();
            buttonParseMDELog = new System.Windows.Forms.Button();
            textBox_AdvancedHuntingPaths = new System.Windows.Forms.TextBox();
            label8 = new System.Windows.Forms.Label();
            textBox_EventLog = new System.Windows.Forms.TextBox();
            eventLogParsing_Result_Panel = new System.Windows.Forms.Panel();
            ahParsingLearnMore_Label = new System.Windows.Forms.Label();
            parseresult_PictureBox = new System.Windows.Forms.PictureBox();
            parseResults_Label = new System.Windows.Forms.Label();
            textBox_EventLogFilePath = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            panel_Edit_XML = new System.Windows.Forms.Panel();
            label3 = new System.Windows.Forms.Label();
            label_Error = new System.Windows.Forms.Label();
            policyInfoPanel.SuspendLayout();
            panel_Progress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox_Progress).BeginInit();
            panel_Page.SuspendLayout();
            panel_EventLog_Conversion.SuspendLayout();
            eventLogParsing_Result_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)parseresult_PictureBox).BeginInit();
            panel_Edit_XML.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Tahoma", 14F);
            label1.ForeColor = System.Drawing.Color.Black;
            label1.Location = new System.Drawing.Point(164, 37);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(462, 29);
            label1.TabIndex = 48;
            label1.Text = "Edit an Existing Policy or Parse Event Logs";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label2.ForeColor = System.Drawing.Color.Black;
            label2.Location = new System.Drawing.Point(165, 76);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(572, 21);
            label2.TabIndex = 108;
            label2.Text = "Browse for your policy on disk or create one from a code integrity event log.";
            // 
            // buttonBrowse
            // 
            buttonBrowse.Font = new System.Drawing.Font("Tahoma", 9F);
            buttonBrowse.Location = new System.Drawing.Point(499, 21);
            buttonBrowse.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            buttonBrowse.Name = "buttonBrowse";
            buttonBrowse.Size = new System.Drawing.Size(110, 28);
            buttonBrowse.TabIndex = 109;
            buttonBrowse.Text = "Browse";
            buttonBrowse.UseVisualStyleBackColor = true;
            buttonBrowse.Click += buttonBrowse_Click;
            // 
            // textBoxPolicyPath
            // 
            textBoxPolicyPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxPolicyPath.Font = new System.Drawing.Font("Tahoma", 9F);
            textBoxPolicyPath.Location = new System.Drawing.Point(17, 22);
            textBoxPolicyPath.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            textBoxPolicyPath.Name = "textBoxPolicyPath";
            textBoxPolicyPath.ReadOnly = true;
            textBoxPolicyPath.Size = new System.Drawing.Size(462, 26);
            textBoxPolicyPath.TabIndex = 1;
            // 
            // policyInfoPanel
            // 
            policyInfoPanel.Controls.Add(label7);
            policyInfoPanel.Controls.Add(label5);
            policyInfoPanel.Controls.Add(buttonNewSaveLocation);
            policyInfoPanel.Controls.Add(textBox_PolicyName);
            policyInfoPanel.Controls.Add(textBoxSaveLocation);
            policyInfoPanel.Controls.Add(label_policyName);
            policyInfoPanel.Controls.Add(textBox_PolicyID);
            policyInfoPanel.Controls.Add(label_fileLocation);
            policyInfoPanel.Location = new System.Drawing.Point(2, 64);
            policyInfoPanel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            policyInfoPanel.Name = "policyInfoPanel";
            policyInfoPanel.Size = new System.Drawing.Size(648, 189);
            policyInfoPanel.TabIndex = 111;
            policyInfoPanel.Visible = false;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Tahoma", 9.5F);
            label7.ForeColor = System.Drawing.Color.Black;
            label7.Location = new System.Drawing.Point(23, 134);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(222, 19);
            label7.TabIndex = 113;
            label7.Text = "New Save Location (optional):";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Tahoma", 9.5F);
            label5.ForeColor = System.Drawing.Color.Black;
            label5.Location = new System.Drawing.Point(16, 7);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(338, 19);
            label5.TabIndex = 11;
            label5.Text = "Edit the policy name and ID, if you would like.\r\n";
            // 
            // buttonNewSaveLocation
            // 
            buttonNewSaveLocation.Font = new System.Drawing.Font("Tahoma", 9F);
            buttonNewSaveLocation.Location = new System.Drawing.Point(502, 153);
            buttonNewSaveLocation.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            buttonNewSaveLocation.Name = "buttonNewSaveLocation";
            buttonNewSaveLocation.Size = new System.Drawing.Size(110, 28);
            buttonNewSaveLocation.TabIndex = 114;
            buttonNewSaveLocation.Text = "Browse";
            buttonNewSaveLocation.UseVisualStyleBackColor = true;
            buttonNewSaveLocation.Click += ButtonNewSaveLocation_Pressed;
            // 
            // textBox_PolicyName
            // 
            textBox_PolicyName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox_PolicyName.Font = new System.Drawing.Font("Tahoma", 9F);
            textBox_PolicyName.Location = new System.Drawing.Point(126, 42);
            textBox_PolicyName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            textBox_PolicyName.Name = "textBox_PolicyName";
            textBox_PolicyName.Size = new System.Drawing.Size(351, 26);
            textBox_PolicyName.TabIndex = 2;
            textBox_PolicyName.TextChanged += TextBox_PolicyName_TextChanged;
            // 
            // textBoxSaveLocation
            // 
            textBoxSaveLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxSaveLocation.Font = new System.Drawing.Font("Tahoma", 9F);
            textBoxSaveLocation.Location = new System.Drawing.Point(20, 154);
            textBoxSaveLocation.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            textBoxSaveLocation.Name = "textBoxSaveLocation";
            textBoxSaveLocation.ReadOnly = true;
            textBoxSaveLocation.Size = new System.Drawing.Size(462, 26);
            textBoxSaveLocation.TabIndex = 112;
            textBoxSaveLocation.DoubleClick += ButtonNewSaveLocation_Pressed;
            // 
            // label_policyName
            // 
            label_policyName.AutoSize = true;
            label_policyName.Font = new System.Drawing.Font("Tahoma", 9F);
            label_policyName.ForeColor = System.Drawing.Color.Black;
            label_policyName.Location = new System.Drawing.Point(26, 44);
            label_policyName.Name = "label_policyName";
            label_policyName.Size = new System.Drawing.Size(92, 18);
            label_policyName.TabIndex = 8;
            label_policyName.Text = "Policy Name:";
            // 
            // textBox_PolicyID
            // 
            textBox_PolicyID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox_PolicyID.Font = new System.Drawing.Font("Tahoma", 9F);
            textBox_PolicyID.Location = new System.Drawing.Point(126, 77);
            textBox_PolicyID.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            textBox_PolicyID.Name = "textBox_PolicyID";
            textBox_PolicyID.Size = new System.Drawing.Size(351, 26);
            textBox_PolicyID.TabIndex = 3;
            textBox_PolicyID.TextChanged += TextBox_PolicyID_TextChanged;
            // 
            // label_fileLocation
            // 
            label_fileLocation.AutoSize = true;
            label_fileLocation.Font = new System.Drawing.Font("Tahoma", 9F);
            label_fileLocation.ForeColor = System.Drawing.Color.Black;
            label_fileLocation.Location = new System.Drawing.Point(26, 79);
            label_fileLocation.Name = "label_fileLocation";
            label_fileLocation.Size = new System.Drawing.Size(69, 18);
            label_fileLocation.TabIndex = 6;
            label_fileLocation.Text = "Policy ID:";
            // 
            // buttonParseEventLog
            // 
            buttonParseEventLog.Location = new System.Drawing.Point(343, 72);
            buttonParseEventLog.Name = "buttonParseEventLog";
            buttonParseEventLog.Size = new System.Drawing.Size(133, 27);
            buttonParseEventLog.TabIndex = 112;
            buttonParseEventLog.Text = "Parse Event Logs";
            buttonParseEventLog.UseVisualStyleBackColor = true;
            buttonParseEventLog.Click += ParseSystemLog_ButtonClick;
            // 
            // buttonParseLogFile
            // 
            buttonParseLogFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            buttonParseLogFile.Location = new System.Drawing.Point(343, 174);
            buttonParseLogFile.Name = "buttonParseLogFile";
            buttonParseLogFile.Size = new System.Drawing.Size(133, 27);
            buttonParseLogFile.TabIndex = 113;
            buttonParseLogFile.Text = "Parse Log File(s)";
            buttonParseLogFile.UseVisualStyleBackColor = true;
            buttonParseLogFile.Click += ParseLog_ButtonClick;
            // 
            // backgroundWorker
            // 
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            // 
            // panel_Progress
            // 
            panel_Progress.Controls.Add(pictureBox_Progress);
            panel_Progress.Controls.Add(label_Progress);
            panel_Progress.Location = new System.Drawing.Point(533, 8);
            panel_Progress.Name = "panel_Progress";
            panel_Progress.Size = new System.Drawing.Size(280, 193);
            panel_Progress.TabIndex = 114;
            panel_Progress.Visible = false;
            // 
            // pictureBox_Progress
            // 
            pictureBox_Progress.BackColor = System.Drawing.Color.Transparent;
            pictureBox_Progress.Image = Properties.Resources.loading;
            pictureBox_Progress.InitialImage = Properties.Resources.loading;
            pictureBox_Progress.Location = new System.Drawing.Point(76, 48);
            pictureBox_Progress.Name = "pictureBox_Progress";
            pictureBox_Progress.Size = new System.Drawing.Size(128, 128);
            pictureBox_Progress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            pictureBox_Progress.TabIndex = 0;
            pictureBox_Progress.TabStop = false;
            pictureBox_Progress.Tag = "IgnoreDarkMode";
            // 
            // label_Progress
            // 
            label_Progress.AutoSize = true;
            label_Progress.BackColor = System.Drawing.Color.Transparent;
            label_Progress.Location = new System.Drawing.Point(15, 18);
            label_Progress.Name = "label_Progress";
            label_Progress.Size = new System.Drawing.Size(256, 20);
            label_Progress.TabIndex = 1;
            label_Progress.Tag = "IgnoreDarkMode";
            label_Progress.Text = "Parsing Rules from Event Log Created";
            label_Progress.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label_LearnMore
            // 
            label_LearnMore.AutoSize = true;
            label_LearnMore.BackColor = System.Drawing.Color.Transparent;
            label_LearnMore.Font = new System.Drawing.Font("Tahoma", 9F);
            label_LearnMore.ForeColor = System.Drawing.Color.FromArgb(16, 110, 190);
            label_LearnMore.Image = Properties.Resources.external_link_symbol_highlight;
            label_LearnMore.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            label_LearnMore.Location = new System.Drawing.Point(166, 216);
            label_LearnMore.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label_LearnMore.Name = "label_LearnMore";
            label_LearnMore.Size = new System.Drawing.Size(289, 18);
            label_LearnMore.TabIndex = 107;
            label_LearnMore.Tag = "IgnoreDarkMode";
            label_LearnMore.Text = "Learn more about event log conversion     ";
            label_LearnMore.Click += Label_LearnMore_Click;
            // 
            // radioButton_EventConversion
            // 
            radioButton_EventConversion.AutoSize = true;
            radioButton_EventConversion.Font = new System.Drawing.Font("Tahoma", 10F);
            radioButton_EventConversion.Location = new System.Drawing.Point(169, 165);
            radioButton_EventConversion.Name = "radioButton_EventConversion";
            radioButton_EventConversion.Size = new System.Drawing.Size(255, 25);
            radioButton_EventConversion.TabIndex = 111;
            radioButton_EventConversion.Text = "Convert Event Logs to a Policy";
            radioButton_EventConversion.UseVisualStyleBackColor = true;
            radioButton_EventConversion.Click += EventConversion_RadioButton_Click;
            // 
            // radioButton_EditXML
            // 
            radioButton_EditXML.AutoSize = true;
            radioButton_EditXML.Checked = true;
            radioButton_EditXML.Font = new System.Drawing.Font("Tahoma", 10F);
            radioButton_EditXML.Location = new System.Drawing.Point(169, 121);
            radioButton_EditXML.Name = "radioButton_EditXML";
            radioButton_EditXML.Size = new System.Drawing.Size(174, 25);
            radioButton_EditXML.TabIndex = 110;
            radioButton_EditXML.TabStop = true;
            radioButton_EditXML.Text = "Edit Policy XML File";
            radioButton_EditXML.UseVisualStyleBackColor = true;
            radioButton_EditXML.Click += EditXML_RadioButton_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label9.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label9.Location = new System.Drawing.Point(166, 193);
            label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(526, 18);
            label9.TabIndex = 109;
            label9.Text = "Convert the app control event logs or MDE Advanced Hunting log file to a policy";
            // 
            // panel_Page
            // 
            panel_Page.Controls.Add(panel_EventLog_Conversion);
            panel_Page.Controls.Add(panel_Edit_XML);
            panel_Page.Location = new System.Drawing.Point(156, 254);
            panel_Page.Name = "panel_Page";
            panel_Page.Size = new System.Drawing.Size(950, 637);
            panel_Page.TabIndex = 115;
            // 
            // panel_EventLog_Conversion
            // 
            panel_EventLog_Conversion.Controls.Add(panel_Progress);
            panel_EventLog_Conversion.Controls.Add(buttonParseMDELog);
            panel_EventLog_Conversion.Controls.Add(textBox_AdvancedHuntingPaths);
            panel_EventLog_Conversion.Controls.Add(label8);
            panel_EventLog_Conversion.Controls.Add(textBox_EventLog);
            panel_EventLog_Conversion.Controls.Add(eventLogParsing_Result_Panel);
            panel_EventLog_Conversion.Controls.Add(buttonParseLogFile);
            panel_EventLog_Conversion.Controls.Add(textBox_EventLogFilePath);
            panel_EventLog_Conversion.Controls.Add(label4);
            panel_EventLog_Conversion.Controls.Add(label6);
            panel_EventLog_Conversion.Controls.Add(buttonParseEventLog);
            panel_EventLog_Conversion.Location = new System.Drawing.Point(3, 265);
            panel_EventLog_Conversion.Name = "panel_EventLog_Conversion";
            panel_EventLog_Conversion.Size = new System.Drawing.Size(856, 388);
            panel_EventLog_Conversion.TabIndex = 1;
            panel_EventLog_Conversion.Visible = false;
            // 
            // buttonParseMDELog
            // 
            buttonParseMDELog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            buttonParseMDELog.Location = new System.Drawing.Point(342, 273);
            buttonParseMDELog.Name = "buttonParseMDELog";
            buttonParseMDELog.Size = new System.Drawing.Size(133, 27);
            buttonParseMDELog.TabIndex = 124;
            buttonParseMDELog.Text = "Parse Log File(s)";
            buttonParseMDELog.UseVisualStyleBackColor = true;
            buttonParseMDELog.Click += ParseMDEAHLogs_ButtonClick;
            // 
            // textBox_AdvancedHuntingPaths
            // 
            textBox_AdvancedHuntingPaths.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox_AdvancedHuntingPaths.Font = new System.Drawing.Font("Tahoma", 9F);
            textBox_AdvancedHuntingPaths.Location = new System.Drawing.Point(22, 232);
            textBox_AdvancedHuntingPaths.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            textBox_AdvancedHuntingPaths.Multiline = true;
            textBox_AdvancedHuntingPaths.Name = "textBox_AdvancedHuntingPaths";
            textBox_AdvancedHuntingPaths.ReadOnly = true;
            textBox_AdvancedHuntingPaths.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            textBox_AdvancedHuntingPaths.Size = new System.Drawing.Size(453, 37);
            textBox_AdvancedHuntingPaths.TabIndex = 126;
            textBox_AdvancedHuntingPaths.Text = "Select MDE Advanced Hunting and/or Log Analytics CSV Files";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            label8.ForeColor = System.Drawing.Color.Black;
            label8.Location = new System.Drawing.Point(23, 210);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(447, 18);
            label8.TabIndex = 125;
            label8.Text = "Parse Advanced Hunting and Log Analytics Events to Policy";
            // 
            // textBox_EventLog
            // 
            textBox_EventLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox_EventLog.Font = new System.Drawing.Font("Tahoma", 9F);
            textBox_EventLog.Location = new System.Drawing.Point(23, 27);
            textBox_EventLog.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            textBox_EventLog.Multiline = true;
            textBox_EventLog.Name = "textBox_EventLog";
            textBox_EventLog.ReadOnly = true;
            textBox_EventLog.Size = new System.Drawing.Size(453, 43);
            textBox_EventLog.TabIndex = 123;
            textBox_EventLog.Text = "Microsoft-Windows-CodeIntegrity/Operational\r\nMicrosoft-Windows-Applocker/MSI and Script";
            // 
            // eventLogParsing_Result_Panel
            // 
            eventLogParsing_Result_Panel.Controls.Add(ahParsingLearnMore_Label);
            eventLogParsing_Result_Panel.Controls.Add(parseresult_PictureBox);
            eventLogParsing_Result_Panel.Controls.Add(parseResults_Label);
            eventLogParsing_Result_Panel.Location = new System.Drawing.Point(17, 305);
            eventLogParsing_Result_Panel.Margin = new System.Windows.Forms.Padding(2);
            eventLogParsing_Result_Panel.Name = "eventLogParsing_Result_Panel";
            eventLogParsing_Result_Panel.Size = new System.Drawing.Size(817, 64);
            eventLogParsing_Result_Panel.TabIndex = 122;
            eventLogParsing_Result_Panel.Visible = false;
            // 
            // ahParsingLearnMore_Label
            // 
            ahParsingLearnMore_Label.AutoSize = true;
            ahParsingLearnMore_Label.BackColor = System.Drawing.Color.Transparent;
            ahParsingLearnMore_Label.Font = new System.Drawing.Font("Tahoma", 9F);
            ahParsingLearnMore_Label.ForeColor = System.Drawing.Color.FromArgb(16, 110, 190);
            ahParsingLearnMore_Label.Image = Properties.Resources.external_link_symbol_highlight;
            ahParsingLearnMore_Label.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            ahParsingLearnMore_Label.Location = new System.Drawing.Point(4, 41);
            ahParsingLearnMore_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            ahParsingLearnMore_Label.Name = "ahParsingLearnMore_Label";
            ahParsingLearnMore_Label.Size = new System.Drawing.Size(467, 18);
            ahParsingLearnMore_Label.TabIndex = 127;
            ahParsingLearnMore_Label.Tag = "IgnoreDarkMode";
            ahParsingLearnMore_Label.Text = "Learn more about parsing Advanced Hunting and Log Analytics logs    ";
            ahParsingLearnMore_Label.Visible = false;
            ahParsingLearnMore_Label.Click += AHLearnMoreLabel_Click;
            // 
            // parseresult_PictureBox
            // 
            parseresult_PictureBox.Image = Properties.Resources.verified;
            parseresult_PictureBox.Location = new System.Drawing.Point(4, 5);
            parseresult_PictureBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            parseresult_PictureBox.Name = "parseresult_PictureBox";
            parseresult_PictureBox.Size = new System.Drawing.Size(23, 26);
            parseresult_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            parseresult_PictureBox.TabIndex = 94;
            parseresult_PictureBox.TabStop = false;
            // 
            // parseResults_Label
            // 
            parseResults_Label.AutoSize = true;
            parseResults_Label.Font = new System.Drawing.Font("Tahoma", 9F);
            parseResults_Label.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            parseResults_Label.Location = new System.Drawing.Point(37, 8);
            parseResults_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            parseResults_Label.Name = "parseResults_Label";
            parseResults_Label.Size = new System.Drawing.Size(223, 18);
            parseResults_Label.TabIndex = 16;
            parseResults_Label.Text = "Policy conversion was successful.";
            // 
            // textBox_EventLogFilePath
            // 
            textBox_EventLogFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox_EventLogFilePath.Font = new System.Drawing.Font("Tahoma", 9F);
            textBox_EventLogFilePath.Location = new System.Drawing.Point(23, 133);
            textBox_EventLogFilePath.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            textBox_EventLogFilePath.Multiline = true;
            textBox_EventLogFilePath.Name = "textBox_EventLogFilePath";
            textBox_EventLogFilePath.ReadOnly = true;
            textBox_EventLogFilePath.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            textBox_EventLogFilePath.Size = new System.Drawing.Size(453, 37);
            textBox_EventLogFilePath.TabIndex = 118;
            textBox_EventLogFilePath.Text = "Select Event Log Files";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            label4.ForeColor = System.Drawing.Color.Black;
            label4.Location = new System.Drawing.Point(23, 111);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(274, 18);
            label4.TabIndex = 117;
            label4.Text = "Parse Event Log evtx Files to Policy";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            label6.ForeColor = System.Drawing.Color.Black;
            label6.Location = new System.Drawing.Point(23, 6);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(432, 18);
            label6.TabIndex = 116;
            label6.Text = "Parse Event Logs from the system Event Viewer to Policy";
            // 
            // panel_Edit_XML
            // 
            panel_Edit_XML.Controls.Add(label3);
            panel_Edit_XML.Controls.Add(policyInfoPanel);
            panel_Edit_XML.Controls.Add(buttonBrowse);
            panel_Edit_XML.Controls.Add(textBoxPolicyPath);
            panel_Edit_XML.Location = new System.Drawing.Point(3, 3);
            panel_Edit_XML.Name = "panel_Edit_XML";
            panel_Edit_XML.Size = new System.Drawing.Size(857, 256);
            panel_Edit_XML.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Tahoma", 9.5F);
            label3.ForeColor = System.Drawing.Color.Black;
            label3.Location = new System.Drawing.Point(20, 2);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(143, 19);
            label3.TabIndex = 12;
            label3.Text = "Policy Path to Edit:";
            // 
            // label_Error
            // 
            label_Error.AutoSize = true;
            label_Error.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label_Error.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label_Error.Location = new System.Drawing.Point(163, 640);
            label_Error.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label_Error.Name = "label_Error";
            label_Error.Size = new System.Drawing.Size(620, 18);
            label_Error.TabIndex = 116;
            label_Error.Text = "Convert the device's Code Integrity event log or an arbitrary log file to a policy XML file";
            label_Error.Visible = false;
            // 
            // EditWorkflow
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.White;
            Controls.Add(label_LearnMore);
            Controls.Add(label9);
            Controls.Add(radioButton_EventConversion);
            Controls.Add(radioButton_EditXML);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(panel_Page);
            Controls.Add(label_Error);
            Margin = new System.Windows.Forms.Padding(2);
            Name = "EditWorkflow";
            Size = new System.Drawing.Size(1208, 894);
            Validated += EditWorkflow_Validated;
            policyInfoPanel.ResumeLayout(false);
            policyInfoPanel.PerformLayout();
            panel_Progress.ResumeLayout(false);
            panel_Progress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox_Progress).EndInit();
            panel_Page.ResumeLayout(false);
            panel_EventLog_Conversion.ResumeLayout(false);
            panel_EventLog_Conversion.PerformLayout();
            eventLogParsing_Result_Panel.ResumeLayout(false);
            eventLogParsing_Result_Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)parseresult_PictureBox).EndInit();
            panel_Edit_XML.ResumeLayout(false);
            panel_Edit_XML.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.TextBox textBoxPolicyPath;
        private System.Windows.Forms.Panel policyInfoPanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_PolicyName;
        private System.Windows.Forms.Label label_policyName;
        private System.Windows.Forms.TextBox textBox_PolicyID;
        private System.Windows.Forms.Label label_fileLocation;
        private System.Windows.Forms.Button buttonParseEventLog;
        private System.Windows.Forms.Button buttonParseLogFile;
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
        private System.Windows.Forms.Button buttonParseMDELog;
        private System.Windows.Forms.TextBox textBox_AdvancedHuntingPaths;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label ahParsingLearnMore_Label;
    }
}
