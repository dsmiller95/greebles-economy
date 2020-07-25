using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TradeModeling.Exchanges
{
    public class AdaptableExchangePool<T> : IEnumerable<SingleExchangeModel<T>>
    {
        private IList<AdaptableSingleExchangeModel<T>> exchangePool;

        public AdaptableExchangePool(int poolSize, AdaptableSingleExchangeModel<T> poolSeed)
        {
            exchangePool = new List<AdaptableSingleExchangeModel<T>>();
            for (; poolSize > 0; poolSize--)
            {
                exchangePool.Add(poolSeed.GetVariant());
            }
        }
        public AdaptableExchangePool(IEnumerable<AdaptableSingleExchangeModel<T>> pool)
        {
            exchangePool = pool.ToList();
        }

        public IEnumerator<SingleExchangeModel<T>> GetEnumerator()
        {
            return exchangePool.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return exchangePool.GetEnumerator();
        }

        public SingleExchangeModel<T> this[int index]
        {
            get => exchangePool[index];
        }
        public int Count => exchangePool.Count;
    }
}
