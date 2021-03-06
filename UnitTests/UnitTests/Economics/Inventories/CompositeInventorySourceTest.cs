﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeModeling.Inventories;

namespace UnitTests.Economics.Inventories
{
    [TestClass]
    public class CompositeInventorySourceTest
    {

        [TestMethod]
        public void ShouldSetItemsDirectlyInSingleChild()
        {
            var sourceInventory = EconomicsTestUtilities.CreateBasicSource(new []{
                (TestItemType.Cactus, 2f),
                (TestItemType.Corn, 3f),
                (TestItemType.Pesos, 5f)
            });
            var composite = new CompositeInventory<TestItemType>(new[] { sourceInventory }, CompositeDistributionMode.EVEN);

            Assert.AreEqual(2f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(3f, composite.Get(TestItemType.Corn));
            Assert.AreEqual(5f, composite.Get(TestItemType.Pesos));

            var option = composite.SetAmount(TestItemType.Cactus, 10f);
            Assert.AreEqual(10f, option.info);
            option.Execute();

            Assert.AreEqual(10f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(10f, sourceInventory.Get(TestItemType.Cactus));
        }


        [TestMethod]
        public void ShouldSetSizeBasedOnChildren()
        {
            var sourceInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[]{
                (TestItemType.Cactus, 2f),
                (TestItemType.Corn, 3f),
                (TestItemType.Pesos, 5f)
            }, 10, new[] { TestItemType.Cactus, TestItemType.Corn });
            var composite = new CompositeInventory<TestItemType>(new[] { sourceInventory }, CompositeDistributionMode.EVEN);

            Assert.AreEqual(2f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(3f, composite.Get(TestItemType.Corn));
            Assert.AreEqual(5f, composite.Get(TestItemType.Pesos));
            Assert.AreEqual(true, composite.CanFitMoreOf(TestItemType.Cactus));

            Assert.AreEqual(10f, composite.GetInventoryCapacity());
            Assert.AreEqual(5f, composite.totalFullSpace);
            Assert.AreEqual(5f, composite.remainingCapacity);
            Assert.AreEqual(.5f, composite.getFullRatio());

            var option = composite.SetAmount(TestItemType.Cactus, 10f);
            Assert.AreEqual(7f, option.info);
            option.Execute();

            Assert.AreEqual(7f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(false, composite.CanFitMoreOf(TestItemType.Cactus));

            Assert.AreEqual(7f, sourceInventory.Get(TestItemType.Cactus));
            Assert.AreEqual(false, sourceInventory.CanFitMoreOf(TestItemType.Cactus));
        }

        [TestMethod]
        public void ShouldFitEverythingEvenlyAcrossMultipleInventories()
        {
            var sourceInventory1 = EconomicsTestUtilities.CreateSpaceFillingSource(new[]{
                (TestItemType.Cactus, 2f),
                (TestItemType.Corn, 0f),
                (TestItemType.Pesos, 5f)
            }, 10, new[] { TestItemType.Cactus, TestItemType.Corn });
            var sourceInventory2 = EconomicsTestUtilities.CreateSpaceFillingSource(new[]{
                (TestItemType.Cactus, 0f),
                (TestItemType.Corn, 3f),
                (TestItemType.Pesos, 5f)
            }, 10, new[] { TestItemType.Cactus, TestItemType.Corn });
            var composite = new CompositeInventory<TestItemType>(new[] { sourceInventory1, sourceInventory2 }, CompositeDistributionMode.EVEN);

            Assert.AreEqual(2f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(3f, composite.Get(TestItemType.Corn));
            Assert.AreEqual(10f, composite.Get(TestItemType.Pesos));
            Assert.AreEqual(true, composite.CanFitMoreOf(TestItemType.Cactus));

            Assert.AreEqual(20f, composite.GetInventoryCapacity());
            Assert.AreEqual(5f, composite.totalFullSpace);
            Assert.AreEqual(15f, composite.remainingCapacity);
            Assert.AreEqual(.25f, composite.getFullRatio());

            // set to 10
            var option = composite.SetAmount(TestItemType.Cactus, 10f);
            Assert.AreEqual(10f, option.info);
            option.Execute();

            Assert.AreEqual(10f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(true, composite.CanFitMoreOf(TestItemType.Cactus));
            Assert.AreEqual(5f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(5f, sourceInventory2.Get(TestItemType.Cactus));

            // set to 11
            option = composite.SetAmount(TestItemType.Cactus, 11f);
            Assert.AreEqual(11f, option.info);
            option.Execute();

            Assert.AreEqual(11f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(true, composite.CanFitMoreOf(TestItemType.Cactus));
            Assert.AreEqual(5.5f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(5.5f, sourceInventory2.Get(TestItemType.Cactus));

            // try to set to 25
            option = composite.SetAmount(TestItemType.Cactus, 25f);
            // can only fit up to 17 because of the 3 Corn in one of the inventories
            Assert.AreEqual(17f, option.info);
            option.Execute();
            Assert.AreEqual(17f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(false, composite.CanFitMoreOf(TestItemType.Cactus));
            Assert.AreEqual(10f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(false, sourceInventory1.CanFitMoreOf(TestItemType.Cactus));
            Assert.AreEqual(7f, sourceInventory2.Get(TestItemType.Cactus));
            Assert.AreEqual(false, sourceInventory2.CanFitMoreOf(TestItemType.Cactus));
        }

        [TestMethod]
        public void ShouldFillInOrder()
        {
            var sourceInventory1 = EconomicsTestUtilities.CreateSpaceFillingSource(new[]{
                (TestItemType.Cactus, 2f),
                (TestItemType.Corn, 0f),
                (TestItemType.Pesos, 5f)
            }, 10, new[] { TestItemType.Cactus, TestItemType.Corn });
            var sourceInventory2 = EconomicsTestUtilities.CreateSpaceFillingSource(new[]{
                (TestItemType.Cactus, 0f),
                (TestItemType.Corn, 3f),
                (TestItemType.Pesos, 5f)
            }, 10, new[] { TestItemType.Cactus, TestItemType.Corn });
            var composite = new CompositeInventory<TestItemType>(new[] { sourceInventory1, sourceInventory2 }, CompositeDistributionMode.IN_ORDER);

            Assert.AreEqual(2f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(3f, composite.Get(TestItemType.Corn));
            Assert.AreEqual(10f, composite.Get(TestItemType.Pesos));
            Assert.AreEqual(true, composite.CanFitMoreOf(TestItemType.Cactus));

            // set to 10
            var option = composite.SetAmount(TestItemType.Cactus, 10f);
            Assert.AreEqual(10f, option.info);
            option.Execute();

            Assert.AreEqual(10f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(10f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(false, sourceInventory1.CanFitMoreOf(TestItemType.Cactus));
            Assert.AreEqual(0f, sourceInventory2.Get(TestItemType.Cactus));

            // set to 11
            option = composite.SetAmount(TestItemType.Cactus, 11f);
            Assert.AreEqual(11f, option.info);
            option.Execute();

            Assert.AreEqual(11f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(10f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(1f, sourceInventory2.Get(TestItemType.Cactus));

            // try to set to 25
            option = composite.SetAmount(TestItemType.Cactus, 25f);
            // can only fit up to 17 because of the 3 Corn in one of the inventories
            Assert.AreEqual(17f, option.info);
            option.Execute();
            Assert.AreEqual(17f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(false, composite.CanFitMoreOf(TestItemType.Cactus));
            Assert.AreEqual(10f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(false, sourceInventory1.CanFitMoreOf(TestItemType.Cactus));
            Assert.AreEqual(7f, sourceInventory2.Get(TestItemType.Cactus));
            Assert.AreEqual(false, sourceInventory2.CanFitMoreOf(TestItemType.Cactus));
        }



        [TestMethod]
        public void ShouldFitEverythingEvenlyWithStickyIntAcrossMultipleInventories()
        {
            var sourceInventory1 = EconomicsTestUtilities.CreateSpaceFillingSource(new[]{
                (TestItemType.Cactus, 0f),
                (TestItemType.Corn, 0f),
                (TestItemType.Pesos, 5f)
            }, 10, new[] { TestItemType.Cactus, TestItemType.Corn });
            var sourceInventory2 = EconomicsTestUtilities.CreateSpaceFillingSource(new[]{
                (TestItemType.Cactus, 0f),
                (TestItemType.Corn, 3f),
                (TestItemType.Pesos, 5f)
            }, 10, new[] { TestItemType.Cactus, TestItemType.Corn });
            var composite = new CompositeInventory<TestItemType>(new[] { sourceInventory1, sourceInventory2 }, CompositeDistributionMode.EVEN_SNAP_TO_INT);

            Assert.AreEqual(0f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(3f, composite.Get(TestItemType.Corn));
            Assert.AreEqual(10f, composite.Get(TestItemType.Pesos));
            Assert.AreEqual(true, composite.CanFitMoreOf(TestItemType.Cactus));

            // set to 1
            var option = composite.SetAmount(TestItemType.Cactus, 1f);
            Assert.AreEqual(1f, option.info);
            option.Execute();

            Assert.AreEqual(1f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(true, composite.CanFitMoreOf(TestItemType.Cactus));
            Assert.AreEqual(1f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(0f, sourceInventory2.Get(TestItemType.Cactus));


            // set to 10
            option = composite.SetAmount(TestItemType.Cactus, 10f);
            Assert.AreEqual(10f, option.info);
            option.Execute();

            Assert.AreEqual(10f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(true, composite.CanFitMoreOf(TestItemType.Cactus));
            Assert.AreEqual(5f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(5f, sourceInventory2.Get(TestItemType.Cactus));

            // set to 11
            option = composite.SetAmount(TestItemType.Cactus, 11f);
            Assert.AreEqual(11f, option.info);
            option.Execute();

            Assert.AreEqual(11f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(true, composite.CanFitMoreOf(TestItemType.Cactus));
            Assert.AreEqual(6f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(5f, sourceInventory2.Get(TestItemType.Cactus));

            option = composite.SetAmount(TestItemType.Cactus, 13.5f);
            Assert.AreEqual(13.5f, option.info);
            option.Execute();
            Assert.AreEqual(13.5f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(7f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(6.5f, sourceInventory2.Get(TestItemType.Cactus));

            // try to set to 25
            option = composite.SetAmount(TestItemType.Cactus, 25f);
            // can only fit up to 17 because of the 3 Corn in one of the inventories
            Assert.AreEqual(17f, option.info);
            option.Execute();
            Assert.AreEqual(17f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(false, composite.CanFitMoreOf(TestItemType.Cactus));
            Assert.AreEqual(10f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(false, sourceInventory1.CanFitMoreOf(TestItemType.Cactus));
            Assert.AreEqual(7f, sourceInventory2.Get(TestItemType.Cactus));
            Assert.AreEqual(false, sourceInventory2.CanFitMoreOf(TestItemType.Cactus));
        }

        [TestMethod]
        public void ShouldFitEverythingEvenlyWithStickyIntAcrossManyInventories()
        {
            var sourceInventory1 = EconomicsTestUtilities.CreateSpaceFillingSource(new[]{
                (TestItemType.Cactus, 0f)
            }, 10, new[] { TestItemType.Cactus });
            var sourceInventory2 = EconomicsTestUtilities.CreateSpaceFillingSource(new[]{
                (TestItemType.Cactus, 0f)
            }, 10, new[] { TestItemType.Cactus });
            var sourceInventory3 = EconomicsTestUtilities.CreateSpaceFillingSource(new[]{
                (TestItemType.Cactus, 0f)
            }, 10, new[] { TestItemType.Cactus });
            var composite = new CompositeInventory<TestItemType>(new[] { sourceInventory1, sourceInventory2, sourceInventory3 }, CompositeDistributionMode.EVEN_SNAP_TO_INT);

            Assert.AreEqual(0f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(true, composite.CanFitMoreOf(TestItemType.Cactus));

            // set to 1
            var option = composite.SetAmount(TestItemType.Cactus, 1f);
            Assert.AreEqual(1f, option.info);
            option.Execute();

            Assert.AreEqual(1f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(1f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(0f, sourceInventory2.Get(TestItemType.Cactus));
            Assert.AreEqual(0f, sourceInventory3.Get(TestItemType.Cactus));

            // set to 2.5
            option = composite.SetAmount(TestItemType.Cactus, 2.5f);
            Assert.AreEqual(2.5f, option.info);
            option.Execute();

            Assert.AreEqual(2.5f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(1f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(1f, sourceInventory2.Get(TestItemType.Cactus));
            Assert.AreEqual(.5f, sourceInventory3.Get(TestItemType.Cactus));

            // set to 14
            option = composite.SetAmount(TestItemType.Cactus, 14f);
            Assert.AreEqual(14f, option.info);
            option.Execute();

            Assert.AreEqual(14f, composite.Get(TestItemType.Cactus));
            Assert.AreEqual(5f, sourceInventory1.Get(TestItemType.Cactus));
            Assert.AreEqual(5f, sourceInventory2.Get(TestItemType.Cactus));
            Assert.AreEqual(4f, sourceInventory3.Get(TestItemType.Cactus));
        }
    }
}
