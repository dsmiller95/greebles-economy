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

    public ExchangeResult Purchase(float amount, bool execute, SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
    {
        var withdrawn = marketInventory.transferResourceInto(type, selfInventory, amount, execute);
        if (execute)
        {
            var value = this.exchangeRate * withdrawn;
            selfInventory.Consume(moneyType, value);
        }

        var purchaseResult = new ResourceSellResult(withdrawn, exchangeRate);

        return new ExchangeResult
        {
            amount = purchaseResult.soldItems,
            cost = purchaseResult.totalRevenue
        };
    }

    public bool CanPurchase(SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
    {
        return marketInventory.Get(type) > 0
            && selfInventory.Get(moneyType) > 0
            && selfInventory.CanFitMoreOf(type);
    }

    public ExchangeResult Sell(float amount, bool execute, SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
    {
        var deposited = selfInventory.transferResourceInto(type, marketInventory, amount, execute);
        if (execute)
        {
            var value = this.exchangeRate * deposited;
            selfInventory.Add(moneyType, value);
        }

        var sellResult = new ResourceSellResult(deposited, exchangeRate);

        return new ExchangeResult
        {
            amount = sellResult.soldItems,
            cost = sellResult.totalRevenue
        };
    }

    public bool CanSell(SpaceFillingInventory<T> selfInventory, SpaceFillingInventory<T> marketInventory)
    {
        return selfInventory.Get(type) > 0
            && marketInventory.CanFitMoreOf(type);
    }
}
