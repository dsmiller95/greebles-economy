using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Resources.InventoryDisplays
{
    [System.Serializable]
    public struct PileSet
    {
        public ResourceType type;
        public GameObject[] pilingObjects;
    }
    public class MultiDifferentObjectPiler : SinglePile
    {
        public PileSet[] pileSets;
        public int maxCapacity = 10;
        public override int capacity => maxCapacity;

        public void Awake()
        {
            _pileTypes = pileSets.Select(x => x.type).ToArray();
            pileSetGameObjects = pileSets.ToDictionary(x => x.type, x => x.pilingObjects);

        }

        private IDictionary<ResourceType, GameObject[]> pileSetGameObjects;
        private ResourceType[] _pileTypes;
        public override ResourceType[] pileTypes => _pileTypes;

        public void Start()
        {
            if (transform.childCount < maxCapacity)
            {
                throw new System.Exception("Must have at least as many children as capacity");
            }
        }

        public override void SetResourceNumbers(IDictionary<ResourceType, int> newResources)
        {
            var piledSoFar = 0;
            foreach (var nextPile in pileSetGameObjects)
            {
                var newPileAmount = piledSoFar + newResources[nextPile.Key];
                for (var pileObjectIndex = 0; pileObjectIndex < nextPile.Value.Length; pileObjectIndex++)
                {
                    var pileObject = nextPile.Value[pileObjectIndex];
                    pileObject.SetActive(piledSoFar <= pileObjectIndex && pileObjectIndex < newPileAmount);
                }

                piledSoFar = newPileAmount;
            }
        }
    }
}
