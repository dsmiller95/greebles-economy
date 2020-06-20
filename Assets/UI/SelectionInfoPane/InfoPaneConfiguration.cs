using Assets.UI.Plotter.Series;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI.InfoPane
{
    public class PlotPaneConfig
    {
        public IMultiPlottableSeries plot;
        public string header;
    }

    public class GenericUIObjectConfig
    {
        public GameObject prefabToInit;
        public Action<GameObject> postInitHook;
    }

    public class InfoPaneConfiguration
    {
        public IList<PlotPaneConfig> plottables;
        public IList<GenericUIObjectConfig> uiObjects;
    }
}
