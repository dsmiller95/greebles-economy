using Assets.UI.InfoPane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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