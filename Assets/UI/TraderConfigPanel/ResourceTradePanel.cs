﻿using Assets.Scripts.Resources;
using Assets.Scripts.Trader;
using TradeModeling.TradeRouteUtilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.UI.TraderConfigPanel
{
    public class ResourceTradePanel : MonoBehaviour
    {
        public ResourceTrade<ResourceType> trade;
        public int maxTradeAmount;

        [Header("UI element bindings")]
        public Text descriptionField;
        public InputField amountInput;
        public Slider amountSlider;
        // Start is called before the first frame update
        void Start()
        {
            descriptionField.text = $"{trade.type}:";

            amountSlider.maxValue = maxTradeAmount;
            amountSlider.minValue = -maxTradeAmount;

            AmountChanged(Mathf.RoundToInt(trade.amount));

            amountInput.onValueChanged.AddListener(AmountTextValueChanged);
            amountSlider.onValueChanged.AddListener(AmountSliderValueChanged);
        }

        public void AmountTextValueChanged(string newValue)
        {
            AmountChanged(int.Parse(newValue));
        }
        public void AmountSliderValueChanged(float newValue)
        {
            AmountChanged(Mathf.RoundToInt(newValue));
        }

        private void AmountChanged(int newValue)
        {
            newValue = Mathf.Clamp(newValue, -maxTradeAmount, maxTradeAmount);
            amountSlider.value = newValue;
            amountInput.text = newValue.ToString();
            trade.amount = newValue;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public static ResourceTradePanel InstantiateOnObject(
            GameObject selfPrefab,
            GameObject container,
            ResourceTrade<ResourceType> trade,
            int maxTradeAmount)
        {
            var newTradeNode = GameObject.Instantiate(selfPrefab, container.transform);
            var selfScript = newTradeNode.GetComponent<ResourceTradePanel>();
            selfScript.trade = trade;
            selfScript.maxTradeAmount = maxTradeAmount;

            return selfScript;
        }
    }
}
