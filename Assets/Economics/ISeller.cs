using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{
    public interface ISeller<Self, Other>
        where Self : class, IExchangeInventory
        where Other : class, IExchangeInventory
    {
        /// <summary>
        /// Sell of a certain amount of the resource. Should -only- change the state of <paramref name="inventory"/>
        /// </summary>
        /// <param name="amount">the amount to sell</param>
        /// <param name="execute">Whether or not to actually execute the sell</param>
        /// <param name="selfInventory">The inventory to sell from</param>
        /// <param name="otherInventory">The inventory to sell to</param>
        /// <returns>the amount gained from selling exactly amount</returns>
        ExchangeResult Sell(float amount, bool execute, Self selfInventory, Other otherInventory);

        /// <summary>
        /// Determines whether or not this seller is capable of executing a sell at this time.
        ///     An example of when it cannot execute a sell is when the inventory is empty
        /// </summary>
        /// <param name="selfInventory">The inventory selling from</param>
        /// <param name="otherInventory">The inventory to sell to</param>
        /// <returns>whether or not a sell of any amount can be executed</returns>
        bool CanSell(Self selfInventory, Other otherInventory);
    }
}
