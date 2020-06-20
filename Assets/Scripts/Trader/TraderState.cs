using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Trader
{
    [Flags]
    public enum TraderState
    {
        TradeTransit = 1,
        TradeExecute = 2,
        Initial = 4,
        All = TradeTransit | TradeExecute | Initial,
        None = 0
    }
}
