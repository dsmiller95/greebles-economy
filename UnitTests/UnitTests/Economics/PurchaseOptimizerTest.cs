using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Economics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Assets.Economics.PurchaseOptimizer<UnitTests.Economics.TestInventoryModel>;

namespace UnitTests.Economics
{
    [TestClass]
    public class PurchaseOptimizerTest
    {
        IList<TestExchangeModel> exchangeModel;
        TestInventoryModel inventoryModel;

        [TestInitialize]
        public void SetupInventoryModel()
        {
        }

        [TestMethod]
        [Timeout(5000)]  // Milliseconds
        public void ShouldOptimizePurchaseWithBigBank()
        {
            inventoryModel = new TestInventoryModel(new [] {
                ("wood", 0f),
                ("food", 10f)
            }, new[] {
                ("wood", 10f),
                ("food", 10f)
            },
            30);
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

            var optimizer = new PurchaseOptimizer<TestInventoryModel>(exchangeModel.Select(x => new ExchangeAdapter
            {
                purchaser = x,
                seller = x,
                utilityFunction = x
            }), inventoryModel);

            optimizer.Optimize(inventoryModel.selfBank);

            // should buy up all of the first item, to match the amount in the second inventory
            Assert.AreEqual(10, inventoryModel.GetSelf("wood"));
            Assert.AreEqual(0, inventoryModel.GetMarket("wood"));
            Assert.AreEqual(0, inventoryModel.selfBank);
            Assert.AreEqual(10, inventoryModel.GetSelf("food"));
            Assert.AreEqual(10, inventoryModel.GetMarket("food"));
        }

        [TestMethod]
        [Timeout(5000)]  // Milliseconds
        public void ShouldExchangeHighValueForHighUtility()
        {
            // Wood is rare and of high utility
            // Food is plentiful and of low utility
            // Selling one food will net one wood purchased
            inventoryModel = new TestInventoryModel(new[] {
                ("wood", 0f),
                ("food", 10f)
            }, new[] {
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

            var optimizer = new PurchaseOptimizer<TestInventoryModel>(exchangeModel.Select(x => new ExchangeAdapter
            {
                purchaser = x,
                seller = x,
                utilityFunction = x
            }), inventoryModel);

            optimizer.Optimize(inventoryModel.selfBank);

            // should exchange food for wood until maximum utility is reached
            // at 2 food and 8 wood
            Assert.AreEqual(8, inventoryModel.GetSelf("wood"));
            Assert.AreEqual(2, inventoryModel.GetMarket("wood"));
            Assert.AreEqual(0, inventoryModel.selfBank);
            Assert.AreEqual(2, inventoryModel.GetSelf("food"));
            Assert.AreEqual(18, inventoryModel.GetMarket("food"));
        }

        [TestMethod]
        [Timeout(5000)]  // Milliseconds
        public void ShouldExchangeExpensiveForCheapWhenEqualUtility()
        {
            // Wood is expensive and plentiful
            // Food is cheap and rare
            // Selling one wood will net two food purchased
            inventoryModel = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 0f)
            }, new[] {
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

            var optimizer = new PurchaseOptimizer<TestInventoryModel>(exchangeModel.Select(x => new ExchangeAdapter
            {
                purchaser = x,
                seller = x,
                utilityFunction = x
            }), inventoryModel);

            optimizer.Optimize(inventoryModel.selfBank);

            // should exchange wood for food until maximum utility is reached
            // continuous integration of the utility function dictates that this will happen at 4.75 wood; 10.5 food
            // discrete evaluation should result with closest fit of 5 wood, 10 food
            Assert.AreEqual(5, inventoryModel.GetSelf("wood"));
            Assert.AreEqual(15, inventoryModel.GetMarket("wood"));
            Assert.AreEqual(0, inventoryModel.selfBank);
            Assert.AreEqual(10, inventoryModel.GetSelf("food"));
            Assert.AreEqual(0, inventoryModel.GetMarket("food"));
        }

        [TestMethod]
        [Timeout(5000)]  // Milliseconds
        public void ShouldExchangeExpensiveForCheapWhenEqualUtilityLimitedByMarketInventory()
        {
            // Wood is expensive and plentiful
            // Food is cheap and rare
            // Selling one wood will net two food purchased
            // but the market only has 7 food
            inventoryModel = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 0f)
            }, new[] {
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

            var optimizer = new PurchaseOptimizer<TestInventoryModel>(exchangeModel.Select(x => new ExchangeAdapter
            {
                purchaser = x,
                seller = x,
                utilityFunction = x
            }), inventoryModel);

            optimizer.Optimize(inventoryModel.selfBank);

            // should exchange wood for food until maximum utility is reached
            // at 6 food at 7 wood; selling one more wood will only net one food since that's all that is left in the market
            //  the utility of 7 -> 6 wood is equal to the utility of 6 -> 7 food; so no exchange should be preferred
            Assert.AreEqual(7, inventoryModel.GetSelf("wood"));
            Assert.AreEqual(13, inventoryModel.GetMarket("wood"));
            Assert.AreEqual(0, inventoryModel.selfBank);
            Assert.AreEqual(6, inventoryModel.GetSelf("food"));
            Assert.AreEqual(1, inventoryModel.GetMarket("food"));
        }
        [TestMethod]
        [Timeout(5000)]  // Milliseconds
        public void ShouldExchangeExpensiveForCheapWhenEqualUtilityLimitedBySelfInventoryCapacity()
        {
            // Wood is expensive and plentiful
            // Food is cheap and rare
            // Selling one wood will net two food purchased
            // but self only has room for 7 food
            inventoryModel = new TestInventoryModel(new[] {
                ("wood", 10f),
                ("food", 0f)
            }, new[] {
                ("wood", 10f),
                ("food", 10f)
            },
            0);
            exchangeModel = new List<TestExchangeModel>
            {
                new TestExchangeModel
                {
                    selfInventoryCapacity = 10,
                    marketInventoryCapacity = 20,
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
                    selfInventoryCapacity = 7,
                    marketInventoryCapacity = 20,
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }),
                    purchasePrice = 2,
                    sellPrice = 1,
                    resourceType = "food"
                },
            };

            var optimizer = new PurchaseOptimizer<TestInventoryModel>(exchangeModel.Select(x => new ExchangeAdapter
            {
                purchaser = x,
                seller = x,
                utilityFunction = x
            }), inventoryModel);

            optimizer.Optimize(inventoryModel.selfBank);

            // should exchange wood for food until maximum utility is reached
            // at 6 food at 7 wood; selling one more wood will only net one food since that's all that is left in the market
            //  the utility of 7 -> 6 wood is equal to the utility of 6 -> 7 food; so no exchange should be preferred
            Assert.AreEqual(7, inventoryModel.GetSelf("wood"));
            Assert.AreEqual(13, inventoryModel.GetMarket("wood"));
            Assert.AreEqual(0, inventoryModel.selfBank);
            Assert.AreEqual(6, inventoryModel.GetSelf("food"));
            Assert.AreEqual(1, inventoryModel.GetMarket("food"));
        }
    }
}
