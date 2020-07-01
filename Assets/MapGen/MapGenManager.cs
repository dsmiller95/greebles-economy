using Assets.MapGen.TileManagement;
using Assets.Scripts.MovementExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapGenSpawnable
{
    public GameObject prefab;
    public float densityPerSurfaceSize;
}

[RequireComponent(typeof(HexMember))]
public class MapGenManager : MonoBehaviour
{
    public MapGenSpawnable[] spawnableItems;
    public HexTileMapManager tileManager;

    public Vector2Int spawnBoxSize = new Vector2Int(3, 3);

    private HexMember hexMember;

    public void Awake()
    {
        hexMember = GetComponent<HexMember>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var spawnArea = spawnBoxSize.x * spawnBoxSize.y;
        foreach(var spawnable in spawnableItems)
        {
            this.SpawnItemsForSpawnable(spawnable, spawnArea);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SpawnItemsForSpawnable(MapGenSpawnable spawnable, float spawnBoxArea)
    {
        var totalSpawns = spawnBoxArea * spawnable.densityPerSurfaceSize;
        for(var i = 0; i < totalSpawns; i++)
        {
            this.SpawnItem(spawnable.prefab);
        }
    }

    private void SpawnItem(GameObject prefab)
    {
        var newItem = Instantiate(prefab, transform);
        var hexItem = newItem.GetComponentInChildren<HexMember>();
        var newPosition = getRandomPosInBounds();
        Debug.Log($"spawning map feature at {newPosition}");
        hexItem.startingPosition = newPosition;
        hexItem.tilemapManager = this.hexMember.tilemapManager;
        //this.tileManager.RegisterNewMapMember(hexItem, newPosition);
    }

    private Vector2Int getRandomPosInBounds()
    {
        return new Vector2Int(
            Random.Range(0, this.spawnBoxSize.x),
            Random.Range(0, this.spawnBoxSize.y)) + this.hexMember.Position;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
        //Gizmos.DrawCube(transform.position, spawnBoxSize);
    }
}
