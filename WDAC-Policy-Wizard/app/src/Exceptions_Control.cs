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
        
        // Flag indicating a rule is in progress; the rule has not been added to the table
        private bool fRuleInProgress; 

        public Exceptions_Control(CustomRuleConditionsPanel pRuleConditionsPanel)
        {
            InitializeComponent();
            this.ExceptionRule = new PolicyCustomRules();
            this.Log = pRuleConditionsPanel.Log;
            this.CustomRule = pRuleConditionsPanel.PolicyCustomRule;
            this.ConditionsPanel = pRuleConditionsPanel;

            this.DefaultValues = new string[5];
            this.fRuleInProgress = false; 
        }


        /// <summary>
        /// Clears the remaining UI elements of the Custom Rules Panel when a user selects the 'Create Rule' button. 
        /// </summary>
        /// <param name="clearComboBox">Bool to reset the Rule Type combobox.</param>
        private void ClearCustomRulesPanel(bool clearComboBox = false)
        {
            // Clear all of UI updates we make based on the type of rule so that the Custom Rules Panel is clear
            //Publisher:
            panel_Publisher_Scroll.Visible = false;
            errorLabel.Visible = false;

            // File attribute:
            checkBox_OriginalFilename.Checked = false;
            checkBox_FileDescription.Checked = false;
            checkBox_Product.Checked = false;
            checkBox_InternalName.Checked = false;
            checkBox_MinVersion.Checked = false;
            checkBoxCustomValues.Checked = false;

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

            ClearLabel_ErrorText();

            // Set rule in progress flag
            this.fRuleInProgress = true; 

            string selectedOpt = this.comboBox_ExceptionType.SelectedItem.ToString();
            ClearCustomRulesPanel(false);

            switch (selectedOpt)
            {
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

        /// <summary>
        /// Fires on Browse Button click. Gets the location of the file or folder provided by the file dialog. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

            if (this.ExceptionRule.GetRuleType() != PolicyCustomRules.RuleType.FolderPath)
            {
                // Open file dialog to get file or folder path
                string refPath = Helper.BrowseForSingleFile(Properties.Resources.OpenPEFileDialogTitle, Helper.BrowseFileType.PEFile);

                if (refPath == String.Empty)
                    return;

                // Get generic file information to be shown to user
                ExceptionRule.FileInfo = new Dictionary<string, string>(); // Reset dict
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(refPath);
                ExceptionRule.ReferenceFile = fileInfo.FileName; // Returns the file path
                string fileVersion = Helper.ConcatFileVersion(fileInfo);
                ExceptionRule.FileInfo.Add("CompanyName", String.IsNullOrEmpty(fileInfo.CompanyName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.CompanyName.Trim());
                ExceptionRule.FileInfo.Add("ProductName", String.IsNullOrEmpty(fileInfo.ProductName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.ProductName.Trim());
                ExceptionRule.FileInfo.Add("OriginalFilename", String.IsNullOrEmpty(fileInfo.OriginalFilename) ? Properties.Resources.DefaultFileAttributeString : fileInfo.OriginalFilename.Trim());
                ExceptionRule.FileInfo.Add("FileVersion", String.IsNullOrEmpty(fileVersion) ? Properties.Resources.DefaultFileAttributeString : fileVersion);
                ExceptionRule.FileInfo.Add("FileName", String.IsNullOrEmpty(fileInfo.OriginalFilename) ? Properties.Resources.DefaultFileAttributeString : fileInfo.OriginalFilename.Trim());
                ExceptionRule.FileInfo.Add("FileDescription", String.IsNullOrEmpty(fileInfo.FileDescription) ? Properties.Resources.DefaultFileAttributeString : fileInfo.FileDescription.Trim());
                ExceptionRule.FileInfo.Add("InternalName", String.IsNullOrEmpty(fileInfo.InternalName) ? Properties.Resources.DefaultFileAttributeString : fileInfo.InternalName.Trim());
            }

            // Set the landing UI depending on the Rule type
            switch (this.ExceptionRule.GetRuleType())
            {
                case PolicyCustomRules.RuleType.FolderPath:

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
                    SetLabel_ErrorText("Rule applies to all files with these file description attributes.");
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
        /// Sets the UI upon loading the exceptions control panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Formats the Rule Condition text at the top of the page
        /// </summary>
        /// <param name="ruleConditionString"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the rule in progress flag back to the CustomRuleConditions panel in case user accidentally clicks the Create 
        /// Rule button instead of the Add Exception button. 
        /// </summary>
        /// <returns></returns>
        public bool IsRuleInProgress()
        {
            return this.fRuleInProgress;
        }

        /// <summary>
        /// Triggered by Add Exception button click on custom rule conditions panel. 
        /// Add exception to the exception table
        /// </summary>
        public void AddException()
        {
            // Set ExceptionRule.Level to opposite of the PolicyCustomRule.Level
            if(this.CustomRule.Permission == PolicyCustomRules.RulePermission.Allow)
            {
                this.ExceptionRule.Permission = PolicyCustomRules.RulePermission.Deny; 
            }
            else
            {
                this.ExceptionRule.Permission = PolicyCustomRules.RulePermission.Allow; 
            }

            // Assert that file path rules are not valid exceptions in the kernel
            if(this.CustomRule.SigningScenarioCheckStates.kmciEnabled && 
                (this.ExceptionRule.Type == PolicyCustomRules.RuleType.FilePath
                || this.ExceptionRule.Type == PolicyCustomRules.RuleType.FolderPath))
            {
                this.ConditionsPanel.SetLabel_ErrorText(Properties.Resources.InvalidKMCIRule);
                return;
            }

            // Check that fields are valid, otherwise break and show error msg
            if(this.ExceptionRule == null 
                || String.IsNullOrEmpty(this.ExceptionRule.ReferenceFile)
                || this.ExceptionRule.Type == PolicyCustomRules.RuleType.None)
            {
                this.ConditionsPanel.SetLabel_ErrorText("Invalid exception selection. Please select a level and reference file");
                return; 
            }

            // Get the values from the textboxes for the file attributes rule
            if(this.ExceptionRule.Type == PolicyCustomRules.RuleType.FileAttributes)
            {
                if(!this.ExceptionRule.IsAnyBoxChecked())
                {
                    this.ConditionsPanel.SetLabel_ErrorText(Properties.Resources.InvalidCheckboxState);
                    return;
                }

                this.ExceptionRule.CustomValues.FileName = textBox_originalfilename.Text;
                this.ExceptionRule.CustomValues.Description = textBox_filedescription.Text;
                this.ExceptionRule.CustomValues.ProductName = textBox_product.Text;
                this.ExceptionRule.CustomValues.InternalName = textBox_internalname.Text; 
                this.ExceptionRule.CustomValues.MinVersion= textBox_minversion.Text;
            }

            // Add the exception to the custom rule and table
            this.CustomRule.AddException(this.ExceptionRule);

            // New Display object
            DisplayObject displayObject = new DisplayObject();
            displayObject.Action = this.ExceptionRule.Permission.ToString();
            displayObject.Level = this.ExceptionRule.Type.ToString();
            displayObject.Name = Path.GetFileName(this.ExceptionRule.ReferenceFile.ToString());

            this.displayObjects.Add(displayObject);
            this.dataGridView_Exceptions.RowCount += 1;

            // Scroll to bottom to see new rule added to list
            this.dataGridView_Exceptions.FirstDisplayedScrollingRowIndex = this.dataGridView_Exceptions.RowCount - 1;
            this.ExceptionRule = new PolicyCustomRules();

            // Reset rule in progress flag
            this.fRuleInProgress = false; 

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

        /// <summary>
        /// Checkbox state for Original Filename was updated.Modify the checkbox state struct. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Checkbox state for File Description was updated. Modify the checkbox state struct. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Checkbox state for product name was updated. Modify the checkbox state struct. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Checkbox state for internal name was updated. Modify the checkbox state struct. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Checkbox state for minimum version was updated. Modify the checkbox state struct. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            this.ConditionsPanel.ClearLabel_ErrorText();
        }

        /// <summary>
        /// Sets the label to visible and the text to errorText
        /// </summary>
        /// <param name="errorText"></param>
        private void SetLabel_ErrorText(string errorText)
        {
            this.ConditionsPanel.SetLabel_ErrorText(errorText);
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
                ClearLabel_ErrorText();

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
                this.ExceptionRule.Type = PolicyCustomRules.RuleType.FolderPath;
            }
        }

        /// <summary>
        /// Fires on Load, Paint, Refresh. 
        /// Sets the colors on the UI elements for Dark and Light mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exceptions_Control_Validated(object sender, EventArgs e)
        {
            // Set Controls Color (e.g. Panels, buttons)
            SetControlsColor();

            // Set Textboxes Color
            List<TextBox> textBoxes = new List<TextBox>();
            GetTextBoxesRecursive(this, textBoxes);
            SetTextBoxesColor(textBoxes);

            // Set Comboboxes Color
            List<ComboBox> comboBoxes = new List<ComboBox>();
            GetComboBoxesRecursive(this, comboBoxes);
            SetComboboxesColor(comboBoxes);

            // Set Labels Color
            List<Label> labels = new List<Label>();
            GetLabelsRecursive(this, labels);
            SetLabelsColor(labels);

            // Set PolicyType Form back color
            SetFormBackColor();

            // Set color of the Grid
            SetGridColors();
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
                foreach (Control control in this.Controls)
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

                    // Radio buttons
                    else if (control is RadioButton radioButton
                        && (radioButton.Tag == null || radioButton.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        radioButton.ForeColor = Color.White;
                        radioButton.BackColor = Color.FromArgb(15, 15, 15);
                    }
                }
            }

            // Light Mode
            else
            {
                foreach (Control control in this.Controls)
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

                    // Radio buttons
                    else if (control is RadioButton radioButton
                        && (radioButton.Tag == null || radioButton.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        radioButton.ForeColor = Color.Black;
                        radioButton.BackColor = Color.White;
                    }
                }
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
                        textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
                        textBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
                        comboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
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
                        comboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
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
        /// Set the Rules Grid colors for Dark and Light Mode
        /// </summary>
        private void SetGridColors()
        {
            // Set the Rules Grid colors for Light and Dark Mode 

            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                dataGridView_Exceptions.BackgroundColor = Color.FromArgb(15, 15, 15);
                dataGridView_Exceptions.GridColor = Color.FromArgb(15, 15, 15);

                // Header
                dataGridView_Exceptions.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 15, 15);
                dataGridView_Exceptions.RowHeadersDefaultCellStyle.ForeColor = Color.White;

                // Cells
                dataGridView_Exceptions.DefaultCellStyle.BackColor = Color.FromArgb(15, 15, 15);
                dataGridView_Exceptions.DefaultCellStyle.ForeColor = Color.White;

                // Grid lines
                dataGridView_Exceptions.GridColor = Color.Silver;
            }

            // Light Mode
            else
            {
                dataGridView_Exceptions.BackgroundColor = Color.LightGray;
                dataGridView_Exceptions.GridColor = Color.DimGray;

                // Header
                dataGridView_Exceptions.RowHeadersDefaultCellStyle.BackColor = Color.LightGray;
                dataGridView_Exceptions.RowHeadersDefaultCellStyle.ForeColor = Color.Black;

                // Cells
                dataGridView_Exceptions.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                dataGridView_Exceptions.DefaultCellStyle.ForeColor = Color.Black;

                // Grid lines
                dataGridView_Exceptions.GridColor = Color.Black;
            }
        }
    }
}