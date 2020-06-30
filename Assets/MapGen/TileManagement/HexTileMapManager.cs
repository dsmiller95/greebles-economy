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

        public Vector2 TileMapPositionToAgnosticCoords(Vector2Int tileMapPosition)
        {
            var agnosticCoords = Vector2.Scale(displacementRatio, new Vector2(tileMapPosition.x, tileMapPosition.y + ((tileMapPosition.x % 2) / 2f)));
            return agnosticCoords;
        }

        public Vector2 TileMapPositionToPositionInPlane(Vector2Int tileMapPosition)
        {
            return TileMapPositionToAgnosticCoords(tileMapPosition) * hexRadius;
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

        public TileRoute GetRouteBetweenMembers(TileMapItem origin, TileMapItem destination)
        {
            var result = new TileRoute();

            var originPoint = origin.positionInTileMap;
            var destinationPoint = destination.positionInTileMap;

            var xSign = (int)Mathf.Sign(destinationPoint.x - originPoint.x);
            for (var x = originPoint.x; x != destinationPoint.x; x += xSign)
            {
                result.AddLastWaypoint(new Vector2Int(x, originPoint.y));
            }

            var ySign = (int)Mathf.Sign(destinationPoint.y - originPoint.y);
            for (var y = originPoint.y; y != destinationPoint.y; y += ySign)
            {
                result.AddLastWaypoint(new Vector2Int(destinationPoint.x, y));
            }

            return result;
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