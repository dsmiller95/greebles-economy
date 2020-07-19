using Assets.UI.InfoPane;
using UnityEngine;

namespace Assets.UI.SelectionManager
{
    /// <summary>
    /// Implement this interface to allow the object to be focused. Will render the ui pane when focused
    /// </summary>
    public interface IFocusable : IClickable
    {
        void OnMeDeselected();
        InfoPaneConfiguration GetInfoPaneConfiguration();

        /// <summary>
        /// Instantiate the buttons on the object action panel.
        /// </summary>
        /// <param name="panelParent">The button panel container, instantiate the buttons underneath this</param>
        /// <returns>The created button object. Will be destroyed when something else needs to go in the panel</returns>
        GameObject InstantiateButtonPanel(GameObject panelParent);
    }
}