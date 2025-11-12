using UnityEngine;

public class OpenDigitalAlbum : MonoBehaviour
{
    public GameObject digitalAlbumPanel; 
    private bool isAlbumOpen = false; 

    public void ToggleAlbum()
    {
        isAlbumOpen = !isAlbumOpen; 

        digitalAlbumPanel.SetActive(isAlbumOpen);
    }

    // Force close
    public void CloseAlbum()
    {
        isAlbumOpen = false;
        digitalAlbumPanel.SetActive(false);
    }
}
