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

    public Home home;

    public float speed = 1;
    public float touchDistance = 1f;


    internal GameObject currentTarget;
    internal float lastTargetCheckTime = 0;

    internal ResourceInventory inventory;
    internal ITimeTracker timeTracker;
    internal GatherBehaviorOptimizer optimizer;

    internal Dictionary<ResourceType, float> gatheringWeights;

    private StateMachine<GathererState, Gatherer> stateMachine;

    // Start is called before the first frame update
    void Start()
    {
        this.inventory = this.GetComponent<ResourceInventory>();
        this.timeTracker = this.GetComponent<ITimeTracker>();
        this.optimizer = new GatherBehaviorOptimizer();
        this.gatheringWeights = optimizer.generateInitialWeights();

        this.stateMachine = new StateMachine<GathererState, Gatherer>(GathererState.Gathering);

        stateMachine.registerStateTransitionHandler(GathererState.All, GathererState.All, (x) =>
        {
            currentTarget = null;
            lastTargetCheckTime = 0;
        });

        stateMachine.registerGenericHandler(new GatheringStateHandler());
        stateMachine.registerGenericHandler(new SellingStateHandler());
        stateMachine.registerGenericHandler(new GoingHomeStateHandler());
    }

    // Update is called once per frame
    void Update()
    {
        this.stateMachine.update(this);
    }

    //private void BeginGoingHome()
    //{
    //    this.currentTarget = home.gameObject;
    //}

    //private GathererState GoHome()
    //{
    //    this.approachPositionWithDistance(currentTarget.transform.position, Time.deltaTime * this.speed);
    //    if (distanceToCurrentTarget() < touchDistance)
    //    {
    //        home.depositAllGoods(this.inventory);
    //        return GathererState.Gathering;
    //    }
    //    return GathererState.GoingHome;
    //}


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
            if(currentTarget != null)
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

    internal void eatResource(GameObject resource)
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
}
