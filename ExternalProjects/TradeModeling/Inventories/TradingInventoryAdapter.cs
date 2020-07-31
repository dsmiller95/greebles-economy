using System;
using System.Collections.Generic;
using TradeModeling.Economics;

namespace TradeModeling.Inventories
{
    /// <summary>
    /// An inventory with infinite capacity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TradingInventoryAdapter<T> : IExchangeInventory, IInventory<T>
    {
        protected T moneyType;

        public IInventoryItemSource<T> itemSource;

        public TradingInventoryAdapter(
            IInventoryItemSource<T> source,
            T moneyType)
        {
            itemSource = source;
            this.moneyType = moneyType;
        }

        protected TradingInventoryAdapter(TradingInventoryAdapter<T> other) : this(other.itemSource.CloneSimulated(), other.moneyType)
        {
        }

        public IInventoryItemSource<T> GetBacker()
        {
            return itemSource;
        }

        /// <summary>
        /// Transfer <paramref name="amount"/> of <paramref name="type"/> into <paramref name="target"/>
        /// </summary>
        /// <param name="type">the type of resource to transfer</param>
        /// <param name="target">the inventory to transfer into</param>
        /// <param name="amount">the amount to transfer</param>
        /// <returns>An option to execute the transfer, wrapping the amount which would be transferred</returns>
        public ActionOption<float> TransferResourceInto(T type, IInventory<T> target, float amount)
        {
            return itemSource.transferResourceInto(type, target.GetBacker(), amount);
        }

        public float Get(T type)
        {
            return itemSource.Get(type);
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
            return new TradingInventoryAdapter<T>(this);
        }
    }
}