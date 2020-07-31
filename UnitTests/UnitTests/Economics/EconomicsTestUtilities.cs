using System.Linq;
using TradeModeling.Exchanges;
using TradeModeling.Functions;
using TradeModeling.Inventories;
using TradeModeling.Inventories.Adapter;

namespace UnitTests.Economics
{
    public enum TestItemType
    {
        Pesos,
        Corn,
        Chips,
        Chilis,
        Cactus
    }
    public static class EconomicsTestUtilities
    {
        public static ISpaceFillingInventory<TestItemType> CreateSpaceFillingSource(
            (TestItemType, float)[] initialItems,
            int capacity = 10,
            TestItemType[] spaceFillingItems = null,
            TestItemType[] validItems = null)
        {
            spaceFillingItems = spaceFillingItems ?? new[] { TestItemType.Cactus, TestItemType.Corn };
            return new SpaceFillingInventory<TestItemType>(
                initialItems.ToDictionary(x => x.Item1, x => x.Item2),
                spaceFillingItems,
                capacity,
                validItems);
        }
        public static IInventory<TestItemType> CreateBasicSource(
            (TestItemType, float)[] initialItems,
            TestItemType[] spaceFillingItems = null)
        {
            spaceFillingItems = spaceFillingItems ?? new[] { TestItemType.Cactus, TestItemType.Corn };
            return new BasicInventory<TestItemType>(
                initialItems.ToDictionary(x => x.Item1, x => x.Item2));
        }

        public static TradingInventoryAdapter<TestItemType> CreateInventoryWithSpaceBacking(
            (TestItemType, float)[] initialItems,
            int capacity = 10,
            TestItemType[] spaceFillingItems = null,
            TestItemType moneyType = TestItemType.Pesos)
        {
            var backing = CreateSpaceFillingSource(initialItems, capacity, spaceFillingItems);
            return new TradingInventoryAdapter<TestItemType>(backing, moneyType);
        }

        public static SingleExchangeModel<TestItemType> CreateExchangeAdapter(
            (TestItemType, float)[] exchangeRates,
            TestItemType moneyType = TestItemType.Pesos)
        {
            return new SingleExchangeModel<TestItemType>(exchangeRates.ToDictionary(x => x.Item1, x => x.Item2), moneyType);
        }

        public static SigmoidMarketExchangeAdapter<TestItemType> CreateSigmoidExchangeAdapter(
            (TestItemType, SigmoidFunctionConfig)[] exchangeRates,
            TestItemType moneyType = TestItemType.Pesos)
        {
            return new SigmoidMarketExchangeAdapter<TestItemType>(exchangeRates
                .ToDictionary(x => x.Item1, x => x.Item2)
                , moneyType);
        }
    }
}
