using System;
using System.Collections.Generic;
using TradeModeling.Economics;

namespace TradeModeling.Inventories
{
    /// <summary>
    /// An inventory with infinite capacity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BasicInventory<T> : IExchangeInventory, IInventory<T>
    {
        protected T moneyType;

        protected IInventoryItemSource<T> itemSource;

        public BasicInventory(
            IInventoryItemSource<T> source,
            T moneyType)
        {
            itemSource = source;
            this.moneyType = moneyType;
        }

        public BasicInventory(
            IDictionary<T, float> initialItems,
            T moneyType) : this(new BasicInventorySource<T>(initialItems), moneyType)
        {
        }

        protected BasicInventory(BasicInventory<T> other) : this(other.itemSource.CloneSimulated(), other.moneyType)
        {
        }

        public Dictionary<T, float> GetCurrentResourceAmounts()
        {
            return itemSource.GetCurrentResourceAmounts();
        }

        /// <summary>
        /// Drain all the items from this inventory of type res
        /// </summary>
        /// <param name="target">the inventory to drain the items into</param>
        /// <param name="res">the type of items to transfer. Could be a flags</param>
        /// <returns>A map from the resource type to the amount transfered</returns>
        public Dictionary<T, float> DrainAllInto(IInventory<T> target, T[] itemTypes)
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
        public ActionOption<float> transferResourceInto(T type, IInventory<T> target, float amount)
        {
            if (amount < 0)
            {
                return target.transferResourceInto(type, this, -amount)
                    .Then(added => -added);
            }
            var currentAmount = Get(type);

            var possibleNewAmount = SetInventoryValue(type, currentAmount - amount).info;
            var idealAddAmount = currentAmount - possibleNewAmount;

            return target
                .Add(type, idealAddAmount)
                .Then(addedAmount => SetInventoryValue(type, currentAmount - addedAmount))
                .Then(newAmount => currentAmount - newAmount);
        }

        public float Get(T type)
        {
            return itemSource.Get(type);
        }

        protected IEnumerable<T> GetAllResourceTypes()
        {
            return itemSource.GetAllResourceTypes();
        }
        /// <summary>
        /// Attempts to add as much of amount as possible into this inventory.
        /// </summary>
        /// <param name="type">the type of resource to add</param>
        /// <param name="amount">the maximum amount of resource to add to the inventory</param>
        /// <returns>An option to execute the transfer, wrapping the amount of the resource that was actually added</returns>
        public virtual ActionOption<float> Add(T type, float amount)
        {
            if (amount < 0)
            {
                throw new NotImplementedException("cannot add a negative amount. use Consume for that purpose");
            }

            var currentAmount = Get(type);
            return SetInventoryValue(type, currentAmount + amount)
                .Then(newAmount => newAmount - currentAmount);
        }

        /// <summary>
        /// Consume up to a certain amount out of the inventory
        /// </summary>
        /// <param name="type">the type to consume</param>
        /// <param name="amount">the amount to attempt to consume</param>
        /// <returns>an optional action representing the amount that was actually consumed</returns>
        public ActionOption<float> Consume(T type, float amount)
        {
            if (amount < 0)
            {
                throw new NotImplementedException();
            }

            var currentAmount = Get(type);
            return SetInventoryValue(type, currentAmount - amount)
                .Then(newAmount => currentAmount - newAmount);

        }

        protected virtual ActionOption<float> SetInventoryValue(T type, float newValue)
        {
            return itemSource.SetAmount(type, newValue);
        }

        /// <summary>
        /// Determine if it's possible to fit any more of the given item in this inventory
        /// </summary>
        /// <param name="resource">The item to attempt to fit</param>
        /// <returns>True if its possible to fit any amount of <paramref name="resource"/> into this inventory</returns>
        public virtual bool CanFitMoreOf(T resource)
        {
            return itemSource.CanFitMoreOf(resource);
        }

        public float GetCurrentFunds()
        {
            return Get(moneyType);
        }

        public virtual IExchangeInventory CreateSimulatedClone()
        {
            return new BasicInventory<T>(this);
        }

        public string ToString(Func<T, string> serializer)
        {
            return MyUtilities.SerializeDictionary(GetCurrentResourceAmounts(), serializer, num => num.ToString());
        }
    }
}