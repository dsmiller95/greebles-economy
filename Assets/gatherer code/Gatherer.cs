using Assets;
using Assets.Gatherer_Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : MonoBehaviour
{
    public ResourceType gatheringType;
    public float speed = 1;
    public float eatDistance = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        this.inventory = new ResourceInventory();
    }

    private GameObject currentTarget;
    private float timeSinceLastTargetCheck = 0;

    private ResourceInventory inventory;


    // Update is called once per frame
    void Update()
    {
        if (currentTarget == null &&
            // only check once per second if nothing found
            (timeSinceLastTargetCheck += Time.deltaTime) > 1)
        {
            currentTarget = this.getNextTarget();
            timeSinceLastTargetCheck = 0;
        }
        if (currentTarget != null)
        {
            this.approachPositionWithDistance(currentTarget.transform.position, Time.deltaTime * this.speed);
            if(distanceToFood(currentTarget.transform.position) < eatDistance)
            {
                this.eatResource(this.currentTarget);
                timeSinceLastTargetCheck = float.MaxValue;
            }
        }
    }

    private void approachPositionWithDistance(Vector3 targetPostion, float distance)
    {
        var difference = targetPostion - this.transform.position;
        var direction = new Vector3(difference.x, 0, difference.z).normalized;
        this.transform.position += direction * distance;
    }

    private float distanceToFood(Vector3 foodPos)
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

    private GameObject getNextTarget()
    {
        Collider[] resourcesInRadius = Physics.OverlapSphere(this.transform.position, 30, (int)UserLayerMasks.Resources);
        if(resourcesInRadius.Length <= 0)
        {
            return null;
        }
        float minDistance = float.MaxValue;
        Collider closestCollider = null;
        foreach (Collider resource in resourcesInRadius)
        {
            float distanceSqr = (this.transform.position - resource.transform.position).sqrMagnitude;
            if (distanceSqr < minDistance)
            {
                minDistance = distanceSqr;
                closestCollider = resource;
            }
        }
        return closestCollider.gameObject;
    }
}
