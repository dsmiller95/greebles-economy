using System;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Economics;

namespace TradeModeling.Inventories
{
    public class SpaceFillingInventory<T> : IExchangeInventory
    {

        private IDictionary<T, float> inventory;
        protected ISet<T> spaceFillingItems;
        protected T moneyType;

        public SpaceFillingInventory(
            int capacity,
            IDictionary<T, float> initialItems,
            ICollection<T> spaceFillingItems,
            T moneyType)
        {
            _inventoryCapacity = capacity;
            inventory = new Dictionary<T, float>(initialItems);
            this.spaceFillingItems = new HashSet<T>(spaceFillingItems);
            this.moneyType = moneyType;
        }

        private SpaceFillingInventory(SpaceFillingInventory<T> other)
        {
            _inventoryCapacity = other._inventoryCapacity;
            inventory = new Dictionary<T, float>(other.inventory);
            spaceFillingItems = new HashSet<T>(other.spaceFillingItems);
            moneyType = other.moneyType;
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

        public Dictionary<T, float> GetCurrentResourceAmounts()
        {
            return this.inventory.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Drain all the items from this inventory of type res
        /// </summary>
        /// <param name="target">the inventory to drain the items into</param>
        /// <param name="res">the type of items to transfer. Could be a flags</param>
        /// <returns>A map from the resource type to the amount transfered</returns>
        public Dictionary<T, float> DrainAllInto(SpaceFillingInventory<T> target, T[] itemTypes)
        {
            var result = new Dictionary<T, float>();
            foreach (var itemType in itemTypes)
            {
                var transferOption = transferResourceInto(itemType, target, Get(itemType));

                result[itemType] = transferOption.info;
                transferOption.Execute();
            }
            return result;
        }

        /// <summary>
        /// Transfer <paramref name="amount"/> of <paramref name="type"/> into <paramref name="target"/>
        /// </summary>
        /// <param name="type">the type of resource to transfer</param>
        /// <param name="target">the inventory to transfer into</param>
        /// <param name="amount">the amount to transfer</param>
        /// <returns>An option to execute the transfer, wrapping the amount which would be transferred</returns>
        public ActionOption<float> transferResourceInto(T type, SpaceFillingInventory<T> target, float amount)
        {
            if (amount < 0)
            {
                return target.transferResourceInto(type, this, -amount)
                    .Then(added => -added);
            }
            var toAdd = Math.Min(amount, Get(type));

            return target
                .Add(type, toAdd)
                .Then(added => added, totalAdded =>
                {
                    SetInventoryValue(type, Get(type) - totalAdded);
                });
        }

        public float Get(T type)
        {
            return inventory[type];
        }

        protected IEnumerable<T> GetAllResourceTypes()
        {
            return inventory.Keys;
        }
        /// <summary>
        /// Attempts to add as much of amount as possible into this inventory.
        /// </summary>
        /// <param name="type">the type of resource to add</param>
        /// <param name="amount">the maximum amount of resource to add to the inventory</param>
        /// <returns>An option to execute the transfer, wrapping the amount of the resource that was actually added</returns>
        public ActionOption<float> Add(T type, float amount)
        {
            if (amount < 0)
            {
                throw new NotImplementedException("cannot add a negative amount. use Consume for that purpose");
            }
            if (spaceFillingItems.Contains(type))
            {
                if (getFullRatio() >= 1)
                {
                    return new ActionOption<float>(0, () => { });
                }
                var remainingSpace = Math.Max(0, inventoryCapacity - totalFullSpace);
                amount = Math.Min(remainingSpace, amount);
            }
            return new ActionOption<float>(amount, () =>
            {
                SetInventoryValue(type, inventory[type] + amount);
            });
        }

        /// <summary>
        /// Consume up to a certain amount out of the inventory
        /// </summary>
        /// <param name="type">the type to consume</param>
        /// <param name="amount">the amount to attempt to consume</param>
        public ActionOption<float> Consume(T type, float amount)
        {
            if (amount < 0)
            {
                throw new NotImplementedException();
            }

            var toConsume = Math.Min(amount, Get(type));

            return new ActionOption<float>(toConsume, () =>
            {
                SetInventoryValue(type, Get(type) - toConsume);
            });
        }

        protected virtual float SetInventoryValue(T type, float newValue)
        {
            inventory[type] = newValue;
            return newValue;
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
        public bool CanFitMoreOf(T resource)
        {
            if (spaceFillingItems.Contains(resource))
            {
                return getFullRatio() < 1;
            }
            return true;
        }

        public float GetCurrentFunds()
        {
            return Get(moneyType);
        }

        public IExchangeInventory CreateSimulatedClone()
        {
            return new SpaceFillingInventory<T>(this);
        }

        public string ToString(Func<T, string> serializer)
        {
            return MyUtilities.SerializeDictionary(inventory, serializer, num => num.ToString());
        }
    }
}