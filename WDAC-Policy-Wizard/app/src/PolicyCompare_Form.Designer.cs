// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace WDAC_Wizard
{
    partial class PolicyCompare_Form
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            label_Title = new System.Windows.Forms.Label();
            label_Subtitle = new System.Windows.Forms.Label();
            policiesDataGrid = new System.Windows.Forms.DataGridView();
            Column_Index = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column_Path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column_Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            button_AddPolicy = new System.Windows.Forms.Button();
            button_RemovePolicy = new System.Windows.Forms.Button();
            button_Compare = new System.Windows.Forms.Button();
            button_Export = new System.Windows.Forms.Button();
            button_Close = new System.Windows.Forms.Button();
            checkBox_DifferencesOnly = new System.Windows.Forms.CheckBox();
            label_Section = new System.Windows.Forms.Label();
            comboBox_Section = new System.Windows.Forms.ComboBox();
            label_Filter = new System.Windows.Forms.Label();
            textBox_Filter = new System.Windows.Forms.TextBox();
            label_Status = new System.Windows.Forms.Label();
            progressBar = new System.Windows.Forms.ProgressBar();
            summaryFlow = new System.Windows.Forms.FlowLayoutPanel();
            resultsListView = new System.Windows.Forms.ListView();
            resultsContextMenu = new System.Windows.Forms.ContextMenuStrip(components);
            menuItem_CopyCell = new System.Windows.Forms.ToolStripMenuItem();
            menuItem_CopyRow = new System.Windows.Forms.ToolStripMenuItem();
            menuItem_CopyRowTsv = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)policiesDataGrid).BeginInit();
            resultsContextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // label_Title
            // 
            label_Title.AutoSize = true;
            label_Title.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label_Title.Location = new System.Drawing.Point(20, 18);
            label_Title.Name = "label_Title";
            label_Title.Size = new System.Drawing.Size(389, 29);
            label_Title.TabIndex = 0;
            label_Title.Text = "Compare App Control Policy Files";
            // 
            // label_Subtitle
            // 
            label_Subtitle.AutoSize = true;
            label_Subtitle.Font = new System.Drawing.Font("Tahoma", 10F);
            label_Subtitle.Location = new System.Drawing.Point(22, 55);
            label_Subtitle.Name = "label_Subtitle";
            label_Subtitle.Size = new System.Drawing.Size(636, 21);
            label_Subtitle.TabIndex = 1;
            label_Subtitle.Text = "Add 2 or more policy files (XML, CIP, or P7B) — drag-and-drop is supported. Click Compare to view differences.";
            // 
            // policiesDataGrid
            // 
            policiesDataGrid.AllowUserToAddRows = false;
            policiesDataGrid.AllowUserToDeleteRows = false;
            policiesDataGrid.AllowDrop = true;
            policiesDataGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            policiesDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            policiesDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Column_Index, Column_Path, Column_Status });
            policiesDataGrid.Location = new System.Drawing.Point(22, 95);
            policiesDataGrid.MultiSelect = true;
            policiesDataGrid.Name = "policiesDataGrid";
            policiesDataGrid.ReadOnly = true;
            policiesDataGrid.RowHeadersWidth = 30;
            policiesDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            policiesDataGrid.Size = new System.Drawing.Size(1100, 170);
            policiesDataGrid.TabIndex = 2;
            // 
            // Column_Index
            // 
            Column_Index.HeaderText = "#";
            Column_Index.MinimumWidth = 40;
            Column_Index.Name = "Column_Index";
            Column_Index.ReadOnly = true;
            Column_Index.Width = 50;
            // 
            // Column_Path
            // 
            Column_Path.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Column_Path.HeaderText = "Policy Path";
            Column_Path.MinimumWidth = 200;
            Column_Path.Name = "Column_Path";
            Column_Path.ReadOnly = true;
            // 
            // Column_Status
            // 
            Column_Status.HeaderText = "Status";
            Column_Status.MinimumWidth = 100;
            Column_Status.Name = "Column_Status";
            Column_Status.ReadOnly = true;
            Column_Status.Width = 220;
            // 
            // button_AddPolicy
            // 
            button_AddPolicy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            button_AddPolicy.Font = new System.Drawing.Font("Tahoma", 9F);
            button_AddPolicy.Location = new System.Drawing.Point(22, 280);
            button_AddPolicy.Name = "button_AddPolicy";
            button_AddPolicy.Size = new System.Drawing.Size(140, 32);
            button_AddPolicy.TabIndex = 3;
            button_AddPolicy.Text = "+ Add Policy";
            button_AddPolicy.UseVisualStyleBackColor = true;
            button_AddPolicy.Click += Button_AddPolicy_Click;
            // 
            // button_RemovePolicy
            // 
            button_RemovePolicy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            button_RemovePolicy.Font = new System.Drawing.Font("Tahoma", 9F);
            button_RemovePolicy.Location = new System.Drawing.Point(170, 280);
            button_RemovePolicy.Name = "button_RemovePolicy";
            button_RemovePolicy.Size = new System.Drawing.Size(140, 32);
            button_RemovePolicy.TabIndex = 4;
            button_RemovePolicy.Text = "- Remove Policy";
            button_RemovePolicy.UseVisualStyleBackColor = true;
            button_RemovePolicy.Click += Button_RemovePolicy_Click;
            // 
            // button_Export
            // 
            button_Export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            button_Export.Enabled = false;
            button_Export.Font = new System.Drawing.Font("Tahoma", 9F);
            button_Export.Location = new System.Drawing.Point(835, 280);
            button_Export.Name = "button_Export";
            button_Export.Size = new System.Drawing.Size(140, 32);
            button_Export.TabIndex = 5;
            button_Export.Text = "Export Report…";
            button_Export.UseVisualStyleBackColor = true;
            button_Export.Click += Button_Export_Click;
            // 
            // button_Compare
            // 
            button_Compare.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            button_Compare.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            button_Compare.Location = new System.Drawing.Point(982, 280);
            button_Compare.Name = "button_Compare";
            button_Compare.Size = new System.Drawing.Size(140, 32);
            button_Compare.TabIndex = 6;
            button_Compare.Text = "Compare";
            button_Compare.UseVisualStyleBackColor = true;
            button_Compare.Click += Button_Compare_Click;
            // 
            // button_Close
            // 
            button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            button_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            button_Close.Font = new System.Drawing.Font("Tahoma", 9F);
            button_Close.Location = new System.Drawing.Point(982, 670);
            button_Close.Name = "button_Close";
            button_Close.Size = new System.Drawing.Size(140, 32);
            button_Close.TabIndex = 99;
            button_Close.Text = "Close";
            button_Close.UseVisualStyleBackColor = true;
            button_Close.Click += Button_Close_Click;
            // 
            // checkBox_DifferencesOnly
            // 
            checkBox_DifferencesOnly.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            checkBox_DifferencesOnly.AutoSize = true;
            checkBox_DifferencesOnly.Checked = true;
            checkBox_DifferencesOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            checkBox_DifferencesOnly.Font = new System.Drawing.Font("Tahoma", 9F);
            checkBox_DifferencesOnly.Location = new System.Drawing.Point(22, 333);
            checkBox_DifferencesOnly.Name = "checkBox_DifferencesOnly";
            checkBox_DifferencesOnly.Size = new System.Drawing.Size(159, 22);
            checkBox_DifferencesOnly.TabIndex = 7;
            checkBox_DifferencesOnly.Text = "Show differences only";
            checkBox_DifferencesOnly.UseVisualStyleBackColor = true;
            checkBox_DifferencesOnly.CheckedChanged += CheckBox_DifferencesOnly_CheckedChanged;
            // 
            // label_Section
            // 
            label_Section.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            label_Section.AutoSize = true;
            label_Section.Font = new System.Drawing.Font("Tahoma", 9F);
            label_Section.Location = new System.Drawing.Point(195, 335);
            label_Section.Name = "label_Section";
            label_Section.Size = new System.Drawing.Size(58, 18);
            label_Section.TabIndex = 8;
            label_Section.Text = "Section:";
            // 
            // comboBox_Section
            // 
            comboBox_Section.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            comboBox_Section.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboBox_Section.Font = new System.Drawing.Font("Tahoma", 9F);
            comboBox_Section.Location = new System.Drawing.Point(258, 332);
            comboBox_Section.Name = "comboBox_Section";
            comboBox_Section.Size = new System.Drawing.Size(200, 26);
            comboBox_Section.TabIndex = 9;
            comboBox_Section.SelectedIndexChanged += ComboBox_Section_SelectedIndexChanged;
            // 
            // label_Filter
            // 
            label_Filter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            label_Filter.AutoSize = true;
            label_Filter.Font = new System.Drawing.Font("Tahoma", 9F);
            label_Filter.Location = new System.Drawing.Point(475, 335);
            label_Filter.Name = "label_Filter";
            label_Filter.Size = new System.Drawing.Size(46, 18);
            label_Filter.TabIndex = 10;
            label_Filter.Text = "Filter:";
            // 
            // textBox_Filter
            // 
            textBox_Filter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            textBox_Filter.Font = new System.Drawing.Font("Tahoma", 9F);
            textBox_Filter.Location = new System.Drawing.Point(525, 332);
            textBox_Filter.Name = "textBox_Filter";
            textBox_Filter.PlaceholderText = "Type to filter visible rows…";
            textBox_Filter.Size = new System.Drawing.Size(597, 26);
            textBox_Filter.TabIndex = 11;
            textBox_Filter.TextChanged += TextBox_Filter_TextChanged;
            // 
            // summaryFlow
            // 
            summaryFlow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            summaryFlow.AutoScroll = true;
            summaryFlow.BackColor = System.Drawing.Color.Transparent;
            summaryFlow.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            summaryFlow.Location = new System.Drawing.Point(22, 365);
            summaryFlow.Name = "summaryFlow";
            summaryFlow.Size = new System.Drawing.Size(1100, 38);
            summaryFlow.TabIndex = 12;
            summaryFlow.WrapContents = false;
            // 
            // label_Status
            // 
            label_Status.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            label_Status.AutoEllipsis = true;
            label_Status.Font = new System.Drawing.Font("Tahoma", 9F);
            label_Status.ForeColor = System.Drawing.Color.DodgerBlue;
            label_Status.Location = new System.Drawing.Point(22, 678);
            label_Status.Name = "label_Status";
            label_Status.Size = new System.Drawing.Size(640, 18);
            label_Status.TabIndex = 13;
            // 
            // progressBar
            // 
            progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            progressBar.Location = new System.Drawing.Point(670, 675);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(160, 20);
            progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 30;
            progressBar.TabIndex = 14;
            progressBar.Visible = false;
            // 
            // resultsListView
            // 
            resultsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            resultsListView.ContextMenuStrip = resultsContextMenu;
            resultsListView.FullRowSelect = true;
            resultsListView.GridLines = true;
            resultsListView.Location = new System.Drawing.Point(22, 410);
            resultsListView.MultiSelect = false;
            resultsListView.Name = "resultsListView";
            resultsListView.Size = new System.Drawing.Size(1100, 250);
            resultsListView.TabIndex = 15;
            resultsListView.UseCompatibleStateImageBehavior = false;
            resultsListView.View = System.Windows.Forms.View.Details;
            resultsListView.OwnerDraw = false;
            // 
            // resultsContextMenu
            // 
            resultsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                menuItem_CopyCell,
                menuItem_CopyRow,
                menuItem_CopyRowTsv });
            resultsContextMenu.Name = "resultsContextMenu";
            resultsContextMenu.Size = new System.Drawing.Size(180, 70);
            // 
            // menuItem_CopyCell
            // 
            menuItem_CopyCell.Name = "menuItem_CopyCell";
            menuItem_CopyCell.Size = new System.Drawing.Size(180, 22);
            menuItem_CopyCell.Text = "Copy cell";
            menuItem_CopyCell.Click += MenuItem_CopyCell_Click;
            // 
            // menuItem_CopyRow
            // 
            menuItem_CopyRow.Name = "menuItem_CopyRow";
            menuItem_CopyRow.Size = new System.Drawing.Size(180, 22);
            menuItem_CopyRow.Text = "Copy row";
            menuItem_CopyRow.Click += MenuItem_CopyRow_Click;
            // 
            // menuItem_CopyRowTsv
            // 
            menuItem_CopyRowTsv.Name = "menuItem_CopyRowTsv";
            menuItem_CopyRowTsv.Size = new System.Drawing.Size(180, 22);
            menuItem_CopyRowTsv.Text = "Copy row as TSV";
            menuItem_CopyRowTsv.Click += MenuItem_CopyRowTsv_Click;
            // 
            // PolicyCompare_Form
            // 
            AllowDrop = true;
            AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            ClientSize = new System.Drawing.Size(1144, 710);
            Controls.Add(progressBar);
            Controls.Add(resultsListView);
            Controls.Add(summaryFlow);
            Controls.Add(textBox_Filter);
            Controls.Add(label_Filter);
            Controls.Add(comboBox_Section);
            Controls.Add(label_Section);
            Controls.Add(label_Status);
            Controls.Add(checkBox_DifferencesOnly);
            Controls.Add(button_Close);
            Controls.Add(button_Compare);
            Controls.Add(button_Export);
            Controls.Add(button_RemovePolicy);
            Controls.Add(button_AddPolicy);
            Controls.Add(policiesDataGrid);
            Controls.Add(label_Subtitle);
            Controls.Add(label_Title);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            MinimumSize = new System.Drawing.Size(960, 640);
            Name = "PolicyCompare_Form";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Compare App Control Policies";
            Load += PolicyCompare_Form_Load;
            ((System.ComponentModel.ISupportInitialize)policiesDataGrid).EndInit();
            resultsContextMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label_Title;
        private System.Windows.Forms.Label label_Subtitle;
        private System.Windows.Forms.DataGridView policiesDataGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Index;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Path;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column_Status;
        private System.Windows.Forms.Button button_AddPolicy;
        private System.Windows.Forms.Button button_RemovePolicy;
        private System.Windows.Forms.Button button_Compare;
        private System.Windows.Forms.Button button_Export;
        private System.Windows.Forms.Button button_Close;
        private System.Windows.Forms.CheckBox checkBox_DifferencesOnly;
        private System.Windows.Forms.Label label_Section;
        private System.Windows.Forms.ComboBox comboBox_Section;
        private System.Windows.Forms.Label label_Filter;
        private System.Windows.Forms.TextBox textBox_Filter;
        private System.Windows.Forms.Label label_Status;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.FlowLayoutPanel summaryFlow;
        private System.Windows.Forms.ListView resultsListView;
        private System.Windows.Forms.ContextMenuStrip resultsContextMenu;
        private System.Windows.Forms.ToolStripMenuItem menuItem_CopyCell;
        private System.Windows.Forms.ToolStripMenuItem menuItem_CopyRow;
        private System.Windows.Forms.ToolStripMenuItem menuItem_CopyRowTsv;
    }
}
