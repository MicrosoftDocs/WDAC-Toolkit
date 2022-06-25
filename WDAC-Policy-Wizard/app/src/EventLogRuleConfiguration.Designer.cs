
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
            this.addButton = new System.Windows.Forms.Button();
            this.publisherRulePanel = new System.Windows.Forms.Panel();
            this.productTextBox = new System.Windows.Forms.TextBox();
            this.versionTextBox = new System.Windows.Forms.TextBox();
            this.filenameTextBox = new System.Windows.Forms.TextBox();
            this.publisherTextBox = new System.Windows.Forms.TextBox();
            this.issuerTextBox = new System.Windows.Forms.TextBox();
            this.productCheckBox = new System.Windows.Forms.CheckBox();
            this.versionCheckBox = new System.Windows.Forms.CheckBox();
            this.filenameCheckBox = new System.Windows.Forms.CheckBox();
            this.publisherCheckBox = new System.Windows.Forms.CheckBox();
            this.issuerCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.ruleTypeComboBox = new System.Windows.Forms.ComboBox();
            this.fileAttributeRulePanel = new System.Windows.Forms.Panel();
            this.pfnTextBox = new System.Windows.Forms.TextBox();
            this.intFileNameTextBox = new System.Windows.Forms.TextBox();
            this.prodNameTextBox = new System.Windows.Forms.TextBox();
            this.fileDescTextBox = new System.Windows.Forms.TextBox();
            this.origFileNameTextBox = new System.Windows.Forms.TextBox();
            this.pfnCheckBox = new System.Windows.Forms.CheckBox();
            this.intFileNameCheckBox = new System.Windows.Forms.CheckBox();
            this.prodNameCheckBox = new System.Windows.Forms.CheckBox();
            this.fileDescCheckBox = new System.Windows.Forms.CheckBox();
            this.origFileNameCheckBox = new System.Windows.Forms.CheckBox();
            this.eventsDataGridView = new System.Windows.Forms.DataGridView();
            this.addedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eventIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filenameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.productColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.policyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.publisherColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.hashRulePanel = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.sha2PageTextBox = new System.Windows.Forms.TextBox();
            this.sha2TextBox = new System.Windows.Forms.TextBox();
            this.sha1PageTextBox = new System.Windows.Forms.TextBox();
            this.sha1TextBox = new System.Windows.Forms.TextBox();
            this.filePathRulePanel = new System.Windows.Forms.Panel();
            this.filePathTextBox = new System.Windows.Forms.TextBox();
            this.folderPathTextBox = new System.Windows.Forms.TextBox();
            this.folderPathCheckBox = new System.Windows.Forms.CheckBox();
            this.filePathCheckBox = new System.Windows.Forms.CheckBox();
            this.publisherRulePanel.SuspendLayout();
            this.fileAttributeRulePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventsDataGridView)).BeginInit();
            this.hashRulePanel.SuspendLayout();
            this.filePathRulePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(167, 653);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(113, 30);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "Add Allow Rule";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // publisherRulePanel
            // 
            this.publisherRulePanel.Controls.Add(this.productTextBox);
            this.publisherRulePanel.Controls.Add(this.versionTextBox);
            this.publisherRulePanel.Controls.Add(this.filenameTextBox);
            this.publisherRulePanel.Controls.Add(this.publisherTextBox);
            this.publisherRulePanel.Controls.Add(this.issuerTextBox);
            this.publisherRulePanel.Controls.Add(this.productCheckBox);
            this.publisherRulePanel.Controls.Add(this.versionCheckBox);
            this.publisherRulePanel.Controls.Add(this.filenameCheckBox);
            this.publisherRulePanel.Controls.Add(this.publisherCheckBox);
            this.publisherRulePanel.Controls.Add(this.issuerCheckBox);
            this.publisherRulePanel.Location = new System.Drawing.Point(167, 447);
            this.publisherRulePanel.Name = "publisherRulePanel";
            this.publisherRulePanel.Size = new System.Drawing.Size(716, 190);
            this.publisherRulePanel.TabIndex = 9;
            // 
            // productTextBox
            // 
            this.productTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.productTextBox.Location = new System.Drawing.Point(102, 159);
            this.productTextBox.Name = "productTextBox";
            this.productTextBox.ReadOnly = true;
            this.productTextBox.Size = new System.Drawing.Size(420, 28);
            this.productTextBox.TabIndex = 20;
            // 
            // versionTextBox
            // 
            this.versionTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionTextBox.Location = new System.Drawing.Point(102, 120);
            this.versionTextBox.Name = "versionTextBox";
            this.versionTextBox.ReadOnly = true;
            this.versionTextBox.Size = new System.Drawing.Size(420, 28);
            this.versionTextBox.TabIndex = 19;
            // 
            // filenameTextBox
            // 
            this.filenameTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filenameTextBox.Location = new System.Drawing.Point(101, 81);
            this.filenameTextBox.Name = "filenameTextBox";
            this.filenameTextBox.ReadOnly = true;
            this.filenameTextBox.Size = new System.Drawing.Size(421, 28);
            this.filenameTextBox.TabIndex = 18;
            // 
            // publisherTextBox
            // 
            this.publisherTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.publisherTextBox.Location = new System.Drawing.Point(103, 42);
            this.publisherTextBox.Name = "publisherTextBox";
            this.publisherTextBox.ReadOnly = true;
            this.publisherTextBox.Size = new System.Drawing.Size(420, 28);
            this.publisherTextBox.TabIndex = 17;
            // 
            // issuerTextBox
            // 
            this.issuerTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.issuerTextBox.Location = new System.Drawing.Point(103, 3);
            this.issuerTextBox.Name = "issuerTextBox";
            this.issuerTextBox.ReadOnly = true;
            this.issuerTextBox.Size = new System.Drawing.Size(420, 28);
            this.issuerTextBox.TabIndex = 16;
            // 
            // productCheckBox
            // 
            this.productCheckBox.AutoSize = true;
            this.productCheckBox.Enabled = false;
            this.productCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.productCheckBox.Location = new System.Drawing.Point(3, 159);
            this.productCheckBox.Name = "productCheckBox";
            this.productCheckBox.Size = new System.Drawing.Size(94, 25);
            this.productCheckBox.TabIndex = 15;
            this.productCheckBox.Text = "Product:";
            this.productCheckBox.UseVisualStyleBackColor = true;
            // 
            // versionCheckBox
            // 
            this.versionCheckBox.AutoSize = true;
            this.versionCheckBox.Enabled = false;
            this.versionCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionCheckBox.Location = new System.Drawing.Point(2, 120);
            this.versionCheckBox.Name = "versionCheckBox";
            this.versionCheckBox.Size = new System.Drawing.Size(93, 25);
            this.versionCheckBox.TabIndex = 14;
            this.versionCheckBox.Text = "Version:";
            this.versionCheckBox.UseVisualStyleBackColor = true;
            // 
            // filenameCheckBox
            // 
            this.filenameCheckBox.AutoSize = true;
            this.filenameCheckBox.Enabled = false;
            this.filenameCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filenameCheckBox.Location = new System.Drawing.Point(3, 81);
            this.filenameCheckBox.Name = "filenameCheckBox";
            this.filenameCheckBox.Size = new System.Drawing.Size(105, 25);
            this.filenameCheckBox.TabIndex = 13;
            this.filenameCheckBox.Text = "Filename:";
            this.filenameCheckBox.UseVisualStyleBackColor = true;
            // 
            // publisherCheckBox
            // 
            this.publisherCheckBox.AutoSize = true;
            this.publisherCheckBox.Enabled = false;
            this.publisherCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.publisherCheckBox.Location = new System.Drawing.Point(3, 42);
            this.publisherCheckBox.Name = "publisherCheckBox";
            this.publisherCheckBox.Size = new System.Drawing.Size(105, 25);
            this.publisherCheckBox.TabIndex = 12;
            this.publisherCheckBox.Text = "Publisher:";
            this.publisherCheckBox.UseVisualStyleBackColor = true;
            // 
            // issuerCheckBox
            // 
            this.issuerCheckBox.AutoSize = true;
            this.issuerCheckBox.Checked = true;
            this.issuerCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.issuerCheckBox.Enabled = false;
            this.issuerCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.issuerCheckBox.Location = new System.Drawing.Point(4, 3);
            this.issuerCheckBox.Name = "issuerCheckBox";
            this.issuerCheckBox.Size = new System.Drawing.Size(84, 25);
            this.issuerCheckBox.TabIndex = 10;
            this.issuerCheckBox.Text = "Issuer:";
            this.issuerCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(163, 415);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 21);
            this.label3.TabIndex = 8;
            this.label3.Text = "Rule Type:";
            // 
            // ruleTypeComboBox
            // 
            this.ruleTypeComboBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ruleTypeComboBox.FormattingEnabled = true;
            this.ruleTypeComboBox.Items.AddRange(new object[] {
            "Publisher",
            "Path",
            "File Attributes",
            "Packaged App",
            "File Hash"});
            this.ruleTypeComboBox.Location = new System.Drawing.Point(259, 412);
            this.ruleTypeComboBox.Name = "ruleTypeComboBox";
            this.ruleTypeComboBox.Size = new System.Drawing.Size(163, 29);
            this.ruleTypeComboBox.TabIndex = 3;
            this.ruleTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.RuleTypeChanged);
            // 
            // fileAttributeRulePanel
            // 
            this.fileAttributeRulePanel.Controls.Add(this.pfnTextBox);
            this.fileAttributeRulePanel.Controls.Add(this.intFileNameTextBox);
            this.fileAttributeRulePanel.Controls.Add(this.prodNameTextBox);
            this.fileAttributeRulePanel.Controls.Add(this.fileDescTextBox);
            this.fileAttributeRulePanel.Controls.Add(this.origFileNameTextBox);
            this.fileAttributeRulePanel.Controls.Add(this.pfnCheckBox);
            this.fileAttributeRulePanel.Controls.Add(this.intFileNameCheckBox);
            this.fileAttributeRulePanel.Controls.Add(this.prodNameCheckBox);
            this.fileAttributeRulePanel.Controls.Add(this.fileDescCheckBox);
            this.fileAttributeRulePanel.Controls.Add(this.origFileNameCheckBox);
            this.fileAttributeRulePanel.Location = new System.Drawing.Point(1021, 99);
            this.fileAttributeRulePanel.Name = "fileAttributeRulePanel";
            this.fileAttributeRulePanel.Size = new System.Drawing.Size(551, 190);
            this.fileAttributeRulePanel.TabIndex = 21;
            this.fileAttributeRulePanel.Visible = false;
            // 
            // pfnTextBox
            // 
            this.pfnTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pfnTextBox.Location = new System.Drawing.Point(172, 159);
            this.pfnTextBox.Name = "pfnTextBox";
            this.pfnTextBox.ReadOnly = true;
            this.pfnTextBox.Size = new System.Drawing.Size(351, 28);
            this.pfnTextBox.TabIndex = 20;
            // 
            // intFileNameTextBox
            // 
            this.intFileNameTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.intFileNameTextBox.Location = new System.Drawing.Point(172, 120);
            this.intFileNameTextBox.Name = "intFileNameTextBox";
            this.intFileNameTextBox.ReadOnly = true;
            this.intFileNameTextBox.Size = new System.Drawing.Size(351, 28);
            this.intFileNameTextBox.TabIndex = 19;
            // 
            // prodNameTextBox
            // 
            this.prodNameTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prodNameTextBox.Location = new System.Drawing.Point(171, 81);
            this.prodNameTextBox.Name = "prodNameTextBox";
            this.prodNameTextBox.ReadOnly = true;
            this.prodNameTextBox.Size = new System.Drawing.Size(352, 28);
            this.prodNameTextBox.TabIndex = 18;
            // 
            // fileDescTextBox
            // 
            this.fileDescTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileDescTextBox.Location = new System.Drawing.Point(173, 42);
            this.fileDescTextBox.Name = "fileDescTextBox";
            this.fileDescTextBox.ReadOnly = true;
            this.fileDescTextBox.Size = new System.Drawing.Size(351, 28);
            this.fileDescTextBox.TabIndex = 17;
            // 
            // origFileNameTextBox
            // 
            this.origFileNameTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.origFileNameTextBox.Location = new System.Drawing.Point(173, 3);
            this.origFileNameTextBox.Name = "origFileNameTextBox";
            this.origFileNameTextBox.ReadOnly = true;
            this.origFileNameTextBox.Size = new System.Drawing.Size(351, 28);
            this.origFileNameTextBox.TabIndex = 16;
            // 
            // pfnCheckBox
            // 
            this.pfnCheckBox.AutoSize = true;
            this.pfnCheckBox.Enabled = false;
            this.pfnCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pfnCheckBox.Location = new System.Drawing.Point(3, 159);
            this.pfnCheckBox.Name = "pfnCheckBox";
            this.pfnCheckBox.Size = new System.Drawing.Size(147, 25);
            this.pfnCheckBox.TabIndex = 15;
            this.pfnCheckBox.Text = "Package Name:";
            this.pfnCheckBox.UseVisualStyleBackColor = true;
            // 
            // intFileNameCheckBox
            // 
            this.intFileNameCheckBox.AutoSize = true;
            this.intFileNameCheckBox.Enabled = false;
            this.intFileNameCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.intFileNameCheckBox.Location = new System.Drawing.Point(2, 120);
            this.intFileNameCheckBox.Name = "intFileNameCheckBox";
            this.intFileNameCheckBox.Size = new System.Drawing.Size(168, 25);
            this.intFileNameCheckBox.TabIndex = 14;
            this.intFileNameCheckBox.Text = "Internal Filename:";
            this.intFileNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // prodNameCheckBox
            // 
            this.prodNameCheckBox.AutoSize = true;
            this.prodNameCheckBox.Enabled = false;
            this.prodNameCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prodNameCheckBox.Location = new System.Drawing.Point(3, 81);
            this.prodNameCheckBox.Name = "prodNameCheckBox";
            this.prodNameCheckBox.Size = new System.Drawing.Size(142, 25);
            this.prodNameCheckBox.TabIndex = 13;
            this.prodNameCheckBox.Text = "Product Name:";
            this.prodNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // fileDescCheckBox
            // 
            this.fileDescCheckBox.AutoSize = true;
            this.fileDescCheckBox.Enabled = false;
            this.fileDescCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileDescCheckBox.Location = new System.Drawing.Point(3, 42);
            this.fileDescCheckBox.Name = "fileDescCheckBox";
            this.fileDescCheckBox.Size = new System.Drawing.Size(153, 25);
            this.fileDescCheckBox.TabIndex = 12;
            this.fileDescCheckBox.Text = "File Description:";
            this.fileDescCheckBox.UseVisualStyleBackColor = true;
            // 
            // origFileNameCheckBox
            // 
            this.origFileNameCheckBox.AutoSize = true;
            this.origFileNameCheckBox.Enabled = false;
            this.origFileNameCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.origFileNameCheckBox.Location = new System.Drawing.Point(4, 3);
            this.origFileNameCheckBox.Name = "origFileNameCheckBox";
            this.origFileNameCheckBox.Size = new System.Drawing.Size(167, 25);
            this.origFileNameCheckBox.TabIndex = 10;
            this.origFileNameCheckBox.Text = "Original Filename:";
            this.origFileNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // eventsDataGridView
            // 
            this.eventsDataGridView.AllowUserToDeleteRows = false;
            this.eventsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.eventsDataGridView.BackgroundColor = System.Drawing.Color.WhiteSmoke;
            this.eventsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eventsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.addedColumn,
            this.eventIdColumn,
            this.filenameColumn,
            this.productColumn,
            this.policyColumn,
            this.publisherColumn});
            this.eventsDataGridView.Location = new System.Drawing.Point(167, 101);
            this.eventsDataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.eventsDataGridView.MultiSelect = false;
            this.eventsDataGridView.Name = "eventsDataGridView";
            this.eventsDataGridView.ReadOnly = true;
            this.eventsDataGridView.RowHeadersVisible = false;
            this.eventsDataGridView.RowHeadersWidth = 70;
            this.eventsDataGridView.RowTemplate.Height = 24;
            this.eventsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.eventsDataGridView.Size = new System.Drawing.Size(799, 287);
            this.eventsDataGridView.TabIndex = 4;
            this.eventsDataGridView.VirtualMode = true;
            this.eventsDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.EventRowClick);
            this.eventsDataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.EventsDataGrid_CellValueNeeded);
            // 
            // addedColumn
            // 
            this.addedColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.addedColumn.HeaderText = "Added To Policy";
            this.addedColumn.MinimumWidth = 100;
            this.addedColumn.Name = "addedColumn";
            this.addedColumn.ReadOnly = true;
            // 
            // eventIdColumn
            // 
            this.eventIdColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.eventIdColumn.HeaderText = "Event Id";
            this.eventIdColumn.MinimumWidth = 6;
            this.eventIdColumn.Name = "eventIdColumn";
            this.eventIdColumn.ReadOnly = true;
            this.eventIdColumn.Width = 82;
            // 
            // filenameColumn
            // 
            this.filenameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.filenameColumn.HeaderText = "Filename";
            this.filenameColumn.MinimumWidth = 6;
            this.filenameColumn.Name = "filenameColumn";
            this.filenameColumn.ReadOnly = true;
            this.filenameColumn.Width = 94;
            // 
            // productColumn
            // 
            this.productColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.productColumn.HeaderText = "Product";
            this.productColumn.MinimumWidth = 6;
            this.productColumn.Name = "productColumn";
            this.productColumn.ReadOnly = true;
            this.productColumn.Width = 86;
            // 
            // policyColumn
            // 
            this.policyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.policyColumn.HeaderText = "Policy Name";
            this.policyColumn.MinimumWidth = 6;
            this.policyColumn.Name = "policyColumn";
            this.policyColumn.ReadOnly = true;
            this.policyColumn.Width = 106;
            // 
            // publisherColumn
            // 
            this.publisherColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.publisherColumn.HeaderText = "Publisher";
            this.publisherColumn.MinimumWidth = 6;
            this.publisherColumn.Name = "publisherColumn";
            this.publisherColumn.ReadOnly = true;
            this.publisherColumn.Width = 96;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(162, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(292, 29);
            this.label1.TabIndex = 6;
            this.label1.Text = "Configure Event Log Rules";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(164, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(391, 18);
            this.label2.TabIndex = 7;
            this.label2.Text = "Create custom rules based on the WDAC event log events";
            // 
            // hashRulePanel
            // 
            this.hashRulePanel.Controls.Add(this.label7);
            this.hashRulePanel.Controls.Add(this.label6);
            this.hashRulePanel.Controls.Add(this.label5);
            this.hashRulePanel.Controls.Add(this.label4);
            this.hashRulePanel.Controls.Add(this.sha2PageTextBox);
            this.hashRulePanel.Controls.Add(this.sha2TextBox);
            this.hashRulePanel.Controls.Add(this.sha1PageTextBox);
            this.hashRulePanel.Controls.Add(this.sha1TextBox);
            this.hashRulePanel.Location = new System.Drawing.Point(1021, 464);
            this.hashRulePanel.Name = "hashRulePanel";
            this.hashRulePanel.Size = new System.Drawing.Size(603, 164);
            this.hashRulePanel.TabIndex = 22;
            this.hashRulePanel.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(12, 124);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(157, 21);
            this.label7.TabIndex = 26;
            this.label7.Text = "SHA256 Page Hash:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(139, 21);
            this.label6.TabIndex = 25;
            this.label6.Text = "SHA1 Page Hash:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(140, 21);
            this.label5.TabIndex = 24;
            this.label5.Text = "SHA256 PE Hash:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 21);
            this.label4.TabIndex = 23;
            this.label4.Text = "SHA1 PE Hash:";
            // 
            // sha2PageTextBox
            // 
            this.sha2PageTextBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sha2PageTextBox.Location = new System.Drawing.Point(172, 120);
            this.sha2PageTextBox.Name = "sha2PageTextBox";
            this.sha2PageTextBox.ReadOnly = true;
            this.sha2PageTextBox.Size = new System.Drawing.Size(351, 26);
            this.sha2PageTextBox.TabIndex = 19;
            // 
            // sha2TextBox
            // 
            this.sha2TextBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sha2TextBox.Location = new System.Drawing.Point(171, 81);
            this.sha2TextBox.Name = "sha2TextBox";
            this.sha2TextBox.ReadOnly = true;
            this.sha2TextBox.Size = new System.Drawing.Size(352, 26);
            this.sha2TextBox.TabIndex = 18;
            // 
            // sha1PageTextBox
            // 
            this.sha1PageTextBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sha1PageTextBox.Location = new System.Drawing.Point(173, 42);
            this.sha1PageTextBox.Name = "sha1PageTextBox";
            this.sha1PageTextBox.ReadOnly = true;
            this.sha1PageTextBox.Size = new System.Drawing.Size(351, 26);
            this.sha1PageTextBox.TabIndex = 17;
            // 
            // sha1TextBox
            // 
            this.sha1TextBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sha1TextBox.Location = new System.Drawing.Point(173, 3);
            this.sha1TextBox.Name = "sha1TextBox";
            this.sha1TextBox.ReadOnly = true;
            this.sha1TextBox.Size = new System.Drawing.Size(351, 26);
            this.sha1TextBox.TabIndex = 16;
            // 
            // filePathRulePanel
            // 
            this.filePathRulePanel.Controls.Add(this.filePathTextBox);
            this.filePathRulePanel.Controls.Add(this.folderPathTextBox);
            this.filePathRulePanel.Controls.Add(this.folderPathCheckBox);
            this.filePathRulePanel.Controls.Add(this.filePathCheckBox);
            this.filePathRulePanel.Location = new System.Drawing.Point(1021, 333);
            this.filePathRulePanel.Name = "filePathRulePanel";
            this.filePathRulePanel.Size = new System.Drawing.Size(743, 93);
            this.filePathRulePanel.TabIndex = 22;
            this.filePathRulePanel.Visible = false;
            // 
            // filePathTextBox
            // 
            this.filePathTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filePathTextBox.Location = new System.Drawing.Point(173, 5);
            this.filePathTextBox.Name = "filePathTextBox";
            this.filePathTextBox.ReadOnly = true;
            this.filePathTextBox.Size = new System.Drawing.Size(516, 28);
            this.filePathTextBox.TabIndex = 18;
            // 
            // folderPathTextBox
            // 
            this.folderPathTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.folderPathTextBox.Location = new System.Drawing.Point(173, 42);
            this.folderPathTextBox.Name = "folderPathTextBox";
            this.folderPathTextBox.ReadOnly = true;
            this.folderPathTextBox.Size = new System.Drawing.Size(516, 28);
            this.folderPathTextBox.TabIndex = 17;
            // 
            // folderPathCheckBox
            // 
            this.folderPathCheckBox.AutoSize = true;
            this.folderPathCheckBox.Checked = true;
            this.folderPathCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.folderPathCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.folderPathCheckBox.Location = new System.Drawing.Point(3, 44);
            this.folderPathCheckBox.Name = "folderPathCheckBox";
            this.folderPathCheckBox.Size = new System.Drawing.Size(122, 25);
            this.folderPathCheckBox.TabIndex = 12;
            this.folderPathCheckBox.Text = "Folder Path:";
            this.folderPathCheckBox.UseVisualStyleBackColor = true;
            // 
            // filePathCheckBox
            // 
            this.filePathCheckBox.AutoSize = true;
            this.filePathCheckBox.Checked = true;
            this.filePathCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.filePathCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filePathCheckBox.Location = new System.Drawing.Point(4, 7);
            this.filePathCheckBox.Name = "filePathCheckBox";
            this.filePathCheckBox.Size = new System.Drawing.Size(102, 25);
            this.filePathCheckBox.TabIndex = 10;
            this.filePathCheckBox.Text = "File Path:";
            this.filePathCheckBox.UseVisualStyleBackColor = true;
            // 
            // EventLogRuleConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.publisherRulePanel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.filePathRulePanel);
            this.Controls.Add(this.ruleTypeComboBox);
            this.Controls.Add(this.hashRulePanel);
            this.Controls.Add(this.fileAttributeRulePanel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.eventsDataGridView);
            this.Controls.Add(this.addButton);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "EventLogRuleConfiguration";
            this.Size = new System.Drawing.Size(1208, 782);
            this.Load += new System.EventHandler(this.EventLogRuleConfiguration_Load);
            this.publisherRulePanel.ResumeLayout(false);
            this.publisherRulePanel.PerformLayout();
            this.fileAttributeRulePanel.ResumeLayout(false);
            this.fileAttributeRulePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventsDataGridView)).EndInit();
            this.hashRulePanel.ResumeLayout(false);
            this.hashRulePanel.PerformLayout();
            this.filePathRulePanel.ResumeLayout(false);
            this.filePathRulePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.DataGridViewTextBoxColumn addedColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn eventIdColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn filenameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn productColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn policyColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn publisherColumn;
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
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox sha2PageTextBox;
        private System.Windows.Forms.TextBox sha2TextBox;
        private System.Windows.Forms.TextBox sha1PageTextBox;
        private System.Windows.Forms.TextBox sha1TextBox;
        private System.Windows.Forms.Panel filePathRulePanel;
        private System.Windows.Forms.TextBox folderPathTextBox;
        private System.Windows.Forms.CheckBox folderPathCheckBox;
        private System.Windows.Forms.CheckBox filePathCheckBox;
        private System.Windows.Forms.TextBox filePathTextBox;
    }
}
