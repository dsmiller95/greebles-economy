using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.MapGen.TileManagement
{
    public class HexTileMapManager : MonoBehaviour
    {
        public float hexRadius;

        public Vector2Int tileMapMin;
        public int hexWidth;
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

        private IList<ITilemapMember> GetListFromCoord(Vector2Int coordinates)
        {
            var positionInGrid = coordinates - tileMapMin;  
            return tileGrid[positionInGrid.y][positionInGrid.x];
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

        public TileRoute GetRouteBetweenMembers(ITilemapMember origin, ITilemapMember destination)
        {
            return new TileRoute(this.GetRouteGenerator(origin.PositionInTileMap, destination.PositionInTileMap).ToList());
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

        public IEnumerable<T> GetItemsAtLocation<T>(Vector2Int position) 
        {
            var positionList = GetListFromCoord(position);
            return positionList.Select(member => member.TryGetType<T>()).Where(x => x != null);
        }

        public void PlaceNewMapMember(ITilemapMember member)
        {
            this.RegisterInGrid(member);
        }

        public void RegisterInGrid(ITilemapMember item)
        {
            var position = item.PositionInTileMap;
            GetListFromCoord(position).Add(item);
            item.UpdateWorldSpace();
        }
        public void DeRegisterInGrid(ITilemapMember member)
        {
            var position = member.PositionInTileMap;
            GetListFromCoord(position).Remove(member);
        }
    }

}