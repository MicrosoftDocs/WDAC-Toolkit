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
            label1 = new System.Windows.Forms.Label();
            labelBase = new System.Windows.Forms.Label();
            labelSupp = new System.Windows.Forms.Label();
            panelSupplName = new System.Windows.Forms.Panel();
            panelSuppl_Base = new System.Windows.Forms.Panel();
            label10 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            textBoxBasePolicyID = new System.Windows.Forms.TextBox();
            basePolicyValidation_Panel = new System.Windows.Forms.Panel();
            Verified_PictureBox = new System.Windows.Forms.PictureBox();
            Verified_Label = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            button_Browse = new System.Windows.Forms.Button();
            textBoxBasePolicyPath = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            button_Browse_Supp = new System.Windows.Forms.Button();
            textBox_PolicyName = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            textBoxSuppPath = new System.Windows.Forms.TextBox();
            label_fileLocation = new System.Windows.Forms.Label();
            basePolicy_PictureBox = new System.Windows.Forms.PictureBox();
            suppPolicy_PictureBox = new System.Windows.Forms.PictureBox();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label_LearnMore = new System.Windows.Forms.Label();
            panel_MultiPolicy = new System.Windows.Forms.Panel();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            radioButton_MultiplePolicy = new System.Windows.Forms.RadioButton();
            radioButton_SinglePolicy = new System.Windows.Forms.RadioButton();
            panelSupplName.SuspendLayout();
            panelSuppl_Base.SuspendLayout();
            basePolicyValidation_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Verified_PictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)basePolicy_PictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)suppPolicy_PictureBox).BeginInit();
            panel_MultiPolicy.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label1.ForeColor = System.Drawing.Color.Black;
            label1.Location = new System.Drawing.Point(164, 19);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(224, 29);
            label1.TabIndex = 0;
            label1.Text = "Select a Policy Type";
            // 
            // labelBase
            // 
            labelBase.AutoSize = true;
            labelBase.Font = new System.Drawing.Font("Tahoma", 9.5F);
            labelBase.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            labelBase.Location = new System.Drawing.Point(41, 26);
            labelBase.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            labelBase.Name = "labelBase";
            labelBase.Size = new System.Drawing.Size(87, 19);
            labelBase.TabIndex = 3;
            labelBase.Text = "Base Policy";
            labelBase.Click += BasePolicy_Selected;
            // 
            // labelSupp
            // 
            labelSupp.AutoSize = true;
            labelSupp.Font = new System.Drawing.Font("Tahoma", 9.5F);
            labelSupp.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            labelSupp.Location = new System.Drawing.Point(41, 93);
            labelSupp.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            labelSupp.Name = "labelSupp";
            labelSupp.Size = new System.Drawing.Size(151, 19);
            labelSupp.TabIndex = 4;
            labelSupp.Text = "Supplemental Policy";
            labelSupp.Click += SupplementalPolicy_Selected;
            // 
            // panelSupplName
            // 
            panelSupplName.BackColor = System.Drawing.Color.Transparent;
            panelSupplName.Controls.Add(panelSuppl_Base);
            panelSupplName.Controls.Add(button_Browse_Supp);
            panelSupplName.Controls.Add(textBox_PolicyName);
            panelSupplName.Controls.Add(label6);
            panelSupplName.Controls.Add(textBoxSuppPath);
            panelSupplName.Controls.Add(label_fileLocation);
            panelSupplName.Location = new System.Drawing.Point(6, 165);
            panelSupplName.Margin = new System.Windows.Forms.Padding(2);
            panelSupplName.Name = "panelSupplName";
            panelSupplName.Size = new System.Drawing.Size(811, 344);
            panelSupplName.TabIndex = 11;
            panelSupplName.Visible = false;
            // 
            // panelSuppl_Base
            // 
            panelSuppl_Base.Controls.Add(label10);
            panelSuppl_Base.Controls.Add(label7);
            panelSuppl_Base.Controls.Add(textBoxBasePolicyID);
            panelSuppl_Base.Controls.Add(basePolicyValidation_Panel);
            panelSuppl_Base.Controls.Add(label5);
            panelSuppl_Base.Controls.Add(button_Browse);
            panelSuppl_Base.Controls.Add(textBoxBasePolicyPath);
            panelSuppl_Base.Controls.Add(label2);
            panelSuppl_Base.Location = new System.Drawing.Point(3, 101);
            panelSuppl_Base.Name = "panelSuppl_Base";
            panelSuppl_Base.Size = new System.Drawing.Size(805, 212);
            panelSuppl_Base.TabIndex = 96;
            panelSuppl_Base.Visible = false;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label10.ForeColor = System.Drawing.Color.Black;
            label10.Location = new System.Drawing.Point(75, 78);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(28, 18);
            label10.TabIndex = 102;
            label10.Text = "OR";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label7.ForeColor = System.Drawing.Color.Black;
            label7.Location = new System.Drawing.Point(34, 46);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(106, 18);
            label7.TabIndex = 101;
            label7.Text = "Base Policy ID:";
            // 
            // textBoxBasePolicyID
            // 
            textBoxBasePolicyID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxBasePolicyID.Font = new System.Drawing.Font("Tahoma", 8.5F);
            textBoxBasePolicyID.ForeColor = System.Drawing.SystemColors.WindowFrame;
            textBoxBasePolicyID.Location = new System.Drawing.Point(169, 43);
            textBoxBasePolicyID.Margin = new System.Windows.Forms.Padding(2);
            textBoxBasePolicyID.Name = "textBoxBasePolicyID";
            textBoxBasePolicyID.Size = new System.Drawing.Size(381, 25);
            textBoxBasePolicyID.TabIndex = 100;
            textBoxBasePolicyID.Text = "e.g. {fd756ea8-ad7f-4e30-96bd-8778288212f6}";
            textBoxBasePolicyID.Click += TextBoxBasePolicyID_Selected;
            textBoxBasePolicyID.TextChanged += TextBoxBasePolicyID_TextChanged;
            textBoxBasePolicyID.DoubleClick += TextBoxBasePolicyID_Selected;
            textBoxBasePolicyID.Leave += TextBoxBasePolicyID_Leave;
            // 
            // basePolicyValidation_Panel
            // 
            basePolicyValidation_Panel.Controls.Add(Verified_PictureBox);
            basePolicyValidation_Panel.Controls.Add(Verified_Label);
            basePolicyValidation_Panel.Location = new System.Drawing.Point(10, 152);
            basePolicyValidation_Panel.Margin = new System.Windows.Forms.Padding(2);
            basePolicyValidation_Panel.Name = "basePolicyValidation_Panel";
            basePolicyValidation_Panel.Size = new System.Drawing.Size(793, 35);
            basePolicyValidation_Panel.TabIndex = 99;
            basePolicyValidation_Panel.Visible = false;
            // 
            // Verified_PictureBox
            // 
            Verified_PictureBox.Image = Properties.Resources.verified;
            Verified_PictureBox.Location = new System.Drawing.Point(4, 5);
            Verified_PictureBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            Verified_PictureBox.Name = "Verified_PictureBox";
            Verified_PictureBox.Size = new System.Drawing.Size(23, 26);
            Verified_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            Verified_PictureBox.TabIndex = 94;
            Verified_PictureBox.TabStop = false;
            Verified_PictureBox.Visible = false;
            // 
            // Verified_Label
            // 
            Verified_Label.AutoSize = true;
            Verified_Label.Font = new System.Drawing.Font("Tahoma", 9F);
            Verified_Label.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            Verified_Label.Location = new System.Drawing.Point(37, 8);
            Verified_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            Verified_Label.Name = "Verified_Label";
            Verified_Label.Size = new System.Drawing.Size(297, 18);
            Verified_Label.TabIndex = 16;
            Verified_Label.Text = "This base policy allows supplemental policies.";
            Verified_Label.Visible = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label5.ForeColor = System.Drawing.Color.Black;
            label5.Location = new System.Drawing.Point(34, 110);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(119, 18);
            label5.TabIndex = 97;
            label5.Text = "Base Policy Path:";
            // 
            // button_Browse
            // 
            button_Browse.Font = new System.Drawing.Font("Tahoma", 9F);
            button_Browse.Location = new System.Drawing.Point(560, 105);
            button_Browse.Margin = new System.Windows.Forms.Padding(2);
            button_Browse.Name = "button_Browse";
            button_Browse.Size = new System.Drawing.Size(107, 28);
            button_Browse.TabIndex = 93;
            button_Browse.Text = "Browse";
            button_Browse.UseVisualStyleBackColor = true;
            button_Browse.Click += Button_Browse_Click;
            // 
            // textBoxBasePolicyPath
            // 
            textBoxBasePolicyPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxBasePolicyPath.Font = new System.Drawing.Font("Tahoma", 8.5F);
            textBoxBasePolicyPath.Location = new System.Drawing.Point(169, 107);
            textBoxBasePolicyPath.Margin = new System.Windows.Forms.Padding(2);
            textBoxBasePolicyPath.Name = "textBoxBasePolicyPath";
            textBoxBasePolicyPath.Size = new System.Drawing.Size(381, 25);
            textBoxBasePolicyPath.TabIndex = 14;
            textBoxBasePolicyPath.Click += Button_Browse_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Tahoma", 9.5F);
            label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label2.Location = new System.Drawing.Point(35, 9);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(621, 19);
            label2.TabIndex = 12;
            label2.Text = "Select the policy that the supplemental policy will apply to or enter the base policy ID. ";
            // 
            // button_Browse_Supp
            // 
            button_Browse_Supp.Font = new System.Drawing.Font("Tahoma", 9F);
            button_Browse_Supp.Location = new System.Drawing.Point(563, 53);
            button_Browse_Supp.Margin = new System.Windows.Forms.Padding(2);
            button_Browse_Supp.Name = "button_Browse_Supp";
            button_Browse_Supp.Size = new System.Drawing.Size(107, 28);
            button_Browse_Supp.TabIndex = 3;
            button_Browse_Supp.Text = "Browse";
            button_Browse_Supp.UseVisualStyleBackColor = true;
            button_Browse_Supp.Click += Button_BrowseSupp_Click;
            // 
            // textBox_PolicyName
            // 
            textBox_PolicyName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBox_PolicyName.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            textBox_PolicyName.ForeColor = System.Drawing.Color.Black;
            textBox_PolicyName.Location = new System.Drawing.Point(172, 11);
            textBox_PolicyName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            textBox_PolicyName.Name = "textBox_PolicyName";
            textBox_PolicyName.Size = new System.Drawing.Size(381, 25);
            textBox_PolicyName.TabIndex = 1;
            textBox_PolicyName.TextChanged += TextBox_PolicyName_TextChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label6.ForeColor = System.Drawing.Color.Black;
            label6.Location = new System.Drawing.Point(38, 14);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(92, 18);
            label6.TabIndex = 8;
            label6.Text = "Policy Name:";
            // 
            // textBoxSuppPath
            // 
            textBoxSuppPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            textBoxSuppPath.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            textBoxSuppPath.ForeColor = System.Drawing.Color.Black;
            textBoxSuppPath.Location = new System.Drawing.Point(172, 55);
            textBoxSuppPath.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            textBoxSuppPath.Name = "textBoxSuppPath";
            textBoxSuppPath.Size = new System.Drawing.Size(381, 25);
            textBoxSuppPath.TabIndex = 2;
            textBoxSuppPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            textBoxSuppPath.Click += Button_BrowseSupp_Click;
            // 
            // label_fileLocation
            // 
            label_fileLocation.AutoSize = true;
            label_fileLocation.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label_fileLocation.ForeColor = System.Drawing.Color.Black;
            label_fileLocation.Location = new System.Drawing.Point(38, 58);
            label_fileLocation.Name = "label_fileLocation";
            label_fileLocation.Size = new System.Drawing.Size(131, 18);
            label_fileLocation.TabIndex = 6;
            label_fileLocation.Text = "Policy File Location:";
            // 
            // basePolicy_PictureBox
            // 
            basePolicy_PictureBox.Image = Properties.Resources.radio_on;
            basePolicy_PictureBox.Location = new System.Drawing.Point(6, 23);
            basePolicy_PictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            basePolicy_PictureBox.Name = "basePolicy_PictureBox";
            basePolicy_PictureBox.Size = new System.Drawing.Size(25, 25);
            basePolicy_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            basePolicy_PictureBox.TabIndex = 12;
            basePolicy_PictureBox.TabStop = false;
            basePolicy_PictureBox.Tag = "Selected";
            basePolicy_PictureBox.Click += BasePolicy_Selected;
            // 
            // suppPolicy_PictureBox
            // 
            suppPolicy_PictureBox.Image = Properties.Resources.radio_off;
            suppPolicy_PictureBox.Location = new System.Drawing.Point(6, 89);
            suppPolicy_PictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            suppPolicy_PictureBox.Name = "suppPolicy_PictureBox";
            suppPolicy_PictureBox.Size = new System.Drawing.Size(25, 25);
            suppPolicy_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            suppPolicy_PictureBox.TabIndex = 13;
            suppPolicy_PictureBox.TabStop = false;
            suppPolicy_PictureBox.Tag = "Unselected";
            suppPolicy_PictureBox.Click += SupplementalPolicy_Selected;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label3.Location = new System.Drawing.Point(41, 54);
            label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(698, 18);
            label3.TabIndex = 14;
            label3.Text = "Creates a new code integrity policy for the system. Multiple base policies can be enforced simultaneously. ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Tahoma", 9F);
            label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label4.Location = new System.Drawing.Point(41, 123);
            label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(452, 18);
            label4.TabIndex = 15;
            label4.Text = "Creates a code integrity policy to expand a pre-existing base policy. ";
            // 
            // label_LearnMore
            // 
            label_LearnMore.AutoSize = true;
            label_LearnMore.Font = new System.Drawing.Font("Tahoma", 9F);
            label_LearnMore.ForeColor = System.Drawing.Color.FromArgb(16, 110, 190);
            label_LearnMore.Image = Properties.Resources.external_link_symbol_highlight;
            label_LearnMore.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            label_LearnMore.Location = new System.Drawing.Point(198, 117);
            label_LearnMore.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label_LearnMore.Name = "label_LearnMore";
            label_LearnMore.Size = new System.Drawing.Size(287, 18);
            label_LearnMore.TabIndex = 98;
            label_LearnMore.Tag = "IgnoreDarkMode";
            label_LearnMore.Text = "Learn more about multiple policy format    ";
            label_LearnMore.Click += Label_LearnMore_Click;
            // 
            // panel_MultiPolicy
            // 
            panel_MultiPolicy.BackColor = System.Drawing.Color.Transparent;
            panel_MultiPolicy.Controls.Add(label4);
            panel_MultiPolicy.Controls.Add(label3);
            panel_MultiPolicy.Controls.Add(suppPolicy_PictureBox);
            panel_MultiPolicy.Controls.Add(basePolicy_PictureBox);
            panel_MultiPolicy.Controls.Add(panelSupplName);
            panel_MultiPolicy.Controls.Add(labelSupp);
            panel_MultiPolicy.Controls.Add(labelBase);
            panel_MultiPolicy.Location = new System.Drawing.Point(169, 242);
            panel_MultiPolicy.Name = "panel_MultiPolicy";
            panel_MultiPolicy.Size = new System.Drawing.Size(827, 511);
            panel_MultiPolicy.TabIndex = 101;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label8.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label8.Location = new System.Drawing.Point(198, 95);
            label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(911, 18);
            label8.TabIndex = 99;
            label8.Text = "Create a base or a supplemental policy. Windows 10 (1903) and newer support an unlimited number of policies as of the April 2024 update";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label9.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label9.Location = new System.Drawing.Point(198, 185);
            label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(775, 18);
            label9.TabIndex = 104;
            label9.Text = "Create an App Control for Business policy to be deployed on any Windows 10 device, including Server 2016 and 2019.\r\n";
            // 
            // radioButton_MultiplePolicy
            // 
            radioButton_MultiplePolicy.AutoSize = true;
            radioButton_MultiplePolicy.BackColor = System.Drawing.Color.Transparent;
            radioButton_MultiplePolicy.Checked = true;
            radioButton_MultiplePolicy.Font = new System.Drawing.Font("Tahoma", 10F);
            radioButton_MultiplePolicy.Location = new System.Drawing.Point(178, 70);
            radioButton_MultiplePolicy.Name = "radioButton_MultiplePolicy";
            radioButton_MultiplePolicy.Size = new System.Drawing.Size(194, 25);
            radioButton_MultiplePolicy.TabIndex = 2;
            radioButton_MultiplePolicy.TabStop = true;
            radioButton_MultiplePolicy.Text = "Multiple Policy Format";
            radioButton_MultiplePolicy.UseVisualStyleBackColor = false;
            radioButton_MultiplePolicy.Click += MultipleFormat_ButtonClick;
            // 
            // radioButton_SinglePolicy
            // 
            radioButton_SinglePolicy.AutoSize = true;
            radioButton_SinglePolicy.BackColor = System.Drawing.Color.Transparent;
            radioButton_SinglePolicy.Font = new System.Drawing.Font("Tahoma", 10F);
            radioButton_SinglePolicy.Location = new System.Drawing.Point(178, 160);
            radioButton_SinglePolicy.Name = "radioButton_SinglePolicy";
            radioButton_SinglePolicy.Size = new System.Drawing.Size(180, 25);
            radioButton_SinglePolicy.TabIndex = 1;
            radioButton_SinglePolicy.Text = "Single Policy Format";
            radioButton_SinglePolicy.UseVisualStyleBackColor = false;
            radioButton_SinglePolicy.Click += SingleFormat_ButtonClick;
            // 
            // PolicyType
            // 
            AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.White;
            Controls.Add(label_LearnMore);
            Controls.Add(radioButton_SinglePolicy);
            Controls.Add(radioButton_MultiplePolicy);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(panel_MultiPolicy);
            Controls.Add(label1);
            ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            Margin = new System.Windows.Forms.Padding(2);
            Name = "PolicyType";
            Size = new System.Drawing.Size(1172, 782);
            Load += PolicyType_Load;
            Validated += PolicyType_Validated;
            panelSupplName.ResumeLayout(false);
            panelSupplName.PerformLayout();
            panelSuppl_Base.ResumeLayout(false);
            panelSuppl_Base.PerformLayout();
            basePolicyValidation_Panel.ResumeLayout(false);
            basePolicyValidation_Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Verified_PictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)basePolicy_PictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)suppPolicy_PictureBox).EndInit();
            panel_MultiPolicy.ResumeLayout(false);
            panel_MultiPolicy.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelBase;
        private System.Windows.Forms.Label labelSupp;
        private System.Windows.Forms.Panel panelSupplName;
        private System.Windows.Forms.Panel panelSuppl_Base;
        private System.Windows.Forms.Panel basePolicyValidation_Panel;
        private System.Windows.Forms.PictureBox Verified_PictureBox;
        private System.Windows.Forms.Label Verified_Label;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.TextBox textBoxBasePolicyPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Browse_Supp;
        private System.Windows.Forms.TextBox textBox_PolicyName;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxSuppPath;
        private System.Windows.Forms.Label label_fileLocation;
        private System.Windows.Forms.PictureBox basePolicy_PictureBox;
        private System.Windows.Forms.PictureBox suppPolicy_PictureBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label_LearnMore;
        private System.Windows.Forms.Panel panel_MultiPolicy;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.RadioButton radioButton_MultiplePolicy;
        private System.Windows.Forms.RadioButton radioButton_SinglePolicy;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxBasePolicyID;
    }
}
