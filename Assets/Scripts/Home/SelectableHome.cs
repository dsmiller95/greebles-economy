using Assets.Scripts.Resources.UI;
using Assets.UI.InfoPane;
using Assets.UI.SelectionManager;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Home
{
    public class SelectableHome : MonoBehaviour, IFocusable
    {
        public ResourceTimeSeriesAdapter ResourcePlotter;

        public GameObject InstantiateButtonPanel(GameObject panelParent)
        {
            return null;
        }

        public InfoPaneConfiguration GetInfoPaneConfiguration()
        {
            return new InfoPaneConfiguration()
            {
                plottables = new List<PlotPaneConfig>() {
                new PlotPaneConfig {
                    plot = ResourcePlotter,
                    header = "Inventory"
                }
            }
            };
        }
        public void OnMeDeselected()
        {
            Debug.Log($"{gameObject.name} deselected");
        }

        public void MeClicked(RaycastHit hit)
        {
            Debug.Log($"{gameObject.name} selected");
        }
    }
}