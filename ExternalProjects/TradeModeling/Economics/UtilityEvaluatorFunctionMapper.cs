using System.Collections.Generic;
using TradeModeling.Functions;
using TradeModeling.Inventories;

namespace TradeModeling.Economics
{
    public class UtilityEvaluatorFunctionMapper<T>: IUtilityEvaluator<T, SpaceFillingInventory<T>>
    {
        private IDictionary<T, IIncrementalFunction> utilityFunctions;
        public UtilityEvaluatorFunctionMapper(IDictionary<T, IIncrementalFunction> utilityFunctions)
        {
            this.utilityFunctions = utilityFunctions;
        }

        public float GetIncrementalUtility(T type, SpaceFillingInventory<T> selfInventory, float increment)
        {
            return utilityFunctions[type].GetIncrementalValue(selfInventory.Get(type), increment);
        }

        public float GetTotalUtility(T type, SpaceFillingInventory<T> inventory)
        {
            return utilityFunctions[type].GetNetValue(inventory.Get(type));
        }
    }
}
