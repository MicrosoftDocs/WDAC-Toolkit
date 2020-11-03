// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

namespace WDAC_Wizard
{
    partial class SigningRules_Control
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
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rulesDataGrid = new System.Windows.Forms.DataGridView();
            this.column_Action = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_Level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Files = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Exceptions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label8 = new System.Windows.Forms.Label();
            this.label_AddCustomRules = new System.Windows.Forms.Label();
            this.button_Create = new System.Windows.Forms.Button();
            this.panel_CustomRules = new System.Windows.Forms.Panel();
            this.publisherInfoLabel = new System.Windows.Forms.Label();
            this.panel_FileFolder = new System.Windows.Forms.Panel();
            this.radioButton_Folder = new System.Windows.Forms.RadioButton();
            this.radioButton_File = new System.Windows.Forms.RadioButton();
            this.label_Info = new System.Windows.Forms.Label();
            this.panel_Publisher_Scroll = new System.Windows.Forms.Panel();
            this.textBoxSlider_3 = new System.Windows.Forms.TextBox();
            this.labelSlider_3 = new System.Windows.Forms.Label();
            this.textBoxSlider_2 = new System.Windows.Forms.TextBox();
            this.labelSlider_2 = new System.Windows.Forms.Label();
            this.textBoxSlider_1 = new System.Windows.Forms.TextBox();
            this.labelSlider_1 = new System.Windows.Forms.Label();
            this.textBoxSlider_0 = new System.Windows.Forms.TextBox();
            this.labelSlider_0 = new System.Windows.Forms.Label();
            this.trackBar_Conditions = new System.Windows.Forms.TrackBar();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.comboBox_RuleType = new System.Windows.Forms.ComboBox();
            this.radioButton_Deny = new System.Windows.Forms.RadioButton();
            this.radioButton_Allow = new System.Windows.Forms.RadioButton();
            this.textBox_ReferenceFile = new System.Windows.Forms.TextBox();
            this.button_Browse = new System.Windows.Forms.Button();
            this.label_condition = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.label_Error = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.rulesDataGrid)).BeginInit();
            this.panel_CustomRules.SuspendLayout();
            this.panel_FileFolder.SuspendLayout();
            this.panel_Publisher_Scroll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Conditions)).BeginInit();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(168, 332);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(0, 21);
            this.label7.TabIndex = 57;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(157, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 29);
            this.label1.TabIndex = 47;
            this.label1.Text = "Policy Signing Rules";
            // 
            // rulesDataGrid
            // 
            this.rulesDataGrid.AllowUserToDeleteRows = false;
            this.rulesDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.rulesDataGrid.BackgroundColor = System.Drawing.Color.Silver;
            this.rulesDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.rulesDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.column_Action,
            this.column_Level,
            this.Column_Name,
            this.Column_Files,
            this.Column_Exceptions});
            this.rulesDataGrid.Location = new System.Drawing.Point(162, 162);
            this.rulesDataGrid.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rulesDataGrid.Name = "rulesDataGrid";
            this.rulesDataGrid.ReadOnly = true;
            this.rulesDataGrid.RowHeadersVisible = false;
            this.rulesDataGrid.RowHeadersWidth = 70;
            this.rulesDataGrid.RowTemplate.Height = 24;
            this.rulesDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.rulesDataGrid.Size = new System.Drawing.Size(421, 440);
            this.rulesDataGrid.TabIndex = 92;
            this.rulesDataGrid.VirtualMode = true;
            this.rulesDataGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataClicked);
            this.rulesDataGrid.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.rulesDataGrid_CellValueNeeded);
            // 
            // column_Action
            // 
            this.column_Action.HeaderText = "Action";
            this.column_Action.MinimumWidth = 6;
            this.column_Action.Name = "column_Action";
            this.column_Action.ReadOnly = true;
            this.column_Action.Width = 76;
            // 
            // column_Level
            // 
            this.column_Level.HeaderText = "Level";
            this.column_Level.MinimumWidth = 6;
            this.column_Level.Name = "column_Level";
            this.column_Level.ReadOnly = true;
            this.column_Level.Width = 71;
            // 
            // Column_Name
            // 
            this.Column_Name.HeaderText = "Name";
            this.Column_Name.MinimumWidth = 6;
            this.Column_Name.Name = "Column_Name";
            this.Column_Name.ReadOnly = true;
            this.Column_Name.Width = 74;
            // 
            // Column_Files
            // 
            this.Column_Files.HeaderText = "Associated Files";
            this.Column_Files.MinimumWidth = 6;
            this.Column_Files.Name = "Column_Files";
            this.Column_Files.ReadOnly = true;
            this.Column_Files.Width = 127;
            // 
            // Column_Exceptions
            // 
            this.Column_Exceptions.HeaderText = "Exceptions";
            this.Column_Exceptions.MinimumWidth = 6;
            this.Column_Exceptions.Name = "Column_Exceptions";
            this.Column_Exceptions.ReadOnly = true;
            this.Column_Exceptions.Width = 105;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(163, 132);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(187, 21);
            this.label8.TabIndex = 91;
            this.label8.Text = "Policy Signing Rules List";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label_AddCustomRules
            // 
            this.label_AddCustomRules.AutoSize = true;
            this.label_AddCustomRules.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label_AddCustomRules.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.label_AddCustomRules.Location = new System.Drawing.Point(464, 134);
            this.label_AddCustomRules.Name = "label_AddCustomRules";
            this.label_AddCustomRules.Size = new System.Drawing.Size(113, 18);
            this.label_AddCustomRules.TabIndex = 92;
            this.label_AddCustomRules.Text = "+ Custom Rules";
            this.label_AddCustomRules.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.label_AddCustomRules.Click += new System.EventHandler(this.label_AddCustomRules_Click);
            // 
            // button_Create
            // 
            this.button_Create.BackColor = System.Drawing.Color.Transparent;
            this.button_Create.FlatAppearance.BorderColor = System.Drawing.SystemColors.MenuHighlight;
            this.button_Create.FlatAppearance.BorderSize = 2;
            this.button_Create.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Create.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Create.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_Create.Location = new System.Drawing.Point(12, 522);
            this.button_Create.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_Create.Name = "button_Create";
            this.button_Create.Size = new System.Drawing.Size(121, 33);
            this.button_Create.TabIndex = 92;
            this.button_Create.Text = "Create Rule";
            this.button_Create.UseVisualStyleBackColor = false;
            this.button_Create.Click += new System.EventHandler(this.button_Create_Click);
            // 
            // panel_CustomRules
            // 
            this.panel_CustomRules.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel_CustomRules.Controls.Add(this.publisherInfoLabel);
            this.panel_CustomRules.Controls.Add(this.button_Create);
            this.panel_CustomRules.Controls.Add(this.panel_FileFolder);
            this.panel_CustomRules.Controls.Add(this.label_Info);
            this.panel_CustomRules.Controls.Add(this.panel_Publisher_Scroll);
            this.panel_CustomRules.Controls.Add(this.label10);
            this.panel_CustomRules.Controls.Add(this.label9);
            this.panel_CustomRules.Controls.Add(this.comboBox_RuleType);
            this.panel_CustomRules.Controls.Add(this.radioButton_Deny);
            this.panel_CustomRules.Controls.Add(this.radioButton_Allow);
            this.panel_CustomRules.Controls.Add(this.textBox_ReferenceFile);
            this.panel_CustomRules.Controls.Add(this.button_Browse);
            this.panel_CustomRules.Controls.Add(this.label_condition);
            this.panel_CustomRules.Controls.Add(this.label4);
            this.panel_CustomRules.Location = new System.Drawing.Point(597, 98);
            this.panel_CustomRules.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel_CustomRules.Name = "panel_CustomRules";
            this.panel_CustomRules.Size = new System.Drawing.Size(599, 560);
            this.panel_CustomRules.TabIndex = 85;
            this.panel_CustomRules.Visible = false;
            // 
            // publisherInfoLabel
            // 
            this.publisherInfoLabel.AutoSize = true;
            this.publisherInfoLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.publisherInfoLabel.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.publisherInfoLabel.Location = new System.Drawing.Point(13, 467);
            this.publisherInfoLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.publisherInfoLabel.Name = "publisherInfoLabel";
            this.publisherInfoLabel.Size = new System.Drawing.Size(472, 36);
            this.publisherInfoLabel.TabIndex = 106;
            this.publisherInfoLabel.Text = "Rule applies to all files signed by this Issuing CA and publisher with this  \r\nfi" +
    "le name with a version at or above the specified version number.";
            this.publisherInfoLabel.Visible = false;
            // 
            // panel_FileFolder
            // 
            this.panel_FileFolder.Controls.Add(this.radioButton_Folder);
            this.panel_FileFolder.Controls.Add(this.radioButton_File);
            this.panel_FileFolder.Location = new System.Drawing.Point(436, 278);
            this.panel_FileFolder.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel_FileFolder.Name = "panel_FileFolder";
            this.panel_FileFolder.Size = new System.Drawing.Size(140, 34);
            this.panel_FileFolder.TabIndex = 104;
            this.panel_FileFolder.Visible = false;
            // 
            // radioButton_Folder
            // 
            this.radioButton_Folder.AutoSize = true;
            this.radioButton_Folder.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton_Folder.ForeColor = System.Drawing.Color.Black;
            this.radioButton_Folder.Location = new System.Drawing.Point(65, 7);
            this.radioButton_Folder.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButton_Folder.Name = "radioButton_Folder";
            this.radioButton_Folder.Size = new System.Drawing.Size(77, 25);
            this.radioButton_Folder.TabIndex = 96;
            this.radioButton_Folder.TabStop = true;
            this.radioButton_Folder.Text = "Folder";
            this.radioButton_Folder.UseVisualStyleBackColor = true;
            // 
            // radioButton_File
            // 
            this.radioButton_File.AutoSize = true;
            this.radioButton_File.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton_File.ForeColor = System.Drawing.Color.Black;
            this.radioButton_File.Location = new System.Drawing.Point(10, 7);
            this.radioButton_File.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButton_File.Name = "radioButton_File";
            this.radioButton_File.Size = new System.Drawing.Size(57, 25);
            this.radioButton_File.TabIndex = 95;
            this.radioButton_File.TabStop = true;
            this.radioButton_File.Text = "File";
            this.radioButton_File.UseVisualStyleBackColor = true;
            this.radioButton_File.CheckedChanged += new System.EventHandler(this.FileButton_CheckedChanged);
            // 
            // label_Info
            // 
            this.label_Info.AutoSize = true;
            this.label_Info.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Info.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label_Info.Location = new System.Drawing.Point(12, 163);
            this.label_Info.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Info.Name = "label_Info";
            this.label_Info.Size = new System.Drawing.Size(76, 18);
            this.label_Info.TabIndex = 95;
            this.label_Info.Text = "Label_Info";
            this.label_Info.Visible = false;
            // 
            // panel_Publisher_Scroll
            // 
            this.panel_Publisher_Scroll.Controls.Add(this.textBoxSlider_3);
            this.panel_Publisher_Scroll.Controls.Add(this.labelSlider_3);
            this.panel_Publisher_Scroll.Controls.Add(this.textBoxSlider_2);
            this.panel_Publisher_Scroll.Controls.Add(this.labelSlider_2);
            this.panel_Publisher_Scroll.Controls.Add(this.textBoxSlider_1);
            this.panel_Publisher_Scroll.Controls.Add(this.labelSlider_1);
            this.panel_Publisher_Scroll.Controls.Add(this.textBoxSlider_0);
            this.panel_Publisher_Scroll.Controls.Add(this.labelSlider_0);
            this.panel_Publisher_Scroll.Controls.Add(this.trackBar_Conditions);
            this.panel_Publisher_Scroll.Location = new System.Drawing.Point(13, 277);
            this.panel_Publisher_Scroll.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel_Publisher_Scroll.Name = "panel_Publisher_Scroll";
            this.panel_Publisher_Scroll.Size = new System.Drawing.Size(472, 187);
            this.panel_Publisher_Scroll.TabIndex = 103;
            this.panel_Publisher_Scroll.Visible = false;
            // 
            // textBoxSlider_3
            // 
            this.textBoxSlider_3.Font = new System.Drawing.Font("Tahoma", 8F);
            this.textBoxSlider_3.Location = new System.Drawing.Point(159, 146);
            this.textBoxSlider_3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxSlider_3.Name = "textBoxSlider_3";
            this.textBoxSlider_3.Size = new System.Drawing.Size(279, 24);
            this.textBoxSlider_3.TabIndex = 103;
            // 
            // labelSlider_3
            // 
            this.labelSlider_3.AutoSize = true;
            this.labelSlider_3.Font = new System.Drawing.Font("Tahoma", 9F);
            this.labelSlider_3.ForeColor = System.Drawing.Color.Black;
            this.labelSlider_3.Location = new System.Drawing.Point(36, 147);
            this.labelSlider_3.Name = "labelSlider_3";
            this.labelSlider_3.Size = new System.Drawing.Size(75, 18);
            this.labelSlider_3.TabIndex = 104;
            this.labelSlider_3.Text = "File name:";
            this.labelSlider_3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textBoxSlider_2
            // 
            this.textBoxSlider_2.Font = new System.Drawing.Font("Tahoma", 8F);
            this.textBoxSlider_2.Location = new System.Drawing.Point(159, 104);
            this.textBoxSlider_2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxSlider_2.Name = "textBoxSlider_2";
            this.textBoxSlider_2.Size = new System.Drawing.Size(279, 24);
            this.textBoxSlider_2.TabIndex = 101;
            // 
            // labelSlider_2
            // 
            this.labelSlider_2.AutoSize = true;
            this.labelSlider_2.Font = new System.Drawing.Font("Tahoma", 9F);
            this.labelSlider_2.ForeColor = System.Drawing.Color.Black;
            this.labelSlider_2.Location = new System.Drawing.Point(36, 105);
            this.labelSlider_2.Name = "labelSlider_2";
            this.labelSlider_2.Size = new System.Drawing.Size(84, 18);
            this.labelSlider_2.TabIndex = 102;
            this.labelSlider_2.Text = "File version:";
            this.labelSlider_2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textBoxSlider_1
            // 
            this.textBoxSlider_1.Font = new System.Drawing.Font("Tahoma", 8F);
            this.textBoxSlider_1.Location = new System.Drawing.Point(159, 62);
            this.textBoxSlider_1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxSlider_1.Name = "textBoxSlider_1";
            this.textBoxSlider_1.Size = new System.Drawing.Size(279, 24);
            this.textBoxSlider_1.TabIndex = 99;
            // 
            // labelSlider_1
            // 
            this.labelSlider_1.AutoSize = true;
            this.labelSlider_1.Font = new System.Drawing.Font("Tahoma", 9F);
            this.labelSlider_1.ForeColor = System.Drawing.Color.Black;
            this.labelSlider_1.Location = new System.Drawing.Point(36, 62);
            this.labelSlider_1.Name = "labelSlider_1";
            this.labelSlider_1.Size = new System.Drawing.Size(69, 18);
            this.labelSlider_1.TabIndex = 100;
            this.labelSlider_1.Text = "Publisher:";
            this.labelSlider_1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // textBoxSlider_0
            // 
            this.textBoxSlider_0.Font = new System.Drawing.Font("Tahoma", 8F);
            this.textBoxSlider_0.Location = new System.Drawing.Point(159, 21);
            this.textBoxSlider_0.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBoxSlider_0.Name = "textBoxSlider_0";
            this.textBoxSlider_0.Size = new System.Drawing.Size(279, 24);
            this.textBoxSlider_0.TabIndex = 95;
            // 
            // labelSlider_0
            // 
            this.labelSlider_0.AutoSize = true;
            this.labelSlider_0.Font = new System.Drawing.Font("Tahoma", 9F);
            this.labelSlider_0.ForeColor = System.Drawing.Color.Black;
            this.labelSlider_0.Location = new System.Drawing.Point(36, 21);
            this.labelSlider_0.Name = "labelSlider_0";
            this.labelSlider_0.Size = new System.Drawing.Size(82, 18);
            this.labelSlider_0.TabIndex = 98;
            this.labelSlider_0.Text = "Issuing CA:";
            this.labelSlider_0.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // trackBar_Conditions
            // 
            this.trackBar_Conditions.LargeChange = 4;
            this.trackBar_Conditions.Location = new System.Drawing.Point(2, 13);
            this.trackBar_Conditions.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.trackBar_Conditions.Maximum = 12;
            this.trackBar_Conditions.Name = "trackBar_Conditions";
            this.trackBar_Conditions.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_Conditions.Size = new System.Drawing.Size(56, 165);
            this.trackBar_Conditions.SmallChange = 4;
            this.trackBar_Conditions.TabIndex = 96;
            this.trackBar_Conditions.TickFrequency = 4;
            this.trackBar_Conditions.Scroll += new System.EventHandler(this.trackBar_Conditions_Scroll);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(13, 38);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(523, 36);
            this.label10.TabIndex = 94;
            this.label10.Text = "Select the rule type, browse for the reference file and choose whether to allow \r" +
    "\nor deny. ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(13, 101);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(79, 18);
            this.label9.TabIndex = 89;
            this.label9.Text = "Rule Type:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // comboBox_RuleType
            // 
            this.comboBox_RuleType.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_RuleType.FormattingEnabled = true;
            this.comboBox_RuleType.Items.AddRange(new object[] {
            "Publisher",
            "Path",
            "File Attributes",
            "File Hash"});
            this.comboBox_RuleType.Location = new System.Drawing.Point(16, 121);
            this.comboBox_RuleType.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox_RuleType.Name = "comboBox_RuleType";
            this.comboBox_RuleType.Size = new System.Drawing.Size(187, 26);
            this.comboBox_RuleType.TabIndex = 89;
            this.comboBox_RuleType.SelectedIndexChanged += new System.EventHandler(this.RuleType_ComboboxChanged);
            // 
            // radioButton_Deny
            // 
            this.radioButton_Deny.AutoSize = true;
            this.radioButton_Deny.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton_Deny.ForeColor = System.Drawing.Color.Black;
            this.radioButton_Deny.Location = new System.Drawing.Point(315, 121);
            this.radioButton_Deny.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButton_Deny.Name = "radioButton_Deny";
            this.radioButton_Deny.Size = new System.Drawing.Size(69, 25);
            this.radioButton_Deny.TabIndex = 90;
            this.radioButton_Deny.TabStop = true;
            this.radioButton_Deny.Text = "Deny";
            this.radioButton_Deny.UseVisualStyleBackColor = true;
            // 
            // radioButton_Allow
            // 
            this.radioButton_Allow.AutoSize = true;
            this.radioButton_Allow.Checked = true;
            this.radioButton_Allow.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton_Allow.ForeColor = System.Drawing.Color.Black;
            this.radioButton_Allow.Location = new System.Drawing.Point(232, 121);
            this.radioButton_Allow.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButton_Allow.Name = "radioButton_Allow";
            this.radioButton_Allow.Size = new System.Drawing.Size(72, 25);
            this.radioButton_Allow.TabIndex = 89;
            this.radioButton_Allow.TabStop = true;
            this.radioButton_Allow.Text = "Allow";
            this.radioButton_Allow.UseVisualStyleBackColor = true;
            this.radioButton_Allow.CheckedChanged += new System.EventHandler(this.radioButton_Allow_CheckedChanged);
            // 
            // textBox_ReferenceFile
            // 
            this.textBox_ReferenceFile.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_ReferenceFile.Location = new System.Drawing.Point(14, 241);
            this.textBox_ReferenceFile.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_ReferenceFile.Name = "textBox_ReferenceFile";
            this.textBox_ReferenceFile.Size = new System.Drawing.Size(408, 26);
            this.textBox_ReferenceFile.TabIndex = 88;
            this.textBox_ReferenceFile.Click += new System.EventHandler(this.button_Browse_Click);
            // 
            // button_Browse
            // 
            this.button_Browse.BackColor = System.Drawing.Color.Transparent;
            this.button_Browse.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.button_Browse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Browse.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Browse.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_Browse.Location = new System.Drawing.Point(440, 234);
            this.button_Browse.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_Browse.Name = "button_Browse";
            this.button_Browse.Size = new System.Drawing.Size(83, 33);
            this.button_Browse.TabIndex = 84;
            this.button_Browse.Text = "Browse";
            this.button_Browse.UseVisualStyleBackColor = false;
            this.button_Browse.Click += new System.EventHandler(this.button_Browse_Click);
            // 
            // label_condition
            // 
            this.label_condition.AutoSize = true;
            this.label_condition.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_condition.ForeColor = System.Drawing.Color.Black;
            this.label_condition.Location = new System.Drawing.Point(12, 218);
            this.label_condition.Name = "label_condition";
            this.label_condition.Size = new System.Drawing.Size(104, 18);
            this.label_condition.TabIndex = 87;
            this.label_condition.Text = "Reference File:";
            this.label_condition.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(12, 2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(186, 21);
            this.label4.TabIndex = 86;
            this.label4.Text = "Custom Rule Conditions";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(159, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(556, 18);
            this.label3.TabIndex = 80;
            this.label3.Text = "Create allow or deny rules for signed files based on its publisher, path or hash " +
    "value.";
            // 
            // deleteButton
            // 
            this.deleteButton.FlatAppearance.BorderSize = 0;
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteButton.Image = global::WDAC_Wizard.Properties.Resources.minus_button;
            this.deleteButton.Location = new System.Drawing.Point(560, 608);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(23, 26);
            this.deleteButton.TabIndex = 93;
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // label_Error
            // 
            this.label_Error.AutoSize = true;
            this.label_Error.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Error.ForeColor = System.Drawing.Color.Red;
            this.label_Error.Location = new System.Drawing.Point(152, 667);
            this.label_Error.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Error.Name = "label_Error";
            this.label_Error.Size = new System.Drawing.Size(648, 18);
            this.label_Error.TabIndex = 96;
            this.label_Error.Text = "Label_Error: Lorem Ipsum text text text text. Lorum Ipsum text text text text tex" +
    "t text text text";
            this.label_Error.Visible = false;
            // 
            // SigningRules_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Controls.Add(this.label_Error);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.panel_CustomRules);
            this.Controls.Add(this.label_AddCustomRules);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rulesDataGrid);
            this.Controls.Add(this.label3);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "SigningRules_Control";
            this.Size = new System.Drawing.Size(1203, 725);
            this.Load += new System.EventHandler(this.SigningRules_Control_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rulesDataGrid)).EndInit();
            this.panel_CustomRules.ResumeLayout(false);
            this.panel_CustomRules.PerformLayout();
            this.panel_FileFolder.ResumeLayout(false);
            this.panel_FileFolder.PerformLayout();
            this.panel_Publisher_Scroll.ResumeLayout(false);
            this.panel_Publisher_Scroll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Conditions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView rulesDataGrid;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label_AddCustomRules;
        private System.Windows.Forms.Button button_Create;
        private System.Windows.Forms.Panel panel_CustomRules;
        private System.Windows.Forms.Label publisherInfoLabel;
        private System.Windows.Forms.Panel panel_FileFolder;
        private System.Windows.Forms.RadioButton radioButton_Folder;
        private System.Windows.Forms.RadioButton radioButton_File;
        private System.Windows.Forms.Label label_Info;
        private System.Windows.Forms.Panel panel_Publisher_Scroll;
        private System.Windows.Forms.TextBox textBoxSlider_2;
        private System.Windows.Forms.Label labelSlider_2;
        private System.Windows.Forms.TextBox textBoxSlider_1;
        private System.Windows.Forms.Label labelSlider_1;
        private System.Windows.Forms.TextBox textBoxSlider_0;
        private System.Windows.Forms.Label labelSlider_0;
        private System.Windows.Forms.TrackBar trackBar_Conditions;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBox_RuleType;
        private System.Windows.Forms.RadioButton radioButton_Deny;
        private System.Windows.Forms.RadioButton radioButton_Allow;
        private System.Windows.Forms.TextBox textBox_ReferenceFile;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.Label label_condition;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxSlider_3;
        private System.Windows.Forms.Label labelSlider_3;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Label label_Error;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Action;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Level;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Files;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Exceptions;
    }
}
