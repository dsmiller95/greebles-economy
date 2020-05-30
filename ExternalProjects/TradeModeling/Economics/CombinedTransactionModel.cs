using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradeModeling.Economics
{
    public class CombinedTransactionModel<Resource>
    {
        private Dictionary<(Resource, Resource), (float, float)> transactionMaps;
        private IList<Resource> mappedResources;
        private Dictionary<Resource, int> mappedResourceIndexes;
        public CombinedTransactionModel(IList<Resource> mappedResources)
        {
            this.mappedResources = mappedResources;
            this.mappedResourceIndexes = mappedResources
                .Select((x, index) => new { x, index })
                .ToDictionary(x => x.x, x => x.index);
            this.transactionMaps = mappedResources
                .SelectMany((resource, index) =>
                    mappedResources
                        .Skip(index + 1)
                        .Select(innerResource => (resource, innerResource))
                    )
                .ToDictionary(x => x, x => (0f, 0f));
        }

        public (float, float) GetTransactionAmounts(Resource sold, Resource bought)
        {
            if (mappedResourceIndexes[sold] < mappedResourceIndexes[bought])
            {
                return transactionMaps[(sold, bought)];
            }
            var transaction = transactionMaps[(bought, sold)];
            return (transaction.Item2, transaction.Item1);
        }

        public void SetTransactionValue(Resource sold, Resource bought, float soldAmount, float boughtAmount)
        {
            if (mappedResourceIndexes[sold] < mappedResourceIndexes[bought])
            {
                transactionMaps[(sold, bought)] = (soldAmount, boughtAmount);
                return;
            }
            transactionMaps[(bought, sold)] = (boughtAmount, soldAmount);
        }
        public void AddTransaction(Resource sold, Resource bought, float soldAmount, float boughtAmount)
        {
            if (mappedResourceIndexes[sold] < mappedResourceIndexes[bought])
            {
                var transaction = transactionMaps[(sold, bought)];
                transactionMaps[(sold, bought)] = (transaction.Item1 + soldAmount, transaction.Item2 + boughtAmount);
                return;
            }
            var reverseTransaction = transactionMaps[(bought, sold)];
            transactionMaps[(bought, sold)] = (reverseTransaction.Item1 + boughtAmount, reverseTransaction.Item2 + soldAmount);
        }
    }
}
