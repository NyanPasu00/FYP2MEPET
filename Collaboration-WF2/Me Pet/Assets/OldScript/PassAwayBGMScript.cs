using UnityEngine;

public class PassAwayBGMScript : MonoBehaviour
{
    public static PassAwayBGMScript Instance;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            audioSource.Play(); // Auto-play when loaded
        }
        else
        {
            Destroy(gameObject);
        }
        FindFirstObjectByType<PassAwayBGMScript>().PlayMusic();
    }

    public void PlayMusic()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void ChangeVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void ChangeClip(AudioSource newClip, bool playImmediately = true)
    {
        audioSource = newClip;
        if (playImmediately)
        {
            PlayMusic();
        }
    }
}
