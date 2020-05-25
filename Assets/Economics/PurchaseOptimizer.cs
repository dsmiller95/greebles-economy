using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{

    public class PurchaseOptimizer<Self, Other>
        where Self : class, IExchangeInventory
        where Other : class, IExchangeInventory
    {
        public class ExchangeAdapter
        {
            public IPurchaser<Self, Other> purchaser;
            public ISeller<Self, Other> seller;
            public IUtilityEvaluator<Self> utilityFunction;
        }

        private IList<ExchangeAdapter> resources;
        private Self selfInventory;
        private Other otherInventory;
        private float increment = 1;

        public PurchaseOptimizer(IEnumerable<ExchangeAdapter> exchangeAdapters, Self selfInventory, Other otherInventory)
        {
            this.resources = exchangeAdapters.ToList();
            this.selfInventory = selfInventory;
            this.otherInventory = otherInventory;
        }

        public void Optimize()
        {
            PurchaseResult.Purchase(this, selfInventory, otherInventory);

            var executesPurchase = true;
            for (var minUtility = GetHighestSellableValuePerUtility(increment, selfInventory, otherInventory);
                minUtility != default && executesPurchase;
                minUtility = GetHighestSellableValuePerUtility(increment, selfInventory, otherInventory))
            {
                var simSelfInventory = CloneSelfInventory();
                var simOtherInventory = CloneOtherInventory();
                // Execute all operations on a simulated inventory to make sure all prices, utilities, and any constraints on size are respected

                minUtility.seller.Sell(increment, true, simSelfInventory, simOtherInventory);
                var purchaseOption = PurchaseResult.Purchase(this, simSelfInventory, simOtherInventory);
                executesPurchase = (purchaseOption.utilityGained
                    + minUtility.utilityFunction.GetIncrementalUtility(
                        selfInventory,
                        -increment
                      ))
                      > 0;
                if (executesPurchase)
                {
                    // Must sell first to get the money; then purchase
                    minUtility.seller.Sell(increment, true, selfInventory, otherInventory);
                    purchaseOption.ReExecutePurchases(selfInventory, otherInventory);
                }
            }
        }

        private Self CloneSelfInventory()
        {
            var clonedInventory = selfInventory.CreateSimulatedClone();
            if (clonedInventory is Self correctTypedInventory)
            {
                return correctTypedInventory;
            }
            throw new ArgumentException("Clone of the base inventory did not return required type");
        }
        private Other CloneOtherInventory()
        {
            var clonedInventory = otherInventory.CreateSimulatedClone();
            if (clonedInventory is Other correctTypedInventory)
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
            private IList<IPurchaser<Self, Other>> purchases;
            private PurchaseOptimizer<Self, Other> optimizer;
            private PurchaseResult(PurchaseOptimizer<Self, Other> optimizer, Self simSelf, Other simOther)
            {
                this.optimizer = optimizer;
                purchases = new List<IPurchaser<Self, Other>>();
                utilityGained = 0f;
                
                //drain the bank
                for (
                    var resource = optimizer.GetHighestPurchaseableUtility(simSelf, simOther);
                    resource != default;
                    resource = optimizer.GetHighestPurchaseableUtility(simSelf, simOther))
                {
                    var purchaseResult = resource.purchaser.Purchase(optimizer.increment, false, simSelf, simOther);
                    utilityGained += resource.utilityFunction.GetIncrementalUtility(
                        simSelf,
                        purchaseResult.amount);

                    resource.purchaser.Purchase(optimizer.increment, true, simSelf, simOther);
                    purchases.Add(resource.purchaser);
                }
            }

            /// <summary>
            /// Buy and sell items until the bank is empty; maximizing utility
            /// </summary>
            /// <param name="bank">the amount of money which can be spent</param>
            /// <returns>the amount of utility gained during purchase</returns>
            public static PurchaseResult Purchase(PurchaseOptimizer<Self, Other> optimizer, Self simSelf, Other simOther)
            {
                return new PurchaseResult(optimizer, simSelf, simOther);
            }

            public void ReExecutePurchases(Self simSelf, Other simOther)
            {
                foreach (var purchase in purchases)
                {
                    purchase.Purchase(optimizer.increment, true, simSelf, simOther);
                }
            }
        }

        /// <summary>
        /// Find the resource with the highest sell price per utility of that resource. Can be used to find
        ///     a resource with either very low incremental utility or very high sell price
        /// </summary>
        /// <param name="increment">The increment to which the value functions are evaluated</param>
        /// <returns></returns>
        private ExchangeAdapter GetHighestSellableValuePerUtility(float increment, Self simSelf, Other simOther)
        {
            return this.resources
                .Where(resource => resource.seller.CanSell(simSelf, simOther))
                .Select(resource => new {
                    resource,
                    valuePerUtility = 
                        resource.seller.Sell(increment, false, simSelf, simOther).cost
                        / -resource.utilityFunction.GetIncrementalUtility(simSelf, - increment)
                })
                .OrderBy(resource => resource.valuePerUtility)
                .LastOrDefault()
                ?.resource;
        }

        private ExchangeAdapter GetHighestPurchaseableUtility(Self simSelf, Other simOther)
        {
            return this.resources
                .Where(resource => resource.purchaser.CanPurchase(simSelf, simOther))
                .Where(resource => resource.purchaser.Purchase(increment, false, simSelf, simOther).cost <= simSelf.GetCurrentFunds())
                .Select(resource => new {
                    resource,
                    utility = resource.utilityFunction.GetIncrementalUtility(simSelf, increment) })
                .OrderBy(resource => resource.utility)
                .LastOrDefault()
                ?.resource;
        }
    }
}
