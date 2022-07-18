﻿// Copyright (c) Microsoft Corporation.
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

        private Logger Log;
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
            this.Policy = pMainWindow.Policy; 
            this.AllFilesinFolder = new List<string>(); 

            this._MainWindow = pMainWindow;
            this._MainWindow.RedoFlowRequired = false;
            this._MainWindow.CustomRuleinProgress = false; 
            this.Log = this._MainWindow.Log;
            this.RowSelected = -1;
            this.isCustomPanelOpen = false; 

            this.Log.AddInfoMsg("==== Signing Rules Page Initialized ====");
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
                this.Log.AddErrorMsg("DisplayRules() encountered an exception.", exp);
                DialogResult res = MessageBox.Show("The Wizard is unable to read all the rules in your CI policy xml file. The policy XML is likely corrupted. " +
                    "Try converting the policy to binary to locate the issue in the XML.",
                    "Parsing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            

            // Set recommended blocklist states
            SetBlocklistStates(); 
        }

        
        /// <summary>
        /// Shows the Custom Rules Panel when the user clicks on +Custom Rules. 
        /// </summary>
        private void Label_AddCustomRules_Click(object sender, EventArgs e)
        {
            // Open the custom rules conditions panel

            if (this.customRuleConditionsPanel == null)
            {
                this.customRuleConditionsPanel = new CustomRuleConditionsPanel(this);
                this.customRuleConditionsPanel.Show();
                this.customRuleConditionsPanel.BringToFront();
                this.customRuleConditionsPanel.Focus();

                // this.label_AddCustomRules.Text = "- Custom Rules"; 
                this.isCustomPanelOpen = true; 
            }
            
            this.Log.AddInfoMsg("--- Create Custom Rules Selected ---"); 
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
            foreach (var siPolicySigner in this.Policy.siPolicy.Signers)
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
            foreach (SigningScenario scenario in this.Policy.siPolicy.SigningScenarios)
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

                        this.displayObjects.Add(new DisplayObject(action, level, friendlyName, fileAttrList, exceptionList, signerID));
                        this.rulesDataGrid.RowCount += 1;
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

                        // Get signer exceptions - if applicable
                        if (scenario.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule != null)
                        {
                            // Iterate through all of the exceptions, get the ID and map to filename
                            for (int j = 0; j < scenario.ProductSigners.AllowedSigners.AllowedSigner[i].ExceptDenyRule.Length; j++)
                            {
                                exceptionID = scenario.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule[j].AllowRuleID;
                                exceptionList += String.Format("{0}, ", exceptionID);

                                if (!fileExceptionsDict.ContainsKey(exceptionID))
                                    fileExceptionsDict.Add(scenario.ProductSigners.DeniedSigners.DeniedSigner[i].ExceptAllowRule[j].AllowRuleID, String.Empty);

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

                        this.displayObjects.Add(new DisplayObject(action, level, friendlyName, fileAttrList, exceptionList, signerID));
                        this.rulesDataGrid.RowCount += 1;
                    }
                }
            } // end of scenarios

           
             // Write all "File Rules" rules
            if (this.Policy.siPolicy.FileRules != null)
            {
                var fileRulesList = this.Policy.siPolicy.FileRules;
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

                    else if (filePath != null || fileName != null)
                    {
                        level = "FilePath";
                        fileAttrList = "FileName: " + fileAttrList; 
                    }

                    else if (packageFamilyName != null)
                    {
                        level = "Package Name"; 
                    }

                    else
                    {
                        level = "FileAttributes";

                        // Precede the friendlyName with the Original FileName sub-level 
                        if (fileName != null)
                            friendlyName = String.Format("FileName; {0}", friendlyName);

                        else if (productName != null)
                            friendlyName = String.Format("ProductName; {0}", friendlyName);

                        else if (fileDescription != null)
                            friendlyName = String.Format("FileDescription; {0}", friendlyName);

                        else if (internalName != null)
                            friendlyName = String.Format("InternalName; {0}", friendlyName);

                        else
                            this.Log.AddWarningMsg("DisplayRules() could not detect a sub-level for FileAttributes ");
                    }

                    // Only display if ID not found in the fileExceptionsDict -- in otherwords, this is a file rule NOT an exception
                    if (!fileExceptionsDict.ContainsKey(fileRuleID))
                    {
                        this.displayObjects.Add(new DisplayObject(action, level, friendlyName, fileAttrList, exceptionList, fileRuleID));
                        this.rulesDataGrid.RowCount += 1;
                    }
                }
            }
        }


        /// <summary>
        /// Method to parse either the template or supplemental policy and store into the custom data structures for digestion. 
        /// </summary>
        private bool ReadSetRules(object sender, EventArgs e)
        {
            // Always going to have to parse an XML file - either going to be pre-exisiting policy (edit mode, supplmental policy) or template policy (new base)
            if (this._MainWindow.Policy.TemplatePath != null)
            {
                this.XmlPath = this._MainWindow.Policy.TemplatePath;
            }
            else
            {
                this.XmlPath = this._MainWindow.Policy.EditPolicyPath;
            }
                
            this.Log.AddInfoMsg("--- Reading Set Signing Rules Beginning ---");

            try
            {
                // Read File
                this.Policy.siPolicy = Helper.DeserializeXMLtoPolicy(this.XmlPath); 
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
            
            BubbleUp(); // all original signing rules are set in MainWindow object - ...
                        //all mutations to rules are from here on completed using cmdlets
            return true; 
        }

        /// <summary>
        /// Method to check the Settings defined by the user for the initial conditions on the recommended blocklist checkboxes. 
        /// </summary>
        private void SetBlocklistStates()
        {
            if(Properties.Settings.Default.useDriverBlockRules)
            {
                this.checkBox_KernelList.Checked = true; 
            }
            else
            {
                this.checkBox_KernelList.Checked = false; 
            }

            if (Properties.Settings.Default.useUsermodeBlockRules)
            {
                this.checkBox_UserModeList.Checked = true;
            }
            else
            {
                this.checkBox_UserModeList.Checked = false;
            }
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
        private void BubbleUp()
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
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            this.Log.AddNewSeparationLine("Delete Rule Button Clicked");

            // Determine whether the user is deleting one row or multiple rows
            int numRowsSelected = this.rulesDataGrid.SelectedRows.Count;
            string userPromptMsg; 

            if(numRowsSelected < 1)
            {
                return; 
            }
            else if(numRowsSelected == 1)
            {
                // Exactly one row/rule to delete
                int rowIdx = this.rulesDataGrid.SelectedRows[0].Index;
                string type = (String)this.rulesDataGrid["Column_Action", rowIdx].Value;
                
                // Assert cannot delete the 'empty' bottom row
                if (String.IsNullOrEmpty(type))
                {
                    return; 
                }

                userPromptMsg = String.Format("Are you sure you want to delete this rule?\n'{0}'", (String)this.rulesDataGrid["Column_Name", rowIdx].Value);
            }
            else
            {
                // Multiple rows - show different UI
                userPromptMsg = String.Format("Are you sure you want to delete these {0} rules?", numRowsSelected.ToString());
            }

            // Prompt the user for additional deletion confirmation
            DialogResult res = MessageBox.Show(userPromptMsg, "Rule Deletion Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (res == DialogResult.No)
            {
                this.Log.AddInfoMsg(Properties.Resources.DeleteRowsCanceled);
                return;
            }

            // Creates background worker to display updates to UI
            this.panel_Progress.Visible = true;
            this.panel_Progress.BringToFront(); 

            if (!this.backgroundWorkerRulesDeleter.IsBusy)
            {
                this.backgroundWorkerRulesDeleter.RunWorkerAsync();
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

            string type = (String)this.rulesDataGrid["Column_Action", rowIdx].Value;

            // Assert cannot delete the 'empty' bottom row
            if (String.IsNullOrEmpty(type))
            {
                return;
            }

            string ruleName = (String)this.rulesDataGrid["Column_Name", rowIdx].Value;
            string ruleType = (String)this.rulesDataGrid["Column_Level", rowIdx].Value;
            string ruleId = (String)this.rulesDataGrid["column_ID", rowIdx].Value;

            this.Log.AddInfoMsg(String.Format("Removing Row: {0} with Name: {1} and ID: {2}", rowIdx.ToString(), ruleName, ruleId)); 

            // Remove from table iff sucessful re-serialization
            // Remove from DisplayObject
            if (rowIdx < this.displayObjects.Count)
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        this.displayObjects.RemoveAt(rowIdx);
                        this.rulesDataGrid.Rows.RemoveAt(rowIdx);
                    }));
                }
                else
                {
                    this.displayObjects.RemoveAt(rowIdx);
                    this.rulesDataGrid.Rows.RemoveAt(rowIdx);
                }
            }

            // Check if this is a custom rule that we can delete from memory without modifying the policy
            if (String.IsNullOrEmpty(ruleId))
            {
                if (this.Policy.CustomRules.Count > 0)
                {
                    foreach (var customRule in this.Policy.CustomRules)
                    {
                        if (customRule.RowNumber == rowIdx)
                        {
                            customRuleIdx = numIdex; // = this.Policy.CustomRules.Where((val, idx) => idx != numIdex).ToArray();
                            this.Log.AddInfoMsg(String.Format("Removing custom rule - {0}", customRule));
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
                        this.Policy.CustomRules.RemoveAt(customRuleIdx);
                        return;
                    }
                }
            }

            // Not a custom rule -- Try to remove from signers -- 
            // use ID to remove from scenarios (Allowed/Denied signer)
            if (ruleType.Equals("Publisher"))
            {
                numIdex = 0;

                foreach (var signer in this.Policy.siPolicy.Signers)
                {
                    if (signer.ID.Equals(ruleId))
                    {
                        this.Policy.siPolicy.Signers = this.Policy.siPolicy.Signers.Where((val, idx) => idx != numIdex).ToArray();
                        this.Log.AddInfoMsg(String.Format("Removing {0} from siPolicy.signers", signer.ID));

                        // Remove the signer from Signing Scenarios
                        RemoveSignerIdFromSigningScenario(signer.ID);

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
                this.Log.AddInfoMsg("Removing Hash Rule");

                numIdex = 0;
                string friendlyName = String.Empty;
                string fileRuleID = String.Empty;
                byte[] hash = new byte[0];

                // Delete all 4 hash rules (sha1, sha256, page, np) from FileRules area
                foreach (var fileRule in this.Policy.siPolicy.FileRules)
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
                        continue;
                    }

                    if (fileRuleID.Equals(ruleId)) // then delete from policy
                    {
                        this.Policy.siPolicy.FileRules = this.Policy.siPolicy.FileRules.Where((val, idx) => idx != numIdex).ToArray();
                        ruleIDsToRemove.Add(fileRuleID);
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
                foreach (var fileRule in this.Policy.siPolicy.FileRules)
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
                        this.Policy.siPolicy.FileRules = this.Policy.siPolicy.FileRules.Where((val, idx) => idx != numIdex).ToArray();
                        this.Log.AddInfoMsg("Removing from siPolicy.signers");
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
                Helper.SerializePolicytoXML(this.Policy.siPolicy, this.XmlPath);
            }
            catch (Exception exp)
            {
                this.Log.AddErrorMsg("Serialization failed after removing rule with error: ", exp);
                return;
            }
        }

        /// <summary>
        /// Helper function which removes a signer rule ID from all signing scenarios
        /// </summary>
        /// <param name="ruleId"></param>
        private void RemoveSignerIdFromSigningScenario(string ruleId)
        {
            foreach (var scenario in this.Policy.siPolicy.SigningScenarios)
            {
                // Check Allowed Signers
                int numIdex = 0;
                if (scenario.ProductSigners.AllowedSigners != null)
                {
                    foreach (var allowedSigner in scenario.ProductSigners.AllowedSigners.AllowedSigner)
                    {
                        if (allowedSigner.SignerId.Equals(ruleId))
                        {
                            scenario.ProductSigners.AllowedSigners.AllowedSigner = scenario.ProductSigners.AllowedSigners.AllowedSigner
                                .Where((val, idx) => idx != numIdex).ToArray();
                            this.Log.AddInfoMsg(String.Format("Removing {0} from AllowedSigners", allowedSigner.SignerId.ToString()));

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
                        if (deniedSigner.SignerId.Equals(ruleId))
                        {
                            scenario.ProductSigners.DeniedSigners.DeniedSigner = scenario.ProductSigners.DeniedSigners.DeniedSigner
                                .Where((val, idx) => idx != numIdex).ToArray();
                            this.Log.AddInfoMsg(String.Format("Removing {0} from DeniedSigners", deniedSigner.SignerId.ToString()));

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
        /// Helper function which removes a rule ID from FileRules ref in the signing scenarios
        /// </summary>
        /// <param name="ruleId"></param>
        private void RemoveRuleIdFromFileAttribs(string ruleId)
        {
            int numIdex = 0; 

            if (this.Policy.siPolicy.SigningScenarios == null)
                return;

            foreach (var scenario in this.Policy.siPolicy.SigningScenarios)
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
                        this.Log.AddInfoMsg(String.Format("Removing fileRef ID: {0}", ruleId));

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
            customRule.RowNumber = this.rulesDataGrid.RowCount - 1;
            string action = displayObjectArray[0]; 
            string level = displayObjectArray[1];
            string name = warnUser ? "*Hash* " + displayObjectArray[2] : displayObjectArray[2];
            string files = displayObjectArray[3];
            string exceptions = displayObjectArray[4];

            // Add to the DisplayObject
            this.displayObjects.Add(new DisplayObject(action, level, name, files, exceptions));
            this.rulesDataGrid.RowCount += 1;

            // Add custom list to RulesList
            this.Policy.CustomRules.Add(customRule);

            // Scroll to bottom to see new rule added to list
            this.rulesDataGrid.FirstDisplayedScrollingRowIndex = this.rulesDataGrid.RowCount - 1;

            BubbleUp();

            // close the custom Rule Conditions Panel
            this.customRuleConditionsPanel.Close();
            this.customRuleConditionsPanel = null;
            // this.label_AddCustomRules.Text = "+ Custom Rules";

        }

        /// <summary>
        /// Nullifies the custom rule conditions panel on form closing
        /// </summary>
        public void CustomRulesPanel_Closing()
        {
            // User has closed custom rules panel. Reset panel and text
            this.customRuleConditionsPanel = null;
            // this.label_AddCustomRules.Text = "+ Custom Rules"; 
        }

        /// <summary>
        /// Closes the CustomRules Panel
        /// </summary>
        public void CloseCustomRulesPanel()
        {
            if (this.customRuleConditionsPanel == null)
            {
                return;
            }

            // Close the custom Rule Conditions Panel
            // Set RuleInEdit to false to not trigger another confirmation from user on FormClosing()
            this.customRuleConditionsPanel.RuleInEdit = false; 
            this.customRuleConditionsPanel.Close();
            this.customRuleConditionsPanel = null;
            this._MainWindow.CustomRuleinProgress = false; 
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
            if(this.checkBox_KernelList.Checked)
            {
                this.Policy.UseKernelModeBlocks = true;
            }
            else
            {
                this.Policy.UseKernelModeBlocks = false;
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
            if (this.checkBox_UserModeList.Checked)
            {
                this.Policy.UseUserModeBlocks = true;
            }
            else
            {
                this.Policy.UseUserModeBlocks = false;
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
            int numRowsSelected = this.rulesDataGrid.SelectedRows.Count;

            for (int i = 0; i < numRowsSelected; i++)
            {
                rowIdxs.Add(this.rulesDataGrid.SelectedRows[i].Index);
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
            this.panel_Progress.Visible = false;
            this.panel_Progress.SendToBack(); 
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
            this.Action = String.Empty;
            this.Level = String.Empty;
            this.Name = String.Empty;
            this.Files = String.Empty;
            this.Exceptions = String.Empty;
            this.Id = String.Empty; 
        }

        public DisplayObject(string action, string level, string name, string files, string exceptions)
        {
            this.Action = action;
            this.Level = level;
            this.Name = name;
            this.Files = files;
            this.Exceptions = exceptions;
        }

        public DisplayObject(string action, string level, string name, string files, string exceptions, string id)
        {
            this.Action = action;
            this.Level = level;
            this.Name = name;
            this.Files = files;
            this.Exceptions = exceptions;
            this.Id = id; 
        }
    }

}
