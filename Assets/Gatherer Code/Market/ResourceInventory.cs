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
        resourceRenderer.setMaxForType(ResourceType.Gold, 200);
        foreach (var resourceType in spaceFillingItems)
        {
            resourceRenderer.setMaxForType(resourceType, inventoryCapacity);
        }
    }
    
    public void Update()
    {
    }

    public float getResource(ResourceType type)
    {
        return inventory[type];
    }
    public float addResource(ResourceType type, float amount)
    {
        if(getFullRatio() >= 1)
        {
            return getResource(type);
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
