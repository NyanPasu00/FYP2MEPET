using UnityEngine;
using UnityEngine.SceneManagement;

public enum CauseOfDeath
{
    Hunger = 0,
    LowHappiness = 1,
    Stress = 2,
    LackMedication = 3,
    OldAge = 4
}

public class DigitalSceneControl : MonoBehaviour
{
    [Header("Scene 1 - Last Word")]
    public bool isScene1; // Assign true if this is Scene 1
    [Header("Scene 2 - Heartfelt Messages")]
    public GameObject hungerHeartfelt;
    public GameObject lowHappinessHeartfelt;
    public GameObject stressHeartfelt;
    public GameObject lackMedicalHeartfelt;
    public GameObject oldAgeHeartfelt;
    [Header("Scene 3 - Cause Messages")]
    public GameObject hungerCause;
    public GameObject lowHappinessCause;
    public GameObject stressCause;
    public GameObject lackMedicationCause;
    public GameObject oldAgeCause;

    private DigitalAlbumManager albumManager;

    void Start()
    {
        // Scene 1 logic
        if (isScene1)
        {
            albumManager = FindObjectOfType<DigitalAlbumManager>();
            if (albumManager != null)
            {
                string json = PlayerPrefs.GetString("PetData");
                PetStatus.PetData currentPet = JsonUtility.FromJson<PetStatus.PetData>(json);

                string id = System.Guid.NewGuid().ToString();
                string petName = currentPet.petName;     
                string petStage = currentPet.stage.ToString();

                albumManager.SaveDeadPet(id, petName, petStage);
            }
            else
            {
                Debug.LogWarning("DigitalAlbumManager not found in scene.");
            }
        }

        // Scene 2 logic
        if (hungerHeartfelt != null)
        {
            DisableAllHeartfelt();
            ShowHeartfelt();
        }

        // Scene 3 logic
        if (hungerCause != null)
        {
            DisableAllCauses();
            ShowCause();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Only stop music if scene 3 exists
            var bgm = FindFirstObjectByType<PassAwayBGMScript>();
   

            // Load the next scene based on current scene
            if (isScene1)
            {

                SceneManager.LoadScene("2Heartfelt");
            }
            else if (hungerHeartfelt != null)
            {
                SceneManager.LoadScene("3Cause");

            }
            else if (hungerCause != null)
            {
                if(bgm != null)
                {
                bgm.StopMusic();
                }
                SceneManager.LoadScene("StartScene");
            }
        }
    }

    // ----- Scene 2 functions -----
    void DisableAllHeartfelt()
    {
        hungerHeartfelt.SetActive(false);
        lowHappinessHeartfelt.SetActive(false);
        stressHeartfelt.SetActive(false);
        lackMedicalHeartfelt.SetActive(false);
        oldAgeHeartfelt.SetActive(false);
    }

    void ShowHeartfelt()
    {
        CauseOfDeath cause = (CauseOfDeath)PlayerPrefs.GetInt("CauseOfDeath", 0);

        switch (cause)
        {
            case CauseOfDeath.Hunger:
                hungerHeartfelt.SetActive(true);
                break;
            case CauseOfDeath.LowHappiness:
                lowHappinessHeartfelt.SetActive(true);
                break;
            case CauseOfDeath.Stress:
                stressHeartfelt.SetActive(true);
                break;
            case CauseOfDeath.LackMedication:
                lackMedicalHeartfelt.SetActive(true);
                break;
            case CauseOfDeath.OldAge:
                oldAgeHeartfelt.SetActive(true);
                break;
        }
    }

    // ----- Scene 3 functions -----
    void DisableAllCauses()
    {
        hungerCause.SetActive(false);
        lowHappinessCause.SetActive(false);
        stressCause.SetActive(false);
        lackMedicationCause.SetActive(false);
        oldAgeCause.SetActive(false);
    }

    void ShowCause()
    {
        CauseOfDeath cause = (CauseOfDeath)PlayerPrefs.GetInt("CauseOfDeath", 0);

        switch (cause)
        {
            case CauseOfDeath.Hunger:
                hungerCause.SetActive(true);
                break;
            case CauseOfDeath.LowHappiness:
                lowHappinessCause.SetActive(true);
                break;
            case CauseOfDeath.Stress:
                stressCause.SetActive(true);
                break;
            case CauseOfDeath.LackMedication:
                lackMedicationCause.SetActive(true);
                break;
            case CauseOfDeath.OldAge:
                oldAgeCause.SetActive(true);
                break;
        }
    }

}

