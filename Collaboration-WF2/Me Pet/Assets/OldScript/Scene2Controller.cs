using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene2Controller : MonoBehaviour
{
    public GameObject hungerHeartfelt;
    public GameObject lowHappinessHeartfelt;
    public GameObject stressHeartfelt;
    public GameObject lackMedicalHeartfelt;
    public GameObject oldAgeHeartfelt;

    void Start()
    {
        DisableAllMessages(); // Make sure all messages are off first
        ShowMessage();        // Then show only the one needed
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("3Cause");
        }
    }

    void DisableAllMessages()
    {
        hungerHeartfelt.SetActive(false);
        lowHappinessHeartfelt.SetActive(false);
        stressHeartfelt.SetActive(false);
        lackMedicalHeartfelt.SetActive(false);
        oldAgeHeartfelt.SetActive(false);
    }

    void ShowMessage()
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
}
