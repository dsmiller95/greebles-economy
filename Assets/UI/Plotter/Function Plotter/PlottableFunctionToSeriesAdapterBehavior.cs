using Assets.UI.Plotter.Series;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.UI.Plotter.Function
{

    public abstract class PlottableFunctionsToSeriesAdapter : MonoBehaviour, IMultiPlottableSeries
    {
        public float updateFrequency;

        private IList<PlottableFunctionToSeriesAdapter> functions;

        protected abstract IList<PlottableFunctionToSeriesAdapter> GetFunctions();

        public void Start()
        {
            functions = GetFunctions();
        }

        // TODO: make this only update when we're looking at it
        private float lastPlot = 0;
        public void Update()
        {
            if (Time.time - lastPlot > updateFrequency)
            {
                lastPlot = Time.time;

                foreach (var func in functions)
                {
                    func.PlotFunction();
                }
            }
        }

        public IEnumerable<IPlottableSeries> GetPlottableSeries()
        {
            Debug.Log($"Rendering an adapter with {functions.Count} plots");
            return functions;
        }
    }
}