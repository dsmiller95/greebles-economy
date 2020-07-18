using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.MapGen
{
    public class HaltonSequenceGenerator
    {
        private int horizontalRadix;
        private int verticalRadix;
        private int indexInSequence;

        public HaltonSequenceGenerator(int horizontalRadix, int verticalRadix, int seed)
        {
            this.horizontalRadix = horizontalRadix;
            this.verticalRadix = verticalRadix;
            indexInSequence = seed;
        }

        public Vector2 Sample(Vector2 maximum, Vector2 minimum = default)
        {
            var offset = minimum;
            var scale = maximum - minimum;
            var sample = new Vector2(
                HaltonSequence.Get(indexInSequence, horizontalRadix),
                HaltonSequence.Get(indexInSequence, verticalRadix)
                );
            indexInSequence++;

            return (sample * scale) + offset;
        }
    }
}
