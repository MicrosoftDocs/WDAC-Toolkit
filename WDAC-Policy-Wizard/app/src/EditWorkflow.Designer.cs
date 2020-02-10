// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

namespace WDAC_Wizard
{
    partial class EditWorkflow
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
            this.label2 = new System.Windows.Forms.Label();
            this.button_Create = new System.Windows.Forms.Button();
            this.textBoxPolicyPath = new System.Windows.Forms.TextBox();
            this.policyInfoPanel = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_PolicyName = new System.Windows.Forms.TextBox();
            this.label_policyName = new System.Windows.Forms.Label();
            this.textBox_PolicyID = new System.Windows.Forms.TextBox();
            this.label_fileLocation = new System.Windows.Forms.Label();
            this.policyInfoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(175, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(286, 29);
            this.label1.TabIndex = 48;
            this.label1.Text = "Edit Exisiting WDAC Policy";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(176, 64);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(298, 21);
            this.label2.TabIndex = 108;
            this.label2.Text = "Browse for your existing policy on disk.";
            // 
            // button_Create
            // 
            this.button_Create.FlatAppearance.BorderColor = System.Drawing.SystemColors.MenuHighlight;
            this.button_Create.FlatAppearance.BorderSize = 2;
            this.button_Create.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_Create.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_Create.ForeColor = System.Drawing.Color.DodgerBlue;
            this.button_Create.Location = new System.Drawing.Point(709, 118);
            this.button_Create.Name = "button_Create";
            this.button_Create.Size = new System.Drawing.Size(126, 35);
            this.button_Create.TabIndex = 109;
            this.button_Create.Text = "Browse";
            this.button_Create.UseVisualStyleBackColor = true;
            this.button_Create.Click += new System.EventHandler(this.button_Create_Click);
            // 
            // textBoxPolicyPath
            // 
            this.textBoxPolicyPath.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBoxPolicyPath.Location = new System.Drawing.Point(183, 122);
            this.textBoxPolicyPath.Name = "textBoxPolicyPath";
            this.textBoxPolicyPath.Size = new System.Drawing.Size(493, 26);
            this.textBoxPolicyPath.TabIndex = 110;
            // 
            // policyInfoPanel
            // 
            this.policyInfoPanel.Controls.Add(this.label5);
            this.policyInfoPanel.Controls.Add(this.textBox_PolicyName);
            this.policyInfoPanel.Controls.Add(this.label_policyName);
            this.policyInfoPanel.Controls.Add(this.textBox_PolicyID);
            this.policyInfoPanel.Controls.Add(this.label_fileLocation);
            this.policyInfoPanel.Location = new System.Drawing.Point(179, 176);
            this.policyInfoPanel.Name = "policyInfoPanel";
            this.policyInfoPanel.Size = new System.Drawing.Size(675, 145);
            this.policyInfoPanel.TabIndex = 111;
            this.policyInfoPanel.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(14, 7);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(350, 21);
            this.label5.TabIndex = 11;
            this.label5.Text = "Edit the policy name and ID, if you would like.\r\n";
            // 
            // textBox_PolicyName
            // 
            this.textBox_PolicyName.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_PolicyName.Location = new System.Drawing.Point(122, 53);
            this.textBox_PolicyName.Name = "textBox_PolicyName";
            this.textBox_PolicyName.Size = new System.Drawing.Size(375, 26);
            this.textBox_PolicyName.TabIndex = 9;
            this.textBox_PolicyName.TextChanged += new System.EventHandler(this.textBox_PolicyName_TextChanged);
            // 
            // label_policyName
            // 
            this.label_policyName.AutoSize = true;
            this.label_policyName.Font = new System.Drawing.Font("Tahoma", 10F);
            this.label_policyName.ForeColor = System.Drawing.Color.Black;
            this.label_policyName.Location = new System.Drawing.Point(14, 52);
            this.label_policyName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_policyName.Name = "label_policyName";
            this.label_policyName.Size = new System.Drawing.Size(106, 21);
            this.label_policyName.TabIndex = 8;
            this.label_policyName.Text = "Policy Name:";
            // 
            // textBox_PolicyID
            // 
            this.textBox_PolicyID.Font = new System.Drawing.Font("Tahoma", 9F);
            this.textBox_PolicyID.Location = new System.Drawing.Point(122, 95);
            this.textBox_PolicyID.Name = "textBox_PolicyID";
            this.textBox_PolicyID.Size = new System.Drawing.Size(375, 26);
            this.textBox_PolicyID.TabIndex = 7;
            this.textBox_PolicyID.TextChanged += new System.EventHandler(this.textBox_PolicyID_TextChanged);
            // 
            // label_fileLocation
            // 
            this.label_fileLocation.AutoSize = true;
            this.label_fileLocation.Font = new System.Drawing.Font("Tahoma", 10F);
            this.label_fileLocation.ForeColor = System.Drawing.Color.Black;
            this.label_fileLocation.Location = new System.Drawing.Point(14, 96);
            this.label_fileLocation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label_fileLocation.Name = "label_fileLocation";
            this.label_fileLocation.Size = new System.Drawing.Size(81, 21);
            this.label_fileLocation.TabIndex = 6;
            this.label_fileLocation.Text = "Policy ID:";
            // 
            // EditWorkflow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.policyInfoPanel);
            this.Controls.Add(this.textBoxPolicyPath);
            this.Controls.Add(this.button_Create);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "EditWorkflow";
            this.Size = new System.Drawing.Size(1289, 680);
            this.policyInfoPanel.ResumeLayout(false);
            this.policyInfoPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button_Create;
        private System.Windows.Forms.TextBox textBoxPolicyPath;
        private System.Windows.Forms.Panel policyInfoPanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_PolicyName;
        private System.Windows.Forms.Label label_policyName;
        private System.Windows.Forms.TextBox textBox_PolicyID;
        private System.Windows.Forms.Label label_fileLocation;
    }
}
