using Assets.MapGen.TileManagement;
using Assets.Scripts.MovementExtensions;
using Assets.UI.InfoPane;
using Assets.UI.SelectionManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableHexCells : MonoBehaviour, IFocusable
{
    public HexTileMapManager mapManager;
    public HexMember selectedMarker;

    public InfoPaneConfiguration GetInfoPaneConfiguration()
    {
        return null;
    }

    public void OnMeDeselected()
    {

    }

    public void OnMeSelected(Vector3 pointHit)
    {
        var positionOnPlane = new Vector2(pointHit.x, pointHit.z);
        var hexCellCoordinate = mapManager.PositionInPlaneToTilemapPosition(positionOnPlane);
        Debug.Log($"Hex cell grid selected {hexCellCoordinate}");
        selectedMarker.PositionInTileMap = hexCellCoordinate;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
