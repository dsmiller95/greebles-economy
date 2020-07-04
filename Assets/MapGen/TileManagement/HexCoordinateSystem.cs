using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.MapGen.TileManagement
{
    class HexCoordinateSystem
    {
        public float hexRadius;
        private readonly Vector2 displacementRatio = new Vector2(3f / 2f, Mathf.Sqrt(3));

        public HexCoordinateSystem(float hexRadius)
        {
            this.hexRadius = hexRadius;
        }

        private bool IsInOffsetColumn(Vector2Int tileMapPos)
        {
            return Math.Abs(tileMapPos.x) % 2 == 0;
        }

        public Vector2 TileMapToReal(Vector2Int tileMapPosition)
        {
            var positionInTileGrid = tileMapPosition;
            var agnosticCoords = Vector2.Scale(
                displacementRatio,
                new Vector2(
                    positionInTileGrid.x,
                    positionInTileGrid.y + (IsInOffsetColumn(positionInTileGrid) ? 0 : 0.5f)
                ));
            return agnosticCoords;
        }
        public Vector2 TileMapPositionToPositionInPlane(Vector2Int tileMapPosition)
        {
            return TileMapToReal(tileMapPosition) * hexRadius;
        }

        private Vector2Int GetClosestMatchingValidMove(Vector2 worldSpaceDestinationVector, bool isInOffsetColumn)
        {
            var angle = Vector2.SignedAngle(Vector2.right, worldSpaceDestinationVector);
            if (0 <= angle && angle < 60)
            {
                return isInOffsetColumn ? new Vector2Int(1, 1) : Vector2Int.right;
            }
            if (60 <= angle && angle < 120)
            {
                return Vector2Int.up;
            }
            if (120 <= angle && angle <= 180)
            {
                return isInOffsetColumn ? new Vector2Int(-1, 1) : new Vector2Int(-1, 0);
            }
            if (-180 <= angle && angle < -120)
            {
                return isInOffsetColumn ? new Vector2Int(-1, 0) : new Vector2Int(-1, -1);
            }
            if (-120 <= angle && angle < -60)
            {
                return Vector2Int.down;
            }
            if (-60 <= angle && angle < 0)
            {
                return isInOffsetColumn ? new Vector2Int(1, 0) : new Vector2Int(1, -1);
            }
            throw new Exception($"error in angle matching {angle}");
        }

        public int DistanceBetweenInJumps(Vector2Int origin, Vector2Int destination)
        {
            var diff = destination - origin;
            var xOffset = Mathf.Abs(diff.x);
            var isFromOffsetPoint = !IsInOffsetColumn(origin);

            var shouldPadX = diff.y > 0 ^ isFromOffsetPoint;
            return Mathf.Max(
                xOffset,
                Mathf.Abs(diff.y) +
                    Mathf.FloorToInt((xOffset + (shouldPadX ? 1 : 0)) / 2f)
                );
        }

        public IEnumerable<Vector2Int> GetPositionsWithinJumpDistance(Vector2Int origin, int jumpDistance)
        {
            var isOffset = IsInOffsetColumn(origin);
            var topHalfWidth = isOffset ? 0 : 1;
            var bottomHalfWidth = isOffset ? 1 : 0;
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
                    yield return new Vector2Int(x, y + heightOffset) + origin;
                }
            }
        }

        public IEnumerable<Vector2Int> GetRouteGenerator(Vector2Int origin, Vector2Int destination)
        {
            var originPoint = origin;
            var destinationPoint = destination;

            var currentTileMapPos = originPoint;
            var iterations = 0;
            while ((currentTileMapPos - destinationPoint).sqrMagnitude > 0)
            {
                var realWorldVectorToDest = TileMapToReal(destinationPoint)
                    - TileMapToReal(currentTileMapPos);

                var nextMoveVector = GetClosestMatchingValidMove(realWorldVectorToDest, !IsInOffsetColumn(currentTileMapPos));

                currentTileMapPos += nextMoveVector;

                yield return currentTileMapPos;
                iterations++;
                if (iterations > 1000)
                {
                    throw new Exception("too many loop brooother");
                }
            }
        }

        public int GetTileDistance(Vector2Int first, Vector2Int second)
        {
            return GetRouteGenerator(first, second).Count();
        }
    }
}
