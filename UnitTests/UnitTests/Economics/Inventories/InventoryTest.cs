using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Economics.Inventories
{
    [TestClass]
    public class InventoryTest
    {
        [TestMethod]
        public void ShouldOnlyConsumeAvailable()
        {
            var basicInventory = EconomicsTestUtilities.CreateBasicInventory(new[]
            {
                (TestItemType.Cactus,   10f),
                (TestItemType.Corn,     8f),
                (TestItemType.Pesos,    5f)
            });

            var consumeOption = basicInventory.Consume(TestItemType.Pesos, 8);

            Assert.AreEqual(5, consumeOption.info);
            Assert.AreEqual(5, basicInventory.Get(TestItemType.Pesos));
            consumeOption.Execute();

            Assert.AreEqual(0, basicInventory.Get(TestItemType.Pesos));
        }
        [TestMethod]
        public void ShouldOnlyTransferAvailable()
        {
            var basicInventorySource = EconomicsTestUtilities.CreateBasicInventory(new[]
            {
                (TestItemType.Cactus,   10f),
                (TestItemType.Corn,     8f),
                (TestItemType.Pesos,    5f)
            });
            var basicInventoryTarget = EconomicsTestUtilities.CreateBasicInventory(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     1f),
                (TestItemType.Pesos,    3f)
            });

            var transferOption = basicInventorySource.transferResourceInto(TestItemType.Pesos, basicInventoryTarget, 10);

            Assert.AreEqual(5, transferOption.info);
            Assert.AreEqual(5, basicInventorySource.Get(TestItemType.Pesos));
            Assert.AreEqual(3, basicInventoryTarget.Get(TestItemType.Pesos));
            transferOption.Execute();

            Assert.AreEqual(0, basicInventorySource.Get(TestItemType.Pesos));
            Assert.AreEqual(8, basicInventoryTarget.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void ShouldAddAll()
        {
            var basicInventory = EconomicsTestUtilities.CreateBasicInventory(new[]
            {
                (TestItemType.Cactus,   10f),
                (TestItemType.Corn,     8f),
                (TestItemType.Pesos,    5f)
            });

            var addOption = basicInventory.Add(TestItemType.Pesos, 10);

            Assert.AreEqual(10, addOption.info);
            Assert.AreEqual(5, basicInventory.Get(TestItemType.Pesos));
            addOption.Execute();

            Assert.AreEqual(15, basicInventory.Get(TestItemType.Pesos));
        }
    }
}
