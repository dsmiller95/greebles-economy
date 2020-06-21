using Assets.Scripts.Resources.UI;
using Assets.Scripts.Trader;
using Assets.UI.InfoPane;
using Assets.UI.PathPlotter;
using Assets.UI.TraderConfigPanel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.Gatherer.StateHandlers.SellingStateHandler;

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
                        tradeNodeList.linkedTrader = this.trader;
                    }
                } }
            };
        }

        public void OnMeDeselected()
        {
            this.meshRenderer.material = this.baseMaterial;
            Debug.Log($"{gameObject.name} deselected");
            Destroy(this.mulitPathPlotter.gameObject);
        }

        public void OnMeSelected()
        {
            this.meshRenderer.material = this.selectedMaterial;
            Debug.Log($"{gameObject.name} selected");

            var plotter = Instantiate(multiPathPlotterPrefab);
            this.mulitPathPlotter = plotter.GetComponent<MultiPathPlotter>();
            this.mulitPathPlotter.path = trader.tradeRoute.Select(x => x.targetMarket.transform.position).ToArray();
        }
    }
}