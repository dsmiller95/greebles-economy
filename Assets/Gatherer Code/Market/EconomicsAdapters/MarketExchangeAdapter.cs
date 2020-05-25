using Assets.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MarketExchangeAdapter<T>:
    ISeller<SpaceFillingInventory<T>, SpaceFillingInventory<T>>,
    IPurchaser<SpaceFillingInventory<T>, SpaceFillingInventory<T>>
{
    public T type {
        get;
        private set;
    }
    private float exchangeRate;
    private T moneyType;
    public MarketExchangeAdapter(T type, float exchangeRate, T moneyType)
    {
        this.type = type;
        this.exchangeRate = exchangeRate;
        this.moneyType = moneyType;
    }

    public ActionOption<ExchangeResult> Purchase(float amount, SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
    {
        var maxPurchase = selfInventory.GetCurrentFunds() / exchangeRate;
        var amountToPurchase = Math.Min(amount, maxPurchase);
        return marketInventory
            .transferResourceInto(type, selfInventory, amountToPurchase)
            .Then(withdrawn => new ExchangeResult
            {
                amount = withdrawn,
                cost = withdrawn * exchangeRate
            }, exchangeResult =>
            {
                selfInventory.Consume(moneyType, exchangeResult.cost);
            });
    }

    public bool CanPurchase(SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
    {
        return marketInventory.Get(type) > 0
            && selfInventory.Get(moneyType) > 0
            && selfInventory.CanFitMoreOf(type);
    }

    public ActionOption<ExchangeResult> Sell(float amount, SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
    {
        return selfInventory
            .transferResourceInto(type, marketInventory, amount)
            .Then(totalDeposited => new ExchangeResult
            {
                amount = totalDeposited,
                cost = totalDeposited * exchangeRate
            }, exchangeResult =>
            {
                var value = exchangeResult.cost;
                selfInventory.Add(moneyType, value).Execute();
            });
    }

    public bool CanSell(SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
    {
        return selfInventory.Get(type) > 0
            && marketInventory.CanFitMoreOf(type);
    }
}
