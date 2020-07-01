using Assets.MapGen.TileManagement;
using UnityEngine;
using static Assets.MapGen.TileManagement.HexTileMapManager;

namespace Assets.Scripts.MovementExtensions
{
    public class HexMember : MonoBehaviour, ITilemapMember
    {
        public HexTileMapManager tilemapManager;
        public Vector2Int startingPosition;


        // Start is called before the first frame update
        void Start()
        {
            Debug.Log($"Registered {gameObject.name} at {startingPosition}");
            tilemapManager.RegisterNewMapMember(this, startingPosition);
        }

        public Vector2Int Position => tileGridItem?.positionInTileMap ?? this.startingPosition;

        #region ITileMapMember
        protected TileMapItem tileGridItem;
        public void SetMapItem(TileMapItem item)
        {
            tileGridItem = item;
        }
        public TileMapItem GetMapItem()
        {
            return tileGridItem;
        }

        public void UpdateWorldSpace()
        {
            //var positionOfManager = tilemapManager.transform.position;

            var placeSpace = tileGridItem.PositionInTilePlane;
            Debug.Log($"Updated world space of {gameObject.name} to {placeSpace} : {tileGridItem.positionInTileMap}");
            var placementInsideTilemap = new Vector3(placeSpace.x, 0, placeSpace.y);
            transform.position = tilemapManager.transform.TransformPoint(placementInsideTilemap);
        }
        #endregion
    }
}