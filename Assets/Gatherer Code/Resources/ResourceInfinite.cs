using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Gatherer_Code;
using System;
using TradeModeling.Inventories;

public class ResourceInfinite : MonoBehaviour, IResource
{
    public ResourceType _type => type;
    public ResourceType type;

    public void Eat(SpaceFillingInventory<ResourceType> inventory, float amount = -1)
    {
        if (amount == -1)
        {
            amount = inventory.inventoryCapacity;
        }
        var eatenInfo = inventory.Add(_type, amount);
        eatenInfo.Execute();
    }
}
