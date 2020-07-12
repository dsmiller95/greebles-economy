using UnityEditor.PackageManager;
using UnityEngine;

namespace Assets.Scripts.Resources.InventoryDisplays
{

    /// <summary>
    /// Scales up the first child based on how much resource this stockpile contains
    ///     Will scale based on volume, or a cube root of the resource number
    /// </summary>
    public class ObjectScalePiler : SinglePileSingleType
    {
        public ResourceType type;
        public int maxCapacity = 10;
        public float scaleForOneItem = 1;
        private float volumePerItem;

        public override ResourceType pileType => type;

        public override int capacity => maxCapacity;

        public void Awake()
        {
            this.volumePerItem = Mathf.Pow(scaleForOneItem, 3f);
        }

        public void Start()
        {
            if(transform.childCount < 1)
            {
                throw new System.Exception("Must have at least one child to scale");
            }
        }

        public override void SetResourceNumber(int newResource)
        {
            var pileChild = transform.GetChild(0);
            if(newResource == 0)
            {
                pileChild.gameObject.SetActive(false);
                return;
            }
            pileChild.gameObject.SetActive(true);
            var targetVolume = volumePerItem * newResource;
            var newScale = Mathf.Pow(targetVolume, 1f / 3f);
            pileChild.localScale = Vector3.one * newScale;
        }
    }
}
