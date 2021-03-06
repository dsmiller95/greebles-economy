﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeModeling.Economics;

namespace UnitTests.Economics
{
    [TestClass]
    public class CombinedTransactionModelTest
    {
        [TestMethod]
        public void ShouldModelOneTransaction()
        {
            var combinedModel = new CombinedTransactionModel<string>(new[] { "corn", "cactus"});
            combinedModel.SetTransactionValue("cactus", "corn", -1, 10);
            Assert.AreEqual((-1, 10), combinedModel.GetTransactionAmounts("cactus", "corn"));
            Assert.AreEqual((10, -1), combinedModel.GetTransactionAmounts("corn", "cactus"));
        }

        [TestMethod]
        public void ShouldModelOneTransactionAndOverwriteWhenReversed()
        {
            var combinedModel = new CombinedTransactionModel<string>(new[] { "corn", "cactus" });
            combinedModel.SetTransactionValue("cactus", "corn", -1, 10);
            Assert.AreEqual((-1, 10), combinedModel.GetTransactionAmounts("cactus", "corn"));
            Assert.AreEqual((10, -1), combinedModel.GetTransactionAmounts("corn", "cactus"));
            combinedModel.SetTransactionValue("corn", "cactus", -1, 10);
            Assert.AreEqual((10, -1), combinedModel.GetTransactionAmounts("cactus", "corn"));
            Assert.AreEqual((-1, 10), combinedModel.GetTransactionAmounts("corn", "cactus"));
        }

        [TestMethod]
        public void ShouldModelManyTransactions()
        {
            var combinedModel = new CombinedTransactionModel<string>(new[] { "corn", "cactus", "chili", "chips" });
            combinedModel.SetTransactionValue("cactus", "corn", -1, 9);
            combinedModel.SetTransactionValue("cactus", "chili", -1, 8);
            combinedModel.SetTransactionValue("chili", "corn", -1, 7);
            combinedModel.SetTransactionValue("chips", "chili", -1, 5);
            combinedModel.SetTransactionValue("chips", "corn", -1, 4);

            Assert.AreEqual((-1, 9), combinedModel.GetTransactionAmounts("cactus", "corn"));
            Assert.AreEqual((-1, 8), combinedModel.GetTransactionAmounts("cactus", "chili"));
            Assert.AreEqual((-1, 7), combinedModel.GetTransactionAmounts("chili", "corn"));
            Assert.AreEqual((-1, 5), combinedModel.GetTransactionAmounts("chips", "chili"));
            Assert.AreEqual((-1, 4), combinedModel.GetTransactionAmounts("chips", "corn"));
        }
        [TestMethod]
        public void ShouldAddManyTransactions()
        {
            var combinedModel = new CombinedTransactionModel<string>(new[] { "corn", "cactus", "chili", "chips" });
            combinedModel.AddTransaction("cactus", "corn", -1, 1);
            combinedModel.AddTransaction("chili", "corn", -2, 1);
            combinedModel.AddTransaction("chips", "corn", -1, 2);

            Assert.AreEqual((-1, 1), combinedModel.GetTransactionAmounts("cactus", "corn"));
            Assert.AreEqual((-2, 1), combinedModel.GetTransactionAmounts("chili", "corn"));
            Assert.AreEqual((-1, 2), combinedModel.GetTransactionAmounts("chips", "corn"));
        }
        [TestMethod]
        public void ShouldModelOneTransactionAndAdd()
        {
            var combinedModel = new CombinedTransactionModel<string>(new[] { "corn", "cactus" });
            combinedModel.SetTransactionValue("cactus", "corn", -2, 4);
            Assert.AreEqual((-2, 4), combinedModel.GetTransactionAmounts("cactus", "corn"));
            Assert.AreEqual((4, -2), combinedModel.GetTransactionAmounts("corn", "cactus"));
            combinedModel.AddTransaction("corn", "cactus", -1, 1);
            Assert.AreEqual((-1, 3), combinedModel.GetTransactionAmounts("cactus", "corn"));
            Assert.AreEqual((3, -1), combinedModel.GetTransactionAmounts("corn", "cactus"));
        }

        [TestMethod]
        public void ShouldModelManyTransactionsSeperatelyAndAddTogether()
        {
            var validTransactionItems = new[] { "corn", "cactus", "chili", "chips" };
            var model1 = new CombinedTransactionModel<string>(validTransactionItems);
            model1.SetTransactionValue("cactus", "corn", -1, 9);
            var model2 = new CombinedTransactionModel<string>(validTransactionItems);
            model2.SetTransactionValue("cactus", "chili", -1, 8);
            var model3 = new CombinedTransactionModel<string>(validTransactionItems);
            model3.SetTransactionValue("chili", "corn", -1, 7);
            var model4 = new CombinedTransactionModel<string>(validTransactionItems);
            model4.SetTransactionValue("chips", "chili", -1, 5);
            var model5 = new CombinedTransactionModel<string>(validTransactionItems);
            model5.SetTransactionValue("chips", "corn", -1, 4);

            var combinedModel = model1 + model2 + model3 + model4 + model5;

            Assert.AreEqual((-1, 9), combinedModel.GetTransactionAmounts("cactus", "corn"));
            Assert.AreEqual((-1, 8), combinedModel.GetTransactionAmounts("cactus", "chili"));
            Assert.AreEqual((-1, 7), combinedModel.GetTransactionAmounts("chili", "corn"));
            Assert.AreEqual((-1, 5), combinedModel.GetTransactionAmounts("chips", "chili"));
            Assert.AreEqual((-1, 4), combinedModel.GetTransactionAmounts("chips", "corn"));
        }



        [TestMethod]
        public void ShouldModelManyTransactionsAndApplyToInventoryModel()
        {
            var validTransactionItems = new[] { "corn", "cactus", "chili", "chips" };
            var combinedModel = new CombinedTransactionModel<string>(validTransactionItems);
            combinedModel.SetTransactionValue("cactus", "corn", -1, 2);
            combinedModel.SetTransactionValue("cactus", "chili", -1, 2);
            combinedModel.SetTransactionValue("chili", "corn", -1, 1);
            combinedModel.SetTransactionValue("chips", "corn", -1, 3);

            Assert.AreEqual((-1, 2), combinedModel.GetTransactionAmounts("cactus", "corn"));
            Assert.AreEqual((-1, 2), combinedModel.GetTransactionAmounts("cactus", "chili"));
            Assert.AreEqual((-1, 1), combinedModel.GetTransactionAmounts("chili", "corn"));
            Assert.AreEqual((-1, 3), combinedModel.GetTransactionAmounts("chips", "corn"));

            var inventory = new Dictionary<string, float>()
            {
                {"corn", 0f },
                {"cactus", 2f },
                {"chili", 10f },
                {"chips", 1f },
            };

            var addedInventory = inventory + combinedModel;

            var expectedInventory = new Dictionary<string, float>()
            {
                {"corn", 6f },
                {"cactus", 0f },
                {"chili", 11f },
                {"chips", 0f },
            };

            Assert.IsTrue(expectedInventory.SequenceEqual(addedInventory));

            // Subtracting the transaction should leave us with the original inventory
            var subtractedInventory = addedInventory - combinedModel;
            Assert.IsTrue(inventory.SequenceEqual(subtractedInventory));
        }
    }
}
