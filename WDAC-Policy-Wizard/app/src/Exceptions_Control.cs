using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;


namespace WDAC_Wizard
{
    public partial class Exceptions_Control : UserControl
    {
        private Logger Log;
        private PolicyCustomRules ExceptionRule;     // One instance of a custom rule. Appended to Policy.CustomRules
        private PolicyCustomRules CustomRule;
        private CustomRuleConditionsPanel ConditionsPanel; 

        public Exceptions_Control(CustomRuleConditionsPanel pRuleConditionsPanel)
        {
            InitializeComponent();
            this.ExceptionRule = new PolicyCustomRules();
            this.Log = pRuleConditionsPanel.Log;
            this.CustomRule = pRuleConditionsPanel.PolicyCustomRule;
            this.ConditionsPanel = pRuleConditionsPanel; 
        }

        private void comboBox_RuleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if the selected item is null (this occurs after reseting it - rule creation)
            if (comboBox_RuleType.SelectedIndex < 0)
                return;

            string selectedOpt = comboBox_RuleType.SelectedItem.ToString();
            ClearCustomRulesPanel(false);
            label_Info.Visible = true;

            switch (selectedOpt)
            {
                case "Publisher":
                    this.ExceptionRule.SetRuleType(PolicyCustomRules.RuleType.Publisher);
                    this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.FilePublisher); // Match UI by default
                    break;

                case "Path":
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
            label_Info.Visible = false;

            // Reset the rule type combobox
            if (clearComboBox)
            {
                this.comboBox_RuleType.SelectedItem = null;
                this.comboBox_RuleType.Text = "--Select--";
            }
        }

        private void button_Browse_Click(object sender, EventArgs e)
        {
            // Browse button for reference file:
            if (comboBox_RuleType.SelectedItem == null)
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

                ExceptionRule.FileInfo.Add("LeafCertificate", String.IsNullOrEmpty(leafCertSubjectName) ? Properties.Resources.DefaultFileAttributeString : leafCertSubjectName);
                ExceptionRule.FileInfo.Add("PCACertificate", String.IsNullOrEmpty(pcaCertSubjectName) ? Properties.Resources.DefaultFileAttributeString : pcaCertSubjectName);

            }

            // Set the landing UI depending on the Rule type
            switch (this.ExceptionRule.GetRuleType())
            {
                case PolicyCustomRules.RuleType.Publisher:


                    // UI
                    textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;
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

                    textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;
                    //ProcessAllFiles(ExceptionRule.ReferenceFile);
                    //ExceptionRule.FolderContents = this.AllFilesinFolder; 
                    this.ExceptionRule.SetRuleLevel(PolicyCustomRules.RuleLevel.Folder);
                    break;


                case PolicyCustomRules.RuleType.FilePath:

                    // FILE LEVEL

                    // UI updates
                    radioButton_File.Checked = true;
                    textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;
                    panel_Publisher_Scroll.Visible = false;
                    break;

                case PolicyCustomRules.RuleType.FileAttributes:
                    // Creates a rule -Level FileName -SpecificFileNameLevel InternalName, FileDescription

                    // UI 
                    textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;
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
                    textBox_ReferenceFile.Text = ExceptionRule.ReferenceFile;
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
                this.ruleCondition_Label.Text = "Rule Condition\n\r" + this.CustomRule.Type.ToString() + " " + this.CustomRule.FileInfo["FileName"];
                this.ruleCondition_Label.Visible = true; 
            }
        }
    }
}
