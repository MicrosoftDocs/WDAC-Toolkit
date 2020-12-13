// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


namespace WDAC_Wizard
{
    public partial class EditWorkflow : UserControl
    {
        public string EditPath { get; set; }
        private Logger Log; 
        public string PolicyName { get; set; }
        public string PolicyID { get; set; }
        public List<PolicySettings> Settings { get; set; }
        private MainWindow _MainWindow;
        private WDAC_Policy _Policy;


        public EditWorkflow(MainWindow pMainWindow)
        {
            InitializeComponent();
            this.PolicyID = "N/A";
            this.PolicyName = "N/A";
            this.Settings = new List<PolicySettings>();
            pMainWindow.Policy._PolicyType = WDAC_Policy.PolicyType.Edit; 

            this._MainWindow = pMainWindow;
            this._MainWindow.ErrorOnPage = false;
            this._MainWindow.RedoFlowRequired = false;
            
            this.Log = this._MainWindow.Log;
            this.Log.AddInfoMsg("==== Edit Workflow Page Initialized ====");

        }

        /// <summary>
        /// Sets the Edit Policy Path parameter of the policy object. Launches the OpenFileDialog so the user can 
        /// select the Policy they would like to edit. 
        /// </summary>
        private void button_Create_Click(object sender, EventArgs e)
        {
            // If user is changing the policy schema being edited, show message
            if(this._MainWindow.PageList.Count > 1)
            {
                DialogResult res = MessageBox.Show("Modifying the current schema to edit will cause you to lose your progress." +
                    "Are you sure you want to do this?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (res == DialogResult.Yes)
                    this._MainWindow.RedoFlowRequired = true;
                else
                    return;
            }
                       
            this.Log.AddInfoMsg("Browsing for existing WDAC Policy on file.");

            // Save dialog box pressed
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Browse for existing WDAC Policy File";
            openFileDialog.CheckPathExists = true;
            openFileDialog.DefaultExt = "xml";
            openFileDialog.Filter = "Policy Files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.RestoreDirectory = true;

            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxPolicyPath.Text = openFileDialog.FileName;                   
                    this.EditPath = openFileDialog.FileName;

                    // Parse the policy for its information and display it
                    ParsePolicy(this.EditPath);
                    DisplayPolicyInfo(); 

                    this._MainWindow.Policy.EditPolicyPath = this.EditPath;

                    // If user has returned to this page and updated the policy, must proceed to page 2
                    this._MainWindow.RedoFlowRequired = true; 
                }
            }

            catch(Exception exp)
            {
                this.Log.AddErrorMsg("EditWorkflow Browse() encountered the following error ", exp); 
            }
            openFileDialog.Dispose();
        }

        /// <summary>
        /// Parses the Edit Policy to read in the Policy Settings (end of policy), specifically, policy name and ID. 
        /// </summary>
        private void ParsePolicy(string xmlPath)
        {
            // Parse the 
            this.Settings = new List<PolicySettings>();

            try
            {
                XmlReader xmlReader = new XmlTextReader(xmlPath);
                // Counter for end of element nodes
                int eoeCount;

                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:

                            if (xmlReader.IsEmptyElement) // Handle empty elements eg. FileRules and UpdatePolicySigners in SignedReputable
                                break;

                            switch (xmlReader.Name)
                            {
                                    case "Settings":
                                    {
                                        PolicySettings policySetting = new PolicySettings();
                                        eoeCount = 0;
                                        while (xmlReader.Read() && eoeCount < 3)
                                        {
                                            switch (xmlReader.NodeType)
                                            {
                                                case XmlNodeType.Element:
                                                    eoeCount = 0;
                                                    switch (xmlReader.Name)
                                                    {

                                                        case "Setting":

                                                            policySetting = new PolicySettings();
                                                            policySetting.Provider = xmlReader.GetAttribute("Provider");
                                                            policySetting.Key = xmlReader.GetAttribute("Key");
                                                            policySetting.ValueName = xmlReader.GetAttribute("ValueName");
                                                            break;

                                                        case "String":
                                                            policySetting.ValString = xmlReader.ReadElementContentAsString();
                                                            break;

                                                        case "Boolean":
                                                            policySetting.ValBool = xmlReader.ReadElementContentAsString() == "true";
                                                            break;
                                                    }

                                                    break;

                                                case XmlNodeType.EndElement:
                                                    eoeCount++;
                                                    if (eoeCount == 2)
                                                    {
                                                        this.Settings.Add(policySetting);
                                                        this.Log.AddInfoMsg(String.Format("Existing Setting Added - Provider: {0},  Key: {1}, Value Name: {2}, String: {3}, Bool: {4}",
                                                            policySetting.Provider, policySetting.Key, policySetting.ValueName,
                                                            policySetting.ValString, policySetting.ValBool));
                                                    }

                                                    break;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;
                    }
                } //end of while

                xmlReader.Close(); 

            } // end of try
            catch (Exception e)
            {
                this.Log.AddErrorMsg("ReadSetRules() has encountered an error: ", e);
            }

            // Re-init PolicyName and ID in case one or both are not defined in this.Settings
            this.PolicyName = @"N/A";
            this.PolicyID = @"N/A";  
    
            foreach(var setting in this.Settings)
            {
                switch(setting.ValueName)
                {
                    case "Name":
                        this.PolicyName = setting.ValString;
                        break;

                    case "Id":
                        this.PolicyID = setting.ValString;
                        break;

                    default:
                        break; 
                }
            }
            
        }

        /// <summary>
        /// Displays the Policy settings: policy name and ID, to the User to allow for modification. 
        /// </summary>
        private void DisplayPolicyInfo()
        {
            textBox_PolicyID.Text = this.PolicyID;
            textBox_PolicyName.Text = this.PolicyName; 
            policyInfoPanel.Visible = true;
        }

        /// <summary>
        /// Sets the new Policy Name setting. 
        /// </summary>
        private void textBox_PolicyName_TextChanged(object sender, EventArgs e)
        {
            this.PolicyName = textBox_PolicyName.Text;
            this._MainWindow.Policy.PolicyName = this.PolicyName;
        }

        /// <summary>
        /// Sets the new Policy ID setting. 
        /// </summary>
        private void textBox_PolicyID_TextChanged(object sender, EventArgs e)
        {
            this.PolicyID = textBox_PolicyID.Text;
            this._MainWindow.Policy.PolicyID = this.PolicyID;
        }
    }
}
