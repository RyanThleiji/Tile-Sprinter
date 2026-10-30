using UnityEngine;

// Chooses the player's sprite each frame: sliding, jumping, or a running animation.
// The run animation speeds up as the game gets faster, and dust kicks up while sliding.
public class PlayerAnimator : MonoBehaviour {
    [Header("Inscribed")]
    public Sprite[] runFrames;          // Walk frames played in a loop while running
    public Sprite jumpSprite;           // Shown while in the air
    public Sprite duckSprite;           // Shown while sliding or fast falling
    public float framesPerSecond = 8f;  // Run animation speed at the starting game speed
    public ParticleSystem slideDust;    // Dust trail shown while sliding on the ground

    private SpriteRenderer sr;
    private PlayerController player;
    private float frameTimer = 0f;
    private int currentFrame = 0;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        player = GetComponent<PlayerController>();
    }

    void Update() {
        if ( player.IsDucking ) {
            sr.sprite = duckSprite;
        } else if ( !player.IsGrounded ) {
            sr.sprite = jumpSprite;
        } else {
            // Legs move faster when the world scrolls faster
            float speedFactor = GameManager.S.ScrollSpeed / GameManager.S.startSpeed;
            frameTimer += Time.deltaTime * speedFactor;

            if ( frameTimer >= 1f / framesPerSecond ) {
                frameTimer = 0f;
                currentFrame = ( currentFrame + 1 ) % runFrames.Length;
            }
            sr.sprite = runFrames[currentFrame];
        }

        // Kick up dust only while sliding along the ground, not during a fast fall
        if ( slideDust != null ) {
            var emission = slideDust.emission;
            emission.enabled = player.IsDucking && player.IsGrounded;
        }
    }
}