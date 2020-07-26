using Assets.Scripts.Market;
using Assets.Scripts.Resources;
using Assets.UI.Plotter;
using Assets.UI.Plotter.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using TradeModeling.Functions;
using TradeModeling.Inventories;
using UnityEngine;

public class MarketPricePlotter : PlottableFunctionsToSeriesAdapter
{
    private IList<PlottableFunctionToSeriesAdapter> adapters;
    public MarketBehavior market;
    public float defaultPlotSizeIfInfiniteSpace = 100;

    new public void Start()
    {
        adapters = new List<PlottableFunctionToSeriesAdapter>();
        var plotSize = (market._inventory.itemSource as ISpaceFillingItemSource<ResourceType>)?.inventoryCapacity ?? defaultPlotSizeIfInfiniteSpace;
        var plotFunctionConfig = new PlottableFunctionConfig
        {
            start = 0,
            end = plotSize,
            steps = (int)plotSize
        };
        var allPriceFunctions = market.GetSellPriceFunctions().ToList();
        var maxYScale = allPriceFunctions.Max(x => x.Value.yRange);
        adapters = allPriceFunctions
            .Select(kvp =>
        {
            var resource = kvp.Key;
            var color = ResourceConfiguration.resourceColoring[resource];
            var plotConfig = new PlottableConfig
            {
                dotColor = default,
                lineColor = color,
                yScale = maxYScale
            };
            var newFunctionInstance = new SigmoidFunction(kvp.Value);
            var adapter = new PlottableFunctionToSeriesAdapter(
                x =>
                {
                    if(Mathf.Abs(x - market._inventory.Get(resource)) < 0.1)
                    {
                        // hackey way to force the graph to show an indication of where the current inventory lies on the price function graph
                        return 0;
                    }
                    return newFunctionInstance.GetValueAtPoint(x);
                },
                plotFunctionConfig,
                plotConfig);
            return adapter;
        }).ToList();

        base.Start();
    }

    protected override IList<PlottableFunctionToSeriesAdapter> GetFunctions()
    {
        return adapters;
    }
}
