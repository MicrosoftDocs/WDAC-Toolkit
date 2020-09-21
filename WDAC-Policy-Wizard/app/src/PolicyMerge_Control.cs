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
        }

        private void button_Browse_Click(object sender, EventArgs e)
        {
            string dspTitle = "Choose the final merged WDAC path location";
            string policyPath = savePolicy(dspTitle);

            if (!String.IsNullOrEmpty(policyPath))
            {
                this.mergePolicyPath = policyPath;
                this.finalPolicyTextBox.Text = policyPath;
                this._MainWindow.Policy.SchemaPath = this.mergePolicyPath;

                this.Log.AddInfoMsg(String.Format("Final Merge Policy set to: {0}", policyPath));
            }
            else
            {
                this.Log.AddInfoMsg("Final Merge Policy set to: Could Not Resolve Path");
            }
        }

        private void button_AddPolicy_Click(object sender, EventArgs e)
        {
            string dspTitle = "Choose a WDAC policy to merge";
            string policyPath = browseForPolicy(dspTitle);

            if (!String.IsNullOrEmpty(policyPath))
            {

                this.policiesToMerge.Add(policyPath);
                this.nPolicies += 1;

                this.displayObjects.Add(new DisplayObject(this.nPolicies.ToString(), policyPath));
                this.policiesDataGrid.RowCount += 1;

                //updateTable();


                this.Log.AddInfoMsg(String.Format("Adding to list of policies to remove: {0}", policyPath));
            }
            else
            {
                this.Log.AddInfoMsg("Adding to list of policies to remove: Could Not Resolve Path");
            }
        }

        private void button_RemovePolicy_Click(object sender, EventArgs e)
        {
            this.Log.AddInfoMsg("-- Delete Rule button clicked -- ");

            // Get info about the rule user wants to delete: row index and path
            int rowIdx = this.policiesDataGrid.CurrentCell.RowIndex;

            if (this.nPolicies == 0 || rowIdx == this.nPolicies)
            {
                return; 
            }

            string policyN = (String)this.policiesDataGrid["Column_Number", rowIdx].Value;
            int policyNumber = int.Parse(policyN); 
            string policyPath = (String)this.policiesDataGrid["Column_Path", rowIdx].Value;

            this.displayObjects.RemoveAt(rowIdx);
            this.policiesDataGrid.Rows.RemoveAt(rowIdx);
            this.nPolicies -= 1; 

            // If deleting the last row in the table, trivial deletion operation
            if (rowIdx == this.nPolicies)
            {

            }

            // else, have to shift every rule number up one
            else
            {
                int num;
                DisplayObject displayObject = null;

                for (int i=rowIdx; i<this.displayObjects.Count; i++)
                {
                    displayObject = (DisplayObject)this.displayObjects[i];
                    num = int.Parse(displayObject.Number) - 1;
                    displayObject.Number = num.ToString(); 

                    this.displayObjects[i] = displayObject; 
                }
            }
        }

        private void updateTable()
        {
            // Iterate through the policiesToMerge list and write them to the table
            for(int i=0; i < this.policiesToMerge.Count; i++)
            {
                string[] row = { i.ToString(), this.policiesToMerge[i]};
                this.policiesDataGrid.Rows.Add(row); 
            }
        }

        private string browseForPolicy(string displayTitle)
        {
            string policyPath = String.Empty;
            // Open file dialog to get file or folder path
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog.Title = displayTitle;
            openFileDialog.CheckPathExists = true;
            openFileDialog.Filter = "Policy File (*.xml)|*.xml";
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                policyPath = openFileDialog.FileName;
            }
            openFileDialog.Dispose();

            return policyPath;
        }

        private string savePolicy(string displayTitle)
        {
            string policyPath = String.Empty;
            // Open file dialog to get file or folder path
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.Title = displayTitle;
            saveFileDialog.CheckPathExists = true;
            saveFileDialog.Filter = "Policy File (*.xml)|*.xml";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                policyPath = saveFileDialog.FileName;
            }
            saveFileDialog.Dispose();

            return policyPath;
        }

        ///
        /// Grid View Specific 
        ///

        private void policiesDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
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
