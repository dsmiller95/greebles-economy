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

    public PurchaseResult Purchase(float amount, bool execute)
    {
        var purchaseResult = this.market.PurchaseItemInto(this.sourceInventory, this.type, amount, execute);

        return new PurchaseResult
        {
            amount = purchaseResult.soldItems,
            cost = purchaseResult.totalRevenue
        };
    }

    public bool CanPurchase()
    {
        var currentAmount = this.market.inventory.getResource(this.type);
        return currentAmount > 0;
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
