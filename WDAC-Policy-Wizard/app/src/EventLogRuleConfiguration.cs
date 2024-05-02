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

        private SiPolicy siPolicy;
        private MainWindow _MainWindow; 

        private EventDisplayObject displayObjectInEdit;
        private int rowInEdit = -1;

        private int SelectedRow;
        private List<int> RuleIdsToAdd;

        // UI States
        private int[] PublisherUIState = { 1, 0, 0, 0, 0};
        private int[] FileAttributesUIState = { 0, 0, 0, 0, 0 };
        private int[] FilePathUIState = { 0, 0 };

        // Reference to all the checkboxes
        private List<CheckBox> CheckBoxes; 

        public EventLogRuleConfiguration(MainWindow pMainWindow)
        {
            InitializeComponent();

            this.CiEvents = pMainWindow.CiEvents; 
            this._MainWindow = pMainWindow; 
            this.siPolicy = Helper.DeserializeXMLStringtoPolicy(Properties.Resources.EmptyWDAC);

            this.DisplayObjects = new List<EventDisplayObject>();

            this.SelectedRow = 0;
            this.RuleIdsToAdd = new List<int>();

            // Set error flag - Bug #234
            this._MainWindow.ErrorOnPage = true;
            this._MainWindow.ErrorMsg = Properties.Resources.InvalidEventRulesCreated;

            this.CheckBoxes = new List<CheckBox>(); 
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
            CiEvent ciEvent = this.CiEvents[this.SelectedRow];

            switch(this.ruleTypeComboBox.SelectedIndex)
            {
                case 0: // Publisher
                    if(IsValidPublisherUiState())
                    {
                        dp.Action = "Added to policy";
                        this.siPolicy = PolicyEventHelper.AddSiPolicyPublisherRule(ciEvent, this.siPolicy, PublisherUIState);
                        this._MainWindow.EventLogPolicy = this.siPolicy;
                        ClearErrorMsg();
                    }
                    break;

                case 1: // File Path Rule
                    if(IsValidFilePathUiState())
                    {
                        dp.Action = "Added to policy"; 
                        this.siPolicy = PolicyEventHelper.AddSiPolicyFilePathRule(ciEvent, this.siPolicy, this.FilePathUIState);
                        this._MainWindow.EventLogPolicy = this.siPolicy;
                        ClearErrorMsg();
                    }
                    break; 

                case 2: // File Attributes Rule
                case 3:
                    if(IsValidFileAttributesUiState())
                    {
                        dp.Action = "Added to policy"; 
                        this.siPolicy = PolicyEventHelper.AddSiPolicyFileAttributeRule(ciEvent, this.siPolicy, this.FileAttributesUIState);
                        this._MainWindow.EventLogPolicy = this.siPolicy;
                        ClearErrorMsg();
                    }
                    break;

                case 4: // Hash rule
                    if(IsValidHashRuleUiState())
                    {
                        dp.Action = "Added to policy";
                        this.siPolicy = PolicyEventHelper.AddSiPolicyHashRules(ciEvent, this.siPolicy);
                        this._MainWindow.EventLogPolicy = this.siPolicy;
                        ClearErrorMsg();
                    }
                    break; 
            }

            this.eventsDataGridView.Refresh();
        }

        /// <summary>
        /// Populates the custom rules panel with CiEvent on table row highlight event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RowSelectionChanged(object sender, EventArgs e)
        {
            // Try to cast sender as Datagrid. If successful, the user highlighted (arrow keys or mouse click) a new row
            var grid = sender as DataGridView;
            if (grid == null)
            {
                return;
            }

            int selectedRow = grid.CurrentRow.Index;
            if (selectedRow >= this.CiEvents.Count)
            {
                return;
            }

            // Set the UI
            ResetCustomRulesPanel();
            SetPublisherPanel(this.CiEvents[selectedRow].SignerInfo.IssuerName,
                              this.CiEvents[selectedRow].SignerInfo.PublisherName,
                              this.CiEvents[selectedRow].OriginalFilename,
                              this.CiEvents[selectedRow].FileVersion,
                              this.CiEvents[selectedRow].ProductName);

            this.SelectedRow = selectedRow;
        }

        /// <summary>
        /// Populates the custom rules panel UI with the contents from the CiEvent list on row mouse click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventRowClick(object sender, DataGridViewCellEventArgs e)
        {
            // Set the UI
            ResetCustomRulesPanel();

            int selectedRow = e.RowIndex;
            if(selectedRow >= this.CiEvents.Count)
            {
                return;
            }

            // Header selected, sort table
            if(selectedRow == -1)
            {
                SortDataGrid(sender, e);
                return;
            }
            
            SetPublisherPanel(this.CiEvents[selectedRow].SignerInfo.IssuerName,
                              this.CiEvents[selectedRow].SignerInfo.PublisherName,
                              this.CiEvents[selectedRow].OriginalFilename,
                              this.CiEvents[selectedRow].FileVersion,
                              this.CiEvents[selectedRow].ProductName);

            this.SelectedRow = selectedRow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SortDataGrid(object sender, DataGridViewCellEventArgs e)
        {
            // If the list is not sorted by the desired column, sort the list
            // Otherwise, simply reverse the list of objects to change from ascending --> descending --> ascending
            if (this.eventsDataGridView.Columns[e.ColumnIndex].Tag != (object)"Sorted")
            {
                SortDisplayObjects(e.ColumnIndex, false);
                ResetColTags();
                this.eventsDataGridView.Columns[e.ColumnIndex].Tag = "Sorted";
            }
            else
            {
                SortDisplayObjects(e.ColumnIndex, true);
            }
            this.eventsDataGridView.Refresh();
        }



        private void SortDisplayObjects(int columnToSort, bool isSorted)
        {
            /*
             *  public string Action;
             *  public string EventId;
             *  public string Filename;
             *  public string Product;
             *  public string PolicyName;
             *  public string Publisher;
            */

            // Sort display objects and CiEvents since row number is used to reference CiEvent
            if (!isSorted)
            {
                switch(columnToSort)
                {
                    case 1:
                        this.DisplayObjects.Sort((x, y) => x.EventId.CompareTo(y.EventId));
                        this.CiEvents.Sort((x, y) => x.EventId.CompareTo(y.EventId));
                        break;

                    case 2:
                        this.DisplayObjects.Sort((x, y) => x.Filename.CompareTo(y.Filename));
                        this.CiEvents.Sort((x, y) => x.FileName.CompareTo(y.FileName));
                        break;

                    case 3:
                        this.DisplayObjects.Sort((x, y) => x.Product.CompareTo(y.Product));
                        this.CiEvents.Sort((x, y) => x.ProductName.CompareTo(y.ProductName));
                        break;

                    case 4:
                        this.DisplayObjects.Sort((x, y) => x.PolicyName.CompareTo(y.PolicyName));
                        this.CiEvents.Sort((x, y) => x.PolicyName.CompareTo(y.PolicyName));
                        break;

                    case 5:
                        this.DisplayObjects.Sort((x, y) => x.Publisher.CompareTo(y.Publisher));
                        this.CiEvents.Sort((x, y) => x.SignerInfo.PublisherName.CompareTo(y.SignerInfo.PublisherName));
                        break;
                }
            }
            else
            {
                this.DisplayObjects.Reverse();
                this.CiEvents.Reverse();
            }

            this._MainWindow.CiEvents = this.CiEvents;
        }

        private void ResetColTags()
        {
            for(int i = 0; i< this.eventsDataGridView.Columns.Count; i++)
            {
                this.eventsDataGridView.Columns[i].Tag = ""; 
            }
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

            this.publisherCheckBox.AutoCheck = false;
            this.filenameCheckBox.AutoCheck = false;
            this.versionCheckBox.AutoCheck = false;
            this.productCheckBox.AutoCheck = false;

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

            if(String.IsNullOrEmpty(this.issuerTextBox.Text) 
                || this.issuerTextBox.Text == Properties.Resources.BadEventPubValue)
            {
                // Log exception error and throw error to user
                MessageBox.Show(this,String.Format("The Issuer is invalid for rule creation. The issuer cannot be empty or '{0}'", Properties.Resources.BadEventPubValue), 
                                                   "New Rule Creation Error", 
                                                   MessageBoxButtons.OK, 
                                                   MessageBoxIcon.Error);

                this.Invalidate();

                Logger.Log.AddWarningMsg("Event Log Config - Invalid publisher rule with Issuer = Unknown");
                return false; 
            }

            if (PublisherUIState[1] == 1 && (String.IsNullOrEmpty(this.publisherTextBox.Text) 
                || this.publisherTextBox.Text == Properties.Resources.BadEventPubValue))
            {
                DialogResult res = MessageBox.Show(String.Format("The Publisher is invalid for rule creation. The Publisher cannot be empty '{0}'", Properties.Resources.BadEventPubValue),
                                                   "New Rule Creation Error",
                                                   MessageBoxButtons.OK,
                                                   MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Invalid publisher rule with Publisher = Unknown");
                return false;
            }

            if (PublisherUIState[2] == 1 && String.IsNullOrEmpty(this.filenameTextBox.Text))
            {
                DialogResult res = MessageBox.Show(String.Format("The Filename is invalid for rule creation. The Filename cannot be empty if its checkbox is selected."),
                                                   "New Rule Creation Error",
                                                   MessageBoxButtons.OK,
                                                   MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Empty Filename");
                return false;
            }

            if (PublisherUIState[3] == 1 && String.IsNullOrEmpty(this.versionCheckBox.Text))
            {
                DialogResult res = MessageBox.Show(String.Format("The Version field is invalid for rule creation. The Version field cannot be empty if its checkbox is selected."),
                                                   "New Rule Creation Error",
                                                   MessageBoxButtons.OK,
                                                   MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Empty Version field");
                return false;
            }

            if (PublisherUIState[4] == 1 && String.IsNullOrEmpty(this.productTextBox.Text))
            {
                DialogResult res = MessageBox.Show(String.Format("The Product name is invalid for rule creation. The Product name cannot be empty if its checkbox is selected."),
                                                   "New Rule Creation Error",
                                                   MessageBoxButtons.OK,
                                                   MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Empty Product name");
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

            // Nothing selected
            if(FileAttributesUIState[0] == 0 &&
                FileAttributesUIState[1] == 0 &&
                FileAttributesUIState[2] == 0 &&
                FileAttributesUIState[3] == 0 &&
                FileAttributesUIState[4] == 0)
            {
                MessageBox.Show(String.Format("At least one file attribute needs to be selected to create the rule."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - File attributes none selected");
                return false; 
            }

            if (FileAttributesUIState[0] == 1 && String.IsNullOrEmpty(this.origFileNameTextBox.Text))
            {
                MessageBox.Show(String.Format("The Original filename is invalid for rule creation. The original filename cannot be empty if its checkbox is selected."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Original filename name");
                return false;
            }

            if (FileAttributesUIState[1] == 1 && String.IsNullOrEmpty(this.fileDescTextBox.Text))
            {
                MessageBox.Show(String.Format("The File description is invalid for rule creation. The File description field cannot be empty if its checkbox is selected."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - File description");
                return false;
            }

            if (FileAttributesUIState[2] == 1 && String.IsNullOrEmpty(this.prodNameTextBox.Text))
            {
                MessageBox.Show(String.Format("The Product name is invalid for rule creation. The Product name field cannot be empty if its checkbox is selected."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Product name ");
                return false;
            }

            if (FileAttributesUIState[3] == 1 && String.IsNullOrEmpty(this.intFileNameTextBox.Text))
            {
                MessageBox.Show(String.Format("The Internal filename is invalid for rule creation. The Internal filename field cannot be empty if its checkbox is selected."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Internal filename");
                return false;
            }

            if (FileAttributesUIState[4] == 1 && String.IsNullOrEmpty(this.pfnTextBox.Text))
            {
                MessageBox.Show(String.Format("The Package Family Name is invalid for rule creation. The Package Family Name field cannot be empty if its checkbox is selected."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Package Family Name ");
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

            // Nothing selected
            if (FilePathUIState[0] == 0 &&
                FilePathUIState[1] == 0)
            {
                MessageBox.Show(String.Format("At least one file path needs to be selected to create the rule."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Path rules none selected");
                return false;
            }

            if (FilePathUIState[0] == 1 && String.IsNullOrEmpty(this.filePathTextBox.Text))
            {
                MessageBox.Show(String.Format("The file path field is invalid for rule creation. The file path field cannot be empty if its checkbox is selected."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - empty file path field");
                return false;
            }
            
            if(FilePathUIState[1] == 1 && String.IsNullOrEmpty(this.folderPathTextBox.Text))
            {
                MessageBox.Show(String.Format("The folder path field is invalid for rule creation. The folder path field cannot be empty if its checkbox is selected."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - empty folder path field");
                return false; 
            }

            return true;
        }

        /// <summary>
        /// Checks that there are hash values for the rare null hashes state
        /// </summary>
        /// <returns></returns>
        public bool IsValidHashRuleUiState()
        {
            // Need at least 1 non-null hash value
            if (String.IsNullOrEmpty(this.sha1TextBox.Text)
                && String.IsNullOrEmpty(this.sha2TextBox.Text))
            {
                MessageBox.Show(String.Format("Hash values cannot be empty in order to create a hash-based rule"),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - hash rules none selected");
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
                    SetPublisherPanel(this.CiEvents[this.SelectedRow].SignerInfo.IssuerName,
                                      this.CiEvents[this.SelectedRow].SignerInfo.PublisherName,
                                      this.CiEvents[this.SelectedRow].OriginalFilename,
                                      this.CiEvents[this.SelectedRow].FileVersion,
                                      this.CiEvents[this.SelectedRow].ProductName);
                    break;

                case 1: // Path
                    SetFilePathPanel(this.CiEvents[this.SelectedRow].FilePath);
                    break;

                case 2: // File Attributes
                case 3: // Package Family Name
                    SetFileAttributesPanel(this.CiEvents[this.SelectedRow].OriginalFilename,
                                           this.CiEvents[this.SelectedRow].FileDescription,
                                           this.CiEvents[this.SelectedRow].ProductName,
                                           this.CiEvents[this.SelectedRow].InternalFilename,
                                           this.CiEvents[this.SelectedRow].PackageFamilyName);
                    break;

                case 4: //FileHash
                    SetFileHashPanel(this.CiEvents[this.SelectedRow].SHA1,
                                     this.CiEvents[this.SelectedRow].SHA2);
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
                this.publisherCheckBox.AutoCheck = true;
            }

            if (!String.IsNullOrEmpty(filename))
            {
                this.filenameCheckBox.Checked = true;
                this.filenameCheckBox.AutoCheck = true;
            }

            if (!String.IsNullOrEmpty(version))
            {
                this.versionCheckBox.Checked = true;
                this.versionCheckBox.AutoCheck = true;
            }

            if (!String.IsNullOrEmpty(product))
            {
                this.productCheckBox.Checked = true;
                this.productCheckBox.AutoCheck = true;
            }

            // Unhide the panel
            this.publisherRulePanel.Visible = true;

            // Set color of disabled checkboxes
            SetDisabledCheckBoxesColor(); 
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
                this.origFileNameCheckBox.AutoCheck = true;
            }

            if (!String.IsNullOrEmpty(fileDescription))
            {
                this.fileDescCheckBox.Checked = true;
                this.fileDescCheckBox.AutoCheck = true;
            }

            if (!String.IsNullOrEmpty(productName))
            {
                this.prodNameCheckBox.Checked = true;
                this.prodNameCheckBox.AutoCheck = true;
            }

            if (!String.IsNullOrEmpty(internalFilename))
            {
                this.intFileNameCheckBox.Checked = true;
                this.intFileNameCheckBox.AutoCheck = true;
            }

            if(!String.IsNullOrEmpty(packagedAppName))
            {
                this.pfnCheckBox.Checked = true;
                this.pfnCheckBox.AutoCheck = true;
            }

            // Unhide the panel
            this.fileAttributeRulePanel.Visible = true;
            this.fileAttributeRulePanel.Location = this.publisherRulePanel.Location; // snap to the loc of pub panel

            // Set color of disabled checkboxes
            SetDisabledCheckBoxesColor();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalFilename"></param>
        /// <param name="fileDescription"></param>
        /// <param name="productName"></param>
        /// <param name="internalFilename"></param>
        /// <param name="packagedAppName"></param>
        private void SetFileHashPanel(byte[] sha1, byte[] sha2)
        {
            this.sha1TextBox.Text = Helper.ConvertHash(sha1);
            this.sha2TextBox.Text = Helper.ConvertHash(sha2);

            // Unhide the panel
            this.hashRulePanel.Visible = true;
            this.hashRulePanel.Location = this.publisherRulePanel.Location; // snap to the loc of pub panel
        }

        /// <summary>
        /// Set the UI for the Path (File and Folder) Panel 
        /// </summary>
        /// <param name="filepath"></param>
        private void SetFilePathPanel(string filepath)
        {
            this.filePathTextBox.Text = filepath;
            this.folderPathTextBox.Text = Path.GetDirectoryName(filepath) + "\\*";

            this.filePathCheckBox.Checked = true;
            this.folderPathCheckBox.Checked = true; 

            // Unhide the panel
            this.filePathRulePanel.Visible = true;
            this.filePathRulePanel.Location = this.publisherRulePanel.Location; // snap to the loc of pub panel

            // Right align text
            if (this.filePathTextBox.TextLength > 0)
            {
                filePathTextBox.SelectionStart = filePathTextBox.TextLength - 1;
                filePathTextBox.ScrollToCaret();
            }

            // Right align text
            if (this.folderPathTextBox.TextLength > 0)
            {
                folderPathTextBox.SelectionStart = folderPathTextBox.TextLength - 1;
                folderPathTextBox.ScrollToCaret();
            }

            // Set color of disabled checkboxes
            SetDisabledCheckBoxesColor();
        }

        /// <summary>
        /// Clears the prior error message from MainForm's UI
        /// </summary>
        private void ClearErrorMsg()
        {
            this._MainWindow.ErrorOnPage = false; 
            this._MainWindow.DisplayInfoText(0);
        }

        /// <summary>
        /// Form painting. Occurs on Form.Refresh, Load and Focus. 
        /// Used for UI element changes for Dark and Light Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventLogRuleConfiguration_Validated(object sender, EventArgs e)
        {
            // Set Controls Color (e.g. Panels, Textboxes, Buttons)
            SetControlsColor();

            // Set Labels Color
            List<Label> labels = new List<Label>();
            GetLabelsRecursive(this, labels);
            SetLabelsColor(labels);

            // Set TextBoxes Color
            List<TextBox> textBoxes = new List<TextBox>();
            GetTextBoxesRecursive(this, textBoxes);
            SetTextBoxesColor(textBoxes);

            // Set Comboboxes Color
            List<ComboBox> comboBoxes = new List<ComboBox>();
            GetComboBoxesRecursive(this, comboBoxes);
            SetComboboxesColor(comboBoxes);

            // Set Checkboxes Color
            List<CheckBox> checkBoxes = new List<CheckBox>();
            GetCheckBoxesRecursive(this, checkBoxes);
            SetCheckBoxesColor(checkBoxes);

            this.CheckBoxes = checkBoxes;
            SetDisabledCheckBoxesColor();

            // Set PolicyType Form back color
            SetFormBackColor();

            // Set the Rules Grid colors
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
                foreach (Control control in this.Controls)
                {
                    // Buttons
                    if (control is Button button
                        && (button.Tag == null || button.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        button.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
                        button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
                        button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
                        button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                        button.ForeColor = System.Drawing.Color.DodgerBlue;
                        button.BackColor = System.Drawing.Color.Transparent;
                    }

                    // Panels
                    else if (control is Panel panel
                        && (panel.Tag == null || panel.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        panel.ForeColor = Color.White;
                        panel.BackColor = Color.FromArgb(15, 15, 15);
                    }

                    // Checkboxes
                    else if (control is TextBox textBox
                        && (textBox.Tag == null || textBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        textBox.ForeColor = Color.White;
                        textBox.BackColor = Color.FromArgb(15, 15, 15);
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
                foreach (Control control in this.Controls)
                {
                    // Buttons
                    if (control is Button button
                        && (button.Tag == null || button.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        button.FlatAppearance.BorderColor = System.Drawing.Color.Black;
                        button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
                        button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(50,30,144,255);
                        button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                        button.ForeColor = System.Drawing.Color.Black;
                        button.BackColor = System.Drawing.Color.WhiteSmoke;
                    }

                    // Panels
                    else if (control is Panel panel
                        && (panel.Tag == null || panel.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        panel.ForeColor = Color.Black;
                        panel.BackColor = Color.White;
                    }

                    // Checkboxes
                    else if (control is TextBox textBox
                        && (textBox.Tag == null || textBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        textBox.ForeColor = Color.Black;
                        textBox.BackColor = Color.White;
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
                        comboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
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
                        comboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
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
                        label.BackColor = Color.Black;
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
        /// Sets the color of the labels defined in the provided List
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
                    }
                }
            }
        }

        /// <summary>
        /// Gets all of the labels on the form recursively
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="labels"></param>
        private void GetCheckBoxesRecursive(Control parent, List<CheckBox> checkBoxes)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is CheckBox checkBox)
                {
                    checkBoxes.Add(checkBox);
                }
                else
                {
                    GetCheckBoxesRecursive(control, checkBoxes);
                }
            }
        }

        /// <summary>
        /// Sets the color of the labels defined in the provided List
        /// </summary>
        /// <param name="labels"></param>
        private void SetCheckBoxesColor(List<CheckBox> checkBoxes)
        {
            // Dark Mode
            if (Properties.Settings.Default.useDarkMode)
            {
                foreach (CheckBox checkBox in checkBoxes)
                {
                    if (checkBox.Tag == null || checkBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        checkBox.ForeColor = Color.White;
                        checkBox.BackColor = Color.FromArgb(15, 15, 15);
                    }
                }
            }

            // Light Mode
            else
            {
                foreach (CheckBox checkBox in checkBoxes)
                {
                    if (checkBox.Tag == null || checkBox.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag)
                    {
                        checkBox.ForeColor = Color.Black;
                        checkBox.BackColor = Color.White;
                    }
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
                eventsDataGridView.BackgroundColor = Color.FromArgb(15, 15, 15);

                // Header
                eventsDataGridView.RowHeadersDefaultCellStyle.BackColor = Color.Black;
                eventsDataGridView.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                eventsDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
                eventsDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                eventsDataGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(26, 82, 118);
                eventsDataGridView.ColumnHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;

                // Borders
                eventsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
                eventsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                eventsDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;

                // Cells
                eventsDataGridView.DefaultCellStyle.BackColor = Color.FromArgb(32, 32, 32);
                eventsDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 24);
                eventsDataGridView.DefaultCellStyle.ForeColor = Color.White;
                eventsDataGridView.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(26, 82, 118);
                eventsDataGridView.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;

                // Grid lines
                eventsDataGridView.GridColor = Color.LightSlateGray;
            }

            // Light Mode
            else
            {
                eventsDataGridView.BackgroundColor = Color.White;

                // Header
                eventsDataGridView.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
                eventsDataGridView.RowHeadersDefaultCellStyle.ForeColor = Color.Black;
                eventsDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
                eventsDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                eventsDataGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(174, 214, 241);
                eventsDataGridView.ColumnHeadersDefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

                // Borders
                eventsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
                eventsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                eventsDataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;

                // Cells
                eventsDataGridView.DefaultCellStyle.BackColor = Color.White;
                eventsDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(236, 240, 241);
                eventsDataGridView.DefaultCellStyle.ForeColor = Color.Black;
                eventsDataGridView.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(174, 214, 241);
                eventsDataGridView.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

                // Grid lines
                eventsDataGridView.GridColor = Color.Black;
            }
        }

        /// <summary>
        /// Sets the color of the checkbox text when enabled changes to disabled
        /// so the text is legible in Dark Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetDisabledCheckBoxesColor()
        {
            // Override the color of the checkbox font from gray to WhiteSmoke 
            // so that the text is legible in Dark Mode

            // Break if not dark mode -- keep default color
            if(!Properties.Settings.Default.useDarkMode)
            {
                return; 
            }

            foreach(CheckBox checkBox in this.CheckBoxes)
            {
                // Set checkbox text to a lighter color for better Dark Mode contrast
                if (!checkBox.AutoCheck)
                {
                    checkBox.ForeColor = Color.Silver;
                }
            } 
        }
    }

    internal static class PolicyEventHelper
    {
        // Counts of file rules created to pipe into IDs
        static public int cPublisherRules = 0;
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
        public static SiPolicy AddSiPolicyPublisherRule(CiEvent ciEvent, SiPolicy siPolicy, int[] uiState)
        {
            bool createFileRule = false; 

            // Create new signer object
            Signer signer = new Signer();
            signer.ID = "ID_SIGNER_A_" + cPublisherRules;
            cPublisherRules++;

            // Create new Certificate Root object and add to CertRoot field
            CertRoot cRoot = new CertRoot();
            cRoot.Type = CertEnumType.TBS;
            cRoot.Value = ciEvent.SignerInfo.IssuerTBSHash;
            signer.CertRoot = cRoot;

            signer.Name = String.Format("Issuing CA = {0}", ciEvent.SignerInfo.IssuerName);

            // Create new CertPublisher object and add CertPublisher field
            if (uiState[1] == 1) // CertPub CN
            {
                CertPublisher cPub = new CertPublisher();
                cPub.Value = ciEvent.SignerInfo.PublisherName;
                signer.CertPublisher = cPub;

                signer.Name = String.Format("Allow CN = {0} issued by {1}", ciEvent.SignerInfo.PublisherName,
                                            ciEvent.SignerInfo.IssuerName);
            }

            // Create new FileAttrib object to link to signer
            FileAttrib fileAttrib = new FileAttrib();
            fileAttrib.ID = "ID_FILEATTRIB_A_" + cFileAttribRules;

            if (uiState[2] == 1) // Original Filename
            {
                fileAttrib.FileName = ciEvent.OriginalFilename;
                createFileRule = true;
            }

            if (uiState[3] == 1) // Version
            {
                fileAttrib.MinimumFileVersion = ciEvent.FileVersion;
                createFileRule = true;
            }

            if (uiState[4] == 1) // Product
            {
                fileAttrib.ProductName = ciEvent.ProductName;
                createFileRule = true;
            }

            if(createFileRule)
            {
                // Link the new FileAttrib object back to the signer
                FileAttribRef fileAttribRef = new FileAttribRef();
                fileAttribRef.RuleID = fileAttrib.ID;

                signer.FileAttribRef = new FileAttribRef[1];
                signer.FileAttribRef[0] = fileAttribRef;
                cFileAttribRules++;

                // Add the FileAttributeReference to SiPolicy
                siPolicy = AddSiPolicyFileAttrib(fileAttrib, siPolicy);
            }

            // Add the allow signer to Signers and the product signers section with Windows Signing Scenario
            siPolicy = AddSiPolicyAllowSigner(signer, siPolicy);

            // Add CiSigner - Github Issue #161
            // Usermode rule you are creating a rule for, you need to add signer to cisigners.
            // Kernel mode rule, don't add signer to cisigners
            // If you don't know always add to cisigners.
            Signer[] signers = new Signer[] { signer };
            siPolicy = PolicyHelper.AddSiPolicyCiSigner(signers, siPolicy);

            return siPolicy;
        }


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
            if(!String.IsNullOrEmpty(ciEvent.CorrelationId))
            {
                allowRule.FriendlyName = String.Format("Allow {0} by file attributes; Correlation Id: {1}",
                                                        ciEvent.FileName, ciEvent.CorrelationId);
            }
            else
            {
                allowRule.FriendlyName = String.Format("Allow {0} by file attributes; Reference Device Id: {1}; Timestamp: {2}",
                                                        ciEvent.FileName, ciEvent.DeviceId, ciEvent.Timestamp);
            }
            
            allowRule.ID = "ID_ALLOW_B_" + cFileAttribRules.ToString();
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
                if (!String.IsNullOrEmpty(ciEvent.CorrelationId))
                {
                    allowRule.FriendlyName = String.Format("Allow path: {0}; Correlation Id: {1}",
                                                            allowRule.FilePath, ciEvent.CorrelationId);
                }
                else
                {
                    allowRule.FriendlyName = String.Format("Allow path: {0}; Reference Device Id: {1}; Timestamp: {2}",
                                                            allowRule.FilePath, ciEvent.DeviceId, ciEvent.Timestamp);
                }

                allowRule.ID = "ID_ALLOW_C_" + cFilePathRules.ToString();
                cFilePathRules++;
                allowPathRules.Add(allowRule);
            }

            // FolderPath
            if (uiState[1] == 1)
            {
                Allow allowRule = new Allow();
                allowRule.FilePath = Path.GetDirectoryName(ciEvent.FilePath) + "\\*";
                if (!String.IsNullOrEmpty(ciEvent.CorrelationId))
                {
                    allowRule.FriendlyName = String.Format("Allow path: {0}; Correlation Id: {1}",
                                                            allowRule.FilePath, ciEvent.CorrelationId);
                }
                else
                {
                    allowRule.FriendlyName = String.Format("Allow path: {0}; Reference Device Id: {1}; Timestamp: {2}",
                                                            allowRule.FilePath, ciEvent.DeviceId, ciEvent.Timestamp);
                }
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
            
            // SHA1 PE Hash
            Allow allowRule = new Allow();
            allowRule.Hash = ciEvent.SHA1;
            if (!String.IsNullOrEmpty(ciEvent.CorrelationId))
            {
                allowRule.FriendlyName = String.Format("{0} SHA1 hash allow rule; Correlation Id: {1}",
                                                        ciEvent.FileName, ciEvent.CorrelationId);
            }
            else
            {
                allowRule.FriendlyName = String.Format("{0} SHA1 hash allow rule; Reference Device Id: {1}; Timestamp: {2}",
                                                        ciEvent.FileName, ciEvent.DeviceId, ciEvent.Timestamp);
            }
            allowRule.ID = String.Format("ID_ALLOW_E_{0}_{1}", cFileHashRules, "SHA1");
            allowHashRules.Add(allowRule);
            cFileHashRules++;

            // SHA256 PE Hash
            allowRule = new Allow();
            allowRule.Hash = ciEvent.SHA2;
            if (!String.IsNullOrEmpty(ciEvent.CorrelationId))
            {
                allowRule.FriendlyName = String.Format("{0} SHA256 hash allow rule; Correlation Id: {1}",
                                                        ciEvent.FileName, ciEvent.CorrelationId);
            }
            else
            {
                allowRule.FriendlyName = String.Format("{0} SHA256 hash allow rule; Reference Device Id: {1}; Timestamp: {2}",
                                                        ciEvent.FileName, ciEvent.DeviceId, ciEvent.Timestamp);
            }
            allowRule.ID = String.Format("ID_ALLOW_E_{0}_{1}", cFileHashRules, "SHA256");
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

        /// <summary>
        /// Handles adding the new FileAttribute object to the provided siPolicy
        /// </summary>
        /// <param name="fileAttrib"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        private static SiPolicy AddSiPolicyFileAttrib(FileAttrib fileAttrib, SiPolicy siPolicy)
        {
            // Copy and replace FileRules section in SiPolicy
            // FileRules always initalized - no need to check if null
            object[] fileRulesCopy = new object[siPolicy.FileRules.Length + 1];
            for (int i = 0; i < fileRulesCopy.Length - 1; i++)
            {
                fileRulesCopy[i] = siPolicy.FileRules[i];
            }

            fileRulesCopy[fileRulesCopy.Length - 1] = fileAttrib;
            siPolicy.FileRules = fileRulesCopy;

            return siPolicy;
        }

        /// <summary>
        /// Handles adding the new AllowSignerobject to the provided siPolicy
        /// </summary>
        /// <param name="signer"></param>
        /// <param name="siPolicy"></param>
        /// <returns></returns>
        private static SiPolicy AddSiPolicyAllowSigner(Signer signer, SiPolicy siPolicy)
        {
            // Copy the SiPolicy signer object and add the signer param to the field
            Signer[] signersCopy = new Signer[siPolicy.Signers.Length + 1];
            for (int i = 0; i < signersCopy.Length - 1; i++)
            {
                signersCopy[i] = siPolicy.Signers[i];
            }

            signersCopy[signersCopy.Length - 1] = signer;
            siPolicy.Signers = signersCopy;

            // Create an AllowedSigner object to add to the SiPolicy ProductSigners section
            AllowedSigner allowedSigner = new AllowedSigner();
            allowedSigner.SignerId = signer.ID;

            // Copy and replace
            if (siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners == null)
            {
                siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners = new AllowedSigners();
                siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners.AllowedSigner = new AllowedSigner[1];
                siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners.AllowedSigner[0] = allowedSigner;
            }
            else
            {
                int cAllowedSigners = siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners.AllowedSigner.Length;
                AllowedSigner[] allowedSigners = new AllowedSigner[cAllowedSigners + 1];

                for (int i = 0; i < cAllowedSigners; i++)
                {
                    allowedSigners[i] = siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners.AllowedSigner[i];
                }

                allowedSigners[cAllowedSigners] = allowedSigner;
                siPolicy.SigningScenarios[1].ProductSigners.AllowedSigners.AllowedSigner = allowedSigners;
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
            this.Filename = String.IsNullOrEmpty(filename) ? String.Empty : filename;
            this.Product = String.IsNullOrEmpty(product) ? String.Empty : product;
            this.PolicyName = String.IsNullOrEmpty(policyName) ? String.Empty : policyName;
            this.Publisher = String.IsNullOrEmpty(publisher) ? String.Empty : publisher;
        }
    }
}
