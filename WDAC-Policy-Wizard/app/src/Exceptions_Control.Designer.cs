namespace WDAC_Wizard
{
    partial class Exceptions_Control
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
            this.panel_CustomRules = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column_Exception = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label_Error = new System.Windows.Forms.Label();
            this.button_CreateException = new System.Windows.Forms.Button();
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
            this.textBox_ReferenceFile = new System.Windows.Forms.TextBox();
            this.button_Browse = new System.Windows.Forms.Button();
            this.label_condition = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel_CustomRules.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel_FileFolder.SuspendLayout();
            this.panel_Publisher_Scroll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Conditions)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_CustomRules
            // 
            this.panel_CustomRules.BackColor = System.Drawing.SystemColors.Control;
            this.panel_CustomRules.Controls.Add(this.panel1);
            this.panel_CustomRules.Controls.Add(this.dataGridView1);
            this.panel_CustomRules.Controls.Add(this.label_Error);
            this.panel_CustomRules.Controls.Add(this.button_CreateException);
            this.panel_CustomRules.Controls.Add(this.label_Info);
            this.panel_CustomRules.Controls.Add(this.label10);
            this.panel_CustomRules.Controls.Add(this.label9);
            this.panel_CustomRules.Controls.Add(this.comboBox_RuleType);
            this.panel_CustomRules.Controls.Add(this.textBox_ReferenceFile);
            this.panel_CustomRules.Controls.Add(this.button_Browse);
            this.panel_CustomRules.Controls.Add(this.label_condition);
            this.panel_CustomRules.Controls.Add(this.label4);
            this.panel_CustomRules.Location = new System.Drawing.Point(2, 2);
            this.panel_CustomRules.Margin = new System.Windows.Forms.Padding(2);
            this.panel_CustomRules.Name = "panel_CustomRules";
            this.panel_CustomRules.Size = new System.Drawing.Size(704, 965);
            this.panel_CustomRules.TabIndex = 87;
            this.panel_CustomRules.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_CustomRules_Paint);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_Exception,
            this.Column_Type});
            this.dataGridView1.Location = new System.Drawing.Point(12, 207);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(509, 150);
            this.dataGridView1.TabIndex = 107;
            // 
            // Column_Exception
            // 
            this.Column_Exception.HeaderText = "Exception";
            this.Column_Exception.MinimumWidth = 6;
            this.Column_Exception.Name = "Column_Exception";
            this.Column_Exception.ReadOnly = true;
            this.Column_Exception.Width = 125;
            // 
            // Column_Type
            // 
            this.Column_Type.HeaderText = "Type";
            this.Column_Type.MinimumWidth = 6;
            this.Column_Type.Name = "Column_Type";
            this.Column_Type.ReadOnly = true;
            this.Column_Type.Width = 125;
            // 
            // label_Error
            // 
            this.label_Error.AutoSize = true;
            this.label_Error.Location = new System.Drawing.Point(47, 777);
            this.label_Error.Name = "label_Error";
            this.label_Error.Size = new System.Drawing.Size(46, 17);
            this.label_Error.TabIndex = 87;
            this.label_Error.Text = "label1";
            this.label_Error.Visible = false;
            // 
            // button_CreateException
            // 
            this.button_CreateException.BackColor = System.Drawing.Color.Transparent;
            this.button_CreateException.FlatAppearance.BorderColor = System.Drawing.SystemColors.MenuHighlight;
            this.button_CreateException.FlatAppearance.BorderSize = 2;
            this.button_CreateException.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_CreateException.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_CreateException.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_CreateException.Location = new System.Drawing.Point(11, 722);
            this.button_CreateException.Margin = new System.Windows.Forms.Padding(2);
            this.button_CreateException.Name = "button_CreateException";
            this.button_CreateException.Size = new System.Drawing.Size(134, 33);
            this.button_CreateException.TabIndex = 92;
            this.button_CreateException.Text = "Create Exception";
            this.button_CreateException.UseVisualStyleBackColor = false;
            this.button_CreateException.Click += new System.EventHandler(this.button_CreateException_Click);
            // 
            // panel_FileFolder
            // 
            this.panel_FileFolder.Controls.Add(this.radioButton_Folder);
            this.panel_FileFolder.Controls.Add(this.radioButton_File);
            this.panel_FileFolder.Location = new System.Drawing.Point(420, 2);
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
            // 
            // label_Info
            // 
            this.label_Info.AutoSize = true;
            this.label_Info.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Info.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.label_Info.Location = new System.Drawing.Point(8, 93);
            this.label_Info.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Info.Name = "label_Info";
            this.label_Info.Size = new System.Drawing.Size(103, 18);
            this.label_Info.TabIndex = 95;
            this.label_Info.Text = "Rule Condition:";
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
            this.panel_Publisher_Scroll.Location = new System.Drawing.Point(12, 22);
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
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(8, 38);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(606, 36);
            this.label10.TabIndex = 94;
            this.label10.Text = "Exceptions allow you to exclude files that would be included in the rule. Select " +
    "the exception \r\ntype and browse for the reference file off which to base the exc" +
    "eption.  ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(11, 147);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(114, 18);
            this.label9.TabIndex = 89;
            this.label9.Text = "Exception Type:";
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
            this.comboBox_RuleType.Location = new System.Drawing.Point(12, 176);
            this.comboBox_RuleType.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_RuleType.Name = "comboBox_RuleType";
            this.comboBox_RuleType.Size = new System.Drawing.Size(187, 26);
            this.comboBox_RuleType.TabIndex = 89;
            this.comboBox_RuleType.SelectedIndexChanged += new System.EventHandler(this.comboBox_RuleType_SelectedIndexChanged);
            // 
            // textBox_ReferenceFile
            // 
            this.textBox_ReferenceFile.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_ReferenceFile.Location = new System.Drawing.Point(12, 398);
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
            this.button_Browse.Location = new System.Drawing.Point(440, 395);
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
            this.label_condition.Location = new System.Drawing.Point(12, 368);
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
            this.label4.Location = new System.Drawing.Point(8, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(189, 21);
            this.label4.TabIndex = 86;
            this.label4.Text = "Custom Rule Exceptions";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel_FileFolder);
            this.panel1.Controls.Add(this.panel_Publisher_Scroll);
            this.panel1.Location = new System.Drawing.Point(15, 441);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(562, 230);
            this.panel1.TabIndex = 108;
            // 
            // Exceptions_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel_CustomRules);
            this.Name = "Exceptions_Control";
            this.Size = new System.Drawing.Size(812, 988);
            this.panel_CustomRules.ResumeLayout(false);
            this.panel_CustomRules.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel_FileFolder.ResumeLayout(false);
            this.panel_FileFolder.PerformLayout();
            this.panel_Publisher_Scroll.ResumeLayout(false);
            this.panel_Publisher_Scroll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Conditions)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_CustomRules;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Exception;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Type;
        private System.Windows.Forms.Label label_Error;
        private System.Windows.Forms.Button button_CreateException;
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
        private System.Windows.Forms.TextBox textBox_ReferenceFile;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.Label label_condition;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
    }
}
