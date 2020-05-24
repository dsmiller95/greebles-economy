using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Economics
{
    [Serializable]
    public struct WeightedRegion
    {
        public WeightedRegion(int beginning, float weight)
        {
            this.RegionBeginning = beginning;
            this.RegionWeight = weight;
        }
        public int RegionBeginning;
        public float RegionWeight;
    }

    public class InverseWeightedUtility : IIncrementalFunction
    {
        private WeightedRegion[] regions;
        private float offset;

        public InverseWeightedUtility(WeightedRegion[] regions, float offset = 1)
        {
            this.regions = regions;
            this.offset = offset;
        }

        /// <summary>
        /// Calculate the utility of getting one more thing
        /// </summary>
        /// <param name="currentInventory">the current amount of the Item</param>
        /// <returns>The additional utility from gaining one more item</returns>
        public float GetIncrementalValue(float currentInventory, float increment)
        {
            if(increment < 0)
            {
                return -GetIncrementalValue(currentInventory + increment, -increment);
            }
            if(Math.Abs(increment - 1) > 0.0001)
            {
                throw new NotImplementedException($"Cannot calculate incremental utility in increments other than 1. attempted {increment}");
            }
            var currentRegion = RegionAt(currentInventory);
            return currentRegion.RegionWeight * BaseUtility(currentInventory);
        }

        private float BaseUtility(float inventory)
        {
            return 1f / (inventory + offset);
        }

        private WeightedRegion RegionAt(float inventory)
        {
            try
            {
                return this.regions
                    .Where(region => region.RegionBeginning <= inventory)
                    .Last();
            } catch
            {
                throw new Exception($"no defined region at {inventory}");
            }
        }
    }
}
