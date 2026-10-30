using UnityEngine;

// Spawns obstacles off the right edge of the screen.
// Most spawns are a single random obstacle, but some are jump pad patterns:
//  - a real jump pad followed by a wall too tall to clear with a normal jump
//  - a bait pad under a sky obstacle, which launches the player into it
// The time between spawns shrinks as the game speeds up, but never below a safe minimum.
public class ObstacleSpawner : MonoBehaviour {
    [Header("Single Obstacles")]
    public GameObject[] obstaclePrefabs;   // The basic obstacle types to choose from
    public float firstSpawnDelay = 1f;     // Wait before the first spawn
    public float minDelay = 1.2f;          // Shortest time between spawns at the start
    public float maxDelay = 2.5f;          // Longest time between spawns at the start
    public float hardestDelayScale = 0.7f; // At max speed, delays shrink to 70%
    public float minimumGap = 1.0f;        // Never spawn closer than this, so every pattern is survivable

    [Header("Jump Pad Patterns")]
    public GameObject jumpPadPrefab;
    public GameObject baitPadPrefab;
    public GameObject tallWallPrefab;
    public GameObject skyObstaclePrefab;
    [Range( 0f, 1f )] public float padPatternChance = 0.2f;  // Chance a spawn is a pad pattern
    [Range( 0f, 1f )] public float baitChance = 0.4f;        // Chance a pad pattern is bait
    public float padToWallTime = 0.65f;   // Seconds from hitting a real pad to reaching the wall
    public float padToSkyTime = 0.45f;    // Seconds from hitting a bait pad to reaching the sky obstacle
    public float patternExtraDelay = 1.2f; // Extra breathing room after a pattern

    void Start() {
        Invoke( nameof( SpawnNext ), firstSpawnDelay );
    }

    void SpawnNext() {
        float extraDelay = 0f;

        if ( Random.value < padPatternChance ) {
            SpawnPadPattern();
            extraDelay = patternExtraDelay;
        } else {
            int index = Random.Range( 0, obstaclePrefabs.Length );
            SpawnAt( obstaclePrefabs[index], transform.position.x );
        }

        // Shorter waits as difficulty rises, but always at least minimumGap
        float scale = Mathf.Lerp( 1f, hardestDelayScale, GameManager.S.DifficultyProgress );
        float delay = Mathf.Max( Random.Range( minDelay, maxDelay ) * scale, minimumGap );
        Invoke( nameof( SpawnNext ), delay + extraDelay );
    }

    // Places a pad and its matching obstacle. The obstacle's distance is based on the
    // current speed, so the timing works the same whether the game is slow or fast.
    void SpawnPadPattern() {
        float padX = transform.position.x;
        float speed = GameManager.S.ScrollSpeed;

        if ( Random.value < baitChance ) {
            SpawnAt( baitPadPrefab, padX );
            SpawnAt( skyObstaclePrefab, padX + speed * padToSkyTime );
        } else {
            SpawnAt( jumpPadPrefab, padX );
            SpawnAt( tallWallPrefab, padX + speed * padToWallTime );
        }
    }

    // Spawns a prefab at the given X, keeping the prefab's own height
    void SpawnAt( GameObject prefab, float x ) {
        Vector3 pos = new Vector3( x, prefab.transform.position.y, 0f );
        Instantiate( prefab, pos, Quaternion.identity );
    }
}