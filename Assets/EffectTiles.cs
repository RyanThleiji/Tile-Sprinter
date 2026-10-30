using UnityEngine;

// A colored tile on the ground that triggers an effect when the player runs over it.
// Good tiles: bonus points and a shield. Bad tile: a temporary speed-up.
public class EffectTile : MonoBehaviour {
    public enum TileEffect { BonusPoints, Shield, SpeedUp }

    [Header("Inscribed")]
    public TileEffect effect;                 // Chosen per prefab in the Inspector
    public int bonusPoints = 50;              // Points from a bonus tile
    public float speedBoostMultiplier = 1.5f; // How much faster a speed-up tile makes the game
    public float speedBoostDuration = 4f;     // How long the speed-up lasts, in seconds

    void OnTriggerEnter2D( Collider2D other ) {
        // Only react to the player
        PlayerController player = other.GetComponent<PlayerController>();
        if ( player == null ) return;

        switch ( effect ) {
            case TileEffect.BonusPoints:
                GameManager.S.AddPoints( bonusPoints );
                break;
            case TileEffect.Shield:
                player.GiveShield();
                break;
            case TileEffect.SpeedUp:
                GameManager.S.StartSpeedBoost( speedBoostMultiplier, speedBoostDuration );
                break;
        }

        // Each tile only works once
        Destroy( gameObject );
    }
}