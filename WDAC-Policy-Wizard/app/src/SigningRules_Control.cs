// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization; 
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel; 

namespace WDAC_Wizard
{
    public partial class SigningRules_Control : UserControl
    {
        // CI Policy objects
        private WDAC_Policy Policy;
        private PolicyCustomRules PolicyCustomRule;     // One instance of a custom rule. Appended to Policy.CustomRules
        private List<string> AllFilesinFolder;          // List to track all files in a folder 

        private Logger Log;
        private MainWindow _MainWindow;
        private string XmlPath;

        private int RowSelected; // Data grid row number selected by the user 
        private int rowInEdit = -1;
        private DisplayObject displayObjectInEdit;

        // Declare an ArrayList to serve as the data store. 
        private System.Collections.ArrayList displayObjects =
            new System.Collections.ArrayList();

        public SigningRules_Control(MainWindow pMainWindow)
        {
            InitializeComponent();
            this.Policy = new WDAC_Policy();
            this.PolicyCustomRule = new PolicyCustomRules();
            this.AllFilesinFolder = new List<string>(); 

            this._MainWindow = pMainWindow;
            this._MainWindow.RedoFlowRequired = false;
            this._MainWindow.CustomRuleinProgress = false; 
            this.Log = this._MainWindow.Log;
            this.RowSelected = -1; 
        }

        /// <summary>
        /// Reads in the template or supplemental policy signed file rules and displays them to the user in the DataGridView. 
        /// Executing on UserControl load.
        /// </summary>
        private void SigningRules_Control_Load(object sender, EventArgs e)
        {
            // Try to read CI policy. Fail out gracefully if corrupt and return to home screen
            if(!readSetRules(sender, e))
                return;
            displayRules();
        }

        
        //private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)

        /// <summary>
        /// Shows the Custom Rules Panel when the user clicks on +Custom Rules. 
        /// </summary>
        private void label_AddCustomRules_Click(object sender, EventArgs e)
        {
            if(panel_CustomRules.Visible)
            {
                panel_CustomRules.Visible = false;
                label_AddCustomRules.Text = "+ Custom Rules"; 
            }
            else
            {
                panel_CustomRules.Visible = true;
                comboBox_RuleType.SelectedItem = "Publisher";             // Set as default 
                label_AddCustomRules.Text = "- Custom Rules";
            }
            

            this.Log.AddInfoMsg("--- Create Custom Rules Selected ---"); 
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
        private void ClearCustomRulesPanel(bool clearComboBox=false)
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
        /// Flips the RuleType from Allow to Deny and vice-versa when either radioButton is selected. 
        /// By default, the rules are set to Type=Allow.
        /// </summary>
        private void radioButton_Allow_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_Allow.Checked && !radioButton_Deny.Checked)
                this.PolicyCustomRule.SetRulePermission(PolicyCustomRules.RulePermission.Allow);

            else
                this.PolicyCustomRule.SetRulePermission(PolicyCustomRules.RulePermission.Deny);


            this.Log.AddInfoMsg(String.Format("Allow Radio Button set to {0}", 
                this.PolicyCustomRule.GetRulePermission() == PolicyCustomRules.RulePermission.Allow));
            
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


        /// <summary>
        /// Launches the FileDialog and prompts user to select the reference file. 
        /// Based on rule type, sets the UI elements for Publisher, FilePath or Hash rules. 
        /// </summary>
        private void button_Browse_Click(object sender, EventArgs e)
        {
            // Browse button for reference file:
            if(comboBox_RuleType.SelectedItem == null)
            {
                label_Error.Visible = true; 
                label_Error.Text = "Please select a rule type first.";
                this.Log.AddWarningMsg("Browse button selected before rule type selected. Set rule type first."); 
                return;
            }

            if(this.PolicyCustomRule.GetRuleType() != PolicyCustomRules.RuleType.Folder)
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
                PolicyCustomRule.FileInfo.Add("CompanyName", String.IsNullOrEmpty(fileInfo.CompanyName)? "N/A": fileInfo.CompanyName);
                PolicyCustomRule.FileInfo.Add("ProductName", String.IsNullOrEmpty(fileInfo.ProductName) ? "N/A" : fileInfo.ProductName);
                PolicyCustomRule.FileInfo.Add("OriginalFilename", String.IsNullOrEmpty(fileInfo.OriginalFilename) ? "N/A" : fileInfo.OriginalFilename);
                PolicyCustomRule.FileInfo.Add("FileVersion", String.IsNullOrEmpty(fileInfo.FileVersion) ? "N/A" : fileInfo.FileVersion);
                PolicyCustomRule.FileInfo.Add("FileName", Path.GetFileName(fileInfo.FileName)); //Get file name without path
                PolicyCustomRule.FileInfo.Add("FileDescription", String.IsNullOrEmpty(fileInfo.FileDescription) ? "N/A" : fileInfo.FileDescription);
                PolicyCustomRule.FileInfo.Add("InternalName", String.IsNullOrEmpty(fileInfo.InternalName) ? "N/A" : fileInfo.InternalName);

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
                catch(Exception exp)
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

                PolicyCustomRule.FileInfo.Add("LeafCertificate", String.IsNullOrEmpty(leafCertSubjectName) ? "N/A" : leafCertSubjectName);
                PolicyCustomRule.FileInfo.Add("PCACertificate", String.IsNullOrEmpty(pcaCertSubjectName) ? "N/A" : pcaCertSubjectName);

            }

            switch (this.PolicyCustomRule.GetRuleType())
            {
                case PolicyCustomRules.RuleType.Publisher:


                    // UI
                    textBox_ReferenceFile.Text = PolicyCustomRule.ReferenceFile;
                    labelSlider_0.Text = @"Issuing CA:";
                    labelSlider_1.Text = @"Publisher:";
                    labelSlider_2.Text = @"File version:";
                    labelSlider_3.Text = @"File name:";
                    textBoxSlider_0.Text = PolicyCustomRule.FileInfo["PCACertificate"];
                    textBoxSlider_1.Text = PolicyCustomRule.FileInfo["LeafCertificate"];
                    textBoxSlider_2.Text = PolicyCustomRule.FileInfo["FileVersion"];
                    textBoxSlider_3.Text = PolicyCustomRule.FileInfo["FileName"];

                    panel_Publisher_Scroll.Visible = true;
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Visible = true;
                    publisherInfoLabel.Text = "Rule applies to all files signed by this Issuing CA and publisher with this \r\n" +
                        "file name with a version at or above the specified version number.";
                    break;

                case PolicyCustomRules.RuleType.Folder:

                    // User wants to create rule by folder level
                    PolicyCustomRule.ReferenceFile = getFolderLocation();
                    this.AllFilesinFolder = new List<string>();
                    if (PolicyCustomRule.ReferenceFile == String.Empty)
                        break;

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
                    labelSlider_0.Text = @"Original filename:";
                    labelSlider_1.Text = @"File description:";
                    labelSlider_2.Text = @"Product name:";
                    labelSlider_3.Text = @"Internal name:";
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
        /// Opens the file dialog and grabs the file path for PEs only and checks if path exists. 
        /// </summary>
        /// <returns>Returns the full path+name of the file</returns>
        private string getFileLocation()
        {
            //TODO: move these common functions to a separate class
            // Open file dialog to get file or folder path

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = @"C:\Program Files";
            openFileDialog.Title = "Browse for a signed file to use as a reference for the rule.";
            openFileDialog.CheckPathExists = true;
            // Performed scan of program files -- most common filetypes (occurence > 20 in the folder) with SIPs: 
            openFileDialog.Filter = "Portable Executable Files (*.exe; *.dll; *.rll; *.bin)|*.EXE;*.DLL;*.RLL;*.BIN|" +
                "Script Files (*.ps1, *.bat, *.vbs, *.js)|*.PS1;*.BAT;*.VBS, *.JS|" +
                "System Files (*.sys, *.hxs, *.mui, *.lex, *.mof)|*.SYS;*.HXS;*.MUI;*.LEX;*.MOF"; 
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

        /// <summary>
        /// Sets the RuleLevel for the Rule Type=Publisher custom rules and all UI elements when the 
        /// scrollbar location is modified. 
        /// </summary>
        
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
                            publisherInfoLabel.Text = "Rule applies to all files signed by this Issuing CA with this publisher, \r\n" +
                                "and file name with a version at or above the specified version number.";
                            this.Log.AddInfoMsg("Publisher file rule level set to file publisher (0)");
                        }
                        else if (pos > 2 && pos <= 6)
                        {
                            // PCACert + LeafCert + Version = SignedVersion
                            trackBar_Conditions.Value = 4;
                            this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.SignedVersion);
                            textBoxSlider_2.Text = this.PolicyCustomRule.FileInfo["FileVersion"];
                            textBoxSlider_3.Text = "*";
                            publisherInfoLabel.Text = "Rule applies to all files signed by this Issuing CA with this publisher, \r\n" +
                                "with a version at or above the specified version number.";
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
                            
                            publisherInfoLabel.Text = "Rule applies to all files signed by this Issuing CA with this publisher.";
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
                            publisherInfoLabel.Text = "Rule applies to all files signed by this issuing CA subject name.";
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
                            if (textBoxSlider_3.Text == "N/A")
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
                            if (textBoxSlider_2.Text == "N/A")
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
                            if (textBoxSlider_1.Text == "N/A")
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
                            if (textBoxSlider_0.Text == "N/A")
                                this.PolicyCustomRule.SetRuleLevel(PolicyCustomRules.RuleLevel.None);
                            publisherInfoLabel.Text = "Rule applies to all files with this original file name attribute.";
                            this.Log.AddInfoMsg(String.Format("Publisher file rule level set to original file name: {0}", textBoxSlider_0.Text));

                        }
                    }
                break; 
            }

            
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
            // If the selected attribute is not found (UI will show "N/A"), do not allow creation
            if(this.PolicyCustomRule.GetRuleLevel() == PolicyCustomRules.RuleLevel.None || (trackBar_Conditions.Value == 0 
                && this.textBoxSlider_3.Text == "N/A"))
            {
                label_Error.Visible = true;
                label_Error.Text = "The file attribute selected cannot be N/A. Please select another attribute or rule type";
                this.Log.AddWarningMsg("Create button rule selected with an empty file attribute.");
                return;
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

             

            switch(this.PolicyCustomRule.GetRuleLevel())
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

           
            this.Log.AddInfoMsg(String.Format("CUSTOM RULE: {0} - {1} - {2} ", action, level, name));

            // Attach the int row number we added it to
            this.PolicyCustomRule.RowNumber = this.rulesDataGrid.RowCount - 1;

            // Add to the DisplayObject
            this.displayObjects.Add(new DisplayObject(action, level, name, files, exceptions));
            this.rulesDataGrid.RowCount += 1;


            // Add custom list to RulesList
            this.Policy.CustomRules.Add(this.PolicyCustomRule);
            this.PolicyCustomRule = new PolicyCustomRules();

            // Scroll to bottom to see new rule added to list
            this.rulesDataGrid.FirstDisplayedScrollingRowIndex = this.rulesDataGrid.RowCount - 1;

            bubbleUp();
            
            // Reset UI view
            ClearCustomRulesPanel(true);
            this._MainWindow.CustomRuleinProgress = false; 
        }

        /// <summary>
        /// Diplays the signing rules from the template policy or the supplemental policy in the DataGridView on Control Load. 
        /// </summary>
        private void displayRules()
        {
            string friendlyName = String.Empty;    //  this.Policy.Signers[signerID].Name;
            string action = String.Empty;
            string level = String.Empty; 
            string exceptionList = String.Empty;
            string fileAttrList = String.Empty;
            string signerID = String.Empty;

            // Increase efficiency by constructing signers dictionary hint
            Dictionary<string, string> signersDict = new Dictionary<string, string>();
            Dictionary<string, string> fileRulesDict = new Dictionary<string, string>();

            foreach (var signer in this.Policy.siPolicy.Signers)
                signersDict.Add(signer.ID, signer.Name);

            // TODO: fix implementation to get file rules
            if(this.Policy.siPolicy.FileRules != null)
            {
                var fileRuleList = (object[])this.Policy.siPolicy.FileRules;
            }


            // Process publisher rules first:
            foreach (SigningScenario scenario in this.Policy.siPolicy.SigningScenarios)
            {
                // Write all Allow Signers rules
                if(scenario.ProductSigners.AllowedSigners != null)
                {
                    for (int i = 0; i < scenario.ProductSigners.AllowedSigners.AllowedSigner.Length; i++)
                    {
                        // Get signer attributes
                        signerID = scenario.ProductSigners.AllowedSigners.AllowedSigner[i].SignerId;
                        friendlyName = signersDict[signerID];    //  this.Policy.Signers[signerID].Name;
                        action = "Allow"; // signer.ID; //  this.Policy.Signers[signerID].Action;
                        level = "Publisher";

                        // Get signer exceptions - if applicable
                        if (scenario.ProductSigners.AllowedSigners.AllowedSigner[i].ExceptDenyRule != null)
                        {
                            // Iterate through all of the exceptions, get the ID and map to filename
                            for (int j = 0; j < scenario.ProductSigners.AllowedSigners.AllowedSigner[i].ExceptDenyRule.Length; j++)
                            {
                                exceptionList += String.Format("{0}, ", scenario.ProductSigners.AllowedSigners
                                    .AllowedSigner[i].ExceptDenyRule[j].DenyRuleID);
                            }
                        }

                        // Get associated/affected files
                        /*if (this.Policy.Signers[signerID].FileAttributes.Count > 0)
                        {
                            // Iterate through all of the exceptions, get the ID and map to filename
                            foreach (string ruleID in this.Policy.Signers[signerID].FileAttributes)
                            {
                                string fileAttrName = this.Policy.FileRules[ruleID].FileName;
                                if (fileAttrName == "*") // applies to all files with ver > min ver
                                    fileAttrName = "All files";
                                string minVersion = this.Policy.FileRules[ruleID].MinimumFileVersion;
                                fileAttrList += String.Format("{0} (v{1}+), ", fileAttrName, minVersion);
                            }
                        }*/

                        this.displayObjects.Add(new DisplayObject(action, level, friendlyName, fileAttrList, exceptionList));
                        this.rulesDataGrid.RowCount += 1;

                        // Get row index #, Scroll to new row index
                        //index = rulesDataGrid.Rows.Add();
                    }
                }

                // Write all Deny Signers rules
                if (scenario.ProductSigners.DeniedSigners != null)
                {
                    for (int i = 0; i < scenario.ProductSigners.DeniedSigners.DeniedSigner.Length; i++)
                    {
                        // Get signer attributes
                        signerID = scenario.ProductSigners.DeniedSigners.DeniedSigner[i].SignerId;
                        friendlyName = signersDict[signerID];    //  this.Policy.Signers[signerID].Name;
                        action = "Deny"; // signer.ID; //  this.Policy.Signers[signerID].Action;
                        level = "Publisher";

                        // Get signer exceptions - if applicable
                        if (scenario.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule != null)
                        {
                            // Iterate through all of the exceptions, get the ID and map to filename
                            for (int j = 0; j < scenario.ProductSigners.AllowedSigners.AllowedSigner[i].ExceptDenyRule.Length; j++)
                            {
                                exceptionList += String.Format("{0}, ", scenario.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule[j].AllowRuleID);
                            }
                        }

                        // Get associated/affected files
                        /*if (this.Policy.Signers[signerID].FileAttributes.Count > 0)
                        {
                            // Iterate through all of the exceptions, get the ID and map to filename
                            foreach (string ruleID in this.Policy.Signers[signerID].FileAttributes)
                            {
                                string fileAttrName = this.Policy.FileRules[ruleID].FileName;
                                if (fileAttrName == "*") // applies to all files with ver > min ver
                                    fileAttrName = "All files";
                                string minVersion = this.Policy.FileRules[ruleID].MinimumFileVersion;
                                fileAttrList += String.Format("{0} (v{1}+), ", fileAttrName, minVersion);
                            }
                        }*/

                        this.displayObjects.Add(new DisplayObject(action, level, friendlyName, fileAttrList, exceptionList));
                        this.rulesDataGrid.RowCount += 1;

                        // Get row index #, Scroll to new row index
                        //index = rulesDataGrid.Rows.Add();
                    }
                }

                // Write all "File Rules" rules
                if (scenario.ProductSigners.FileRulesRef != null)
                {
                    for (int i = 0; i < scenario.ProductSigners.FileRulesRef.FileRuleRef.Length; i++)
                    {
                        // Get signer attributes
                        signerID = scenario.ProductSigners.FileRulesRef.FileRuleRef[i].RuleID;
                        friendlyName = signersDict[signerID];    //  this.Policy.Signers[signerID].Name;
                        action = "Deny"; // signer.ID; //  this.Policy.Signers[signerID].Action;
                        level = "Publisher";

                        // Get signer exceptions - if applicable
                        if (scenario.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule != null)
                        {
                            // Iterate through all of the exceptions, get the ID and map to filename
                            for (int j = 0; j < scenario.ProductSigners.AllowedSigners.AllowedSigner[i].ExceptDenyRule.Length; j++)
                            {
                                exceptionList += String.Format("{0}, ", scenario.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule[j].AllowRuleID);
                            }
                        }

                        // Get associated/affected files
                        /*if (this.Policy.Signers[signerID].FileAttributes.Count > 0)
                        {
                            // Iterate through all of the exceptions, get the ID and map to filename
                            foreach (string ruleID in this.Policy.Signers[signerID].FileAttributes)
                            {
                                string fileAttrName = this.Policy.FileRules[ruleID].FileName;
                                if (fileAttrName == "*") // applies to all files with ver > min ver
                                    fileAttrName = "All files";
                                string minVersion = this.Policy.FileRules[ruleID].MinimumFileVersion;
                                fileAttrList += String.Format("{0} (v{1}+), ", fileAttrName, minVersion);
                            }
                        }*/

                        this.displayObjects.Add(new DisplayObject(action, level, friendlyName, fileAttrList, exceptionList));
                        this.rulesDataGrid.RowCount += 1;
                    }
                }

            }

            // Process file rules (hash, file path, file name)
            if (this.Policy.siPolicy.FileRules.Length > 0)
            {
                for(int i=0; i< this.Policy.siPolicy.FileRules.Length; i++)
                {
                    /*var ruleID = this.Policy.FileRules[i]; 
                    if (ruleID.FriendlyName.Contains("Page")
                        || this.Policy.FileRules[ruleID].FriendlyName.Contains("Sha256")) // Skip the 3 other hash instances -- no need to show to user (saves time)
                        continue;
                    else
                    {
                        // Write to UI
                        action = this.Policy.FileRules[ruleID].Action;
                        level = this.Policy.FileRules[ruleID].GetRuleType().ToString();

                        if (this.Policy.FileRules[ruleID].GetRuleType() == PolicyFileRules.RuleType.FileName &&
                            this.Policy.FileRules[ruleID].FileName != null)
                            friendlyName = this.Policy.FileRules[ruleID].FileName;
                        else
                            friendlyName = this.Policy.FileRules[ruleID].FriendlyName;

                        this.displayObjects.Add(new DisplayObject(action, level, friendlyName, fileAttrList, exceptionList));
                        this.rulesDataGrid.RowCount += 1;
                    }*/
                }
            }

            // Scroll to bottom of table
            rulesDataGrid.FirstDisplayedScrollingRowIndex = this.rulesDataGrid.RowCount-1;
        }


        /// <summary>
        /// Method to parse either the template or supplemental policy and store into the custom data structures for digestion. 
        /// </summary>
        private bool readSetRules(object sender, EventArgs e)
        {
            // Always going to have to parse an XML file - either going to be pre-exisiting policy (edit mode, supplmental policy) or template policy (new base)
            if (this._MainWindow.Policy.TemplatePath != null)
                this.XmlPath = this._MainWindow.Policy.TemplatePath;
            else
                this.XmlPath = this._MainWindow.Policy.EditPolicyPath;
            this.Log.AddInfoMsg("--- Reading Set Signing Rules Beginning ---");

            try
            {
                // Read File
                XmlSerializer serializer = new XmlSerializer(typeof(SiPolicy));
                StreamReader reader = new StreamReader(this.XmlPath);
                this.Policy.siPolicy = (SiPolicy)serializer.Deserialize(reader);
                reader.Close();

            } 
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("ReadSetRules() has encountered an error: ", exp);
                // Prompt user for additional confirmation
                DialogResult res = MessageBox.Show("The Wizard is unable to read your CI policy xml file. The policy XML is corrupted. ",
                    "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (res == DialogResult.OK)
                    this._MainWindow.ResetWorkflow(sender, e);
                return false; 
            }
            
            bubbleUp(); // all original signing rules are set in MainWindow object - ...
                        //all mutations to rules are from here on completed using cmdlets
            return true; 
        }

        /// <summary>
        /// A reccursive function to list all of the PE files in a folder and subfolders to create allow and 
        /// deny rules on folder path rules. Stores filepaths in this.AllFilesinFolder. 
        /// </summary>
        /// <param name="folder">The folder path </param>
        private void ProcessAllFiles(string folder)
        {
            // All extensions we look for
            var ext = new List<string> { ".exe", ".ps1", ".bat", ".vbs", ".js" };
            foreach (var file in Directory.GetFiles(folder,"*.*").Where(s => ext.Contains(Path.GetExtension(s))))
                this.AllFilesinFolder.Add(file);

            // Reccursively grab files from sub dirs
            foreach (string subDir in Directory.GetDirectories(folder))
            {
                try
                {
                    ProcessAllFiles(subDir);
                }
                catch(Exception e)
                {
                    Console.WriteLine(String.Format("Exception found: {0} ", e));
                }
            }

            //PolicyCustomRule.FolderContents = Directory.GetFiles(PolicyCustomRule.ReferenceFile, "*.*", SearchOption.AllDirectories)
        }

        /// <summary>
        /// Method to set all of the MainWindow objects to the local instances of the Policy helper class objects.
        /// </summary>
        private void bubbleUp()
        {
            // Passing rule, signing scenarios, etc datastructs to MainWindow class
           this._MainWindow.Policy.CISigners = this.Policy.CISigners;
           this._MainWindow.Policy.EKUs = this.Policy.EKUs;
           this._MainWindow.Policy.FileRules = this.Policy.FileRules;
           this._MainWindow.Policy.Signers = this.Policy.Signers;
           this._MainWindow.Policy.SigningScenarios = this.Policy.SigningScenarios;
           this._MainWindow.Policy.UpdateSigners = this.Policy.UpdateSigners;
           this._MainWindow.Policy.SupplementalSigners = this.Policy.SupplementalSigners;
           this._MainWindow.Policy.CISigners = this.Policy.CISigners;
           this._MainWindow.Policy.PolicySettings = this.Policy.PolicySettings;
           this._MainWindow.Policy.CustomRules = this.Policy.CustomRules;

        }

        /// <summary>
        /// Removes the highlighted rule row in the this.rulesDataGrid DataGridView. 
        /// Can only be executed on custom rules from this session. 
        /// </summary>
        private void deleteButton_Click(object sender, EventArgs e)
        {
  
            // Get info about the rule user wants to delete: row index and value
            int rowIdx = this.rulesDataGrid.CurrentCell.RowIndex;

            string ruleName = (String)this.rulesDataGrid["Column_Name", rowIdx].Value;
            string ruleType = (String)this.rulesDataGrid["Column_Level", rowIdx].Value;

            if (String.IsNullOrEmpty(ruleName) && String.IsNullOrEmpty(ruleType)) // Not a valid rule -- break
                return; 

            // Prompt the user for additional deletion confirmation
            DialogResult res = MessageBox.Show(String.Format("Are you sure you want to delete the '{0}' rule?", ruleName), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (res == DialogResult.Yes)
            {
                var ruleIDs = new List<string>();

                // Convert ruleName to SignerID to delete from //Signers and //SigningScenarios
                // Handle both Signer template rules and custom rules
                Dictionary<string, PolicySigners>.KeyCollection _keys = this.Policy.Signers.Keys;
                Dictionary<string, PolicyFileRules>.KeyCollection _fileRulekeys = this.Policy.FileRules.Keys;

                if (ruleType == "Publisher")
                {
                    foreach (var signer in this.Policy.siPolicy.Signers)
                    {
                        if (ruleName == signer.Name)
                            ruleIDs.Add(signer.ID);
                    }
                }

                else
                {
                    // Get list of IDs for related rules. ie. Deleting one hash should delete all four hash values.
                    if (ruleType == "Hash")
                        ruleName = ruleName.Substring(0, ruleName.IndexOf("Hash") - 1);

                    foreach (var key in _fileRulekeys)
                    {
                        var pFileRule = this.Policy.FileRules[key];
                        if (pFileRule.FriendlyName.Contains(ruleName))
                            ruleIDs.Add(key);

                        foreach (var fileRule in this.Policy.siPolicy.FileRules)
                        {
                            //if(fileRule.)
                        }
                    }
                }

                // Remove from DisplayObject
                if (rowIdx < this.displayObjects.Count)
                    this.displayObjects.RemoveAt(rowIdx);


                // Only structure we have to remove the rule from is the one that is used in writing rules -- custom rules
                for (int i = this.Policy.CustomRules.Count - 1; i >= 0; i--)
                {
                    if (this.Policy.CustomRules[i].RowNumber == rowIdx)
                        this.Policy.CustomRules.Remove(this.Policy.CustomRules[i]); // Remove from structs
                }

                // Try to delete the rule from the doc
                XmlDocument doc = new XmlDocument();
                doc.Load(this.XmlPath); // Reading from the template (either one of the 3 bases or editing)


                // A friendly name can have multiple references in the doc -- remove each one
                // Skips section if custom rule
                if (ruleType == "Publisher")
                {
                    // Signer specific
                    XmlNodeList signerNodeList = doc.GetElementsByTagName("Signer");
                    XmlNodeList signingScenarioAllowList = doc.GetElementsByTagName("AllowedSigner");
                    XmlNodeList signingScenarioDenyList = doc.GetElementsByTagName("DeniedSigner");
                    foreach (var ruleID in ruleIDs)
                    {
                        for (int i = signerNodeList.Count - 1; i >= 0; i--) // Traverse through xml elements and delete signers == ruleID
                        {
                            if (signerNodeList[i].OuterXml.Contains(ruleID))
                                signerNodeList[i].ParentNode.RemoveChild(signerNodeList[i]);
                        }

                        for (int i = signingScenarioAllowList.Count - 1; i >= 0; i--) // Remove from signing scenarios too
                        {
                            if (signingScenarioAllowList[i].OuterXml.Contains(ruleID))
                                signingScenarioAllowList[i].ParentNode.RemoveChild(signingScenarioAllowList[i]);
                        }

                        for (int i = signingScenarioDenyList.Count - 1; i >= 0; i--) // Remove from signing scenarios too
                        {
                            if (signingScenarioDenyList[i].OuterXml.Contains(ruleID))
                                signingScenarioDenyList[i].ParentNode.RemoveChild(signingScenarioDenyList[i]);
                        }

                    }
                }

                else
                {
                    // Filerule specific
                    XmlNodeList allowFileRuleNodeList = doc.GetElementsByTagName("Allow"); // in <FileRules>
                    XmlNodeList denyFileRuleNodeList = doc.GetElementsByTagName("Deny");   // in <FileRules>
                    XmlNodeList fileAttrNodeList = doc.GetElementsByTagName("FileRuleRef"); // in <SigningScnearios-->FileRulesRef>
                    foreach (var ruleID in ruleIDs)
                    {
                        for (int i = allowFileRuleNodeList.Count - 1; i >= 0; i--) // Traverse through xml elements and delete signers == ruleID
                        {
                            if (allowFileRuleNodeList[i].OuterXml.Contains(ruleID))
                                allowFileRuleNodeList[i].ParentNode.RemoveChild(allowFileRuleNodeList[i]);
                        }

                        for (int i = denyFileRuleNodeList.Count - 1; i >= 0; i--) // Remove from file rule
                        {
                            if (denyFileRuleNodeList[i].OuterXml.Contains(ruleID))
                                denyFileRuleNodeList[i].ParentNode.RemoveChild(denyFileRuleNodeList[i]);
                        }

                        for (int i = fileAttrNodeList.Count - 1; i >= 0; i--) // Remove from signing scenarios too
                        {
                            if (fileAttrNodeList[i].OuterXml.Contains(ruleID))
                                fileAttrNodeList[i].ParentNode.RemoveChild(fileAttrNodeList[i]);
                        }

                    }
                }

                // Delete from UI elements:
                this.rulesDataGrid.Rows.RemoveAt(rowIdx);
                doc.Save(this.XmlPath);
            } 
        }

        /// <summary>
        /// Highlights the row of data in the DataGridView
        /// </summary>
        private void DataClicked(object sender, DataGridViewCellEventArgs e)
        {
            // Remove highlighting from previous selected row
            DataGridViewCellStyle defaultCellStyle = new DataGridViewCellStyle();
            defaultCellStyle.BackColor = Color.White;
            if(this.RowSelected > 0 && this.RowSelected < this.rulesDataGrid.Rows.Count)
                this.rulesDataGrid.Rows[this.RowSelected].DefaultCellStyle = defaultCellStyle; 

            // Highlight the row to show user feedback
            DataGridViewCellStyle highlightCellStyle = new DataGridViewCellStyle();
            highlightCellStyle.BackColor = Color.FromArgb(0, 120, 215); 
            DataGridViewRow customRow = this.rulesDataGrid.CurrentRow;
            this.rulesDataGrid.Rows[customRow.Index].DefaultCellStyle = highlightCellStyle;
            this.RowSelected = customRow.Index; 
            
        }

        /// <summary>
        /// Called when DataGridView needs to paint data
        /// </summary>
        private void rulesDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            // If this is the row for new records, no values are needed.
            if (e.RowIndex == this.rulesDataGrid.RowCount - 1) return;

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
            switch (this.rulesDataGrid.Columns[e.ColumnIndex].Name)
            {
                case "column_Action":
                    e.Value = displayObject.Action;
                    break;

                case "column_Level":
                    e.Value = displayObject.Level;
                    break;

                case "Column_Name":
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

        private void SettingUpdateTimer_Tick(object sender, EventArgs e)
        {
            this.label_Error.Visible = false;
        }
    }

    // Class for the datastore
    public class DisplayObject
    {
        public string Action;
        public string Level;
        public string Name;
        public string Files;
        public string Exceptions; 

        public DisplayObject()
        {
            this.Action = String.Empty;
            this.Level = String.Empty;
            this.Name = String.Empty;
            this.Files = String.Empty;
            this.Exceptions = String.Empty;
        }

        public DisplayObject(string action, string level, string name, string files, string exceptions)
        {
            this.Action = action;
            this.Level = level;
            this.Name = name;
            this.Files = files;
            this.Exceptions = exceptions;
        }
    }

}
