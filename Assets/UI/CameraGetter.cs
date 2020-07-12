using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.UI
{
    public static class CameraGetter
    {
        public static GameObject GetCameraObject()
        {
            return GameObject.FindGameObjectsWithTag("MainCamera").First();
        }
    }
}
