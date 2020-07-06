using Assets.MapGen;
using Assets.MapGen.TileManagement;
using Assets.Scripts.MovementExtensions;
using Assets.UI.InfoPane;
using Assets.UI.SelectionManager;
using Simulation.Tiling;
using UnityEngine;

public class SelectableHexCells : MonoBehaviour, IFocusable
{
    public HexTileMapManager mapManager;
    public HexMember selectedMarker;
    public HexTileGenerator tileGenerator;

    public InfoPaneConfiguration GetInfoPaneConfiguration()
    {
        return null;
    }

    public void OnMeDeselected()
    {
        if(lastSelectedTile != default)
        {
            tileGenerator.ResetHexTilecolor(lastSelectedTile);
        }
    }

    private OffsetCoordinate lastSelectedTile;
    public void OnMeSelected(Vector3 pointHit)
    {
        var positionOnPlane = new Vector2(pointHit.x, pointHit.z);
        var hexCellCoordinate = mapManager.PositionInPlaneToTilemapPosition(positionOnPlane);

        Debug.Log($"Hex cell grid selected {hexCellCoordinate}");
        selectedMarker.PositionInTileMap = hexCellCoordinate;
        tileGenerator.SetHexTileColor(hexCellCoordinate, Color.Lerp(Color.red, Color.cyan, Random.value));

        this.lastSelectedTile = hexCellCoordinate;
    }
}
