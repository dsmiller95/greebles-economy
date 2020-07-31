using System.Collections.Generic;
using TradeModeling.Economics;

namespace TradeModeling.Inventories
{
    public interface IInventory<T> : IExchangeInventory
    {
        IInventoryItemSource<T> GetBacker();

        /// <summary>
        /// Transfer <paramref name="amount"/> of <paramref name="type"/> into <paramref name="target"/>
        /// </summary>
        /// <param name="type">the type of resource to transfer</param>
        /// <param name="target">the inventory to transfer into</param>
        /// <param name="amount">the amount to transfer</param>
        /// <returns>An option to execute the transfer, wrapping the amount which would be transferred</returns>
        ActionOption<float> TransferResourceInto(T type, IInventory<T> target, float amount);
        float Get(T type);

        /// <summary>
        /// Determine if it's possible to fit any more of the given item in this inventory
        /// </summary>
        /// <param name="resource">The item to attempt to fit</param>
        /// <returns>True if its possible to fit any amount of <paramref name="resource"/> into this inventory</returns>
        bool CanFitMoreOf(T resource);
    }
}
