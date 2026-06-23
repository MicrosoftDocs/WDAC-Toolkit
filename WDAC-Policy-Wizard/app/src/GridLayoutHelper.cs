// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Windows.Forms;

namespace WDAC_Wizard
{
    /// <summary>
    /// Helper utilities for DataGridView layout behavior shared across the wizard pages.
    /// </summary>
    internal static class GridLayoutHelper
    {
        /// <summary>
        /// Sizes each column to fit its content. The columns remain user-resizable afterwards
        /// because the grid's AutoSizeColumnsMode is left as None.
        /// The resize is deferred until the grid has a created handle so that text measurement
        /// works correctly (calling it too early, e.g. during Load, is a silent no-op).
        /// </summary>
        /// <param name="grid">The DataGridView to resize.</param>
        public static void AutoFitColumns(DataGridView grid)
        {
            if (grid == null)
            {
                return;
            }

            void Resize()
            {
                if (grid.IsDisposed || grid.ColumnCount == 0)
                {
                    return;
                }

                grid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }

            if (grid.IsHandleCreated)
            {
                // Defer to the message loop so it runs after the current layout pass completes.
                grid.BeginInvoke((MethodInvoker)Resize);
            }
            else
            {
                // Handle not created yet (e.g. populated during Load). Resize once it is.
                void Handler(object sender, System.EventArgs e)
                {
                    grid.HandleCreated -= Handler;
                    grid.BeginInvoke((MethodInvoker)Resize);
                }

                grid.HandleCreated += Handler;
            }
        }
    }
}
