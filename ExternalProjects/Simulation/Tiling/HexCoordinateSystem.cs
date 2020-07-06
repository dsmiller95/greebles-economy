using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Tiling
{
    public class HexCoordinateSystem
    {
        public float hexRadius;
        private readonly Vector2 displacementRatio = new Vector2(3f / 2f, Mathf.Sqrt(3));

        //private HexGrid backingGrid;

        public HexCoordinateSystem(float hexRadius)
        {
            //backingGrid = new HexGrid(hexRadius);
            this.hexRadius = hexRadius;
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
#pragma warning disable CS0618 // Type or member is obsolete
            var isInOffset = offsetCoordinates.IsInOffsetColumn();
#pragma warning restore CS0618 // Type or member is obsolete

            var agnosticCoords = Vector2.Scale(
                displacementRatio,
                new Vector2(
                    offsetCoordinates.column,
                    offsetCoordinates.row + (isInOffset ? 0.5f : 0)
                ));
            return agnosticCoords;
        }
        public Vector2 TileMapToReal(OffsetCoordinate offsetCoordinates)
        {
            return TileMapToRelative(offsetCoordinates) * hexRadius;
        }

        public OffsetCoordinate RelativeToTileMap(Vector2 relativePosition)
        {
            var cubicFloating = ConvertSizeScaledPointToFloatingCubic(relativePosition - new Vector2(0, displacementRatio.y / 2f));
            cubicFloating.z = -cubicFloating.x - cubicFloating.y;
            var cubicRounded = RoundToNearestCube(cubicFloating);
            var offsetCoords = cubicRounded.ToOffset();
            return new OffsetCoordinate(offsetCoords.column, -offsetCoords.row);
        }

        public OffsetCoordinate RealToTileMap(Vector2 realPosition)
        {
            var relativePositioning = realPosition / hexRadius;
            return RelativeToTileMap(relativePositioning);
        }

        public int DistanceBetweenInJumps(OffsetCoordinate origin, OffsetCoordinate destination)
        {
            var diff = destination - origin;
            var xOffset = Mathf.Abs(diff.column);
#pragma warning disable CS0618 // Type or member is obsolete
            var isFromOffsetPoint = origin.IsInOffsetColumn();
#pragma warning restore CS0618 // Type or member is obsolete

            var shouldPadX = diff.row > 0 ^ isFromOffsetPoint;
            return Mathf.Max(
                xOffset,
                Mathf.Abs(diff.row) +
                    Mathf.FloorToInt((xOffset + (shouldPadX ? 1 : 0)) / 2f)
                );
        }

        public IEnumerable<OffsetCoordinate> GetPositionsWithinJumpDistance(OffsetCoordinate origin, int jumpDistance)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var isOffset = origin.IsInOffsetColumn();
#pragma warning restore CS0618 // Type or member is obsolete
            var topHalfWidth = isOffset ? 1 : 0;
            var bottomHalfWidth = isOffset ? 0 : 1;
            var maxWidth = jumpDistance;
            var maxHeight = jumpDistance * 2;

            var heightOffset = -jumpDistance;

            for (var y = 0; y <= maxHeight; y++)
            {
                var topSlopeAmount = topHalfWidth + (maxHeight - y) * 2;
                var bottomSlopeAmount = bottomHalfWidth + y * 2;
                var currentHalfWidth = Mathf.Min(topSlopeAmount, bottomSlopeAmount, maxWidth);
                for (var x = -currentHalfWidth; x <= currentHalfWidth; x++)
                {
                    yield return new OffsetCoordinate(x, y + heightOffset) + origin;
                }
            }
        }

        public IEnumerable<Vector2Int> GetPositionsSpiralingAround(Vector2Int origin)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OffsetCoordinate> GetRouteGenerator(OffsetCoordinate origin, OffsetCoordinate destination)
        {
            var originPoint = origin;
            var destinationPoint = destination;

            var currentTileMapPos = originPoint;
            var iterations = 0;
            while (!(currentTileMapPos - destinationPoint).IsZero())
            {
                var realWorldVectorToDest = TileMapToRelative(destinationPoint)
                    - TileMapToRelative(currentTileMapPos);

#pragma warning disable CS0618 // Type or member is obsolete
                var nextMoveVector = GetClosestMatchingValidMove(realWorldVectorToDest, currentTileMapPos.IsInOffsetColumn());
#pragma warning restore CS0618 // Type or member is obsolete

                currentTileMapPos += nextMoveVector;

                yield return currentTileMapPos;
                iterations++;
                if (iterations > 1000)
                {
                    throw new Exception("too many loop brooother");
                }
            }
        }

        private OffsetCoordinate GetClosestMatchingValidMove(Vector2 worldSpaceDestinationVector, bool isInOffsetColumn)
        {
            //TODO: use the offsetCoordinate to get the neighbors
            var angle = Vector2.SignedAngle(Vector2.right, worldSpaceDestinationVector);
            if (0 <= angle && angle < 60)
            {
                return isInOffsetColumn ? new OffsetCoordinate(1, 1) : new OffsetCoordinate(1, 0);
            }
            if (60 <= angle && angle < 120)
            {
                return new OffsetCoordinate(0, 1);
            }
            if (120 <= angle && angle <= 180)
            {
                return isInOffsetColumn ? new OffsetCoordinate(-1, 1) : new OffsetCoordinate(-1, 0);
            }
            if (-180 <= angle && angle < -120)
            {
                return isInOffsetColumn ? new OffsetCoordinate(-1, 0) : new OffsetCoordinate(-1, -1);
            }
            if (-120 <= angle && angle < -60)
            {
                return new OffsetCoordinate(0, -1);
            }
            if (-60 <= angle && angle < 0)
            {
                return isInOffsetColumn ? new OffsetCoordinate(1, 0) : new OffsetCoordinate(1, -1);
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
