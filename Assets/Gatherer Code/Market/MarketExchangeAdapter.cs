using Assets.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MarketExchangeAdapter : ISeller, IPurchaser
{
    private Market market;
    private ResourceInventory sourceInventory;
    public ResourceType type {
        get;
        private set;
    }
    public MarketExchangeAdapter(ResourceInventory sourceInventory, Market market, ResourceType type)
    {
        this.market = market;
        this.type = type;
        this.sourceInventory = sourceInventory;
    }

    public PurchaseResult Purchase(float amount, bool execute, float simulatedMarketInventory)
    {
        //TODO: add support for simulated market inventory. The PurchaseItemInto method should accept a new parameter to allow for this
        var purchaseResult = this.market.PurchaseItemInto(this.sourceInventory, this.type, amount, execute);

        return new PurchaseResult
        {
            amount = purchaseResult.soldItems,
            cost = purchaseResult.totalRevenue
        };
    }

    public bool CanPurchase(float simulatedPurchase)
    {
        var currentAmount = this.GetCurrentMarketInventory();
        return currentAmount - simulatedPurchase > 0;
    }

    public float GetCurrentMarketInventory()
    {
        return this.market.inventory.getResource(this.type);
    }

    public float Sell(float amount, bool execute)
    {
        return this.market.SellItemFrom(this.sourceInventory, this.type, amount, execute).totalRevenue;
    }

    public bool CanSell()
    {
        var currentAmount = this.sourceInventory.getResource(this.type);
        return currentAmount > 0;
    }
}
