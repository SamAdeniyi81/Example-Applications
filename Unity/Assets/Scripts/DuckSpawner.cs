using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckSpawner : MonoBehaviour
{
    public List<GameObject> duckPrefab;  // The duck prefab to spawn
    private GameObject randomDuck;

    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 5f;

    public List<Transform> waypoints;

    // Start is called before the first frame update
    void Start()
    {
        // Start spawning ducks
        StartCoroutine(SpawnDucks());
    }

    IEnumerator SpawnDucks()
    {
        while (true)
        {
            // Wait for a random interval before spawning the next duck
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(spawnInterval);

            // Spawn a duck at a random position along the river
           Vector3 spawnPosition = waypoints[Random.Range(0, waypoints.Count)].position;

            randomDuck = duckPrefab[Random.Range(0, duckPrefab.Count)];

            Instantiate(randomDuck, spawnPosition, Quaternion.identity);
        }
    }
}
