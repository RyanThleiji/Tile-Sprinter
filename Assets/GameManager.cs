using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Runs the overall game: the start screen, how-to-play page, scoring, difficulty,
// speed boosts, game over, and restarting.
public class GameManager : MonoBehaviour {
    static public GameManager S;  // Lets other scripts reach the GameManager easily

    [Header("Inscribed")]
    public GameObject startPanel;
    public GameObject tutorialPanel;      // How to Play page, opened with H from the start screen
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI finalScoreText;
    public float pointsPerSecond = 10f;   // Score earned per second survived

    [Header("Difficulty")]
    public float startSpeed = 6f;               // Scroll speed at the start of a run
    public float maxSpeed = 14f;                // Fastest the game can get
    public float speedIncreasePerSecond = 0.1f; // How quickly the game speeds up

    [Header("Dynamic")]
    public float score = 0f;
    public bool isPlaying = false;
    public bool isGameOver = false;
    public float currentSpeed;            // Base speed, rises over time
    public float speedMultiplier = 1f;    // Temporary boost from speed-up tiles
    public float speedBoostTimer = 0f;    // Seconds left on the current boost
    public float speedLockTimer = 0f;     // While above 0, the speed can't change (protects jump pad timing)

    private bool hasPendingBoost = false;  // A boost picked up during a speed lock, waiting to start
    private float pendingMultiplier;
    private float pendingDuration;

    // The speed everything in the world actually scrolls at
    public float ScrollSpeed {
        get { return currentSpeed * speedMultiplier; }
    }

    // 0 at the start of a run, 1 once max speed is reached
    public float DifficultyProgress {
        get { return ( currentSpeed - startSpeed ) / ( maxSpeed - startSpeed ); }
    }

    void Awake() {
        S = this;
        currentSpeed = startSpeed;
    }

    void Start() {
        // Begin frozen on the start screen
        startPanel.SetActive( true );
        tutorialPanel.SetActive( false );
        gameOverPanel.SetActive( false );
        Time.timeScale = 0f;
    }

    void Update() {
        // Before the run starts, H opens and closes the How to Play page
        if ( !isPlaying && !isGameOver && Input.GetKeyDown( KeyCode.H ) ) {
            tutorialPanel.SetActive( !tutorialPanel.activeSelf );
        }

        // Space starts the game from the menu or tutorial, or restarts after game over
        if ( Input.GetKeyDown( KeyCode.Space ) ) {
            if ( isGameOver ) {
                Restart();
            } else if ( !isPlaying ) {
                StartGame();
            }
        }

        if ( isPlaying ) {
            // Score goes up the longer the player survives
            score += pointsPerSecond * Time.deltaTime;
            UpdateScoreDisplay();

            if ( speedLockTimer > 0f ) {
                // The player just hit a jump pad, so hold the speed steady until the obstacle passes
                speedLockTimer -= Time.deltaTime;
            } else {
                // Gradually speed up, capped at maxSpeed
                currentSpeed = Mathf.Min( currentSpeed + speedIncreasePerSecond * Time.deltaTime, maxSpeed );

                // Start a boost that was picked up during a speed lock
                if ( hasPendingBoost ) {
                    hasPendingBoost = false;
                    speedMultiplier = pendingMultiplier;
                    speedBoostTimer = pendingDuration;
                }

                // Count down an active speed boost and end it when time runs out
                if ( speedBoostTimer > 0f ) {
                    speedBoostTimer -= Time.deltaTime;
                    if ( speedBoostTimer <= 0f ) {
                        speedMultiplier = 1f;
                    }
                }
            }
        }
    }

    void StartGame() {
        isPlaying = true;
        startPanel.SetActive( false );
        tutorialPanel.SetActive( false );
        Time.timeScale = 1f;
        SoundManager.Play( SoundManager.S.gameStart );
    }

    // Lets other scripts (like bonus tiles) give extra points
    public void AddPoints( int points ) {
        score += points;
        UpdateScoreDisplay();
    }

    // Called by speed-up tiles; hitting another one restarts the timer.
    // During a speed lock, the boost waits until the lock ends.
    public void StartSpeedBoost( float multiplier, float duration ) {
        if ( speedLockTimer > 0f ) {
            hasPendingBoost = true;
            pendingMultiplier = multiplier;
            pendingDuration = duration;
            return;
        }
        speedMultiplier = multiplier;
        speedBoostTimer = duration;
    }

    // Called by a jump pad when the player hits it, so the speed can't change
    // until the wall or sky obstacle has passed
    public void LockSpeed( float seconds ) {
        speedLockTimer = Mathf.Max( speedLockTimer, seconds );
    }

    void UpdateScoreDisplay() {
        int wholeScore = Mathf.FloorToInt( score );
        scoreText.text = "Score: " + wholeScore.ToString( "#,0" );
        HighScore.TRY_SET_HIGH_SCORE( wholeScore );
    }

    // Called by the player when they hit an obstacle
    public void GameOver() {
        isPlaying = false;
        isGameOver = true;
        Time.timeScale = 0f;
        finalScoreText.text = "Score: " + Mathf.FloorToInt( score ).ToString( "#,0" );
        gameOverPanel.SetActive( true );
    }

    void Restart() {
        Time.timeScale = 1f;  // Must reset before reloading, since it carries over between scenes
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }
}