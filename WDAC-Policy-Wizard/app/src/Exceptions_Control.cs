using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using System.IO; 


namespace WDAC_Wizard
{
    public partial class Exceptions_Control : UserControl
    {
        private Logger Log;
        private PolicyCustomRules ExceptionRule;     // One instance of a custom rule. Appended to Policy.CustomRules
        private PolicyCustomRules CustomRule;
        private CustomRuleConditionsPanel ConditionsPanel;

        // Declare an ArrayList to serve as the data store. 
        private System.Collections.ArrayList displayObjects =
            new System.Collections.ArrayList();
        private int RowSelected; // Data grid row number selected by the user 
        private int rowInEdit = -1;
        private DisplayObject displayObjectInEdit;

        public Exceptions_Control(CustomRuleConditionsPanel pRuleConditionsPanel)
        {
            InitializeComponent();
            this.ExceptionRule = new PolicyCustomRules();
            this.Log = pRuleConditionsPanel.Log;
            this.CustomRule = pRuleConditionsPanel.PolicyCustomRule;
            this.ConditionsPanel = pRuleConditionsPanel; 
        }


        /// <summary>
        /// Clears the remaining UI elements of the Custom Rules Panel when a user selects the 'Create Rule' button. 
        /// /// </summary>
        /// /// <param name="clearComboBox">Bool to reset the Rule Type combobox.</param>
        private void ClearCustomRulesPanel(bool clearComboBox = false)
        {
            // Clear all of UI updates we make based on the type of rule so that the Custom Rules Panel is clear
            //Publisher:
            this.panel_Publisher_Scroll.Visible = false;
            this.publisherInfoLabel.Visible = false;
            this.trackBar_Conditions.ResetText();
            this.trackBar_Conditions.Value = 0; // default bottom position 

            //File Path:
            this.panel_FileFolder.Visible = false;

            //Other
            this.textBox_ReferenceFile.Clear();

            // Reset the rule type combobox
            if (clearComboBox)
            {
                this.comboBox_ExceptionType.SelectedItem = null;
                this.comboBox_ExceptionType.Text = "--Select--";
            }
        }

        private void comboBox_ExceptionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if the selected item is null (this occurs after reseting it - rule creation)
            if (this.comboBox_ExceptionType.SelectedIndex < 0)
                return;

            string selectedOpt = this.comboBox_ExceptionType.SelectedItem.ToString();
            ClearCustomRulesPanel(false);

            switch (selectedOpt)
            {
                case "Publisher":
                    // TODO: figure out why publisher exceptions are not compatible with publisher rules
                    this.ExceptionRule.SetRuleType(PolicyCustomRules.RuleType.Publisher);
                    this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FilePublisher); // Match UI by default
                    break;

                case "File Path":
                    this.ExceptionRule.SetRuleType(PolicyCustomRules.RuleType.FilePath);
                    this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FilePath);
                    panel_FileFolder.Visible = true;
                    radioButton_File.Checked = true; // By default, 
                    break;

                case "File Attributes":
                    this.ExceptionRule.SetRuleType(PolicyCustomRules.RuleType.FileAttributes);
                    this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.InternalName); // Match UI by default
                    break;

                case "File Hash":
                    this.ExceptionRule.SetRuleType(PolicyCustomRules.RuleType.Hash);
                    this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.Hash);
                    break;

                default:
                    break;
            }
            this.Log.AddInfoMsg(String.Format("Custom File Rule Level Set to {0}", selectedOpt));
        }


        private void button_Browse_Click(object sender, EventArgs e)
        {
            // Browse button for reference file:
            if (comboBox_ExceptionType.SelectedItem == null)
            {
                // Set CustomRule conditios panel need method
                this.ConditionsPanel.SetLabel_ErrorText("Please set exception rule type first");
                this.Log.AddWarningMsg("Browse button selected before rule type selected. Set rule type first.");
                return;
            }

            if (this.ExceptionRule.GetRuleType() != PolicyCustomRules.RuleType.Folder)
            {
                string refPath = getFileLocation();

                if (refPath == String.Empty)
                    return;

                // Custom rule in progress
               // this._MainWindow.CustomRuleinProgress = true;

                // Get generic file information to be shown to user
                ExceptionRule.FileInfo = new Dictionary<string, string>(); // Reset dict
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(refPath);
                ExceptionRule.ReferenceFile = fileInfo.FileName; // Returns the file path
                ExceptionRule.FileInfo.Add("CompanyName", String.IsNullOrEmpty(fileInfo.CompanyName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.CompanyName);
                ExceptionRule.FileInfo.Add("ProductName", String.IsNullOrEmpty(fileInfo.ProductName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.ProductName);
                ExceptionRule.FileInfo.Add("OriginalFilename", String.IsNullOrEmpty(fileInfo.OriginalFilename) ? Properties.Resources.DefaultFileAttributeString : fileInfo.OriginalFilename);
                ExceptionRule.FileInfo.Add("FileVersion", String.IsNullOrEmpty(fileInfo.FileVersion) ? "0.0.0.0" : fileInfo.FileVersion);
                ExceptionRule.FileInfo.Add("FileName", String.IsNullOrEmpty(fileInfo.OriginalFilename) ? Properties.Resources.DefaultFileAttributeString : fileInfo.OriginalFilename); // WDAC configCI uses original filename 
                ExceptionRule.FileInfo.Add("FileDescription", String.IsNullOrEmpty(fileInfo.FileDescription) ? Properties.Resources.DefaultFileAttributeString : fileInfo.FileDescription);
                ExceptionRule.FileInfo.Add("InternalName", String.IsNullOrEmpty(fileInfo.InternalName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.InternalName);

                // Get cert chain info to be shown to the user
                string leafCertSubjectName = "";
                string pcaCertSubjectName = "";

                if(this.ExceptionRule.Type == PolicyCustomRules.RuleType.Publisher)
                {
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
                        // Set labelError text in CustomRuleConditionsPanel
                        this.ConditionsPanel.SetLabel_ErrorText(Properties.Resources.CertificateBuild_Error + fileInfo.FileName);
                    }
                }
                
                ExceptionRule.FileInfo.Add("LeafCertificate", String.IsNullOrEmpty(leafCertSubjectName) ? Properties.Resources.DefaultFileAttributeString : leafCertSubjectName);
                ExceptionRule.FileInfo.Add("PCACertificate", String.IsNullOrEmpty(pcaCertSubjectName) ? Properties.Resources.DefaultFileAttributeString : pcaCertSubjectName);
            }

            // Set the landing UI depending on the Rule type
            switch (this.ExceptionRule.GetRuleType())
            {
                case PolicyCustomRules.RuleType.Publisher:

                    // UI
                    this.textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;
                    // Show right side of the text
                    this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                    this.textBox_ReferenceFile.ScrollToCaret();
                    break;

                    labelSlider_0.Text = "Issuing CA:";
                    labelSlider_1.Text = "Publisher:";
                    labelSlider_2.Text = "File version:";
                    labelSlider_3.Text = "File name:";
                    textBoxSlider_0.Text = ExceptionRule.FileInfo["PCACertificate"];
                    textBoxSlider_1.Text = ExceptionRule.FileInfo["LeafCertificate"];
                    textBoxSlider_2.Text = ExceptionRule.FileInfo["FileVersion"];
                    textBoxSlider_3.Text = ExceptionRule.FileInfo["FileName"];

                    textBoxSlider_0.BackColor = Color.FromArgb(240, 240, 240);

                    panel_Publisher_Scroll.Visible = true;
                    break;

                case PolicyCustomRules.RuleType.Folder:

                    // User wants to create rule by folder level
                    ExceptionRule.ReferenceFile = getFolderLocation();
                    if (ExceptionRule.ReferenceFile == String.Empty)
                    {
                        break;
                    }

                    // Custom rule in progress
                    //this._MainWindow.CustomRuleinProgress = true;

                    this.textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;
                    // Show right side of the text
                    this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                    this.textBox_ReferenceFile.ScrollToCaret();
                    //ProcessAllFiles(ExceptionRule.ReferenceFile);
                    //ExceptionRule.FolderContents = this.AllFilesinFolder; 
                    this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.Folder);
                    break;


                case PolicyCustomRules.RuleType.FilePath:

                    // FILE LEVEL

                    // UI updates
                    radioButton_File.Checked = true;
                    this.textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;
                    // Show right side of the text
                    this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                    this.textBox_ReferenceFile.ScrollToCaret();

                    panel_Publisher_Scroll.Visible = false;
                    break;

                case PolicyCustomRules.RuleType.FileAttributes:
                    // Creates a rule -Level FileName -SpecificFileNameLevel InternalName, FileDescription

                    // UI 
                    this.textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;
                    // Show right side of the text
                    this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                    this.textBox_ReferenceFile.ScrollToCaret();
                    labelSlider_0.Text = "Original filename:";
                    labelSlider_1.Text = "File description:";
                    labelSlider_2.Text = "Product name:";
                    labelSlider_3.Text = "Internal name:";
                    textBoxSlider_0.Text = ExceptionRule.FileInfo["OriginalFilename"];
                    textBoxSlider_1.Text = ExceptionRule.FileInfo["FileDescription"];
                    textBoxSlider_2.Text = ExceptionRule.FileInfo["ProductName"];
                    textBoxSlider_3.Text = ExceptionRule.FileInfo["InternalName"];

                    panel_Publisher_Scroll.Visible = true;
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Text = "Rule applies to all files with this file description attribute.";

                    break;

                case PolicyCustomRules.RuleType.Hash:

                    // UI updates
                    panel_Publisher_Scroll.Visible = false;
                    this.textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;
                    // Show right side of the text
                    this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                    this.textBox_ReferenceFile.ScrollToCaret();
                    
                    break;
            }
        }

        private void button_CreateException_Click(object sender, EventArgs e)
        {
            // Add the exception to the table
            this.CustomRule.AddException(this.ExceptionRule);
            this.ExceptionRule = null; 
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
            {
                return String.Empty;
            }
        }

        private void Exceptions_Control_Load(object sender, EventArgs e)
        {
            // On load, set the rule condition label
            if(this.CustomRule != null)
            {
                string ruleConditionString = this.CustomRule.Permission.ToString() + " "; // e.g. "Allow " or "Deny " ...

                switch (this.CustomRule.Level)
                {
                    case PolicyCustomRules.RuleLevel.PcaCertificate:
                        ruleConditionString += "files signed by CA: ";
                        ruleConditionString += this.CustomRule.FileInfo["PCACertificate"];
                        break;

                    case PolicyCustomRules.RuleLevel.Publisher:
                        ruleConditionString += "files signed by: ";
                        ruleConditionString += this.CustomRule.FileInfo["LeafCertificate"];
                        break;

                    case PolicyCustomRules.RuleLevel.SignedVersion:
                        ruleConditionString += "files signed by: ";
                        ruleConditionString += this.CustomRule.FileInfo["LeafCertificate"]; 
                        ruleConditionString += " with version " + this.CustomRule.FileInfo["FileVersion"] + " or greater";
                        break;

                    case PolicyCustomRules.RuleLevel.FilePublisher:
                        ruleConditionString += "files signed by: ";
                        ruleConditionString += this.CustomRule.FileInfo["LeafCertificate"]; 
                        ruleConditionString += " with filename " + this.CustomRule.FileInfo["FileName"];
                        break;

                    case PolicyCustomRules.RuleLevel.FileName:
                        ruleConditionString += "files with file name: ";
                        ruleConditionString += this.CustomRule.FileInfo["FileName"]; 
                        break;

                    case PolicyCustomRules.RuleLevel.FileDescription:
                        ruleConditionString += "files with file description: ";
                        ruleConditionString += this.CustomRule.FileInfo["FileDescription"];
                        break;

                    case PolicyCustomRules.RuleLevel.InternalName:
                        ruleConditionString += "files with internal name: "; 
                        ruleConditionString += this.CustomRule.FileInfo["InternalName"];
                        break;

                    case PolicyCustomRules.RuleLevel.OriginalFileName:
                        ruleConditionString += "files with original file name: ";
                        ruleConditionString += this.CustomRule.FileInfo["OriginalFilename"];
                        break;

                    case PolicyCustomRules.RuleLevel.ProductName:
                        ruleConditionString += "files with product name: "; 
                        ruleConditionString += this.CustomRule.FileInfo["ProductName"];
                        break;

                    case PolicyCustomRules.RuleLevel.FilePath:
                        ruleConditionString += "files with path: ";
                        ruleConditionString += this.CustomRule.ReferenceFile; //Full file path
                        break;

                    case PolicyCustomRules.RuleLevel.Folder:
                        ruleConditionString += "files under folder path: ";
                        ruleConditionString += this.CustomRule.ReferenceFile; //Full folder path
                        break;

                    case PolicyCustomRules.RuleLevel.Hash:
                        ruleConditionString += "hash defined by: "; 
                        ruleConditionString += this.CustomRule.FileInfo["FileName"];
                        break;
                }

                this.ruleCondition_Label.Text = FormatText(ruleConditionString); // "Rule Condition:\n\r" + this.CustomRule.Level.ToString() + " " + this.CustomRule.FileInfo["FileName"];
                this.ruleCondition_Label.Visible = true; 
            }
        }

        public string FormatText(string ruleConditionString)
        {
            int MAX_LEN = 100;

            // Need to check that in between /r/n's that length !> MAX_LEN
            if(ruleConditionString.Length > MAX_LEN )
            {
                if(this.CustomRule.Type == PolicyCustomRules.RuleType.Publisher)
                {
                    // Publisher formatting split between O= and L=
                    int split_idx = ruleConditionString.IndexOf("L=");
                    int split_idx_backup = ruleConditionString.IndexOf("O=");
                    if (split_idx > 1 && split_idx < MAX_LEN) // found
                    {
                        ruleConditionString = ruleConditionString.Substring(0, split_idx) + Environment.NewLine + ruleConditionString.Substring(split_idx);
                    }
                    else if(split_idx_backup > 1 && split_idx_backup < MAX_LEN)
                    {
                        ruleConditionString = ruleConditionString.Substring(0, split_idx_backup) + Environment.NewLine + ruleConditionString.Substring(split_idx_backup);
                    }
                    else
                    {
                        int mid_pt = ruleConditionString.Length / 2;
                        ruleConditionString = ruleConditionString.Substring(0, mid_pt) + Environment.NewLine + ruleConditionString.Substring(mid_pt);
                    }
                }
                else if (this.CustomRule.Type == PolicyCustomRules.RuleType.FileAttributes)
                {
                    // Last space before max length
                    int split_idx = ruleConditionString.Substring(0, MAX_LEN).LastIndexOf(" ");
                    if (split_idx > 1)
                    {
                        ruleConditionString = ruleConditionString.Substring(0, split_idx) + Environment.NewLine + ruleConditionString.Substring(split_idx);
                    }
                    else
                    {
                        ruleConditionString = ruleConditionString.Substring(0, MAX_LEN) + Environment.NewLine + ruleConditionString.Substring(MAX_LEN);
                    }
                }
                else if(this.CustomRule.Type == PolicyCustomRules.RuleType.Hash)
                {
                    ruleConditionString = ruleConditionString.Substring(0, MAX_LEN) + Environment.NewLine + ruleConditionString.Substring(MAX_LEN);
                }
                else //folder or file path
                {
                    // Last back slash before max length
                    int split_idx = ruleConditionString.Substring(0, MAX_LEN).LastIndexOf("\\"); 
                    if(split_idx > 1)
                    {
                        ruleConditionString = ruleConditionString.Substring(0, split_idx) + Environment.NewLine + ruleConditionString.Substring(split_idx); 
                    }
                    else
                    {
                        ruleConditionString = ruleConditionString.Substring(0, MAX_LEN) + Environment.NewLine + ruleConditionString.Substring(MAX_LEN);
                    }
                }
            }
            return ruleConditionString; 
        }

        // Triggered by Add Exception button click on custom rule conditions panel. 
        // Add exception to the exception table
        public void AddException()
        {
            //Set ExceptionRule.Level to opposite of the PolicyCustomRule.Level
            if(this.CustomRule.Permission == PolicyCustomRules.RulePermission.Allow)
            {
                this.ExceptionRule.Permission = PolicyCustomRules.RulePermission.Deny; 
            }
            else
            {
                this.ExceptionRule.Permission = PolicyCustomRules.RulePermission.Allow; 
            }

            // Check that fields are valid, otherwise break and show error msg
            if(this.ExceptionRule == null || this.ExceptionRule.ReferenceFile == null ||
                this.ExceptionRule.Level == PolicyCustomRules.RuleLevel.None)
            {
                this.ConditionsPanel.SetLabel_ErrorText("Invalid exception selection. Please select a level and reference file");
                return; 
            }

            // Add the exception to the custom rule and table
            this.CustomRule.AddException(this.ExceptionRule);

            // New Display object
            DisplayObject displayObject = new DisplayObject();
            displayObject.Action = this.ExceptionRule.Permission.ToString();
            displayObject.Level = this.ExceptionRule.Level.ToString();
            displayObject.Name = Path.GetFileName(this.ExceptionRule.ReferenceFile.ToString());

            this.displayObjects.Add(displayObject);
            this.dataGridView_Exceptions.RowCount += 1;

            // Scroll to bottom to see new rule added to list
            this.dataGridView_Exceptions.FirstDisplayedScrollingRowIndex = this.dataGridView_Exceptions.RowCount - 1;

            this.ExceptionRule = new PolicyCustomRules();

            // Reset the UI
            ClearCustomRulesPanel(true); 
        }

        private void trackBar_Conditions_Scroll(object sender, EventArgs e)
        {
            int pos = trackBar_Conditions.Value; //Publisher file rules conditions

            switch (this.ExceptionRule.Type)
            {
                case PolicyCustomRules.RuleType.Publisher:
                    {
                        // Setting the trackBar values snaps the cursor to one of the four options
                        if (pos <= 2)
                        {
                            // PCACert + LeafCert + Version + FileName = FilePublisher
                            trackBar_Conditions.Value = 0;
                            this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FilePublisher);
                            textBoxSlider_3.Text = this.ExceptionRule.FileInfo["FileName"];
                            publisherInfoLabel.Text = Properties.Resources.FilePublisherInfo;
                            this.Log.AddInfoMsg("Publisher file rule level set to file publisher (0)");
                        }
                        else if (pos > 2 && pos <= 6)
                        {
                            // PCACert + LeafCert + Version = SignedVersion
                            trackBar_Conditions.Value = 4;
                            this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.SignedVersion);
                            textBoxSlider_2.Text = this.ExceptionRule.FileInfo["FileVersion"];
                            textBoxSlider_3.Text = "*";
                            publisherInfoLabel.Text = Properties.Resources.SignedVersionInfo;
                            this.Log.AddInfoMsg("Publisher file rule level set to file publisher (4)");
                        }
                        else if (pos > 6 && pos <= 10)
                        {
                            // PCACert + LeafCert  = Publisher
                            trackBar_Conditions.Value = 8;
                            this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.Publisher);
                            textBoxSlider_1.Text = this.ExceptionRule.FileInfo["LeafCertificate"];
                            textBoxSlider_2.Text = "*";
                            textBoxSlider_3.Text = "*";
                            publisherInfoLabel.Text = Properties.Resources.PublisherInfo;
                            this.Log.AddInfoMsg("Publisher file rule level set to publisher (8)");
                        }
                        else
                        {
                            // PCACert = PCACertificate
                            trackBar_Conditions.Value = 12;
                            this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.PcaCertificate);
                            textBoxSlider_0.Text = this.ExceptionRule.FileInfo["PCACertificate"];
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
                            this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.InternalName);
                            textBoxSlider_3.Text = this.ExceptionRule.FileInfo["InternalName"];

                            // If attribute is not applicable, set to RuleLevel = None to block from creating rule in button_Create_Click
                            if (textBoxSlider_3.Text == Properties.Resources.DefaultFileAttributeString)
                                this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.None);
                            publisherInfoLabel.Text = "Rule applies to all files with this internal name attribute.";
                            this.Log.AddInfoMsg(String.Format("Publisher file rule level set to file internal name: {0}", textBoxSlider_3.Text));
                        }
                        else if (pos > 2 && pos <= 6)
                        {
                            // Product name
                            trackBar_Conditions.Value = 4;
                            this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.ProductName);
                            textBoxSlider_2.Text = this.ExceptionRule.FileInfo["ProductName"];

                            // If attribute is not applicable, set to RuleLevel = None to block from creating rule in button_Create_Click
                            if (textBoxSlider_2.Text == Properties.Resources.DefaultFileAttributeString)
                                this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.None);
                            publisherInfoLabel.Text = "Rule applies to all files with this product name attribute.";
                            this.Log.AddInfoMsg(String.Format("Publisher file rule level set to product name: {0}", textBoxSlider_2.Text));
                        }
                        else if (pos > 6 && pos <= 10)
                        {
                            //File description
                            trackBar_Conditions.Value = 8;
                            this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FileDescription);
                            textBoxSlider_1.Text = this.ExceptionRule.FileInfo["FileDescription"];

                            // If attribute is not applicable, set to RuleLevel = None to block from creating rule in button_Create_Click
                            if (textBoxSlider_1.Text == Properties.Resources.DefaultFileAttributeString)
                                this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.None);
                            publisherInfoLabel.Text = "Rule applies to all files with this file description attribute.";
                            this.Log.AddInfoMsg(String.Format("Publisher file rule level set to file description: {0}", textBoxSlider_1.Text));
                        }
                        else
                        {
                            //Original filename
                            trackBar_Conditions.Value = 12;
                            this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.OriginalFileName);
                            textBoxSlider_0.Text = this.ExceptionRule.FileInfo["OriginalFilename"];

                            // If attribute is not applicable, set to RuleLevel = None to block from creating rule in button_Create_Click
                            if (textBoxSlider_0.Text == Properties.Resources.DefaultFileAttributeString)
                                this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.None);
                            publisherInfoLabel.Text = "Rule applies to all files with this original file name attribute.";
                            this.Log.AddInfoMsg(String.Format("Publisher file rule level set to original file name: {0}", textBoxSlider_0.Text));

                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Called when DataGridView needs to paint data
        /// </summary>
        private void dataGridView_Exceptions_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            // If this is the row for new records, no values are needed.
            if (e.RowIndex == this.dataGridView_Exceptions.RowCount - 1) return;

            DisplayObject displayObject = null;

            // Store a reference to the Customer object for the row being painted.
            if (e.RowIndex == rowInEdit)
            {
                displayObject = this.displayObjectInEdit;
            }
            else
            {
                displayObject = (DisplayObject)this.displayObjects[e.RowIndex];
            }

            // Set the cell value to paint using the Customer object retrieved.
            switch (this.dataGridView_Exceptions.Columns[e.ColumnIndex].Name)
            {
                case "column_Action":
                    e.Value = displayObject.Action;
                    break;

                case "column_Level":
                    e.Value = displayObject.Level;
                    break;

                case "column_Name":
                    e.Value = displayObject.Name;
                    break;

                case "Column_Files":
                    e.Value = displayObject.Files;
                    break;

                case "Column_Exceptions":
                    e.Value = displayObject.Exceptions;
                    break;
            }
            
        }
    }
}
