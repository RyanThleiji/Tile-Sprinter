using UnityEngine;

// Plays all of the game's sound effects from one place.
// Other scripts call SoundManager.Play( SoundManager.S.jump ) and so on.
public class SoundManager : MonoBehaviour {
    static public SoundManager S;  // Lets other scripts reach the SoundManager easily

    [Header("Player")]
    public AudioClip jump;
    public AudioClip land;
    public AudioClip slide;
    public AudioClip fastFall;
    public AudioClip launch;
    public AudioClip death;

    [Header("Tiles and Shield")]
    public AudioClip gem;
    public AudioClip shieldGet;
    public AudioClip shieldBreak;
    public AudioClip speedUp;
    public AudioClip dropWarning;

    [Header("Game")]
    public AudioClip gameStart;

    [Header("Settings")]
    [Range( 0f, 1f )] public float volume = 0.8f;

    private AudioSource source;

    void Awake() {
        S = this;
        source = GetComponent<AudioSource>();
    }

    // Plays a clip once; does nothing if the clip slot is empty
    static public void Play( AudioClip clip ) {
        if ( S == null || clip == null ) return;
        S.source.PlayOneShot( clip, S.volume );
    }
}