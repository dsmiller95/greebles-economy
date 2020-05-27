using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeModeling.Economics
{

    public interface IIncrementalFunction
    {
        /// <summary>
        /// Calculate the gain/loss of moving along the function by increment
        /// </summary>
        /// <returns>The gain/loss from moving across the function by increment</returns>
        float GetIncrementalValue(float startPoint, float increment);

        /// <summary>
        /// Gets the instantaneous rate of change at the given point
        /// </summary>
        /// <param name="startPoint"></param>
        /// <returns>the instantaneous rate of change at this point</returns>
        // float GetDerivativeAtPoint(float startPoint);
    }
}
