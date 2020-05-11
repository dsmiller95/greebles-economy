using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{
    interface ISeller
    {

        /// <summary>
        /// Sell of a certain amount of the resource
        /// </summary>
        /// <param name="amount">the amount to sell</param>
        /// <param name="execute">Whether or not to actually execute the sell</param>
        /// <returns>the amount gained from selling exactly amount</returns>
        float Sell(float amount, bool execute);
    }
}
