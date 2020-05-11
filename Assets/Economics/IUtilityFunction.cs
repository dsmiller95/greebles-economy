using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{

    public interface IUtilityFunction
    {
        /// <summary>
        /// Calculate the utility of getting one more thing
        /// </summary>
        /// <returns>The additional utility from gaining one more item</returns>
        float GetIncrementalUtility(float currentInventory, float increment);
    }
}
