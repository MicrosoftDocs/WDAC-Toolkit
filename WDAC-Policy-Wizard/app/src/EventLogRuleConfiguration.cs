using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WDAC_Wizard
{
    public partial class EventLogRuleConfiguration : UserControl
    {
        private List<CiEvent> CiEvents;
        // Declare an ArrayList to serve as the data store. 
        private System.Collections.ArrayList DisplayObjects =
            new System.Collections.ArrayList();

        private Logger Log;
        private SiPolicy siPolicy; 

        public EventLogRuleConfiguration(MainWindow pMainWindow)
        {
            InitializeComponent();

            this.CiEvents = pMainWindow.CiEvents; 
            this.Log = pMainWindow.Log;
            this.siPolicy = new SiPolicy(); 
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
            this.customRulePanel.Visible = true;

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
                foreach (SignerEvent signerEvent in ciEvent.SignerEvents)
                {
                    EventDisplayObject dpObject = new EventDisplayObject(
                        ciEvent.EventId.ToString(),
                        ciEvent.FileName,
                        ciEvent.ProductName,
                        ciEvent.PolicyName,
                        signerEvent.PublisherName);
                }
            }
        }

        /// <summary>
        /// Adds rule to the SiPolicy object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, EventArgs e)
        {

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
            SetCustomRulesPanel(
                this.CiEvents[selectedRow].SignerEvents[0].IssuerName,
                this.CiEvents[selectedRow].SignerEvents[0].PublisherName,
                this.CiEvents[selectedRow].FileName,
                this.CiEvents[selectedRow].FileVersion,
                this.CiEvents[selectedRow].ProductName);
        }

        /// <summary>
        /// Resets the custom rules panel 
        /// </summary>
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
        /// Sets the text in the publisher textboxes and the default checkbox states
        /// </summary>
        /// <param name="issuer"></param>
        /// <param name="publisher"></param>
        /// <param name="filename"></param>
        /// <param name="version"></param>
        /// <param name="product"></param>
        private void SetCustomRulesPanel(string issuer, string publisher, string filename, string version, string product)
        {
            this.issuerTextBox.Text = issuer; 
            this.publisherTextBox.Text = publisher;
            this.filenameTextBox.Text = filename;
            this.versionTextBox.Text = version;
            this.productTextBox.Text = product;

            // Default checkbox checked and enabled states
            if(!String.IsNullOrEmpty(publisher))
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
            this.Action = "-";
            this.EventId = eventId;
            this.Filename = filename;
            this.Product = product;
            this.PolicyName = policyName;
            this.Publisher = publisher;
        }

    }
}
