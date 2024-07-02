// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

namespace WDAC_Wizard
{
    partial class BuildPage
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
            label_WaitMsg = new System.Windows.Forms.Label();
            progressBar = new System.Windows.Forms.ProgressBar();
            finishLabel = new System.Windows.Forms.Label();
            progress_Label = new System.Windows.Forms.Label();
            xmlFilePathLabel = new System.Windows.Forms.Label();
            finishPanel = new System.Windows.Forms.Panel();
            binFilePathLabel = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            progressString_Label = new System.Windows.Forms.Label();
            finishPanel.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label1.ForeColor = System.Drawing.Color.Black;
            label1.Location = new System.Drawing.Point(167, 47);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(290, 29);
            label1.TabIndex = 48;
            label1.Text = "Building your WDAC Policy";
            // 
            // label_WaitMsg
            // 
            label_WaitMsg.AutoSize = true;
            label_WaitMsg.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label_WaitMsg.ForeColor = System.Drawing.Color.Black;
            label_WaitMsg.Location = new System.Drawing.Point(164, 203);
            label_WaitMsg.Name = "label_WaitMsg";
            label_WaitMsg.Size = new System.Drawing.Size(481, 18);
            label_WaitMsg.TabIndex = 81;
            label_WaitMsg.Text = "The wizard is building your WDAC policy. This may take several minutes.";
            // 
            // progressBar
            // 
            progressBar.Location = new System.Drawing.Point(232, 149);
            progressBar.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(362, 28);
            progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            progressBar.TabIndex = 82;
            // 
            // finishLabel
            // 
            finishLabel.AutoSize = true;
            finishLabel.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            finishLabel.ForeColor = System.Drawing.Color.Black;
            finishLabel.Location = new System.Drawing.Point(1, 9);
            finishLabel.Name = "finishLabel";
            finishLabel.Size = new System.Drawing.Size(284, 22);
            finishLabel.TabIndex = 83;
            finishLabel.Text = "Finished creating WDAC policy";
            // 
            // progress_Label
            // 
            progress_Label.AutoSize = true;
            progress_Label.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            progress_Label.ForeColor = System.Drawing.Color.Black;
            progress_Label.Location = new System.Drawing.Point(167, 153);
            progress_Label.Name = "progress_Label";
            progress_Label.Size = new System.Drawing.Size(45, 21);
            progress_Label.TabIndex = 84;
            progress_Label.Text = "50%";
            progress_Label.Visible = false;
            // 
            // xmlFilePathLabel
            // 
            xmlFilePathLabel.AutoSize = true;
            xmlFilePathLabel.BackColor = System.Drawing.Color.Transparent;
            xmlFilePathLabel.Font = new System.Drawing.Font("Tahoma", 9.5F);
            xmlFilePathLabel.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            xmlFilePathLabel.Location = new System.Drawing.Point(1, 76);
            xmlFilePathLabel.Name = "xmlFilePathLabel";
            xmlFilePathLabel.Size = new System.Drawing.Size(155, 19);
            xmlFilePathLabel.TabIndex = 1;
            xmlFilePathLabel.Tag = "IgnoreDarkMode";
            xmlFilePathLabel.Text = "XmlFilePath Location";
            xmlFilePathLabel.Click += XmlFilePathLabel_Click;
            // 
            // finishPanel
            // 
            finishPanel.Controls.Add(binFilePathLabel);
            finishPanel.Controls.Add(label4);
            finishPanel.Controls.Add(xmlFilePathLabel);
            finishPanel.Controls.Add(finishLabel);
            finishPanel.Location = new System.Drawing.Point(167, 237);
            finishPanel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            finishPanel.Name = "finishPanel";
            finishPanel.Size = new System.Drawing.Size(867, 182);
            finishPanel.TabIndex = 87;
            finishPanel.Visible = false;
            // 
            // binFilePathLabel
            // 
            binFilePathLabel.AutoSize = true;
            binFilePathLabel.BackColor = System.Drawing.Color.Transparent;
            binFilePathLabel.Font = new System.Drawing.Font("Tahoma", 9.5F);
            binFilePathLabel.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            binFilePathLabel.Location = new System.Drawing.Point(3, 127);
            binFilePathLabel.Name = "binFilePathLabel";
            binFilePathLabel.Size = new System.Drawing.Size(150, 19);
            binFilePathLabel.TabIndex = 2;
            binFilePathLabel.Tag = "IgnoreDarkMode";
            binFilePathLabel.Text = "BinFilePath Location";
            binFilePathLabel.Click += BinFilePathLabel_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Tahoma", 9.5F);
            label4.ForeColor = System.Drawing.Color.Black;
            label4.Location = new System.Drawing.Point(1, 48);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(237, 19);
            label4.TabIndex = 87;
            label4.Text = "Output locations (click to open):";
            // 
            // progressString_Label
            // 
            progressString_Label.AutoSize = true;
            progressString_Label.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            progressString_Label.ForeColor = System.Drawing.Color.Black;
            progressString_Label.Location = new System.Drawing.Point(167, 118);
            progressString_Label.Name = "progressString_Label";
            progressString_Label.Size = new System.Drawing.Size(214, 18);
            progressString_Label.TabIndex = 88;
            progressString_Label.Text = "Configuring Policy Parameters...";
            // 
            // BuildPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.White;
            Controls.Add(progressString_Label);
            Controls.Add(finishPanel);
            Controls.Add(progress_Label);
            Controls.Add(progressBar);
            Controls.Add(label_WaitMsg);
            Controls.Add(label1);
            Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            Name = "BuildPage";
            Size = new System.Drawing.Size(1443, 833);
            Validated += BuildPage_Validated;
            finishPanel.ResumeLayout(false);
            finishPanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_WaitMsg;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label finishLabel;
        private System.Windows.Forms.Label progress_Label;
        private System.Windows.Forms.Label xmlFilePathLabel;
        private System.Windows.Forms.Panel finishPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label progressString_Label;
        private System.Windows.Forms.Label binFilePathLabel;
    }
}
