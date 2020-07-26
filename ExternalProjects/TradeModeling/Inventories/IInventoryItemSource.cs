using System;
using System.Collections.Generic;
using TradeModeling.Economics;

namespace TradeModeling.Inventories
{
    /// <summary>
    /// Represents the most basic form of an inventory: it only holds items
    ///     and does not interact with other inventories directly.
    /// Used to build inventories that are backed by multiple other inventories,
    ///     or any other bit of data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInventoryItemSource<T>
    {
        Action<T, float> OnResourceChanged { set; }

        Dictionary<T, float> GetCurrentResourceAmounts();

        float Get(T type);

        IEnumerable<T> GetAllResourceTypes();

        /// <summary>
        /// Sets the amount of a given type in this inventory
        /// </summary>
        /// <param name="type">the type of resource</param>
        /// <param name="amount">the new amount of the resour</param>
        /// <returns>An option to execute the transfer, wrapping the actual amount that will be set in the inventory</returns>
        ActionOption<float> SetAmount(T type, float amount);

        /// <summary>
        /// Determine if it's possible to fit any more of the given item in this inventory
        /// </summary>
        /// <param name="resource">The item to attempt to fit</param>
        /// <returns>True if its possible to fit any amount of <paramref name="resource"/> into this inventory</returns>
        bool CanFitMoreOf(T resource);

        /// <summary>
        /// Create an item source which will simulate the interactions on this inventory
        ///     this should include things like space restrictions and the current amount of items in the inventory
        ///     Should not copy event bindings or external references
        /// </summary>
        /// <returns>A simulated clone</returns>
        IInventoryItemSource<T> CloneSimulated();
    }
}
