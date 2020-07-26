using System.Collections.Generic;
using TradeModeling.Economics;

namespace TradeModeling.Inventories
{
    public interface IInventory<T> : IExchangeInventory
    {
        Dictionary<T, float> DrainAllInto(IInventory<T> target, T[] items);

        /// <summary>
        /// Transfer <paramref name="amount"/> of <paramref name="type"/> into <paramref name="target"/>
        /// </summary>
        /// <param name="type">the type of resource to transfer</param>
        /// <param name="target">the inventory to transfer into</param>
        /// <param name="amount">the amount to transfer</param>
        /// <returns>An option to execute the transfer, wrapping the amount which would be transferred</returns>
        ActionOption<float> transferResourceInto(T type, IInventory<T> target, float amount);
        float Get(T type);

        /// <summary>
        /// Attempts to add as much of amount as possible into this inventory.
        /// </summary>
        /// <param name="type">the type of resource to add</param>
        /// <param name="amount">the maximum amount of resource to add to the inventory</param>
        /// <returns>An option to execute the transfer, wrapping the amount of the resource that was actually added</returns>
        ActionOption<float> Add(T type, float amount);

        /// <summary>
        /// Consume up to a certain amount out of the inventory
        /// </summary>
        /// <param name="type">the type to consume</param>
        /// <param name="amount">the amount to attempt to consume</param>
        ActionOption<float> Consume(T type, float amount);
        /// <summary>
        /// Determine if it's possible to fit any more of the given item in this inventory
        /// </summary>
        /// <param name="resource">The item to attempt to fit</param>
        /// <returns>True if its possible to fit any amount of <paramref name="resource"/> into this inventory</returns>
        bool CanFitMoreOf(T resource);
    }
}
