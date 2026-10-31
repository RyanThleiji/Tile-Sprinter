using UnityEngine;

// A tile on the ground that launches the player high into the air when they run over it.
// Real pads are needed to clear tall walls; bait pads launch the player into a sky obstacle.
// Once hit, the pad briefly locks the game speed so the launch timing can't be thrown off.
public class JumpPad : MonoBehaviour {
    [Header("Inscribed")]
    public float launchForce = 20f;   // Upward speed given to the player (a normal jump is 12)

    [HideInInspector] public float speedLockTime = 0f;  // Set by the spawner for each pattern

    public bool Used { get; private set; }  // True once the player has hit this pad

    void OnTriggerEnter2D( Collider2D other ) {
        if ( Used ) return;

        // Only react to the player, not other objects
        PlayerController player = other.GetComponent<PlayerController>();
        if ( player != null ) {
            Used = true;
            player.Launch( launchForce );

            // Hold the speed steady until the wall or sky obstacle has passed
            if ( speedLockTime > 0f ) {
                GameManager.S.LockSpeed( speedLockTime );
            }
        }
    }
}