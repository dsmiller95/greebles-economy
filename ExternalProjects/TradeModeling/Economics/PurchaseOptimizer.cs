using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeModeling.Economics
{

    public class PurchaseOptimizer<Resource, Self, Other>
        where Self : class, IExchangeInventory
        where Other : class, IExchangeInventory
    {
        public struct PurchaseOperationResult
        {
            public IList<ExchangeResult> exchages;
            public float utilityGained;
        }

        private Self selfInventory;
        private Other otherInventory;
        private IList<Resource> tradeableResources;
        private IPurchaser<Resource, Self, Other> purchaser;
        private ISeller<Resource, Self, Other> seller;
        private IUtilityEvaluator<Resource, Self> utilityFunction;

        private float increment = 1;

        public PurchaseOptimizer(
            Self selfInventory,
            Other otherInventory,
            IEnumerable<Resource> tradeableResources,
            IPurchaser<Resource, Self, Other> purchaser,
            ISeller<Resource, Self, Other> seller,
            IUtilityEvaluator<Resource, Self> utilityFunction)
        {
            this.selfInventory = selfInventory;
            this.otherInventory = otherInventory;
            this.tradeableResources = tradeableResources.ToList();
            this.purchaser = purchaser;
            this.seller = seller;
            this.utilityFunction = utilityFunction;
        }

        /// <summary>
        /// Optimize transactions on the provided exchange
        /// </summary>
        /// <returns></returns>
        public IList<(ExchangeResult?, PurchaseOperationResult)> Optimize()
        {
            var transactionLedger = new List<(ExchangeResult?, PurchaseOperationResult)>();
            var purchase = PurchaseResult.Purchase(this, selfInventory, otherInventory);
            transactionLedger.Add((null, purchase.ledger));

            var executesPurchase = true;
            for (var minUtility = GetHighestSellableValuePerUtility(increment, selfInventory, otherInventory);
                !EqualityComparer<Resource>.Default.Equals(minUtility, default) && executesPurchase;
                minUtility = GetHighestSellableValuePerUtility(increment, selfInventory, otherInventory))
            {
                var simSelfInventory = CloneSelfInventory();
                var simOtherInventory = CloneOtherInventory();
                // Execute all operations on a simulated inventory to make sure all prices, utilities, and any constraints on size are respected

                var sellOption = seller.Sell(minUtility, increment, simSelfInventory, simOtherInventory);
                sellOption.Execute();

                var purchaseOption = PurchaseResult.Purchase(this, simSelfInventory, simOtherInventory);
                executesPurchase = (purchaseOption.ledger.utilityGained
                    + utilityFunction.GetIncrementalUtility(
                        minUtility,
                        selfInventory,
                        -increment
                      ))
                      > 0;
                if (executesPurchase)
                {
                    // Must sell first to get the money; then purchase
                    seller.Sell(minUtility, increment, selfInventory, otherInventory).Execute();
                    purchaseOption.ReExecutePurchases(selfInventory, otherInventory);
                    transactionLedger.Add((sellOption.info, purchaseOption.ledger));
                }
            }

            return transactionLedger;
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
            private IList<Resource> purchases;
            public PurchaseOperationResult ledger {
                get;
                private set;
            }
            private PurchaseOptimizer<Resource, Self, Other> optimizer;
            private PurchaseResult(PurchaseOptimizer<Resource, Self, Other> optimizer, Self simSelf, Other simOther)
            {
                this.optimizer = optimizer;
                purchases = new List<Resource>();
                var ledger = new PurchaseOperationResult();
                ledger.utilityGained = 0;
                ledger.exchages = new List<ExchangeResult>();
                
                //drain the bank
                for (
                    var resource = optimizer.GetHighestPurchaseableUtilityPerCost(simSelf, simOther);
                    !EqualityComparer<Resource>.Default.Equals(resource, default);
                    resource = optimizer.GetHighestPurchaseableUtilityPerCost(simSelf, simOther))
                {
                    var purchaseResult = optimizer.purchaser.Purchase(resource, optimizer.increment, simSelf, simOther);
                    ledger.utilityGained += optimizer.utilityFunction.GetIncrementalUtility(
                        resource,
                        simSelf,
                        purchaseResult.info.amount);
                    purchaseResult.Execute();
                    ledger.exchages.Add(purchaseResult.info);
                    purchases.Add(resource);
                }

                this.ledger = ledger;
            }

            /// <summary>
            /// Buy and sell items until the bank is empty; maximizing utility
            /// </summary>
            /// <param name="bank">the amount of money which can be spent</param>
            /// <returns>the amount of utility gained during purchase</returns>
            public static PurchaseResult Purchase(PurchaseOptimizer<Resource, Self, Other> optimizer, Self simSelf, Other simOther)
            {
                return new PurchaseResult(optimizer, simSelf, simOther);
            }

            public void ReExecutePurchases(Self simSelf, Other simOther)
            {
                foreach (var purchase in purchases)
                {
                    optimizer.purchaser.Purchase(purchase, optimizer.increment, simSelf, simOther).Execute();
                }
            }
        }

        /// <summary>
        /// Find the resource with the highest sell price per utility of that resource. Can be used to find
        ///     a resource with either very low incremental utility or very high sell price
        /// </summary>
        /// <param name="increment">The increment to which the value functions are evaluated</param>
        /// <returns></returns>
        private Resource GetHighestSellableValuePerUtility(float increment, Self simSelf, Other simOther)
        {
            var resourceObject = tradeableResources
                .Where(resource => seller.CanSell(resource, simSelf, simOther))
                .Select(resource => new {
                    resource,
                    valuePerUtility = 
                        seller.Sell(resource, increment, simSelf, simOther).info.cost
                        / -utilityFunction.GetIncrementalUtility(resource, simSelf, - increment)
                })
                .OrderBy(resource => resource.valuePerUtility)
                .LastOrDefault();

            if (resourceObject == default)
            {
                return default(Resource);
            }
            return resourceObject.resource;
        }

        private Resource GetHighestPurchaseableUtilityPerCost(Self simSelf, Other simOther)
        {
            var resourceObject = tradeableResources
                .Where(resource => purchaser.CanPurchase(resource, simSelf, simOther))
                .Select(resource => new {
                    resource,
                    utility = utilityFunction.GetIncrementalUtility(resource, simSelf, increment),
                    purchase = purchaser.Purchase(resource, increment, simSelf, simOther).info
                })
                // The purchase could have been restricted in size based on available funds
                .Where(resource => resource.purchase.amount == increment)
                .Where(resource => resource.purchase.cost <= simSelf.GetCurrentFunds())
                .OrderBy(resource => resource.utility / resource.purchase.cost)
                .LastOrDefault();

            if(resourceObject == default)
            {
                return default(Resource);
            }
            return resourceObject.resource;
        }
    }
}
