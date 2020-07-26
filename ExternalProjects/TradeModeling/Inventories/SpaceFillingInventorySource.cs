using System;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Economics;

namespace TradeModeling.Inventories
{
    /// <summary>
    /// An inventory with infinite capacity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpaceFillingInventorySource<T> : BasicInventorySource<T>
    {
        protected ISet<T> spaceFillingItems;

        public SpaceFillingInventorySource(
            IDictionary<T, float> initialItems,
            ICollection<T> spaceFillingItems,
            int capacity) : base(initialItems)
        {
            _inventoryCapacity = capacity;
            this.spaceFillingItems = new HashSet<T>(spaceFillingItems);
        }

        protected SpaceFillingInventorySource(SpaceFillingInventorySource<T> other) : base(other)
        {
            _inventoryCapacity = other._inventoryCapacity;
            spaceFillingItems = new HashSet<T>(other.spaceFillingItems);
        }

        public IEnumerable<T> SpaceFillingItems => spaceFillingItems;

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

        public float totalFullSpace => spaceFillingItems.Select(x => Get(x)).Sum();

        public float remainingCapacity => this.inventoryCapacity - totalFullSpace;

        public float getFullRatio()
        {
            return totalFullSpace / inventoryCapacity;
        }

        public override ActionOption<float> SetAmount(T type, float amount)
        {
            var spaceRestrictedAmount = Math.Min(amount, Get(type) + remainingCapacity);
            return base.SetAmount(type, spaceRestrictedAmount);
        }

        public override bool CanFitMoreOf(T resource)
        {
            if (!spaceFillingItems.Contains(resource))
            {
                return base.CanFitMoreOf(resource);
            }
            return remainingCapacity > 0;
        }
        public override IInventoryItemSource<T> Clone()
        {
            return new SpaceFillingInventorySource<T>(this);
        }
    }
}