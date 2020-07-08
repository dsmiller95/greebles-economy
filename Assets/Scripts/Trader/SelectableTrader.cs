using Assets.Scripts.Resources;
using Assets.Scripts.Resources.UI;
using Assets.Scripts.Trader;
using Assets.UI.InfoPane;
using Assets.UI.PathPlotter;
using Assets.UI.SelectionManager;
using Assets.UI.TraderConfigPanel;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Gatherer
{
    public class SelectableTrader : MonoBehaviour, IFocusable
    {
        public ResourceTimeSeriesAdapter ResourcePlotter;
        public TraderBehavior trader;

        public GameObject tradePanelPrefab;
        public GameObject multiPathPlotterPrefab;
        private MultiPathPlotter multiPathPlotter;

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
                            multiPathPlotter.SetPath(trader.tradeRoute.Select(x => x.target.gameObject.transform.position).ToList());
                        };
                    }
                } }
            };
        }

        public void OnMeDeselected()
        {
            Debug.Log($"{gameObject.name} deselected");
            TeardownPathPlot();
        }

        public void OnMeSelected(Vector3 pointHit)
        {
            Debug.Log($"{gameObject.name} selected");
            SetupPathPlot();
        }

        private void TeardownPathPlot()
        {   
            SelectionTracker.globalTracker.RemoveSelectionInput(multiPathPlotter);
            Destroy(multiPathPlotter.gameObject);
        }

        private void SetupPathPlot()
        {
            var plotter = Instantiate(multiPathPlotterPrefab);
            multiPathPlotter = plotter.GetComponent<MultiPathPlotter>();
            multiPathPlotter.SetPath(trader.tradeRoute.Select(x => x.target.gameObject.transform.position).ToList());

            multiPathPlotter.ShouldSnapToObject = o =>
            {
                var willSnap = o.GetComponentInParent<TradeStop>() != null;
                Debug.Log($"Should snap to {o.gameObject.name}: {willSnap}");
                return willSnap;
            };

            multiPathPlotter.HasDroppedOnObject = (o, index) =>
            {
                var newTradeNode = new TradeNode
                {
                    target = o.GetComponentInParent<TradeStop>(),
                    trades = new ResourceTrade[]
                    {
                        new ResourceTrade
                        {
                            amount = 0,
                            type = ResourceType.Food
                        },
                        new ResourceTrade
                        {
                            amount = 0,
                            type = ResourceType.Wood
                        }
                    }
                };
                trader.AddTradeNode(newTradeNode, index);
                multiPathPlotter.SetPath(trader.tradeRoute.Select(x => x.target.gameObject.transform.position).ToList());
            };

            SelectionTracker.globalTracker.PushSelectionInput(multiPathPlotter);
        }
    }
}