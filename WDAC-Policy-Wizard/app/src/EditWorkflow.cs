// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
// jogeurte 11/19

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace WDAC_Wizard
{
    public partial class EditWorkflow : UserControl
    {
        public string EditPath { get; set; }
        private Logger Log; 
        private MainWindow _MainWindow;
        private WDAC_Policy Policy;


        public EditWorkflow(MainWindow pMainWindow)
        {
            InitializeComponent();

            this._MainWindow = pMainWindow;
            this._MainWindow.ErrorOnPage = false;
            this._MainWindow.RedoFlowRequired = false;
            this._MainWindow.Policy._PolicyType = WDAC_Policy.PolicyType.Edit;

            this.Policy = this._MainWindow.Policy; 
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
            // Serialize the policy into the policy object
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SiPolicy));
                StreamReader reader = new StreamReader(xmlPath);
                this.Policy.siPolicy = (SiPolicy)serializer.Deserialize(reader);
                reader.Close();
                this._MainWindow.ErrorOnPage = false;

                // Set the policy type (base, supplemental) to know which policy rule-options to set on ConfigTemplate_Control
                // Set the policy format type for the policy creation step in MainForm.cs
                if(this.Policy.siPolicy.PolicyTypeID == Properties.Resources.LegacyPolicyID_GUID)
                {
                    this._MainWindow.Policy._Format = WDAC_Policy.Format.Legacy;
                }
                else
                {
                    this._MainWindow.Policy._Format = WDAC_Policy.Format.MultiPolicy;
                }
            }

            catch (Exception e)
            {
                // Log eexception error and throw error to user
                DialogResult res = MessageBox.Show("The base policy you have selected cannot be parsed by the Wizard\n\n" +
                    "This is typically a result of a malformed policy.",
                    "Policy Parsing Issue", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Log.AddErrorMsg("ParsePolicy encountered the following error message" + e.ToString()); 
                this._MainWindow.ErrorOnPage = true;
            }
        }

        /// <summary>
        /// Displays the Policy settings: policy name and ID, to the User to allow for modification. 
        /// </summary>
        private void DisplayPolicyInfo()
        {
            // Set the default text fields to N/A. Will overwrite if we find true settings
            textBox_PolicyID.Text =   "N/A"; 
            textBox_PolicyName.Text = "N/A"; 
            if(this.Policy.siPolicy.Settings != null)
            {
                foreach (var setting in this.Policy.siPolicy.Settings)
                {
                    if (setting.ValueName == "Name")
                    {
                        // Found the name of the policy
                        textBox_PolicyName.Text = setting.Value.Item.ToString();
                    }
                    else if (setting.ValueName == "Id")
                    {
                        // Found the name of the policy
                        textBox_PolicyID.Text = setting.Value.Item.ToString();
                    }
                    else
                    {
                        // Found another setting that we do not show to user
                        continue;
                    }
                }
            }

            policyInfoPanel.Visible = true;
        }

        /// <summary>
        /// Sets the new Policy Name setting. 
        /// </summary>
        private void textBox_PolicyName_TextChanged(object sender, EventArgs e)
        {
            this._MainWindow.Policy.PolicyName = textBox_PolicyName.Text;
        }

        /// <summary>
        /// Sets the new Policy ID setting. 
        /// </summary>
        private void textBox_PolicyID_TextChanged(object sender, EventArgs e)
        {
            this._MainWindow.Policy.PolicyID = textBox_PolicyID.Text;
        }
    }
}
