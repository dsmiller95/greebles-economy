using System;
using System.Collections.Generic;
using TradeModeling.Economics;

namespace TradeModeling.Inventories
{

    public struct ResourceChanged<T>
    {
        public ResourceChanged(T type, float newValue)
        {
            this.type = type;
            this.newValue = newValue;
        }
        public T type;
        public float newValue;
    }

    public class NotifyingInventory<T> : SpaceFillingInventory<T>
    {
        //public Subject<ResourceChanged<T>> resourceChangeSubject;

        public event EventHandler<ResourceChanged<T>> resourceAmountChanged;
        public event EventHandler<ResourceChanged<T>> resourceCapacityChanges;

        public NotifyingInventory(int capacity, IDictionary<T, float> initialItems, ICollection<T> spaceFillingItems, T moneyType)
            : base(capacity, initialItems, spaceFillingItems, moneyType)
        {
        }

        /// <summary>
        /// Send out an event on all resources with the current values and capacities; if applicable
        /// </summary>
        /// <param name="extraCapacityChanges">any extra capacity change events to emit. This can be used to help configure listeners
        ///     attached to non-space-filling items that still need some capacity information</param>
        public void NotifyAll(IEnumerable<ResourceChanged<T>> extraCapacityChanges)
        {
            // Make sure the capacity changes happen first -- otherwise, the listeners might not have made room for the amounts
            NotifyAllCapacityChange();
            foreach (var capacityChange in extraCapacityChanges)
            {
                resourceCapacityChanges?.Invoke(this, capacityChange);
            }
            foreach (var resource in GetAllResourceTypes())
            {
                resourceAmountChanged?.Invoke(this, new ResourceChanged<T>(resource, Get(resource)));
            }
        }

        protected override int SetInventoryCapacity(int newCapacity)
        {
            var actualNewCapacity = base.SetInventoryCapacity(newCapacity);
            NotifyAllCapacityChange();
            return actualNewCapacity;
        }

        private void NotifyAllCapacityChange()
        {
            foreach (var resourceType in (this.itemSource as SpaceFillingInventorySource<T>).SpaceFillingItems)
            {
                resourceCapacityChanges?.Invoke(this, new ResourceChanged<T>(resourceType, GetInventoryCapacity()));
            }
        }

        protected override ActionOption<float> SetInventoryValue(T type, float attemptNewValue)
        {
            return base
                .SetInventoryValue(type, attemptNewValue)
                .Then(actualNewValue =>
                    {
                        resourceAmountChanged?.Invoke(this, new ResourceChanged<T>(type, actualNewValue));
                    });
        }
    }
}