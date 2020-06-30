﻿using Assets.Scripts.Resources;
using Assets.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Trader.StateHandlers
{
    public class TradeExecuteStateHandler : GenericStateHandler<TraderState, TraderBehavior>
    {
        private float timeDelay;
        public TradeExecuteStateHandler(float timeDelay = 0.2f)
        {
            this.timeDelay = timeDelay;
        }

        class TradeExecuteStateData
        {
            public float lastExchangeTime;
            public IList<ResourceTrade> remainingTrades;
        }

        public TraderState stateHandle => TraderState.TradeExecute;

        public Task<TraderState> HandleState(TraderBehavior data)
        {
            TradeExecuteStateData myState = data.stateData[stateHandle];
            if (myState.remainingTrades.Count == 0)
            {
                return Task.FromResult(TraderState.TradeTransit);
            }
            if ((myState.lastExchangeTime + timeDelay) < Time.time)
            {
                myState.lastExchangeTime = Time.time;
                
                var trade = myState.remainingTrades[0];
                var targetInventory = data.currentTradeNodeTarget.target.tradeInventory;
                var option = data.inventory.transferResourceInto(trade.type, targetInventory, Mathf.Sign(trade.amount));
                var remainingToTrade = trade.amount - option.info;
                option.Execute();
                
                if (Mathf.Abs(option.info) <= 1E-05 || Mathf.Abs(remainingToTrade) <= 1E-05)
                {
                    myState.remainingTrades.RemoveAt(0);
                }
                else
                {
                    var newTrade = new ResourceTrade
                    {
                        amount = remainingToTrade,
                        type = trade.type
                    };
                    myState.remainingTrades[0] = newTrade;
                }
            }
            return Task.FromResult(TraderState.TradeExecute);
        }

        public TraderState validPreviousStates => TraderState.TradeTransit;
        public void TransitionIntoState(TraderBehavior data)
        {
            data.objectSeeker.ClearCurrentTarget();
            data.stateData[stateHandle] = new TradeExecuteStateData
            {
                lastExchangeTime = Time.time,
                remainingTrades = data.currentTradeNodeTarget.trades.Where(trade => Math.Abs(trade.amount) > 1E-5).ToList()
            };
        }

        public TraderState validNextStates => ~TraderState.TradeExecute;
        public void TransitionOutOfState(TraderBehavior data)
        {
            data.NextTradeRoute();
        }
    }
}
