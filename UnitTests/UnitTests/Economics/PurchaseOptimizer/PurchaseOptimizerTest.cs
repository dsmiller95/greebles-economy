﻿using System;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Economics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeModeling.Functions;

namespace UnitTests.Economics
{
    [TestClass]
    public class PurchaseOptimizerTest
    {
        TestExchangeModel exchangeModel;
        TestInventoryModel selfInventory;
        TestInventoryModel marketInventory;


        [TestMethod]
        public void ShouldOptimizePurchaseWithBigBank()
        {
            selfInventory = new TestInventoryModel(new [] {
                ("wood", 0f),
                ("food", 10f)
            },
            30);
            marketInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 10f)
            },
            0);
            exchangeModel = new TestExchangeModel()
            {
                utilityFunctions = new Dictionary<string, IIncrementalFunction>
                {
                    { "wood", new InverseWeightedUtility(new WeightedRegion[] {
                        new WeightedRegion(0, 5),
                        new WeightedRegion(2, 1)
                    }) },
                    { "food", new InverseWeightedUtility(new WeightedRegion[] {
                        new WeightedRegion(0, 5),
                        new WeightedRegion(2, 1)
                    }) }
                },
                purchasePrices = new Dictionary<string, float>
                {
                    { "wood", 3 },
                    { "food", 3 },
                },
                sellPrices = new Dictionary<string, float>
                {
                    { "wood", 2 },
                    { "food", 2 },
                }
            };

            var optimizer = new PurchaseOptimizer<string, TestInventoryModel, TestInventoryModel>(
                selfInventory,
                marketInventory,
                new[] { "wood", "food" },
                exchangeModel, exchangeModel);

            var _ledger = optimizer.Optimize();

            // should buy up all of the first item, to match the amount in the second inventory
            Assert.AreEqual(10, selfInventory.Get("wood"));
            Assert.AreEqual(0, marketInventory.Get("wood"));
            Assert.AreEqual(0, selfInventory.bank);
            Assert.AreEqual(10, selfInventory.Get("food"));
            Assert.AreEqual(10, marketInventory.Get("food"));
        }

        [TestMethod]
        public void ShouldExchangeHighValueForHighUtility()
        {
            // Wood is rare and of high utility
            // Food is plentiful and of low utility
            // Selling one food will net one wood purchased
            selfInventory = new TestInventoryModel(new[] {
                ("wood", 0f),
                ("food", 10f)
            },
            0);
            marketInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 10f)
            },
            0);
            exchangeModel = new TestExchangeModel()
            {
                utilityFunctions = new Dictionary<string, IIncrementalFunction>
                {
                    { "wood", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(10, 1)
                        }) },
                    { "food", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }) }
                },
                purchasePrices = new Dictionary<string, float>
                {
                    { "wood", 3 },
                    { "food", 4 },
                },
                sellPrices = new Dictionary<string, float>
                {
                    { "wood", 2 },
                    { "food", 3 },
                }
            };

            var optimizer = new PurchaseOptimizer<string, TestInventoryModel, TestInventoryModel>(
                selfInventory,
                marketInventory,
                new[] { "wood", "food" },
                exchangeModel, exchangeModel);

            optimizer.Optimize();

            // should exchange food for wood until maximum utility is reached
            // at 2 food and 8 wood
            Assert.AreEqual(8, selfInventory.Get("wood"));
            Assert.AreEqual(2, marketInventory.Get("wood"));
            Assert.AreEqual(0, selfInventory.bank);
            Assert.AreEqual(2, selfInventory.Get("food"));
            Assert.AreEqual(18, marketInventory.Get("food"));
        }

        [TestMethod]
        public void ShouldExchangeExpensiveForCheapWhenEqualUtility()
        {
            // Wood is expensive and plentiful
            // Food is cheap and rare
            // Selling one wood will net two food purchased
            selfInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 0f)
            },
            0);
            marketInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 10f)
            },
            0);
            exchangeModel = new TestExchangeModel()
            {
                utilityFunctions = new Dictionary<string, IIncrementalFunction>
                {
                    { "wood", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }) },
                    { "food", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }) }
                },
                purchasePrices = new Dictionary<string, float>
                {
                    { "wood", 5 },
                    { "food", 2 },
                },
                sellPrices = new Dictionary<string, float>
                {
                    { "wood", 4 },
                    { "food", 1 },
                }
            };

            var optimizer = new PurchaseOptimizer<string, TestInventoryModel, TestInventoryModel>(
                selfInventory,
                marketInventory,
                new[] { "wood", "food" },
                exchangeModel, exchangeModel);

            optimizer.Optimize();

            // should exchange wood for food until maximum utility is reached
            // continuous integration of the utility function dictates that this will happen at 4.75 wood; 10.5 food
            // discrete evaluation should result with closest fit of 5 wood, 10 food
            Assert.AreEqual(5, selfInventory.Get("wood"));
            Assert.AreEqual(15, marketInventory.Get("wood"));
            Assert.AreEqual(0, selfInventory.bank);
            Assert.AreEqual(10, selfInventory.Get("food"));
            Assert.AreEqual(0, marketInventory.Get("food"));
        }


        [TestMethod]
        public void ShouldExchangeExpensiveForCheapWhenEqualUtility2()
        {
            // Wood is expensive and plentiful
            // Food is cheap and rare
            // Selling one wood will net two food purchased
            selfInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 2f)
            },
            2);
            marketInventory = new TestInventoryModel(new[] {
                ("wood", 20f),
                ("food", 20f)
            },
            5);
            exchangeModel = new TestExchangeModel()
            {
                utilityFunctions = new Dictionary<string, IIncrementalFunction>
                {
                    { "wood", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1)
                        }) },
                    { "food", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1)
                        }) }
                },
                purchasePrices = new Dictionary<string, float>
                {
                    { "wood", 2 },
                    { "food", 1 },
                },
                sellPrices = new Dictionary<string, float>
                {
                    { "wood", 2 },
                    { "food", 1 },
                }
            };

            var optimizer = new PurchaseOptimizer<string, TestInventoryModel, TestInventoryModel>(
                selfInventory,
                marketInventory,
                new[] { "wood", "food" },
                exchangeModel, exchangeModel);

            optimizer.Optimize();

            // should exchange wood for food until maximum utility is reached
            // at 6 wood at 12 food; selling one more wood for two food will net a negative utility
            Assert.AreEqual(12, selfInventory.Get("food"));
            Assert.AreEqual(6, selfInventory.Get("wood"));

            Assert.AreEqual(0, selfInventory.bank);

            Assert.AreEqual(10, marketInventory.Get("food"));
            Assert.AreEqual(24, marketInventory.Get("wood"));
        }

        [TestMethod]
        public void ShouldExchangeExpensiveForCheapWhenEqualUtilityLimitedByMarketInventory()
        {
            // Wood is expensive and plentiful
            // Food is cheap and rare
            // Selling one wood will net two food purchased
            // but the market only has 7 food
            selfInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 0f)
            },
            0);
            marketInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 7f)
            },
            0);
            exchangeModel = new TestExchangeModel()
            {
                utilityFunctions = new Dictionary<string, IIncrementalFunction>
                {
                    { "wood", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }) },
                    { "food", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }) }
                },
                purchasePrices = new Dictionary<string, float>
                {
                    { "wood", 5 },
                    { "food", 2 },
                },
                sellPrices = new Dictionary<string, float>
                {
                    { "wood", 4 },
                    { "food", 1 },
                }
            };

            var optimizer = new PurchaseOptimizer<string, TestInventoryModel, TestInventoryModel>(
                selfInventory,
                marketInventory,
                new[] { "wood", "food" },
                exchangeModel, exchangeModel);

            optimizer.Optimize();

            // should exchange wood for food until maximum utility is reached
            // at 6 food at 7 wood; selling one more wood will only net one food since that's all that is left in the market
            //  the utility of 7 -> 6 wood is equal to the utility of 6 -> 7 food; so no exchange should be preferred
            Assert.AreEqual(7, selfInventory.Get("wood"));
            Assert.AreEqual(13, marketInventory.Get("wood"));
            Assert.AreEqual(0, selfInventory.bank);
            Assert.AreEqual(6, selfInventory.Get("food"));
            Assert.AreEqual(1, marketInventory.Get("food"));
        }

        [TestMethod]
        public void ShouldOptimizePurchaseAndReturnPurchaseRecord()
        {
            // Wood is expensive and locally plentiful
            // Food is cheap and locally rare
            // Selling one wood will net two food purchased
            selfInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 2f)
            },
            2);
            marketInventory = new TestInventoryModel(new[] {
                ("wood", 20f),
                ("food", 20f)
            },
            5);
            exchangeModel = new TestExchangeModel()
            {
                utilityFunctions = new Dictionary<string, IIncrementalFunction>
                {
                    { "wood", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1)
                        }) },
                    { "food", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1)
                        }) }
                },
                purchasePrices = new Dictionary<string, float>
                {
                    { "wood", 2 },
                    { "food", 1 },
                },
                sellPrices = new Dictionary<string, float>
                {
                    { "wood", 2 },
                    { "food", 1 },
                }
            };

            var optimizer = new PurchaseOptimizer<string, TestInventoryModel, TestInventoryModel>(
                selfInventory,
                marketInventory,
                new[] { "wood", "food" },
                exchangeModel, exchangeModel);

            var transactionLedger = optimizer.Optimize();

            // should exchange wood for food until maximum utility is reached
            // at 6 wood at 12 food; selling one more wood for two food will net a negative utility
            Assert.AreEqual(12, selfInventory.Get("food"));
            Assert.AreEqual(6, selfInventory.Get("wood"));

            Assert.AreEqual(0, selfInventory.bank);

            Assert.AreEqual(10, marketInventory.Get("food"));
            Assert.AreEqual(24, marketInventory.Get("wood"));

            Assert.AreEqual(5, transactionLedger.Count);
            Assert.IsTrue(transactionLedger.All(transaction =>
                transaction.Item2.exchages.All(purchase => purchase.amount == 1 && purchase.cost == 1 && purchase.type == "food")
                && transaction.Item2.exchages.Count == 2
            ));
            Assert.IsTrue(transactionLedger.Skip(1).All(transaction =>
                transaction.Item1.HasValue
                && transaction.Item1.Value.amount == 1
                && transaction.Item1.Value.cost == 2
                && transaction.Item1.Value.type == "wood"
            ));
            Assert.IsTrue(transactionLedger.Select(trans => trans.Item2.utilityGained)
                .SequenceEqual(new float[] {
                    1/3f + 1/4f,
                    1/5f + 1/6f - 1/10f,
                    1/7f + 1/8f - 1/9f,
                    1/9f + 1/10f - 1/8f,
                    1/11f + 1/12f - 1/7f
                }));
        }
        
        [TestMethod]
        public void ShouldExchangeCheapForExpensiveWhenGreaterUtilityinExpensive()
        {
            // Wood is cheap and plentiful
            // Food is expensive and rare
            // Selling two wood will net one food purchased
            selfInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 0f)
            },
            0);
            marketInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 10f)
            },
            0);
            exchangeModel = new TestExchangeModel()
            {
                utilityFunctions = new Dictionary<string, IIncrementalFunction>
                {
                    { "wood", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1),
                        }) },
                    { "food", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1),
                        }) }
                },
                purchasePrices = new Dictionary<string, float>
                {
                    { "wood", 3 },
                    { "food", 4 },
                },
                sellPrices = new Dictionary<string, float>
                {
                    { "wood", 2 },
                    { "food", 3 },
                }
            };

            var optimizer = new PurchaseOptimizer<string, TestInventoryModel, TestInventoryModel>(
                selfInventory,
                marketInventory,
                new[] { "wood", "food" },
                exchangeModel, exchangeModel);

            var transactionLedger = optimizer.Optimize();

            // should exchange wood for food until maximum utility is reached
            // discrete evaluation should result with closest fit of 4 wood, 3 food
            Assert.AreEqual(6, selfInventory.Get("wood"));
            Assert.AreEqual(14, marketInventory.Get("wood"));
            Assert.AreEqual(0, selfInventory.bank);
            Assert.AreEqual(2, selfInventory.Get("food"));
            Assert.AreEqual(8, marketInventory.Get("food"));

            Assert.AreEqual(2, transactionLedger.Count);
            Assert.IsTrue(transactionLedger.All(transaction =>
                transaction.Item1.HasValue
                && transaction.Item1.Value.amount == 2
                && transaction.Item1.Value.cost == 4
                && transaction.Item1.Value.type == "wood"
                && transaction.Item2.exchages.Count == 1
                && transaction.Item2.exchages.First().type == "food"
                && transaction.Item2.exchages.First().amount == 1
                && transaction.Item2.exchages.First().cost == 4
            ));
            Assert.IsTrue(transactionLedger.Select(trans => trans.Item2.utilityGained)
                .SequenceEqual(new float[] {
                    1f - (1/10f + 1/9f),
                    1/2f - (1/8f + 1/7f),
                }));
        }
        [TestMethod]
        public void ShouldExchangeCheapForVeryExpensiveWhenGreaterUtilityinExpensive()
        {
            // Wood is cheap and plentiful
            // Food is very expensive and rare
            // Selling four wood will net one food purchased
            selfInventory = new TestInventoryModel(new[] {
                ("wood", 6f),
                ("food", 0f)
            },
            0);
            marketInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 10f)
            },
            0);
            exchangeModel = new TestExchangeModel()
            {
                utilityFunctions = new Dictionary<string, IIncrementalFunction>
                {
                    { "wood", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1),
                        }) },
                    { "food", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1),
                        }) }
                },
                purchasePrices = new Dictionary<string, float>
                {
                    { "wood", 3 },
                    { "food", 8 },
                },
                sellPrices = new Dictionary<string, float>
                {
                    { "wood", 2 },
                    { "food", 7 },
                }
            };

            var optimizer = new PurchaseOptimizer<string, TestInventoryModel, TestInventoryModel>(
                selfInventory,
                marketInventory,
                new[] { "wood", "food" },
                exchangeModel, exchangeModel);

            var transactionLedger = optimizer.Optimize();

            // should make one exchange of 4 wood for one food. [2-6] should have a total utility of 0.95
            //  but [0-1] should have a total utility of 1, giving the exchange a slight gain in utility
            Assert.AreEqual(2, selfInventory.Get("wood"));
            Assert.AreEqual(14, marketInventory.Get("wood"));
            Assert.AreEqual(0, selfInventory.bank);
            Assert.AreEqual(1, selfInventory.Get("food"));
            Assert.AreEqual(9, marketInventory.Get("food"));

            Assert.AreEqual(1, transactionLedger.Count);
        }

        [Ignore] // TODO
                 // This test fails because the optimizer will always look for the first available purchase
                 //  when attempting to sell an amount of a resource to buy something.
                 //  This causes the optimizer to always end up buying wood/water when attempting to sell water/wood;
                 //  and abort the sell attempt because it does not search other options
        [TestMethod]
        public void ShouldExchangeCheapForVeryExpensiveWhenGreaterUtilityinExpensiveWith3Resources()
        {
            // Wood is cheap and plentiful
            // Water is also cheap and plentiful
            // Food is very expensive and rare
            // Selling four wood or water will net one food purchased
            // wood and water can be exchanged for each other one-for-one without loss
            selfInventory = new TestInventoryModel(new[] {
                ("wood", 6f),
                ("water", 6f),
                ("food", 0f)
            },
            0);
            marketInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("water", 10f),
                ("food", 10f)
            },
            0);
            exchangeModel = new TestExchangeModel()
            {
                utilityFunctions = new Dictionary<string, IIncrementalFunction>
                {
                    { "wood", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1),
                        }) },
                    { "water", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1),
                        }) },
                    { "food", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1),
                        }) }
                },
                purchasePrices = new Dictionary<string, float>
                {
                    { "wood", 2 },
                    { "water", 2 },
                    { "food", 8 },
                },
                sellPrices = new Dictionary<string, float>
                {
                    { "wood", 2 },
                    { "water", 2 },
                    { "food", 7 },
                }
            };

            var optimizer = new PurchaseOptimizer<string, TestInventoryModel, TestInventoryModel>(
                selfInventory,
                marketInventory,
                new[] { "wood", "water", "food" },
                exchangeModel, exchangeModel);

            var transactionLedger = optimizer.Optimize();

            // should make one exchange of 4 wood for one food. [2-6] should have a total utility of 0.95
            //  but [0-1] should have a total utility of 1, giving the exchange a slight gain in utility
            // should also exchange water for wood or vise-versa to roughly equalize the exchange,
            //  since they can be exchanged one-for-one it should fully equalize
            Assert.AreEqual(3, transactionLedger.Count);

            Assert.AreEqual(4, selfInventory.Get("wood"));
            Assert.AreEqual(4, selfInventory.Get("water"));
            Assert.AreEqual(1, selfInventory.Get("food"));
            
            Assert.AreEqual(0, selfInventory.bank);

            Assert.AreEqual(12, marketInventory.Get("wood"));
            Assert.AreEqual(12, marketInventory.Get("water"));
            Assert.AreEqual(9, marketInventory.Get("food"));

        }

        [TestMethod]
        public void ShouldAvoidExchangeCheapForExpensiveWhenGreaterUtilityinExpensiveButTooExpensive()
        {
            // Wood is cheap and plentiful
            // Food is extremely expensive and rare
            // Wood purchase prices are also quite high -- making it such that the optimizer will not
            //      be able to purchase back any wood after comitting to or simulating a sell of wood
            // Selling 10 wood will net one food purchased
            selfInventory = new TestInventoryModel(new[] {
                ("wood", 5f),
                ("food", 0f)
            },
            0);
            marketInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 10f)
            },
            0);
            exchangeModel = new TestExchangeModel()
            {
                utilityFunctions = new Dictionary<string, IIncrementalFunction>
                {
                    { "wood", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1),
                        }) },
                    { "food", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1),
                        }) }
                },
                purchasePrices = new Dictionary<string, float>
                {
                    { "wood", 11 },
                    { "food", 20 },
                },
                sellPrices = new Dictionary<string, float>
                {
                    { "wood", 2 },
                    { "food", 19 },
                }
            };

            var optimizer = new PurchaseOptimizer<string, TestInventoryModel, TestInventoryModel>(
                selfInventory,
                marketInventory,
                new[] { "wood", "food" },
                exchangeModel, exchangeModel);

            var transactionLedger = optimizer.Optimize();

            // should not make any exchanges
            //  the optimizer should attempt to sell a bunch of wood, but stop when it realizes that there is
            //  not enough wood to buy one food
            Assert.AreEqual(5, selfInventory.Get("wood"));
            Assert.AreEqual(10, marketInventory.Get("wood"));
            Assert.AreEqual(0, selfInventory.bank);
            Assert.AreEqual(0, selfInventory.Get("food"));
            Assert.AreEqual(10, marketInventory.Get("food"));

            Assert.AreEqual(0, transactionLedger.Count);
        }

        [TestMethod]
        public void ShouldLeaveSomeMoneyWhenDecimalPointPrices()
        {
            // Wood is plentiful
            // Food is rare
            // Selling one wood will net one food purchased, plus a little extra gold
            selfInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 0f)
            },
            0);
            marketInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 10f)
            },
            0);
            exchangeModel = new TestExchangeModel()
            {
                utilityFunctions = new Dictionary<string, IIncrementalFunction>
                {
                    { "wood", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1),
                        }) },
                    { "food", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1),
                        }) }
                },
                purchasePrices = new Dictionary<string, float>
                {
                    { "wood", 3 },
                    { "food", 2 },
                },
                sellPrices = new Dictionary<string, float>
                {
                    { "wood", 2.1f },
                    { "food", 1.1f },
                }
            };

            var optimizer = new PurchaseOptimizer<string, TestInventoryModel, TestInventoryModel>(
                selfInventory,
                marketInventory,
                new[] { "wood", "food" },
                exchangeModel, exchangeModel);

            var transactionLedger = optimizer.Optimize();

            // should exchange wood for food until maximum utility is reached
            // discrete evaluation should result with closest fit of 4 wood, 3 food
            Assert.AreEqual(5, selfInventory.Get("wood"));
            Assert.AreEqual(15, marketInventory.Get("wood"));
            Assert.AreEqual(0.5f, selfInventory.bank, 1E-5);
            Assert.AreEqual(5, selfInventory.Get("food"));
            Assert.AreEqual(5, marketInventory.Get("food"));
        }

        [TestMethod]
        public void ShouldUseExtraMoneyMoneyWhenDecimalPointPrices()
        {
            // Wood is plentiful
            // Food is rare
            // Selling one wood will net one food purchased, plus a little extra gold
            selfInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 0f)
            },
            0);
            marketInventory = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 10f)
            },
            0);
            exchangeModel = new TestExchangeModel()
            {
                utilityFunctions = new Dictionary<string, IIncrementalFunction>
                {
                    { "wood", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1),
                        }) },
                    { "food", new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1),
                        }) }
                },
                purchasePrices = new Dictionary<string, float>
                {
                    { "wood", 3 },
                    { "food", 2 },
                },
                sellPrices = new Dictionary<string, float>
                {
                    { "wood", 2.4f },
                    { "food", 1.4f },
                }
            };

            var optimizer = new PurchaseOptimizer<string, TestInventoryModel, TestInventoryModel>(
                selfInventory,
                marketInventory,
                new[] { "wood", "food" },
                exchangeModel, exchangeModel);

            var transactionLedger = optimizer.Optimize();

            // should exchange wood for food until maximum utility is reached
            // discrete evaluation should result with closest fit of 4 wood, 3 food
            Assert.AreEqual(5, selfInventory.Get("wood"));
            Assert.AreEqual(15, marketInventory.Get("wood"));
            Assert.AreEqual(0f, selfInventory.bank, 1E-5);
            Assert.AreEqual(6, selfInventory.Get("food"));
            Assert.AreEqual(4, marketInventory.Get("food"));
        }

    }
}
