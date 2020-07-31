using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using TradeModeling.Inventories;

namespace UnitTests.Economics.Inventories
{
    [TestClass]
    public class InventoryTest
    {
        [TestMethod]
        public void ShouldOnlyConsumeAvailable()
        {
            var basicInventory = EconomicsTestUtilities.CreateBasicSource(new[]
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
            var basicInventorySource = EconomicsTestUtilities.CreateBasicSource(new[]
            {
                (TestItemType.Cactus,   10f),
                (TestItemType.Corn,     8f),
                (TestItemType.Pesos,    5f)
            });
            var basicInventoryTarget = EconomicsTestUtilities.CreateBasicSource(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     1f),
                (TestItemType.Pesos,    3f)
            });

            var transferOption = basicInventorySource.TransferResourceInto(TestItemType.Pesos, basicInventoryTarget, 10);

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
            var basicInventory = EconomicsTestUtilities.CreateBasicSource(new[]
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

        [TestMethod]
        public void ShouldAddUpToCapacity()
        {
            var spaceFillingInventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     3f),
                (TestItemType.Pesos,    5f)
            }, 10, new[] { TestItemType.Cactus, TestItemType.Corn });

            var addOption = spaceFillingInventory.Add(TestItemType.Cactus, 10);

            Assert.AreEqual(5, addOption.info);
            Assert.AreEqual(2, spaceFillingInventory.Get(TestItemType.Cactus));
            addOption.Execute();

            Assert.AreEqual(7, spaceFillingInventory.Get(TestItemType.Cactus));
        }

        [TestMethod]
        public void ShouldTransferBasedOnCapacity()
        {
            var spaceFillingInventorySource = EconomicsTestUtilities.CreateSpaceFillingSource(new[]
            {
                (TestItemType.Cactus,   10f),
                (TestItemType.Corn,     10f),
                (TestItemType.Pesos,    5f)
            }, 100, new[] { TestItemType.Cactus, TestItemType.Corn });
            var spaceFillingInventoryTarget = EconomicsTestUtilities.CreateSpaceFillingSource(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     3f),
                (TestItemType.Pesos,    5f)
            }, 10, new[] { TestItemType.Cactus, TestItemType.Corn });

            var transferOption = spaceFillingInventorySource.TransferResourceInto(TestItemType.Cactus, spaceFillingInventoryTarget, 10);

            Assert.AreEqual(5, transferOption.info);
            Assert.AreEqual(10, spaceFillingInventorySource.Get(TestItemType.Cactus));
            Assert.AreEqual(2, spaceFillingInventoryTarget.Get(TestItemType.Cactus));
            transferOption.Execute();

            Assert.AreEqual(5, spaceFillingInventorySource.Get(TestItemType.Cactus));
            Assert.AreEqual(7, spaceFillingInventoryTarget.Get(TestItemType.Cactus));
        }

        [TestMethod]
        public void ShouldNotTransferBasedOnCapacityWhenNotInSpacefilling()
        {
            var spaceFillingInventorySource = EconomicsTestUtilities.CreateSpaceFillingSource(new[]
            {
                (TestItemType.Cactus,   10f),
                (TestItemType.Corn,     10f),
                (TestItemType.Pesos,    5f)
            }, 100, new[] { TestItemType.Cactus, TestItemType.Corn });
            var spaceFillingInventoryTarget = EconomicsTestUtilities.CreateSpaceFillingSource(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     3f),
                (TestItemType.Pesos,    5f)
            }, 10, new[] { TestItemType.Cactus, TestItemType.Corn });

            var transferOption = spaceFillingInventorySource.TransferResourceInto(TestItemType.Pesos, spaceFillingInventoryTarget, 10);

            Assert.AreEqual(5, transferOption.info);
            Assert.AreEqual(5f, spaceFillingInventorySource.Get(TestItemType.Pesos));
            Assert.AreEqual(5f, spaceFillingInventoryTarget.Get(TestItemType.Pesos));
            transferOption.Execute();

            Assert.AreEqual(0, spaceFillingInventorySource.Get(TestItemType.Pesos));
            Assert.AreEqual(10, spaceFillingInventoryTarget.Get(TestItemType.Pesos));
        }
        [TestMethod]
        public void ShouldSetAmountBasedOnCapacityWhenPartOfSpaceFilling()
        {
            var spaceFillingInventoryTarget = EconomicsTestUtilities.CreateSpaceFillingSource(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     3f),
                (TestItemType.Pesos,    5f)
            }, 10, new[] { TestItemType.Cactus, TestItemType.Corn });

            var transferOption = spaceFillingInventoryTarget.SetAmount(TestItemType.Cactus, 10);

            Assert.AreEqual(7, transferOption.info);
            Assert.AreEqual(2, spaceFillingInventoryTarget.Get(TestItemType.Cactus));
            transferOption.Execute();

            Assert.AreEqual(7, spaceFillingInventoryTarget.Get(TestItemType.Cactus));
        }

        [TestMethod]
        public void ShouldNotSetAmountBasedOnCapacityWhenNotPartOfSpaceFilling()
        {
            var spaceFillingInventoryTarget = EconomicsTestUtilities.CreateSpaceFillingSource(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     3f),
                (TestItemType.Pesos,    5f)
            }, 10, new[] { TestItemType.Cactus, TestItemType.Corn });

            var transferOption = spaceFillingInventoryTarget.SetAmount(TestItemType.Pesos, 20);

            Assert.AreEqual(20, transferOption.info);
            Assert.AreEqual(5, spaceFillingInventoryTarget.Get(TestItemType.Pesos));
            transferOption.Execute();

            Assert.AreEqual(20, spaceFillingInventoryTarget.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void ShouldThrowErrorWhenInitialInventoryContainsInvalidOptions()
        {
            try
            {
                var spaceFillingInventoryTarget = EconomicsTestUtilities.CreateSpaceFillingSource(new[]
                    {
                    (TestItemType.Cactus,   2f),
                    (TestItemType.Corn,     3f),
                    (TestItemType.Pesos,    5f)
                },
                    10,
                    new[] { TestItemType.Cactus, TestItemType.Corn },
                    new[] { TestItemType.Cactus, TestItemType.Corn });
            }catch(Exception e)
            {
                Assert.IsTrue(e is ArgumentException);
                return;
            }
            Assert.Fail("Should not allow an inventory to be created with invalid items");
        }

        [TestMethod]
        public void ShouldNotSetAmountWhenNotPartOfValidOptions()
        {
            var spaceFillingInventoryTarget = EconomicsTestUtilities.CreateSpaceFillingSource(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     3f)
            },
            10,
            new[] { TestItemType.Cactus, TestItemType.Corn },
            new[] { TestItemType.Cactus, TestItemType.Corn });

            Assert.IsFalse(spaceFillingInventoryTarget.CanFitMoreOf(TestItemType.Pesos));
            Assert.IsTrue(spaceFillingInventoryTarget.CanFitMoreOf(TestItemType.Cactus));

            var transferOption = spaceFillingInventoryTarget.SetAmount(TestItemType.Pesos, 20);

            Assert.AreEqual(0, transferOption.info);
            Assert.AreEqual(0, spaceFillingInventoryTarget.Get(TestItemType.Pesos));
            transferOption.Execute();
            Assert.AreEqual(0, spaceFillingInventoryTarget.Get(TestItemType.Pesos));
        }

        [TestMethod]
        public void ShouldNotifyOfResourceChanges()
        {
            var inventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     3f),
                (TestItemType.Pesos,    5f)
            }, 100, new[] { TestItemType.Cactus, TestItemType.Corn });
            var notifier = new InventoryNotifier<TestItemType>(inventory, 50);

            var notifications = new Queue<ResourceChanged<TestItemType>>();
            var amountAtNotification = -1f;
            notifier.resourceAmountChanged += (sender, change) =>
            {
                notifications.Enqueue(change);
                amountAtNotification = inventory.Get(change.type);
            };

            inventory.Add(TestItemType.Cactus, 10).Execute();
            Assert.AreEqual(notifications.Count, 1);
            var notification = notifications.Dequeue();
            Assert.AreEqual(TestItemType.Cactus, notification.type);
            Assert.AreEqual(12, notification.newValue);
            Assert.AreEqual(12, amountAtNotification);
        }

        [TestMethod]
        public void ShouldNotifyOfCapacityChange()
        {
            var inventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     3f),
                (TestItemType.Pesos,    5f)
            }, 100, new[] { TestItemType.Cactus, TestItemType.Corn });
            var notifier = new InventoryNotifier<TestItemType>(inventory, 50);

            var notifications = new List<ResourceChanged<TestItemType>>();
            notifier.resourceCapacityChanges += (sender, change) =>
            {
                notifications.Add(change);
            };

            var source = inventory;
            source.inventoryCapacity = 30;

            Assert.AreEqual(notifications.Count, 3);
            Assert.AreEqual(30, notifications.Find(x => x.type == TestItemType.Cactus).newValue);
            Assert.AreEqual(30, notifications.Find(x => x.type == TestItemType.Corn).newValue);
            Assert.AreEqual(50, notifications.Find(x => x.type == TestItemType.Pesos).newValue);
        }

        [TestMethod]
        public void ShouldNotifyEverything()
        {
            var inventory = EconomicsTestUtilities.CreateSpaceFillingSource(new[]
            {
                (TestItemType.Cactus,   2f),
                (TestItemType.Corn,     3f),
                (TestItemType.Pesos,    5f)
            }, 20, new[] { TestItemType.Cactus, TestItemType.Corn });
            var notifier = new InventoryNotifier<TestItemType>(inventory, 50);


            var resourceNotifications = new List<ResourceChanged<TestItemType>>();
            notifier.resourceAmountChanged += (sender, change) =>
            {
                resourceNotifications.Add(change);
            };
            var capacityNotifications = new List<ResourceChanged<TestItemType>>();
            notifier.resourceCapacityChanges += (sender, change) =>
            {
                capacityNotifications.Add(change);
            };

            notifier.NotifyAll();

            Assert.AreEqual(resourceNotifications.Count, 3);
            Assert.AreEqual(2, resourceNotifications.Find(x => x.type == TestItemType.Cactus).newValue);
            Assert.AreEqual(3, resourceNotifications.Find(x => x.type == TestItemType.Corn).newValue);
            Assert.AreEqual(5, resourceNotifications.Find(x => x.type == TestItemType.Pesos).newValue);

            Assert.AreEqual(capacityNotifications.Count, 3);
            Assert.AreEqual(20, capacityNotifications.Find(x => x.type == TestItemType.Cactus).newValue);
            Assert.AreEqual(20, capacityNotifications.Find(x => x.type == TestItemType.Corn).newValue);
            Assert.AreEqual(50, capacityNotifications.Find(x => x.type == TestItemType.Pesos).newValue);
        }

    }
}
