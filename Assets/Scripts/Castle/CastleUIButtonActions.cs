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

    public GameObject homePlaceObject;
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
            Debug.LogWarning("Ya can't afford it ya shmuck");
            return;
        }

        foreach(var costConsume in consumes)
        {
            costConsume.purchase.Execute();
        }
        

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
