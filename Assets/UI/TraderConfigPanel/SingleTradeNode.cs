using Assets.Scripts.Trader;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.TraderConfigPanel
{
    public class SingleTradeNode : MonoBehaviour
    {
        public TradeNode tradeNode;
        public Text description;

        [Header("Button Expander")]
        public Button expandButton;
        public Text expandButtonText;

        [Header("Expander Items")]
        public GameObject singleResourceTradePrefab;
        public GameObject singleResourceTradeContainer;

        private bool expanded = false;
        // Start is called before the first frame update
        void Start()
        {
            description.text = $"Target: {tradeNode.targetMarket.name}";

            //var tradeNodes = tradeNode.trades.Select(node => CreateSingleResourceTrade(node)).ToList();
            expandButton.onClick.AddListener(this.ExpandButtonClicked);
        }

        private void ExpandButtonClicked()
        {
            if (expanded)
            {
                foreach(Transform child in singleResourceTradeContainer.transform)
                {
                    Destroy(child.gameObject);
                }
            } else
            {
                foreach(var node in tradeNode.trades)
                {
                    CreateSingleResourceTrade(node);
                }
            }
            expanded = !expanded;
            expandButtonText.text = expanded ? "^" : "v";
        }

        private GameObject CreateSingleResourceTrade(ResourceTrade trade)
        {
            var newTradeNode = GameObject.Instantiate(singleResourceTradePrefab, singleResourceTradeContainer.transform);
            var singleResourceTradeScript = newTradeNode.GetComponent<SingleMaterialTrade>();
            singleResourceTradeScript.trade = trade;

            return newTradeNode;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
