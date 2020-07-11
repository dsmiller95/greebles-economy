using UnityEditor.PackageManager;
using UnityEngine;

namespace Assets.Scripts.Resources.InventoryDisplays
{
    //[RequireComponent(typeof(HexMember))]
    public class WoodPile : SinglePile
    {
        public ResourceType type;
        public int maxCapacity = 10;
        public override ResourceType pileType => ResourceType.Wood;

        public override int capacity => maxCapacity;

        public void Start()
        {
            if(transform.childCount < maxCapacity)
            {
                throw new System.Exception("Must have at least as many children as capacity");
            }
        }

        public override void SetResourceNumber(int newResource)
        {
            for(var child = 0; child < maxCapacity; child++)
            {
                transform.GetChild(child).gameObject.SetActive(child < newResource);
            }
        }
    }
}
