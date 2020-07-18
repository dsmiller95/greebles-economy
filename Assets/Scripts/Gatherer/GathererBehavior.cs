using Assets.MapGen.TileManagement;
using Assets.Scripts.Gatherer.StateHandlers;
using Assets.Scripts.Home;
using Assets.Scripts.MovementExtensions;
using Assets.Scripts.Resources;
using Assets.Scripts.Resources.Inventory;
using Assets.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public const int searchRadius = 20;
        public const float waitTimeBetweenSearches = 3f;

        public HomeBehavior home;

        public int backpackSize = 10;
        public GameObject backpack;
        public int pocketSize = 1;

        public ResourceType[] StuffIEat;
        public GameObject RealObjectRoot;

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

        public IObjectSeeker objectSeeker;

        private void Awake()
        {
            objectSeeker = GetComponentInParent<IObjectSeeker>();
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
                objectSeeker.ClearCurrentTarget();
                lastTargetCheckTime = 0;
            }, StateChangeExecutionOrder.StateExit);

            var sellingStateHandler = new SellingStateHandler();
            sellingStateHandler.InstantiateOnObject(this);

            stateMachine.registerGenericHandler(new GatheringStateHandler());
            stateMachine.registerGenericHandler(new GoingHomeStateHandler());
            stateMachine.registerGenericHandler(new WaitTillMarketStateHandler());
            stateMachine.registerGenericHandler(sellingStateHandler);
            stateMachine.registerGenericHandler(new GoingToConsumeHandler());
            stateMachine.registerGenericHandler(new ConsumingStateHandler());
            stateMachine.registerGenericHandler(new DieStateHandler());
            stateMachine.registerGenericHandler(new SleepStateHandler());

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
            backpack.SetActive(true);
            inventory.inventoryCapacity = pocketSize + backpackSize;
        }

        internal void removeBackpack()
        {
            backpack.SetActive(false);
            inventory.inventoryCapacity = pocketSize;
        }


        /// <summary>
        /// Attempts to set the current target to an object within <see cref="searchRadius"/> based on the layer mask and filter function
        ///     returns true if the currentTarget was set to a valid target
        /// </summary>
        /// <param name="layerMask"></param>
        /// <param name="weightFunction"></param>
        /// <returns>true on the frame when a target is found or changed</returns>
        public bool AttemptToEnsureTarget<T>(Func<T, bool> filter, Func<T, float, float> weightFunction)
        {
            if (objectSeeker.CurrentTarget == null &&
                // only check once per second if nothing found
                (lastTargetCheckTime + waitTimeBetweenSearches) < Time.time)
            {
                lastTargetCheckTime = Time.time;
                var newTarget = GetClosestObjectSatisfyingCondition(
                    filter,
                    weightFunction);
                if (newTarget != null)
                {
                    var obj = (newTarget as MonoBehaviour)?.gameObject;
                    if (obj)
                    {
                        objectSeeker.BeginApproachingNewTarget(obj);

                        var member = obj.GetComponentInParent<HexMember>();
                        var distance = GetComponentInParent<HexMember>().PositionInTileMap.DistanceTo(member.PositionInTileMap);
                        if (distance > 40)
                        {
                            Debug.LogError("too long");
                        }
                        return true;
                    }
                    return false;
                }
                objectSeeker.ClearCurrentTarget();
            }
            return false;
        }

        private T GetClosestObjectSatisfyingCondition<T>(Func<T, bool> filter, Func<T, float, float> weightFunction)
        {
            var validObjects = objectSeeker.GetObjectsWithinDistanceFromFilter(searchRadius, filter).ToList();
            if (validObjects.Count <= 0)
            {
                return default;
            }

            return validObjects
                .Select(x =>
                    new
                    {
                        obj = x.Item1,
                        weight = weightFunction(x.Item1, x.Item2)
                    })
                .Aggregate((aggregate, current) => aggregate.weight >= current.weight ? aggregate : current)
                .obj;
        }
        internal async Task<bool> eatResource(GameObject resource)
        {
            var resourceType = resource.GetComponent<IResource>();
            return await resourceType.Eat(inventory);
        }

        public void Die()
        {
            Destroy(RealObjectRoot);
        }
    }
}