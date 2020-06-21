using Assets.Scripts.Market;
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
        public MarketBehavior targetMarket;
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

        public TradeNode[] tradeRoute;

        internal NotifyingInventory<ResourceType> inventory;

        private void Awake()
        {
            this.stateData = new Dictionary<TraderState, dynamic>();
        }
        // Start is called before the first frame update
        void Start()
        {
            this.inventory = this.GetComponent<ResourceInventory>().backingInventory;
            this.stateMachine = new AsyncStateMachine<TraderState, TraderBehavior>(TraderState.Initial);

            stateMachine.registerStateTransitionHandler(TraderState.All, TraderState.All, (x) =>
            {
                currentTarget = null;
            }, StateChangeExecutionOrder.StateExit);

            stateMachine.registerGenericHandler(new InitalStateHandler());
            stateMachine.registerGenericHandler(new TradeTransitStateHandler());
            stateMachine.registerGenericHandler(new TradeExecuteStateHandler());
            stateMachine.LockStateHandlers();
        }

        // Update is called once per frame
        void Update()
        {
            this.myUpdate();
        }

        async void myUpdate()
        {
            try
            {
                await this.stateMachine.update(this);
            } catch
            {
                throw;
            }
        }

        private int currentTradeTargetIndex = 0;
        public TradeNode currentTradeNodeTarget => tradeRoute[currentTradeTargetIndex];
        public void NextTradeRoute()
        {
            currentTradeTargetIndex = (currentTradeTargetIndex + 1) % tradeRoute.Length;
        }
        public void SetNewTradeRoute(TradeNode[] tradeRoute)
        {
            var previousTarget = currentTradeNodeTarget.targetMarket;
            this.tradeRoute = tradeRoute;
            this.currentTradeTargetIndex = tradeRoute
                .Select((trade, index) => new { trade.targetMarket, index })
                .Where(x => x.targetMarket == previousTarget)
                .First().index;
        }


        /********************************************
         * code governing moving towards a target
         */
        public float speed = 1;
        public float touchDistance = 1f;
        internal GameObject currentTarget;

        internal bool seekTargetToTouch()
        {
            if (!currentTarget)
            {
                return false;
            }
            moveTowardsPosition(currentTarget.transform.position);
            return isTouchingCurrentTarget();
        }

        public void ClearCurrentTarget()
        {
            currentTarget = null;
        }

        private void moveTowardsPosition(Vector3 targetPostion)
        {
            var difference = targetPostion - transform.position;
            var direction = new Vector3(difference.x, 0, difference.z).normalized;
            transform.position += direction * Time.deltaTime * speed;
        }

        internal bool isTouchingCurrentTarget()
        {
            return distanceToCurrentTarget() <= touchDistance;
        }
        private float distanceToCurrentTarget()
        {
            if (!currentTarget)
            {
                return float.MaxValue;
            }
            var difference = transform.position - currentTarget.transform.position;
            return new Vector3(difference.x, difference.y, difference.z).magnitude;
        }
    }
}
