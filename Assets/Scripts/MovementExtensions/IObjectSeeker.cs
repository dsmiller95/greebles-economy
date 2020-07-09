using Boo.Lang;
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
        /// <summary>
        /// Returns true on the frame that the seeker touches the target
        /// </summary>
        /// <returns></returns>
        GameObject seekTargetToTouch(bool intersect = false);
        void ClearCurrentTarget();
        bool isTouchingCurrentTarget();
        GameObject CurrentTarget { get; }

        void BeginApproachingNewTarget(GameObject target);

        IEnumerable<(T, float)> GetObjectsWithinDistanceFromFilter<T>(float distance, Func<T, bool> filter);
    }
}
