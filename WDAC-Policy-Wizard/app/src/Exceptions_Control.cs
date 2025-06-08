using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO; 


namespace WDAC_Wizard
{
    public partial class Exceptions_Control : UserControl
    {
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
            ExceptionRule = new PolicyCustomRules();
            CustomRule = pRuleConditionsPanel.PolicyCustomRule;
            ConditionsPanel = pRuleConditionsPanel;

            DefaultValues = new string[5];
            fRuleInProgress = false; 
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
            panel_FileFolder.Visible = false;

            //Other
            textBox_ReferenceFile.Clear();
            ClearLabel_ErrorText(); 

            // Reset the rule type combobox
            if (clearComboBox)
            {
                comboBox_ExceptionType.SelectedItem = null;
                comboBox_ExceptionType.Text = "--Select--";
            }
        }

        private void ComboBox_ExceptionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if the selected item is null (this occurs after reseting it - rule creation)
            if (comboBox_ExceptionType.SelectedIndex < 0)
                return;

            ClearLabel_ErrorText();

            // Set rule in progress flag
            fRuleInProgress = true; 

            string selectedOpt = comboBox_ExceptionType.SelectedItem.ToString();
            ClearCustomRulesPanel(false);

            switch (selectedOpt)
            {
                case "File Path":
                    ExceptionRule.SetRuleType(PolicyCustomRules.RuleType.FilePath);
                    ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FilePath);
                    panel_FileFolder.Visible = true;
                    radioButton_File.Checked = true; // By default, 
                    break;

                case "File Attributes":
                    ExceptionRule.SetRuleType(PolicyCustomRules.RuleType.FileAttributes);
                    ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.InternalName); // Match UI by default
                    break;

                case "File Hash":
                    ExceptionRule.SetRuleType(PolicyCustomRules.RuleType.Hash);
                    ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.Hash);
                    break;

                default:
                    break;
            }
            Logger.Log.AddInfoMsg(String.Format("Exception File Rule Level Set to {0}", selectedOpt));
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
                ConditionsPanel.SetLabel_ErrorText("Please set exception rule type first");
                Logger.Log.AddWarningMsg("Browse button selected before rule type selected. Set rule type first.");
                return;
            }

            if (ExceptionRule.GetRuleType() != PolicyCustomRules.RuleType.FolderPath)
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
            switch (ExceptionRule.GetRuleType())
            {
                case PolicyCustomRules.RuleType.FolderPath:

                    // User wants to create rule by folder level
                    ExceptionRule.ReferenceFile = GetFolderLocation();
                    if (ExceptionRule.ReferenceFile == String.Empty)
                    {
                        break;
                    }

                    textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;

                    // Show right side of the text
                    if(textBox_ReferenceFile.TextLength > 0)
                    {
                        textBox_ReferenceFile.SelectionStart = textBox_ReferenceFile.TextLength - 1;
                        textBox_ReferenceFile.ScrollToCaret();
                    }
                    
                    ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.Folder);
                    break;


                case PolicyCustomRules.RuleType.FilePath:

                    // FILE LEVEL

                    // UI updates
                    radioButton_File.Checked = true;
                    textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;

                    // Show right side of the text
                    if(textBox_ReferenceFile.TextLength > 0)
                    {
                        textBox_ReferenceFile.SelectionStart = textBox_ReferenceFile.TextLength - 1;
                        textBox_ReferenceFile.ScrollToCaret();
                    }
                   
                    panel_Publisher_Scroll.Visible = false;
                    break;

                case PolicyCustomRules.RuleType.FileAttributes:

                    // UI 
                    textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;

                    // Show right side of the text
                    if (textBox_ReferenceFile.TextLength > 0)
                    {
                        textBox_ReferenceFile.SelectionStart = textBox_ReferenceFile.TextLength - 1;
                        textBox_ReferenceFile.ScrollToCaret();
                    }

                    DefaultValues[0] = ExceptionRule.FileInfo["OriginalFilename"];
                    DefaultValues[1] = ExceptionRule.FileInfo["FileDescription"];
                    DefaultValues[2] = ExceptionRule.FileInfo["ProductName"];
                    DefaultValues[3] = ExceptionRule.FileInfo["InternalName"];
                    DefaultValues[4] = ExceptionRule.FileInfo["FileVersion"];

                    textBox_originalfilename.Text = DefaultValues[0]; 
                    textBox_filedescription.Text = DefaultValues[1];
                    textBox_product.Text = DefaultValues[2];
                    textBox_internalname.Text = DefaultValues[3];
                    textBox_minversion.Text = DefaultValues[4];

                    panel_Publisher_Scroll.Visible = true;
                    SetLabel_ErrorText("Rule applies to all files with these file description attributes.");
                    break;

                case PolicyCustomRules.RuleType.Hash:

                    // UI updates
                    panel_Publisher_Scroll.Visible = false;
                    textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;
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
            if(CustomRule != null)
            {
                string ruleConditionString = CustomRule.Permission.ToString() + " "; // e.g. "Allow " or "Deny " ...

                switch (CustomRule.Level)
                {
                    case PolicyCustomRules.RuleLevel.PcaCertificate:
                        ruleConditionString += "files signed by CA: ";
                        ruleConditionString += CustomRule.FileInfo["PCACertificate"];
                        break;

                    case PolicyCustomRules.RuleLevel.Publisher:
                        ruleConditionString += "files signed by: ";
                        ruleConditionString += CustomRule.FileInfo["LeafCertificate"];
                        break;

                    case PolicyCustomRules.RuleLevel.SignedVersion:
                        ruleConditionString += "files signed by: ";
                        ruleConditionString += CustomRule.FileInfo["LeafCertificate"]; 
                        ruleConditionString += " with version " + CustomRule.FileInfo["FileVersion"] + " or greater";
                        break;

                    case PolicyCustomRules.RuleLevel.FilePublisher:
                        ruleConditionString += "files signed by: ";
                        ruleConditionString += CustomRule.FileInfo["LeafCertificate"]; 
                        ruleConditionString += " with filename " + CustomRule.FileInfo["FileName"];
                        break;

                    case PolicyCustomRules.RuleLevel.FileName:
                        ruleConditionString += "files with file name: ";
                        ruleConditionString += CustomRule.FileInfo["FileName"]; 
                        break;

                    case PolicyCustomRules.RuleLevel.FileDescription:
                        ruleConditionString += "files with file description: ";
                        ruleConditionString += CustomRule.FileInfo["FileDescription"];
                        break;

                    case PolicyCustomRules.RuleLevel.InternalName:
                        ruleConditionString += "files with internal name: "; 
                        ruleConditionString += CustomRule.FileInfo["InternalName"];
                        break;

                    case PolicyCustomRules.RuleLevel.OriginalFileName:
                        ruleConditionString += "files with original file name: ";
                        ruleConditionString += CustomRule.FileInfo["OriginalFilename"];
                        break;

                    case PolicyCustomRules.RuleLevel.ProductName:
                        ruleConditionString += "files with product name: "; 
                        ruleConditionString += CustomRule.FileInfo["ProductName"];
                        break;

                    case PolicyCustomRules.RuleLevel.FilePath:
                        ruleConditionString += "files with path: ";
                        ruleConditionString += CustomRule.ReferenceFile; //Full file path
                        break;

                    case PolicyCustomRules.RuleLevel.Folder:
                        ruleConditionString += "files under folder path: ";
                        ruleConditionString += CustomRule.ReferenceFile; //Full folder path
                        break;

                    case PolicyCustomRules.RuleLevel.Hash:
                        ruleConditionString += "hash defined by: "; 
                        ruleConditionString += CustomRule.FileInfo["FileName"];
                        break;
                }

                ruleCondition_Label.Text = FormatText(ruleConditionString); // "Rule Condition:\n\r" + this.CustomRule.Level.ToString() + " " + this.CustomRule.FileInfo["FileName"];
                ruleCondition_Label.Visible = true; 
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
                if(CustomRule.Type == PolicyCustomRules.RuleType.Publisher)
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
                else if (CustomRule.Type == PolicyCustomRules.RuleType.FileAttributes)
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
                else if(CustomRule.Type == PolicyCustomRules.RuleType.Hash)
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
            return fRuleInProgress;
        }

        /// <summary>
        /// Triggered by Add Exception button click on custom rule conditions panel. 
        /// Add exception to the exception table
        /// </summary>
        public void AddException()
        {
            // Set ExceptionRule.Level to opposite of the PolicyCustomRule.Level
            if(CustomRule.Permission == PolicyCustomRules.RulePermission.Allow)
            {
                ExceptionRule.Permission = PolicyCustomRules.RulePermission.Deny; 
            }
            else
            {
                ExceptionRule.Permission = PolicyCustomRules.RulePermission.Allow; 
            }

            // Assert that file path rules are not valid exceptions in the kernel
            if(CustomRule.SigningScenarioCheckStates.kmciEnabled && 
                (ExceptionRule.Type == PolicyCustomRules.RuleType.FilePath
                || ExceptionRule.Type == PolicyCustomRules.RuleType.FolderPath))
            {
                ConditionsPanel.SetLabel_ErrorText(Properties.Resources.InvalidKMCIRule);
                return;
            }

            // Check that fields are valid, otherwise break and show error msg
            if(ExceptionRule == null 
                || String.IsNullOrEmpty(ExceptionRule.ReferenceFile)
                || ExceptionRule.Type == PolicyCustomRules.RuleType.None)
            {
                ConditionsPanel.SetLabel_ErrorText("Invalid exception selection. Please select a level and reference file");
                return; 
            }

            // Get the values from the textboxes for the file attributes rule
            if(ExceptionRule.Type == PolicyCustomRules.RuleType.FileAttributes)
            {
                if(!ExceptionRule.IsAnyBoxChecked())
                {
                    ConditionsPanel.SetLabel_ErrorText(Properties.Resources.InvalidCheckboxState);
                    return;
                }

                ExceptionRule.CustomValues.FileName = textBox_originalfilename.Text;
                ExceptionRule.CustomValues.Description = textBox_filedescription.Text;
                ExceptionRule.CustomValues.ProductName = textBox_product.Text;
                ExceptionRule.CustomValues.InternalName = textBox_internalname.Text; 
                ExceptionRule.CustomValues.MinVersion= textBox_minversion.Text;
            }

            // Add the exception to the custom rule and table
            CustomRule.AddException(ExceptionRule);

            // New Display object
            DisplayObject displayObject = new DisplayObject();
            displayObject.Action = ExceptionRule.Permission.ToString();
            displayObject.Level = ExceptionRule.Type.ToString();
            displayObject.Name = Path.GetFileName(ExceptionRule.ReferenceFile.ToString());

            displayObjects.Add(displayObject);
            dataGridView_Exceptions.RowCount += 1;

            // Scroll to bottom to see new rule added to list
            dataGridView_Exceptions.FirstDisplayedScrollingRowIndex = dataGridView_Exceptions.RowCount - 1;
            ExceptionRule = new PolicyCustomRules();

            // Reset rule in progress flag
            fRuleInProgress = false; 

            // Reset the UI
            ClearCustomRulesPanel(true); 
        }

        /// <summary>
        /// Called when DataGridView needs to paint data
        /// </summary>
        private void DataGridView_Exceptions_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            // If this is the row for new records, no values are needed.
            if (e.RowIndex == dataGridView_Exceptions.RowCount - 1) return;

            DisplayObject displayObject = null;

            // Store a reference to the Customer object for the row being painted.
            if (e.RowIndex == rowInEdit)
            {
                displayObject = displayObjectInEdit;
            }
            else
            {
                displayObject = (DisplayObject)displayObjects[e.RowIndex];
            }

            // Set the cell value to paint using the Customer object retrieved.
            switch (dataGridView_Exceptions.Columns[e.ColumnIndex].Name)
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
            if (checkBox_OriginalFilename.Checked)
            {
                if (textBox_originalfilename.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(textBox_originalfilename.Text))
                {
                    ClearLabel_ErrorText();
                    ExceptionRule.CheckboxCheckStates.checkBox0 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            checkBox_OriginalFilename.Checked = false;
            ExceptionRule.CheckboxCheckStates.checkBox0 = false;
        }

        /// <summary>
        /// Checkbox state for File Description was updated. Modify the checkbox state struct. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_FileDescription_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_FileDescription.Checked)
            {
                if (textBox_filedescription.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(textBox_filedescription.Text))
                {
                    ClearLabel_ErrorText();
                    ExceptionRule.CheckboxCheckStates.checkBox1 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            checkBox_FileDescription.Checked = false;
            ExceptionRule.CheckboxCheckStates.checkBox1 = false;
        }

        /// <summary>
        /// Checkbox state for product name was updated. Modify the checkbox state struct. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_Product_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_Product.Checked)
            {
                if (textBox_product.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(textBox_product.Text))
                {
                    ClearLabel_ErrorText();
                    ExceptionRule.CheckboxCheckStates.checkBox2 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            checkBox_Product.Checked = false;
            ExceptionRule.CheckboxCheckStates.checkBox3 = false;
        }

        /// <summary>
        /// Checkbox state for internal name was updated. Modify the checkbox state struct. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_InternalName_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_InternalName.Checked)
            {
                if (textBox_internalname.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(textBox_internalname.Text))
                {
                    ClearLabel_ErrorText();
                    ExceptionRule.CheckboxCheckStates.checkBox3 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            checkBox_InternalName.Checked = false;
            ExceptionRule.CheckboxCheckStates.checkBox3 = false;
        }

        /// <summary>
        /// Checkbox state for minimum version was updated. Modify the checkbox state struct. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_MinVersion_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_MinVersion.Checked)
            {
                if (textBox_minversion.Text != Properties.Resources.DefaultFileAttributeString ||
                    String.IsNullOrEmpty(textBox_minversion.Text))
                {
                    ClearLabel_ErrorText();
                    ExceptionRule.CheckboxCheckStates.checkBox4 = true;
                    return;
                }
                else
                {
                    SetLabel_ErrorText(Properties.Resources.InvalidAttributeSelection_Error);
                }
            }

            checkBox_MinVersion.Checked = false;
            ExceptionRule.CheckboxCheckStates.checkBox4 = false;
        }

        /// <summary>
        /// Clear the error label text and set to invisible
        /// </summary>
        private void ClearLabel_ErrorText()
        {
            ConditionsPanel.ClearLabel_ErrorText();
        }

        /// <summary>
        /// Sets the label to visible and the text to errorText
        /// </summary>
        /// <param name="errorText"></param>
        private void SetLabel_ErrorText(string errorText)
        {
            ConditionsPanel.SetLabel_ErrorText(errorText);
        }

        private void checkBoxCustomValues_CheckedChanged(object sender, EventArgs e)
        {
            // Set the UI first
            if (checkBoxCustomValues.Checked)
            {
                SetTextBoxStates(true);

                // Set the custom values based on existing
                ExceptionRule.CustomValues.FileName = textBox_originalfilename.Text;
                ExceptionRule.CustomValues.Description = textBox_filedescription.Text;
                ExceptionRule.CustomValues.ProductName = textBox_product.Text;
                ExceptionRule.CustomValues.InternalName = textBox_internalname.Text;
                ExceptionRule.CustomValues.MinVersion = textBox_minversion.Text; 

                ExceptionRule.UsingCustomValues = true;
            }
            else
            {
                // Clear error if applicable
                ClearLabel_ErrorText();

                // Set text values back to default
                SetTextBoxStates(false);
                textBox_originalfilename.Text = DefaultValues[0];
                textBox_filedescription.Text = DefaultValues[1];
                textBox_product.Text = DefaultValues[2];
                textBox_internalname.Text = DefaultValues[3];
                textBox_minversion.Text = DefaultValues[4];

                ExceptionRule.UsingCustomValues = false;
            }
        }

        private void SetTextBoxStates(bool enabled)
        {
            if (enabled)
            {
                // If enabled, allow user input
                textBox_originalfilename.ReadOnly = false; 
                textBox_filedescription.ReadOnly = false;
                textBox_product.ReadOnly = false;
                textBox_internalname.ReadOnly = false;
                textBox_minversion.ReadOnly = false;

                textBox_originalfilename.Enabled = true;
                textBox_filedescription.Enabled = true;
                textBox_product.Enabled = true;
                textBox_internalname.Enabled = true;
                textBox_minversion.Enabled = true;

                // Set back color to white to help user determine boxes are userwriteable
                textBox_originalfilename.BackColor = Color.White;
                textBox_filedescription.BackColor = Color.White;
                textBox_product.BackColor = Color.White;
                textBox_internalname.BackColor = Color.White;
                textBox_minversion.BackColor = Color.White;
            }
            else
            {
                // Set to read only if disabled
                textBox_originalfilename.ReadOnly = true;
                textBox_filedescription.ReadOnly = true;
                textBox_product.ReadOnly = true;
                textBox_internalname.ReadOnly = true;
                textBox_minversion.ReadOnly = true;

                textBox_originalfilename.Enabled = false;
                textBox_filedescription.Enabled = false;
                textBox_product.Enabled = false;
                textBox_internalname.Enabled = false;
                textBox_minversion.Enabled = false;

                // Set back color to white to help user determine boxes are userwriteable
                textBox_originalfilename.BackColor = SystemColors.Control;
                textBox_filedescription.BackColor = SystemColors.Control;
                textBox_product.BackColor = SystemColors.Control;
                textBox_internalname.BackColor = SystemColors.Control;
                textBox_minversion.BackColor = SystemColors.Control;
            }
        }

        /// <summary>
        /// Triggered by the user selecting either File or Folder. Sets the rule level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileFolderButtonClick(object sender, EventArgs e)
        {
            if(radioButton_File.Checked)
            {
                ExceptionRule.Level = PolicyCustomRules.RuleLevel.FilePath;
                ExceptionRule.Type = PolicyCustomRules.RuleType.FilePath;
            }
            else
            {
                ExceptionRule.Level = PolicyCustomRules.RuleLevel.Folder;
                ExceptionRule.Type = PolicyCustomRules.RuleType.FolderPath;
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

            // Set UI for the 'button_Browse' Button
            Setbutton_BrowseUI();

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
        /// Sets the colors for the button_Browse Button which depends on the 
        /// state of Light and Dark Mode
        /// </summary>
        public void Setbutton_BrowseUI()
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                button_Browse.FlatAppearance.BorderColor = Color.DodgerBlue;
                button_Browse.FlatAppearance.MouseDownBackColor = Color.FromArgb(50,30,144,255);
                button_Browse.FlatAppearance.MouseOverBackColor = Color.FromArgb(50,30,144,255);
                button_Browse.FlatStyle = FlatStyle.Flat;
                button_Browse.ForeColor = Color.DodgerBlue;
                button_Browse.BackColor = Color.Transparent;
            }

            // Light Mode
            else
            {
                button_Browse.FlatAppearance.BorderColor = Color.Black;
                button_Browse.FlatAppearance.MouseDownBackColor = Color.FromArgb(50,30,144,255);
                button_Browse.FlatAppearance.MouseOverBackColor = Color.FromArgb(50,30,144,255);
                button_Browse.FlatStyle = FlatStyle.Flat;
                button_Browse.ForeColor = Color.Black;
                button_Browse.BackColor = Color.WhiteSmoke;
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

                // Header
                dataGridView_Exceptions.RowHeadersDefaultCellStyle.BackColor = Color.Black;
                dataGridView_Exceptions.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView_Exceptions.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
                dataGridView_Exceptions.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView_Exceptions.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 82, 118);
                dataGridView_Exceptions.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

                // Borders
                dataGridView_Exceptions.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dataGridView_Exceptions.BorderStyle = BorderStyle.Fixed3D;
                dataGridView_Exceptions.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

                // Cells
                dataGridView_Exceptions.DefaultCellStyle.BackColor = Color.FromArgb(32, 32, 32);
                dataGridView_Exceptions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 24);
                dataGridView_Exceptions.DefaultCellStyle.ForeColor = Color.White;
                dataGridView_Exceptions.DefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 82, 118);
                dataGridView_Exceptions.DefaultCellStyle.SelectionForeColor = Color.White;

                // Grid lines
                dataGridView_Exceptions.GridColor = Color.LightSlateGray;
            }

            // Light Mode
            else
            {
                dataGridView_Exceptions.BackgroundColor = Color.White;

                // Header
                dataGridView_Exceptions.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
                dataGridView_Exceptions.RowHeadersDefaultCellStyle.ForeColor = Color.Black;
                dataGridView_Exceptions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
                dataGridView_Exceptions.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                dataGridView_Exceptions.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(174, 214, 241);
                dataGridView_Exceptions.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

                // Borders
                dataGridView_Exceptions.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                dataGridView_Exceptions.BorderStyle = BorderStyle.Fixed3D;
                dataGridView_Exceptions.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

                // Cells
                dataGridView_Exceptions.DefaultCellStyle.BackColor = Color.White;
                dataGridView_Exceptions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(236, 240, 241);
                dataGridView_Exceptions.DefaultCellStyle.ForeColor = Color.Black;
                dataGridView_Exceptions.DefaultCellStyle.SelectionBackColor = Color.FromArgb(174, 214, 241);
                dataGridView_Exceptions.DefaultCellStyle.SelectionForeColor = Color.Black;

                // Grid lines
                dataGridView_Exceptions.GridColor = Color.Black;
            }
        }
    }
}