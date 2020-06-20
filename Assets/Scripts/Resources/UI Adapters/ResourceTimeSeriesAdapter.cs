using Assets.UI.Plotter.Series;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Resources.UI
{
    [Serializable]
    public class ResourceGraphConfiguration
    {
        public ResourceType type;
        public float yScale;
    }
    public abstract class ResourceTimeSeriesAdapter : GenericTimeSeries<ResourceType>, IMultiPlottableSeries
    {
        public ResourceGraphConfiguration[] resourcePlotConfig;

        protected abstract float GetResourceValue(ResourceType resourceType);

        // Start is called before the first frame update
        void Start()
        {
            this.StartTimeSeries(
                (type) =>
                {
                    return this.GetResourceValue(type);
                },
                this.resourcePlotConfig.Select(x => new TypedGraphConfiguration<ResourceType>()
                {
                    color = ResourceConfiguration.resourceColoring[x.type],
                    yScale = x.yScale,
                    type = x.type
                })
                );
        }
        private void Update()
        {
            this.UpdateTimeSeries();
        }
    }
}