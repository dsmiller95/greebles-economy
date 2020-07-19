using Assets.Scripts.Resources.UI;
using Assets.UI.InfoPane;
using Assets.UI.SelectionManager;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Castle
{
    public class SelectableCastle : MonoBehaviour, IFocusable
    {
        public ResourceTimeSeriesAdapter ResourcePlotter;

        public GameObject buttonPanelPrefab;

        public GameObject InstantiateButtonPanel(GameObject panelParent)
        {
            var newButtonPanel = Instantiate(buttonPanelPrefab, panelParent.transform);
            newButtonPanel.GetComponentInChildren<CastleUIButtonActions>().castle = GetComponent<CastleBehavior>();
            return newButtonPanel;
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