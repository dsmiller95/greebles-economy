﻿using Assets.Scripts.MovementExtensions;
using Assets.Scripts.Resources;
using Assets.Scripts.Resources.Inventory;
using Assets.Scripts.Trader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scripts.Home
{
    public class HomeBehavior : TradeStop
    {
        public ResourceInventory inventory;
        private IInventory<ResourceType> _inventory;

        public override IInventory<ResourceType> tradeInventory => _inventory;

        private void Awake()
        {
            _inventory = inventory.backingInventory;
        }

        /// <summary>
        /// Deposit all the items from the given inventory into this inventory
        /// </summary>
        /// <param name="inventoryToDrain">the inventory to drain</param>
        /// <returns>True if the home's inventory is full</returns>
        public bool depositAllGoods(IInventory<ResourceType> inventoryToDrain)
        {
            inventoryToDrain.DrainAllInto(_inventory, ResourceConfiguration.spaceFillingItems);
            return ((_inventory as ISpaceFillingInventoryAccess<ResourceType>)?.getFullRatio() ?? 0) >= 1;
        }
        public void withdrawAllGoods(IInventory<ResourceType> inventoryToDepositTo)
        {
            _inventory.DrainAllInto(inventoryToDepositTo, ResourceConfiguration.spaceFillingItems);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}