using Assets.Scripts.Resources.UI;
using Assets.UI.InfoPane;
using Assets.UI.SelectionManager;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Gatherer.StateHandlers.SellingStateHandler;

namespace Assets.Scripts.Gatherer
{
    public class SelectableGatherer : MonoBehaviour, IFocusable
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
            var sellingData = gatherer.stateData[GathererState.Selling] as SellingStateData;
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
            meshRenderer.material = baseMaterial;
            Debug.Log($"{gameObject.name} deselected");
        }

        public void OnMeSelected()
        {
            meshRenderer.material = selectedMaterial;
            Debug.Log($"{gameObject.name} selected");
        }
    }
}