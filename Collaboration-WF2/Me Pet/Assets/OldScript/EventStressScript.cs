using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class EventStressTrigger : MonoBehaviour
{
    public string categoryFriendName;

    public TextMeshProUGUI stressText;
    public GameObject stressPanel;
    public GameObject ChooseComfortPanel;
    public GameObject ContinueLearningPanel;
    public GameObject LetDrinkPanel;
    public GameObject PlayGamePanel;
    public Energy_Bar happiness;
    

    void Start()
    {
        string petName = PlayerPrefs.GetString("PetName", "Pet");
        stressText.text = $"{petName}'s Felt Depressed Due To Studying Stress , Let Help Him ";
    }
    public void StressEventTrigger()
    {
        
        stressPanel.SetActive(false);
        ChooseComfortPanel.SetActive(true);
    }

    public void ChoosePlayGame()
    {
        ChooseComfortPanel.SetActive(false);
        PlayGamePanel.SetActive(true);
        happiness.increaseHappiness(20);
    }

    public void ChooseContinueLearning()
    {
        ChooseComfortPanel.SetActive(false);
        ContinueLearningPanel.SetActive(true);
        happiness.decreaseHappiness(20);
    }

    public void ChooseLetDrink()
    {
        ChooseComfortPanel.SetActive(false);
        LetDrinkPanel.SetActive(true);
        happiness.increaseHappiness(20);
    }


}
