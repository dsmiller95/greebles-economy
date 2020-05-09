using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Flags]
public enum GathererState
{
    Gathering = 1,
    GoingHome = 2,
    Selling = 4,
    All = 7
}
