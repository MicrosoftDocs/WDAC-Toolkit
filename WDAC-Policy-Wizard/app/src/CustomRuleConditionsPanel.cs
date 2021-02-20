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
using System.IO;


namespace WDAC_Wizard
{
    public partial class CustomRuleConditionsPanel : Form
    {
        // CI Policy objects
        private WDAC_Policy Policy;
        private PolicyCustomRules PolicyCustomRule;     // One instance of a custom rule. Appended to Policy.CustomRules
        private List<string> AllFilesinFolder;          // List to track all files in a folder 

        private Logger Log;
        private MainWindow _MainWindow;
        private SigningRules_Control SigningControl;
        private bool RuleInEdit = false; 

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
        }

        /// <summary>
        /// Appends the custom rule to the bottom of the DataGridView and creates the rule in the CustomRules list. 
        /// </summary>
        private void button_Create_Click(object sender, EventArgs e)
        {
            // At a minimum, we need  rule level, and pub/hash/file - defult fallback
            if (!radioButton_Allow.Checked && !radioButton_Deny.Checked || this.PolicyCustomRule.ReferenceFile == null)
            {
                label_Error.Visible = true;
                label_Error.Text = "Please select a rule type, a file and whether to allow or deny.";
                this.Log.AddWarningMsg("Create button rule selected without allow/deny setting and a reference file.");
                return;
            }

            // Check to make sure none of the fields are invalid
            // If the selected attribute is not found (UI will show Properties.Resources.DefaultFileAttributeString), do not allow creation
            if (this.PolicyCustomRule.GetRuleLevel() == PolicyCustomRules.RuleLevel.None || (trackBar_Conditions.Value == 0
                && this.textBoxSlider_3.Text == Properties.Resources.DefaultFileAttributeString))
            {
                label_Error.Visible = true;
                label_Error.Text = "The file attribute selected cannot be N/A. Please select another attribute or rule type";
                this.Log.AddWarningMsg("Create button rule selected with an empty file attribute.");
                return;
            }

            bool warnUser = false; 
            switch(this.PolicyCustomRule.GetRuleLevel())
            {
                case PolicyCustomRules.RuleLevel.PcaCertificate:
                    if(this.PolicyCustomRule.FileInfo["PCACertificate"] == Properties.Resources.DefaultFileAttributeString)
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
                        this.PolicyCustomRule.FileInfo["FileVersion"] == Properties.Resources.DefaultVersionString)
                    {
                        warnUser = true;
                        this.Log.AddWarningMsg("RuleLevel.SignedVersion rule attempt with null attribute(s)");
                    }
                    break;

                case PolicyCustomRules.RuleLevel.FilePublisher:
                    if (this.PolicyCustomRule.FileInfo["PCACertificate"] == Properties.Resources.DefaultFileAttributeString ||
                        this.PolicyCustomRule.FileInfo["LeafCertificate"] == Properties.Resources.DefaultFileAttributeString ||
                        this.PolicyCustomRule.FileInfo["FileVersion"] == Properties.Resources.DefaultVersionString || 
                        this.PolicyCustomRule.FileInfo["FileName"] == Properties.Resources.DefaultFileAttributeString)
                    {
                        warnUser = true;
                        this.Log.AddWarningMsg("RuleLevel.FilePublisher rule attempt with null attribute(s)");
                    }
                    break;
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

            // Set Action value to Allow or Deny
            action = this.PolicyCustomRule.GetRulePermission().ToString();

            // Set Level value to the RuleLevel value//or should this be type for simplicity? 
            level = this.PolicyCustomRule.GetRuleType().ToString();

            switch (this.PolicyCustomRule.GetRuleLevel())
            {
                case PolicyCustomRules.RuleLevel.PcaCertificate:

                    name += String.Format("{0}: {1} ", this.PolicyCustomRule.GetRuleLevel(), this.PolicyCustomRule.FileInfo["PCACertificate"]);
                    break;
                case PolicyCustomRules.RuleLevel.Publisher:
                    name += String.Format("{0}: {1}, {2} ", this.PolicyCustomRule.GetRuleLevel(), this.PolicyCustomRule.FileInfo["PCACertificate"],
                        this.PolicyCustomRule.FileInfo["LeafCertificate"]);
                    break;

                case PolicyCustomRules.RuleLevel.SignedVersion:
                    name += String.Format("{0}: {1}, {2}, {3} ", this.PolicyCustomRule.GetRuleLevel(), this.PolicyCustomRule.FileInfo["PCACertificate"],
                        this.PolicyCustomRule.FileInfo["LeafCertificate"], this.PolicyCustomRule.FileInfo["FileVersion"]);
                    break;

                case PolicyCustomRules.RuleLevel.FilePublisher:
                    name += String.Format("{0}: {1}, {2}, {3}, {4} ", this.PolicyCustomRule.GetRuleLevel(), this.PolicyCustomRule.FileInfo["PCACertificate"],
                        this.PolicyCustomRule.FileInfo["LeafCertificate"], this.PolicyCustomRule.FileInfo["FileVersion"], this.PolicyCustomRule.FileInfo["FileName"]);
                    break;

                case PolicyCustomRules.RuleLevel.OriginalFileName:
                    name = String.Format("{0}; {1}", this.PolicyCustomRule.GetRuleLevel(), this.PolicyCustomRule.FileInfo["OriginalFilename"]);
                    break;

                case PolicyCustomRules.RuleLevel.InternalName:
                    name = String.Format("{0}; {1}", this.PolicyCustomRule.GetRuleLevel(), this.PolicyCustomRule.FileInfo["InternalName"]);
                    break;

                case PolicyCustomRules.RuleLevel.FileDescription:
                    name = String.Format("{0}; {1}", this.PolicyCustomRule.GetRuleLevel(), this.PolicyCustomRule.FileInfo["FileDescription"]);
                    break;

                case PolicyCustomRules.RuleLevel.ProductName:
                    name = String.Format("{0}; {1}", this.PolicyCustomRule.GetRuleLevel(), this.PolicyCustomRule.FileInfo["ProductName"]);
                    break;

                default:
                    name = String.Format("{0}; {1}", this.PolicyCustomRule.GetRuleLevel(), this.PolicyCustomRule.ReferenceFile);
                    break;

            }

            this.Log.AddInfoMsg(String.Format("CUSTOM RULE Created: {0} - {1} - {2} ", action, level, name));
            string[] stringArr = new string[5] { action , level, name, files, exceptions};

            // Offboard this to signingRules_Condition
            this.SigningControl.AddRuleToTable(stringArr, this.PolicyCustomRule, warnUser); 

            // Reset UI view
            ClearCustomRulesPanel(true);
            this._MainWindow.CustomRuleinProgress = false;
            this.RuleInEdit = false;
            this.PolicyCustomRule = new PolicyCustomRules(); 
        }

        /// <summary>
        /// Sets the RuleLevel to publisher, filepath or hash for the CustomRules object. 
        /// Executes when user selects the Rule Type dropdown combobox. 
        /// </summary>
        private void RuleType_ComboboxChanged(object sender, EventArgs e)
        {
            // Check if the selected item is null (this occurs after reseting it - rule creation)
            if (comboBox_RuleType.SelectedIndex < 0)
                return;

            string selectedOpt = comboBox_RuleType.SelectedItem.ToString();
            ClearCustomRulesPanel(false);
            label_Info.Visible = true;
            label_Error.Visible = false; // Clear error label

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
                    break;

                case "File Attributes":
                    this.PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.FileAttributes);
                    this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.InternalName); // Match UI by default
                    label_Info.Text = "Creates a rule for a file based on one of its attributes. \r\n" +
                        "Select a file to use as reference for your rule.";
                    break;

                case "File Hash":
                    this.PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.Hash);
                    this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.Hash);
                    label_Info.Text = "Creates a rule for a file that is not signed. \r\n" +
                        "Select the file for which you wish to create a hash rule.";
                    break;
                default:
                    break;
            }

            this.Log.AddInfoMsg(String.Format("Custom File Rule Level Set to {0}", selectedOpt));
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
        }


        /// <summary>
        /// Launches the FileDialog and prompts user to select the reference file. 
        /// Based on rule type, sets the UI elements for Publisher, FilePath or Hash rules. 
        /// </summary>
        private void button_Browse_Click(object sender, EventArgs e)
        {
            // Browse button for reference file:
            if (comboBox_RuleType.SelectedItem == null)
            {
                label_Error.Visible = true;
                label_Error.Text = "Please select a rule type first.";
                this.Log.AddWarningMsg("Browse button selected before rule type selected. Set rule type first.");
                return;
            }

            if (this.PolicyCustomRule.GetRuleType() != PolicyCustomRules.RuleType.Folder)
            {
                string refPath = getFileLocation();
                if (refPath == String.Empty)
                    return;

                // Custom rule in progress
                this._MainWindow.CustomRuleinProgress = true;

                // Get generic file information to be shown to user
                PolicyCustomRule.FileInfo = new Dictionary<string, string>(); // Reset dict
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(refPath);
                PolicyCustomRule.ReferenceFile = fileInfo.FileName; // Returns the file path
                PolicyCustomRule.FileInfo.Add("CompanyName", String.IsNullOrEmpty(fileInfo.CompanyName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.CompanyName);
                PolicyCustomRule.FileInfo.Add("ProductName", String.IsNullOrEmpty(fileInfo.ProductName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.ProductName);
                PolicyCustomRule.FileInfo.Add("OriginalFilename", String.IsNullOrEmpty(fileInfo.OriginalFilename) ? Properties.Resources.DefaultFileAttributeString : fileInfo.OriginalFilename);
                PolicyCustomRule.FileInfo.Add("FileVersion", String.IsNullOrEmpty(fileInfo.FileVersion) ? "0.0.0.0" : fileInfo.FileVersion);
                PolicyCustomRule.FileInfo.Add("FileName", String.IsNullOrEmpty(fileInfo.OriginalFilename) ? Properties.Resources.DefaultFileAttributeString : fileInfo.OriginalFilename); // WDAC configCI uses original filename 
                PolicyCustomRule.FileInfo.Add("FileDescription", String.IsNullOrEmpty(fileInfo.FileDescription) ? Properties.Resources.DefaultFileAttributeString : fileInfo.FileDescription);
                PolicyCustomRule.FileInfo.Add("InternalName", String.IsNullOrEmpty(fileInfo.InternalName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.InternalName);

                // Get cert chain info to be shown to the user
                string leafCertSubjectName = "";
                string pcaCertSubjectName = "";
                try
                {
                    var signer = X509Certificate.CreateFromSignedFile(refPath);
                    var cert = new X509Certificate2(signer);
                    var certChain = new X509Chain();
                    var certChainIsValid = certChain.Build(cert);

                    leafCertSubjectName = cert.SubjectName.Name;
                    if (certChain.ChainElements.Count > 1)
                        pcaCertSubjectName = certChain.ChainElements[1].Certificate.SubjectName.Name;

                }
                catch (Exception exp)
                {
                    this._MainWindow.Log.AddErrorMsg(String.Format("Caught exception {0} when trying to create cert from the following signed file {1}",
                        exp, refPath));
                    this.label_Error.Text = "Unable to find certificate chain for " + fileInfo.FileName;
                    this.label_Error.Visible = true;

                    Timer settingsUpdateNotificationTimer = new Timer();
                    settingsUpdateNotificationTimer.Interval = (5000); // 1.5 secs
                    settingsUpdateNotificationTimer.Tick += new EventHandler(SettingUpdateTimer_Tick);
                    settingsUpdateNotificationTimer.Start();
                }

                PolicyCustomRule.FileInfo.Add("LeafCertificate", String.IsNullOrEmpty(leafCertSubjectName) ? Properties.Resources.DefaultFileAttributeString : leafCertSubjectName);
                PolicyCustomRule.FileInfo.Add("PCACertificate", String.IsNullOrEmpty(pcaCertSubjectName) ? Properties.Resources.DefaultFileAttributeString : pcaCertSubjectName);

            }

            // Set the landing UI depending on the Rule type
            switch (this.PolicyCustomRule.GetRuleType())
            {
                case PolicyCustomRules.RuleType.Publisher:


                    // UI
                    textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    labelSlider_0.Text = "Issuing CA:";
                    labelSlider_1.Text = "Publisher:";
                    labelSlider_2.Text = "File version:";
                    labelSlider_3.Text = "File name:";
                    textBoxSlider_0.Text = PolicyCustomRule.FileInfo["PCACertificate"];
                    textBoxSlider_1.Text = PolicyCustomRule.FileInfo["LeafCertificate"];
                    textBoxSlider_2.Text = PolicyCustomRule.FileInfo["FileVersion"];
                    textBoxSlider_3.Text = PolicyCustomRule.FileInfo["FileName"];

                    panel_Publisher_Scroll.Visible = true;
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Text = Properties.Resources.FilePublisherInfo; 
                    break;

                case PolicyCustomRules.RuleType.Folder:

                    // User wants to create rule by folder level
                    PolicyCustomRule.ReferenceFile = getFolderLocation();
                    this.AllFilesinFolder = new List<string>();
                    if (PolicyCustomRule.ReferenceFile == String.Empty)
                    {
                        break; 
                    }

                    // Custom rule in progress
                    this._MainWindow.CustomRuleinProgress = true;

                    textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    //ProcessAllFiles(PolicyCustomRule.ReferenceFile);
                    //PolicyCustomRule.FolderContents = this.AllFilesinFolder; 
                    this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.Folder);
                    break;


                case PolicyCustomRules.RuleType.FilePath:

                    // FILE LEVEL

                    // UI updates
                    radioButton_File.Checked = true;
                    textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    panel_Publisher_Scroll.Visible = false;
                    break;

                case PolicyCustomRules.RuleType.FileAttributes:
                    // Creates a rule -Level FileName -SpecificFileNameLevel InternalName, FileDescription

                    // UI 
                    textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    labelSlider_0.Text = "Original filename:";
                    labelSlider_1.Text = "File description:";
                    labelSlider_2.Text = "Product name:";
                    labelSlider_3.Text = "Internal name:";
                    textBoxSlider_0.Text = PolicyCustomRule.FileInfo["OriginalFilename"];
                    textBoxSlider_1.Text = PolicyCustomRule.FileInfo["FileDescription"];
                    textBoxSlider_2.Text = PolicyCustomRule.FileInfo["ProductName"];
                    textBoxSlider_3.Text = PolicyCustomRule.FileInfo["InternalName"];

                    panel_Publisher_Scroll.Visible = true;
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Text = "Rule applies to all files with this file description attribute.";

                    break;

                case PolicyCustomRules.RuleType.Hash:

                    // UI updates
                    panel_Publisher_Scroll.Visible = false;
                    textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
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
                this.PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.FilePath);

            else
                this.PolicyCustomRule.SetRuleType(PolicyCustomRules.RuleType.Folder);

            // Check if user changed Rule Level after already browsing and selecting a reference file
            if (this.PolicyCustomRule.ReferenceFile != null)
                button_Browse_Click(sender, e);

        }

        private void trackBar_Conditions_Scroll(object sender, EventArgs e)
        {
            int pos = trackBar_Conditions.Value; //Publisher file rules conditions
            label_Error.Visible = false; // Clear error label

            switch (this.PolicyCustomRule.GetRuleType())
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
        }


        /// <summary>
        /// Opens the file dialog and grabs the file path for PEs only and checks if path exists. 
        /// </summary>
        /// <returns>Returns the full path+name of the file</returns>
        private string getFileLocation()
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
                return String.Empty;

        }

        /// <summary>
        /// Opens the folder dialog and grabs the folder path. Requires Folder to be toggled when Browse button 
        /// is selected. 
        /// </summary>
        /// <returns>Returns the full path of the folder</returns>
        private string getFolderLocation()
        {
            FolderBrowserDialog openFolderDialog = new FolderBrowserDialog();
            openFolderDialog.Description = "Browse for a folder to use as a reference for the rule.";

            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                openFolderDialog.Dispose();
                return openFolderDialog.SelectedPath;
            }
            else
                return String.Empty;

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
                    e.Cancel = false; 
                }
                else
                {
                    e.Cancel = true; 
                }
            }            
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            e.Cancel = true;
            base.OnFormClosing(e);
        }
    }   
}
