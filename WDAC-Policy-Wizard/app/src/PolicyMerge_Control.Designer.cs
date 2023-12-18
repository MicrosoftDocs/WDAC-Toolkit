namespace WDAC_Wizard.src
{
    partial class PolicyMerge_Control
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
            this.label3 = new System.Windows.Forms.Label();
            this.finalPolicyTextBox = new System.Windows.Forms.TextBox();
            this.button_Browse = new System.Windows.Forms.Button();
            this.policiesDataGrid = new System.Windows.Forms.DataGridView();
            this.Column_Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column_Path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button_AddPolicy = new System.Windows.Forms.Button();
            this.button_RemovePolicy = new System.Windows.Forms.Button();
            this.label_Error = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.policiesDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(165, 42);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(327, 29);
            this.label1.TabIndex = 1;
            this.label1.Text = "Merge Multiple WDAC Policies";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 10F);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(165, 87);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(262, 21);
            this.label2.TabIndex = 2;
            this.label2.Text = "Select at least 2 policies to merge";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 10F);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(165, 374);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 21);
            this.label3.TabIndex = 3;
            this.label3.Text = "Final policy location:";
            // 
            // finalPolicyTextBox
            // 
            this.finalPolicyTextBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.finalPolicyTextBox.Location = new System.Drawing.Point(165, 409);
            this.finalPolicyTextBox.Name = "finalPolicyTextBox";
            this.finalPolicyTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.finalPolicyTextBox.Size = new System.Drawing.Size(448, 26);
            this.finalPolicyTextBox.TabIndex = 4;
            this.finalPolicyTextBox.Click += new System.EventHandler(this.Button_Browse_Click);
            // 
            // button_Browse
            // 
            this.button_Browse.Font = new System.Drawing.Font("Tahoma", 9F);
            this.button_Browse.Location = new System.Drawing.Point(632, 408);
            this.button_Browse.Margin = new System.Windows.Forms.Padding(2);
            this.button_Browse.Name = "button_Browse";
            this.button_Browse.Size = new System.Drawing.Size(107, 28);
            this.button_Browse.TabIndex = 94;
            this.button_Browse.Text = "Browse";
            this.button_Browse.UseVisualStyleBackColor = true;
            this.button_Browse.Click += new System.EventHandler(this.Button_Browse_Click);
            // 
            // policiesDataGrid
            // 
            this.policiesDataGrid.AllowUserToDeleteRows = false;
            this.policiesDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.policiesDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column_Number,
            this.Column_Path});
            this.policiesDataGrid.EnableHeadersVisualStyles = false;
            this.policiesDataGrid.Location = new System.Drawing.Point(165, 139);
            this.policiesDataGrid.Name = "policiesDataGrid";
            this.policiesDataGrid.RowHeadersWidth = 51;
            this.policiesDataGrid.RowTemplate.Height = 24;
            this.policiesDataGrid.Size = new System.Drawing.Size(678, 150);
            this.policiesDataGrid.TabIndex = 95;
            this.policiesDataGrid.VirtualMode = true;
            this.policiesDataGrid.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.PoliciesDataGrid_CellValueNeeded);
            // 
            // Column_Number
            // 
            this.Column_Number.HeaderText = "Number";
            this.Column_Number.MinimumWidth = 6;
            this.Column_Number.Name = "Column_Number";
            this.Column_Number.Width = 75;
            // 
            // Column_Path
            // 
            this.Column_Path.HeaderText = "Policy Path";
            this.Column_Path.MinimumWidth = 6;
            this.Column_Path.Name = "Column_Path";
            this.Column_Path.Width = 550;
            // 
            // button_AddPolicy
            // 
            this.button_AddPolicy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_AddPolicy.Location = new System.Drawing.Point(165, 307);
            this.button_AddPolicy.Name = "button_AddPolicy";
            this.button_AddPolicy.Size = new System.Drawing.Size(130, 30);
            this.button_AddPolicy.TabIndex = 96;
            this.button_AddPolicy.Text = "+ Add Policy";
            this.button_AddPolicy.UseVisualStyleBackColor = true;
            this.button_AddPolicy.Click += new System.EventHandler(this.Button_AddPolicy_Click);
            // 
            // button_RemovePolicy
            // 
            this.button_RemovePolicy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_RemovePolicy.Location = new System.Drawing.Point(317, 307);
            this.button_RemovePolicy.Name = "button_RemovePolicy";
            this.button_RemovePolicy.Size = new System.Drawing.Size(130, 30);
            this.button_RemovePolicy.TabIndex = 97;
            this.button_RemovePolicy.Text = "- Remove Policy";
            this.button_RemovePolicy.UseVisualStyleBackColor = true;
            this.button_RemovePolicy.Click += new System.EventHandler(this.Button_RemovePolicy_Click);
            // 
            // label_Error
            // 
            this.label_Error.AutoSize = true;
            this.label_Error.BackColor = System.Drawing.Color.Transparent;
            this.label_Error.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_Error.ForeColor = System.Drawing.Color.Red;
            this.label_Error.Location = new System.Drawing.Point(166, 600);
            this.label_Error.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Error.Name = "label_Error";
            this.label_Error.Size = new System.Drawing.Size(648, 18);
            this.label_Error.TabIndex = 98;
            this.label_Error.Tag = "IgnoreDarkMode";
            this.label_Error.Text = "Label_Error: Lorem Ipsum text text text text. Lorum Ipsum text text text text tex" +
    "t text text text";
            this.label_Error.Visible = false;
            // 
            // PolicyMerge_Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.label_Error);
            this.Controls.Add(this.button_RemovePolicy);
            this.Controls.Add(this.button_AddPolicy);
            this.Controls.Add(this.policiesDataGrid);
            this.Controls.Add(this.button_Browse);
            this.Controls.Add(this.finalPolicyTextBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "PolicyMerge_Control";
            this.Size = new System.Drawing.Size(1172, 782);
            this.Validated += new System.EventHandler(this.PolicyMerge_Control_Validated);
            ((System.ComponentModel.ISupportInitialize)(this.policiesDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox finalPolicyTextBox;
        private System.Windows.Forms.Button button_Browse;
        private System.Windows.Forms.DataGridView policiesDataGrid;
        private System.Windows.Forms.Button button_AddPolicy;
        private System.Windows.Forms.Button button_RemovePolicy;
        private System.Windows.Forms.Label label_Error;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Path;
    }
}
