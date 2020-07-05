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

        private Vector2 offset;
        private Vector2 scale;

        public HaltonSequenceGenerator(int horizontalRadix, int verticalRadix, int seed, Vector2 maximum, Vector2 minimum = default)
        {
            this.horizontalRadix = horizontalRadix;
            this.verticalRadix = verticalRadix;
            indexInSequence = seed;

            this.offset = minimum;
            this.scale = maximum - minimum;
        }

        public Vector2 Sample()
        {
            var sample = new Vector2(
                HaltonSequence.Get(indexInSequence, horizontalRadix),
                HaltonSequence.Get(indexInSequence, verticalRadix)
                );
            indexInSequence++;

            return sample * scale - offset;
        }
    }
}
