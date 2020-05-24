using Assets.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Economics
{
    public class TestBank
    {
        public float money;
    }

    class TestExchangeModel : IPurchaser, ISeller, IUtilityEvaluator
    {

        public float selfInventory;
        public float selfInventoryCapacity = 100;
        public TestBank selfBank;
        public float marketInventory;
        public float marketInventoryCapacity = 100;
        public IIncrementalFunction utilityFunction;
        public float purchasePrice;
        public float sellPrice;
        public string name;


        public float GetIncrementalUtility(float increment, float amount)
        {
            return this.utilityFunction.GetIncrementalValue(amount, increment);
        }

        public float GetCurrentAmount()
        {
            return this.selfInventory;
        }

        public PurchaseResult Purchase(float amount, bool execute, float simulatedMarketInventory = 0)
        {
            simulatedMarketInventory = execute ? marketInventory : simulatedMarketInventory;
            var actualPurchase = Math.Min(amount, simulatedMarketInventory);
            var price = actualPurchase * purchasePrice;
            if (execute)
            {
                this.selfInventory += actualPurchase;
                if (price > selfBank.money)
                {
                    throw new Exception("Attempted to purchase more than current funds allow");
                }
                this.selfBank.money -= price;

                if (actualPurchase > this.marketInventory)
                {
                    throw new Exception("Attempted to purchase more than is in the market");
                }
                this.marketInventory -= actualPurchase;
                // TODO: give the market a bank as well
            }

            return new PurchaseResult
            {
                cost = price,
                amount = actualPurchase
            };
        }

        public bool CanPurchase(float simulatedMarketInventory = -1)
        {
            simulatedMarketInventory = simulatedMarketInventory == -1 ? this.marketInventory : simulatedMarketInventory;
            return simulatedMarketInventory > 0;
        }

        public float GetCurrentMarketInventory()
        {
            return this.marketInventory;
        }

        public float Sell(float amount, bool execute)
        {
            var actualSell = Math.Min(amount, selfInventory);
            var price = actualSell * sellPrice;
            if (execute)
            {
                if (actualSell > this.selfInventory)
                {
                    throw new Exception("Attempted to sell more than is in inventory");
                }
                this.selfInventory -= actualSell;
                this.selfBank.money += price;

                this.marketInventory += actualSell;
                // TODO: give the market a bank as well
            }

            return price;
        }
        public bool CanSell()
        {
            return this.selfInventory > 0;
        }
    }
}
