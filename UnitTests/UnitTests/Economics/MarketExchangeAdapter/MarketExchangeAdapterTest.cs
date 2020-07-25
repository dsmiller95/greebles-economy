using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeModeling.Exchanges;

namespace UnitTests.Economics
{

    [TestClass]
    public class MarketExchangeAdapterTest
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

            var adapter = new SingleExchangeModel<TestItemType>(new Dictionary<TestItemType, float> { { TestItemType.Corn, 2 } }, TestItemType.Pesos);

            Assert.IsTrue(adapter.CanPurchase(TestItemType.Corn, self, market));

            var purchaseResult = adapter.Purchase(TestItemType.Corn, 1, self, market);
            Assert.AreEqual(1, purchaseResult.info.amount);
            Assert.AreEqual(2, purchaseResult.info.cost);

            purchaseResult.Execute();
            Assert.AreEqual(1, market.Get(TestItemType.Corn));
            Assert.AreEqual(3, self.Get(TestItemType.Corn));

            Assert.AreEqual(7, market.Get(TestItemType.Pesos));
            Assert.AreEqual(3, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void ShouldSimulateExecutionSimplePurchase()
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

            var adapter = new SingleExchangeModel<TestItemType>(new Dictionary<TestItemType, float> { { TestItemType.Corn, 2 } }, TestItemType.Pesos);

            Assert.IsTrue(adapter.CanPurchase(TestItemType.Corn, self, market));
            var purchaseResult = adapter.Purchase(TestItemType.Corn, 1, self, market);

            Assert.AreEqual(1, purchaseResult.info.amount);
            Assert.AreEqual(2, purchaseResult.info.cost);

            Assert.AreEqual(2, market.Get(TestItemType.Corn));
            Assert.AreEqual(2, self.Get(TestItemType.Corn));

            Assert.AreEqual(5, market.Get(TestItemType.Pesos));
            Assert.AreEqual(5, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void WhenMarketIsEmptyShouldNotPurchase()
        {
            var market = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     0f),
                (TestItemType.Pesos,    5f)
            });
            var self = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });

            var adapter = new SingleExchangeModel<TestItemType>(new Dictionary<TestItemType, float> { { TestItemType.Corn, 2 } }, TestItemType.Pesos);

            Assert.IsFalse(adapter.CanPurchase(TestItemType.Corn, self, market));

            var purchaseResult = adapter.Purchase(TestItemType.Corn, 1, self, market);
            Assert.AreEqual(0, purchaseResult.info.amount);
            Assert.AreEqual(0, purchaseResult.info.cost);

            purchaseResult.Execute();
            Assert.AreEqual(0, market.Get(TestItemType.Corn));
            Assert.AreEqual(2, self.Get(TestItemType.Corn));

            Assert.AreEqual(5, market.Get(TestItemType.Pesos));
            Assert.AreEqual(5, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void WhenSelfIsFullShouldNotPurchase()
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
            }, 4);
            
            var adapter = new SingleExchangeModel<TestItemType>(new Dictionary<TestItemType, float> { { TestItemType.Corn, 2 } }, TestItemType.Pesos);

            Assert.IsFalse(adapter.CanPurchase(TestItemType.Corn, self, market));
            var purchaseResult = adapter.Purchase(TestItemType.Corn, 1, self, market);

            Assert.AreEqual(0, purchaseResult.info.amount);
            Assert.AreEqual(0, purchaseResult.info.cost);

            purchaseResult.Execute();
            Assert.AreEqual(2, market.Get(TestItemType.Corn));
            Assert.AreEqual(2, self.Get(TestItemType.Corn));

            Assert.AreEqual(5, market.Get(TestItemType.Pesos));
            Assert.AreEqual(5, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void WhenSelfIsBrokeShouldNotPurchase()
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
                (TestItemType.Pesos,    0f)
            });

            var adapter = new SingleExchangeModel<TestItemType>(new Dictionary<TestItemType, float> { { TestItemType.Corn, 2 } }, TestItemType.Pesos);

            Assert.IsFalse(adapter.CanPurchase(TestItemType.Corn, self, market));
            var purchaseResult = adapter.Purchase(TestItemType.Corn, 1, self, market);

            Assert.AreEqual(0, purchaseResult.info.amount);
            Assert.AreEqual(0, purchaseResult.info.cost);

            purchaseResult.Execute();
            Assert.AreEqual(2, market.Get(TestItemType.Corn));
            Assert.AreEqual(2, self.Get(TestItemType.Corn));

            Assert.AreEqual(5, market.Get(TestItemType.Pesos));
            Assert.AreEqual(0, self.Get(TestItemType.Pesos));
        }


        [TestMethod]
        public void ShouldExecuteSimpleSell()
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

            var adapter = new SingleExchangeModel<TestItemType>(new Dictionary<TestItemType, float> { { TestItemType.Corn, 2 } }, TestItemType.Pesos);

            Assert.IsTrue(adapter.CanSell(TestItemType.Corn, self, market));
            var sellResult = adapter.Sell(TestItemType.Corn, 1, self, market);

            Assert.AreEqual(1, sellResult.info.amount);
            Assert.AreEqual(2, sellResult.info.cost);

            sellResult.Execute();
            Assert.AreEqual(3, market.Get(TestItemType.Corn));
            Assert.AreEqual(1, self.Get(TestItemType.Corn));

            Assert.AreEqual(7, self.Get(TestItemType.Pesos));
            Assert.AreEqual(3, market.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void ShouldSimulateExecutionOfSimpleSell()
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

            var adapter = new SingleExchangeModel<TestItemType>(new Dictionary<TestItemType, float> { { TestItemType.Corn, 2 } }, TestItemType.Pesos);

            Assert.IsTrue(adapter.CanSell(TestItemType.Corn, self, market));
            var sellResult = adapter.Sell(TestItemType.Corn, 1, self, market);

            Assert.AreEqual(1, sellResult.info.amount);
            Assert.AreEqual(2, sellResult.info.cost);

            Assert.AreEqual(2, market.Get(TestItemType.Corn));
            Assert.AreEqual(2, self.Get(TestItemType.Corn));

            Assert.AreEqual(5, market.Get(TestItemType.Pesos));
            Assert.AreEqual(5, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void WhenSelfIsEmptyShouldNotSell()
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
                (TestItemType.Corn,     0f),
                (TestItemType.Pesos,    5f)
            });

            var adapter = new SingleExchangeModel<TestItemType>(new Dictionary<TestItemType, float> { { TestItemType.Corn, 2 } }, TestItemType.Pesos);

            Assert.IsFalse(adapter.CanSell(TestItemType.Corn, self, market));
            var sellResult = adapter.Sell(TestItemType.Corn, 1, self, market);
            Assert.AreEqual(0, sellResult.info.amount);
            Assert.AreEqual(0, sellResult.info.cost);

            sellResult.Execute();
            Assert.AreEqual(2, market.Get(TestItemType.Corn));
            Assert.AreEqual(0, self.Get(TestItemType.Corn));

            Assert.AreEqual(5, market.Get(TestItemType.Pesos));
            Assert.AreEqual(5, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void WhenMarketIsFullShouldNotSell()
        {
            var market = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            }, 4);
            var self = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });

            var adapter = new SingleExchangeModel<TestItemType>(new Dictionary<TestItemType, float> { { TestItemType.Corn, 2 } }, TestItemType.Pesos);

            Assert.IsFalse(adapter.CanSell(TestItemType.Corn, self, market));
            var sellResult = adapter.Sell(TestItemType.Corn, 1, self, market);
            Assert.AreEqual(0, sellResult.info.amount);
            Assert.AreEqual(0, sellResult.info.cost);

            sellResult.Execute();
            Assert.AreEqual(2, market.Get(TestItemType.Corn));
            Assert.AreEqual(2, self.Get(TestItemType.Corn));

            Assert.AreEqual(5, market.Get(TestItemType.Pesos));
            Assert.AreEqual(5, self.Get(TestItemType.Pesos));
        }


        [TestMethod]
        public void WhenMarketIsBrokeShouldNotSell()
        {
            var market = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    0f)
            });
            var self = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });

            var adapter = new SingleExchangeModel<TestItemType>(new Dictionary<TestItemType, float> { { TestItemType.Corn, 2 } }, TestItemType.Pesos);

            Assert.IsFalse(adapter.CanSell(TestItemType.Corn, self, market));
            var sellResult = adapter.Sell(TestItemType.Corn, 1, self, market);
            Assert.AreEqual(0, sellResult.info.amount);
            Assert.AreEqual(0, sellResult.info.cost);

            sellResult.Execute();
            Assert.AreEqual(2, market.Get(TestItemType.Corn));
            Assert.AreEqual(2, self.Get(TestItemType.Corn));

            Assert.AreEqual(0, market.Get(TestItemType.Pesos));
            Assert.AreEqual(5, self.Get(TestItemType.Pesos));
        }

    }
}
