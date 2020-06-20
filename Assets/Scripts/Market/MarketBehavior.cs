using Assets.Scrips.Resources;
using Assets.Scrips.Resources.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Exchanges;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scrips.Market
{
    [Serializable]
    public struct SellPrice
    {
        public ResourceType type;
        public float price;
    }

    public class MarketBehavior : MonoBehaviour
    {
        public SellPrice[] sellPrices;


        private Dictionary<ResourceType, float> exchangeRates;
        public ResourceInventory inventory;
        public SpaceFillingInventory<ResourceType> _inventory;

        private void Awake()
        {
            exchangeRates = sellPrices.ToDictionary(x => x.type, x => x.price);
            this._inventory = inventory.backingInventory;
        }

        public MarketExchangeAdapter<ResourceType> GetExchangeAdapter()
        {
            return new MarketExchangeAdapter<ResourceType>(exchangeRates, ResourceType.Gold);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public Dictionary<ResourceType, ResourceSellResult> sellAllGoodsInInventory(SpaceFillingInventory<ResourceType> inventory)
        {
            return SellAllGoods(inventory, _inventory, ResourceConfiguration.spaceFillingItems, this.exchangeRates);
        }

        private static Dictionary<ResourceType, ResourceSellResult> SellAllGoods(SpaceFillingInventory<ResourceType> seller, SpaceFillingInventory<ResourceType> consumer, ResourceType[] types, Dictionary<ResourceType, float> prices)
        {
            var result = seller.DrainAllInto(consumer, types)
                .Select(pair =>
                {
                    var sellResult = new ResourceSellResult(pair.Value, prices[pair.Key]);
                    return new { type = pair.Key, sellResult };
                })
                .ToDictionary(x => x.type, x => x.sellResult);

            seller.Add(ResourceType.Gold, result.Values.Sum(sellResult => sellResult.totalRevenue)).Execute();
            return result;
        }
    }
}