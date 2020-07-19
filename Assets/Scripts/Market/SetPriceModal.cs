using Assets.Scripts.Resources;
using Assets.UI.Modals;
using Assets.UI.Plotter;
using Assets.UI.Plotter.Function;
using Boo.Lang;
using TradeModeling.Functions;
using UnityEngine;

namespace Assets.Scripts.Market
{
    public class SetPriceModal : ModalManager
    {
        public MarketBehavior market;
        public GraphPlotter plotter;

        public void Start()
        {
            var resource = ResourceType.Food;
            var color = ResourceConfiguration.resourceColoring[resource];

            var priceFunctionConfig = market.GetSellPriceFunctions()[resource];
            var priceFunction = new SigmoidFunction(priceFunctionConfig);

            var functionAdapter = new PlottableFunctionToSeriesAdapter(
                x => priceFunction.GetValueAtPoint(x),
                new PlottableFunctionConfig
                {
                    start = 0,
                    end = market._inventory.inventoryCapacity,
                    steps = market._inventory.inventoryCapacity
                },
                new PlottableConfig
                {
                    dotColor = default,
                    lineColor = color,
                    yScale = priceFunctionConfig.yRange
                });

            plotter.Plottables = new[] { functionAdapter };
            functionAdapter.PlotFunction();
        }

        public override void OnConfirm()
        {
            base.OnConfirm();
            Debug.Log($"setting prices for {market.name}");
        }
    }
}
