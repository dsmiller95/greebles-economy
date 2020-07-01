using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.MapGen.TileManagement
{
    public class HexTileMapManager : MonoBehaviour
    {
        public float hexRadius;
        public float width;
        public float height;

        private readonly Vector2 displacementRatio = new Vector2(3f / 2f, Mathf.Sqrt(3));

        private IList<TileMapItem>[][] tileGrid;
        public int tileMapHeight => tileGrid.Length;
        public int tileMapWidth => tileGrid[0].Length;


        /// <summary>
        /// distance between each cell and the row of vertical cells next to it
        /// </summary>
        private float horizontalDisplacement;
        /// <summary>
        /// distance between each cell and the cell directly beloy it
        /// </summary>
        private float verticalDisplacement;

        public void Awake()
        {
            horizontalDisplacement = hexRadius * displacementRatio.x;
            verticalDisplacement = hexRadius * displacementRatio.y;
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
            var totalCells = new Vector2Int((int)(width / horizontalDisplacement), (int)(height / verticalDisplacement));
            tileGrid = new IList<TileMapItem>[totalCells.y][];
            for (var verticalIndex = 0; verticalIndex < totalCells.y; verticalIndex++)
            {
                tileGrid[verticalIndex] = new IList<TileMapItem>[totalCells.x];
                for (var horizontalIndex = 0; horizontalIndex < totalCells.x; horizontalIndex++)
                {
                    tileGrid[verticalIndex][horizontalIndex] = new List<TileMapItem>();
                }
            }
        }

        public bool IsWithinDistance(TileMapItem first, TileMapItem second, int distance)
        {
            return !GetRouteGenerator(first.positionInTileMap, second.positionInTileMap).Skip(distance).Any();
        }
        public int GetTileDistance(TileMapItem first, TileMapItem second)
        {
            return GetRouteGenerator(first.positionInTileMap, second.positionInTileMap).Count();
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

        public TileRoute GetRouteBetweenMembers(TileMapItem origin, TileMapItem destination)
        {
            return new TileRoute(this.GetRouteGenerator(origin.positionInTileMap, destination.positionInTileMap).ToList());
        }

        public TileRoute GetRouteBetweenMembers(ITilemapMember origin, ITilemapMember destination)
        {
            return GetRouteBetweenMembers(origin.GetMapItem(), destination.GetMapItem());
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
            return positionList.Select(item => item.member).OfType<T>();
        }

        public TileMapItem RegisterNewMapMember(ITilemapMember member, Vector2Int position)
        {
            return new TileMapItem(position, this, member);
        }

        private void RegisterInGrid(TileMapItem item)
        {
            var position = item.positionInTileMap;
            tileGrid[position.y][position.x].Add(item);
            item.member.UpdateWorldSpace();
        }
        private void DeRegisterInGrid(TileMapItem member)
        {
            var position = member.positionInTileMap;
            tileGrid[position.y][position.x].Remove(member);
        }

        private void MoveItem(TileMapItem member, Vector2Int newPosition)
        {
            DeRegisterInGrid(member);
            member.positionInTileMap = newPosition;
            RegisterInGrid(member);
        }

        public class TileMapItem
        {
            public Vector2Int positionInTileMap;
            private HexTileMapManager tileMapManager;
            public ITilemapMember member;

            internal TileMapItem(Vector2Int position, HexTileMapManager mapManager, ITilemapMember member)
            {
                this.member = member;
                positionInTileMap = position;
                tileMapManager = mapManager;
                member.SetMapItem(this);
                mapManager.RegisterInGrid(this);
            }

            public void SetPositionInTileMap(Vector2Int position)
            {
                tileMapManager.MoveItem(this, position);
            }

            public Vector2 PositionInTilePlane => tileMapManager.TileMapPositionToPositionInPlane(positionInTileMap);
            public HexTileMapManager MapManager => tileMapManager;
        }
    }

}