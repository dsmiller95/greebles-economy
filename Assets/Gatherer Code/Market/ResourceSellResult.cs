using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Gatherer_Code.Market
{
    public class ResourceSellResult
    {
        public float WoodResult { get; set; }
        public float FoodResult { get; set; }
        public float TotalResult
        {
            get => WoodResult + FoodResult;
        }
    }
}
