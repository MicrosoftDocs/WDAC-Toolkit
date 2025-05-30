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
        private MainWindow _MainWindow;
        private DisplayObject displayObjectInEdit;

        // Declare an ArrayList to serve as the data store. 
        private System.Collections.ArrayList displayObjects =
            new System.Collections.ArrayList();

        private int rowInEdit; 

        public PolicyMerge_Control(MainWindow pMainWindow)
        {
            InitializeComponent();

            nPolicies = 0;
            mergePolicyPath = String.Empty;
            policiesToMerge = new List<string>();
            rowInEdit = -1;

            _MainWindow = pMainWindow;
            _MainWindow.ErrorOnPage = true;
            _MainWindow.RedoFlowRequired = false;
            _MainWindow.ErrorMsg = "Please choose at least 2 policies to merge and a final output location.";
        }

        /// <summary>
        /// User has selected the Browse button. Prompts user to select a file location to save their merged WDAC policy output file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Browse_Click(object sender, EventArgs e)
        {
            string dspTitle = "Choose the final policy save location";
            string policyPath = SavePolicy(dspTitle);

            if (!String.IsNullOrEmpty(policyPath))
            {
                mergePolicyPath = policyPath;
                finalPolicyTextBox.Text = policyPath;
                // Show right side of the text
                if(finalPolicyTextBox.TextLength > 0)
                {
                    finalPolicyTextBox.SelectionStart = finalPolicyTextBox.TextLength - 1;
                    finalPolicyTextBox.ScrollToCaret();
                }

                _MainWindow.Policy.SchemaPath = mergePolicyPath;
                Logger.Log.AddInfoMsg(String.Format("Final Merge Policy set to: {0}", policyPath));

                if(nPolicies >= 2)
                {
                    _MainWindow.ErrorOnPage = false; 
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
            string dspTitle = "Choose App Control policies to merge";
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
                    foreach (string existingPath in policiesToMerge)
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
                        policiesToMerge.Add(policyPath);
                        nPolicies += 1;
                        displayObjects.Add(new DisplayObject(nPolicies.ToString(), policyPath));
                        policiesDataGrid.RowCount += 1;

                        _MainWindow.Policy.PoliciesToMerge = policiesToMerge;

                        if (nPolicies >= 2 && !String.IsNullOrEmpty(mergePolicyPath))
                        {
                            _MainWindow.ErrorOnPage = false;
                        }

                        Logger.Log.AddInfoMsg(String.Format("Adding to list of policies to merge: {0}", policyPath));
                    }
                    else
                    {
                        Logger.Log.AddInfoMsg("Adding to list of policies to merge: Could Not Resolve Path");
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
            Logger.Log.AddInfoMsg("-- Delete Rule button clicked -- ");

            // Get info about the rule user wants to delete: row index and path
            DataGridViewSelectedCellCollection cellCollection = policiesDataGrid.SelectedCells; 

            for(int i=0; i<cellCollection.Count; i++)
            {
                int rowIdx = cellCollection[i].RowIndex;

                // Row index cannot equal nPolicies -- this would be the empty row at bottom of view grid
                if (nPolicies == 0 || rowIdx == nPolicies || rowIdx == -1)
                {
                    continue;
                }
                string policyN = (String)policiesDataGrid["Column_Number", rowIdx].Value;
                string policyPath = (String)policiesDataGrid["Column_Path", rowIdx].Value;

                DialogResult res = MessageBox.Show(String.Format("Are you sure you want to remove {0} from the merge list?",Path.GetFileName(policyPath)),
                    "Remove Policy Confirmation", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);

                if (res == DialogResult.No)
                {
                    continue;
                }

                Logger.Log.AddInfoMsg(String.Format("Removing from list of policies to merge: {0} @RULE# {1}", policyPath, policyN));

                displayObjects.RemoveAt(rowIdx);
                policiesDataGrid.Rows.RemoveAt(rowIdx);

                policiesToMerge.RemoveAt(rowIdx);
                _MainWindow.Policy.PoliciesToMerge = policiesToMerge;

                nPolicies -= 1;

                if (nPolicies < 2)
                {
                    _MainWindow.ErrorOnPage = true;
                }

                // If deleting the last row in the table, trivial deletion operation
                if (rowIdx == nPolicies)
                {

                }

                // else, have to shift every rule number up one
                else
                {
                    int num;
                    DisplayObject displayObject = null;

                    for (int j = rowIdx; j < displayObjects.Count; j++)
                    {
                        displayObject = (DisplayObject)displayObjects[j];
                        num = int.Parse(displayObject.Number) - 1;
                        displayObject.Number = num.ToString();

                        displayObjects[j] = displayObject;
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
            label_Error.Text = dspStr; 
            label_Error.Visible = true;

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
            if (e.RowIndex == policiesDataGrid.RowCount - 1) return;

            DisplayObject displayObject = null;

            if (e.RowIndex == rowInEdit)
            {
                displayObject = displayObjectInEdit;
            }
            else
            {
                displayObject = (DisplayObject)displayObjects[e.RowIndex];
            }

            // Set the cell value to paint using the Customer object retrieved.
            switch (policiesDataGrid.Columns[e.ColumnIndex].Name)
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
            label_Error.Visible = false;
        }

        /// <summary>
        /// Form painting. Occurs on Form.Refresh, Load and Focus. 
        /// Used for UI element changes for Dark and Light Mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PolicyMerge_Control_Validated(object sender, EventArgs e)
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
                policiesDataGrid.BackgroundColor = Color.FromArgb(15, 15, 15);

                // Header
                policiesDataGrid.RowHeadersDefaultCellStyle.BackColor = Color.Black;
                policiesDataGrid.RowHeadersDefaultCellStyle.ForeColor = Color.White;
                policiesDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.Black;
                policiesDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                policiesDataGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 82, 118);
                policiesDataGrid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;

                // Borders
                policiesDataGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                policiesDataGrid.BorderStyle = BorderStyle.Fixed3D;
                policiesDataGrid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

                // Cells
                policiesDataGrid.DefaultCellStyle.BackColor = Color.FromArgb(32, 32, 32);
                policiesDataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 24);
                policiesDataGrid.DefaultCellStyle.ForeColor = Color.White;
                policiesDataGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(26, 82, 118);
                policiesDataGrid.DefaultCellStyle.SelectionForeColor = Color.White;

                // Grid lines
                policiesDataGrid.GridColor = Color.LightSlateGray;
            }

            // Light Mode
            else
            {
                policiesDataGrid.BackgroundColor = Color.White;

                // Header
                policiesDataGrid.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
                policiesDataGrid.RowHeadersDefaultCellStyle.ForeColor = Color.Black;
                policiesDataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
                policiesDataGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                policiesDataGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(174, 214, 241);
                policiesDataGrid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;

                // Borders
                policiesDataGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
                policiesDataGrid.BorderStyle = BorderStyle.Fixed3D;
                policiesDataGrid.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

                // Cells
                policiesDataGrid.DefaultCellStyle.BackColor = Color.White;
                policiesDataGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(236, 240, 241);
                policiesDataGrid.DefaultCellStyle.ForeColor = Color.Black;
                policiesDataGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(174, 214, 241);
                policiesDataGrid.DefaultCellStyle.SelectionForeColor = Color.Black;

                // Grid lines
                policiesDataGrid.GridColor = Color.Black;
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
            Number = String.Empty;
            Path = String.Empty;
        }

        public DisplayObject(string number, string path)
        {
            Number = number;
            Path = path;
        }
    }

}
