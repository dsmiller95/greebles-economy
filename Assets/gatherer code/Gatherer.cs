using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : MonoBehaviour
{
    public ResourceType gatheringType;
    public float speed;
    public float eatDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private GameObject currentTarget;
    private float timeSinceLastTargetCheck = 0;

    // Update is called once per frame
    void Update()
    {
        timeSinceLastTargetCheck += Time.deltaTime;
        if (currentTarget == null &&
            // only check once per second if nothing found
            timeSinceLastTargetCheck > 1)
        {
            currentTarget = this.getNextTarget();
            timeSinceLastTargetCheck = 0;
        }

        if(currentTarget != null)
        {
            this.approachPositionWithDistance(currentTarget.transform.position, Time.deltaTime * this.speed);
            if((transform.position - currentTarget.transform.position).magnitude < eatDistance)
            {
                this.eatResource(this.currentTarget);
            }
        }
    }

    private void approachPositionWithDistance(Vector3 targetPostion, float distance)
    {
        var direction = (targetPostion - this.transform.position).normalized;
        this.transform.position += direction * distance;
    }

    private void eatResource(GameObject resource)
    {
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
