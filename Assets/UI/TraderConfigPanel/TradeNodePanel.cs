using Assets.Scripts.Trader;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.UI.TraderConfigPanel
{
    public class TradeNodePanel : MonoBehaviour
    {
        public TradeNode tradeNode;
        public float maxTradeAmount;

        public Text description;

        [Header("Button Expander")]
        public Button expandButton;
        public Text expandButtonText;

        public MaterialListDragZone materialItemList;

        private bool expanded = false;
        // Start is called before the first frame update
        void Start()
        {
            description.text = $"Target: {tradeNode.targetMarket.name}";

            materialItemList.tradeNode = this.tradeNode;
            materialItemList.maxTradeAmount = this.maxTradeAmount;

            //var tradeNodes = tradeNode.trades.Select(node => CreateSingleResourceTrade(node)).ToList();
            expandButton.onClick.AddListener(ExpandButtonClicked);
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
