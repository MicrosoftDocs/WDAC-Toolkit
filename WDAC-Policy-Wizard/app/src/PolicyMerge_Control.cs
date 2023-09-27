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

        /// <summary>
        /// User has selected the Browse button. Prompts user to select a file location to save their merged WDAC policy output file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Browse_Click(object sender, EventArgs e)
        {
            string dspTitle = "Choose the final merged WDAC path location";
            string policyPath = SavePolicy(dspTitle);

            if (!String.IsNullOrEmpty(policyPath))
            {
                this.mergePolicyPath = policyPath;
                this.finalPolicyTextBox.Text = policyPath;
                // Show right side of the text
                if(this.finalPolicyTextBox.TextLength > 0)
                {
                    this.finalPolicyTextBox.SelectionStart = this.finalPolicyTextBox.TextLength - 1;
                    this.finalPolicyTextBox.ScrollToCaret();
                }

                this._MainWindow.Policy.SchemaPath = this.mergePolicyPath;
                this.Log.AddInfoMsg(String.Format("Final Merge Policy set to: {0}", policyPath));

                if(this.nPolicies >= 2)
                {
                    this._MainWindow.ErrorOnPage = false; 
                }
            }
        }

        /// <summary>
        /// User has selected the + Add Policy button. Prompts user to select multiple WDAC policy files to be merged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// User has selected the - Remove Policy button. Prompts user to select the WDAC policy files to be removed from the table
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
              
        /// <summary>
        /// Calls the helper function to get save file location path
        /// </summary>
        /// <param name="displayTitle"></param>
        /// <returns>Path to save the merged WDAC file output. String.Empty if user cancels</returns>
        private string SavePolicy(string displayTitle)
        {
            // Open file dialog to get file or folder path
            return Helper.SaveSingleFile(Properties.Resources.SaveXMLFileDialogTitle, Helper.BrowseFileType.Policy);
        }

        /// <summary>
        /// Display error label. Set timer to show the error for 5 seconds
        /// </summary>
        /// <param name="dspStr"></param>
        private void ShowError(string dspStr)
        {
            this.label_Error.Text = dspStr; 
            this.label_Error.Visible = true;

            Timer settingsUpdateNotificationTimer = new Timer();
            settingsUpdateNotificationTimer.Interval = (5000); // 5 secs
            settingsUpdateNotificationTimer.Tick += new EventHandler(SettingUpdateTimer_Tick);
            settingsUpdateNotificationTimer.Start();
        }

        /// <summary>
        /// Method to retrieve the Cell Values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Method to hide the error label. Called when timer runs out (5s)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingUpdateTimer_Tick(object sender, EventArgs e)
        {
            this.label_Error.Visible = false;
        }
        /// <summary>
        /// Form painting. Occurs on Form.Refresh, Load and Focus. 
        /// Used for UI element changes for Dark and Light Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PolicyMerge_Control_Paint(object sender, PaintEventArgs e)
        {
            // Set Controls Color (e.g. Labels Panels, Textboxes, Buttons)
            SetControlsColor();

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
                        button.ForeColor = Color.DodgerBlue;
                        button.BackColor = Color.FromArgb(15, 15, 15);
                        button.FlatAppearance.BorderColor = Color.DodgerBlue;
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

                    // Labels
                    else if (control is Label label
                        && (label.Tag == null || label.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        label.ForeColor = Color.White;
                        label.BackColor = Color.FromArgb(15, 15, 15);
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
                        button.ForeColor = Color.DodgerBlue;
                        button.BackColor = Color.White;
                        button.FlatAppearance.BorderColor = Color.DodgerBlue;
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

                    // Labels
                    else if (control is Label label
                        && (label.Tag == null || label.Tag.ToString() != Properties.Resources.IgnoreDarkModeTag))
                    {
                        label.ForeColor = Color.Black;
                        label.BackColor = Color.White;
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
                        label.ForeColor = Color.FromArgb(15,15,15);
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
                policiesDataGrid.BackgroundColor = Color.Black; // FromArgb(15, 15, 15);
                policiesDataGrid.GridColor = Color.Black; // FromArgb(15, 15, 15);

                // Headers
                policiesDataGrid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 15, 15);
                policiesDataGrid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                policiesDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 15, 15);
                policiesDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                // Cells
                policiesDataGrid.DefaultCellStyle.BackColor = Color.FromArgb(15, 15, 15);
                policiesDataGrid.DefaultCellStyle.ForeColor = Color.White;
            }

            // Light Mode
            else
            {
                policiesDataGrid.BackgroundColor = Color.LightGray;
                policiesDataGrid.GridColor = Color.DimGray;

                // Header
                policiesDataGrid.RowHeadersDefaultCellStyle.BackColor = Color.LightGray;
                policiesDataGrid.RowHeadersDefaultCellStyle.ForeColor = Color.Black;
                policiesDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
                policiesDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;

                // Cells
                policiesDataGrid.DefaultCellStyle.BackColor = Color.WhiteSmoke;
                policiesDataGrid.DefaultCellStyle.ForeColor = Color.Black;
            }
        }
    }

    // Class for the table datastore
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
