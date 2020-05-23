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

        inventory.resourceAmountChanges += Inventory_resourceAmountChanges;
    }

    private void Inventory_resourceAmountChanges(object sender, ResourceChanged e)
    {
        this.inventoryTimeSeries[e.type]?.AddPoint(e.newValue);
    }

    public IEnumerable<IPlottableSeries> GetPlottableSeries()
    {
        return this.inventoryTimeSeries.Values;
    }
}
