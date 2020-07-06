using Assets.MapGen;
using Assets.MapGen.TileManagement;
using Assets.Scripts.MovementExtensions;
using Simulation.Tiling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    private HaltonSequenceGenerator pointGenerator;
    // Start is called before the first frame update
    void Start()
    {
        var spawnArea = spawnBoxSize.x * spawnBoxSize.y;
        var seed = Random.Range(100, 1000);
        pointGenerator = new HaltonSequenceGenerator(2, 3, seed, spawnBoxSize + new Vector2Int(1, 1));

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
        hexItem.localPosition = newPosition;
    }

    private OffsetCoordinate getRandomPosInBounds()
    {
        var nextPoint = pointGenerator.Sample();

        return new OffsetCoordinate(
            Mathf.FloorToInt(nextPoint.x),
            Mathf.FloorToInt(nextPoint.y));
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
        //Gizmos.DrawCube(transform.position, spawnBoxSize);
    }
}
