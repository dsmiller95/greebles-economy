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
            var market = EconomicsTestUtilities.CreateInventoryWithSpaceBacking(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });
            var self = EconomicsTestUtilities.CreateInventoryWithSpaceBacking(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });

            var functionConfig = new SigmoidFunctionConfig { range = 10, offset = 0, yRange = 1 };
            var function = new SigmoidFunction(functionConfig);
            var adapter = new SigmoidMarketExchangeAdapter<TestItemType>(
                new Dictionary<TestItemType, SigmoidFunctionConfig> {
                    { TestItemType.Corn, functionConfig }
                }, TestItemType.Pesos);

            Assert.IsTrue(adapter.CanPurchase(TestItemType.Corn, self, market));

            var purchaseResult = adapter.Purchase(TestItemType.Corn, 1, self, market);
            Assert.AreEqual(1, purchaseResult.info.amount);

            var expectedPurchasePrice = function.GetIncrementalValue(1, 1);
            Assert.AreEqual(expectedPurchasePrice, purchaseResult.info.cost);

            purchaseResult.Execute();
            Assert.AreEqual(1, market.Get(TestItemType.Corn));
            Assert.AreEqual(3, self.Get(TestItemType.Corn));

            Assert.AreEqual(5 + expectedPurchasePrice, market.Get(TestItemType.Pesos));
            Assert.AreEqual(5 - expectedPurchasePrice, self.Get(TestItemType.Pesos));
        }
        [TestMethod]
        public void ShouldExecuteSimplePurchaseLimitedBySelfFunds()
        {
            var market = EconomicsTestUtilities.CreateInventoryWithSpaceBacking(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    2f)
            });
            var self = EconomicsTestUtilities.CreateInventoryWithSpaceBacking(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    1f)
            });

            var functionConfig = new SigmoidFunctionConfig { range = 10, offset = 0, yRange = 1 };
            var function = new SigmoidFunction(functionConfig);
            var adapter = new SigmoidMarketExchangeAdapter<TestItemType>(
                new Dictionary<TestItemType, SigmoidFunctionConfig> {
                    { TestItemType.Corn, functionConfig }
                }, TestItemType.Pesos);

            Assert.IsTrue(adapter.CanPurchase(TestItemType.Corn, self, market));

            var purchaseResult = adapter.Purchase(TestItemType.Corn, 10, self, market);

            var expectedPurchasePrice = 1;
            var expectedEndAmount = function.GetPointFromNetExtraValueFromPoint(-expectedPurchasePrice, 2);
            var expectedPurchasedAmount = 2 - expectedEndAmount;

            Assert.AreEqual(expectedPurchasedAmount, purchaseResult.info.amount, 1e-5);
            Assert.AreEqual(expectedPurchasePrice, purchaseResult.info.cost, 1e-5);

            purchaseResult.Execute();
            Assert.AreEqual(2 - expectedPurchasedAmount, market.Get(TestItemType.Corn), 1e-5);
            Assert.AreEqual(2 + expectedPurchasedAmount, self.Get(TestItemType.Corn), 1e-5);

            Assert.AreEqual(2 + expectedPurchasePrice, market.Get(TestItemType.Pesos), 1e-5);
            Assert.AreEqual(0, self.Get(TestItemType.Pesos), 1e-5);
        }

        [TestMethod]
        public void ShouldExecuteSimpleSell()
        {
            var market = EconomicsTestUtilities.CreateInventoryWithSpaceBacking(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });
            var self = EconomicsTestUtilities.CreateInventoryWithSpaceBacking(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });

            var functionConfig = new SigmoidFunctionConfig { range = 10, offset = 0, yRange = 1 };
            var priceFunction = new SigmoidFunction(functionConfig);
            var adapter = new SigmoidMarketExchangeAdapter<TestItemType>(
                new Dictionary<TestItemType, SigmoidFunctionConfig> {
                    { TestItemType.Corn, functionConfig }
                }, TestItemType.Pesos);

            Assert.IsTrue(adapter.CanSell(TestItemType.Corn, self, market));

            var purchaseResult = adapter.Sell(TestItemType.Corn, 1, self, market);
            Assert.AreEqual(1, purchaseResult.info.amount);

            var expectedSellPrice = priceFunction.GetIncrementalValue(2, 1);
            Assert.AreEqual(expectedSellPrice, purchaseResult.info.cost);

            purchaseResult.Execute();
            Assert.AreEqual(3, market.Get(TestItemType.Corn));
            Assert.AreEqual(1, self.Get(TestItemType.Corn));

            Assert.AreEqual(5 - expectedSellPrice, market.Get(TestItemType.Pesos));
            Assert.AreEqual(5 + expectedSellPrice, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void ShouldExecuteSimpleSellLimitedByMarketFunds()
        {
            var market = EconomicsTestUtilities.CreateInventoryWithSpaceBacking(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    1f)
            });
            var self = EconomicsTestUtilities.CreateInventoryWithSpaceBacking(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    2f)
            });

            var functionConfig = new SigmoidFunctionConfig { range = 10, offset = 0, yRange = 1 };
            var function = new SigmoidFunction(functionConfig);
            var adapter = new SigmoidMarketExchangeAdapter<TestItemType>(
                new Dictionary<TestItemType, SigmoidFunctionConfig> {
                    { TestItemType.Corn, functionConfig }
                }, TestItemType.Pesos);

            Assert.IsTrue(adapter.CanSell(TestItemType.Corn, self, market));

            var purchaseResult = adapter.Sell(TestItemType.Corn, 10, self, market);

            var expectedSellPrice = 1;
            var expectedEndAmount = function.GetPointFromNetExtraValueFromPoint(expectedSellPrice, 2);
            var expectedSoldAmount =  expectedEndAmount - 2;

            Assert.AreEqual(expectedSoldAmount, purchaseResult.info.amount, 1e-5);
            Assert.AreEqual(expectedSellPrice, purchaseResult.info.cost, 1e-5);

            purchaseResult.Execute();
            Assert.AreEqual(2 + expectedSoldAmount, market.Get(TestItemType.Corn), 1e-5);
            Assert.AreEqual(2 - expectedSoldAmount, self.Get(TestItemType.Corn), 1e-5);

            Assert.AreEqual(0, market.Get(TestItemType.Pesos), 1e-5);
            Assert.AreEqual(2 + expectedSellPrice, self.Get(TestItemType.Pesos), 1e-5);
        }

    }
}
