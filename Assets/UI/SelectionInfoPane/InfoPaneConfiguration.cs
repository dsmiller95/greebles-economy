using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PlotPaneConfig
{
    public IMultiPlottableSeries plot;
    public string header;
}

public class InfoPaneConfiguration
{
    public IList<PlotPaneConfig> plottables;
}
