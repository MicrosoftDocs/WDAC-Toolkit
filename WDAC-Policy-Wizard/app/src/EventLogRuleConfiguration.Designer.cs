
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
            addedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            eventIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            filenameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            productColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            policyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            publisherColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            publisherRulePanel.SuspendLayout();
            fileAttributeRulePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)eventsDataGridView).BeginInit();
            hashRulePanel.SuspendLayout();
            filePathRulePanel.SuspendLayout();
            SuspendLayout();
            // 
            // addButton
            // 
            addButton.Location = new System.Drawing.Point(442, 412);
            addButton.Name = "addButton";
            addButton.Size = new System.Drawing.Size(130, 30);
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
            publisherRulePanel.Location = new System.Drawing.Point(167, 447);
            publisherRulePanel.Name = "publisherRulePanel";
            publisherRulePanel.Size = new System.Drawing.Size(716, 190);
            publisherRulePanel.TabIndex = 9;
            // 
            // productTextBox
            // 
            productTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            productTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            productTextBox.Location = new System.Drawing.Point(109, 157);
            productTextBox.Name = "productTextBox";
            productTextBox.ReadOnly = true;
            productTextBox.Size = new System.Drawing.Size(420, 28);
            productTextBox.TabIndex = 20;
            // 
            // versionTextBox
            // 
            versionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            versionTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            versionTextBox.Location = new System.Drawing.Point(109, 118);
            versionTextBox.Name = "versionTextBox";
            versionTextBox.ReadOnly = true;
            versionTextBox.Size = new System.Drawing.Size(420, 28);
            versionTextBox.TabIndex = 19;
            // 
            // filenameTextBox
            // 
            filenameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            filenameTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            filenameTextBox.Location = new System.Drawing.Point(109, 79);
            filenameTextBox.Name = "filenameTextBox";
            filenameTextBox.ReadOnly = true;
            filenameTextBox.Size = new System.Drawing.Size(420, 28);
            filenameTextBox.TabIndex = 18;
            // 
            // publisherTextBox
            // 
            publisherTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            publisherTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            publisherTextBox.Location = new System.Drawing.Point(109, 40);
            publisherTextBox.Name = "publisherTextBox";
            publisherTextBox.ReadOnly = true;
            publisherTextBox.Size = new System.Drawing.Size(420, 28);
            publisherTextBox.TabIndex = 17;
            // 
            // issuerTextBox
            // 
            issuerTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            issuerTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            issuerTextBox.Location = new System.Drawing.Point(109, 1);
            issuerTextBox.Name = "issuerTextBox";
            issuerTextBox.ReadOnly = true;
            issuerTextBox.Size = new System.Drawing.Size(420, 28);
            issuerTextBox.TabIndex = 16;
            // 
            // productCheckBox
            // 
            productCheckBox.AutoCheck = false;
            productCheckBox.AutoSize = true;
            productCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            productCheckBox.Location = new System.Drawing.Point(4, 159);
            productCheckBox.Name = "productCheckBox";
            productCheckBox.Size = new System.Drawing.Size(94, 25);
            productCheckBox.TabIndex = 15;
            productCheckBox.Text = "Product:";
            productCheckBox.UseVisualStyleBackColor = true;
            // 
            // versionCheckBox
            // 
            versionCheckBox.AutoCheck = false;
            versionCheckBox.AutoSize = true;
            versionCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            versionCheckBox.Location = new System.Drawing.Point(4, 120);
            versionCheckBox.Name = "versionCheckBox";
            versionCheckBox.Size = new System.Drawing.Size(93, 25);
            versionCheckBox.TabIndex = 14;
            versionCheckBox.Text = "Version:";
            versionCheckBox.UseVisualStyleBackColor = true;
            // 
            // filenameCheckBox
            // 
            filenameCheckBox.AutoCheck = false;
            filenameCheckBox.AutoSize = true;
            filenameCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            filenameCheckBox.Location = new System.Drawing.Point(4, 81);
            filenameCheckBox.Name = "filenameCheckBox";
            filenameCheckBox.Size = new System.Drawing.Size(105, 25);
            filenameCheckBox.TabIndex = 13;
            filenameCheckBox.Text = "Filename:";
            filenameCheckBox.UseVisualStyleBackColor = true;
            // 
            // publisherCheckBox
            // 
            publisherCheckBox.AutoCheck = false;
            publisherCheckBox.AutoSize = true;
            publisherCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            publisherCheckBox.Location = new System.Drawing.Point(4, 42);
            publisherCheckBox.Name = "publisherCheckBox";
            publisherCheckBox.Size = new System.Drawing.Size(105, 25);
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
            issuerCheckBox.Location = new System.Drawing.Point(4, 3);
            issuerCheckBox.Name = "issuerCheckBox";
            issuerCheckBox.Size = new System.Drawing.Size(84, 25);
            issuerCheckBox.TabIndex = 10;
            issuerCheckBox.Text = "Issuer:";
            issuerCheckBox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label3.Location = new System.Drawing.Point(163, 415);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(90, 21);
            label3.TabIndex = 8;
            label3.Text = "Rule Type:";
            // 
            // ruleTypeComboBox
            // 
            ruleTypeComboBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            ruleTypeComboBox.FormattingEnabled = true;
            ruleTypeComboBox.Items.AddRange(new object[] { "Publisher", "Path", "File Attributes", "Packaged App", "File Hash" });
            ruleTypeComboBox.Location = new System.Drawing.Point(259, 412);
            ruleTypeComboBox.Name = "ruleTypeComboBox";
            ruleTypeComboBox.Size = new System.Drawing.Size(163, 29);
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
            fileAttributeRulePanel.Location = new System.Drawing.Point(906, 447);
            fileAttributeRulePanel.Name = "fileAttributeRulePanel";
            fileAttributeRulePanel.Size = new System.Drawing.Size(551, 190);
            fileAttributeRulePanel.TabIndex = 21;
            fileAttributeRulePanel.Visible = false;
            // 
            // pfnTextBox
            // 
            pfnTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            pfnTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            pfnTextBox.Location = new System.Drawing.Point(173, 157);
            pfnTextBox.Name = "pfnTextBox";
            pfnTextBox.ReadOnly = true;
            pfnTextBox.Size = new System.Drawing.Size(351, 28);
            pfnTextBox.TabIndex = 20;
            // 
            // intFileNameTextBox
            // 
            intFileNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            intFileNameTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            intFileNameTextBox.Location = new System.Drawing.Point(173, 118);
            intFileNameTextBox.Name = "intFileNameTextBox";
            intFileNameTextBox.ReadOnly = true;
            intFileNameTextBox.Size = new System.Drawing.Size(351, 28);
            intFileNameTextBox.TabIndex = 19;
            // 
            // prodNameTextBox
            // 
            prodNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            prodNameTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            prodNameTextBox.Location = new System.Drawing.Point(173, 79);
            prodNameTextBox.Name = "prodNameTextBox";
            prodNameTextBox.ReadOnly = true;
            prodNameTextBox.Size = new System.Drawing.Size(352, 28);
            prodNameTextBox.TabIndex = 18;
            // 
            // fileDescTextBox
            // 
            fileDescTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            fileDescTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            fileDescTextBox.Location = new System.Drawing.Point(173, 40);
            fileDescTextBox.Name = "fileDescTextBox";
            fileDescTextBox.ReadOnly = true;
            fileDescTextBox.Size = new System.Drawing.Size(351, 28);
            fileDescTextBox.TabIndex = 17;
            // 
            // origFileNameTextBox
            // 
            origFileNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            origFileNameTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            origFileNameTextBox.Location = new System.Drawing.Point(173, 1);
            origFileNameTextBox.Name = "origFileNameTextBox";
            origFileNameTextBox.ReadOnly = true;
            origFileNameTextBox.Size = new System.Drawing.Size(351, 28);
            origFileNameTextBox.TabIndex = 16;
            // 
            // pfnCheckBox
            // 
            pfnCheckBox.AutoCheck = false;
            pfnCheckBox.AutoSize = true;
            pfnCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            pfnCheckBox.Location = new System.Drawing.Point(4, 159);
            pfnCheckBox.Name = "pfnCheckBox";
            pfnCheckBox.Size = new System.Drawing.Size(147, 25);
            pfnCheckBox.TabIndex = 15;
            pfnCheckBox.Text = "Package Name:";
            pfnCheckBox.UseVisualStyleBackColor = true;
            // 
            // intFileNameCheckBox
            // 
            intFileNameCheckBox.AutoCheck = false;
            intFileNameCheckBox.AutoSize = true;
            intFileNameCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            intFileNameCheckBox.Location = new System.Drawing.Point(4, 120);
            intFileNameCheckBox.Name = "intFileNameCheckBox";
            intFileNameCheckBox.Size = new System.Drawing.Size(168, 25);
            intFileNameCheckBox.TabIndex = 14;
            intFileNameCheckBox.Text = "Internal Filename:";
            intFileNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // prodNameCheckBox
            // 
            prodNameCheckBox.AutoCheck = false;
            prodNameCheckBox.AutoSize = true;
            prodNameCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            prodNameCheckBox.Location = new System.Drawing.Point(4, 81);
            prodNameCheckBox.Name = "prodNameCheckBox";
            prodNameCheckBox.Size = new System.Drawing.Size(142, 25);
            prodNameCheckBox.TabIndex = 13;
            prodNameCheckBox.Text = "Product Name:";
            prodNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // fileDescCheckBox
            // 
            fileDescCheckBox.AutoCheck = false;
            fileDescCheckBox.AutoSize = true;
            fileDescCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            fileDescCheckBox.Location = new System.Drawing.Point(4, 42);
            fileDescCheckBox.Name = "fileDescCheckBox";
            fileDescCheckBox.Size = new System.Drawing.Size(153, 25);
            fileDescCheckBox.TabIndex = 12;
            fileDescCheckBox.Text = "File Description:";
            fileDescCheckBox.UseVisualStyleBackColor = true;
            // 
            // origFileNameCheckBox
            // 
            origFileNameCheckBox.AutoCheck = false;
            origFileNameCheckBox.AutoSize = true;
            origFileNameCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            origFileNameCheckBox.Location = new System.Drawing.Point(4, 3);
            origFileNameCheckBox.Name = "origFileNameCheckBox";
            origFileNameCheckBox.Size = new System.Drawing.Size(167, 25);
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
            eventsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { addedColumn, eventIdColumn, filenameColumn, productColumn, policyColumn, publisherColumn });
            eventsDataGridView.EnableHeadersVisualStyles = false;
            eventsDataGridView.Location = new System.Drawing.Point(167, 101);
            eventsDataGridView.Margin = new System.Windows.Forms.Padding(2);
            eventsDataGridView.MultiSelect = false;
            eventsDataGridView.Name = "eventsDataGridView";
            eventsDataGridView.ReadOnly = true;
            eventsDataGridView.RowHeadersVisible = false;
            eventsDataGridView.RowHeadersWidth = 70;
            eventsDataGridView.RowTemplate.Height = 24;
            eventsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            eventsDataGridView.Size = new System.Drawing.Size(896, 287);
            eventsDataGridView.TabIndex = 4;
            eventsDataGridView.VirtualMode = true;
            eventsDataGridView.CellClick += EventRowClick;
            eventsDataGridView.CellValueNeeded += EventsDataGrid_CellValueNeeded;
            eventsDataGridView.SelectionChanged += RowSelectionChanged;
            // 
            // addedColumn
            // 
            addedColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            addedColumn.HeaderText = "Added To Policy";
            addedColumn.MinimumWidth = 100;
            addedColumn.Name = "addedColumn";
            addedColumn.ReadOnly = true;
            addedColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            addedColumn.Width = 145;
            // 
            // eventIdColumn
            // 
            eventIdColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            eventIdColumn.HeaderText = "Event Id";
            eventIdColumn.MinimumWidth = 6;
            eventIdColumn.Name = "eventIdColumn";
            eventIdColumn.ReadOnly = true;
            eventIdColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            eventIdColumn.Width = 91;
            // 
            // filenameColumn
            // 
            filenameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            filenameColumn.HeaderText = "Filename";
            filenameColumn.MinimumWidth = 6;
            filenameColumn.Name = "filenameColumn";
            filenameColumn.ReadOnly = true;
            filenameColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            filenameColumn.Width = 98;
            // 
            // productColumn
            // 
            productColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            productColumn.HeaderText = "Product";
            productColumn.MinimumWidth = 6;
            productColumn.Name = "productColumn";
            productColumn.ReadOnly = true;
            productColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            productColumn.Width = 89;
            // 
            // policyColumn
            // 
            policyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            policyColumn.HeaderText = "Policy Name";
            policyColumn.MinimumWidth = 6;
            policyColumn.Name = "policyColumn";
            policyColumn.ReadOnly = true;
            policyColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            policyColumn.Width = 120;
            // 
            // publisherColumn
            // 
            publisherColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            publisherColumn.HeaderText = "Publisher";
            publisherColumn.MinimumWidth = 6;
            publisherColumn.Name = "publisherColumn";
            publisherColumn.ReadOnly = true;
            publisherColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            publisherColumn.Width = 98;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(162, 29);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(292, 29);
            label1.TabIndex = 6;
            label1.Text = "Configure Event Log Rules";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label2.Location = new System.Drawing.Point(164, 71);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(391, 18);
            label2.TabIndex = 7;
            label2.Text = "Create custom rules based on the WDAC event log events";
            // 
            // hashRulePanel
            // 
            hashRulePanel.Controls.Add(label5);
            hashRulePanel.Controls.Add(label4);
            hashRulePanel.Controls.Add(sha2TextBox);
            hashRulePanel.Controls.Add(sha1TextBox);
            hashRulePanel.Location = new System.Drawing.Point(765, 664);
            hashRulePanel.Name = "hashRulePanel";
            hashRulePanel.Size = new System.Drawing.Size(603, 93);
            hashRulePanel.TabIndex = 22;
            hashRulePanel.Visible = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label5.Location = new System.Drawing.Point(12, 55);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(140, 21);
            label5.TabIndex = 24;
            label5.Text = "SHA256 PE Hash:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label4.Location = new System.Drawing.Point(12, 18);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(122, 21);
            label4.TabIndex = 23;
            label4.Text = "SHA1 PE Hash:";
            // 
            // sha2TextBox
            // 
            sha2TextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            sha2TextBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            sha2TextBox.Location = new System.Drawing.Point(173, 52);
            sha2TextBox.Name = "sha2TextBox";
            sha2TextBox.ReadOnly = true;
            sha2TextBox.Size = new System.Drawing.Size(399, 26);
            sha2TextBox.TabIndex = 18;
            // 
            // sha1TextBox
            // 
            sha1TextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            sha1TextBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            sha1TextBox.Location = new System.Drawing.Point(173, 15);
            sha1TextBox.Name = "sha1TextBox";
            sha1TextBox.ReadOnly = true;
            sha1TextBox.Size = new System.Drawing.Size(397, 26);
            sha1TextBox.TabIndex = 16;
            // 
            // filePathRulePanel
            // 
            filePathRulePanel.Controls.Add(filePathTextBox);
            filePathRulePanel.Controls.Add(folderPathTextBox);
            filePathRulePanel.Controls.Add(folderPathCheckBox);
            filePathRulePanel.Controls.Add(filePathCheckBox);
            filePathRulePanel.Location = new System.Drawing.Point(16, 664);
            filePathRulePanel.Name = "filePathRulePanel";
            filePathRulePanel.Size = new System.Drawing.Size(743, 93);
            filePathRulePanel.TabIndex = 22;
            filePathRulePanel.Visible = false;
            // 
            // filePathTextBox
            // 
            filePathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            filePathTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            filePathTextBox.Location = new System.Drawing.Point(173, 16);
            filePathTextBox.Name = "filePathTextBox";
            filePathTextBox.ReadOnly = true;
            filePathTextBox.Size = new System.Drawing.Size(516, 28);
            filePathTextBox.TabIndex = 18;
            // 
            // folderPathTextBox
            // 
            folderPathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            folderPathTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            folderPathTextBox.Location = new System.Drawing.Point(173, 53);
            folderPathTextBox.Name = "folderPathTextBox";
            folderPathTextBox.ReadOnly = true;
            folderPathTextBox.Size = new System.Drawing.Size(516, 28);
            folderPathTextBox.TabIndex = 17;
            // 
            // folderPathCheckBox
            // 
            folderPathCheckBox.AutoSize = true;
            folderPathCheckBox.Checked = true;
            folderPathCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            folderPathCheckBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            folderPathCheckBox.Location = new System.Drawing.Point(3, 55);
            folderPathCheckBox.Name = "folderPathCheckBox";
            folderPathCheckBox.Size = new System.Drawing.Size(122, 25);
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
            filePathCheckBox.Location = new System.Drawing.Point(3, 18);
            filePathCheckBox.Name = "filePathCheckBox";
            filePathCheckBox.Size = new System.Drawing.Size(102, 25);
            filePathCheckBox.TabIndex = 10;
            filePathCheckBox.Text = "File Path:";
            filePathCheckBox.UseVisualStyleBackColor = true;
            // 
            // EventLogRuleConfiguration
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
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
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "EventLogRuleConfiguration";
            Size = new System.Drawing.Size(1208, 782);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn policyColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn publisherColumn;
    }
}
