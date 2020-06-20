using Assets.Scrips.Resources.Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scrips.Resources.UI
{
    public class ResourceInventoryTimeSeries : ResourceTimeSeriesAdapter
    {

        public ResourceInventory inventory;
        private SpaceFillingInventory<ResourceType> _inventory;

        private void Awake()
        {
            _inventory = inventory.backingInventory;
        }

        protected override float GetResourceValue(ResourceType resourceType)
        {
            return this._inventory.Get(resourceType);
        }
    }
}