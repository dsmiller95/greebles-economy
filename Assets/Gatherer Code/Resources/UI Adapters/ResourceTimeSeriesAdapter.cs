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
        public PlottableConfig plotConfig;
    }

    public ResourceInventory inventory;
    public float timeRange = 20;
    public float timeStep = 0.2f;
    public ResourceGraphConfiguration[] resourcePlotConfig;
    private Dictionary<ResourceType, PlottableTimeSeries> inventoryTimeSeries = new Dictionary<ResourceType, PlottableTimeSeries>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (var configuration in resourcePlotConfig)
        {
            var newTimeSeries = new PlottableTimeSeries(Time.time, configuration.plotConfig, timeRange);
            newTimeSeries.AddPoint(this.inventory.getResource(configuration.type));
            inventoryTimeSeries.Add(configuration.type, newTimeSeries);
        }
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
            timeSeries.Value.AddPoint(this.inventory.getResource(timeSeries.Key));
        }
    }

    public IEnumerable<IPlottableSeries> GetPlottableSeries()
    {
        return this.inventoryTimeSeries.Values;
    }
}
