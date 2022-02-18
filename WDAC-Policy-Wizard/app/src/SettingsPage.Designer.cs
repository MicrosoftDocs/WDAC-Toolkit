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
            this.label2 = new System.Windows.Forms.Label();
            this.appVersion_Label = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.terms_Label = new System.Windows.Forms.Label();
            this.privacy_Label = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.useDefaultStrings_CheckBox = new System.Windows.Forms.PictureBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_DriverBlock = new System.Windows.Forms.Label();
            this.label_UsermodeBlock = new System.Windows.Forms.Label();
            this.useUsermodeBlockRules_CheckBox = new System.Windows.Forms.PictureBox();
            this.useDriverBlockRules_CheckBox = new System.Windows.Forms.PictureBox();
            this.convertPolicyToBinary_CheckBox = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.allowTelemetry_CheckBox = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.useEnvVars_CheckBox = new System.Windows.Forms.PictureBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.resetButton = new System.Windows.Forms.Button();
            this.Update_Label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.useDefaultStrings_CheckBox)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.useUsermodeBlockRules_CheckBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.useDriverBlockRules_CheckBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.convertPolicyToBinary_CheckBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowTelemetry_CheckBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.useEnvVars_CheckBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 11F);
            this.label2.Location = new System.Drawing.Point(25, 344);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "About";
            // 
            // appVersion_Label
            // 
            this.appVersion_Label.AutoSize = true;
            this.appVersion_Label.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appVersion_Label.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.appVersion_Label.Location = new System.Drawing.Point(27, 379);
            this.appVersion_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.appVersion_Label.Name = "appVersion_Label";
            this.appVersion_Label.Size = new System.Drawing.Size(168, 21);
            this.appVersion_Label.TabIndex = 2;
            this.appVersion_Label.Text = "App Version 1.903.22";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.label3.Location = new System.Drawing.Point(27, 408);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(243, 42);
            this.label3.TabIndex = 3;
            this.label3.Text = "2022 Microsoft Corporation ©\r\nJordan.Geurten@microsoft.com";
            // 
            // terms_Label
            // 
            this.terms_Label.AutoSize = true;
            this.terms_Label.BackColor = System.Drawing.Color.White;
            this.terms_Label.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.terms_Label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.terms_Label.Image = global::WDAC_Wizard.Properties.Resources.external_link_symbol_highlight;
            this.terms_Label.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.terms_Label.Location = new System.Drawing.Point(25, 464);
            this.terms_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.terms_Label.Name = "terms_Label";
            this.terms_Label.Size = new System.Drawing.Size(129, 21);
            this.terms_Label.TabIndex = 4;
            this.terms_Label.Text = "Terms of Use    ";
            this.terms_Label.Click += new System.EventHandler(this.Terms_Label_Click);
            // 
            // privacy_Label
            // 
            this.privacy_Label.AutoSize = true;
            this.privacy_Label.BackColor = System.Drawing.Color.White;
            this.privacy_Label.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.privacy_Label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.privacy_Label.Image = global::WDAC_Wizard.Properties.Resources.external_link_symbol_highlight;
            this.privacy_Label.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.privacy_Label.Location = new System.Drawing.Point(25, 493);
            this.privacy_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.privacy_Label.Name = "privacy_Label";
            this.privacy_Label.Size = new System.Drawing.Size(164, 21);
            this.privacy_Label.TabIndex = 5;
            this.privacy_Label.Text = "Privacy Statement    ";
            this.privacy_Label.Click += new System.EventHandler(this.Privacy_Label_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 11F);
            this.label4.Location = new System.Drawing.Point(25, 210);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 23);
            this.label4.TabIndex = 6;
            this.label4.Text = "Defaults";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(70, 250);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(285, 21);
            this.label5.TabIndex = 7;
            this.label5.Text = "Autofill text fields with default values";
            // 
            // useDefaultStrings_CheckBox
            // 
            this.useDefaultStrings_CheckBox.BackgroundImage = global::WDAC_Wizard.Properties.Resources.check_box_checked;
            this.useDefaultStrings_CheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.useDefaultStrings_CheckBox.Location = new System.Drawing.Point(38, 247);
            this.useDefaultStrings_CheckBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.useDefaultStrings_CheckBox.Name = "useDefaultStrings_CheckBox";
            this.useDefaultStrings_CheckBox.Size = new System.Drawing.Size(23, 26);
            this.useDefaultStrings_CheckBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.useDefaultStrings_CheckBox.TabIndex = 8;
            this.useDefaultStrings_CheckBox.TabStop = false;
            this.useDefaultStrings_CheckBox.Tag = "Checked";
            this.useDefaultStrings_CheckBox.Click += new System.EventHandler(this.DefaultString_CheckBox_Click);
            this.useDefaultStrings_CheckBox.MouseLeave += new System.EventHandler(this.SettingCheckBox_Leave);
            this.useDefaultStrings_CheckBox.MouseHover += new System.EventHandler(this.SettingCheckBox_Hover);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(72, 52);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(0, 21);
            this.label6.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.label_DriverBlock);
            this.panel1.Controls.Add(this.label_UsermodeBlock);
            this.panel1.Controls.Add(this.useUsermodeBlockRules_CheckBox);
            this.panel1.Controls.Add(this.useDriverBlockRules_CheckBox);
            this.panel1.Controls.Add(this.convertPolicyToBinary_CheckBox);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.allowTelemetry_CheckBox);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.useEnvVars_CheckBox);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.useDefaultStrings_CheckBox);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.privacy_Label);
            this.panel1.Controls.Add(this.terms_Label);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.appVersion_Label);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(164, 93);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(713, 528);
            this.panel1.TabIndex = 12;
            // 
            // label_DriverBlock
            // 
            this.label_DriverBlock.AutoSize = true;
            this.label_DriverBlock.BackColor = System.Drawing.Color.White;
            this.label_DriverBlock.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_DriverBlock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.label_DriverBlock.Image = global::WDAC_Wizard.Properties.Resources.external_link_symbol_highlight;
            this.label_DriverBlock.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label_DriverBlock.Location = new System.Drawing.Point(72, 173);
            this.label_DriverBlock.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_DriverBlock.Name = "label_DriverBlock";
            this.label_DriverBlock.Size = new System.Drawing.Size(516, 21);
            this.label_DriverBlock.TabIndex = 25;
            this.label_DriverBlock.Text = "Create policies with Microsoft\'s Recommended Driver Block Rules    ";
            this.label_DriverBlock.Click += new System.EventHandler(this.LabelDriverBlock_Click);
            // 
            // label_UsermodeBlock
            // 
            this.label_UsermodeBlock.AutoSize = true;
            this.label_UsermodeBlock.BackColor = System.Drawing.Color.White;
            this.label_UsermodeBlock.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_UsermodeBlock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(110)))), ((int)(((byte)(190)))));
            this.label_UsermodeBlock.Image = global::WDAC_Wizard.Properties.Resources.external_link_symbol_highlight;
            this.label_UsermodeBlock.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label_UsermodeBlock.Location = new System.Drawing.Point(72, 133);
            this.label_UsermodeBlock.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_UsermodeBlock.Name = "label_UsermodeBlock";
            this.label_UsermodeBlock.Size = new System.Drawing.Size(466, 21);
            this.label_UsermodeBlock.TabIndex = 24;
            this.label_UsermodeBlock.Text = "Create policies with Microsoft\'s Recommended Block Rules    ";
            this.label_UsermodeBlock.Click += new System.EventHandler(this.Label_UsermodeBlock_Click);
            // 
            // useUsermodeBlockRules_CheckBox
            // 
            this.useUsermodeBlockRules_CheckBox.BackgroundImage = global::WDAC_Wizard.Properties.Resources.check_box_unchecked;
            this.useUsermodeBlockRules_CheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.useUsermodeBlockRules_CheckBox.Location = new System.Drawing.Point(39, 130);
            this.useUsermodeBlockRules_CheckBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.useUsermodeBlockRules_CheckBox.Name = "useUsermodeBlockRules_CheckBox";
            this.useUsermodeBlockRules_CheckBox.Size = new System.Drawing.Size(23, 26);
            this.useUsermodeBlockRules_CheckBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.useUsermodeBlockRules_CheckBox.TabIndex = 23;
            this.useUsermodeBlockRules_CheckBox.TabStop = false;
            this.useUsermodeBlockRules_CheckBox.Tag = "Unchecked";
            this.useUsermodeBlockRules_CheckBox.Click += new System.EventHandler(this.UsermodeRecList_checkBox_Click);
            // 
            // useDriverBlockRules_CheckBox
            // 
            this.useDriverBlockRules_CheckBox.BackgroundImage = global::WDAC_Wizard.Properties.Resources.check_box_unchecked;
            this.useDriverBlockRules_CheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.useDriverBlockRules_CheckBox.Location = new System.Drawing.Point(39, 170);
            this.useDriverBlockRules_CheckBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.useDriverBlockRules_CheckBox.Name = "useDriverBlockRules_CheckBox";
            this.useDriverBlockRules_CheckBox.Size = new System.Drawing.Size(23, 26);
            this.useDriverBlockRules_CheckBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.useDriverBlockRules_CheckBox.TabIndex = 21;
            this.useDriverBlockRules_CheckBox.TabStop = false;
            this.useDriverBlockRules_CheckBox.Tag = "Unchecked";
            this.useDriverBlockRules_CheckBox.Click += new System.EventHandler(this.KernelmodeRecList_checkBox_Click);
            // 
            // convertPolicyToBinary_CheckBox
            // 
            this.convertPolicyToBinary_CheckBox.BackgroundImage = global::WDAC_Wizard.Properties.Resources.check_box_unchecked;
            this.convertPolicyToBinary_CheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.convertPolicyToBinary_CheckBox.Location = new System.Drawing.Point(39, 90);
            this.convertPolicyToBinary_CheckBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.convertPolicyToBinary_CheckBox.Name = "convertPolicyToBinary_CheckBox";
            this.convertPolicyToBinary_CheckBox.Size = new System.Drawing.Size(23, 26);
            this.convertPolicyToBinary_CheckBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.convertPolicyToBinary_CheckBox.TabIndex = 19;
            this.convertPolicyToBinary_CheckBox.TabStop = false;
            this.convertPolicyToBinary_CheckBox.Tag = "Unchecked";
            this.convertPolicyToBinary_CheckBox.Click += new System.EventHandler(this.ConvertPolicy_CheckBox_Click);
            this.convertPolicyToBinary_CheckBox.MouseLeave += new System.EventHandler(this.SettingCheckBox_Leave);
            this.convertPolicyToBinary_CheckBox.MouseHover += new System.EventHandler(this.SettingCheckBox_Hover);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(72, 93);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(320, 21);
            this.label7.TabIndex = 18;
            this.label7.Text = "Convert policy to binary after xml creation";
            // 
            // allowTelemetry_CheckBox
            // 
            this.allowTelemetry_CheckBox.BackgroundImage = global::WDAC_Wizard.Properties.Resources.check_box_checked;
            this.allowTelemetry_CheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.allowTelemetry_CheckBox.Location = new System.Drawing.Point(38, 290);
            this.allowTelemetry_CheckBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.allowTelemetry_CheckBox.Name = "allowTelemetry_CheckBox";
            this.allowTelemetry_CheckBox.Size = new System.Drawing.Size(23, 26);
            this.allowTelemetry_CheckBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.allowTelemetry_CheckBox.TabIndex = 17;
            this.allowTelemetry_CheckBox.TabStop = false;
            this.allowTelemetry_CheckBox.Tag = "Checked";
            this.allowTelemetry_CheckBox.Click += new System.EventHandler(this.Telemetry_CheckBox_Click);
            this.allowTelemetry_CheckBox.MouseLeave += new System.EventHandler(this.SettingCheckBox_Leave);
            this.allowTelemetry_CheckBox.MouseHover += new System.EventHandler(this.SettingCheckBox_Hover);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(70, 293);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(197, 21);
            this.label10.TabIndex = 16;
            this.label10.Text = "Share logs with Microsoft";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 11F);
            this.label9.Location = new System.Drawing.Point(27, 15);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 23);
            this.label9.TabIndex = 15;
            this.label9.Text = "Policy";
            // 
            // useEnvVars_CheckBox
            // 
            this.useEnvVars_CheckBox.BackgroundImage = global::WDAC_Wizard.Properties.Resources.check_box_checked;
            this.useEnvVars_CheckBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.useEnvVars_CheckBox.Location = new System.Drawing.Point(39, 50);
            this.useEnvVars_CheckBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.useEnvVars_CheckBox.Name = "useEnvVars_CheckBox";
            this.useEnvVars_CheckBox.Size = new System.Drawing.Size(23, 26);
            this.useEnvVars_CheckBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.useEnvVars_CheckBox.TabIndex = 14;
            this.useEnvVars_CheckBox.TabStop = false;
            this.useEnvVars_CheckBox.Tag = "Checked";
            this.useEnvVars_CheckBox.Click += new System.EventHandler(this.EnvVar_CheckBox_Click);
            this.useEnvVars_CheckBox.MouseLeave += new System.EventHandler(this.SettingCheckBox_Leave);
            this.useEnvVars_CheckBox.MouseHover += new System.EventHandler(this.SettingCheckBox_Hover);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(72, 53);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(441, 21);
            this.label8.TabIndex = 13;
            this.label8.Text = "Use environment variables in filepath rules where possible";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(164, 43);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 29);
            this.label1.TabIndex = 13;
            this.label1.Text = "Application Settings";
            // 
            // resetButton
            // 
            this.resetButton.BackColor = System.Drawing.Color.WhiteSmoke;
            this.resetButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.resetButton.Location = new System.Drawing.Point(785, 624);
            this.resetButton.Margin = new System.Windows.Forms.Padding(2);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(92, 32);
            this.resetButton.TabIndex = 14;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = false;
            this.resetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // Update_Label
            // 
            this.Update_Label.AutoSize = true;
            this.Update_Label.Font = new System.Drawing.Font("Tahoma", 10F);
            this.Update_Label.ForeColor = System.Drawing.Color.Black;
            this.Update_Label.Location = new System.Drawing.Point(165, 624);
            this.Update_Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Update_Label.Name = "Update_Label";
            this.Update_Label.Size = new System.Drawing.Size(135, 21);
            this.Update_Label.TabIndex = 15;
            this.Update_Label.Text = "Saving Setting ...";
            this.Update_Label.Visible = false;
            // 
            // SettingsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.Update_Label);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "SettingsPage";
            this.Size = new System.Drawing.Size(1342, 729);
            this.Load += new System.EventHandler(this.SettingsPage_Load);
            ((System.ComponentModel.ISupportInitialize)(this.useDefaultStrings_CheckBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.useUsermodeBlockRules_CheckBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.useDriverBlockRules_CheckBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.convertPolicyToBinary_CheckBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.allowTelemetry_CheckBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.useEnvVars_CheckBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.PictureBox allowTelemetry_CheckBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.Label Update_Label;
        private System.Windows.Forms.PictureBox convertPolicyToBinary_CheckBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.PictureBox useUsermodeBlockRules_CheckBox;
        private System.Windows.Forms.PictureBox useDriverBlockRules_CheckBox;
        private System.Windows.Forms.Label label_DriverBlock;
        private System.Windows.Forms.Label label_UsermodeBlock;
    }
}
