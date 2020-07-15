using Assets.Scripts.Resources;
using Assets.Scripts.Resources.Inventory;
using Assets.Scripts.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scripts.Castle
{
    public class CastleBehavior : TradeStop
    {
        public ResourceInventory inventory;
        private SpaceFillingInventory<ResourceType> _inventory;
        public override SpaceFillingInventory<ResourceType> tradeInventory => _inventory;

    }
}
