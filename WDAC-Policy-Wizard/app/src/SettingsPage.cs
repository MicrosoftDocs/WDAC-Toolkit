﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Xml; 


namespace WDAC_Wizard
{
    public partial class SettingsPage : UserControl
    {
        private Dictionary<string, bool> SettingsDict;
        private MainWindow _MainWindow;
        private Logger Log; 

        public SettingsPage(MainWindow pMainWindow)
        {
            InitializeComponent();
            ShowVersionNumber();

            this._MainWindow = pMainWindow;
            this.Log = pMainWindow.Log; 
            this.SettingsDict = new Dictionary<string, bool>(); 
        }

        //
        // Summary:
        //     Method to set the current build version at the bottom of the About section of the settings page. 
        //      
        // Returns:
        //     None.
        private void ShowVersionNumber()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            this.appVersion_Label.Text = "App Version: " + versionInfo.FileVersion;
        }

        //
        // Summary:
        //     Method maps the checkbox that is toggled to the corresponding Preference.Setting 
        //     Sets the UI elements and modifies the Setting. Settings.Default are saved after each mod.
        //      
        // Returns:
        //     None.

        private void EnvVar_CheckBox_Click(object sender, EventArgs e)
        {
            // Toggle the UI and set the setting
            // Currently true, set to false
            PictureBox checkBox = ((PictureBox)sender);

            if (Properties.Settings.Default.useEnvVars)
            {
                checkBox.BackgroundImage = Properties.Resources.check_box_unchecked;
                checkBox.Tag = "Unchecked";
                Properties.Settings.Default.useEnvVars = false; 
            }
            else // false, set to true
            {
                checkBox.BackgroundImage = Properties.Resources.check_box_checked;
                checkBox.Tag = "Checked";
                Properties.Settings.Default.useEnvVars = true;
            }

            // Save setting and show update message to user
            SaveSetting(); 
        }

        private void ConvertPolicy_CheckBox_Click(object sender, EventArgs e)
        {
            // Toggle the UI and set the setting
            // Currently true, set to false
            PictureBox checkBox = ((PictureBox)sender);

            if (Properties.Settings.Default.convertPolicyToBinary)
            {
                checkBox.BackgroundImage = Properties.Resources.check_box_unchecked;
                checkBox.Tag = "Unchecked";
                Properties.Settings.Default.convertPolicyToBinary = false;
            }
            else // false, set to true
            {
                checkBox.BackgroundImage = Properties.Resources.check_box_checked;
                checkBox.Tag = "Checked";
                Properties.Settings.Default.convertPolicyToBinary = true;
            }

            // Save setting and show update message to user
            SaveSetting();
        }

        private void DefaultString_CheckBox_Click(object sender, EventArgs e)
        {
            // Toggle the UI and set the setting
            // Currently true, set to false
            PictureBox checkBox = ((PictureBox)sender);

            if (Properties.Settings.Default.useDefaultStrings)
            {
                checkBox.BackgroundImage = Properties.Resources.check_box_unchecked;
                checkBox.Tag = "Unchecked";
                Properties.Settings.Default.useDefaultStrings = false;
            }
            else // false, set to true
            {
                checkBox.BackgroundImage = Properties.Resources.check_box_checked;
                checkBox.Tag = "Checked";
                Properties.Settings.Default.useDefaultStrings = true;
            }

            // Save setting and show update message to user
            SaveSetting();
        }

        /// <summary>
        /// Sets the state for the Kernel Mode Recommended blocklist setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KernelmodeRecList_checkBox_Click(object sender, EventArgs e)
        {
            // Toggle the UI and set the setting
            // Currently true, set to false
            PictureBox checkBox = ((PictureBox)sender);

            if (Properties.Settings.Default.useDriverBlockRules)
            {
                checkBox.BackgroundImage = Properties.Resources.check_box_unchecked;
                checkBox.Tag = "Unchecked";
                Properties.Settings.Default.useDriverBlockRules = false;
            }
            else // false, set to true
            {
                checkBox.BackgroundImage = Properties.Resources.check_box_checked;
                checkBox.Tag = "Checked";
                Properties.Settings.Default.useDriverBlockRules = true;
            }

            // Save setting and show update message to user
            SaveSetting();
        }

        /// <summary>
        /// /// Sets the state for the User Mode Recommended blocklist setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UsermodeRecList_checkBox_Click(object sender, EventArgs e)
        {
            // Toggle the UI and set the setting
            // Currently true, set to false
            PictureBox checkBox = ((PictureBox)sender);

            if (Properties.Settings.Default.useUsermodeBlockRules)
            {
                checkBox.BackgroundImage = Properties.Resources.check_box_unchecked;
                checkBox.Tag = "Unchecked";
                Properties.Settings.Default.useUsermodeBlockRules = false;
            }
            else // false, set to true
            {
                checkBox.BackgroundImage = Properties.Resources.check_box_checked;
                checkBox.Tag = "Checked";
                Properties.Settings.Default.useUsermodeBlockRules = true;
            }

            // Save setting and show update message to user
            SaveSetting();
        }

        /// <summary>
        /// Sets the state for the Dark Mode Interface Setting. If disabled, light mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DarkMode_CheckBox_Click(object sender, EventArgs e)
        {
            // Toggle the UI and set the setting
            // Currently true, set to false
            PictureBox checkBox = ((PictureBox)sender);

            if (Properties.Settings.Default.useDarkMode)
            {
                checkBox.BackgroundImage = Properties.Resources.check_box_unchecked;
                checkBox.Tag = "Unchecked";
                Properties.Settings.Default.useDarkMode = false;
            }
            else // false, set to true
            {
                checkBox.BackgroundImage = Properties.Resources.check_box_checked;
                checkBox.Tag = "Checked";
                Properties.Settings.Default.useDarkMode = true;
            }

            // Save setting and show update message to user
            SaveSetting();

            // Set the Light or Dark Mode UI Elements
            SetPageUI();

            // Mode changed. Need to repaint all other UI elements like 
            // MainWindow, Control Panel, other existing Pages
            RepaintUIModeChange(); 
        }

        /// <summary>
        /// Saves the Wizard Setting and displays the update label
        /// </summary>
        private void SaveSetting()
        {
            // Save settings and show settings update to user
            Properties.Settings.Default.Save();
            this.Update_Label.Visible = true;

            Timer settingsUpdateNotificationTimer = new Timer();
            settingsUpdateNotificationTimer.Interval = (1500); // 1.5 secs
            settingsUpdateNotificationTimer.Tick += new EventHandler(SettingUpdateTimer_Tick);
            settingsUpdateNotificationTimer.Start();
        }

        /// <summary>
        /// /// Opens the EULA page on the WDAC Toolkit Github page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Terms_Label_Click(object sender, EventArgs e)
        {
            // Launch the terms of use page
            try
            {
                string webpage = "https://github.com/MicrosoftDocs/WDAC-Toolkit/blob/master/README.md";
                System.Diagnostics.Process.Start(webpage);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("Launching webpage for Terms of Use link encountered the following error", exp);
            }

        }

        /// <summary>
        /// Opens the privacy page on the WDAC Toolkit Github page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Privacy_Label_Click(object sender, EventArgs e)
        {
            // Launch the privacy agreement page
            try
            {
                string webpage = "https://github.com/MicrosoftDocs/WDAC-Toolkit/blob/master/PRIVACY.md";
                System.Diagnostics.Process.Start(webpage);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("Launching webpage for Privacy agreement link encountered the following error", exp);
            }

        }

        /// <summary>
        /// Resets all Wizard settings to their shipping default when the Reset button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetButton_Click(object sender, EventArgs e)
        {
            // Prompt user for additional confirmation
            DialogResult res = MessageBox.Show(Properties.Resources.ResetSettingsString,
                "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (res == DialogResult.Yes)
            {
                // Read the exe config file
                XmlDocument doc = new XmlDocument();
                string configPath = System.IO.Path.Combine(this._MainWindow.ExeFolderPath, "WDAC Wizard.exe.config");
                doc.Load(configPath); // Reading from the xml config file
                XmlNodeList settingsNodes = doc.GetElementsByTagName("setting");
                const int START = 15; 
                foreach(XmlNode settingNode in settingsNodes)
                {
                    int stop = settingNode.OuterXml.IndexOf("serialize") - 2;
                    string settingName = settingNode.OuterXml.Substring(START, stop - START);
                    string settingVal = settingNode.InnerText;
                    this.SettingsDict[settingName] = settingVal=="True";

                    this.Log.AddInfoMsg(String.Format("Parsed {0} = {1}", settingName, settingVal)); 
                }

                // Set the UI state for all the checkboxes
                SetSettingsValues(this.SettingsDict);
                Properties.Settings.Default.Reset();

                // Re-load page to set Light Mode UI Elements
                SetPageUI(); 
            }
        }

        /// <summary>
        /// Fires on the load event for the Settings Page and sets the SettingsDict
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsPage_Load(object sender, EventArgs e)
        {
            // On load, configure UI to match the app settings
            this.Log.AddNewSeparationLine("Settings Page Load"); 

            this.SettingsDict.Add("useEnvVars", (bool)Properties.Settings.Default.useEnvVars);
            this.SettingsDict.Add("useDefaultStrings", (bool)Properties.Settings.Default.useDefaultStrings);
            this.SettingsDict.Add("convertPolicyToBinary", (bool)Properties.Settings.Default.convertPolicyToBinary);
            this.SettingsDict.Add("useUsermodeBlockRules", (bool)Properties.Settings.Default.useUsermodeBlockRules);
            this.SettingsDict.Add("useDriverBlockRules", (bool)Properties.Settings.Default.useDriverBlockRules);
            this.SettingsDict.Add("useDarkMode", (bool)Properties.Settings.Default.useDarkMode);

            this.Log.AddInfoMsg("Successfully read in the following Default Settings: ");
            foreach (var key in this.SettingsDict.Keys)
            {
                this.Log.AddInfoMsg(String.Format("{0}: {1}", key, this.SettingsDict[key].ToString()));
            }

            // Set the UI state for all the checkboxes
            SetSettingsValues(this.SettingsDict);

            // Set the Light or Dark Mode UI Elements
            SetPageUI();
        }

        /// <summary>
        /// Sets the UI state foreach Settings Dictionary key value pair
        /// </summary>
        /// <param name="settingDict"></param>
        private void SetSettingsValues(Dictionary<string, bool> settingDict)
        {
            // if the setting is set to false, otherwise keep the default UI state of checked
            foreach (var settingName in settingDict.Keys)
            {
                string checkBoxName = settingName + "_CheckBox";

                // Skip this setting, there is no checkbox to update
                if (this.Controls.Find(checkBoxName, true).Length < 1)
                {
                    continue; 
                }

                if (!settingDict[settingName]) //False case
                {
                    this.Controls.Find(checkBoxName, true).FirstOrDefault().Tag = "Unchecked";
                    this.Controls.Find(checkBoxName, true).FirstOrDefault().BackgroundImage = Properties.Resources.check_box_unchecked; 
                }
                else
                {
                    this.Controls.Find(checkBoxName, true).FirstOrDefault().Tag = "Checked";
                    this.Controls.Find(checkBoxName, true).FirstOrDefault().BackgroundImage = Properties.Resources.check_box_checked;
                }

                this.Log.AddInfoMsg(String.Format("Setting {0} set to {1}", settingName, settingDict[settingName])); 
            }            
        }

        /// <summary>
        /// Hides the Update Label at the end of the timer countdown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingUpdateTimer_Tick(object sender, EventArgs e)
        {
            this.Update_Label.Visible = false; 
        }

        /// <summary>
        /// Changes the color of the checkbox when the setting checkbox is hovered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingCheckBox_Hover(object sender, EventArgs e)
        {
            // Change the background color when mouse is hovering above checkbox
            PictureBox checkBox = ((PictureBox)sender);
            checkBox.BackColor = Color.FromArgb(190, 230, 253);
        }

        /// <summary>
        /// Sets the back color to white for the checkbox picturebox when the user is no longer hovering the mouse over a checkbox.  
        /// </summary>
        /// <param name="sender">Sender is the picturebox control </param>
        private void SettingCheckBox_Leave(object sender, EventArgs e)
        {
            PictureBox checkBox = ((PictureBox)sender);
            checkBox.BackColor = Color.White;
        }

        /// <summary>
        /// Opens the recommended driver blocklist MS Doc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelDriverBlock_Click(object sender, EventArgs e)
        {
            // Launch the WDAC recommended blocklist page
            try
            {
                string webpage = "https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/microsoft-recommended-driver-block-rules";
                System.Diagnostics.Process.Start(webpage);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("Launching webpage for Driver recommended Blocklist link encountered the following error", exp);
            }
        }

        /// <summary>
        /// Opens the recommended user mode blocklist MS Doc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Label_UsermodeBlock_Click(object sender, EventArgs e)
        {
            // Launch the WDAC recommended blocklist page
            try
            {
                string webpage = "https://docs.microsoft.com/windows/security/threat-protection/windows-defender-application-control/microsoft-recommended-block-rules";
                System.Diagnostics.Process.Start(webpage);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("Launching webpage for user mode recommended Blocklist link encountered the following error", exp);
            }
        }

        /// <summary>
        /// Sets all of the Light and Dark Mode UI Elements
        /// </summary>
        private void SetPageUI()
        {
            // Set Labels Color
            List<Label> labels = new List<Label>();
            GetLabelsRecursive(this, labels);
            SetLabelsColor(labels);

            // Set Form Back Color
            SetFormBackColor();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RepaintUIModeChange()
        {
            // MainForm UI Elements
            SetResetButtonColor(); // Set 'Reset' Button Colors
            ResetControlPanelUI(); // Set the Control Panel Colors
            ResetNextButtonUI();  // Set the Next Button Colors

            // Revalidate and paint all existing pages
            RepaintAllExistingPages();
        }

        /// <summary>
        /// Gets all of the labels on the form recursively
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="labels"></param>
        private void GetLabelsRecursive(Control parent, List<Label> labels)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Label label)
                {
                    labels.Add(label);
                }
                else
                {
                    GetLabelsRecursive(control, labels);
                }
            }
        }

        /// <summary>
        /// Sets the color of the labels defined in the provided List
        /// </summary>
        /// <param name="labels"></param>
        private void SetLabelsColor(List<Label> labels)
        {
            // Dark Mode
            if(Properties.Settings.Default.useDarkMode)
            {
                foreach(Label label in labels)
                {
                    if(label.Tag == null || label.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        label.ForeColor = Color.White;
                        label.BackColor = Color.Black; 
                    }
                }
            }

            // Light Mode
            else
            {
                foreach (Label label in labels)
                {
                    if (label.Tag == null || label.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        label.ForeColor = Color.Black;
                        label.BackColor = Color.White; 
                    }
                }
            }
        }

        /// <summary>
        /// Sets the color of the form to white or black depending on the 
        /// state of the Dark Mode setting
        /// </summary>
        private void SetFormBackColor()
        {
            // Dark Mode
            if(Properties.Settings.Default.useDarkMode)
            {
                BackColor = Color.FromArgb(15, 15, 15);
            }

            // Light Mode
            else
            {
                BackColor = Color.White;
            }
        }

        /// <summary>
        /// Sets the colors for the Reset Button depending 
        /// on the state of Light or Dark Mode
        /// </summary>
        private void SetResetButtonColor()
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                resetButton.ForeColor = Color.DodgerBlue;
                resetButton.BackColor = Color.Black; 
            }

            // Light Mode
            else
            {
                resetButton.ForeColor = Color.Black;
                resetButton.BackColor = Color.WhiteSmoke;
            }
        }

        /// <summary>
        /// Resets the color of the control panel in
        /// the MainWindow class
        /// </summary>
        private void ResetControlPanelUI()
        {
            this._MainWindow.SetControlPanelUI(); 
        }

        /// <summary>
        /// Resets the color of the Next Button UI in 
        /// the MainWindow class
        /// </summary>
        private void ResetNextButtonUI()
        {
            this._MainWindow.SetNextButtonUI(); 
        }

        /// <summary>
        /// Forces revalidation and painting of all previously loaded pages
        /// so they are in the correct Dark or Light mode
        /// </summary>
        private void RepaintAllExistingPages()
        {
            this._MainWindow.ReloadPreviousPages(); 
        }
    }
}
