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
using System.Security.Cryptography.X509Certificates;

namespace WDAC_Wizard
{
    public partial class SigningRules_Control : UserControl
    {
        // CI Policy objects
        public WDAC_Policy Policy;
        private List<string> AllFilesinFolder;          // List to track all files in a folder 

        public MainWindow _MainWindow;
        private string XmlPath;

        private int RowSelected; // Data grid row number selected by the user 
        private int rowInEdit = -1;
        private DisplayObject displayObjectInEdit;
        private CustomRuleConditionsPanel customRuleConditionsPanel = null; 

        // Declare an ArrayList to serve as the data store. 
        private System.Collections.ArrayList displayObjects =
            new System.Collections.ArrayList();

        // Bool tracking whether CustomRules Panel open
        public bool isCustomPanelOpen; 

        public SigningRules_Control(MainWindow pMainWindow)
        {
            InitializeComponent();
            Policy = pMainWindow.Policy; 
            AllFilesinFolder = new List<string>(); 

            _MainWindow = pMainWindow;
            _MainWindow.RedoFlowRequired = false;
            _MainWindow.CustomRuleinProgress = false; 
            RowSelected = -1;
            isCustomPanelOpen = false; 

            Logger.Log.AddInfoMsg("==== Signing Rules Page Initialized ====");
        }

        /// <summary>
        /// Reads in the template or supplemental policy signed file rules and displays them to the user in the DataGridView. 
        /// Executing on UserControl load.
        /// </summary>
        private void SigningRules_Control_Load(object sender, EventArgs e)
        {
            // Try to read CI policy. Fail out gracefully if corrupt and return to home screen
            if(!ReadSetRules(sender, e))
            {
                return; 
            }

            try
            {
                DisplayRules();
            }
            catch(Exception exp)
            {
                Logger.Log.AddErrorMsg("DisplayRules() encountered an exception.", exp);
                DialogResult res = MessageBox.Show("The Wizard is unable to read all the rules in your CI policy xml file. The policy XML is likely corrupted. " +
                                                    "Try converting the policy to binary to locate the issue in the XML.",
                                                    "Parsing Error", 
                                                    MessageBoxButtons.OK, 
                                                    MessageBoxIcon.Error);
            }

            // Set recommended blocklist states
            SetBlocklistStates(); 
        }

        
        /// <summary>
        /// Shows the Custom Rules Panel when the user clicks on +Custom Rules. 
        /// </summary>
        private void Label_AddCustomRules_Click(object sender, EventArgs e)
        {
            // Check if the customRuleConditionsPanel is already open
            if (customRuleConditionsPanel == null || customRuleConditionsPanel.IsDisposed)
            {
                // Create and show the custom rules conditions panel
                customRuleConditionsPanel = new CustomRuleConditionsPanel(this);
                customRuleConditionsPanel.StartPosition = FormStartPosition.CenterParent;
                customRuleConditionsPanel.Owner = _MainWindow; // Set the parent form explicitly
                customRuleConditionsPanel.FormClosed += (s, args) => customRuleConditionsPanel = null; // Reset on close
                customRuleConditionsPanel.ShowDialog();


                // this.label_AddCustomRules.Text = "- Custom Rules"; 

                // Mark the panel as open
                isCustomPanelOpen = true;
            }
            else
            {
                // Bring the existing panel to the front
                customRuleConditionsPanel.BringToFront();
                customRuleConditionsPanel.Focus();
            }

            Logger.Log.AddInfoMsg("--- Create Custom Rules Selected ---"); 
        }

        
        /// <summary>
        /// Diplays the signing rules from the template policy or the supplemental policy in the DataGridView on Control Load. 
        /// </summary>
        private void DisplayRules()
        {
            string friendlyName = String.Empty;    //  this.Policy.Signers[signerID].Name;
            string action = String.Empty;
            string level = String.Empty; 
            string exceptionList = String.Empty;
            string fileAttrList = String.Empty;
            string signerID = String.Empty;
            string ruleID = String.Empty;

            // Increase efficiency by constructing signers dictionary hint
            Dictionary<string, WDACSigner> signersDict = new Dictionary<string, WDACSigner>();
            Dictionary<string, string> fileExceptionsDict = new Dictionary<string, string>();

            // Parse the siPolicy Signers to Diction<ID, Signer objs>
            foreach (var siPolicySigner in Policy.siPolicy.Signers)
            {
                WDACSigner signer = new WDACSigner();
                
                if (siPolicySigner.FileAttribRef != null)
                {
                    foreach(var fileRef in siPolicySigner.FileAttribRef)
                    {
                        signer.FileAttribRefs.Add(fileRef.RuleID);
                    }
                }

                signer.Name = siPolicySigner.Name;
                signer.ID = siPolicySigner.ID; 

                // CN
                if(siPolicySigner.CertPublisher != null)
                {
                    signer.CommonName = siPolicySigner.CertPublisher.Value; 
                }

                // Opus
                if (siPolicySigner.CertOemID != null)
                {
                    signer.CertOemID = siPolicySigner.CertOemID.Value;
                }

                signersDict.Add(siPolicySigner.ID, signer); 
            }

            // Process publisher rules first:
            foreach (SigningScenario scenario in Policy.siPolicy.SigningScenarios)
            {
                // Write all Allow Signers rules
                if(scenario.ProductSigners.AllowedSigners != null)
                {
                    for (int i = 0; i < scenario.ProductSigners.AllowedSigners.AllowedSigner.Length; i++)
                    {
                        // Get signer attributes
                        signerID = scenario.ProductSigners.AllowedSigners.AllowedSigner[i].SignerId;
                        friendlyName = signersDict[signerID].Name;   
                        action = "Allow"; 
                        level = "Publisher";
                        string exceptionID;

                        // Re-init exceptions list so subsequent signers don't inherit the exceptions of previous signers
                        exceptionList = String.Empty;

                        // Get signer exceptions - if applicable
                        if (scenario.ProductSigners.AllowedSigners.AllowedSigner[i].ExceptDenyRule != null)
                        {
                            // Iterate through all of the exceptions, get the ID and map to filename
                            for (int j = 0; j < scenario.ProductSigners.AllowedSigners.AllowedSigner[i].ExceptDenyRule.Length; j++)
                            {
                                exceptionID = scenario.ProductSigners.AllowedSigners.AllowedSigner[i].ExceptDenyRule[j].DenyRuleID;
                                exceptionList += String.Format("{0}, ", exceptionID);

                                if(!fileExceptionsDict.ContainsKey(exceptionID))
                                {
                                    fileExceptionsDict.Add(scenario.ProductSigners.AllowedSigners.AllowedSigner[i].ExceptDenyRule[j].DenyRuleID, String.Empty);
                                }
                            }
                        }

                        fileAttrList = string.Empty;

                        // Add Common Name (Cert Publisher value)
                        if (!String.IsNullOrEmpty(signersDict[signerID].CommonName))
                        {
                            fileAttrList += String.Format("CN = {0}; ", signersDict[signerID].CommonName);
                        }

                        // Add CertOemId (OPUS) field
                        if (!String.IsNullOrEmpty(signersDict[signerID].CertOemID))
                        {
                            fileAttrList += String.Format("CertOemId = {0}; ", signersDict[signerID].CertOemID);
                        }   

                        // Get associated/affected files -- FileAttributes
                        foreach (var fileRef in signersDict[signerID].FileAttribRefs)
                        {
                            fileAttrList += String.Format("{0}, ", fileRef);
                        }

                        if (!String.IsNullOrEmpty(fileAttrList))
                        {
                            fileAttrList = fileAttrList.Remove(fileAttrList.Length - 2);
                        }

                        // Remove trailing comma and whitespace on list of Exceptions
                        if (!String.IsNullOrEmpty(exceptionList))
                        {
                            exceptionList = exceptionList.Remove(exceptionList.Length - 2);
                        }

                        displayObjects.Add(new DisplayObject(action, level, friendlyName, fileAttrList, exceptionList, signerID));
                        rulesDataGrid.RowCount += 1;
                    }
                }

                // Write all Deny Signers rules
                if (scenario.ProductSigners.DeniedSigners != null)
                {
                    for (int i = 0; i < scenario.ProductSigners.DeniedSigners.DeniedSigner.Length; i++)
                    {
                        // Get signer attributes
                        signerID = scenario.ProductSigners.DeniedSigners.DeniedSigner[i].SignerId;
                        friendlyName = signersDict[signerID].Name;   
                        action = "Deny"; 
                        level = "Publisher";
                        string exceptionID;

                        // Re-init exceptions list so subsequent signers don't inherit the exceptions of previous signers
                        exceptionList = String.Empty;

                        // Get signer exceptions - if applicable
                        if (scenario.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule != null)
                        {
                            // Iterate through all of the exceptions, get the ID and map to filename
                            for (int j = 0; j < scenario.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule.Length; j++)
                            {
                                exceptionID = scenario.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule[j].AllowRuleID;
                                exceptionList += String.Format("{0}, ", exceptionID);

                                if (!fileExceptionsDict.ContainsKey(exceptionID))
                                {
                                    fileExceptionsDict.Add(scenario.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule[j].AllowRuleID, String.Empty);
                                }
                            }
                        }

                        fileAttrList = string.Empty;

                        // Add Common Name (Cert Publisher value)
                        if (!String.IsNullOrEmpty(signersDict[signerID].CommonName))
                        {
                            fileAttrList += String.Format("CN = {0}; ", signersDict[signerID].CommonName);
                        }

                        // Add CertOemId (OPUS) field
                        if(!String.IsNullOrEmpty(signersDict[signerID].CertOemID))
                        {
                            fileAttrList += String.Format("CertOemId = {0}; ", signersDict[signerID].CertOemID);
                        }

                        // Get associated/affected files -- FileAttributes
                        foreach (var fileRef in signersDict[signerID].FileAttribRefs)
                        {
                            fileAttrList += String.Format("{0}, ", fileRef);
                        }

                        if(!String.IsNullOrEmpty(fileAttrList))
                        {
                            fileAttrList = fileAttrList.Remove(fileAttrList.Length - 2);
                        }

                        // Remove trailing comma and whitespace on list of Exceptions
                        if (!String.IsNullOrEmpty(exceptionList))
                        {
                            exceptionList = exceptionList.Remove(exceptionList.Length - 2);
                        }

                        displayObjects.Add(new DisplayObject(action, level, friendlyName, fileAttrList, exceptionList, signerID));
                        rulesDataGrid.RowCount += 1;
                    }
                }
            } // end of scenarios

             // Write all "File Rules" rules
            if (Policy.siPolicy.FileRules != null)
            {
                var fileRulesList = Policy.siPolicy.FileRules;
                string fileRuleID = String.Empty;
                string filePath = String.Empty;
                byte[] hash = new byte[0];
                string fileName = String.Empty; 
                string productName = String.Empty;
                string fileDescription = String.Empty;
                string internalName = String.Empty;
                string packageFamilyName = String.Empty; 

                for (int i = 0; i < fileRulesList.Length; i++)
                {
                    var fileRule = fileRulesList[i];

                    if (fileRule.GetType() == typeof(Deny))
                    {
                        action = "Deny";
                        fileRuleID = ((Deny)fileRule).ID;
                        friendlyName = ((Deny)fileRule).FriendlyName;
                        fileAttrList = ((Deny)fileRule).FileName;

                        // 
                        filePath = ((Deny)fileRule).FilePath;
                        hash = ((Deny)fileRule).Hash;
                        fileName = ((Deny)fileRule).FileName;
                        productName = ((Deny)fileRule).ProductName;
                        fileDescription = ((Deny)fileRule).FileDescription;
                        internalName = ((Deny)fileRule).InternalName;
                        packageFamilyName = ((Deny)fileRule).PackageFamilyName; 

                    }
                    else if(fileRule.GetType() == typeof(Allow))
                    {
                        action = "Allow";
                        fileRuleID = ((Allow)fileRule).ID;
                        friendlyName = ((Allow)fileRule).FriendlyName;
                        fileAttrList = ((Allow)fileRule).FileName;

                        filePath = ((Allow)fileRule).FilePath;
                        hash = ((Allow)fileRule).Hash;
                        fileName = ((Allow)fileRule).FileName;
                        productName = ((Allow)fileRule).ProductName;
                        fileDescription = ((Allow)fileRule).FileDescription;
                        internalName = ((Allow)fileRule).InternalName;
                        packageFamilyName = ((Allow)fileRule).PackageFamilyName;
                    }

                    else
                    {
                        // Do nothing for FileAttribute rows
                        continue; 
                    }


                    // Determine the filerule type - Hash, FilePath, FileAttribute (Name, Product Name, Original FileNme)

                    if (hash != null)
                    {
                        // If this is a hash rule, only show the one SHA256 Hash to the user. Easier to remove from table if they are to delete the rule
                        level = "Hash";
                        //if (!friendlyName.Contains("Hash Page Sha256"))
                        //{
                        //    continue; 
                        //}
                    }

                    // Filepath Rule
                    else if (filePath != null)
                    {
                        level = "FilePath";
                        fileAttrList = "Filepath: " + filePath;

                        // Fix issue where file path not in friendly name introduced in version 2.2 of the Wizard by appending filepath
                        // FriendlyName="Allow by path: "
                        // FriendlyName = "Deny by path: "
                        if(friendlyName == Properties.Resources.TruncatedPathAllowFriendlyName
                            || friendlyName == Properties.Resources.TruncatedPathDenyFriendlyName)
                        {
                            friendlyName += filePath; 
                        }
                    }

                    // Packaged App Rule
                    else if (packageFamilyName != null)
                    {
                        level = "Package Name";
                        fileAttrList = "Packaged Family Name (PFN): " + packageFamilyName;
                    }

                    // Allow/Deny with File attributes rules
                    // I.e. no publisher information
                    else
                    {
                        level = "FileAttributes";

                        // Precede the friendlyName with the Original FileName sub-level 
                        if (fileName != null) 
                        { 
                            fileAttrList += String.Format("FileName: {0}, ", fileName);
                        }

                        if (productName != null)
                        {
                            fileAttrList = String.Format("ProductName: {0}, ", productName);
                        }

                        if (fileDescription != null)
                        {
                            fileAttrList = String.Format("FileDescription: {0}, ", fileDescription);
                        }

                        if (internalName != null)
                        {
                            fileAttrList = String.Format("InternalName: {0}, ", friendlyName);
                        }

                        // Remove trailing comma and whitespace on list of FileAttributes
                        if(!String.IsNullOrEmpty(fileAttrList))
                        {
                            char[] trimChars = { ',', ' ' };
                            fileAttrList = fileAttrList.TrimEnd(trimChars);
                        }

                        // Remove trailing comma and whitespace on list of Exceptions
                        if (!String.IsNullOrEmpty(exceptionList))
                        {
                            exceptionList = exceptionList.Remove(exceptionList.Length - 2);
                        }
                    }

                    // Only display if ID not found in the fileExceptionsDict -- in otherwords, this is a file rule NOT an exception
                    if (!fileExceptionsDict.ContainsKey(fileRuleID))
                    {
                        displayObjects.Add(new DisplayObject(action, level, friendlyName, fileAttrList, exceptionList, fileRuleID));
                        rulesDataGrid.RowCount += 1;
                    }
                }
            }

            // Show any AppId Tags
            if(Policy.siPolicy.PolicyType == global::PolicyType.AppIDTaggingPolicy)
            {
                foreach (SigningScenario scenario in Policy.siPolicy.SigningScenarios)
                {
                    if (scenario.AppIDTags != null)
                    {
                        foreach (var tag in scenario.AppIDTags.AppIDTag)
                        {

                            displayObjects.Add(new DisplayObject("",
                                                                      "AppID Tag",
                                                                      "Key: " + tag.Key,
                                                                      "Value: " + tag.Value,
                                                                      ""));
                            rulesDataGrid.RowCount += 1;

                        }
                    }
                }
            }

           // Finally, show any COM Object rules
           if (Policy.siPolicy.Settings != null)
            {
                foreach(var setting in Policy.siPolicy.Settings)
                {
                    // Don't show Policy.Id or Policy.Name settings
                    if (!(setting.Provider == "PolicyInfo"
                        && (setting.ValueName == "Name" || setting.ValueName == "Id")))
                    {
                        displayObjects.Add(new DisplayObject(setting.Value.Item.ToString() == "True" ? "Allow" : "Deny",
                                                                  "COM Object",
                                                                  "Provider: " + setting.Provider,
                                                                  "Key: " + setting.Key,
                                                                  ""));
                        rulesDataGrid.RowCount += 1;
                    }
                }
            }
        }


        /// <summary>
        /// Method to parse either the template or supplemental policy and store into the custom data structures for digestion. 
        /// </summary>
        private bool ReadSetRules(object sender, EventArgs e)
        {
            // Always going to have to parse an XML file:
            // Edit Policies - Read from the copy of the policy under edit (stored under /Temp/ as Policy.TemplatePath)
            // New Policies
            //     - Read from Template path if Base Policy
            //     - Read from Template path if AppId Tagging Policy
            //     - Read from Empty Supplemental if Supplemental Policy i.e. no rules to show

            if (_MainWindow.Policy.PolicyWorkflow == WDAC_Policy.Workflow.Edit)
            {
                XmlPath = _MainWindow.Policy.TemplatePath; // existing policy - read from temp dir path
            }
            else // New Policy
            {
                // Base Policy - Read from Template Path
                if(_MainWindow.Policy._PolicyType == WDAC_Policy.PolicyType.BasePolicy
                    || _MainWindow.Policy._PolicyType == WDAC_Policy.PolicyType.AppIdTaggingPolicy)
                {
                    XmlPath = _MainWindow.Policy.TemplatePath; 
                }
                
                // Supplemental Policy - Read from Empty_Supplemental.xml
                else
                {
                    XmlPath = Path.Combine(_MainWindow.ExeFolderPath, "Templates", Properties.Resources.EmptyWdacSupplementalXml);
                }
            }
                
            Logger.Log.AddInfoMsg("--- Reading Set Signing Rules Beginning ---");
            Logger.Log.AddInfoMsg("Reading file rules from path: " + XmlPath); 

            try
            {
                // Read File
                Policy.siPolicy = Helper.DeserializeXMLtoPolicy(XmlPath); 
            } 
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg("ReadSetRules() has encountered an error: ", exp);
                // Prompt user for additional confirmation
                DialogResult res = MessageBox.Show("The Wizard is unable to read your CI policy xml file. The policy XML is corrupted. ",
                                                    "Parsing Error", 
                                                    MessageBoxButtons.OK, 
                                                    MessageBoxIcon.Error);

                if (res == DialogResult.OK)
                {
                    _MainWindow.ResetWorkflow(sender, e);
                }
                return false; 
            }
            
            BubbleUp(); // all original signing rules are set in MainWindow object - ...
                        //all mutations to rules are from here on completed using cmdlets
            return true; 
        }

        /// <summary>
        /// Method to check the Settings defined by the user for the initial conditions on the recommended blocklist checkboxes. 
        /// </summary>
        private void SetBlocklistStates()
        {
            // Determine whether to show the checkboxes - hide if new/editing a supplemental policy
            if(Policy._PolicyType == WDAC_Policy.PolicyType.SupplementalPolicy
                || Policy._PolicyType == WDAC_Policy.PolicyType.AppIdTaggingPolicy)
            {
                checkBox_KernelList.Visible = false;
                checkBox_UserModeList.Visible = false;
                return; 
            }
            else
            {
                checkBox_KernelList.Visible = true;
                checkBox_UserModeList.Visible = true;
            }

            // Recommended Kernel Driver Blocklist
            if(Properties.Settings.Default.useDriverBlockRules)
            {
                checkBox_KernelList.Checked = true; 
            }
            else
            {
                checkBox_KernelList.Checked = false; 
            }

            // Recommended User Mode Blocklist
            if (Properties.Settings.Default.useUsermodeBlockRules)
            {
                checkBox_UserModeList.Checked = true;
            }
            else
            {
                checkBox_UserModeList.Checked = false;
            }
        }

        /// <summary>
        /// Method to set all of the MainWindow objects to the local instances of the Policy helper class objects.
        /// </summary>
        private void BubbleUp()
        {
            // Passing rule, signing scenarios, etc datastructs to MainWindow class
           _MainWindow.Policy.CISigners = Policy.CISigners;
           _MainWindow.Policy.EKUs = Policy.EKUs;
           _MainWindow.Policy.FileRules = Policy.FileRules;
           _MainWindow.Policy.Signers = Policy.Signers;
           _MainWindow.Policy.SigningScenarios = Policy.SigningScenarios;
           _MainWindow.Policy.UpdateSigners = Policy.UpdateSigners;
           _MainWindow.Policy.SupplementalSigners = Policy.SupplementalSigners;
           _MainWindow.Policy.CISigners = Policy.CISigners;
           _MainWindow.Policy.PolicySettings = Policy.PolicySettings;
           _MainWindow.Policy.CustomRules = Policy.CustomRules;

        }

        /// <summary>
        /// Removes the highlighted rule row in the this.rulesDataGrid DataGridView. 
        /// Can only be executed on custom rules from this session. 
        /// </summary>
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            Logger.Log.AddNewSeparationLine("Delete Rule Button Clicked");

            // Determine whether the user is deleting one row or multiple rows
            int numRowsSelected = rulesDataGrid.SelectedRows.Count;
            string userPromptMsg; 

            if(numRowsSelected < 1)
            {
                return; 
            }
            else if(numRowsSelected == 1)
            {
                // Exactly one row/rule to delete
                int rowIdx = rulesDataGrid.SelectedRows[0].Index;
                string type = (String)rulesDataGrid["Column_Action", rowIdx].Value;
                string level = (String)rulesDataGrid["Column_Level", rowIdx].Value;

                // Assert cannot delete the 'empty' bottom row
                if (String.IsNullOrEmpty(type)
                    && String.IsNullOrEmpty(level))
                {
                    return; 
                }

                userPromptMsg = String.Format("Are you sure you want to delete this rule?\n'{0}'", (String)rulesDataGrid["Column_Name", rowIdx].Value);
            }
            else
            {
                // Multiple rows - show different UI
                userPromptMsg = String.Format("Are you sure you want to delete these {0} rules?", numRowsSelected.ToString());
            }

            // Prompt the user for additional deletion confirmation
            DialogResult res = MessageBox.Show(userPromptMsg, 
                                                "Rule Deletion Confirmation", 
                                                MessageBoxButtons.YesNo, 
                                                MessageBoxIcon.Question);

            if (res == DialogResult.No)
            {
                Logger.Log.AddInfoMsg(Properties.Resources.DeleteRowsCanceled);
                return;
            }

            // Creates background worker to display updates to UI
            panel_Progress.Visible = true;
            panel_Progress.BringToFront(); 

            if (!backgroundWorkerRulesDeleter.IsBusy)
            {
                backgroundWorkerRulesDeleter.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Deletes all rules defined in rowIdxs by calling DeleteRowFromTable per row index
        /// </summary>
        /// <param name="rowIdxs"></param>
        private void DeleteRowsFromRulesTable(List<int> rowIdxs)
        {
            foreach(int rowIdx in rowIdxs)
            {
                DeleteRowFromRulesTable(rowIdx);
            }
        }

        /// <summary>
        /// Delete rule and row (where applicable)
        /// </summary>
        /// <param name="rowIdx"></param>
        private void DeleteRowFromRulesTable(int rowIdx)
        {
            int numIdex = 0;
            int customRuleIdx = -1;
            List<string> ruleIDsToRemove = new List<string>();

            if (rowIdx >= rulesDataGrid.RowCount) return; 

            string type = (String)rulesDataGrid["Column_Action", rowIdx].Value;
            string level = (String)rulesDataGrid["Column_Level", rowIdx].Value;

            // Assert cannot delete the 'empty' bottom row
            if (String.IsNullOrEmpty(type)
                && String.IsNullOrEmpty(level))
            {
                return;
            }

            string ruleName = (String)rulesDataGrid["Column_Name", rowIdx].Value;
            string ruleType = (String)rulesDataGrid["Column_Level", rowIdx].Value;
            string ruleId = (String)rulesDataGrid["column_ID", rowIdx].Value;
            string ruleKey = (String)rulesDataGrid["Column_Files", rowIdx].Value;

            Logger.Log.AddInfoMsg(String.Format("Removing Row: {0} with Name: {1} and ID: {2}", rowIdx.ToString(), ruleName, ruleId)); 

            // Remove from table iff sucessful re-serialization
            // Remove from DisplayObject
            if (rowIdx < displayObjects.Count)
            {
                if (InvokeRequired)
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        displayObjects.RemoveAt(rowIdx);
                        rulesDataGrid.Rows.RemoveAt(rowIdx);
                    }));
                }
                else
                {
                    displayObjects.RemoveAt(rowIdx);
                    rulesDataGrid.Rows.RemoveAt(rowIdx);
                }
            }

            // Check if this is a custom rule that we can delete from memory without modifying the policy
            if (String.IsNullOrEmpty(ruleId))
            {
                if (Policy.CustomRules.Count > 0)
                {
                    foreach (var customRule in Policy.CustomRules)
                    {
                        if (customRule.RowNumber == rowIdx)
                        {
                            customRuleIdx = numIdex; // = this.Policy.CustomRules.Where((val, idx) => idx != numIdex).ToArray();
                            Logger.Log.AddInfoMsg(String.Format("Removing custom rule - {0}", customRule));
                            break;
                        }
                        else
                        {
                            numIdex++;
                        }
                    }

                    // Check if we assigned a value to custom rule indx to remove
                    if (customRuleIdx != -1)
                    {
                        Policy.CustomRules.RemoveAt(customRuleIdx);
                        return;
                    }
                }

                // Handle COM object deletion from xml policy under edit
                if (ruleType == "COM Object")
                {
                    string key = ruleKey.Split(':')[1].Trim();
                    string provider = ruleName.Split(':')[1].Trim();
                    int settingIdx = -1;
                    int numIdx = 0;

                    // Find matching COM object rule
                    foreach (var comRule in Policy.siPolicy.Settings)
                    {
                        if (comRule.Provider == provider && comRule.Key == key)
                        {
                            settingIdx = numIdx; // = this.Policy.CustomRules.Where((val, idx) => idx != numIdex).ToArray();
                            Logger.Log.AddInfoMsg(String.Format("Removing COM rule - {0}.{1}", provider, key));
                            break;
                        }
                        else
                        {
                            numIdx++;
                        }
                    }

                    // Check if we assigned a value to settingIdx, remove that COM rule from the Settings[] array
                    if (settingIdx != -1)
                    {
                        Setting[] tempSettings = Policy.siPolicy.Settings;
                        // Move all indices to the left 1 starting at settingIdx. Finish by resizing the Settings array
                        for (int i = settingIdx; i < tempSettings.Length - 1; i++)
                        {
                            tempSettings[i] = tempSettings[i + 1];
                        }
                        Array.Resize(ref tempSettings, tempSettings.Length - 1);
                        Policy.siPolicy.Settings = tempSettings;
                    }
                }

                // Handle AppID Tags
                if(ruleType == "AppID Tag")
                {
                    string value = ruleKey.Split(':')[1].Trim();
                    string key = ruleName.Split(':')[1].Trim();
                    int tagIdx = -1;
                    int numIdx = 0;

                    foreach (var scenario in Policy.siPolicy.SigningScenarios)
                    {
                        if(scenario.AppIDTags != null)
                        {
                            foreach (var tag in scenario.AppIDTags.AppIDTag)
                            {
                                if(tag.Value == value && tag.Key == key)
                                {
                                    tagIdx = numIdx;
                                    Logger.Log.AddInfoMsg($"Removing AppID Tag - Key: {key}, Value:{value}"); 
                                    break;
                                }
                                else
                                {
                                    numIdx++;
                                }
                            }

                            // Check if we assigned a value to settingIdx, remove that AppID Tag from the AppIDTags[] array
                            if (tagIdx != -1)
                            {
                                AppIDTag[] tempTags = Policy.siPolicy.SigningScenarios[0].AppIDTags.AppIDTag; 

                                if(tempTags.Length == 1)
                                {
                                    Policy.siPolicy.SigningScenarios[0].AppIDTags = null;
                                }

                                else
                                {
                                    // Move all indices to the left 1 starting at settingIdx. Finish by resizing the Settings array
                                    for (int i = tagIdx; i < tempTags.Length - 1; i++)
                                    {
                                        tempTags[i] = tempTags[i + 1];
                                    }
                                    Array.Resize(ref tempTags, tempTags.Length - 1);
                                    Policy.siPolicy.SigningScenarios[0].AppIDTags.AppIDTag = tempTags;
                                }
                            }
                        }
                    }
                }

            }

            // Not a custom rule -- Try to remove from signers -- 
            // use ID to remove from scenarios (Allowed/Denied signer)
            else if (ruleType.Equals("Publisher"))
            {
                numIdex = 0;

                foreach (var signer in Policy.siPolicy.Signers)
                {
                    if (signer.ID.Equals(ruleId))
                    {
                        Policy.siPolicy.Signers = Policy.siPolicy.Signers.Where((val, idx) => idx != numIdex).ToArray();
                        Logger.Log.AddInfoMsg(String.Format("Removing {0} from siPolicy.signers", signer.ID));

                        // Remove the signer from Signing Scenarios
                        RemoveSignerIdFromSigningScenario(signer.ID);

                        // Remove from CiSigners, if applicable
                        RemoveSignerIdFromCiSigners(signer.ID);

                        // Remove any associted FileAttributeRef refrences
                        if (signer.FileAttribRef != null)
                        {
                            foreach (var fileAttrib in signer.FileAttribRef)
                            {
                                RemoveRuleIdFromFileAttribs(fileAttrib.RuleID);
                            }
                        }
                        break;

                    }
                    else
                    {
                        numIdex++;
                    }
                }
            }

            else if (ruleType.Equals("Hash"))
            {
                Logger.Log.AddInfoMsg("Removing Hash Rule");

                numIdex = 0;
                string friendlyName = String.Empty;
                string fileRuleID = String.Empty;
                byte[] hash = new byte[0];

                // Delete all 4 hash rules (sha1, sha256, page, np) from FileRules area
                foreach (var fileRule in Policy.siPolicy.FileRules)
                {
                    if (fileRule.GetType() == typeof(Deny))
                    {
                        fileRuleID = ((Deny)fileRule).ID;
                        friendlyName = ((Deny)fileRule).FriendlyName;
                        hash = ((Deny)fileRule).Hash;
                    }
                    else if (fileRule.GetType() == typeof(Allow))
                    {
                        fileRuleID = ((Allow)fileRule).ID;
                        friendlyName = ((Allow)fileRule).FriendlyName;
                        hash = ((Allow)fileRule).Hash;
                    }
                    else
                    {
                        numIdex++;
                        continue;
                    }

                    if (fileRuleID.Equals(ruleId)) // then delete from policy
                    {
                        Policy.siPolicy.FileRules = Policy.siPolicy.FileRules.Where((val, idx) => idx != numIdex).ToArray();
                        ruleIDsToRemove.Add(fileRuleID);
                        break;
                    }
                    else
                    {
                        numIdex++;
                    }
                }

                // Remove rule from signer/scenario fields
                foreach (var ruleIDtoRemove in ruleIDsToRemove)
                {
                    RemoveRuleIdFromFileAttribs(ruleIDtoRemove);
                }
            }

            // Non-hash File Rules can simply be removed from the Signing Scenarios and FileRulesRef
            else
            {
                // Remove from FileRules
                numIdex = 0;
                foreach (var fileRule in Policy.siPolicy.FileRules)
                {
                    string fileRuleID = string.Empty;

                    if (fileRule.GetType() == typeof(Deny))
                    {
                        fileRuleID = ((Deny)fileRule).ID;
                    }
                    else if (fileRule.GetType() == typeof(Allow))
                    {
                        fileRuleID = ((Allow)fileRule).ID;
                    }
                    else
                    {
                        fileRuleID = ((FileAttrib)fileRule).ID;
                    }


                    if (fileRuleID.Equals(ruleId))
                    {
                        Policy.siPolicy.FileRules = Policy.siPolicy.FileRules.Where((val, idx) => idx != numIdex).ToArray();
                        Logger.Log.AddInfoMsg("Removing from siPolicy.signers");
                        break;
                    }
                    else
                    {
                        numIdex++;
                    }
                }

                // Remove from Signing Scenario
                RemoveRuleIdFromFileAttribs(ruleId);
            }

            // Serialize to new policy
            try
            {
                Helper.SerializePolicytoXML(Policy.siPolicy, XmlPath);
            }
            catch (Exception exp)
            {
                Logger.Log.AddErrorMsg("Serialization failed after removing rule with error: ", exp);
                return;
            }
        }

        /// <summary>
        /// Helper function which removes a signer rule ID from all signing scenarios
        /// </summary>
        /// <param name="ruleId"></param>
        private void RemoveSignerIdFromSigningScenario(string signerId)
        {
            foreach (var scenario in Policy.siPolicy.SigningScenarios)
            {
                // Check Allowed Signers
                int numIdex = 0;
                if (scenario.ProductSigners.AllowedSigners != null)
                {
                    foreach (var allowedSigner in scenario.ProductSigners.AllowedSigners.AllowedSigner)
                    {
                        if (allowedSigner.SignerId.Equals(signerId))
                        {
                            scenario.ProductSigners.AllowedSigners.AllowedSigner = scenario.ProductSigners.AllowedSigners.AllowedSigner
                                .Where((val, idx) => idx != numIdex).ToArray();
                            Logger.Log.AddInfoMsg(String.Format("Removing {0} from AllowedSigners", allowedSigner.SignerId.ToString()));

                            // If removing the last AllowedSigner, set AllowedSigners to null so serialization is successful
                            if (scenario.ProductSigners.AllowedSigners.AllowedSigner.Length == 0)
                            {
                                scenario.ProductSigners.AllowedSigners = null;
                            }
                            break;
                        }

                        else
                            numIdex++;
                    }
                }

                // Check Denied Signers
                numIdex = 0;
                if (scenario.ProductSigners.DeniedSigners != null)
                {
                    foreach (var deniedSigner in scenario.ProductSigners.DeniedSigners.DeniedSigner)
                    {
                        if (deniedSigner.SignerId.Equals(signerId))
                        {
                            scenario.ProductSigners.DeniedSigners.DeniedSigner = scenario.ProductSigners.DeniedSigners.DeniedSigner
                                .Where((val, idx) => idx != numIdex).ToArray();
                            Logger.Log.AddInfoMsg(String.Format("Removing {0} from DeniedSigners", deniedSigner.SignerId.ToString()));

                            if (scenario.ProductSigners.DeniedSigners.DeniedSigner.Length == 0)
                            {
                                scenario.ProductSigners.DeniedSigners = null;
                            }
                            break;
                        }

                        else
                            numIdex++;
                    }
                }
            }
        }

        /// <summary>
        /// Helper function which removes a signer rule ID from the Ci Signers section
        /// </summary>
        /// <param name="ruleId"></param>
        private void RemoveSignerIdFromCiSigners(string signerId)
        {
            // Verify that CiSigners is not null. Few policies have a CiSigners section
            if(Policy.siPolicy.CiSigners == null)
            {
                return; 
            }

            int numIdex = 0;

            // Remove the SignerId reference from the CiSigners section
            foreach (CiSigner ciSigner in Policy.siPolicy.CiSigners)
            {
                if (ciSigner.SignerId == signerId)
                {
                    Policy.siPolicy.CiSigners = Policy.siPolicy.CiSigners
                        .Where((val, idx) => idx != numIdex).ToArray();
                    
                    // If removed the last CiSigners ref, set the CiSigners to null to serialize
                    if (Policy.siPolicy.CiSigners.Length == 0)
                    {
                        Policy.siPolicy.CiSigners = null;
                    }

                    Logger.Log.AddInfoMsg(String.Format("Removing {0} from CiSigners", signerId));
                    break;
                }
                else
                {
                    numIdex++;
                }
            }
        }

        /// <summary>
        /// Helper function which removes a rule ID from FileRules ref in the signing scenarios
        /// </summary>
        /// <param name="ruleId"></param>
        private void RemoveRuleIdFromFileAttribs(string ruleId)
        {
            int numIdex = 0; 

            if (Policy.siPolicy.SigningScenarios == null)
                return;

            foreach (var scenario in Policy.siPolicy.SigningScenarios)
            {
                numIdex = 0;

                if (scenario.ProductSigners.FileRulesRef == null)
                    continue;

                foreach (var fileRef in scenario.ProductSigners.FileRulesRef.FileRuleRef)
                {
                    if (fileRef.RuleID.Equals(ruleId))
                    {
                        scenario.ProductSigners.FileRulesRef.FileRuleRef = scenario.ProductSigners.FileRulesRef.FileRuleRef.
                            Where((val, idx) => idx != numIdex).ToArray();
                        Logger.Log.AddInfoMsg(String.Format("Removing fileRef ID: {0}", ruleId));

                        if(scenario.ProductSigners.FileRulesRef.FileRuleRef.Length == 0)
                        {
                            scenario.ProductSigners.FileRulesRef = null; 
                        }
                        break;
                    }

                    else
                        numIdex++;
                }
            }
        }

        /// <summary>
        /// Sets the display object when the DataGridView needed to paint data
        /// </summary>
        private void RulesDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            // If this is the row for new records, no values are needed.
            if (e.RowIndex == rulesDataGrid.RowCount - 1) return;

            DisplayObject displayObject = null;

            // Store a reference to the Customer object for the row being painted.
            if (e.RowIndex == rowInEdit)
            {
                displayObject = displayObjectInEdit;
            }
            else
            {
                if(e.RowIndex  < displayObjects.Count)
                {
                    displayObject = (DisplayObject)displayObjects[e.RowIndex];
                }
                else
                {
                    return;
                }
            }

            // Set the cell value to paint using the Customer object retrieved.
            switch (rulesDataGrid.Columns[e.ColumnIndex].Name)
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

                case "column_ID":
                    e.Value = displayObject.Id;
                    break; 
            }
        }

        /// <summary>
        /// Adds a new rule to the DataGrid Table
        /// </summary>
        /// <param name="displayObjectArray"></param>
        /// <param name="customRule"></param>
        /// <param name="warnUser"></param>
        public void AddRuleToTable(string [] displayObjectArray, PolicyCustomRules customRule, bool warnUser)
        {
            // Attach the int row number we added it to
            customRule.RowNumber = rulesDataGrid.RowCount - 1;
            string action = displayObjectArray[0]; 
            string level = displayObjectArray[1];
            string name = warnUser ? "*Hash Fallback Possible* " + displayObjectArray[2] : displayObjectArray[2];
            string files = displayObjectArray[3];
            string exceptions = displayObjectArray[4];

            // Add to the DisplayObject
            displayObjects.Add(new DisplayObject(action, level, name, files, exceptions));
            rulesDataGrid.RowCount += 1;

            // Add custom list to RulesList
            Policy.CustomRules.Add(customRule);

            // Scroll to bottom to see new rule added to list
            rulesDataGrid.FirstDisplayedScrollingRowIndex = rulesDataGrid.RowCount - 1;

            BubbleUp();

            // close the custom Rule Conditions Panel
            customRuleConditionsPanel.Close();
            customRuleConditionsPanel = null;
        }

        /// <summary>
        /// Nullifies the custom rule conditions panel on form closing
        /// </summary>
        public void CustomRulesPanel_Closing()
        {
            // User has closed custom rules panel. Reset panel and text
            customRuleConditionsPanel = null;
            // this.label_AddCustomRules.Text = "+ Custom Rules"; 
        }

        /// <summary>
        /// Closes the CustomRules Panel
        /// </summary>
        public void CloseCustomRulesPanel()
        {
            if (customRuleConditionsPanel == null)
            {
                return;
            }

            // Close the custom Rule Conditions Panel
            // Set RuleInEdit to false to not trigger another confirmation from user on FormClosing()
            customRuleConditionsPanel.RuleInEdit = false; 
            customRuleConditionsPanel.Close();
            customRuleConditionsPanel = null;
            _MainWindow.CustomRuleinProgress = false; 
        }

        /// <summary>
        /// Sets the back color to "highlight" for the checkbox picturebox when the user hovers the mouse over the label (button).  
        /// </summary>
        /// <param name="sender">Sender is the picturebox control </param>
        private void AddCustomRules_MouseHover(object sender, EventArgs e)
        {
            Label checkBox = ((Label)sender);
            checkBox.BackColor = Color.WhiteSmoke;
        }

        /// <summary>
        /// Sets the UseKernelModeBlocks attribute dictated by the state of the checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_KernelList_CheckedChanged(object sender, EventArgs e)
        {
            // If checked, create a policy with the recommended driver block rules
            if(checkBox_KernelList.Checked)
            {
                Policy.UseKernelModeBlocks = true;
            }
            else
            {
                Policy.UseKernelModeBlocks = false;
            }
        }

        /// <summary>
        /// Sets the UseKernelModeBlocks attribute dictated by the state of the checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_UserModeList_CheckedChanged(object sender, EventArgs e)
        {
            // If checked, create a policy with the recommended user mode block rules
            if (checkBox_UserModeList.Checked)
            {
                Policy.UseUserModeBlocks = true;
            }
            else
            {
                Policy.UseUserModeBlocks = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoWorkBackgroundWorker(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            //
            List<int> rowIdxs = new List<int>();
            int numRowsSelected = rulesDataGrid.SelectedRows.Count;

            for (int i = 0; i < numRowsSelected; i++)
            {
                rowIdxs.Add(rulesDataGrid.SelectedRows[i].Index);
            }

            // Call helper function to delete all the rows defined in rowIdxs
            DeleteRowsFromRulesTable(rowIdxs);
        }


        /// <summary>
        /// Background worker is done computationally expensive work. Hides the progress panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinishedBackgroundWorker(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            // Hide progress panel
            panel_Progress.Visible = false;
            panel_Progress.SendToBack(); 
        }

        /// <summary>
        /// Form painting. Occurs on Form.Refresh, Load and Focus. 
        /// Used for UI element changes for Dark and Light Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SigningRules_Control_Validated(object sender, EventArgs e)
        {
            // Set Controls Color (e.g. Panels)
            SetControlsColor();

            // Set Labels Color
            List<Label> labels = new List<Label>();
            GetLabelsRecursive(this, labels);
            SetLabelsColor(labels);

            // Set PolicyType Form back color
            SetFormBackColor();

            // Set Grid Colors
            SetGridColors();

            // Repaint the Custom Rules Condition Panel, if applicable
            if (customRuleConditionsPanel != null)
            {
                customRuleConditionsPanel.ForceRepaint();
            }
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
                        button.FlatAppearance.BorderColor = Color.DodgerBlue;
                        button.FlatAppearance.BorderSize = 0;
                        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(50,30,144,255);
                        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(50,30,144,255);
                        button.FlatStyle = FlatStyle.Flat;
                        button.ForeColor = Color.DodgerBlue;
                        button.BackColor = Color.Transparent;
                    }

                    // Panels
                    else if(control is Panel panel
                        && (panel.Tag == null || panel.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        panel.ForeColor = Color.White;
                        panel.BackColor = Color.FromArgb(15, 15, 15);
                    }

                    // Checkboxes
                    else if(control is CheckBox checkBox
                        && (checkBox.Tag == null || checkBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        checkBox.ForeColor = Color.White;
                        checkBox.BackColor = Color.FromArgb(15, 15, 15);
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
                        button.FlatAppearance.BorderColor = Color.Black;
                        button.FlatAppearance.BorderSize = 0;
                        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(50,30,144,255);
                        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(50,30,144,255);
                        button.FlatStyle = FlatStyle.Flat;
                        button.ForeColor = Color.FromArgb(16, 110, 190);
                        button.BackColor = Color.Transparent;
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
                        label.BackColor = Color.FromArgb(15,15,15);
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
        /// Set the Rules Grid colors for Dark and Light Mode
        /// </summary>
        private void SetGridColors()
        {
            // Set the Rules Grid colors for Light and Dark Mode 

            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                rulesDataGrid.BackgroundColor = Color.FromArgb(15, 15, 15);

                // Header
                rulesDataGrid.RowHeadersDefaultCellStyle.BackColor = Color.Black;
                rulesDataGrid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                rulesDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
                rulesDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                rulesDataGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 82, 118);
                rulesDataGrid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

                // Borders
                rulesDataGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                rulesDataGrid.BorderStyle = BorderStyle.Fixed3D;
                rulesDataGrid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

                // Cells
                rulesDataGrid.DefaultCellStyle.BackColor = Color.FromArgb(32, 32, 32);
                rulesDataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 24);
                rulesDataGrid.DefaultCellStyle.ForeColor = Color.White;
                rulesDataGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 82, 118);
                rulesDataGrid.DefaultCellStyle.SelectionForeColor = Color.White;

                // Grid lines
                rulesDataGrid.GridColor = Color.LightSlateGray;
            }

            // Light Mode
            else
            {
                rulesDataGrid.BackgroundColor = Color.White;

                // Header
                rulesDataGrid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
                rulesDataGrid.RowHeadersDefaultCellStyle.ForeColor = Color.Black;
                rulesDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
                rulesDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                rulesDataGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(174, 214, 241);
                rulesDataGrid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

                // Borders
                rulesDataGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                rulesDataGrid.BorderStyle = BorderStyle.Fixed3D;
                rulesDataGrid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

                // Cells
                rulesDataGrid.DefaultCellStyle.BackColor = Color.White;
                rulesDataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(236, 240, 241);
                rulesDataGrid.DefaultCellStyle.ForeColor = Color.Black;
                rulesDataGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(174, 214, 241);
                rulesDataGrid.DefaultCellStyle.SelectionForeColor = Color.Black;

                // Grid lines
                rulesDataGrid.GridColor = Color.Black;
            }
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
        public string Id; 

        public DisplayObject()
        {
            Action = String.Empty;
            Level = String.Empty;
            Name = String.Empty;
            Files = String.Empty;
            Exceptions = String.Empty;
            Id = String.Empty; 
        }

        public DisplayObject(string action, string level, string name, string files, string exceptions)
        {
            Action = action;
            Level = level;
            Name = name;
            Files = files;
            Exceptions = exceptions;
        }

        public DisplayObject(string action, string level, string name, string files, string exceptions, string id)
        {
            Action = action;
            Level = level;
            Name = name;
            Files = files;
            Exceptions = exceptions;
            Id = id; 
        }
    }
}
