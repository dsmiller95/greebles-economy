using Assets.Scripts.Trader;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.TraderConfigPanel
{
    public class SingleMaterialTrade : MonoBehaviour
    {
        public ResourceTrade trade;
        public Text descriptionField;
        // Start is called before the first frame update
        void Start()
        {
            descriptionField.text = $"{trade.type}: {trade.amount}";
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
