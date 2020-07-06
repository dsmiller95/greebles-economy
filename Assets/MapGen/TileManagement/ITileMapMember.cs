using Simulation.Tiling;
using UnityEngine;

namespace Assets.MapGen.TileManagement
{
    public interface ITilemapMember
    {
        Vector2 PositionInTilePlane { get; }
        HexTileMapManager MapManager { get; set; }
        OffsetCoordinate PositionInTileMap { get; set; }
        T TryGetType<T>();
    }
}
