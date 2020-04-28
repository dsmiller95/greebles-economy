using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Gatherer_Code
{
    public enum UserLayerMasks
    {
        Resources = 1 << 8,
        Gatherer = 1 << 9,
        Market = 1 << 10
    }
}
