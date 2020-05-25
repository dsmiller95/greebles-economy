using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Economics.MarketExchangeAdapter
{
    [TestClass]
    public class MarketExchangeAdapterTest
    {
        enum TestItemType
        {
            Pesos,
            Corn,
            Cactus
        }

        private SpaceFillingInventory<TestItemType> CreateInventory(
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

        [TestMethod]
        public void ShouldExecuteSimplePurchase()
        {
            var market = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });
            var self = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });

            var adapter = new MarketExchangeAdapter<TestItemType>(TestItemType.Corn, 2, TestItemType.Pesos);

            Assert.IsTrue(adapter.CanPurchase(self, market));
            var purchaseResult = adapter.Purchase(1, true, self, market);

            Assert.AreEqual(1, purchaseResult.amount);
            Assert.AreEqual(2, purchaseResult.cost);

            Assert.AreEqual(1, market.Get(TestItemType.Corn));
            Assert.AreEqual(3, self.Get(TestItemType.Corn));

            Assert.AreEqual(3, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void ShouldSimulateExecutionSimplePurchase()
        {
            var market = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });
            var self = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });

            var adapter = new MarketExchangeAdapter<TestItemType>(TestItemType.Corn, 2, TestItemType.Pesos);

            Assert.IsTrue(adapter.CanPurchase(self, market));
            var purchaseResult = adapter.Purchase(1, false, self, market);

            Assert.AreEqual(1, purchaseResult.amount);
            Assert.AreEqual(2, purchaseResult.cost);

            Assert.AreEqual(2, market.Get(TestItemType.Corn));
            Assert.AreEqual(2, self.Get(TestItemType.Corn));

            Assert.AreEqual(5, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void WhenMarketIsEmptyShouldNotPurchase()
        {
            var market = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     0f),
                (TestItemType.Pesos,    5f)
            });
            var self = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });

            var adapter = new MarketExchangeAdapter<TestItemType>(TestItemType.Corn, 2, TestItemType.Pesos);

            Assert.IsFalse(adapter.CanPurchase(self, market));
            var purchaseResult = adapter.Purchase(1, true, self, market);

            Assert.AreEqual(0, purchaseResult.amount);
            Assert.AreEqual(0, purchaseResult.cost);

            Assert.AreEqual(0, market.Get(TestItemType.Corn));
            Assert.AreEqual(2, self.Get(TestItemType.Corn));

            Assert.AreEqual(5, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void WhenSelfIsFullShouldNotPurchase()
        {
            var market = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });
            var self = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            }, 4);

            var adapter = new MarketExchangeAdapter<TestItemType>(TestItemType.Corn, 2, TestItemType.Pesos);

            Assert.IsFalse(adapter.CanPurchase(self, market));
            var purchaseResult = adapter.Purchase(1, true, self, market);

            Assert.AreEqual(0, purchaseResult.amount);
            Assert.AreEqual(0, purchaseResult.cost);

            Assert.AreEqual(2, market.Get(TestItemType.Corn));
            Assert.AreEqual(2, self.Get(TestItemType.Corn));

            Assert.AreEqual(5, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void WhenSelfIsBrokeShouldNotPurchase()
        {
            var market = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });
            var self = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    0f)
            });

            var adapter = new MarketExchangeAdapter<TestItemType>(TestItemType.Corn, 2, TestItemType.Pesos);

            Assert.IsFalse(adapter.CanPurchase(self, market));
            var purchaseResult = adapter.Purchase(1, true, self, market);

            Assert.AreEqual(0, purchaseResult.amount);
            Assert.AreEqual(0, purchaseResult.cost);

            Assert.AreEqual(2, market.Get(TestItemType.Corn));
            Assert.AreEqual(2, self.Get(TestItemType.Corn));

            Assert.AreEqual(0, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void ShouldSimulateExecutionOfSimpleSell()
        {
            var market = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });
            var self = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });

            var adapter = new MarketExchangeAdapter<TestItemType>(TestItemType.Corn, 2, TestItemType.Pesos);

            Assert.IsTrue(adapter.CanSell(self, market));
            var sellResult = adapter.Sell(1, false, self, market);

            Assert.AreEqual(1, sellResult.amount);
            Assert.AreEqual(2, sellResult.cost);

            Assert.AreEqual(2, market.Get(TestItemType.Corn));
            Assert.AreEqual(2, self.Get(TestItemType.Corn));

            Assert.AreEqual(5, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void ShouldExecuteSimpleSell()
        {
            var market = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });
            var self = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });

            var adapter = new MarketExchangeAdapter<TestItemType>(TestItemType.Corn, 2, TestItemType.Pesos);

            Assert.IsTrue(adapter.CanSell(self, market));
            var sellResult = adapter.Sell(1, true, self, market);

            Assert.AreEqual(1, sellResult.amount);
            Assert.AreEqual(2, sellResult.cost);

            Assert.AreEqual(3, market.Get(TestItemType.Corn));
            Assert.AreEqual(1, self.Get(TestItemType.Corn));

            Assert.AreEqual(7, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void WhenSelfIsEmptyShouldNotSell()
        {
            var market = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });
            var self = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     0f),
                (TestItemType.Pesos,    5f)
            });

            var adapter = new MarketExchangeAdapter<TestItemType>(TestItemType.Corn, 2, TestItemType.Pesos);

            Assert.IsFalse(adapter.CanSell(self, market));
            var sellResult = adapter.Sell(1, true, self, market);

            Assert.AreEqual(0, sellResult.amount);
            Assert.AreEqual(0, sellResult.cost);

            Assert.AreEqual(2, market.Get(TestItemType.Corn));
            Assert.AreEqual(0, self.Get(TestItemType.Corn));

            Assert.AreEqual(5, self.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void WhenMarketIsFullShouldNotSell()
        {
            var market = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            }, 4);
            var self = CreateInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     2f),
                (TestItemType.Pesos,    5f)
            });

            var adapter = new MarketExchangeAdapter<TestItemType>(TestItemType.Corn, 2, TestItemType.Pesos);

            Assert.IsFalse(adapter.CanSell(self, market));
            var sellResult = adapter.Sell(1, true, self, market);

            Assert.AreEqual(0, sellResult.amount);
            Assert.AreEqual(0, sellResult.cost);

            Assert.AreEqual(2, market.Get(TestItemType.Corn));
            Assert.AreEqual(2, self.Get(TestItemType.Corn));

            Assert.AreEqual(5, self.Get(TestItemType.Pesos));
        }
    }
}
