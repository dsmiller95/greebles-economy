using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{

    public class PurchaseOptimizer<T> where T: class, IExchangeInventory
    {
        public class ExchangeAdapter
        {
            public IPurchaser<T> purchaser;
            public ISeller<T> seller;
            public IUtilityEvaluator<T> utilityFunction;
        }

        private IList<ExchangeAdapter> resources;
        private T baseInventory;
        private float increment = 1;

        public PurchaseOptimizer(IEnumerable<ExchangeAdapter> exchangeAdapters, T inventory)
        {
            this.resources = exchangeAdapters.ToList();
            this.baseInventory = inventory;
        }

        public void Optimize(float bank)
        {
            var initialPurchase = PurchaseResult.Purchase(this, bank, CloneBaseInventory());
            initialPurchase.ExecutePurchases(baseInventory);
            bank -= initialPurchase.totalCost;

            var executesPurchase = true;
            for (var minUtility = GetHighestSellableValuePerUtility(increment, baseInventory);
                minUtility != default && executesPurchase;
                minUtility = GetHighestSellableValuePerUtility(increment, baseInventory))
            {
                var simulatedInventory = CloneBaseInventory();
                // Execute all operations on a simulated inventory to make sure all prices, utilities, and any constraints on size are respected


                var sellPrice = minUtility.seller.Sell(simulatedInventory, increment, true);
                var purchaseOption = PurchaseResult.Purchase(this, sellPrice + bank, simulatedInventory);
                executesPurchase = (purchaseOption.utilityGained
                    + minUtility.utilityFunction.GetIncrementalUtility(
                        baseInventory,
                        -increment
                      ))
                      > 0;
                if (executesPurchase)
                {
                    bank += sellPrice - purchaseOption.totalCost;
                    // Must sell first to get the money; then purchase
                    minUtility.seller.Sell(baseInventory, increment, true);
                    purchaseOption.ExecutePurchases(baseInventory);
                }
            }
        }

        private T CloneBaseInventory()
        {
            var clonedInventory = baseInventory.CreateSimulatedClone();
            if (clonedInventory is T correctTypedInventory)
            {
                return correctTypedInventory;
            }
            throw new ArgumentException("Clone of the base inventory did not return required type");
        }

        class PurchaseResult
        {
            public float utilityGained {
                get;
                private set;
            }
            public float totalCost
            {
                get;
                private set;
            }
            private IList<IPurchaser<T>> purchases;
            private PurchaseOptimizer<T> optimizer;
            private PurchaseResult(PurchaseOptimizer<T> optimizer, float bank, T simulatedInventory)
            {
                this.optimizer = optimizer;
                purchases = new List<IPurchaser<T>>();
                utilityGained = 0f;
                totalCost = 0f;
                
                //drain the bank
                for (
                    var resource = optimizer.GetHighestPurchaseableUtility(bank - totalCost, simulatedInventory);
                    resource != default;
                    resource = optimizer.GetHighestPurchaseableUtility(bank - totalCost, simulatedInventory))
                {
                    var purchaseResult = resource.purchaser.Purchase(simulatedInventory, optimizer.increment, false);
                    utilityGained += resource.utilityFunction.GetIncrementalUtility(
                        simulatedInventory,
                        purchaseResult.amount);
                    totalCost += purchaseResult.cost;

                    resource.purchaser.Purchase(simulatedInventory, optimizer.increment, true);
                    purchases.Add(resource.purchaser);
                }
            }

            /// <summary>
            /// Buy and sell items until the bank is empty; maximizing utility
            /// </summary>
            /// <param name="bank">the amount of money which can be spent</param>
            /// <returns>the amount of utility gained during purchase</returns>
            public static PurchaseResult Purchase(PurchaseOptimizer<T> optimizer, float bank, T simulatedInventory)
            {
                return new PurchaseResult(optimizer, bank, simulatedInventory);
            }

            public void ExecutePurchases(T inventory)
            {
                foreach (var purchase in purchases)
                {
                    purchase.Purchase(inventory, optimizer.increment, true);
                }
            }
        }

        /// <summary>
        /// Find the resource with the highest sell price per utility of that resource. Can be used to find
        ///     a resource with either very low incremental utility or very high sell price
        /// </summary>
        /// <param name="increment">The increment to which the value functions are evaluated</param>
        /// <returns></returns>
        private ExchangeAdapter GetHighestSellableValuePerUtility(float increment, T inventory)
        {
            return this.resources
                .Where(resource => resource.seller.CanSell(inventory))
                .Select(resource => new {
                    resource,
                    valuePerUtility = 
                        resource.seller.Sell(inventory, increment, false)
                        / -resource.utilityFunction.GetIncrementalUtility(inventory, - increment)
                })
                .OrderBy(resource => resource.valuePerUtility)
                .LastOrDefault()
                ?.resource;
        }

        private ExchangeAdapter GetHighestPurchaseableUtility(float maxPurchase, T inventory)
        {
            return this.resources
                .Where(resource => resource.purchaser.CanPurchase(inventory))
                .Where(resource => resource.purchaser.Purchase(inventory, increment, false).cost <= maxPurchase)
                .Select(resource => new {
                    resource,
                    utility = resource.utilityFunction.GetIncrementalUtility(inventory, increment) })
                .OrderBy(resource => resource.utility)
                .LastOrDefault()
                ?.resource;
        }
    }
}
