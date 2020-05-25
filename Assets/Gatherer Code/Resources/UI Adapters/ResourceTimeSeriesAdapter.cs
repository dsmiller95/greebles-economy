using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTimeSeriesAdapter : MonoBehaviour, IMultiPlottableSeries
{
    [Serializable]
    public struct ResourceGraphConfiguration
    {
        public ResourceType type;
        public float yScale;
    }

    public ResourceInventory inventory;
    private SpaceFillingInventory<ResourceType> _inventory;
    public float timeRange = 20;
    /// <summary>
    /// total individual samples that should be on the screen at one time
    /// </summary>
    public float totalSteps = 40;
    private float timeStep;
    public ResourceGraphConfiguration[] resourcePlotConfig;
    private Dictionary<ResourceType, PlottableTimeSeries> inventoryTimeSeries = new Dictionary<ResourceType, PlottableTimeSeries>();

    private void Awake()
    {
        _inventory = inventory.backingInventory;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var configuration in resourcePlotConfig)
        {
            var coloring = ResourceConfiguration.resourceColoring[configuration.type];
            var plotConfig = new PlottableConfig
            {
                dotColor = default,
                lineColor = coloring,
                yScale = configuration.yScale
            };
            var newTimeSeries = new PlottableTimeSeries(Time.time, plotConfig, timeRange);
            newTimeSeries.AddPoint(_inventory.getResource(configuration.type));
            inventoryTimeSeries.Add(configuration.type, newTimeSeries);
        }
        this.timeStep = this.timeRange / this.totalSteps;
    }

    private float lastPlot = 0;
    private void Update()
    {

        if (Time.time - lastPlot > timeStep)
        {
            lastPlot = Time.time;
            this.LogCurrentResources();
        }
    }

    private void LogCurrentResources()
    {
        foreach (var timeSeries in inventoryTimeSeries)
        {
            timeSeries.Value.AddPoint(_inventory.getResource(timeSeries.Key));
        }
    }

    public IEnumerable<IPlottableSeries> GetPlottableSeries()
    {
        return this.inventoryTimeSeries.Values;
    }
}
