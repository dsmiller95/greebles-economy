using System;
using System.Collections.Generic;

namespace TradeModeling.Inventories
{
    /// <summary>
    /// Represents and extension of the most basic inventory item source. It can contain items
    ///    and has a set capacity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISpaceFillingInventory<T> : IInventory<T>
    {
        Action<float> OnCapacityChanged { set; }
        IEnumerable<T> SpaceFillingItems { get; }
        int inventoryCapacity { get; set; }
        float totalFullSpace { get; }
        float remainingCapacity { get; }
        float getFullRatio();
    }
}
