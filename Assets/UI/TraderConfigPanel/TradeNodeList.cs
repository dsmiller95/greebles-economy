﻿using Assets.Scripts.Trader;
using Assets.UI.Draggable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.UI.TraderConfigPanel
{
    [RequireComponent(typeof(DragZone))]
    public class TradeNodeList : MonoBehaviour
    {
        public TraderBehavior linkedTrader;
        public GameObject singleTradeNodePrefab;
        public Action<TradeNode[]> tradeRouteUpdated;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log($"Node list opened with {linkedTrader?.name}");
            var largestTrade = linkedTrader.inventory.inventoryCapacity;
            var tradeNodes = linkedTrader.tradeRoute
                .Select(node => CreateSingleTradeNode(node, largestTrade))
                .ToList();
            GetComponent<DragZone>().orderingChanged += SetOrder;
        }
        private void SetOrder()
        {
            this.tradeRouteUpdated?.Invoke(this.GetComponentsInChildren<TradeNodePanel>().Select(x => x.tradeNode).ToArray());
        }

        private TradeNodePanel CreateSingleTradeNode(TradeNode node, float maxTradeAmount)
        {
            return TradeNodePanel.InstantiateOnObject(
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