using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gatherer_Code;
using UnityEngine;

public class ResourceInventory : MonoBehaviour
{
    public int inventoryCapacity = 10;

    public Dictionary<ResourceType, float> inventory;

    public ResourceType[] spaceFillingItems = new ResourceType[] { ResourceType.Food, ResourceType.Wood };

    public ResourceInventory()
    {
        inventory = new Dictionary<ResourceType, float>();
        var resourceTypes = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();
        foreach (var resource in resourceTypes)
        {
            inventory[resource] = 0;
        }
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
        return inventory[type] += amount;
    }

    public void emptySpaceFillingInventory()
    {
        foreach (var item in spaceFillingItems)
        {
            this.inventory[item] = 0;
        }
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
