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
        private bool RuleInEdit = false;
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
            this._MainWindow.CustomRuleinProgress = false;
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
        private void button_CreateRule_Click(object sender, EventArgs e)
        {
            // Assert that the reference file cannot be null, unless we are creating a custom value rule or a PFN rule
            if (this.PolicyCustomRule.ReferenceFile == null)
            {
                if(this.PolicyCustomRule.UsingCustomValues || this.PolicyCustomRule.Level == PolicyCustomRules.RuleLevel.PackagedFamilyName)
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

            // Check to make sure none of the fields are invalid
            // If the selected attribute is not found (UI will show Properties.Resources.DefaultFileAttributeString), do not allow creation
            if(this.PolicyCustomRule.Type != PolicyCustomRules.RuleType.Publisher)
            {
                if (this.PolicyCustomRule.Level == PolicyCustomRules.RuleLevel.None || (trackBar_Conditions.Value == 0
                && this.textBoxSlider_3.Text == Properties.Resources.DefaultFileAttributeString))
                {
                    label_Error.Visible = true;
                    label_Error.Text = Properties.Resources.InvalidAttributeSelection_Error;
                    this.Log.AddWarningMsg("Create button rule selected with an empty file attribute.");
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
            // Flag to warn user that N/A's in the CustomRules pane may result in a hash rule
            bool warnUser = false;

            // Ensure custom values are valid
            if (this.PolicyCustomRule.UsingCustomValues)
            {
                if(this.PolicyCustomRule.CustomValues.Publisher != null)
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

                if(this.PolicyCustomRule.CustomValues.MinVersion != null && this.PolicyCustomRule.CustomValues.MinVersion != "*") 
                {
                    if(!Helper.IsValidVersion(this.PolicyCustomRule.CustomValues.MinVersion))
                    {
                        label_Error.Visible = true;
                        label_Error.Text = Properties.Resources.InvalidVersionFormat_Error; 
                        this.Log.AddWarningMsg(String.Format("Invalid version format for CustomMinVersion", this.PolicyCustomRule.CustomValues.MinVersion));
                        return;
                    }
                }

                if (this.PolicyCustomRule.CustomValues.MaxVersion != null && this.PolicyCustomRule.CustomValues.MaxVersion != "*")
                {
                    if (!Helper.IsValidVersion(this.PolicyCustomRule.CustomValues.MaxVersion))
                    {
                        label_Error.Visible = true;
                        label_Error.Text = Properties.Resources.InvalidVersionFormat_Error;
                        this.Log.AddWarningMsg(String.Format("Invalid version format for CustomMinVersion: {0}", this.PolicyCustomRule.CustomValues.MaxVersion));
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
                        if(!String.IsNullOrEmpty(hash))
                        {
                            if(hash.Length == 64)
                            {
                                this.PolicyCustomRule.CustomValues.Hashes.Add(hash);
                            }
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

                // Check for N/A's only in the PCA and Publisher section 
                switch (this.PolicyCustomRule.Level)
                {
                    case PolicyCustomRules.RuleLevel.PcaCertificate:
                        if (this.PolicyCustomRule.FileInfo["PCACertificate"] == Properties.Resources.DefaultFileAttributeString)
                        {
                            warnUser = true;
                            this.Log.AddWarningMsg("RuleLevel.PCACertificate rule attempt with null attribute");
                        }
                        break;

                    case PolicyCustomRules.RuleLevel.Publisher:
                        if (this.PolicyCustomRule.FileInfo["PCACertificate"] == Properties.Resources.DefaultFileAttributeString ||
                            this.PolicyCustomRule.FileInfo["LeafCertificate"] == Properties.Resources.DefaultFileAttributeString)
                        {
                            warnUser = true;
                            this.Log.AddWarningMsg("RuleLevel.Publisher rule attempt with null attribute(s)");
                        }
                        break;
                }
            }
            else
            // Check for N/A's if not using custom rules only
            {
                switch (this.PolicyCustomRule.Level)
                {
                    case PolicyCustomRules.RuleLevel.PcaCertificate:
                        if (this.PolicyCustomRule.FileInfo["PCACertificate"] == Properties.Resources.DefaultFileAttributeString)
                        {
                            warnUser = true;
                            this.Log.AddWarningMsg("RuleLevel.PCACertificate rule attempt with null attribute");
                        }
                        break;

                    case PolicyCustomRules.RuleLevel.Publisher:
                        if (this.PolicyCustomRule.FileInfo["PCACertificate"] == Properties.Resources.DefaultFileAttributeString ||
                            this.PolicyCustomRule.FileInfo["LeafCertificate"] == Properties.Resources.DefaultFileAttributeString)
                        {
                            warnUser = true;
                            this.Log.AddWarningMsg("RuleLevel.Publisher rule attempt with null attribute(s)");
                        }
                        break;

                    case PolicyCustomRules.RuleLevel.SignedVersion:
                        if (this.PolicyCustomRule.FileInfo["PCACertificate"] == Properties.Resources.DefaultFileAttributeString ||
                            this.PolicyCustomRule.FileInfo["LeafCertificate"] == Properties.Resources.DefaultFileAttributeString ||
                            this.PolicyCustomRule.FileInfo["FileVersion"] == Properties.Resources.DefaultFileAttributeString)
                        // There is a bug in the cmdlets where SignedVersion rules will be created with a null version. 
                        // Wizard will enforce null versions falling back to hash
                        {
                            warnUser = true;
                            this.Log.AddWarningMsg("RuleLevel.SignedVersion rule attempt with null attribute(s)");
                        }
                        break;

                    case PolicyCustomRules.RuleLevel.FilePublisher:
                        if (this.PolicyCustomRule.FileInfo["PCACertificate"] == Properties.Resources.DefaultFileAttributeString ||
                            this.PolicyCustomRule.FileInfo["LeafCertificate"] == Properties.Resources.DefaultFileAttributeString ||
                            this.PolicyCustomRule.FileInfo["FileVersion"] == Properties.Resources.DefaultFileAttributeString ||
                            this.PolicyCustomRule.FileInfo["FileName"] == Properties.Resources.DefaultFileAttributeString)
                        {
                            warnUser = true;
                            this.Log.AddWarningMsg("RuleLevel.FilePublisher rule attempt with null attribute(s)");
                        }
                        break;
                }
            }

            if(warnUser)
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
                    return; 
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

            switch (this.PolicyCustomRule.Level)
            {
                case PolicyCustomRules.RuleLevel.PcaCertificate:

                    name += String.Format("{0}: {1} ", this.PolicyCustomRule.Level, this.PolicyCustomRule.FileInfo["PCACertificate"]);
                    break;
                case PolicyCustomRules.RuleLevel.Publisher:
                    
                    if (this.PolicyCustomRule.UsingCustomValues)
                    {
                        name = String.Format("{0}: {1} & CN={2}", this.PolicyCustomRule.Level, this.PolicyCustomRule.FileInfo["PCACertificate"],
                             String.IsNullOrEmpty(this.PolicyCustomRule.CustomValues.Publisher) ? this.PolicyCustomRule.FileInfo["LeafCertificate"] : this.PolicyCustomRule.CustomValues.Publisher);
                    }
                    else
                    {
                        name += String.Format("{0}: {1} & CN={2}", this.PolicyCustomRule.Level, this.PolicyCustomRule.FileInfo["PCACertificate"],
                        this.PolicyCustomRule.FileInfo["LeafCertificate"]);
                    }
                    break;

                case PolicyCustomRules.RuleLevel.SignedVersion:
                    
                    if (this.PolicyCustomRule.UsingCustomValues)
                    {
                        name = String.Format("{0}: {1}, CN={2} & versions {3} {4}", this.PolicyCustomRule.Level, this.PolicyCustomRule.FileInfo["PCACertificate"],
                            String.IsNullOrEmpty(this.PolicyCustomRule.CustomValues.Publisher) ? this.PolicyCustomRule.FileInfo["LeafCertificate"] : this.PolicyCustomRule.CustomValues.Publisher,
                             String.IsNullOrEmpty(this.PolicyCustomRule.CustomValues.MinVersion) ? this.PolicyCustomRule.FileInfo["FileVersion"] : this.PolicyCustomRule.CustomValues.MinVersion,
                             String.IsNullOrEmpty(this.PolicyCustomRule.CustomValues.MaxVersion) ? "and greater" : " up to " + this.PolicyCustomRule.CustomValues.MaxVersion);
                    }
                    else
                    {
                        name = String.Format("{0}: {1}, CN={2} & version {3} and greater ", this.PolicyCustomRule.Level, this.PolicyCustomRule.FileInfo["PCACertificate"],
                        this.PolicyCustomRule.FileInfo["LeafCertificate"], this.PolicyCustomRule.FileInfo["FileVersion"]);
                    }
                    break;

                case PolicyCustomRules.RuleLevel.FilePublisher:
                    if (this.PolicyCustomRule.UsingCustomValues)
                    {
                         name = String.Format("{0}: {1}, CN={2} & versions {3} {4} with filename = {5}", this.PolicyCustomRule.Level, this.PolicyCustomRule.FileInfo["PCACertificate"],
                             String.IsNullOrEmpty(this.PolicyCustomRule.CustomValues.Publisher) ? this.PolicyCustomRule.FileInfo["LeafCertificate"] : this.PolicyCustomRule.CustomValues.Publisher,
                             String.IsNullOrEmpty(this.PolicyCustomRule.CustomValues.MinVersion) ? this.PolicyCustomRule.FileInfo["FileVersion"] : this.PolicyCustomRule.CustomValues.MinVersion,
                             String.IsNullOrEmpty(this.PolicyCustomRule.CustomValues.MaxVersion) ? "and greater" : " up to " + this.PolicyCustomRule.CustomValues.MaxVersion,
                             String.IsNullOrEmpty(this.PolicyCustomRule.CustomValues.FileName) ? this.PolicyCustomRule.FileInfo["FileName"] : this.PolicyCustomRule.CustomValues.FileName); 

                    }
                    else
                    {
                        name = String.Format("{0}: {1}, CN={2} & version {3} and greater with filename = {4} ", this.PolicyCustomRule.Level, this.PolicyCustomRule.FileInfo["PCACertificate"],
                       this.PolicyCustomRule.FileInfo["LeafCertificate"], this.PolicyCustomRule.FileInfo["FileVersion"], this.PolicyCustomRule.FileInfo["FileName"]);
                    }
                   
                    break;

                case PolicyCustomRules.RuleLevel.OriginalFileName:
                    if (this.PolicyCustomRule.UsingCustomValues)
                    {
                        name = String.Format("{0}; {1}", this.PolicyCustomRule.Level, this.PolicyCustomRule.CustomValues.FileName); 
                    }
                    else
                    {
                        name = String.Format("{0}; {1}", this.PolicyCustomRule.Level, this.PolicyCustomRule.FileInfo["OriginalFilename"]);
                    }
                    break;

                case PolicyCustomRules.RuleLevel.InternalName:
                    if (this.PolicyCustomRule.UsingCustomValues)
                    {
                        name = String.Format("{0}; {1}", this.PolicyCustomRule.Level, this.PolicyCustomRule.CustomValues.InternalName);
                    }
                    else
                    {
                        name = String.Format("{0}; {1}", this.PolicyCustomRule.Level, this.PolicyCustomRule.FileInfo["InternalName"]);
                    }
                    break;

                case PolicyCustomRules.RuleLevel.FileDescription:
                    if (this.PolicyCustomRule.UsingCustomValues)
                    {
                        name = String.Format("{0}; {1}", this.PolicyCustomRule.Level, this.PolicyCustomRule.CustomValues.Description);
                    }
                    else
                    {
                        name = String.Format("{0}; {1}", this.PolicyCustomRule.Level, this.PolicyCustomRule.FileInfo["FileDescription"]);
                    }
                    break;

                case PolicyCustomRules.RuleLevel.ProductName:
                    if (this.PolicyCustomRule.UsingCustomValues)
                    {
                        name = String.Format("{0}; {1}", this.PolicyCustomRule.Level, this.PolicyCustomRule.CustomValues.ProductName);
                    }
                    else
                    {
                        name = String.Format("{0}; {1}", this.PolicyCustomRule.Level, this.PolicyCustomRule.FileInfo["ProductName"]);
                    }
                    break;

                case PolicyCustomRules.RuleLevel.PackagedFamilyName:
                    
                    name = String.Format("{0}; {1}", this.PolicyCustomRule.Level, this.textBox_Packaged_App.Text);
                    files = Helper.GetListofPackages(this.PolicyCustomRule); 
                    break;

                case PolicyCustomRules.RuleLevel.Folder:
                    
                    if (this.PolicyCustomRule.UsingCustomValues)
                    {
                        name = String.Format("{0}; {1}", this.PolicyCustomRule.Level, this.PolicyCustomRule.CustomValues.Path);
                    }
                    else
                    {
                        name = String.Format("{0}; {1}", this.PolicyCustomRule.Level,  this.PolicyCustomRule.ReferenceFile);
                    }
                    break;

                case PolicyCustomRules.RuleLevel.FilePath:

                    if (this.PolicyCustomRule.UsingCustomValues)
                    {
                        name = String.Format("{0}; {1}", this.PolicyCustomRule.Level, this.PolicyCustomRule.CustomValues.Path);
                    }
                    else
                    {
                        name = String.Format("{0}; {1}", this.PolicyCustomRule.Level, this.PolicyCustomRule.ReferenceFile);
                    }
                    break;

                default:
                    name = String.Format("{0}; {1}", this.PolicyCustomRule.Level, String.IsNullOrEmpty(this.PolicyCustomRule.ReferenceFile) ? "Custom Hash List" : this.PolicyCustomRule.ReferenceFile);
                    break;
            }

            // Handle exceptions
            if(this.PolicyCustomRule.ExceptionList.Count > 0)
            {
                foreach(var exception in this.PolicyCustomRule.ExceptionList)
                {
                    string exceptionString = String.Format("{0}: {1} {2}; ", exception.Permission, exception.Level,
                        exception.ReferenceFile);
                    exceptions += exceptionString; 
                }

                // Remove trailing semi-colon
                exceptions.Trim(';');
            }

            this.Log.AddInfoMsg(String.Format("CUSTOM RULE Created: {0} - {1} - {2} ", action, level, name));
            string[] stringArr = new string[5] { action , level, name, files, exceptions};

            // Offboard this to signingRules_Condition
            this.RuleInEdit = false;
            this.SigningControl.AddRuleToTable(stringArr, this.PolicyCustomRule, warnUser);

            // Renew the custom rule instance
            this.PolicyCustomRule = new PolicyCustomRules();

            // Reset UI view
            ClearCustomRulesPanel(true);
            this._MainWindow.CustomRuleinProgress = false;
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
                    this.PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.FileAttributes);
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
            publisherInfoLabel.Visible = false;
            trackBar_Conditions.ResetText();
            trackBar_Conditions.Value = 0; // default bottom position 

            //File Path:
            panel_FileFolder.Visible = false;

            //Other
            textBox_ReferenceFile.Clear();
            radioButton_Allow.Checked = true;   //default
            label_Info.Visible = false;

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
                    return;

                this.DefaultValues[4] = refPath; 

                // Custom rule in progress
                this._MainWindow.CustomRuleinProgress = true;

                // Get generic file information to be shown to user
                PolicyCustomRule.FileInfo = new Dictionary<string, string>(); // Reset dict
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(refPath);
                
                PolicyCustomRule.ReferenceFile = fileInfo.FileName; // Returns the file path
                PolicyCustomRule.FileInfo.Add("CompanyName", String.IsNullOrEmpty(fileInfo.CompanyName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.CompanyName);
                PolicyCustomRule.FileInfo.Add("ProductName", String.IsNullOrEmpty(fileInfo.ProductName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.ProductName);
                PolicyCustomRule.FileInfo.Add("OriginalFilename", String.IsNullOrEmpty(fileInfo.OriginalFilename) ? Properties.Resources.DefaultFileAttributeString : fileInfo.OriginalFilename);
                PolicyCustomRule.FileInfo.Add("FileVersion", String.IsNullOrEmpty(fileInfo.FileVersion) ? Properties.Resources.DefaultFileAttributeString : fileInfo.FileVersion.Replace(',', '.')); // Replace misleading commas in version with '.'
                PolicyCustomRule.FileInfo.Add("FileName", String.IsNullOrEmpty(fileInfo.OriginalFilename) ? Properties.Resources.DefaultFileAttributeString : fileInfo.OriginalFilename); // WDAC configCI uses original filename 
                PolicyCustomRule.FileInfo.Add("FileDescription", String.IsNullOrEmpty(fileInfo.FileDescription) ? Properties.Resources.DefaultFileAttributeString : fileInfo.FileDescription);
                PolicyCustomRule.FileInfo.Add("InternalName", String.IsNullOrEmpty(fileInfo.InternalName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.InternalName);

                // Get cert chain info to be shown to the user if type is publisher. Otherwise, we don't check or try to build the cert chain
                string leafCertSubjectName = "";
                string pcaCertSubjectName = "";

                if(this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
                {
                    try
                    {
                        var signer = X509Certificate.CreateFromSignedFile(refPath);

                        var cert = new X509Certificate2(signer);
                        var certChain = new X509Chain();
                        var certChainIsValid = certChain.Build(cert);

                        leafCertSubjectName = cert.SubjectName.Name;
                        leafCertSubjectName = FormatSubjectName(leafCertSubjectName);

                        if (certChain.ChainElements.Count > 1)
                        {
                            pcaCertSubjectName = certChain.ChainElements[1].Certificate.SubjectName.Name;
                            // Remove everything past C=..
                            pcaCertSubjectName = FormatSubjectName(pcaCertSubjectName);
                        }
                    }

                    catch (Exception exp)
                    {
                        this._MainWindow.Log.AddErrorMsg(String.Format("Caught exception {0} when trying to create cert from the following signed file {1}", exp, refPath));
                        this.label_Error.Text = "Unable to find certificate chain for " + fileInfo.FileName;
                        this.label_Error.Visible = true;

                        Timer settingsUpdateNotificationTimer = new Timer();
                        settingsUpdateNotificationTimer.Interval = (5000); // 1.5 secs
                        settingsUpdateNotificationTimer.Tick += new EventHandler(SettingUpdateTimer_Tick);
                        settingsUpdateNotificationTimer.Start();
                    }
                }
                
                PolicyCustomRule.FileInfo.Add("LeafCertificate", String.IsNullOrEmpty(leafCertSubjectName) ? Properties.Resources.DefaultFileAttributeString : leafCertSubjectName);
                PolicyCustomRule.FileInfo.Add("PCACertificate", String.IsNullOrEmpty(pcaCertSubjectName) ? Properties.Resources.DefaultFileAttributeString : pcaCertSubjectName);
            }

            // Set the landing UI depending on the Rule type
            switch (this.PolicyCustomRule.Type)
            {
                case PolicyCustomRules.RuleType.Publisher:

                    // UI
                    this.textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    // Show right side of the text
                    this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                    this.textBox_ReferenceFile.ScrollToCaret();
                    this.labelSlider_0.Text = "Issuing CA:";
                    this.labelSlider_1.Text = "Publisher:";
                    this.labelSlider_2.Text = "Min version:";
                    this.labelSlider_3.Text = "File name:";

                    // Version textbox should be set to normal size
                    this.textBoxSlider_2.Size = this.textBoxSlider_3.Size; 

                    // Set defaults to restore to if custom values is ever reset
                    this.DefaultValues[0] = PolicyCustomRule.FileInfo["PCACertificate"];
                    this.DefaultValues[1] = PolicyCustomRule.FileInfo["LeafCertificate"];
                    this.DefaultValues[2] = PolicyCustomRule.FileInfo["FileVersion"];
                    this.DefaultValues[3] = PolicyCustomRule.FileInfo["FileName"];

                    this.textBoxSlider_0.Text = this.DefaultValues[0];
                    this.textBoxSlider_1.Text = this.DefaultValues[1];
                    this.textBoxSlider_2.Text = this.DefaultValues[2];
                    this.textBoxSlider_3.Text = this.DefaultValues[3];
                    
                    this.textBoxSlider_0.BackColor = Color.FromArgb(240, 240, 240); 

                    panel_Publisher_Scroll.Visible = true;
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Text = Properties.Resources.FilePublisherInfo; 
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
                    this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                    this.textBox_ReferenceFile.ScrollToCaret();
                    break;


                case PolicyCustomRules.RuleType.FilePath:

                    // FILE LEVEL

                    // UI updates
                    radioButton_File.Checked = true;
                    this.textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    // Show right side of the text
                    this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                    this.textBox_ReferenceFile.ScrollToCaret();

                    panel_Publisher_Scroll.Visible = false;
                    break;

                case PolicyCustomRules.RuleType.FileAttributes:
                    // Creates a rule -Level FileName -SpecificFileNameLevel InternalName, FileDescription

                    // UI 
                    this.textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    // Show right side of the text
                    this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                    this.textBox_ReferenceFile.ScrollToCaret();

                    this.labelSlider_0.Text = "Original filename:";
                    this.labelSlider_1.Text = "File description:";
                    this.labelSlider_2.Text = "Product name:";
                    this.labelSlider_3.Text = "Internal name:";

                    // Product Name textbox should be set to normal size
                    this.textBoxSlider_2.Size = this.textBoxSlider_3.Size;

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
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Text = "Rule applies to all files with this file description attribute.";

                    break;

                case PolicyCustomRules.RuleType.Hash:

                    // UI updates
                    panel_Publisher_Scroll.Visible = false;
                    this.textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    // Show right side of the text
                    this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                    this.textBox_ReferenceFile.ScrollToCaret();
                    break;
            }

            // Returned from exceptions user control to modify the reference path
            if(this.exceptionsControl != null)
            {
                this.redoRequired = true; 
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

        private void TrackBar_Conditions_Scroll(object sender, EventArgs e)
        {
            int pos = trackBar_Conditions.Value; //Publisher file rules conditions
            label_Error.Visible = false; // Clear error label

            switch (this.PolicyCustomRule.Type)
            {
                case PolicyCustomRules.RuleType.Publisher:
                    {
                        // Setting the trackBar values snaps the cursor to one of the four options
                        if (pos <= 2)
                        {
                            // PCACert + LeafCert + Version + FileName = FilePublisher
                            trackBar_Conditions.Value = 0;
                            this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FilePublisher);
                            textBoxSlider_3.Text = this.PolicyCustomRule.FileInfo["FileName"];
                            publisherInfoLabel.Text = Properties.Resources.FilePublisherInfo;
                            this.Log.AddInfoMsg("Publisher file rule level set to file publisher (0)");
                        }
                        else if (pos > 2 && pos <= 6)
                        {
                            // PCACert + LeafCert + Version = SignedVersion
                            trackBar_Conditions.Value = 4;
                            this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.SignedVersion);
                            textBoxSlider_2.Text = this.PolicyCustomRule.FileInfo["FileVersion"];
                            textBoxSlider_3.Text = "*";
                            publisherInfoLabel.Text = Properties.Resources.SignedVersionInfo; 
                            this.Log.AddInfoMsg("Publisher file rule level set to file publisher (4)");
                        }
                        else if (pos > 6 && pos <= 10)
                        {
                            // PCACert + LeafCert  = Publisher
                            trackBar_Conditions.Value = 8;
                            this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.Publisher);
                            textBoxSlider_1.Text = this.PolicyCustomRule.FileInfo["LeafCertificate"];
                            textBoxSlider_2.Text = "*";
                            textBoxSlider_3.Text = "*";
                            publisherInfoLabel.Text = Properties.Resources.PublisherInfo;
                            this.Log.AddInfoMsg("Publisher file rule level set to publisher (8)");
                        }
                        else
                        {
                            // PCACert = PCACertificate
                            trackBar_Conditions.Value = 12;
                            this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.PcaCertificate);
                            textBoxSlider_0.Text = this.PolicyCustomRule.FileInfo["PCACertificate"];
                            textBoxSlider_1.Text = "*";
                            textBoxSlider_2.Text = "*";
                            textBoxSlider_3.Text = "*";
                            publisherInfoLabel.Text = Properties.Resources.PCACertificateInfo; 
                            this.Log.AddInfoMsg("Publisher file rule level set to PCA certificate (12)");
                        }
                    }
                    break;

                case PolicyCustomRules.RuleType.FileAttributes:
                    {
                        // Setting the trackBar values snaps the cursor to one of the four options
                        textBoxSlider_3.Text = "*"; //@"Internal name:";
                        textBoxSlider_2.Text = "*"; //@"Product name:";
                        textBoxSlider_1.Text = "*"; //@"File description
                        textBoxSlider_0.Text = "*"; //@"Original file name:";

                        if (pos <= 2)
                        {
                            // Internal name
                            trackBar_Conditions.Value = 0;
                            this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.InternalName);
                            textBoxSlider_3.Text = this.PolicyCustomRule.FileInfo["InternalName"];

                            // If attribute is not applicable, set to RuleLevel = None to block from creating rule in button_Create_Click
                            if (textBoxSlider_3.Text == Properties.Resources.DefaultFileAttributeString)
                                this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.None);
                            publisherInfoLabel.Text = "Rule applies to all files with this internal name attribute.";
                            this.Log.AddInfoMsg(String.Format("Publisher file rule level set to file internal name: {0}", textBoxSlider_3.Text));
                        }
                        else if (pos > 2 && pos <= 6)
                        {
                            // Product name
                            trackBar_Conditions.Value = 4;
                            this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.ProductName);
                            textBoxSlider_2.Text = this.PolicyCustomRule.FileInfo["ProductName"];

                            // If attribute is not applicable, set to RuleLevel = None to block from creating rule in button_Create_Click
                            if (textBoxSlider_2.Text == Properties.Resources.DefaultFileAttributeString)
                                this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.None);
                            publisherInfoLabel.Text = "Rule applies to all files with this product name attribute.";
                            this.Log.AddInfoMsg(String.Format("Publisher file rule level set to product name: {0}", textBoxSlider_2.Text));
                        }
                        else if (pos > 6 && pos <= 10)
                        {
                            //File description
                            trackBar_Conditions.Value = 8;
                            this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FileDescription);
                            textBoxSlider_1.Text = this.PolicyCustomRule.FileInfo["FileDescription"];

                            // If attribute is not applicable, set to RuleLevel = None to block from creating rule in button_Create_Click
                            if (textBoxSlider_1.Text == Properties.Resources.DefaultFileAttributeString)
                                this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.None);
                            publisherInfoLabel.Text = "Rule applies to all files with this file description attribute.";
                            this.Log.AddInfoMsg(String.Format("Publisher file rule level set to file description: {0}", textBoxSlider_1.Text));
                        }
                        else
                        {
                            //Original filename
                            trackBar_Conditions.Value = 12;
                            this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.OriginalFileName);
                            textBoxSlider_0.Text = this.PolicyCustomRule.FileInfo["OriginalFilename"];

                            // If attribute is not applicable, set to RuleLevel = None to block from creating rule in button_Create_Click
                            if (textBoxSlider_0.Text == Properties.Resources.DefaultFileAttributeString)
                                this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.None);
                            publisherInfoLabel.Text = "Rule applies to all files with this original file name attribute.";
                            this.Log.AddInfoMsg(String.Format("Publisher file rule level set to original file name: {0}", textBoxSlider_0.Text));

                        }
                    }
                    break;
            }

            // Returned from exceptions user control to modify the level of the publisher rule
            if (this.exceptionsControl != null)
            {
                this.redoRequired = true;
            }
        }

        /// <summary>
        /// Opens the file dialog and grabs the file path for PEs only and checks if path exists. 
        /// </summary>
        /// <returns>Returns the full path+name of the file</returns>
        private string GetFileLocation()
        {
            //TODO: move these common functions to a separate class
            // Open file dialog to get file or folder path

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse for a signed file to use as a reference for the rule.";
            openFileDialog.CheckPathExists = true;
            // Performed scan of program files -- most common filetypes (occurence > 20 in the folder) with SIPs: 
            openFileDialog.Filter = "Portable Executable Files (*.exe; *.dll; *.rll; *.bin)|*.EXE;*.DLL;*.RLL;*.BIN|" +
                "Script Files (*.ps1, *.bat, *.vbs, *.js)|*.PS1;*.BAT;*.VBS, *.JS|" +
                "System Files (*.sys, *.hxs, *.mui, *.lex, *.mof)|*.SYS;*.HXS;*.MUI;*.LEX;*.MOF|" +
                "All Binary Files (*.exe, ...) |*.EXE;*.DLL;*.RLL;*.BIN,*.PS1;*.BAT;*.VBS, *.JS, *.SYS;*.HXS;*.MUI;*.LEX;*.MOF|" +
                "All files (*.*)|*.*";

            openFileDialog.FilterIndex = 4; // Display All Binary Files by default (everything)

            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                openFileDialog.Dispose();
                return openFileDialog.FileName;
            }
            else
            {
                return String.Empty;
            }
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
        }

        /// <summary>
        /// Opens the folder dialog and grabs the folder path. Requires Folder to be toggled when Browse button 
        /// is selected. 
        /// </summary>
        /// <returns>Returns the full path of the folder</returns>
        private string GetFolderLocation()
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            openFolderDialog.Description = "Browse for a folder to use as a reference for the rule.";

            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                openFolderDialog.Dispose();
                return openFolderDialog.SelectedPath;
            }
            else
            {
                return String.Empty;
            }
        }

        private void SettingUpdateTimer_Tick(object sender, EventArgs e)
        {
            this.label_Error.Visible = false;
        }

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

        private void Button_Next_Click(object sender, EventArgs e)
        {
            // Show the exception UI

            // Check custom values first before proceeding
            // Ensure custom values are valid
            if (this.PolicyCustomRule.UsingCustomValues)
            {
                if (this.PolicyCustomRule.CustomValues.MinVersion != null)
                {
                    if (!Helper.IsValidVersion(this.PolicyCustomRule.CustomValues.MinVersion))
                    {
                        label_Error.Visible = true;
                        label_Error.Text = Properties.Resources.InvalidVersion_Error;
                        this.Log.AddWarningMsg("Invalid version format for CustomMinVersion");
                        return;
                    }
                }

                if (this.PolicyCustomRule.CustomValues.FileName != null)
                {
                    // Some check here - do not know what at the momemnt
                }
            }

            // Check required fields - that a reference file is selected
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

        private void button_Back_Click(object sender, EventArgs e)
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

        public void SetLabel_ErrorText(string errorText, bool shouldPersist=false)
        {
            this.label_Error.Focus();
            this.label_Error.BringToFront();

            this.label_Error.Text = errorText; 
            this.label_Error.Visible = true;

            if (!shouldPersist)
            {
                Timer settingsUpdateNotificationTimer = new Timer();
                settingsUpdateNotificationTimer.Interval = (5000);
                settingsUpdateNotificationTimer.Tick += new EventHandler(SettingUpdateTimer_Tick);
                settingsUpdateNotificationTimer.Start();
            }
        }

        public void ClearLabel_ErrorText()
        {
            this.label_Error.Text = "";
            this.label_Error.Visible = false;
        }

        // Add exception button clicked. 
        private void button_AddException_Click(object sender, EventArgs e)
        {
            this.exceptionsControl.AddException(); 
        }

        // Custom Rule is Deny Rule
        private void radioButton_Deny_Click(object sender, EventArgs e)
        {
            this.PolicyCustomRule.Permission = PolicyCustomRules.RulePermission.Deny;
            this.Log.AddInfoMsg("Rule Permission set to " + this.PolicyCustomRule.Permission.ToString());

            // Returned back from exceptions to change Rule Type - Redo is required
            if (this.exceptionsControl != null)
            {
                this.redoRequired = true; 
            }
        }

        // Custom Rule is an Allow Rule
        private void radioButton_Allow_Click(object sender, EventArgs e)
        {
            this.PolicyCustomRule.Permission = PolicyCustomRules.RulePermission.Allow;
            this.Log.AddInfoMsg("Rule Permission set to " + this.PolicyCustomRule.Permission.ToString());

            // Returned back from exceptions to change Rule Type - Redo is required
            if (this.exceptionsControl != null)
            {
                this.redoRequired = true;
            }
        }

        private string FormatSubjectName(string certSubjectName)
        {
            // Remove unwanted info from the subject name (C= onwards)
            int country_idx = certSubjectName.IndexOf("C=");
            if (country_idx > 1)
            {
                int comma_idx = certSubjectName.IndexOf(',', country_idx);
                if (comma_idx > 1)
                {
                    certSubjectName = certSubjectName.Substring(0, comma_idx);
                }
            }

            return certSubjectName;
        }

        private void UseRuleCustomValues(object sender, EventArgs e)
        {
            // Set the UI first
            if (this.checkBox_CustomValues.Checked)
            {
                if(this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.FileAttributes)
                {
                    this.textBoxSlider_0.ReadOnly = false;
                    this.textBoxSlider_1.ReadOnly = false;
                    this.textBoxSlider_2.ReadOnly = false;
                    this.textBoxSlider_3.ReadOnly = false;
                    this.textBox_MaxVersion.ReadOnly = false;

                    // Format the version text boxes
                    this.textBoxSlider_2.Size = this.textBoxSlider_0.Size;
                    this.textBox_MaxVersion.Visible = false;
                    this.label_To.Visible = false;
                }
                else
                {
                    // Publisher 
                    this.textBoxSlider_0.ReadOnly = true; // Custom text values for PCA are not supported
                    this.textBoxSlider_1.ReadOnly = false; 
                    this.textBoxSlider_2.ReadOnly = false;
                    this.textBoxSlider_3.ReadOnly = false;
                    this.textBox_MaxVersion.ReadOnly = false;

                    // Format the version text boxes
                    this.textBoxSlider_2.Size = this.textBox_MaxVersion.Size;
                    this.labelSlider_2.Text = "Version range:";
                    this.textBox_MaxVersion.Visible = true;
                    this.label_To.Visible = true;
                }

                this.PolicyCustomRule.UsingCustomValues = true; 
            }
            else
            {
                // Clear error if applicable
                this.ClearLabel_ErrorText(); 

                // Set text values back to default
                this.textBoxSlider_0.ReadOnly = true; 
                this.textBoxSlider_1.ReadOnly = true; 
                this.textBoxSlider_2.ReadOnly = true; 
                this.textBoxSlider_3.ReadOnly = true; 
                this.textBox_MaxVersion.ReadOnly = true;

                // Format the version text boxes
                this.textBoxSlider_2.Size = this.textBoxSlider_0.Size;
                this.labelSlider_2.Text = "Min version:";
                this.textBox_MaxVersion.Visible = false;
                this.label_To.Visible = false;

                this.textBoxSlider_0.Text = this.DefaultValues[0];
                this.textBoxSlider_1.Text = this.DefaultValues[1];
                this.textBoxSlider_2.Text = this.DefaultValues[2];
                this.textBoxSlider_3.Text = this.DefaultValues[3];

                this.PolicyCustomRule.UsingCustomValues = false;
            }
        }

        // Action handlers for custom text in the publiser and file attributes text fields

        private void textBoxSlider_3_TextChanged(object sender, EventArgs e)
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
                this.PolicyCustomRule.CustomValues.InternalName = textBoxSlider_3.Text;
            }
        }

        private void textBoxSlider_2_TextChanged(object sender, EventArgs e)
        {
            // Version (publisher) or ProductName (file attributes)
            // Break if not using custom values. This will be reached during setting values once proto file is chosen

            if (!this.PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            if (this.PolicyCustomRule.Type == PolicyCustomRules.RuleType.Publisher)
            {
                this.PolicyCustomRule.CustomValues.MinVersion = textBoxSlider_2.Text;
            }
            else
            {
                this.PolicyCustomRule.CustomValues.ProductName = textBoxSlider_2.Text;
            }
        }

        private void textBoxSlider_1_TextChanged(object sender, EventArgs e)
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
                this.PolicyCustomRule.CustomValues.Description = textBoxSlider_1.Text;
            }
            
        }

        private void textBoxSlider_0_TextChanged(object sender, EventArgs e)
        {
            /// Only accessible by File Attributes
            // Original filename (file attributes)
            // Break if not using custom values. This will be reached during setting values once proto file is chosen

            if (!this.PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            this.PolicyCustomRule.CustomValues.FileName = textBoxSlider_0.Text;
        }

        private void textBox_MaxVersion_TextChanged(object sender, EventArgs e)
        {
            // Only accessible by publisher
            // Set Custom Values.MaxValue
            if (!this.PolicyCustomRule.UsingCustomValues)
            {
                return;
            }

            this.PolicyCustomRule.CustomValues.MaxVersion = textBox_MaxVersion.Text;
        }

        // Allow for custom text fields
        private void ReferenceFileTextChanged(object sender, EventArgs e)
        {
            if(this.PolicyCustomRule.UsingCustomValues)
            {
                this.PolicyCustomRule.CustomValues.Path = textBox_ReferenceFile.Text; 
            }
        }

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
                }
                else
                {
                    this.PolicyCustomRule.UsingCustomValues = false;
                    this.textBox_ReferenceFile.ReadOnly = true;
                }
            }
           
        }

        private void richTextBox_CustomHashes_Click(object sender, EventArgs e)
        {
            if(this.richTextBox_CustomHashes.Tag.ToString() == "Title")
            {
                this.richTextBox_CustomHashes.ResetText();
                this.richTextBox_CustomHashes.Tag = "Values"; 
            }
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            
        }

        // If enter button is clicked, start search process
        private void textBox_Packaged_App_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //enter key is down
                ButtonSearch_Click(sender, e); 
            }
        }

        // Event handler to begin searching for packaged apps
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

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
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

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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
    }
}   

