using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{
    interface IPurchaser
    {
        /// <summary>
        /// Purchase given amount
        /// </summary>
        /// <param name="amount">the amount to purchase</param>
        /// <param name="execute">Whether or not to execute the purchase</param>
        /// <returns>the price of purchasing exactly amount</returns>
        float Purchase(float amount, bool execute);
    }
}
