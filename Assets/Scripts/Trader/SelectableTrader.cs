using Assets.MapGen.TileManagement;
using Assets.Scripts.MovementExtensions;
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

        private HexTileMapManager MapManager => GetComponentInParent<HexMember>().MapManager;
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

        public void MeClicked(RaycastHit hit)
        {
            Debug.Log($"{gameObject.name} selected");
            SetupPathPlot();
        }

        private void TeardownPathPlot()
        {
            Destroy(multiPathPlotter.gameObject);
        }

        private void SetupPathPlot()
        {
            var plotter = Instantiate(multiPathPlotterPrefab);
            multiPathPlotter = plotter.GetComponent<MultiPathPlotter>();
            multiPathPlotter.SetPath(trader.tradeRoute.Select(x => x.target.gameObject.transform.position).ToList());

            var snapDistance = 2;
            multiPathPlotter.GetPathPointOnPlaneFromPointHitOnDragoutPlane = pointHit =>
            {
                var manager = MapManager;

                var tilePostion = manager.PositionInPlaneToTilemapPosition(pointHit);

                var closestStop = manager
                    .GetMembersWithinJumpDistanceSlow(tilePostion, snapDistance)
                    .Distinct()
                    .Where(member => member.TryGetType<TradeStop>() != null)
                    .Select(member => new { position = member.PositionInTileMap, dist = member.PositionInTileMap.DistanceTo(tilePostion) })
                    .OrderBy(data => data.dist)
                    .FirstOrDefault()?.position ?? tilePostion;

                var snappedPosition = manager.TileMapPositionToPositionInPlane(closestStop);

                return snappedPosition;
            };

            multiPathPlotter.PathDragEnd = (pointEnd, index) =>
            {
                var manager = MapManager;

                var tilePostion = manager.PositionInPlaneToTilemapPosition(pointEnd);

                var members = manager.GetItemsAtLocation<TradeStop>(tilePostion);
                if (members == null || members.Count() <= 0)
                {
                    return;
                }
                var targetStop = members.First();

                var newTradeNode = GetDefaultTradeNode(targetStop);
                trader.AddTradeNode(newTradeNode, index);
                multiPathPlotter.SetPath(trader.tradeRoute.Select(x => x.target.gameObject.transform.position).ToList());
            };
        }

        private TradeNode GetDefaultTradeNode(TradeStop stop)
        {
            return new TradeNode
            {
                target = stop,
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
        }
    }
}