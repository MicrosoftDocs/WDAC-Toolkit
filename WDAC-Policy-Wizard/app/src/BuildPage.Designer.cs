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
            this.label1 = new System.Windows.Forms.Label();
            this.label_WaitMsg = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.finishLabel = new System.Windows.Forms.Label();
            this.progress_Label = new System.Windows.Forms.Label();
            this.hyperlinkLabel = new System.Windows.Forms.Label();
            this.finishPanel = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.progressString_Label = new System.Windows.Forms.Label();
            this.finishPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(200, 56);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(341, 34);
            this.label1.TabIndex = 48;
            this.label1.Text = "Building your WDAC Policy";
            // 
            // label_WaitMsg
            // 
            this.label_WaitMsg.AutoSize = true;
            this.label_WaitMsg.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_WaitMsg.ForeColor = System.Drawing.Color.Black;
            this.label_WaitMsg.Location = new System.Drawing.Point(197, 244);
            this.label_WaitMsg.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_WaitMsg.Name = "label_WaitMsg";
            this.label_WaitMsg.Size = new System.Drawing.Size(704, 22);
            this.label_WaitMsg.TabIndex = 81;
            this.label_WaitMsg.Text = "The wizard is building your WDAC Code Integrity policy. This may take several min" +
    "utes.";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(278, 179);
            this.progressBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(434, 34);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 82;
            // 
            // finishLabel
            // 
            this.finishLabel.AutoSize = true;
            this.finishLabel.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.finishLabel.ForeColor = System.Drawing.Color.Black;
            this.finishLabel.Location = new System.Drawing.Point(1, 21);
            this.finishLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.finishLabel.Name = "finishLabel";
            this.finishLabel.Size = new System.Drawing.Size(308, 27);
            this.finishLabel.TabIndex = 83;
            this.finishLabel.Text = "Finished creating CI policy";
            // 
            // progress_Label
            // 
            this.progress_Label.AutoSize = true;
            this.progress_Label.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progress_Label.ForeColor = System.Drawing.Color.Black;
            this.progress_Label.Location = new System.Drawing.Point(201, 184);
            this.progress_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.progress_Label.Name = "progress_Label";
            this.progress_Label.Size = new System.Drawing.Size(52, 24);
            this.progress_Label.TabIndex = 84;
            this.progress_Label.Text = "50%";
            this.progress_Label.Visible = false;
            // 
            // hyperlinkLabel
            // 
            this.hyperlinkLabel.AutoSize = true;
            this.hyperlinkLabel.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.hyperlinkLabel.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.hyperlinkLabel.Location = new System.Drawing.Point(4, 91);
            this.hyperlinkLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.hyperlinkLabel.Name = "hyperlinkLabel";
            this.hyperlinkLabel.Size = new System.Drawing.Size(145, 23);
            this.hyperlinkLabel.TabIndex = 85;
            this.hyperlinkLabel.Text = "Unable to locate";
            this.hyperlinkLabel.Click += new System.EventHandler(this.hyperlinkLabel_Click);
            // 
            // finishPanel
            // 
            this.finishPanel.Controls.Add(this.label4);
            this.finishPanel.Controls.Add(this.hyperlinkLabel);
            this.finishPanel.Controls.Add(this.finishLabel);
            this.finishPanel.Location = new System.Drawing.Point(200, 294);
            this.finishPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.finishPanel.Name = "finishPanel";
            this.finishPanel.Size = new System.Drawing.Size(1040, 218);
            this.finishPanel.TabIndex = 87;
            this.finishPanel.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(1, 58);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(145, 23);
            this.label4.TabIndex = 87;
            this.label4.Text = "Output location:";
            // 
            // progressString_Label
            // 
            this.progressString_Label.AutoSize = true;
            this.progressString_Label.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressString_Label.ForeColor = System.Drawing.Color.Black;
            this.progressString_Label.Location = new System.Drawing.Point(201, 142);
            this.progressString_Label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.progressString_Label.Name = "progressString_Label";
            this.progressString_Label.Size = new System.Drawing.Size(260, 22);
            this.progressString_Label.TabIndex = 88;
            this.progressString_Label.Text = "Configuring Policy Parameters...";
            // 
            // BuildPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.progressString_Label);
            this.Controls.Add(this.finishPanel);
            this.Controls.Add(this.progress_Label);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label_WaitMsg);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "BuildPage";
            this.Size = new System.Drawing.Size(1732, 1000);
            this.finishPanel.ResumeLayout(false);
            this.finishPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_WaitMsg;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label finishLabel;
        private System.Windows.Forms.Label progress_Label;
        private System.Windows.Forms.Label hyperlinkLabel;
        private System.Windows.Forms.Panel finishPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label progressString_Label;
    }
}
