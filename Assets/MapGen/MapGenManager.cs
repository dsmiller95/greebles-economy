using Assets.MapGen;
using Assets.MapGen.TileManagement;
using Assets.Scripts.Market;
using Assets.Scripts.MovementExtensions;
using Simulation.Tiling;
using UnityEngine;
using UnityEngine.Video;

[System.Serializable]
public struct MapGenSpawnable
{
    public GameObject prefab;
    public float densityPerSurfaceSize;
    /// <summary>
    /// Set to 0 or 1 to spawn across the whole surface. represents the percentage size of the map
    /// that should be covered by this spawn
    /// </summary>
    public float scaleFactor;
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
        pointGenerator = new HaltonSequenceGenerator(2, 3, seed);

        foreach (var spawnable in spawnableItems)
        {
            SpawnItemsForSpawnable(
                spawnable,
                spawnArea * (
                    spawnable.scaleFactor == 0 ?
                        1 :
                        (spawnable.scaleFactor * spawnable.scaleFactor)
                ));
        }
        timeOfMapGen = Time.time;
    }

    private void MapGenCompleted()
    {
        MarketDataInitializer.CalculateServiceRanges(tileManager);
    }

    private float timeOfMapGen;
    private bool isFirstUpdate = true;

    // Update is called once per frame
    void Update()
    {
        if (isFirstUpdate && (timeOfMapGen + 1 < Time.time))
        {
            isFirstUpdate = false;
            Debug.Log("Map gen completed");
            //TODO: there has to be a better way :(
            MapGenCompleted();
        }
    }

    private void SpawnItemsForSpawnable(MapGenSpawnable spawnable, float spawnBoxArea)
    {
        var totalSpawns = spawnBoxArea * spawnable.densityPerSurfaceSize;
        for (var i = 0; i < totalSpawns; i++)
        {
            SpawnItem(spawnable);
        }
    }

    private void SpawnItem(MapGenSpawnable spawnable)
    {
        var newItem = Instantiate(spawnable.prefab, transform);
        var hexItem = newItem.GetComponentInChildren<HexMember>();
        var newPosition = getRandomPosInBounds(spawnable.scaleFactor);
        hexItem.localPosition = newPosition;
    }

    private AxialCoordinate getRandomPosInBounds(float scaleFactor)
    {
        Vector2 nextPoint;
        if(scaleFactor == 0)
        {
            nextPoint = pointGenerator.Sample(spawnBoxSize + new Vector2Int(1, 1));
        }else
        {
            var maxSize = spawnBoxSize + new Vector2Int(1, 1);
            var newMin = ((Vector2)maxSize) * ((1 - scaleFactor) / 2f);
            var newMax = ((Vector2)maxSize) * ((1 + scaleFactor) / 2f);

            nextPoint = pointGenerator.Sample(newMax, newMin);
        }

        return new OffsetCoordinate(
            Mathf.FloorToInt(nextPoint.x),
            Mathf.FloorToInt(nextPoint.y)).ToAxial();
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
        //Gizmos.DrawCube(transform.position, spawnBoxSize);
    }
}
