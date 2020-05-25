using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{
    public interface ISeller<T> where T : IExchangeInventory
    {

        /// <summary>
        /// Sell of a certain amount of the resource. Should -only- change the state of <paramref name="inventory"/>
        /// </summary>
        /// <param name="inventory">The inventory to operate on</param>
        /// <param name="amount">the amount to sell</param>
        /// <param name="execute">Whether or not to actually execute the sell</param>
        /// <returns>the amount gained from selling exactly amount</returns>
        float Sell(T inventory, float amount, bool execute);

        /// <summary>
        /// Determines whether or not this seller is capable of executing a sell at this time.
        ///     An example of when it cannot execute a sell is when the inventory is empty
        /// </summary>
        /// <param name="inventory">The inventory to operate on</param>
        /// <returns>whether or not a sell of any amount can be executed</returns>
        bool CanSell(T inventory);
    }
}
