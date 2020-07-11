using Assets.Scripts.MovementExtensions;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Resources.InventoryDisplays
{
    //[RequireComponent(typeof(HexMember))]
    public abstract class SinglePile : MonoBehaviour
    {
        public abstract ResourceType pileType { get; }

        public abstract int capacity { get; }

        public abstract void SetResourceNumber(int newResource);
    }
}
