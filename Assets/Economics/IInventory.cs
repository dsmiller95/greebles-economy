using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{

    public interface IInventory
    {
        /// <summary>
        /// Get the current amount of items in posession
        /// </summary>
        /// <returns>amount in inventory</returns>
        float GetCurrentInventory();
    }
}
