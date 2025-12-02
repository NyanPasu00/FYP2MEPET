using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene3Controller : MonoBehaviour
{
    public GameObject hungerCause;
    public GameObject lowHappinessCause;
    public GameObject stressCause;
    public GameObject lackMedicationCause;
    public GameObject oldAgeCause;

    void Start()
    {
        DisableAllCauses();  // Hide all
        ShowCause();         // Show the right one
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // You can change this to any scene you want to go next
            FindFirstObjectByType<PassAwayBGMScript>().StopMusic();
            SceneManager.LoadScene("StartScene");
        }
    }

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
