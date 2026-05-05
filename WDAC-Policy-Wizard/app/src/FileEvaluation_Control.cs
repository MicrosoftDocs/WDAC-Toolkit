// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WDAC_Wizard
{
    public partial class FileEvaluation_Control : UserControl
    {
        private MainWindow _MainWindow;
        private IFileEvaluator _fileEvaluator;
        private List<string> _filePaths;
        private BindingList<FileEvaluationRow> _evaluationResults;

        public FileEvaluation_Control(MainWindow pMainWindow)
            : this(pMainWindow, new FileEvaluator())
        {
        }

        public FileEvaluation_Control(MainWindow pMainWindow, IFileEvaluator fileEvaluator)
        {
            InitializeComponent();

            this._MainWindow = pMainWindow;
            this._fileEvaluator = fileEvaluator;
            this._filePaths = new List<string>();
            this._evaluationResults = new BindingList<FileEvaluationRow>();

            this._MainWindow.ErrorOnPage = false;
            this._MainWindow.RedoFlowRequired = false;

            SetupDataGrid();
            CheckApiAvailability();
        }

        private void SetupDataGrid()
        {
            this.resultsDataGridView.AutoGenerateColumns = false;
            this.resultsDataGridView.DataSource = this._evaluationResults;

            var filePathCol = new DataGridViewTextBoxColumn
            {
                HeaderText = "File Path",
                DataPropertyName = "FilePath",
                Width = 500,
                ReadOnly = true
            };

            var resultCol = new DataGridViewTextBoxColumn
            {
                HeaderText = "Result",
                DataPropertyName = "Result",
                Width = 250,
                ReadOnly = true
            };

            var statusCol = new DataGridViewTextBoxColumn
            {
                HeaderText = "Status",
                DataPropertyName = "Status",
                Width = 100,
                ReadOnly = true
            };

            this.resultsDataGridView.Columns.Add(filePathCol);
            this.resultsDataGridView.Columns.Add(resultCol);
            this.resultsDataGridView.Columns.Add(statusCol);

            this.resultsDataGridView.CellFormatting += ResultsDataGridView_CellFormatting;
        }

        private void CheckApiAvailability()
        {
            if (!this._fileEvaluator.IsApiAvailable())
            {
                this.label_Status.Text = "Warning: WldpCanExecuteFile API requires Windows 11 Build 22621+. " +
                    "Evaluation may not work on this system.";
                this.label_Status.ForeColor = Color.OrangeRed;
                this.label_Status.Visible = true;
            }
        }

        private void ResultsDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= this._evaluationResults.Count)
                return;

            var row = this._evaluationResults[e.RowIndex];

            if (e.ColumnIndex == 1 || e.ColumnIndex == 2) // Result or Status column
            {
                switch (row.Status)
                {
                    case "Allowed":
                        e.CellStyle.ForeColor = Color.DarkGreen;
                        e.CellStyle.Font = new Font(this.resultsDataGridView.Font, FontStyle.Bold);
                        break;
                    case "Blocked":
                        e.CellStyle.ForeColor = Color.Red;
                        e.CellStyle.Font = new Font(this.resultsDataGridView.Font, FontStyle.Bold);
                        break;
                    case "Audit":
                        e.CellStyle.ForeColor = Color.DarkOrange;
                        e.CellStyle.Font = new Font(this.resultsDataGridView.Font, FontStyle.Bold);
                        break;
                    case "Error":
                        e.CellStyle.ForeColor = Color.Gray;
                        break;
                }
            }
        }

        /// <summary>
        /// Add files button clicked - allows multi-select of files to evaluate
        /// </summary>
        private void Button_AddFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select files to evaluate against active WDAC policy";
                openFileDialog.Multiselect = true;
                openFileDialog.Filter = "All Files (*.*)|*.*|Executables (*.exe)|*.exe|DLLs (*.dll)|*.dll|" +
                    "Scripts (*.ps1;*.bat;*.cmd;*.vbs;*.js)|*.ps1;*.bat;*.cmd;*.vbs;*.js|" +
                    "MSI Packages (*.msi;*.msp)|*.msi;*.msp|Drivers (*.sys)|*.sys";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in openFileDialog.FileNames)
                    {
                        if (!this._filePaths.Contains(file))
                        {
                            this._filePaths.Add(file);
                            this._evaluationResults.Add(new FileEvaluationRow
                            {
                                FilePath = file,
                                Result = "Pending",
                                Status = "Pending"
                            });
                        }
                    }

                    UpdateFileCount();
                    this.button_Evaluate.Enabled = this._filePaths.Count > 0;
                }
            }
        }

        /// <summary>
        /// Remove selected files from the list
        /// </summary>
        private void Button_RemoveFiles_Click(object sender, EventArgs e)
        {
            if (this.resultsDataGridView.SelectedRows.Count > 0)
            {
                var rowsToRemove = new List<int>();
                foreach (DataGridViewRow row in this.resultsDataGridView.SelectedRows)
                {
                    rowsToRemove.Add(row.Index);
                }

                rowsToRemove.Sort();
                rowsToRemove.Reverse();

                foreach (int index in rowsToRemove)
                {
                    if (index < this._evaluationResults.Count)
                    {
                        this._filePaths.RemoveAt(index);
                        this._evaluationResults.RemoveAt(index);
                    }
                }

                UpdateFileCount();
                this.button_Evaluate.Enabled = this._filePaths.Count > 0;
            }
        }

        /// <summary>
        /// Evaluate all files against the active system WDAC policy
        /// </summary>
        private async void Button_Evaluate_Click(object sender, EventArgs e)
        {
            this.button_Evaluate.Enabled = false;
            this.button_AddFiles.Enabled = false;
            this.button_RemoveFiles.Enabled = false;
            this.progressBar_Evaluation.Visible = true;
            this.progressBar_Evaluation.Maximum = this._filePaths.Count;
            this.progressBar_Evaluation.Value = 0;
            this.label_Status.Text = "Evaluating files...";
            this.label_Status.ForeColor = Color.DodgerBlue;
            this.label_Status.Visible = true;

            int allowed = 0;
            int blocked = 0;
            int errors = 0;

            for (int i = 0; i < this._filePaths.Count; i++)
            {
                string filePath = this._filePaths[i];

                // Run evaluation off the UI thread
                FileEvaluationResult result = await Task.Run(() => this._fileEvaluator.EvaluateFile(filePath));

                // Update the grid row
                var row = this._evaluationResults[i];
                row.Result = result.GetResultDescription();

                if (!result.ApiCallSucceeded)
                {
                    row.Status = "Error";
                    errors++;
                }
                else if (result.IsAllowed)
                {
                    row.Status = "Allowed";
                    allowed++;
                }
                else
                {
                    row.Status = "Blocked";
                    blocked++;
                }

                this._evaluationResults.ResetItem(i);
                this.progressBar_Evaluation.Value = i + 1;
            }

            // Show summary
            this.label_Status.Text = $"Evaluation complete: {allowed} allowed, {blocked} blocked, {errors} errors " +
                $"(out of {this._filePaths.Count} files)";
            this.label_Status.ForeColor = blocked > 0 ? Color.Red : Color.DarkGreen;
            this.label_Status.Visible = true;

            this.button_Evaluate.Enabled = true;
            this.button_AddFiles.Enabled = true;
            this.button_RemoveFiles.Enabled = true;

            Logger.Log.AddInfoMsg($"File Evaluation completed: {allowed} allowed, {blocked} blocked, {errors} errors");
        }

        private void UpdateFileCount()
        {
            this.label_FileCount.Text = $"{this._filePaths.Count} file(s) selected";
        }
    }

    /// <summary>
    /// Data row for the evaluation results DataGridView.
    /// </summary>
    public class FileEvaluationRow
    {
        public string FilePath { get; set; }
        public string Result { get; set; }
        public string Status { get; set; }
    }
}
