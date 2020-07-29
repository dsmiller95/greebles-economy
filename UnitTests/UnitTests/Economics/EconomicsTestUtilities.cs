using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeModeling.Economics;
using TradeModeling.Exchanges;
using TradeModeling.Inventories;
using TradeModeling;
using TradeModeling.Functions;

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
        public static ISpaceFillingItemSource<TestItemType> CreateSpaceFillingSource(
            (TestItemType, float)[] initialItems,
            int capacity = 10,
            TestItemType[] spaceFillingItems = null)
        {
            spaceFillingItems = spaceFillingItems ?? new[] { TestItemType.Cactus, TestItemType.Corn };
            return new SpaceFillingInventorySource<TestItemType>(
                initialItems.ToDictionary(x => x.Item1, x => x.Item2),
                spaceFillingItems,
                capacity);
        }
        public static IInventoryItemSource<TestItemType> CreateBasicSource(
            (TestItemType, float)[] initialItems,
            TestItemType[] spaceFillingItems = null)
        {
            spaceFillingItems = spaceFillingItems ?? new[] { TestItemType.Cactus, TestItemType.Corn };
            return new BasicInventorySource<TestItemType>(
                initialItems.ToDictionary(x => x.Item1, x => x.Item2));
        }

        public static BasicInventory<TestItemType> CreateInventoryWithSpaceBacking(
            (TestItemType, float)[] initialItems,
            int capacity = 10,
            TestItemType[] spaceFillingItems = null,
            TestItemType moneyType = TestItemType.Pesos)
        {
            var backing = CreateSpaceFillingSource(initialItems, capacity, spaceFillingItems);
            return new BasicInventory<TestItemType>(backing, moneyType);
        }
        public static BasicInventory<TestItemType> CreateBasicInventory(
            (TestItemType, float)[] initialItems,
            TestItemType moneyType = TestItemType.Pesos)
        {
            return new BasicInventory<TestItemType>(
                initialItems.ToDictionary(x => x.Item1, x => x.Item2),
                moneyType);
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
