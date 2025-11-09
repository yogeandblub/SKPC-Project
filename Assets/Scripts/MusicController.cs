using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource christmasMusic;
    public AudioSource danceMusic;

    void Start()
    {
        christmasMusic.Play();
    }

    public void SwitchToDanceMusic()
    {
        Debug.Log("SwitchToDanceMusic is called!");

        if (christmasMusic.isPlaying)
            christmasMusic.Stop();

        danceMusic.Play();
    }
}
