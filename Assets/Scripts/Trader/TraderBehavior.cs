using Assets.Scripts.MovementExtensions;
using Assets.Scripts.Resources;
using Assets.Scripts.Resources.Inventory;
using Assets.Scripts.Trader.StateHandlers;
using Assets.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Inventories;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.Trader
{

    [Serializable]
    public class ResourceTrade
    {
        /// <summary>
        /// The type of resource to move
        /// </summary>
        public ResourceType type;
        /// <summary>
        /// the amount of resource to move into the market's inventory. Set to a negative value to withdraw
        ///     resources from the market
        /// </summary>
        public float amount;
    }

    [Serializable]
    public class TradeNode
    {
        public TradeStop target;
        /// <summary>
        /// A list of resource exchanges to attempt at this market
        /// </summary>
        public ResourceTrade[] trades;
    }

    [RequireComponent(typeof(ResourceInventory))]
    public class TraderBehavior : MonoBehaviour
    {
        private AsyncStateMachine<TraderState, TraderBehavior> stateMachine;
        public IDictionary<TraderState, dynamic> stateData;

        /// <summary>
        /// This is only used to set the trade route in the inspector
        ///     it should not be used to represent the trade route as it currently
        ///     exists in the game
        /// </summary>
        public TradeNode[] tradeRoute;

        public ReactiveProperty<TradeNode[]> tradeRouteReactive { get; private set; }

        public HexMovementManager objectSeeker;

        internal NotifyingInventory<ResourceType> inventory;

        private void Awake()
        {
            tradeRouteReactive = new ReactiveProperty<TradeNode[]>(tradeRoute);
            tradeRouteReactive.Buffer(2, 1).Subscribe(routes =>
            {
                OnNewTradeRouteSet(routes[0], routes[1]);
            }).AddTo(this);
            stateData = new Dictionary<TraderState, dynamic>();
        }
        // Start is called before the first frame update
        void Start()
        {
            inventory = GetComponent<ResourceInventory>().backingInventory;
            stateMachine = new AsyncStateMachine<TraderState, TraderBehavior>(TraderState.Initial);

            stateMachine.registerStateTransitionHandler(TraderState.All, TraderState.All, (x) =>
            {
                objectSeeker.ClearCurrentTarget();
            }, StateChangeExecutionOrder.StateExit);

            stateMachine.registerGenericHandler(new InitalStateHandler());
            stateMachine.registerGenericHandler(new TradeTransitStateHandler());
            stateMachine.registerGenericHandler(new TradeExecuteStateHandler());
            stateMachine.LockStateHandlers();
        }

        // Update is called once per frame
        void Update()
        {
            myUpdate();
        }

        async void myUpdate()
        {
            try
            {
                await stateMachine.update(this);
            }
            catch
            {
                throw;
            }
        }

        private int currentTradeTargetIndex = 0;
        public bool hasTradeNodeTarget => tradeRouteReactive.Value.Length > 0 && currentTradeTargetIndex >= 0;
        public TradeNode currentTradeNodeTarget => tradeRouteReactive.Value[currentTradeTargetIndex];
        public void NextTradeRoute()
        {
            currentTradeTargetIndex = (currentTradeTargetIndex + 1) % tradeRouteReactive.Value.Length;
        }
        public void SetNewTradeRoute(TradeNode[] tradeRoute)
        {
            //this.tradeRoute = tradeRoute;
            tradeRouteReactive.Value = tradeRoute.ToList().ToArray();
        }

        private void OnNewTradeRouteSet(TradeNode[] previousTradeRoute, TradeNode[] newTradeRoute)
        {
            TradeStop previousTarget = null;
            if (currentTradeTargetIndex >= 0 && previousTradeRoute.Length > 0)
            {
                previousTarget = previousTradeRoute[currentTradeTargetIndex].target;
            }
            if (previousTarget != null)
            {
                currentTradeTargetIndex = newTradeRoute
                    .Select((trade, index) => new { trade.target, index })
                    .Where(x => x.target == previousTarget)
                    .First().index;
            }
            else
            {
                currentTradeTargetIndex = 0;
            }
        }

        public void AddTradeNode(TradeNode node, int indexToInsert = -1)
        {
            var currentRoute = tradeRouteReactive.Value;
            if (indexToInsert == -1)
            {
                indexToInsert = currentRoute.Length;
            }
            var newRoute = currentRoute.ToList();
            newRoute.Insert(indexToInsert, node);
            SetNewTradeRoute(newRoute.ToArray());
        }
    }
}
