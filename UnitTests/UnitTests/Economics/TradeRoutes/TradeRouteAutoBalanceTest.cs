using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.TradeRouteUtilities;

namespace UnitTests.Economics.TradeRoutes
{
    [TestClass]
    public class TradeRouteAutoBalanceTest
    {
        private IEnumerable<Dictionary<TestItemType, float>> GetMaxResourceAmounts(float maxForAll, int length, IList<TestItemType> tradeItems)
        {
            for (; length > 0; length--)
            {
                yield return tradeItems.ToDictionary(x => x, x => maxForAll);
            }
        }

        [TestMethod]
        public void ShouldEmitNoTradesForSingleInventory()
        {
            var tradeableItems = new[] { TestItemType.Cactus, TestItemType.Corn };
            var distributeInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[] { (TestItemType.Cactus, 0f), (TestItemType.Corn, 0f) }, 100);
            var inventories = new[]
            {
                EconomicsTestUtilities.CreateSpaceFillingSource(new []
                {
                    (TestItemType.Cactus, 1f),
                    (TestItemType.Corn, 2f)
                }, 100)
            };

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(
                distributeInventory,
                inventories,
                GetMaxResourceAmounts(100, inventories.Length, tradeableItems).ToList(),
                tradeableItems
                );

            Assert.AreEqual(1, balanceTrades.Length);
            Assert.AreEqual(0, balanceTrades[0].Length);
        }

        [TestMethod]
        public void ShouldEmitOneTradePerInventoryForTwoImbalancedInventories()
        {
            var tradeableItems = new[] { TestItemType.Cactus, TestItemType.Corn };
            var distributeInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[] { (TestItemType.Cactus, 0f), (TestItemType.Corn, 0f) }, 100);
            var inventories = new[]
            {
                EconomicsTestUtilities.CreateSpaceFillingSource(new []
                {
                    (TestItemType.Cactus, 1f),
                    (TestItemType.Corn, 2f)
                }, 100),
                EconomicsTestUtilities.CreateSpaceFillingSource(new []
                {
                    (TestItemType.Cactus, 1f),
                    (TestItemType.Corn, 0f)
                }, 100)
            };

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(
                distributeInventory,
                inventories,
                GetMaxResourceAmounts(100, inventories.Length, tradeableItems).ToList(),
                tradeableItems);

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
            var tradeableItems = new[] { TestItemType.Cactus, TestItemType.Corn };
            var distributeInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[] { (TestItemType.Cactus, 0f), (TestItemType.Corn, 0f) }, 100);
            var inventories = new[]
            {
                EconomicsTestUtilities.CreateSpaceFillingSource(new []
                {
                    (TestItemType.Cactus, 4f),
                    (TestItemType.Corn, 2f)
                }, 100),
                EconomicsTestUtilities.CreateSpaceFillingSource(new []
                {
                    (TestItemType.Cactus, 10f),
                    (TestItemType.Corn, 0f)
                }, 100)
            };

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(
                distributeInventory,
                inventories,
                GetMaxResourceAmounts(100, inventories.Length, tradeableItems).ToList(),
                tradeableItems);

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
        public void ShouldEmitTradesAddingFromDistributeInventory()
        {
            var tradeableItems = new[] { TestItemType.Cactus, TestItemType.Corn };
            var distributeInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[]{
                (TestItemType.Cactus, 4f),
                (TestItemType.Corn, 8f),
                }, 100);
            var inventories = new[]
            {
                EconomicsTestUtilities.CreateSpaceFillingSource(new []
                {
                    (TestItemType.Cactus, 4f),
                    (TestItemType.Corn, 2f)
                }, 100),
                EconomicsTestUtilities.CreateSpaceFillingSource(new []
                {
                    (TestItemType.Cactus, 10f),
                    (TestItemType.Corn, 0f)
                }, 100)
            };

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(
                distributeInventory,
                inventories,
                GetMaxResourceAmounts(100, inventories.Length, tradeableItems).ToList(),
                tradeableItems);

            Assert.AreEqual(2, balanceTrades.Length);

            Assert.AreEqual(2, balanceTrades[0].Length);
            var firstMarketTrade = balanceTrades[0][0];
            Assert.AreEqual(TestItemType.Cactus, firstMarketTrade.type);
            Assert.AreEqual(5, firstMarketTrade.amount, 1e-5f);
            firstMarketTrade = balanceTrades[0][1];
            Assert.AreEqual(TestItemType.Corn, firstMarketTrade.type);
            Assert.AreEqual(3, firstMarketTrade.amount, 1e-5f);

            Assert.AreEqual(2, balanceTrades[1].Length);
            var secondMarketTrade = balanceTrades[1][0];
            Assert.AreEqual(TestItemType.Cactus, secondMarketTrade.type);
            Assert.AreEqual(-1, secondMarketTrade.amount, 1e-5f);
            secondMarketTrade = balanceTrades[1][1];
            Assert.AreEqual(TestItemType.Corn, secondMarketTrade.type);
            Assert.AreEqual(5, secondMarketTrade.amount, 1e-5f);
        }

        [TestMethod]
        public void ShouldEmitFloatingPointTrade()
        {
            var tradeableItems = new[] { TestItemType.Cactus };
            var distributeInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[] { (TestItemType.Cactus, 0f) }, 100);
            var inventories = new[]
            {
                EconomicsTestUtilities.CreateSpaceFillingSource(new []
                {
                    (TestItemType.Cactus, 5f)
                }, 100),
                EconomicsTestUtilities.CreateSpaceFillingSource(new []
                {
                    (TestItemType.Cactus, 10f)
                }, 100)
            };

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(
                distributeInventory,
                inventories,
                GetMaxResourceAmounts(100, inventories.Length, tradeableItems).ToList(),
                tradeableItems);

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
        public void ShouldEmitIntegerTrade()
        {
            var tradeableItems = new[] { TestItemType.Cactus };
            var distributeInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[] { (TestItemType.Cactus, 0f) }, 100);
            var inventories = new[]
            {
                EconomicsTestUtilities.CreateSpaceFillingSource(new []
                {
                    (TestItemType.Cactus, 5f)
                }, 100),
                EconomicsTestUtilities.CreateSpaceFillingSource(new []
                {
                    (TestItemType.Cactus, 10f)
                }, 100)
            };

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(
                distributeInventory,
                inventories,
                GetMaxResourceAmounts(100, inventories.Length, tradeableItems).ToList(),
                tradeableItems,
                true);

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
        public void ShouldEmitFloatingPointTradeAcrossMany()
        {
            var tradeableItems = new[] { TestItemType.Cactus };
            var distributeInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[] { (TestItemType.Cactus, 0f) }, 100);
            var inventories = new[]
                {5f, 10f, 4f }
                .Select(x => EconomicsTestUtilities.CreateSpaceFillingSource(new[]
                    {
                        (TestItemType.Cactus, x)
                    }, 100))
                .ToArray();

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(
                distributeInventory,
                inventories,
                GetMaxResourceAmounts(100, inventories.Length, tradeableItems).ToList(),
                tradeableItems);

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
        public void ShouldEmitIntegerTradeAcrossMany()
        {
            var tradeableItems = new[] { TestItemType.Cactus };
            var distributeInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[] { (TestItemType.Cactus, 0f) }, 100);
            var inventories = new[]
                {5f, 10f, 4f }
                .Select(x => EconomicsTestUtilities.CreateSpaceFillingSource(new[]
                    {
                        (TestItemType.Cactus, x)
                    }, 100))
                .ToArray();

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(
                distributeInventory,
                inventories,
                GetMaxResourceAmounts(100, inventories.Length, tradeableItems).ToList(),
                tradeableItems,
                true);

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
        [TestMethod]
        public void ShouldEmitFloatingPointTradeAcrossVeryManySmallAmounts()
        {
            var tradeableItems = new[] { TestItemType.Cactus };
            var distributeInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[] { (TestItemType.Cactus, 0f) }, 100);
            var inventories = new[]
                {10f, 4f, 4f, 4f, 4f }
                .Select(x => EconomicsTestUtilities.CreateSpaceFillingSource(new[]
                    {
                        (TestItemType.Cactus, x)
                    }, 100))
                .ToArray();

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(
                distributeInventory,
                inventories,
                GetMaxResourceAmounts(100, inventories.Length, tradeableItems).ToList(),
                tradeableItems);

            Assert.AreEqual(5, balanceTrades.Length);

            var expectedTradeAmounts = new[]
            {
                -4.8f,
                1.2f,
                1.2f,
                1.2f,
                1.2f,
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
        public void ShouldEmitIntegerTradeAcrossVeryManySmallAmounts()
        {
            var tradeableItems = new[] { TestItemType.Cactus };
            var distributeInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[] { (TestItemType.Cactus, 0f) }, 100);
            var inventories = new[]
                {10f, 4f, 4f, 4f, 4f }
                .Select(x => EconomicsTestUtilities.CreateSpaceFillingSource(new[]
                    {
                        (TestItemType.Cactus, x)
                    }, 100))
                .ToArray();

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(
                distributeInventory,
                inventories,
                GetMaxResourceAmounts(100, inventories.Length, tradeableItems).ToList(),
                tradeableItems,
                true);

            Assert.AreEqual(5, balanceTrades.Length);

            var expectedTradeAmounts = new[]
            {
                -4,
                1,
                1,
                1,
                1,
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
        public void ShouldEmitFloatingPointTradeAcrossVeryManyLargeAmounts()
        {
            var tradeableItems = new[] { TestItemType.Cactus };
            var distributeInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[] { (TestItemType.Cactus, 0f) }, 100);
            var inventories = new[]
                {4f, 10f, 10f, 10f, 10f }
                .Select(x => EconomicsTestUtilities.CreateSpaceFillingSource(new[]
                    {
                        (TestItemType.Cactus, x)
                    }, 100))
                .ToArray();

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(
                distributeInventory,
                inventories,
                GetMaxResourceAmounts(100, inventories.Length, tradeableItems).ToList(),
                tradeableItems);

            Assert.AreEqual(5, balanceTrades.Length);

            var expectedTradeAmounts = new[]
            {
                4.8f,
                -1.2f,
                -1.2f,
                -1.2f,
                -1.2f,
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
        public void ShouldEmitIntegerTradeAcrossVeryManyLargeAmounts()
        {
            var tradeableItems = new[] { TestItemType.Cactus };
            var distributeInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[] { (TestItemType.Cactus, 0f) }, 100);
            var inventories = new[]
                {4f, 10f, 10f, 10f, 10f }
                .Select(x => EconomicsTestUtilities.CreateSpaceFillingSource(new[]
                    {
                        (TestItemType.Cactus, x)
                    }, 100))
                .ToArray();

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(
                distributeInventory,
                inventories,
                GetMaxResourceAmounts(100, inventories.Length, tradeableItems).ToList(),
                tradeableItems,
                true);

            Assert.AreEqual(5, balanceTrades.Length);

            var expectedTradeAmounts = new[]
            {
                4f,
                -1f,
                -1f,
                -1f,
                -1f,
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
        public void ShouldEmitFloatingPointTradeLimitedByTargetConstraints()
        {
            var tradeableItems = new[] { TestItemType.Cactus };
            var distributeInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[] { (TestItemType.Cactus, 0f) }, 100);
            var inventories = new[]
                {4f, 10f, 10f, 10f, 10f }
                .Select(x => EconomicsTestUtilities.CreateSpaceFillingSource(new[]
                    {
                        (TestItemType.Cactus, x)
                    }, 100))
                .ToArray();

            var inventoryTargets = new[]
            {
                5f, 20f, 12f, 5f, 5f
            }.Select(x => new Dictionary<TestItemType, float> { { TestItemType.Cactus, x } }).ToList();

            var balanceTrades = TradeRouteAutoBalance.GetTradesWhichBalanceInventories(
                distributeInventory,
                inventories,
                inventoryTargets,
                tradeableItems);

            Assert.AreEqual(5, balanceTrades.Length);

            var expectedTradeAmounts = new[]
            {
                1f,
                7f,
                2f,
                -5f,
                -5f,
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
