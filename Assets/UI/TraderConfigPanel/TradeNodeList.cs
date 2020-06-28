using Assets.Scripts.Resources;
using Assets.Scripts.Trader;
using Assets.UI.Draggable;
using Assets.UI.SelectionManager;
using Assets.UI.SelectionManager.GetObjectSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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


        public Action<TradeNode[]> tradeRouteUpdated;


        private IList<TradeNodePanel> myPanels;
        // Start is called before the first frame update
        void Start()
        {
            RecreateAllPanels();
            GetComponent<DragZone>().orderingChanged += SetOrder;

            addNewTradeNodeButton.onClick.AddListener(AddNewTradeNodeButtonClicked);
        }

        void RecreateAllPanels()
        {
            if (myPanels != default)
            {
                foreach (var panel in myPanels)
                {
                    Destroy(panel.gameObject);
                }
            }
            var largestTrade = linkedTrader.inventory.inventoryCapacity;
            myPanels = linkedTrader.tradeRoute
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
                var selection = await SelectionTracker.globalTracker.GetInputAsync<TradeStop>(market => true);
                Debug.Log($"got market {selection.gameObject.name}");
                NewTradeRoute(linkedTrader.tradeRoute.Append(new TradeNode
                {
                    target = selection,
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
                }));

                RecreateAllPanels();
            }
            catch (ObjectSelectionCancelledException) { }
            catch (Exception e)
            {
                Debug.LogError($"Error when using the asynchronous selection requester");
                Debug.LogException(e);
                throw; }
        }

        private void SetOrder()
        {
            NewTradeRoute(GetComponentsInChildren<TradeNodePanel>().Select(x => x.tradeNode));
        }
        private void NewTradeRoute(IEnumerable<TradeNode> newRoute)
        {
            tradeRouteUpdated?.Invoke(newRoute.ToArray());
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
                    tradeRouteUpdated?.Invoke(linkedTrader.tradeRoute);
                });
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}