using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeModeling.Exchanges;
using TradeModeling.Functions;

namespace UnitTests.Economics
{

    [TestClass]
    public class SigmoidMarketExchangeAdapterTest
    {

        [TestMethod]
        public void ShouldExecuteSimplePurchase()
        {
            var market = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });
            var self = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });

            var functionConfig = new SigmoidFunctionConfig { range = 10, offset = 0 };
            var function = new SigmoidFunction(functionConfig);
            var adapter = new SigmoidMarketExchangeAdapter<TestItemType>(
                new Dictionary<TestItemType, SigmoidFunctionConfig> { 
                    { TestItemType.Corn, functionConfig }
                }, TestItemType.Pesos);

            Assert.IsTrue(adapter.CanPurchase(TestItemType.Corn, self, market));

            var purchaseResult = adapter.Purchase(TestItemType.Corn, 1, self, market);
            Assert.AreEqual(1, purchaseResult.info.amount);

            var expectedPurchasePrice = function.GetIncrementalValue(2, 1);
            Assert.AreEqual(expectedPurchasePrice, purchaseResult.info.cost);

            purchaseResult.Execute();
            Assert.AreEqual(1, market.Get(TestItemType.Corn));
            Assert.AreEqual(3, self.Get(TestItemType.Corn));

            Assert.AreEqual(5 + expectedPurchasePrice, market.Get(TestItemType.Pesos));
            Assert.AreEqual(5 - expectedPurchasePrice, self.Get(TestItemType.Pesos));
        }

    }
}
