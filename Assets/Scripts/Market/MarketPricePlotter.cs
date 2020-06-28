using Assets.Scripts.Market;
using Assets.Scripts.Resources;
using Assets.UI.Plotter;
using Assets.UI.Plotter.Function;
using System;
using System.Collections;
using System.Collections.Generic;
using TradeModeling.Functions;
using UnityEngine;

public class MarketPricePlotter : PlottableFunctionsToSeriesAdapter
{
    private IList<PlottableFunctionToSeriesAdapter> adapters;
    public MarketBehavior market;

    new public void Start()
    {
        
        this.adapters = new List<PlottableFunctionToSeriesAdapter>();
        var plotFunctionConfig = new PlottableFunctionConfig
        {
            start = 0,
            end = market._inventory.inventoryCapacity,
            steps = market._inventory.inventoryCapacity
        };
        foreach (var sellPrice in market.GetSellPriceFunctions())
        {
            var resource = sellPrice.Key;
            Debug.Log($"adding new function adapter for {Enum.GetName(typeof(ResourceType), resource)}");
            var functionConfiguration = sellPrice.Value;
            var color = ResourceConfiguration.resourceColoring[resource];
            var plotConfig = new PlottableConfig
            {
                dotColor = default,
                lineColor = color,
                yScale = functionConfiguration.yRange
            };
            var newFunctionInstance = new SigmoidFunction(functionConfiguration);
            var adapter = new PlottableFunctionToSeriesAdapter(
                x => newFunctionInstance.GetValueAtPoint(x),
                plotFunctionConfig,
                plotConfig);
            adapters.Add(adapter);
        }

        base.Start();
    }

    protected override IList<PlottableFunctionToSeriesAdapter> GetFunctions()
    {
        return this.adapters;
    }
}
