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
            var totalSpace = SetInventoryValueInOrder(type, amount).Sum(x => x.info);

            // cannot have a negative amount
            var nonNegativeAmount = Math.Max(amount, 0);
            var newAmount = Math.Min(nonNegativeAmount, totalSpace);
            return new ActionOption<float>(newAmount, () =>
            {
                SetInventoryValue(type, newAmount);
                OnResourceChanged?.Invoke(type, newAmount);
            });
        }

        private IEnumerable<ActionOption<float>> SetInventoryValueInOrder(T type, float newValue)
        {
            foreach(var inventory in inventorySources)
            {
                var option = inventory.SetAmount(type, newValue);
                yield return option;
                newValue -= option.info;
                if(newValue <= 1e-5)
                {
                    yield break;
                }
            }
        }

        private void SetInventoryValue(T type, float newValue)
        {
            var inventorySourcesToFill = inventorySources;
            var average = newValue / inventorySourcesToFill.Count;
            var totalItemsToAllocate = newValue;
            foreach (var inventory in inventorySourcesToFill)
            {
                var setOption = inventory.SetAmount(type, average);
                totalItemsToAllocate -= setOption.info;
                setOption.Execute();
            }
            inventorySourcesToFill = inventorySourcesToFill.Where(x => x.CanFitMoreOf(type)).ToList();

            while(totalItemsToAllocate > 0 && inventorySourcesToFill.Count > 0)
            {
                var averageAmountToAdd = totalItemsToAllocate / inventorySourcesToFill.Count;
                foreach(var inventory in inventorySourcesToFill)
                {
                    var addOption = inventory.Add(type, averageAmountToAdd);
                    totalItemsToAllocate -= addOption.info;
                    addOption.Execute();
                }
                inventorySourcesToFill = inventorySourcesToFill.Where(x => x.CanFitMoreOf(type)).ToList();
            }
        }

        public virtual bool CanFitMoreOf(T resource)
        {
            return inventorySources.Any(x => x.CanFitMoreOf(resource));
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