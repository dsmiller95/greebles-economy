using Assets.Scripts.Trader;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.TraderConfigPanel
{
    public class SingleTradeNode : MonoBehaviour
    {
        public TradeNode tradeNode;
        public float maxTradeAmount;

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
            expandButton.onClick.AddListener(ExpandButtonClicked);
        }

        private void ExpandButtonClicked()
        {
            if (expanded)
            {
                foreach (Transform child in singleResourceTradeContainer.transform)
                {
                    Destroy(child.gameObject);
                }
            }
            else
            {
                foreach (var node in tradeNode.trades)
                {
                    CreateSingleResourceTrade(node);
                }
            }
            expanded = !expanded;
            expandButtonText.text = expanded ? "^" : "v";
        }

        private SingleMaterialTrade CreateSingleResourceTrade(ResourceTrade trade)
        {
            return SingleMaterialTrade.InstantiateOnObject(
                singleResourceTradePrefab,
                singleResourceTradeContainer,
                trade,
                (int)maxTradeAmount);
        }

        public static SingleTradeNode InstantiateOnObject(
            GameObject selfPrefab,
            GameObject container,
            TradeNode node,
            float maxTradeAmount)
        {
            Debug.Log($"Creating single node with {node.targetMarket.name}");
            var newTradeNode = GameObject.Instantiate(selfPrefab, container.transform);
            var selfScript = newTradeNode.GetComponent<SingleTradeNode>();
            selfScript.tradeNode = node;
            selfScript.maxTradeAmount = maxTradeAmount;
            Debug.Log($"Assigned node with {node.trades.Length} trades");
            return selfScript;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
