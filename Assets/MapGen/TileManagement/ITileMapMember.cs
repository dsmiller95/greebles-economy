using UnityEngine;

namespace Assets.MapGen.TileManagement
{
    public interface ITilemapMember
    {
        Vector2 PositionInTilePlane { get; }
        HexTileMapManager MapManager { get; set; }
        Vector2Int PositionInTileMap { get; set; }
        void UpdateWorldSpace();
        T TryGetType<T>();
    }
}
