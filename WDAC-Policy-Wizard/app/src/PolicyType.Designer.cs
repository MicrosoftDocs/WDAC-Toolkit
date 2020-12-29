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
            this.labelBase = new System.Windows.Forms.Label();
            this.labelSupp = new System.Windows.Forms.Label();
            this.panelSupplName = new System.Windows.Forms.Panel();
            this.panelSuppl_Base = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.Verified_Label = new System.Windows.Forms.Label();
            this.Verified_PictureBox = new System.Windows.Forms.PictureBox();
            this.button_Browse = new System.Windows.Forms.Button();
            this.textBoxBasePolicyPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button_Browse_Supp = new System.Windows.Forms.Button();
            this.textBox_PolicyName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxSuppPath = new System.Windows.Forms.TextBox();
            this.label_fileLocation = new System.Windows.Forms.Label();
            this.basePolicy_PictureBox = new System.Windows.Forms.PictureBox();
            this.suppPolicy_PictureBox = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SupplementalInfoLabel = new System.Windows.Forms.Label();
            this.basePolicyValidation_Panel = new System.Windows.Forms.Panel();
            this.panelSupplName.SuspendLayout();
            this.panelSuppl_Base.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Verified_PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePolicy_PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.suppPolicy_PictureBox)).BeginInit();
            this.basePolicyValidation_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(197, 50);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(324, 34);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a New Policy Type";
            // 
            // labelBase
            // 
            this.labelBase.AutoSize = true;
            this.labelBase.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelBase.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelBase.Location = new System.Drawing.Point(248, 134);
            this.labelBase.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelBase.Name = "labelBase";
            this.labelBase.Size = new System.Drawing.Size(131, 29);
            this.labelBase.TabIndex = 4;
            this.labelBase.Text = "Base Policy";
            this.labelBase.Click += new System.EventHandler(this.BasePolicy_Selected);
            // 
            // labelSupp
            // 
            this.labelSupp.AutoSize = true;
            this.labelSupp.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSupp.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelSupp.Location = new System.Drawing.Point(248, 240);
            this.labelSupp.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSupp.Name = "labelSupp";
            this.labelSupp.Size = new System.Drawing.Size(222, 29);
            this.labelSupp.TabIndex = 5;
            this.labelSupp.Text = "Supplemental Policy";
            this.labelSupp.Click += new System.EventHandler(this.SupplementalPolicy_Selected);
            // 
            // panelSupplName
            // 
            this.panelSupplName.BackColor = System.Drawing.Color.White;
            this.panelSupplName.Controls.Add(this.panelSuppl_Base);
            this.panelSupplName.Controls.Add(this.button_Browse_Supp);
            this.panelSupplName.Controls.Add(this.textBox_PolicyName);
            this.panelSupplName.Controls.Add(this.label6);
            this.panelSupplName.Controls.Add(this.textBoxSuppPath);
            this.panelSupplName.Controls.Add(this.label_fileLocation);
            this.panelSupplName.Location = new System.Drawing.Point(206, 364);
            this.panelSupplName.Margin = new System.Windows.Forms.Padding(2);
            this.panelSupplName.Name = "panelSupplName";
            this.panelSupplName.Size = new System.Drawing.Size(973, 374);
            this.panelSupplName.TabIndex = 11;
            this.panelSupplName.Visible = false;
            // 
            // panelSuppl_Base
            // 
            this.panelSuppl_Base.Controls.Add(this.basePolicyValidation_Panel);
            this.panelSuppl_Base.Controls.Add(this.label5);
            this.panelSuppl_Base.Controls.Add(this.button_Browse);
            this.panelSuppl_Base.Controls.Add(this.textBoxBasePolicyPath);
            this.panelSuppl_Base.Controls.Add(this.label2);
            this.panelSuppl_Base.Location = new System.Drawing.Point(0, 177);
            this.panelSuppl_Base.Margin = new System.Windows.Forms.Padding(4);
            this.panelSuppl_Base.Name = "panelSuppl_Base";
            this.panelSuppl_Base.Size = new System.Drawing.Size(826, 172);
            this.panelSuppl_Base.TabIndex = 96;
            this.panelSuppl_Base.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(8, 58);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(104, 22);
            this.label5.TabIndex = 97;
            this.label5.Text = "Base Policy:";
            // 
            // Verified_Label
            // 
            this.Verified_Label.AutoSize = true;
            this.Verified_Label.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.Verified_Label.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Verified_Label.Location = new System.Drawing.Point(45, 10);
            this.Verified_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Verified_Label.Name = "Verified_Label";
            this.Verified_Label.Size = new System.Drawing.Size(388, 23);
            this.Verified_Label.TabIndex = 16;
            this.Verified_Label.Text = "This base policy allows supplemental policies.";
            this.Verified_Label.Visible = false;
            // 
            // Verified_PictureBox
            // 
            this.Verified_PictureBox.Image = global::WDAC_Wizard.Properties.Resources.verified;
            this.Verified_PictureBox.Location = new System.Drawing.Point(5, 6);
            this.Verified_PictureBox.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.Verified_PictureBox.Name = "Verified_PictureBox";
            this.Verified_PictureBox.Size = new System.Drawing.Size(28, 31);
            this.Verified_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Verified_PictureBox.TabIndex = 94;
            this.Verified_PictureBox.TabStop = false;
            this.Verified_PictureBox.Visible = false;
            // 
            // button_Browse
            // 
            this.button_Browse.FlatAppearance.BorderColor = System.Drawing.SystemColors.MenuHighlight;
            this.button_Browse.FlatAppearance.BorderSize = 2;
            this.button_Browse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Browse.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Browse.ForeColor = System.Drawing.Color.Black;
            this.button_Browse.Location = new System.Drawing.Point(635, 47);
            this.button_Browse.Margin = new System.Windows.Forms.Padding(2);
            this.button_Browse.Name = "button_Browse";
            this.button_Browse.Size = new System.Drawing.Size(163, 42);
            this.button_Browse.TabIndex = 93;
            this.button_Browse.Text = "Browse";
            this.button_Browse.UseVisualStyleBackColor = true;
            this.button_Browse.Click += new System.EventHandler(this.button_Browse_Click);
            // 
            // textBoxBasePolicyPath
            // 
            this.textBoxBasePolicyPath.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.textBoxBasePolicyPath.Location = new System.Drawing.Point(169, 55);
            this.textBoxBasePolicyPath.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxBasePolicyPath.Name = "textBoxBasePolicyPath";
            this.textBoxBasePolicyPath.Size = new System.Drawing.Size(456, 28);
            this.textBoxBasePolicyPath.TabIndex = 14;
            this.textBoxBasePolicyPath.Click += new System.EventHandler(this.button_Browse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Italic);
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(5, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(506, 23);
            this.label2.TabIndex = 12;
            this.label2.Text = "Select the policy that the supplemental policy will apply to. ";
            // 
            // button_Browse_Supp
            // 
            this.button_Browse_Supp.FlatAppearance.BorderColor = System.Drawing.SystemColors.MenuHighlight;
            this.button_Browse_Supp.FlatAppearance.BorderSize = 2;
            this.button_Browse_Supp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Browse_Supp.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Browse_Supp.ForeColor = System.Drawing.Color.Black;
            this.button_Browse_Supp.Location = new System.Drawing.Point(638, 63);
            this.button_Browse_Supp.Margin = new System.Windows.Forms.Padding(2);
            this.button_Browse_Supp.Name = "button_Browse_Supp";
            this.button_Browse_Supp.Size = new System.Drawing.Size(163, 42);
            this.button_Browse_Supp.TabIndex = 94;
            this.button_Browse_Supp.Text = "Browse";
            this.button_Browse_Supp.UseVisualStyleBackColor = true;
            this.button_Browse_Supp.Click += new System.EventHandler(this.button_BrowseSupp_Click);
            // 
            // textBox_PolicyName
            // 
            this.textBox_PolicyName.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_PolicyName.ForeColor = System.Drawing.Color.Black;
            this.textBox_PolicyName.Location = new System.Drawing.Point(173, 14);
            this.textBox_PolicyName.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.textBox_PolicyName.Name = "textBox_PolicyName";
            this.textBox_PolicyName.Size = new System.Drawing.Size(456, 28);
            this.textBox_PolicyName.TabIndex = 9;
            this.textBox_PolicyName.TextChanged += new System.EventHandler(this.textBox_PolicyName_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(8, 14);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(112, 22);
            this.label6.TabIndex = 8;
            this.label6.Text = "Policy Name:";
            // 
            // textBoxSuppPath
            // 
            this.textBoxSuppPath.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSuppPath.ForeColor = System.Drawing.Color.Black;
            this.textBoxSuppPath.Location = new System.Drawing.Point(173, 68);
            this.textBoxSuppPath.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.textBoxSuppPath.Name = "textBoxSuppPath";
            this.textBoxSuppPath.Size = new System.Drawing.Size(456, 28);
            this.textBoxSuppPath.TabIndex = 7;
            this.textBoxSuppPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxSuppPath.Click += new System.EventHandler(this.button_BrowseSupp_Click);
            // 
            // label_fileLocation
            // 
            this.label_fileLocation.AutoSize = true;
            this.label_fileLocation.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_fileLocation.ForeColor = System.Drawing.Color.Black;
            this.label_fileLocation.Location = new System.Drawing.Point(8, 70);
            this.label_fileLocation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_fileLocation.Name = "label_fileLocation";
            this.label_fileLocation.Size = new System.Drawing.Size(165, 22);
            this.label_fileLocation.TabIndex = 6;
            this.label_fileLocation.Text = "Policy File Location:";
            // 
            // basePolicy_PictureBox
            // 
            this.basePolicy_PictureBox.Image = global::WDAC_Wizard.Properties.Resources.radio_on;
            this.basePolicy_PictureBox.Location = new System.Drawing.Point(206, 134);
            this.basePolicy_PictureBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.basePolicy_PictureBox.Name = "basePolicy_PictureBox";
            this.basePolicy_PictureBox.Size = new System.Drawing.Size(30, 30);
            this.basePolicy_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.basePolicy_PictureBox.TabIndex = 12;
            this.basePolicy_PictureBox.TabStop = false;
            this.basePolicy_PictureBox.Click += new System.EventHandler(this.BasePolicy_Selected);
            // 
            // suppPolicy_PictureBox
            // 
            this.suppPolicy_PictureBox.Image = global::WDAC_Wizard.Properties.Resources.radio_off;
            this.suppPolicy_PictureBox.Location = new System.Drawing.Point(206, 241);
            this.suppPolicy_PictureBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
            this.label3.Location = new System.Drawing.Point(248, 180);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(898, 23);
            this.label3.TabIndex = 14;
            this.label3.Text = "Creates a new code integrity policy for the system. Multiple base policies can be" +
    " enforced simultaneously. ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label4.Location = new System.Drawing.Point(248, 284);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(581, 23);
            this.label4.TabIndex = 15;
            this.label4.Text = "Creates a code integrity policy to expand a pre-existing base policy. ";
            // 
            // SupplementalInfoLabel
            // 
            this.SupplementalInfoLabel.AutoSize = true;
            this.SupplementalInfoLabel.Font = new System.Drawing.Font("Tahoma", 10F);
            this.SupplementalInfoLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.SupplementalInfoLabel.Image = global::WDAC_Wizard.Properties.Resources.external_link_symbol_highlight;
            this.SupplementalInfoLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.SupplementalInfoLabel.Location = new System.Drawing.Point(248, 314);
            this.SupplementalInfoLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.SupplementalInfoLabel.Name = "SupplementalInfoLabel";
            this.SupplementalInfoLabel.Size = new System.Drawing.Size(316, 24);
            this.SupplementalInfoLabel.TabIndex = 98;
            this.SupplementalInfoLabel.Text = "What is a supplemental policy?     \r\n";
            this.SupplementalInfoLabel.Click += new System.EventHandler(this.SupplementalInfoLabel_Click);
            // 
            // basePolicyValidation_Panel
            // 
            this.basePolicyValidation_Panel.Controls.Add(this.Verified_PictureBox);
            this.basePolicyValidation_Panel.Controls.Add(this.Verified_Label);
            this.basePolicyValidation_Panel.Location = new System.Drawing.Point(12, 105);
            this.basePolicyValidation_Panel.Name = "basePolicyValidation_Panel";
            this.basePolicyValidation_Panel.Size = new System.Drawing.Size(539, 42);
            this.basePolicyValidation_Panel.TabIndex = 99;
            this.basePolicyValidation_Panel.Visible = false;
            // 
            // PolicyType
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.SupplementalInfoLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.suppPolicy_PictureBox);
            this.Controls.Add(this.basePolicy_PictureBox);
            this.Controls.Add(this.panelSupplName);
            this.Controls.Add(this.labelSupp);
            this.Controls.Add(this.labelBase);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PolicyType";
            this.Size = new System.Drawing.Size(1406, 938);
            this.panelSupplName.ResumeLayout(false);
            this.panelSupplName.PerformLayout();
            this.panelSuppl_Base.ResumeLayout(false);
            this.panelSuppl_Base.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Verified_PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePolicy_PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.suppPolicy_PictureBox)).EndInit();
            this.basePolicyValidation_Panel.ResumeLayout(false);
            this.basePolicyValidation_Panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelBase;
        private System.Windows.Forms.Label labelSupp;
        private System.Windows.Forms.Panel panelSupplName;
        private System.Windows.Forms.TextBox textBoxBasePolicyPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.PictureBox basePolicy_PictureBox;
        private System.Windows.Forms.PictureBox suppPolicy_PictureBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox Verified_PictureBox;
        private System.Windows.Forms.Label Verified_Label;
        private System.Windows.Forms.Label SupplementalInfoLabel;
        private System.Windows.Forms.Button button_Browse_Supp;
        private System.Windows.Forms.TextBox textBox_PolicyName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxSuppPath;
        private System.Windows.Forms.Label label_fileLocation;
        private System.Windows.Forms.Panel panelSuppl_Base;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel basePolicyValidation_Panel;
    }
}
