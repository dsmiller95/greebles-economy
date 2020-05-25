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

    class TestExchangeModel : IPurchaser<TestInventoryModel>, ISeller<TestInventoryModel>, IUtilityEvaluator<TestInventoryModel>
    {
        public float selfInventoryCapacity = 100;
        public float marketInventoryCapacity = 100;
        public IIncrementalFunction utilityFunction;
        public float purchasePrice;
        public float sellPrice;

        public string resourceType;


        public float GetIncrementalUtility(TestInventoryModel inventory, float increment)
        {
            return this.utilityFunction.GetIncrementalValue(inventory.GetSelf(resourceType), increment);
        }

        public float GetCurrentAmount(TestInventoryModel inventory)
        {
            return inventory.GetSelf(this.resourceType);
        }

        public PurchaseResult Purchase(TestInventoryModel inventory, float amount, bool execute)
        {
            var marketInventory = inventory.GetMarket(resourceType);
            var actualPurchase = Math.Min(amount, marketInventory);
            var price = actualPurchase * purchasePrice;
            if (execute)
            {
                inventory.AddSelf(resourceType, actualPurchase);
                if (price > inventory.selfBank)
                {
                    throw new Exception($"Attempted to purchase more than current funds allow when purchasing {resourceType}");
                }
                inventory.selfBank -= price;

                inventory.AddMarket(resourceType, -actualPurchase);
                // TODO: give the market a bank as well
            }

            return new PurchaseResult
            {
                cost = price,
                amount = actualPurchase
            };
        }

        public bool CanPurchase(TestInventoryModel inventory)
        {
            return inventory.GetMarket(resourceType) > 0;
        }

        public float GetCurrentMarketInventory(TestInventoryModel inventory)
        {
            return inventory.GetMarket(resourceType);
        }

        public float Sell(TestInventoryModel inventory, float amount, bool execute)
        {
            var actualSell = Math.Min(amount, inventory.GetSelf(resourceType));
            var price = actualSell * sellPrice;
            if (execute)
            {
                inventory.AddSelf(resourceType, -actualSell);
                inventory.selfBank += price;

                inventory.AddMarket(resourceType, actualSell);
                // TODO: give the market a bank as well
            }

            return price;
        }
        public bool CanSell(TestInventoryModel inventory)
        {
            return inventory.GetSelf(resourceType) > 0;
        }
    }
}
