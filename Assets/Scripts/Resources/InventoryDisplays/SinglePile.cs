using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Resources.InventoryDisplays
{
    public abstract class SinglePile : MonoBehaviour
    {
        public abstract ResourceType[] pileTypes { get; }

        public abstract int capacity { get; }

        public abstract void SetResourceNumbers(IDictionary<ResourceType, int> newResources);
    }
}
