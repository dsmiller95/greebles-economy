using Assets.UI.Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Market
{
    public class SetPriceModal : ModalManager
    {
        public MarketBehavior market;
        public override void OnConfirm()
        {
            base.OnConfirm();
            Debug.Log($"setting prices for {market.name}");
        }
    }
}
