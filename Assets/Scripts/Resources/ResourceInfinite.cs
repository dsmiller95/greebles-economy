using System.Threading.Tasks;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scripts.Resources
{
    public class ResourceInfinite : MonoBehaviour, IResource
    {
        public ResourceType _type => type;

        public float amount => float.MaxValue;

        public ResourceType type;
        public float gatherTime = 1;

        public async Task<bool> Eat(TradingInventoryAdapter<ResourceType> inventory, float amount = -1)
        {
            await Task.Delay((int)(gatherTime * 1000));
            if (amount == -1)
            {
                amount = float.MaxValue;
            }
            var eatenInfo = inventory.Add(_type, amount);
            eatenInfo.Execute();
            return true;
        }
    }
}