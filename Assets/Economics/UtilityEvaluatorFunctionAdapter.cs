using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{
    class UtilityEvaluatorFunctionAdapter : IUtilityEvaluator
    {
        private IUtilityFunction utilityFunction;
        private Func<float> getAmount;
        public UtilityEvaluatorFunctionAdapter(IUtilityFunction utilityFunction, Func<float> getAmount)
        {
            this.utilityFunction = utilityFunction;
            this.getAmount = getAmount;
        }

        public float GetIncrementalUtility(float increment)
        {
            return this.utilityFunction.GetIncrementalUtility(this.getAmount(), increment);
        }
    }
}
