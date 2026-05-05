// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace WDAC_Wizard
{
    partial class FileEvaluation_Control
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
            this.label_Title = new System.Windows.Forms.Label();
            this.label_Description = new System.Windows.Forms.Label();
            this.button_AddFiles = new System.Windows.Forms.Button();
            this.button_RemoveFiles = new System.Windows.Forms.Button();
            this.button_Evaluate = new System.Windows.Forms.Button();
            this.resultsDataGridView = new System.Windows.Forms.DataGridView();
            this.label_FileCount = new System.Windows.Forms.Label();
            this.label_Status = new System.Windows.Forms.Label();
            this.progressBar_Evaluation = new System.Windows.Forms.ProgressBar();
            this.label_Note = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.resultsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label_Title
            // 
            this.label_Title.AutoSize = true;
            this.label_Title.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Title.Location = new System.Drawing.Point(14, 14);
            this.label_Title.Name = "label_Title";
            this.label_Title.Size = new System.Drawing.Size(300, 29);
            this.label_Title.TabIndex = 0;
            this.label_Title.Text = "File Evaluation Against Active Policy";
            // 
            // label_Description
            // 
            this.label_Description.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.label_Description.Location = new System.Drawing.Point(14, 50);
            this.label_Description.Name = "label_Description";
            this.label_Description.Size = new System.Drawing.Size(850, 40);
            this.label_Description.TabIndex = 1;
            this.label_Description.Text = "Select one or more files to evaluate whether the active system WDAC policy allows," +
                " audits, or blocks their execution. Results reflect the currently enforced policy on this machine.";
            // 
            // label_Note
            // 
            this.label_Note.AutoSize = true;
            this.label_Note.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Italic);
            this.label_Note.ForeColor = System.Drawing.Color.Gray;
            this.label_Note.Location = new System.Drawing.Point(14, 90);
            this.label_Note.Name = "label_Note";
            this.label_Note.Size = new System.Drawing.Size(600, 17);
            this.label_Note.TabIndex = 9;
            this.label_Note.Text = "Note: This evaluates against the currently deployed/enforced WDAC policy on this system, not a policy XML file.";
            // 
            // button_AddFiles
            // 
            this.button_AddFiles.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.button_AddFiles.Location = new System.Drawing.Point(14, 120);
            this.button_AddFiles.Name = "button_AddFiles";
            this.button_AddFiles.Size = new System.Drawing.Size(140, 35);
            this.button_AddFiles.TabIndex = 2;
            this.button_AddFiles.Text = "+ Add Files";
            this.button_AddFiles.UseVisualStyleBackColor = true;
            this.button_AddFiles.Click += new System.EventHandler(this.Button_AddFiles_Click);
            // 
            // button_RemoveFiles
            // 
            this.button_RemoveFiles.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.button_RemoveFiles.Location = new System.Drawing.Point(164, 120);
            this.button_RemoveFiles.Name = "button_RemoveFiles";
            this.button_RemoveFiles.Size = new System.Drawing.Size(140, 35);
            this.button_RemoveFiles.TabIndex = 3;
            this.button_RemoveFiles.Text = "- Remove Selected";
            this.button_RemoveFiles.UseVisualStyleBackColor = true;
            this.button_RemoveFiles.Click += new System.EventHandler(this.Button_RemoveFiles_Click);
            // 
            // button_Evaluate
            // 
            this.button_Evaluate.Enabled = false;
            this.button_Evaluate.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.button_Evaluate.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_Evaluate.Location = new System.Drawing.Point(750, 120);
            this.button_Evaluate.Name = "button_Evaluate";
            this.button_Evaluate.Size = new System.Drawing.Size(160, 35);
            this.button_Evaluate.TabIndex = 4;
            this.button_Evaluate.Text = "Evaluate Files";
            this.button_Evaluate.UseVisualStyleBackColor = true;
            this.button_Evaluate.Click += new System.EventHandler(this.Button_Evaluate_Click);
            // 
            // label_FileCount
            // 
            this.label_FileCount.AutoSize = true;
            this.label_FileCount.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label_FileCount.Location = new System.Drawing.Point(320, 129);
            this.label_FileCount.Name = "label_FileCount";
            this.label_FileCount.Size = new System.Drawing.Size(100, 18);
            this.label_FileCount.TabIndex = 6;
            this.label_FileCount.Text = "0 file(s) selected";
            // 
            // resultsDataGridView
            // 
            this.resultsDataGridView.AllowUserToAddRows = false;
            this.resultsDataGridView.AllowUserToDeleteRows = false;
            this.resultsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.resultsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.resultsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.resultsDataGridView.Location = new System.Drawing.Point(14, 170);
            this.resultsDataGridView.Name = "resultsDataGridView";
            this.resultsDataGridView.ReadOnly = true;
            this.resultsDataGridView.RowHeadersWidth = 30;
            this.resultsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.resultsDataGridView.Size = new System.Drawing.Size(900, 400);
            this.resultsDataGridView.TabIndex = 5;
            // 
            // progressBar_Evaluation
            // 
            this.progressBar_Evaluation.Location = new System.Drawing.Point(14, 582);
            this.progressBar_Evaluation.Name = "progressBar_Evaluation";
            this.progressBar_Evaluation.Size = new System.Drawing.Size(900, 23);
            this.progressBar_Evaluation.TabIndex = 7;
            this.progressBar_Evaluation.Visible = false;
            // 
            // label_Status
            // 
            this.label_Status.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.label_Status.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label_Status.Location = new System.Drawing.Point(14, 615);
            this.label_Status.Name = "label_Status";
            this.label_Status.Size = new System.Drawing.Size(900, 25);
            this.label_Status.TabIndex = 8;
            this.label_Status.Text = "";
            this.label_Status.Visible = false;
            // 
            // FileEvaluation_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label_Note);
            this.Controls.Add(this.label_Status);
            this.Controls.Add(this.progressBar_Evaluation);
            this.Controls.Add(this.label_FileCount);
            this.Controls.Add(this.resultsDataGridView);
            this.Controls.Add(this.button_Evaluate);
            this.Controls.Add(this.button_RemoveFiles);
            this.Controls.Add(this.button_AddFiles);
            this.Controls.Add(this.label_Description);
            this.Controls.Add(this.label_Title);
            this.Name = "FileEvaluation_Control";
            this.Size = new System.Drawing.Size(930, 650);
            ((System.ComponentModel.ISupportInitialize)(this.resultsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label_Title;
        private System.Windows.Forms.Label label_Description;
        private System.Windows.Forms.Button button_AddFiles;
        private System.Windows.Forms.Button button_RemoveFiles;
        private System.Windows.Forms.Button button_Evaluate;
        private System.Windows.Forms.DataGridView resultsDataGridView;
        private System.Windows.Forms.Label label_FileCount;
        private System.Windows.Forms.ProgressBar progressBar_Evaluation;
        private System.Windows.Forms.Label label_Status;
        private System.Windows.Forms.Label label_Note;
    }
}
