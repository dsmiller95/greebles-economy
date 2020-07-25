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
        public static SpaceFillingInventory<TestItemType> CreateInventory(
            (TestItemType, float)[] initialItems,
            int capacity = 10,
            TestItemType[] spaceFillingItems = null,
            TestItemType moneyType = TestItemType.Pesos)
        {
            spaceFillingItems = spaceFillingItems ?? new[] { TestItemType.Cactus, TestItemType.Corn };
            return new SpaceFillingInventory<TestItemType>(
                capacity,
                initialItems.ToDictionary(x => x.Item1, x => x.Item2),
                spaceFillingItems,
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
