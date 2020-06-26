using TradeModeling.Functions;
using UnityEngine;

namespace Assets.UI.Plotter.Function
{
    public class InverseUtilityPlottableAdapter : MonoBehaviour, IPlottableFunction
    {
        public float increment = 1f;
        public WeightedRegion[] weightedRegions;
        public float offset = 1;
        private IIncrementalFunction utilityFunction;

        void Awake()
        {
            utilityFunction = new InverseWeightedUtility(weightedRegions, offset);
        }
        public float PlotAt(float x)
        {
            return utilityFunction.GetIncrementalValue(x, increment);
        }
    }
}