using UnityEngine;

public class GalleryButtonHandler : MonoBehaviour
{
    public DigitalAlbumManager digitalAlbumManager;

    public void OnGalleryButtonClicked()
    {
        if (digitalAlbumManager != null)
        {
            digitalAlbumManager.ToggleDigitalAlbum();
        }
        else
        {
            Debug.LogWarning("DigitalAlbumManager is not assigned!");
        }
    }
}

