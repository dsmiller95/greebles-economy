using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeModeling.Economics;
using TradeModeling.Inventories;

namespace TradeModeling.Exchanges
{
    public class MarketExchangeAdapter<T> :
    ISeller<T, SpaceFillingInventory<T>, SpaceFillingInventory<T>>,
    IPurchaser<T, SpaceFillingInventory<T>, SpaceFillingInventory<T>>
    {
        private IDictionary<T, float> marketSellRates;
        private IDictionary<T, float> marketBuyRates;

        private T moneyType;

        public MarketExchangeAdapter(IDictionary<T, float> sellPrices, IDictionary<T, float> buyPrices, T moneyType)
        {
            this.marketBuyRates = buyPrices;
            this.marketSellRates = sellPrices;
            this.moneyType = moneyType;
        }
        public MarketExchangeAdapter(IDictionary<T, float> exchangeRates, T moneyType): this(exchangeRates, exchangeRates, moneyType)
        {
        }

        public ActionOption<ExchangeResult<T>> Purchase(T type, float amount, SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
        {
            // using the buying rate because this is a purchase from the "other". I.E. a sell from the perspective of the market
            var exchangeRate = marketSellRates[type];
            var maxPurchase = selfInventory.GetCurrentFunds() / exchangeRate;
            var amountToPurchase = Math.Min(amount, maxPurchase);
            return marketInventory
                .transferResourceInto(type, selfInventory, amountToPurchase)
                .Then(withdrawn => new ExchangeResult<T>
                {
                    amount = withdrawn,
                    cost = withdrawn * exchangeRate,
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
            // using the buying rate because this is a sell from the "other". I.E. a purchase from the market
            var exchangeRate = marketBuyRates[type];
            var maxSell = marketInventory.GetCurrentFunds() / exchangeRate;
            var amountToSell = Math.Min(amount, maxSell);
            return selfInventory
                .transferResourceInto(type, marketInventory, amountToSell)
                .Then(totalDeposited => new ExchangeResult<T>
                {
                    amount = totalDeposited,
                    cost = totalDeposited * exchangeRate,
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