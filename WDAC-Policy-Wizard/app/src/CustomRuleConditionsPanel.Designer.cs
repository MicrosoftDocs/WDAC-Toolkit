namespace WDAC_Wizard
{
    partial class CustomRuleConditionsPanel
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomRuleConditionsPanel));
            this.panel_CustomRules = new System.Windows.Forms.Panel();
            this.label_Error = new System.Windows.Forms.Label();
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
            this.button_Create = new System.Windows.Forms.Button();
            this.button_AddException = new System.Windows.Forms.Button();
            this.control_Panel = new System.Windows.Forms.Panel();
            this.workflow_Label = new System.Windows.Forms.Label();
            this.page2_Button = new System.Windows.Forms.Button();
            this.page1_Button = new System.Windows.Forms.Button();
            this.controlHighlight_Panel = new System.Windows.Forms.Panel();
            this.panel_CustomRules.SuspendLayout();
            this.panel_FileFolder.SuspendLayout();
            this.panel_Publisher_Scroll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Conditions)).BeginInit();
            this.control_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_CustomRules
            // 
            this.panel_CustomRules.BackColor = System.Drawing.SystemColors.Control;
            this.panel_CustomRules.Controls.Add(this.label_Error);
            this.panel_CustomRules.Controls.Add(this.publisherInfoLabel);
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
            this.panel_CustomRules.Location = new System.Drawing.Point(122, 5);
            this.panel_CustomRules.Margin = new System.Windows.Forms.Padding(2);
            this.panel_CustomRules.Name = "panel_CustomRules";
            this.panel_CustomRules.Size = new System.Drawing.Size(617, 762);
            this.panel_CustomRules.TabIndex = 86;
            // 
            // label_Error
            // 
            this.label_Error.AutoSize = true;
            this.label_Error.Location = new System.Drawing.Point(7, 728);
            this.label_Error.Name = "label_Error";
            this.label_Error.Size = new System.Drawing.Size(46, 17);
            this.label_Error.TabIndex = 87;
            this.label_Error.Text = "label1";
            this.label_Error.Visible = false;
            // 
            // publisherInfoLabel
            // 
            this.publisherInfoLabel.AutoSize = true;
            this.publisherInfoLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.publisherInfoLabel.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.publisherInfoLabel.Location = new System.Drawing.Point(13, 527);
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
            this.panel_FileFolder.Location = new System.Drawing.Point(440, 326);
            this.panel_FileFolder.Margin = new System.Windows.Forms.Padding(2);
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
            this.radioButton_Folder.Location = new System.Drawing.Point(61, 7);
            this.radioButton_Folder.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_Folder.Name = "radioButton_Folder";
            this.radioButton_Folder.Size = new System.Drawing.Size(77, 25);
            this.radioButton_Folder.TabIndex = 96;
            this.radioButton_Folder.TabStop = true;
            this.radioButton_Folder.Text = "Folder";
            this.radioButton_Folder.UseVisualStyleBackColor = true;
            this.radioButton_Folder.Click += new System.EventHandler(this.FileButton_CheckedChanged);
            // 
            // radioButton_File
            // 
            this.radioButton_File.AutoSize = true;
            this.radioButton_File.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton_File.ForeColor = System.Drawing.Color.Black;
            this.radioButton_File.Location = new System.Drawing.Point(2, 7);
            this.radioButton_File.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_File.Name = "radioButton_File";
            this.radioButton_File.Size = new System.Drawing.Size(57, 25);
            this.radioButton_File.TabIndex = 95;
            this.radioButton_File.TabStop = true;
            this.radioButton_File.Text = "File";
            this.radioButton_File.UseVisualStyleBackColor = true;
            this.radioButton_File.Click += new System.EventHandler(this.FileButton_CheckedChanged);
            // 
            // label_Info
            // 
            this.label_Info.AutoSize = true;
            this.label_Info.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Info.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label_Info.Location = new System.Drawing.Point(10, 207);
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
            this.panel_Publisher_Scroll.Location = new System.Drawing.Point(11, 326);
            this.panel_Publisher_Scroll.Margin = new System.Windows.Forms.Padding(2);
            this.panel_Publisher_Scroll.Name = "panel_Publisher_Scroll";
            this.panel_Publisher_Scroll.Size = new System.Drawing.Size(494, 187);
            this.panel_Publisher_Scroll.TabIndex = 103;
            this.panel_Publisher_Scroll.Visible = false;
            // 
            // textBoxSlider_3
            // 
            this.textBoxSlider_3.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxSlider_3.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBoxSlider_3.Location = new System.Drawing.Point(159, 144);
            this.textBoxSlider_3.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSlider_3.Name = "textBoxSlider_3";
            this.textBoxSlider_3.ReadOnly = true;
            this.textBoxSlider_3.Size = new System.Drawing.Size(327, 26);
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
            this.textBoxSlider_2.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxSlider_2.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBoxSlider_2.Location = new System.Drawing.Point(159, 102);
            this.textBoxSlider_2.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSlider_2.Name = "textBoxSlider_2";
            this.textBoxSlider_2.ReadOnly = true;
            this.textBoxSlider_2.Size = new System.Drawing.Size(327, 26);
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
            this.textBoxSlider_1.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxSlider_1.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBoxSlider_1.Location = new System.Drawing.Point(159, 60);
            this.textBoxSlider_1.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSlider_1.Name = "textBoxSlider_1";
            this.textBoxSlider_1.ReadOnly = true;
            this.textBoxSlider_1.Size = new System.Drawing.Size(327, 26);
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
            this.textBoxSlider_0.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxSlider_0.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBoxSlider_0.Location = new System.Drawing.Point(159, 19);
            this.textBoxSlider_0.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSlider_0.Name = "textBoxSlider_0";
            this.textBoxSlider_0.ReadOnly = true;
            this.textBoxSlider_0.Size = new System.Drawing.Size(327, 26);
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
            this.trackBar_Conditions.Margin = new System.Windows.Forms.Padding(2);
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
            this.label9.Location = new System.Drawing.Point(10, 139);
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
            this.comboBox_RuleType.Location = new System.Drawing.Point(11, 168);
            this.comboBox_RuleType.Margin = new System.Windows.Forms.Padding(2);
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
            this.radioButton_Deny.Location = new System.Drawing.Point(91, 100);
            this.radioButton_Deny.Margin = new System.Windows.Forms.Padding(2);
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
            this.radioButton_Allow.Location = new System.Drawing.Point(11, 100);
            this.radioButton_Allow.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_Allow.Name = "radioButton_Allow";
            this.radioButton_Allow.Size = new System.Drawing.Size(72, 25);
            this.radioButton_Allow.TabIndex = 89;
            this.radioButton_Allow.TabStop = true;
            this.radioButton_Allow.Text = "Allow";
            this.radioButton_Allow.UseVisualStyleBackColor = true;
            // 
            // textBox_ReferenceFile
            // 
            this.textBox_ReferenceFile.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_ReferenceFile.Location = new System.Drawing.Point(12, 279);
            this.textBox_ReferenceFile.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_ReferenceFile.Name = "textBox_ReferenceFile";
            this.textBox_ReferenceFile.Size = new System.Drawing.Size(408, 26);
            this.textBox_ReferenceFile.TabIndex = 88;
            // 
            // button_Browse
            // 
            this.button_Browse.BackColor = System.Drawing.Color.Transparent;
            this.button_Browse.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.button_Browse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Browse.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Browse.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_Browse.Location = new System.Drawing.Point(440, 276);
            this.button_Browse.Margin = new System.Windows.Forms.Padding(2);
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
            this.label_condition.Location = new System.Drawing.Point(12, 249);
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
            // button_Create
            // 
            this.button_Create.BackColor = System.Drawing.Color.Transparent;
            this.button_Create.FlatAppearance.BorderColor = System.Drawing.SystemColors.MenuHighlight;
            this.button_Create.FlatAppearance.BorderSize = 2;
            this.button_Create.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Create.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Create.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_Create.Location = new System.Drawing.Point(123, 786);
            this.button_Create.Margin = new System.Windows.Forms.Padding(2);
            this.button_Create.Name = "button_Create";
            this.button_Create.Size = new System.Drawing.Size(121, 33);
            this.button_Create.TabIndex = 92;
            this.button_Create.Text = "Create Rule";
            this.button_Create.UseVisualStyleBackColor = false;
            this.button_Create.Click += new System.EventHandler(this.button_Create_Click);
            // 
            // button_AddException
            // 
            this.button_AddException.BackColor = System.Drawing.Color.Transparent;
            this.button_AddException.FlatAppearance.BorderColor = System.Drawing.SystemColors.InfoText;
            this.button_AddException.FlatAppearance.BorderSize = 2;
            this.button_AddException.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_AddException.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_AddException.ForeColor = System.Drawing.Color.Black;
            this.button_AddException.Location = new System.Drawing.Point(609, 786);
            this.button_AddException.Margin = new System.Windows.Forms.Padding(2);
            this.button_AddException.Name = "button_AddException";
            this.button_AddException.Size = new System.Drawing.Size(130, 33);
            this.button_AddException.TabIndex = 107;
            this.button_AddException.Text = "Add Exception >";
            this.button_AddException.UseVisualStyleBackColor = false;
            this.button_AddException.Click += new System.EventHandler(this.button_AddException_Click);
            // 
            // control_Panel
            // 
            this.control_Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.control_Panel.Controls.Add(this.workflow_Label);
            this.control_Panel.Controls.Add(this.page2_Button);
            this.control_Panel.Controls.Add(this.page1_Button);
            this.control_Panel.Controls.Add(this.controlHighlight_Panel);
            this.control_Panel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.control_Panel.Location = new System.Drawing.Point(0, 0);
            this.control_Panel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.control_Panel.Name = "control_Panel";
            this.control_Panel.Size = new System.Drawing.Size(123, 767);
            this.control_Panel.TabIndex = 108;
            // 
            // workflow_Label
            // 
            this.workflow_Label.AutoSize = true;
            this.workflow_Label.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.workflow_Label.Location = new System.Drawing.Point(11, 9);
            this.workflow_Label.Name = "workflow_Label";
            this.workflow_Label.Size = new System.Drawing.Size(82, 21);
            this.workflow_Label.TabIndex = 40;
            this.workflow_Label.Text = "File Rules";
            this.workflow_Label.Visible = false;
            // 
            // page2_Button
            // 
            this.page2_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.page2_Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.page2_Button.Enabled = false;
            this.page2_Button.FlatAppearance.BorderSize = 0;
            this.page2_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page2_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page2_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.page2_Button.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.page2_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page2_Button.Location = new System.Drawing.Point(12, 150);
            this.page2_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.page2_Button.Name = "page2_Button";
            this.page2_Button.Size = new System.Drawing.Size(122, 60);
            this.page2_Button.TabIndex = 36;
            this.page2_Button.Text = "Rule Exceptions";
            this.page2_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page2_Button.UseVisualStyleBackColor = false;
            this.page2_Button.Visible = false;
            // 
            // page1_Button
            // 
            this.page1_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.page1_Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.page1_Button.Enabled = false;
            this.page1_Button.FlatAppearance.BorderSize = 0;
            this.page1_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page1_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page1_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.page1_Button.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.page1_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page1_Button.Location = new System.Drawing.Point(12, 69);
            this.page1_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.page1_Button.Name = "page1_Button";
            this.page1_Button.Size = new System.Drawing.Size(120, 52);
            this.page1_Button.TabIndex = 35;
            this.page1_Button.Text = "Rule Conditions";
            this.page1_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page1_Button.UseVisualStyleBackColor = false;
            this.page1_Button.Visible = false;
            // 
            // controlHighlight_Panel
            // 
            this.controlHighlight_Panel.BackColor = System.Drawing.Color.CornflowerBlue;
            this.controlHighlight_Panel.Location = new System.Drawing.Point(0, 0);
            this.controlHighlight_Panel.Name = "controlHighlight_Panel";
            this.controlHighlight_Panel.Size = new System.Drawing.Size(8, 35);
            this.controlHighlight_Panel.TabIndex = 33;
            // 
            // CustomRuleConditionsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 830);
            this.Controls.Add(this.control_Panel);
            this.Controls.Add(this.button_AddException);
            this.Controls.Add(this.panel_CustomRules);
            this.Controls.Add(this.button_Create);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "CustomRuleConditionsPanel";
            this.Text = "Custom Rules ";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CustomRulesPanel_FormClosing);
            this.panel_CustomRules.ResumeLayout(false);
            this.panel_CustomRules.PerformLayout();
            this.panel_FileFolder.ResumeLayout(false);
            this.panel_FileFolder.PerformLayout();
            this.panel_Publisher_Scroll.ResumeLayout(false);
            this.panel_Publisher_Scroll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Conditions)).EndInit();
            this.control_Panel.ResumeLayout(false);
            this.control_Panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_CustomRules;
        private System.Windows.Forms.Label publisherInfoLabel;
        private System.Windows.Forms.Button button_Create;
        private System.Windows.Forms.Panel panel_FileFolder;
        private System.Windows.Forms.RadioButton radioButton_Folder;
        private System.Windows.Forms.RadioButton radioButton_File;
        private System.Windows.Forms.Label label_Info;
        private System.Windows.Forms.Panel panel_Publisher_Scroll;
        private System.Windows.Forms.TextBox textBoxSlider_3;
        private System.Windows.Forms.Label labelSlider_3;
        private System.Windows.Forms.TextBox textBoxSlider_2;
        private System.Windows.Forms.Label labelSlider_2;
        private System.Windows.Forms.TextBox textBoxSlider_1;
        private System.Windows.Forms.Label labelSlider_1;
        private System.Windows.Forms.TextBox textBoxSlider_0;
        private System.Windows.Forms.Label labelSlider_0;
        private System.Windows.Forms.TrackBar trackBar_Conditions;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox comboBox_RuleType;
        private System.Windows.Forms.RadioButton radioButton_Deny;
        private System.Windows.Forms.RadioButton radioButton_Allow;
        private System.Windows.Forms.TextBox textBox_ReferenceFile;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.Label label_condition;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label_Error;
        private System.Windows.Forms.Button button_AddException;
        private System.Windows.Forms.Panel control_Panel;
        private System.Windows.Forms.Label workflow_Label;
        private System.Windows.Forms.Button page2_Button;
        public System.Windows.Forms.Button page1_Button;
        private System.Windows.Forms.Panel controlHighlight_Panel;
    }
}