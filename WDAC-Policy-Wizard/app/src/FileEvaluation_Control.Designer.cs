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
            label_Title = new System.Windows.Forms.Label();
            label_Description = new System.Windows.Forms.Label();
            button_AddFiles = new System.Windows.Forms.Button();
            button_RemoveFiles = new System.Windows.Forms.Button();
            button_Evaluate = new System.Windows.Forms.Button();
            resultsDataGridView = new System.Windows.Forms.DataGridView();
            label_FileCount = new System.Windows.Forms.Label();
            label_Status = new System.Windows.Forms.Label();
            progressBar_Evaluation = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)resultsDataGridView).BeginInit();
            SuspendLayout();
            // 
            // label_Title
            // 
            label_Title.AutoSize = true;
            label_Title.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label_Title.Location = new System.Drawing.Point(183, 19);
            label_Title.Name = "label_Title";
            label_Title.Size = new System.Drawing.Size(388, 29);
            label_Title.TabIndex = 0;
            label_Title.Text = "File Evaluation Against Active Policy";
            // 
            // label_Description
            // 
            label_Description.Font = new System.Drawing.Font("Tahoma", 9.5F);
            label_Description.Location = new System.Drawing.Point(183, 55);
            label_Description.Name = "label_Description";
            label_Description.Size = new System.Drawing.Size(850, 40);
            label_Description.TabIndex = 1;
            label_Description.Text = "Select one or more files to evaluate against the active system policy. Results reflect the currently enforced policy on this machine.";
            // 
            // button_AddFiles
            // 
            button_AddFiles.Font = new System.Drawing.Font("Tahoma", 9.5F);
            button_AddFiles.Location = new System.Drawing.Point(183, 125);
            button_AddFiles.Name = "button_AddFiles";
            button_AddFiles.Size = new System.Drawing.Size(140, 35);
            button_AddFiles.TabIndex = 2;
            button_AddFiles.Text = "+ Add Files";
            button_AddFiles.UseVisualStyleBackColor = true;
            button_AddFiles.Click += Button_AddFiles_Click;
            // 
            // button_RemoveFiles
            // 
            button_RemoveFiles.Font = new System.Drawing.Font("Tahoma", 9.5F);
            button_RemoveFiles.Location = new System.Drawing.Point(333, 125);
            button_RemoveFiles.Name = "button_RemoveFiles";
            button_RemoveFiles.Size = new System.Drawing.Size(155, 35);
            button_RemoveFiles.TabIndex = 3;
            button_RemoveFiles.Text = "- Remove Selected";
            button_RemoveFiles.UseVisualStyleBackColor = true;
            button_RemoveFiles.Click += Button_RemoveFiles_Click;
            // 
            // button_Evaluate
            // 
            button_Evaluate.Enabled = false;
            button_Evaluate.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            button_Evaluate.ForeColor = System.Drawing.Color.DodgerBlue;
            button_Evaluate.Location = new System.Drawing.Point(919, 125);
            button_Evaluate.Name = "button_Evaluate";
            button_Evaluate.Size = new System.Drawing.Size(160, 35);
            button_Evaluate.TabIndex = 4;
            button_Evaluate.Text = "Evaluate Files";
            button_Evaluate.UseVisualStyleBackColor = true;
            button_Evaluate.Click += Button_Evaluate_Click;
            // 
            // resultsDataGridView
            // 
            resultsDataGridView.AllowUserToAddRows = false;
            resultsDataGridView.AllowUserToDeleteRows = false;
            resultsDataGridView.BackgroundColor = System.Drawing.Color.White;
            resultsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resultsDataGridView.Location = new System.Drawing.Point(183, 175);
            resultsDataGridView.Name = "resultsDataGridView";
            resultsDataGridView.ReadOnly = true;
            resultsDataGridView.RowHeadersWidth = 30;
            resultsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            resultsDataGridView.Size = new System.Drawing.Size(900, 400);
            resultsDataGridView.TabIndex = 5;
            // 
            // label_FileCount
            // 
            label_FileCount.AutoSize = true;
            label_FileCount.Font = new System.Drawing.Font("Tahoma", 9F);
            label_FileCount.Location = new System.Drawing.Point(521, 134);
            label_FileCount.Name = "label_FileCount";
            label_FileCount.Size = new System.Drawing.Size(115, 18);
            label_FileCount.TabIndex = 6;
            label_FileCount.Text = "0 file(s) selected";
            // 
            // label_Status
            // 
            label_Status.Font = new System.Drawing.Font("Tahoma", 9.5F);
            label_Status.ForeColor = System.Drawing.Color.DodgerBlue;
            label_Status.Location = new System.Drawing.Point(183, 620);
            label_Status.Name = "label_Status";
            label_Status.Size = new System.Drawing.Size(900, 25);
            label_Status.TabIndex = 8;
            label_Status.Visible = false;
            // 
            // progressBar_Evaluation
            // 
            progressBar_Evaluation.Location = new System.Drawing.Point(183, 587);
            progressBar_Evaluation.Name = "progressBar_Evaluation";
            progressBar_Evaluation.Size = new System.Drawing.Size(900, 23);
            progressBar_Evaluation.TabIndex = 7;
            progressBar_Evaluation.Visible = false;
            // 
            // FileEvaluation_Control
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.White;
            Controls.Add(label_Status);
            Controls.Add(progressBar_Evaluation);
            Controls.Add(label_FileCount);
            Controls.Add(resultsDataGridView);
            Controls.Add(button_Evaluate);
            Controls.Add(button_RemoveFiles);
            Controls.Add(button_AddFiles);
            Controls.Add(label_Description);
            Controls.Add(label_Title);
            Name = "FileEvaluation_Control";
            Size = new System.Drawing.Size(1137, 650);
            ((System.ComponentModel.ISupportInitialize)resultsDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
    }
}
