using Assets.UI.InfoPane;
using UnityEngine;

namespace Assets.UI.SelectionManager
{
    /// <summary>
    /// Implement this interface to allow the object to be focused. Will render the ui pane when focused
    /// </summary>
    public interface IFocusable
    {
        void OnMeSelected(Vector3 pointHit);
        void OnMeDeselected();
        InfoPaneConfiguration GetInfoPaneConfiguration();
    }
}