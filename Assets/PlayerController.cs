using UnityEngine;

// Controls the player: jumping with space and crouching with the down arrow.
// The player stays in place horizontally; the world scrolls toward them.
public class PlayerController : MonoBehaviour {
    [Header("Inscribed")]
    public float jumpForce = 12f;      // Upward speed applied when jumping
    public float crouchScaleY = 0.5f;  // How much the player shrinks when crouching

    private Rigidbody2D rb;
    private bool isGrounded = false;   // True while standing on the ground
    private Vector3 normalScale;       // Original size, restored after crouching

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        normalScale = transform.localScale;
    }

    void Update() {
        // Jump only when on the ground, so the player can't jump in midair
        if ( Input.GetKeyDown( KeyCode.Space ) && isGrounded ) {
            rb.linearVelocity = new Vector2( rb.linearVelocity.x, jumpForce );
            isGrounded = false;
        }

        // Shrink while the down arrow is held, then return to normal size
        if ( Input.GetKey( KeyCode.DownArrow ) ) {
            transform.localScale = new Vector3( normalScale.x, normalScale.y * crouchScaleY, normalScale.z );
        } else {
            transform.localScale = normalScale;
        }
    }

    // Landing on the ground allows the player to jump again
    void OnCollisionEnter2D( Collision2D coll ) {
        if ( coll.gameObject.CompareTag( "Ground" ) ) {
            isGrounded = true;
        }
    }
}