using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace WDAC_Wizard
{
    public partial class EKUPanel : Form
    {
        private int RowSelected; // Data grid row number selected by the user 
        private int rowInEdit = -1;
        private EKUDisplayObject displayObjectInEdit;

        // Declare an ArrayList to serve as the data store. 
        private System.Collections.ArrayList displayObjects =
            new System.Collections.ArrayList();

        private OidCollection EKUCollection { get; set; } // EKUs to add from the cert
        private List<bool> EkusPosToAdd { get; set; }
        private bool FormClosingAutoInitiated; 

        CustomRuleConditionsPanel _customRulesControl; 

        public EKUPanel(CustomRuleConditionsPanel customRuleConditionsPanel)
        {
            this._customRulesControl = customRuleConditionsPanel;
            this.EKUCollection = new OidCollection(); 
            this.EkusPosToAdd = new List<bool>();
            this.FormClosingAutoInitiated = false; 
            InitializeComponent();
        }

        /// <summary>
        /// Virtual method for Cell values required 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EKUDataGridViewCellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            // If this is the row for new records, no values are needed.
            if (e.RowIndex == this.ekuDataGridView.RowCount - 1) return;
            if (e.RowIndex >= this.displayObjects.Count) return; 

            EKUDisplayObject displayObject = null;

            // Store a reference to the Customer object for the row being painted.
            if (e.RowIndex == rowInEdit)
            {
                displayObject = this.displayObjectInEdit;
            }
            else
            {
                displayObject = (EKUDisplayObject)this.displayObjects[e.RowIndex];
            }

            // Set the cell value to paint using the Customer object retrieved.
            switch (this.ekuDataGridView.Columns[e.ColumnIndex].Name)
            {
                case "Column_ToAdd":
                    e.Value = displayObject.ToAdd; 
                    break;

                case "Column_EKUFriendlyName":
                    e.Value = displayObject.EKUFriendlyName;
                    break;

                case "Column_EKUValue":
                    e.Value = displayObject.EKUValue;
                    break;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCreateEKURules(object sender, EventArgs e)
        {
            // Add selected OIDs stored in ekusToAdd
            // Notifies CustomRuleConditionsPanel that form is closing
            int cEkus = 0; 
            OidCollection oidsToAdd = new OidCollection();  

            for(int i=0; i< this.EkusPosToAdd.Count; i++)
            {
                if(this.EkusPosToAdd[i] == true)
                {
                    if (this.EKUCollection[i] != null)
                    {
                        if (IsValidEku(this.EKUCollection[i]))
                        {
                            oidsToAdd.Add(this.EKUCollection[i]);
                            cEkus++;
                        }
                    }
                }
            }

            // Assert that at least 1 EKU must be selected
            if (cEkus == 0)
            {
                MessageBox.Show(Properties.Resources.InvalidEkuSelection, "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetEkuCollection(oidsToAdd);

            // Close form 
            this.FormClosingAutoInitiated = true; 
            this.Close(); 
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetEkuCollection(OidCollection oidCollection)
        {
            if(this._customRulesControl != null)
            {
                this._customRulesControl.SetOidCollection(oidCollection); 
            }
        }

        /// <summary>
        /// On Panel Loading event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EKUPanelOnLoad(object sender, EventArgs e)
        {
            // Set the EKU table with values from the oidCollection
            SetEKUTable(); 
        }

        /// <summary>
        /// Sets the EKU Table with values defined in the CertEKUs structure from CustomRulesControls
        /// </summary>
        private void SetEKUTable()
        {
            string toAdd = "Included"; // default setting all values to true
            string friendlyName;
            string value;

            OidCollection oidCollection = this._customRulesControl.GetOidCollection(); 
            if(oidCollection == null)
            {
                return; 
            }

            foreach (var eku in oidCollection)
            {
                friendlyName = eku.FriendlyName;
                value = eku.Value;

                if(value != Properties.Resources.CodeSigningEKUValue)
                {
                    this.displayObjects.Add(new EKUDisplayObject(toAdd, friendlyName, value));
                    this.ekuDataGridView.RowCount += 1;

                    // Add to struct
                    this.EKUCollection.Add(eku);
                    this.EkusPosToAdd.Add(true); 
                }
            }
        }

        /// <summary>
        /// Fires on form closing event. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFormClosing(object sender, EventArgs e)
        {
            // If user closing the form, need to clean up the UI on CustomRuleConditionsPanel
            if(!this.FormClosingAutoInitiated)
            {
                this._customRulesControl.EkuPanelClosing(); 
            }
        }

        /// <summary>
        /// Edits the GridView cell when user modifies the table contents and submits modification (i.e. selects enter)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EKUDataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // These are true during first paint of the control
            // Ignore any Checkbox change event. These are picked up in the StateChange handler
            if(e.RowIndex < 0 || e.ColumnIndex < 1)
            { 
                return; 
            }

            // Get modifyable text field values
            string friendlyName = this.ekuDataGridView[1, e.RowIndex].EditedFormattedValue.ToString(); 
            string ekuValue = this.ekuDataGridView[2, e.RowIndex].EditedFormattedValue.ToString();

            // Check if a DisplayObject exist. If true, update all parameters. Else, create one
            if (e.RowIndex >= this.displayObjects.Count)
            {
                this.displayObjects.Add(new EKUDisplayObject("-", friendlyName, ekuValue)); // force false to trigger validation upon enablement
            }
            else
            {
                EKUDisplayObject displayObject = (EKUDisplayObject)this.displayObjects[e.RowIndex];
                displayObject.EKUFriendlyName = friendlyName;
                displayObject.EKUValue = ekuValue; 
            }

            // Check if an EKUCollection exists. If true, update the parameters. Else, create one. ToAdd is handled by _Click eventhandler
            if (e.RowIndex >= this.EKUCollection.Count)
            {
                this.EKUCollection.Add(new Oid(ekuValue, friendlyName));
            }
            else
            {
                this.EKUCollection[e.RowIndex].FriendlyName = friendlyName;
                this.EKUCollection[e.RowIndex].Value = ekuValue;
            }
        }

        /// <summary>
        /// Sets the text on the error label
        /// </summary>
        /// <param name="dispString"></param>
        private void ShowErrorDialog(string dispString)
        {
            MessageBox.Show(dispString, "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Fires on any state change in the table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EKUDataGridViewStateChange(object sender, EventArgs e)
        {
            int currRow = this.ekuDataGridView.CurrentCell.RowIndex;
            int currCol = this.ekuDataGridView.CurrentCell.ColumnIndex; 

            if(currCol != 0)
            {
                return; 
            }

            if(currRow >= this.EkusPosToAdd.Count)
            {
                this.EkusPosToAdd.Add(false); 
            }

            string enabledValue = this.ekuDataGridView[0, currRow].EditedFormattedValue.ToString();
            if(enabledValue == "Include")
            {
                this.EkusPosToAdd[currRow] = true;
            }
            else
            {
                this.EkusPosToAdd[currRow] = false;
            }
        }

        private bool IsValidEku(Oid oid)
        {
            bool isValidEku = true;

            // Assert non null/empty EKU value cell
            if (String.IsNullOrEmpty(oid.Value))
            {
                ShowErrorDialog(Properties.Resources.NullEkuValue);
                return false;
            }

            // Assert properly formatted EKU
            if (String.IsNullOrEmpty(Helper.EKUValueToTLVEncoding(oid.Value)))
            {
                ShowErrorDialog(oid.Value + Properties.Resources.InvalidEKUFormat_Error);
                return false;
            }
            return isValidEku; 
        }
    }

    // Class for the datastore
    public class EKUDisplayObject
    {
        //public string ToAdd { get; set; }
        public string ToAdd { get; set; }

        public string EKUFriendlyName { get; set; }
        public string EKUValue { get; set; }

        public EKUDisplayObject()
        {
            this.ToAdd = "-"; 
            this.EKUFriendlyName = String.Empty;
            this.EKUValue = String.Empty;
        }
        public EKUDisplayObject(string toAdd, string friendlyName, string value)
        {
            this.ToAdd = toAdd;
            this.EKUFriendlyName = friendlyName;
            this.EKUValue = value; 
        }
    }
}
