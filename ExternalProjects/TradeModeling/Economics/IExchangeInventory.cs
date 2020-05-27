using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeModeling.Economics
{
    public interface IExchangeInventory
    {
        float GetCurrentFunds();

        /// <summary>
        /// Creates a copy of this exchange inventory to be used for evaluating simulated transactions
        ///     Executing transactions on the new inventory shouldn't effect the backing to this inventory
        /// </summary>
        /// <returns>A new simulated clone</returns>
        IExchangeInventory CreateSimulatedClone();
    }
}
