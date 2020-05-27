using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TradeModeling;

namespace UnitTests
{
    [TestClass]
    public class Utility_EnsureAllObjectsCreated
    {
        class TestClass
        {
            public TestClass(int value = 0)
            {
                this.myValue = value;
            }
            public int myValue;
        }
        [TestMethod]
        public void WhenCreatingObjectsInEmptyList()
        {
            var beginningList = new List<TestClass>();
            var resultList = Utilities.EnsureAllObjectsCreated(10, beginningList, () => new TestClass(2), obj => obj.myValue = 10);

            Assert.AreEqual(10, resultList.Count);
            Assert.IsTrue(resultList.All(x => x.myValue == 2));
        }
        [TestMethod]
        public void WhenRemovingObjectInFullList_ShouldRemove()
        {
            var beginningList = new List<TestClass>() { new TestClass(3), new TestClass(3), new TestClass(3) };
            var beginningListCopy = beginningList.Select(x => x).ToList();
            var resultList = Utilities.EnsureAllObjectsCreated(1, beginningList, () => new TestClass(2), obj => obj.myValue = 10);

            Assert.AreEqual(1, resultList.Count);
            Assert.IsTrue(resultList.All(x => x.myValue == 3));
            Assert.AreEqual(10, beginningListCopy[1].myValue);
            Assert.AreEqual(10, beginningListCopy[2].myValue);
        }
    }
}
