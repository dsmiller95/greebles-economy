using Assets.Scripts.Trader;
using Assets.UI.Draggable;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace Assets.UI.TraderConfigPanel
{
    [RequireComponent(typeof(DragZone))]
    public class MaterialListDragZone : MonoBehaviour
    {
        public Scripts.Trader.TradeNode tradeNode;
        public float maxTradeAmount;

        [Header("Expander Items")]
        public GameObject singleResourceTradePrefab;
        public GameObject singleResourceTradeContainer;
        // Start is called before the first frame update
        void Start()
        {
            GetComponent<DragZone>().orderingChanged += SetOrder;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SetOrder()
        {
            tradeNode.trades = this.GetComponentsInChildren<ResourceTradePanel>().Select(x => x.trade).ToArray();
        }

        public void SetExpander(bool expanded)
        {
            if (!expanded)
            {
                foreach (Transform child in singleResourceTradeContainer.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            else
            {
                foreach (var node in tradeNode.trades)
                {
                    CreateSingleResourceTrade(node);
                }
            }
        }

        private ResourceTradePanel CreateSingleResourceTrade(ResourceTrade trade)
        {
            return ResourceTradePanel.InstantiateOnObject(
                singleResourceTradePrefab,
                singleResourceTradeContainer,
                trade,
                (int)maxTradeAmount);
        }

        public static TradeNodePanel InstantiateOnObject(
            GameObject selfPrefab,
            GameObject container,
            TradeNode node,
            float maxTradeAmount)
        {
            Debug.Log($"Creating single node with {node.target.gameObject.name}");
            var newTradeNode = GameObject.Instantiate(selfPrefab, container.transform);
            var selfScript = newTradeNode.GetComponent<TradeNodePanel>();
            selfScript.tradeNode = node;
            selfScript.maxTradeAmount = maxTradeAmount;
            Debug.Log($"Assigned node with {node.trades.Length} trades");
            return selfScript;
        }
    }
}