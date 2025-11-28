using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    public GameObject petListPanel;
    public GameObject petAlbumPanel;
    public GameObject digitalAlbumParent;

    public void OnBackButtonClicked()
    {
        if (petAlbumPanel.activeSelf)
        {
            // If viewing pet album, return to pet list
            petAlbumPanel.SetActive(false);
            petListPanel.SetActive(true);
        }
        else if (petListPanel.activeSelf)
        {
            // If already on pet list, close the digital album
            FindFirstObjectByType<DigitalAlbumManager>().ToggleDigitalAlbum();
            digitalAlbumParent.SetActive(false);
        }
    }
}
