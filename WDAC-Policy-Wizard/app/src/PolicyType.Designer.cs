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
            multiPolicyCheckbox = new System.Windows.Forms.PictureBox();
            appIdPolicyCheckbox = new System.Windows.Forms.PictureBox();
            singlePolicyCheckbox = new System.Windows.Forms.PictureBox();
            label11 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label14 = new System.Windows.Forms.Label();
            label_LearnMore_AppId = new System.Windows.Forms.Label();
            appIdPolicy_Panel = new System.Windows.Forms.Panel();
            browseAppId_Button = new System.Windows.Forms.Button();
            appIdPolicyName_Textbox = new System.Windows.Forms.TextBox();
            label20 = new System.Windows.Forms.Label();
            appIdPolicyLocation_Textbox = new System.Windows.Forms.TextBox();
            label21 = new System.Windows.Forms.Label();
            panelSupplName.SuspendLayout();
            panelSuppl_Base.SuspendLayout();
            basePolicyValidation_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Verified_PictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)basePolicy_PictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)suppPolicy_PictureBox).BeginInit();
            panel_MultiPolicy.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)multiPolicyCheckbox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)appIdPolicyCheckbox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)singlePolicyCheckbox).BeginInit();
            appIdPolicy_Panel.SuspendLayout();
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
            labelBase.Location = new System.Drawing.Point(41, 9);
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
            labelSupp.Location = new System.Drawing.Point(41, 76);
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
            panelSupplName.Location = new System.Drawing.Point(6, 136);
            panelSupplName.Margin = new System.Windows.Forms.Padding(2);
            panelSupplName.Name = "panelSupplName";
            panelSupplName.Size = new System.Drawing.Size(811, 282);
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
            panelSuppl_Base.Size = new System.Drawing.Size(805, 172);
            panelSuppl_Base.TabIndex = 96;
            panelSuppl_Base.Visible = false;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label10.ForeColor = System.Drawing.Color.Black;
            label10.Location = new System.Drawing.Point(568, 46);
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
            textBoxBasePolicyID.Location = new System.Drawing.Point(170, 43);
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
            basePolicyValidation_Panel.Location = new System.Drawing.Point(32, 114);
            basePolicyValidation_Panel.Margin = new System.Windows.Forms.Padding(2);
            basePolicyValidation_Panel.Name = "basePolicyValidation_Panel";
            basePolicyValidation_Panel.Size = new System.Drawing.Size(721, 35);
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
            label5.Location = new System.Drawing.Point(35, 87);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(119, 18);
            label5.TabIndex = 97;
            label5.Text = "Base Policy Path:";
            // 
            // button_Browse
            // 
            button_Browse.Font = new System.Drawing.Font("Tahoma", 9F);
            button_Browse.Location = new System.Drawing.Point(561, 82);
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
            textBoxBasePolicyPath.Location = new System.Drawing.Point(170, 84);
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
            basePolicy_PictureBox.Location = new System.Drawing.Point(11, 9);
            basePolicy_PictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            basePolicy_PictureBox.Name = "basePolicy_PictureBox";
            basePolicy_PictureBox.Size = new System.Drawing.Size(20, 20);
            basePolicy_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            basePolicy_PictureBox.TabIndex = 12;
            basePolicy_PictureBox.TabStop = false;
            basePolicy_PictureBox.Tag = "Selected";
            basePolicy_PictureBox.Click += BasePolicy_Selected;
            // 
            // suppPolicy_PictureBox
            // 
            suppPolicy_PictureBox.Image = Properties.Resources.radio_off;
            suppPolicy_PictureBox.Location = new System.Drawing.Point(9, 76);
            suppPolicy_PictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            suppPolicy_PictureBox.Name = "suppPolicy_PictureBox";
            suppPolicy_PictureBox.Size = new System.Drawing.Size(20, 20);
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
            label3.Location = new System.Drawing.Point(41, 37);
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
            label4.Location = new System.Drawing.Point(41, 106);
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
            label_LearnMore.Location = new System.Drawing.Point(198, 125);
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
            panel_MultiPolicy.Location = new System.Drawing.Point(198, 311);
            panel_MultiPolicy.Name = "panel_MultiPolicy";
            panel_MultiPolicy.Size = new System.Drawing.Size(874, 427);
            panel_MultiPolicy.TabIndex = 101;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label8.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label8.Location = new System.Drawing.Point(198, 103);
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
            label9.Location = new System.Drawing.Point(198, 186);
            label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(775, 18);
            label9.TabIndex = 104;
            label9.Text = "Create an App Control for Business policy to be deployed on any Windows 10 device, including Server 2016 and 2019.\r\n";
            // 
            // multiPolicyCheckbox
            // 
            multiPolicyCheckbox.Image = Properties.Resources.radio_on;
            multiPolicyCheckbox.Location = new System.Drawing.Point(170, 79);
            multiPolicyCheckbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            multiPolicyCheckbox.Name = "multiPolicyCheckbox";
            multiPolicyCheckbox.Size = new System.Drawing.Size(20, 20);
            multiPolicyCheckbox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            multiPolicyCheckbox.TabIndex = 16;
            multiPolicyCheckbox.TabStop = false;
            multiPolicyCheckbox.Tag = "Selected";
            multiPolicyCheckbox.Click += MultipleFormat_ButtonClick;
            // 
            // appIdPolicyCheckbox
            // 
            appIdPolicyCheckbox.Image = Properties.Resources.radio_off;
            appIdPolicyCheckbox.Location = new System.Drawing.Point(170, 225);
            appIdPolicyCheckbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            appIdPolicyCheckbox.Name = "appIdPolicyCheckbox";
            appIdPolicyCheckbox.Size = new System.Drawing.Size(20, 20);
            appIdPolicyCheckbox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            appIdPolicyCheckbox.TabIndex = 16;
            appIdPolicyCheckbox.TabStop = false;
            appIdPolicyCheckbox.Tag = "Unselected";
            appIdPolicyCheckbox.Click += AppIdPolicy_Click;
            // 
            // singlePolicyCheckbox
            // 
            singlePolicyCheckbox.Image = Properties.Resources.radio_off;
            singlePolicyCheckbox.Location = new System.Drawing.Point(170, 160);
            singlePolicyCheckbox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            singlePolicyCheckbox.Name = "singlePolicyCheckbox";
            singlePolicyCheckbox.Size = new System.Drawing.Size(20, 20);
            singlePolicyCheckbox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            singlePolicyCheckbox.TabIndex = 17;
            singlePolicyCheckbox.TabStop = false;
            singlePolicyCheckbox.Tag = "Unselected";
            singlePolicyCheckbox.Click += SingleFormat_ButtonClick;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label11.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label11.Location = new System.Drawing.Point(198, 78);
            label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(183, 22);
            label11.TabIndex = 105;
            label11.Text = "Multiple Policy Format";
            label11.Click += MultipleFormat_ButtonClick;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label12.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label12.Location = new System.Drawing.Point(198, 159);
            label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(169, 22);
            label12.TabIndex = 106;
            label12.Text = "Single Policy Format";
            label12.Click += SingleFormat_ButtonClick;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Font = new System.Drawing.Font("Tahoma", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label13.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label13.Location = new System.Drawing.Point(198, 225);
            label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(173, 22);
            label13.TabIndex = 108;
            label13.Text = "AppIdTagging Policy";
            label13.Click += AppIdPolicy_Click;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label14.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label14.Location = new System.Drawing.Point(198, 252);
            label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(874, 18);
            label14.TabIndex = 107;
            label14.Text = "Create an App Control for Business Tagging policy to be deployed on any Windows 11 device, including Server 2019, 2022 and 2025.\r\n";
            // 
            // label_LearnMore_AppId
            // 
            label_LearnMore_AppId.AutoSize = true;
            label_LearnMore_AppId.Font = new System.Drawing.Font("Tahoma", 9F);
            label_LearnMore_AppId.ForeColor = System.Drawing.Color.FromArgb(16, 110, 190);
            label_LearnMore_AppId.Image = Properties.Resources.external_link_symbol_highlight;
            label_LearnMore_AppId.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            label_LearnMore_AppId.Location = new System.Drawing.Point(198, 272);
            label_LearnMore_AppId.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label_LearnMore_AppId.Name = "label_LearnMore_AppId";
            label_LearnMore_AppId.Size = new System.Drawing.Size(295, 18);
            label_LearnMore_AppId.TabIndex = 109;
            label_LearnMore_AppId.Tag = "IgnoreDarkMode";
            label_LearnMore_AppId.Text = "Learn more about AppId Tagging policies    \r\n";
            label_LearnMore_AppId.Click += Label_LearnMoreAppId_Click;
            // 
            // appIdPolicy_Panel
            // 
            appIdPolicy_Panel.BackColor = System.Drawing.Color.Transparent;
            appIdPolicy_Panel.Controls.Add(browseAppId_Button);
            appIdPolicy_Panel.Controls.Add(appIdPolicyName_Textbox);
            appIdPolicy_Panel.Controls.Add(label20);
            appIdPolicy_Panel.Controls.Add(appIdPolicyLocation_Textbox);
            appIdPolicy_Panel.Controls.Add(label21);
            appIdPolicy_Panel.Location = new System.Drawing.Point(64, 763);
            appIdPolicy_Panel.Margin = new System.Windows.Forms.Padding(2);
            appIdPolicy_Panel.Name = "appIdPolicy_Panel";
            appIdPolicy_Panel.Size = new System.Drawing.Size(811, 116);
            appIdPolicy_Panel.TabIndex = 110;
            appIdPolicy_Panel.Visible = false;
            // 
            // browseAppId_Button
            // 
            browseAppId_Button.Font = new System.Drawing.Font("Tahoma", 9F);
            browseAppId_Button.Location = new System.Drawing.Point(537, 53);
            browseAppId_Button.Margin = new System.Windows.Forms.Padding(2);
            browseAppId_Button.Name = "browseAppId_Button";
            browseAppId_Button.Size = new System.Drawing.Size(107, 28);
            browseAppId_Button.TabIndex = 3;
            browseAppId_Button.Text = "Browse";
            browseAppId_Button.UseVisualStyleBackColor = true;
            browseAppId_Button.Click += AppIdPolicyLocation_Click;
            // 
            // appIdPolicyName_Textbox
            // 
            appIdPolicyName_Textbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            appIdPolicyName_Textbox.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            appIdPolicyName_Textbox.ForeColor = System.Drawing.Color.Black;
            appIdPolicyName_Textbox.Location = new System.Drawing.Point(146, 11);
            appIdPolicyName_Textbox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            appIdPolicyName_Textbox.Name = "appIdPolicyName_Textbox";
            appIdPolicyName_Textbox.Size = new System.Drawing.Size(381, 25);
            appIdPolicyName_Textbox.TabIndex = 1;
            appIdPolicyName_Textbox.TextChanged += AppIdPolicyNameTextbox_TextChanged;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label20.ForeColor = System.Drawing.Color.Black;
            label20.Location = new System.Drawing.Point(12, 14);
            label20.Name = "label20";
            label20.Size = new System.Drawing.Size(92, 18);
            label20.TabIndex = 8;
            label20.Text = "Policy Name:";
            // 
            // appIdPolicyLocation_Textbox
            // 
            appIdPolicyLocation_Textbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            appIdPolicyLocation_Textbox.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            appIdPolicyLocation_Textbox.ForeColor = System.Drawing.Color.Black;
            appIdPolicyLocation_Textbox.Location = new System.Drawing.Point(146, 55);
            appIdPolicyLocation_Textbox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            appIdPolicyLocation_Textbox.Name = "appIdPolicyLocation_Textbox";
            appIdPolicyLocation_Textbox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            appIdPolicyLocation_Textbox.Size = new System.Drawing.Size(381, 25);
            appIdPolicyLocation_Textbox.TabIndex = 2;
            appIdPolicyLocation_Textbox.Click += AppIdPolicyLocation_Click;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label21.ForeColor = System.Drawing.Color.Black;
            label21.Location = new System.Drawing.Point(12, 58);
            label21.Name = "label21";
            label21.Size = new System.Drawing.Size(131, 18);
            label21.TabIndex = 6;
            label21.Text = "Policy File Location:";
            // 
            // PolicyType
            // 
            AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.White;
            Controls.Add(appIdPolicy_Panel);
            Controls.Add(label_LearnMore_AppId);
            Controls.Add(label13);
            Controls.Add(label14);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(appIdPolicyCheckbox);
            Controls.Add(singlePolicyCheckbox);
            Controls.Add(multiPolicyCheckbox);
            Controls.Add(label_LearnMore);
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
            ((System.ComponentModel.ISupportInitialize)multiPolicyCheckbox).EndInit();
            ((System.ComponentModel.ISupportInitialize)appIdPolicyCheckbox).EndInit();
            ((System.ComponentModel.ISupportInitialize)singlePolicyCheckbox).EndInit();
            appIdPolicy_Panel.ResumeLayout(false);
            appIdPolicy_Panel.PerformLayout();
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
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxBasePolicyID;
        private System.Windows.Forms.PictureBox multiPolicyCheckbox;
        private System.Windows.Forms.PictureBox appIdPolicyCheckbox;
        private System.Windows.Forms.PictureBox singlePolicyCheckbox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label_LearnMore_AppId;
        private System.Windows.Forms.Panel appIdPolicy_Panel;
        private System.Windows.Forms.Button browseAppId_Button;
        private System.Windows.Forms.TextBox appIdPolicyName_Textbox;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox appIdPolicyLocation_Textbox;
        private System.Windows.Forms.Label label21;
    }
}
