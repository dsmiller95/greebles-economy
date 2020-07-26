using System;
using System.Collections.Generic;
using TradeModeling.Economics;
using TradeModeling.Functions;

namespace UnitTests.Economics
{
    public class TestBank
    {
        public float money;
    }

    class TestExchangeModel : IExchange<string, TestInventoryModel, TestInventoryModel>,
        IUtilityEvaluator<string, TestInventoryModel>
    {
        public IDictionary<string, IIncrementalFunction> utilityFunctions;
        public IDictionary<string, float> purchasePrices;
        public IDictionary<string, float> sellPrices;

        public float GetIncrementalUtility(string resourceType, TestInventoryModel self, float increment)
        {
            return utilityFunctions[resourceType].GetIncrementalValue(self.Get(resourceType), increment);
        }

        public ActionOption<ExchangeResult<string>> Purchase(string resourceType, float amount, TestInventoryModel self, TestInventoryModel market)
        {
            var purchasePrice = purchasePrices[resourceType];
            var marketInventory = market.Get(resourceType);
            var actualPurchase = Math.Min(amount, marketInventory);
            var price = actualPurchase * purchasePrice;

            return new ActionOption<ExchangeResult<string>>(new ExchangeResult<string>
            {
                cost = price,
                amount = actualPurchase,
                type = resourceType
            }, () =>
            {
                self.Add(resourceType, actualPurchase);
                if (price > self.bank)
                {
                    throw new Exception($"Attempted to purchase more than current funds allow when purchasing {resourceType}");
                }
                self.bank -= price;

                market.Add(resourceType, -actualPurchase);
            });
        }

        public bool CanPurchase(string resourceType, TestInventoryModel self, TestInventoryModel market)
        {
            return market.Get(resourceType) > 0;
        }

        public ActionOption<ExchangeResult<string>> Sell(string resourceType, float amount, TestInventoryModel self, TestInventoryModel market)
        {
            var sellPrice = sellPrices[resourceType];
            var actualSell = Math.Min(amount, self.Get(resourceType));
            var price = actualSell * sellPrice;

            return new ActionOption<ExchangeResult<string>>(new ExchangeResult<string>
            {
                cost = price,
                amount = actualSell,
                type = resourceType
            }, () =>
            {
                self.Add(resourceType, -actualSell);
                self.bank += price;

                market.Add(resourceType, actualSell);
            });
        }
        public bool CanSell(string resourceType, TestInventoryModel self, TestInventoryModel market)
        {
            return self.Get(resourceType) > 0;
        }

        public float GetTotalUtility(string type, TestInventoryModel inventory)
        {
            throw new NotImplementedException();
        }
    }
}
