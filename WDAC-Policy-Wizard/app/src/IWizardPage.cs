namespace WDAC_Wizard
{
    /// <summary>
    /// Marker interface implemented by full-page wizard content (UserControls) that the MainWindow
    /// hosts and docks to fill its client area. The host uses this marker to dock only intended
    /// wizard pages, rather than every UserControl that may be added to the form. Helper or
    /// non-page UserControls should NOT implement this interface so they keep their own layout.
    /// </summary>
    public interface IWizardPage
    {
    }
}
