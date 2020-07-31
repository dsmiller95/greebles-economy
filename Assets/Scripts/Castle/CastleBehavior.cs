using Assets.Scripts.Resources;
using Assets.Scripts.Resources.Inventory;
using Assets.Scripts.Trader;
using TradeModeling.Inventories;

namespace Assets.Scripts.Castle
{
    public class CastleBehavior : TradeStop
    {
        public ResourceInventory inventory;
        private IInventory<ResourceType> _inventory;
        public override IInventory<ResourceType> tradeInventory => _inventory;

        private void Awake()
        {
            _inventory = inventory.backingInventory;
        }
    }
}
