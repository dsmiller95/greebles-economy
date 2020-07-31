using Assets.Scripts.Resources;
using System.Collections.Generic;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scripts.Trader
{
    public abstract class TradeStop : MonoBehaviour
    {
        public abstract IInventoryItemSource<ResourceType> tradeInventory { get; }

        /// <summary>
        /// dictionary used to specify the amount that the trader should attempt to 
        /// put into this inventory, in ideal conditions.
        /// </summary>
        public Dictionary<ResourceType, float> targetInventoryAmounts = new Dictionary<ResourceType, float>();

        public void Start()
        {
            var source = tradeInventory;
            if(source is ISpaceFillingItemSource<ResourceType> spaceFilling)
            {
                foreach (ResourceType resource in spaceFilling.SpaceFillingItems)
                {
                    targetInventoryAmounts[resource] = spaceFilling.inventoryCapacity;
                }
            }
        }
    }
}
