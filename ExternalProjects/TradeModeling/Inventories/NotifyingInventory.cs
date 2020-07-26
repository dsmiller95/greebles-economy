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

    public class NotifyingInventory<T> : SpaceFillingInventory<T>, INotifyingInventory<T>
    {
        public event EventHandler<ResourceChanged<T>> resourceCapacityChanges
        {
            add
            {
                ((INotifyingInventory<T>)myNotifier).resourceCapacityChanges += value;
            }

            remove
            {
                ((INotifyingInventory<T>)myNotifier).resourceCapacityChanges -= value;
            }
        }

        public event EventHandler<ResourceChanged<T>> resourceAmountChanged
        {
            add
            {
                ((INotifyingInventory<T>)myNotifier).resourceAmountChanged += value;
            }

            remove
            {
                ((INotifyingInventory<T>)myNotifier).resourceAmountChanged -= value;
            }
        }

        private InventoryNotifier<T> myNotifier;

        public NotifyingInventory(int capacity, IDictionary<T, float> initialItems, ICollection<T> spaceFillingItems, T moneyType, float defaultCapacity)
            : base(capacity, initialItems, spaceFillingItems, moneyType)
        {
            myNotifier = new InventoryNotifier<T>(this.itemSource, defaultCapacity);
        }

        public void NotifyAll()
        {
            ((INotifyingInventory<T>)myNotifier).NotifyAll();
        }
    }
}