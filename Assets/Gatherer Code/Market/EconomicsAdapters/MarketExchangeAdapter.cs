using Assets.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MarketExchangeAdapter: ISeller<SpaceFillingInventory<ResourceType>, SpaceFillingInventory<ResourceType>>, IPurchaser<SpaceFillingInventory<ResourceType>, SpaceFillingInventory<ResourceType>>
{
    public ResourceType type {
        get;
        private set;
    }
    private float exchangeRate;
    public MarketExchangeAdapter(ResourceType type, float exchangeRate)
    {
        this.type = type;
        this.exchangeRate = exchangeRate;
    }

    public PurchaseResult Purchase(float amount, bool execute, SpaceFillingInventory<ResourceType> selfInventory, SpaceFillingInventory<ResourceType> marketInventory)
    {
        var withdrawn = marketInventory.transferResourceInto(type, selfInventory, amount, execute);
        if (execute)
        {
            var value = this.exchangeRate * withdrawn;
            selfInventory.Consume(ResourceType.Gold, value);
        }

        var purchaseResult = new ResourceSellResult(withdrawn, exchangeRate);

        return new PurchaseResult
        {
            amount = purchaseResult.soldItems,
            cost = purchaseResult.totalRevenue
        };
    }

    public bool CanPurchase(SpaceFillingInventory<ResourceType> selfInventory, SpaceFillingInventory<ResourceType> marketInventory)
    {
        return marketInventory.getResource(type) > 0;
    }

    public float Sell(float amount, bool execute, SpaceFillingInventory<ResourceType> selfInventory, SpaceFillingInventory<ResourceType> marketInventory)
    {
        var deposited = selfInventory.transferResourceInto(type, marketInventory, amount, execute);
        if (execute)
        {
            var value = this.exchangeRate * deposited;
            selfInventory.addResource(ResourceType.Gold, value);
        }

        var sellResult = new ResourceSellResult(deposited, exchangeRate);
        return sellResult.totalRevenue;
    }

    public bool CanSell(SpaceFillingInventory<ResourceType> selfInventory, SpaceFillingInventory<ResourceType> marketInventory)
    {
        return selfInventory.getResource(type) > 0;
    }
}
