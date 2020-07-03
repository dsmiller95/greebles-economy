using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RandomLayout : MonoBehaviour
{
    public int initialSpawnNumber = 1;

    public GameObject[] spawnPrefabs;

    public Vector3 spawnBoxSize = new Vector3(1, 1, 1);

    // Start is called before the first frame update
    void Start()
    {
        this.Respawn();
    }

    private void Respawn()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        for (var i = 0; i < initialSpawnNumber; i++)
        {
            this.SpawnItem();
        }
    }

    private void SpawnItem()
    {
        var newPosition = getRandomPosInBounds();

        var thisPrefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];

        var newItem = Instantiate(thisPrefab, transform);
        newItem.transform.localPosition = newPosition;
    }

    private Vector3 getRandomPosInBounds()
    {
        return new Vector3(
            Random.Range(-this.spawnBoxSize.x / 2, this.spawnBoxSize.x / 2),
            Random.Range(-this.spawnBoxSize.y / 2, this.spawnBoxSize.y / 2),
            Random.Range(-this.spawnBoxSize.z / 2, this.spawnBoxSize.z / 2));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.8f, 0.2f, 0.2f, 0.5f);
        Gizmos.DrawCube(transform.position, spawnBoxSize);
    }
}
