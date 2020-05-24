using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Gatherer_Code.Market
{
    public class ResourceSellResult
    {
        public ResourceSellResult(float items, float price)
        {
            this.soldItems = items;
            this.sellPrice = price;
        }

        /// <summary>
        /// How many items were sold
        /// </summary>
        public float soldItems;
        /// <summary>
        /// The price at which each item was sold
        /// </summary>
        public float sellPrice;
        /// <summary>
        /// The total amount of money exchanged
        /// </summary>
        public float totalRevenue { get => soldItems * sellPrice; }
    }
}
