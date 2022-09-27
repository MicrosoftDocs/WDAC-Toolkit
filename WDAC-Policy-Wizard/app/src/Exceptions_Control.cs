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

        private string[] DefaultValues;

        public Exceptions_Control(CustomRuleConditionsPanel pRuleConditionsPanel)
        {
            InitializeComponent();
            this.ExceptionRule = new PolicyCustomRules();
            this.Log = pRuleConditionsPanel.Log;
            this.CustomRule = pRuleConditionsPanel.PolicyCustomRule;
            this.ConditionsPanel = pRuleConditionsPanel;

            this.DefaultValues = new string[5];
        }


        /// <summary>
        /// Clears the remaining UI elements of the Custom Rules Panel when a user selects the 'Create Rule' button. 
        /// </summary>
        /// <param name="clearComboBox">Bool to reset the Rule Type combobox.</param>
        private void ClearCustomRulesPanel(bool clearComboBox = false)
        {
            // Clear all of UI updates we make based on the type of rule so that the Custom Rules Panel is clear
            //Publisher:
            this.panel_Publisher_Scroll.Visible = false;
            this.publisherInfoLabel.Visible = false;

            //File Path:
            this.panel_FileFolder.Visible = false;

            //Other
            this.textBox_ReferenceFile.Clear();
            ClearLabel_ErrorText(); 

            // Reset the rule type combobox
            if (clearComboBox)
            {
                this.comboBox_ExceptionType.SelectedItem = null;
                this.comboBox_ExceptionType.Text = "--Select--";
            }
        }

        private void ComboBox_ExceptionType_SelectedIndexChanged(object sender, EventArgs e)
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
            this.Log.AddInfoMsg(String.Format("Exception File Rule Level Set to {0}", selectedOpt));
        }


        private void Button_Browse_Click(object sender, EventArgs e)
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
                string refPath = GetFileLocation();

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
                case PolicyCustomRules.RuleType.Folder:

                    // User wants to create rule by folder level
                    ExceptionRule.ReferenceFile = GetFolderLocation();
                    if (ExceptionRule.ReferenceFile == String.Empty)
                    {
                        break;
                    }

                    this.textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;

                    // Show right side of the text
                    if(this.textBox_ReferenceFile.TextLength > 0)
                    {
                        this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                        this.textBox_ReferenceFile.ScrollToCaret();
                    }
                    
                    this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.Folder);
                    break;


                case PolicyCustomRules.RuleType.FilePath:

                    // FILE LEVEL

                    // UI updates
                    radioButton_File.Checked = true;
                    this.textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;

                    // Show right side of the text
                    if(this.textBox_ReferenceFile.TextLength > 0)
                    {
                        this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                        this.textBox_ReferenceFile.ScrollToCaret();
                    }
                   
                    panel_Publisher_Scroll.Visible = false;
                    break;

                case PolicyCustomRules.RuleType.FileAttributes:
                    // Creates a rule -Level FileName -SpecificFileNameLevel InternalName, FileDescription

                    // UI 
                    this.textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;

                    // Show right side of the text
                    if (this.textBox_ReferenceFile.TextLength > 0)
                    {
                        this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                        this.textBox_ReferenceFile.ScrollToCaret();
                    }

                    this.DefaultValues[0] = ExceptionRule.FileInfo["OriginalFilename"];
                    this.DefaultValues[1] = ExceptionRule.FileInfo["FileDescription"];
                    this.DefaultValues[2] = ExceptionRule.FileInfo["ProductName"];
                    this.DefaultValues[3] = ExceptionRule.FileInfo["InternalName"];
                    this.DefaultValues[4] = ExceptionRule.FileInfo["FileVersion"];

                    this.textBox_originalfilename.Text = this.DefaultValues[0]; 
                    this.textBox_filedescription.Text = this.DefaultValues[1];
                    this.textBox_product.Text = this.DefaultValues[2];
                    this.textBox_internalname.Text = this.DefaultValues[3];
                    this.textBox_minversion.Text = this.DefaultValues[4];

                    panel_Publisher_Scroll.Visible = true;
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Text = "Rule applies to all files with these file description attributes.";

                    break;

                case PolicyCustomRules.RuleType.Hash:

                    // UI updates
                    panel_Publisher_Scroll.Visible = false;
                    this.textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;
                    // Show right side of the text
                    if (this.textBox_ReferenceFile.TextLength > 0)
                    {
                        this.textBox_ReferenceFile.SelectionStart = this.textBox_ReferenceFile.TextLength - 1;
                        this.textBox_ReferenceFile.ScrollToCaret();
                    }

                    break;
            }
        }

        private void Button_CreateException_Click(object sender, EventArgs e)
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

        /// <summary>
        /// Opens the file dialog and grabs the file path for PEs only and checks if path exists. 
        /// </summary>
        /// <returns>Returns the full path+name of the file</returns>
        private string GetFileLocation()
        {
            // Open file dialog to get file or folder path
            return Helper.BrowseForSingleFile(Properties.Resources.OpenPEFileDialogTitle, Helper.BrowseFileType.PEFile);
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
            if(this.ExceptionRule == null || this.ExceptionRule.ReferenceFile == null 
                || this.ExceptionRule.Level == PolicyCustomRules.RuleLevel.None)
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

        /// <summary>
        /// Called when DataGridView needs to paint data
        /// </summary>
        private void DataGridView_Exceptions_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
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

        private void checkBox_OriginalFilename_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_OriginalFilename.Checked)
            {
                if (this.textBox_originalfilename.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(this.textBox_originalfilename.Text))
                {
                    ClearLabel_ErrorText();
                    this.ExceptionRule.CheckboxCheckStates.checkBox0 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            this.checkBox_OriginalFilename.Checked = false;
            this.ExceptionRule.CheckboxCheckStates.checkBox0 = false;
        }

        private void checkBox_FileDescription_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_FileDescription.Checked)
            {
                if (this.textBox_filedescription.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(this.textBox_filedescription.Text))
                {
                    ClearLabel_ErrorText();
                    this.ExceptionRule.CheckboxCheckStates.checkBox1 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            this.checkBox_FileDescription.Checked = false;
            this.ExceptionRule.CheckboxCheckStates.checkBox1 = false;
        }

        private void checkBox_Product_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_Product.Checked)
            {
                if (this.textBox_product.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(this.textBox_product.Text))
                {
                    ClearLabel_ErrorText();
                    this.ExceptionRule.CheckboxCheckStates.checkBox2 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            this.checkBox_Product.Checked = false;
            this.ExceptionRule.CheckboxCheckStates.checkBox3 = false;
        }

        private void checkBox_InternalName_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_InternalName.Checked)
            {
                if (this.textBox_internalname.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(this.textBox_internalname.Text))
                {
                    ClearLabel_ErrorText();
                    this.ExceptionRule.CheckboxCheckStates.checkBox3 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            this.checkBox_InternalName.Checked = false;
            this.ExceptionRule.CheckboxCheckStates.checkBox3 = false;
        }

        private void checkBox_MinVersion_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_MinVersion.Checked)
            {
                if (this.textBox_minversion.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(this.textBox_minversion.Text))
                {
                    ClearLabel_ErrorText();
                    this.ExceptionRule.CheckboxCheckStates.checkBox4 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            this.checkBox_MinVersion.Checked = false;
            this.ExceptionRule.CheckboxCheckStates.checkBox4 = false;
        }

        /// <summary>
        /// Clear the error label text and set to invisible
        /// </summary>
        private void ClearLabel_ErrorText()
        {
            this.publisherInfoLabel.Text = "";
            this.publisherInfoLabel.Visible = false;
        }

        private void SetLabel_ErrorText(string errorText)
        {
            this.publisherInfoLabel.Focus();
            this.publisherInfoLabel.BringToFront();
            this.publisherInfoLabel.Text = errorText;
            this.publisherInfoLabel.Visible = true;
        }

        private void checkBoxCustomValues_CheckedChanged(object sender, EventArgs e)
        {
            // Set the UI first
            if (this.checkBoxCustomValues.Checked)
            {
                SetTextBoxStates(true);

                // Set the custom values based on existing
                this.ExceptionRule.CustomValues.FileName = textBox_originalfilename.Text;
                this.ExceptionRule.CustomValues.Description = textBox_filedescription.Text;
                this.ExceptionRule.CustomValues.ProductName = textBox_product.Text;
                this.ExceptionRule.CustomValues.InternalName = textBox_internalname.Text;
                this.ExceptionRule.CustomValues.MinVersion = textBox_minversion.Text; 

                this.ExceptionRule.UsingCustomValues = true;
            }
            else
            {
                // Clear error if applicable
                this.ClearLabel_ErrorText();

                // Set text values back to default
                SetTextBoxStates(false);
                this.textBox_originalfilename.Text = this.DefaultValues[0];
                this.textBox_filedescription.Text = this.DefaultValues[1];
                this.textBox_product.Text = this.DefaultValues[2];
                this.textBox_internalname.Text = this.DefaultValues[3];
                this.textBox_minversion.Text = this.DefaultValues[4];

                this.ExceptionRule.UsingCustomValues = false;
            }
        }

        private void SetTextBoxStates(bool enabled)
        {
            if (enabled)
            {
                // If enabled, allow user input
                this.textBox_originalfilename.ReadOnly = false; 
                this.textBox_filedescription.ReadOnly = false;
                this.textBox_product.ReadOnly = false;
                this.textBox_internalname.ReadOnly = false;
                this.textBox_minversion.ReadOnly = false;

                this.textBox_originalfilename.Enabled = true;
                this.textBox_filedescription.Enabled = true;
                this.textBox_product.Enabled = true;
                this.textBox_internalname.Enabled = true;
                this.textBox_minversion.Enabled = true;

                // Set back color to white to help user determine boxes are userwriteable
                this.textBox_originalfilename.BackColor = Color.White;
                this.textBox_filedescription.BackColor = Color.White;
                this.textBox_product.BackColor = Color.White;
                this.textBox_internalname.BackColor = Color.White;
                this.textBox_minversion.BackColor = Color.White;
            }
            else
            {
                // Set to read only if disabled
                this.textBox_originalfilename.ReadOnly = true;
                this.textBox_filedescription.ReadOnly = true;
                this.textBox_product.ReadOnly = true;
                this.textBox_internalname.ReadOnly = true;
                this.textBox_minversion.ReadOnly = true;

                this.textBox_originalfilename.Enabled = false;
                this.textBox_filedescription.Enabled = false;
                this.textBox_product.Enabled = false;
                this.textBox_internalname.Enabled = false;
                this.textBox_minversion.Enabled = false;

                // Set back color to white to help user determine boxes are userwriteable
                this.textBox_originalfilename.BackColor = SystemColors.Control;
                this.textBox_filedescription.BackColor = SystemColors.Control;
                this.textBox_product.BackColor = SystemColors.Control;
                this.textBox_internalname.BackColor = SystemColors.Control;
                this.textBox_minversion.BackColor = SystemColors.Control;
            }
        }

        /// <summary>
        /// Triggered by the user selecting either File or Folder. Sets the rule level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileFolderButtonClick(object sender, EventArgs e)
        {
            if(this.radioButton_File.Checked)
            {
                this.ExceptionRule.Level = PolicyCustomRules.RuleLevel.FilePath;
                this.ExceptionRule.Type = PolicyCustomRules.RuleType.FilePath;
            }
            else
            {
                this.ExceptionRule.Level = PolicyCustomRules.RuleLevel.Folder;
                this.ExceptionRule.Type = PolicyCustomRules.RuleType.Folder;
            }
        }
    }
}
