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
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            finalPolicyTextBox = new System.Windows.Forms.TextBox();
            button_Browse = new System.Windows.Forms.Button();
            policiesDataGrid = new System.Windows.Forms.DataGridView();
            Column_Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column_Path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            button_AddPolicy = new System.Windows.Forms.Button();
            button_RemovePolicy = new System.Windows.Forms.Button();
            label_Error = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)policiesDataGrid).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label1.ForeColor = System.Drawing.Color.Black;
            label1.Location = new System.Drawing.Point(165, 42);
            label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(521, 29);
            label1.TabIndex = 1;
            label1.Text = "Merge Multiple App Control for Business Policies";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Tahoma", 10F);
            label2.ForeColor = System.Drawing.Color.Black;
            label2.Location = new System.Drawing.Point(165, 87);
            label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(262, 21);
            label2.TabIndex = 2;
            label2.Text = "Select at least 2 policies to merge";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Tahoma", 10F);
            label3.ForeColor = System.Drawing.Color.Black;
            label3.Location = new System.Drawing.Point(165, 374);
            label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(161, 21);
            label3.TabIndex = 3;
            label3.Text = "Final policy location:";
            // 
            // finalPolicyTextBox
            // 
            finalPolicyTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            finalPolicyTextBox.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            finalPolicyTextBox.Location = new System.Drawing.Point(165, 409);
            finalPolicyTextBox.Name = "finalPolicyTextBox";
            finalPolicyTextBox.Size = new System.Drawing.Size(448, 26);
            finalPolicyTextBox.TabIndex = 4;
            finalPolicyTextBox.Click += Button_Browse_Click;
            // 
            // button_Browse
            // 
            button_Browse.Font = new System.Drawing.Font("Tahoma", 9F);
            button_Browse.Location = new System.Drawing.Point(632, 408);
            button_Browse.Margin = new System.Windows.Forms.Padding(2);
            button_Browse.Name = "button_Browse";
            button_Browse.Size = new System.Drawing.Size(107, 28);
            button_Browse.TabIndex = 94;
            button_Browse.Text = "Browse";
            button_Browse.UseVisualStyleBackColor = true;
            button_Browse.Click += Button_Browse_Click;
            // 
            // policiesDataGrid
            // 
            policiesDataGrid.AllowUserToDeleteRows = false;
            policiesDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            policiesDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Column_Number, Column_Path });
            policiesDataGrid.EnableHeadersVisualStyles = false;
            policiesDataGrid.Location = new System.Drawing.Point(165, 139);
            policiesDataGrid.Name = "policiesDataGrid";
            policiesDataGrid.RowHeadersWidth = 51;
            policiesDataGrid.RowTemplate.Height = 24;
            policiesDataGrid.Size = new System.Drawing.Size(678, 150);
            policiesDataGrid.TabIndex = 95;
            policiesDataGrid.VirtualMode = true;
            policiesDataGrid.CellValueNeeded += PoliciesDataGrid_CellValueNeeded;
            // 
            // Column_Number
            // 
            Column_Number.HeaderText = "Number";
            Column_Number.MinimumWidth = 6;
            Column_Number.Name = "Column_Number";
            Column_Number.Width = 75;
            // 
            // Column_Path
            // 
            Column_Path.HeaderText = "Policy Path";
            Column_Path.MinimumWidth = 6;
            Column_Path.Name = "Column_Path";
            Column_Path.Width = 550;
            // 
            // button_AddPolicy
            // 
            button_AddPolicy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            button_AddPolicy.Location = new System.Drawing.Point(165, 307);
            button_AddPolicy.Name = "button_AddPolicy";
            button_AddPolicy.Size = new System.Drawing.Size(130, 30);
            button_AddPolicy.TabIndex = 96;
            button_AddPolicy.Text = "+ Add Policy";
            button_AddPolicy.UseVisualStyleBackColor = true;
            button_AddPolicy.Click += Button_AddPolicy_Click;
            // 
            // button_RemovePolicy
            // 
            button_RemovePolicy.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            button_RemovePolicy.Location = new System.Drawing.Point(317, 307);
            button_RemovePolicy.Name = "button_RemovePolicy";
            button_RemovePolicy.Size = new System.Drawing.Size(130, 30);
            button_RemovePolicy.TabIndex = 97;
            button_RemovePolicy.Text = "- Remove Policy";
            button_RemovePolicy.UseVisualStyleBackColor = true;
            button_RemovePolicy.Click += Button_RemovePolicy_Click;
            // 
            // label_Error
            // 
            label_Error.AutoSize = true;
            label_Error.BackColor = System.Drawing.Color.Transparent;
            label_Error.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label_Error.ForeColor = System.Drawing.Color.Red;
            label_Error.Location = new System.Drawing.Point(166, 600);
            label_Error.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            label_Error.Name = "label_Error";
            label_Error.Size = new System.Drawing.Size(648, 18);
            label_Error.TabIndex = 98;
            label_Error.Tag = "IgnoreDarkMode";
            label_Error.Text = "Label_Error: Lorem Ipsum text text text text. Lorum Ipsum text text text text text text text text";
            label_Error.Visible = false;
            // 
            // PolicyMerge_Control
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            Controls.Add(label_Error);
            Controls.Add(button_RemovePolicy);
            Controls.Add(button_AddPolicy);
            Controls.Add(policiesDataGrid);
            Controls.Add(button_Browse);
            Controls.Add(finalPolicyTextBox);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "PolicyMerge_Control";
            Size = new System.Drawing.Size(1172, 782);
            Validated += PolicyMerge_Control_Validated;
            ((System.ComponentModel.ISupportInitialize)policiesDataGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
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
