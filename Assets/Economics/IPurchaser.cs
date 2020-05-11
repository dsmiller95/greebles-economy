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
        /// Get the cost to purchase a given amount
        /// </summary>
        /// <param name="amount">the amount to purchase</param>
        /// <returns>the price of purchasing exactly amount</returns>
        float PurchaseCost(float amount);

        /// <summary>
        /// Execute a purchase of a certain amount
        /// </summary>
        /// <param name="amount">the amount to purchase</param>
        /// <returns>The cost of the purchase</returns>
        float ExecutePurchase(float amount);
    }
}
