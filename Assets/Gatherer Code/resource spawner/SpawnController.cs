using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public int maximumConcurrentSpawned = 10;
    public float spawnProbability = 0.5f;
    public GameObject spawnPrefab;

    public Vector3 spawnBoxSize = new Vector3(1, 1, 1);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount < maximumConcurrentSpawned)
        {
            if(Random.value < spawnProbability * Time.deltaTime)
            {
                Instantiate(spawnPrefab, getRandomPosInBounds() + this.transform.position, Quaternion.identity, this.transform);
            }
        }
    }

    private Vector3 getRandomPosInBounds()
    {
        return new Vector3(
            this.spawnBoxSize.x * (Random.value - 0.5f),
            this.spawnBoxSize.y * (Random.value - 0.5f),
            this.spawnBoxSize.z * (Random.value - 0.5f));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
        Gizmos.DrawCube(transform.position, spawnBoxSize);
    }
}
