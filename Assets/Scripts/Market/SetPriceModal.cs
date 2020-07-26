using Assets.Scripts.Resources;
using Assets.UI.Modals;
using Assets.UI.Plotter;
using Assets.UI.Plotter.Function;
using Boo.Lang;
using TMPro;
using TradeModeling.Functions;
using TradeModeling.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Market
{
    public class SetPriceModal : ModalManager
    {
        public MarketBehavior market;
        public GraphPlotter plotter;
        public Slider slider;
        public int maxTargetValueDefaultWhenInfiniteCapacity = 100;

        public ResourceType defaultResource = ResourceType.Food;
        private ResourceType currentResource;

        public void Start()
        {
            this.SetupSelfForResource(defaultResource);
        }

        public TextMeshProUGUI sliderLabelText;
        public void OnPriceSliderChanged(float sliderValue)
        {
            sliderLabelText.text = sliderValue.ToString("F0");
            market.targetInventoryAmounts[currentResource] = sliderValue;
        }

        public void OnResourceSelected(ResourceType resource)
        {
            this.SetupSelfForResource(resource);
        }

        private void SetupSelfForResource(ResourceType resource)
        {
            currentResource = resource;

            var color = ResourceConfiguration.resourceColoring[currentResource];
            var inventoryCapacity = (market._inventory.itemSource as ISpaceFillingItemSource<ResourceType>)?.inventoryCapacity ?? maxTargetValueDefaultWhenInfiniteCapacity;

            var priceFunctionConfig = market.GetSellPriceFunctions()[currentResource];
            var priceFunction = new SigmoidFunction(priceFunctionConfig);

            var functionAdapter = new PlottableFunctionToSeriesAdapter(
                x => priceFunction.GetValueAtPoint(x),
                new PlottableFunctionConfig
                {
                    start = 0,
                    end = inventoryCapacity,
                    steps = inventoryCapacity
                },
                new PlottableConfig
                {
                    dotColor = default,
                    lineColor = color,
                    yScale = priceFunctionConfig.yRange
                });
            slider.minValue = 0f;
            slider.maxValue = inventoryCapacity;
            slider.value = market.targetInventoryAmounts[currentResource];
            plotter.Plottables = new[] { functionAdapter };
            functionAdapter.PlotFunction();
        }

        public override void OnConfirm()
        {
            base.OnConfirm();
            Debug.Log($"setting prices for {market.name}");
        }

        #region event handler assistors
        public void OnFoodSelected()
        {
            this.OnResourceSelected(ResourceType.Food);
        }
        public void OnWoodSelected()
        {
            this.OnResourceSelected(ResourceType.Wood);
        }
        #endregion
    }
}
