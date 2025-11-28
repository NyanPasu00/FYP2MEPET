using UnityEngine;

public class RadioMenuController : MonoBehaviour
{
    public GameObject SongMenuPanel;

    // Toggle visibility of the Song Menu Panel
    public void ToggleSongMenu()
    {
        Debug.Log("Button Clicked! Toggling Song Menu");
        SongMenuPanel.SetActive(!SongMenuPanel.activeSelf);
    }
}
