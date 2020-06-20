using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scrips.Resources
{
    public class ResourceConfigurer : MonoBehaviour
    {
        public ResourceType[] SpaceFillingItems;

        [Serializable]
        public struct ResourceColoring
        {
            public ResourceType type;
            public Color color;
        }
        public ResourceColoring[] coloring;

        private void Awake()
        {
            try
            {
                if (ResourceConfiguration.configSet)
                {
                    throw new Exception("Configuration already set. Can only set the configuration once");
                }
                ResourceConfiguration.spaceFillingItems = this.SpaceFillingItems;
                var coloringDictionary = this.coloring.ToDictionary(coloring => coloring.type, coloring => coloring.color);
                foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
                {
                    if (!coloringDictionary.ContainsKey(type))
                    {
                        throw new ArgumentException($"Coloring map is missing color for {Enum.GetName(typeof(ResourceType), type)}");
                    }
                }
                ResourceConfiguration.resourceColoring = coloringDictionary;
                ResourceConfiguration.configSet = true;
            }
            catch (Exception e)
            {
                // Any errors setting up config will be fatal
                Debug.LogError(e);
                Application.Quit();
                throw;
            }
        }
    }
}