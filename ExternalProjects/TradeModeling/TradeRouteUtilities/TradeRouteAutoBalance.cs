using System;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Inventories;

namespace TradeModeling.TradeRouteUtilities
{
    [Serializable]
    public class ResourceTrade<T> where T : System.Enum
    {
        /// <summary>
        /// The type of resource to move
        /// </summary>
        public T type;
        /// <summary>
        /// the amount of resource to move into the market's inventory. Set to a negative value to withdraw
        ///     resources from the market
        /// </summary>
        public float amount;
    }
    public static class TradeRouteAutoBalance
    {
        public static ResourceTrade<T>[][] GetTradesWhichBalanceInventories<T>(IList<SpaceFillingInventory<T>> inventories, T[] resourcesToTrade, bool roundToInts = false)
            where T : System.Enum
        {
            var averageInventoryAmounts = resourcesToTrade.ToDictionary(x => x, x => 0f);
            foreach (var inventory in inventories)
            {
                foreach (var resource in resourcesToTrade)
                {
                    averageInventoryAmounts[resource] += inventory.Get(resource);
                }
            }

            foreach (var resource in resourcesToTrade)
            {
                averageInventoryAmounts[resource] /= inventories.Count;
            }

            var tradeAmounts = inventories.Select(inventory =>
                GetResourceTradesFromInventoryAndAverage(inventory, averageInventoryAmounts)
                .Where(trade => trade.amount != 0)
                .ToArray()
                );

            return tradeAmounts.ToArray();
        }

        private static IEnumerable<ResourceTrade<T>> GetResourceTradesFromInventoryAndAverage<T>(SpaceFillingInventory<T> inventory, IDictionary<T, float> averageInventoryAmounts)
            where T : System.Enum
        {
            foreach (var resource in averageInventoryAmounts.Keys)
            {
                var resourceDiff = averageInventoryAmounts[resource] - inventory.Get(resource);
                yield return new ResourceTrade<T>
                {
                    type = resource,
                    amount = resourceDiff
                };
            }
        }
    }
}
