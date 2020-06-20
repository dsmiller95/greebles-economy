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
            var tradeNodes = linkedTrader.tradeRoute.Select(node => CreateSingleTradeNode(node)).ToList();
        }

        private GameObject CreateSingleTradeNode(TradeNode node)
        {
            Debug.Log($"Creating single node with {node.targetMarket.name}");
            var newTradeNode = GameObject.Instantiate(singleTradeNodePrefab, transform);
            var singleTradeNodeScript = newTradeNode.GetComponent<SingleTradeNode>();
            singleTradeNodeScript.tradeNode = node;
            Debug.Log($"Assigned node with {node.trades.Length} trades");
            return newTradeNode;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}