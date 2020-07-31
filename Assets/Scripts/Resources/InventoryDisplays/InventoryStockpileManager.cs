using Assets.Scripts.Resources.Inventory;
using System.Collections.Generic;
using TradeModeling.Inventories;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.Resources.InventoryDisplays
{
    public class InventoryStockpileManager : MonoBehaviour
    {
        public ResourceInventory inventoryForInspector;

        // Start is called before the first frame update
        void Start()
        {
            inventoryForInspector.ResourceAmountsChangedAsObservable()
                .Subscribe(resource =>
                {
                    OnResourceAmountChanged(resource);
                }).AddTo(this);
        }

        private IEnumerable<SinglePile> GetPiles()
        {
            foreach (Transform child in transform)
            {
                if (!child.gameObject.activeSelf)
                {
                    continue;
                }
                var pile = child.gameObject.GetComponent<SinglePile>();
                if (pile == null)
                {
                    break;
                }
                yield return pile;
            }
        }

        void OnResourceAmountChanged(ResourceChanged<ResourceType> type)
        {
            var currentInventory = inventoryForInspector.backingInventory.CreateSimulatedClone() as TradingInventoryAdapter<ResourceType>;

            foreach (var pile in GetPiles())
            {
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