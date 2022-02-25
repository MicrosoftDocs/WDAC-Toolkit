using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace WDAC_Wizard.src
{
    public partial class PolicyMerge_Control : UserControl
    {
        private int nPolicies;
        private string mergePolicyPath;
        private List<string> policiesToMerge;
        private Logger Log;
        private MainWindow _MainWindow;
        private DisplayObject displayObjectInEdit;

        // Declare an ArrayList to serve as the data store. 
        private System.Collections.ArrayList displayObjects =
            new System.Collections.ArrayList();

        private int rowInEdit; 

        public PolicyMerge_Control(MainWindow pMainWindow)
        {
            InitializeComponent();

            this.nPolicies = 0;
            this.mergePolicyPath = String.Empty;
            this.policiesToMerge = new List<string>();
            this.rowInEdit = -1; 

            this._MainWindow = pMainWindow;
            this.Log = pMainWindow.Log;

            this._MainWindow.ErrorOnPage = true;
            this._MainWindow.RedoFlowRequired = false;
            this._MainWindow.ErrorMsg = "Please choose at least 2 policies to merge and a final output location.";
        }

        private void Button_Browse_Click(object sender, EventArgs e)
        {
            string dspTitle = "Choose the final merged WDAC path location";
            string policyPath = SavePolicy(dspTitle);

            if (!String.IsNullOrEmpty(policyPath))
            {
                this.mergePolicyPath = policyPath;
                this.finalPolicyTextBox.Text = policyPath;
                // Show right side of the text
                this.finalPolicyTextBox.SelectionStart = this.finalPolicyTextBox.TextLength - 1;
                this.finalPolicyTextBox.ScrollToCaret();

                this._MainWindow.Policy.SchemaPath = this.mergePolicyPath;

                this.Log.AddInfoMsg(String.Format("Final Merge Policy set to: {0}", policyPath));

                if(this.nPolicies >= 2)
                {
                    this._MainWindow.ErrorOnPage = false; 
                }
            }
            else
            {
                this.Log.AddInfoMsg("Final Merge Policy set to: Could Not Resolve Path");
            }

        }

        private void Button_AddPolicy_Click(object sender, EventArgs e)
        {
            string dspTitle = "Choose WDAC policies to merge";
            List<string> policyPathsList = Helper.BrowseForMultiFiles(dspTitle, Helper.BrowseFileType.Policy);

            if (policyPathsList == null)
            {
                return;
            }

            foreach (var policyPath in policyPathsList)
            {
                bool isNewPolicy = true; 
                if (!String.IsNullOrEmpty(policyPath))
                {
                    // Check that policy to merge is not already in table
                    foreach (string existingPath in this.policiesToMerge)
                    {
                        if (existingPath.Equals(policyPath))
                        {
                            ShowError(String.Format("{0} is already selected and in table.", Path.GetFileName(policyPath)));
                            isNewPolicy = false; 
                            break;
                        }
                    }

                    if(isNewPolicy)
                    {
                        this.policiesToMerge.Add(policyPath);
                        this.nPolicies += 1;
                        this.displayObjects.Add(new DisplayObject(this.nPolicies.ToString(), policyPath));
                        this.policiesDataGrid.RowCount += 1;

                        this._MainWindow.Policy.PoliciesToMerge = this.policiesToMerge;

                        if (this.nPolicies >= 2 && !String.IsNullOrEmpty(this.mergePolicyPath))
                        {
                            this._MainWindow.ErrorOnPage = false;
                        }

                        this.Log.AddInfoMsg(String.Format("Adding to list of policies to merge: {0}", policyPath));
                    }
                    else
                    {
                        this.Log.AddInfoMsg("Adding to list of policies to merge: Could Not Resolve Path");
                    }
                }
            }
        }

        /// <summary>
        /// User selected the Remove Policy button. Remove the selected rows from the table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_RemovePolicy_Click(object sender, EventArgs e)
        {
            this.Log.AddInfoMsg("-- Delete Rule button clicked -- ");

            // Get info about the rule user wants to delete: row index and path
            DataGridViewSelectedCellCollection cellCollection = this.policiesDataGrid.SelectedCells; 

            for(int i=0; i<cellCollection.Count; i++)
            {
                int rowIdx = cellCollection[i].RowIndex;

                // Row index cannot equal nPolicies -- this would be the empty row at bottom of view grid
                if (this.nPolicies == 0 || rowIdx == this.nPolicies || rowIdx == -1)
                {
                    continue;
                }
                string policyN = (String)this.policiesDataGrid["Column_Number", rowIdx].Value;
                string policyPath = (String)this.policiesDataGrid["Column_Path", rowIdx].Value;

                DialogResult res = MessageBox.Show(String.Format("Are you sure you want to remove {0} from the merge list?",Path.GetFileName(policyPath)),
                    "Remove Policy Confirmation", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (res == DialogResult.No)
                {
                    continue;
                }

                this.Log.AddInfoMsg(String.Format("Removing from list of policies to merge: {0} @RULE# {1}", policyPath, policyN));

                this.displayObjects.RemoveAt(rowIdx);
                this.policiesDataGrid.Rows.RemoveAt(rowIdx);

                this.policiesToMerge.RemoveAt(rowIdx);
                this._MainWindow.Policy.PoliciesToMerge = this.policiesToMerge;

                this.nPolicies -= 1;

                if (this.nPolicies < 2)
                {
                    this._MainWindow.ErrorOnPage = true;
                }

                // If deleting the last row in the table, trivial deletion operation
                if (rowIdx == this.nPolicies)
                {

                }

                // else, have to shift every rule number up one
                else
                {
                    int num;
                    DisplayObject displayObject = null;

                    for (int j = rowIdx; j < this.displayObjects.Count; j++)
                    {
                        displayObject = (DisplayObject)this.displayObjects[j];
                        num = int.Parse(displayObject.Number) - 1;
                        displayObject.Number = num.ToString();

                        this.displayObjects[j] = displayObject;
                    }
                }
            }
        }
              

        private string SavePolicy(string displayTitle)
        {
            // Open file dialog to get file or folder path
            return Helper.SaveSingleFile(Properties.Resources.SaveXMLFileDialogTitle, Helper.BrowseFileType.Policy);
        }


        private void ShowError(string dspStr)
        {
            this.label_Error.Text = dspStr; 
            this.label_Error.Visible = true;

            Timer settingsUpdateNotificationTimer = new Timer();
            settingsUpdateNotificationTimer.Interval = (5000); // 1.5 secs
            settingsUpdateNotificationTimer.Tick += new EventHandler(SettingUpdateTimer_Tick);
            settingsUpdateNotificationTimer.Start();
        }


        ///
        /// Grid View Specific 
        ///

        private void PoliciesDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            // If this is the row for new records, no values are needed.
            if (e.RowIndex == this.policiesDataGrid.RowCount - 1) return;

            DisplayObject displayObject = null;

            if (e.RowIndex == rowInEdit)
            {
                displayObject = this.displayObjectInEdit;
            }
            else
            {
                displayObject = (DisplayObject)this.displayObjects[e.RowIndex];
            }

            // Set the cell value to paint using the Customer object retrieved.
            switch (this.policiesDataGrid.Columns[e.ColumnIndex].Name)
            {
                case "Column_Number":
                    e.Value = displayObject.Number;
                    break;

                case "Column_Path":
                    e.Value = displayObject.Path;
                    break;
            }
        }

        private void SettingUpdateTimer_Tick(object sender, EventArgs e)
        {
            this.label_Error.Visible = false;
        }
    }

    // Class for the datastore
    public class DisplayObject
    {
        public string Number;
        public string Path;
       
        public DisplayObject()
        {
            this.Number = String.Empty;
            this.Path = String.Empty;
        }

        public DisplayObject(string number, string path)
        {
            this.Number = number;
            this.Path = path;
        }
    }

}
