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
            tilemapManager.RegisterNewMapMember(this, startingPosition);
        }

        public Vector2Int Position => tileGridItem?.positionInTileMap ?? this.startingPosition;

        #region ITileMapMember
        protected TileMapItem tileGridItem;
        public void SetMapItem(TileMapItem item)
        {
            tileGridItem = item;
            this.tilemapManager = item.MapManager;
        }
        public TileMapItem GetMapItem()
        {
            return tileGridItem;
        }

        public void UpdateWorldSpace()
        {
            var placeSpace = tileGridItem.PositionInTilePlane;
            var worldPlacement = new Vector3(placeSpace.x, 0, placeSpace.y);
            transform.localPosition = worldPlacement;
        }
        #endregion
    }
}