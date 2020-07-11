using Assets.Scripts.MovementExtensions;
using Assets.Scripts.Resources.Inventory;
using Simulation.Tiling;
using System;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Inventories;
using UnityEngine;

namespace Assets.Scripts.Resources.InventoryDisplays
{
    [Serializable]
    public struct PileConfig
    {
        public GameObject pilePrefab;
        public AxialCoordinate location;
    }

    public class InventoryStockpileManager : MonoBehaviour
    {
        public PileConfig[] pileLocations;

        //private SinglePile[] piles;
        private Dictionary<ResourceType, IList<SinglePile>> piles;

        public ResourceInventory inventoryForInspector;
        //public HexMember

        internal NotifyingInventory<ResourceType> inventory;

        // Start is called before the first frame update
        void Start()
        {
            this.inventory = inventoryForInspector.backingInventory;
            piles = new Dictionary<ResourceType, IList<SinglePile>>();
            foreach(var pile in pileLocations
                .Select(x => CreateNewPile(x)))
            {
                IList<SinglePile> list;
                if(!piles.TryGetValue(pile.pileType, out list))
                {
                    list = new List<SinglePile>();
                    piles[pile.pileType] = list;
                }

                list.Add(pile);
            }

            inventory.resourceAmountChanges += OnResourceAmountChanged;
        }

        void OnResourceAmountChanged(object sender, ResourceChanged<ResourceType> type)
        {
            IList<SinglePile> pileList;
            if(!piles.TryGetValue(type.type, out pileList))
            {
                return;
            }
            int remainingAmount = (int)type.newValue;
            foreach(var pile in pileList)
            {
                var amountForPile = Math.Min(pile.capacity, remainingAmount);
                pile.SetResourceNumber(amountForPile);
                // when remainingAmount hits 0, just keep going and set the resource number to 0 for ever other pile
                remainingAmount -= amountForPile;
            }
        }

        private SinglePile CreateNewPile(PileConfig coordinate)
        {
            var newObject = Instantiate(coordinate.pilePrefab, transform);
            var hexMember = newObject.GetComponent<HexMember>();
            hexMember.localPosition = coordinate.location;

            var pile = newObject.GetComponent<SinglePile>();
            return pile;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}