using Assets.Scripts.Trader;
using Assets.UI.Modals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Market
{
    public class MarketUIActions : MonoBehaviour
    {
        public MarketBehavior market;

        public GameObject priceSetModalPrefab;

        public void SetNewPrices()
        {
            Debug.Log($"opening new modal for {market.name}");
            var newModal = Instantiate(priceSetModalPrefab, ModalContainer.instance.transform);
            newModal.GetComponent<SetPriceModal>().market = this.market;
        }
    }
}