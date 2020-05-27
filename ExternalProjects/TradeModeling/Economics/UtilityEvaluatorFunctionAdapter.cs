using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeModeling.Inventories;

namespace TradeModeling.Economics
{
    public class UtilityEvaluatorFunctionAdapter<T>: IUtilityEvaluator<SpaceFillingInventory<T>>
    {
        private IIncrementalFunction utilityFunction;
        private T type;
        public UtilityEvaluatorFunctionAdapter(IIncrementalFunction utilityFunction, T type)
        {
            this.utilityFunction = utilityFunction;
            this.type = type;
        }

        public float GetIncrementalUtility(SpaceFillingInventory<T> selfInventory, float increment)
        {
            return this.utilityFunction.GetIncrementalValue(selfInventory.Get(type), increment);
        }
    }
}
