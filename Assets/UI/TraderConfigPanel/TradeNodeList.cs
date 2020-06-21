using Assets.Scripts.Trader;
using System.Linq;
using UnityEngine;

namespace Assets.UI.TraderConfigPanel
{
    public class TradeNodeList : MonoBehaviour
    {
        public TraderBehavior linkedTrader;
        public GameObject singleTradeNodePrefab;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log($"Node list opened with {linkedTrader?.name}");
            var largestTrade = linkedTrader.inventory.inventoryCapacity;
            var tradeNodes = linkedTrader.tradeRoute
                .Select(node => CreateSingleTradeNode(node, largestTrade))
                .ToList();
        }

        private SingleTradeNode CreateSingleTradeNode(TradeNode node, float maxTradeAmount)
        {
            return SingleTradeNode.InstantiateOnObject(
                singleTradeNodePrefab,
                this.gameObject,
                node,
                maxTradeAmount);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}