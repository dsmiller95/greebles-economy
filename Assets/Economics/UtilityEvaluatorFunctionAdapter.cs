using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{
    class UtilityEvaluatorFunctionAdapter //TODO : IUtilityEvaluator
    {
        private IIncrementalFunction utilityFunction;
        private Func<float> getAmount;
        public UtilityEvaluatorFunctionAdapter(IIncrementalFunction utilityFunction, Func<float> getAmount)
        {
            this.utilityFunction = utilityFunction;
            this.getAmount = getAmount;
        }

        public float GetCurrentAmount()
        {
            return this.getAmount();
        }

        public float GetIncrementalUtility(float increment, float amount)
        {
            return this.utilityFunction.GetIncrementalValue(amount, increment);
        }
    }
}
