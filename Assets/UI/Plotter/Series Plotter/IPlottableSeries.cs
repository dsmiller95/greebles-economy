﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

delegate void SeriesUpdatedHandler(object sender);
interface IPlottableSeries
{
    event SeriesUpdatedHandler SeriesUpdated;
    IEnumerable<Vector2> GetSeries();
    float GetPointRange();
    PlottableConfig GetPlottableConfig();
}

[Serializable]
public struct PlottableConfig
{
    /// <summary>
    /// The actual value of the plot is divided by this to attempt to normalize it into a range of [0, 1]
    /// </summary>
    public float yScale;
    public Color lineColor;
    public Color dotColor;
}
