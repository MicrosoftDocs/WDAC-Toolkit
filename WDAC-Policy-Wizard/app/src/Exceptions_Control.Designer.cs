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
            this.panel_ExceptionRule = new System.Windows.Forms.Panel();
            this.errorLabel = new System.Windows.Forms.Label();
            this.ruleCondition_Label = new System.Windows.Forms.Label();
            this.panel_FileFolder = new System.Windows.Forms.Panel();
            this.radioButton_Folder = new System.Windows.Forms.RadioButton();
            this.radioButton_File = new System.Windows.Forms.RadioButton();
            this.panel_Publisher_Scroll = new System.Windows.Forms.Panel();
            this.checkBoxCustomValues = new System.Windows.Forms.CheckBox();
            this.textBox_minversion = new System.Windows.Forms.TextBox();
            this.checkBox_InternalName = new System.Windows.Forms.CheckBox();
            this.textBox_internalname = new System.Windows.Forms.TextBox();
            this.checkBox_MinVersion = new System.Windows.Forms.CheckBox();
            this.checkBox_FileDescription = new System.Windows.Forms.CheckBox();
            this.textBox_product = new System.Windows.Forms.TextBox();
            this.checkBox_Product = new System.Windows.Forms.CheckBox();
            this.textBox_filedescription = new System.Windows.Forms.TextBox();
            this.checkBox_OriginalFilename = new System.Windows.Forms.CheckBox();
            this.textBox_originalfilename = new System.Windows.Forms.TextBox();
            this.dataGridView_Exceptions = new System.Windows.Forms.DataGridView();
            this.column_Action = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_Level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ruleCondition_static_Label = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.comboBox_ExceptionType = new System.Windows.Forms.ComboBox();
            this.textBox_ReferenceFile = new System.Windows.Forms.TextBox();
            this.button_Browse = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.panel_ExceptionRule.SuspendLayout();
            this.panel_FileFolder.SuspendLayout();
            this.panel_Publisher_Scroll.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Exceptions)).BeginInit();
            this.SuspendLayout();
            // 
            // panel_ExceptionRule
            // 
            this.panel_ExceptionRule.BackColor = System.Drawing.Color.White;
            this.panel_ExceptionRule.Controls.Add(this.errorLabel);
            this.panel_ExceptionRule.Controls.Add(this.ruleCondition_Label);
            this.panel_ExceptionRule.Controls.Add(this.panel_FileFolder);
            this.panel_ExceptionRule.Controls.Add(this.panel_Publisher_Scroll);
            this.panel_ExceptionRule.Controls.Add(this.dataGridView_Exceptions);
            this.panel_ExceptionRule.Controls.Add(this.ruleCondition_static_Label);
            this.panel_ExceptionRule.Controls.Add(this.label7);
            this.panel_ExceptionRule.Controls.Add(this.label8);
            this.panel_ExceptionRule.Controls.Add(this.comboBox_ExceptionType);
            this.panel_ExceptionRule.Controls.Add(this.textBox_ReferenceFile);
            this.panel_ExceptionRule.Controls.Add(this.button_Browse);
            this.panel_ExceptionRule.Controls.Add(this.label11);
            this.panel_ExceptionRule.Location = new System.Drawing.Point(130, 0);
            this.panel_ExceptionRule.Margin = new System.Windows.Forms.Padding(2);
            this.panel_ExceptionRule.Name = "panel_ExceptionRule";
            this.panel_ExceptionRule.Size = new System.Drawing.Size(704, 965);
            this.panel_ExceptionRule.TabIndex = 87;
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.Location = new System.Drawing.Point(7, 787);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(74, 17);
            this.errorLabel.TabIndex = 110;
            this.errorLabel.Text = "errorLabel";
            this.errorLabel.Visible = false;
            // 
            // ruleCondition_Label
            // 
            this.ruleCondition_Label.AutoSize = true;
            this.ruleCondition_Label.Font = new System.Drawing.Font("Tahoma", 8.7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ruleCondition_Label.ForeColor = System.Drawing.Color.Black;
            this.ruleCondition_Label.Location = new System.Drawing.Point(7, 143);
            this.ruleCondition_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ruleCondition_Label.Name = "ruleCondition_Label";
            this.ruleCondition_Label.Size = new System.Drawing.Size(103, 18);
            this.ruleCondition_Label.TabIndex = 109;
            this.ruleCondition_Label.Text = "Rule Condition:";
            this.ruleCondition_Label.Visible = false;
            // 
            // panel_FileFolder
            // 
            this.panel_FileFolder.Controls.Add(this.radioButton_Folder);
            this.panel_FileFolder.Controls.Add(this.radioButton_File);
            this.panel_FileFolder.Location = new System.Drawing.Point(493, 490);
            this.panel_FileFolder.Margin = new System.Windows.Forms.Padding(2);
            this.panel_FileFolder.Name = "panel_FileFolder";
            this.panel_FileFolder.Size = new System.Drawing.Size(140, 34);
            this.panel_FileFolder.TabIndex = 104;
            this.panel_FileFolder.Visible = false;
            // 
            // radioButton_Folder
            // 
            this.radioButton_Folder.AutoSize = true;
            this.radioButton_Folder.Font = new System.Drawing.Font("Tahoma", 9F);
            this.radioButton_Folder.ForeColor = System.Drawing.Color.Black;
            this.radioButton_Folder.Location = new System.Drawing.Point(61, 7);
            this.radioButton_Folder.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_Folder.Name = "radioButton_Folder";
            this.radioButton_Folder.Size = new System.Drawing.Size(68, 22);
            this.radioButton_Folder.TabIndex = 96;
            this.radioButton_Folder.TabStop = true;
            this.radioButton_Folder.Text = "Folder";
            this.radioButton_Folder.UseVisualStyleBackColor = true;
            this.radioButton_Folder.CheckedChanged += new System.EventHandler(this.FileFolderButtonClick);
            // 
            // radioButton_File
            // 
            this.radioButton_File.AutoSize = true;
            this.radioButton_File.Font = new System.Drawing.Font("Tahoma", 9F);
            this.radioButton_File.ForeColor = System.Drawing.Color.Black;
            this.radioButton_File.Location = new System.Drawing.Point(2, 7);
            this.radioButton_File.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_File.Name = "radioButton_File";
            this.radioButton_File.Size = new System.Drawing.Size(49, 22);
            this.radioButton_File.TabIndex = 95;
            this.radioButton_File.TabStop = true;
            this.radioButton_File.Text = "File";
            this.radioButton_File.UseVisualStyleBackColor = true;
            this.radioButton_File.Click += new System.EventHandler(this.FileFolderButtonClick);
            // 
            // panel_Publisher_Scroll
            // 
            this.panel_Publisher_Scroll.Controls.Add(this.checkBoxCustomValues);
            this.panel_Publisher_Scroll.Controls.Add(this.textBox_minversion);
            this.panel_Publisher_Scroll.Controls.Add(this.checkBox_InternalName);
            this.panel_Publisher_Scroll.Controls.Add(this.textBox_internalname);
            this.panel_Publisher_Scroll.Controls.Add(this.checkBox_MinVersion);
            this.panel_Publisher_Scroll.Controls.Add(this.checkBox_FileDescription);
            this.panel_Publisher_Scroll.Controls.Add(this.textBox_product);
            this.panel_Publisher_Scroll.Controls.Add(this.checkBox_Product);
            this.panel_Publisher_Scroll.Controls.Add(this.textBox_filedescription);
            this.panel_Publisher_Scroll.Controls.Add(this.checkBox_OriginalFilename);
            this.panel_Publisher_Scroll.Controls.Add(this.textBox_originalfilename);
            this.panel_Publisher_Scroll.Location = new System.Drawing.Point(8, 492);
            this.panel_Publisher_Scroll.Margin = new System.Windows.Forms.Padding(2);
            this.panel_Publisher_Scroll.Name = "panel_Publisher_Scroll";
            this.panel_Publisher_Scroll.Size = new System.Drawing.Size(494, 254);
            this.panel_Publisher_Scroll.TabIndex = 108;
            this.panel_Publisher_Scroll.Visible = false;
            // 
            // checkBoxCustomValues
            // 
            this.checkBoxCustomValues.AutoSize = true;
            this.checkBoxCustomValues.Location = new System.Drawing.Point(9, 227);
            this.checkBoxCustomValues.Name = "checkBoxCustomValues";
            this.checkBoxCustomValues.Size = new System.Drawing.Size(153, 21);
            this.checkBoxCustomValues.TabIndex = 126;
            this.checkBoxCustomValues.Text = "Use Custom Values";
            this.checkBoxCustomValues.UseVisualStyleBackColor = true;
            this.checkBoxCustomValues.CheckedChanged += new System.EventHandler(this.checkBoxCustomValues_CheckedChanged);
            // 
            // textBox_minversion
            // 
            this.textBox_minversion.BackColor = System.Drawing.SystemColors.Control;
            this.textBox_minversion.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_minversion.Location = new System.Drawing.Point(150, 184);
            this.textBox_minversion.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_minversion.Name = "textBox_minversion";
            this.textBox_minversion.ReadOnly = true;
            this.textBox_minversion.Size = new System.Drawing.Size(336, 26);
            this.textBox_minversion.TabIndex = 125;
            // 
            // checkBox_InternalName
            // 
            this.checkBox_InternalName.AutoSize = true;
            this.checkBox_InternalName.Location = new System.Drawing.Point(9, 147);
            this.checkBox_InternalName.Name = "checkBox_InternalName";
            this.checkBox_InternalName.Size = new System.Drawing.Size(122, 21);
            this.checkBox_InternalName.TabIndex = 124;
            this.checkBox_InternalName.Text = "Internal Name:";
            this.checkBox_InternalName.UseVisualStyleBackColor = true;
            this.checkBox_InternalName.CheckedChanged += new System.EventHandler(this.checkBox_InternalName_CheckedChanged);
            // 
            // textBox_internalname
            // 
            this.textBox_internalname.BackColor = System.Drawing.SystemColors.Control;
            this.textBox_internalname.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_internalname.Location = new System.Drawing.Point(150, 143);
            this.textBox_internalname.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_internalname.Name = "textBox_internalname";
            this.textBox_internalname.ReadOnly = true;
            this.textBox_internalname.Size = new System.Drawing.Size(336, 26);
            this.textBox_internalname.TabIndex = 103;
            // 
            // checkBox_MinVersion
            // 
            this.checkBox_MinVersion.AutoSize = true;
            this.checkBox_MinVersion.Location = new System.Drawing.Point(9, 189);
            this.checkBox_MinVersion.Name = "checkBox_MinVersion";
            this.checkBox_MinVersion.Size = new System.Drawing.Size(112, 21);
            this.checkBox_MinVersion.TabIndex = 123;
            this.checkBox_MinVersion.Text = "Min. Version:";
            this.checkBox_MinVersion.UseVisualStyleBackColor = true;
            this.checkBox_MinVersion.CheckedChanged += new System.EventHandler(this.checkBox_MinVersion_CheckedChanged);
            // 
            // checkBox_FileDescription
            // 
            this.checkBox_FileDescription.AutoSize = true;
            this.checkBox_FileDescription.Location = new System.Drawing.Point(9, 63);
            this.checkBox_FileDescription.Name = "checkBox_FileDescription";
            this.checkBox_FileDescription.Size = new System.Drawing.Size(127, 21);
            this.checkBox_FileDescription.TabIndex = 122;
            this.checkBox_FileDescription.Text = "File Description";
            this.checkBox_FileDescription.UseVisualStyleBackColor = true;
            this.checkBox_FileDescription.CheckedChanged += new System.EventHandler(this.checkBox_FileDescription_CheckedChanged);
            // 
            // textBox_product
            // 
            this.textBox_product.BackColor = System.Drawing.SystemColors.Control;
            this.textBox_product.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_product.Location = new System.Drawing.Point(150, 102);
            this.textBox_product.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_product.Name = "textBox_product";
            this.textBox_product.ReadOnly = true;
            this.textBox_product.Size = new System.Drawing.Size(336, 26);
            this.textBox_product.TabIndex = 101;
            // 
            // checkBox_Product
            // 
            this.checkBox_Product.AutoSize = true;
            this.checkBox_Product.Location = new System.Drawing.Point(9, 105);
            this.checkBox_Product.Name = "checkBox_Product";
            this.checkBox_Product.Size = new System.Drawing.Size(83, 21);
            this.checkBox_Product.TabIndex = 121;
            this.checkBox_Product.Text = "Product:";
            this.checkBox_Product.UseVisualStyleBackColor = true;
            this.checkBox_Product.CheckedChanged += new System.EventHandler(this.checkBox_Product_CheckedChanged);
            // 
            // textBox_filedescription
            // 
            this.textBox_filedescription.BackColor = System.Drawing.SystemColors.Control;
            this.textBox_filedescription.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_filedescription.Location = new System.Drawing.Point(150, 61);
            this.textBox_filedescription.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_filedescription.Name = "textBox_filedescription";
            this.textBox_filedescription.ReadOnly = true;
            this.textBox_filedescription.Size = new System.Drawing.Size(336, 26);
            this.textBox_filedescription.TabIndex = 99;
            // 
            // checkBox_OriginalFilename
            // 
            this.checkBox_OriginalFilename.AutoSize = true;
            this.checkBox_OriginalFilename.Location = new System.Drawing.Point(9, 21);
            this.checkBox_OriginalFilename.Name = "checkBox_OriginalFilename";
            this.checkBox_OriginalFilename.Size = new System.Drawing.Size(144, 21);
            this.checkBox_OriginalFilename.TabIndex = 120;
            this.checkBox_OriginalFilename.Text = "Original Filename:";
            this.checkBox_OriginalFilename.UseVisualStyleBackColor = true;
            this.checkBox_OriginalFilename.CheckedChanged += new System.EventHandler(this.checkBox_OriginalFilename_CheckedChanged);
            // 
            // textBox_originalfilename
            // 
            this.textBox_originalfilename.BackColor = System.Drawing.SystemColors.Control;
            this.textBox_originalfilename.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_originalfilename.Location = new System.Drawing.Point(150, 20);
            this.textBox_originalfilename.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_originalfilename.Name = "textBox_originalfilename";
            this.textBox_originalfilename.ReadOnly = true;
            this.textBox_originalfilename.Size = new System.Drawing.Size(336, 26);
            this.textBox_originalfilename.TabIndex = 95;
            // 
            // dataGridView_Exceptions
            // 
            this.dataGridView_Exceptions.AllowUserToDeleteRows = false;
            this.dataGridView_Exceptions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView_Exceptions.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridView_Exceptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Exceptions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.column_Action,
            this.column_Level,
            this.column_Name});
            this.dataGridView_Exceptions.Location = new System.Drawing.Point(7, 270);
            this.dataGridView_Exceptions.Name = "dataGridView_Exceptions";
            this.dataGridView_Exceptions.ReadOnly = true;
            this.dataGridView_Exceptions.RowHeadersWidth = 51;
            this.dataGridView_Exceptions.RowTemplate.Height = 24;
            this.dataGridView_Exceptions.Size = new System.Drawing.Size(621, 150);
            this.dataGridView_Exceptions.TabIndex = 107;
            this.dataGridView_Exceptions.VirtualMode = true;
            this.dataGridView_Exceptions.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.DataGridView_Exceptions_CellValueNeeded);
            // 
            // column_Action
            // 
            this.column_Action.HeaderText = "Action";
            this.column_Action.MinimumWidth = 8;
            this.column_Action.Name = "column_Action";
            this.column_Action.ReadOnly = true;
            this.column_Action.Width = 76;
            // 
            // column_Level
            // 
            this.column_Level.HeaderText = "Type";
            this.column_Level.MinimumWidth = 6;
            this.column_Level.Name = "column_Level";
            this.column_Level.ReadOnly = true;
            this.column_Level.Width = 69;
            // 
            // column_Name
            // 
            this.column_Name.HeaderText = "Exception";
            this.column_Name.MinimumWidth = 6;
            this.column_Name.Name = "column_Name";
            this.column_Name.ReadOnly = true;
            this.column_Name.Width = 98;
            // 
            // ruleCondition_static_Label
            // 
            this.ruleCondition_static_Label.AutoSize = true;
            this.ruleCondition_static_Label.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ruleCondition_static_Label.ForeColor = System.Drawing.Color.Black;
            this.ruleCondition_static_Label.Location = new System.Drawing.Point(7, 125);
            this.ruleCondition_static_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ruleCondition_static_Label.Name = "ruleCondition_static_Label";
            this.ruleCondition_static_Label.Size = new System.Drawing.Size(122, 18);
            this.ruleCondition_static_Label.TabIndex = 95;
            this.ruleCondition_static_Label.Text = "Rule Condition:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(7, 65);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(606, 36);
            this.label7.TabIndex = 94;
            this.label7.Text = "Exceptions allow you to exclude files that would be included in the rule. Select " +
    "the exception \r\ntype and browse for the reference file off which to base the exc" +
    "eption.  ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(7, 210);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(114, 18);
            this.label8.TabIndex = 89;
            this.label8.Text = "Exception Type:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // comboBox_ExceptionType
            // 
            this.comboBox_ExceptionType.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox_ExceptionType.FormattingEnabled = true;
            this.comboBox_ExceptionType.Items.AddRange(new object[] {
            "File Path",
            "File Attributes",
            "File Hash"});
            this.comboBox_ExceptionType.Location = new System.Drawing.Point(7, 239);
            this.comboBox_ExceptionType.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox_ExceptionType.Name = "comboBox_ExceptionType";
            this.comboBox_ExceptionType.Size = new System.Drawing.Size(187, 26);
            this.comboBox_ExceptionType.TabIndex = 89;
            this.comboBox_ExceptionType.SelectedIndexChanged += new System.EventHandler(this.ComboBox_ExceptionType_SelectedIndexChanged);
            // 
            // textBox_ReferenceFile
            // 
            this.textBox_ReferenceFile.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_ReferenceFile.Location = new System.Drawing.Point(7, 456);
            this.textBox_ReferenceFile.Margin = new System.Windows.Forms.Padding(2);
            this.textBox_ReferenceFile.Name = "textBox_ReferenceFile";
            this.textBox_ReferenceFile.Size = new System.Drawing.Size(467, 26);
            this.textBox_ReferenceFile.TabIndex = 88;
            // 
            // button_Browse
            // 
            this.button_Browse.BackColor = System.Drawing.Color.Transparent;
            this.button_Browse.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.button_Browse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Browse.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Browse.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_Browse.Location = new System.Drawing.Point(493, 455);
            this.button_Browse.Margin = new System.Windows.Forms.Padding(2);
            this.button_Browse.Name = "button_Browse";
            this.button_Browse.Size = new System.Drawing.Size(107, 28);
            this.button_Browse.TabIndex = 84;
            this.button_Browse.Text = "Browse";
            this.button_Browse.UseVisualStyleBackColor = false;
            this.button_Browse.Click += new System.EventHandler(this.Button_Browse_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Location = new System.Drawing.Point(7, 430);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(104, 18);
            this.label11.TabIndex = 87;
            this.label11.Text = "Reference File:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Exceptions_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel_ExceptionRule);
            this.Name = "Exceptions_Control";
            this.Size = new System.Drawing.Size(1001, 988);
            this.Load += new System.EventHandler(this.Exceptions_Control_Load);
            this.Validated += new System.EventHandler(this.Exceptions_Control_Validated);
            this.panel_ExceptionRule.ResumeLayout(false);
            this.panel_ExceptionRule.PerformLayout();
            this.panel_FileFolder.ResumeLayout(false);
            this.panel_FileFolder.PerformLayout();
            this.panel_Publisher_Scroll.ResumeLayout(false);
            this.panel_Publisher_Scroll.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Exceptions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel_ExceptionRule;
        private System.Windows.Forms.Panel panel_FileFolder;
        private System.Windows.Forms.RadioButton radioButton_Folder;
        private System.Windows.Forms.RadioButton radioButton_File;
        private System.Windows.Forms.DataGridView dataGridView_Exceptions;
        private System.Windows.Forms.Label ruleCondition_static_Label;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox comboBox_ExceptionType;
        private System.Windows.Forms.TextBox textBox_ReferenceFile;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel_Publisher_Scroll;
        private System.Windows.Forms.TextBox textBox_internalname;
        private System.Windows.Forms.TextBox textBox_product;
        private System.Windows.Forms.TextBox textBox_filedescription;
        private System.Windows.Forms.TextBox textBox_originalfilename;
        private System.Windows.Forms.Label ruleCondition_Label;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Action;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Level;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Name;
        private System.Windows.Forms.TextBox textBox_minversion;
        private System.Windows.Forms.CheckBox checkBox_InternalName;
        private System.Windows.Forms.CheckBox checkBox_MinVersion;
        private System.Windows.Forms.CheckBox checkBox_FileDescription;
        private System.Windows.Forms.CheckBox checkBox_Product;
        private System.Windows.Forms.CheckBox checkBox_OriginalFilename;
        private System.Windows.Forms.CheckBox checkBoxCustomValues;
        private System.Windows.Forms.Label errorLabel;
    }
}
