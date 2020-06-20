using Assets.Scripts.Resources.UI;
using Assets.Scripts.Trader;
using Assets.UI.InfoPane;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Gatherer.StateHandlers.SellingStateHandler;

namespace Assets.Scripts.Gatherer
{
    public class SelectableTrader : MonoBehaviour, ISelectable
    {
        public ResourceTimeSeriesAdapter ResourcePlotter;
        public TraderBehavior gatherer;
        public MeshRenderer meshRenderer;

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
            return new InfoPaneConfiguration()
            {
                plottables = new List<PlotPaneConfig>() {
                new PlotPaneConfig {
                    plot = ResourcePlotter,
                    header = "Inventory"
                },
            }
            };
        }

        public void OnMeDeselected()
        {
            this.meshRenderer.material = this.baseMaterial;
            Debug.Log($"{gameObject.name} deselected");
        }

        public void OnMeSelected()
        {
            this.meshRenderer.material = this.selectedMaterial;
            Debug.Log($"{gameObject.name} selected");
        }
    }
}