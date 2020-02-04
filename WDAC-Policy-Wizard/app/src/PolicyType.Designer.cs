// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

namespace WDAC_Wizard
{
    partial class PolicyType
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
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.labelBase = new System.Windows.Forms.Label();
            this.labelSupp = new System.Windows.Forms.Label();
            this.panelSupplementalPolicy = new System.Windows.Forms.Panel();
            this.Verified_Label = new System.Windows.Forms.Label();
            this.Verified_PictureBox = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_Browse = new System.Windows.Forms.Button();
            this.textBoxPolicyPath = new System.Windows.Forms.TextBox();
            this.label_policyName = new System.Windows.Forms.Label();
            this.basePolicy_PictureBox = new System.Windows.Forms.PictureBox();
            this.suppPolicy_PictureBox = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panelSupplementalPolicy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Verified_PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePolicy_PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.suppPolicy_PictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(175, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(287, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a New Policy Type:";
            // 
            // labelBase
            // 
            this.labelBase.AutoSize = true;
            this.labelBase.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBase.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelBase.Location = new System.Drawing.Point(231, 106);
            this.labelBase.Name = "labelBase";
            this.labelBase.Size = new System.Drawing.Size(110, 24);
            this.labelBase.TabIndex = 4;
            this.labelBase.Text = "Base Policy";
            this.labelBase.Click += new System.EventHandler(this.BasePolicy_Selected);
            // 
            // labelSupp
            // 
            this.labelSupp.AutoSize = true;
            this.labelSupp.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSupp.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelSupp.Location = new System.Drawing.Point(231, 191);
            this.labelSupp.Name = "labelSupp";
            this.labelSupp.Size = new System.Drawing.Size(189, 24);
            this.labelSupp.TabIndex = 5;
            this.labelSupp.Text = "Supplemental Policy";
            this.labelSupp.Click += new System.EventHandler(this.SupplementalPolicy_Selected);
            // 
            // panelSupplementalPolicy
            // 
            this.panelSupplementalPolicy.BackColor = System.Drawing.Color.White;
            this.panelSupplementalPolicy.Controls.Add(this.Verified_Label);
            this.panelSupplementalPolicy.Controls.Add(this.Verified_PictureBox);
            this.panelSupplementalPolicy.Controls.Add(this.label2);
            this.panelSupplementalPolicy.Controls.Add(this.button_Browse);
            this.panelSupplementalPolicy.Controls.Add(this.textBoxPolicyPath);
            this.panelSupplementalPolicy.Controls.Add(this.label_policyName);
            this.panelSupplementalPolicy.Location = new System.Drawing.Point(225, 279);
            this.panelSupplementalPolicy.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panelSupplementalPolicy.Name = "panelSupplementalPolicy";
            this.panelSupplementalPolicy.Size = new System.Drawing.Size(865, 206);
            this.panelSupplementalPolicy.TabIndex = 11;
            this.panelSupplementalPolicy.Visible = false;
            // 
            // Verified_Label
            // 
            this.Verified_Label.AutoSize = true;
            this.Verified_Label.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.Verified_Label.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Verified_Label.Location = new System.Drawing.Point(48, 120);
            this.Verified_Label.Name = "Verified_Label";
            this.Verified_Label.Size = new System.Drawing.Size(332, 19);
            this.Verified_Label.TabIndex = 16;
            this.Verified_Label.Text = "This base policy allows supplemental policies.";
            this.Verified_Label.Visible = false;
            // 
            // Verified_PictureBox
            // 
            this.Verified_PictureBox.Image = global::WDAC_Wizard.Properties.Resources.verified;
            this.Verified_PictureBox.Location = new System.Drawing.Point(17, 120);
            this.Verified_PictureBox.Name = "Verified_PictureBox";
            this.Verified_PictureBox.Size = new System.Drawing.Size(25, 25);
            this.Verified_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Verified_PictureBox.TabIndex = 94;
            this.Verified_PictureBox.TabStop = false;
            this.Verified_PictureBox.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Italic);
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(4, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(270, 19);
            this.label2.TabIndex = 12;
            this.label2.Text = "Select the base policy to build upon. ";
            // 
            // button_Browse
            // 
            this.button_Browse.FlatAppearance.BorderColor = System.Drawing.SystemColors.MenuHighlight;
            this.button_Browse.FlatAppearance.BorderSize = 2;
            this.button_Browse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Browse.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Browse.ForeColor = System.Drawing.Color.Black;
            this.button_Browse.Location = new System.Drawing.Point(435, 68);
            this.button_Browse.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_Browse.Name = "button_Browse";
            this.button_Browse.Size = new System.Drawing.Size(145, 34);
            this.button_Browse.TabIndex = 93;
            this.button_Browse.Text = "Browse";
            this.button_Browse.UseVisualStyleBackColor = true;
            this.button_Browse.Click += new System.EventHandler(this.button_Browse_Click);
            // 
            // textBoxPolicyPath
            // 
            this.textBoxPolicyPath.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.textBoxPolicyPath.Location = new System.Drawing.Point(12, 72);
            this.textBoxPolicyPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxPolicyPath.Name = "textBoxPolicyPath";
            this.textBoxPolicyPath.Size = new System.Drawing.Size(407, 25);
            this.textBoxPolicyPath.TabIndex = 14;
            this.textBoxPolicyPath.Click += new System.EventHandler(this.button_Browse_Click);
            // 
            // label_policyName
            // 
            this.label_policyName.AutoSize = true;
            this.label_policyName.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_policyName.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label_policyName.Location = new System.Drawing.Point(8, 5);
            this.label_policyName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_policyName.Name = "label_policyName";
            this.label_policyName.Size = new System.Drawing.Size(138, 23);
            this.label_policyName.TabIndex = 13;
            this.label_policyName.Text = "Policy Location:";
            // 
            // basePolicy_PictureBox
            // 
            this.basePolicy_PictureBox.Image = global::WDAC_Wizard.Properties.Resources.radio_on_button;
            this.basePolicy_PictureBox.Location = new System.Drawing.Point(194, 106);
            this.basePolicy_PictureBox.Margin = new System.Windows.Forms.Padding(4);
            this.basePolicy_PictureBox.Name = "basePolicy_PictureBox";
            this.basePolicy_PictureBox.Size = new System.Drawing.Size(30, 30);
            this.basePolicy_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.basePolicy_PictureBox.TabIndex = 12;
            this.basePolicy_PictureBox.TabStop = false;
            this.basePolicy_PictureBox.Click += new System.EventHandler(this.BasePolicy_Selected);
            // 
            // suppPolicy_PictureBox
            // 
            this.suppPolicy_PictureBox.Image = global::WDAC_Wizard.Properties.Resources.radio_off_button;
            this.suppPolicy_PictureBox.Location = new System.Drawing.Point(194, 191);
            this.suppPolicy_PictureBox.Margin = new System.Windows.Forms.Padding(4);
            this.suppPolicy_PictureBox.Name = "suppPolicy_PictureBox";
            this.suppPolicy_PictureBox.Size = new System.Drawing.Size(30, 30);
            this.suppPolicy_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.suppPolicy_PictureBox.TabIndex = 13;
            this.suppPolicy_PictureBox.TabStop = false;
            this.suppPolicy_PictureBox.Click += new System.EventHandler(this.SupplementalPolicy_Selected);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label3.Location = new System.Drawing.Point(231, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(757, 19);
            this.label3.TabIndex = 14;
            this.label3.Text = "Creates a new code integrity policy for the system. Multiple base policies can be" +
    " enforced simultaneously. ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label4.Location = new System.Drawing.Point(231, 226);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(490, 19);
            this.label4.TabIndex = 15;
            this.label4.Text = "Creates a code integrity policy to expand a pre-existing base policy. ";
            // 
            // PolicyType
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.suppPolicy_PictureBox);
            this.Controls.Add(this.basePolicy_PictureBox);
            this.Controls.Add(this.panelSupplementalPolicy);
            this.Controls.Add(this.labelSupp);
            this.Controls.Add(this.labelBase);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "PolicyType";
            this.Size = new System.Drawing.Size(1250, 750);
            this.panelSupplementalPolicy.ResumeLayout(false);
            this.panelSupplementalPolicy.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Verified_PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePolicy_PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.suppPolicy_PictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.Label labelBase;
        private System.Windows.Forms.Label labelSupp;
        private System.Windows.Forms.Panel panelSupplementalPolicy;
        private System.Windows.Forms.Label label_policyName;
        private System.Windows.Forms.TextBox textBoxPolicyPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.PictureBox basePolicy_PictureBox;
        private System.Windows.Forms.PictureBox suppPolicy_PictureBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox Verified_PictureBox;
        private System.Windows.Forms.Label Verified_Label;
    }
}
