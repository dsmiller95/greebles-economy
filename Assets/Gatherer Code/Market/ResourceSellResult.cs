using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Gatherer_Code.Market
{
    public class ResourceSellResult
    {
        public ResourceSellResult(int items, float price)
        {
            this.soldItems = items;
            this.sellPrice = price;
        }

        public int soldItems;
        public float sellPrice;
        public float totalRevenue { get => soldItems * sellPrice; }
    }
}
