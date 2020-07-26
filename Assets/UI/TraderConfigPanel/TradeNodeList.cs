using Assets.Scripts.Resources;
using Assets.Scripts.Trader;
using Assets.UI.Draggable;
using Assets.UI.SelectionManager;
using Assets.UI.SelectionManager.GetObjectSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeModeling.Inventories;
using TradeModeling.TradeRouteUtilities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.TraderConfigPanel
{
    [RequireComponent(typeof(DragZone))]
    public class TradeNodeList : MonoBehaviour
    {
        public TraderBehavior linkedTrader;

        public Button addNewTradeNodeButton;
        public GameObject singleTradeNodePrefab;

        private IList<TradeNodePanel> myPanels;
        // Start is called before the first frame update
        void Start()
        {
            linkedTrader.tradeRouteReactive.Subscribe(route =>
            {
                RecreateAllPanels(route);
            }).AddTo(this);
            GetComponent<DragZone>().orderingChanged += SetOrder;

            addNewTradeNodeButton.onClick.AddListener(AddNewTradeNodeButtonClicked);
        }

        void RecreateAllPanels(TradeNode[] tradeRoute)
        {
            if (myPanels != default)
            {
                foreach (var panel in myPanels)
                {
                    Destroy(panel.gameObject);
                }
            }
            // todo: ?
            var largestTrade = (linkedTrader.inventory.itemSource as ISpaceFillingItemSource<ResourceType>)?.inventoryCapacity ?? 100;
            myPanels = tradeRoute
                .Select(node => CreateSingleTradeNode(node, largestTrade))
                .ToList();
        }

        private void AddNewTradeNodeButtonClicked()
        {
            _ = AttemptToAddNewTradeNode();
        }

        private async Task AttemptToAddNewTradeNode()
        {
            try
            {
                var selection = await SelectionTracker.instance.GetInputAsync<TradeStop>(market => true);
                var newTradeNode = new TradeNode
                {
                    target = selection,
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

                linkedTrader.AddTradeNode(newTradeNode);
            }
            catch (ObjectSelectionCancelledException) { }
            catch (Exception e)
            {
                Debug.LogError($"Error when using the asynchronous selection requester");
                Debug.LogException(e);
                throw;
            }
        }

        private void SetOrder()
        {
            NewTradeRoute(GetComponentsInChildren<TradeNodePanel>().Select(x => x.tradeNode));
        }

        private void InternalTradeRouteUpdated(TradeNode[] newRoute)
        {
            linkedTrader.SetNewTradeRoute(newRoute);
        }

        private void NewTradeRoute(IEnumerable<TradeNode> newRoute)
        {
            var routeArray = newRoute.ToArray();
            InternalTradeRouteUpdated(routeArray);
        }

        private TradeNodePanel CreateSingleTradeNode(TradeNode node, float maxTradeAmount)
        {
            return TradeNodePanel.InstantiateOnObject(
                singleTradeNodePrefab,
                gameObject,
                node,
                maxTradeAmount,
                () =>
                {
                    InternalTradeRouteUpdated(linkedTrader.tradeRouteReactive.Value);
                });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}