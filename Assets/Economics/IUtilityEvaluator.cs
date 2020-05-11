﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{
    /// <summary>
    /// An object which can give the current incremental utility; keeping track of an inventory
    /// </summary>
    public interface IUtilityEvaluator
    {
        /// <summary>
        /// Calculate the utility of getting increment more
        ///     Acts like an integral; if increment is negative utility will be inverted
        /// </summary>
        /// <param name="increment">the additional amount of the item</param>
        /// <returns>The additional utility from gaining more item</returns>
        float GetIncrementalUtility(float increment);
    }
}
