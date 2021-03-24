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
            this.button_Create = new System.Windows.Forms.Button();
            this.textBoxPolicyPath = new System.Windows.Forms.TextBox();
            this.policyInfoPanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_PolicyName = new System.Windows.Forms.TextBox();
            this.label_policyName = new System.Windows.Forms.Label();
            this.textBox_PolicyID = new System.Windows.Forms.TextBox();
            this.label_fileLocation = new System.Windows.Forms.Label();
            this.button_ParseEventLog = new System.Windows.Forms.Button();
            this.button_Parse_LogFile = new System.Windows.Forms.Button();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.panel_Progress = new System.Windows.Forms.Panel();
            this.label_Progress = new System.Windows.Forms.Label();
            this.pictureBox_Progress = new System.Windows.Forms.PictureBox();
            this.policyInfoPanel.SuspendLayout();
            this.panel_Progress.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Progress)).BeginInit();
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
            this.label2.Size = new System.Drawing.Size(236, 21);
            this.label2.TabIndex = 108;
            this.label2.Text = "Browse for your policy on disk.";
            // 
            // button_Create
            // 
            this.button_Create.FlatAppearance.BorderColor = System.Drawing.SystemColors.MenuHighlight;
            this.button_Create.FlatAppearance.BorderSize = 2;
            this.button_Create.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Create.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Create.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_Create.Location = new System.Drawing.Point(665, 132);
            this.button_Create.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.button_Create.Name = "button_Create";
            this.button_Create.Size = new System.Drawing.Size(118, 37);
            this.button_Create.TabIndex = 109;
            this.button_Create.Text = "Browse";
            this.button_Create.UseVisualStyleBackColor = true;
            this.button_Create.Click += new System.EventHandler(this.button_Create_Click);
            // 
            // textBoxPolicyPath
            // 
            this.textBoxPolicyPath.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBoxPolicyPath.Location = new System.Drawing.Point(172, 136);
            this.textBoxPolicyPath.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxPolicyPath.Name = "textBoxPolicyPath";
            this.textBoxPolicyPath.Size = new System.Drawing.Size(462, 26);
            this.textBoxPolicyPath.TabIndex = 110;
            // 
            // policyInfoPanel
            // 
            this.policyInfoPanel.Controls.Add(this.label5);
            this.policyInfoPanel.Controls.Add(this.textBox_PolicyName);
            this.policyInfoPanel.Controls.Add(this.label_policyName);
            this.policyInfoPanel.Controls.Add(this.textBox_PolicyID);
            this.policyInfoPanel.Controls.Add(this.label_fileLocation);
            this.policyInfoPanel.Location = new System.Drawing.Point(167, 192);
            this.policyInfoPanel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.policyInfoPanel.Name = "policyInfoPanel";
            this.policyInfoPanel.Size = new System.Drawing.Size(632, 151);
            this.policyInfoPanel.TabIndex = 111;
            this.policyInfoPanel.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(13, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(350, 21);
            this.label5.TabIndex = 11;
            this.label5.Text = "Edit the policy name and ID, if you would like.\r\n";
            // 
            // textBox_PolicyName
            // 
            this.textBox_PolicyName.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_PolicyName.Location = new System.Drawing.Point(123, 54);
            this.textBox_PolicyName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_PolicyName.Name = "textBox_PolicyName";
            this.textBox_PolicyName.Size = new System.Drawing.Size(351, 26);
            this.textBox_PolicyName.TabIndex = 9;
            this.textBox_PolicyName.TextChanged += new System.EventHandler(this.textBox_PolicyName_TextChanged);
            // 
            // label_policyName
            // 
            this.label_policyName.AutoSize = true;
            this.label_policyName.Font = new System.Drawing.Font("Tahoma", 10F);
            this.label_policyName.ForeColor = System.Drawing.Color.Black;
            this.label_policyName.Location = new System.Drawing.Point(13, 54);
            this.label_policyName.Name = "label_policyName";
            this.label_policyName.Size = new System.Drawing.Size(106, 21);
            this.label_policyName.TabIndex = 8;
            this.label_policyName.Text = "Policy Name:";
            // 
            // textBox_PolicyID
            // 
            this.textBox_PolicyID.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_PolicyID.Location = new System.Drawing.Point(123, 99);
            this.textBox_PolicyID.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_PolicyID.Name = "textBox_PolicyID";
            this.textBox_PolicyID.Size = new System.Drawing.Size(351, 26);
            this.textBox_PolicyID.TabIndex = 7;
            this.textBox_PolicyID.TextChanged += new System.EventHandler(this.textBox_PolicyID_TextChanged);
            // 
            // label_fileLocation
            // 
            this.label_fileLocation.AutoSize = true;
            this.label_fileLocation.Font = new System.Drawing.Font("Tahoma", 10F);
            this.label_fileLocation.ForeColor = System.Drawing.Color.Black;
            this.label_fileLocation.Location = new System.Drawing.Point(13, 100);
            this.label_fileLocation.Name = "label_fileLocation";
            this.label_fileLocation.Size = new System.Drawing.Size(81, 21);
            this.label_fileLocation.TabIndex = 6;
            this.label_fileLocation.Text = "Policy ID:";
            // 
            // button_ParseEventLog
            // 
            this.button_ParseEventLog.Location = new System.Drawing.Point(169, 375);
            this.button_ParseEventLog.Name = "button_ParseEventLog";
            this.button_ParseEventLog.Size = new System.Drawing.Size(145, 23);
            this.button_ParseEventLog.TabIndex = 112;
            this.button_ParseEventLog.Text = "Parse Event Log";
            this.button_ParseEventLog.UseVisualStyleBackColor = true;
            this.button_ParseEventLog.Click += new System.EventHandler(this.button_ParseEventLog_Click);
            // 
            // button_Parse_LogFile
            // 
            this.button_Parse_LogFile.Location = new System.Drawing.Point(169, 404);
            this.button_Parse_LogFile.Name = "button_Parse_LogFile";
            this.button_Parse_LogFile.Size = new System.Drawing.Size(145, 23);
            this.button_Parse_LogFile.TabIndex = 113;
            this.button_Parse_LogFile.Text = "Parse Log File";
            this.button_Parse_LogFile.UseVisualStyleBackColor = true;
            this.button_Parse_LogFile.Click += new System.EventHandler(this.button_Parse_LogFile_Click);
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerReportsProgress = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // panel_Progress
            // 
            this.panel_Progress.Controls.Add(this.label_Progress);
            this.panel_Progress.Controls.Add(this.pictureBox_Progress);
            this.panel_Progress.Location = new System.Drawing.Point(378, 375);
            this.panel_Progress.Name = "panel_Progress";
            this.panel_Progress.Size = new System.Drawing.Size(280, 179);
            this.panel_Progress.TabIndex = 114;
            this.panel_Progress.Visible = false;
            // 
            // label_Progress
            // 
            this.label_Progress.AutoSize = true;
            this.label_Progress.Location = new System.Drawing.Point(59, 10);
            this.label_Progress.Name = "label_Progress";
            this.label_Progress.Size = new System.Drawing.Size(154, 17);
            this.label_Progress.TabIndex = 1;
            this.label_Progress.Text = "23 / 137 Rules Created";
            // 
            // pictureBox_Progress
            // 
            this.pictureBox_Progress.Image = global::WDAC_Wizard.Properties.Resources.eventlog_progress;
            this.pictureBox_Progress.InitialImage = global::WDAC_Wizard.Properties.Resources.eventlog_progress;
            this.pictureBox_Progress.Location = new System.Drawing.Point(35, 51);
            this.pictureBox_Progress.Name = "pictureBox_Progress";
            this.pictureBox_Progress.Size = new System.Drawing.Size(202, 114);
            this.pictureBox_Progress.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_Progress.TabIndex = 0;
            this.pictureBox_Progress.TabStop = false;
            // 
            // EditWorkflow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel_Progress);
            this.Controls.Add(this.button_Parse_LogFile);
            this.Controls.Add(this.button_ParseEventLog);
            this.Controls.Add(this.policyInfoPanel);
            this.Controls.Add(this.textBoxPolicyPath);
            this.Controls.Add(this.button_Create);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "EditWorkflow";
            this.Size = new System.Drawing.Size(1208, 708);
            this.policyInfoPanel.ResumeLayout(false);
            this.policyInfoPanel.PerformLayout();
            this.panel_Progress.ResumeLayout(false);
            this.panel_Progress.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Progress)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Create;
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
    }
}
