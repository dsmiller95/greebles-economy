using UnityEngine;

namespace Assets.Scripts.Resources.InventoryDisplays
{
    //[RequireComponent(typeof(HexMember))]
    public class WoodPile : SinglePile
    {
        public GameObject woodModel;
        public float pileOffset;

        public override ResourceType pileType => ResourceType.Wood;

        public override void SetResourceNumber(int newResource)
        {
            int childCount;
            while ((childCount = transform.childCount) < newResource)
            {
                var offset = Vector3.up * pileOffset * childCount;
                var newModel = Instantiate(woodModel, transform);
                newModel.transform.localPosition = offset;
            }
            if ((childCount = transform.childCount) > newResource)
            {
                int count = 0;
                for (var removedChild = newResource; removedChild < childCount; removedChild++)
                {
                    Destroy(transform.GetChild(removedChild).gameObject);
                    count++;
                    if (count > 1000)
                    {
                        throw new System.Exception("Too many l00p br0ther");
                    }
                }
            }
        }
    }
}
