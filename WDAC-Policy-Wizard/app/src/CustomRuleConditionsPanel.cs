using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections.ObjectModel; 


namespace WDAC_Wizard
{
    public partial class CustomRuleConditionsPanel : Form
    {
        // CI Policy objects
        private WDAC_Policy Policy;
        public PolicyCustomRules PolicyCustomRule;     // One instance of a custom rule. Appended to Policy.CustomRules
        private List<string> AllFilesinFolder;          // List to track all files in a folder 

        public Logger Log;
        private MainWindow _MainWindow;
        private SigningRules_Control SigningControl;
        public bool RuleInEdit = false;
        private UIState state;
        private Exceptions_Control exceptionsControl;
        private bool redoRequired;
        private string[] DefaultValues;
        private Dictionary<string,string> FoundPackages;

        private enum UIState
        {
            RuleConditions = 0,
            RuleExceptions = 1
        }

        public CustomRuleConditionsPanel(SigningRules_Control pControl)
        {
            InitializeComponent();
            this.Policy = pControl.Policy;
            this.PolicyCustomRule = new PolicyCustomRules();
            this.AllFilesinFolder = new List<string>();

            this._MainWindow = pControl._MainWindow;
            this._MainWindow.RedoFlowRequired = false;
            this._MainWindow.CustomRuleinProgress = true;
            this.Log = this._MainWindow.Log;
            this.Log.AddInfoMsg("==== Custom Signing Rules Panel Initialized ====");
            this.SigningControl = pControl;
            this.RuleInEdit = true;
            this.state = UIState.RuleConditions;
            this.redoRequired = false; 
            this.exceptionsControl = null;
            this.DefaultValues = new string[5];
            this.FoundPackages = new Dictionary<string,string>();
        }

        /// <summary>
        /// Appends the custom rule to the bottom of the DataGridView and creates the rule in the CustomRules list. 
        /// </summary>
        private void Button_CreateRule_Click(object sender, EventArgs e)
        {
            // Assert that the reference file cannot be null, unless we are creating a custom value rule or a PFN rule
            if (this.PolicyCustomRule.ReferenceFile == null)
            {
                if(this.PolicyCustomRule.UsingCustomValues || 
                    this.PolicyCustomRule.Level == PolicyCustomRules.RuleLevel.PackagedFamilyName)
                {
                    
                }
                else
                {
                    label_Error.Visible = true;
                    label_Error.Text = Properties.Resources.InvalidRule_Error;
                    this.Log.AddWarningMsg("Create button rule selected without allow/deny setting and a reference file.");
                    return;
                }
            }

            // Flag to warn user that N/A's in the CustomRules pane may result in a hash rule
            bool warnUser = false;



            // Publisher and File Attribute checks
            // Check to make sure none of the fields are invalid
            if (this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher ||
                this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.FileAttributes)
            {
                // Assert one checkbox needs to be selected
                if (!(this.PolicyCustomRule.CheckboxCheckStates.checkBox0 || this.PolicyCustomRule.CheckboxCheckStates.checkBox1 ||
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox2 || this.PolicyCustomRule.CheckboxCheckStates.checkBox3 ||
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox4))
                {
                    label_Error.Visible = true;
                    label_Error.Text = Properties.Resources.InvalidCheckboxState;
                    this.Log.AddWarningMsg("Invalid checkbox state. No checkboxes selected.");
                    return;
                }

                if (this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
                {
                    if (this.PolicyCustomRule.CheckboxCheckStates.checkBox0 && !Helper.IsValidText(this.textBoxSlider_0.Text))
                    {
                        warnUser = true; 
                        this.Log.AddWarningMsg("PCACertificate field with null attribute");
                    }

                    if (this.PolicyCustomRule.CheckboxCheckStates.checkBox1 && !Helper.IsValidText(this.textBoxSlider_1.Text))
                    {
                        warnUser = true;
                        this.Log.AddWarningMsg("Publisher field with null attribute");
                    }

                    if (this.PolicyCustomRule.CheckboxCheckStates.checkBox4 && !Helper.IsValidText(this.textBoxSlider_4.Text))
                    {
                        ShowInvalidErrorLabel();
                        return;
                    }
                }
                else
                {
                    if (this.PolicyCustomRule.CheckboxCheckStates.checkBox0 && !Helper.IsValidText(this.textBoxSlider_0.Text))
                    {
                        ShowInvalidErrorLabel();
                        return;
                    }

                    if (this.PolicyCustomRule.CheckboxCheckStates.checkBox1 && !Helper.IsValidText(this.textBoxSlider_1.Text))
                    {
                        ShowInvalidErrorLabel();
                        return;
                    }
                }

                if (this.PolicyCustomRule.CheckboxCheckStates.checkBox2 && !Helper.IsValidText(this.textBoxSlider_2.Text))
                {
                    ShowInvalidErrorLabel();
                    return;
                }

                if (this.PolicyCustomRule.CheckboxCheckStates.checkBox3 && !Helper.IsValidText(this.textBoxSlider_3.Text))
                {
                    ShowInvalidErrorLabel();
                    return;
                }
            }

            // Packaged family name apps. Set the list of apps at button create time
            if (this.PolicyCustomRule.Level == PolicyCustomRules.RuleLevel.PackagedFamilyName)
            {
                // Assert >=1 packaged apps must be selected
                if (this.checkedListBoxPackagedApps.CheckedItems.Count < 1)
                {
                    label_Error.Visible = true;
                    label_Error.Text = Properties.Resources.PFNEmptyList_Error;
                    this.Log.AddWarningMsg("Create button rule selected with an empty packaged app list.");
                    return;
                }
                else
                {
                    // Using for loop to avoid System.InvalidOperationException despite list not changing
                    for(int i= 0; i< this.checkedListBoxPackagedApps.CheckedItems.Count; i++)
                    {
                        var item = this.checkedListBoxPackagedApps.CheckedItems[i]; 
                        this.PolicyCustomRule.PackagedFamilyNames.Add(item.ToString()); 
                    }
                }
            }
            
            // Check custom rules
            if (this.PolicyCustomRule.UsingCustomValues)
            {
                // Check custom publisher field
                if(this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher && this.PolicyCustomRule.CheckboxCheckStates.checkBox1)
                {
                    if (!Helper.IsValidPublisher(this.PolicyCustomRule.CustomValues.Publisher))
                    {
                        label_Error.Visible = true;
                        label_Error.Text = Properties.Resources.InvalidPublisherFormat_Error;
                        this.Log.AddWarningMsg(String.Format("Invalid format for Custom Publisher", this.PolicyCustomRule.CustomValues.Publisher));
                        return;
                    }
                    else
                    {
                        // Valid publisher, format so WDAC is happy with the input
                        this.PolicyCustomRule.CustomValues.Publisher = Helper.FormatPublisherCN(this.PolicyCustomRule.CustomValues.Publisher); 
                    }
                }

                // Check custom product field
                if (this.PolicyCustomRule.CheckboxCheckStates.checkBox2 && !Helper.IsValidText(this.textBoxSlider_2.Text))
                {
                    ShowInvalidErrorLabel();
                    return;
                }

                // Check custom original filename
                if (this.PolicyCustomRule.CheckboxCheckStates.checkBox3 && !Helper.IsValidText(this.textBoxSlider_3.Text))
                {
                    ShowInvalidErrorLabel();
                    return;
                }

                // Check custom versions
                if (this.PolicyCustomRule.CheckboxCheckStates.checkBox4) 
                {
                    if(!Helper.IsValidVersion(this.PolicyCustomRule.CustomValues.MinVersion))
                    {
                        label_Error.Visible = true;
                        label_Error.Text = Properties.Resources.InvalidVersionFormat_Error; 
                        this.Log.AddWarningMsg(String.Format("Invalid version format for CustomMinVersion: {0}", this.PolicyCustomRule.CustomValues.MinVersion));
                        return;
                    }

                    // Check MaxVersion
                    if (this.PolicyCustomRule.CustomValues.MaxVersion != null)
                    {
                        if (!Helper.IsValidVersion(this.PolicyCustomRule.CustomValues.MaxVersion))
                        {
                            label_Error.Visible = true;
                            label_Error.Text = Properties.Resources.InvalidVersionFormat_Error;
                            this.Log.AddWarningMsg(String.Format("Invalid version format for CustomMaxVersion: {0}", this.PolicyCustomRule.CustomValues.MaxVersion));
                            return;
                        }

                        if (Helper.CompareVersions(this.PolicyCustomRule.CustomValues.MinVersion, this.PolicyCustomRule.CustomValues.MaxVersion) < 0)
                        {
                            label_Error.Visible = true;
                            label_Error.Text = Properties.Resources.InvalidVersionRange_Error;
                            this.Log.AddWarningMsg(String.Format("CustomMinVersion {0} !< CustomMaxVersion {1}", this.PolicyCustomRule.CustomValues.MinVersion, this.PolicyCustomRule.CustomValues.MaxVersion));
                            return;
                        }
                    }
                }

                // Check custom path
                if (this.PolicyCustomRule.CustomValues.Path != null)
                {
                    if(!Helper.IsValidPathRule(this.PolicyCustomRule.CustomValues.Path))
                    {
                        label_Error.Visible = true;
                        label_Error.Text = Properties.Resources.InvalidWildcardPath_Error;
                        this.Log.AddWarningMsg("Invalid custom path rule");
                        return;
                    }
                }

                // Check custom EKU value 
                if (!String.IsNullOrEmpty(this.PolicyCustomRule.CustomValues.EKUFriendly))
                {
                    string ekuTLVEncoded = Helper.EKUValueToTLVEncoding(this.PolicyCustomRule.CustomValues.EKUFriendly); 
                    if(String.IsNullOrEmpty(ekuTLVEncoded))
                    {
                        this.Log.AddErrorMsg("EKU Encoding Failed for user-input EKU value " +
                            this.PolicyCustomRule.CustomValues.EKUFriendly);
                        label_Error.Visible = true;
                        label_Error.Text = Properties.Resources.InvalidEKUFormat_Error;
                        return; 
                    }
                    else
                    {
                        this.PolicyCustomRule.CustomValues.EKUEncoded = ekuTLVEncoded; 
                    }
                }

                // Parse package family names into PolicyCustomRule.CustomValues.PackageFamilyNames
                if (this.PolicyCustomRule.Level == PolicyCustomRules.RuleLevel.PackagedFamilyName)
                {
                    this.PolicyCustomRule.CustomValues.PackageFamilyNames = this.PolicyCustomRule.PackagedFamilyNames;
                }

                // Parse hashes into PolicyCustomRule.CustomValues.Hash
                if (this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Hash)
                {
                    var hashList = this.richTextBox_CustomHashes.Text.Split(',');
                    foreach(var hash in hashList)
                    {
                        if(!String.IsNullOrEmpty(hash) && hash.Trim().Length%2 == 0) // must be an even number
                        {
                            this.PolicyCustomRule.CustomValues.Hashes.Add(hash.Trim());
                        }
                    }

                    if(this.PolicyCustomRule.CustomValues.Hashes.Count == 0)
                    {
                        label_Error.Visible = true;
                        label_Error.Text = Properties.Resources.HashEmptyList_Error;
                        this.Log.AddWarningMsg("Zero hash values located.");
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
            this.RuleInEdit = false;
            this.SigningControl.AddRuleToTable(displayString, this.PolicyCustomRule, warnUser);

            // Renew the custom rule instance
            this.PolicyCustomRule = new PolicyCustomRules();

            // Reset UI view
            ClearCustomRulesPanel(true);
            this._MainWindow.CustomRuleinProgress = false;
        }

        private string[] FormatTableDisplayString(bool warnUser)
        {
            // Show warning message to user to notify that a signing chain was not found; may result in a hash rule created
            if (warnUser)
            {
                DialogResult res = MessageBox.Show("One or more of the file attributes could not be found. Creating this rule may result in a hash rule if unsuccessful. " +
                    "\n\nWould you like to proceed anyway?", "Proceed with Rule Creation?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (res == DialogResult.Yes)
                {
                    this.Log.AddInfoMsg("Proceeding with Rule Creation anyway. Rule may fallback to hash");
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

            this.Log.AddInfoMsg("--- New Custom Rule Added ---");

            // Set Action/Permission value to Allow or Deny
            action = this.PolicyCustomRule.Permission.ToString();

            // Set Level value to the RuleLevel value//or should this be type for simplicity? 
            level = this.PolicyCustomRule.Type.ToString();

            // Format rule display string
            switch (this.PolicyCustomRule.Type)
            {
                // Signer rules
                case PolicyCustomRules.RuleType.Publisher:
                    {
                        name += "CA: " + this.textBoxSlider_0.Text; 
                        if(this.PolicyCustomRule.CheckboxCheckStates.checkBox1)
                        {
                            name += " & CN: " + this.textBoxSlider_1.Text;
                        }
                        if (this.PolicyCustomRule.CheckboxCheckStates.checkBox2)
                        {
                            name += " & Product: " + this.textBoxSlider_2.Text;
                        }
                        if (this.PolicyCustomRule.CheckboxCheckStates.checkBox3)
                        {
                            name += " & Filename: " + this.textBoxSlider_3.Text;
                        }
                        if (this.PolicyCustomRule.CheckboxCheckStates.checkBox4)
                        {
                            name += " & Min Version: " + this.textBoxSlider_4.Text;
                        }
                        if(this.PolicyCustomRule.UsingCustomValues && this.PolicyCustomRule.CustomValues.MaxVersion != null)
                        {
                            name += " & Max Version: " + this.PolicyCustomRule.CustomValues.MaxVersion;
                        }
                        break;
                    }

                case PolicyCustomRules.RuleType.FileAttributes:
                    {
                        if (this.PolicyCustomRule.CheckboxCheckStates.checkBox0)
                        {
                            name += " & Original Filename: " + this.textBoxSlider_0.Text;
                        }
                        if (this.PolicyCustomRule.CheckboxCheckStates.checkBox1)
                        {
                            name += " & File Description: " + this.textBoxSlider_1.Text;
                        }
                        if (this.PolicyCustomRule.CheckboxCheckStates.checkBox2)
                        {
                            name += " & Product: " + this.textBoxSlider_2.Text;
                        }
                        if (this.PolicyCustomRule.CheckboxCheckStates.checkBox3)
                        {
                            name += " & Internal Name: " + this.textBoxSlider_3.Text;
                        }
                        name = name.Substring(3); // Offset by 3 to remove the first occurence of ' & '
                        break; 
                    }

                case PolicyCustomRules.RuleType.PackagedApp:
                    {
                        name = "Packaged apps matching the Package Family Name (PFN) rules in the Files cell";
                        files = Helper.GetListofPackages(this.PolicyCustomRule);
                        break;
                    }

                case PolicyCustomRules.RuleType.Folder:
                    {
                        if (this.PolicyCustomRule.UsingCustomValues)
                        {
                            name = "Files under path: " + this.PolicyCustomRule.CustomValues.Path;
                        }
                        else
                        {
                            name = "Files under path: " + this.PolicyCustomRule.ReferenceFile;
                        }
                        break;
                    }

                    

                case PolicyCustomRules.RuleType.FilePath:
                    {
                        if (this.PolicyCustomRule.UsingCustomValues)
                        {
                            name = "Files matching: " + this.PolicyCustomRule.CustomValues.Path;
                        }
                        else
                        {
                            name = "Files matching: " + this.PolicyCustomRule.ReferenceFile;
                        }
                        break;
                    }

                case PolicyCustomRules.RuleType.Hash:
                    {
                        if (this.PolicyCustomRule.UsingCustomValues)
                        {
                            if (PolicyCustomRule.CustomValues.Hashes.Count > 1)
                            {
                                name = String.Format("Custom Hash List: {0}, ...", this.PolicyCustomRule.CustomValues.Hashes[0]);
                            }
                            else
                            {
                                name = "Custom Hash Value: " + this.PolicyCustomRule.CustomValues.Hashes[0];
                            }
                        }
                        else
                        {
                            name = "File Hash Rule: " + this.PolicyCustomRule.ReferenceFile;
                        }
                        break;
                    }
            }

            // Handle custom EKU
            if (!String.IsNullOrEmpty(this.PolicyCustomRule.CustomValues.EKUFriendly))
            {
                files += "EKU: " + this.PolicyCustomRule.CustomValues.EKUFriendly;
            }

            // Handle exceptions
            if (this.PolicyCustomRule.ExceptionList.Count > 0)
            {
                foreach (var exception in this.PolicyCustomRule.ExceptionList)
                {
                    string exceptionString = String.Format("{0}: {1} {2}; ", exception.Permission, exception.Level,
                        exception.ReferenceFile);
                    exceptions += exceptionString;
                }

                // Remove trailing semi-colon
                exceptions.Trim(';');
            }

            this.Log.AddInfoMsg(String.Format("CUSTOM RULE Created: {0} - {1} - {2} - {3} ", action, level, name, files));
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
            if(this.checkBox_CustomValues.Checked)
            {
                this.checkBox_CustomValues.Checked = false;
                this.PolicyCustomRule.UsingCustomValues = false; 
            }

            // Clear panels
            this.checkBox_CustomPath.Visible = false;
            this.checkBox_CustomPath.Checked = false;
            this.panelPackagedApps.Visible = false;

            switch (selectedOpt)
            {
                case "Publisher":
                    this.PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.Publisher);
                    this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FilePublisher); // Match UI by default
                    label_Info.Text = "Creates a rule for a file that is signed by the software publisher. \r\n" +
                        "Select a file to use as reference for your rule.";
                    break;

                case "Path":
                    this.PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.FilePath);
                    this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FilePath);
                    label_Info.Text = "Creates a rule for a specific file or folder. \r\n" +
                        "Selecting folder will affect all files in the folder.";
                    panel_FileFolder.Visible = true;
                    radioButton_File.Checked = true; // By default, 

                    this.checkBox_CustomPath.Visible = true;
                    this.checkBox_CustomPath.Text = "Use Custom Path";
                    break;

                case "File Attributes":
                    this.PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.FileAttributes);
                    this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.InternalName); // Match UI by default
                    label_Info.Text = "Creates a rule for a file based on one of its attributes. \r\n" +
                        "Select a file to use as reference for your rule.";
                    break;

                case "Packaged App":
                    this.PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.PackagedApp);
                    this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.PackagedFamilyName);
                    this.panelPackagedApps.Location = this.label_condition.Location;
                    this.panelPackagedApps.Visible = true;
                    this.panelPackagedApps.BringToFront();
                    label_Info.Text = "Creates a rule for a packaged app based on its package family name.\r\nSearch for the name of the packages to allow/deny.";
                    break; 

                case "File Hash":
                    this.PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.Hash);
                    this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.Hash);
                    label_Info.Text = "Creates a rule for a file that is not signed. \r\n" +
                        "Select the file for which you wish to create a hash rule.";
                    
                    this.checkBox_CustomPath.Visible = true;
                    this.checkBox_CustomPath.Text = "Use Custom Hash Values"; 
                    break;
                default:
                    break;
            }

            this.Log.AddInfoMsg(String.Format("Custom File Rule Level Set to {0}", selectedOpt));

            // Returned back from exceptions to change Rule Type - Redo is required
            if(this.exceptionsControl != null)
            {
                this.redoRequired = true; 
            }

            // Show new UI based on the rule type selected if the user has already selected a reference file
            if(this.PolicyCustomRule.ReferenceFile != null && !this.redoRequired)
            {
                SetDefaultUIState(this.PolicyCustomRule.GetRuleType());
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

            // Reset the rule type combobox
            if (clearComboBox)
            {
                this.comboBox_RuleType.SelectedItem = null;
                this.comboBox_RuleType.Text = "--Select--";
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
            // Browse button for reference file:
            if (comboBox_RuleType.SelectedItem == null)
            {
                label_Error.Visible = true;
                label_Error.Text = "Please select a rule type first.";
                this.Log.AddWarningMsg("Browse button selected before rule type selected. Set rule type first.");
                return;
            }

            if (this.PolicyCustomRule.Type != PolicyCustomRules.RuleType.Folder)
            {
                string refPath = GetFileLocation();
                if (refPath == String.Empty)
                {
                    return; 
                }

                this.DefaultValues[4] = refPath; 

                // Custom rule in progress
                this._MainWindow.CustomRuleinProgress = true;

                // Get generic file information to be shown to user
                SetFileSignerInfo(refPath);
            }

            // Set the landing UI depending on the Rule type
            SetDefaultUIState(this.PolicyCustomRule.Type);

            // Returned from exceptions user control to modify the reference path
            if(this.exceptionsControl != null)
            {
                this.redoRequired = true; 
            }
        }

        /// <summary>
        /// Retrieves the file attribute and signer info
        /// </summary>
        /// <param name="refPath"></param>
        private void SetFileSignerInfo(string refPath)
        {
            this.PolicyCustomRule.FileInfo = new Dictionary<string, string>(); // Reset dict
            FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(refPath);

            this.PolicyCustomRule.ReferenceFile = fileInfo.FileName; // Returns the file path
            this.PolicyCustomRule.FileInfo.Add("CompanyName", String.IsNullOrEmpty(fileInfo.CompanyName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.CompanyName);
            this.PolicyCustomRule.FileInfo.Add("ProductName", String.IsNullOrEmpty(fileInfo.ProductName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.ProductName);
            this.PolicyCustomRule.FileInfo.Add("OriginalFilename", String.IsNullOrEmpty(fileInfo.OriginalFilename) ? Properties.Resources.DefaultFileAttributeString : fileInfo.OriginalFilename);
            this.PolicyCustomRule.FileInfo.Add("FileVersion", String.IsNullOrEmpty(fileInfo.FileVersion) ? Properties.Resources.DefaultFileAttributeString : fileInfo.FileVersion.Replace(',', '.')); // Replace misleading commas in version with '.'
            this.PolicyCustomRule.FileInfo.Add("FileName", String.IsNullOrEmpty(fileInfo.OriginalFilename) ? Properties.Resources.DefaultFileAttributeString : fileInfo.OriginalFilename); // WDAC configCI uses original filename 
            this.PolicyCustomRule.FileInfo.Add("FileDescription", String.IsNullOrEmpty(fileInfo.FileDescription) ? Properties.Resources.DefaultFileAttributeString : fileInfo.FileDescription);
            this.PolicyCustomRule.FileInfo.Add("InternalName", String.IsNullOrEmpty(fileInfo.InternalName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.InternalName);

            // Get cert chain info to be shown to the user if type is publisher. Otherwise, we don't check or try to build the cert chain
            string leafCertSubjectName = "";
            string pcaCertSubjectName = "";

            if (this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
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
                }

                catch (Exception exp)
                {
                    this._MainWindow.Log.AddErrorMsg(String.Format("Caught exception {0} when trying to create cert from the following signed file {1}", exp, refPath));
                    this.label_Error.Text = "Unable to find certificate chain for " + fileInfo.FileName;
                    this.label_Error.Visible = true;
                }
            }

            this.PolicyCustomRule.FileInfo.Add("LeafCertificate", String.IsNullOrEmpty(leafCertSubjectName) ? Properties.Resources.DefaultFileAttributeString : leafCertSubjectName);
            this.PolicyCustomRule.FileInfo.Add("PCACertificate", String.IsNullOrEmpty(pcaCertSubjectName) ? Properties.Resources.DefaultFileAttributeString : pcaCertSubjectName);
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
                    this.textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    // Show right side of the text
                    if (this.textBox_ReferenceFile.TextLength > 0)
                    {
                        this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                        this.textBox_ReferenceFile.ScrollToCaret();
                    }

                    // Set defaults to restore to if custom values is ever reset
                    this.DefaultValues[0] = PolicyCustomRule.FileInfo["PCACertificate"];
                    this.DefaultValues[1] = PolicyCustomRule.FileInfo["LeafCertificate"];
                    this.DefaultValues[2] = PolicyCustomRule.FileInfo["ProductName"];
                    this.DefaultValues[3] = PolicyCustomRule.FileInfo["FileName"];
                    this.DefaultValues[4] = PolicyCustomRule.FileInfo["FileVersion"];

                    // Set checkbox struct
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox0 = true;
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox1 = true;
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox2 = false;
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox3 = true;
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox4 = true;

                    // Set all fields checked by default, except for product to match legacy (slider bar) behavior unless null original filename or version
                    this.checkBoxAttribute0.Checked = true; 
                    this.checkBoxAttribute1.Checked = true;
                    this.checkBoxAttribute3.Checked = true;
                    this.checkBoxAttribute4.Checked = true;

                    // Do not check for N/As in PCA or publisher fields since the file may be cat
                    // signed which the Wizard cannot handle right now

                    if (this.DefaultValues[3] == Properties.Resources.DefaultFileAttributeString)
                    {
                        this.checkBoxAttribute3.Checked = false;
                        this.PolicyCustomRule.CheckboxCheckStates.checkBox3 = false;
                    }

                    if (this.DefaultValues[4] == Properties.Resources.DefaultFileAttributeString)
                    {
                        this.checkBoxAttribute4.Checked = false;
                        this.PolicyCustomRule.CheckboxCheckStates.checkBox4 = false;
                    }

                    this.checkBoxAttribute0.Text = "Issuing CA:";
                    this.checkBoxAttribute1.Text = "Publisher:";
                    this.checkBoxAttribute2.Text = "Product name:";
                    this.checkBoxAttribute3.Text = "File name:";
                    this.checkBoxAttribute4.Text = "Min. Version:";

                    // Version textbox should be set to normal size
                    this.textBoxSlider_4.Size = this.textBoxSlider_3.Size;

                    // Show version boxes
                    this.textBoxSlider_4.Visible = true;
                    this.checkBoxAttribute4.Visible = true;

                    this.textBoxSlider_0.Text = this.DefaultValues[0];
                    this.textBoxSlider_1.Text = this.DefaultValues[1];
                    this.textBoxSlider_2.Text = this.DefaultValues[2];
                    this.textBoxSlider_3.Text = this.DefaultValues[3];
                    this.textBoxSlider_4.Text = this.DefaultValues[4];

                    this.textBoxSlider_0.BackColor = Color.FromArgb(240, 240, 240); // Grayed out; cannot be overwritten by custom values

                    panel_Publisher_Scroll.Visible = true;
                    break;

                case PolicyCustomRules.RuleType.Folder:

                    // User wants to create rule by folder level
                    this.PolicyCustomRule.ReferenceFile = GetFolderLocation();
                    this.DefaultValues[4] = this.PolicyCustomRule.ReferenceFile;
                    this.AllFilesinFolder = new List<string>();
                    if (PolicyCustomRule.ReferenceFile == String.Empty)
                    {
                        break;
                    }

                    // Add an asterix to the end of the path to allow all 
                    this.PolicyCustomRule.ReferenceFile += "\\*";

                    // Custom rule in progress
                    this._MainWindow.CustomRuleinProgress = true;

                    this.textBox_ReferenceFile.Text = this.PolicyCustomRule.ReferenceFile;

                    // Show right side of the text
                    if (this.textBox_ReferenceFile.TextLength > 0)
                    {
                        this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                        this.textBox_ReferenceFile.ScrollToCaret();
                    }

                    break;


                case PolicyCustomRules.RuleType.FilePath:

                    // UI updates
                    radioButton_File.Checked = true;
                    this.textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;

                    // Show right side of the text
                    if (this.textBox_ReferenceFile.TextLength > 0)
                    {
                        this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                        this.textBox_ReferenceFile.ScrollToCaret();
                    }

                    panel_Publisher_Scroll.Visible = false;
                    break;

                case PolicyCustomRules.RuleType.FileAttributes:

                    this.textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;

                    // Show right side of the text
                    if (this.textBox_ReferenceFile.TextLength > 0)
                    {
                        this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                        this.textBox_ReferenceFile.ScrollToCaret();
                    }

                    this.checkBoxAttribute0.Text = "Original filename:";
                    this.checkBoxAttribute1.Text = "File description:";
                    this.checkBoxAttribute2.Text = "Product name:";
                    this.checkBoxAttribute3.Text = "Internal name:";

                    // Set checkbox states to all disabled -- allow user to select the ones desired
                    this.checkBoxAttribute0.Checked = false;
                    this.checkBoxAttribute1.Checked = false;
                    this.checkBoxAttribute2.Checked = false;
                    this.checkBoxAttribute3.Checked = false;
                    this.checkBoxAttribute4.Checked = false;

                    // Set checkbox struct
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox0 = false;
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox1 = false;
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox2 = false;
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox3 = false;
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox4 = false;

                    // Hide version boxes
                    this.textBoxSlider_4.Visible = false;
                    this.checkBoxAttribute4.Visible = false;

                    // Set defaults to restore to if custom values is ever reset
                    this.DefaultValues[0] = PolicyCustomRule.FileInfo["OriginalFilename"];
                    this.DefaultValues[1] = PolicyCustomRule.FileInfo["FileDescription"];
                    this.DefaultValues[2] = PolicyCustomRule.FileInfo["ProductName"];
                    this.DefaultValues[3] = PolicyCustomRule.FileInfo["InternalName"];

                    this.textBoxSlider_0.Text = this.DefaultValues[0];
                    this.textBoxSlider_1.Text = this.DefaultValues[1];
                    this.textBoxSlider_2.Text = this.DefaultValues[2];
                    this.textBoxSlider_3.Text = this.DefaultValues[3];

                    panel_Publisher_Scroll.Visible = true;
                    break;

                case PolicyCustomRules.RuleType.Hash:

                    // UI updates
                    panel_Publisher_Scroll.Visible = false;
                    this.textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    // Show right side of the text
                    if (this.textBox_ReferenceFile.TextLength > 0)
                    {
                        this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                        this.textBox_ReferenceFile.ScrollToCaret();
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
                this.PolicyCustomRule.Level = PolicyCustomRules.RuleLevel.FilePath; 
                this.PolicyCustomRule.Type = PolicyCustomRules.RuleType.FilePath;
            } 
            else
            {
                this.PolicyCustomRule.Level = PolicyCustomRules.RuleLevel.Folder;
                this.PolicyCustomRule.Type = PolicyCustomRules.RuleType.Folder;
            }

            // Check if user changed Rule Level after already browsing and selecting a reference file
            if (this.PolicyCustomRule.ReferenceFile != null)
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
            this.textBoxSlider_0.Clear();
            this.textBoxSlider_1.Clear();
            this.textBoxSlider_2.Clear();
            this.textBoxSlider_3.Clear();
            this.textBoxSlider_4.Clear();            
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
            if (this.RuleInEdit)
            {
                DialogResult res = MessageBox.Show("Are you sure you want to abandon rule creation?", "Confirmation",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (res == DialogResult.Yes)
                {
                    this.SigningControl.CustomRulesPanel_Closing();
                    this._MainWindow.CustomRuleinProgress = false; 
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
            // Assert not a path rule since path rules cannot be excepted in WDAC
            if(this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Folder ||
                this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.FilePath || 
                this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Hash)
            {
                label_Error.Visible = true;
                label_Error.Text = Properties.Resources.RuleTypeNoExceptionAllowed;
                this.Log.AddWarningMsg("Cannot proceed to Exceptions Panel. Path and hash rules cannot be excepted.");
                return;
            }

            // Check required fields - that a reference file is selected
            // Show the exception UI
            if (this.PolicyCustomRule.Type != PolicyCustomRules.RuleType.None 
                && this.PolicyCustomRule.ReferenceFile != null)
            {
                this.state = UIState.RuleExceptions; 
                SetUIState();

                // Disable next button 
                //this.button_Next.ForeColor = Color.Gray;
                //this.button_Back.FlatAppearance.BorderColor = Color.Gray;
                this.button_Next.Enabled = false;

                // Enable Back & exception button
                this.button_Back.ForeColor = Color.Black;
                this.button_Back.FlatAppearance.BorderColor = Color.Black;
                this.button_Back.Enabled = true;

                this.button_AddException.ForeColor = Color.Black;
                this.button_AddException.FlatAppearance.BorderColor = Color.Black;
                this.button_AddException.Enabled = true;
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
            this.state = UIState.RuleConditions;
            SetUIState();

            // Enable next button 
            this.button_Next.ForeColor = Color.Black;
            this.button_Back.FlatAppearance.BorderColor = Color.Black;
            this.button_Next.Enabled = true;

            // Disable Back button
            //this.button_Back.ForeColor = Color.Gray;
            //this.button_Back.FlatAppearance.BorderColor = Color.Gray;
            this.button_Back.Enabled = false;
        }

        /// <summary>
        /// Sets the state of the UI. Will show the CustomRule or Exceptions control panel
        /// </summary>
        private void SetUIState()
        {
            // bring info label to front
            this.Controls.Add(this.label_Error);
            this.label_Error.Focus();
            this.label_Error.BringToFront();

            switch (this.state)
            {
                case UIState.RuleConditions:

                    // Hide the Exceptions User Control 
                    this.exceptionsControl.Hide();
                    this.exceptionsControl.SendToBack();
                    this.redoRequired = false; // Reset flag as returning back to rule conditions user control should not auto trigger a redo

                    // Enable side panel
                    // Show control panel
                    this.Controls.Add(this.control_Panel);
                    this.control_Panel.BringToFront();
                    this.control_Panel.Focus();

                    // Set the control highlight rectangle pos
                    this.controlHighlight_Panel.Location = new Point(3, 138);
                    this.controlHighlight_Panel.BringToFront();
                    this.controlHighlight_Panel.Focus(); 

                    // Show header panel                        
                    this.headerLabel.Text = "Custom Rule Conditions";

                    break;

                case UIState.RuleExceptions:
                    {
                        //TODO: check if create new exceptions_control or show existing one
                        if (this.exceptionsControl == null || this.redoRequired == true)
                        {
                            this.exceptionsControl = new Exceptions_Control(this);
                            this.Controls.Add(this.exceptionsControl);
                        }
                        else
                        {
                            // show existing one
                        }

                        // Show the exceptions control
                        this.exceptionsControl.Show();
                        this.exceptionsControl.BringToFront();
                        this.exceptionsControl.Focus();

                        // Enable side panel
                        // Show control panel
                        this.Controls.Add(this.control_Panel);
                        this.control_Panel.BringToFront();
                        this.control_Panel.Focus();

                        // Set the control highlight rectangle pos
                        this.controlHighlight_Panel.Location = new Point(3, 226);
                        this.controlHighlight_Panel.BringToFront();
                        this.controlHighlight_Panel.Focus();

                        // Show header panel                        
                        this.headerLabel.Text = "Custom Rule Exceptions";
                        this.Controls.Add(this.headerLabel);
                        this.headerLabel.BringToFront();
                        this.headerLabel.Focus(); 
                    }

                    break;

                default:

                    break;
            }

            // Show buttons
            this.button_Next.BringToFront();
            this.button_Next.Focus();

            this.button_CreateRule.BringToFront();
            this.button_CreateRule.Focus();

            this.button_Back.BringToFront();
            this.button_Back.Focus();

            this.button_AddException.BringToFront();
            this.button_AddException.Focus();
        }

        /// <summary>
        /// Sets the error text string and whether the error label should persist/is sticky
        /// </summary>
        /// <param name="errorText"></param>
        /// <param name="shouldPersist"></param>
        public void SetLabel_ErrorText(string errorText, bool shouldPersist=false)
        {
            this.label_Error.Focus();
            this.label_Error.BringToFront();

            this.label_Error.Text = errorText; 
            this.label_Error.Visible = true;

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
            this.label_Error.Text = "";
            this.label_Error.Visible = false;
        }

        /// <summary>
        /// Shows the invalid error label when a checkbox is selected with invalid text
        /// </summary>
        private void ShowInvalidErrorLabel()
        {
            label_Error.Visible = true;
            label_Error.Text = Properties.Resources.InvalidAttributeSelection_Error;
            this.Log.AddWarningMsg("Create button rule selected with an empty file attribute.");
        }

        /// <summary>
        /// Add exception button clicked. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AddException_Click(object sender, EventArgs e)
        {
            this.exceptionsControl.AddException(); 
        }

        /// <summary>
        /// Deny radio button selected. Custom rule is a deny Rule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Deny_Click(object sender, EventArgs e)
        {
            this.PolicyCustomRule.Permission = PolicyCustomRules.RulePermission.Deny;
            this.Log.AddInfoMsg("Rule Permission set to " + this.PolicyCustomRule.Permission.ToString());

            // Returned back from exceptions to change Rule Type - Redo is required
            if (this.exceptionsControl != null)
            {
                this.redoRequired = true; 
            }
        }

        /// <summary>
        /// Allow radio button selected. Custom rule is an Allow Rule
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Allow_Click(object sender, EventArgs e)
        {
            this.PolicyCustomRule.Permission = PolicyCustomRules.RulePermission.Allow;
            this.Log.AddInfoMsg("Rule Permission set to " + this.PolicyCustomRule.Permission.ToString());

            // Returned back from exceptions to change Rule Type - Redo is required
            if (this.exceptionsControl != null)
            {
                this.redoRequired = true;
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
            if (this.checkBox_CustomValues.Checked)
            {
                if(this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.FileAttributes)
                {
                    SetTextBoxStates(true, PolicyCustomRules.RuleType.FileAttributes);

                    // Set the custom values based on existing
                    this.PolicyCustomRule.CustomValues.FileName = textBoxSlider_0.Text;
                    this.PolicyCustomRule.CustomValues.Description = textBoxSlider_1.Text;
                    this.PolicyCustomRule.CustomValues.ProductName = textBoxSlider_2.Text;
                    this.PolicyCustomRule.CustomValues.InternalName = textBoxSlider_3.Text;
                }
                else
                {
                    // Set textbox states to write, enabled, and white back color
                    SetTextBoxStates(true);

                    // Set the custom values based on existing
                    this.PolicyCustomRule.CustomValues.Publisher = textBoxSlider_1.Text;
                    this.PolicyCustomRule.CustomValues.ProductName = textBoxSlider_2.Text;
                    this.PolicyCustomRule.CustomValues.FileName = textBoxSlider_3.Text;
                    this.PolicyCustomRule.CustomValues.MinVersion = textBoxSlider_4.Text;
                }

                this.PolicyCustomRule.UsingCustomValues = true; 
            }
            else
            {
                // Clear error if applicable
                this.ClearLabel_ErrorText();

                // Set text values back to default
                SetTextBoxStates(false); 

                // Format the version text boxes
                this.textBoxSlider_4.Size = this.textBoxSlider_0.Size;
                this.checkBoxAttribute1.Text = "Min version:";
                this.textBox_MaxVersion.Visible = false;
                this.label_To.Visible = false;

                // Re-populate the text boxes
                this.textBoxSlider_0.Text = this.DefaultValues[0];
                this.textBoxSlider_1.Text = this.DefaultValues[1];
                this.textBoxSlider_2.Text = this.DefaultValues[2];
                this.textBoxSlider_3.Text = this.DefaultValues[3];
                this.textBoxSlider_4.Text = this.DefaultValues[4];

                this.PolicyCustomRule.UsingCustomValues = false;
            }
        }

        /// <summary>
        /// Set the UI states (enabled, readonly) and appearances for the publisher or file attribute textboxes
        /// </summary>
        /// <param name="enabled"></param>
        private void SetTextBoxStates(bool enabled, PolicyCustomRules.RuleType ruleType=PolicyCustomRules.RuleType.Publisher)
        {
            if(enabled)
            {
                // If enabled, allow user input
                this.textBoxSlider_0.ReadOnly = true; // Custom text values for PCA are not supported
                this.textBoxSlider_1.ReadOnly = false; // Publisher | File description
                this.textBoxSlider_2.ReadOnly = false; // Product   | Product name
                this.textBoxSlider_3.ReadOnly = false; // Filename  | Internal name
                this.textBoxSlider_4.ReadOnly = false; // Min version
                this.textBox_MaxVersion.ReadOnly = false;

                this.textBoxSlider_0.Enabled = false; 
                this.textBoxSlider_1.Enabled = true;
                this.textBoxSlider_2.Enabled = true;
                this.textBoxSlider_3.Enabled = true;
                this.textBoxSlider_4.Enabled = true;
                this.textBox_MaxVersion.Enabled = true;

                // Set back color to white to help user determine boxes are userwriteable
                this.textBoxSlider_0.BackColor = Color.White;
                this.textBoxSlider_1.BackColor = Color.White;
                this.textBoxSlider_2.BackColor = Color.White;
                this.textBoxSlider_3.BackColor = Color.White;
                this.textBoxSlider_4.BackColor = Color.White;
                this.textBox_MaxVersion.BackColor = Color.White;

                // Format the version text boxes
                this.textBoxSlider_4.Visible = true;
                this.textBox_MaxVersion.Visible = true;
                this.label_To.Visible = true;

                this.textBoxSlider_4.Size = this.textBox_MaxVersion.Size;
                this.checkBoxAttribute4.Text = "Version range:";

                // If RuleType == FileAttributes, ensure first textbox is user writeable
                if (ruleType == PolicyCustomRules.RuleType.FileAttributes)
                {
                    this.textBoxSlider_0.ReadOnly = false;
                    this.textBoxSlider_0.Enabled = true;

                    // Hide version boxes
                    this.textBoxSlider_4.Visible = false;
                    this.textBox_MaxVersion.Visible = false;
                    this.label_To.Visible = false;
                }
            }
            else
            {
                // Set to read only if disabled
                this.textBoxSlider_0.ReadOnly = true;
                this.textBoxSlider_1.ReadOnly = true;
                this.textBoxSlider_4.ReadOnly = true;
                this.textBoxSlider_3.ReadOnly = true;
                this.textBox_MaxVersion.ReadOnly = true;

                // Set to not enabled so accepts no user interaction
                this.textBoxSlider_0.Enabled = false;
                this.textBoxSlider_1.Enabled = false;
                this.textBoxSlider_2.Enabled = false;
                this.textBoxSlider_3.Enabled = false;
                this.textBoxSlider_4.Enabled = false;
                this.textBox_MaxVersion.Enabled = false;

                // Set back to default color
                this.textBoxSlider_0.BackColor = SystemColors.Control;
                this.textBoxSlider_1.BackColor = SystemColors.Control;
                this.textBoxSlider_2.BackColor = SystemColors.Control;
                this.textBoxSlider_3.BackColor = SystemColors.Control;
                this.textBoxSlider_4.BackColor = SystemColors.Control;
                this.textBox_MaxVersion.BackColor = SystemColors.Control;
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
            // Filename (publisher) or InternalName (file attributes)
            // Break if not using custom values. This will be reached during setting values once proto file is chosen

            if (!this.PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            if (this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                this.PolicyCustomRule.CustomValues.MinVersion = textBoxSlider_4.Text;
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

            if (!this.PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            if (this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                this.PolicyCustomRule.CustomValues.FileName = textBoxSlider_3.Text;
            }
            else
            {
                this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.InternalName);
                this.PolicyCustomRule.CustomValues.InternalName = textBoxSlider_3.Text;
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

            if (!this.PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            if (this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                this.PolicyCustomRule.CustomValues.ProductName = textBoxSlider_2.Text;
            }
            else
            {
                this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.ProductName);
                this.PolicyCustomRule.CustomValues.ProductName = textBoxSlider_2.Text;
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

            if (!this.PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            if (this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                this.PolicyCustomRule.CustomValues.Publisher = textBoxSlider_1.Text;
            }
            else
            {
                this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FileDescription);
                this.PolicyCustomRule.CustomValues.Description = textBoxSlider_1.Text;
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

            if (!this.PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            this.PolicyCustomRule.CustomValues.FileName = textBoxSlider_0.Text;
            this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.OriginalFileName);
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
            if (!this.PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            this.PolicyCustomRule.CustomValues.MaxVersion = textBox_MaxVersion.Text;
        }

        /// <summary>
        /// Text has been modified in the Reference Textbox. Picks up the user input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReferenceFileTextChanged(object sender, EventArgs e)
        {
            if(this.PolicyCustomRule.UsingCustomValues)
            {
                this.PolicyCustomRule.CustomValues.Path = textBox_ReferenceFile.Text; 
            }
        }

        /// <summary>
        /// User has selected the UseCustomPath checkbox. Could be use custom path rules or custom hash rule.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UseCustomPath(object sender, EventArgs e)
        {
            if(this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Hash)
            {
                if (this.checkBox_CustomPath.Checked)
                {
                    this.richTextBox_CustomHashes.Visible = true;
                    this.richTextBox_CustomHashes.Location = this.panel_Publisher_Scroll.Location;
                    this.richTextBox_CustomHashes.Tag = "Title";

                    this.PolicyCustomRule.UsingCustomValues = true;
                    this.textBox_ReferenceFile.Text = String.Empty;
                }
                else
                {
                    this.richTextBox_CustomHashes.Visible = false;
                    this.PolicyCustomRule.UsingCustomValues = false;
                }
            }
            else
            {
                if (this.checkBox_CustomPath.Checked)
                {
                    this.PolicyCustomRule.UsingCustomValues = true;
                    this.textBox_ReferenceFile.ReadOnly = false;
                    this.textBox_ReferenceFile.Enabled = true; 
                    this.textBox_ReferenceFile.BackColor = Color.White; 
                }
                else
                {
                    this.PolicyCustomRule.UsingCustomValues = false;
                    this.textBox_ReferenceFile.ReadOnly = true;
                    this.textBox_ReferenceFile.Enabled = false;
                    this.textBox_ReferenceFile.BackColor = SystemColors.Control;
                    this.textBox_ReferenceFile.Text = String.Empty; 
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
            if(this.richTextBox_CustomHashes.Tag.ToString() == "Title")
            {
                this.richTextBox_CustomHashes.ResetText();
                this.richTextBox_CustomHashes.Tag = "Values"; 
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
            if (String.IsNullOrEmpty(this.textBox_Packaged_App.Text))
            {
                label_Error.Visible = true;
                label_Error.Text = Properties.Resources.PFNSearch_Warn;
                this.Log.AddWarningMsg("Empty packaged app search criteria");
                return;
            }

            // Check whether we are creating a PFN based on arbitrary package name
            if(!this.checkBox_CustomPFN.Checked)
            {
                // Searching for PFN on device
                // Prep UI
                this.panel_Progress.Visible = true;
                this.panel_Progress.BringToFront();
                this.label_Error.Visible = false;

                // Create background worker to display updates to UI
                if (!this.backgroundWorker.IsBusy)
                {
                    this.backgroundWorker.RunWorkerAsync();
                }
            }
            else
            {
                // Using arbitrary/custom PFN in rule creation
                // Add PFN to list with checkbox checked
                string arbitraryPFN = this.textBox_Packaged_App.Text; 
                if(arbitraryPFN.Length > 3)
                {
                    arbitraryPFN = String.Concat(arbitraryPFN.Where(c => !Char.IsWhiteSpace(c)));
                    this.checkedListBoxPackagedApps.Items.Add(arbitraryPFN, true);

                    // Once added to the table, clear the textbox automatically
                    this.textBox_Packaged_App.Clear(); 
                }
                else
                {
                    this.label_Error.Visible = true;
                    this.label_Error.Text = "Package Family name must be at least 3 characters.";
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
            this.backgroundWorker = sender as BackgroundWorker;
            string script = String.Format("Get-AppxPackage -Name *{0}*", this.textBox_Packaged_App.Text);
            // Create runspace
            Runspace runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            // Create the real pipeline
            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(script);
            pipeline.Commands.Add("Out-String");

            try
            {
                Collection<PSObject> results = pipeline.Invoke();
                this.FoundPackages = Helper.ParsePSOutput(results);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg(String.Format("Exception encountered in MergeCustomRulesPolicy(): {0}", exp));
            }

            if (this.FoundPackages.Count == 0)
            {
                label_Error.Visible = true;
                label_Error.Text = String.Format("No packages found with name: {0}", this.textBox_Packaged_App.Text);
                this.Log.AddWarningMsg(String.Format("No packaged apps found with name: {0}", this.textBox_Packaged_App.Text));
                return;
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
            this.panel_Progress.Visible = false;

            // Unsuccessful conversion
            if (e.Error != null)
            {
                this.Log.AddErrorMsg("ProcessPolicy() caught the following exception ", e.Error);
                
            }
            else
            {
                
            }

            this.Log.AddNewSeparationLine("Packaged App Searching Workflow -- DONE");

            // Bring checkbox list to front. Sort keys to display alphabetically to user
            this.checkedListBoxPackagedApps.BringToFront();
            var sortedPackages = this.FoundPackages.Keys.ToList();
            sortedPackages.Sort(); 

            foreach (var key in sortedPackages)
            {
                this.checkedListBoxPackagedApps.Items.Add(key, false);
            }
            //foreach($i in $package){$Rule += New-CIPolicyRule -Package $i} - in MainForm to resolve conflicts
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
            if(this.checkBox_CustomPFN.Checked)
            {
                this.buttonSearch.Text = "Create";
                this.PolicyCustomRule.UsingCustomValues = true; 
            }

            // Else, return text to 'Search' button
            // Unhide the PFN search UI
            // If there are any checked boxes, clear the list of arbitrary/custom PFN rules after prompting user
            else
            {
                if(this.checkedListBoxPackagedApps.Items.Count > 0)
                {
                    DialogResult res = MessageBox.Show("You have active custom PFN rules that will be deleted. Are you sure you want to switch to default PFN rule creation?", "Confirmation",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (res == DialogResult.Yes)
                    {
                        this.buttonSearch.Text = "Search";
                        this.PolicyCustomRule.UsingCustomValues = false;
                        int n_Rules = this.checkedListBoxPackagedApps.Items.Count; 

                        for(int j= 0; j < n_Rules; j++)
                        {
                            // Remove at the 0th index n_Rules times
                            this.checkedListBoxPackagedApps.Items.RemoveAt(0); 
                        }
                    }
                    else
                    {
                        this.checkBox_CustomPFN.Checked = true; 
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
            if(this.checkBoxEku.Checked)
            {
                this.textBoxEKU.Enabled = true;
                this.textBoxEKU.ReadOnly = false;
                this.textBoxEKU.BackColor = Color.White; 
                this.PolicyCustomRule.UsingCustomValues = true; 
            }
            else
            {
                this.textBoxEKU.Enabled = false;
                this.textBoxEKU.ReadOnly = true;
                this.textBoxEKU.BackColor = SystemColors.Control;
                this.textBoxEKU.Text = String.Empty;
                this.PolicyCustomRule.CustomValues.EKUFriendly = String.Empty; 

                // Reset the UsingCustomValues field iff not set custom using the checkbox
                if (!this.checkBox_CustomValues.Checked)
                {
                    this.PolicyCustomRule.UsingCustomValues = false; 
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
            this.PolicyCustomRule.CustomValues.EKUFriendly = this.textBoxEKU.Text; 
        }

       private void CheckBoxAttrib4CheckChanged(object sender, EventArgs e)
       {
            // Version
            if (this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                if(this.checkBoxAttribute4.Checked)
                {
                    if (this.textBoxSlider_4.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(this.textBoxSlider_4.Text))
                    {
                        this.PolicyCustomRule.CheckboxCheckStates.checkBox4 = true;
                        ClearLabel_ErrorText();
                        return; 
                    }
                    else
                    { 
                        SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                    }
                }
            }

            this.checkBoxAttribute4.Checked = false;
            this.PolicyCustomRule.CheckboxCheckStates.checkBox4 = false;
       }

        private void CheckBoxAttrib3CheckChanged(object sender, EventArgs e)
        {
            // File name || Internal name

            if (this.checkBoxAttribute3.Checked)
            {
                if (this.textBoxSlider_3.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(this.textBoxSlider_3.Text))
                {
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox3 = true;
                    ClearLabel_ErrorText();
                    return; 
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            this.checkBoxAttribute3.Checked = false;
            this.PolicyCustomRule.CheckboxCheckStates.checkBox3 = false;
        }

        private void CheckBoxAttrib2CheckChanged(object sender, EventArgs e)
        {
            // Product name (Pub rule) || Product name

            if (this.checkBoxAttribute2.Checked)
            {
                if (this.textBoxSlider_2.Text != Properties.Resources.DefaultFileAttributeString || 
                    String.IsNullOrEmpty(this.textBoxSlider_2.Text))
                {
                    ClearLabel_ErrorText();
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox2 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            this.checkBoxAttribute2.Checked = false;
            this.PolicyCustomRule.CheckboxCheckStates.checkBox2 = false;
        }

        private void CheckBoxAttrib1CheckChanged(object sender, EventArgs e)
        {
            // Publisher || File description

            if (this.checkBoxAttribute1.Checked)
            {
                if (this.textBoxSlider_1.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(this.textBoxSlider_1.Text))
                {
                    ClearLabel_ErrorText();
                    this.PolicyCustomRule.CheckboxCheckStates.checkBox1 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            this.checkBoxAttribute1.Checked = false;
            this.PolicyCustomRule.CheckboxCheckStates.checkBox1 = false;
        }

        private void CheckBoxAttrib0CheckChanged(object sender, EventArgs e)
        {
            // PCA Certificate || Original filename

            if (this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                // Cannot uncheck Issuing CA checkbox. Rule must include a root
                this.checkBoxAttribute0.Checked = true;
                this.PolicyCustomRule.CheckboxCheckStates.checkBox0 = true;
            }
            else // Original Filename
            {
                if (this.checkBoxAttribute0.Checked)
                {
                    if (this.textBoxSlider_0.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(this.textBoxSlider_0.Text))
                    {
                        ClearLabel_ErrorText();
                        this.PolicyCustomRule.CheckboxCheckStates.checkBox0 = true;
                        return; 
                    }
                    else
                    {
                        SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                    }
                }

                this.checkBoxAttribute0.Checked = false;
                this.PolicyCustomRule.CheckboxCheckStates.checkBox0 = false;
            }
        }
    }
}