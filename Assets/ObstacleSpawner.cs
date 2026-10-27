using UnityEngine;

// Spawns a random obstacle off the right edge of the screen at random intervals.
public class ObstacleSpawner : MonoBehaviour {
    [Header("Inscribed")]
    public GameObject[] obstaclePrefabs;  // The obstacle types to choose from
    public float firstSpawnDelay = 1f;    // Wait before the first obstacle
    public float minDelay = 1.2f;         // Shortest time between obstacles
    public float maxDelay = 2.5f;         // Longest time between obstacles

    void Start() {
        Invoke( nameof( SpawnObstacle ), firstSpawnDelay );
    }

    void SpawnObstacle() {
        // Pick a random obstacle type
        int index = Random.Range( 0, obstaclePrefabs.Length );
        GameObject prefab = obstaclePrefabs[index];

        // Spawn at the spawner's X, but keep the prefab's own height
        // so ground obstacles stay low and overhead ones stay high
        Vector3 spawnPos = new Vector3( transform.position.x, prefab.transform.position.y, 0f );
        Instantiate( prefab, spawnPos, Quaternion.identity );

        // Schedule the next obstacle after a random delay
        Invoke( nameof( SpawnObstacle ), Random.Range( minDelay, maxDelay ) );
    }
}