using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{

    public struct PurchaseResult
    {
        /// <summary>
        /// The amount of money which is paid for the items
        /// </summary>
        public float cost;
        /// <summary>
        /// Actual amount of items purchased. this can be lower than requested if the selling inventory is empty
        /// </summary>
        public float amount;
    }
    public interface IPurchaser
    {
        /// <summary>
        /// Purchase given amount
        /// </summary>
        /// <param name="amount">the amount to purchase</param>
        /// <param name="execute">Whether or not to execute the purchase</param>
        /// <returns>A summary of the transaction</returns>
        PurchaseResult Purchase(float amount, bool execute);

        /// <summary>
        /// Determines whether or not this purchaser is capable of executing a purchase at this time.
        ///     An example of when it cannot execute a purchase is when the selling inventory is empty
        /// </summary>
        /// <returns>whether or not a purchase of any amount can be executed</returns>
        bool CanPurchase();
    }
}
