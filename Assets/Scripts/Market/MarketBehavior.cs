using Assets.Scripts.MovementExtensions;
using Assets.Scripts.Resources;
using Assets.Scripts.Resources.Inventory;
using Assets.Scripts.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using TradeModeling;
using TradeModeling.Exchanges;
using TradeModeling.Functions;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scripts.Market
{
    [Serializable]
    public struct SellPrice
    {
        public ResourceType type;
        public float sellPrice;
        public float buyPrice;
    }

    [RequireComponent(typeof(HexMember))]
    public class MarketBehavior : TradeStop
    {
        public SellPrice[] exchangeRates;

        private Dictionary<ResourceType, float> sellPriceDictionary;
        private Dictionary<ResourceType, float> purchasePriceDictionary;
        public ResourceInventory inventory;
        public SpaceFillingInventory<ResourceType> _inventory;


        public override SpaceFillingInventory<ResourceType> tradeInventory => _inventory;

        private void Awake()
        {
            sellPriceDictionary = exchangeRates.ToDictionary(x => x.type, x => x.sellPrice);
            purchasePriceDictionary = exchangeRates.ToDictionary(x => x.type, x => x.buyPrice);
            _inventory = inventory.backingInventory;
        }

        public IMarketExchangeAdapter<ResourceType> GetExchangeAdapter()
        {
            return new SigmoidMarketExchangeAdapter<ResourceType>(
                GetSellPriceFunctions(),
                GetPurchasePriceFunctions(),
                ResourceType.Gold);
        }

        public IDictionary<ResourceType, SigmoidFunctionConfig> GetSellPriceFunctions()
        {
            return MapPricesToConfigs(sellPriceDictionary);
        }
        public IDictionary<ResourceType, SigmoidFunctionConfig> GetPurchasePriceFunctions()
        {
            return MapPricesToConfigs(purchasePriceDictionary);
        }

        private IDictionary<ResourceType, SigmoidFunctionConfig> MapPricesToConfigs(IDictionary<ResourceType, float> prices)
        {
            return prices.SelectDictionary(x => new SigmoidFunctionConfig
            {
                range = tradeInventory.inventoryCapacity,
                yRange = x
            });
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        [Obsolete("Use the MarketExchangeAdapter provided by GetExchangeAdapter()", true)]
        public Dictionary<ResourceType, ResourceSellResult> sellAllGoodsInInventory(SpaceFillingInventory<ResourceType> inventory)
        {
            return SellAllGoods(inventory, _inventory, ResourceConfiguration.spaceFillingItems, sellPriceDictionary);
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