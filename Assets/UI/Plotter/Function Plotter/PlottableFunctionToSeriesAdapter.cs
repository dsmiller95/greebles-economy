using Assets.UI.Plotter.Function;
using Assets.UI.Plotter.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UI.Plotter.Function
{
    public class PlottableFunctionToSeriesAdapter : IPlottableSeries
    {
        public PlottableFunctionToSeriesAdapter(Func<float, float> function, PlottableFunctionConfig functionConfig, PlottableConfig plotConfig)
        {
            this.function = function;
            this.functionConfig = functionConfig;
            this.plotConfig = plotConfig;
        }

        private Func<float, float> function;
        private PlottableFunctionConfig functionConfig;
        private PlottableConfig plotConfig;
        private IList<Vector2> plotSeries;

        public void PlotFunction()
        {
            this.plotSeries = this.GeneratePlotVectors().ToArray();
            this.SeriesUpdated?.Invoke(this);
        }
        private IEnumerable<Vector2> GeneratePlotVectors()
        {
            float xStep = (functionConfig.end - functionConfig.start) / functionConfig.steps;
            for (int i = 0; i < functionConfig.steps; i++)
            {
                var xPos = i * xStep;
                var value = function(xPos);
                var dotPos = new Vector2(xPos, value);
                yield return dotPos;
            }
        }

        public event SeriesUpdatedHandler SeriesUpdated;

        public float GetPointRange()
        {
            return this.functionConfig.end - this.functionConfig.start;
        }

        public IEnumerable<Vector2> GetSeries()
        {
            return this.plotSeries ?? new List<Vector2>();
        }

        public PlottableConfig GetPlottableConfig()
        {
            return this.plotConfig;
        }
    }
}