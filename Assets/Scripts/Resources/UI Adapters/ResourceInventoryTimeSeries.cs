using Assets.Scripts.Resources.Inventory;
using TradeModeling.Inventories;

namespace Assets.Scripts.Resources.UI
{
    public class ResourceInventoryTimeSeries : ResourceTimeSeriesAdapter
    {

        public ResourceInventory inventory;
        private TradingInventoryAdapter<ResourceType> _inventory;

        private void Awake()
        {
            _inventory = inventory.backingInventory;
        }

        protected override float GetResourceValue(ResourceType resourceType)
        {
            return _inventory.Get(resourceType);
        }
    }
}