﻿using Assets.Scripts.Trader;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.TraderConfigPanel
{
    public class SingleTradeNode : MonoBehaviour
    {
        public TradeNode tradeNode;
        public Text description;

        public GameObject singleResourceTradePrefab;
        public GameObject singleResourceTradeContainer;
        // Start is called before the first frame update
        void Start()
        {
            description.text = $"Target: {tradeNode.targetMarket.name}";

            var tradeNodes = tradeNode.trades.Select(node => CreateSingleResourceTrade(node)).ToList();
        }

        private GameObject CreateSingleResourceTrade(ResourceTrade trade)
        {
            var newTradeNode = GameObject.Instantiate(singleResourceTradePrefab, singleResourceTradeContainer.transform);
            var singleResourceTradeScript = newTradeNode.GetComponent<SingleMaterialTrade>();
            singleResourceTradeScript.trade = trade;

            return newTradeNode;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
