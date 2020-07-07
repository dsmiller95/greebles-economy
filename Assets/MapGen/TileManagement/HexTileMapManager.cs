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

        public OffsetCoordinate tileMapMin;
        /// <summary>
        /// how many hexes wide this map is
        /// </summary>
        public int hexWidth;
        /// <summary>
        /// how many hexes tall this map is
        /// </summary>
        public int hexHeight;

        [HideInInspector()]
        public OffsetCoordinate tileMapMax;

        public int tileMapHeight => tileGrid.Length;
        public int tileMapWidth => tileGrid[0].Length;

        private IList<ITilemapMember>[][] tileGrid;

        private HexCoordinateSystem coordinateSystem;

        public void Awake()
        {
            this.coordinateSystem = new HexCoordinateSystem(this.hexRadius);
            tileMapMax = new OffsetCoordinate(tileMapMin.column + hexWidth, tileMapMin.row + hexHeight);

            tileGrid = new IList<ITilemapMember>[hexHeight][];
            for (var verticalIndex = 0; verticalIndex < hexHeight; verticalIndex++)
            {
                tileGrid[verticalIndex] = new IList<ITilemapMember>[hexWidth];
                for (var horizontalIndex = 0; horizontalIndex < hexWidth; horizontalIndex++)
                {
                    tileGrid[verticalIndex][horizontalIndex] = new List<ITilemapMember>();
                }
            }
        }

        #region hex coordinate system
        public Vector2 TileMapToReal(AxialCoordinate tileMapPosition)
        {
            return this.coordinateSystem.TileMapToRelative(tileMapPosition);
        }
        public Vector2 TileMapPositionToPositionInPlane(AxialCoordinate tileMapPosition)
        {
            return this.coordinateSystem.TileMapToReal(tileMapPosition);
        }
        public AxialCoordinate PositionInPlaneToTilemapPosition(Vector2 positionInPlane)
        {
            return this.coordinateSystem.RealToTileMap(positionInPlane).ToAxial();
        }
        public static IEnumerable<AxialCoordinate> GetPositionsWithinJumpDistance(AxialCoordinate origin, int jumpDistance)
        {
            return HexCoordinateSystem.GetPositionsWithinJumpDistance(origin, jumpDistance);
        }
        public bool IsWithinDistance(ITilemapMember first, ITilemapMember second, int distance)
        {
            //TODO: replace with distance function
            return !coordinateSystem.GetRouteGenerator(first.PositionInTileMap, second.PositionInTileMap).Skip(distance).Any();
        }
        public TileRoute GetRouteBetweenMembers(ITilemapMember origin, ITilemapMember destination)
        {
            return new TileRoute(coordinateSystem.GetRouteGenerator(origin.PositionInTileMap, destination.PositionInTileMap).ToList());
        }
        #endregion

        public IEnumerable<T> GetItemsWithinJumpDistance<T>(AxialCoordinate origin, int jumpDistance)
        {
            return GetPositionsWithinJumpDistance(origin, jumpDistance)
                .Select(position => GetItemsAtLocation<T>(position))
                .Where(items => items != null)
                .SelectMany(items => items)
                .Where(x => x != null);
        }



        private IList<ITilemapMember> GetListFromCoord(OffsetCoordinate coordinate)
        {
            var vectorInGrid = new Vector2Int(coordinate.column - tileMapMin.column, coordinate.row - tileMapMin.row);
            if (vectorInGrid.x < 0 || vectorInGrid.y < 0 || vectorInGrid.y >= tileMapHeight || vectorInGrid.x >= tileMapWidth)
            {
                return null;
            }

            return tileGrid[vectorInGrid.y][vectorInGrid.x];
        }

        private IList<ITilemapMember> GetListFromCoord(AxialCoordinate coordinates)
        {
            return this.GetListFromCoord(coordinates.ToOffset());
        }

        public IEnumerable<T> GetMembersAtLocation<T>(AxialCoordinate position, Func<T, bool> filter) where T : ITilemapMember
        {
            var positionList = GetListFromCoord(position);
            return positionList?
                .OfType<T>()
                .Where(member => filter(member));
        }
        public IEnumerable<T> GetMembersAtLocation<T>(AxialCoordinate position) where T : ITilemapMember
        {
            var positionList = GetListFromCoord(position);
            return positionList?
                .OfType<T>();
        }

        public IEnumerable<T> GetItemsAtLocation<T>(AxialCoordinate position)
        {
            var positionList = GetListFromCoord(position);
            return positionList?
                .Select(member => member.TryGetType<T>())
                .Where(x => x != null);
        }

        public IEnumerable<ITilemapMember> GetAllMembers()
        {
            return this.GetAllMemberInternal().SelectMany(x => x);
        }
        private IEnumerable<IEnumerable<ITilemapMember>> GetAllMemberInternal()
        {
            for (var row = 0; row < tileGrid.Length; row++)
            {
                for (var col = 0; col < tileGrid[row].Length; col++)
                {
                    yield return tileGrid[row][col];
                }
            }
        }
        public IEnumerable<T> GetAllOfType<T>()
        {
            return this.GetAllMembers()
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