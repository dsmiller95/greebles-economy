using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gatherer_Code;
using UnityEngine;

public class SpaceFillingInventory<T>
{

    private IDictionary<T, float> inventory;
    protected ISet<T> spaceFillingItems;

    public SpaceFillingInventory(int capacity, IDictionary<T, float> initialItems, ICollection<T> spaceFillingItems)
    {
        this._inventoryCapacity = capacity;
        inventory = new Dictionary<T, float>(initialItems);
        this.spaceFillingItems = new HashSet<T>(spaceFillingItems);
    }

    private int _inventoryCapacity;
    public int inventoryCapacity {
        get => GetInventoryCapacity();
        set => SetInventoryCapacity(value);
    }
    protected int GetInventoryCapacity()
    {
        return _inventoryCapacity;
    }

    protected virtual int SetInventoryCapacity(int newCapacity)
    {
        return this._inventoryCapacity = newCapacity;
    }

    /// <summary>
    /// Drain all the items from this inventory of type res
    /// </summary>
    /// <param name="target">the inventory to drain the items into</param>
    /// <param name="res">the type of items to transfer. Could be a flags</param>
    /// <returns>A map from the resource type to the amount transfered</returns>
    public Dictionary<T, float> DrainAllInto(SpaceFillingInventory<T> target, T[] itemTypes)
    {
        var result = new Dictionary<T, float>();
        foreach (var itemType in itemTypes)
        {
            result[itemType] = this.transferResourceInto(itemType, target);
        }
        return result;
    }

    public float transferResourceInto(T type, SpaceFillingInventory<T> target, float amount = -1, bool execute = true)
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
        if (execute)
        {
            this.SetInventoryValue(type, getResource(type) - added);
        }
        return added;
    }

    /// <summary>
    /// Consume up to a certain amount out of the inventory
    /// </summary>
    /// <param name="type">the type to consume</param>
    /// <param name="amount">the amount to attempt to consume</param>
    public void Consume(T type, float amount)
    {
        if (amount < 0)
        {
            throw new NotImplementedException();
        }

        var toConsume = Mathf.Min(amount, getResource(type));
        this.SetInventoryValue(type, getResource(type) - toConsume);
    }

    public float getResource(T type)
    {
        return inventory[type];
    }

    protected IEnumerable<T> GetAllResourceTypes()
    {
        return this.inventory.Keys;
    }
    /// <summary>
    /// Attempts to add as much of amount as possible into this inventory.
    /// </summary>
    /// <param name="type">the type of resource to add</param>
    /// <param name="amount">the maximum amount of resource to add to the inventory</param>
    /// <returns>the amount of the resource that was actuall added</returns>
    public float addResource(T type, float amount)
    {
        if (this.spaceFillingItems.Contains(type))
        {
            if (getFullRatio() >= 1)
            {
                return 0;
            }
            var remainingSpace = Mathf.Max(0, this.inventoryCapacity - totalFullSpace);
            amount = Mathf.Min(remainingSpace, amount);
        }
        SetInventoryValue(type, inventory[type] + amount);
        return amount;
    }

    [Obsolete]
    public void emptySpaceFillingInventory()
    {
        foreach (var item in this.spaceFillingItems)
        {
            SetInventoryValue(item, 0);
        }
    }

    protected virtual float SetInventoryValue(T type, float newValue)
    {
        inventory[type] = newValue;
        return newValue;
    }

    public float totalFullSpace
    {
        get => this.spaceFillingItems.Select(x => inventory[x]).Sum();
    }

    public float getFullRatio()
    {
        return totalFullSpace / inventoryCapacity;
    }
}
