using Simulation.Tiling;
using UnityEngine;

namespace Assets.MapGen.TileManagement
{
    public interface ITilemapMember
    {
        Vector2 PositionInTilePlane { get; }

        /// <summary>
        /// sets whether this member is searchable through the map manager
        ///     if set to false, then the only way to gain access to this member is via reference
        ///     if set to true, this member can be found by querying a location or range of locations
        ///         in the map manager
        /// </summary>
        bool DoesRegisterInIndex { get; }
        HexTileMapManager MapManager { get; set; }
        AxialCoordinate PositionInTileMap { get; set; }
        T TryGetType<T>();
    }
}
