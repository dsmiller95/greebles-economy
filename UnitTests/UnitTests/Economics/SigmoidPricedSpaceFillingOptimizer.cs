using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TradeModeling.Economics;
using TradeModeling.Functions;
using TradeModeling.Inventories;

namespace UnitTests.Economics
{
    /// <summary>
    /// 
    /// Integration Tests over the whole economics package which relates to optimizing the exchange of items
    ///     between two space filling inventories. moderated by a fixed-price market exchange adapter
    /// </summary>
    [TestClass]
    public class SigmoidPricedSpaceFillingOptimizer
    {
        [TestMethod]
        public void ShouldOptimizeForUtilityWithVeryLargeInventoryAndEqualUtility()
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

            var exchangeAdapter = EconomicsTestUtilities.CreateSigmoidExchangeAdapter(
                new[] {
                    (TestItemType.Cactus,
                    new SigmoidFunctionConfig{
                        range = 30f,
                        offset = 0f,
                        yRange = 1f
                    }),
                    (TestItemType.Corn,
                    new SigmoidFunctionConfig{
                        range = 30f,
                        offset = 0f,
                        yRange = 1f
                    }) }
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
            
            Assert.AreEqual(0.1f, self.Get(TestItemType.Pesos), 0.1f);
            Assert.AreEqual(12, self.Get(TestItemType.Cactus));
            Assert.AreEqual(7, self.Get(TestItemType.Corn));

            Assert.AreEqual(18, market.Get(TestItemType.Cactus));
            Assert.AreEqual(15, market.Get(TestItemType.Corn));
            Assert.AreEqual(7 - 0.1f, market.Get(TestItemType.Pesos), 0.1f);
        }

        // TODO: make sure everything works when starting with non-int money amounts

    }
}
