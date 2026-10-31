using UnityEngine;

// Controls the player: jumping with space, sliding with the down arrow,
// fast falling when the down arrow is held in midair, jump pad launches, and the shield.
// The player stays in place horizontally; the world scrolls toward them.
public class PlayerController : MonoBehaviour {
    [Header("Inscribed")]
    public float jumpForce = 12f;                 // Upward speed applied when jumping
    public float fastFallGravityMultiplier = 3f;  // How much stronger gravity is during a fast fall
    public Vector2 standingColliderSize = new Vector2( 0.8f, 1f );   // Hitbox while standing
    public Vector2 crouchColliderSize = new Vector2( 0.8f, 0.5f );   // Smaller hitbox while sliding
    public float landSoundMinSpeed = 2f;          // Only play the land sound for real landings
    public GameObject shieldVisual;               // Bubble shown while the shield is active

    [Header("Dynamic")]
    public bool hasShield = false;     // True while a shield will absorb the next hit

    private Rigidbody2D rb;
    private BoxCollider2D box;
    private bool isGrounded = false;   // True while standing on the ground
    private bool isDucking = false;    // True while the down arrow is held
    private bool wasDucking = false;   // Last frame's ducking, to detect when a slide or fast fall starts
    private bool isLaunched = false;   // True from a jump pad launch until landing; disables fast fall
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
        // Ignore the down arrow during a jump pad launch, so it can't be cancelled with a fast fall
        isDucking = Input.GetKey( KeyCode.DownArrow ) && !isLaunched;

        // Play a sound the moment a slide or fast fall begins
        if ( isDucking && !wasDucking ) {
            SoundManager.Play( isGrounded ? SoundManager.S.slide : SoundManager.S.fastFall );
        }
        wasDucking = isDucking;

        // Jump only when on the ground, so the player can't jump in midair
        if ( Input.GetKeyDown( KeyCode.Space ) && isGrounded ) {
            rb.linearVelocity = new Vector2( rb.linearVelocity.x, jumpForce );
            isGrounded = false;
            SoundManager.Play( SoundManager.S.jump );
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

        // Shrink the hitbox while sliding so the player fits under overhead obstacles
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
            // Skip the sound when just settling onto the ground at the start
            if ( coll.relativeVelocity.magnitude > landSoundMinSpeed ) {
                SoundManager.Play( SoundManager.S.land );
            }
            isGrounded = true;
            isLaunched = false;   // Landing ends the launch, so down works again
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
        SoundManager.Play( SoundManager.S.shieldGet );
    }

    // The shield breaks and destroys the obstacle instead of the player dying
    void UseShield( GameObject obstacle ) {
        hasShield = false;
        if ( shieldVisual != null ) {
            shieldVisual.SetActive( false );
        }
        SoundManager.Play( SoundManager.S.shieldBreak );
        Destroy( obstacle );
    }

    // Called by jump pads: sends the player much higher than a normal jump,
    // and locks out fast falling until they land
    public void Launch( float force ) {
        rb.linearVelocity = new Vector2( rb.linearVelocity.x, force );
        isGrounded = false;
        isLaunched = true;
        SoundManager.Play( SoundManager.S.launch );
    }

    // Ends the run by telling the GameManager to show the Game Over screen
    void Die() {
        SoundManager.Play( SoundManager.S.death );
        GameManager.S.GameOver();
    }
}