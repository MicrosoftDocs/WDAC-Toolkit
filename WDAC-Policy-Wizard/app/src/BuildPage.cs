// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace WDAC_Wizard
{
    public partial class BuildPage : UserControl
    {
        public string XmlFilePath { get; set; } // File path for the WDAC policy XML file
        public string BinFilePath { get; set; } // File path for the WDAC policy binary file
        private MainWindow _MainWindow;

        const int PATH_LENGTH_LIMIT = 100; 

        public BuildPage(MainWindow pMainWindow)
        {
            InitializeComponent();
            progressBar.Maximum = 100;
            progressBar.Step = 1;
            progressBar.Value = 0;
            this._MainWindow = pMainWindow; 
        }

        /// <summary>
        /// Sets the value of the Progres bar, the percent label and the process label on the UserControl 
        /// </summary>
        /// <param name="progress">Int between 0-100 representing the percent progress of the CI policy 
        /// build operations</param>
        /// <param name="process">String shown on UI representing the CI policy build process currently executing.</param>
        public void UpdateProgressBar(int progress, string process)
        {
            progressBar.Value = progress;
            progress_Label.Visible = true; 
            progress_Label.Text = String.Format("{0} %", Convert.ToString(progress));
            progressString_Label.Text = process;
        }

        /// <summary>
        /// Displays and formats the output path for the XML policy file only
        /// </summary>
        /// <param name="policyFilePath">File location for the XML policy file</param>
        public void ShowFinishMsg(string policyFilePath)
        {
            this.XmlFilePath = policyFilePath;

            // Format the path in cases of long strings
            this.xmlFilePathLabel.Text = FormatText(policyFilePath);

            // Update the UI
            this.xmlFilePathLabel.Enabled = true;
            this.finishPanel.Visible = true;
            this.label_WaitMsg.Visible = false;

            // Hide the binFilePathLabel - XML file output only
            this.binFilePathLabel.Visible = false; 
        }

        public void ShowFinishMsg(string policyFilePath, string binFilePath)
        {
            // Set the path objects to unformatted text
            this.XmlFilePath = policyFilePath;
            this.BinFilePath = binFilePath;

            // Format the paths in cases of long strings
            this.xmlFilePathLabel.Text = FormatText(policyFilePath);
            this.binFilePathLabel.Text = FormatText(binFilePath);

            // Update the UI - show xmlFilePath label
            this.xmlFilePathLabel.Enabled = true;
            this.finishPanel.Visible = true;
            this.label_WaitMsg.Visible = false;

            // Update the UI - show binFilePath label
            this.binFilePathLabel.Visible = true;
            this.binFilePathLabel.Enabled = true;
        }

        /// <summary>
        /// Shows an error message if the CI policy build process throws an error 
        /// </summary>
        public string FormatText(string longstring)
        {
            if(longstring.Length > PATH_LENGTH_LIMIT)
            {
                // Get the last instance of the \ between the start and the path limit (80)
                int splitLoc = longstring.Substring(0, PATH_LENGTH_LIMIT).LastIndexOf("\\");
                if(splitLoc > 1)
                {
                    longstring = longstring.Substring(0, splitLoc) + Environment.NewLine + longstring.Substring(splitLoc);
                }
                else
                {
                    // Split midway through
                    int mid_pt = longstring.Length / 2; 
                    longstring = longstring.Substring(0, mid_pt) + Environment.NewLine + longstring.Substring(mid_pt);
                }
            }

            return longstring; 
        }

        /// <summary>
        /// Shows an error message if the WDAC policy build process throws an error. 
        /// Prompts the user to check the log file
        /// </summary>
        public void ShowError()
        {
            finishPanel.Visible = true;
            this.finishLabel.Text = Properties.Resources.PolicyBuild_Error + this._MainWindow.TempFolderPath;
            UpdateProgressBar(0, "Error");

            this.xmlFilePathLabel.Enabled = false;
            this.binFilePathLabel.Enabled = false;
            this.label_WaitMsg.Visible = false;
        }

        /// <summary>
        /// Opens the WDAC XML policy file in the default app for XML files
        /// </summary>
        private void XmlFilePathLabel_Click(object sender, EventArgs e)
        {
            // Open XML file in the default application set for XML files
            if (File.Exists(this.XmlFilePath))
            {
                try
                {
                    Process.Start(this.XmlFilePath);
                }
                catch (Exception ex)
                {
                    // Log file already closed
                    string displayMsg = Properties.Resources.FileOpen_Exception + "\r\n" + ex.Message;
                    DialogResult res = MessageBox.Show(displayMsg,
                                                       "WDAC Wizard Exception",
                                                       MessageBoxButtons.OK,
                                                       MessageBoxIcon.Error);
                }
            }
            else
            {
                string displayMsg = Properties.Resources.XMLFileOpen_Error + this._MainWindow.TempFolderPath; 
                DialogResult res = MessageBox.Show(displayMsg,
                                                   "WDAC Wizard Error",
                                                   MessageBoxButtons.OK,
                                                   MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Opens the File Explorer to the folder in which the Bin file resides
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BinFilePathLabel_Click(object sender, EventArgs e)
        {
            // Open Binary's directory to browse for file in Explorer
            if (File.Exists(this.BinFilePath))
            {
                try
                {
                    // Open File Explorer to the directory of the binary
                    string fileDirectory = Path.GetDirectoryName(BinFilePath);
                    Process.Start("explorer.exe", fileDirectory);
                }
                catch (Exception ex)
                {
                    // Log file already closed
                    string displayMsg = Properties.Resources.FileOpen_Exception + "\r\n" + ex.Message;
                    DialogResult res = MessageBox.Show(displayMsg,
                                                       "WDAC Wizard Exception",
                                                       MessageBoxButtons.OK,
                                                       MessageBoxIcon.Error);
                }
            }
            else
            {
                string displayMsg = Properties.Resources.BINFileOpen_Error + this._MainWindow.TempFolderPath;
                DialogResult res = MessageBox.Show(displayMsg,
                                                   "WDAC Wizard Error",
                                                   MessageBoxButtons.OK,
                                                   MessageBoxIcon.Error);
            }
        }
    }
}
