using Assets.MapGen.TileManagement;
using Assets.Scripts.MovementExtensions;
using Simulation.Tiling;
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
            //foreach (var market in allMarkets)
            //{
            //    var marketMember = market.GetComponentInParent<HexMember>();

            //    var myHexPosition = marketMember.PositionInTileMap;
            //    var effectiveRange = HexTileMapManager
            //        .GetPositionsWithinJumpDistance(myHexPosition, 2);
            //    market.myServiceRange = effectiveRange.ToArray();
            //}
            
            var allMarkets = manager.GetAllOfType<MarketBehavior>().ToList();
            Debug.Log($"Initializing {allMarkets.Count} markets");
            var marketPositions = allMarkets
                .Select(x => x.GetComponentInParent<HexMember>())
                .Select(x => x.PositionInTileMap)
                .ToList();

            var minimum = manager.tileMapMin;
            var minimumAxial = minimum.ToAxial();
            var maximum = manager.tileMapMax;

            var voroniData = VoroniTilingMapper.SetupVoroniMap(marketPositions, minimum, maximum);

            for(var row = 0; row < voroniData.Length; row++)
            {
                for(var col = 0; col < voroniData[row].Length; col++)
                {
                    var index = voroniData[row][col];
                    var coordinate = new OffsetCoordinate(col, row).ToAxial() + minimumAxial;
                    allMarkets[index].myServiceRange.Add(coordinate.ToOffset());
                }
            }
        }
    }
}
