﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeModeling.Economics
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

        public struct PurchaseOperationResult
        {
            public IList<ExchangeResult> exchages;
            public float utilityGained;
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
                minUtility != default && executesPurchase;
                minUtility = GetHighestSellableValuePerUtility(increment, selfInventory, otherInventory))
            {
                var simSelfInventory = CloneSelfInventory();
                var simOtherInventory = CloneOtherInventory();
                // Execute all operations on a simulated inventory to make sure all prices, utilities, and any constraints on size are respected

                var sellOption = minUtility.seller.Sell(increment, simSelfInventory, simOtherInventory);
                sellOption.Execute();

                var purchaseOption = PurchaseResult.Purchase(this, simSelfInventory, simOtherInventory);
                executesPurchase = (purchaseOption.ledger.utilityGained
                    + minUtility.utilityFunction.GetIncrementalUtility(
                        selfInventory,
                        -increment
                      ))
                      > 0;
                if (executesPurchase)
                {
                    // Must sell first to get the money; then purchase
                    minUtility.seller.Sell(increment, selfInventory, otherInventory).Execute();
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
            private IList<IPurchaser<Self, Other>> purchases;
            public PurchaseOperationResult ledger {
                get;
                private set;
            }
            private PurchaseOptimizer<Self, Other> optimizer;
            private PurchaseResult(PurchaseOptimizer<Self, Other> optimizer, Self simSelf, Other simOther)
            {
                this.optimizer = optimizer;
                purchases = new List<IPurchaser<Self, Other>>();
                var ledger = new PurchaseOperationResult();
                ledger.utilityGained = 0;
                ledger.exchages = new List<ExchangeResult>();
                
                //drain the bank
                for (
                    var resource = optimizer.GetHighestPurchaseableUtilityPerCost(simSelf, simOther);
                    resource != default;
                    resource = optimizer.GetHighestPurchaseableUtilityPerCost(simSelf, simOther))
                {
                    var purchaseResult = resource.purchaser.Purchase(optimizer.increment, simSelf, simOther);
                    ledger.utilityGained += resource.utilityFunction.GetIncrementalUtility(
                        simSelf,
                        purchaseResult.info.amount);
                    purchaseResult.Execute();
                    ledger.exchages.Add(purchaseResult.info);
                    purchases.Add(resource.purchaser);
                }

                this.ledger = ledger;
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
                    purchase.Purchase(optimizer.increment, simSelf, simOther).Execute();
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
                        resource.seller.Sell(increment, simSelf, simOther).info.cost
                        / -resource.utilityFunction.GetIncrementalUtility(simSelf, - increment)
                })
                .OrderBy(resource => resource.valuePerUtility)
                .LastOrDefault()
                ?.resource;
        }

        private ExchangeAdapter GetHighestPurchaseableUtilityPerCost(Self simSelf, Other simOther)
        {
            return this.resources
                .Where(resource => resource.purchaser.CanPurchase(simSelf, simOther))
                .Select(resource => new {
                    resource,
                    utility = resource.utilityFunction.GetIncrementalUtility(simSelf, increment),
                    purchase = resource.purchaser.Purchase(increment, simSelf, simOther).info
                })
                // The purchase could have been restricted in size based on available funds
                .Where(resource => resource.purchase.amount == increment)
                .Where(resource => resource.purchase.cost <= simSelf.GetCurrentFunds())
                .OrderBy(resource => resource.utility / resource.purchase.cost)
                .LastOrDefault()
                ?.resource;
        }
    }
}