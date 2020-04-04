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
            this.page5_Button = new System.Windows.Forms.Button();
            this.page4_Button = new System.Windows.Forms.Button();
            this.page3_Button = new System.Windows.Forms.Button();
            this.page2_Button = new System.Windows.Forms.Button();
            this.page1_Button = new System.Windows.Forms.Button();
            this.workflow_Button = new System.Windows.Forms.Button();
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
            this.label_Welcome.Font = new System.Drawing.Font("Tahoma", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Welcome.ForeColor = System.Drawing.Color.Black;
            this.label_Welcome.Location = new System.Drawing.Point(222, 50);
            this.label_Welcome.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Welcome.Name = "label_Welcome";
            this.label_Welcome.Size = new System.Drawing.Size(136, 36);
            this.label_Welcome.TabIndex = 3;
            this.label_Welcome.Text = "Welcome";
            // 
            // button_New
            // 
            this.button_New.FlatAppearance.BorderSize = 0;
            this.button_New.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(230)))), ((int)(((byte)(253)))));
            this.button_New.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(230)))), ((int)(((byte)(253)))));
            this.button_New.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_New.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_New.Image = global::WDAC_Wizard.Properties.Resources.newPolicy;
            this.button_New.Location = new System.Drawing.Point(394, 270);
            this.button_New.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.button_New.Name = "button_New";
            this.button_New.Size = new System.Drawing.Size(234, 260);
            this.button_New.TabIndex = 10;
            this.button_New.Text = "Policy Creator";
            this.button_New.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button_New.UseVisualStyleBackColor = true;
            this.button_New.Click += new System.EventHandler(this.button_New_Click);
            // 
            // label_Info
            // 
            this.label_Info.AutoSize = true;
            this.label_Info.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Info.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.label_Info.Location = new System.Drawing.Point(202, 809);
            this.label_Info.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_Info.Name = "label_Info";
            this.label_Info.Size = new System.Drawing.Size(97, 25);
            this.label_Info.TabIndex = 9;
            this.label_Info.Text = "Info Text";
            this.label_Info.Visible = false;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // button_Edit
            // 
            this.button_Edit.FlatAppearance.BorderSize = 0;
            this.button_Edit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(230)))), ((int)(((byte)(253)))));
            this.button_Edit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(230)))), ((int)(((byte)(253)))));
            this.button_Edit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Edit.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Edit.Image = global::WDAC_Wizard.Properties.Resources.tools;
            this.button_Edit.Location = new System.Drawing.Point(680, 270);
            this.button_Edit.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.button_Edit.Name = "button_Edit";
            this.button_Edit.Size = new System.Drawing.Size(234, 260);
            this.button_Edit.TabIndex = 25;
            this.button_Edit.Text = "Policy Editor";
            this.button_Edit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button_Edit.UseVisualStyleBackColor = true;
            this.button_Edit.Click += new System.EventHandler(this.button_Edit_Click);
            // 
            // button_Merge
            // 
            this.button_Merge.FlatAppearance.BorderSize = 0;
            this.button_Merge.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(230)))), ((int)(((byte)(253)))));
            this.button_Merge.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(190)))), ((int)(((byte)(230)))), ((int)(((byte)(253)))));
            this.button_Merge.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Merge.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Merge.Image = global::WDAC_Wizard.Properties.Resources.merge;
            this.button_Merge.Location = new System.Drawing.Point(976, 270);
            this.button_Merge.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.button_Merge.Name = "button_Merge";
            this.button_Merge.Size = new System.Drawing.Size(234, 260);
            this.button_Merge.TabIndex = 26;
            this.button_Merge.Text = "Policy Merger";
            this.button_Merge.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.button_Merge.UseVisualStyleBackColor = true;
            this.button_Merge.Click += new System.EventHandler(this.button_Merge_Click);
            // 
            // control_Panel
            // 
            this.control_Panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.control_Panel.Controls.Add(this.page5_Button);
            this.control_Panel.Controls.Add(this.page4_Button);
            this.control_Panel.Controls.Add(this.page3_Button);
            this.control_Panel.Controls.Add(this.page2_Button);
            this.control_Panel.Controls.Add(this.page1_Button);
            this.control_Panel.Controls.Add(this.workflow_Button);
            this.control_Panel.Controls.Add(this.controlHighlight_Panel);
            this.control_Panel.Controls.Add(this.home_Button);
            this.control_Panel.Controls.Add(this.settings_Button);
            this.control_Panel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.control_Panel.Location = new System.Drawing.Point(0, 0);
            this.control_Panel.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.control_Panel.Name = "control_Panel";
            this.control_Panel.Size = new System.Drawing.Size(180, 840);
            this.control_Panel.TabIndex = 30;
            // 
            // page5_Button
            // 
            this.page5_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.page5_Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.page5_Button.Enabled = false;
            this.page5_Button.FlatAppearance.BorderSize = 0;
            this.page5_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page5_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page5_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.page5_Button.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.page5_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page5_Button.Location = new System.Drawing.Point(18, 480);
            this.page5_Button.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.page5_Button.Name = "page5_Button";
            this.page5_Button.Size = new System.Drawing.Size(175, 54);
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
            this.page4_Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.page4_Button.Enabled = false;
            this.page4_Button.FlatAppearance.BorderSize = 0;
            this.page4_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page4_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page4_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.page4_Button.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.page4_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page4_Button.Location = new System.Drawing.Point(18, 420);
            this.page4_Button.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.page4_Button.Name = "page4_Button";
            this.page4_Button.Size = new System.Drawing.Size(175, 54);
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
            this.page3_Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.page3_Button.Enabled = false;
            this.page3_Button.FlatAppearance.BorderSize = 0;
            this.page3_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page3_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page3_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.page3_Button.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.page3_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page3_Button.Location = new System.Drawing.Point(18, 360);
            this.page3_Button.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.page3_Button.Name = "page3_Button";
            this.page3_Button.Size = new System.Drawing.Size(175, 54);
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
            this.page2_Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.page2_Button.Enabled = false;
            this.page2_Button.FlatAppearance.BorderSize = 0;
            this.page2_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page2_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page2_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.page2_Button.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.page2_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page2_Button.Location = new System.Drawing.Point(18, 300);
            this.page2_Button.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.page2_Button.Name = "page2_Button";
            this.page2_Button.Size = new System.Drawing.Size(175, 54);
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
            this.page1_Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.page1_Button.Enabled = false;
            this.page1_Button.FlatAppearance.BorderSize = 0;
            this.page1_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page1_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.page1_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.page1_Button.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.page1_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page1_Button.Location = new System.Drawing.Point(18, 240);
            this.page1_Button.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.page1_Button.Name = "page1_Button";
            this.page1_Button.Size = new System.Drawing.Size(175, 54);
            this.page1_Button.TabIndex = 35;
            this.page1_Button.Text = "Page 1";
            this.page1_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.page1_Button.UseVisualStyleBackColor = false;
            this.page1_Button.Visible = false;
            this.page1_Button.Click += new System.EventHandler(this.PageNButton_Click);
            // 
            // workflow_Button
            // 
            this.workflow_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.workflow_Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.workflow_Button.FlatAppearance.BorderSize = 0;
            this.workflow_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.workflow_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.workflow_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.workflow_Button.Font = new System.Drawing.Font("Tahoma", 10F);
            this.workflow_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.workflow_Button.Location = new System.Drawing.Point(6, 180);
            this.workflow_Button.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.workflow_Button.Name = "workflow_Button";
            this.workflow_Button.Size = new System.Drawing.Size(156, 54);
            this.workflow_Button.TabIndex = 34;
            this.workflow_Button.Text = "Workflow";
            this.workflow_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.workflow_Button.UseVisualStyleBackColor = false;
            this.workflow_Button.Visible = false;
            // 
            // controlHighlight_Panel
            // 
            this.controlHighlight_Panel.BackColor = System.Drawing.Color.CornflowerBlue;
            this.controlHighlight_Panel.Location = new System.Drawing.Point(0, 115);
            this.controlHighlight_Panel.Margin = new System.Windows.Forms.Padding(4);
            this.controlHighlight_Panel.Name = "controlHighlight_Panel";
            this.controlHighlight_Panel.Size = new System.Drawing.Size(10, 42);
            this.controlHighlight_Panel.TabIndex = 33;
            // 
            // home_Button
            // 
            this.home_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.home_Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.home_Button.FlatAppearance.BorderSize = 0;
            this.home_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.home_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.home_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.home_Button.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.home_Button.Image = global::WDAC_Wizard.Properties.Resources.house;
            this.home_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.home_Button.Location = new System.Drawing.Point(18, 110);
            this.home_Button.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.home_Button.Name = "home_Button";
            this.home_Button.Size = new System.Drawing.Size(155, 54);
            this.home_Button.TabIndex = 32;
            this.home_Button.Text = "     Home";
            this.home_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.home_Button.UseVisualStyleBackColor = false;
            this.home_Button.Click += new System.EventHandler(this.home_Button_Click);
            // 
            // settings_Button
            // 
            this.settings_Button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.settings_Button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            this.settings_Button.FlatAppearance.BorderSize = 0;
            this.settings_Button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.settings_Button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(149)))));
            this.settings_Button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settings_Button.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settings_Button.Image = global::WDAC_Wizard.Properties.Resources.gear;
            this.settings_Button.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settings_Button.Location = new System.Drawing.Point(18, 776);
            this.settings_Button.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.settings_Button.Name = "settings_Button";
            this.settings_Button.Size = new System.Drawing.Size(155, 54);
            this.settings_Button.TabIndex = 31;
            this.settings_Button.Text = "     Settings";
            this.settings_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.settings_Button.UseVisualStyleBackColor = false;
            this.settings_Button.Click += new System.EventHandler(this.settings_Button_Click);
            // 
            // button_Next
            // 
            this.button_Next.Location = new System.Drawing.Point(1361, 796);
            this.button_Next.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.button_Next.Name = "button_Next";
            this.button_Next.Size = new System.Drawing.Size(112, 40);
            this.button_Next.TabIndex = 31;
            this.button_Next.Text = "Next";
            this.button_Next.UseVisualStyleBackColor = true;
            this.button_Next.Visible = false;
            this.button_Next.Click += new System.EventHandler(this.button_Next_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(222, 98);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(373, 29);
            this.label1.TabIndex = 32;
            this.label1.Text = "Select a task below to get started";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label2.Location = new System.Drawing.Point(355, 544);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(293, 44);
            this.label2.TabIndex = 33;
            this.label2.Text = "Create a new base or supplemental \r\npolicy";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label3.Location = new System.Drawing.Point(676, 544);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(245, 22);
            this.label3.TabIndex = 34;
            this.label3.Text = "Edit an existing policy on disk";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F);
            this.label4.Location = new System.Drawing.Point(962, 544);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(293, 22);
            this.label4.TabIndex = 35;
            this.label4.Text = "Merge two existing policies into one\r\n";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1478, 844);
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
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Windows Defender App Control Policy Wizard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClosing_Event);
            this.control_Panel.ResumeLayout(false);
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
        private System.Windows.Forms.Button workflow_Button;
        private System.Windows.Forms.Button page3_Button;
        private System.Windows.Forms.Button page2_Button;
        private System.Windows.Forms.Button page4_Button;
        public System.Windows.Forms.Button page1_Button;
        private System.Windows.Forms.Button page5_Button;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

