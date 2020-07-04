using Simulation.Tiling;
using System;
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

        private IList<ITilemapMember>[][] tileGrid;

        private HexCoordinateSystem coordinateSystem;

        public void Awake()
        {
            this.coordinateSystem = new HexCoordinateSystem(this.hexRadius);
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

        #region hex coordinate system
        public Vector2 TileMapToReal(Vector2Int tileMapPosition)
        {
            return this.coordinateSystem.TileMapToRelative(tileMapPosition);
        }
        public Vector2 TileMapPositionToPositionInPlane(Vector2Int tileMapPosition)
        {
            return this.coordinateSystem.TileMapToReal(tileMapPosition);
        }
        public IEnumerable<Vector2Int> GetPositionsWithinJumpDistance(Vector2Int origin, int jumpDistance)
        {
            return coordinateSystem.GetPositionsWithinJumpDistance(origin, jumpDistance);
        }
        public bool IsWithinDistance(ITilemapMember first, ITilemapMember second, int distance)
        {
            //TODO: replace with distance function
            return !coordinateSystem.GetRouteGenerator(first.PositionInTileMap, second.PositionInTileMap).Skip(distance).Any();
        }
        public int DistanceBetweenInJumps(Vector2Int origin, Vector2Int destination)
        {
            return coordinateSystem.DistanceBetweenInJumps(origin, destination);
        }
        public TileRoute GetRouteBetweenMembers(ITilemapMember origin, ITilemapMember destination)
        {
            return new TileRoute(coordinateSystem.GetRouteGenerator(origin.PositionInTileMap, destination.PositionInTileMap).ToList());
        }
        #endregion

        public IEnumerable<T> GetItemsWithinJumpDistance<T>(Vector2Int origin, int jumpDistance)
        {
            return GetPositionsWithinJumpDistance(origin, jumpDistance)
                .Select(position => GetItemsAtLocation<T>(position))
                .Where(items => items != null)
                .SelectMany(items => items)
                .Where(x => x != null);
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
        }
        public void DeRegisterInGrid(ITilemapMember member)
        {
            var position = member.PositionInTileMap;
            GetListFromCoord(position)?.Remove(member);
        }
    }

}