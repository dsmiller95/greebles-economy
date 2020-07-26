using TradeModeling.Economics;
using TradeModeling.Inventories;

namespace TradeModeling.Exchanges
{
    public interface IExchangeAdapter<T> : IExchange<T, BasicInventory<T>, BasicInventory<T>>
    {
    }
}
