using Assets.MapGen.TileManagement;
using Assets.Scripts.MovementExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Market
{
    /// <summary>
    /// A class used to initialize the data required for markets in a static way
    ///     Used to compute the voroni mapping used to show the relevant service ranges
    /// </summary>
    public class MarketDataInitializer
    {

        public void CalculateServiceRanges(HexTileMapManager mapManager, IList<MarketBehavior> allMarkets)
        {
            var marketMembers = allMarkets.Select(x => x.GetComponentInParent<HexMember>());
        }
    }
}
