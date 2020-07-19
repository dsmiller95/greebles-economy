using Assets.MapGen;
using Assets.MapGen.TileManagement;
using Assets.Scripts.Resources.UI;
using Assets.UI.InfoPane;
using Assets.UI.SelectionManager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Market
{
    public class SelectableMarket : MonoBehaviour, IFocusable
    {
        public ResourceTimeSeriesAdapter ResourcePlotter;
        public MarketPricePlotter PricePlotter;
        public MarketBehavior myMarket;

        public Color32 tileColoring;

        private HexMember myMember;
        private HexTileGenerator tileGenerator;

        // Start is called before the first frame update
        void Start()
        {
            myMember = GetComponentInParent<HexMember>();
            tileGenerator = GetComponentInParent<HexTileGenerator>();
        }

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
                },
                new PlotPaneConfig {
                    plot = PricePlotter,
                    header = "Prices"
                }
            }
            };
        }

        private HexTileGenerator.HexTileColorChangeRecord hexTilesChanged;

        public void OnMeDeselected()
        {
            tileGenerator.ResetHexTileColors(hexTilesChanged);
            Debug.Log($"{gameObject.name} deselected");
        }

        public void MeClicked(RaycastHit hit)
        {
            Debug.Log($"{gameObject.name} selected");

            var myHexPosition = myMember.PositionInTileMap;
            var mapManager = myMember.MapManager;
            var colorChanges = myMarket.myServiceRange
                //.Select(axial => axial.ToOffset())
                .Select(position => (position, tileColoring));

            hexTilesChanged = tileGenerator.SetHexTileColors(colorChanges);
        }
    }
}