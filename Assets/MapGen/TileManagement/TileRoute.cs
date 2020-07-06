using Simulation.Tiling;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.MapGen.TileManagement
{
    public class TileRoute
    {
        public IList<OffsetCoordinate> waypoints;
        public TileRoute(IList<OffsetCoordinate> waypoints)
        {
            this.waypoints = waypoints;
        }
        public TileRoute() : this(new List<OffsetCoordinate>())
        {
        }

        public int TotalWaypoints()
        {
            return waypoints.Count;
        }

        public void AddLastWaypoint(OffsetCoordinate waypoint)
        {
            waypoints.Add(waypoint);
        }

        public OffsetCoordinate PeekNext()
        {
            return waypoints.FirstOrDefault();
        }

        public OffsetCoordinate PopNextWaypoint()
        {
            var nextWaypoint = waypoints.First();
            waypoints.RemoveAt(0);
            return nextWaypoint;
        }
    }
}
