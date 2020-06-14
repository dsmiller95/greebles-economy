using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public int maximumConcurrentSpawned = 10;
    public float spawnProbability = 0.5f;


    [Header("Boost Settings")]
    [Tooltip("Amount of extra probability to add on top of the base spawn probability when there are no spawned items")]
    [Range(0, 5)]
    public float boostProbabilityAt0 = 0f;
    [Tooltip("offset of the boost curve. higher numbers will result in a flatter curve")]
    [Min(float.Epsilon)]
    public float boostCurveOffset = 1f;
    private float boostCurveMultiplier;

    public GameObject spawnPrefab;

    public Vector3 spawnBoxSize = new Vector3(1, 1, 1);

    // Start is called before the first frame update
    void Start()
    {
        this.boostCurveMultiplier = this.boostCurveOffset * this.boostProbabilityAt0;
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
