using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Simulation.Tiling
{
    public class HexCoordinateSystem
    {
        public float hexRadius;
        private readonly Vector2 displacementRatio = new Vector2(3f / 2f, Mathf.Sqrt(3));

        public HexCoordinateSystem(float hexRadius)
        {
            this.hexRadius = hexRadius;
        }

        [Obsolete("Should not be used by external classes, this is only public for unit testing purposes")]
        public bool IsInOffsetColumn(int xPosition)
        {
            return Math.Abs(xPosition) % 2 == 0;
        }

        /// <summary>
        /// Translate a tile map coordinate to a standard "real" position. this is not scaled based
        ///     on the size of the hexes. only use it for calculations that need to know about
        ///     relative positioning of hex members as opposed to real positioning
        /// </summary>
        /// <param name="tileMapPosition"></param>
        /// <returns></returns>
        public Vector2 TileMapToRelative(Vector2Int tileMapPosition)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var isInOffset = IsInOffsetColumn(tileMapPosition.x);
#pragma warning restore CS0618 // Type or member is obsolete

            var agnosticCoords = Vector2.Scale(
                displacementRatio,
                new Vector2(
                    tileMapPosition.x,
                    tileMapPosition.y + (isInOffset ? 0.5f : 0)
                ));
            return agnosticCoords;
        }
        public Vector2 TileMapToReal(Vector2Int tileMapPosition)
        {
            return TileMapToRelative(tileMapPosition) * hexRadius;
        }

        public Vector2Int RelativeToTileMap(Vector2 relativePosition)
        {
            var whichColumn = Mathf.RoundToInt(relativePosition.x / displacementRatio.x);
#pragma warning disable CS0618 // Type or member is obsolete
            var isInOffset = IsInOffsetColumn(whichColumn);
#pragma warning restore CS0618 // Type or member is obsolete
            var yOffset = (isInOffset ? 0.5f : 0) * displacementRatio.y;
            var whichRow = Mathf.RoundToInt((relativePosition.y - yOffset) / displacementRatio.y);

            return new Vector2Int(whichColumn, whichRow);
        }

        public Vector2Int RealToTileMap(Vector2 realPosition)
        {
            var relativePositioning = realPosition / hexRadius;
            return this.RelativeToTileMap(relativePositioning);
        }

        public int DistanceBetweenInJumps(Vector2Int origin, Vector2Int destination)
        {
            var diff = destination - origin;
            var xOffset = Mathf.Abs(diff.x);
#pragma warning disable CS0618 // Type or member is obsolete
            var isFromOffsetPoint = IsInOffsetColumn(origin.x);
#pragma warning restore CS0618 // Type or member is obsolete

            var shouldPadX = diff.y > 0 ^ isFromOffsetPoint;
            return Mathf.Max(
                xOffset,
                Mathf.Abs(diff.y) +
                    Mathf.FloorToInt((xOffset + (shouldPadX ? 1 : 0)) / 2f)
                );
        }

        public IEnumerable<Vector2Int> GetPositionsWithinJumpDistance(Vector2Int origin, int jumpDistance)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var isOffset = IsInOffsetColumn(origin.x);
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
                var realWorldVectorToDest = TileMapToRelative(destinationPoint)
                    - TileMapToRelative(currentTileMapPos);

#pragma warning disable CS0618 // Type or member is obsolete
                var nextMoveVector = GetClosestMatchingValidMove(realWorldVectorToDest, IsInOffsetColumn(currentTileMapPos.x));
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
    }
}
