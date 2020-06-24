using Assets.Scripts.Trader;
using Assets.UI.SelectionManager;
using Assets.UI.SelectionManager.GetObjectSelector;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.TraderConfigPanel
{
    public class TradeNodePanel : MonoBehaviour
    {
        public TradeNode tradeNode;
        public float maxTradeAmount;

        public Text description;

        public Button changeMarketButton;

        [Header("Button Expander")]
        public Button expandButton;
        public Text expandButtonText;
        public MaterialListDragZone materialItemList;


        private Action marketChanged;
        private bool expanded = false;
        // Start is called before the first frame update
        void Start()
        {
            description.text = $"Target: {tradeNode.target.gameObject.name}";

            materialItemList.tradeNode = tradeNode;
            materialItemList.maxTradeAmount = maxTradeAmount;

            //var tradeNodes = tradeNode.trades.Select(node => CreateSingleResourceTrade(node)).ToList();
            expandButton.onClick.AddListener(ExpandButtonClicked);
            changeMarketButton.onClick.AddListener(SwitchMarketClicked);
        }

        private void SwitchMarketClicked()
        {
            SelectionTracker.globalTracker.PushSelectionInput(new SingleObjectHighlightSelector<TradeStop>(
                market => true,
                market =>
                {
                    MarketChanged(market);
                }));
        }

        //TODO: switch out callback method for task one
        private async Task SwitchMarketClickAsync()
        {
            try
            {
                var selection = await SelectionTracker.globalTracker.GetInputAsync<TradeStop>(target => true);

                Debug.Log($"Market Picked: {selection.gameObject.name}");
                tradeNode.target = selection;
                description.text = $"Target: {tradeNode.target.gameObject.name}";
                marketChanged?.Invoke();
            }
            catch (ObjectSelectionCancelledException) { }
        }

        private void MarketChanged(TradeStop market)
        {
            Debug.Log($"Market Picked: {market.gameObject.name}");
            tradeNode.target = market;
            description.text = $"Target: {tradeNode.target.gameObject.name}";
            marketChanged?.Invoke();
        }

        private void ExpandButtonClicked()
        {
            expanded = !expanded;
            expandButtonText.text = expanded ? "^" : "v";
            materialItemList.SetExpander(expanded);
        }

        public static TradeNodePanel InstantiateOnObject(
            GameObject selfPrefab,
            GameObject container,
            TradeNode node,
            float maxTradeAmount,
            Action marketChanged)
        {
            var newTradeNode = GameObject.Instantiate(selfPrefab, container.transform);
            var selfScript = newTradeNode.GetComponent<TradeNodePanel>();
            selfScript.tradeNode = node;
            selfScript.maxTradeAmount = maxTradeAmount;
            selfScript.marketChanged = marketChanged;
            return selfScript;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
