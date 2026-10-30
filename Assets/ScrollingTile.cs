using UnityEngine;

// Scrolls a tiled sprite left with the game and quietly wraps it back,
// so the ground and background look endless.
// A speed factor below 1 makes far-away layers move slower (parallax).
public class ScrollingTile : MonoBehaviour {
    [Header("Inscribed")]
    public float speedFactor = 1f;  // 1 = moves with the obstacles; smaller = farther away
    public float tileWidth = 1f;    // Width of one repeat of the image, in world units

    private float startX;

    void Start() {
        startX = transform.position.x;
    }

    void Update() {
        Vector3 pos = transform.position;
        pos.x -= GameManager.S.ScrollSpeed * speedFactor * Time.deltaTime;

        // Once it has moved one full tile, jump back; the repeat pattern hides the jump
        if ( pos.x <= startX - tileWidth ) {
            pos.x += tileWidth;
        }
        transform.position = pos;
    }
}