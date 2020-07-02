using Assets.MapGen.TileManagement;
using Assets.Scripts.MovementExtensions;
using Assets.Scripts.Resources;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
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

    public GameObject spawnPrefab;

    public Vector2Int spawnBoxSize = new Vector2Int(1, 1);
    public HexMember hexMember;

    private int myID;
    private void Awake()
    {
        this.hexMember = this.GetComponentInParent<HexMember>();
        myID = Random.Range(0, int.MaxValue);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.boostCurveMultiplier = this.boostCurveOffset * this.boostProbabilityAt0;
        for(var i = 0; i < initialSpawnNumber; i++)
        {
            this.SpawnItem();
        }
    }

    // Update is called once per frame
    void Update()
    {
        var childrenNumber = transform.childCount;
        if (childrenNumber < maximumConcurrentSpawned)
        {
            var probability = spawnProbability + (this.boostCurveMultiplier / (childrenNumber + this.boostCurveOffset));
            if(Random.value < probability * Time.deltaTime)
            {
                this.SpawnItem();
            }
        }
    }

    private void SpawnItem()
    {
        var checks = 0;
        Vector2Int newPosition;
        bool hasExistingMember;
        do
        {
            newPosition = getRandomPosInBounds();
            var newRealPosition = newPosition + this.hexMember.PositionInTileMap;
            hasExistingMember = hexMember.MapManager
                .GetMembersAtLocation<HexMember>(newRealPosition)
                .Where(x => x.GetComponent<SpawnMarker>()?.id == myID)
                .Any();
            checks++;
        } while (hasExistingMember && checks < 10);

        if (hasExistingMember)
        {
            return;
        }

        var newItem = Instantiate(spawnPrefab, transform);
        var hexItem = newItem.GetComponentInChildren<HexMember>();
        var marker = newItem.AddComponent<SpawnMarker>();
        marker.id = this.myID;
        //Debug.Log($"spawning map feature at {newPosition}");
        hexItem.localPosition = newPosition;
        // the item will get the manager from the parent tree
        // hexItem.tilemapManager = this.hexMember.tilemapManager;
        //this.hexMember.tilemapManager.RegisterNewMapMember(hexItem, getRandomPosInBounds());
    }

    private Vector2Int getRandomPosInBounds()
    {
        return new Vector2Int(
            Random.Range(0, this.spawnBoxSize.x),
            Random.Range(0, this.spawnBoxSize.y));
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
        //Gizmos.DrawCube(transform.position, spawnBoxSize);
    }
}
