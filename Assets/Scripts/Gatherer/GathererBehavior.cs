using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TradeModeling.Inventories;
using TradeModeling.Economics;
using System.Threading.Tasks;
using Assets.Utilities;
using Assets.Scrips.Gatherer.StateHandlers;
using Assets.Scrips.Resources.Inventory;
using Assets.Scrips.Resources;
using Assets.Scrips.Home;

namespace Assets.Scrips.Gatherer
{
    [RequireComponent(typeof(ResourceInventory))]
    [RequireComponent(typeof(TimeTracker))]
    public class GathererBehavior : MonoBehaviour
    {
        public const int searchRadius = 100;
        public const float waitTimeBetweenSearches = 0.3f;

        public HomeBehavior home;

        public float speed = 1;
        public float touchDistance = 1f;
        public int backpackSize = 10;
        public int pocketSize = 1;

        internal GameObject currentTarget;
        internal float lastTargetCheckTime = 0;

        internal NotifyingInventory<ResourceType> inventory;
        internal ITimeTracker timeTracker;
        internal GatherBehaviorOptimizer optimizer;

        internal IDictionary<ResourceType, float> gatheringWeights;

        private AsyncStateMachine<GathererState, GathererBehavior> stateMachine;
        public IDictionary<GathererState, dynamic> stateData;
        public IUtilityEvaluator<ResourceType, SpaceFillingInventory<ResourceType>> utilityFunction
        {
            get;
            private set;
        }

        private void Awake()
        {
            this.stateData = new Dictionary<GathererState, dynamic>();
            this.utilityFunction = new UtilityEvaluatorFunctionMapper<ResourceType>(new Dictionary<ResourceType, IIncrementalFunction>()
        {
            {
                ResourceType.Food,
                new InverseWeightedUtility(new WeightedRegion[] {
                    new WeightedRegion(0, 10f),
                    new WeightedRegion(2, 1f)
                })
            },
            {
                ResourceType.Wood,
                new InverseWeightedUtility(new WeightedRegion[] {
                    new WeightedRegion(0, 10),
                    new WeightedRegion(2, 1f)
                })
            }
        });
        }

        // Start is called before the first frame update
        void Start()
        {
            this.inventory = this.GetComponent<ResourceInventory>().backingInventory;

            this.timeTracker = this.GetComponent<ITimeTracker>();
            this.optimizer = new GatherBehaviorOptimizer();
            this.gatheringWeights = optimizer.generateInitialWeights();

            this.stateMachine = new AsyncStateMachine<GathererState, GathererBehavior>(GathererState.Gathering);

            stateMachine.registerStateTransitionHandler(GathererState.All, GathererState.All, (x) =>
            {
                currentTarget = null;
                lastTargetCheckTime = 0;
            });

            stateMachine.registerGenericHandler(new GatheringStateHandler());
            var sellingStateHandler = new SellingStateHandler();
            sellingStateHandler.InstantiateOnObject(this);
            stateMachine.registerGenericHandler(sellingStateHandler);
            stateMachine.registerGenericHandler(new GoingHomeStateHandler());
            stateMachine.registerGenericHandler(new GoingToConsumeHandler());
            stateMachine.registerGenericHandler(new ConsumingStateHandler());
        }

        // Update is called once per frame
        void Update()
        {
            this.stateMachine.update(this);
        }

        internal void attachBackpack()
        {
            inventory.inventoryCapacity = pocketSize + backpackSize;
        }

        internal void removeBackpack()
        {
            inventory.inventoryCapacity = pocketSize;
        }


        /// <summary>
        /// Attempts to set the current target to an object within <see cref="searchRadius"/> based on the layer mask and filter function
        ///     returns true if the currentTarget was set to a valid target
        /// </summary>
        /// <param name="layerMask"></param>
        /// <param name="weightFunction"></param>
        /// <returns>true on the frame when a target is found or changed</returns>
        public bool attemptToEnsureTarget(UserLayerMasks layerMask, Func<GameObject, float, float> weightFunction)
        {
            if (currentTarget == null &&
                // only check once per second if nothing found
                (lastTargetCheckTime + waitTimeBetweenSearches) < Time.time)
            {
                lastTargetCheckTime = Time.time;
                currentTarget = this.getClosestObjectSatisfyingCondition(
                    layerMask,
                    weightFunction);
                if (currentTarget != null)
                {
                    return true;
                }
            }
            return false;
        }

        internal bool seekTargetToTouch()
        {
            if (!currentTarget)
            {
                return false;
            }
            this.moveTowardsPosition(this.currentTarget.transform.position);
            return isTouchingCurrentTarget();
        }

        public void ClearCurrentTarget()
        {
            this.currentTarget = null;
            this.lastTargetCheckTime = 0;
        }

        private void moveTowardsPosition(Vector3 targetPostion)
        {
            var difference = targetPostion - this.transform.position;
            var direction = new Vector3(difference.x, 0, difference.z).normalized;
            this.transform.position += direction * Time.deltaTime * this.speed;
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

        internal async Task<bool> eatResource(GameObject resource)
        {
            var resourceType = resource.GetComponent<IResource>();
            return await resourceType.Eat(this.inventory);
        }

        private GameObject getClosestObjectSatisfyingCondition(UserLayerMasks layerMask, Func<GameObject, float, float> weightFunction)
        {
            Collider[] resourcesInRadius = Physics.OverlapSphere(this.transform.position, searchRadius, (int)layerMask);
            if (resourcesInRadius.Length <= 0)
            {
                return null;
            }
            float maxWeight = float.MinValue;
            Collider highestWeightCollider = null;
            foreach (Collider resource in resourcesInRadius)
            {
                float distance = (this.transform.position - resource.transform.position).magnitude;
                float weight = weightFunction(resource.gameObject, distance);
                if (weight > maxWeight)
                {
                    maxWeight = weight;
                    highestWeightCollider = resource;
                }
            }
            return highestWeightCollider?.gameObject;
        }
    }
}