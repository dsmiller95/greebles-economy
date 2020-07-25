using TradeModeling.Economics;
using TradeModeling.Inventories;

namespace TradeModeling.Exchanges
{
    public interface IExchangeAdapter<T> : IExchange<T, SpaceFillingInventory<T>, SpaceFillingInventory<T>>
    {
    }
}
