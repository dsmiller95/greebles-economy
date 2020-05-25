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

        public void Optimize()
        {
            PurchaseResult.Purchase(this, baseInventory);

            var executesPurchase = true;
            for (var minUtility = GetHighestSellableValuePerUtility(increment, baseInventory);
                minUtility != default && executesPurchase;
                minUtility = GetHighestSellableValuePerUtility(increment, baseInventory))
            {
                var simulatedInventory = CloneBaseInventory();
                // Execute all operations on a simulated inventory to make sure all prices, utilities, and any constraints on size are respected

                minUtility.seller.Sell(simulatedInventory, increment, true);
                var purchaseOption = PurchaseResult.Purchase(this, simulatedInventory);
                executesPurchase = (purchaseOption.utilityGained
                    + minUtility.utilityFunction.GetIncrementalUtility(
                        baseInventory,
                        -increment
                      ))
                      > 0;
                if (executesPurchase)
                {
                    // Must sell first to get the money; then purchase
                    minUtility.seller.Sell(baseInventory, increment, true);
                    purchaseOption.ReExecutePurchases(baseInventory);
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
            private IList<IPurchaser<T>> purchases;
            private PurchaseOptimizer<T> optimizer;
            private PurchaseResult(PurchaseOptimizer<T> optimizer, T simulatedInventory)
            {
                this.optimizer = optimizer;
                purchases = new List<IPurchaser<T>>();
                utilityGained = 0f;
                
                //drain the bank
                for (
                    var resource = optimizer.GetHighestPurchaseableUtility(simulatedInventory);
                    resource != default;
                    resource = optimizer.GetHighestPurchaseableUtility(simulatedInventory))
                {
                    var purchaseResult = resource.purchaser.Purchase(simulatedInventory, optimizer.increment, false);
                    utilityGained += resource.utilityFunction.GetIncrementalUtility(
                        simulatedInventory,
                        purchaseResult.amount);

                    resource.purchaser.Purchase(simulatedInventory, optimizer.increment, true);
                    purchases.Add(resource.purchaser);
                }
            }

            /// <summary>
            /// Buy and sell items until the bank is empty; maximizing utility
            /// </summary>
            /// <param name="bank">the amount of money which can be spent</param>
            /// <returns>the amount of utility gained during purchase</returns>
            public static PurchaseResult Purchase(PurchaseOptimizer<T> optimizer, T simulatedInventory)
            {
                return new PurchaseResult(optimizer, simulatedInventory);
            }

            public void ReExecutePurchases(T inventory)
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

        private ExchangeAdapter GetHighestPurchaseableUtility(T inventory)
        {
            return this.resources
                .Where(resource => resource.purchaser.CanPurchase(inventory))
                .Where(resource => resource.purchaser.Purchase(inventory, increment, false).cost <= inventory.GetCurrentSelfMoney())
                .Select(resource => new {
                    resource,
                    utility = resource.utilityFunction.GetIncrementalUtility(inventory, increment) })
                .OrderBy(resource => resource.utility)
                .LastOrDefault()
                ?.resource;
        }
    }
}
