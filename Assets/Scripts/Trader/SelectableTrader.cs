using Assets.Scripts.Resources.UI;
using Assets.Scripts.Trader;
using Assets.UI.InfoPane;
using Assets.UI.PathPlotter;
using Assets.UI.TraderConfigPanel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Gatherer
{
    public class SelectableTrader : MonoBehaviour, ISelectable
    {
        public ResourceTimeSeriesAdapter ResourcePlotter;
        public TraderBehavior trader;
        public MeshRenderer meshRenderer;

        public GameObject tradePanelPrefab;
        public GameObject multiPathPlotterPrefab;
        private MultiPathPlotter mulitPathPlotter;

        public Material baseMaterial;
        public Material selectedMaterial;

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
            //var traderPanel = GameObject.Instantiate(tradePanelPrefab, )
            return new InfoPaneConfiguration()
            {
                plottables = new List<PlotPaneConfig>() {
                new PlotPaneConfig {
                    plot = ResourcePlotter,
                    header = "Inventory"
                },
            },
                uiObjects = new List<GenericUIObjectConfig>()
                {new GenericUIObjectConfig{
                    prefabToInit = tradePanelPrefab,
                    postInitHook = (panel) =>
                    {
                        var tradeNodeList = panel.GetComponentInChildren<TradeNodeList>();
                        tradeNodeList.linkedTrader = trader;
                        tradeNodeList.tradeRouteUpdated = (tradeRoute) =>
                        {
                            trader.SetNewTradeRoute(tradeRoute);
                            mulitPathPlotter.SetPath(trader.tradeRoute.Select(x => x.targetMarket.transform.position));
                        };
                    }
                } }
            };
        }

        public void OnMeDeselected()
        {
            meshRenderer.material = baseMaterial;
            Debug.Log($"{gameObject.name} deselected");
            TeardownPathPlot();
        }

        public void OnMeSelected()
        {
            meshRenderer.material = selectedMaterial;
            Debug.Log($"{gameObject.name} selected");
            SetupPathPlot();
        }

        private void TeardownPathPlot()
        {
            Destroy(mulitPathPlotter.gameObject);
        }

        private void SetupPathPlot()
        {
            var plotter = Instantiate(multiPathPlotterPrefab);
            mulitPathPlotter = plotter.GetComponent<MultiPathPlotter>();
            mulitPathPlotter.SetPath(trader.tradeRoute.Select(x => x.targetMarket.transform.position));
        }
    }
}