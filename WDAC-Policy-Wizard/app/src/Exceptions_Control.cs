using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WDAC_Wizard
{
    public partial class Exceptions_Control : UserControl
    {
        public Exceptions_Control(CustomRuleConditionsPanel pRuleConditionsPanel)
        {
            InitializeComponent();
        }



        private void comboBox_RuleType_SelectedIndexChanged(object sender, EventArgs e)
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

                    textBoxSlider_0.BackColor = Color.FromArgb(240, 240, 240);

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

        private void button_CreateException_Click(object sender, EventArgs e)
        {
            // Add the exception to the table
        }

        private void panel_CustomRules_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
