using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeModeling.Economics;

namespace UnitTests.Economics
{
    public class TestBank
    {
        public float money;
    }

    class TestExchangeModel :
        IPurchaser<string, TestInventoryModel, TestInventoryModel>,
        ISeller<string, TestInventoryModel, TestInventoryModel>,
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
            }, () => {
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
            }, () => {
                self.Add(resourceType, -actualSell);
                self.bank += price;

                market.Add(resourceType, actualSell);
                // TODO: give the market a bank as well
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
