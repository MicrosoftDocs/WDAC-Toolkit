
namespace WDAC_Wizard
{
    partial class EKUPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EKUPanel));
            this.headerLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ekuDataGridView = new System.Windows.Forms.DataGridView();
            this.Column_ToAdd = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column_EKUFriendlyName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_EKUValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button_CreateEKURule = new System.Windows.Forms.Button();
            this.labelError = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ekuDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // headerLabel
            // 
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Tahoma", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ForeColor = System.Drawing.Color.Black;
            this.headerLabel.Location = new System.Drawing.Point(12, 9);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(194, 24);
            this.headerLabel.TabIndex = 110;
            this.headerLabel.Text = "Custom Rule EKUs";
            this.headerLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(13, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(417, 18);
            this.label1.TabIndex = 111;
            this.label1.Text = "Select the EKUs you want to add to your custom publisher rule";
            // 
            // ekuDataGridView
            // 
            this.ekuDataGridView.AllowUserToDeleteRows = false;
            this.ekuDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.ekuDataGridView.BackgroundColor = System.Drawing.SystemColors.ControlLight;
            this.ekuDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ekuDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_ToAdd,
            this.Column_EKUFriendlyName,
            this.Column_EKUValue});
            this.ekuDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.ekuDataGridView.Location = new System.Drawing.Point(16, 88);
            this.ekuDataGridView.Margin = new System.Windows.Forms.Padding(2);
            this.ekuDataGridView.Name = "ekuDataGridView";
            this.ekuDataGridView.RowHeadersVisible = false;
            this.ekuDataGridView.RowHeadersWidth = 70;
            this.ekuDataGridView.RowTemplate.Height = 24;
            this.ekuDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ekuDataGridView.Size = new System.Drawing.Size(420, 196);
            this.ekuDataGridView.TabIndex = 113;
            this.ekuDataGridView.VirtualMode = true;
            this.ekuDataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.EKUDataGridViewCellClick);
            this.ekuDataGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.EKUDataGridViewCellValueChanged);
            this.ekuDataGridView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.EKUDataGridViewCellValueNeeded);
            // 
            // Column_ToAdd
            // 
            this.Column_ToAdd.HeaderText = "Enabled";
            this.Column_ToAdd.MinimumWidth = 6;
            this.Column_ToAdd.Name = "Column_ToAdd";
            this.Column_ToAdd.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Column_ToAdd.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Column_ToAdd.Width = 89;
            // 
            // Column_EKUFriendlyName
            // 
            this.Column_EKUFriendlyName.HeaderText = "Friendly Name";
            this.Column_EKUFriendlyName.MinimumWidth = 6;
            this.Column_EKUFriendlyName.Name = "Column_EKUFriendlyName";
            this.Column_EKUFriendlyName.Width = 128;
            // 
            // Column_EKUValue
            // 
            this.Column_EKUValue.HeaderText = "EKU Value";
            this.Column_EKUValue.MinimumWidth = 6;
            this.Column_EKUValue.Name = "Column_EKUValue";
            this.Column_EKUValue.Width = 105;
            // 
            // button_CreateEKURule
            // 
            this.button_CreateEKURule.BackColor = System.Drawing.Color.Transparent;
            this.button_CreateEKURule.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.button_CreateEKURule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_CreateEKURule.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_CreateEKURule.ForeColor = System.Drawing.Color.Black;
            this.button_CreateEKURule.Location = new System.Drawing.Point(326, 314);
            this.button_CreateEKURule.Margin = new System.Windows.Forms.Padding(2);
            this.button_CreateEKURule.Name = "button_CreateEKURule";
            this.button_CreateEKURule.Size = new System.Drawing.Size(110, 30);
            this.button_CreateEKURule.TabIndex = 114;
            this.button_CreateEKURule.Text = "Add EKUs";
            this.button_CreateEKURule.UseVisualStyleBackColor = false;
            this.button_CreateEKURule.Click += new System.EventHandler(this.ButtonCreateEKURules);
            // 
            // labelError
            // 
            this.labelError.AutoSize = true;
            this.labelError.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelError.ForeColor = System.Drawing.Color.Black;
            this.labelError.Location = new System.Drawing.Point(13, 289);
            this.labelError.Name = "labelError";
            this.labelError.Size = new System.Drawing.Size(148, 18);
            this.labelError.TabIndex = 115;
            this.labelError.Text = "My name is labelError";
            this.labelError.Visible = false;
            // 
            // EKUPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 355);
            this.Controls.Add(this.labelError);
            this.Controls.Add(this.button_CreateEKURule);
            this.Controls.Add(this.ekuDataGridView);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.headerLabel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EKUPanel";
            this.Text = "Custom Rules";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Load += new System.EventHandler(this.EKUPanelOnLoad);
            ((System.ComponentModel.ISupportInitialize)(this.ekuDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView ekuDataGridView;
        private System.Windows.Forms.Button button_CreateEKURule;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column_ToAdd;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_EKUFriendlyName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_EKUValue;
        private System.Windows.Forms.Label labelError;
    }
}