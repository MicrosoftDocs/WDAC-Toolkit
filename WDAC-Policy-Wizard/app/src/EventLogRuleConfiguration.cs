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
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

            CiEvents = pMainWindow.CiEvents; 
            _MainWindow = pMainWindow; 
            siPolicy = Helper.DeserializeXMLStringtoPolicy(Properties.Resources.Empty_Supplemental);

            // Set the Base Policy ID of the supplemental policy
            siPolicy.BasePolicyID = PolicyEventHelper.GetPolicyBaseId(siPolicy, CiEvents);

            DisplayObjects = new List<EventDisplayObject>();

            SelectedRow = 0;
            RuleIdsToAdd = new List<int>();

            // Set error flag - Bug #234
            _MainWindow.ErrorOnPage = true;
            _MainWindow.ErrorMsg = Properties.Resources.InvalidEventRulesCreated;

            CheckBoxes = new List<CheckBox>(); 
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
            ruleTypeComboBox.SelectedIndex = 0;
            publisherRulePanel.Visible = true;

            // Set the table, let's eat
            DisplayEvents();
        }

        /// <summary>
        /// Creates a new display object for the DataGridView for each CiEvent object
        /// </summary>
        private void DisplayEvents()
        {
            foreach(CiEvent ciEvent in CiEvents)
            {
                // Create one row per ciEvent per signer Event
                // File signed by 3 signers will create 3 rows/rules
                EventDisplayObject dpObject = new EventDisplayObject(
                        ciEvent.EventId.ToString(),
                        ciEvent.FileName,
                        ciEvent.ProductName,
                        ciEvent.PolicyName,
                        ciEvent.SignerInfo.PublisherName);

                DisplayObjects.Add(dpObject);
                eventsDataGridView.RowCount += 1;
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
            EventDisplayObject dp = DisplayObjects[SelectedRow];
            CiEvent ciEvent = CiEvents[SelectedRow];

            // Check if rule was already added
            if(dp.Action != null &&
                dp.Action.Contains("Added to policy"))
            {
                DialogResult res = MessageBox.Show("This allow rule was already created and will be added to the resulting policy. Do you want to create the rule again?",
                                                   "Wizard - New Rule Creation",
                                                   MessageBoxButtons.YesNo,
                                                   MessageBoxIcon.Warning);
                if (res == DialogResult.No)
                {
                    return;
                }
            }

            switch(ruleTypeComboBox.SelectedIndex)
            {
                case 0: // Publisher
                    if(IsValidPublisherUiState())
                    {
                        dp.Action = "Added to policy";
                        siPolicy = PolicyEventHelper.AddSiPolicyPublisherRule(ciEvent, siPolicy, PublisherUIState);
                        _MainWindow.EventLogPolicy = siPolicy;
                        ClearErrorMsg();
                    }
                    break;

                case 1: // File Path Rule
                    if(IsValidFilePathUiState())
                    {
                        dp.Action = "Added to policy"; 
                        siPolicy = PolicyEventHelper.AddSiPolicyFilePathRule(ciEvent, siPolicy, FilePathUIState);
                        _MainWindow.EventLogPolicy = siPolicy;
                        ClearErrorMsg();
                    }
                    break; 

                case 2: // File Attributes Rule
                case 3:
                    if(IsValidFileAttributesUiState())
                    {
                        dp.Action = "Added to policy"; 
                        siPolicy = PolicyEventHelper.AddSiPolicyFileAttributeRule(ciEvent, siPolicy, FileAttributesUIState);
                        _MainWindow.EventLogPolicy = siPolicy;
                        ClearErrorMsg();
                    }
                    break;

                case 4: // Hash rule
                    if(IsValidHashRuleUiState())
                    {
                        dp.Action = "Added to policy";
                        siPolicy = PolicyEventHelper.AddSiPolicyHashRules(ciEvent, siPolicy);
                        _MainWindow.EventLogPolicy = siPolicy;
                        ClearErrorMsg();
                    }
                    break; 
            }

            eventsDataGridView.Refresh();
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
            if (selectedRow >= CiEvents.Count)
            {
                return;
            }

            // Set the UI
            ResetCustomRulesPanel();
            SetPublisherPanel(CiEvents[selectedRow].SignerInfo.IssuerName,
                              CiEvents[selectedRow].SignerInfo.PublisherName,
                              CiEvents[selectedRow].OriginalFilename,
                              CiEvents[selectedRow].FileVersion,
                              CiEvents[selectedRow].ProductName);

            SelectedRow = selectedRow;
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
            if(selectedRow >= CiEvents.Count)
            {
                return;
            }

            // Header selected, sort table
            if(selectedRow == -1)
            {
                SortDataGrid(sender, e);
                return;
            }
            
            SetPublisherPanel(CiEvents[selectedRow].SignerInfo.IssuerName,
                              CiEvents[selectedRow].SignerInfo.PublisherName,
                              CiEvents[selectedRow].OriginalFilename,
                              CiEvents[selectedRow].FileVersion,
                              CiEvents[selectedRow].ProductName);

            SelectedRow = selectedRow;
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
            if (eventsDataGridView.Columns[e.ColumnIndex].Tag != (object)"Sorted")
            {
                SortDisplayObjects(e.ColumnIndex, false);
                ResetColTags();
                eventsDataGridView.Columns[e.ColumnIndex].Tag = "Sorted";
            }
            else
            {
                SortDisplayObjects(e.ColumnIndex, true);
            }
            eventsDataGridView.Refresh();
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
                        DisplayObjects.Sort((x, y) => x.EventId.CompareTo(y.EventId));
                        CiEvents.Sort((x, y) => x.EventId.CompareTo(y.EventId));
                        break;

                    case 2:
                        DisplayObjects.Sort((x, y) => x.Filename.CompareTo(y.Filename));
                        CiEvents.Sort((x, y) => x.FileName.CompareTo(y.FileName));
                        break;

                    case 3:
                        DisplayObjects.Sort((x, y) => x.Product.CompareTo(y.Product));
                        CiEvents.Sort((x, y) => x.ProductName.CompareTo(y.ProductName));
                        break;

                    case 4:
                        DisplayObjects.Sort((x, y) => x.PolicyName.CompareTo(y.PolicyName));
                        CiEvents.Sort((x, y) => x.PolicyName.CompareTo(y.PolicyName));
                        break;

                    case 5:
                        DisplayObjects.Sort((x, y) => x.Publisher.CompareTo(y.Publisher));
                        CiEvents.Sort((x, y) => x.SignerInfo.PublisherName.CompareTo(y.SignerInfo.PublisherName));
                        break;
                }
            }
            else
            {
                DisplayObjects.Reverse();
                CiEvents.Reverse();
            }

            _MainWindow.CiEvents = CiEvents;
        }

        private void ResetColTags()
        {
            for(int i = 0; i< eventsDataGridView.Columns.Count; i++)
            {
                eventsDataGridView.Columns[i].Tag = ""; 
            }
        }

        /// <summary>
        /// Resets the custom rules panel 
        /// </summary>this.SelectedRow = selectedRow; 
        private void ResetCustomRulesPanel()
        {
            // Uncheck boxes
            publisherCheckBox.Checked = false;
            filenameCheckBox.Checked = false;
            versionCheckBox.Checked = false;
            productCheckBox.Checked = false;

            publisherCheckBox.AutoCheck = false;
            filenameCheckBox.AutoCheck = false;
            versionCheckBox.AutoCheck = false;
            productCheckBox.AutoCheck = false;

            // Clear all textboxes
            issuerTextBox.Clear();
            publisherTextBox.Clear();
            filenameTextBox.Clear();
            versionTextBox.Clear();
            productTextBox.Clear();

            // Dropdown
            ruleTypeComboBox.SelectedIndex = 0; 
        }


        /// <summary>
        /// Gets the state of the checkboxes and determines whether the UI state is valid
        /// </summary>
        /// <returns></returns>
        public bool IsValidPublisherUiState()
        {
            // UI States:
            // this.PublisherUIState[0] == 1 always; it cannot be modified
            PublisherUIState[1] = publisherCheckBox.Checked == true ? 1 : 0;
            PublisherUIState[2] = filenameCheckBox.Checked == true ? 1 : 0;
            PublisherUIState[3] = versionCheckBox.Checked == true ? 1 : 0;
            PublisherUIState[4] = productCheckBox.Checked == true ? 1 : 0;

            if(String.IsNullOrEmpty(issuerTextBox.Text) 
                || issuerTextBox.Text == Properties.Resources.BadEventPubValue)
            {
                // Log exception error and throw error to user
                MessageBox.Show(this,String.Format("The Issuer is invalid for rule creation. The issuer cannot be empty or '{0}'", Properties.Resources.BadEventPubValue), 
                                                   "New Rule Creation Error", 
                                                   MessageBoxButtons.OK, 
                                                   MessageBoxIcon.Error);

                Invalidate();

                Logger.Log.AddWarningMsg("Event Log Config - Invalid publisher rule with Issuer = Unknown");
                return false; 
            }

            if (PublisherUIState[1] == 1 && (String.IsNullOrEmpty(publisherTextBox.Text) 
                || publisherTextBox.Text == Properties.Resources.BadEventPubValue))
            {
                DialogResult res = MessageBox.Show(String.Format("The Publisher is invalid for rule creation. The Publisher cannot be empty '{0}'", Properties.Resources.BadEventPubValue),
                                                   "New Rule Creation Error",
                                                   MessageBoxButtons.OK,
                                                   MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Invalid publisher rule with Publisher = Unknown");
                return false;
            }

            if (PublisherUIState[2] == 1 && String.IsNullOrEmpty(filenameTextBox.Text))
            {
                DialogResult res = MessageBox.Show(String.Format("The Filename is invalid for rule creation. The Filename cannot be empty if its checkbox is selected."),
                                                   "New Rule Creation Error",
                                                   MessageBoxButtons.OK,
                                                   MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Empty Filename");
                return false;
            }

            if (PublisherUIState[3] == 1 && String.IsNullOrEmpty(versionCheckBox.Text))
            {
                DialogResult res = MessageBox.Show(String.Format("The Version field is invalid for rule creation. The Version field cannot be empty if its checkbox is selected."),
                                                   "New Rule Creation Error",
                                                   MessageBoxButtons.OK,
                                                   MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Empty Version field");
                return false;
            }

            if (PublisherUIState[4] == 1 && String.IsNullOrEmpty(productTextBox.Text))
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
            FileAttributesUIState[0] = origFileNameCheckBox.Checked == true ? 1 : 0;
            FileAttributesUIState[1] = fileDescCheckBox.Checked == true ? 1 : 0;
            FileAttributesUIState[2] = prodNameCheckBox.Checked == true ? 1 : 0;
            FileAttributesUIState[3] = intFileNameCheckBox.Checked == true ? 1 : 0;
            FileAttributesUIState[4] = pfnCheckBox.Checked == true ? 1 : 0;

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

            if (FileAttributesUIState[0] == 1 && String.IsNullOrEmpty(origFileNameTextBox.Text))
            {
                MessageBox.Show(String.Format("The Original filename is invalid for rule creation. The original filename cannot be empty if its checkbox is selected."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Original filename name");
                return false;
            }

            if (FileAttributesUIState[1] == 1 && String.IsNullOrEmpty(fileDescTextBox.Text))
            {
                MessageBox.Show(String.Format("The File description is invalid for rule creation. The File description field cannot be empty if its checkbox is selected."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - File description");
                return false;
            }

            if (FileAttributesUIState[2] == 1 && String.IsNullOrEmpty(prodNameTextBox.Text))
            {
                MessageBox.Show(String.Format("The Product name is invalid for rule creation. The Product name field cannot be empty if its checkbox is selected."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Product name ");
                return false;
            }

            if (FileAttributesUIState[3] == 1 && String.IsNullOrEmpty(intFileNameTextBox.Text))
            {
                MessageBox.Show(String.Format("The Internal filename is invalid for rule creation. The Internal filename field cannot be empty if its checkbox is selected."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - Internal filename");
                return false;
            }

            if (FileAttributesUIState[4] == 1 && String.IsNullOrEmpty(pfnTextBox.Text))
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
            FilePathUIState[0] = filePathCheckBox.Checked == true ? 1 : 0;
            FilePathUIState[1] = folderPathCheckBox.Checked == true ? 1 : 0;

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

            if (FilePathUIState[0] == 1 && String.IsNullOrEmpty(filePathTextBox.Text))
            {
                MessageBox.Show(String.Format("The file path field is invalid for rule creation. The file path field cannot be empty if its checkbox is selected."),
                    "New Rule Creation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                Logger.Log.AddWarningMsg("Event Log Config - empty file path field");
                return false;
            }
            
            if(FilePathUIState[1] == 1 && String.IsNullOrEmpty(folderPathTextBox.Text))
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
            if (String.IsNullOrEmpty(sha1TextBox.Text)
                && String.IsNullOrEmpty(sha2TextBox.Text))
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
            if (e.RowIndex == eventsDataGridView.RowCount - 1) return;

            EventDisplayObject displayObject = null;

            // Store a reference to the Customer object for the row being painted.
            if (e.RowIndex == rowInEdit)
            {
                displayObject = displayObjectInEdit;
            }
            else
            {
                displayObject = (EventDisplayObject)DisplayObjects[e.RowIndex];
            }

            // Set the cell value to paint using the Customer object retrieved.
            switch (eventsDataGridView.Columns[e.ColumnIndex].Name)
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
            publisherRulePanel.Visible = false;
            fileAttributeRulePanel.Visible = false;
            filePathRulePanel.Visible = false; 
            hashRulePanel.Visible = false; 
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

            switch (ruleTypeComboBox.SelectedIndex)
            {
                case 0: // Publisher
                    SetPublisherPanel(CiEvents[SelectedRow].SignerInfo.IssuerName,
                                      CiEvents[SelectedRow].SignerInfo.PublisherName,
                                      CiEvents[SelectedRow].OriginalFilename,
                                      CiEvents[SelectedRow].FileVersion,
                                      CiEvents[SelectedRow].ProductName);
                    break;

                case 1: // Path
                    SetFilePathPanel(CiEvents[SelectedRow].FilePath);
                    break;

                case 2: // File Attributes
                case 3: // Package Family Name
                    SetFileAttributesPanel(CiEvents[SelectedRow].OriginalFilename,
                                           CiEvents[SelectedRow].FileDescription,
                                           CiEvents[SelectedRow].ProductName,
                                           CiEvents[SelectedRow].InternalFilename,
                                           CiEvents[SelectedRow].PackageFamilyName);
                    break;

                case 4: //FileHash
                    SetFileHashPanel(CiEvents[SelectedRow].SHA1,
                                     CiEvents[SelectedRow].SHA2);
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
            issuerTextBox.Text = issuer;
            publisherTextBox.Text = publisher;
            filenameTextBox.Text = filename;
            versionTextBox.Text = version;
            productTextBox.Text = product;

            // Default checkbox checked and enabled states
            if (!String.IsNullOrEmpty(publisher))
            {
                publisherCheckBox.Checked = true;
                publisherCheckBox.AutoCheck = true;
            }

            if (!String.IsNullOrEmpty(filename))
            {
                filenameCheckBox.Checked = true;
                filenameCheckBox.AutoCheck = true;
            }

            if (!String.IsNullOrEmpty(version))
            {
                versionCheckBox.Checked = true;
                versionCheckBox.AutoCheck = true;
            }

            if (!String.IsNullOrEmpty(product))
            {
                productCheckBox.Checked = true;
                productCheckBox.AutoCheck = true;
            }

            // Unhide the panel
            publisherRulePanel.Visible = true;

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
            origFileNameTextBox.Text = originalFilename;
            fileDescTextBox.Text = fileDescription;
            prodNameTextBox.Text = productName;
            intFileNameTextBox.Text = internalFilename;
            pfnTextBox.Text = packagedAppName;

            // Default checkbox checked and enabled states
            if (!String.IsNullOrEmpty(originalFilename))
            {
                origFileNameCheckBox.Checked = true;
                origFileNameCheckBox.AutoCheck = true;
            }

            if (!String.IsNullOrEmpty(fileDescription))
            {
                fileDescCheckBox.Checked = true;
                fileDescCheckBox.AutoCheck = true;
            }

            if (!String.IsNullOrEmpty(productName))
            {
                prodNameCheckBox.Checked = true;
                prodNameCheckBox.AutoCheck = true;
            }

            if (!String.IsNullOrEmpty(internalFilename))
            {
                intFileNameCheckBox.Checked = true;
                intFileNameCheckBox.AutoCheck = true;
            }

            if(!String.IsNullOrEmpty(packagedAppName))
            {
                pfnCheckBox.Checked = true;
                pfnCheckBox.AutoCheck = true;
            }

            // Unhide the panel
            fileAttributeRulePanel.Visible = true;
            fileAttributeRulePanel.Location = publisherRulePanel.Location; // snap to the loc of pub panel

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
            sha1TextBox.Text = Helper.ConvertHash(sha1);
            sha2TextBox.Text = Helper.ConvertHash(sha2);

            // Unhide the panel
            hashRulePanel.Visible = true;
            hashRulePanel.Location = publisherRulePanel.Location; // snap to the loc of pub panel
        }

        /// <summary>
        /// Set the UI for the Path (File and Folder) Panel 
        /// </summary>
        /// <param name="filepath"></param>
        private void SetFilePathPanel(string filepath)
        {
            filePathTextBox.Text = filepath;
            folderPathTextBox.Text = Path.GetDirectoryName(filepath) + "\\*";

            filePathCheckBox.Checked = true;
            folderPathCheckBox.Checked = true; 

            // Unhide the panel
            filePathRulePanel.Visible = true;
            filePathRulePanel.Location = publisherRulePanel.Location; // snap to the loc of pub panel

            // Right align text
            if (filePathTextBox.TextLength > 0)
            {
                filePathTextBox.SelectionStart = filePathTextBox.TextLength - 1;
                filePathTextBox.ScrollToCaret();
            }

            // Right align text
            if (folderPathTextBox.TextLength > 0)
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
            _MainWindow.ErrorOnPage = false; 
            _MainWindow.DisplayInfoText(0);
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

            CheckBoxes = checkBoxes;
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
                foreach (Control control in Controls)
                {
                    // Buttons
                    if (control is Button button
                        && (button.Tag == null || button.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        button.FlatAppearance.BorderColor = Color.DodgerBlue;
                        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(50,30,144,255);
                        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(50,30,144,255);
                        button.FlatStyle = FlatStyle.Flat;
                        button.ForeColor = Color.DodgerBlue;
                        button.BackColor = Color.Transparent;
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
                foreach (Control control in Controls)
                {
                    // Buttons
                    if (control is Button button
                        && (button.Tag == null || button.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        button.FlatAppearance.BorderColor = Color.Black;
                        button.FlatAppearance.MouseDownBackColor = Color.FromArgb(50,30,144,255);
                        button.FlatAppearance.MouseOverBackColor = Color.FromArgb(50,30,144,255);
                        button.FlatStyle = FlatStyle.Flat;
                        button.ForeColor = Color.Black;
                        button.BackColor = Color.WhiteSmoke;
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
                        comboBox.FlatStyle = FlatStyle.Flat;
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
                        comboBox.FlatStyle = FlatStyle.Standard;
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
                eventsDataGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 82, 118);
                eventsDataGridView.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

                // Borders
                eventsDataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                eventsDataGridView.BorderStyle = BorderStyle.Fixed3D;
                eventsDataGridView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

                // Cells
                eventsDataGridView.DefaultCellStyle.BackColor = Color.FromArgb(32, 32, 32);
                eventsDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 24);
                eventsDataGridView.DefaultCellStyle.ForeColor = Color.White;
                eventsDataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 82, 118);
                eventsDataGridView.DefaultCellStyle.SelectionForeColor = Color.White;

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
                eventsDataGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(174, 214, 241);
                eventsDataGridView.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

                // Borders
                eventsDataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                eventsDataGridView.BorderStyle = BorderStyle.Fixed3D;
                eventsDataGridView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

                // Cells
                eventsDataGridView.DefaultCellStyle.BackColor = Color.White;
                eventsDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(236, 240, 241);
                eventsDataGridView.DefaultCellStyle.ForeColor = Color.Black;
                eventsDataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(174, 214, 241);
                eventsDataGridView.DefaultCellStyle.SelectionForeColor = Color.Black;

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

            foreach(CheckBox checkBox in CheckBoxes)
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

        /// <summary>
        /// Gets the Base Policy ID of the supplemental policy being created using the 
        /// most common instance of the Policy GUID from the CiEvents
        /// </summary>
        internal static string GetPolicyBaseId(SiPolicy siPolicy, List<CiEvent> ciEvents)
        {
            // Get all the Policy GUIDs from the CiEvents and store counts
            Dictionary<string, int> policyGuidCounts = new Dictionary<string, int>();

            foreach(var ciEvent in ciEvents)
            {
                if (ciEvent.PolicyGUID == null)
                    continue; 

                if (policyGuidCounts.ContainsKey(ciEvent.PolicyGUID))
                {
                    policyGuidCounts[ciEvent.PolicyGUID] += 1; // increment count
                }
                else
                {
                    policyGuidCounts.Add(ciEvent.PolicyGUID, 1);
                }
            }

            // Get policy GUID with largest event count
            string mostCommonGuid = Guid.NewGuid().ToString();
            int eventCount = 0;

            foreach(var key in policyGuidCounts.Keys)
            {
                // Update leading GUID and event count
                if (policyGuidCounts[key] > eventCount)
                {
                    mostCommonGuid = key;
                    eventCount = policyGuidCounts[key];
                }
            }

            // Check for curly braces
            if(mostCommonGuid.Contains('{'))
            {
                return mostCommonGuid.ToUpperInvariant(); 
            }

            // Return the most common GUID
            return "{" + mostCommonGuid.ToUpperInvariant() + "}";
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
            Action = string.Empty;
            EventId = string.Empty;
            Filename = string.Empty;
            Product = string.Empty;
            PolicyName = string.Empty;
            Publisher = string.Empty;
        }

        public EventDisplayObject(string eventId, string filename, string product, string policyName, string publisher)
        {
            Action = "   ---   ";
            EventId = eventId;
            Filename = String.IsNullOrEmpty(filename) ? String.Empty : filename;
            Product = String.IsNullOrEmpty(product) ? String.Empty : product;
            PolicyName = String.IsNullOrEmpty(policyName) ? String.Empty : policyName;
            Publisher = String.IsNullOrEmpty(publisher) ? String.Empty : publisher;
        }
    }
}
