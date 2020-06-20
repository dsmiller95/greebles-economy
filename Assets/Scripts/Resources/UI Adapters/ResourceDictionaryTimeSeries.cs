using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Resources.UI
{
    class ResourceDictionaryTimeSeries : ResourceTimeSeriesAdapter
    {
        public IDictionary<ResourceType, float> values;

        protected override float GetResourceValue(ResourceType resourceType)
        {
            return values[resourceType];
        }
    }
}