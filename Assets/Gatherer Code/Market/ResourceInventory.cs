using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Gatherer_Code;
using UnityEngine;

public class ResourceInventory : MonoBehaviour
{
    public int inventoryCapacity = 10;

    public int wood = 0;
    public int food = 0;

    public int getResource(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Food: return food;
            case ResourceType.Wood: return wood;
            default: throw new NotImplementedException();
        }
    }
    public int addResource(ResourceType type, int amount)
    {
        if(getFullRatio() >= 1)
        {
            return getResource(type);
        }
        switch (type)
        {
            case ResourceType.Food: return food += amount;
            case ResourceType.Wood: return wood += amount;
            default: throw new NotImplementedException();
        }
    }

    public void empty()
    {
        wood = food = 0;
    }

    public float getFullRatio()
    {
        return (wood + food) / inventoryCapacity;
    }
}
