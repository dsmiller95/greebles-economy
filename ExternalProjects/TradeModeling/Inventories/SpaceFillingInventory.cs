using System;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Economics;

namespace TradeModeling.Inventories
{
    public class SpaceFillingInventory<T> : BasicInventory<T>
    {
        protected ISet<T> spaceFillingItems;

        public SpaceFillingInventory(
            int capacity,
            IDictionary<T, float> initialItems,
            ICollection<T> spaceFillingItems,
            T moneyType) : base(initialItems, moneyType)
        {
            _inventoryCapacity = capacity;
            this.spaceFillingItems = new HashSet<T>(spaceFillingItems);
        }

        private SpaceFillingInventory(SpaceFillingInventory<T> other) : base(other)
        {
            _inventoryCapacity = other._inventoryCapacity;
            spaceFillingItems = new HashSet<T>(other.spaceFillingItems);
        }

        private int _inventoryCapacity;
        public int inventoryCapacity
        {
            get => GetInventoryCapacity();
            set => SetInventoryCapacity(value);
        }
        protected int GetInventoryCapacity()
        {
            return _inventoryCapacity;
        }

        protected virtual int SetInventoryCapacity(int newCapacity)
        {
            return _inventoryCapacity = newCapacity;
        }

        /// <summary>
        /// Attempts to add as much of amount as possible into this inventory.
        /// </summary>
        /// <param name="type">the type of resource to add</param>
        /// <param name="amount">the maximum amount of resource to add to the inventory</param>
        /// <returns>An option to execute the transfer, wrapping the amount of the resource that was actually added</returns>
        public override ActionOption<float> Add(T type, float amount)
        {
            if (!spaceFillingItems.Contains(type))
            {
                return base.Add(type, amount);
            }

            if (getFullRatio() >= 1)
            {
                return new ActionOption<float>(0, () => { });
            }
            var remainingSpace = Math.Max(0, inventoryCapacity - totalFullSpace);
            amount = Math.Min(remainingSpace, amount);
            return base.Add(type, amount);
        }

        public float totalFullSpace => spaceFillingItems.Select(x => inventory[x]).Sum();

        public float getFullRatio()
        {
            return totalFullSpace / inventoryCapacity;
        }

        /// <summary>
        /// Determine if it's possible to fit any more of the given item in this inventory
        /// </summary>
        /// <param name="resource">The item to attempt to fit</param>
        /// <returns>True if its possible to fit any amount of <paramref name="resource"/> into this inventory</returns>
        public override bool CanFitMoreOf(T resource)
        {
            if (!spaceFillingItems.Contains(resource))
            {
                return base.CanFitMoreOf(resource);
            }
            return getFullRatio() < 1;
        }

        public override IExchangeInventory CreateSimulatedClone()
        {
            return new SpaceFillingInventory<T>(this);
        }
    }
}