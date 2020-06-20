using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scripts.Resources.Inventory
{
    [Serializable]
    public struct StartingInventoryAmount
    {
        public ResourceType type;
        public float amount;
    }

    public class ResourceInventory : MonoBehaviour
    {
        public int inventoryCapacitySetForUI = 10;

        public StartingInventoryAmount[] startingInventoryAmounts;

        public NotifyingInventory<ResourceType> backingInventory
        {
            get;
            private set;
        }

        void Awake()
        {
            var initialInventory = new Dictionary<ResourceType, float>();
            var resourceTypes = Enum.GetValues(typeof(ResourceType)).Cast<ResourceType>();
            foreach (var resource in resourceTypes)
            {
                // create the key with default. Emit set events in Start(); once everyone has had a chance to subscribe to updates
                initialInventory[resource] = 0;
            }
            foreach (var startingAmount in startingInventoryAmounts)
            {
                initialInventory[startingAmount.type] = startingAmount.amount;
            }
            backingInventory = new NotifyingInventory<ResourceType>(
                this.inventoryCapacitySetForUI,
                initialInventory,
                ResourceConfiguration.spaceFillingItems,
                ResourceType.Gold);
        }

        public void Start()
        {
            this.backingInventory.NotifyAll(new[]
            {
            new ResourceChanged<ResourceType>(ResourceType.Gold, 200)
        });
        }

        public void Update()
        {
        }
    }
}