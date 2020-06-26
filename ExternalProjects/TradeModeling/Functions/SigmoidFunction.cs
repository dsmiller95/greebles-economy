using System;
using System.Collections.Generic;
using System.Text;

namespace TradeModeling.Functions
{
    [Serializable]
    public struct SigmoidFunctionConfig
    {
        /// <summary>
        /// The range across which the sigmoid function should exhibit most of its variance
        ///     a range of [0, 1] maps to a range of [-5, 5] on the sigmoid function, when offset is 0
        /// </summary>
        public float range;
        /// <summary>
        /// The offset from the center of the range where the sigmoid function should cross 0.5
        /// </summary>
        public float offset;
    }

    public class SigmoidFunction : ISingleFunction, IIncrementalFunction
    {
        //public float range = 1;
        //public float offset = 0;

        private float realOffset;
        private float scale;

        private float integralOffset;

        public SigmoidFunction(SigmoidFunctionConfig config)
        {
            var range = config.range;
            var offset = config.offset;

            realOffset = offset + (range / 2);
            scale = 10 / range;
            integralOffset = (float)Math.Log(Math.Pow(Math.E, -scale * realOffset) + 1f) / scale;
        }

        public float GetValueAtPoint(float point)
        {
            return Sigmoid(point, realOffset, scale);

        }
        public float GetIncrementalValue(float startPoint, float increment)
        {
            return GetNetValue(startPoint + increment) - GetNetValue(startPoint);
        }

        public float GetNetValue(float startPoint)
        {
            return SigmoidIntegral(startPoint, realOffset, scale, integralOffset);
        }
        public static float Sigmoid(double value, float offset, float scale)
        {
            return (float)(1.0 / (1.0 + Math.Pow(Math.E, -(offset - value) * scale)));
        }
        public static float SigmoidIntegral(double value, float offset, float scale, float integralOffset)
        {
            return (float)(value
                + integralOffset
                - ((Math.Log(
                    1 +
                    Math.Pow(
                        Math.E,
                        (value - offset) * scale)
                    )) / scale)
                );
        }
    }
}
