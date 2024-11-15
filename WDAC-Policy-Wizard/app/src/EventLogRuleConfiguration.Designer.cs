﻿
namespace WDAC_Wizard
{
    partial class EventLogRuleConfiguration
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
            addButton = new System.Windows.Forms.Button();
            publisherRulePanel = new System.Windows.Forms.Panel();
            productTextBox = new System.Windows.Forms.TextBox();
            versionTextBox = new System.Windows.Forms.TextBox();
            filenameTextBox = new System.Windows.Forms.TextBox();
            publisherTextBox = new System.Windows.Forms.TextBox();
            issuerTextBox = new System.Windows.Forms.TextBox();
            productCheckBox = new System.Windows.Forms.CheckBox();
            versionCheckBox = new System.Windows.Forms.CheckBox();
            filenameCheckBox = new System.Windows.Forms.CheckBox();
            publisherCheckBox = new System.Windows.Forms.CheckBox();
            issuerCheckBox = new System.Windows.Forms.CheckBox();
            label3 = new System.Windows.Forms.Label();
            ruleTypeComboBox = new System.Windows.Forms.ComboBox();
            fileAttributeRulePanel = new System.Windows.Forms.Panel();
            pfnTextBox = new System.Windows.Forms.TextBox();
            intFileNameTextBox = new System.Windows.Forms.TextBox();
            prodNameTextBox = new System.Windows.Forms.TextBox();
            fileDescTextBox = new System.Windows.Forms.TextBox();
            origFileNameTextBox = new System.Windows.Forms.TextBox();
            pfnCheckBox = new System.Windows.Forms.CheckBox();
            intFileNameCheckBox = new System.Windows.Forms.CheckBox();
            prodNameCheckBox = new System.Windows.Forms.CheckBox();
            fileDescCheckBox = new System.Windows.Forms.CheckBox();
            origFileNameCheckBox = new System.Windows.Forms.CheckBox();
            eventsDataGridView = new System.Windows.Forms.DataGridView();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            hashRulePanel = new System.Windows.Forms.Panel();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            sha2TextBox = new System.Windows.Forms.TextBox();
            sha1TextBox = new System.Windows.Forms.TextBox();
            filePathRulePanel = new System.Windows.Forms.Panel();
            filePathTextBox = new System.Windows.Forms.TextBox();
            folderPathTextBox = new System.Windows.Forms.TextBox();
            folderPathCheckBox = new System.Windows.Forms.CheckBox();
            filePathCheckBox = new System.Windows.Forms.CheckBox();
            addedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            eventIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            filenameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            productColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            publisherColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            policyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            publisherRulePanel.SuspendLayout();
            fileAttributeRulePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)eventsDataGridView).BeginInit();
            hashRulePanel.SuspendLayout();
            filePathRulePanel.SuspendLayout();
            SuspendLayout();
            // 
            // addButton
            // 
            addButton.Location = new System.Drawing.Point(530, 494);
            addButton.Margin = new System.Windows.Forms.Padding(4);
            addButton.Name = "addButton";
            addButton.Size = new System.Drawing.Size(156, 36);
            addButton.TabIndex = 0;
            addButton.Text = "+ Add Allow Rule";
            addButton.UseVisualStyleBackColor = true;
            addButton.Click += AddButton_Click;
            // 
            // publisherRulePanel
            // 
            publisherRulePanel.Controls.Add(productTextBox);
            publisherRulePanel.Controls.Add(versionTextBox);
            publisherRulePanel.Controls.Add(filenameTextBox);
            publisherRulePanel.Controls.Add(publisherTextBox);
            publisherRulePanel.Controls.Add(issuerTextBox);
            publisherRulePanel.Controls.Add(productCheckBox);
            publisherRulePanel.Controls.Add(versionCheckBox);
            publisherRulePanel.Controls.Add(filenameCheckBox);
            publisherRulePanel.Controls.Add(publisherCheckBox);
            publisherRulePanel.Controls.Add(issuerCheckBox);
            publisherRulePanel.Location = new System.Drawing.Point(200, 536);
            publisherRulePanel.Margin = new System.Windows.Forms.Padding(4);
            publisherRulePanel.Name = "publisherRulePanel";
            publisherRulePanel.Size = new System.Drawing.Size(859, 228);
            publisherRulePanel.TabIndex = 9;
            // 
            // productTextBox
            // 
            productTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            productTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            productTextBox.Location = new System.Drawing.Point(131, 188);
            productTextBox.Margin = new System.Windows.Forms.Padding(4);
            productTextBox.Name = "productTextBox";
            productTextBox.ReadOnly = true;
            productTextBox.Size = new System.Drawing.Size(504, 32);
            productTextBox.TabIndex = 20;
            // 
            // versionTextBox
            // 
            versionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            versionTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            versionTextBox.Location = new System.Drawing.Point(131, 142);
            versionTextBox.Margin = new System.Windows.Forms.Padding(4);
            versionTextBox.Name = "versionTextBox";
            versionTextBox.ReadOnly = true;
            versionTextBox.Size = new System.Drawing.Size(504, 32);
            versionTextBox.TabIndex = 19;
            // 
            // filenameTextBox
            // 
            filenameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            filenameTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            filenameTextBox.Location = new System.Drawing.Point(131, 95);
            filenameTextBox.Margin = new System.Windows.Forms.Padding(4);
            filenameTextBox.Name = "filenameTextBox";
            filenameTextBox.ReadOnly = true;
            filenameTextBox.Size = new System.Drawing.Size(504, 32);
            filenameTextBox.TabIndex = 18;
            // 
            // publisherTextBox
            // 
            publisherTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            publisherTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            publisherTextBox.Location = new System.Drawing.Point(131, 48);
            publisherTextBox.Margin = new System.Windows.Forms.Padding(4);
            publisherTextBox.Name = "publisherTextBox";
            publisherTextBox.ReadOnly = true;
            publisherTextBox.Size = new System.Drawing.Size(504, 32);
            publisherTextBox.TabIndex = 17;
            // 
            // issuerTextBox
            // 
            issuerTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            issuerTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            issuerTextBox.Location = new System.Drawing.Point(131, 1);
            issuerTextBox.Margin = new System.Windows.Forms.Padding(4);
            issuerTextBox.Name = "issuerTextBox";
            issuerTextBox.ReadOnly = true;
            issuerTextBox.Size = new System.Drawing.Size(504, 32);
            issuerTextBox.TabIndex = 16;
            // 
            // productCheckBox
            // 
            productCheckBox.AutoCheck = false;
            productCheckBox.AutoSize = true;
            productCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            productCheckBox.Location = new System.Drawing.Point(5, 191);
            productCheckBox.Margin = new System.Windows.Forms.Padding(4);
            productCheckBox.Name = "productCheckBox";
            productCheckBox.Size = new System.Drawing.Size(110, 28);
            productCheckBox.TabIndex = 15;
            productCheckBox.Text = "Product:";
            productCheckBox.UseVisualStyleBackColor = true;
            // 
            // versionCheckBox
            // 
            versionCheckBox.AutoCheck = false;
            versionCheckBox.AutoSize = true;
            versionCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            versionCheckBox.Location = new System.Drawing.Point(5, 144);
            versionCheckBox.Margin = new System.Windows.Forms.Padding(4);
            versionCheckBox.Name = "versionCheckBox";
            versionCheckBox.Size = new System.Drawing.Size(109, 28);
            versionCheckBox.TabIndex = 14;
            versionCheckBox.Text = "Version:";
            versionCheckBox.UseVisualStyleBackColor = true;
            // 
            // filenameCheckBox
            // 
            filenameCheckBox.AutoCheck = false;
            filenameCheckBox.AutoSize = true;
            filenameCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            filenameCheckBox.Location = new System.Drawing.Point(5, 97);
            filenameCheckBox.Margin = new System.Windows.Forms.Padding(4);
            filenameCheckBox.Name = "filenameCheckBox";
            filenameCheckBox.Size = new System.Drawing.Size(124, 28);
            filenameCheckBox.TabIndex = 13;
            filenameCheckBox.Text = "Filename:";
            filenameCheckBox.UseVisualStyleBackColor = true;
            // 
            // publisherCheckBox
            // 
            publisherCheckBox.AutoCheck = false;
            publisherCheckBox.AutoSize = true;
            publisherCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            publisherCheckBox.Location = new System.Drawing.Point(5, 50);
            publisherCheckBox.Margin = new System.Windows.Forms.Padding(4);
            publisherCheckBox.Name = "publisherCheckBox";
            publisherCheckBox.Size = new System.Drawing.Size(124, 28);
            publisherCheckBox.TabIndex = 12;
            publisherCheckBox.Text = "Publisher:";
            publisherCheckBox.UseVisualStyleBackColor = true;
            // 
            // issuerCheckBox
            // 
            issuerCheckBox.AutoCheck = false;
            issuerCheckBox.AutoSize = true;
            issuerCheckBox.Checked = true;
            issuerCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            issuerCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            issuerCheckBox.Location = new System.Drawing.Point(5, 4);
            issuerCheckBox.Margin = new System.Windows.Forms.Padding(4);
            issuerCheckBox.Name = "issuerCheckBox";
            issuerCheckBox.Size = new System.Drawing.Size(97, 28);
            issuerCheckBox.TabIndex = 10;
            issuerCheckBox.Text = "Issuer:";
            issuerCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label3.Location = new System.Drawing.Point(196, 498);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(106, 24);
            label3.TabIndex = 8;
            label3.Text = "Rule Type:";
            // 
            // ruleTypeComboBox
            // 
            ruleTypeComboBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            ruleTypeComboBox.FormattingEnabled = true;
            ruleTypeComboBox.Items.AddRange(new object[] { "Publisher", "Path", "File Attributes", "Packaged App", "File Hash" });
            ruleTypeComboBox.Location = new System.Drawing.Point(311, 494);
            ruleTypeComboBox.Margin = new System.Windows.Forms.Padding(4);
            ruleTypeComboBox.Name = "ruleTypeComboBox";
            ruleTypeComboBox.Size = new System.Drawing.Size(195, 32);
            ruleTypeComboBox.TabIndex = 3;
            ruleTypeComboBox.SelectedIndexChanged += RuleTypeChanged;
            // 
            // fileAttributeRulePanel
            // 
            fileAttributeRulePanel.Controls.Add(pfnTextBox);
            fileAttributeRulePanel.Controls.Add(intFileNameTextBox);
            fileAttributeRulePanel.Controls.Add(prodNameTextBox);
            fileAttributeRulePanel.Controls.Add(fileDescTextBox);
            fileAttributeRulePanel.Controls.Add(origFileNameTextBox);
            fileAttributeRulePanel.Controls.Add(pfnCheckBox);
            fileAttributeRulePanel.Controls.Add(intFileNameCheckBox);
            fileAttributeRulePanel.Controls.Add(prodNameCheckBox);
            fileAttributeRulePanel.Controls.Add(fileDescCheckBox);
            fileAttributeRulePanel.Controls.Add(origFileNameCheckBox);
            fileAttributeRulePanel.Location = new System.Drawing.Point(1087, 536);
            fileAttributeRulePanel.Margin = new System.Windows.Forms.Padding(4);
            fileAttributeRulePanel.Name = "fileAttributeRulePanel";
            fileAttributeRulePanel.Size = new System.Drawing.Size(661, 228);
            fileAttributeRulePanel.TabIndex = 21;
            fileAttributeRulePanel.Visible = false;
            // 
            // pfnTextBox
            // 
            pfnTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pfnTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            pfnTextBox.Location = new System.Drawing.Point(208, 188);
            pfnTextBox.Margin = new System.Windows.Forms.Padding(4);
            pfnTextBox.Name = "pfnTextBox";
            pfnTextBox.ReadOnly = true;
            pfnTextBox.Size = new System.Drawing.Size(421, 32);
            pfnTextBox.TabIndex = 20;
            // 
            // intFileNameTextBox
            // 
            intFileNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            intFileNameTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            intFileNameTextBox.Location = new System.Drawing.Point(208, 142);
            intFileNameTextBox.Margin = new System.Windows.Forms.Padding(4);
            intFileNameTextBox.Name = "intFileNameTextBox";
            intFileNameTextBox.ReadOnly = true;
            intFileNameTextBox.Size = new System.Drawing.Size(421, 32);
            intFileNameTextBox.TabIndex = 19;
            // 
            // prodNameTextBox
            // 
            prodNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            prodNameTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            prodNameTextBox.Location = new System.Drawing.Point(208, 95);
            prodNameTextBox.Margin = new System.Windows.Forms.Padding(4);
            prodNameTextBox.Name = "prodNameTextBox";
            prodNameTextBox.ReadOnly = true;
            prodNameTextBox.Size = new System.Drawing.Size(422, 32);
            prodNameTextBox.TabIndex = 18;
            // 
            // fileDescTextBox
            // 
            fileDescTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            fileDescTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            fileDescTextBox.Location = new System.Drawing.Point(208, 48);
            fileDescTextBox.Margin = new System.Windows.Forms.Padding(4);
            fileDescTextBox.Name = "fileDescTextBox";
            fileDescTextBox.ReadOnly = true;
            fileDescTextBox.Size = new System.Drawing.Size(421, 32);
            fileDescTextBox.TabIndex = 17;
            // 
            // origFileNameTextBox
            // 
            origFileNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            origFileNameTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            origFileNameTextBox.Location = new System.Drawing.Point(208, 1);
            origFileNameTextBox.Margin = new System.Windows.Forms.Padding(4);
            origFileNameTextBox.Name = "origFileNameTextBox";
            origFileNameTextBox.ReadOnly = true;
            origFileNameTextBox.Size = new System.Drawing.Size(421, 32);
            origFileNameTextBox.TabIndex = 16;
            // 
            // pfnCheckBox
            // 
            pfnCheckBox.AutoCheck = false;
            pfnCheckBox.AutoSize = true;
            pfnCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            pfnCheckBox.Location = new System.Drawing.Point(5, 191);
            pfnCheckBox.Margin = new System.Windows.Forms.Padding(4);
            pfnCheckBox.Name = "pfnCheckBox";
            pfnCheckBox.Size = new System.Drawing.Size(175, 28);
            pfnCheckBox.TabIndex = 15;
            pfnCheckBox.Text = "Package Name:";
            pfnCheckBox.UseVisualStyleBackColor = true;
            // 
            // intFileNameCheckBox
            // 
            intFileNameCheckBox.AutoCheck = false;
            intFileNameCheckBox.AutoSize = true;
            intFileNameCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            intFileNameCheckBox.Location = new System.Drawing.Point(5, 144);
            intFileNameCheckBox.Margin = new System.Windows.Forms.Padding(4);
            intFileNameCheckBox.Name = "intFileNameCheckBox";
            intFileNameCheckBox.Size = new System.Drawing.Size(200, 28);
            intFileNameCheckBox.TabIndex = 14;
            intFileNameCheckBox.Text = "Internal Filename:";
            intFileNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // prodNameCheckBox
            // 
            prodNameCheckBox.AutoCheck = false;
            prodNameCheckBox.AutoSize = true;
            prodNameCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            prodNameCheckBox.Location = new System.Drawing.Point(5, 97);
            prodNameCheckBox.Margin = new System.Windows.Forms.Padding(4);
            prodNameCheckBox.Name = "prodNameCheckBox";
            prodNameCheckBox.Size = new System.Drawing.Size(168, 28);
            prodNameCheckBox.TabIndex = 13;
            prodNameCheckBox.Text = "Product Name:";
            prodNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // fileDescCheckBox
            // 
            fileDescCheckBox.AutoCheck = false;
            fileDescCheckBox.AutoSize = true;
            fileDescCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            fileDescCheckBox.Location = new System.Drawing.Point(5, 50);
            fileDescCheckBox.Margin = new System.Windows.Forms.Padding(4);
            fileDescCheckBox.Name = "fileDescCheckBox";
            fileDescCheckBox.Size = new System.Drawing.Size(180, 28);
            fileDescCheckBox.TabIndex = 12;
            fileDescCheckBox.Text = "File Description:";
            fileDescCheckBox.UseVisualStyleBackColor = true;
            // 
            // origFileNameCheckBox
            // 
            origFileNameCheckBox.AutoCheck = false;
            origFileNameCheckBox.AutoSize = true;
            origFileNameCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            origFileNameCheckBox.Location = new System.Drawing.Point(5, 4);
            origFileNameCheckBox.Margin = new System.Windows.Forms.Padding(4);
            origFileNameCheckBox.Name = "origFileNameCheckBox";
            origFileNameCheckBox.Size = new System.Drawing.Size(199, 28);
            origFileNameCheckBox.TabIndex = 10;
            origFileNameCheckBox.Text = "Original Filename:";
            origFileNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // eventsDataGridView
            // 
            eventsDataGridView.AllowUserToDeleteRows = false;
            eventsDataGridView.AllowUserToResizeRows = false;
            eventsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            eventsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            eventsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { addedColumn, eventIdColumn, filenameColumn, productColumn, publisherColumn, policyColumn });
            eventsDataGridView.EnableHeadersVisualStyles = false;
            eventsDataGridView.Location = new System.Drawing.Point(200, 121);
            eventsDataGridView.Margin = new System.Windows.Forms.Padding(2);
            eventsDataGridView.MultiSelect = false;
            eventsDataGridView.Name = "eventsDataGridView";
            eventsDataGridView.ReadOnly = true;
            eventsDataGridView.RowHeadersVisible = false;
            eventsDataGridView.RowHeadersWidth = 70;
            eventsDataGridView.RowTemplate.Height = 24;
            eventsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            eventsDataGridView.Size = new System.Drawing.Size(1075, 344);
            eventsDataGridView.TabIndex = 4;
            eventsDataGridView.VirtualMode = true;
            eventsDataGridView.CellClick += EventRowClick;
            eventsDataGridView.CellValueNeeded += EventsDataGrid_CellValueNeeded;
            eventsDataGridView.SelectionChanged += RowSelectionChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(194, 35);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(342, 34);
            label1.TabIndex = 6;
            label1.Text = "Configure Event Log Rules";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label2.Location = new System.Drawing.Point(197, 85);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(613, 22);
            label2.TabIndex = 7;
            label2.Text = "Create custom rules based on the App Control for Business event log events";
            // 
            // hashRulePanel
            // 
            hashRulePanel.Controls.Add(label5);
            hashRulePanel.Controls.Add(label4);
            hashRulePanel.Controls.Add(sha2TextBox);
            hashRulePanel.Controls.Add(sha1TextBox);
            hashRulePanel.Location = new System.Drawing.Point(918, 797);
            hashRulePanel.Margin = new System.Windows.Forms.Padding(4);
            hashRulePanel.Name = "hashRulePanel";
            hashRulePanel.Size = new System.Drawing.Size(724, 112);
            hashRulePanel.TabIndex = 22;
            hashRulePanel.Visible = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label5.Location = new System.Drawing.Point(14, 66);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(166, 24);
            label5.TabIndex = 24;
            label5.Text = "SHA256 PE Hash:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label4.Location = new System.Drawing.Point(14, 22);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(144, 24);
            label4.TabIndex = 23;
            label4.Text = "SHA1 PE Hash:";
            // 
            // sha2TextBox
            // 
            sha2TextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            sha2TextBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            sha2TextBox.Location = new System.Drawing.Point(208, 62);
            sha2TextBox.Margin = new System.Windows.Forms.Padding(4);
            sha2TextBox.Name = "sha2TextBox";
            sha2TextBox.ReadOnly = true;
            sha2TextBox.Size = new System.Drawing.Size(478, 29);
            sha2TextBox.TabIndex = 18;
            // 
            // sha1TextBox
            // 
            sha1TextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            sha1TextBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            sha1TextBox.Location = new System.Drawing.Point(208, 18);
            sha1TextBox.Margin = new System.Windows.Forms.Padding(4);
            sha1TextBox.Name = "sha1TextBox";
            sha1TextBox.ReadOnly = true;
            sha1TextBox.Size = new System.Drawing.Size(476, 29);
            sha1TextBox.TabIndex = 16;
            // 
            // filePathRulePanel
            // 
            filePathRulePanel.Controls.Add(filePathTextBox);
            filePathRulePanel.Controls.Add(folderPathTextBox);
            filePathRulePanel.Controls.Add(folderPathCheckBox);
            filePathRulePanel.Controls.Add(filePathCheckBox);
            filePathRulePanel.Location = new System.Drawing.Point(19, 797);
            filePathRulePanel.Margin = new System.Windows.Forms.Padding(4);
            filePathRulePanel.Name = "filePathRulePanel";
            filePathRulePanel.Size = new System.Drawing.Size(892, 112);
            filePathRulePanel.TabIndex = 22;
            filePathRulePanel.Visible = false;
            // 
            // filePathTextBox
            // 
            filePathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            filePathTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            filePathTextBox.Location = new System.Drawing.Point(208, 19);
            filePathTextBox.Margin = new System.Windows.Forms.Padding(4);
            filePathTextBox.Name = "filePathTextBox";
            filePathTextBox.ReadOnly = true;
            filePathTextBox.Size = new System.Drawing.Size(619, 32);
            filePathTextBox.TabIndex = 18;
            // 
            // folderPathTextBox
            // 
            folderPathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            folderPathTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            folderPathTextBox.Location = new System.Drawing.Point(208, 64);
            folderPathTextBox.Margin = new System.Windows.Forms.Padding(4);
            folderPathTextBox.Name = "folderPathTextBox";
            folderPathTextBox.ReadOnly = true;
            folderPathTextBox.Size = new System.Drawing.Size(619, 32);
            folderPathTextBox.TabIndex = 17;
            // 
            // folderPathCheckBox
            // 
            folderPathCheckBox.AutoSize = true;
            folderPathCheckBox.Checked = true;
            folderPathCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            folderPathCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            folderPathCheckBox.Location = new System.Drawing.Point(4, 66);
            folderPathCheckBox.Margin = new System.Windows.Forms.Padding(4);
            folderPathCheckBox.Name = "folderPathCheckBox";
            folderPathCheckBox.Size = new System.Drawing.Size(144, 28);
            folderPathCheckBox.TabIndex = 12;
            folderPathCheckBox.Text = "Folder Path:";
            folderPathCheckBox.UseVisualStyleBackColor = true;
            // 
            // filePathCheckBox
            // 
            filePathCheckBox.AutoSize = true;
            filePathCheckBox.Checked = true;
            filePathCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            filePathCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            filePathCheckBox.Location = new System.Drawing.Point(4, 22);
            filePathCheckBox.Margin = new System.Windows.Forms.Padding(4);
            filePathCheckBox.Name = "filePathCheckBox";
            filePathCheckBox.Size = new System.Drawing.Size(120, 28);
            filePathCheckBox.TabIndex = 10;
            filePathCheckBox.Text = "File Path:";
            filePathCheckBox.UseVisualStyleBackColor = true;
            // 
            // addedColumn
            // 
            addedColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            addedColumn.HeaderText = "Added To Policy";
            addedColumn.MinimumWidth = 100;
            addedColumn.Name = "addedColumn";
            addedColumn.ReadOnly = true;
            addedColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            addedColumn.Width = 150;
            // 
            // eventIdColumn
            // 
            eventIdColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            eventIdColumn.HeaderText = "Event Id";
            eventIdColumn.MinimumWidth = 70;
            eventIdColumn.Name = "eventIdColumn";
            eventIdColumn.ReadOnly = true;
            eventIdColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            eventIdColumn.Width = 70;
            // 
            // filenameColumn
            // 
            filenameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            filenameColumn.HeaderText = "Filename";
            filenameColumn.MinimumWidth = 100;
            filenameColumn.Name = "filenameColumn";
            filenameColumn.ReadOnly = true;
            filenameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // productColumn
            // 
            productColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            productColumn.HeaderText = "Product";
            productColumn.MinimumWidth = 100;
            productColumn.Name = "productColumn";
            productColumn.ReadOnly = true;
            productColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // publisherColumn
            // 
            publisherColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            publisherColumn.HeaderText = "Publisher";
            publisherColumn.MinimumWidth = 100;
            publisherColumn.Name = "publisherColumn";
            publisherColumn.ReadOnly = true;
            publisherColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // policyColumn
            // 
            policyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            policyColumn.HeaderText = "Policy Name";
            policyColumn.MinimumWidth = 134;
            policyColumn.Name = "policyColumn";
            policyColumn.ReadOnly = true;
            policyColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            policyColumn.Width = 134;
            // 
            // EventLogRuleConfiguration
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            BackColor = System.Drawing.Color.White;
            Controls.Add(publisherRulePanel);
            Controls.Add(label3);
            Controls.Add(filePathRulePanel);
            Controls.Add(ruleTypeComboBox);
            Controls.Add(hashRulePanel);
            Controls.Add(fileAttributeRulePanel);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(eventsDataGridView);
            Controls.Add(addButton);
            Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            Name = "EventLogRuleConfiguration";
            Size = new System.Drawing.Size(1450, 938);
            Load += EventLogRuleConfiguration_Load;
            Validated += EventLogRuleConfiguration_Validated;
            publisherRulePanel.ResumeLayout(false);
            publisherRulePanel.PerformLayout();
            fileAttributeRulePanel.ResumeLayout(false);
            fileAttributeRulePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)eventsDataGridView).EndInit();
            hashRulePanel.ResumeLayout(false);
            hashRulePanel.PerformLayout();
            filePathRulePanel.ResumeLayout(false);
            filePathRulePanel.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ComboBox ruleTypeComboBox;
        private System.Windows.Forms.DataGridView eventsDataGridView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel publisherRulePanel;
        private System.Windows.Forms.TextBox productTextBox;
        private System.Windows.Forms.TextBox versionTextBox;
        private System.Windows.Forms.TextBox filenameTextBox;
        private System.Windows.Forms.TextBox publisherTextBox;
        private System.Windows.Forms.TextBox issuerTextBox;
        private System.Windows.Forms.CheckBox productCheckBox;
        private System.Windows.Forms.CheckBox versionCheckBox;
        private System.Windows.Forms.CheckBox filenameCheckBox;
        private System.Windows.Forms.CheckBox publisherCheckBox;
        private System.Windows.Forms.CheckBox issuerCheckBox;
        private System.Windows.Forms.Panel fileAttributeRulePanel;
        private System.Windows.Forms.TextBox pfnTextBox;
        private System.Windows.Forms.TextBox intFileNameTextBox;
        private System.Windows.Forms.TextBox prodNameTextBox;
        private System.Windows.Forms.TextBox fileDescTextBox;
        private System.Windows.Forms.TextBox origFileNameTextBox;
        private System.Windows.Forms.CheckBox pfnCheckBox;
        private System.Windows.Forms.CheckBox intFileNameCheckBox;
        private System.Windows.Forms.CheckBox prodNameCheckBox;
        private System.Windows.Forms.CheckBox fileDescCheckBox;
        private System.Windows.Forms.CheckBox origFileNameCheckBox;
        private System.Windows.Forms.Panel hashRulePanel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox sha2TextBox;
        private System.Windows.Forms.TextBox sha1TextBox;
        private System.Windows.Forms.Panel filePathRulePanel;
        private System.Windows.Forms.TextBox folderPathTextBox;
        private System.Windows.Forms.CheckBox folderPathCheckBox;
        private System.Windows.Forms.CheckBox filePathCheckBox;
        private System.Windows.Forms.TextBox filePathTextBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn addedColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn eventIdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn filenameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn productColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn publisherColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn policyColumn;
    }
}
