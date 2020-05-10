using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gatherer_Code;
using UnityEngine;

public struct ResourceChanged
{
    public ResourceChanged(ResourceType type, float newValue)
    {
        this.type = type;
        this.newValue = newValue;
    }
    public ResourceType type;
    public float newValue;
}

public class ResourceInventory : MonoBehaviour
{
    public int inventoryCapacitySetForUI = 10;
    private int _inventoryCapacity;
    public int inventoryCapacity {
        get => _inventoryCapacity;
        set {
            _inventoryCapacity = value;
            foreach (var resourceType in spaceFillingItems)
            {
                resourceCapacityChanges?.Invoke(this, new ResourceChanged(resourceType, inventoryCapacity));
            }
        }
    }

    public event EventHandler<ResourceChanged> resourceAmountChanges;
    public event EventHandler<ResourceChanged> resourceCapacityChanges;

    private Dictionary<ResourceType, float> inventory;

    public ResourceType[] spaceFillingItems = new ResourceType[] { ResourceType.Food, ResourceType.Wood };

    void Awake()
    {
        this.inventoryCapacity = inventoryCapacitySetForUI; 
        inventory = new Dictionary<ResourceType, float>();
        var resourceTypes = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();
        foreach (var resource in resourceTypes)
        {
            // create the key with default. Emit set events in Start(); once everyone has had a chance to subscribe to updates
            inventory[resource] = 0;
        }
    }

    public void Start()
    {
        var resourceTypes = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();
        foreach (var resource in resourceTypes)
        {
            this.setInventoryValue(resource, 0);
        }
        foreach (var resourceType in spaceFillingItems)
        {
            resourceCapacityChanges?.Invoke(this, new ResourceChanged(resourceType, inventoryCapacity));
        }
        resourceCapacityChanges?.Invoke(this, new ResourceChanged(ResourceType.Gold, 200));
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
    /// <summary>
    /// Attempts to add as much of amount as possible into this inventory.
    /// </summary>
    /// <param name="type">the type of resource to add</param>
    /// <param name="amount">the maximum amount of resource to add to the inventory</param>
    /// <returns>the amount of the resource that was actuall added</returns>
    public float addResource(ResourceType type, float amount)
    {
        if (spaceFillingItems.Contains(type))
        {
            if (getFullRatio() >= 1)
            {
                return 0;
            }
            var remainingSpace = Mathf.Max(0, this.inventoryCapacity - totalFullSpace);
            amount = Mathf.Min(remainingSpace, amount);
        }
        setInventoryValue(type, inventory[type] + amount);
        return amount;
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
        this.resourceAmountChanges?.Invoke(this, new ResourceChanged(type, newValue));
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
