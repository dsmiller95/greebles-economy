using Assets;
using Assets.Gatherer_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(ResourceInventory))]
[RequireComponent(typeof(TimeTracker))]
public class Gatherer : MonoBehaviour
{
    public const int searchRadius = 100;
    public const float waitTimeBetweenSearches = 0.3f;

    public ResourceType gatheringType;
    public float speed = 1;
    public float touchDistance = 1f;

    public float gold = 0;

    // Start is called before the first frame update
    void Start()
    {
        this.inventory = this.GetComponent<ResourceInventory>();
        this.timeTracker = this.GetComponent<ITimeTracker>();
        this.optimizer = new GatherBehaviorOptimizer();
        this.gatheringWeights = optimizer.generateInitialWeights();
    }

    private GameObject currentTarget;
    private float timeSinceLastTargetCheck = 0;

    private ResourceInventory inventory;
    private ITimeTracker timeTracker;
    private GatherBehaviorOptimizer optimizer;
    private GathererState currentState = GathererState.Gathering;

    private Dictionary<ResourceType, float> gatheringWeights;


    // Update is called once per frame
    void Update()
    {
        switch (this.currentState)
        {
            case GathererState.Gathering:
                this.Gather();
                break;
            case GathererState.Selling:
                this.SellGoods();
                break;
        }
    }

    private void SellGoods()
    {
        attemptToEnsureTarget(UserLayerMasks.Market,
            (gameObject, distance) => {
                if (gameObject?.GetComponent<Market>() != null)
                {
                    return -distance;
                }
                return float.MinValue;
            });
        if (currentTarget != null)
        {
            this.approachPositionWithDistance(currentTarget.transform.position, Time.deltaTime * this.speed);
            if (distanceToCurrentTarget() < touchDistance)
            {
                var market = this.currentTarget.GetComponent<Market>();
                var sellResult = market.sellAllGoods(this.inventory);
                var timeSummary = timeTracker.getResourceTimeSummary();

                gold += sellResult.Values.Aggregate(0f, (aggregate, current) => current.totalRevenue + aggregate);

                gatheringWeights = optimizer.generateNewWeights(gatheringWeights, timeSummary, sellResult);
                Debug.Log("new weights:");
                Debug.Log(serializeDictionary(gatheringWeights));
                timeTracker.clearTime();

                currentTarget = null;
                timeSinceLastTargetCheck = float.MaxValue;
                this.currentState = GathererState.Gathering;
            }
        }
    }

    public static string serializeDictionary<T>(Dictionary<ResourceType, T> dictionary, Func<T, string> serializer = null)
    {
        serializer = serializer ?? (s => s.ToString());
        return dictionary.Select(entry => $"Type: {Enum.GetName(typeof(ResourceType), entry.Key)}\t Value: {serializer(entry.Value)}").Aggregate((agg, current) => agg + '\n' + current);
    }

    private void Gather()
    {
        if (attemptToEnsureTarget(UserLayerMasks.Resources,
            (gameObject, distance) => {
                var resource = gameObject?.GetComponent<Resource>();
                if (resource != null)
                {
                    var type = resource.type;
                    return -(distance/gatheringWeights[type]);
                }
                return float.MinValue;
            }))
        {
            this.timeTracker.startTrackingResource(currentTarget.GetComponent<Resource>().type);
        };
        if (currentTarget != null)
        {
            this.approachPositionWithDistance(currentTarget.transform.position, Time.deltaTime * this.speed);
            if (distanceToCurrentTarget() < touchDistance)
            {
                this.eatResource(this.currentTarget);
                timeSinceLastTargetCheck = float.MaxValue;
            }
        }

        if(this.inventory.getFullRatio() >= 1)
        {
            currentTarget = null;
            timeSinceLastTargetCheck = float.MaxValue;
            this.timeTracker.pauseTracking();
            this.currentState = GathererState.Selling;
        }
    }


    /// <summary>
    /// Attempts to set the current target to an object within <see cref="searchRadius"/> based on the layer mask and filter function
    ///     returns true if the currentTarget was set to a valid target
    /// </summary>
    /// <param name="layerMask"></param>
    /// <param name="weightFunction"></param>
    /// <returns></returns>
    private bool attemptToEnsureTarget(UserLayerMasks layerMask, Func<GameObject, float, float> weightFunction)
    {
        if (currentTarget == null &&
            // only check once per second if nothing found
            (timeSinceLastTargetCheck += Time.deltaTime) > waitTimeBetweenSearches)
        {
            currentTarget = this.getClosestObjectSatisfyingCondition(
                layerMask,
                weightFunction);
            timeSinceLastTargetCheck = 0;
            if(currentTarget != null)
            {
                return true;
            }
        }
        return false;
    }

    private void approachPositionWithDistance(Vector3 targetPostion, float distance)
    {
        var difference = targetPostion - this.transform.position;
        var direction = new Vector3(difference.x, 0, difference.z).normalized;
        this.transform.position += direction * distance;
    }

    private float distanceToCurrentTarget()
    {
        var difference = transform.position - currentTarget.transform.position;
        return new Vector3(difference.x, difference.y, difference.z).magnitude;
    }

    private void eatResource(GameObject resource)
    {
        var resourceType = resource.GetComponent<Resource>();
        if (!resourceType.eaten)
        {
            resourceType.eaten = true;
            this.inventory.addResource(resourceType.type, resourceType.value);
            Destroy(resource);
        }
    }

    private GameObject getClosestObjectSatisfyingCondition(UserLayerMasks layerMask, Func<GameObject, float, float> weightFunction)
    {
        Collider[] resourcesInRadius = Physics.OverlapSphere(this.transform.position, searchRadius, (int)layerMask);
        if(resourcesInRadius.Length <= 0)
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

    enum GathererState
    {
        Gathering,
        Selling
    }
}
