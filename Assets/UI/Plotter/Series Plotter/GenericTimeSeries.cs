using System;
using System.Collections;
using System.Collections.Generic;
using TradeModeling.Inventories;
using UnityEngine;

[Serializable]
public class TypedGraphConfiguration<TSeriesKey>
{
    public TSeriesKey type;
    public float yScale;
    public Color color;
}

public abstract class GenericTimeSeries<TSeriesKey> : MonoBehaviour, IMultiPlottableSeries
{

    private Func<TSeriesKey, float> getValue;

    [Tooltip("Range of time in seconds that the chart should cover.")]
    [InspectorName("Time Range (s)")]
    public float timeRange = 20;
    [Tooltip("Total individual samples that should be on the screen at one time.")]
    [InspectorName("Steps")]
    public float totalSteps = 40;


    private float timeStep;
    private Dictionary<TSeriesKey, PlottableTimeSeries> timeSeriesPlots = new Dictionary<TSeriesKey, PlottableTimeSeries>();

    public void StartTimeSeries(
        Func<TSeriesKey, float> getValue,
        IEnumerable<TypedGraphConfiguration<TSeriesKey>> plotconfig)
    {
        this.getValue = getValue;

        foreach (var configuration in plotconfig)
        {
            var plotConfig = new PlottableConfig
            {
                dotColor = default,
                lineColor = configuration.color,
                yScale = configuration.yScale
            };
            var newTimeSeries = new PlottableTimeSeries(Time.time, plotConfig, timeRange);
            newTimeSeries.AddPoint(getValue(configuration.type));
            timeSeriesPlots.Add(configuration.type, newTimeSeries);
        }
        this.timeStep = this.timeRange / this.totalSteps;
    }

    private float lastPlot = 0;
    protected void UpdateTimeSeries()
    {

        if (Time.time - lastPlot > timeStep)
        {
            lastPlot = Time.time;
            this.LogCurrentResources();
        }
    }

    private void LogCurrentResources()
    {
        foreach (var timeSeries in timeSeriesPlots)
        {
            timeSeries.Value.AddPoint(getValue(timeSeries.Key));
        }
    }

    public IEnumerable<IPlottableSeries> GetPlottableSeries()
    {
        return this.timeSeriesPlots.Values;
    }
}
