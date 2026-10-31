using UnityEngine;

// Drops an obstacle from above the screen until it lands on the ground.
// The Obstacle script on the same object handles scrolling it left.
public class FallingObstacle : MonoBehaviour {
    [Header("Inscribed")]
    public float fallSpeed = 8f;     // How fast it drops
    public float groundY = -3.1f;    // Its center height once resting on the ground

    void Update() {
        Vector3 pos = transform.position;

        // Keep falling until it reaches the ground, then stop
        if ( pos.y > groundY ) {
            pos.y = Mathf.Max( pos.y - fallSpeed * Time.deltaTime, groundY );
            transform.position = pos;
        }
    }
}