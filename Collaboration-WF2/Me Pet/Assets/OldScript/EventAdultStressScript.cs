using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class EventAdultStressTrigger : MonoBehaviour
{
    public string categoryFriendName;

    public TextMeshProUGUI stressText;
    public GameObject stressPanel;
    public GameObject ChooseRelieveStressPanel;
    public GameObject ResignPanel;
    public GameObject MoviePanel;
    public GameObject TravelPanel;
    public PetStatus happiness;
    

    void Start()
    {
        string petName = PlayerPrefs.GetString("PetName", "Pet");
        stressText.text = $"{petName}'s Felt Work Stress Always scold by Boss , Let Help Him to relieve stress ";
    }
    public void StressEventTrigger()
    {

        stressPanel.SetActive(false);
        ChooseRelieveStressPanel.SetActive(true);
    }

    public void ChooseTravel()
    {
        ChooseRelieveStressPanel.SetActive(false);
        TravelPanel.SetActive(true);
        happiness.increaseHappiness(20);
    }

    public void ChooseResign()
    {
        ChooseRelieveStressPanel.SetActive(false);
        ResignPanel.SetActive(true);
        happiness.decreaseHappiness(20);
    }

    public void ChooseMovie()
    {
        ChooseRelieveStressPanel.SetActive(false);
        MoviePanel.SetActive(true);
        happiness.increaseHappiness(20);
    }


}
