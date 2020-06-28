using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TradeModeling.Economics;
using TradeModeling.Functions;

namespace UnitTests.Economics.UtilityAnalyzerTests
{
    [TestClass]
    public class UtilityFromLedgerTest
    {
        private ExchangeResult<TestItemType> GetExchangeResult(TestItemType type, float cost, float amount = 1)
        {
            return new ExchangeResult<TestItemType>
            {
                amount = amount,
                cost = cost,
                type = type
            };
        }

        private (ExchangeResult<TestItemType>, PurchaseOperationResult<TestItemType>) GetLedgerTransaction(
            TestItemType soldType = TestItemType.Corn,
            TestItemType boughtType = TestItemType.Cactus,
            float sellAmount = 1,
            float purchaseAmount = 1)
        {
            return
                (GetExchangeResult(soldType, 1, sellAmount)
                , new PurchaseOperationResult<TestItemType>
                {
                    exchages = new[] {
                        GetExchangeResult(boughtType, 1, purchaseAmount)
                    },
                    utilityGained = 0.5f
                });
        }

        private UtilityEvaluatorFunctionMapper<TestItemType> GetGenericUtilityFunction(float cactusWeight = 1, float cornWeight = 1, float chiliWeight = 1)
        {
            return new UtilityEvaluatorFunctionMapper<TestItemType>(new Dictionary<TestItemType, IIncrementalFunction>() {
                {TestItemType.Cactus, new InverseWeightedUtility(new []
                    {
                        new WeightedRegion(0, cactusWeight),
                    })
                },
                {TestItemType.Corn, new InverseWeightedUtility(new []
                    {
                        new WeightedRegion(0, cornWeight),
                    })
                },
                {TestItemType.Chilis, new InverseWeightedUtility(new []
                    {
                        new WeightedRegion(0, chiliWeight),
                    })
                },
            });
        }

        [TestMethod]
        public void ShouldEvaluateUtilityAtEndWithNoTransactions()
        {
            var endingInventory = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus, 1f),
                (TestItemType.Corn, 2f)
            });
            var ledger = new (ExchangeResult<TestItemType>?, PurchaseOperationResult<TestItemType>)[0];
            var utilityEvaluator = GetGenericUtilityFunction();
            var utilityAnalyzer = new UtilityAnalyzer<TestItemType>();

            var utility = utilityAnalyzer.GetTotalUtilityByInitialResource(
                new[] { TestItemType.Cactus, TestItemType.Corn },
                endingInventory,
                ledger,
                utilityEvaluator);

            var expetedUtilityPerBeginningResource = new Dictionary<TestItemType, float>() {
                { TestItemType.Cactus, 1f },
                { TestItemType.Corn, 1.5f }
            };

            foreach (var utilityPair in utility)
            {
                Assert.AreEqual(expetedUtilityPerBeginningResource[utilityPair.Key], utilityPair.Value,
                    $"Utility not equal for {Enum.GetName(typeof(TestItemType), utilityPair.Key)}.");
            }
        }
        [TestMethod]
        public void ShouldEvaluateUtilityWhenAllCornTradedForCactus()
        {
            var endingInventory = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus, 2f),
                (TestItemType.Corn, 0f)
            });
            var ledger = new (ExchangeResult<TestItemType>?, PurchaseOperationResult<TestItemType>)[] {
                GetLedgerTransaction()
                };
            var utilityEvaluator = GetGenericUtilityFunction(3, 1);
            var utilityAnalyzer = new UtilityAnalyzer<TestItemType>();

            var utility = utilityAnalyzer.GetTotalUtilityByInitialResource(
                new[] { TestItemType.Cactus, TestItemType.Corn },
                endingInventory,
                ledger,
                utilityEvaluator);

            var expetedUtilityPerBeginningResource = new Dictionary<TestItemType, float>() {
                { TestItemType.Cactus, 2.25f },
                { TestItemType.Corn, 2.25f }
            };

            foreach (var utilityPair in utility)
            {
                Assert.AreEqual(expetedUtilityPerBeginningResource[utilityPair.Key], utilityPair.Value,
                    $"Utility not equal for {Enum.GetName(typeof(TestItemType), utilityPair.Key)}.");
            }
        }
        [TestMethod]
        public void ShouldEvaluateUtilityWhenOneCornTradedForTwoCactus()
        {
            var endingInventory = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus, 2f),
                (TestItemType.Corn, 0f)
            });
            var ledger = new (ExchangeResult<TestItemType>?, PurchaseOperationResult<TestItemType>)[] {
                GetLedgerTransaction(TestItemType.Corn, TestItemType.Cactus, 1, 2)
                };
            var utilityEvaluator = GetGenericUtilityFunction(1, 1);
            var utilityAnalyzer = new UtilityAnalyzer<TestItemType>();

            var utility = utilityAnalyzer.GetTotalUtilityByInitialResource(
                new[] { TestItemType.Cactus, TestItemType.Corn },
                endingInventory,
                ledger,
                utilityEvaluator);

            var expetedUtiexpetedUtilityPerBeginningResourceity = new Dictionary<TestItemType, float>() {
                { TestItemType.Cactus, 0f },
                { TestItemType.Corn, 1.5f }
            };

            foreach (var utilityPair in utility)
            {
                Assert.AreEqual(expetedUtiexpetedUtilityPerBeginningResourceity[utilityPair.Key], utilityPair.Value,
                    $"Utility not equal for {Enum.GetName(typeof(TestItemType), utilityPair.Key)}.");
            }
        }
        [TestMethod]
        public void ShouldEvaluateUtilityWhenOneCornTradedForTwoCactusOverBaseInventory()
        {
            var endingInventory = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus, 3f),
                (TestItemType.Corn, 1f)
            });
            var ledger = new (ExchangeResult<TestItemType>?, PurchaseOperationResult<TestItemType>)[] {
                GetLedgerTransaction(TestItemType.Corn, TestItemType.Cactus, 1, 2)
                };
            var utilityEvaluator = GetGenericUtilityFunction(1, 1);
            var utilityAnalyzer = new UtilityAnalyzer<TestItemType>();

            var utility = utilityAnalyzer.GetTotalUtilityByInitialResource(
                new[] { TestItemType.Cactus, TestItemType.Corn },
                endingInventory,
                ledger,
                utilityEvaluator);

            var transferredCactusUtility = utilityEvaluator.GetTotalUtility(TestItemType.Cactus, endingInventory)
                * (2f / 3f);
            var expectedTotalCactusUtility = utilityEvaluator.GetTotalUtility(TestItemType.Cactus, endingInventory) - transferredCactusUtility;
            var expectedTotalCornUtility = utilityEvaluator.GetTotalUtility(TestItemType.Corn, endingInventory) + transferredCactusUtility;

            var expetedUtilityPerBeginningResource = new Dictionary<TestItemType, float>() {
                { TestItemType.Cactus, expectedTotalCactusUtility },
                { TestItemType.Corn, expectedTotalCornUtility }
            };
            foreach (var utilityPair in utility)
            {
                Assert.AreEqual(expetedUtilityPerBeginningResource[utilityPair.Key], utilityPair.Value,
                    $"Utility not equal for {Enum.GetName(typeof(TestItemType), utilityPair.Key)}.");
            }
        }

        [TestMethod]
        public void ShouldEvaluateUtilityWhenAllCornTradedForCactus2DifferentAmounts()
        {
            var endingInventory = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus, 6f),
                (TestItemType.Corn, 12f)
            });
            var ledger = new (ExchangeResult<TestItemType>?, PurchaseOperationResult<TestItemType>)[] {
                GetLedgerTransaction(TestItemType.Cactus, TestItemType.Corn, 1, 2),
                GetLedgerTransaction(TestItemType.Cactus, TestItemType.Corn, 1, 2),
                GetLedgerTransaction(TestItemType.Cactus, TestItemType.Corn, 1, 2),
                GetLedgerTransaction(TestItemType.Cactus, TestItemType.Corn, 1, 2),
                };
            var utilityEvaluator = GetGenericUtilityFunction(1, 1);
            var utilityAnalyzer = new UtilityAnalyzer<TestItemType>();

            var utility = utilityAnalyzer.GetTotalUtilityByInitialResource(
                new[] { TestItemType.Cactus, TestItemType.Corn },
                endingInventory,
                ledger,
                utilityEvaluator);
            var transferredCornUtility = utilityEvaluator.GetTotalUtility(TestItemType.Corn, endingInventory)
                * (8f / 12f);
            var expectedTotalCactusUtility = utilityEvaluator.GetTotalUtility(TestItemType.Cactus, endingInventory) + transferredCornUtility;
            var expectedTotalCornUtility = utilityEvaluator.GetTotalUtility(TestItemType.Corn, endingInventory) - transferredCornUtility;

            var expetedUtility = new Dictionary<TestItemType, float>() {
                { TestItemType.Cactus, expectedTotalCactusUtility },
                { TestItemType.Corn, expectedTotalCornUtility }
            };

            foreach (var utilityPair in utility)
            {
                Assert.AreEqual(expetedUtility[utilityPair.Key], utilityPair.Value,
                    $"Utility not equal for {Enum.GetName(typeof(TestItemType), utilityPair.Key)}.");
            }
        }

        [TestMethod]
        public void ShouldEvaluateUtilityWhenTradedThroughIntermediary()
        {
            var endingInventory = EconomicsTestUtilities.CreateInventory(new[]
            {
                (TestItemType.Cactus, 2f),
                (TestItemType.Chilis, 0f),
                (TestItemType.Corn, 0f)
            });
            var ledger = new (ExchangeResult<TestItemType>?, PurchaseOperationResult<TestItemType>)[] {
                GetLedgerTransaction(TestItemType.Chilis, TestItemType.Corn, 1, 1),
                GetLedgerTransaction(TestItemType.Corn, TestItemType.Cactus, 1, 1),
                };
            var utilityEvaluator = GetGenericUtilityFunction(3, 1, 1);
            var utilityAnalyzer = new UtilityAnalyzer<TestItemType>();

            var utility = utilityAnalyzer.GetTotalUtilityByInitialResource(
                new[] { TestItemType.Cactus, TestItemType.Corn, TestItemType.Chilis },
                endingInventory,
                ledger,
                utilityEvaluator);

            var expetedUtility = new Dictionary<TestItemType, float>() {
                { TestItemType.Cactus, 2.25f },
                { TestItemType.Chilis, 2.25f },
                { TestItemType.Corn, 0f }
            };

            foreach (var respource in expetedUtility.Keys)
            {
                Assert.AreEqual(expetedUtility[respource], utility[respource],
                    $"Utility not equal for {Enum.GetName(typeof(TestItemType), respource)}.");
            }
        }

        [TestMethod]
        public void ShouldCreateTransactionMapFromSellAllForCorn()
        {
            var ledger = new (ExchangeResult<TestItemType>?, PurchaseOperationResult<TestItemType>)[] {
                GetLedgerTransaction(TestItemType.Cactus, TestItemType.Corn, 1, 1),
                GetLedgerTransaction(TestItemType.Chilis, TestItemType.Corn, 2, 1),
                GetLedgerTransaction(TestItemType.Chips, TestItemType.Corn, 1, 2),
                };

            var analyzer = new UtilityAnalyzer<TestItemType>();
            var map = analyzer.GetTransactionMapFromLedger(
                new[] { TestItemType.Cactus, TestItemType.Corn, TestItemType.Chips, TestItemType.Chilis },
                ledger);

            Assert.AreEqual((-1, 1), map.GetTransactionAmounts(TestItemType.Cactus, TestItemType.Corn));
            Assert.AreEqual((-2, 1), map.GetTransactionAmounts(TestItemType.Chilis, TestItemType.Corn));
            Assert.AreEqual((-1, 2), map.GetTransactionAmounts(TestItemType.Chips, TestItemType.Corn));
        }

        [TestMethod]
        public void ShouldCreateTransactionMapFromSeveralExchanges()
        {
            var ledger = new (ExchangeResult<TestItemType>?, PurchaseOperationResult<TestItemType>)[] {
                GetLedgerTransaction(TestItemType.Cactus, TestItemType.Corn, 1, 1),
                GetLedgerTransaction(TestItemType.Chilis, TestItemType.Corn, 2, 1),
                GetLedgerTransaction(TestItemType.Corn, TestItemType.Chips, 2, 1),
                GetLedgerTransaction(TestItemType.Corn, TestItemType.Chilis, 1, 1),
                GetLedgerTransaction(TestItemType.Cactus, TestItemType.Chilis, 2, 1),

                };

            var analyzer = new UtilityAnalyzer<TestItemType>();
            var map = analyzer.GetTransactionMapFromLedger(
                new[] { TestItemType.Cactus, TestItemType.Corn, TestItemType.Chips, TestItemType.Chilis },
                ledger);

            Assert.AreEqual((-1, 1), map.GetTransactionAmounts(TestItemType.Cactus, TestItemType.Corn));
            Assert.AreEqual((-1, 0), map.GetTransactionAmounts(TestItemType.Chilis, TestItemType.Corn));
            Assert.AreEqual((1, -2), map.GetTransactionAmounts(TestItemType.Chips, TestItemType.Corn));
            Assert.AreEqual((-2, 1), map.GetTransactionAmounts(TestItemType.Cactus, TestItemType.Chilis));
        }
    }
}
