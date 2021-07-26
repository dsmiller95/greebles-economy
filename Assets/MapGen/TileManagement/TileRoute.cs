using Simulation.Tiling.HexCoords;
using System.Collections.Generic;
using System.Linq;

namespace Assets.MapGen.TileManagement
{
    public class TileRoute
    {
        public IList<AxialCoordinate> waypoints;
        public TileRoute(IList<AxialCoordinate> waypoints)
        {
            this.waypoints = waypoints;
        }
        public TileRoute() : this(new List<AxialCoordinate>())
        {
        }

        public int TotalWaypoints()
        {
            return waypoints.Count;
        }

        public void AddLastWaypoint(AxialCoordinate waypoint)
        {
            var distance = PeekNext().ToCube().DistanceTo(waypoint.ToCube());
            if (distance > 1)
            {
                throw new System.Exception($"Attempted to add {waypoint} to route ending in {PeekNext()}. distance between points is {distance}");
            }
            waypoints.Add(waypoint);
        }

        public AxialCoordinate PeekNext()
        {
            return waypoints.FirstOrDefault();
        }

        public AxialCoordinate PopNextWaypoint()
        {
            var nextWaypoint = waypoints.First();
            waypoints.RemoveAt(0);
            return nextWaypoint;
        }
    }
}
