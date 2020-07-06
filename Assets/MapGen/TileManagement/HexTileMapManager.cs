﻿using Simulation.Tiling;
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
            return this.coordinateSystem.TileMapToRelative(tileMapPosition.ToOffset());
        }
        public Vector2 TileMapPositionToPositionInPlane(AxialCoordinate tileMapPosition)
        {
            return this.coordinateSystem.TileMapToReal(tileMapPosition.ToCube().ToOffset());
        }
        public AxialCoordinate PositionInPlaneToTilemapPosition(Vector2 positionInPlane)
        {
            return this.coordinateSystem.RealToTileMap(positionInPlane).ToAxial();
        }
        public IEnumerable<AxialCoordinate> GetPositionsWithinJumpDistance(AxialCoordinate origin, int jumpDistance)
        {
            return coordinateSystem.GetPositionsWithinJumpDistance(origin.ToOffset(), jumpDistance).Select(x => x.ToAxial());
        }
        public IEnumerable<Vector2Int> GetInfinitePositionsInJumpDistanceOrder(Vector2Int origin)
        {
            return coordinateSystem.GetPositionsSpiralingAround(origin);
        }
        public bool IsWithinDistance(ITilemapMember first, ITilemapMember second, int distance)
        {
            //TODO: replace with distance function
            return !coordinateSystem.GetRouteGenerator(first.PositionInTileMap, second.PositionInTileMap).Skip(distance).Any();
        }
        public int DistanceBetweenInJumps(AxialCoordinate origin, AxialCoordinate destination)
        {
            return coordinateSystem.DistanceBetweenInJumps(origin.ToOffset(), destination.ToOffset());
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

        private IList<ITilemapMember> GetListFromCoord(AxialCoordinate coordinates)
        {
            var offsetCoordinates = coordinates.ToOffset();
            var vectorInGrid = new Vector2Int(offsetCoordinates.column - tileMapMin.column, offsetCoordinates.row - tileMapMin.row);
            if (vectorInGrid.x < 0 || vectorInGrid.y < 0 || vectorInGrid.y >= tileMapHeight || vectorInGrid.x >= tileMapWidth)
            {
                return null;
            }

            return tileGrid[vectorInGrid.y][vectorInGrid.x];
        }

        public IEnumerable<T> GetMembersAtLocation<T>(AxialCoordinate position, Func<T, bool> filter) where T :ITilemapMember
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