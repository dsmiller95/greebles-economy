using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeModeling.Economics;
using TradeModeling.Inventories;

namespace TradeModeling.Exchanges
{
    public class AdaptableSingleExchangeModel<T> : SingleExchangeModel<T>
    {
        public AdaptableSingleExchangeModel(IDictionary<T, float> sellPrices, IDictionary<T, float> buyPrices, T moneyType) : base(sellPrices, buyPrices, moneyType)
        {
        }
        protected AdaptableSingleExchangeModel(AdaptableSingleExchangeModel<T> other) : base(other)
        {
        }

        public AdaptableSingleExchangeModel<T> GetVariant()
        {
            return new AdaptableSingleExchangeModel<T>(this);
        }
    }
}