using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Assets.UI.Plotter.Series
{
    public delegate void SeriesUpdatedHandler(object sender);
    public interface IPlottableSeries
    {
        event SeriesUpdatedHandler SeriesUpdated;
        IEnumerable<Vector2> GetSeries();
        float GetPointRange();
        PlottableConfig GetPlottableConfig();
    }
}