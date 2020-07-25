using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeModeling.Exchanges;

namespace UnitTests.Economics.Exchanges
{
    [TestClass]
    public class AdaptableExchangePoolTest
    {
        [TestMethod]
        public void ShouldDistributeResourcesAcrossPoolOnCreation()
        {
            var starterMarketInventory = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   10f),
                (TestItemType.Corn,     8f),
                (TestItemType.Pesos,    5f)
            });

            var exchangeSeed = new SingleExchangeModel<TestItemType>(new Dictionary<TestItemType, float> { { TestItemType.Corn, 2 } }, TestItemType.Pesos);

            var pool = new AdaptableExchangePool<TestItemType>(2, new AdaptableExchangeModel<TestItemType>(exchangeSeed));
        }
    }
}
