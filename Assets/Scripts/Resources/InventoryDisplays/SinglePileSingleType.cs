using System.Collections.Generic;

namespace Assets.Scripts.Resources.InventoryDisplays
{
    //[RequireComponent(typeof(HexMember))]
    public abstract class SinglePileSingleType : SinglePile
    {
        public abstract ResourceType pileType { get; }
        public override ResourceType[] pileTypes => new[] { pileType };

        public abstract void SetResourceNumber(int newResource);

        public override void SetResourceNumbers(IDictionary<ResourceType, int> newResources)
        {
            SetResourceNumber(newResources[pileType]);
        }

        private int GetResourceNumber(IDictionary<ResourceType, int> newResources)
        {
            int resourceAmount;
            if (newResources.TryGetValue(pileType, out resourceAmount))
            {
                return resourceAmount;
            }
            return 0;
        }
    }
}
