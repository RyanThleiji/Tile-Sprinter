using UnityEngine;

// Controls the player: jumping with space, crouching with the down arrow,
// fast falling when the down arrow is held in midair, jump pad launches, and the shield.
// The player stays in place horizontally; the world scrolls toward them.
public class PlayerController : MonoBehaviour {
    [Header("Inscribed")]
    public float jumpForce = 12f;                 // Upward speed applied when jumping
    public float fastFallGravityMultiplier = 3f;  // How much stronger gravity is during a fast fall
    public Vector2 standingColliderSize = new Vector2( 0.8f, 1f );   // Hitbox while standing
    public Vector2 crouchColliderSize = new Vector2( 0.8f, 0.5f );   // Smaller hitbox while ducking
    public GameObject shieldVisual;               // Bubble shown while the shield is active

    [Header("Dynamic")]
    public bool hasShield = false;     // True while a shield will absorb the next hit

    private Rigidbody2D rb;
    private BoxCollider2D box;
    private bool isGrounded = false;   // True while standing on the ground
    private bool isDucking = false;    // True while the down arrow is held
    private float normalGravity;       // Original gravity, restored after a fast fall

    // Read by PlayerAnimator to choose the right sprite
    public bool IsGrounded { get { return isGrounded; } }
    public bool IsDucking { get { return isDucking; } }

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();
        normalGravity = rb.gravityScale;
        SetColliderSize( standingColliderSize );

        // Start without a shield
        if ( shieldVisual != null ) {
            shieldVisual.SetActive( false );
        }
    }

    void Update() {
        isDucking = Input.GetKey( KeyCode.DownArrow );

        // Jump only when on the ground, so the player can't jump in midair
        if ( Input.GetKeyDown( KeyCode.Space ) && isGrounded ) {
            rb.linearVelocity = new Vector2( rb.linearVelocity.x, jumpForce );
            isGrounded = false;
        }

        // Fast fall: holding down in the air pulls the player to the ground faster
        if ( isDucking && !isGrounded ) {
            rb.gravityScale = normalGravity * fastFallGravityMultiplier;

            // Cancel any upward motion so the drop starts immediately
            if ( rb.linearVelocity.y > 0f ) {
                rb.linearVelocity = new Vector2( rb.linearVelocity.x, 0f );
            }
        } else {
            rb.gravityScale = normalGravity;
        }

        // Shrink the hitbox while ducking so the player fits under overhead obstacles
        SetColliderSize( isDucking ? crouchColliderSize : standingColliderSize );
    }

    // Resizes the hitbox from the feet up, so it always stays on the ground
    void SetColliderSize( Vector2 size ) {
        box.size = size;
        box.offset = new Vector2( 0f, size.y / 2f );
    }

    // Landing on the ground allows jumping again; touching an obstacle uses the shield or ends the run
    void OnCollisionEnter2D( Collision2D coll ) {
        if ( coll.gameObject.CompareTag( "Ground" ) ) {
            isGrounded = true;
        } else if ( coll.gameObject.CompareTag( "Obstacle" ) ) {
            if ( hasShield ) {
                UseShield( coll.gameObject );
            } else {
                Die();
            }
        }
    }

    // Called by shield tiles; only one shield can be held at a time
    public void GiveShield() {
        hasShield = true;
        if ( shieldVisual != null ) {
            shieldVisual.SetActive( true );
        }
    }

    // The shield breaks and destroys the obstacle instead of the player dying
    void UseShield( GameObject obstacle ) {
        hasShield = false;
        if ( shieldVisual != null ) {
            shieldVisual.SetActive( false );
        }
        Destroy( obstacle );
    }

    // Called by jump pads: sends the player much higher than a normal jump
    public void Launch( float force ) {
        rb.linearVelocity = new Vector2( rb.linearVelocity.x, force );
        isGrounded = false;
    }

    // Ends the run by telling the GameManager to show the Game Over screen
    void Die() {
        GameManager.S.GameOver();
    }
}