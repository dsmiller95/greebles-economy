using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeModeling.Inventories;
using TradeModeling.TradeRouteUtilities;

namespace UnitTests.Economics.TradeRoutes
{
    [TestClass]
    public class TradeRouteAutoBalanceTest
    {
        [TestMethod]
        public void ShouldEmitNoTradesForSingleInventory()
        {
            var inventories = new[]
            {
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 1f),
                    (TestItemType.Corn, 2f)
                }, 100)
            };

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(inventories, new[] { TestItemType.Cactus, TestItemType.Corn });

            Assert.AreEqual(1, balanceTrades.Length);
            Assert.AreEqual(0, balanceTrades[0].Length);
        }

        [TestMethod]
        public void ShouldEmitOneTradePerInventoryForTwoImbalancedInventories()
        {
            var inventories = new[]
            {
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 1f),
                    (TestItemType.Corn, 2f)
                }, 100),
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 1f),
                    (TestItemType.Corn, 0f)
                }, 100)
            };

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(inventories, new[] { TestItemType.Cactus, TestItemType.Corn });

            Assert.AreEqual(2, balanceTrades.Length);
            Assert.AreEqual(1, balanceTrades[0].Length);
            var firstTrade = balanceTrades[0][0];
            Assert.AreEqual(TestItemType.Corn, firstTrade.type);
            Assert.AreEqual(-1, firstTrade.amount, 1e-5f);

            Assert.AreEqual(1, balanceTrades[1].Length);
            var secondTrade = balanceTrades[1][0];
            Assert.AreEqual(TestItemType.Corn, secondTrade.type);
            Assert.AreEqual(1, secondTrade.amount, 1e-5f);
        }

        [TestMethod]
        public void ShouldEmitTwoTradesPerMarketForTwoImbalancedInventories()
        {
            var inventories = new[]
            {
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 4f),
                    (TestItemType.Corn, 2f)
                }, 100),
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 10f),
                    (TestItemType.Corn, 0f)
                }, 100)
            };

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(inventories, new[] { TestItemType.Cactus, TestItemType.Corn });

            Assert.AreEqual(2, balanceTrades.Length);

            Assert.AreEqual(2, balanceTrades[0].Length);
            var firstMarketTrade = balanceTrades[0][0];
            Assert.AreEqual(TestItemType.Cactus, firstMarketTrade.type);
            Assert.AreEqual(3, firstMarketTrade.amount, 1e-5f);
            firstMarketTrade = balanceTrades[0][1];
            Assert.AreEqual(TestItemType.Corn, firstMarketTrade.type);
            Assert.AreEqual(-1, firstMarketTrade.amount, 1e-5f);

            Assert.AreEqual(2, balanceTrades[1].Length);
            var secondMarketTrade = balanceTrades[1][0];
            Assert.AreEqual(TestItemType.Cactus, secondMarketTrade.type);
            Assert.AreEqual(-3, secondMarketTrade.amount, 1e-5f);
            secondMarketTrade = balanceTrades[1][1];
            Assert.AreEqual(TestItemType.Corn, secondMarketTrade.type);
            Assert.AreEqual(1, secondMarketTrade.amount, 1e-5f);
        }

        [TestMethod]
        public void ShouldEmitFloatingPointTrade()
        {
            var inventories = new[]
            {
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 5f)
                }, 100),
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 10f)
                }, 100)
            };

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(inventories, new[] { TestItemType.Cactus });

            Assert.AreEqual(2, balanceTrades.Length);

            Assert.AreEqual(1, balanceTrades[0].Length);
            var firstMarketTrade = balanceTrades[0][0];
            Assert.AreEqual(TestItemType.Cactus, firstMarketTrade.type);
            Assert.AreEqual(2.5f, firstMarketTrade.amount, 1e-5f);

            Assert.AreEqual(1, balanceTrades[1].Length);
            var secondMarketTrade = balanceTrades[1][0];
            Assert.AreEqual(TestItemType.Cactus, secondMarketTrade.type);
            Assert.AreEqual(-2.5f, secondMarketTrade.amount, 1e-5f);
        }
        [TestMethod]
        public void ShouldEmitFloatingPointTradeAcrossMany()
        {
            var inventories = new[]
            {
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 5f)
                }, 100),
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 10f)
                }, 100),
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 4f)
                }, 100)
            };

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(inventories, new[] { TestItemType.Cactus });

            Assert.AreEqual(3, balanceTrades.Length);

            var expectedTradeAmounts = new[]
            {
                1.333333f,
                -3.666666f,
                2.333333f
            };

            for (var tradeIndex = 0; tradeIndex < balanceTrades.Length; tradeIndex++)
            {
                var trades = balanceTrades[tradeIndex];
                Assert.AreEqual(1, trades.Length);
                var trade = trades[0];
                Assert.AreEqual(TestItemType.Cactus, trade.type);
                Assert.AreEqual(expectedTradeAmounts[tradeIndex], trade.amount, 1e-5f);
            }
        }

        [TestMethod]
        public void ShouldEmitIntegerTrade()
        {
            var inventories = new[]
            {
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 5f)
                }, 100),
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 10f)
                }, 100)
            };

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(inventories, new[] { TestItemType.Cactus }, true);

            Assert.AreEqual(2, balanceTrades.Length);

            Assert.AreEqual(1, balanceTrades[0].Length);
            var firstMarketTrade = balanceTrades[0][0];
            Assert.AreEqual(TestItemType.Cactus, firstMarketTrade.type);
            Assert.AreEqual(2f, firstMarketTrade.amount, 1e-5f);

            Assert.AreEqual(1, balanceTrades[1].Length);
            var secondMarketTrade = balanceTrades[1][0];
            Assert.AreEqual(TestItemType.Cactus, secondMarketTrade.type);
            Assert.AreEqual(-2f, secondMarketTrade.amount, 1e-5f);
        }

        [TestMethod]
        public void ShouldEmitIntegerTradeAcrossMany()
        {
            var inventories = new[]
            {
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 5f)
                }, 100),
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 10f)
                }, 100),
                EconomicsTestUtilities.CreateInventory(new []
                {
                    (TestItemType.Cactus, 4f)
                }, 100)
            };

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(inventories, new[] { TestItemType.Cactus }, true);

            Assert.AreEqual(3, balanceTrades.Length);

            var expectedTradeAmounts = new[]
            {
                1f,
                -3f,
                2f
            };

            for (var tradeIndex = 0; tradeIndex < balanceTrades.Length; tradeIndex++)
            {
                var trades = balanceTrades[tradeIndex];
                Assert.AreEqual(1, trades.Length);
                var trade = trades[0];
                Assert.AreEqual(TestItemType.Cactus, trade.type);
                Assert.AreEqual(expectedTradeAmounts[tradeIndex], trade.amount, 1e-5f);
            }
        }
    }
}
