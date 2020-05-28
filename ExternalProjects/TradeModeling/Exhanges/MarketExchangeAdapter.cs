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
        private IDictionary<T, float> exchangeRates;
        private T moneyType;
        public MarketExchangeAdapter(IDictionary<T, float> exchangeRates, T moneyType)
        {
            this.exchangeRates = exchangeRates;
            this.moneyType = moneyType;
        }

        public ActionOption<ExchangeResult<T>> Purchase(T type, float amount, SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
        {
            var exchangeRate = exchangeRates[type];
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
                    selfInventory.Consume(moneyType, exchangeResult.cost);
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
            var exchangeRate = exchangeRates[type];
            return selfInventory
                .transferResourceInto(type, marketInventory, amount)
                .Then(totalDeposited => new ExchangeResult<T>
                {
                    amount = totalDeposited,
                    cost = totalDeposited * exchangeRate,
                    type = type
                }, exchangeResult =>
                {
                    var value = exchangeResult.cost;
                    selfInventory.Add(moneyType, value).Execute();
                });
        }

        public bool CanSell(T type, SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
        {
            return selfInventory.Get(type) > 0
                && marketInventory.CanFitMoreOf(type);
        }
    }
}