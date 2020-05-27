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

    class TestExchangeModel : IPurchaser<TestInventoryModel, TestInventoryModel>, ISeller<TestInventoryModel, TestInventoryModel>, IUtilityEvaluator<TestInventoryModel>
    {
        public IIncrementalFunction utilityFunction;
        public float purchasePrice;
        public float sellPrice;

        public string resourceType;

        public float GetIncrementalUtility(TestInventoryModel self, float increment)
        {
            return this.utilityFunction.GetIncrementalValue(self.Get(resourceType), increment);
        }

        public ActionOption<ExchangeResult> Purchase(float amount, TestInventoryModel self, TestInventoryModel market)
        {
            var marketInventory = market.Get(resourceType);
            var actualPurchase = Math.Min(amount, marketInventory);
            var price = actualPurchase * purchasePrice;

            return new ActionOption<ExchangeResult>(new ExchangeResult
            {
                cost = price,
                amount = actualPurchase
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

        public bool CanPurchase(TestInventoryModel self, TestInventoryModel market)
        {
            return market.Get(resourceType) > 0;
        }

        public ActionOption<ExchangeResult> Sell(float amount, TestInventoryModel self, TestInventoryModel market)
        {
            var actualSell = Math.Min(amount, self.Get(resourceType));
            var price = actualSell * sellPrice;

            return new ActionOption<ExchangeResult>(new ExchangeResult
            {
                cost = price,
                amount = actualSell
            }, () => {
                self.Add(resourceType, -actualSell);
                self.bank += price;

                market.Add(resourceType, actualSell);
                // TODO: give the market a bank as well
            });
        }
        public bool CanSell(TestInventoryModel self, TestInventoryModel market)
        {
            return self.Get(resourceType) > 0;
        }
    }
}
