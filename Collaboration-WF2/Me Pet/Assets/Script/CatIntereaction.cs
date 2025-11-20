using UnityEngine;

public class CatIntereaction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        Debug.Log("Clicked");
        if (audioSource != null)
        {
            audioSource.Play();
            Invoke(nameof(StopMeow), 1.4f); 
        }

    }
    void StopMeow()
    {
        audioSource.Stop();
    }

}
