using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class PlottableTimeSeries : IPlottableSeries
{
    public event SeriesUpdatedHandler SeriesUpdated;

    private PlottableConfig plotConfig;
    private IList<Vector2> currentTimeSeries;
    private float startTime;
    private float timeRange;

    /// <summary>
    /// Create a new plottable time-series graph
    /// </summary>
    /// <param name="initialStartTime">The point in time that will map to x=0</param>
    /// <param name="timeRange">The time range in seconds the graph should represent</param>
    public PlottableTimeSeries(float initialStartTime, PlottableConfig plotConfig, float timeRange = 20)
    {
        this.currentTimeSeries = new List<Vector2>();
        this.startTime = initialStartTime;
        this.timeRange = timeRange;
        this.plotConfig = plotConfig;
    }

    public void AddPoint(float value)
    {
        var newPoint = new Vector2(Time.time, value);
        this.currentTimeSeries.Add(newPoint);

        var timeShift = newPoint.x - (this.startTime + this.timeRange);
        if (timeShift > 0)
        {
            //the new point is past the end of the graph; shift the rolling window
            this.startTime += timeShift;
            this.currentTimeSeries = this.currentTimeSeries
                .Where(point => point.x > startTime)
                .ToList();
        }
        this.SeriesUpdated?.Invoke(this);
    }

    public IEnumerable<Vector2> GetSeries()
    {
        return this.currentTimeSeries.Select(point => new Vector2(point.x - this.startTime, point.y));
    }

    public float GetPointRange()
    {
        return this.timeRange;
    }

    public PlottableConfig GetPlottableConfig()
    {
        return this.plotConfig;
    }
}
