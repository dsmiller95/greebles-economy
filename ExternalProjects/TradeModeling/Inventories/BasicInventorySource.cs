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
    public class BasicInventorySource<T> : IInventoryItemSource<T>
    {
        protected IDictionary<T, float> inventory;

        public BasicInventorySource(
            IDictionary<T, float> initialItems)
        {
            inventory = new Dictionary<T, float>(initialItems);
        }

        protected BasicInventorySource(BasicInventorySource<T> other)
        {
            inventory = new Dictionary<T, float>(other.inventory);
        }

        public Dictionary<T, float> GetCurrentResourceAmounts()
        {
            return inventory.ToDictionary(x => x.Key, x => x.Value);
        }

        public float Get(T type)
        {
            return inventory[type];
        }

        public IEnumerable<T> GetAllResourceTypes()
        {
            return inventory.Keys;
        }

        public virtual ActionOption<float> SetAmount(T type, float amount)
        {
            // cannot have a negative amount
            var newInventoryAmount = Math.Max(amount, 0);
            return new ActionOption<float>(newInventoryAmount, () =>
            {
                SetInventoryValue(type, newInventoryAmount);
            });
        }

        private float SetInventoryValue(T type, float newValue)
        {
            inventory[type] = newValue;
            return newValue;
        }

        public virtual bool CanFitMoreOf(T resource)
        {
            return true;
        }
        public virtual IInventoryItemSource<T> Clone()
        {
            return new BasicInventorySource<T>(this);
        }

        public string ToString(Func<T, string> serializer)
        {
            return MyUtilities.SerializeDictionary(GetCurrentResourceAmounts(), serializer, num => num.ToString());
        }
    }
}