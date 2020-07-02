using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.MapGen.TileManagement
{
    public class HexTileMapManager : MonoBehaviour
    {
        public float hexRadius;
        public int hexWidth;
        public int hexHeight;

        private readonly Vector2 displacementRatio = new Vector2(3f / 2f, Mathf.Sqrt(3));

        private IList<ITilemapMember>[][] tileGrid;
        public int tileMapHeight => tileGrid.Length;
        public int tileMapWidth => tileGrid[0].Length;


        public void Awake()
        {
            SetupGrid();
        }

        // Start is called before the first frame update
        void Start()
        {
        }


        // Update is called once per frame
        void Update()
        {
        }

        public Vector2 TileMapToReal(Vector2Int tileMapPosition)
        {
            var agnosticCoords = Vector2.Scale(displacementRatio, new Vector2(tileMapPosition.x, tileMapPosition.y + ((tileMapPosition.x % 2) / 2f)));
            return agnosticCoords;
        }

        public Vector2 TileMapPositionToPositionInPlane(Vector2Int tileMapPosition)
        {
            return TileMapToReal(tileMapPosition) * hexRadius;
        }

        private void SetupGrid()
        {
            var totalCells = new Vector2Int(hexWidth, hexHeight);
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

                var nextMoveVector = GetClosestMatchingValidMove(realWorldVectorToDest, !(currentTileMapPos.x % 2 == 0));

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
            var positionList = tileGrid[position.y][position.x];
            return positionList.Select(member => member.TryGetType<T>()).Where(x => x != null);
        }

        public void PlaceNewMapMember(ITilemapMember member)
        {
            this.RegisterInGrid(member);
        }

        public void RegisterInGrid(ITilemapMember item)
        {
            var position = item.PositionInTileMap;
            tileGrid[position.y][position.x].Add(item);
            item.UpdateWorldSpace();
        }
        public void DeRegisterInGrid(ITilemapMember member)
        {
            var position = member.PositionInTileMap;
            tileGrid[position.y][position.x].Remove(member);
        }
    }

}