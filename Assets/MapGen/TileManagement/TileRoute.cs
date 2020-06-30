using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MapGen.TileManagement
{
    public class TileRoute : IEnumerable<Vector2Int>
    {
        private IList<Vector2Int> waypoints;
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

        public void RemoveFirstWaypoint()
        {
            this.waypoints.RemoveAt(0);
        }

        public Vector2Int LastWaypoint()
        {
            return waypoints.Last();
        }
        public Vector2Int FirstWaypoint()
        {
            return waypoints.First();
        }

        public IEnumerator<Vector2Int> GetEnumerator()
        {
            return waypoints.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return waypoints.GetEnumerator();
        }
    }
}
