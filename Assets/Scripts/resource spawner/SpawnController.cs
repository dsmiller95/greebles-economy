using Assets.MapGen.TileManagement;
using Simulation.Tiling.HexCoords;
using System.Linq;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public int maximumConcurrentSpawned = 10;
    public float spawnProbability = 0.5f;
    public int initialSpawnNumber = 1;

    [Header("Boost Settings")]
    [Tooltip("Amount of extra probability to add on top of the base spawn probability when there are no spawned items")]
    [Min(0)]
    public float boostProbabilityAt0 = 0f;
    [Tooltip("offset of the boost curve. higher numbers will result in a flatter curve")]
    [Min(float.Epsilon)]
    public float boostCurveOffset = 1f;
    private float boostCurveMultiplier;

    public GameObject[] spawnPrefabs;

    public Vector2Int spawnBoxSize = new Vector2Int(1, 1);
    public HexMember hexMember;

    private int myID;
    private void Awake()
    {
        hexMember = GetComponentInParent<HexMember>();
        myID = Random.Range(0, int.MaxValue);
    }

    // Start is called before the first frame update
    void Start()
    {
        boostCurveMultiplier = boostCurveOffset * boostProbabilityAt0;
        for (var i = 0; i < initialSpawnNumber; i++)
        {
            SpawnItem();
        }
    }

    // Update is called once per frame
    void Update()
    {
        var childrenNumber = transform.childCount;
        if (childrenNumber < maximumConcurrentSpawned)
        {
            var probability = spawnProbability + (boostCurveMultiplier / (childrenNumber + boostCurveOffset));
            if (Random.value < probability * Time.deltaTime)
            {
                SpawnItem();
            }
        }
    }

    private void SpawnItem()
    {
        var checks = 0;
        AxialCoordinate newPosition;
        bool hasExistingMember;
        do
        {
            newPosition = getRandomPosInBounds();
            var newRealPosition = newPosition + hexMember.PositionInTileMap;
            var memberAtLocation = hexMember.MapManager
                .GetMembersAtLocation<HexMember>(newRealPosition);
            if (memberAtLocation == null)
            {
                Debug.LogError($"Attempted to spanw item at {newRealPosition} but was out of bounds");
                Debug.LogError($"{newRealPosition.ToOffset()}");
            }
            hasExistingMember = memberAtLocation
                .Where(x => x.GetComponent<SpawnMarker>()?.id == myID)
                .Any();
            checks++;
        } while (hasExistingMember && checks < 10);

        if (hasExistingMember)
        {
            return;
        }
        var thisPrefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];

        var newItem = Instantiate(thisPrefab, transform);
        var hexItem = newItem.GetComponentInChildren<HexMember>();
        var marker = newItem.AddComponent<SpawnMarker>();
        marker.id = myID;
        //Debug.Log($"spawning map feature at {newPosition}");
        hexItem.localPosition = newPosition;
        // the item will get the manager from the parent tree
        // hexItem.tilemapManager = this.hexMember.tilemapManager;
        //this.hexMember.tilemapManager.RegisterNewMapMember(hexItem, getRandomPosInBounds());
    }

    private AxialCoordinate getRandomPosInBounds()
    {
        return new OffsetCoordinate(
            Random.Range(0, spawnBoxSize.x),
            Random.Range(0, spawnBoxSize.y)).ToAxial();
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
        //Gizmos.DrawCube(transform.position, spawnBoxSize);
    }
}
