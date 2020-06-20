using Assets.Scrips.Resources.UI;
using Assets.UI.InfoPane;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scrips.Gatherer.StateHandlers.SellingStateHandler;

namespace Assets.Scrips.Gatherer
{
    public class SelectableGatherer : MonoBehaviour, ISelectable
    {
        public ResourceTimeSeriesAdapter ResourcePlotter;
        public GathererBehavior gatherer;
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
            var sellingData = this.gatherer.stateData[GathererState.Selling] as SellingStateData;
            return new InfoPaneConfiguration()
            {
                plottables = new List<PlotPaneConfig>() {
                new PlotPaneConfig {
                    plot = ResourcePlotter,
                    header = "Inventory"
                },
                new PlotPaneConfig
                {
                    plot = sellingData.weightsChart,
                    header = "Resource Weights"
                }
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