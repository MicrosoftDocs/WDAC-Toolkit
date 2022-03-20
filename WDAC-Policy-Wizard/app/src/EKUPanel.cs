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
                    oidsToAdd.Add(this.EKUCollection[i]);
                    cEkus++; 
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
            bool toAdd = true; // default setting all values to true
            string friendlyName;
            string value;

            foreach (var eku in this._customRulesControl.GetOidCollection())
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
        /// Fires on cell click. Updates lists of EKUs to Add based on checkbox state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EKUDataGridViewCellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Assert non null/empty EKU value cell
            if(String.IsNullOrEmpty((String)this.ekuDataGridView.Rows[e.RowIndex].Cells[2].Value))
            {
                return; 
            }

            if (e.ColumnIndex != 0 || e.RowIndex < 0)
            {
                return; 
            }

            EKUDisplayObject displayObject = (EKUDisplayObject)this.displayObjects[e.RowIndex];

            // Check state of 'Enabled'/ToAdd column
            // Update state of EkusPosToAdd at e.RowIndex
            if ((bool)this.ekuDataGridView.Rows[e.RowIndex].Cells[0].Value)
            {
                this.EkusPosToAdd[e.RowIndex] = false;
                displayObject.ToAdd = false; // Sets checkbox state
            }
            else
            {
                this.EkusPosToAdd[e.RowIndex] = true;
                displayObject.ToAdd = true;  // Sets checkbox state
            }
        }
    }

    // Class for the datastore
    public class EKUDisplayObject
    {
        public bool ToAdd { get; set; }
        public string EKUFriendlyName { get; set; }
        public string EKUValue { get; set; }

        public EKUDisplayObject()
        {
            this.ToAdd = true; 
            this.EKUFriendlyName = String.Empty;
            this.EKUValue = String.Empty;
        }
        public EKUDisplayObject(bool toAdd, string friendlyName, string value)
        {
            this.ToAdd = toAdd;
            this.EKUFriendlyName = friendlyName;
            this.EKUValue = value; 
        }
    }
}
