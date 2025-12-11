using UnityEngine;

public class BGMScript : MonoBehaviour
{
    public static BGMScript Instance { get; private set; }

    private AudioSource audioSource;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        // Optional: auto-play only in the first scene
        if (audioSource != null && audioSource.playOnAwake)
        {
            PlayMusic();
        }
    }

    public void PlayMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        Destroy(gameObject);
    }

    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
    }

    public void ChangeVolume(float volume)
    {
        if (audioSource != null)
            audioSource.volume = volume;
    }

    // If you want to switch songs, it's better to use AudioClip instead of AudioSource.
    public void ChangeClip(AudioClip newClip, bool playImmediately = true)
    {
        if (audioSource == null || newClip == null) return;

        audioSource.clip = newClip;
        if (playImmediately)
        {
            PlayMusic();
        }
    }
}
