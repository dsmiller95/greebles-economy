using Assets.Scripts.Resources.UI;
using Assets.UI.InfoPane;
using Assets.UI.SelectionManager;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Market
{
    public class SelectableMarket : MonoBehaviour, IFocusable
    {
        public ResourceTimeSeriesAdapter ResourcePlotter;
        public MarketPricePlotter PricePlotter;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public InfoPaneConfiguration GetInfoPaneConfiguration()
        {
            return new InfoPaneConfiguration()
            {
                plottables = new List<PlotPaneConfig>() {
                new PlotPaneConfig {
                    plot = ResourcePlotter,
                    header = "Inventory"
                },
                new PlotPaneConfig {
                    plot = PricePlotter,
                    header = "Prices"
                }
            }
            };
        }
        public void OnMeDeselected()
        {
            Debug.Log($"{gameObject.name} deselected");
        }

        public void OnMeSelected(Vector3 pointHit)
        {
            Debug.Log($"{gameObject.name} selected");
        }
    }
}