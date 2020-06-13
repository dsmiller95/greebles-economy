using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeModeling.Inventories;

namespace TradeModeling.Economics
{
    public class UtilityAnalyzer<Resource> where Resource : IComparable
    {
        public Dictionary<Resource, float> GetUtilityPerInitialResource(
            IList<Resource> tradeableResources,
            SpaceFillingInventory<Resource> endingInventory,
            IList<(ExchangeResult<Resource>?, PurchaseOperationResult<Resource>)> transactionLedger,
            IUtilityEvaluator<Resource, SpaceFillingInventory<Resource>> utilityEvaluator
            )
        {
            var endingUtilities = tradeableResources.ToDictionary(
                resource => resource,
                resource => utilityEvaluator.GetTotalUtility(resource, endingInventory)
              );
            var endingInventoryDictionary = tradeableResources.ToDictionary(
                resource => resource,
                resource => endingInventory.Get(resource)
              );

            var transactionListReverse = transactionLedger
                .Select(transaction =>
                    this.GetTransactionMapFromSingleTransaction(tradeableResources, transaction)
                ).Reverse();

            foreach(var transaction in transactionListReverse)
            {
                this.DistributeUtilityBasedOnTransaction(tradeableResources, endingUtilities, transaction, endingInventoryDictionary);
                endingInventoryDictionary = endingInventoryDictionary - transaction;
            }

            return endingUtilities;
        }

        /// <summary>
        /// Move the utility through the utilities dictionary based on the amount of items transferred thorugh
        ///     The utility per item is evaluated based on the ending inventory: so any amount that is moved as a result
        ///     of the transaction moves an amount of utility based on the ratio between the amount of resource moved vs the total inventory
        /// </summary>
        /// <param name="utilities"></param>
        /// <param name="transaction"></param>
        private void DistributeUtilityBasedOnTransaction(
            IList<Resource> tradeableResources,
            Dictionary<Resource, float> utilities,
            CombinedTransactionModel<Resource> transaction,
            Dictionary<Resource, float> endingInventory)
        {
            foreach (var sold in tradeableResources)
            {
                foreach (var bought in tradeableResources.Where(x => !EqualityComparer<Resource>.Default.Equals(x, sold)))
                {
                    var resourceAmount = endingInventory[bought];
                    if (resourceAmount == 0)
                    {
                        continue;
                    }
                    var singleTransaction = transaction.GetTransactionAmounts(sold, bought);
                    if (singleTransaction.Item1 < 0)
                    {
                        var soldRatio = -singleTransaction.Item1 / resourceAmount;
                        var transferredUtility = utilities[bought] * soldRatio;
                        utilities[bought] -= transferredUtility;
                        utilities[sold] += transferredUtility;
                    }
                }
            }
        }

        public CombinedTransactionModel<Resource> GetTransactionMapFromLedger(
            IList<Resource> tradeableResources,
            IList<(ExchangeResult<Resource>?, PurchaseOperationResult<Resource>)> transactionLedger)
        {

            var allTransactions = new CombinedTransactionModel<Resource>(tradeableResources);

            foreach (var transaction in transactionLedger.Where(x => x.Item1.HasValue))
            {
                var sourceResourcePerResultResource = transaction.Item1.Value.amount / transaction.Item2.exchages.Count;
                foreach (var boughtItem in transaction.Item2.exchages)
                {
                    allTransactions.AddTransaction(
                        transaction.Item1.Value.type,
                        boughtItem.type,
                        -sourceResourcePerResultResource,
                        boughtItem.amount);
                }
            }

            return allTransactions;
        }

        private CombinedTransactionModel<Resource> GetTransactionMapFromSingleTransaction(
            IList<Resource> tradeableResources,
            (ExchangeResult<Resource>?, PurchaseOperationResult<Resource>) transaction)
        {

            var transactionModel = new CombinedTransactionModel<Resource>(tradeableResources);

            if (transaction.Item1.HasValue)
            {
                var sourceResourcePerResultResource = transaction.Item1.Value.amount / transaction.Item2.exchages.Count;
                foreach (var boughtItem in transaction.Item2.exchages)
                {
                    transactionModel.AddTransaction(
                        transaction.Item1.Value.type,
                        boughtItem.type,
                        -sourceResourcePerResultResource,
                        boughtItem.amount);
                }
            }

            return transactionModel;
        }
    }
}
