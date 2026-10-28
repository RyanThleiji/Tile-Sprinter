using UnityEngine;
using TMPro;

// Tracks the best score across runs, saved with PlayerPrefs so it survives restarts.
public class HighScore : MonoBehaviour {
    static private TextMeshProUGUI _UI_TEXT;
    static private int _SCORE = 0;

    void Awake() {
        _UI_TEXT = GetComponent<TextMeshProUGUI>();
        // Load the saved high score (0 if none) and show it
        SCORE = PlayerPrefs.GetInt( "HighScore", 0 );
    }

    // Setting SCORE saves it and updates the on-screen text
    static public int SCORE {
        get { return _SCORE; }
        private set {
            _SCORE = value;
            PlayerPrefs.SetInt( "HighScore", value );
            if ( _UI_TEXT != null ) {
                _UI_TEXT.text = "High Score: " + value.ToString( "#,0" );
            }
        }
    }

    // Only replaces the high score if the new score beats it
    static public void TRY_SET_HIGH_SCORE( int scoreToTry ) {
        if ( scoreToTry <= SCORE ) return;
        SCORE = scoreToTry;
    }

    [Tooltip( "Check this box to reset the high score to 0" )]
    public bool resetHighScoreNow = false;

    void OnDrawGizmos() {
        if ( resetHighScoreNow ) {
            resetHighScoreNow = false;
            PlayerPrefs.SetInt( "HighScore", 0 );
            Debug.LogWarning( "High score reset to 0." );
        }
    }
}