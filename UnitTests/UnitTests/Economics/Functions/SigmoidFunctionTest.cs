using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TradeModeling.Functions;

namespace UnitTests.Economics.Functions
{
    [TestClass]
    public class SigmoidFunctionTest
    {
        [TestMethod]
        public void ShouldScaleAndOffsetCorrectly()
        {
            var myFunction = new SigmoidFunction(new SigmoidFunctionConfig
            {
                offset = 1f,
                range = 10f
            });

            var expectedValueTable = new Dictionary<float, float>
            {
                {0f, 0.997527376843365225f },

                {2f, 0.982013790037908442f },

                {4f, 0.880797077977882444f },

                {6f, 0.5f },
                {7f, 0.268941421369995120f },


                {10f, 0.01798620996209156f },
            };

            foreach(var expectedPair in expectedValueTable)
            {
                Assert.AreEqual(expectedPair.Value, myFunction.GetValueAtPoint(expectedPair.Key), 1e-5);
            }
        }


        [TestMethod]
        public void ShouldHaveCorrectNetValues()
        {
            var myFunction = new SigmoidFunction(new SigmoidFunctionConfig
            {
                offset = 0f,
                range = 10f
            });

            var expectedValueTable = new Dictionary<float, float>
            {
                {0f, 0f },
                {1f, 0.988565420571308328f },
                {2f, 1.958127996915376009f },
                {6f, 4.693453660970895236f },
                {7f, 4.879787337446145572f },
                {10f, 5f },
            };

            foreach (var expectedPair in expectedValueTable)
            {
                Assert.AreEqual(expectedPair.Value, myFunction.GetNetValue(expectedPair.Key), 1e-5);
            }
        }
    }
}
