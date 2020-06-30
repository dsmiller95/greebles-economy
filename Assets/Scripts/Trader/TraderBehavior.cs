using Assets.Scripts.MovementExtensions;
using Assets.Scripts.Resources;
using Assets.Scripts.Resources.Inventory;
using Assets.Scripts.Trader.StateHandlers;
using Assets.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Inventories;
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
    [RequireComponent(typeof(FreeFormObjectSeeker))]
    public class TraderBehavior : MonoBehaviour
    {
        private AsyncStateMachine<TraderState, TraderBehavior> stateMachine;
        public IDictionary<TraderState, dynamic> stateData;

        public TradeNode[] tradeRoute;

        internal NotifyingInventory<ResourceType> inventory;

        public IObjectSeeker objectSeeker;
        private void Awake()
        {
            stateData = new Dictionary<TraderState, dynamic>();
            objectSeeker = GetComponent<IObjectSeeker>();
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
        public bool hasTradeNodeTarget => tradeRoute.Length > 0 && currentTradeTargetIndex >= 0;
        public TradeNode currentTradeNodeTarget => tradeRoute[currentTradeTargetIndex];
        public void NextTradeRoute()
        {
            currentTradeTargetIndex = (currentTradeTargetIndex + 1) % tradeRoute.Length;
        }
        public void SetNewTradeRoute(TradeNode[] tradeRoute)
        {
            var previousTarget = hasTradeNodeTarget ? currentTradeNodeTarget.target : null;
            this.tradeRoute = tradeRoute;
            if (previousTarget != null)
            {
                currentTradeTargetIndex = tradeRoute
                    .Select((trade, index) => new { trade.target, index })
                    .Where(x => x.target == previousTarget)
                    .First().index;
            }
            else
            {
                currentTradeTargetIndex = 0;
            }
        }
    }
}
