using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Economics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Assets.Economics.PurchaseOptimizer;

namespace UnitTests.Economics
{
    [TestClass]
    public class PurchaseOptimizerTest
    {
        IList<TestExchangeModel> inventoryModel;
        TestBank selfBank;

        [TestInitialize]
        public void SetupInventoryModel()
        {
        }

        [TestMethod]
        [Timeout(5000)]  // Milliseconds
        public void ShouldOptimizePurchaseWithBigBank()
        {
            selfBank = new TestBank { money = 30 };
            inventoryModel = new List<TestExchangeModel>
            {
                new TestExchangeModel
                {
                    selfInventory = 0,
                    selfBank = selfBank,
                    marketInventory = 10,
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }),
                    purchasePrice = 3,
                    sellPrice = 2,
                    name = "wood"
                },
                new TestExchangeModel
                {
                    selfInventory = 10,
                    selfBank = selfBank,
                    marketInventory = 10,
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }),
                    purchasePrice = 3,
                    sellPrice = 2,
                    name = "food"
                },
            };

            var optimizer = new PurchaseOptimizer(inventoryModel.Select(x => new CostUtilityAdapter
            {
                purchaser = x,
                seller = x,
                utilityFunction = x
            }));

            optimizer.Optimize(selfBank.money);

            // should buy up all of the first item, to match the amount in the second inventory
            Assert.AreEqual(10, inventoryModel[0].selfInventory);
            Assert.AreEqual(0, inventoryModel[0].marketInventory);
            Assert.AreEqual(0, selfBank.money);
            Assert.AreEqual(10, inventoryModel[1].selfInventory);
            Assert.AreEqual(10, inventoryModel[1].marketInventory);
        }

        [TestMethod]
        [Timeout(5000)]  // Milliseconds
        public void ShouldExchangeHighValueForHighUtility()
        {
            selfBank = new TestBank { money = 0 };
            // Wood is rare and of high utility
            // Food is plentiful and of low utility
            // Selling one food will net one wood purchased
            inventoryModel = new List<TestExchangeModel>
            {
                new TestExchangeModel
                {
                    selfInventory = 0,
                    selfBank = selfBank,
                    marketInventory = 10,
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(10, 1)
                        }),
                    purchasePrice = 3,
                    sellPrice = 2,
                    name = "wood"
                },
                new TestExchangeModel
                {
                    selfInventory = 10,
                    selfBank = selfBank,
                    marketInventory = 10,
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }),
                    purchasePrice = 4,
                    sellPrice = 3,
                    name = "food"
                },
            };

            var optimizer = new PurchaseOptimizer(inventoryModel.Select(x => new CostUtilityAdapter
            {
                purchaser = x,
                seller = x,
                utilityFunction = x
            }));

            optimizer.Optimize(selfBank.money);

            // should exchange food for wood until maximum utility is reached
            // at 2 food and 8 wood
            Assert.AreEqual(8, inventoryModel[0].selfInventory);
            Assert.AreEqual(2, inventoryModel[0].marketInventory);
            Assert.AreEqual(0, selfBank.money);
            Assert.AreEqual(2, inventoryModel[1].selfInventory);
            Assert.AreEqual(18, inventoryModel[1].marketInventory);
        }

        [TestMethod]
        // [Timeout(5000)]  // Milliseconds
        public void ShouldExchangeExpensiveForCheapWhenEqualUtility()
        {
            selfBank = new TestBank { money = 0 };
            // Wood is expensive and plentiful
            // Food is cheap and rare
            // Selling one wood will net two food purchased
            inventoryModel = new List<TestExchangeModel>
            {
                new TestExchangeModel
                {
                    selfInventory = 10,
                    selfBank = selfBank,
                    marketInventory = 10,
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }),
                    purchasePrice = 5,
                    sellPrice = 4,
                    name = "wood"
                },
                new TestExchangeModel
                {
                    selfInventory = 0,
                    selfBank = selfBank,
                    marketInventory = 10,
                    utilityFunction = new InverseWeightedUtility(new WeightedRegion[] {
                            new WeightedRegion(0, 5),
                            new WeightedRegion(2, 1)
                        }),
                    purchasePrice = 2,
                    sellPrice = 1,
                    name = "food"
                },
            };

            var optimizer = new PurchaseOptimizer(inventoryModel.Select(x => new CostUtilityAdapter
            {
                purchaser = x,
                seller = x,
                utilityFunction = x
            }));

            optimizer.Optimize(selfBank.money);

            // should exchange wood for food until maximum utility is reached
            // continuous integration of the utility function dictates that this will happen at 4.75 wood; 10.5 food
            // discrete evaluation should result with closest fit of 5 wood, 10 food
            Assert.AreEqual(5, inventoryModel[0].selfInventory);
            Assert.AreEqual(15, inventoryModel[0].marketInventory);
            Assert.AreEqual(0, selfBank.money);
            Assert.AreEqual(10, inventoryModel[1].selfInventory);
            Assert.AreEqual(0, inventoryModel[1].marketInventory);
        }
    }
}
