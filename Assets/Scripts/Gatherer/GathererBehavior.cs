using Assets.Scripts.Gatherer.StateHandlers;
using Assets.Scripts.Home;
using Assets.Scripts.Resources;
using Assets.Scripts.Resources.Inventory;
using Assets.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeModeling.Economics;
using TradeModeling.Functions;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scripts.Gatherer
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

        public ResourceType[] StuffIEat;

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
            stateData = new Dictionary<GathererState, dynamic>();
            utilityFunction = new UtilityEvaluatorFunctionMapper<ResourceType>(new Dictionary<ResourceType, IIncrementalFunction>()
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
            inventory = GetComponent<ResourceInventory>().backingInventory;

            timeTracker = GetComponent<ITimeTracker>();
            optimizer = new GatherBehaviorOptimizer();
            gatheringWeights = optimizer.generateInitialWeights();

            stateMachine = new AsyncStateMachine<GathererState, GathererBehavior>(GathererState.Gathering);

            stateMachine.registerStateTransitionHandler(GathererState.All, GathererState.All, (x) =>
            {
                currentTarget = null;
                lastTargetCheckTime = 0;
            }, StateChangeExecutionOrder.StateExit);

            stateMachine.registerGenericHandler(new GatheringStateHandler());
            var sellingStateHandler = new SellingStateHandler();
            sellingStateHandler.InstantiateOnObject(this);
            stateMachine.registerGenericHandler(sellingStateHandler);
            stateMachine.registerGenericHandler(new GoingHomeStateHandler());
            stateMachine.registerGenericHandler(new GoingToConsumeHandler());
            stateMachine.registerGenericHandler(new ConsumingStateHandler());
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
                currentTarget = getClosestObjectSatisfyingCondition(
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
            moveTowardsPosition(currentTarget.transform.position);
            return isTouchingCurrentTarget();
        }

        public void ClearCurrentTarget()
        {
            currentTarget = null;
            lastTargetCheckTime = 0;
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

        internal async Task<bool> eatResource(GameObject resource)
        {
            var resourceType = resource.GetComponent<IResource>();
            return await resourceType.Eat(inventory);
        }

        private GameObject getClosestObjectSatisfyingCondition(UserLayerMasks layerMask, Func<GameObject, float, float> weightFunction)
        {
            Collider[] resourcesInRadius = Physics.OverlapSphere(transform.position, searchRadius, (int)layerMask);
            if (resourcesInRadius.Length <= 0)
            {
                return null;
            }
            float maxWeight = float.MinValue;
            Collider highestWeightCollider = null;
            foreach (Collider resource in resourcesInRadius)
            {
                float distance = (transform.position - resource.transform.position).magnitude;
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