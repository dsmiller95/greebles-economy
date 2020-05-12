﻿using Assets.Economics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MarketSellerAdapter : ISeller, IPurchaser
{
    private Market market;
    private ResourceInventory sourceInventory;
    private ResourceType type;
    public MarketSellerAdapter(ResourceInventory sourceInventory, Market market, ResourceType type)
    {
        this.market = market;
        this.type = type;
        this.sourceInventory = sourceInventory;
    }

    public float Purchase(float amount, bool execute)
    {
        return this.market.PurchaseItemInto(this.sourceInventory, this.type, amount, execute).totalRevenue;
    }

    public float Sell(float amount, bool execute)
    {
        return this.market.SellItemFrom(this.sourceInventory, this.type, amount, execute).totalRevenue;
    }
}