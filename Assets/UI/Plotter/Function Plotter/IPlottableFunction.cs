using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.UI.Plotter.Function
{
    public interface IPlottableFunction
    {
        float PlotAt(float x);
    }

    [Serializable]
    public struct PlottableFunctionConfig
    {
        /// <summary>
        /// The beginning of the segment to be plotted
        /// </summary>
        public float start;
        /// <summary>
        /// The end of the segment to be plotted
        /// </summary>
        public float end;
        /// <summary>
        /// The number of individual points to be drawn
        /// </summary>
        public int steps;
    }
}