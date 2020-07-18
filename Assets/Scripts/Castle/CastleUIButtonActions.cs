using Assets.MapGen.TileManagement;
using Assets.Scripts.Castle;
using Assets.Scripts.MovementExtensions;
using Assets.UI.SelectionManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleUIButtonActions : MonoBehaviour
{
    public CastleBehavior castle;

    public GameObject homePlaceObject;
    public GameObject homePrefab;

    public void StartBuildingTrader()
    {
    }

    public void StartBuildingHome()
    {
        Debug.Log("home building starting");
        var placer = ObjectPlacer.instance;
        placer.PlaceObject(homePlaceObject, (coords) =>
        {
            Debug.Log("home buildt");
            Debug.Log(coords);

            var newItem = Instantiate(homePrefab, castle.GetComponentInParent<HexTileMapManager>().transform);
            var hexItem = newItem.GetComponentInChildren<HexMember>();
            hexItem.localPosition = coords;
        });
    }
}
