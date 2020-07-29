using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TradeModeling.Economics;

namespace TradeModeling.Inventories
{
    public enum CompositeDistributionMode
    {
        EVEN,
        EVEN_SNAP_TO_INT,
        IN_ORDER
    }

    /// <summary>
    /// An inventory which only stores items in a list of other inventory sources
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CompositeInventorySource<T> : IInventoryItemSource<T>
    {
        private IList<IInventoryItemSource<T>> inventorySources;

        public Action<T, float> OnResourceChanged { private get; set; }

        private ISet<T> AllResourceTypes;
        private CompositeDistributionMode distributionMode;

        public CompositeInventorySource(IList<IInventoryItemSource<T>> composingInventories, CompositeDistributionMode mode)
        {
            this.inventorySources = composingInventories;
            distributionMode = mode;
            AllResourceTypes = new HashSet<T>();
            foreach(var inv in inventorySources)
            {
                foreach(var type in inv.GetAllResourceTypes())
                {
                    AllResourceTypes.Add(type);
                }
            }
        }

        public Dictionary<T, float> GetCurrentResourceAmounts()
        {
            var result = new Dictionary<T, float>();
            foreach(var inv in inventorySources)
            {
                foreach(var amount in inv.GetCurrentResourceAmounts())
                {
                    if (!result.ContainsKey(amount.Key))
                    {
                        result[amount.Key] = 0f;
                    }
                    result[amount.Key] += amount.Value;
                }
            }
            return result;
        }

        public float Get(T type)
        {
            return inventorySources.Sum(inv => inv.Get(type));
        }

        public IEnumerable<T> GetAllResourceTypes()
        {
            return AllResourceTypes;
        }

        public virtual ActionOption<float> SetAmount(T type, float amount)
        {
            // cannot have a negative amount
            var newInventoryAmount = Math.Max(amount, 0);
            return new ActionOption<float>(newInventoryAmount, () =>
            {
                SetInventoryValue(type, newInventoryAmount);
                OnResourceChanged?.Invoke(type, newInventoryAmount);
            });
        }

        private float SetInventoryValue(T type, float newValue)
        {
            //TODO: set up some sort of distribution algoreythm
            //inventory[type] = newValue;
            return newValue;
        }

        public virtual bool CanFitMoreOf(T resource)
        {
            return true;
        }
        public virtual IInventoryItemSource<T> CloneSimulated()
        {
            throw new NotImplementedException();
            //TODO: hmmmm
            //return new BasicInventorySource<T>(this);
        }

        public string ToString(Func<T, string> serializer)
        {
            return MyUtilities.SerializeDictionary(GetCurrentResourceAmounts(), serializer, num => num.ToString());
        }
    }
}