using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scrips.Resources
{
    public static class ResourceConfiguration
    {
        public static bool configSet = false;
        public static ResourceType[] spaceFillingItems;
        public static Dictionary<ResourceType, Color> resourceColoring;
    }
}