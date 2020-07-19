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
using UniRx;
using TradeModeling.TradeRouteUtilities;
using Assets.UI;
using RTS_Cam;

namespace Assets.Scripts.Gatherer
{
    public class SelectableTrader : MonoBehaviour, IFocusable
    {
        public ResourceTimeSeriesAdapter ResourcePlotter;
        public TraderBehavior trader;
        public int tradeStopSnapDistance = 2;

        public GameObject tradePanelPrefab;
        public GameObject multiPathPlotterPrefab;

        public GameObject buttonPanelPrefab;

        private MultiPathPlotter multiPathPlotter;

        private HexTileMapManager MapManager => GetComponentInParent<HexMember>().MapManager;

        public InfoPaneConfiguration GetInfoPaneConfiguration()
        {
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
                    }
                } }
            };
        }
        public GameObject InstantiateButtonPanel(GameObject panelParent)
        {
            var newButtonPanel = Instantiate(buttonPanelPrefab, panelParent.transform);
            newButtonPanel.GetComponentInChildren<TraderUIActions>().trader = GetComponent<TraderBehavior>();
            return newButtonPanel;
        }

        public void OnMeDeselected()
        {
            var rtsCam = CameraGetter.GetCameraObject().GetComponent<RTS_Camera>();
            rtsCam.targetFollow = default;
            Debug.Log($"{gameObject.name} deselected");
            TeardownPathPlot();
        }

        public void MeClicked(RaycastHit hit)
        {
            var rtsCam = CameraGetter.GetCameraObject().GetComponent<RTS_Camera>();
            rtsCam.targetFollow = this.transform;
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

            trader.tradeRouteReactive.Subscribe(tradeRoute =>
            {
                multiPathPlotter.SetPath(tradeRoute.Select(x => x.target.gameObject.transform.position).ToList());
            }).AddTo(plotter);

            multiPathPlotter.GetPathPointOnPlaneFromPointHitOnDragoutPlane = pointHit =>
            {
                var manager = MapManager;

                var tilePostion = manager.PositionInPlaneToTilemapPosition(pointHit);

                var closestStop = manager
                    .GetMembersWithinJumpDistanceSlow(tilePostion, tradeStopSnapDistance)
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
            };
        }

        private TradeNode GetDefaultTradeNode(TradeStop stop)
        {
            return new TradeNode
            {
                target = stop,
                trades = new ResourceTrade<ResourceType>[]
                {
                        new ResourceTrade<ResourceType>
                        {
                            amount = 0,
                            type = ResourceType.Food
                        },
                        new ResourceTrade<ResourceType>
                        {
                            amount = 0,
                            type = ResourceType.Wood
                        }
                }
            };
        }
    }
}