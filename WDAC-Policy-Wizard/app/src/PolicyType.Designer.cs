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
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxBasePolicyID = new System.Windows.Forms.TextBox();
            this.basePolicyValidation_Panel = new System.Windows.Forms.Panel();
            this.Verified_PictureBox = new System.Windows.Forms.PictureBox();
            this.Verified_Label = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
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
            this.label_LearnMore = new System.Windows.Forms.Label();
            this.panel_MultiPolicy = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.radioButton_MultiplePolicy = new System.Windows.Forms.RadioButton();
            this.radioButton_SinglePolicy = new System.Windows.Forms.RadioButton();
            this.panelSupplName.SuspendLayout();
            this.panelSuppl_Base.SuspendLayout();
            this.basePolicyValidation_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Verified_PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePolicy_PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.suppPolicy_PictureBox)).BeginInit();
            this.panel_MultiPolicy.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(164, 19);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(224, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select a Policy Type";
            // 
            // labelBase
            // 
            this.labelBase.AutoSize = true;
            this.labelBase.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.labelBase.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelBase.Location = new System.Drawing.Point(41, 26);
            this.labelBase.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelBase.Name = "labelBase";
            this.labelBase.Size = new System.Drawing.Size(87, 19);
            this.labelBase.TabIndex = 3;
            this.labelBase.Text = "Base Policy";
            this.labelBase.Click += new System.EventHandler(this.BasePolicy_Selected);
            // 
            // labelSupp
            // 
            this.labelSupp.AutoSize = true;
            this.labelSupp.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.labelSupp.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.labelSupp.Location = new System.Drawing.Point(41, 93);
            this.labelSupp.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelSupp.Name = "labelSupp";
            this.labelSupp.Size = new System.Drawing.Size(151, 19);
            this.labelSupp.TabIndex = 4;
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
            this.panelSupplName.Location = new System.Drawing.Point(6, 165);
            this.panelSupplName.Margin = new System.Windows.Forms.Padding(2);
            this.panelSupplName.Name = "panelSupplName";
            this.panelSupplName.Size = new System.Drawing.Size(811, 344);
            this.panelSupplName.TabIndex = 11;
            this.panelSupplName.Visible = false;
            // 
            // panelSuppl_Base
            // 
            this.panelSuppl_Base.Controls.Add(this.label10);
            this.panelSuppl_Base.Controls.Add(this.label7);
            this.panelSuppl_Base.Controls.Add(this.textBoxBasePolicyID);
            this.panelSuppl_Base.Controls.Add(this.basePolicyValidation_Panel);
            this.panelSuppl_Base.Controls.Add(this.label5);
            this.panelSuppl_Base.Controls.Add(this.button_Browse);
            this.panelSuppl_Base.Controls.Add(this.textBoxBasePolicyPath);
            this.panelSuppl_Base.Controls.Add(this.label2);
            this.panelSuppl_Base.Location = new System.Drawing.Point(3, 101);
            this.panelSuppl_Base.Name = "panelSuppl_Base";
            this.panelSuppl_Base.Size = new System.Drawing.Size(805, 212);
            this.panelSuppl_Base.TabIndex = 96;
            this.panelSuppl_Base.Visible = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(75, 78);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(28, 18);
            this.label10.TabIndex = 102;
            this.label10.Text = "OR";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(34, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 18);
            this.label7.TabIndex = 101;
            this.label7.Text = "Base Policy ID:";
            // 
            // textBoxBasePolicyID
            // 
            this.textBoxBasePolicyID.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.textBoxBasePolicyID.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.textBoxBasePolicyID.Location = new System.Drawing.Point(169, 43);
            this.textBoxBasePolicyID.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxBasePolicyID.Name = "textBoxBasePolicyID";
            this.textBoxBasePolicyID.Size = new System.Drawing.Size(381, 25);
            this.textBoxBasePolicyID.TabIndex = 100;
            this.textBoxBasePolicyID.Text = "e.g. {fd756ea8-ad7f-4e30-96bd-8778288212f6}";
            this.textBoxBasePolicyID.Click += new System.EventHandler(this.TextBoxBasePolicyID_Selected);
            this.textBoxBasePolicyID.TextChanged += new System.EventHandler(this.TextBoxBasePolicyID_TextChanged);
            this.textBoxBasePolicyID.DoubleClick += new System.EventHandler(this.TextBoxBasePolicyID_Selected);
            this.textBoxBasePolicyID.Leave += new System.EventHandler(this.TextBoxBasePolicyID_Leave);
            // 
            // basePolicyValidation_Panel
            // 
            this.basePolicyValidation_Panel.Controls.Add(this.Verified_PictureBox);
            this.basePolicyValidation_Panel.Controls.Add(this.Verified_Label);
            this.basePolicyValidation_Panel.Location = new System.Drawing.Point(10, 152);
            this.basePolicyValidation_Panel.Margin = new System.Windows.Forms.Padding(2);
            this.basePolicyValidation_Panel.Name = "basePolicyValidation_Panel";
            this.basePolicyValidation_Panel.Size = new System.Drawing.Size(793, 35);
            this.basePolicyValidation_Panel.TabIndex = 99;
            this.basePolicyValidation_Panel.Visible = false;
            // 
            // Verified_PictureBox
            // 
            this.Verified_PictureBox.Image = global::WDAC_Wizard.Properties.Resources.verified;
            this.Verified_PictureBox.Location = new System.Drawing.Point(4, 5);
            this.Verified_PictureBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Verified_PictureBox.Name = "Verified_PictureBox";
            this.Verified_PictureBox.Size = new System.Drawing.Size(23, 26);
            this.Verified_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Verified_PictureBox.TabIndex = 94;
            this.Verified_PictureBox.TabStop = false;
            this.Verified_PictureBox.Visible = false;
            // 
            // Verified_Label
            // 
            this.Verified_Label.AutoSize = true;
            this.Verified_Label.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Verified_Label.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Verified_Label.Location = new System.Drawing.Point(37, 8);
            this.Verified_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Verified_Label.Name = "Verified_Label";
            this.Verified_Label.Size = new System.Drawing.Size(297, 18);
            this.Verified_Label.TabIndex = 16;
            this.Verified_Label.Text = "This base policy allows supplemental policies.";
            this.Verified_Label.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(34, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 18);
            this.label5.TabIndex = 97;
            this.label5.Text = "Base Policy Path:";
            // 
            // button_Browse
            // 
            this.button_Browse.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.button_Browse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Browse.Font = new System.Drawing.Font("Tahoma", 9F);
            this.button_Browse.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_Browse.Location = new System.Drawing.Point(560, 105);
            this.button_Browse.Margin = new System.Windows.Forms.Padding(2);
            this.button_Browse.Name = "button_Browse";
            this.button_Browse.Size = new System.Drawing.Size(107, 28);
            this.button_Browse.TabIndex = 93;
            this.button_Browse.Text = "Browse";
            this.button_Browse.UseVisualStyleBackColor = true;
            this.button_Browse.Click += new System.EventHandler(this.Button_Browse_Click);
            // 
            // textBoxBasePolicyPath
            // 
            this.textBoxBasePolicyPath.Font = new System.Drawing.Font("Tahoma", 8.5F);
            this.textBoxBasePolicyPath.Location = new System.Drawing.Point(169, 107);
            this.textBoxBasePolicyPath.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxBasePolicyPath.Name = "textBoxBasePolicyPath";
            this.textBoxBasePolicyPath.Size = new System.Drawing.Size(381, 25);
            this.textBoxBasePolicyPath.TabIndex = 14;
            this.textBoxBasePolicyPath.Click += new System.EventHandler(this.Button_Browse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(35, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(621, 19);
            this.label2.TabIndex = 12;
            this.label2.Text = "Select the policy that the supplemental policy will apply to or enter the base po" +
    "licy ID. ";
            // 
            // button_Browse_Supp
            // 
            this.button_Browse_Supp.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.button_Browse_Supp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Browse_Supp.Font = new System.Drawing.Font("Tahoma", 9F);
            this.button_Browse_Supp.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_Browse_Supp.Location = new System.Drawing.Point(563, 53);
            this.button_Browse_Supp.Margin = new System.Windows.Forms.Padding(2);
            this.button_Browse_Supp.Name = "button_Browse_Supp";
            this.button_Browse_Supp.Size = new System.Drawing.Size(107, 28);
            this.button_Browse_Supp.TabIndex = 3;
            this.button_Browse_Supp.Text = "Browse";
            this.button_Browse_Supp.UseVisualStyleBackColor = true;
            this.button_Browse_Supp.Click += new System.EventHandler(this.Button_BrowseSupp_Click);
            // 
            // textBox_PolicyName
            // 
            this.textBox_PolicyName.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_PolicyName.ForeColor = System.Drawing.Color.Black;
            this.textBox_PolicyName.Location = new System.Drawing.Point(172, 11);
            this.textBox_PolicyName.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox_PolicyName.Name = "textBox_PolicyName";
            this.textBox_PolicyName.Size = new System.Drawing.Size(381, 25);
            this.textBox_PolicyName.TabIndex = 1;
            this.textBox_PolicyName.TextChanged += new System.EventHandler(this.TextBox_PolicyName_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(38, 14);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 18);
            this.label6.TabIndex = 8;
            this.label6.Text = "Policy Name:";
            // 
            // textBoxSuppPath
            // 
            this.textBoxSuppPath.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSuppPath.ForeColor = System.Drawing.Color.Black;
            this.textBoxSuppPath.Location = new System.Drawing.Point(172, 55);
            this.textBoxSuppPath.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBoxSuppPath.Name = "textBoxSuppPath";
            this.textBoxSuppPath.Size = new System.Drawing.Size(381, 25);
            this.textBoxSuppPath.TabIndex = 2;
            this.textBoxSuppPath.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxSuppPath.Click += new System.EventHandler(this.Button_BrowseSupp_Click);
            // 
            // label_fileLocation
            // 
            this.label_fileLocation.AutoSize = true;
            this.label_fileLocation.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_fileLocation.ForeColor = System.Drawing.Color.Black;
            this.label_fileLocation.Location = new System.Drawing.Point(38, 58);
            this.label_fileLocation.Name = "label_fileLocation";
            this.label_fileLocation.Size = new System.Drawing.Size(131, 18);
            this.label_fileLocation.TabIndex = 6;
            this.label_fileLocation.Text = "Policy File Location:";
            // 
            // basePolicy_PictureBox
            // 
            this.basePolicy_PictureBox.Image = global::WDAC_Wizard.Properties.Resources.radio_on;
            this.basePolicy_PictureBox.Location = new System.Drawing.Point(6, 23);
            this.basePolicy_PictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.basePolicy_PictureBox.Name = "basePolicy_PictureBox";
            this.basePolicy_PictureBox.Size = new System.Drawing.Size(25, 25);
            this.basePolicy_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.basePolicy_PictureBox.TabIndex = 12;
            this.basePolicy_PictureBox.TabStop = false;
            this.basePolicy_PictureBox.Tag = "Selected";
            this.basePolicy_PictureBox.Click += new System.EventHandler(this.BasePolicy_Selected);
            // 
            // suppPolicy_PictureBox
            // 
            this.suppPolicy_PictureBox.Image = global::WDAC_Wizard.Properties.Resources.radio_off;
            this.suppPolicy_PictureBox.Location = new System.Drawing.Point(6, 89);
            this.suppPolicy_PictureBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.suppPolicy_PictureBox.Name = "suppPolicy_PictureBox";
            this.suppPolicy_PictureBox.Size = new System.Drawing.Size(25, 25);
            this.suppPolicy_PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.suppPolicy_PictureBox.TabIndex = 13;
            this.suppPolicy_PictureBox.TabStop = false;
            this.suppPolicy_PictureBox.Tag = "Unselected";
            this.suppPolicy_PictureBox.Click += new System.EventHandler(this.SupplementalPolicy_Selected);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label3.Location = new System.Drawing.Point(41, 54);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(698, 18);
            this.label3.TabIndex = 14;
            this.label3.Text = "Creates a new code integrity policy for the system. Multiple base policies can be" +
    " enforced simultaneously. ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label4.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label4.Location = new System.Drawing.Point(41, 123);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(452, 18);
            this.label4.TabIndex = 15;
            this.label4.Text = "Creates a code integrity policy to expand a pre-existing base policy. ";
            // 
            // label_LearnMore
            // 
            this.label_LearnMore.AutoSize = true;
            this.label_LearnMore.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label_LearnMore.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.label_LearnMore.Image = global::WDAC_Wizard.Properties.Resources.external_link_symbol_highlight;
            this.label_LearnMore.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label_LearnMore.Location = new System.Drawing.Point(198, 117);
            this.label_LearnMore.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_LearnMore.Name = "label_LearnMore";
            this.label_LearnMore.Size = new System.Drawing.Size(287, 18);
            this.label_LearnMore.TabIndex = 98;
            this.label_LearnMore.Text = "Learn more about multiple policy format    ";
            this.label_LearnMore.Click += new System.EventHandler(this.Label_LearnMore_Click);
            // 
            // panel_MultiPolicy
            // 
            this.panel_MultiPolicy.Controls.Add(this.label4);
            this.panel_MultiPolicy.Controls.Add(this.label3);
            this.panel_MultiPolicy.Controls.Add(this.suppPolicy_PictureBox);
            this.panel_MultiPolicy.Controls.Add(this.basePolicy_PictureBox);
            this.panel_MultiPolicy.Controls.Add(this.panelSupplName);
            this.panel_MultiPolicy.Controls.Add(this.labelSupp);
            this.panel_MultiPolicy.Controls.Add(this.labelBase);
            this.panel_MultiPolicy.Location = new System.Drawing.Point(169, 242);
            this.panel_MultiPolicy.Name = "panel_MultiPolicy";
            this.panel_MultiPolicy.Size = new System.Drawing.Size(827, 511);
            this.panel_MultiPolicy.TabIndex = 101;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label8.Location = new System.Drawing.Point(198, 95);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(825, 18);
            this.label8.TabIndex = 99;
            this.label8.Text = "Create a base or a supplemental policy. Windows 10 version 1903 and above support" +
    "s up to 32 active policies on one device.\r\n";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label9.Location = new System.Drawing.Point(198, 185);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(650, 18);
            this.label9.TabIndex = 104;
            this.label9.Text = "Create a WDAC policy to be deployed on any Windows 10 device, including Server 20" +
    "16 and 2019.\r\n";
            // 
            // radioButton_MultiplePolicy
            // 
            this.radioButton_MultiplePolicy.AutoSize = true;
            this.radioButton_MultiplePolicy.Checked = true;
            this.radioButton_MultiplePolicy.Font = new System.Drawing.Font("Tahoma", 10F);
            this.radioButton_MultiplePolicy.Location = new System.Drawing.Point(178, 70);
            this.radioButton_MultiplePolicy.Name = "radioButton_MultiplePolicy";
            this.radioButton_MultiplePolicy.Size = new System.Drawing.Size(194, 25);
            this.radioButton_MultiplePolicy.TabIndex = 2;
            this.radioButton_MultiplePolicy.TabStop = true;
            this.radioButton_MultiplePolicy.Text = "Multiple Policy Format";
            this.radioButton_MultiplePolicy.UseVisualStyleBackColor = true;
            this.radioButton_MultiplePolicy.Click += new System.EventHandler(this.MultipleFormat_ButtonClick);
            // 
            // radioButton_SinglePolicy
            // 
            this.radioButton_SinglePolicy.AutoSize = true;
            this.radioButton_SinglePolicy.Font = new System.Drawing.Font("Tahoma", 10F);
            this.radioButton_SinglePolicy.Location = new System.Drawing.Point(178, 160);
            this.radioButton_SinglePolicy.Name = "radioButton_SinglePolicy";
            this.radioButton_SinglePolicy.Size = new System.Drawing.Size(180, 25);
            this.radioButton_SinglePolicy.TabIndex = 1;
            this.radioButton_SinglePolicy.Text = "Single Policy Format";
            this.radioButton_SinglePolicy.UseVisualStyleBackColor = true;
            this.radioButton_SinglePolicy.Click += new System.EventHandler(this.SingleFormat_ButtonClick);
            // 
            // PolicyType
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label_LearnMore);
            this.Controls.Add(this.radioButton_SinglePolicy);
            this.Controls.Add(this.radioButton_MultiplePolicy);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.panel_MultiPolicy);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PolicyType";
            this.Size = new System.Drawing.Size(1172, 782);
            this.Load += new System.EventHandler(this.PolicyType_Load);
            this.panelSupplName.ResumeLayout(false);
            this.panelSupplName.PerformLayout();
            this.panelSuppl_Base.ResumeLayout(false);
            this.panelSuppl_Base.PerformLayout();
            this.basePolicyValidation_Panel.ResumeLayout(false);
            this.basePolicyValidation_Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Verified_PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePolicy_PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.suppPolicy_PictureBox)).EndInit();
            this.panel_MultiPolicy.ResumeLayout(false);
            this.panel_MultiPolicy.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
