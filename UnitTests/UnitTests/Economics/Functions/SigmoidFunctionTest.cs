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
                range = 10f,
                yRange = 1f
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
        public void ShouldScaleAndOffsetCorrectlyWithYRange()
        {
            var myFunction = new SigmoidFunction(new SigmoidFunctionConfig
            {
                offset = 1f,
                range = 10f,
                yRange = 3f
            });

            var expectedValueTable = new Dictionary<float, float>
            {
                {0f, 0.997527376843365225f * 3 },

                {2f, 0.982013790037908442f * 3 },

                {4f, 0.880797077977882444f * 3 },

                {6f, 0.5f * 3 },
                {7f, 0.268941421369995120f * 3 },


                {10f, 0.01798620996209156f * 3 },
            };

            foreach (var expectedPair in expectedValueTable)
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
                range = 10f,
                yRange = 1f
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


        [TestMethod]
        public void ShouldHaveCorrectNetValuesInverse()
        {
            var myFunction = new SigmoidFunction(new SigmoidFunctionConfig
            {
                offset = 0f,
                range = 10f,
                yRange = 1f
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
                Assert.AreEqual(expectedPair.Key, myFunction.GetPointFromNetValue(expectedPair.Value), 1e-5);
            }
        }


        [TestMethod]
        public void ShouldHaveCorrectNetValuesInverseWithOffset()
        {
            var myFunction = new SigmoidFunction(new SigmoidFunctionConfig
            {
                offset = 0f,
                range = 10f,
                yRange = 1f
            });

            var offsettedValue = 0.988565420571308328;

            var expectedValueTable = new Dictionary<float, float>
            {
                {0f, (float)(0 - offsettedValue) },
                {1f, (float)(0.988565420571308328 - offsettedValue) },
                {2f, (float)(1.958127996915376009 - offsettedValue) },
                {6f, (float)(4.693453660970895236 - offsettedValue) },
                {7f, (float)(4.879787337446145572 - offsettedValue) },
                {10f, (float)(5f - offsettedValue) },
            };

            foreach (var expectedPair in expectedValueTable)
            {
                Assert.AreEqual(expectedPair.Key, myFunction.GetPointFromNetExtraValueFromPoint(expectedPair.Value, 1), 1e-5);
            }
        }


        [TestMethod]
        public void ShouldHaveCorrectNetValuesInverseWithOffsetPast0()
        {
            var myFunction = new SigmoidFunction(new SigmoidFunctionConfig
            {
                offset = 0f,
                range = 10f,
                yRange = 1f
            });

            var offsettedValue = 0.988565420571308328;

            var expectedValueTable = new Dictionary<float, float>
            {
                {0f, (float)(0 - offsettedValue) },
                {-1f, (float)(-0.99576033664861238091444299431088089366184175169765377856 - offsettedValue) },
                {-2f, (float)(-1.99419611796465617607528580607077258667289101133381732908 - offsettedValue) },
                {-6f, (float)(-5.99330135307220032534204550450772727429328160638888197171 - offsettedValue) },
                {-7f, (float)(-6.99329079570435966418901789133356611158827908472468216254 - offsettedValue) },
                {-10f, (float)(-9.99328495741315564510406920928653520521498975323882941468 - offsettedValue) },
            };

            foreach (var expectedPair in expectedValueTable)
            {
                Assert.AreEqual(expectedPair.Key, myFunction.GetPointFromNetExtraValueFromPoint(expectedPair.Value, 1), 1e-5);
            }
        }

        [TestMethod]
        public void ShouldHaveCorrectNetValuesInverseWithYRange()
        {
            var myFunction = new SigmoidFunction(new SigmoidFunctionConfig
            {
                offset = 0f,
                range = 10f,
                yRange = 4f
            });

            var expectedValueTable = new Dictionary<float, float>
            {
                {0f, 0f * 4 },
                {1f, 0.988565420571308328f * 4 },
                {2f, 1.958127996915376009f * 4 },
                {6f, 4.693453660970895236f * 4 },
                {7f, 4.879787337446145572f * 4 },
                {10f, 5f * 4 },
            };

            foreach (var expectedPair in expectedValueTable)
            {
                Assert.AreEqual(expectedPair.Key, myFunction.GetPointFromNetValue(expectedPair.Value), 1e-5);
            }
        }


        [TestMethod]
        public void ShouldHaveCorrectNetValuesInverseWhenPastLimitOfIntegral()
        {
            var myFunction = new SigmoidFunction(new SigmoidFunctionConfig
            {
                offset = 0f,
                range = 10f,
                yRange = 1f
            });

            var expectedValueTable = new Dictionary<float, float>
            {
                {float.MaxValue, (float)(100f) },
            };

            foreach (var expectedPair in expectedValueTable)
            {
                Assert.AreEqual(expectedPair.Key, myFunction.GetPointFromNetExtraValueFromPoint(expectedPair.Value, 1), 1e-5);
            }
        }
    }
}
