using Assets.UI.InfoPane;

namespace Assets.UI.SelectionManager
{
    /// <summary>
    /// Implement this interface to allow the object to be focused. Will render the ui pane when focused
    /// </summary>
    public interface IFocusable
    {
        void OnMeSelected();
        void OnMeDeselected();
        InfoPaneConfiguration GetInfoPaneConfiguration();
    }
}