using Assets.Scripts.Trader;
using Assets.UI.InfoPane;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.UI.TraderConfigPanel
{
    public class TradeNodeList : MonoBehaviour
    {
        public TraderBehavior linkedTrader;
        public GameObject singleTradeNodePrefab;

        private UIElementSeriesBuilder uiBuilder;
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log($"Node list opened with {this.linkedTrader?.name}");
            uiBuilder = new UIElementSeriesBuilder(this.gameObject);

            var tradeNodes = linkedTrader.tradeRoute.Select(node => this.CreateSingleTradeNode(node));

            uiBuilder.AddAllPanels(tradeNodes);
        }

        private GameObject CreateSingleTradeNode(TradeNode node)
        {
            Debug.Log($"Creating single node with {node.targetMarket.name}");
            var newTradeNode = GameObject.Instantiate(this.singleTradeNodePrefab, transform);
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