using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{
    struct CostUtilityFunction
    {
        public IPurchaser purchaser;
        public ISeller seller;
        public IUtilityEvaluator utilityFunction;
    }

    class PurchaseOptimizer
    {
        private CostUtilityFunction[] resources;
        private float increment = 1;

        public PurchaseOptimizer(CostUtilityFunction[] resources)
        {
            this.resources = resources;
        }

        public void Optimize(float startingBank)
        {
            PurchaseResult.Purchase(this, startingBank).ExecutePurchases();

            var executesPurchase = true;
            for (var minUtility = GetHighestValuePerUtility(increment); minUtility.HasValue && executesPurchase; minUtility = GetHighestValuePerUtility(increment))
            {
                var sellPrice = minUtility.Value.seller.Sell(increment, false);
                var purchaseOption = PurchaseResult.Purchase(this, sellPrice);
                executesPurchase = purchaseOption.utilityGained + minUtility.Value.utilityFunction.GetIncrementalUtility(-increment) > 0;
                if (executesPurchase)
                    purchaseOption.ExecutePurchases();
            }
        }

        class PurchaseResult
        {
            public float utilityGained {
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
                //drain the bank
                for (var resource = optimizer.GetHighestUtility(bank); resource.HasValue; resource = optimizer.GetHighestUtility(bank))
                {
                    utilityGained += resource.Value.utilityFunction.GetIncrementalUtility(optimizer.increment);
                    purchases.Add(resource.Value.purchaser);
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
        private CostUtilityFunction? GetHighestValuePerUtility(float increment)
        {
            return this.resources
                .Select(resource => new {
                    resource,
                    valuePerUtility = resource.utilityFunction.GetIncrementalUtility(-increment) * resource.seller.Sell(increment, false)
                })
                .OrderBy(resource => resource.valuePerUtility)
                .LastOrDefault()
                ?.resource;
        }

        private CostUtilityFunction? GetHighestUtility(float maxPurchase)
        {
            return this.resources
                .Where(resource => resource.purchaser.Purchase(increment, false) <= maxPurchase)
                .Select(resource => new { resource, utility = resource.utilityFunction.GetIncrementalUtility(increment) })
                .OrderBy(resource => resource.utility)
                .LastOrDefault()
                ?.resource;
        }
    }
}
