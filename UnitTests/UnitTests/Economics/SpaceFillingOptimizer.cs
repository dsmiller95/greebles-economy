using System;
using System.Linq;
using Assets.Economics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Assets.Economics.PurchaseOptimizer<SpaceFillingInventory<UnitTests.Economics.TestItemType>, SpaceFillingInventory<UnitTests.Economics.TestItemType>>;

namespace UnitTests.Economics
{
    [TestClass]
    public class SpaceFillingOptimizer
    {
        [TestMethod]
        public void ShouldOptimizeForUtilityWithVeryLargeInventory()
        {
            var self = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   10f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    2f)
            }, 100);
            var market = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   20f),
                (TestItemType.Corn,     20f),
                (TestItemType.Pesos,    5f)
            }, 100);

            var exchangeAdapters = new[]
            {
                EconomicsTestUtilities.CreateExchangeAdapter(TestItemType.Cactus, 2),
                EconomicsTestUtilities.CreateExchangeAdapter(TestItemType.Corn, 1),
            };

            var utilityFunctions = new[]
            {
                EconomicsTestUtilities.CreateUtilityFunction(TestItemType.Cactus, new []
                {
                    new WeightedRegion(0, 1)
                }),
                EconomicsTestUtilities.CreateUtilityFunction(TestItemType.Corn, new []
                {
                    new WeightedRegion(0, 1)
                })
            };

            var optimizer = new PurchaseOptimizer<SpaceFillingInventory<TestItemType>, SpaceFillingInventory<TestItemType>>(
                exchangeAdapters.Zip(utilityFunctions, (exchange, utility) => new ExchangeAdapter
                {
                    purchaser = exchange,
                    seller = exchange,
                    utilityFunction = utility
                }),
                self,
                market);

            optimizer.Optimize();

            Assert.AreEqual(6, self.Get(TestItemType.Cactus));
            Assert.AreEqual(12, self.Get(TestItemType.Corn));
            Assert.AreEqual(0, self.Get(TestItemType.Pesos));

            Assert.AreEqual(24, market.Get(TestItemType.Cactus));
            Assert.AreEqual(10, market.Get(TestItemType.Corn));
        }

        [TestMethod]
        public void ShouldOptimizeForMaxUtilityWithRestrictedInventoryUnlimitedMoney()
        {
            var self = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   0f),
                (TestItemType.Corn,     0f),
                (TestItemType.Pesos,    100f)
            }, 10);
            var market = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   20f),
                (TestItemType.Corn,     20f),
                (TestItemType.Pesos,    5f)
            }, 100);

            var exchangeAdapters = new[]
            {
                EconomicsTestUtilities.CreateExchangeAdapter(TestItemType.Cactus, 1),
                EconomicsTestUtilities.CreateExchangeAdapter(TestItemType.Corn, 1),
            };

            var utilityFunctions = new[]
            {
                EconomicsTestUtilities.CreateUtilityFunction(TestItemType.Cactus, new []
                {
                    new WeightedRegion(0, 2)
                }),
                EconomicsTestUtilities.CreateUtilityFunction(TestItemType.Corn, new []
                {
                    new WeightedRegion(0, 1)
                })
            };

            var optimizer = new PurchaseOptimizer<SpaceFillingInventory<TestItemType>, SpaceFillingInventory<TestItemType>>(
                exchangeAdapters.Zip(utilityFunctions, (exchange, utility) => new ExchangeAdapter
                {
                    purchaser = exchange,
                    seller = exchange,
                    utilityFunction = utility
                }),
                self,
                market);

            optimizer.Optimize();

            // at 7 cactus and 3 corn, the incremental utility is identical
            Assert.AreEqual(7, self.Get(TestItemType.Cactus));
            Assert.AreEqual(3, self.Get(TestItemType.Corn));
            Assert.AreEqual(90, self.Get(TestItemType.Pesos));

            Assert.AreEqual(13, market.Get(TestItemType.Cactus));
            Assert.AreEqual(17, market.Get(TestItemType.Corn));
        }

        [TestMethod]
        public void ShouldOptimizeForMaxUtilityWithRestrictedInventoryNoMoney()
        {
            var self = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   0f),
                (TestItemType.Corn,     10f),
                (TestItemType.Pesos,    0f)
            }, 10);
            var market = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   20f),
                (TestItemType.Corn,     20f),
                (TestItemType.Pesos,    5f)
            }, 100);

            var exchangeAdapters = new[]
            {
                EconomicsTestUtilities.CreateExchangeAdapter(TestItemType.Cactus, 1),
                EconomicsTestUtilities.CreateExchangeAdapter(TestItemType.Corn, 1),
            };

            var utilityFunctions = new[]
            {
                EconomicsTestUtilities.CreateUtilityFunction(TestItemType.Cactus, new []
                {
                    new WeightedRegion(0, 2)
                }),
                EconomicsTestUtilities.CreateUtilityFunction(TestItemType.Corn, new []
                {
                    new WeightedRegion(0, 1)
                })
            };

            var optimizer = new PurchaseOptimizer<SpaceFillingInventory<TestItemType>, SpaceFillingInventory<TestItemType>>(
                exchangeAdapters.Zip(utilityFunctions, (exchange, utility) => new ExchangeAdapter
                {
                    purchaser = exchange,
                    seller = exchange,
                    utilityFunction = utility
                }),
                self,
                market);

            optimizer.Optimize();

            // at 7 cactus and 3 corn, the incremental utility is identical
            Assert.AreEqual(7, self.Get(TestItemType.Cactus));
            Assert.AreEqual(3, self.Get(TestItemType.Corn));
            Assert.AreEqual(0, self.Get(TestItemType.Pesos));

            Assert.AreEqual(13, market.Get(TestItemType.Cactus));
            Assert.AreEqual(27, market.Get(TestItemType.Corn));
        }
    }
}
