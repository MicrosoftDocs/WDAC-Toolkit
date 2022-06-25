using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WDAC_Wizard
{
    public partial class EventLogRuleConfiguration : UserControl
    {
        private List<CiEvent> CiEvents;
        // Declare an ArrayList to serve as the data store. 
        private List<EventDisplayObject> DisplayObjects;

        private Logger Log;
        private SiPolicy siPolicy;

        private EventDisplayObject displayObjectInEdit;
        private int rowInEdit = -1;

        private int SelectedRow;
        private List<int> RuleIdsToAdd;

        // UI States
        private int[] PublisherUIState = { 1, 0, 0, 0, 0};
        private int[] FileAttributesUIState = { 0, 0, 0, 0, 0 };
        private int[] FilePathUIState = { 0, 0 };

        public EventLogRuleConfiguration(MainWindow pMainWindow)
        {
            InitializeComponent();

            this.CiEvents = pMainWindow.CiEvents; 
            this.Log = pMainWindow.Log;
            this.siPolicy = Helper.DeserializeXMLStringtoPolicy(Properties.Resources.EmptyWDAC);

            this.DisplayObjects = new List<EventDisplayObject>();

            this.SelectedRow = 0;
            this.RuleIdsToAdd = new List<int>();
        }

        /// <summary>
        /// On page load, set the UI and populate the event table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventLogRuleConfiguration_Load(object sender, EventArgs e)
        {
            // UI Elements
            // Set publisher rule by default
            this.ruleTypeComboBox.SelectedIndex = 0;
            this.publisherRulePanel.Visible = true;

            // Highlight/select the first row in the table


            // Set the table, let's eat
            DisplayEvents();
        }

        /// <summary>
        /// Creates a new display object for the DataGridView for each CiEvent object
        /// </summary>
        private void DisplayEvents()
        {
            foreach(CiEvent ciEvent in this.CiEvents)
            {
                // Create one row per ciEvent per signer Event
                // File signed by 3 signers will create 3 rows/rules
                EventDisplayObject dpObject = new EventDisplayObject(
                        ciEvent.EventId.ToString(),
                        ciEvent.FileName,
                        ciEvent.ProductName,
                        ciEvent.PolicyName,
                        ciEvent.SignerInfo.PublisherName);

                this.DisplayObjects.Add(dpObject);
                this.eventsDataGridView.RowCount += 1;
            }
        }

        /// <summary>
        /// Adds rule to the SiPolicy object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, EventArgs e)
        {
            // Update UI and SiPolicy object
            EventDisplayObject dp = this.DisplayObjects[this.SelectedRow];
            dp.Action = "Adding"; 
            CiEvent ciEvent = this.CiEvents[this.SelectedRow];

            switch(this.ruleTypeComboBox.SelectedIndex)
            {
                case 0: // Publisher
                    if(IsValidPublisherUiState())
                    {
                        // this.siPolicy = PolicyHelper.AddSiPolicyPublisherRule(ciEvent, this.siPolicy);
                    }
                    break;

                case 1: // File Path Rule
                    if(IsValidFilePathUiState())
                    {
                        this.siPolicy = PolicyHelper.AddSiPolicyFilePathRule(ciEvent, this.siPolicy, this.FilePathUIState);
                    }
                    break; 

                case 2: // File Attributes Rule
                case 3:
                    if(IsValidFileAttributesUiState())
                    {
                        this.siPolicy = PolicyHelper.AddSiPolicyFileAttributeRule(ciEvent, this.siPolicy, this.FileAttributesUIState);
                    }
                    break;

                case 4: // Hash rule
                    this.siPolicy = PolicyHelper.AddSiPolicyHashRules(ciEvent, this.siPolicy);
                    break; 
            }

            this.eventsDataGridView.Refresh();
            // Need to scroll somewhere else to update the cellvalue 
        }

        /// <summary>
        /// Populates the custom rules panel UI with the contents from the CiEvent list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventRowClick(object sender, DataGridViewCellEventArgs e)
        {
            // Set the UI
            ResetCustomRulesPanel();

            int selectedRow = e.RowIndex;
            
            SetPublisherPanel(
                this.CiEvents[selectedRow].SignerInfo.IssuerName,
                this.CiEvents[selectedRow].SignerInfo.PublisherName,
                this.CiEvents[selectedRow].OriginalFilename,
                this.CiEvents[selectedRow].FileVersion,
                this.CiEvents[selectedRow].ProductName);

            this.SelectedRow = selectedRow;
        }

        /// <summary>
        /// Resets the custom rules panel 
        /// </summary>this.SelectedRow = selectedRow; 
        private void ResetCustomRulesPanel()
        {
            // Uncheck boxes
            this.publisherCheckBox.Checked = false;
            this.filenameCheckBox.Checked = false;
            this.versionCheckBox.Checked = false;
            this.productCheckBox.Checked = false;

            this.publisherCheckBox.Enabled = false;
            this.filenameCheckBox.Enabled = false;
            this.versionCheckBox.Enabled = false;
            this.productCheckBox.Enabled = false;

            // Clear all textboxes
            this.issuerTextBox.Clear();
            this.publisherTextBox.Clear();
            this.filenameTextBox.Clear();
            this.versionTextBox.Clear();
            this.productTextBox.Clear();

            // Dropdown
            this.ruleTypeComboBox.SelectedIndex = 0; 
        }


        /// <summary>
        /// Gets the state of the checkboxes and determines whether the UI state is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValidPublisherUiState()
        {
            // UI States:
            // this.PublisherUIState[0] == 1 always; it cannot be modified
            this.PublisherUIState[1] = this.publisherCheckBox.Checked == true ? 1 : 0;
            this.PublisherUIState[2] = this.filenameCheckBox.Checked == true ? 1 : 0;
            this.PublisherUIState[3] = this.versionCheckBox.Checked == true ? 1 : 0;
            this.PublisherUIState[4] = this.productCheckBox.Checked == true ? 1 : 0;

            if(String.IsNullOrEmpty(this.issuerTextBox.Text) ||
                this.issuerTextBox.Text == Properties.Resources.BadEventPubValue )
            {
                return false; 
            }

            if (PublisherUIState[1] == 1 && (String.IsNullOrEmpty(this.publisherTextBox.Text) || 
                this.publisherTextBox.Text == Properties.Resources.BadEventPubValue))
            {
                return false;
            }

            if (PublisherUIState[2] == 1 && String.IsNullOrEmpty(this.filenameTextBox.Text))
            {
                return false;
            }

            if (PublisherUIState[3] == 1 && String.IsNullOrEmpty(this.versionCheckBox.Text))
            {
                return false;
            }

            if (PublisherUIState[4] == 1 && String.IsNullOrEmpty(this.productTextBox.Text))
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Gets the state of the checkboxes and determines whether the UI state is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValidFileAttributesUiState()
        {
            // UI States:
            this.FileAttributesUIState[0] = this.origFileNameCheckBox.Checked == true ? 1 : 0;
            this.FileAttributesUIState[1] = this.fileDescCheckBox.Checked == true ? 1 : 0;
            this.FileAttributesUIState[2] = this.prodNameCheckBox.Checked == true ? 1 : 0;
            this.FileAttributesUIState[3] = this.intFileNameCheckBox.Checked == true ? 1 : 0;
            this.FileAttributesUIState[4] = this.pfnCheckBox.Checked == true ? 1 : 0;

            if (FileAttributesUIState[0] == 1 && String.IsNullOrEmpty(this.origFileNameTextBox.Text))
            {
                return false;
            }

            if (FileAttributesUIState[1] == 1 && String.IsNullOrEmpty(this.fileDescTextBox.Text))
            {
                return false;
            }

            if (FileAttributesUIState[2] == 1 && String.IsNullOrEmpty(this.prodNameTextBox.Text))
            {
                return false;
            }

            if (FileAttributesUIState[3] == 1 && String.IsNullOrEmpty(this.intFileNameTextBox.Text))
            {
                return false;
            }

            if (FileAttributesUIState[4] == 1 && String.IsNullOrEmpty(this.pfnTextBox.Text))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the state of the checkboxes and determines whether the UI state is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValidFilePathUiState()
        {
            // UI States:
            this.FilePathUIState[0] = this.filePathCheckBox.Checked == true ? 1 : 0;
            this.FilePathUIState[1] = this.folderPathCheckBox.Checked == true ? 1 : 0;

            if (FilePathUIState[0] == 1 && String.IsNullOrEmpty(this.filePathTextBox.Text))
            {
                return false;
            }
            
            if(FilePathUIState[1] == 1 && String.IsNullOrEmpty(this.folderPathTextBox.Text))
            {
                return false; 
            }

           

            return true;
        }

        private void EventsDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            // If this is the row for new records, no values are needed.
            if (e.RowIndex == this.eventsDataGridView.RowCount - 1) return;

            EventDisplayObject displayObject = null;

            // Store a reference to the Customer object for the row being painted.
            if (e.RowIndex == rowInEdit)
            {
                displayObject = this.displayObjectInEdit;
            }
            else
            {
                displayObject = (EventDisplayObject)this.DisplayObjects[e.RowIndex];
            }

            // Set the cell value to paint using the Customer object retrieved.
            switch (this.eventsDataGridView.Columns[e.ColumnIndex].Name)
            {
                case "addedColumn":
                    e.Value = displayObject.Action;
                    break;

                case "eventIdColumn":
                    e.Value = displayObject.EventId;
                    break;

                case "filenameColumn":
                    e.Value = displayObject.Filename;
                    break;

                case "productColumn":
                    e.Value = displayObject.Product;
                    break;

                case "policyColumn":
                    e.Value = displayObject.PolicyName;
                    break;

                case "publisherColumn":
                    e.Value = displayObject.Publisher;
                    break;
            }
        }

        /// <summary>
        /// Hides all the panels from the user
        /// </summary>
        private void HideAllPanels()
        {
            this.publisherRulePanel.Visible = false;
            this.fileAttributeRulePanel.Visible = false;
            this.filePathRulePanel.Visible = false; 
            this.hashRulePanel.Visible = false; 
        }

        /// <summary>
        /// User has changed the custom rule type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RuleTypeChanged(object sender, EventArgs e)
        {
            // Publisher
            // Path
            // File Attributes
            // File Hash

            HideAllPanels();

            switch (this.ruleTypeComboBox.SelectedIndex)
            {
                case 0: // Publisher
                    SetPublisherPanel(
                        this.CiEvents[this.SelectedRow].SignerInfo.IssuerName,
                        this.CiEvents[this.SelectedRow].SignerInfo.PublisherName,
                        this.CiEvents[this.SelectedRow].OriginalFilename,
                        this.CiEvents[this.SelectedRow].FileVersion,
                        this.CiEvents[this.SelectedRow].ProductName);
                    break;

                case 1: // Path
                    SetFilePathPanel(
                        this.CiEvents[this.SelectedRow].FilePath);
                    break;

                case 2: // File Attributes
                case 3:
                    SetFileAttributesPanel(
                        this.CiEvents[this.SelectedRow].OriginalFilename,
                        this.CiEvents[this.SelectedRow].FileDescription,
                        this.CiEvents[this.SelectedRow].ProductName,
                        this.CiEvents[this.SelectedRow].InternalFilename,
                        this.CiEvents[this.SelectedRow].PackageFamilyName);

                    break;

                case 4: //FileHash
                    SetFileHashPanel(
                        this.CiEvents[this.SelectedRow].SHA1,
                        this.CiEvents[this.SelectedRow].SHA1Page,
                        this.CiEvents[this.SelectedRow].SHA2,
                        this.CiEvents[this.SelectedRow].SHA2Page);
                    break; 
            }
        }

        /// <summary>
        /// Sets the text in the publisher textboxes and the default checkbox states
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="publisher"></param>
        /// <param name="filename"></param>
        /// <param name="version"></param>
        /// <param name="product"></param>
        private void SetPublisherPanel(string issuer, string publisher, string filename, string version, string product)
        {
            this.issuerTextBox.Text = issuer;
            this.publisherTextBox.Text = publisher;
            this.filenameTextBox.Text = filename;
            this.versionTextBox.Text = version;
            this.productTextBox.Text = product;

            // Default checkbox checked and enabled states
            if (!String.IsNullOrEmpty(publisher))
            {
                this.publisherCheckBox.Checked = true;
                this.publisherCheckBox.Enabled = true;
            }

            if (!String.IsNullOrEmpty(filename))
            {
                this.filenameCheckBox.Checked = true;
                this.filenameCheckBox.Enabled = true;
            }

            if (!String.IsNullOrEmpty(version))
            {
                this.versionCheckBox.Checked = true;
                this.versionCheckBox.Enabled = true;
            }

            if (!String.IsNullOrEmpty(product))
            {
                this.productCheckBox.Checked = true;
                this.productCheckBox.Enabled = true;
            }

            // Unhide the panel
            this.publisherRulePanel.Visible = true; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalFilename"></param>
        /// <param name="fileDescription"></param>
        /// <param name="productName"></param>
        /// <param name="internalFilename"></param>
        /// <param name="packagedAppName"></param>
        private void SetFileAttributesPanel(string originalFilename, string fileDescription, string productName,
            string internalFilename, string packagedAppName)
        {
            this.origFileNameTextBox.Text = originalFilename;
            this.fileDescTextBox.Text = fileDescription;
            this.prodNameTextBox.Text = productName;
            this.intFileNameTextBox.Text = internalFilename;
            this.pfnTextBox.Text = packagedAppName;

            // Default checkbox checked and enabled states
            if (!String.IsNullOrEmpty(originalFilename))
            {
                this.origFileNameCheckBox.Checked = true;
                this.origFileNameCheckBox.Enabled = true;
            }

            if (!String.IsNullOrEmpty(fileDescription))
            {
                this.fileDescCheckBox.Checked = true;
                this.fileDescCheckBox.Enabled = true;
            }

            if (!String.IsNullOrEmpty(productName))
            {
                this.prodNameCheckBox.Checked = true;
                this.prodNameCheckBox.Enabled = true;
            }

            if (!String.IsNullOrEmpty(internalFilename))
            {
                this.intFileNameCheckBox.Checked = true;
                this.intFileNameCheckBox.Enabled = true;
            }

            if(!String.IsNullOrEmpty(packagedAppName))
            {
                this.pfnCheckBox.Checked = true;
                this.pfnCheckBox.Enabled = true;
            }

            // Unhide the panel
            this.fileAttributeRulePanel.Visible = true;
            this.fileAttributeRulePanel.Location = this.publisherRulePanel.Location; // snap to the loc of pub panel
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalFilename"></param>
        /// <param name="fileDescription"></param>
        /// <param name="productName"></param>
        /// <param name="internalFilename"></param>
        /// <param name="packagedAppName"></param>
        private void SetFileHashPanel(byte[] sha1, byte[] sha1page, byte[] sha2, byte[] sha2page)
        {
            this.sha1TextBox.Text = Helper.ConvertHash(sha1);
            this.sha1PageTextBox.Text = Helper.ConvertHash(sha1page);
            this.sha2TextBox.Text = Helper.ConvertHash(sha2);
            this.sha2PageTextBox.Text = Helper.ConvertHash(sha2page);

            // Unhide the panel
            this.hashRulePanel.Visible = true;
            this.hashRulePanel.Location = this.publisherRulePanel.Location; // snap to the loc of pub panel
        }

        private void SetFilePathPanel(string filepath)
        {
            this.filePathTextBox.Text = filepath;
            this.folderPathTextBox.Text = Path.GetDirectoryName(filepath);

            this.filePathCheckBox.Checked = true;
            this.folderPathCheckBox.Checked = true; 

            // Unhide the panel
            this.filePathRulePanel.Visible = true;
            this.filePathRulePanel.Location = this.publisherRulePanel.Location; // snap to the loc of pub panel
        }
    }

    internal static class PolicyHelper
    {
        // Counts of file rules created to pipe into IDs
        static public int cFilePublisherRules = 0;
        static public int cFileAttribRules = 0;
        static public int cFilePathRules = 0;
        static public int cFileHashRules = 0;

        /// <summary>
        /// Creates up to 2 path rules. One for the parent path and one for the full path
        /// </summary>
        /// <param name="ciEvent"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy AddSiPolicyFileAttributeRule(CiEvent ciEvent, SiPolicy siPolicy, int[] uiState)
        {
            List<Allow> allowFileAttributeRules = new List<Allow>(); 

            Allow allowRule = new Allow();
            allowRule.FriendlyName = String.Format("Allow file {0} by file attributes; Correlation Id: {1}", 
                ciEvent.FileName, ciEvent.CorrelationId);
            allowRule.ID = "ID_ALLOW_B_" + cFilePathRules.ToString();
            cFileAttribRules++;

            // Original filename
            if (uiState[0] == 1)
            {
                allowRule.FileName = ciEvent.OriginalFilename;
            }

            // file description
            if (uiState[1] == 1)
            {
                allowRule.FileDescription = ciEvent.FileDescription;
            }

            // Product name
            if (uiState[2] == 1)
            {
                allowRule.ProductName = ciEvent.ProductName;
            }

            // Internal File name
            if (uiState[3] == 1)
            {
                allowRule.InternalName = ciEvent.InternalFilename;
            }

            // Package name
            if (uiState[4] == 1)
            {
                allowRule.PackageFamilyName = ciEvent.PackageFamilyName;
            }

            allowFileAttributeRules.Add(allowRule);

            // Add the Allow rule to FileRules and FileRuleRef section with Windows Signing Scenario
            siPolicy = AddSiPolicyAllowRules(allowFileAttributeRules, siPolicy);
            return siPolicy;
        }



        /// <summary>
        /// Creates up to 2 path rules. One for the parent path and one for the full path
        /// </summary>
        /// <param name="ciEvent"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy AddSiPolicyFilePathRule(CiEvent ciEvent, SiPolicy siPolicy, int[] uiState)
        {
            List<Allow> allowPathRules = new List<Allow>();

            // FilePath
            if(uiState[0] == 1)
            {
                Allow allowRule = new Allow();
                allowRule.FilePath = ciEvent.FilePath;
                allowRule.FriendlyName = String.Format("Allow path: {0}; Correlation Id: {1}", allowRule.FilePath, ciEvent.CorrelationId);
                allowRule.ID = "ID_ALLOW_C_" + cFilePathRules.ToString();
                cFilePathRules++;
                allowPathRules.Add(allowRule);
            }

            // FolderPath
            if (uiState[1] == 1)
            {
                Allow allowRule = new Allow();
                allowRule.FilePath = Path.GetDirectoryName(ciEvent.FilePath) + "\\*";
                allowRule.FriendlyName = String.Format("Allow path: {0}; Correlation Id: {1}", allowRule.FilePath, ciEvent.CorrelationId);
                allowRule.ID = "ID_ALLOW_C_" + cFilePathRules.ToString();
                cFilePathRules++;
                allowPathRules.Add(allowRule);
            }

            // Add the Allow rule to FileRules and FileRuleRef section with Windows Signing Scenario
            siPolicy = AddSiPolicyAllowRules(allowPathRules, siPolicy);
            return siPolicy;
        }

        /// <summary>
        /// Creates 4 hash rules (SHA1, SHA2 page and flat)
        /// </summary>
        /// <param name="ciEvent"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        public static SiPolicy AddSiPolicyHashRules(CiEvent ciEvent, SiPolicy siPolicy)
        {
            List<Allow> allowHashRules = new List<Allow>();
            
            // SHA1
            Allow allowRule = new Allow();
            allowRule.Hash = ciEvent.SHA1;
            allowRule.FriendlyName = String.Format("{0} hash rule; Correlation Id: {1}", ciEvent.FileName, ciEvent.CorrelationId);
            allowRule.ID = String.Format("ID_ALLOW_E_{0}_{1}", cFileHashRules, "SHA1");
            allowHashRules.Add(allowRule);
            cFileHashRules++;

            // SHA1 Page
            allowRule = new Allow();
            allowRule.Hash = ciEvent.SHA1Page;
            allowRule.FriendlyName = String.Format("{0} hash rule; Correlation Id: {1}", ciEvent.FileName, ciEvent.CorrelationId);
            allowRule.ID = String.Format("ID_ALLOW_E_{0}_{1}", cFileHashRules, "SHA1_PAGE");
            allowHashRules.Add(allowRule);
            cFileHashRules++;


            // SHA2
            allowRule = new Allow();
            allowRule.Hash = ciEvent.SHA2;
            allowRule.FriendlyName = String.Format("{0} hash rule; Correlation Id: {1}", ciEvent.FileName, ciEvent.CorrelationId);
            allowRule.ID = String.Format("ID_ALLOW_E_{0}_{1}", cFileHashRules, "SHA2");
            allowHashRules.Add(allowRule);
            cFileHashRules++;

            // SHA2 Page
            allowRule = new Allow();
            allowRule.Hash = ciEvent.SHA1Page;
            allowRule.FriendlyName = String.Format("{0} hash rule; Correlation Id: {1}", ciEvent.FileName, ciEvent.CorrelationId);
            allowRule.ID = String.Format("ID_ALLOW_E_{0}_{1}", cFileHashRules, "SHA2_PAGE");
            allowHashRules.Add(allowRule);
            cFileHashRules++;

            // Add the Allow rule to FileRules and FileRuleRef section with Windows Signing Scenario
            siPolicy = AddSiPolicyAllowRules(allowHashRules, siPolicy);
            return siPolicy;
        }

        /// <summary>
        /// Handles adding the new Allow Rule object to the provided siPolicy
        /// </summary>
        /// <param name="allowRule"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        private static SiPolicy AddSiPolicyAllowRules(List<Allow> allowRules, SiPolicy siPolicy)
        {
            // Copy and replace the FileRules obj[] in siPolicy
            // FileRules always initalized - no need to check if null

            foreach(Allow allowRule in allowRules)
            {
                object[] fileRulesCopy = new object[siPolicy.FileRules.Length + 1];
                for (int i = 0; i < fileRulesCopy.Length - 1; i++)
                {
                    fileRulesCopy[i] = siPolicy.FileRules[i];
                }

                fileRulesCopy[fileRulesCopy.Length - 1] = allowRule;
                siPolicy.FileRules = fileRulesCopy;

                // Copy and replace the FileRulesRef section to add to Signing Scenarios
                FileRulesRef refCopy = new FileRulesRef();
                if (siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef == null)
                {
                    refCopy.FileRuleRef = new FileRuleRef[1];
                    refCopy.FileRuleRef[0] = new FileRuleRef();
                    refCopy.FileRuleRef[0].RuleID = allowRule.ID;
                }
                else
                {
                    refCopy.FileRuleRef = new FileRuleRef[siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef.FileRuleRef.Length + 1];
                    for (int i = 0; i < refCopy.FileRuleRef.Length - 1; i++)
                    {
                        refCopy.FileRuleRef[i] = siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef.FileRuleRef[i];
                    }

                    refCopy.FileRuleRef[refCopy.FileRuleRef.Length - 1] = new FileRuleRef();
                    refCopy.FileRuleRef[refCopy.FileRuleRef.Length - 1].RuleID = allowRule.ID;
                }

                siPolicy.SigningScenarios[1].ProductSigners.FileRulesRef = refCopy;
            }
            
            return siPolicy;
        }
    }

    // Class for the datastore
    public class EventDisplayObject
    {
        public string Action;
        public string EventId;
        public string Filename;
        public string Product;
        public string PolicyName;
        public string Publisher;

        public EventDisplayObject()
        {
            this.Action = string.Empty;
            this.EventId = string.Empty;
            this.Filename = string.Empty;
            this.Product = string.Empty;
            this.PolicyName = string.Empty;
            this.Publisher = string.Empty;
        }

        public EventDisplayObject(string eventId, string filename, string product, string policyName, string publisher)
        {
            this.Action = "   ---   ";
            this.EventId = eventId;
            this.Filename = filename;
            this.Product = product;
            this.PolicyName = policyName;
            this.Publisher = publisher;
        }

    }
}
