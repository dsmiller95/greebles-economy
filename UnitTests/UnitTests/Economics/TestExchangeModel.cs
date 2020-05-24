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
        public TestBank selfBank;
        public float marketInventory;
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

        public PurchaseResult Purchase(float amount, bool execute)
        {
            var actualPurchase = Math.Min(amount, marketInventory);
            var price = actualPurchase * purchasePrice;
            if (execute)
            {
                if(price > selfBank.money)
                {
                    throw new Exception("Attempted to purchase more than current funds allow");
                }
                this.selfInventory += actualPurchase;
                this.selfBank.money -= price;

                this.marketInventory -= actualPurchase;
                // TODO: give the market a bank as well
            }

            return new PurchaseResult
            {
                cost = price,
                amount = actualPurchase
            };
        }

        public bool CanPurchase()
        {
            return this.marketInventory > 0;
        }

        public float Sell(float amount, bool execute)
        {
            var actualSell = Math.Min(amount, selfInventory);
            var price = actualSell * sellPrice;
            if (execute)
            {
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
