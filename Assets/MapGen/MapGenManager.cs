using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct MapGenSpawnable
{
    public GameObject prefab;
    public float densityPerSurfaceSize;
}

public class MapGenManager : MonoBehaviour
{
    public MapGenSpawnable[] spawnableItems;
    public Vector3 spawnBoxSize = new Vector3(1, 0, 1);

    // Start is called before the first frame update
    void Start()
    {
        var spawnArea = spawnBoxSize.x * spawnBoxSize.z;
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
        Instantiate(prefab, getRandomPosInBounds() + this.transform.position, Quaternion.identity, this.transform);
    }

    private Vector3 getRandomPosInBounds()
    {
        return new Vector3(
            this.spawnBoxSize.x * (UnityEngine.Random.value - 0.5f),
            this.spawnBoxSize.y * (UnityEngine.Random.value - 0.5f),
            this.spawnBoxSize.z * (UnityEngine.Random.value - 0.5f));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
        Gizmos.DrawCube(transform.position, spawnBoxSize);
    }
}
