using Assets.Scripts.MovementExtensions;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Resources.InventoryDisplays
{
    //[RequireComponent(typeof(HexMember))]
    public abstract class SinglePile : MonoBehaviour
    {
        public abstract ResourceType pileType { get; }
        public int capacity { get; set; }

        public abstract void SetResourceNumber(int newResource);
    }
}
