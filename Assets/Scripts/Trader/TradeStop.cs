using Assets.Scripts.Resources;
using System.Collections.Generic;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scripts.Trader
{
    public abstract class TradeStop : MonoBehaviour
    {
        public abstract SpaceFillingInventory<ResourceType> tradeInventory { get; }

        /// <summary>
        /// dictionary used to specify the amount that the trader should attempt to 
        /// put into this inventory, in ideal conditions.
        /// </summary>
        public Dictionary<ResourceType, float> targetInventoryAmounts = new Dictionary<ResourceType, float>();

        public void Start()
        {
            foreach(ResourceType resource in System.Enum.GetValues(typeof(ResourceType)))
            {
                targetInventoryAmounts[resource] = tradeInventory.inventoryCapacity;
            }
        }
    }
}
