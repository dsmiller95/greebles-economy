using Assets.MapGen.TileManagement;
using Assets.Scripts.MovementExtensions;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Market
{
    /// <summary>
    /// A class used to initialize the data required for markets in a static way
    ///     Used to compute the voroni mapping used to show the relevant service ranges
    /// </summary>
    public class MarketDataInitializer
    {
        public static void CalculateServiceRanges(HexTileMapManager manager)
        {
            var allMarkets = manager.GetAllOfType<MarketBehavior>().ToList();
            Debug.Log($"Initializing {allMarkets.Count} markets");
            foreach (var market in allMarkets)
            {
                var marketMember = market.GetComponentInParent<HexMember>();

                var myHexPosition = marketMember.PositionInTileMap;
                var effectiveRange = HexTileMapManager
                    .GetPositionsWithinJumpDistance(myHexPosition, 2);
                market.myServiceRange = effectiveRange.ToArray();
            }

            //var marketMembers = allMarkets.Select(x => x.GetComponentInParent<HexMember>());
        }
    }
}
