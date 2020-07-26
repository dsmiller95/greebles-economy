using System.Collections.Generic;

namespace TradeModeling.Inventories
{
    public class SpaceFillingInventory<T> : BasicInventory<T>
    {
        public SpaceFillingInventory(
            int capacity,
            IDictionary<T, float> initialItems,
            ICollection<T> spaceFillingItems,
            T moneyType) : base(new SpaceFillingInventorySource<T>(initialItems, spaceFillingItems, capacity), moneyType)
        {
        }

        private SpaceFillingInventory(SpaceFillingInventory<T> other) : base(other)
        {
        }

        public int inventoryCapacity
        {
            get => GetInventoryCapacity();
            set => SetInventoryCapacity(value);
        }
        protected int GetInventoryCapacity()
        {
            return (itemSource as ISpaceFillingItemSource<T>).inventoryCapacity;
        }

        protected virtual int SetInventoryCapacity(int newCapacity)
        {
            return (itemSource as ISpaceFillingItemSource<T>).inventoryCapacity = newCapacity;
        }

        public float totalFullSpace => (itemSource as ISpaceFillingItemSource<T>).totalFullSpace;

        public float getFullRatio()
        {
            return (itemSource as ISpaceFillingItemSource<T>).getFullRatio();
        }

        public override IExchangeInventory CreateSimulatedClone()
        {
            return new SpaceFillingInventory<T>(this);
        }
    }
}