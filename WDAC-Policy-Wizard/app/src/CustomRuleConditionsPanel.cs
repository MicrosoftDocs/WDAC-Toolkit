using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;


namespace WDAC_Wizard
{
    public partial class CustomRuleConditionsPanel : Form
    {
        // CI Policy objects
        private WDAC_Policy Policy;
        public PolicyCustomRules PolicyCustomRule;     // One instance of a custom rule. Appended to Policy.CustomRules
        private List<string> AllFilesinFolder;          // List to track all files in a folder 

        private MainWindow _MainWindow;
        private SigningRules_Control SigningControl;
        public bool RuleInEdit = false;
        private UIState state;
        private Exceptions_Control exceptionsControl;
        private bool redoRequired;
        private string[] DefaultValues;
        private List<string> FoundPackages;

        // Previous state of the COM Guid
        private string PrevComText = String.Empty;
        private bool IgnoreInput = false;

        private enum UIState
        {
            RuleConditions = 0,
            RuleExceptions = 1
        }

        public CustomRuleConditionsPanel(SigningRules_Control pControl)
        {
            InitializeComponent();
            Policy = pControl.Policy;
            PolicyCustomRule = new PolicyCustomRules();
            AllFilesinFolder = new List<string>();

            _MainWindow = pControl._MainWindow;
            _MainWindow.RedoFlowRequired = false;
            _MainWindow.CustomRuleinProgress = true;
            Logger.Log.AddInfoMsg("==== Custom Signing Rules Panel Initialized ====");
            SigningControl = pControl;
            RuleInEdit = true;
            state = UIState.RuleConditions;
            redoRequired = false;
            exceptionsControl = null;
            DefaultValues = new string[5];
            FoundPackages = new List<string>();
        }

        /// <summary>
        /// Appends the custom rule to the bottom of the DataGridView and creates the rule in the CustomRules list. 
        /// </summary>
        private void Button_CreateRule_Click(object sender, EventArgs e)
        {
            // Verify first that an exception flow is not in progress
            if (exceptionsControl != null
                && exceptionsControl.IsRuleInProgress())
            {
                DialogResult res = MessageBox.Show(Properties.Resources.RuleExceptionInProgressText,
                                                   "Confirmation",
                                                   MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Question);
                if (res == DialogResult.No)
                {
                    return;
                }
            }

            // Check COM Object rule for valid GUID
            // Skip scenario and reference file states for COM rules
            if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Com)
            {
                // Snap GUID at time of rule creation and remove whitespace
                if (comboBoxComKeyType.SelectedItem.ToString() != Properties.Resources.ComObjectAllKeys)
                {
                    PolicyCustomRule.COMObject.Guid = Regex.Replace(textBoxObjectKey.Text, @"\s", "");
                }

                if (!PolicyCustomRule.COMObject.IsValidRule())
                {
                    label_Error.Visible = true;
                    label_Error.Text = Properties.Resources.ComInvalidGuid;
                    Logger.Log.AddWarningMsg("Invalid COM Object Guid " + PolicyCustomRule.COMObject.Guid);
                    return;
                }

                // Set COM Object value to state of the rule
                if (PolicyCustomRule.Permission == PolicyCustomRules.RulePermission.Allow)
                {
                    PolicyCustomRule.COMObject.ValueItem = true;
                }
                else
                {
                    PolicyCustomRule.COMObject.ValueItem = false;
                }
            }

            // Check AppID Tag key-value pair
            else if(PolicyCustomRule.Type == PolicyCustomRules.RuleType.AppIDTag)
            {
                PolicyCustomRule.AppIDTag.Key= appIDTagKeyTextbox.Text;
                PolicyCustomRule.AppIDTag.Value = appIDTagValueTextbox.Text;

                if (!PolicyCustomRule.AppIDTag.IsValidTag())
                {
                    label_Error.Visible = true;
                    label_Error.Text = Properties.Resources.AppIDInvalidTag;
                    Logger.Log.AddWarningMsg($"Invalid AppID Tag for key {PolicyCustomRule.AppIDTag.Key} - value {PolicyCustomRule.AppIDTag.Value} pair"); 
                    return;
                }
            }

            // Validate KMCI scenario settings and reference file state where applicable
            else
            {
                if (!ValidRuleState())
                {
                    return;
                }
            }

            // Flag to warn user that N/A's in the CustomRules pane may result in a hash rule
            bool warnUser = false;

            // Publisher and File Attribute checks
            // Check to make sure none of the fields are invalid
            if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher ||
                PolicyCustomRule.Type == PolicyCustomRules.RuleType.FileAttributes)
            {
                // Assert one checkbox needs to be selected
                if (!(PolicyCustomRule.CheckboxCheckStates.checkBox0 || PolicyCustomRule.CheckboxCheckStates.checkBox1
                    || PolicyCustomRule.CheckboxCheckStates.checkBox2 || PolicyCustomRule.CheckboxCheckStates.checkBox3
                    || PolicyCustomRule.CheckboxCheckStates.checkBox4))
                {
                    label_Error.Visible = true;
                    label_Error.Text = Properties.Resources.InvalidCheckboxState;
                    Logger.Log.AddWarningMsg("Invalid checkbox state. No checkboxes selected.");
                    return;
                }

                if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
                {
                    if (PolicyCustomRule.CheckboxCheckStates.checkBox0 && !Helper.IsValidText(textBoxSlider_0.Text))
                    {
                        warnUser = true;
                        Logger.Log.AddWarningMsg("PCACertificate field with null attribute");
                    }

                    if (PolicyCustomRule.CheckboxCheckStates.checkBox1 && !Helper.IsValidText(textBoxSlider_1.Text))
                    {
                        warnUser = true;
                        Logger.Log.AddWarningMsg("Publisher field with null attribute");
                    }

                    if (PolicyCustomRule.CheckboxCheckStates.checkBox4 && !Helper.IsValidText(textBoxSlider_4.Text))
                    {
                        ShowInvalidErrorLabel();
                        return;
                    }

                    // Check custom EKU value if applicable
                    if (!String.IsNullOrEmpty(PolicyCustomRule.EKUFriendly))
                    {
                        string ekuTLVEncoded = Helper.EKUValueToTLVEncoding(PolicyCustomRule.EKUFriendly.Trim());
                        if (String.IsNullOrEmpty(ekuTLVEncoded))
                        {
                            Logger.Log.AddErrorMsg("EKU Encoding Failed for user-input EKU value " + PolicyCustomRule.EKUFriendly);
                            label_Error.Visible = true;
                            label_Error.Text = Properties.Resources.InvalidEKUFormat_Error;
                            return;
                        }
                        else
                        {
                            PolicyCustomRule.EKUEncoded = ekuTLVEncoded;
                        }
                    }
                }
                else
                {
                    if (PolicyCustomRule.CheckboxCheckStates.checkBox0 && !Helper.IsValidText(textBoxSlider_0.Text))
                    {
                        ShowInvalidErrorLabel();
                        return;
                    }

                    if (PolicyCustomRule.CheckboxCheckStates.checkBox1 && !Helper.IsValidText(textBoxSlider_1.Text))
                    {
                        ShowInvalidErrorLabel();
                        return;
                    }
                }

                if (PolicyCustomRule.CheckboxCheckStates.checkBox2 && !Helper.IsValidText(textBoxSlider_2.Text))
                {
                    ShowInvalidErrorLabel();
                    return;
                }

                if (PolicyCustomRule.CheckboxCheckStates.checkBox3 && !Helper.IsValidText(textBoxSlider_3.Text))
                {
                    ShowInvalidErrorLabel();
                    return;
                }
            }

            // Packaged family name apps
            // Set the list of apps at button create time

            if (PolicyCustomRule.Level == PolicyCustomRules.RuleLevel.PackagedFamilyName)
            {
                // Assert >=1 packaged apps must be selected
                if (checkedListBoxPackagedApps.CheckedItems.Count < 1)
                {
                    label_Error.Visible = true;
                    label_Error.Text = Properties.Resources.PFNEmptyList_Error;
                    Logger.Log.AddWarningMsg("Create button rule selected with an empty packaged app list.");
                    return;
                }
                else
                {
                    // Using for loop to avoid System.InvalidOperationException despite list not changing
                    for (int i = 0; i < checkedListBoxPackagedApps.CheckedItems.Count; i++)
                    {
                        var item = checkedListBoxPackagedApps.CheckedItems[i];
                        PolicyCustomRule.PackagedFamilyNames.Add(item.ToString());
                    }
                }
            }

            // Folder Scan 
            // Set the list of omitted paths at button create time
            if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.FolderScan)
            {
                // Assert >=1 rule levels must be selected
                if (checkedListBoxRuleLevels.CheckedItems.Count < 1)
                {
                    label_Error.Visible = true;
                    label_Error.Text = Properties.Resources.RuleLevelEmptyList_Error;
                    Logger.Log.AddWarningMsg("Create button rule selected with an empty folder scan rule level list.");
                    return;
                }
                // Set the rule level ordered list
                else
                {
                    for (int i = 0; i < checkedListBoxRuleLevels.CheckedItems.Count; i++)
                    {
                        PolicyCustomRule.Scan.Levels.Add(checkedListBoxRuleLevels.CheckedItems[i].ToString());
                    }
                }

                // Check for Omit Scan Paths
                if (checkedListBoxOmitPaths.CheckedItems.Count > 1)
                {
                    // Using for loop to avoid System.InvalidOperationException despite list not changing
                    for (int i = 0; i < checkedListBoxOmitPaths.CheckedItems.Count; i++)
                    {
                        PolicyCustomRule.Scan.OmitPaths.Add(checkedListBoxOmitPaths.CheckedItems[i].ToString());
                    }
                }
            }

            // Check custom rules
            if (PolicyCustomRule.UsingCustomValues)
            {
                // Check custom publisher field
                if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher && PolicyCustomRule.CheckboxCheckStates.checkBox1)
                {
                    if (!Helper.IsValidPublisher(PolicyCustomRule.CustomValues.Publisher))
                    {
                        label_Error.Visible = true;
                        label_Error.Text = Properties.Resources.InvalidPublisherFormat_Error;
                        Logger.Log.AddWarningMsg(String.Format("Invalid format for Custom Publisher", PolicyCustomRule.CustomValues.Publisher));
                        return;
                    }
                    else
                    {
                        // Valid publisher, format so WDAC is happy with the input
                        PolicyCustomRule.CustomValues.Publisher = Helper.FormatPublisherCN(PolicyCustomRule.CustomValues.Publisher);
                    }
                }

                // Check custom product field
                if (PolicyCustomRule.CheckboxCheckStates.checkBox2 && !Helper.IsValidText(textBoxSlider_2.Text))
                {
                    ShowInvalidErrorLabel();
                    return;
                }

                // Check custom original filename
                if (PolicyCustomRule.CheckboxCheckStates.checkBox3 && !Helper.IsValidText(textBoxSlider_3.Text))
                {
                    ShowInvalidErrorLabel();
                    return;
                }

                // Check custom versions
                if (PolicyCustomRule.CheckboxCheckStates.checkBox4)
                {
                    if (!Helper.IsValidVersion(PolicyCustomRule.CustomValues.MinVersion))
                    {
                        label_Error.Visible = true;
                        label_Error.Text = Properties.Resources.InvalidVersionFormat_Error;
                        Logger.Log.AddWarningMsg(String.Format("Invalid version format for CustomMinVersion: {0}", PolicyCustomRule.CustomValues.MinVersion));
                        return;
                    }

                    // Check MaxVersion
                    if (PolicyCustomRule.CustomValues.MaxVersion != null)
                    {
                        if (!Helper.IsValidVersion(PolicyCustomRule.CustomValues.MaxVersion))
                        {
                            label_Error.Visible = true;
                            label_Error.Text = Properties.Resources.InvalidVersionFormat_Error;
                            Logger.Log.AddWarningMsg(String.Format("Invalid version format for CustomMaxVersion: {0}", PolicyCustomRule.CustomValues.MaxVersion));
                            return;
                        }

                        if (Helper.CompareVersions(PolicyCustomRule.CustomValues.MinVersion, PolicyCustomRule.CustomValues.MaxVersion) < 0)
                        {
                            label_Error.Visible = true;
                            label_Error.Text = Properties.Resources.InvalidVersionRange_Error;
                            Logger.Log.AddWarningMsg(String.Format("CustomMinVersion {0} !< CustomMaxVersion {1}", PolicyCustomRule.CustomValues.MinVersion, PolicyCustomRule.CustomValues.MaxVersion));
                            return;
                        }
                    }
                }

                // Check custom path
                if (PolicyCustomRule.CustomValues.Path != null)
                {
                    // Check if this is a valid path rules. I.e. supported macros
                    if (!Helper.IsValidPathRule(PolicyCustomRule.CustomValues.Path))
                    {
                        label_Error.Visible = true;
                        label_Error.Text = Properties.Resources.InvalidPath_Error;
                        Logger.Log.AddWarningMsg("Invalid custom path rule for path: " + PolicyCustomRule.CustomValues.Path);
                        return;
                    }

                    // Check number of wildcards.
                    // If the number is greater than 1, warn the user IFF the warn setting (default on) is on
                    if (Helper.GetNumberofWildcards(PolicyCustomRule.CustomValues.Path) > 1
                       && Properties.Settings.Default.warnWildcardPath)
                    {
                        Logger.Log.AddWarningMsg("Warning - Path Rule Windows Version Support for path: " + PolicyCustomRule.CustomValues.Path);

                        var res = MessageBox.Show(Properties.Resources.PathRule_Warning,
                                                  "Warning - Path Rule Windows Version Support",
                                                  MessageBoxButtons.YesNoCancel,
                                                  MessageBoxIcon.Warning);

                        Logger.Log.AddInfoMsg("Message box result: " + res.ToString());

                        // User wants to modify the rule
                        // Escape the checks so they may edit the path
                        if (res == DialogResult.Cancel)
                        {
                            return;
                        }

                        // User does not want to be warned about path rules on Windows 11 only. 
                        // Set the warnWildcardPath to false to bypass future warnings
                        if (res == DialogResult.No)
                        {
                            Properties.Settings.Default.warnWildcardPath = false;
                            Properties.Settings.Default.Save();

                            Logger.Log.AddInfoMsg("Set warnWildcardPath to: false");
                        }
                    }
                }

                // Parse package family names into PolicyCustomRule.CustomValues.PackageFamilyNames
                if (PolicyCustomRule.Level == PolicyCustomRules.RuleLevel.PackagedFamilyName)
                {
                    PolicyCustomRule.CustomValues.PackageFamilyNames = PolicyCustomRule.PackagedFamilyNames;
                }

                // Parse hashes into PolicyCustomRule.CustomValues.Hash
                if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Hash)
                {
                    var hashList = richTextBox_CustomHashes.Text.Split(',');
                    foreach (var hash in hashList)
                    {
                        if (!String.IsNullOrEmpty(hash) && hash.Trim().Length % 2 == 0) // must be an even number
                        {
                            PolicyCustomRule.CustomValues.Hashes.Add(hash.Trim());
                        }
                    }

                    if (PolicyCustomRule.CustomValues.Hashes.Count == 0)
                    {
                        label_Error.Visible = true;
                        label_Error.Text = Properties.Resources.HashEmptyList_Error;
                        Logger.Log.AddWarningMsg("Zero hash values located.");
                        return;
                    }

                }
            }

            string[] displayString = FormatTableDisplayString(warnUser);

            if (displayString == null)
            {
                return;
            }

            // Offboard this to signingRules_Condition
            RuleInEdit = false;
            SigningControl.AddRuleToTable(displayString, PolicyCustomRule, warnUser);

            // Renew the custom rule instance
            PolicyCustomRule = new PolicyCustomRules();

            // Reset UI view
            ClearCustomRulesPanel(true);
            _MainWindow.CustomRuleinProgress = false;
        }

        private bool ValidRuleState()
        {
            // Assert one of umci or kmci must be set
            if (!(PolicyCustomRule.SigningScenarioCheckStates.kmciEnabled || PolicyCustomRule.SigningScenarioCheckStates.umciEnabled))
            {
                label_Error.Visible = true;
                label_Error.Text = Properties.Resources.InvalidSigningScenarioCheckboxState;
                Logger.Log.AddWarningMsg("Invalid signing scenarios checkbox state. No checkboxes selected.");
                return false;
            }

            // Assert KMCI cannot be set for PFN or path rules
            if (PolicyCustomRule.SigningScenarioCheckStates.kmciEnabled
                && (PolicyCustomRule.Type == PolicyCustomRules.RuleType.PackagedApp
                || PolicyCustomRule.Type == PolicyCustomRules.RuleType.FilePath
                || PolicyCustomRule.Type == PolicyCustomRules.RuleType.FolderPath))
            {
                label_Error.Visible = true;
                label_Error.Text = Properties.Resources.InvalidKMCIRule;
                Logger.Log.AddWarningMsg("KMCI rule scoping set for PFN or path rule.");
                return false;
            }

            // Assert that the reference file cannot be null, unless we are creating a custom value rule or a PFN rule
            if (PolicyCustomRule.ReferenceFile == null)
            {
                if (PolicyCustomRule.UsingCustomValues
                    || PolicyCustomRule.Level == PolicyCustomRules.RuleLevel.PackagedFamilyName)
                {

                }
                else
                {
                    label_Error.Visible = true;
                    label_Error.Text = Properties.Resources.InvalidRule_Error;
                    Logger.Log.AddWarningMsg("Create button rule selected without allow/deny setting and a reference file.");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Creates the data structure to provide to the data table to view the state of the custom rule
        /// </summary>
        /// <param name="warnUser">Flag indicating whether to warn user that rule created may fallback to hash rule</param>
        /// <returns></returns>
        private string[] FormatTableDisplayString(bool warnUser)
        {
            // Show warning message to user to notify that a signing chain was not found; may result in a hash rule created
            if (warnUser)
            {
                DialogResult res = MessageBox.Show("One or more of the file attributes could not be found. Creating this rule may result in a hash rule if unsuccessful. " +
                                                    "\n\nWould you like to proceed anyway?",
                                                    "Proceed with Rule Creation?",
                                                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (res == DialogResult.Yes)
                {
                    Logger.Log.AddInfoMsg("Proceeding with Rule Creation anyway. Rule may fallback to hash");
                }
                else
                {
                    return null;
                }
            }

            // Add rule and exceptions to the table and master list & Scroll to new row index
            string action = String.Empty;
            string level = String.Empty;
            string name = String.Empty;
            string files = String.Empty;
            string exceptions = String.Empty;

            Logger.Log.AddInfoMsg("--- New Custom Rule Added ---");

            // Set Action/Permission value to Allow or Deny
            action = PolicyCustomRule.Permission.ToString();

            // Set Level value to the RuleLevel value//or should this be type for simplicity? 
            level = PolicyCustomRule.Type.ToString();

            // Get the Rule Scope for logging
            string scope = String.Empty;

            if (PolicyCustomRule.SigningScenarioCheckStates.kmciEnabled)
            {
                if (PolicyCustomRule.SigningScenarioCheckStates.umciEnabled)
                {
                    scope = "Kernel and Usermode Rule";
                }
                else
                {
                    scope = "Kernel Rule";
                }
            }

            else
            {
                scope = "Usermode Rule";
            }

            // Format rule display string
            switch (PolicyCustomRule.Type)
            {
                // Signer rules
                case PolicyCustomRules.RuleType.Publisher:
                    {
                        name += "CA: " + textBoxSlider_0.Text;
                        if (PolicyCustomRule.CheckboxCheckStates.checkBox1)
                        {
                            name += " & CN: " + textBoxSlider_1.Text;
                        }
                        if (PolicyCustomRule.CheckboxCheckStates.checkBox2)
                        {
                            name += " & Product: " + textBoxSlider_2.Text;
                        }
                        if (PolicyCustomRule.CheckboxCheckStates.checkBox3)
                        {
                            name += " & Filename: " + textBoxSlider_3.Text;
                        }
                        if (PolicyCustomRule.CheckboxCheckStates.checkBox4)
                        {
                            name += " & Min Version: " + textBoxSlider_4.Text;
                        }
                        if (PolicyCustomRule.UsingCustomValues && PolicyCustomRule.CustomValues.MaxVersion != null)
                        {
                            name += " & Max Version: " + PolicyCustomRule.CustomValues.MaxVersion;
                        }
                        break;
                    }

                case PolicyCustomRules.RuleType.FileAttributes:
                    {
                        if (PolicyCustomRule.CheckboxCheckStates.checkBox0)
                        {
                            name += " & Original Filename: " + textBoxSlider_0.Text;
                        }
                        if (PolicyCustomRule.CheckboxCheckStates.checkBox1)
                        {
                            name += " & File Description: " + textBoxSlider_1.Text;
                        }
                        if (PolicyCustomRule.CheckboxCheckStates.checkBox2)
                        {
                            name += " & Product: " + textBoxSlider_2.Text;
                        }
                        if (PolicyCustomRule.CheckboxCheckStates.checkBox3)
                        {
                            name += " & Internal Name: " + textBoxSlider_3.Text;
                        }
                        name = name.Substring(3); // Offset by 3 to remove the first occurence of ' & '
                        break;
                    }

                case PolicyCustomRules.RuleType.PackagedApp:
                    {
                        name = "Packaged apps matching the Package Family Name (PFN) rules in the Files cell";
                        files = Helper.GetListofPackages(PolicyCustomRule);
                        break;
                    }

                case PolicyCustomRules.RuleType.FolderPath:
                    {
                        if (PolicyCustomRule.UsingCustomValues)
                        {
                            name = "Files under path: " + PolicyCustomRule.CustomValues.Path;
                        }
                        else
                        {
                            name = "Files under path: " + PolicyCustomRule.ReferenceFile;
                        }
                        break;
                    }

                case PolicyCustomRules.RuleType.FilePath:
                    {
                        if (PolicyCustomRule.UsingCustomValues)
                        {
                            name = "Files matching: " + PolicyCustomRule.CustomValues.Path;
                        }
                        else
                        {
                            name = "Files matching: " + PolicyCustomRule.ReferenceFile;
                        }
                        break;
                    }

                case PolicyCustomRules.RuleType.Hash:
                    {
                        if (PolicyCustomRule.UsingCustomValues)
                        {
                            if (PolicyCustomRule.CustomValues.Hashes.Count > 1)
                            {
                                name = String.Format("Custom Hash List: {0}, ...", PolicyCustomRule.CustomValues.Hashes[0]);
                            }
                            else
                            {
                                name = "Custom Hash Value: " + PolicyCustomRule.CustomValues.Hashes[0];
                            }
                        }
                        else
                        {
                            name = "File Hash Rule: " + PolicyCustomRule.ReferenceFile;
                        }
                        break;
                    }

                case PolicyCustomRules.RuleType.Com:
                    {
                        level = "COM Object";
                        name = "Provider: " + PolicyCustomRule.COMObject.Provider;
                        files = "Key: " + PolicyCustomRule.COMObject.Guid;

                        break;
                    }

                case PolicyCustomRules.RuleType.FolderScan:
                    {
                        level = PolicyCustomRule.Scan.Levels[0];
                        name = "Folder Scan - " + PolicyCustomRule.ReferenceFile;
                        if (PolicyCustomRule.Scan.Levels.Count > 1)
                        {
                            files = "Level Fallback to ";
                            for (int i = 1; i < PolicyCustomRule.Scan.Levels.Count; i++)
                            {
                                files += PolicyCustomRule.Scan.Levels[i] + ", ";
                            }
                            files = files.Substring(0, files.Length - 2);
                        }
                        break;
                    }

                case PolicyCustomRules.RuleType.Certificate:
                    {
                        level = "Certificate";
                        name = $"Certificate File Rule: {PolicyCustomRule.ReferenceFile}";
                        break;
                    }

                case PolicyCustomRules.RuleType.AppIDTag:

                    action = "";
                    level = "AppID Tag";
                    name = $"Key: {PolicyCustomRule.AppIDTag.Key}";
                    files = $"Value: {PolicyCustomRule.AppIDTag.Value}";
                    break;
            }

            // Handle custom EKU
            if (!String.IsNullOrEmpty(PolicyCustomRule.EKUFriendly))
            {
                files += "EKU: " + PolicyCustomRule.EKUFriendly;
            }

            // Handle exceptions
            if (PolicyCustomRule.ExceptionList.Count > 0)
            {
                foreach (var exception in PolicyCustomRule.ExceptionList)
                {
                    string exceptionString = String.Format("{0}: {1} {2}; ", exception.Permission, exception.Level,
                        exception.ReferenceFile);
                    exceptions += exceptionString;
                }

                // Remove trailing semi-colon
                exceptions.Trim(';');
            }

            Logger.Log.AddInfoMsg(String.Format("Custom {0} created: {1} - {2} - {3} - {4} ", scope, action, level, name, files));
            return new string[5] { action, level, name, files, exceptions };
        }

        /// <summary>
        /// Sets the RuleLevel to publisher, filepath or hash for the CustomRules object. 
        /// Executes when user selects the Rule Type dropdown combobox. 
        /// </summary>
        private void RuleType_ComboboxChanged(object sender, EventArgs e)
        {
            // Check if the selected item is null (this occurs after reseting it - rule creation)
            if (comboBox_RuleType.SelectedIndex < 0)
            {
                return;
            }

            string selectedOpt = comboBox_RuleType.SelectedItem.ToString();
            ClearCustomRulesPanel(false);
            label_Info.Visible = true;
            label_Error.Visible = false; // Clear error label

            // Reset checkbox to unchecked
            if (checkBox_CustomValues.Checked)
            {
                checkBox_CustomValues.Checked = false;
                PolicyCustomRule.UsingCustomValues = false;
            }

            // Clear panels
            checkBox_CustomPath.Visible = false;
            checkBox_CustomPath.Checked = false;
            panelPackagedApps.Visible = false;
            panelComObject.Visible = false;
            panelFolderScanConditions.Visible = false;
            label_condition.Text = "Reference File:";
            appIdPanel.Visible = false;

            switch (selectedOpt)
            {
                case "Publisher":

                    PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.Publisher);
                    PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FilePublisher); // Match UI by default
                    label_Info.Text = "Creates a rule for a file that is signed by the software publisher. \r\n" +
                        "Select a file to use as reference for your rule.";
                    break;

                case "Path":

                    PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.FilePath);
                    PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FilePath);
                    label_Info.Text = "Creates a rule for a specific file or folder. \r\n" +
                        "Selecting folder will affect all files in the folder.";
                    panel_FileFolder.Visible = true;
                    radioButton_File.Checked = true; // By default, 

                    checkBox_CustomPath.Visible = true;
                    checkBox_CustomPath.Text = "Use Custom Path";
                    break;

                case "File Attributes":

                    PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.FileAttributes);
                    PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.InternalName); // Match UI by default
                    label_Info.Text = "Creates a rule for a file based on one of its attributes. \r\n" +
                        "Select a file to use as reference for your rule.";
                    break;

                case "Packaged App":

                    PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.PackagedApp);
                    PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.PackagedFamilyName);
                    panelPackagedApps.Location = label_condition.Location;
                    panelPackagedApps.Visible = true;
                    panelPackagedApps.BringToFront();
                    label_Info.Text = "Creates a rule for a packaged app based on its package family name.\r\nSearch for the name of the packages to allow/deny.";
                    break;

                case "File Hash":

                    PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.Hash);
                    PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.Hash);
                    label_Info.Text = "Creates a rule for a file that is not signed. \r\n" +
                        "Select the file for which you wish to create a hash rule.";
                    checkBox_CustomPath.Visible = true;
                    checkBox_CustomPath.Text = "Use Custom Hash Values";
                    break;

                case "COM Object":

                    if (Policy._PolicyType == WDAC_Policy.PolicyType.AppIdTaggingPolicy)
                    {
                        _ = MessageBox.Show("COM Object Rules are not supported in AppID Tagging Policies. Please select another rule type",
                                            "Unsupported Rule - COM Objects",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Question);

                        ClearCustomRulesPanel(true);
                        return;
                    }

                    PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.Com);
                    label_Info.Text = "Creates a rule for COM object and software provider.";
                    panelComObject.Location = label_condition.Location;
                    panelComObject.Visible = true;
                    panelComObject.BringToFront();
                    break;

                case "Folder Scan":

                    PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.FolderScan);
                    label_Info.Text = "Creates a file rule for each file found in the scanned directory and it's subdirectories.";
                    panelFolderScanConditions.Location = checkBox_CustomPath.Location;
                    panelFolderScanConditions.Visible = true;
                    label_condition.Text = "Scan Path:";
                    break;

                case "Certificate File":

                    PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.Certificate);
                    label_Info.Text = "Creates a signer rule rule based off the selected certificate file.";
                    break;

                case "AppID Tags":

                    if(Policy._PolicyType != WDAC_Policy.PolicyType.AppIdTaggingPolicy)
                    {
                        _ = MessageBox.Show("AppID Tags are supported only in AppID Tagging Policies. Please select another rule type",
                                            "Unsupported Rule - AppID Tags",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Question);
                        ClearCustomRulesPanel(true);
                        return; 
                    }

                    PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.AppIDTag);
                    label_Info.Text = "Creates an AppID Tag key value pair for an AppID Tagging Policy";
                    appIdPanel.Location = label_condition.Location;
                    appIdPanel.Visible = true;
                    appIdPanel.BringToFront();
                    break; 

                default:
                    break;
            }

            Logger.Log.AddInfoMsg(String.Format("Custom File Rule Level Set to {0}", selectedOpt));

            // Returned back from exceptions to change Rule Type - Redo is required
            if (exceptionsControl != null)
            {
                redoRequired = true;
            }

            // Break if Com Object; nothing else is needed to proceed in rule creation
            if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Com
                || PolicyCustomRule.Type == PolicyCustomRules.RuleType.AppIDTag)
            {
                return;
            }

            // Show new UI based on the rule type selected if the user has already selected a reference file
            if (PolicyCustomRule.ReferenceFile != null && !redoRequired)
            {
                SetDefaultUIState(PolicyCustomRule.GetRuleType());
            }
        }

        /// <summary>
        /// Clears the remaining UI elements of the Custom Rules Panel when a user selects the 'Create Rule' button. 
        /// /// </summary>
        /// /// <param name="clearComboBox">Bool to reset the Rule Type combobox.</param>
        private void ClearCustomRulesPanel(bool clearComboBox = false)
        {
            // Clear all of UI updates we make based on the type of rule so that the Custom Rules Panel is clear
            //Publisher:
            panel_Publisher_Scroll.Visible = false;

            //File Path:
            panel_FileFolder.Visible = false;
            textBox_ReferenceFile.Clear();

            // Reset the rule type combobox
            if (clearComboBox)
            {
                comboBox_RuleType.SelectedItem = null;
                comboBox_RuleType.Text = "--Select--";

                label_Info.Visible = false; 
            }

            // Reset the Slider UI textboxes
            ResetSliderUI();
        }


        /// <summary>
        /// Launches the FileDialog and prompts user to select the reference file. 
        /// Based on rule type, sets the UI elements for Publisher, FilePath or Hash rules. 
        /// </summary>
        private void Button_Browse_Click(object sender, EventArgs e)
        {
            // Clear any error messages
            ClearLabel_ErrorText();

            // Browse button for reference file:
            if (comboBox_RuleType.SelectedItem == null)
            {
                label_Error.Visible = true;
                label_Error.Text = "Please select a rule type first.";
                Logger.Log.AddWarningMsg("Browse button selected before rule type selected. Set rule type first.");
                return;
            }

            if (PolicyCustomRule.Type != PolicyCustomRules.RuleType.FolderPath
                && PolicyCustomRule.Type != PolicyCustomRules.RuleType.FolderScan)
            {
                string refPath = GetFileLocation();
                if (refPath == String.Empty)
                {
                    return;
                }

                DefaultValues[4] = refPath;

                // Check if certificate file is valid
                if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Certificate)
                {
                    if (!Helper.IsValidCertificateFile(refPath))
                    {
                        // Bad certificate file - reset UI
                        CertificateUICleanUp();
                        return;
                    }
                }

                // Custom rule in progress
                _MainWindow.CustomRuleinProgress = true;

                // Get generic file information to be shown to user
                SetFileSignerInfo(refPath);

                // Unsupported crypto or antother issue with the file
                // Start over
                if (!PolicyCustomRule.SupportedCrypto && PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
                {
                    UnSupportedCryptoCleanUp();
                    return;
                }
            }

            // Set the landing UI depending on the Rule type
            SetDefaultUIState(PolicyCustomRule.Type);

            // Returned from exceptions user control to modify the reference path
            if (exceptionsControl != null)
            {
                redoRequired = true;
            }
        }

        /// <summary>
        /// Retrieves the file attribute and signer info
        /// </summary>
        /// <param name="refPath"></param>
        /// <returns>True if successful. False otherwise. </returns>
        private void SetFileSignerInfo(string refPath)
        {
            PolicyCustomRule.FileInfo = new Dictionary<string, string>(); // Reset dict
            FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(refPath);

            PolicyCustomRule.ReferenceFile = fileInfo.FileName; // Returns the file path
            string fileVersion = Helper.ConcatFileVersion(fileInfo);
            PolicyCustomRule.FileInfo.Add("CompanyName", String.IsNullOrEmpty(fileInfo.CompanyName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.CompanyName.Trim());
            PolicyCustomRule.FileInfo.Add("ProductName", String.IsNullOrEmpty(fileInfo.ProductName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.ProductName.Trim());
            PolicyCustomRule.FileInfo.Add("OriginalFilename", String.IsNullOrEmpty(fileInfo.OriginalFilename) ? Properties.Resources.DefaultFileAttributeString : fileInfo.OriginalFilename.Trim());
            PolicyCustomRule.FileInfo.Add("FileVersion", String.IsNullOrEmpty(fileVersion) ? Properties.Resources.DefaultFileAttributeString : fileVersion);
            PolicyCustomRule.FileInfo.Add("FileName", String.IsNullOrEmpty(fileInfo.OriginalFilename) ? Properties.Resources.DefaultFileAttributeString : fileInfo.OriginalFilename.Trim());
            PolicyCustomRule.FileInfo.Add("FileDescription", String.IsNullOrEmpty(fileInfo.FileDescription) ? Properties.Resources.DefaultFileAttributeString : fileInfo.FileDescription.Trim());
            PolicyCustomRule.FileInfo.Add("InternalName", String.IsNullOrEmpty(fileInfo.InternalName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.InternalName.Trim());

            // Get cert chain info to be shown to the user irrespective of the initial type.
            // Otherwise, we don't check or try to build the cert chain
            string leafCertSubjectName = "";
            string pcaCertSubjectName = "";

            try
            {
                var signer = X509Certificate.CreateFromSignedFile(refPath);

                var cert = new X509Certificate2(signer);
                var certChain = new X509Chain();
                var certChainIsValid = certChain.Build(cert);

                leafCertSubjectName = cert.SubjectName.Name;
                leafCertSubjectName = Helper.FormatSubjectName(leafCertSubjectName);

                if (certChain.ChainElements.Count > 1)
                {
                    pcaCertSubjectName = certChain.ChainElements[1].Certificate.SubjectName.Name;
                    // Remove everything past C=..
                    pcaCertSubjectName = Helper.FormatSubjectName(pcaCertSubjectName);
                }

                // Check that the parsed certificate chain uses supported crytpo
                if (Helper.IsCryptoInvalid(certChain))
                {
                    Logger.Log.AddWarningMsg(String.Format("Unsupported Crypto detected for {0} signed by {1}", refPath, leafCertSubjectName));
                    PolicyCustomRule.SupportedCrypto = false;
                    return;
                }
            }

            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg(String.Format("Caught exception {0} when trying to create cert from the following signed file {1}", exp, refPath));
                label_Error.Text = "Unable to find certificate chain for " + fileInfo.FileName;
                label_Error.Visible = true;
            }

            PolicyCustomRule.FileInfo.Add("LeafCertificate", String.IsNullOrEmpty(leafCertSubjectName) ? Properties.Resources.DefaultFileAttributeString : leafCertSubjectName);
            PolicyCustomRule.FileInfo.Add("PCACertificate", String.IsNullOrEmpty(pcaCertSubjectName) ? Properties.Resources.DefaultFileAttributeString : pcaCertSubjectName);
            PolicyCustomRule.SupportedCrypto = true;
        }

        /// <summary>
        /// Sets the default state of the textboxes and checkboxes based on the rule type
        /// </summary>
        /// <param name="ruleType"></param>
        private void SetDefaultUIState(PolicyCustomRules.RuleType ruleType)
        {
            switch (ruleType)
            {
                case PolicyCustomRules.RuleType.Publisher:

                    // UI
                    textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    // Show right side of the text
                    if (textBox_ReferenceFile.TextLength > 0)
                    {
                        textBox_ReferenceFile.SelectionStart = textBox_ReferenceFile.TextLength - 1;
                        textBox_ReferenceFile.ScrollToCaret();
                    }

                    // Check if supported crypto first before getting FileInfo
                    // PCACert and LeafCert will be null in the case where user chose non publisher rule type for ECC-signed file,
                    // for example, then scanned file and then set rule type to publisher 
                    if (!PolicyCustomRule.SupportedCrypto)
                    {
                        UnSupportedCryptoCleanUp();
                        return;
                    }

                    // Set defaults to restore to if custom values is ever reset
                    if (PolicyCustomRule.FileInfo != null && PolicyCustomRule.FileInfo.Count > 0)
                    {
                        DefaultValues[0] = PolicyCustomRule.FileInfo["PCACertificate"];
                        DefaultValues[1] = PolicyCustomRule.FileInfo["LeafCertificate"];
                        DefaultValues[2] = PolicyCustomRule.FileInfo["ProductName"];
                        DefaultValues[3] = PolicyCustomRule.FileInfo["FileName"];
                        DefaultValues[4] = PolicyCustomRule.FileInfo["FileVersion"];
                    }

                    // Set checkbox struct
                    PolicyCustomRule.CheckboxCheckStates.checkBox0 = true;
                    PolicyCustomRule.CheckboxCheckStates.checkBox1 = true;
                    PolicyCustomRule.CheckboxCheckStates.checkBox2 = false;
                    PolicyCustomRule.CheckboxCheckStates.checkBox3 = true;
                    PolicyCustomRule.CheckboxCheckStates.checkBox4 = true;

                    // Set all fields checked by default, except for product to match legacy (slider bar) behavior unless null original filename or version
                    checkBoxAttribute0.Checked = true;
                    checkBoxAttribute1.Checked = true;
                    checkBoxAttribute3.Checked = true;
                    checkBoxAttribute4.Checked = true;

                    // Do not check for N/As in PCA or publisher fields since the file may be cat
                    // signed which the Wizard cannot handle right now
                    if (DefaultValues[3] == Properties.Resources.DefaultFileAttributeString)
                    {
                        checkBoxAttribute3.Checked = false;
                        PolicyCustomRule.CheckboxCheckStates.checkBox3 = false;
                    }

                    if (DefaultValues[4] == Properties.Resources.DefaultFileAttributeString)
                    {
                        checkBoxAttribute4.Checked = false;
                        PolicyCustomRule.CheckboxCheckStates.checkBox4 = false;
                    }

                    checkBoxAttribute0.Text = "Issuing CA:";
                    checkBoxAttribute1.Text = "Publisher:";
                    checkBoxAttribute2.Text = "Product name:";
                    checkBoxAttribute3.Text = "File name:";
                    checkBoxAttribute4.Text = "Min. Version:";

                    // Version textbox should be set to normal size
                    textBoxSlider_4.Size = textBoxSlider_3.Size;

                    // Show version boxes
                    textBoxSlider_4.Visible = true;
                    checkBoxAttribute4.Visible = true;

                    textBoxSlider_0.Text = DefaultValues[0];
                    textBoxSlider_1.Text = DefaultValues[1];
                    textBoxSlider_2.Text = DefaultValues[2];
                    textBoxSlider_3.Text = DefaultValues[3];
                    textBoxSlider_4.Text = DefaultValues[4];

                    if (!Properties.Settings.Default.useDarkMode)
                    {
                        textBoxSlider_0.BackColor = Color.FromArgb(240, 240, 240); // Grayed out; cannot be overwritten by custom values
                    }

                    panel_Publisher_Scroll.Visible = true;
                    break;

                case PolicyCustomRules.RuleType.FolderPath:

                    // User wants to create rule by folder level
                    PolicyCustomRule.ReferenceFile = GetFolderLocation();
                    DefaultValues[4] = PolicyCustomRule.ReferenceFile + "\\*";
                    AllFilesinFolder = new List<string>();
                    if (PolicyCustomRule.ReferenceFile == String.Empty)
                    {
                        break;
                    }

                    // Add an asterix to the end of the path to allow all 
                    PolicyCustomRule.ReferenceFile += "\\*";

                    // Custom rule in progress
                    _MainWindow.CustomRuleinProgress = true;

                    textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;

                    // Show right side of the text
                    if (textBox_ReferenceFile.TextLength > 0)
                    {
                        textBox_ReferenceFile.SelectionStart = textBox_ReferenceFile.TextLength - 1;
                        textBox_ReferenceFile.ScrollToCaret();
                    }

                    break;

                case PolicyCustomRules.RuleType.FilePath:

                    // UI updates
                    radioButton_File.Checked = true;
                    textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;

                    // Show right side of the text
                    if (textBox_ReferenceFile.TextLength > 0)
                    {
                        textBox_ReferenceFile.SelectionStart = textBox_ReferenceFile.TextLength - 1;
                        textBox_ReferenceFile.ScrollToCaret();
                    }

                    panel_Publisher_Scroll.Visible = false;
                    break;

                case PolicyCustomRules.RuleType.FileAttributes:

                    textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;

                    // Show right side of the text
                    if (textBox_ReferenceFile.TextLength > 0)
                    {
                        textBox_ReferenceFile.SelectionStart = textBox_ReferenceFile.TextLength - 1;
                        textBox_ReferenceFile.ScrollToCaret();
                    }

                    checkBoxAttribute0.Text = "Original filename:";
                    checkBoxAttribute1.Text = "File description:";
                    checkBoxAttribute2.Text = "Product name:";
                    checkBoxAttribute3.Text = "Internal name:";

                    // Set checkbox states to all disabled -- allow user to select the ones desired
                    checkBoxAttribute0.Checked = false;
                    checkBoxAttribute1.Checked = false;
                    checkBoxAttribute2.Checked = false;
                    checkBoxAttribute3.Checked = false;
                    checkBoxAttribute4.Checked = false;

                    // Set checkbox struct
                    PolicyCustomRule.CheckboxCheckStates.checkBox0 = false;
                    PolicyCustomRule.CheckboxCheckStates.checkBox1 = false;
                    PolicyCustomRule.CheckboxCheckStates.checkBox2 = false;
                    PolicyCustomRule.CheckboxCheckStates.checkBox3 = false;
                    PolicyCustomRule.CheckboxCheckStates.checkBox4 = false;

                    // Hide version boxes
                    textBoxSlider_4.Visible = false;
                    checkBoxAttribute4.Visible = false;

                    // Set defaults to restore to if custom values is ever reset
                    DefaultValues[0] = PolicyCustomRule.FileInfo["OriginalFilename"];
                    DefaultValues[1] = PolicyCustomRule.FileInfo["FileDescription"];
                    DefaultValues[2] = PolicyCustomRule.FileInfo["ProductName"];
                    DefaultValues[3] = PolicyCustomRule.FileInfo["InternalName"];

                    textBoxSlider_0.Text = DefaultValues[0];
                    textBoxSlider_1.Text = DefaultValues[1];
                    textBoxSlider_2.Text = DefaultValues[2];
                    textBoxSlider_3.Text = DefaultValues[3];

                    panel_Publisher_Scroll.Visible = true;
                    break;

                case PolicyCustomRules.RuleType.Hash:

                    // UI updates
                    panel_Publisher_Scroll.Visible = false;
                    textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    // Show right side of the text
                    if (textBox_ReferenceFile.TextLength > 0)
                    {
                        textBox_ReferenceFile.SelectionStart = textBox_ReferenceFile.TextLength - 1;
                        textBox_ReferenceFile.ScrollToCaret();
                    }

                    break;

                case PolicyCustomRules.RuleType.FolderScan:

                    // User wants to create rules for each file in the selected folder
                    PolicyCustomRule.ReferenceFile = GetFolderLocation();
                    if (PolicyCustomRule.ReferenceFile == String.Empty)
                    {
                        break;
                    }

                    // UI updates
                    textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;

                    // Show right side of the text
                    if (textBox_ReferenceFile.TextLength > 0)
                    {
                        textBox_ReferenceFile.SelectionStart = textBox_ReferenceFile.TextLength - 1;
                        textBox_ReferenceFile.ScrollToCaret();
                    }

                    // Populate the Omit Paths CheckedListBox with the sub-directories found
                    string[] subPaths = Directory.GetDirectories(PolicyCustomRule.ReferenceFile);
                    if (subPaths.Length != 0)
                    {
                        foreach (string subPath in subPaths)
                        {
                            checkedListBoxOmitPaths.Items.Add(subPath, false); // set to unchecked by default
                        }
                    }

                    break;

                case PolicyCustomRules.RuleType.Certificate:

                    // UI updates
                    panel_Publisher_Scroll.Visible = false;
                    textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    
                    // Show right side of the text
                    if (textBox_ReferenceFile.TextLength > 0)
                    {
                        textBox_ReferenceFile.SelectionStart = textBox_ReferenceFile.TextLength - 1;
                        textBox_ReferenceFile.ScrollToCaret();
                    }

                    break;

            }
        }

        /// <summary>
        /// Flips the PolicyCustom RuleType from FilePath to FolderPath, and vice-versa. 
        /// Prompts user to select another reference path if flipping RuleType after reference
        /// has already been set. 
        /// </summary>
        private void FileButton_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_File.Checked)
            {
                PolicyCustomRule.Level = PolicyCustomRules.RuleLevel.FilePath;
                PolicyCustomRule.Type = PolicyCustomRules.RuleType.FilePath;
            }
            else
            {
                PolicyCustomRule.Level = PolicyCustomRules.RuleLevel.Folder;
                PolicyCustomRule.Type = PolicyCustomRules.RuleType.FolderPath;
            }

            // Check if user changed Rule Level after already browsing and selecting a reference file
            if (PolicyCustomRule.ReferenceFile != null)
            {
                Button_Browse_Click(sender, e);
            }
        }

        /// <summary>
        /// Opens the file dialog and grabs the file path for PEs only and checks if path exists. 
        /// </summary>
        /// <returns>Returns the full path+name of the file</returns>
        private string GetFileLocation()
        {
            // Open file dialog to get file or folder path
            return Helper.BrowseForSingleFile(Properties.Resources.OpenPEFileDialogTitle, Helper.BrowseFileType.PEFile);
        }

        /// <summary>
        /// Resets the Slider UI to empty values 
        /// </summary>
        private void ResetSliderUI()
        {
            textBoxSlider_0.Clear();
            textBoxSlider_1.Clear();
            textBoxSlider_2.Clear();
            textBoxSlider_3.Clear();
            textBoxSlider_4.Clear();
        }

        /// <summary>
        /// Opens the folder dialog and grabs the folder path. Requires Folder to be toggled when Browse button 
        /// is selected. 
        /// </summary>
        /// <returns>Returns the full path of the folder</returns>
        private string GetFolderLocation()
        {
            return Helper.GetFolderPath("Browse for a folder to use as a reference for the rule.");
        }

        /// <summary>
        /// Fires at Form Closing event. Determine whether to close the panel or cancel in case of active rule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomRulesPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (RuleInEdit)
            {
                DialogResult res = MessageBox.Show("Are you sure you want to abandon rule creation?",
                                                    "Confirmation",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    SigningControl.CustomRulesPanel_Closing();
                    _MainWindow.CustomRuleinProgress = false;
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = false;
            }
        }

        /// <summary>
        /// Displays error message, cleans up the UI and resets the PolicyCustomRule object
        /// </summary>
        private void UnSupportedCryptoCleanUp()
        {
            DialogResult res = MessageBox.Show(Properties.Resources.UnsupportedCrypto_Error,
                                                        "Unsupported Cryptographic Algorithm Found",
                                                        MessageBoxButtons.OK,
                                                        MessageBoxIcon.Error);

            // Renew the custom rule instance
            PolicyCustomRule = new PolicyCustomRules();

            // Reset UI view
            ClearCustomRulesPanel(true);
            _MainWindow.CustomRuleinProgress = false;
        }

        /// <summary>
        /// UI Cleanup when a certificate rule type is chosen but the file is not a certificate
        /// </summary>
        private void CertificateUICleanUp()
        {
            DialogResult res = MessageBox.Show(Properties.Resources.CertificateParsing_Error,
                                                        "Unsupported Certificate File Found",
                                                        MessageBoxButtons.OK,
                                                        MessageBoxIcon.Error);

            // Renew the custom rule instance
            PolicyCustomRule = new PolicyCustomRules();

            // Reset UI view
            ClearCustomRulesPanel(true);
            _MainWindow.CustomRuleinProgress = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // e.Cancel = true;
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Next button clicked by user. Check UI state before proceeding to Exceptions panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Next_Click(object sender, EventArgs e)
        {
            // Assert only signer rules can be excepted in WDAC
            if (PolicyCustomRule.Type != PolicyCustomRules.RuleType.Publisher)
            {
                label_Error.Visible = true;
                label_Error.Text = Properties.Resources.RuleTypeNoExceptionAllowed;
                Logger.Log.AddWarningMsg("Cannot proceed to Exceptions Panel. Path and hash rules cannot be excepted.");
                return;
            }

            // Check required fields - that a reference file is selected
            // Show the exception UI
            if (PolicyCustomRule.Type != PolicyCustomRules.RuleType.None
                && PolicyCustomRule.ReferenceFile != null)
            {
                state = UIState.RuleExceptions;
                SetUIState();

                // Enable Back & exception button

                // Dark Mode
                if (Properties.Settings.Default.useDarkMode)
                {
                    button_Back.FlatAppearance.BorderColor = Color.DodgerBlue;
                    button_Back.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                    button_Back.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                    button_Back.FlatStyle = FlatStyle.Flat;
                    button_Back.ForeColor = Color.DodgerBlue;
                    button_Back.BackColor = Color.Transparent;
                    button_Back.Enabled = true;
                    button_AddException.FlatAppearance.BorderColor = Color.DodgerBlue;
                    button_AddException.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                    button_AddException.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                    button_AddException.FlatStyle = FlatStyle.Flat;
                    button_AddException.ForeColor = Color.DodgerBlue;
                    button_AddException.BackColor = Color.Transparent;
                    button_AddException.Enabled = true;
                    // Disable next button 
                    button_Next.FlatAppearance.BorderColor = Color.Gray;
                    button_Next.Enabled = false;
                }

                // Light Mode
                else
                {
                    button_Back.FlatAppearance.BorderColor = Color.Black;
                    button_Back.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                    button_Back.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                    button_Back.FlatStyle = FlatStyle.Flat;
                    button_Back.ForeColor = Color.Black;
                    button_Back.BackColor = Color.WhiteSmoke;
                    button_Back.Enabled = true;
                    button_AddException.FlatAppearance.BorderColor = Color.Black;
                    button_AddException.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                    button_AddException.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                    button_AddException.FlatStyle = FlatStyle.Flat;
                    button_AddException.ForeColor = Color.Black;
                    button_AddException.BackColor = Color.WhiteSmoke;
                    button_AddException.Enabled = true;
                    // Disable next button 
                    button_Next.FlatAppearance.BorderColor = Color.LightGray;
                    button_Next.Enabled = false;
                }

            }
            else
            {
                SetLabel_ErrorText(Properties.Resources.InvalidCustomRule_Error, false);
            }
        }

        /// <summary>
        /// Back button has been selected. Show CustomRules if sitting on Exceptions Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Back_Click(object sender, EventArgs e)
        {
            state = UIState.RuleConditions;
            SetUIState();

            // Enable next button 

            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                button_Next.FlatAppearance.BorderColor = Color.DodgerBlue;
                button_Next.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                button_Next.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                button_Next.FlatStyle = FlatStyle.Flat;
                button_Next.ForeColor = Color.DodgerBlue;
                button_Next.BackColor = Color.Transparent;
                button_Next.Enabled = true;
                // Disable Back button
                button_Back.FlatAppearance.BorderColor = Color.Gray;
                button_Back.Enabled = false;
            }

            // Light Mode
            else
            {
                button_Next.FlatAppearance.BorderColor = Color.Black;
                button_Next.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                button_Next.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                button_Next.FlatStyle = FlatStyle.Flat;
                button_Next.ForeColor = Color.Black;
                button_Next.BackColor = Color.WhiteSmoke;
                button_Next.Enabled = true;
                // Disable Back button
                //this.button_Back.ForeColor = Color.Gray;
                button_Back.FlatAppearance.BorderColor = Color.LightGray;
                button_Back.Enabled = false;
            }
        }

        /// <summary>
        /// Sets the state of the UI. Will show the CustomRule or Exceptions control panel
        /// </summary>
        private void SetUIState()
        {
            // bring info label to front
            Controls.Add(label_Error);
            label_Error.Focus();
            label_Error.BringToFront();

            switch (state)
            {
                case UIState.RuleConditions:

                    // Hide the Exceptions User Control 
                    exceptionsControl.Hide();
                    exceptionsControl.SendToBack();
                    redoRequired = false; // Reset flag as returning back to rule conditions user control should not auto trigger a redo

                    // Enable side panel
                    // Show control panel
                    Controls.Add(control_Panel);
                    control_Panel.BringToFront();
                    control_Panel.Focus();

                    // Set the control highlight rectangle pos
                    controlHighlight_Panel.Location = new Point(3, 138);
                    controlHighlight_Panel.BringToFront();
                    controlHighlight_Panel.Focus();

                    // Show header panel                        
                    headerLabel.Text = "Custom Rule Conditions";

                    break;

                case UIState.RuleExceptions:
                    {
                        //TODO: check if create new exceptions_control or show existing one
                        if (exceptionsControl == null || redoRequired == true)
                        {
                            exceptionsControl = new Exceptions_Control(this);
                            Controls.Add(exceptionsControl);
                        }
                        else
                        {
                            // show existing one
                        }

                        // Show the exceptions control
                        exceptionsControl.Show();
                        exceptionsControl.BringToFront();
                        exceptionsControl.Focus();

                        // Enable side panel
                        // Show control panel
                        Controls.Add(control_Panel);
                        control_Panel.BringToFront();
                        control_Panel.Focus();

                        // Set the control highlight rectangle pos
                        controlHighlight_Panel.Location = new Point(3, 226);
                        controlHighlight_Panel.BringToFront();
                        controlHighlight_Panel.Focus();

                        // Show header panel                        
                        headerLabel.Text = "Custom Rule Exceptions";
                        Controls.Add(headerLabel);
                        headerLabel.BringToFront();
                        headerLabel.Focus();
                    }

                    break;

                default:

                    break;
            }

            // Show buttons
            button_Next.BringToFront();
            button_Next.Focus();

            button_CreateRule.BringToFront();
            button_CreateRule.Focus();

            button_Back.BringToFront();
            button_Back.Focus();

            button_AddException.BringToFront();
            button_AddException.Focus();
        }

        /// <summary>
        /// Sets the error text string and whether the error label should persist/is sticky
        /// </summary>
        /// <param name="errorText"></param>
        /// <param name="shouldPersist"></param>
        public void SetLabel_ErrorText(string errorText, bool shouldPersist = false)
        {
            label_Error.Focus();
            label_Error.BringToFront();

            label_Error.Text = errorText;
            label_Error.Visible = true;

            if (!shouldPersist)
            {
                // Timer settingsUpdateNotificationTimer = new Timer();
                // settingsUpdateNotificationTimer.Interval = (5000);
                // settingsUpdateNotificationTimer.Tick += new EventHandler(SettingUpdateTimer_Tick);
                // settingsUpdateNotificationTimer.Start();
            }
        }

        /// <summary>
        /// Clear the error label text and set to invisible
        /// </summary>
        public void ClearLabel_ErrorText()
        {
            label_Error.Text = "";
            label_Error.Visible = false;
        }

        /// <summary>
        /// Shows the invalid error label when a checkbox is selected with invalid text
        /// </summary>
        private void ShowInvalidErrorLabel()
        {
            label_Error.Visible = true;
            label_Error.Text = Properties.Resources.InvalidAttributeSelection_Error;
            Logger.Log.AddWarningMsg("Create button rule selected with an empty file attribute.");
        }

        /// <summary>
        /// Add exception button clicked. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AddException_Click(object sender, EventArgs e)
        {
            exceptionsControl.AddException();
        }

        /// <summary>
        /// Deny radio button selected. Custom rule is a deny Rule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Deny_Click(object sender, EventArgs e)
        {
            // Assert that supplemental policy edit/new workflow cannot create deny rules
            if (Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy)
            {
                var res = MessageBox.Show(Properties.Resources.SupplementalPolicyDenyRuleError,
                                          "Invalid Option",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Exclamation);
                radioButton_Deny.Checked = false;
                radioButton_Allow.Checked = true;
                return;
            }

            PolicyCustomRule.Permission = PolicyCustomRules.RulePermission.Deny;
            Logger.Log.AddInfoMsg("Rule Permission set to " + PolicyCustomRule.Permission.ToString());

            // Returned back from exceptions to change Rule Type - Redo is required
            if (exceptionsControl != null)
            {
                redoRequired = true;
            }
        }

        /// <summary>
        /// Allow radio button selected. Custom rule is an Allow Rule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Allow_Click(object sender, EventArgs e)
        {
            PolicyCustomRule.Permission = PolicyCustomRules.RulePermission.Allow;
            Logger.Log.AddInfoMsg("Rule Permission set to " + PolicyCustomRule.Permission.ToString());

            // Returned back from exceptions to change Rule Type - Redo is required
            if (exceptionsControl != null)
            {
                redoRequired = true;
            }
        }

        /// <summary>
        /// The Use Custom Rules checkbox has been clicked by the user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UseRuleCustomValues(object sender, EventArgs e)
        {
            // Set the UI first
            if (checkBox_CustomValues.Checked)
            {
                if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.FileAttributes)
                {
                    SetTextBoxStates(true, PolicyCustomRules.RuleType.FileAttributes);

                    // Set the custom values based on existing
                    PolicyCustomRule.CustomValues.FileName = textBoxSlider_0.Text;
                    PolicyCustomRule.CustomValues.Description = textBoxSlider_1.Text;
                    PolicyCustomRule.CustomValues.ProductName = textBoxSlider_2.Text;
                    PolicyCustomRule.CustomValues.InternalName = textBoxSlider_3.Text;
                }
                else
                {
                    // Set textbox states to write, enabled, and white back color
                    SetTextBoxStates(true);

                    // Set the custom values based on existing
                    PolicyCustomRule.CustomValues.Publisher = textBoxSlider_1.Text;
                    PolicyCustomRule.CustomValues.ProductName = textBoxSlider_2.Text;
                    PolicyCustomRule.CustomValues.FileName = textBoxSlider_3.Text;
                    PolicyCustomRule.CustomValues.MinVersion = textBoxSlider_4.Text;
                }

                PolicyCustomRule.UsingCustomValues = true;
            }
            else
            {
                // Clear error if applicable
                ClearLabel_ErrorText();

                // Set text values back to default
                SetTextBoxStates(false);

                // Format the version text boxes
                textBoxSlider_4.Size = textBoxSlider_0.Size;
                textBox_MaxVersion.Visible = false;
                label_To.Visible = false;

                // Flip the label back to Min Version (from Version Range) if Pubisher rule
                if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
                {
                    checkBoxAttribute4.Text = "Min version:";
                }

                // Re-populate the text boxes
                textBoxSlider_0.Text = DefaultValues[0];
                textBoxSlider_1.Text = DefaultValues[1];
                textBoxSlider_2.Text = DefaultValues[2];
                textBoxSlider_3.Text = DefaultValues[3];
                textBoxSlider_4.Text = DefaultValues[4];

                PolicyCustomRule.UsingCustomValues = false;
            }
        }

        /// <summary>
        /// Set the UI states (enabled, readonly) and appearances for the publisher or file attribute textboxes
        /// </summary>
        /// <param name="enabled"></param>
        private void SetTextBoxStates(bool enabled, PolicyCustomRules.RuleType ruleType = PolicyCustomRules.RuleType.Publisher)
        {
            if (enabled)
            {
                // If enabled, allow user input
                textBoxSlider_0.ReadOnly = true; // Custom text values for PCA are not supported
                textBoxSlider_1.ReadOnly = false; // Publisher | File description
                textBoxSlider_2.ReadOnly = false; // Product   | Product name
                textBoxSlider_3.ReadOnly = false; // Filename  | Internal name
                textBoxSlider_4.ReadOnly = false; // Min version
                textBox_MaxVersion.ReadOnly = false;

                textBoxSlider_0.Enabled = false;
                textBoxSlider_1.Enabled = true;
                textBoxSlider_2.Enabled = true;
                textBoxSlider_3.Enabled = true;
                textBoxSlider_4.Enabled = true;
                textBox_MaxVersion.Enabled = true;

                // Set back color to white to help user determine boxes are userwriteable
                if (!Properties.Settings.Default.useDarkMode)
                {
                    textBoxSlider_0.BackColor = Color.White;
                    textBoxSlider_1.BackColor = Color.White;
                    textBoxSlider_2.BackColor = Color.White;
                    textBoxSlider_3.BackColor = Color.White;
                    textBoxSlider_4.BackColor = Color.White;
                    textBox_MaxVersion.BackColor = Color.White;

                    // Text color
                    textBoxSlider_0.ForeColor = Color.Black;
                    textBoxSlider_1.ForeColor = Color.Black;
                    textBoxSlider_2.ForeColor = Color.Black;
                    textBoxSlider_3.ForeColor = Color.Black;
                    textBoxSlider_4.ForeColor = Color.Black;
                    textBox_MaxVersion.ForeColor = Color.Black;
                }
                else
                {
                    textBoxSlider_0.BackColor = Color.FromArgb(15, 15, 15);
                    textBoxSlider_1.BackColor = Color.FromArgb(15, 15, 15);
                    textBoxSlider_2.BackColor = Color.FromArgb(15, 15, 15);
                    textBoxSlider_3.BackColor = Color.FromArgb(15, 15, 15);
                    textBoxSlider_4.BackColor = Color.FromArgb(15, 15, 15);
                    textBox_MaxVersion.BackColor = Color.FromArgb(15, 15, 15);

                    // Text color
                    textBoxSlider_0.ForeColor = Color.White;
                    textBoxSlider_1.ForeColor = Color.White;
                    textBoxSlider_2.ForeColor = Color.White;
                    textBoxSlider_3.ForeColor = Color.White;
                    textBoxSlider_4.ForeColor = Color.White;
                    textBox_MaxVersion.ForeColor = Color.White;
                }


                // Format the version text boxes
                textBoxSlider_4.Visible = true;
                textBox_MaxVersion.Visible = true;
                label_To.Visible = true;

                textBoxSlider_4.Size = textBox_MaxVersion.Size;
                checkBoxAttribute4.Text = "Version range:";

                // If RuleType == FileAttributes, ensure first textbox is user writeable
                if (ruleType == PolicyCustomRules.RuleType.FileAttributes)
                {
                    textBoxSlider_0.ReadOnly = false;
                    textBoxSlider_0.Enabled = true;

                    // Hide version boxes
                    textBoxSlider_4.Visible = false;
                    textBox_MaxVersion.Visible = false;
                    label_To.Visible = false;
                }
            }
            else
            {
                // Set to read only if disabled
                textBoxSlider_0.ReadOnly = true;
                textBoxSlider_1.ReadOnly = true;
                textBoxSlider_4.ReadOnly = true;
                textBoxSlider_3.ReadOnly = true;
                textBox_MaxVersion.ReadOnly = true;

                // Set to not enabled so accepts no user interaction
                textBoxSlider_0.Enabled = false;
                textBoxSlider_1.Enabled = false;
                textBoxSlider_2.Enabled = false;
                textBoxSlider_3.Enabled = false;
                textBoxSlider_4.Enabled = false;
                textBox_MaxVersion.Enabled = false;

                // Set back to default color
                List<TextBox> textBoxes = new List<TextBox>();
                GetTextBoxesRecursive(this, textBoxes);
                SetTextBoxesColor(textBoxes);
            }

        }

        // Action handlers for custom text in the publiser and file attributes text fields

        /// <summary>
        /// Fourth textbox text has been modified by the user. Picks up the input 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxSlider_4_TextChanged(object sender, EventArgs e)
        {
            // Min version (publisher) or InternalName (file attributes)
            // Break if not using custom values. This will be reached during setting values once proto file is chosen

            if (!PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                PolicyCustomRule.CustomValues.MinVersion = textBoxSlider_4.Text;
            }
        }

        /// <summary>
        /// Third textbox text has been modified by the user. Picks up the input 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxSlider_3_TextChanged(object sender, EventArgs e)
        {
            // Filename (publisher) or InternalName (file attributes)
            // Break if not using custom values. This will be reached during setting values once proto file is chosen

            if (!PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                PolicyCustomRule.CustomValues.FileName = textBoxSlider_3.Text;
            }
            else
            {
                PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.InternalName);
                PolicyCustomRule.CustomValues.InternalName = textBoxSlider_3.Text;
            }
        }

        /// <summary>
        /// Second textbox text has been modified by the user. Picks up the input 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxSlider_2_TextChanged(object sender, EventArgs e)
        {
            // Version (publisher) or ProductName (file attributes)
            // Break if not using custom values. This will be reached during setting values once proto file is chosen

            if (!PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                PolicyCustomRule.CustomValues.ProductName = textBoxSlider_2.Text;
            }
            else
            {
                PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.ProductName);
                PolicyCustomRule.CustomValues.ProductName = textBoxSlider_2.Text;
            }
        }

        /// <summary>
        /// Second textbox text has been modified by the user. Picks up the input 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxSlider_1_TextChanged(object sender, EventArgs e)
        {
            // Leaf cert publisher (publisher) or Description (file attributes)
            // Break if not using custom values. This will be reached during setting values once proto file is chosen

            if (!PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                PolicyCustomRule.CustomValues.Publisher = textBoxSlider_1.Text;
            }
            else
            {
                PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FileDescription);
                PolicyCustomRule.CustomValues.Description = textBoxSlider_1.Text;
            }

        }

        /// <summary>
        /// First textbox text has been modified by the user. Picks up the input 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxSlider_0_TextChanged(object sender, EventArgs e)
        {
            /// Only accessible by File Attributes
            // Original filename (file attributes)
            // Break if not using custom values. This will be reached during setting values once proto file is chosen

            if (!PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            PolicyCustomRule.CustomValues.FileName = textBoxSlider_0.Text;
            PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.OriginalFileName);
        }

        /// <summary>
        /// The max version textbox has been modified by the user. Picks up the input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_MaxVersion_TextChanged(object sender, EventArgs e)
        {
            // Only accessible by publisher
            // Set Custom Values.MaxValue
            if (!PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            PolicyCustomRule.CustomValues.MaxVersion = textBox_MaxVersion.Text;
        }

        /// <summary>
        /// Text has been modified in the Reference Textbox. Picks up the user input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReferenceFileTextChanged(object sender, EventArgs e)
        {
            if (PolicyCustomRule.UsingCustomValues)
            {
                PolicyCustomRule.CustomValues.Path = textBox_ReferenceFile.Text;
            }
        }

        /// <summary>
        /// User has selected the UseCustomPath checkbox. Could be use custom path rules or custom hash rule.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UseCustomPath(object sender, EventArgs e)
        {
            if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Hash)
            {
                if (checkBox_CustomPath.Checked)
                {
                    // Dark Mode
                    if (Properties.Settings.Default.useDarkMode)
                    {
                        richTextBox_CustomHashes.Visible = true;
                        richTextBox_CustomHashes.Location = panel_Publisher_Scroll.Location;
                        richTextBox_CustomHashes.Tag = "Title";
                        richTextBox_CustomHashes.BackColor = Color.FromArgb(15, 15, 15);
                        richTextBox_CustomHashes.ForeColor = Color.White;

                        PolicyCustomRule.UsingCustomValues = true;
                        textBox_ReferenceFile.Text = String.Empty;
                    }

                    // Light Mode
                    else
                    {
                        richTextBox_CustomHashes.Visible = true;
                        richTextBox_CustomHashes.Location = panel_Publisher_Scroll.Location;
                        richTextBox_CustomHashes.Tag = "Title";
                        richTextBox_CustomHashes.BackColor = Color.White;
                        richTextBox_CustomHashes.ForeColor = Color.Black;

                        PolicyCustomRule.UsingCustomValues = true;
                        textBox_ReferenceFile.Text = String.Empty;
                    }
                }
                else
                {
                    richTextBox_CustomHashes.Visible = false;
                    PolicyCustomRule.UsingCustomValues = false;
                }
            }
            else
            {
                if (checkBox_CustomPath.Checked)
                {
                    // Dark Mode
                    if (Properties.Settings.Default.useDarkMode)
                    {
                        PolicyCustomRule.UsingCustomValues = true;
                        textBox_ReferenceFile.ReadOnly = false;
                        textBox_ReferenceFile.Enabled = true;
                        textBox_ReferenceFile.BackColor = Color.FromArgb(15, 15, 15);
                    }

                    // Light Mode
                    else
                    {
                        PolicyCustomRule.UsingCustomValues = true;
                        textBox_ReferenceFile.ReadOnly = false;
                        textBox_ReferenceFile.Enabled = true;
                        textBox_ReferenceFile.BackColor = Color.White;
                    }
                }
                else
                {
                    // Dark Mode
                    if (Properties.Settings.Default.useDarkMode)
                    {
                        PolicyCustomRule.UsingCustomValues = false;
                        textBox_ReferenceFile.ReadOnly = true;
                        textBox_ReferenceFile.Enabled = false;
                        textBox_ReferenceFile.BackColor = Color.FromArgb(15, 15, 15);
                    }

                    // Light Mode
                    else
                    {
                        PolicyCustomRule.UsingCustomValues = false;
                        textBox_ReferenceFile.ReadOnly = true;
                        textBox_ReferenceFile.Enabled = false;
                        textBox_ReferenceFile.BackColor = Color.White;
                    }

                    // Set back to the reference file path
                    if (DefaultValues[4] != null && DefaultValues[4].Length > 0)
                    {
                        textBox_ReferenceFile.Text = DefaultValues[4];

                        textBox_ReferenceFile.SelectionStart = textBox_ReferenceFile.TextLength - 1;
                        textBox_ReferenceFile.ScrollToCaret();
                    }
                }
            }

        }

        /// <summary>
        /// Updates the custom hashes values when the hash table has been modified by user input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RichTextBox_CustomHashes_Click(object sender, EventArgs e)
        {
            if (richTextBox_CustomHashes.Tag.ToString() == "Title")
            {
                richTextBox_CustomHashes.ResetText();
                richTextBox_CustomHashes.Tag = "Values";
            }
        }

        /// <summary>
        /// Starts Appx search process once user selects search button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Packaged_App_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //enter key is down
                ButtonSearch_Click(sender, e);
            }
        }

        /// <summary>
        /// Event handler to begin searching for packaged apps
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSearch_Click(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(textBox_Packaged_App.Text))
            {
                label_Error.Visible = true;
                label_Error.Text = Properties.Resources.PFNSearch_Warn;
                Logger.Log.AddWarningMsg("Empty packaged app search criteria");
                return;
            }

            // Check whether we are creating a PFN based on arbitrary package name
            if (!checkBox_CustomPFN.Checked)
            {
                // Searching for PFN on device
                // Prep UI
                panel_Progress.Visible = true;
                panel_Progress.BringToFront();
                label_Error.Visible = false;

                // Create background worker to display updates to UI
                if (!backgroundWorker.IsBusy)
                {
                    backgroundWorker.RunWorkerAsync();
                }
            }
            else
            {
                // Using arbitrary/custom PFN in rule creation
                // Add PFN to list with checkbox checked
                string arbitraryPFN = textBox_Packaged_App.Text;
                if (arbitraryPFN.Length > 3)
                {
                    arbitraryPFN = String.Concat(arbitraryPFN.Where(c => !Char.IsWhiteSpace(c)));
                    checkedListBoxPackagedApps.Items.Add(arbitraryPFN, true);

                    // Once added to the table, clear the textbox automatically
                    textBox_Packaged_App.Clear();
                }
                else
                {
                    label_Error.Visible = true;
                    label_Error.Text = "Package Family name must be at least 3 characters.";
                }
            }

        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {


        }

        /// <summary>
        /// Runs the Get-AppxPackage -Name command and parses the output
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                FoundPackages = Helper.GetAppxPackages(textBox_Packaged_App.Text);
            }
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg(String.Format("Exception encountered in MergeCustomRulesPolicy(): {0}", exp));
            }
        }

        /// <summary>
        /// Lists all of the Appx packages and sorts them to display to user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Remove GIF // Update UI 
            panel_Progress.Visible = false;

            // Unsuccessful conversion
            if (e.Error != null)
            {
                Logger.Log.AddErrorMsg("ProcessPolicy() caught the following exception ", e.Error);

            }

            Logger.Log.AddNewSeparationLine("Packaged App Searching Workflow -- DONE");

            // Check for the case where no packages were found and return
            if (FoundPackages.Count == 0)
            {
                label_Error.Visible = true;
                label_Error.Text = String.Format("No packages found with name: {0}", textBox_Packaged_App.Text);
                Logger.Log.AddWarningMsg(String.Format("No packaged apps found with name: {0}", textBox_Packaged_App.Text));
                return;
            }

            // Bring checkbox list to front. Sort keys to display alphabetically to user
            checkedListBoxPackagedApps.BringToFront();
            var sortedPackages = FoundPackages;
            sortedPackages.Sort();

            foreach (var key in sortedPackages)
            {
                checkedListBoxPackagedApps.Items.Add(key, false);
            }
        }

        /// <summary>
        /// Sets the UI for custom PFN rules. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Checkbox_CustomPFN_Checked(object sender, EventArgs e)
        {
            // If checked, update text on the 'Search' button
            // Hide the PFN search UI

            if (checkBox_CustomPFN.Checked)
            {
                buttonSearch.Text = "Create";
                PolicyCustomRule.UsingCustomValues = true;
            }

            // Else, return text to 'Search' button
            // Unhide the PFN search UI
            // If there are any checked boxes, clear the list of arbitrary/custom PFN rules after prompting user
            else
            {
                if (checkedListBoxPackagedApps.Items.Count > 0)
                {
                    DialogResult res = MessageBox.Show("You have active custom PFN rules that will be deleted. Are you sure you want to switch to default PFN rule creation?",
                                                        "Confirmation",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Question);

                    if (res == DialogResult.Yes)
                    {
                        buttonSearch.Text = "Search";
                        PolicyCustomRule.UsingCustomValues = false;
                        int n_Rules = checkedListBoxPackagedApps.Items.Count;

                        for (int j = 0; j < n_Rules; j++)
                        {
                            // Remove at the 0th index n_Rules times
                            checkedListBoxPackagedApps.Items.RemoveAt(0);
                        }
                    }
                    else
                    {
                        checkBox_CustomPFN.Checked = true;
                    }
                }
            }
        }

        /// <summary>
        /// EKU checkbox state changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxEkuStateChanged(object sender, EventArgs e)
        {
            // If user wants to use checkbox
            // Enable the textbox
            if (checkBoxEku.Checked)
            {
                textBoxEKU.Enabled = true;
                textBoxEKU.ReadOnly = false;
                textBoxEKU.BackColor = Color.White;
                PolicyCustomRule.EKUFriendly = String.Empty;
            }
            else
            {
                textBoxEKU.Enabled = false;
                textBoxEKU.ReadOnly = true;
                textBoxEKU.BackColor = SystemColors.Control;
                textBoxEKU.Text = String.Empty;
                PolicyCustomRule.EKUFriendly = String.Empty;

                // Reset the UsingCustomValues field iff not set custom using the checkbox
                if (!checkBox_CustomValues.Checked)
                {
                    PolicyCustomRule.UsingCustomValues = false;
                }
            }
        }

        /// <summary>
        /// EKU textbox value changed. User input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBoxEKU_TextChanged(object sender, EventArgs e)
        {
            PolicyCustomRule.EKUFriendly = textBoxEKU.Text;
        }

        /// <summary>
        /// Sets the PolicyCustomRule Checkbox4 state based on change in state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAttrib4CheckChanged(object sender, EventArgs e)
        {
            // Version
            if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                if (checkBoxAttribute4.Checked)
                {
                    if (textBoxSlider_4.Text != Properties.Resources.DefaultFileAttributeString
                        || String.IsNullOrEmpty(textBoxSlider_4.Text))
                    {
                        PolicyCustomRule.CheckboxCheckStates.checkBox4 = true;
                        ClearLabel_ErrorText();
                        return;
                    }
                    else
                    {
                        SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                    }
                }
            }

            checkBoxAttribute4.Checked = false;
            PolicyCustomRule.CheckboxCheckStates.checkBox4 = false;
        }

        /// <summary>
        /// Sets the PolicyCustomRule Checkbox3 state based on change in state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAttrib3CheckChanged(object sender, EventArgs e)
        {
            // File name || Internal name

            if (checkBoxAttribute3.Checked)
            {
                if (textBoxSlider_3.Text != Properties.Resources.DefaultFileAttributeString
                    || String.IsNullOrEmpty(textBoxSlider_3.Text))
                {
                    PolicyCustomRule.CheckboxCheckStates.checkBox3 = true;
                    ClearLabel_ErrorText();
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            checkBoxAttribute3.Checked = false;
            PolicyCustomRule.CheckboxCheckStates.checkBox3 = false;
        }

        /// <summary>
        /// Sets the PolicyCustomRule Checkbox2 state based on change in state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAttrib2CheckChanged(object sender, EventArgs e)
        {
            // Product name (Pub rule) || Product name

            if (checkBoxAttribute2.Checked)
            {
                if (textBoxSlider_2.Text != Properties.Resources.DefaultFileAttributeString
                    || String.IsNullOrEmpty(textBoxSlider_2.Text))
                {
                    ClearLabel_ErrorText();
                    PolicyCustomRule.CheckboxCheckStates.checkBox2 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            checkBoxAttribute2.Checked = false;
            PolicyCustomRule.CheckboxCheckStates.checkBox2 = false;
        }

        /// <summary>
        /// Sets the PolicyCustomRule Checkbox1 state based on change in state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAttrib1CheckChanged(object sender, EventArgs e)
        {
            // Publisher || File description

            if (checkBoxAttribute1.Checked)
            {
                if (textBoxSlider_1.Text != Properties.Resources.DefaultFileAttributeString
                    || String.IsNullOrEmpty(textBoxSlider_1.Text))
                {
                    ClearLabel_ErrorText();
                    PolicyCustomRule.CheckboxCheckStates.checkBox1 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            checkBoxAttribute1.Checked = false;
            PolicyCustomRule.CheckboxCheckStates.checkBox1 = false;
        }

        /// <summary>
        /// Sets the PolicyCustomRule Checkbox0 state based on change in state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAttrib0CheckChanged(object sender, EventArgs e)
        {
            // PCA Certificate || Original filename

            if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                // Cannot uncheck Issuing CA checkbox. Rule must include a root
                checkBoxAttribute0.Checked = true;
                PolicyCustomRule.CheckboxCheckStates.checkBox0 = true;
            }
            else // Original Filename
            {
                if (checkBoxAttribute0.Checked)
                {
                    if (textBoxSlider_0.Text != Properties.Resources.DefaultFileAttributeString
                        || String.IsNullOrEmpty(textBoxSlider_0.Text))
                    {
                        ClearLabel_ErrorText();
                        PolicyCustomRule.CheckboxCheckStates.checkBox0 = true;
                        return;
                    }
                    else
                    {
                        SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                    }
                }

                checkBoxAttribute0.Checked = false;
                PolicyCustomRule.CheckboxCheckStates.checkBox0 = false;
            }
        }

        /// <summary>
        /// Sets the signing scenario state for custom rules to affect user mode signing scenarios
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_userMode_CheckedChanged(object sender, EventArgs e)
        {
            // Set the state for the custom rule
            // If the policy doesn't support UMCI, prompt the user and set it
            if (checkBox_userMode.Checked)
            {
                if (!PolicyHelper.PolicyHasRule(Policy.PolicyRuleOptions, OptionType.EnabledUMCI))
                {
                    DialogResult res = MessageBox.Show("Your policy does not have User mode code integrity (UMCI) enabled so this UMCI rule will not be enforced. Would you like the Wizard to enable UMCI?",
                                                        "Proceed with UMCI Rule Creation?",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Question);

                    // Set UMCI
                    if (res == DialogResult.Yes)
                    {
                        RuleType umciRule = new RuleType();
                        umciRule.Item = OptionType.EnabledUMCI;
                        _MainWindow.Policy.PolicyRuleOptions.Add(umciRule);
                    }
                }

                PolicyCustomRule.SigningScenarioCheckStates.umciEnabled = true;
            }
            else
            {
                PolicyCustomRule.SigningScenarioCheckStates.umciEnabled = false;
            }
        }

        /// <summary>
        /// Sets the signing scenario state for custom rules to affect kernel signing scenarios
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_kernelMode_CheckedChanged(object sender, EventArgs e)
        {
            // Set the state for the custom rule
            // Assert path rules and packaged app rules cannot be used for kernel mode
            if (checkBox_kernelMode.Checked)
            {
                if (PolicyCustomRule.Type == PolicyCustomRules.RuleType.PackagedApp ||
                    PolicyCustomRule.Type == PolicyCustomRules.RuleType.FilePath ||
                    PolicyCustomRule.Type == PolicyCustomRules.RuleType.FolderPath)
                {
                    DialogResult res = MessageBox.Show(Properties.Resources.InvalidKMCIRule,
                                                        "Unsupported Kernel Rule Type",
                                                        MessageBoxButtons.OK,
                                                        MessageBoxIcon.Exclamation);

                    checkBox_kernelMode.Checked = false;
                    PolicyCustomRule.SigningScenarioCheckStates.kmciEnabled = false;
                }
                else
                {
                    PolicyCustomRule.SigningScenarioCheckStates.kmciEnabled = true;
                }
            }
            else
            {
                PolicyCustomRule.SigningScenarioCheckStates.kmciEnabled = false;
            }
        }

        /// <summary>
        /// Sets the default UI for the panel on loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoad(object sender, EventArgs e)
        {
            // Fix to load CustomRuleConditionsPanel UI in light or dark mode
            CustomRuleConditionsPanel_Validated(sender, e);

            // AppId Tagging Policies do not support kmci rules
            // Uncheck kmci and disable the checkbox
            if (Policy._PolicyType == WDAC_Policy.PolicyType.AppIdTaggingPolicy)
            {
                checkBox_kernelMode.Checked = false;
                checkBox_kernelMode.Enabled = false;
                checkBox_userMode.Checked = true;
                return;
            }

            // If the policy does not support UMCI, uncheck umci and check kmci
            if (!PolicyHelper.PolicyHasRule(Policy.PolicyRuleOptions, OptionType.EnabledUMCI))
            {
                checkBox_kernelMode.Checked = true;
                checkBox_userMode.Checked = false;
            }
            else
            {
                checkBox_kernelMode.Checked = false;
                checkBox_userMode.Checked = true;
            }
        }

        /// <summary>
        /// Shows the MS Doc article for COM Objects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelLearnMoreCom_Click(object sender, EventArgs e)
        {
            // Label for learn more about policy options clicked. Launch msft docs page. 
            try
            {
                Process.Start(new ProcessStartInfo(Properties.Resources.MSDocLink_ComObjects) { UseShellExecute = true });
            }
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg(String.Format("Launching {0} for policy options link encountered the following error", Properties.Resources.MSDocLink_ComObjects), exp);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxComProviderChanged(object sender, EventArgs e)
        {
            switch (comboBoxComProvider.SelectedIndex)
            {
                case 0:
                    PolicyCustomRule.COMObject.Provider = COM.ProviderType.PowerShell;
                    break;

                case 1:
                    PolicyCustomRule.COMObject.Provider = COM.ProviderType.WSH;
                    break;

                case 2:
                    PolicyCustomRule.COMObject.Provider = COM.ProviderType.IE;
                    break;

                case 3:
                    PolicyCustomRule.COMObject.Provider = COM.ProviderType.VBA;
                    break;

                case 4:
                    PolicyCustomRule.COMObject.Provider = COM.ProviderType.MSI;
                    break;

                case 5:
                    PolicyCustomRule.COMObject.Provider = COM.ProviderType.AllHostIds;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxComKeyTypeChanged(object sender, EventArgs e)
        {
            // All Keys
            if (comboBoxComKeyType.SelectedItem == comboBoxComKeyType.Items[0])
            {
                PolicyCustomRule.COMObject.Guid = Properties.Resources.ComObjectAllKeys;
                panelComKey.Visible = false;
            }
            // Custom Key
            else
            {
                panelComKey.Visible = true;
            }
        }


        /// <summary>
        /// Fires when the textbox is mouse clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComKeyMouseClick(object sender, MouseEventArgs e)
        {
            // Clear the textbox when the user selects it
            if (String.Equals(textBoxObjectKey.Text, Properties.Resources.ComInitialGuid))
            {
                textBoxObjectKey.Clear();
            }
        }

        /// <summary>
        /// Fires when user clicks on the learn more. Opens the WDAC file rule level webpage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelFolderScanLearnMore_Click(object sender, EventArgs e)
        {
            // Label for learn more about policy options clicked. Launch msft docs page. 
            try
            {
                Process.Start(new ProcessStartInfo(Properties.Resources.MSDocLink_RuleLevels) { UseShellExecute = true });
            }
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg(String.Format("Launching {0} for policy options link encountered the following error",
                                     Properties.Resources.MSDocLink_RuleLevels), exp);
            }
        }

        /// <summary>
        /// Fires when the row is selected down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RuleLevelsList_MouseDown(object sender, MouseEventArgs e)
        {
            if (checkedListBoxRuleLevels.SelectedItem == null || e.X < 15 || (e.X > 150 && e.X < 165)) return; // e.X < 15 - left most column checkboxes. 150 < e.X < 165 - right most checkboxes
            checkedListBoxRuleLevels.DoDragDrop(checkedListBoxRuleLevels.SelectedItem, DragDropEffects.Move);
        }

        private void RuleLevelsList_DragDropDone(object sender, DragEventArgs e)
        {
            Point point = checkedListBoxRuleLevels.PointToClient(new Point(e.X, e.Y));
            int index = checkedListBoxRuleLevels.IndexFromPoint(point);
            if (index < 0) index = checkedListBoxRuleLevels.Items.Count - 1;
            bool isChecked = checkedListBoxRuleLevels.GetItemChecked(index);

            object data = checkedListBoxRuleLevels.SelectedItem;
            checkedListBoxRuleLevels.Items.Remove(data);
            checkedListBoxRuleLevels.Items.Insert(index, data);
            checkedListBoxRuleLevels.SetItemChecked(index, isChecked);
        }

        private void RuleLevelsList_DragInProgress(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }


        /// <summary>
        /// Opens the AppIdTagging Microsoft Learn doc
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppIdTaggingLearnMoreLink_Click(object sender, EventArgs e)
        {
            // Label for learn more about policy options clicked. Launch msft docs page. 
            try
            {
                Process.Start(new ProcessStartInfo(Properties.Resources.AppIdTaggingDocsLink) { UseShellExecute = true });
            }
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg($"Launching {Properties.Resources.AppIdTaggingDocsLink} for policy options link encountered the following error", exp);
            }
        }

        /// <summary>
        /// Fires on Load, Paint, Refresh. 
        /// Sets the colors on the UI elements for Dark and Light mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomRuleConditionsPanel_Validated(object sender, EventArgs e)
        {
            // Set Controls Color (e.g. Panels, buttons)
            SetControlsColor();

            // Set UI for the 'button_Browse' Button
            SetButtonUI(button_Back);
            SetButtonUI(button_Next);
            SetButtonUI(button_AddException);
            SetButtonUI(button_CreateRule);
            SetButtonUI(button_Browse);
            SetButtonUI(buttonSearch);

            // Set Textboxes Color
            List<TextBox> textBoxes = new List<TextBox>();
            GetTextBoxesRecursive(this, textBoxes);
            SetTextBoxesColor(textBoxes);

            // Set checkedListBoxes Color
            List<CheckedListBox> checkedListBoxes = new List<CheckedListBox>();
            GetCheckedListBoxesRecursive(this, checkedListBoxes);
            SetCheckedListBoxesColor(checkedListBoxes);

            // Set Comboboxes Color
            List<ComboBox> comboBoxes = new List<ComboBox>();
            GetComboBoxesRecursive(this, comboBoxes);
            SetComboboxesColor(comboBoxes);

            // Set Labels Color
            List<Label> labels = new List<Label>();
            GetLabelsRecursive(this, labels);
            SetLabelsColor(labels);

            // Set Radio Buttons Color
            List<RadioButton> radioButtons = new List<RadioButton>();
            GetRadioButtonsRecursive(this, radioButtons);
            SetRadioButtonsColor(radioButtons);

            // Set Side Panel
            SetSidePanelColor();

            // Set PolicyType Form back color
            SetFormBackColor();
        }

        /// <summary>
        /// Public method to call the _Paint method. Triggered by the signing rules control
        /// </summary>
        public void ForceRepaint()
        {
            CustomRuleConditionsPanel_Validated(null, null);
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
        /// Sets the color of the controls
        /// </summary>
        /// <param name="labels"></param>
        private void SetControlsColor()
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                foreach (Control control in Controls)
                {
                    // Buttons
                    if (control is Button button
                        && (button.Tag == null || button.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        button.ForeColor = Color.White;
                        button.BackColor = Color.FromArgb(15, 15, 15);
                    }

                    // Panels
                    else if (control is Panel panel
                        && (panel.Tag == null || panel.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        panel.ForeColor = Color.White;
                        panel.BackColor = Color.FromArgb(15, 15, 15);
                    }

                    // Checkboxes
                    else if (control is CheckBox checkBox
                        && (checkBox.Tag == null || checkBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        checkBox.ForeColor = Color.White;
                        checkBox.BackColor = Color.FromArgb(15, 15, 15);
                    }
                }
            }

            // Light Mode
            else
            {
                foreach (Control control in Controls)
                {
                    // Buttons
                    if (control is Button button
                        && (button.Tag == null || button.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        button.ForeColor = Color.Black;
                        button.BackColor = Color.White;
                    }

                    // Panels
                    else if (control is Panel panel
                        && (panel.Tag == null || panel.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        panel.ForeColor = Color.Black;
                        panel.BackColor = Color.White;
                    }

                    // Checkboxes
                    else if (control is CheckBox checkBox
                        && (checkBox.Tag == null || checkBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        checkBox.ForeColor = Color.Black;
                        checkBox.BackColor = Color.White;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the colors for the button_Browse Button which depends on the 
        /// state of Light and Dark Mode
        /// </summary>
        public void SetButtonUI(Button button)
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                button.FlatAppearance.BorderColor = Color.DodgerBlue;
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                button.FlatStyle = FlatStyle.Flat;
                button.ForeColor = Color.DodgerBlue;
                button.BackColor = Color.Transparent;
            }

            // Light Mode
            else
            {
                button.FlatAppearance.BorderColor = Color.Black;
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                button.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                button.FlatStyle = FlatStyle.Flat;
                button.ForeColor = Color.Black;
                button.BackColor = Color.WhiteSmoke;
            }
        }

        /// <summary>
        /// Sets the color of the textBoxes defined in the provided List
        /// </summary>
        /// <param name="labels"></param>
        private void SetTextBoxesColor(List<TextBox> textBoxes)
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                foreach (TextBox textBox in textBoxes)
                {
                    if (textBox.Tag == null || textBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        textBox.ForeColor = Color.White;
                        textBox.BackColor = Color.FromArgb(15, 15, 15);
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                    }
                }
            }

            // Light Mode
            else
            {
                foreach (TextBox textBox in textBoxes)
                {
                    if (textBox.Tag == null || textBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        textBox.ForeColor = Color.Black;
                        textBox.BackColor = Color.White;
                        textBox.BorderStyle = BorderStyle.FixedSingle;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the color of the checkedListBoxes defined in the provided List
        /// </summary>
        /// <param name="labels"></param>
        private void SetCheckedListBoxesColor(List<CheckedListBox> checkedListBoxes)
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                foreach (CheckedListBox checkedListBox in checkedListBoxes)
                {
                    if (checkedListBox.Tag == null || checkedListBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        checkedListBox.ForeColor = Color.White;
                        checkedListBox.BackColor = Color.FromArgb(15, 15, 15);
                    }
                }
            }

            // Light Mode
            else
            {
                foreach (CheckedListBox checkedListBox in checkedListBoxes)
                {
                    if (checkedListBox.Tag == null || checkedListBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        checkedListBox.ForeColor = Color.Black;
                        checkedListBox.BackColor = Color.White;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the color of the comboBoxes defined in the provided List
        /// </summary>
        /// <param name="labels"></param>
        private void SetComboboxesColor(List<ComboBox> comboBoxes)
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                foreach (ComboBox comboBox in comboBoxes)
                {
                    if (comboBox.Tag == null || comboBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        comboBox.ForeColor = Color.White;
                        comboBox.BackColor = Color.FromArgb(15, 15, 15);
                        comboBox.FlatStyle = FlatStyle.Flat;
                        comboBox.Text = "--Select--";
                    }
                }
            }

            // Light Mode
            else
            {
                foreach (ComboBox comboBox in comboBoxes)
                {
                    if (comboBox.Tag == null || comboBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        comboBox.ForeColor = Color.Black;
                        comboBox.BackColor = Color.White;
                        comboBox.FlatStyle = FlatStyle.Standard;
                        comboBox.Text = "--Select--";
                    }
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
            if (Properties.Settings.Default.useDarkMode)
            {
                foreach (Label label in labels)
                {
                    if (label.Tag == null || label.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        label.ForeColor = Color.White;
                        label.BackColor = Color.FromArgb(15, 15, 15);
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
        /// Gets the list of radio buttons recursively
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="radioButtons"></param>
        private void GetRadioButtonsRecursive(Control parent, List<RadioButton> radioButtons)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is RadioButton radioButton)
                {
                    radioButtons.Add(radioButton);
                }
                else
                {
                    GetRadioButtonsRecursive(control, radioButtons);
                }
            }
        }

        /// <summary>
        /// Set the color of radio buttons
        /// </summary>
        /// <param name="radioButtons"></param>
        private void SetRadioButtonsColor(List<RadioButton> radioButtons)
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                foreach (RadioButton radioButton in radioButtons)
                {
                    if (radioButton.Tag == null || radioButton.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        radioButton.ForeColor = Color.White;
                        radioButton.BackColor = Color.FromArgb(15, 15, 15);
                        radioButton.FlatAppearance.BorderColor = Color.White;
                    }
                }
            }

            // Light Mode
            else
            {
                foreach (RadioButton radioButton in radioButtons)
                {
                    if (radioButton.Tag == null || radioButton.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        radioButton.ForeColor = Color.Black;
                        radioButton.BackColor = Color.White;
                        radioButton.FlatAppearance.BorderColor = Color.Black;
                    }
                }
            }
        }

        /// <summary>
        /// Set the color of the elements in the side panel
        /// </summary>
        private void SetSidePanelColor()
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                // Side Panel
                control_Panel.BackColor = Color.Black;
                control_Panel.ForeColor = Color.Black;

                // Label
                workflow_Label.ForeColor = Color.White;
                workflow_Label.BackColor = Color.Black;

                // Side Panel Buttons
                page1_Button.ForeColor = Color.White;
                page1_Button.BackColor = Color.Black;
                page2_Button.ForeColor = Color.White;
                page2_Button.BackColor = Color.Black;
            }
            // Light Mode
            else
            {
                // Side Panel
                control_Panel.BackColor = Color.FromArgb(230, 230, 230);
                control_Panel.ForeColor = Color.FromArgb(230, 230, 230);

                // Label
                workflow_Label.ForeColor = Color.Black;
                workflow_Label.BackColor = Color.FromArgb(230, 230, 230);

                // Side Panel Buttons
                page1_Button.ForeColor = Color.Black;
                page1_Button.BackColor = Color.FromArgb(230, 230, 230);
                page2_Button.ForeColor = Color.Gray;
                page2_Button.BackColor = Color.FromArgb(230, 230, 230);
            }
        }


        /// <summary>
        /// Sets the Back Color of the form depending on the
        /// state of Dark and Light Mode
        /// </summary>
        private void SetFormBackColor()
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
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
        /// Gets all of the toggle buttons on the form recursively
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="labels"></param>
        private void GetTogglesRecursive(Control parent, List<Button> toggles)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Button toggle)
                {
                    toggles.Add(toggle);
                }
                else
                {
                    GetTogglesRecursive(control, toggles);
                }
            }
        }

        /// <summary>
        /// Gets all of the labels on the form recursively
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="labels"></param>
        private void GetTextBoxesRecursive(Control parent, List<TextBox> textBoxes)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBoxes.Add(textBox);
                }
                else
                {
                    GetTextBoxesRecursive(control, textBoxes);
                }
            }
        }

        /// <summary>
        /// Gets all of the labels on the form recursively
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="labels"></param>
        private void GetCheckedListBoxesRecursive(Control parent, List<CheckedListBox> checkedListBoxes)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is CheckedListBox checkedListBox)
                {
                    checkedListBoxes.Add(checkedListBox);
                }
                else
                {
                    GetCheckedListBoxesRecursive(control, checkedListBoxes);
                }
            }
        }

        /// <summary>
        /// Gets all of the labels on the form recursively
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="labels"></param>
        private void GetComboBoxesRecursive(Control parent, List<ComboBox> comboBoxes)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is ComboBox comboBox)
                {
                    comboBoxes.Add(comboBox);
                }
                else
                {
                    GetComboBoxesRecursive(control, comboBoxes);
                }
            }
        }

        /// <summary>
        /// Sets the color of the Button_AddException when enablement changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AddException_EnabledChanged(object sender, EventArgs e)
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                button_AddException.FlatAppearance.BorderColor = Color.DodgerBlue;
                button_AddException.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                button_AddException.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                button_AddException.FlatStyle = FlatStyle.Flat;
                button_AddException.ForeColor = Color.DodgerBlue;
                button_AddException.BackColor = Color.Transparent;
            }

            // Light Mode
            else
            {
                button_AddException.FlatAppearance.BorderColor = Color.Black;
                button_AddException.FlatAppearance.MouseDownBackColor = Color.FromArgb(50, 30, 144, 255);
                button_AddException.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 30, 144, 255);
                button_AddException.FlatStyle = FlatStyle.Flat;
                button_AddException.ForeColor = Color.Black;
                button_AddException.BackColor = Color.WhiteSmoke;
            }
        }
    }
}
