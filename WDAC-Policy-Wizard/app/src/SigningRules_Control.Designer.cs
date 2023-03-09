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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SigningRules_Control));
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.rulesDataGrid = new System.Windows.Forms.DataGridView();
            this.column_Action = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_Level = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Files = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Exceptions = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.deleteButton = new System.Windows.Forms.Button();
            this.label_Error = new System.Windows.Forms.Label();
            this.label_AddCustomRules = new System.Windows.Forms.Button();
            this.checkBox_KernelList = new System.Windows.Forms.CheckBox();
            this.checkBox_UserModeList = new System.Windows.Forms.CheckBox();
            this.panel_Progress = new System.Windows.Forms.Panel();
            this.label_Progress = new System.Windows.Forms.Label();
            this.pictureBox_Progress = new System.Windows.Forms.PictureBox();
            this.backgroundWorkerRulesDeleter = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.rulesDataGrid)).BeginInit();
            this.panel_Progress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Progress)).BeginInit();
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
            this.label1.Location = new System.Drawing.Point(162, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 29);
            this.label1.TabIndex = 47;
            this.label1.Text = "File Rules";
            // 
            // rulesDataGrid
            // 
            this.rulesDataGrid.AllowUserToDeleteRows = false;
            this.rulesDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.rulesDataGrid.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.rulesDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.rulesDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.column_Action,
            this.column_Level,
            this.Column_Name,
            this.Column_Files,
            this.Column_Exceptions,
            this.column_ID});
            this.rulesDataGrid.Location = new System.Drawing.Point(163, 162);
            this.rulesDataGrid.Margin = new System.Windows.Forms.Padding(2);
            this.rulesDataGrid.Name = "rulesDataGrid";
            this.rulesDataGrid.ReadOnly = true;
            this.rulesDataGrid.RowHeadersVisible = false;
            this.rulesDataGrid.RowHeadersWidth = 70;
            this.rulesDataGrid.RowTemplate.Height = 24;
            this.rulesDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.rulesDataGrid.Size = new System.Drawing.Size(879, 440);
            this.rulesDataGrid.TabIndex = 92;
            this.rulesDataGrid.VirtualMode = true;
            this.rulesDataGrid.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.RulesDataGrid_CellValueNeeded);
            // 
            // column_Action
            // 
            this.column_Action.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.column_Action.HeaderText = "Action";
            this.column_Action.MinimumWidth = 6;
            this.column_Action.Name = "column_Action";
            this.column_Action.ReadOnly = true;
            this.column_Action.Width = 76;
            // 
            // column_Level
            // 
            this.column_Level.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.column_Level.HeaderText = "Level";
            this.column_Level.MinimumWidth = 6;
            this.column_Level.Name = "column_Level";
            this.column_Level.ReadOnly = true;
            this.column_Level.Width = 71;
            // 
            // Column_Name
            // 
            this.Column_Name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_Name.HeaderText = "Name";
            this.Column_Name.MinimumWidth = 6;
            this.Column_Name.Name = "Column_Name";
            this.Column_Name.ReadOnly = true;
            this.Column_Name.Width = 74;
            // 
            // Column_Files
            // 
            this.Column_Files.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_Files.HeaderText = "Associated Files";
            this.Column_Files.MinimumWidth = 6;
            this.Column_Files.Name = "Column_Files";
            this.Column_Files.ReadOnly = true;
            this.Column_Files.Width = 127;
            // 
            // Column_Exceptions
            // 
            this.Column_Exceptions.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Column_Exceptions.HeaderText = "Exceptions";
            this.Column_Exceptions.MinimumWidth = 6;
            this.Column_Exceptions.Name = "Column_Exceptions";
            this.Column_Exceptions.ReadOnly = true;
            this.Column_Exceptions.Width = 105;
            // 
            // column_ID
            // 
            this.column_ID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.column_ID.HeaderText = "Rule ID";
            this.column_ID.MinimumWidth = 8;
            this.column_ID.Name = "column_ID";
            this.column_ID.ReadOnly = true;
            this.column_ID.Width = 77;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(162, 132);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(187, 21);
            this.label8.TabIndex = 91;
            this.label8.Text = "Policy Signing Rules List";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(162, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(591, 18);
            this.label3.TabIndex = 80;
            this.label3.Text = "Create allow or deny rules for files based on publisher, path, file attributes or" +
    " hash values.";
            // 
            // deleteButton
            // 
            this.deleteButton.FlatAppearance.BorderSize = 0;
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteButton.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteButton.Image = global::WDAC_Wizard.Properties.Resources.minus_button;
            this.deleteButton.Location = new System.Drawing.Point(903, 607);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(154, 26);
            this.deleteButton.TabIndex = 93;
            this.deleteButton.Text = "   Remove Rule";
            this.deleteButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.deleteButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // label_Error
            // 
            this.label_Error.AutoSize = true;
            this.label_Error.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Error.ForeColor = System.Drawing.Color.Red;
            this.label_Error.Location = new System.Drawing.Point(152, 666);
            this.label_Error.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Error.Name = "label_Error";
            this.label_Error.Size = new System.Drawing.Size(648, 18);
            this.label_Error.TabIndex = 96;
            this.label_Error.Text = "Label_Error: Lorem Ipsum text text text text. Lorum Ipsum text text text text tex" +
    "t text text text";
            this.label_Error.Visible = false;
            // 
            // label_AddCustomRules
            // 
            this.label_AddCustomRules.FlatAppearance.BorderSize = 0;
            this.label_AddCustomRules.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_AddCustomRules.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_AddCustomRules.Image = ((System.Drawing.Image)(resources.GetObject("label_AddCustomRules.Image")));
            this.label_AddCustomRules.Location = new System.Drawing.Point(881, 132);
            this.label_AddCustomRules.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.label_AddCustomRules.Name = "label_AddCustomRules";
            this.label_AddCustomRules.Size = new System.Drawing.Size(174, 26);
            this.label_AddCustomRules.TabIndex = 97;
            this.label_AddCustomRules.Text = "   Add Custom Rule";
            this.label_AddCustomRules.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label_AddCustomRules.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.label_AddCustomRules.UseVisualStyleBackColor = true;
            this.label_AddCustomRules.Click += new System.EventHandler(this.Label_AddCustomRules_Click);
            // 
            // checkBox_KernelList
            // 
            this.checkBox_KernelList.AutoSize = true;
            this.checkBox_KernelList.Location = new System.Drawing.Point(163, 634);
            this.checkBox_KernelList.Name = "checkBox_KernelList";
            this.checkBox_KernelList.Size = new System.Drawing.Size(320, 21);
            this.checkBox_KernelList.TabIndex = 98;
            this.checkBox_KernelList.Text = "Merge with Recommended Kernel Block Rules";
            this.checkBox_KernelList.UseVisualStyleBackColor = true;
            this.checkBox_KernelList.CheckedChanged += new System.EventHandler(this.CheckBox_KernelList_CheckedChanged);
            // 
            // checkBox_UserModeList
            // 
            this.checkBox_UserModeList.AutoSize = true;
            this.checkBox_UserModeList.Location = new System.Drawing.Point(163, 607);
            this.checkBox_UserModeList.Name = "checkBox_UserModeList";
            this.checkBox_UserModeList.Size = new System.Drawing.Size(348, 21);
            this.checkBox_UserModeList.TabIndex = 99;
            this.checkBox_UserModeList.Text = "Merge with Recommended User Mode Block Rules";
            this.checkBox_UserModeList.UseVisualStyleBackColor = true;
            this.checkBox_UserModeList.CheckedChanged += new System.EventHandler(this.CheckBox_UserModeList_CheckedChanged);
            // 
            // panel_Progress
            // 
            this.panel_Progress.Controls.Add(this.label_Progress);
            this.panel_Progress.Controls.Add(this.pictureBox_Progress);
            this.panel_Progress.Location = new System.Drawing.Point(527, 270);
            this.panel_Progress.Name = "panel_Progress";
            this.panel_Progress.Size = new System.Drawing.Size(156, 168);
            this.panel_Progress.TabIndex = 115;
            this.panel_Progress.Visible = false;
            // 
            // label_Progress
            // 
            this.label_Progress.AutoSize = true;
            this.label_Progress.Location = new System.Drawing.Point(11, 2);
            this.label_Progress.Name = "label_Progress";
            this.label_Progress.Size = new System.Drawing.Size(111, 17);
            this.label_Progress.TabIndex = 1;
            this.label_Progress.Text = "Removing Rules";
            this.label_Progress.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pictureBox_Progress
            // 
            this.pictureBox_Progress.Image = global::WDAC_Wizard.Properties.Resources.loading;
            this.pictureBox_Progress.InitialImage = global::WDAC_Wizard.Properties.Resources.loading;
            this.pictureBox_Progress.Location = new System.Drawing.Point(12, 32);
            this.pictureBox_Progress.Name = "pictureBox_Progress";
            this.pictureBox_Progress.Size = new System.Drawing.Size(128, 128);
            this.pictureBox_Progress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox_Progress.TabIndex = 0;
            this.pictureBox_Progress.TabStop = false;
            // 
            // backgroundWorkerRulesDeleter
            // 
            this.backgroundWorkerRulesDeleter.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoWorkBackgroundWorker);
            this.backgroundWorkerRulesDeleter.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.FinishedBackgroundWorker);
            // 
            // SigningRules_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.Controls.Add(this.panel_Progress);
            this.Controls.Add(this.checkBox_UserModeList);
            this.Controls.Add(this.checkBox_KernelList);
            this.Controls.Add(this.label_AddCustomRules);
            this.Controls.Add(this.label_Error);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rulesDataGrid);
            this.Controls.Add(this.label3);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SigningRules_Control";
            this.Size = new System.Drawing.Size(1203, 725);
            this.Load += new System.EventHandler(this.SigningRules_Control_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rulesDataGrid)).EndInit();
            this.panel_Progress.ResumeLayout(false);
            this.panel_Progress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Progress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView rulesDataGrid;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Label label_Error;
        private System.Windows.Forms.Button label_AddCustomRules;
        private System.Windows.Forms.CheckBox checkBox_KernelList;
        private System.Windows.Forms.CheckBox checkBox_UserModeList;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Action;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_Level;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Name;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Files;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Exceptions;
        private System.Windows.Forms.DataGridViewTextBoxColumn column_ID;
        private System.Windows.Forms.Panel panel_Progress;
        private System.Windows.Forms.Label label_Progress;
        private System.Windows.Forms.PictureBox pictureBox_Progress;
        private System.ComponentModel.BackgroundWorker backgroundWorkerRulesDeleter;
    }
}
