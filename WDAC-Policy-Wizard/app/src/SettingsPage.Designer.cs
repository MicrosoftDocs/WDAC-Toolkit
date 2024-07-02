// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

namespace WDAC_Wizard
{
    partial class SettingsPage
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
            label2 = new System.Windows.Forms.Label();
            appVersion_Label = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            terms_Label = new System.Windows.Forms.Label();
            privacy_Label = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            useDefaultStrings_CheckBox = new System.Windows.Forms.PictureBox();
            label6 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            useDarkMode_Checkbox = new System.Windows.Forms.PictureBox();
            label10 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label_DriverBlock = new System.Windows.Forms.Label();
            label_UsermodeBlock = new System.Windows.Forms.Label();
            useUsermodeBlockRules_CheckBox = new System.Windows.Forms.PictureBox();
            useDriverBlockRules_CheckBox = new System.Windows.Forms.PictureBox();
            convertPolicyToBinary_CheckBox = new System.Windows.Forms.PictureBox();
            label7 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            useEnvVars_CheckBox = new System.Windows.Forms.PictureBox();
            label8 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            resetButton = new System.Windows.Forms.Button();
            Update_Label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)useDefaultStrings_CheckBox).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)useDarkMode_Checkbox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)useUsermodeBlockRules_CheckBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)useDriverBlockRules_CheckBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)convertPolicyToBinary_CheckBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)useEnvVars_CheckBox).BeginInit();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Tahoma", 11F);
            label2.Location = new System.Drawing.Point(25, 371);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(59, 23);
            label2.TabIndex = 1;
            label2.Text = "About";
            // 
            // appVersion_Label
            // 
            appVersion_Label.AutoSize = true;
            appVersion_Label.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            appVersion_Label.ForeColor = System.Drawing.SystemColors.ControlDark;
            appVersion_Label.Location = new System.Drawing.Point(37, 407);
            appVersion_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            appVersion_Label.Name = "appVersion_Label";
            appVersion_Label.Size = new System.Drawing.Size(168, 21);
            appVersion_Label.TabIndex = 2;
            appVersion_Label.Text = "App Version 1.903.22";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label3.ForeColor = System.Drawing.SystemColors.ControlDark;
            label3.Location = new System.Drawing.Point(37, 435);
            label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(243, 42);
            label3.TabIndex = 3;
            label3.Text = "2019 Microsoft Corporation ©\r\nJordan.Geurten@microsoft.com";
            // 
            // terms_Label
            // 
            terms_Label.AutoSize = true;
            terms_Label.BackColor = System.Drawing.Color.Transparent;
            terms_Label.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            terms_Label.ForeColor = System.Drawing.Color.FromArgb(16, 110, 190);
            terms_Label.Image = Properties.Resources.external_link_symbol_highlight;
            terms_Label.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            terms_Label.Location = new System.Drawing.Point(35, 483);
            terms_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            terms_Label.Name = "terms_Label";
            terms_Label.Size = new System.Drawing.Size(129, 21);
            terms_Label.TabIndex = 10;
            terms_Label.Tag = "IgnoreDarkMode";
            terms_Label.Text = "Terms of Use    ";
            terms_Label.Click += Terms_Label_Click;
            // 
            // privacy_Label
            // 
            privacy_Label.AutoSize = true;
            privacy_Label.BackColor = System.Drawing.Color.Transparent;
            privacy_Label.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            privacy_Label.ForeColor = System.Drawing.Color.FromArgb(16, 110, 190);
            privacy_Label.Image = Properties.Resources.external_link_symbol_highlight;
            privacy_Label.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            privacy_Label.Location = new System.Drawing.Point(181, 483);
            privacy_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            privacy_Label.Name = "privacy_Label";
            privacy_Label.Size = new System.Drawing.Size(164, 21);
            privacy_Label.TabIndex = 11;
            privacy_Label.Tag = "IgnoreDarkMode";
            privacy_Label.Text = "Privacy Statement    ";
            privacy_Label.Click += Privacy_Label_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Tahoma", 11F);
            label4.Location = new System.Drawing.Point(25, 292);
            label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(78, 23);
            label4.TabIndex = 6;
            label4.Text = "Defaults";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label5.ForeColor = System.Drawing.Color.Black;
            label5.Location = new System.Drawing.Point(70, 332);
            label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(285, 21);
            label5.TabIndex = 7;
            label5.Text = "Autofill text fields with default values";
            // 
            // useDefaultStrings_CheckBox
            // 
            useDefaultStrings_CheckBox.BackgroundImage = Properties.Resources.check_box_checked;
            useDefaultStrings_CheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            useDefaultStrings_CheckBox.Location = new System.Drawing.Point(38, 329);
            useDefaultStrings_CheckBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            useDefaultStrings_CheckBox.Name = "useDefaultStrings_CheckBox";
            useDefaultStrings_CheckBox.Size = new System.Drawing.Size(25, 25);
            useDefaultStrings_CheckBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            useDefaultStrings_CheckBox.TabIndex = 8;
            useDefaultStrings_CheckBox.TabStop = false;
            useDefaultStrings_CheckBox.Tag = "Checked";
            useDefaultStrings_CheckBox.Click += DefaultString_CheckBox_Click;
            useDefaultStrings_CheckBox.MouseLeave += SettingCheckBox_Leave;
            useDefaultStrings_CheckBox.MouseHover += SettingCheckBox_Hover;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label6.ForeColor = System.Drawing.Color.Black;
            label6.Location = new System.Drawing.Point(72, 52);
            label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(0, 21);
            label6.TabIndex = 9;
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.Controls.Add(useDarkMode_Checkbox);
            panel1.Controls.Add(label10);
            panel1.Controls.Add(label11);
            panel1.Controls.Add(label_DriverBlock);
            panel1.Controls.Add(label_UsermodeBlock);
            panel1.Controls.Add(useUsermodeBlockRules_CheckBox);
            panel1.Controls.Add(useDriverBlockRules_CheckBox);
            panel1.Controls.Add(convertPolicyToBinary_CheckBox);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(label9);
            panel1.Controls.Add(useEnvVars_CheckBox);
            panel1.Controls.Add(label8);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(useDefaultStrings_CheckBox);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(privacy_Label);
            panel1.Controls.Add(terms_Label);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(appVersion_Label);
            panel1.Controls.Add(label2);
            panel1.Location = new System.Drawing.Point(164, 75);
            panel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(713, 528);
            panel1.TabIndex = 12;
            // 
            // useDarkMode_Checkbox
            // 
            useDarkMode_Checkbox.BackgroundImage = Properties.Resources.check_box_unchecked;
            useDarkMode_Checkbox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            useDarkMode_Checkbox.Location = new System.Drawing.Point(38, 247);
            useDarkMode_Checkbox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            useDarkMode_Checkbox.Name = "useDarkMode_Checkbox";
            useDarkMode_Checkbox.Size = new System.Drawing.Size(25, 25);
            useDarkMode_Checkbox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            useDarkMode_Checkbox.TabIndex = 28;
            useDarkMode_Checkbox.TabStop = false;
            useDarkMode_Checkbox.Tag = "Checked";
            useDarkMode_Checkbox.Click += DarkMode_CheckBox_Click;
            useDarkMode_Checkbox.MouseLeave += SettingCheckBox_Leave;
            useDarkMode_Checkbox.MouseHover += SettingCheckBox_Hover;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label10.ForeColor = System.Drawing.Color.Black;
            label10.Location = new System.Drawing.Point(70, 250);
            label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(226, 21);
            label10.TabIndex = 27;
            label10.Text = "Use the Wizard in Dark Mode";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Font = new System.Drawing.Font("Tahoma", 11F);
            label11.Location = new System.Drawing.Point(25, 210);
            label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(86, 23);
            label11.TabIndex = 26;
            label11.Text = "Interface";
            // 
            // label_DriverBlock
            // 
            label_DriverBlock.AutoSize = true;
            label_DriverBlock.BackColor = System.Drawing.Color.Transparent;
            label_DriverBlock.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label_DriverBlock.ForeColor = System.Drawing.Color.FromArgb(16, 110, 190);
            label_DriverBlock.Image = Properties.Resources.external_link_symbol_highlight;
            label_DriverBlock.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            label_DriverBlock.Location = new System.Drawing.Point(72, 169);
            label_DriverBlock.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label_DriverBlock.Name = "label_DriverBlock";
            label_DriverBlock.Size = new System.Drawing.Size(516, 21);
            label_DriverBlock.TabIndex = 25;
            label_DriverBlock.Tag = "IgnoreDarkMode";
            label_DriverBlock.Text = "Create policies with Microsoft's Recommended Driver Block Rules    ";
            label_DriverBlock.Click += LabelDriverBlock_Click;
            // 
            // label_UsermodeBlock
            // 
            label_UsermodeBlock.AutoSize = true;
            label_UsermodeBlock.BackColor = System.Drawing.Color.Transparent;
            label_UsermodeBlock.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label_UsermodeBlock.ForeColor = System.Drawing.Color.FromArgb(16, 110, 190);
            label_UsermodeBlock.Image = Properties.Resources.external_link_symbol_highlight;
            label_UsermodeBlock.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            label_UsermodeBlock.Location = new System.Drawing.Point(72, 129);
            label_UsermodeBlock.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label_UsermodeBlock.Name = "label_UsermodeBlock";
            label_UsermodeBlock.Size = new System.Drawing.Size(466, 21);
            label_UsermodeBlock.TabIndex = 24;
            label_UsermodeBlock.Tag = "IgnoreDarkMode";
            label_UsermodeBlock.Text = "Create policies with Microsoft's Recommended Block Rules    ";
            label_UsermodeBlock.Click += Label_UsermodeBlock_Click;
            // 
            // useUsermodeBlockRules_CheckBox
            // 
            useUsermodeBlockRules_CheckBox.AccessibleRole = System.Windows.Forms.AccessibleRole.CheckButton;
            useUsermodeBlockRules_CheckBox.BackgroundImage = Properties.Resources.check_box_unchecked;
            useUsermodeBlockRules_CheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            useUsermodeBlockRules_CheckBox.Location = new System.Drawing.Point(39, 126);
            useUsermodeBlockRules_CheckBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            useUsermodeBlockRules_CheckBox.Name = "useUsermodeBlockRules_CheckBox";
            useUsermodeBlockRules_CheckBox.Size = new System.Drawing.Size(25, 25);
            useUsermodeBlockRules_CheckBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            useUsermodeBlockRules_CheckBox.TabIndex = 23;
            useUsermodeBlockRules_CheckBox.TabStop = false;
            useUsermodeBlockRules_CheckBox.Tag = "Unchecked";
            useUsermodeBlockRules_CheckBox.Click += UsermodeRecList_checkBox_Click;
            useUsermodeBlockRules_CheckBox.MouseLeave += SettingCheckBox_Leave;
            useUsermodeBlockRules_CheckBox.MouseHover += SettingCheckBox_Hover;
            // 
            // useDriverBlockRules_CheckBox
            // 
            useDriverBlockRules_CheckBox.BackColor = System.Drawing.Color.White;
            useDriverBlockRules_CheckBox.BackgroundImage = Properties.Resources.check_box_unchecked;
            useDriverBlockRules_CheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            useDriverBlockRules_CheckBox.Location = new System.Drawing.Point(39, 166);
            useDriverBlockRules_CheckBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            useDriverBlockRules_CheckBox.Name = "useDriverBlockRules_CheckBox";
            useDriverBlockRules_CheckBox.Size = new System.Drawing.Size(25, 25);
            useDriverBlockRules_CheckBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            useDriverBlockRules_CheckBox.TabIndex = 21;
            useDriverBlockRules_CheckBox.TabStop = false;
            useDriverBlockRules_CheckBox.Tag = "Unchecked";
            useDriverBlockRules_CheckBox.Click += KernelmodeRecList_checkBox_Click;
            useDriverBlockRules_CheckBox.MouseLeave += SettingCheckBox_Leave;
            useDriverBlockRules_CheckBox.MouseHover += SettingCheckBox_Hover;
            // 
            // convertPolicyToBinary_CheckBox
            // 
            convertPolicyToBinary_CheckBox.BackgroundImage = Properties.Resources.check_box_unchecked;
            convertPolicyToBinary_CheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            convertPolicyToBinary_CheckBox.Location = new System.Drawing.Point(39, 86);
            convertPolicyToBinary_CheckBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            convertPolicyToBinary_CheckBox.Name = "convertPolicyToBinary_CheckBox";
            convertPolicyToBinary_CheckBox.Size = new System.Drawing.Size(25, 25);
            convertPolicyToBinary_CheckBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            convertPolicyToBinary_CheckBox.TabIndex = 19;
            convertPolicyToBinary_CheckBox.TabStop = false;
            convertPolicyToBinary_CheckBox.Tag = "Unchecked";
            convertPolicyToBinary_CheckBox.Click += ConvertPolicy_CheckBox_Click;
            convertPolicyToBinary_CheckBox.MouseLeave += SettingCheckBox_Leave;
            convertPolicyToBinary_CheckBox.MouseHover += SettingCheckBox_Hover;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label7.ForeColor = System.Drawing.Color.Black;
            label7.Location = new System.Drawing.Point(72, 89);
            label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(320, 21);
            label7.TabIndex = 18;
            label7.Text = "Convert policy to binary after xml creation";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new System.Drawing.Font("Tahoma", 11F);
            label9.Location = new System.Drawing.Point(27, 15);
            label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(56, 23);
            label9.TabIndex = 15;
            label9.Text = "Policy";
            // 
            // useEnvVars_CheckBox
            // 
            useEnvVars_CheckBox.BackgroundImage = Properties.Resources.check_box_checked;
            useEnvVars_CheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            useEnvVars_CheckBox.Location = new System.Drawing.Point(39, 50);
            useEnvVars_CheckBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            useEnvVars_CheckBox.Name = "useEnvVars_CheckBox";
            useEnvVars_CheckBox.Size = new System.Drawing.Size(25, 25);
            useEnvVars_CheckBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            useEnvVars_CheckBox.TabIndex = 14;
            useEnvVars_CheckBox.TabStop = false;
            useEnvVars_CheckBox.Tag = "Checked";
            useEnvVars_CheckBox.Click += EnvVar_CheckBox_Click;
            useEnvVars_CheckBox.MouseLeave += SettingCheckBox_Leave;
            useEnvVars_CheckBox.MouseHover += SettingCheckBox_Hover;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label8.ForeColor = System.Drawing.Color.Black;
            label8.Location = new System.Drawing.Point(72, 53);
            label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(441, 21);
            label8.TabIndex = 13;
            label8.Text = "Use environment variables in filepath rules where possible";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(164, 43);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(218, 29);
            label1.TabIndex = 13;
            label1.Text = "Application Settings";
            // 
            // resetButton
            // 
            resetButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            resetButton.Location = new System.Drawing.Point(785, 624);
            resetButton.Margin = new System.Windows.Forms.Padding(2);
            resetButton.Name = "resetButton";
            resetButton.Size = new System.Drawing.Size(92, 32);
            resetButton.TabIndex = 12;
            resetButton.Text = "Reset";
            resetButton.UseVisualStyleBackColor = true;
            resetButton.Click += ResetButton_Click;
            // 
            // Update_Label
            // 
            Update_Label.AutoSize = true;
            Update_Label.Font = new System.Drawing.Font("Tahoma", 10F);
            Update_Label.ForeColor = System.Drawing.Color.Black;
            Update_Label.Location = new System.Drawing.Point(165, 624);
            Update_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            Update_Label.Name = "Update_Label";
            Update_Label.Size = new System.Drawing.Size(135, 21);
            Update_Label.TabIndex = 15;
            Update_Label.Text = "Saving Setting ...";
            Update_Label.Visible = false;
            // 
            // SettingsPage
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.White;
            Controls.Add(Update_Label);
            Controls.Add(resetButton);
            Controls.Add(label1);
            Controls.Add(panel1);
            Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            Name = "SettingsPage";
            Size = new System.Drawing.Size(1342, 729);
            Load += SettingsPage_Load;
            ((System.ComponentModel.ISupportInitialize)useDefaultStrings_CheckBox).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)useDarkMode_Checkbox).EndInit();
            ((System.ComponentModel.ISupportInitialize)useUsermodeBlockRules_CheckBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)useDriverBlockRules_CheckBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)convertPolicyToBinary_CheckBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)useEnvVars_CheckBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label appVersion_Label;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label terms_Label;
        private System.Windows.Forms.Label privacy_Label;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox useDefaultStrings_CheckBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox useEnvVars_CheckBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Label Update_Label;
        private System.Windows.Forms.PictureBox convertPolicyToBinary_CheckBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox useUsermodeBlockRules_CheckBox;
        private System.Windows.Forms.PictureBox useDriverBlockRules_CheckBox;
        private System.Windows.Forms.Label label_DriverBlock;
        private System.Windows.Forms.Label label_UsermodeBlock;
        private System.Windows.Forms.PictureBox useDarkMode_Checkbox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
    }
}
