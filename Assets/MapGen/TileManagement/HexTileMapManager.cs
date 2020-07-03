﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.MapGen.TileManagement
{
    public class HexTileMapManager : MonoBehaviour
    {
        public float hexRadius;

        public Vector2Int tileMapMin;
        /// <summary>
        /// how many hexes wide this map is
        /// </summary>
        public int hexWidth;
        /// <summary>
        /// how many hexes tall this map is
        /// </summary>
        public int hexHeight;

        [HideInInspector()]
        public Vector2Int tileMapMax;

        public int tileMapHeight => tileGrid.Length;
        public int tileMapWidth => tileGrid[0].Length;


        private readonly Vector2 displacementRatio = new Vector2(3f / 2f, Mathf.Sqrt(3));
        private IList<ITilemapMember>[][] tileGrid;

        public void Awake()
        {
            var totalCells = new Vector2Int(hexWidth, hexHeight);
            tileMapMax = tileMapMin + totalCells;

            tileGrid = new IList<ITilemapMember>[totalCells.y][];
            for (var verticalIndex = 0; verticalIndex < totalCells.y; verticalIndex++)
            {
                tileGrid[verticalIndex] = new IList<ITilemapMember>[totalCells.x];
                for (var horizontalIndex = 0; horizontalIndex < totalCells.x; horizontalIndex++)
                {
                    tileGrid[verticalIndex][horizontalIndex] = new List<ITilemapMember>();
                }
            }
        }

        public bool IsInOffsetColumn(Vector2Int tileMapPos)
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


        public bool IsWithinDistance(ITilemapMember first, ITilemapMember second, int distance)
        {
            return !GetRouteGenerator(first.PositionInTileMap, second.PositionInTileMap).Skip(distance).Any();
        }
        public int GetTileDistance(ITilemapMember first, ITilemapMember second)
        {
            return GetRouteGenerator(first.PositionInTileMap, second.PositionInTileMap).Count();
        }

        private IEnumerable<Vector2Int> GetRouteGenerator(Vector2Int origin, Vector2Int destination)
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

        public IEnumerable<T> GetItemsWithinJumpDistance<T>(Vector2Int origin, int jumpDistance)
        {
            return GetPositionsWithinJumpDistance(origin, jumpDistance)
                .Select(position => GetItemsAtLocation<T>(position))
                .Where(items => items != null)
                .SelectMany(items => items)
                .Where(x => x != null);
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

        public TileRoute GetRouteBetweenMembers(ITilemapMember origin, ITilemapMember destination)
        {
            return new TileRoute(GetRouteGenerator(origin.PositionInTileMap, destination.PositionInTileMap).ToList());
        }

        public Vector2Int GetClosestMatchingValidMove(Vector2 worldSpaceDestinationVector, bool isInOffsetColumn)
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

        private IList<ITilemapMember> GetListFromCoord(Vector2Int coordinates)
        {
            var positionInGrid = coordinates - tileMapMin;
            if (positionInGrid.x < 0 || positionInGrid.y < 0 || positionInGrid.y >= tileMapHeight || positionInGrid.x >= tileMapWidth)
            {
                return null;
            }

            return tileGrid[positionInGrid.y][positionInGrid.x];
        }

        public IEnumerable<T> GetMembersAtLocation<T>(Vector2Int position, Func<T, bool> filter) where T :ITilemapMember
        {
            var positionList = GetListFromCoord(position);
            return positionList?
                .OfType<T>()
                .Where(member => filter(member));
        }
        public IEnumerable<T> GetMembersAtLocation<T>(Vector2Int position) where T : ITilemapMember
        {
            var positionList = GetListFromCoord(position);
            return positionList?
                .OfType<T>();
        }

        public IEnumerable<T> GetItemsAtLocation<T>(Vector2Int position)
        {
            var positionList = GetListFromCoord(position);
            return positionList?
                .Select(member => member.TryGetType<T>())
                .Where(x => x != null);
        }

        public void PlaceNewMapMember(ITilemapMember member)
        {
            RegisterInGrid(member);
        }

        public void RegisterInGrid(ITilemapMember item)
        {
            var position = item.PositionInTileMap;
            GetListFromCoord(position)?.Add(item);
            item.UpdateWorldSpace();
        }
        public void DeRegisterInGrid(ITilemapMember member)
        {
            var position = member.PositionInTileMap;
            GetListFromCoord(position)?.Remove(member);
        }
    }

}