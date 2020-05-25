using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{
    public interface IExchangeInventory
    {
        float GetCurrentSelfMoney();

        /// <summary>
        /// Creates a copy of this exchange inventory to be used for evaluating simulated transactions
        ///     Executing transactions on the new inventory shouldn't effect the backing to this inventory
        /// </summary>
        /// <returns>A new simulated clone</returns>
        IExchangeInventory CreateSimulatedClone();
    }
}
