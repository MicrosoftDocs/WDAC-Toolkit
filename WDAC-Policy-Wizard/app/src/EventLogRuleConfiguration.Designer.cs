
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
            this.button1 = new System.Windows.Forms.Button();
            this.customRulePanel = new System.Windows.Forms.Panel();
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
            this.eventsDataGridView = new System.Windows.Forms.DataGridView();
            this.addedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eventIdColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filenameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.productColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.policyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.publisherColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.customRulePanel.SuspendLayout();
            this.publisherRulePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(177, 677);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 30);
            this.button1.TabIndex = 0;
            this.button1.Text = "Add -->";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // customRulePanel
            // 
            this.customRulePanel.Controls.Add(this.publisherRulePanel);
            this.customRulePanel.Controls.Add(this.label3);
            this.customRulePanel.Controls.Add(this.ruleTypeComboBox);
            this.customRulePanel.Location = new System.Drawing.Point(167, 429);
            this.customRulePanel.Name = "customRulePanel";
            this.customRulePanel.Size = new System.Drawing.Size(669, 242);
            this.customRulePanel.TabIndex = 1;
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
            this.publisherRulePanel.Location = new System.Drawing.Point(6, 49);
            this.publisherRulePanel.Name = "publisherRulePanel";
            this.publisherRulePanel.Size = new System.Drawing.Size(493, 190);
            this.publisherRulePanel.TabIndex = 9;
            // 
            // productTextBox
            // 
            this.productTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.productTextBox.Location = new System.Drawing.Point(102, 159);
            this.productTextBox.Name = "productTextBox";
            this.productTextBox.Size = new System.Drawing.Size(351, 28);
            this.productTextBox.TabIndex = 20;
            // 
            // versionTextBox
            // 
            this.versionTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionTextBox.Location = new System.Drawing.Point(102, 120);
            this.versionTextBox.Name = "versionTextBox";
            this.versionTextBox.Size = new System.Drawing.Size(351, 28);
            this.versionTextBox.TabIndex = 19;
            // 
            // filenameTextBox
            // 
            this.filenameTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.filenameTextBox.Location = new System.Drawing.Point(101, 81);
            this.filenameTextBox.Name = "filenameTextBox";
            this.filenameTextBox.Size = new System.Drawing.Size(352, 28);
            this.filenameTextBox.TabIndex = 18;
            // 
            // publisherTextBox
            // 
            this.publisherTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.publisherTextBox.Location = new System.Drawing.Point(103, 42);
            this.publisherTextBox.Name = "publisherTextBox";
            this.publisherTextBox.Size = new System.Drawing.Size(351, 28);
            this.publisherTextBox.TabIndex = 17;
            // 
            // issuerTextBox
            // 
            this.issuerTextBox.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.issuerTextBox.Location = new System.Drawing.Point(103, 3);
            this.issuerTextBox.Name = "issuerTextBox";
            this.issuerTextBox.Size = new System.Drawing.Size(351, 28);
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
            this.label3.Location = new System.Drawing.Point(3, 17);
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
            this.ruleTypeComboBox.Location = new System.Drawing.Point(99, 14);
            this.ruleTypeComboBox.Name = "ruleTypeComboBox";
            this.ruleTypeComboBox.Size = new System.Drawing.Size(163, 29);
            this.ruleTypeComboBox.TabIndex = 3;
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
            this.eventsDataGridView.Location = new System.Drawing.Point(167, 123);
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
            // EventLogRuleConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.eventsDataGridView);
            this.Controls.Add(this.customRulePanel);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "EventLogRuleConfiguration";
            this.Size = new System.Drawing.Size(1208, 782);
            this.Load += new System.EventHandler(this.EventLogRuleConfiguration_Load);
            this.customRulePanel.ResumeLayout(false);
            this.customRulePanel.PerformLayout();
            this.publisherRulePanel.ResumeLayout(false);
            this.publisherRulePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.eventsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel customRulePanel;
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
    }
}
