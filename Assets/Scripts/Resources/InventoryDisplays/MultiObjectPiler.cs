﻿using UnityEditor.PackageManager;
using UnityEngine;

namespace Assets.Scripts.Resources.InventoryDisplays
{
    public class MultiObjectPiler : SinglePileSingleType
    {
        public ResourceType type;
        public int maxCapacity = 10;
        public override ResourceType pileType => type;
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
