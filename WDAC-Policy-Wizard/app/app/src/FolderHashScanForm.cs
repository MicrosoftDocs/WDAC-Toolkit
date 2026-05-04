using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WDAC_Wizard
{
    /// <summary>
    /// Form that allows users to scan a folder for files and select which ones to create hash rules for.
    /// </summary>
    public class FolderHashScanForm : Form
    {
        private Label labelFolderPath;
        private TextBox textBoxFolderPath;
        private Button buttonBrowseFolder;
        private CheckBox checkBoxSubfolders;
        private Button buttonScan;
        private CheckedListBox checkedListBoxFiles;
        private Button buttonSelectAll;
        private Button buttonDeselectAll;
        private Button buttonOK;
        private Button buttonCancel;
        private Label labelStatus;
        private Label labelHashTypes;
        private CheckBox checkBoxHashSha1;
        private CheckBox checkBoxHashSha256;
        private CheckBox checkBoxHashPageSha1;
        private CheckBox checkBoxHashPageSha256;
        private CheckBox checkBoxHashAll;
        private Panel panelHashTypes;

        /// <summary>
        /// The list of file paths the user selected
        /// </summary>
        public List<string> SelectedFiles { get; private set; }

        /// <summary>
        /// The original folder path that was scanned
        /// </summary>
        public string SourceFolderPath { get; private set; }

        /// <summary>
        /// Whether subfolders were included in the scan
        /// </summary>
        public bool IncludeSubfolders { get; private set; }

        /// <summary>
        /// Whether all scanned files were selected (no unchecking)
        /// </summary>
        public bool AllFilesSelected { get; private set; }

        /// <summary>
        /// The hash types selected by the user to keep in the generated policy.
        /// Values match FriendlyName patterns: "Hash Sha1", "Hash Sha256", "Hash Page Sha1", "Hash Page Sha256"
        /// </summary>
        public HashSet<string> SelectedHashTypes { get; private set; }

        public FolderHashScanForm()
        {
            SelectedFiles = new List<string>();
            SelectedHashTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Folder Hash Scan";
            this.Size = new Size(700, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            int yPos = 15;

            // Folder path label
            labelFolderPath = new Label
            {
                Text = "Folder Path:",
                Location = new Point(15, yPos),
                Size = new Size(80, 20),
                Font = new Font("Tahoma", 9F)
            };
            this.Controls.Add(labelFolderPath);

            // Folder path textbox
            textBoxFolderPath = new TextBox
            {
                Location = new Point(100, yPos),
                Size = new Size(460, 24),
                Font = new Font("Tahoma", 9F),
                ReadOnly = true
            };
            this.Controls.Add(textBoxFolderPath);

            // Browse button
            buttonBrowseFolder = new Button
            {
                Text = "Browse...",
                Location = new Point(570, yPos - 2),
                Size = new Size(90, 26),
                Font = new Font("Tahoma", 9F)
            };
            buttonBrowseFolder.Click += ButtonBrowseFolder_Click;
            this.Controls.Add(buttonBrowseFolder);

            yPos += 35;

            // Include subfolders checkbox
            checkBoxSubfolders = new CheckBox
            {
                Text = "Include subfolders",
                Location = new Point(100, yPos),
                Size = new Size(160, 22),
                Font = new Font("Tahoma", 9F),
                Checked = false
            };
            this.Controls.Add(checkBoxSubfolders);

            // Hash type checkboxes
            labelHashTypes = new Label
            {
                Text = "Hash Types to Keep:",
                Location = new Point(280, yPos),
                Size = new Size(130, 20),
                Font = new Font("Tahoma", 9F)
            };
            this.Controls.Add(labelHashTypes);

            panelHashTypes = new Panel
            {
                Location = new Point(15, yPos + 25),
                Size = new Size(650, 28)
            };

            checkBoxHashAll = new CheckBox
            {
                Text = "All",
                Location = new Point(0, 2),
                Size = new Size(50, 22),
                Font = new Font("Tahoma", 9F),
                Checked = true
            };
            checkBoxHashAll.CheckedChanged += CheckBoxHashAll_CheckedChanged;
            panelHashTypes.Controls.Add(checkBoxHashAll);

            checkBoxHashSha1 = new CheckBox
            {
                Text = "Hash SHA1",
                Location = new Point(55, 2),
                Size = new Size(100, 22),
                Font = new Font("Tahoma", 9F),
                Checked = true
            };
            panelHashTypes.Controls.Add(checkBoxHashSha1);

            checkBoxHashSha256 = new CheckBox
            {
                Text = "Hash SHA256",
                Location = new Point(160, 2),
                Size = new Size(115, 22),
                Font = new Font("Tahoma", 9F),
                Checked = true
            };
            panelHashTypes.Controls.Add(checkBoxHashSha256);

            checkBoxHashPageSha1 = new CheckBox
            {
                Text = "Hash Page SHA1",
                Location = new Point(280, 2),
                Size = new Size(130, 22),
                Font = new Font("Tahoma", 9F),
                Checked = true
            };
            panelHashTypes.Controls.Add(checkBoxHashPageSha1);

            checkBoxHashPageSha256 = new CheckBox
            {
                Text = "Hash Page SHA256",
                Location = new Point(415, 2),
                Size = new Size(145, 22),
                Font = new Font("Tahoma", 9F),
                Checked = true
            };
            panelHashTypes.Controls.Add(checkBoxHashPageSha256);

            this.Controls.Add(panelHashTypes);

            yPos += 55;

            // Scan button - on its own row for visibility
            buttonScan = new Button
            {
                Text = "Scan Folder",
                Location = new Point(100, yPos - 2),
                Size = new Size(120, 28),
                Font = new Font("Tahoma", 9F, FontStyle.Bold)
            };
            buttonScan.Click += ButtonScan_Click;
            this.Controls.Add(buttonScan);

            yPos += 35;

            // Status label
            labelStatus = new Label
            {
                Text = "Select a folder and click Scan to find files.",
                Location = new Point(15, yPos),
                Size = new Size(650, 20),
                Font = new Font("Tahoma", 9F)
            };
            this.Controls.Add(labelStatus);

            yPos += 25;

            // Checked list box for files
            checkedListBoxFiles = new CheckedListBox
            {
                Location = new Point(15, yPos),
                Size = new Size(645, 320),
                Font = new Font("Tahoma", 8.5F),
                CheckOnClick = true,
                HorizontalScrollbar = true
            };
            this.Controls.Add(checkedListBoxFiles);

            yPos += 330;

            // Select All / Deselect All buttons
            buttonSelectAll = new Button
            {
                Text = "Select All",
                Location = new Point(15, yPos),
                Size = new Size(90, 28),
                Font = new Font("Tahoma", 9F)
            };
            buttonSelectAll.Click += (s, e) =>
            {
                for (int i = 0; i < checkedListBoxFiles.Items.Count; i++)
                    checkedListBoxFiles.SetItemChecked(i, true);
            };
            this.Controls.Add(buttonSelectAll);

            buttonDeselectAll = new Button
            {
                Text = "Deselect All",
                Location = new Point(115, yPos),
                Size = new Size(100, 28),
                Font = new Font("Tahoma", 9F)
            };
            buttonDeselectAll.Click += (s, e) =>
            {
                for (int i = 0; i < checkedListBoxFiles.Items.Count; i++)
                    checkedListBoxFiles.SetItemChecked(i, false);
            };
            this.Controls.Add(buttonDeselectAll);

            // OK / Cancel buttons
            buttonCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(560, yPos),
                Size = new Size(100, 28),
                Font = new Font("Tahoma", 9F),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(buttonCancel);

            buttonOK = new Button
            {
                Text = "Add Selected",
                Location = new Point(450, yPos),
                Size = new Size(100, 28),
                Font = new Font("Tahoma", 9F)
            };
            buttonOK.Click += ButtonOK_Click;
            this.Controls.Add(buttonOK);

            this.AcceptButton = buttonOK;
            this.CancelButton = buttonCancel;

            // Apply dark mode if needed
            if (Properties.Settings.Default.useDarkMode)
            {
                ApplyDarkMode();
            }
        }

        private void ApplyDarkMode()
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;

            foreach (Control ctrl in this.Controls)
            {
                ctrl.ForeColor = Color.White;
                if (ctrl is TextBox tb)
                {
                    tb.BackColor = Color.FromArgb(15, 15, 15);
                }
                else if (ctrl is CheckedListBox clb)
                {
                    clb.BackColor = Color.FromArgb(15, 15, 15);
                }
                else if (ctrl is Button btn)
                {
                    btn.BackColor = Color.FromArgb(50, 50, 50);
                    btn.FlatStyle = FlatStyle.Flat;
                }
                else if (ctrl is Panel pnl)
                {
                    pnl.ForeColor = Color.White;
                    foreach (Control child in pnl.Controls)
                    {
                        child.ForeColor = Color.White;
                    }
                }
            }
        }

        private void ButtonBrowseFolder_Click(object sender, EventArgs e)
        {
            // Use OpenFileDialog in folder-picker mode via CommonDialog workaround
            // FolderBrowserDialog in .NET 8 supports UseDescriptionForTitle for a modern look
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Select a folder to scan for files";
            folderDialog.UseDescriptionForTitle = true;
            folderDialog.ShowNewFolderButton = false;

            if (!string.IsNullOrEmpty(textBoxFolderPath.Text) && Directory.Exists(textBoxFolderPath.Text))
            {
                folderDialog.InitialDirectory = textBoxFolderPath.Text;
            }

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxFolderPath.Text = folderDialog.SelectedPath;
                folderDialog.Dispose();
            }
        }

        private void ButtonScan_Click(object sender, EventArgs e)
        {
            string folderPath = textBoxFolderPath.Text;
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                labelStatus.Text = "Please select a valid folder path.";
                return;
            }

            checkedListBoxFiles.Items.Clear();

            try
            {
                SearchOption searchOption = checkBoxSubfolders.Checked
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly;

                // Get all files - filter for common PE and script types
                string[] extensions = new[]
                {
                    "*.exe", "*.dll", "*.sys", "*.rll", "*.bin",
                    "*.ps1", "*.bat", "*.vbs", "*.js",
                    "*.hxs", "*.mui", "*.lex", "*.mof",
                    "*.msi", "*.msp", "*.ocx", "*.drv", "*.scr", "*.cpl"
                };

                var files = new List<string>();
                foreach (string ext in extensions)
                {
                    try
                    {
                        files.AddRange(Directory.GetFiles(folderPath, ext, searchOption));
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Skip folders we can't access
                    }
                }

                // Also allow all files option - if no PE files found, get all files
                if (files.Count == 0)
                {
                    try
                    {
                        files.AddRange(Directory.GetFiles(folderPath, "*.*", searchOption));
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }

                files = files.Distinct().OrderBy(f => f).ToList();

                foreach (string file in files)
                {
                    checkedListBoxFiles.Items.Add(file, true); // Checked by default
                }

                labelStatus.Text = $"Found {files.Count} file(s). Select the files to create hash rules for.";
            }
            catch (Exception ex)
            {
                labelStatus.Text = $"Error scanning folder: {ex.Message}";
                Logger.Log.AddErrorMsg($"FolderHashScanForm scan error: {ex}");
            }
        }

        private void CheckBoxHashAll_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = checkBoxHashAll.Checked;
            checkBoxHashSha1.Checked = isChecked;
            checkBoxHashSha256.Checked = isChecked;
            checkBoxHashPageSha1.Checked = isChecked;
            checkBoxHashPageSha256.Checked = isChecked;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            SelectedFiles.Clear();
            for (int i = 0; i < checkedListBoxFiles.Items.Count; i++)
            {
                if (checkedListBoxFiles.GetItemChecked(i))
                {
                    SelectedFiles.Add(checkedListBoxFiles.Items[i].ToString());
                }
            }

            // Build the set of hash types to keep
            SelectedHashTypes.Clear();
            if (checkBoxHashSha1.Checked)
                SelectedHashTypes.Add("Hash Sha1");
            if (checkBoxHashSha256.Checked)
                SelectedHashTypes.Add("Hash Sha256");
            if (checkBoxHashPageSha1.Checked)
                SelectedHashTypes.Add("Hash Page Sha1");
            if (checkBoxHashPageSha256.Checked)
                SelectedHashTypes.Add("Hash Page Sha256");
            // Also handle Authenticode SIP variants (for non-PE files like .js)
            if (checkBoxHashSha256.Checked)
                SelectedHashTypes.Add("Hash Authenticode SIP Sha256");

            SourceFolderPath = textBoxFolderPath.Text;
            IncludeSubfolders = checkBoxSubfolders.Checked;
            AllFilesSelected = (SelectedFiles.Count == checkedListBoxFiles.Items.Count);

            if (SelectedFiles.Count == 0)
            {
                labelStatus.Text = "Please select at least one file.";
                return;
            }

            if (SelectedHashTypes.Count == 0)
            {
                labelStatus.Text = "Please select at least one hash type.";
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
