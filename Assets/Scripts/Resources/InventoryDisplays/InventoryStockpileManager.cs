using Assets.Scripts.MovementExtensions;
using Assets.Scripts.Resources.Inventory;
using Simulation.Tiling;
using System;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Inventories;
using UnityEngine;
using UniRx;
using UniRx.Triggers; // need UniRx.Triggers namespace for extend gameObejct

namespace Assets.Scripts.Resources.InventoryDisplays
{
    public class InventoryStockpileManager : MonoBehaviour
    {
        private IList<SinglePile> piles;

        public ResourceInventory inventoryForInspector;

        // Start is called before the first frame update
        void Start()
        {
            //this.inventory = inventoryForInspector.backingInventory;
            piles = new List<SinglePile>();
            foreach(Transform child in transform)
            {
                var pile = child.gameObject.GetComponent<SinglePile>();
                if(pile == null)
                {
                    break;
                }
                piles.Add(pile);
            }

            inventoryForInspector.ResourceAmountsChangedAsObservable()
                .Subscribe(resource =>
                {
                    this.OnResourceAmountChanged(resource);
                }).AddTo(this);
        }

        void OnResourceAmountChanged(ResourceChanged<ResourceType> type)
        {
            var currentInventory = inventoryForInspector.backingInventory.CreateSimulatedClone() as SpaceFillingInventory<ResourceType>;

            foreach (var pile in piles) {
                var piledAmounts = new Dictionary<ResourceType, int>();

                float amountPiledSoFar = 0;
                foreach (var resource in pile.pileTypes)
                {
                    var consumeAttempt = currentInventory.Consume(resource, pile.capacity - amountPiledSoFar);
                    consumeAttempt.Execute();
                    amountPiledSoFar += consumeAttempt.info;

                    piledAmounts[resource] = (int)consumeAttempt.info;
                }
                pile.SetResourceNumbers(piledAmounts);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}