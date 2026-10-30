using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Runs the overall game: the start screen, scoring, difficulty, game over, and restarting.
public class GameManager : MonoBehaviour {
    static public GameManager S;  // Lets other scripts reach the GameManager easily

    [Header("Inscribed")]
    public GameObject startPanel;
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
    public float speedMultiplier = 1f;    // Temporary boosts (used by tiles later)

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
        gameOverPanel.SetActive( false );
        Time.timeScale = 0f;
    }

    void Update() {
        // Space starts the game from the menu, or restarts after game over
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

            // Gradually speed up, capped at maxSpeed
            currentSpeed = Mathf.Min( currentSpeed + speedIncreasePerSecond * Time.deltaTime, maxSpeed );
        }
    }

    void StartGame() {
        isPlaying = true;
        startPanel.SetActive( false );
        Time.timeScale = 1f;
    }

    // Lets other scripts (like tiles later) give bonus points
    public void AddPoints( int points ) {
        score += points;
        UpdateScoreDisplay();
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