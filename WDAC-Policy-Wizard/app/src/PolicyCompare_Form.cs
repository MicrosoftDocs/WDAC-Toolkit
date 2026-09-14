// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WDAC_Wizard
{
    /// <summary>
    /// Standalone window that lets the user select 2 or more policy files (XML or binary) and view a
    /// side-by-side comparison of their settings, rule options, file rules, signers, etc.
    /// </summary>
    public partial class PolicyCompare_Form : Form
    {
        private const string AllSectionsLabel = "All sections";

        private readonly List<string> _policyPaths;
        private PolicyComparer.ComparisonResult _lastResult;
        private BackgroundWorker _compareWorker;

        public PolicyCompare_Form()
        {
            InitializeComponent();
            _policyPaths = new List<string>();
        }

        // -----------------------------------------------------------------------
        // Lifecycle
        // -----------------------------------------------------------------------

        private void PolicyCompare_Form_Load(object sender, EventArgs e)
        {
            ApplyTheme();

            resultsListView.SizeChanged += ResultsListView_SizeChanged;

            // Drag-and-drop on the form and on the policies grid
            DragEnter += PolicyCompare_Form_DragEnter;
            DragDrop += PolicyCompare_Form_DragDrop;
            policiesDataGrid.DragEnter += PolicyCompare_Form_DragEnter;
            policiesDataGrid.DragDrop += PolicyCompare_Form_DragDrop;

            // Section combo: start with "All sections"; populated more after a comparison runs
            ResetSectionCombo();

            UpdateButtonStates();
        }

        // -----------------------------------------------------------------------
        // Drag-and-drop
        // -----------------------------------------------------------------------

        private void PolicyCompare_Form_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void PolicyCompare_Form_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data == null || !e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            string[] paths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (paths == null) return;

            int added = 0;
            int skipped = 0;
            foreach (var p in paths)
            {
                if (TryAddPolicyPath(p)) added++; else skipped++;
            }

            if (added > 0 || skipped > 0)
            {
                SetStatus(string.Format("Drag-and-drop: {0} added, {1} skipped.", added, skipped),
                          added > 0 ? Color.SeaGreen : Color.OrangeRed);
            }
            UpdateButtonStates();
        }

        // -----------------------------------------------------------------------
        // Add / Remove policies
        // -----------------------------------------------------------------------

        private void Button_AddPolicy_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Choose App Control policies to compare";
                openFileDialog.Filter = "App Control Policy Files (*.xml; *.cip; *.p7b)|*.xml;*.cip;*.p7b|" +
                                        "XML Policy Files (*.xml)|*.xml|" +
                                        "Binary Policy Files (*.cip; *.p7b)|*.cip;*.p7b|" +
                                        "All Files (*.*)|*.*";
                openFileDialog.Multiselect = true;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.CheckPathExists = true;

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                foreach (string path in openFileDialog.FileNames)
                {
                    TryAddPolicyPath(path);
                }
            }

            UpdateButtonStates();
        }

        /// <summary>
        /// Adds a single policy path to the list (if valid and not already present). Returns true
        /// if the path was added, false if it was rejected (duplicate, missing, unsupported).
        /// </summary>
        private bool TryAddPolicyPath(string path)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return false;
            }

            string ext = Path.GetExtension(path).ToLowerInvariant();
            if (ext != ".xml" && ext != ".cip" && ext != ".p7b")
            {
                SetStatus(string.Format("Skipped {0}: unsupported extension.", Path.GetFileName(path)),
                          Color.OrangeRed);
                return false;
            }

            if (_policyPaths.Any(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase)))
            {
                SetStatus(string.Format("{0} is already added.", Path.GetFileName(path)), Color.OrangeRed);
                return false;
            }

            _policyPaths.Add(path);
            policiesDataGrid.Rows.Add(_policyPaths.Count.ToString(), path, "Pending compare");
            Logger.Log?.AddInfoMsg("PolicyCompare: added policy to compare list: " + path);
            return true;
        }

        private void Button_RemovePolicy_Click(object sender, EventArgs e)
        {
            if (policiesDataGrid.SelectedRows.Count == 0)
            {
                SetStatus("Select a policy row to remove.", Color.OrangeRed);
                return;
            }

            var rowsToRemove = policiesDataGrid.SelectedRows
                                                .Cast<DataGridViewRow>()
                                                .OrderByDescending(r => r.Index)
                                                .ToList();

            foreach (var row in rowsToRemove)
            {
                if (row.Index >= 0 && row.Index < _policyPaths.Count)
                {
                    _policyPaths.RemoveAt(row.Index);
                }
                policiesDataGrid.Rows.RemoveAt(row.Index);
            }

            for (int i = 0; i < policiesDataGrid.Rows.Count; i++)
            {
                policiesDataGrid.Rows[i].Cells[Column_Index.Index].Value = (i + 1).ToString();
            }

            UpdateButtonStates();
        }

        // -----------------------------------------------------------------------
        // Compare (background)
        // -----------------------------------------------------------------------

        private void Button_Compare_Click(object sender, EventArgs e)
        {
            if (_policyPaths.Count < 2)
            {
                SetStatus("Add at least two policies to compare.", Color.OrangeRed);
                return;
            }

            if (_compareWorker != null && _compareWorker.IsBusy)
            {
                SetStatus("Comparison already in progress…", Color.DodgerBlue);
                return;
            }

            // Reset prior status text on the grid
            for (int i = 0; i < policiesDataGrid.Rows.Count; i++)
            {
                policiesDataGrid.Rows[i].Cells[Column_Status.Index].Value = "Loading…";
                policiesDataGrid.Rows[i].Cells[Column_Status.Index].Style.ForeColor = Color.DodgerBlue;
            }

            // Disable interactive elements during the load
            SetCompareControlsEnabled(false);
            progressBar.Visible = true;
            SetStatus("Loading policies…", Color.DodgerBlue);
            Logger.Log?.AddNewSeparationLine("Workflow -- Compare Policies");

            // Snapshot paths to avoid concurrent modification
            var pathsSnapshot = new List<string>(_policyPaths);

            _compareWorker = new BackgroundWorker { WorkerReportsProgress = true };
            _compareWorker.DoWork += CompareWorker_DoWork;
            _compareWorker.ProgressChanged += CompareWorker_ProgressChanged;
            _compareWorker.RunWorkerCompleted += CompareWorker_RunWorkerCompleted;
            _compareWorker.RunWorkerAsync(pathsSnapshot);
        }

        private void CompareWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var paths = (List<string>)e.Argument;
            var loadedPolicies = new List<PolicyComparer.LoadedPolicy>();
            var worker = (BackgroundWorker)sender;

            for (int i = 0; i < paths.Count; i++)
            {
                var loaded = PolicyComparer.LoadPolicy(paths[i]);
                loadedPolicies.Add(loaded);

                int percent = (int)((i + 1) / (double)paths.Count * 100.0);
                worker.ReportProgress(percent, new LoadProgress { Index = i, Loaded = loaded });
            }

            var result = PolicyComparer.Compare(loadedPolicies);
            e.Result = result;
        }

        private class LoadProgress
        {
            public int Index;
            public PolicyComparer.LoadedPolicy Loaded;
        }

        private void CompareWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var p = e.UserState as LoadProgress;
            if (p == null) return;

            string statusText = p.Loaded.Policy != null
                ? "Loaded"
                : ("Error: " + (p.Loaded.LoadError ?? "Unknown"));

            if (p.Index < policiesDataGrid.Rows.Count)
            {
                policiesDataGrid.Rows[p.Index].Cells[Column_Status.Index].Value = statusText;
                policiesDataGrid.Rows[p.Index].Cells[Column_Status.Index].Style.ForeColor =
                    p.Loaded.Policy != null ? Color.SeaGreen : Color.Firebrick;
            }
        }

        private void CompareWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Visible = false;

            try
            {
                if (e.Error != null)
                {
                    Logger.Log?.AddErrorMsg("PolicyCompare: comparison failed", e.Error);
                    SetStatus("Comparison failed: " + e.Error.Message, Color.Firebrick);
                    _lastResult = null;
                    RenderComparison(null);
                    return;
                }

                var result = e.Result as PolicyComparer.ComparisonResult;
                int successfullyLoaded = result?.Policies.Count(lp => lp.Policy != null) ?? 0;
                if (result == null || successfullyLoaded < 2)
                {
                    SetStatus("Need at least two policies that load successfully to compare.", Color.Firebrick);
                    _lastResult = null;
                    RenderComparison(null);
                    return;
                }

                _lastResult = result;
                RebuildSectionCombo();
                RenderComparison(_lastResult);
                BuildSummaryStrip(_lastResult);

                int diffCount = _lastResult.Entries.Count(en => en.IsDifferent);
                SetStatus(string.Format("Compared {0} policies. {1} differences found.",
                                        successfullyLoaded, diffCount),
                          diffCount == 0 ? Color.SeaGreen : Color.DodgerBlue);
            }
            finally
            {
                _compareWorker = null;
                // Re-enable controls AFTER _lastResult has been assigned, so the Export button
                // correctly reflects whether a comparison result is available.
                SetCompareControlsEnabled(true);
            }
        }

        private void SetCompareControlsEnabled(bool enabled)
        {
            button_Compare.Enabled = enabled && _policyPaths.Count >= 2;
            button_AddPolicy.Enabled = enabled;
            button_RemovePolicy.Enabled = enabled && _policyPaths.Count > 0;
            button_Export.Enabled = enabled && _lastResult != null
                                    && _lastResult.Entries != null
                                    && _lastResult.Entries.Count > 0;
        }

        // -----------------------------------------------------------------------
        // Section combo / Filter / Differences-only
        // -----------------------------------------------------------------------

        private void ResetSectionCombo()
        {
            comboBox_Section.BeginUpdate();
            try
            {
                comboBox_Section.Items.Clear();
                comboBox_Section.Items.Add(AllSectionsLabel);
                comboBox_Section.SelectedIndex = 0;
            }
            finally
            {
                comboBox_Section.EndUpdate();
            }
        }

        private void RebuildSectionCombo()
        {
            if (_lastResult == null)
            {
                ResetSectionCombo();
                return;
            }

            string previous = comboBox_Section.SelectedItem as string;

            comboBox_Section.BeginUpdate();
            try
            {
                comboBox_Section.Items.Clear();
                comboBox_Section.Items.Add(AllSectionsLabel);

                var sections = _lastResult.Entries
                                          .Select(en => en.Section)
                                          .Distinct(StringComparer.OrdinalIgnoreCase)
                                          .OrderBy(PolicyComparer.OrderOf)
                                          .ThenBy(s => s, StringComparer.OrdinalIgnoreCase)
                                          .ToArray();
                foreach (var s in sections) comboBox_Section.Items.Add(s);

                int idx = 0;
                if (!string.IsNullOrEmpty(previous))
                {
                    int found = comboBox_Section.Items.IndexOf(previous);
                    if (found >= 0) idx = found;
                }
                comboBox_Section.SelectedIndex = idx;
            }
            finally
            {
                comboBox_Section.EndUpdate();
            }
        }

        private void ComboBox_Section_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lastResult != null) RenderComparison(_lastResult);
        }

        private void TextBox_Filter_TextChanged(object sender, EventArgs e)
        {
            if (_lastResult != null) RenderComparison(_lastResult);
        }

        private void CheckBox_DifferencesOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (_lastResult != null) RenderComparison(_lastResult);
        }

        // -----------------------------------------------------------------------
        // Summary header strip
        // -----------------------------------------------------------------------

        private void BuildSummaryStrip(PolicyComparer.ComparisonResult result)
        {
            summaryFlow.Controls.Clear();
            if (result == null) return;

            var counts = PolicyCompareReport.SummaryCounts(result);
            var ordered = counts.OrderBy(kv => PolicyComparer.OrderOf(kv.Key))
                                .ThenBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase);

            foreach (var kv in ordered)
            {
                var btn = new Button
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    FlatStyle = FlatStyle.Flat,
                    Margin = new Padding(0, 4, 6, 0),
                    Padding = new Padding(8, 2, 8, 2),
                    Tag = kv.Key,
                    Text = string.Format("{0}: {1}", kv.Key, kv.Value),
                    UseVisualStyleBackColor = false,
                    Cursor = Cursors.Hand,
                };

                btn.FlatAppearance.BorderColor = Color.LightGray;
                if (kv.Value > 0)
                {
                    btn.BackColor = Color.FromArgb(231, 240, 250);
                    btn.ForeColor = Color.Black;
                }
                else
                {
                    btn.BackColor = Color.FromArgb(240, 240, 240);
                    btn.ForeColor = Color.DimGray;
                }

                btn.Click += SummaryButton_Click;
                summaryFlow.Controls.Add(btn);
            }
        }

        private void SummaryButton_Click(object sender, EventArgs e)
        {
            if (_lastResult == null) return;
            var btn = sender as Button;
            if (btn == null) return;

            string section = btn.Tag as string;
            if (string.IsNullOrEmpty(section)) return;

            // Set the section combo to that section. If not present (because filter was hiding it),
            // fall back to "All sections".
            int idx = comboBox_Section.Items.IndexOf(section);
            comboBox_Section.SelectedIndex = idx >= 0 ? idx : 0;

            // Scroll the listview to the first item in that section
            ScrollToSection(section);
        }

        private void ScrollToSection(string section)
        {
            foreach (ListViewItem item in resultsListView.Items)
            {
                if (item.Group != null && string.Equals(item.Group.Header, section, StringComparison.OrdinalIgnoreCase))
                {
                    item.EnsureVisible();
                    item.Selected = true;
                    item.Focused = true;
                    resultsListView.Focus();
                    break;
                }
            }
        }

        // -----------------------------------------------------------------------
        // Render comparison
        // -----------------------------------------------------------------------

        private void RenderComparison(PolicyComparer.ComparisonResult result)
        {
            resultsListView.BeginUpdate();
            try
            {
                resultsListView.Items.Clear();
                resultsListView.Groups.Clear();
                resultsListView.Columns.Clear();

                if (result == null || result.Policies.Count == 0)
                {
                    return;
                }

                resultsListView.Columns.Add("Section", 160);
                resultsListView.Columns.Add("Item", 260);

                var validPolicies = result.Policies.Where(p => p.Policy != null).ToList();
                foreach (var policy in validPolicies)
                {
                    resultsListView.Columns.Add(policy.DisplayName, 200);
                }

                ResizePolicyColumns();

                bool diffOnly = checkBox_DifferencesOnly.Checked;
                string sectionFilter = comboBox_Section.SelectedItem as string;
                string textFilter = textBox_Filter.Text?.Trim();

                IEnumerable<PolicyComparer.ComparisonEntry> source = result.Entries;

                if (diffOnly)
                {
                    source = source.Where(en => en.IsDifferent);
                }

                if (!string.IsNullOrEmpty(sectionFilter)
                    && !string.Equals(sectionFilter, AllSectionsLabel, StringComparison.OrdinalIgnoreCase))
                {
                    source = source.Where(en => string.Equals(en.Section, sectionFilter, StringComparison.OrdinalIgnoreCase));
                }

                if (!string.IsNullOrEmpty(textFilter))
                {
                    source = source.Where(en => MatchesTextFilter(en, validPolicies, textFilter));
                }

                var grouped = source.GroupBy(en => en.Section)
                                    .OrderBy(g => PolicyComparer.OrderOf(g.Key))
                                    .ThenBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

                foreach (var sectionGroup in grouped)
                {
                    var lvGroup = new ListViewGroup(sectionGroup.Key, sectionGroup.Key);
                    resultsListView.Groups.Add(lvGroup);

                    foreach (var entry in sectionGroup.OrderBy(en => en.DisplayName, StringComparer.OrdinalIgnoreCase))
                    {
                        var item = new ListViewItem(entry.Section) { Group = lvGroup };
                        item.SubItems.Add(entry.DisplayName);

                        foreach (var policy in validPolicies)
                        {
                            entry.Values.TryGetValue(policy.DisplayName, out string value);
                            item.SubItems.Add(value ?? "<not present>");
                        }

                        if (entry.IsDifferent)
                        {
                            item.BackColor = Color.FromArgb(255, 245, 220);
                            item.ForeColor = Color.Black;
                        }

                        resultsListView.Items.Add(item);
                    }
                }
            }
            finally
            {
                resultsListView.EndUpdate();
            }
        }

        private static bool MatchesTextFilter(PolicyComparer.ComparisonEntry entry,
                                              List<PolicyComparer.LoadedPolicy> validPolicies,
                                              string filter)
        {
            if (entry.Section != null && entry.Section.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (entry.DisplayName != null && entry.DisplayName.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0) return true;

            foreach (var p in validPolicies)
            {
                entry.Values.TryGetValue(p.DisplayName, out string v);
                if (!string.IsNullOrEmpty(v) && v.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        // -----------------------------------------------------------------------
        // Context menu (copy)
        // -----------------------------------------------------------------------

        private void MenuItem_CopyCell_Click(object sender, EventArgs e)
        {
            var hit = resultsListView.PointToClient(MousePosition);
            var info = resultsListView.HitTest(hit);
            if (info?.SubItem != null && !string.IsNullOrEmpty(info.SubItem.Text))
            {
                SafeSetClipboard(info.SubItem.Text);
                return;
            }

            // Fall back: copy first selected row first cell
            if (resultsListView.SelectedItems.Count > 0)
            {
                SafeSetClipboard(resultsListView.SelectedItems[0].Text);
            }
        }

        private void MenuItem_CopyRow_Click(object sender, EventArgs e)
        {
            CopySelectedRow(separator: " | ");
        }

        private void MenuItem_CopyRowTsv_Click(object sender, EventArgs e)
        {
            CopySelectedRow(separator: "\t");
        }

        private void CopySelectedRow(string separator)
        {
            if (resultsListView.SelectedItems.Count == 0) return;
            var item = resultsListView.SelectedItems[0];
            var sb = new StringBuilder();
            for (int i = 0; i < item.SubItems.Count; i++)
            {
                if (i > 0) sb.Append(separator);
                sb.Append(item.SubItems[i].Text);
            }
            SafeSetClipboard(sb.ToString());
        }

        private static void SafeSetClipboard(string text)
        {
            try { Clipboard.SetText(text ?? string.Empty); }
            catch (Exception ex) { Logger.Log?.AddWarningMsg("PolicyCompare: clipboard set failed: " + ex.Message); }
        }

        // -----------------------------------------------------------------------
        // Export
        // -----------------------------------------------------------------------

        private void Button_Export_Click(object sender, EventArgs e)
        {
            if (_lastResult == null || _lastResult.Entries.Count == 0)
            {
                SetStatus("Run a comparison first, then export.", Color.OrangeRed);
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = "Export comparison report";
                sfd.Filter = "HTML Report (*.html)|*.html|" +
                             "CSV File (*.csv)|*.csv|" +
                             "Markdown File (*.md)|*.md";
                sfd.FilterIndex = 1;
                sfd.AddExtension = true;
                sfd.RestoreDirectory = true;
                sfd.FileName = "policy-comparison";

                if (sfd.ShowDialog(this) != DialogResult.OK) return;

                PolicyCompareReport.ReportFormat format;
                string ext = Path.GetExtension(sfd.FileName).ToLowerInvariant();
                switch (ext)
                {
                    case ".csv": format = PolicyCompareReport.ReportFormat.Csv; break;
                    case ".md": format = PolicyCompareReport.ReportFormat.Markdown; break;
                    case ".html":
                    case ".htm":
                    default: format = PolicyCompareReport.ReportFormat.Html; break;
                }

                try
                {
                    PolicyCompareReport.Write(_lastResult, sfd.FileName, format,
                                              checkBox_DifferencesOnly.Checked);
                    SetStatus("Exported report to " + sfd.FileName, Color.SeaGreen);
                    Logger.Log?.AddInfoMsg("PolicyCompare: exported " + format + " report to " + sfd.FileName);
                }
                catch (Exception ex)
                {
                    Logger.Log?.AddErrorMsg("PolicyCompare: export failed", ex);
                    SetStatus("Export failed: " + ex.Message, Color.Firebrick);
                }
            }
        }

        // -----------------------------------------------------------------------
        // Misc
        // -----------------------------------------------------------------------

        private void Button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ResultsListView_SizeChanged(object sender, EventArgs e)
        {
            ResizePolicyColumns();
        }

        private void ResizePolicyColumns()
        {
            const int fixedColumnsWidth = 160 + 260;
            const int verticalScrollAllowance = 24;

            if (resultsListView.Columns.Count <= 2)
            {
                return;
            }

            int policyColumnCount = resultsListView.Columns.Count - 2;
            int available = Math.Max(0, resultsListView.ClientSize.Width - fixedColumnsWidth - verticalScrollAllowance);
            int width = Math.Max(140, available / policyColumnCount);

            for (int i = 2; i < resultsListView.Columns.Count; i++)
            {
                resultsListView.Columns[i].Width = width;
            }
        }

        private void UpdateButtonStates()
        {
            button_Compare.Enabled = _policyPaths.Count >= 2 && (_compareWorker == null || !_compareWorker.IsBusy);
            button_RemovePolicy.Enabled = _policyPaths.Count > 0;
            button_Export.Enabled = _lastResult != null
                                    && _lastResult.Entries != null
                                    && _lastResult.Entries.Count > 0;
        }

        private void SetStatus(string text, Color color)
        {
            label_Status.Text = text;
            label_Status.ForeColor = color;
        }

        /// <summary>
        /// Apply Light/Dark theming consistent with other forms in the Wizard.
        /// </summary>
        private void ApplyTheme()
        {
            bool dark = Properties.Settings.Default.useDarkMode;

            BackColor = dark ? Color.FromArgb(15, 15, 15) : Color.White;
            ForeColor = dark ? Color.White : Color.Black;

            foreach (Control ctrl in Controls)
            {
                if (ctrl is Label || ctrl is CheckBox)
                {
                    ctrl.BackColor = BackColor;
                    if (ctrl != label_Status) // keep status label highlight color
                    {
                        ctrl.ForeColor = ForeColor;
                    }
                }
                else if (ctrl is TextBox tb)
                {
                    tb.BackColor = dark ? Color.FromArgb(32, 32, 32) : Color.White;
                    tb.ForeColor = dark ? Color.White : Color.Black;
                }
                else if (ctrl is ComboBox cb)
                {
                    cb.BackColor = dark ? Color.FromArgb(32, 32, 32) : Color.White;
                    cb.ForeColor = dark ? Color.White : Color.Black;
                }
                else if (ctrl is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = dark ? Color.DodgerBlue : Color.Black;
                    btn.BackColor = dark ? Color.Transparent : Color.WhiteSmoke;
                    btn.ForeColor = dark ? Color.DodgerBlue : Color.Black;
                }
            }

            policiesDataGrid.BackgroundColor = dark ? Color.FromArgb(15, 15, 15) : Color.White;
            policiesDataGrid.DefaultCellStyle.BackColor = dark ? Color.FromArgb(32, 32, 32) : Color.White;
            policiesDataGrid.DefaultCellStyle.ForeColor = dark ? Color.White : Color.Black;
            policiesDataGrid.AlternatingRowsDefaultCellStyle.BackColor = dark ? Color.FromArgb(24, 24, 24) : Color.WhiteSmoke;
            policiesDataGrid.ColumnHeadersDefaultCellStyle.BackColor = dark ? Color.Black : Color.WhiteSmoke;
            policiesDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = dark ? Color.White : Color.Black;
            policiesDataGrid.EnableHeadersVisualStyles = false;
            policiesDataGrid.GridColor = dark ? Color.LightSlateGray : Color.LightGray;

            resultsListView.BackColor = dark ? Color.FromArgb(15, 15, 15) : Color.White;
            resultsListView.ForeColor = dark ? Color.White : Color.Black;

            summaryFlow.BackColor = BackColor;
        }
    }
}
