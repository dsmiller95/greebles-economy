using Assets.Scripts.Market;
using Assets.Scripts.Trader;
using Assets.UI.SelectionManager;
using Assets.UI.SelectionManager.GetObjectSelector;
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



        private bool expanded = false;
        // Start is called before the first frame update
        void Start()
        {
            description.text = $"Target: {tradeNode.targetMarket.name}";

            materialItemList.tradeNode = tradeNode;
            materialItemList.maxTradeAmount = maxTradeAmount;

            //var tradeNodes = tradeNode.trades.Select(node => CreateSingleResourceTrade(node)).ToList();
            expandButton.onClick.AddListener(ExpandButtonClicked);
            changeMarketButton.onClick.AddListener(SwitchMarketClicked);
        }

        private void SwitchMarketClicked()
        {
            SelectionTracker.globalTracker.PushSelectionInput(new SingleObjectHighlightSelector<MarketBehavior>(
                market => true,
                market =>
                {
                    Debug.Log("Picked something");
                    Debug.Log($"Market Picked: {market.name}");
                }));
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
            float maxTradeAmount)
        {
            var newTradeNode = GameObject.Instantiate(selfPrefab, container.transform);
            var selfScript = newTradeNode.GetComponent<TradeNodePanel>();
            selfScript.tradeNode = node;
            selfScript.maxTradeAmount = maxTradeAmount;
            return selfScript;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
