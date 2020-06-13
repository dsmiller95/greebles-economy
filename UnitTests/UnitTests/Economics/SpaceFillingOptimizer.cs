using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeModeling.Economics;
using TradeModeling.Inventories;
using static TradeModeling.Economics.PurchaseOptimizer<UnitTests.Economics.TestItemType, TradeModeling.Inventories.SpaceFillingInventory<UnitTests.Economics.TestItemType>, TradeModeling.Inventories.SpaceFillingInventory<UnitTests.Economics.TestItemType>>;

namespace UnitTests.Economics
{
    /// <summary>
    /// 
    /// Integration Tests over the whole economics package which relates to optimizing the exchange of items
    ///     between two space filling inventories. moderated by a fixed-price market exchange adapter
    /// </summary>
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

            var exchangeAdapter = EconomicsTestUtilities.CreateExchangeAdapter(
                new[] { (TestItemType.Cactus, 2f), (TestItemType.Corn, 1f) }
                );

            var utilityFunctions = new UtilityEvaluatorFunctionMapper<TestItemType>(new Dictionary<TestItemType, IIncrementalFunction>
            {
                { TestItemType.Cactus, new InverseWeightedUtility(new []
                {
                    new WeightedRegion(0, 1)
                }) },
                { TestItemType.Corn, new InverseWeightedUtility(new []
                {
                    new WeightedRegion(0, 1)
                }) }
            });

            var optimizer = new PurchaseOptimizer<TestItemType, SpaceFillingInventory<TestItemType>, SpaceFillingInventory<TestItemType>>(
                self,
                market,
                new[] { TestItemType.Cactus, TestItemType.Corn },
                exchangeAdapter,
                exchangeAdapter,
                utilityFunctions);

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

            var exchangeAdapter = EconomicsTestUtilities.CreateExchangeAdapter(
                new[] { (TestItemType.Cactus, 1f), (TestItemType.Corn, 1f) }
                );

            var utilityFunctions = new UtilityEvaluatorFunctionMapper<TestItemType>(new Dictionary<TestItemType, IIncrementalFunction>
            {
                { TestItemType.Cactus, new InverseWeightedUtility(new []
                {
                    new WeightedRegion(0, 2)
                }) },
                { TestItemType.Corn, new InverseWeightedUtility(new []
                {
                    new WeightedRegion(0, 1)
                }) }
            });

            var optimizer = new PurchaseOptimizer<TestItemType, SpaceFillingInventory<TestItemType>, SpaceFillingInventory<TestItemType>>(
                self,
                market,
                new[] { TestItemType.Cactus, TestItemType.Corn },
                exchangeAdapter,
                exchangeAdapter,
                utilityFunctions);

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

            var exchangeAdapter = EconomicsTestUtilities.CreateExchangeAdapter(
                new[] { (TestItemType.Cactus, 1f), (TestItemType.Corn, 1f) }
                );

            var utilityFunctions = new UtilityEvaluatorFunctionMapper<TestItemType>(new Dictionary<TestItemType, IIncrementalFunction>
            {
                { TestItemType.Cactus, new InverseWeightedUtility(new []
                {
                    new WeightedRegion(0, 2)
                }) },
                { TestItemType.Corn, new InverseWeightedUtility(new []
                {
                    new WeightedRegion(0, 1)
                }) }
            });

            var optimizer = new PurchaseOptimizer<TestItemType, SpaceFillingInventory<TestItemType>, SpaceFillingInventory<TestItemType>>(
                self,
                market,
                new[] { TestItemType.Cactus, TestItemType.Corn },
                exchangeAdapter,
                exchangeAdapter,
                utilityFunctions);

            optimizer.Optimize();

            // at 7 cactus and 3 corn, the incremental utility is identical
            Assert.AreEqual(7, self.Get(TestItemType.Cactus));
            Assert.AreEqual(3, self.Get(TestItemType.Corn));
            Assert.AreEqual(0, self.Get(TestItemType.Pesos));

            Assert.AreEqual(13, market.Get(TestItemType.Cactus));
            Assert.AreEqual(27, market.Get(TestItemType.Corn));
        }
        [TestMethod]
        public void ShouldOptimizeForUtilityWithVeryLargeInventoryAndGenerateSourceUtilityWeights()
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

            var exchangeAdapter = EconomicsTestUtilities.CreateExchangeAdapter(
                new[] { (TestItemType.Cactus, 2f), (TestItemType.Corn, 1f) }
                );

            var utilityFunctions = new UtilityEvaluatorFunctionMapper<TestItemType>(new Dictionary<TestItemType, IIncrementalFunction>
            {
                { TestItemType.Cactus, new InverseWeightedUtility(new []
                {
                    new WeightedRegion(0, 1)
                }) },
                { TestItemType.Corn, new InverseWeightedUtility(new []
                {
                    new WeightedRegion(0, 1)
                }) }
            });

            var tradeableResources = new[] { TestItemType.Cactus, TestItemType.Corn };
            var optimizer = new PurchaseOptimizer<TestItemType, SpaceFillingInventory<TestItemType>, SpaceFillingInventory<TestItemType>>(
                self,
                market,
                new[] { TestItemType.Cactus, TestItemType.Corn },
                exchangeAdapter,
                exchangeAdapter,
                utilityFunctions);

            var ledger = optimizer.Optimize();

            Assert.AreEqual(6, self.Get(TestItemType.Cactus));
            Assert.AreEqual(12, self.Get(TestItemType.Corn));
            Assert.AreEqual(0, self.Get(TestItemType.Pesos));

            Assert.AreEqual(24, market.Get(TestItemType.Cactus));
            Assert.AreEqual(10, market.Get(TestItemType.Corn));

            var utilityAnalyzer = new UtilityAnalyzer<TestItemType>();
            var sourceUtilities = utilityAnalyzer.GetUtilityPerInitialResource(
                tradeableResources,
                self,
                ledger,
                utilityFunctions);

            var expectedUtilities = new Dictionary<TestItemType, float>()
            {   //these numbers verified in very official spreadsheet
                {TestItemType.Cactus, 4.518807119f },
                {TestItemType.Corn, 1.034403559f }
            };
            Assert.AreEqual(expectedUtilities[TestItemType.Cactus], sourceUtilities[TestItemType.Cactus], 0.000001f);
            Assert.AreEqual(expectedUtilities[TestItemType.Corn], sourceUtilities[TestItemType.Corn], 0.000001f);
        }

    }
}
