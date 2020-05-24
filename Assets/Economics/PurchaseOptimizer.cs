using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{

    public class PurchaseOptimizer
    {
        public class CostUtilityAdapter
        {
            public IPurchaser purchaser;
            public ISeller seller;
            public IUtilityEvaluator utilityFunction;
            public float simulatedSelfInventory;
            public float simulatedMarketInventory;
        }

        private IList<CostUtilityAdapter> resources;
        private float increment = 1;

        public PurchaseOptimizer(IEnumerable<CostUtilityAdapter> resources)
        {
            this.resources = resources.ToList();
        }

        public void Optimize(float bank)
        {
            var initialPurchase = PurchaseResult.Purchase(this, bank);
            initialPurchase.ExecutePurchases();
            bank -= initialPurchase.totalCost;

            var executesPurchase = true;
            for (var minUtility = GetHighestSellableValuePerUtility(increment); minUtility != default && executesPurchase; minUtility = GetHighestSellableValuePerUtility(increment))
            {
                var sellPrice = minUtility.seller.Sell(increment, false);
                var purchaseOption = PurchaseResult.Purchase(this, sellPrice + bank);
                executesPurchase = (purchaseOption.utilityGained
                    + minUtility.utilityFunction.GetIncrementalUtility(
                        -increment,
                        minUtility.utilityFunction.GetCurrentAmount()
                      ))
                      > 0;
                if (executesPurchase)
                {
                    bank += sellPrice - purchaseOption.totalCost;
                    // Must sell first to get the money; then purchase
                    minUtility.seller.Sell(increment, true);
                    purchaseOption.ExecutePurchases();
                }
            }
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
            private IList<IPurchaser> purchases;
            private PurchaseOptimizer optimizer;
            private PurchaseResult(PurchaseOptimizer optimizer, float bank)
            {
                this.optimizer = optimizer;
                purchases = new List<IPurchaser>();
                utilityGained = 0f;
                totalCost = 0f;
                foreach (var resource in optimizer.resources)
                {
                    resource.simulatedSelfInventory = resource.utilityFunction.GetCurrentAmount();
                    resource.simulatedMarketInventory = resource.purchaser.GetCurrentMarketInventory();
                }
                //drain the bank
                for (var resource = optimizer.GetHighestPurchaseableUtility(bank - totalCost); resource != default; resource = optimizer.GetHighestPurchaseableUtility(bank - totalCost))
                {
                    var purchaseResult = resource.purchaser.Purchase(optimizer.increment, false, resource.simulatedMarketInventory);
                    totalCost += purchaseResult.cost;
                    utilityGained += resource.utilityFunction.GetIncrementalUtility(
                        purchaseResult.amount,
                        resource.simulatedSelfInventory);
                    resource.simulatedSelfInventory += purchaseResult.amount;
                    resource.simulatedMarketInventory -= purchaseResult.amount;
                    purchases.Add(resource.purchaser);
                }
            }

            /// <summary>
            /// Buy and sell items until the bank is empty; maximizing utility
            /// </summary>
            /// <param name="bank">the amount of money which can be spent</param>
            /// <returns>the amount of utility gained during purchase</returns>
            public static PurchaseResult Purchase(PurchaseOptimizer optimizer, float bank)
            {
                return new PurchaseResult(optimizer, bank);
            }

            public void ExecutePurchases()
            {
                foreach (var purchase in purchases)
                {
                    purchase.Purchase(optimizer.increment, true);
                }
            }
        }

        /// <summary>
        /// Find the resource with the highest sell price per utility of that resource. Can be used to find
        ///     a resource with either very low incremental utility or very high sell price
        /// </summary>
        /// <param name="increment">The increment to which the value functions are evaluated</param>
        /// <returns></returns>
        private CostUtilityAdapter GetHighestSellableValuePerUtility(float increment)
        {
            return this.resources
                .Where(resource => resource.seller.CanSell())
                .Select(resource => new {
                    resource,
                    valuePerUtility = 
                        resource.seller.Sell(increment, false)
                        / -resource.utilityFunction.GetIncrementalUtility(-increment, resource.utilityFunction.GetCurrentAmount())
                })
                .OrderBy(resource => resource.valuePerUtility)
                .LastOrDefault()
                ?.resource;
        }

        private CostUtilityAdapter GetHighestPurchaseableUtility(float maxPurchase)
        {
            return this.resources
                .Where(resource => resource.purchaser.CanPurchase(resource.simulatedMarketInventory))
                .Where(resource => resource.purchaser.Purchase(increment, false, resource.simulatedMarketInventory).cost <= maxPurchase)
                .Select(resource => new {
                    resource,
                    utility = resource.utilityFunction.GetIncrementalUtility(increment, resource.simulatedSelfInventory) })
                .OrderBy(resource => resource.utility)
                .LastOrDefault()
                ?.resource;
        }
    }
}
