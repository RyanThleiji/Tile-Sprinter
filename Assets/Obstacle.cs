using UnityEngine;

// Moves an obstacle left toward the player and removes it once it's off screen.
public class Obstacle : MonoBehaviour {
    [Header("Inscribed")]
    public float speed = 6f;          // How fast the obstacle scrolls left
    public float destroyX = -12f;     // X position where it's off screen

    void Update() {
        Vector3 pos = transform.position;
        pos.x -= speed * Time.deltaTime;
        transform.position = pos;

        // Clean up obstacles the player has already passed
        if ( pos.x < destroyX ) {
            Destroy( gameObject );
        }
    }
}