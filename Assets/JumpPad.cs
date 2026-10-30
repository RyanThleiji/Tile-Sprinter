using UnityEngine;

// A tile on the ground that launches the player high into the air when they run over it.
// Real pads are needed to clear tall walls; bait pads launch the player into a sky obstacle.
public class JumpPad : MonoBehaviour {
    [Header("Inscribed")]
    public float launchForce = 20f;   // Upward speed given to the player (a normal jump is 12)

    void OnTriggerEnter2D( Collider2D other ) {
        // Only react to the player, not other objects
        PlayerController player = other.GetComponent<PlayerController>();
        if ( player != null ) {
            player.Launch( launchForce );
        }
    }
}