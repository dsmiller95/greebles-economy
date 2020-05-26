using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Economics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Assets.Economics.PurchaseOptimizer<UnitTests.Economics.TestInventoryModel, UnitTests.Economics.TestInventoryModel>;

namespace UnitTests.Economics
{
    [TestClass]
    public class PurchaseOptimizerTest
    {
        IList<TestExchangeModel> exchangeModel;
        TestInventoryModel selfInventory;
        TestInventoryModel marketInventory;

        [TestInitialize]
        public void SetupInventoryModel()
        {
        }

        [TestMethod]
        [Timeout(5000)]  // Milliseconds
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
            exchangeModel = new List<TestExchangeModel>
            {
                new TestExchangeModel
                {
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }),
                    purchasePrice = 3,
                    sellPrice = 2,
                    resourceType = "wood"
                },
                new TestExchangeModel
                {
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }),
                    purchasePrice = 3,
                    sellPrice = 2,
                    resourceType = "food"
                },
            };

            var optimizer = new PurchaseOptimizer<TestInventoryModel, TestInventoryModel>(exchangeModel.Select(x => new ExchangeAdapter
            {
                purchaser = x,
                seller = x,
                utilityFunction = x
            }), selfInventory, marketInventory);

            optimizer.Optimize();

            // should buy up all of the first item, to match the amount in the second inventory
            Assert.AreEqual(10, selfInventory.Get("wood"));
            Assert.AreEqual(0, marketInventory.Get("wood"));
            Assert.AreEqual(0, selfInventory.bank);
            Assert.AreEqual(10, selfInventory.Get("food"));
            Assert.AreEqual(10, marketInventory.Get("food"));
        }

        [TestMethod]
        [Timeout(5000)]  // Milliseconds
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
            exchangeModel = new List<TestExchangeModel>
            {
                new TestExchangeModel
                {
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(10, 1)
                        }),
                    purchasePrice = 3,
                    sellPrice = 2,
                    resourceType = "wood"
                },
                new TestExchangeModel
                {
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }),
                    purchasePrice = 4,
                    sellPrice = 3,
                    resourceType = "food",
                },
            };

            var optimizer = new PurchaseOptimizer<TestInventoryModel, TestInventoryModel>(exchangeModel.Select(x => new ExchangeAdapter
            {
                purchaser = x,
                seller = x,
                utilityFunction = x
            }), selfInventory, marketInventory);

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
        [Timeout(5000)]  // Milliseconds
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
            exchangeModel = new List<TestExchangeModel>
            {
                new TestExchangeModel
                {
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }),
                    purchasePrice = 5,
                    sellPrice = 4,
                    resourceType = "wood"
                },
                new TestExchangeModel
                {
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }),
                    purchasePrice = 2,
                    sellPrice = 1,
                    resourceType = "food"
                },
            };

            var optimizer = new PurchaseOptimizer<TestInventoryModel, TestInventoryModel>(exchangeModel.Select(x => new ExchangeAdapter
            {
                purchaser = x,
                seller = x,
                utilityFunction = x
            }), selfInventory, marketInventory);

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
        // [Timeout(5000)]  // Milliseconds
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
            exchangeModel = new List<TestExchangeModel>
            {
                new TestExchangeModel
                {
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1)
                        }),
                    purchasePrice = 2,
                    sellPrice = 2,
                    resourceType = "wood"
                },
                new TestExchangeModel
                {
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 1)
                        }),
                    purchasePrice = 1,
                    sellPrice = 1,
                    resourceType = "food"
                },
            };

            var optimizer = new PurchaseOptimizer<TestInventoryModel, TestInventoryModel>(exchangeModel.Select(x => new ExchangeAdapter
            {
                purchaser = x,
                seller = x,
                utilityFunction = x
            }), selfInventory, marketInventory);

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
        [Timeout(5000)]  // Milliseconds
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
            exchangeModel = new List<TestExchangeModel>
            {
                new TestExchangeModel
                {
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }),
                    purchasePrice = 5,
                    sellPrice = 4,
                    resourceType = "wood"
                },
                new TestExchangeModel
                {
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }),
                    purchasePrice = 2,
                    sellPrice = 1,
                    resourceType = "food"
                },
            };

            var optimizer = new PurchaseOptimizer<TestInventoryModel, TestInventoryModel>(exchangeModel.Select(x => new ExchangeAdapter
            {
                purchaser = x,
                seller = x,
                utilityFunction = x
            }), selfInventory, marketInventory);

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
    }
}
