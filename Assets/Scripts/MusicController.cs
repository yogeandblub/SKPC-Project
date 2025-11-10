using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource christmasMusic;
    public AudioSource danceMusic;

    // Track whether we are currently in "dance mode"
    private bool isDancing = false;

    void Start()
    {
        // Start with Christmas music playing (optional)
        if (christmasMusic != null)
        {
            christmasMusic.Play();
        }
    }

    // Call this from the SAME button as the animation
    public void ToggleDanceMusic()
    {
        Debug.Log("ToggleDanceMusic is called!");

        if (!isDancing)
        {
            // Go INTO dance mode
            if (christmasMusic != null && christmasMusic.isPlaying)
                christmasMusic.Stop();

            if (danceMusic != null && !danceMusic.isPlaying)
                danceMusic.Play();
        }
        else
        {
            // Go OUT of dance mode
            if (danceMusic != null && danceMusic.isPlaying)
                danceMusic.Stop();

            // If you want to go back to Christmas music:
            if (christmasMusic != null && !christmasMusic.isPlaying)
                christmasMusic.Play();
        }

        // Flip state
        isDancing = !isDancing;
    }
}
