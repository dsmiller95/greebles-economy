using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Gatherer_Code;
using System;
using TradeModeling.Inventories;
using System.Threading.Tasks;

public class Resource : MonoBehaviour, IResource
{
    public ResourceType _type => type;

    public float amount => this.value;

    public ResourceType type;
    public float value = 1;
    public float gatherTime = 1;

    private bool isCompletelyEaten = false;

    public async Task Eat(SpaceFillingInventory<ResourceType> inventory, float amount = -1)
    {
        if (amount == -1)
        {
            amount = this.value;
        }
        var eatOption = inventory.Add(_type, amount);
        lock (this)
        {
            /* Determine if this resource will be completely consumed by this operation
             * If it will be, mark as completely eaten and then get rid of the gameObject when this operation completes
             */
            if (this.isCompletelyEaten)
            {
                return;
            }
            this.value -= eatOption.info;
            if (Math.Abs(value) <= float.Epsilon * 10)
            {
                this.isCompletelyEaten = true;
            }
        }
        await Task.Delay((int)(this.gatherTime * 1000));
        if (this.isCompletelyEaten)
        {
            // Should only ever get here once
            Destroy(this.gameObject);
        }
        eatOption.Execute();
        return;
    }
}
