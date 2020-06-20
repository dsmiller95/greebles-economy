using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TradeModeling.Inventories;
using System.Threading.Tasks;

namespace Assets.Scrips.Resources
{
    public class ResourceInfinite : MonoBehaviour, IResource
    {
        public ResourceType _type => type;

        public float amount => float.MaxValue;

        public ResourceType type;
        public float gatherTime = 1;

        public async Task<bool> Eat(SpaceFillingInventory<ResourceType> inventory, float amount = -1)
        {
            await Task.Delay((int)(this.gatherTime * 1000));
            if (amount == -1)
            {
                amount = inventory.inventoryCapacity;
            }
            var eatenInfo = inventory.Add(_type, amount);
            eatenInfo.Execute();
            return true;
        }
    }
}