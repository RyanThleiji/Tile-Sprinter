using UnityEngine;

// Moves an obstacle left at the game's current scroll speed and removes it once it's off screen.
public class Obstacle : MonoBehaviour {
    [Header("Inscribed")]
    public float destroyX = -12f;     // X position where it's off screen

    void Update() {
        Vector3 pos = transform.position;
        pos.x -= GameManager.S.ScrollSpeed * Time.deltaTime;
        transform.position = pos;

        // Clean up obstacles the player has already passed
        if ( pos.x < destroyX ) {
            Destroy( gameObject );
        }
    }
}