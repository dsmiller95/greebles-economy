using Assets.Scripts.Trader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderUIActions : MonoBehaviour
{
    public TraderBehavior trader;

    public void ClearTradeRoute()
    {
        this.trader.SetNewTradeRoute(new TradeNode[0]);
    }
}
