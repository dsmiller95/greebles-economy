using System;
using System.Collections.Generic;
using System.Text;
using TradeModeling.Economics;
using TradeModeling.Inventories;

namespace TradeModeling.Exchanges
{
    public interface IMarketExchangeAdapter<T> :
    ISeller<T, SpaceFillingInventory<T>, SpaceFillingInventory<T>>,
    IPurchaser<T, SpaceFillingInventory<T>, SpaceFillingInventory<T>>
    {
    }
}
