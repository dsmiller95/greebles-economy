using Assets.MapGen.TileManagement;
using Assets.Scripts.Castle;
using Assets.Scripts.MovementExtensions;
using Assets.Scripts.Resources;
using Assets.UI.SelectionManager;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx.Triggers;
using UnityEngine;

[Serializable]
public struct ResourceCost
{
    public ResourceType resource;
    public int cost;
}

public class CastleUIButtonActions : MonoBehaviour
{
    public CastleBehavior castle;

    public GameObject homePlaceObjectPrefab;
    public GameObject homePrefab;
    /// <summary>
    /// the cost of home ownership
    /// shouldn't put multiple costs for the same resource in this list
    /// </summary>
    public ResourceCost[] homeCost;


    public void StartBuildingTrader()
    {
    }
    public void StartBuildingHome()
    {
        var castleInventory = castle.inventory.backingInventory;
        var consumes = homeCost
            .Select(x => new { cost = x, purchase = castleInventory.Consume(x.resource, x.cost) }).ToList();
        if(consumes.Any(x => Mathf.Abs(x.cost.cost - x.purchase.info) > 1e-5))
        {
            Debug.LogWarning("You can't afford it ya shmuck");
            return;
        }

        foreach(var costConsume in consumes)
        {
            costConsume.purchase.Execute();
        }
        
        var placer = ObjectPlacer.instance;
        var hexMapParent = castle.GetComponentInParent<HexTileMapManager>().transform;
        var homePlaceObject = Instantiate(homePlaceObjectPrefab, hexMapParent);
        placer.PlaceObject(homePlaceObject, (coords) =>
        {
            var newItem = Instantiate(homePrefab, hexMapParent);
            var hexItem = newItem.GetComponentInChildren<HexMember>();
            hexItem.PositionInTileMap = coords;
        }, () =>
        {
            Destroy(homePlaceObject);
        });
    }
}
