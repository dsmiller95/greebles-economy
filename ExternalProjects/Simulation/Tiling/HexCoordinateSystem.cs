using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace Simulation.Tiling
{
    public class HexCoordinateSystem
    {
        public float hexRadius;

        private readonly Vector2 qBasis = new Vector2(3f/2f, -Mathf.Sqrt(3)/2f);
        private readonly Vector2 rBasis = new Vector2(0, -Mathf.Sqrt(3));

        public HexCoordinateSystem(float hexRadius)
        {
            this.hexRadius = hexRadius;
        }

        public Vector2 TileMapToRelative(AxialCoordinate axial)
        {
            var x = qBasis.x * axial.q;
            var y = qBasis.y * axial.q + rBasis.y * axial.r;
            return new Vector2(x, y);
        }
        /// <summary>
        /// Translate a tile map coordinate to a standard "real" position. this is not scaled based
        ///     on the size of the hexes. only use it for calculations that need to know about
        ///     relative positioning of hex members as opposed to real positioning
        /// </summary>
        /// <param name="offsetCoordinates"></param>
        /// <returns></returns>
        public Vector2 TileMapToRelative(OffsetCoordinate offsetCoordinates)
        {
            return TileMapToRelative(offsetCoordinates.ToAxial());
        }
        public Vector2 TileMapToReal(OffsetCoordinate offsetCoordinates)
        {
            return TileMapToRelative(offsetCoordinates) * hexRadius;
        }

        public CubeCoordinate RelativeToTileMap(Vector2 relativePosition)
        {
            var cubicFloating = ConvertSizeScaledPointToFloatingCubic(relativePosition);
            cubicFloating.z = -cubicFloating.x - cubicFloating.y;
            return RoundToNearestCube(cubicFloating);
        }
        public CubeCoordinate RealToTileMap(Vector2 realPosition)
        {
            var relativePositioning = realPosition / hexRadius;
            return RelativeToTileMap(relativePositioning);
        }


        public int DistanceBetweenInJumps(OffsetCoordinate origin, OffsetCoordinate destination)
        {
            return this.DistanceBetweenInJumps(origin.ToCube(), destination.ToCube());
        }

        public int DistanceBetweenInJumps(AxialCoordinate origin, AxialCoordinate destination)
        {
            return this.DistanceBetweenInJumps(origin.ToCube(), destination.ToCube());
        }

        public int DistanceBetweenInJumps(CubeCoordinate origin, CubeCoordinate destination)
        {
            return origin.DistanceTo(destination);
        }

        public IEnumerable<AxialCoordinate> GetPositionsWithinJumpDistance(AxialCoordinate origin, int jumpDistance)
        {
            for(var q = -jumpDistance; q <= jumpDistance; q++)
            {
                var sliceStart = Mathf.Max(-jumpDistance, -q - jumpDistance);
                var sliceEnd = Mathf.Min(jumpDistance, -q + jumpDistance);
                for(var r = sliceStart; r <= sliceEnd; r++)
                {
                    yield return new AxialCoordinate(q, r) + origin;
                }
            }
        }

        public IEnumerable<OffsetCoordinate> GetPositionsWithinJumpDistance(OffsetCoordinate origin, int jumpDistance)
        {
            return GetPositionsWithinJumpDistance(origin.ToAxial(), jumpDistance).Select(x => x.ToOffset());

        }

        public IEnumerable<Vector2Int> GetPositionsSpiralingAround(Vector2Int origin)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AxialCoordinate> GetRouteGenerator(AxialCoordinate origin, AxialCoordinate destination)
        {
            if(origin.DistanceTo(destination) > 1000)
            {
                Debug.Log("trying to find route between preposterously distant points");
            }

            var currentTileMapPos = new AxialCoordinate(origin.q, origin.r);
            var myDest = new AxialCoordinate(destination.q, destination.r);
            var iterations = 0;
            while (!currentTileMapPos.Equals(myDest))
            {
                var realWorldVectorToDest = TileMapToRelative(myDest)
                    - TileMapToRelative(currentTileMapPos);

                var nextMoveVector = GetClosestMatchingValidMove(realWorldVectorToDest);

                var lastDistance = currentTileMapPos.DistanceTo(myDest);
                currentTileMapPos = currentTileMapPos + nextMoveVector;
                var newDistance = currentTileMapPos.DistanceTo(myDest);
                if(newDistance >= lastDistance)
                {
                    throw new Exception("we're goin the wrong way!!");
                }

                yield return currentTileMapPos;
                iterations++;
                if (iterations > 1000)
                {
                    throw new Exception("too many loop brooother");
                }
            }
        }

        private AxialCoordinate GetClosestMatchingValidMove(Vector2 worldSpaceDestinationVector)
        {
            //TODO: use the offsetCoordinate to get the neighbors
            var angle = Vector2.SignedAngle(Vector2.right, worldSpaceDestinationVector);
            if (0 <= angle && angle < 60)
            {
                return new AxialCoordinate(1, -1);;
            }
            if (60 <= angle && angle < 120)
            {
                return new AxialCoordinate(0, -1);
            }
            if (120 <= angle && angle <= 180)
            {
                return new AxialCoordinate(-1, 0);
            }
            if (-180 <= angle && angle < -120)
            {
                return new AxialCoordinate(-1, 1);
            }
            if (-120 <= angle && angle < -60)
            {
                return new AxialCoordinate(0, 1);
            }
            if (-60 <= angle && angle < 0)
            {
                return new AxialCoordinate(1, 0);
            }
            throw new Exception($"error in angle matching {angle}");
        }

        private Vector3 ConvertSizeScaledPointToFloatingCubic(Vector2 point)
        {
            var q = 2f / 3f * point.x;
            var r = -1f / 3f * point.x + Mathf.Sqrt(3) / 3f * point.y;
            var cubicCoords = new Vector3(q, r, 0);
            return cubicCoords;
        }

        private CubeCoordinate RoundToNearestCube(Vector3 floatCube)
        {
            var roundedCoord = new CubeCoordinate(
                Mathf.RoundToInt(floatCube.x),
                Mathf.RoundToInt(floatCube.y),
                Mathf.RoundToInt(floatCube.z)
                );

            var xDiff = Mathf.Abs(roundedCoord.x - floatCube.x);
            var yDiff = Mathf.Abs(roundedCoord.y - floatCube.y);
            var zDiff = Mathf.Abs(roundedCoord.z - floatCube.z);

            if(xDiff > yDiff && xDiff > zDiff)
            {
                roundedCoord.x = -roundedCoord.y - roundedCoord.z;
            }else if (yDiff > zDiff)
            {
                roundedCoord.y = -roundedCoord.x - roundedCoord.z;
            }else
            {
                roundedCoord.z = -roundedCoord.x - roundedCoord.y;
            }

            return roundedCoord;
        }

    }
}
