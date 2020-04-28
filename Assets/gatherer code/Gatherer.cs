using Assets;
using Assets.Gatherer_Code;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    }

    private GameObject currentTarget;
    private float timeSinceLastTargetCheck = 0;

    private ResourceInventory inventory;
    private GathererState currentState = GathererState.Gathering;


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
            gameObject => gameObject?.GetComponent<Market>() != null);
        if (currentTarget != null)
        {
            this.approachPositionWithDistance(currentTarget.transform.position, Time.deltaTime * this.speed);
            if (distanceToCurrentTarget() < touchDistance)
            {
                var market = this.currentTarget.GetComponent<Market>();
                var sellResult = market.sellAllGoods(this.inventory);
                gold += sellResult.TotalResult;

                currentTarget = null;
                timeSinceLastTargetCheck = float.MaxValue;
                this.currentState = GathererState.Gathering;
            }
        }
    }

    private void Gather()
    {
        attemptToEnsureTarget(UserLayerMasks.Resources,
            gameObject => gameObject?.GetComponent<Resource>()?.type == gatheringType);
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
            this.currentState = GathererState.Selling;
        }
    }

    private void attemptToEnsureTarget(UserLayerMasks layerMask, Func<GameObject, bool> filter)
    {
        if (currentTarget == null &&
            // only check once per second if nothing found
            (timeSinceLastTargetCheck += Time.deltaTime) > waitTimeBetweenSearches)
        {
            currentTarget = this.getClosestObjectSatisfyingCondition(
                layerMask,
                filter);
            timeSinceLastTargetCheck = 0;
        }
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
        this.inventory.addResource(resourceType.type, resourceType.value);
        Destroy(resource);
    }

    private GameObject getClosestObjectSatisfyingCondition(UserLayerMasks layerMask, Func<GameObject, bool> filter)
    {
        Collider[] resourcesInRadius = Physics.OverlapSphere(this.transform.position, searchRadius, (int)layerMask);
        if(resourcesInRadius.Length <= 0)
        {
            return null;
        }
        float minDistance = float.MaxValue;
        Collider closestCollider = null;
        foreach (Collider resource in resourcesInRadius)
        {
            if (!filter(resource.gameObject))
            {
                continue;
            }
            float distanceSqr = (this.transform.position - resource.transform.position).sqrMagnitude;
            if (distanceSqr < minDistance)
            {
                minDistance = distanceSqr;
                closestCollider = resource;
            }
        }
        return closestCollider?.gameObject;
    }

    enum GathererState
    {
        Gathering,
        Selling
    }
}
