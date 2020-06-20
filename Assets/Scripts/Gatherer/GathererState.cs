using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scrips.Gatherer
{
    [Flags]
    public enum GathererState
    {
        Gathering = 1,
        GoingHome = 2,
        Selling = 4,
        GoingHomeToEat = 8,
        Consuming = 16,
        All = Gathering | GoingHome | Selling | GoingHomeToEat | Consuming
    }
}