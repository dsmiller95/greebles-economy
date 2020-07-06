using Assets.MapGen;
using Assets.Scripts.MovementExtensions;
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

        private HexMember myMember;
        private HexTileGenerator tileGenerator;

        // Start is called before the first frame update
        void Start()
        {
            myMember = GetComponentInParent<HexMember>();
            tileGenerator = GetComponentInParent<HexTileGenerator>();
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

        private HexTileGenerator.HexTileColorChangeRecord hexTilesChanged;

        public void OnMeDeselected()
        {
            tileGenerator.ResetHexTileColors(hexTilesChanged);
            Debug.Log($"{gameObject.name} deselected");
        }

        public void OnMeSelected(Vector3 pointHit)
        {
            Debug.Log($"{gameObject.name} selected");

            Color32 color = Color.green;
            var myHexPosition = myMember.PositionInTileMap;
            var mapManager = myMember.MapManager;
            var colorChanges = mapManager
                .GetPositionsWithinJumpDistance(myHexPosition, 2)
                .Select(position => (position, color));

            hexTilesChanged = tileGenerator.SetHexTileColors(colorChanges);
        }
    }
}