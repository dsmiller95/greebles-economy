using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeModeling.Economics;
using TradeModeling.Functions;
using TradeModeling.Inventories;

namespace TradeModeling.Exchanges
{
    public class SigmoidMarketExchangeAdapter<T> :
    ISeller<T, SpaceFillingInventory<T>, SpaceFillingInventory<T>>,
    IPurchaser<T, SpaceFillingInventory<T>, SpaceFillingInventory<T>>
    {
        private IDictionary<T, SigmoidFunction> marketSellRates;
        private IDictionary<T, SigmoidFunction> marketBuyRates;
        private T moneyType;

        public SigmoidMarketExchangeAdapter(IDictionary<T, SigmoidFunctionConfig> sellPrices, IDictionary<T, SigmoidFunctionConfig> buyPrices, T moneyType)
        {
            this.marketBuyRates = buyPrices.SelectDictionary(config => new SigmoidFunction(config));
            this.marketSellRates = sellPrices.SelectDictionary(config => new SigmoidFunction(config));
            this.moneyType = moneyType;
        }
        public SigmoidMarketExchangeAdapter(IDictionary<T, SigmoidFunctionConfig> exchangeRates, T moneyType): this(exchangeRates, exchangeRates, moneyType)
        {
        }

        public ActionOption<ExchangeResult<T>> Purchase(T type, float amount, SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
        {
            //throw new NotImplementedException();
            // using the buying rate because this is a purchase from the "other". I.E. a sell from the perspective of the market
            var exchangeRateFunction = marketSellRates[type];
            var maxPurchase = exchangeRateFunction.GetPointFromNetExtraValueFromPoint(selfInventory.GetCurrentFunds(), marketInventory.Get(type));
            var amountToPurchase = Math.Min(amount, maxPurchase);
            return marketInventory
                .transferResourceInto(type, selfInventory, amountToPurchase)
                .Then(withdrawn => new ExchangeResult<T>
                {
                    amount = withdrawn,
                    cost = exchangeRateFunction.GetIncrementalValue(marketInventory.Get(type), withdrawn),
                    type = type
                }, exchangeResult =>
                {
                    selfInventory.transferResourceInto(moneyType, marketInventory, exchangeResult.cost).Execute();
                });
        }

        public bool CanPurchase(T type, SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
        {
            return marketInventory.Get(type) > 0
                && selfInventory.Get(moneyType) > 0
                && selfInventory.CanFitMoreOf(type);
        }

        public ActionOption<ExchangeResult<T>> Sell(T type, float amount, SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
        {
            //throw new NotImplementedException();
            // using the buying rate because this is a sell from the "other". I.E. a purchase from the market
            var exchangeRateFunction = marketBuyRates[type];
            var remainingAfterSell = exchangeRateFunction.GetPointFromNetExtraValueFromPoint(-marketInventory.GetCurrentFunds(), marketInventory.Get(type));// marketInventory.GetCurrentFunds() / exchangeRate;
            var maxSell = marketInventory.Get(type) - remainingAfterSell;
           
            var amountToSell = Math.Min(amount, maxSell);
            return selfInventory
                .transferResourceInto(type, marketInventory, amountToSell)
                .Then(totalDeposited => new ExchangeResult<T>
                {
                    amount = totalDeposited,
                    cost = -exchangeRateFunction.GetIncrementalValue(marketInventory.Get(type), -totalDeposited),
                    type = type
                }, exchangeResult =>
                {
                    marketInventory.transferResourceInto(moneyType, selfInventory, exchangeResult.cost).Execute();
                });
        }

        public bool CanSell(T type, SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
        {
            return selfInventory.Get(type) > 0
                && marketInventory.Get(moneyType) > 0
                && marketInventory.CanFitMoreOf(type);
        }
    }
}