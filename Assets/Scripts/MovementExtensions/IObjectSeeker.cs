using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.MovementExtensions
{
    public interface IObjectSeeker
    {
        bool seekTargetToTouch();
        void ClearCurrentTarget();
        bool isTouchingCurrentTarget();
        GameObject CurrentTarget { get; set; }
    }
}
