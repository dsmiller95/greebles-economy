using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.UI.InfoPane
{
    public interface ISelectable
    {
        void OnMeSelected();
        void OnMeDeselected();
        InfoPaneConfiguration GetInfoPaneConfiguration();
    }
}