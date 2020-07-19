using Assets.UI.Plotter.Series;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Assets.UI.Plotter
{
    public class GraphPlotter : MonoBehaviour
    {
        public GameObject[] plots;
        public RectTransform container;
        public Sprite circleSprite;

        private IList<IPlottableSeries> plottables;
        private IList<PlotContainer> plotContainers;

        public IEnumerable<IPlottableSeries> Plottables
        {
            set
            {
                if (plotContainers != null)
                {
                    foreach (var container in plotContainers)
                    {
                        container.OnDestroy();
                    }
                }
                plottables = value.ToList();
                plotContainers = plottables
                    .Select(plottable => new PlotContainer(plottable, this))
                    .ToList();
                foreach (var container in plotContainers)
                {
                    container.Init();
                }
            }
        }

        public void SetPlottablesPreStart(IEnumerable<IPlottableSeries> plottables)
        {
            this.plottables = plottables.ToList();
        }

        void Start()
        {
            if (plottables == default)
            {
                Plottables = plots
                    .SelectMany(gameObject => gameObject.GetComponents<IPlottableSeries>())
                    .Where(plottable => plottable != default)
                    .Concat(plots
                        .SelectMany(gameObject => gameObject.GetComponents<IMultiPlottableSeries>())
                        .Where(multiPlottable => multiPlottable != default)
                        .SelectMany(multiPlottable => multiPlottable.GetPlottableSeries()))
                    .ToList();
            }
        }

        private void OnDestroy()
        {
            foreach (var container in plotContainers)
            {
                container.OnDestroy();
            }
        }
    }
}