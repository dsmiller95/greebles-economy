﻿using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Gatherer_Code;
using System;
using TradeModeling.Inventories;

public class Resource : MonoBehaviour, IResource
{
    public ResourceType _type => type;
    public ResourceType type;
    public float value = 1;

    private bool eaten = false;

    public void Eat(SpaceFillingInventory<ResourceType> inventory, float amount = -1)
    {
        if (amount == -1)
        {
            amount = this.value;
        }
        if (this.eaten)
        {
            return;
        }
        var eatenInfo = inventory.Add(_type, amount);
        this.value -= eatenInfo.info;
        eatenInfo.Execute();
        if(Math.Abs(value) <= float.Epsilon * 10)
        {
            this.eaten = true;
            Destroy(this.gameObject);
        }
    }
}
