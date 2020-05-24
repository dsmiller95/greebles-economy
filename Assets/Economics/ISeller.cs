using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{
    public interface ISeller
    {

        /// <summary>
        /// Sell of a certain amount of the resource
        /// </summary>
        /// <param name="amount">the amount to sell</param>
        /// <param name="execute">Whether or not to actually execute the sell</param>
        /// <returns>the amount gained from selling exactly amount</returns>
        float Sell(float amount, bool execute);

        /// <summary>
        /// Determines whether or not this seller is capable of executing a sell at this time.
        ///     An example of when it cannot execute a sell is when the inventory is empty
        /// </summary>
        /// <returns>whether or not a sell of any amount can be executed</returns>
        bool CanSell();
    }
}
