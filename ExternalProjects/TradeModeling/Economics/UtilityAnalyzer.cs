using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeModeling.Inventories;

namespace TradeModeling.Economics
{
    public class UtilityAnalyzer<Resource>
    {

        public Dictionary<Resource, float> GetUtilityPerInitialResource(
            IList<Resource> tradeableResources,
            SpaceFillingInventory<Resource> endingInventory,
            IList<(ExchangeResult<Resource>?, PurchaseOperationResult<Resource>)> transactionLedger,
            IUtilityEvaluator<Resource, SpaceFillingInventory<Resource>> utilityEvaluator
            )
        {
            var result = tradeableResources.ToDictionary(x => x, x => 0f);
            var endingUtilities = tradeableResources.ToDictionary(
                resource => resource,
                resource => utilityEvaluator.GetTotalUtility(resource, endingInventory)
              );

            var allTransactions = GetTransactionMapFromLedger(tradeableResources, transactionLedger);

            var derivedUtilities = tradeableResources.ToDictionary(x => x,
                resource =>
                {
                    var derivedUtility = 0f;



                    return derivedUtility;
                });
            

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
    }
}
