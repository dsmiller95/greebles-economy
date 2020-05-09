using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gatherer_Code;
using UnityEngine;

public class ResourceInventory : MonoBehaviour
{
    public int inventoryCapacity = 10;

    private Dictionary<ResourceType, float> inventory;

    public ResourceType[] spaceFillingItems = new ResourceType[] { ResourceType.Food, ResourceType.Wood };

    public ResourceDisplay resourceRenderer;

    public void Start()
    {
        inventory = new Dictionary<ResourceType, float>();
        var resourceTypes = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();
        foreach (var resource in resourceTypes)
        {
            // create the key with default; then set value to activate any extra functionality
            inventory[resource] = 0;
            this.setInventoryValue(resource, 0);
        }
        foreach (var resourceType in spaceFillingItems)
        {
            resourceRenderer.setMaxForType(resourceType, inventoryCapacity);
        }
        resourceRenderer.setMaxForType(ResourceType.Gold, 200);
    }
    
    public void Update()
    {
    }

    public void drainAllInto(ResourceInventory target)
    {
        foreach (var resourceType in Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>())
        {
            this.transferResourceInto(resourceType, target);
        }
    }

    public float transferResourceInto(ResourceType type, ResourceInventory target, float amount = -1)
    {
        if(amount == -1)
        {
            amount = getResource(type);
        }
        else if (amount < 0)
        {
            throw new NotImplementedException();
        }
        var toAdd = Mathf.Min(amount, getResource(type));

        var added = target.addResource(type, toAdd);
        this.setInventoryValue(type, getResource(type) - added);
        return added;
    }

    public float getResource(ResourceType type)
    {
        return inventory[type];
    }
    public float addResource(ResourceType type, float amount)
    {
        if (spaceFillingItems.Contains(type))
        {
            if (getFullRatio() >= 1)
            {
                return getResource(type);
            }
            var remainingSpace = Mathf.Max(0, this.inventoryCapacity - totalFullSpace);
            amount = Mathf.Min(remainingSpace, amount);
        }
        return setInventoryValue(type, inventory[type] + amount);
    }

    public void emptySpaceFillingInventory()
    {
        foreach (var item in spaceFillingItems)
        {
            setInventoryValue(item, 0);
        }
    }

    private float setInventoryValue(ResourceType type, float newValue)
    {
        this.resourceRenderer.setValue(type, newValue);
        return inventory[type] = newValue;
    }

    public float totalFullSpace
    {
        get => spaceFillingItems.Select(x => inventory[x]).Sum();
    }

    public float getFullRatio()
    {
        return totalFullSpace / inventoryCapacity;
    }
}
