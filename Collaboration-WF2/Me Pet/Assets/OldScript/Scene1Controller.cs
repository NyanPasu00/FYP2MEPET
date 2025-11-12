using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene1Controller : MonoBehaviour
{
    private DigitalAlbumManager albumManager;
    void Start()
    {
        
        albumManager = FindObjectOfType<DigitalAlbumManager>();

        if (albumManager != null)
        {
            string id = System.Guid.NewGuid().ToString();
            string petName = PlayerPrefs.GetString("PetName");
            string petStage = PlayerPrefs.GetString("CurrentStage");
            albumManager.SaveDeadPet(id,petName, petStage);

        }
        else
        {
            Debug.LogWarning("DigitalAlbumManager not found in scene.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("2Heartfelt");
        }
    }
}
