// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

namespace WDAC_Wizard
{
    partial class MainWindow
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.label_Welcome = new System.Windows.Forms.Label();
            this.button_New = new System.Windows.Forms.Button();
            this.label_Info = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.button_Edit = new System.Windows.Forms.Button();
            this.button_Merge = new System.Windows.Forms.Button();
            this.control_Panel = new System.Windows.Forms.Panel();
            this.workflow_Label = new System.Windows.Forms.Label();
            this.page5_Button = new System.Windows.Forms.Button();
            this.page4_Button = new System.Windows.Forms.Button();
            this.page3_Button = new System.Windows.Forms.Button();
            this.page2_Button = new System.Windows.Forms.Button();
            this.page1_Button = new System.Windows.Forms.Button();
            this.controlHighlight_Panel = new System.Windows.Forms.Panel();
            this.home_Button = new System.Windows.Forms.Button();
            this.settings_Button = new System.Windows.Forms.Button();
            this.button_Next = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.control_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label_Welcome
            // 
            this.label_Welcome.AutoSize = true;
            this.label_Welcome.BackColor = System.Drawing.Color.Transparent;
            this.label_Welcome.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Welcome.ForeColor = System.Drawing.Color.Black;
            this.label_Welcome.Location = new System.Drawing.Point(185, 42);
            this.label_Welcome.Name = "label_Welcome";
            this.label_Welcome.Size = new System.Drawing.Size(115, 30);
            this.label_Welcome.TabIndex = 3;
            this.label_Welcome.Text = "Welcome";
            // 
            // button_New
            // 
            this.button_New.BackColor = System.Drawing.Color.Transparent;
            this.button_New.FlatAppearance.BorderSize = 0;
            this.button_New.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.button_New.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.button_New.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_New.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_New.Image = Properties.Resources.newPolicy;
            this.button_New.Location = new System.Drawing.Point(328, 225);
            this.button_New.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_New.Name = "button_New";
            this.button_New.Size = new System.Drawing.Size(195, 217);
            this.button_New.TabIndex = 10;
            this.button_New.Text = "Policy Creator";
            this.button_New.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button_New.UseVisualStyleBackColor = false;
            this.button_New.Click += new System.EventHandler(this.Button_New_Click);
            // 
            // label_Info
            // 
            this.label_Info.AutoSize = true;
            this.label_Info.BackColor = System.Drawing.Color.Transparent;
            this.label_Info.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Info.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label_Info.Location = new System.Drawing.Point(168, 674);
            this.label_Info.Name = "label_Info";
            this.label_Info.Size = new System.Drawing.Size(71, 18);
            this.label_Info.TabIndex = 9;
            this.label_Info.Tag = "IgnoreDarkMode";
            this.label_Info.Text = "Info Text";
            this.label_Info.Visible = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // button_Edit
            // 
            this.button_Edit.BackColor = System.Drawing.Color.Transparent;
            this.button_Edit.FlatAppearance.BorderSize = 0;
            this.button_Edit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.button_Edit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.button_Edit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Edit.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Edit.Image = global::WDAC_Wizard.Properties.Resources.tools;
            this.button_Edit.Location = new System.Drawing.Point(567, 225);
            this.button_Edit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_Edit.Name = "button_Edit";
            this.button_Edit.Size = new System.Drawing.Size(195, 217);
            this.button_Edit.TabIndex = 25;
            this.button_Edit.Text = "Policy Editor";
            this.button_Edit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button_Edit.UseVisualStyleBackColor = false;
            this.button_Edit.Click += new System.EventHandler(this.Button_Edit_Click);
            // 
            // button_Merge
            // 
            this.button_Merge.BackColor = System.Drawing.Color.Transparent;
            this.button_Merge.FlatAppearance.BorderSize = 0;
            this.button_Merge.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.button_Merge.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.button_Merge.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Merge.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Merge.Image = global::WDAC_Wizard.Properties.Resources.merge;
            this.button_Merge.Location = new System.Drawing.Point(813, 225);
            this.button_Merge.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_Merge.Name = "button_Merge";
            this.button_Merge.Size = new System.Drawing.Size(195, 217);
            this.button_Merge.TabIndex = 26;
            this.button_Merge.Text = "Policy Merger";
            this.button_Merge.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button_Merge.UseVisualStyleBackColor = false;
            this.button_Merge.Click += new System.EventHandler(this.Button_Merge_Click);
            // 
            // control_Panel
            // 
            this.control_Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.control_Panel.Controls.Add(this.workflow_Label);
            this.control_Panel.Controls.Add(this.page5_Button);
            this.control_Panel.Controls.Add(this.page4_Button);
            this.control_Panel.Controls.Add(this.page3_Button);
            this.control_Panel.Controls.Add(this.page2_Button);
            this.control_Panel.Controls.Add(this.page1_Button);
            this.control_Panel.Controls.Add(this.controlHighlight_Panel);
            this.control_Panel.Controls.Add(this.home_Button);
            this.control_Panel.Controls.Add(this.settings_Button);
            this.control_Panel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.control_Panel.Location = new System.Drawing.Point(0, 0);
            this.control_Panel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.control_Panel.Name = "control_Panel";
            this.control_Panel.Size = new System.Drawing.Size(150, 700);
            this.control_Panel.TabIndex = 30;
            // 
            // workflow_Label
            // 
            this.workflow_Label.AutoSize = true;
            this.workflow_Label.BackColor = System.Drawing.Color.Transparent;
            this.workflow_Label.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.workflow_Label.Location = new System.Drawing.Point(12, 162);
            this.workflow_Label.Name = "workflow_Label";
            this.workflow_Label.Size = new System.Drawing.Size(79, 21);
            this.workflow_Label.TabIndex = 40;
            this.workflow_Label.Text = "Workflow";
            this.workflow_Label.Visible = false;
            // 
            // page5_Button
            // 
            this.page5_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.page5_Button.BackColor = System.Drawing.Color.Transparent;
            this.page5_Button.Enabled = false;
            this.page5_Button.FlatAppearance.BorderSize = 0;
            this.page5_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.page5_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.page5_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.page5_Button.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.page5_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page5_Button.Location = new System.Drawing.Point(15, 400);
            this.page5_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.page5_Button.Name = "page5_Button";
            this.page5_Button.Size = new System.Drawing.Size(146, 45);
            this.page5_Button.TabIndex = 39;
            this.page5_Button.Text = "Page 5";
            this.page5_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page5_Button.UseVisualStyleBackColor = false;
            this.page5_Button.Visible = false;
            this.page5_Button.Click += new System.EventHandler(this.PageNButton_Click);
            // 
            // page4_Button
            // 
            this.page4_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.page4_Button.BackColor = System.Drawing.Color.Transparent;
            this.page4_Button.Enabled = false;
            this.page4_Button.FlatAppearance.BorderSize = 0;
            this.page4_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.page4_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.page4_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.page4_Button.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.page4_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page4_Button.Location = new System.Drawing.Point(15, 350);
            this.page4_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.page4_Button.Name = "page4_Button";
            this.page4_Button.Size = new System.Drawing.Size(146, 45);
            this.page4_Button.TabIndex = 38;
            this.page4_Button.Text = "Page 4";
            this.page4_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page4_Button.UseVisualStyleBackColor = false;
            this.page4_Button.Visible = false;
            this.page4_Button.Click += new System.EventHandler(this.PageNButton_Click);
            // 
            // page3_Button
            // 
            this.page3_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.page3_Button.BackColor = System.Drawing.Color.Transparent;
            this.page3_Button.Enabled = false;
            this.page3_Button.FlatAppearance.BorderSize = 0;
            this.page3_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.page3_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.page3_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.page3_Button.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.page3_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page3_Button.Location = new System.Drawing.Point(15, 300);
            this.page3_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.page3_Button.Name = "page3_Button";
            this.page3_Button.Size = new System.Drawing.Size(146, 45);
            this.page3_Button.TabIndex = 37;
            this.page3_Button.Text = "Page 3";
            this.page3_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page3_Button.UseVisualStyleBackColor = false;
            this.page3_Button.Visible = false;
            this.page3_Button.Click += new System.EventHandler(this.PageNButton_Click);
            // 
            // page2_Button
            // 
            this.page2_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.page2_Button.BackColor = System.Drawing.Color.Transparent;
            this.page2_Button.Enabled = false;
            this.page2_Button.FlatAppearance.BorderSize = 0;
            this.page2_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.page2_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.page2_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.page2_Button.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.page2_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page2_Button.Location = new System.Drawing.Point(15, 250);
            this.page2_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.page2_Button.Name = "page2_Button";
            this.page2_Button.Size = new System.Drawing.Size(146, 45);
            this.page2_Button.TabIndex = 36;
            this.page2_Button.Text = "Page 2";
            this.page2_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page2_Button.UseVisualStyleBackColor = false;
            this.page2_Button.Visible = false;
            this.page2_Button.Click += new System.EventHandler(this.PageNButton_Click);
            // 
            // page1_Button
            // 
            this.page1_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.page1_Button.BackColor = System.Drawing.Color.Transparent;
            this.page1_Button.Enabled = false;
            this.page1_Button.FlatAppearance.BorderSize = 0;
            this.page1_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.page1_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.page1_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.page1_Button.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.page1_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page1_Button.Location = new System.Drawing.Point(15, 200);
            this.page1_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.page1_Button.Name = "page1_Button";
            this.page1_Button.Size = new System.Drawing.Size(146, 45);
            this.page1_Button.TabIndex = 35;
            this.page1_Button.Text = "Page 1";
            this.page1_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page1_Button.UseVisualStyleBackColor = false;
            this.page1_Button.Visible = false;
            this.page1_Button.Click += new System.EventHandler(this.PageNButton_Click);
            // 
            // controlHighlight_Panel
            // 
            this.controlHighlight_Panel.BackColor = System.Drawing.Color.CornflowerBlue;
            this.controlHighlight_Panel.Location = new System.Drawing.Point(0, 96);
            this.controlHighlight_Panel.Name = "controlHighlight_Panel";
            this.controlHighlight_Panel.Size = new System.Drawing.Size(8, 35);
            this.controlHighlight_Panel.TabIndex = 33;
            this.controlHighlight_Panel.Tag = "IgnoreDarkMode";
            // 
            // home_Button
            // 
            this.home_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.home_Button.BackColor = System.Drawing.Color.Transparent;
            this.home_Button.FlatAppearance.BorderSize = 0;
            this.home_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.home_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.home_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.home_Button.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.home_Button.Image = ((System.Drawing.Image)(resources.GetObject("home_Button.Image")));
            this.home_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.home_Button.Location = new System.Drawing.Point(15, 92);
            this.home_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.home_Button.Name = "home_Button";
            this.home_Button.Size = new System.Drawing.Size(129, 45);
            this.home_Button.TabIndex = 32;
            this.home_Button.Text = "     Home";
            this.home_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.home_Button.UseVisualStyleBackColor = false;
            this.home_Button.Click += new System.EventHandler(this.Home_Button_Click);
            // 
            // settings_Button
            // 
            this.settings_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.settings_Button.BackColor = System.Drawing.Color.Transparent;
            this.settings_Button.FlatAppearance.BorderSize = 0;
            this.settings_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.settings_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
            this.settings_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settings_Button.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settings_Button.Image = global::WDAC_Wizard.Properties.Resources.gear;
            this.settings_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settings_Button.Location = new System.Drawing.Point(15, 647);
            this.settings_Button.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.settings_Button.Name = "settings_Button";
            this.settings_Button.Size = new System.Drawing.Size(129, 45);
            this.settings_Button.TabIndex = 31;
            this.settings_Button.Text = "     Settings";
            this.settings_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settings_Button.UseVisualStyleBackColor = false;
            this.settings_Button.Click += new System.EventHandler(this.Settings_Button_Click);
            // 
            // button_Next
            // 
            this.button_Next.Location = new System.Drawing.Point(1134, 663);
            this.button_Next.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button_Next.Name = "button_Next";
            this.button_Next.Size = new System.Drawing.Size(93, 33);
            this.button_Next.TabIndex = 31;
            this.button_Next.Text = "Next";
            this.button_Next.UseVisualStyleBackColor = true;
            this.button_Next.Visible = false;
            this.button_Next.Click += new System.EventHandler(this.Button_Next_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Tahoma", 11.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(185, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(311, 24);
            this.label1.TabIndex = 32;
            this.label1.Text = "Select a task below to get started";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label2.Location = new System.Drawing.Point(296, 453);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(244, 36);
            this.label2.TabIndex = 33;
            this.label2.Text = "Create a new base or supplemental \r\npolicy";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label3.Location = new System.Drawing.Point(563, 453);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(218, 36);
            this.label3.TabIndex = 34;
            this.label3.Text = "Edit an existing policy on disk or \r\nconvert event logs to a policy";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label4.Location = new System.Drawing.Point(802, 453);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(236, 18);
            this.label4.TabIndex = 35;
            this.label4.Text = "Merge two existing policies into one\r\n";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1232, 703);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Next);
            this.Controls.Add(this.control_Panel);
            this.Controls.Add(this.label_Welcome);
            this.Controls.Add(this.button_Merge);
            this.Controls.Add(this.button_Edit);
            this.Controls.Add(this.label_Info);
            this.Controls.Add(this.button_New);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Windows Defender App Control Policy Wizard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClosing_Event);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.control_Panel.ResumeLayout(false);
            this.control_Panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label_Welcome;
        private System.Windows.Forms.Label label_Info;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private EditWorkflow editWorkflow1;
        private System.Windows.Forms.Button button_New;
        private System.Windows.Forms.Button button_Edit;
        private System.Windows.Forms.Button button_Merge;
        private System.Windows.Forms.Panel control_Panel;
        private System.Windows.Forms.Button settings_Button;
        private System.Windows.Forms.Button home_Button;
        private System.Windows.Forms.Button button_Next;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel controlHighlight_Panel;
        private System.Windows.Forms.Button page3_Button;
        private System.Windows.Forms.Button page2_Button;
        private System.Windows.Forms.Button page4_Button;
        public System.Windows.Forms.Button page1_Button;
        private System.Windows.Forms.Button page5_Button;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label workflow_Label;
    }
}

