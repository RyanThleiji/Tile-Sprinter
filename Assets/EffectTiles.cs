using UnityEngine;

// A colored tile on the ground that triggers an effect when the player runs over it.
// Good tiles: bonus points and a shield. Bad tiles: a temporary speed-up and obstacles dropping from above.
public class EffectTile : MonoBehaviour {
    public enum TileEffect { BonusPoints, Shield, SpeedUp, DropObstacles }

    [Header("Inscribed")]
    public TileEffect effect;                 // Chosen per prefab in the Inspector
    public int bonusPoints = 50;              // Points from a bonus tile
    public float speedBoostMultiplier = 1.5f; // How much faster a speed-up tile makes the game
    public float speedBoostDuration = 4f;     // How long the speed-up lasts, in seconds

    [Header("Drop Obstacles Settings")]
    public GameObject fallingObstaclePrefab;  // What falls from the sky
    public int dropCount = 2;                 // How many obstacles fall
    public float dropMinX = 1f;               // Leftmost drop position (ahead of the player)
    public float dropMaxX = 7f;               // Rightmost drop position
    public float dropHeight = 6f;             // Starting height, just above the top of the screen

    void OnTriggerEnter2D( Collider2D other ) {
        // Only react to the player
        PlayerController player = other.GetComponent<PlayerController>();
        if ( player == null ) return;

        switch ( effect ) {
            case TileEffect.BonusPoints:
                GameManager.S.AddPoints( bonusPoints );
                SoundManager.Play( SoundManager.S.gem );
                break;
            case TileEffect.Shield:
                player.GiveShield();   // Plays its own sound
                break;
            case TileEffect.SpeedUp:
                GameManager.S.StartSpeedBoost( speedBoostMultiplier, speedBoostDuration );
                SoundManager.Play( SoundManager.S.speedUp );
                break;
            case TileEffect.DropObstacles:
                DropObstacles();
                SoundManager.Play( SoundManager.S.dropWarning );
                break;
        }

        // Each tile only works once
        Destroy( gameObject );
    }

    // Spreads the falling obstacles evenly across the drop area, with a little randomness,
    // so they never land on top of each other
    void DropObstacles() {
        for ( int i = 0; i < dropCount; i++ ) {
            float t = ( i + 0.5f ) / dropCount;
            float x = Mathf.Lerp( dropMinX, dropMaxX, t ) + Random.Range( -0.5f, 0.5f );
            Instantiate( fallingObstaclePrefab, new Vector3( x, dropHeight, 0f ), Quaternion.identity );
        }
    }
}