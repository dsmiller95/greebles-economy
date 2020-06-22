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