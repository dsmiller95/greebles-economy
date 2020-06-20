using Assets.Scripts.Trader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.TraderConfigPanel
{
    public class SingleTradeNode : MonoBehaviour
    {
        public TradeNode tradeNode;
        public Text description;
        // Start is called before the first frame update
        void Start()
        {
            description.text = $"Target: {this.tradeNode.targetMarket.name}";
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
