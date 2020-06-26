using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeModeling.Functions;

namespace UnitTests.Economics.UtilityAnalyzerTests
{
    [TestClass]
    public class InverseWeightedUtilityTest
    {
        [TestMethod]
        public void IncrementalUtilityAt0ShouldBe1()
        {
            var utilityFunction = new InverseWeightedUtility(new[]
                    {
                        new WeightedRegion(0, 1),
                    });
            Assert.AreEqual(1, utilityFunction.GetIncrementalValue(0, 1));
        }
        [TestMethod]
        public void IncrementalUtilityAt2ShouldBeFractional()
        {
            var utilityFunction = new InverseWeightedUtility(new[]
                    {
                        new WeightedRegion(0, 1),
                    });
            Assert.AreEqual(1f / 3f, utilityFunction.GetIncrementalValue(2, 1));
        }
        [TestMethod]
        public void TotalUtilityAt1ShouldBe1()
        {
            var utilityFunction = new InverseWeightedUtility(new[]
                    {
                        new WeightedRegion(0, 1),
                    });
            Assert.AreEqual(1f, utilityFunction.GetNetValue(1));
        }
        [TestMethod]
        public void TotalUtilityAt2ShouldBeFractionalSum()
        {
            var utilityFunction = new InverseWeightedUtility(new[]
                    {
                        new WeightedRegion(0, 1),
                    });
            Assert.AreEqual(1f + 1f / 2f, utilityFunction.GetNetValue(2));
        }
        [TestMethod]
        public void TotalUtilityAt9ShouldBeFractionalSum()
        {
            var utilityFunction = new InverseWeightedUtility(new[]
                    {
                        new WeightedRegion(0, 1),
                    });
            Assert.AreEqual(1f + 1f / 2f + 1f / 3f + 1f / 4f + 1f / 5f + 1f / 6f + 1f / 7f + 1f / 8f + 1f / 9f, utilityFunction.GetNetValue(9), 0.000001);
        }
    }
}
