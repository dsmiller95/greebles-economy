using System;
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
    }
}
