using Assets.MapGen.TileManagement;
using System.Threading.Tasks;
using TradeModeling.Economics;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scripts.Resources
{
    [RequireComponent(typeof(HexMember))]
    public class Resource : MonoBehaviour, IResource
    {
        public ResourceType _type => type;

        public float amount => value;

        public ResourceType type;
        public float value = 1;
        public float gatherTime = 1;

        private bool isCompletelyEaten = false;

        private int currentConsumerCount = 0;

        public async Task<bool> Eat(SpaceFillingInventory<ResourceType> inventory, float amount = -1)
        {
            ActionOption<float> eatOption;
            lock (this)
            {
                if (amount == -1)
                {
                    amount = value;
                }
                eatOption = inventory.Add(_type, amount);
                /* Determine if this resource will be completely consumed by this operation
                 * If it will be, mark as completely eaten and then get rid of the gameObject when this operation completes
                 */
                if (isCompletelyEaten)
                {
                    return false;
                }
                value -= eatOption.info;
                if (Mathf.Abs(value) <= float.Epsilon * 10)
                {
                    isCompletelyEaten = true;
                }
                currentConsumerCount++;
            }
            await Task.Delay((int)(gatherTime * 1000));
            eatOption.Execute();

            DestroyIfNoOtherConsumers();

            return true;
        }

        private void DestroyIfNoOtherConsumers()
        {
            var willDestroy = false;
            lock (this)
            {
                currentConsumerCount--;
                willDestroy = currentConsumerCount == 0 && isCompletelyEaten;
            }
            if (willDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}