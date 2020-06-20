using Assets.UI.Plotter.Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UI.Plotter.Series
{
    class PlottableFunctionToSeriesAdapter : MonoBehaviour, IPlottableSeries
    {
        public GameObject plottableFunctionObject;
        public PlottableFunctionConfig functionConfig;
        public PlottableConfig plotConfig;
        private IList<Vector2> plotSeries;


        private IPlottableFunction function;

        private void Awake()
        {
            this.function = plottableFunctionObject.GetComponent<IPlottableFunction>();
        }

        private float lastPlot = 0;
        public void Update()
        {
            if (Time.time - lastPlot > functionConfig.updateFrequency)
            {
                lastPlot = Time.time;
                this.PlotFunction();
            }
        }

        private void PlotFunction()
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
                var value = function.PlotAt(xPos);
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