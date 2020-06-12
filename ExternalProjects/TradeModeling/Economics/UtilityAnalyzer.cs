using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeModeling.Inventories;

namespace TradeModeling.Economics
{
    public class UtilityAnalyzer<Resource> where Resource : IComparable
    {

        // TODO: support ledgers that contain intermediate transactions. Currently this will only work when all bought resources
        //  are never sold after they have been bought
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
            var result = tradeableResources.ToDictionary(x => x, x => 0f);
            var allTransactions = GetTransactionMapFromLedger(tradeableResources, transactionLedger);

            foreach (var sold in tradeableResources)
            {
                foreach (var bought in tradeableResources.Where(x => !EqualityComparer<Resource>.Default.Equals(x, sold)))
                {
                    if (endingInventory.Get(bought) == 0)
                    {
                        continue;
                    }
                    var transaction = allTransactions.GetTransactionAmounts(sold, bought);
                    if(transaction.Item1 < 0)
                    {
                        var soldRatio = -transaction.Item1 / endingInventory.Get(bought);
                        var transferredUtility = endingUtilities[bought] * soldRatio;
                        endingUtilities[bought] -= transferredUtility;
                        endingUtilities[sold] += transferredUtility;
                    }
                }
            }
            

            return endingUtilities;
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


        //private CombinedTransactionModel<Resource> GetTransactionMapFromSingleTransaction(
        //    IList<Resource> tradeableResources,
        //    (ExchangeResult<Resource>, PurchaseOperationResult<Resource>) transaction)
        //{

        //    var transactionModel = new CombinedTransactionModel<Resource>(tradeableResources);

        //    var sourceResourcePerResultResource = transaction.Item1.amount / transaction.Item2.exchages.Count;
        //    foreach (var boughtItem in transaction.Item2.exchages)
        //    {
        //        transactionModel.AddTransaction(
        //            transaction.Item1.type,
        //            boughtItem.type,
        //            -sourceResourcePerResultResource,
        //            boughtItem.amount);
        //    }

        //    return transactionModel;
        //}
    }
}
