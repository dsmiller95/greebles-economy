using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MapGen.TileManagement
{
    public class TileRoute
    {
        public IList<Vector2Int> waypoints;
        public TileRoute(IList<Vector2Int> waypoints)
        {
            this.waypoints = waypoints;
        }
        public TileRoute() : this(new List<Vector2Int>())
        {
        }

        public void AddLastWaypoint(Vector2Int waypoint)
        {
            this.waypoints.Add(waypoint);
        }

        public Vector2Int PopNextWaypoint()
        {
            var nextWaypoint = this.waypoints.First();
            this.waypoints.RemoveAt(0);
            return nextWaypoint;
        }
    }
}
