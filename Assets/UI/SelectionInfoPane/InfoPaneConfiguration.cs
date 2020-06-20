using Assets.UI.Plotter.Series;
using System.Collections.Generic;

namespace Assets.UI.InfoPane
{
    public class PlotPaneConfig
    {
        public IMultiPlottableSeries plot;
        public string header;
    }

    public class InfoPaneConfiguration
    {
        public IList<PlotPaneConfig> plottables;
    }
}
