using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Economics
{
    class UtilityEvaluatorFunctionAdapter: IUtilityEvaluator<SpaceFillingInventory<ResourceType>>
    {
        private IIncrementalFunction utilityFunction;
        private ResourceType type;
        public UtilityEvaluatorFunctionAdapter(IIncrementalFunction utilityFunction, ResourceType type)
        {
            this.utilityFunction = utilityFunction;
            this.type = type;
        }

        public float GetIncrementalUtility(SpaceFillingInventory<ResourceType> selfInventory, float increment)
        {
            return this.utilityFunction.GetIncrementalValue(selfInventory.Get(type), increment);
        }
    }
}
