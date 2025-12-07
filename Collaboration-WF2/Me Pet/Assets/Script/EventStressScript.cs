using Facebook.MiniJSON;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public PetStatus happiness;


    public TextMeshProUGUI lonelyText;
    public TextMeshProUGUI friendLikedText;
    public GameObject lonelyPanel;
    public GameObject ChooseFriendPanel;
    public GameObject DialogForFriend;
    public Animator petAnimator;
    public Transform petTransform;
    public GameObject NewFriendButton;
    public GameObject BackHallButton;

    public TextMeshProUGUI stressTextTwo;
    public GameObject stressPanelTwo;
    public GameObject ChooseRelieveStressPanel;
    public GameObject ResignPanel;
    public GameObject MoviePanel;
    public GameObject TravelPanel;

    void Start()
    {
        string json = PlayerPrefs.GetString("PetData", string.Empty);

        PetStatus.PetData data = JsonUtility.FromJson<PetStatus.PetData>(json);
        if (data != null && !string.IsNullOrEmpty(data.petName))
        {
            string petName = data.petName;
            if(stressText != null)
            {
                stressText.text = $"{petName}'s Felt Depressed Due To Studying Stress , Let Help Him ";
            }

            if(stressTextTwo != null)
            {
                stressTextTwo.text = $"{petName}'s Felt Work Stress Always scold by Boss , Let Help Him to relieve stress ";
            }

            if(lonelyText != null)
            {
                lonelyText.text = $"{petName}'s Felt Lonely is time to find him a friend";
            }
        }
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
        happiness.SavePetData();
    }

    public void ChooseContinueLearning()
    {
        
        ChooseComfortPanel.SetActive(false);
        ContinueLearningPanel.SetActive(true);
        happiness.decreaseHappiness(20);
        happiness.SavePetData();
        
    }

    public void ChooseLetDrink()
    {
        ChooseComfortPanel.SetActive(false);
        LetDrinkPanel.SetActive(true);
        happiness.increaseHappiness(20);
        happiness.SavePetData();
    }

    public void FriendEventTrigger()
    {
        DialogForFriend.SetActive(false);
        lonelyPanel.SetActive(false);
        ChooseFriendPanel.SetActive(true);
    }

    public void ChooseAFriend()
    {
        if (IsLikedFriend(categoryFriendName))
        {
            petAnimator.SetBool("Sad", false);
            NewFriendButton.SetActive(false);
            BackHallButton.SetActive(true);
            ChooseFriendPanel.SetActive(false);
            DialogForFriend.SetActive(true);
            friendLikedText.text = "I Like This Friend Thank You";
            petAnimator.SetBool("Laydown", false);
            petTransform.localPosition = new Vector3(-0.32f, -2.71f, 0f);
            petAnimator.SetBool("Excited", true);
            happiness.increaseHappiness(30);
            happiness.SavePetData();

        }
        else
        {
            BackHallButton.SetActive(false);
            ChooseFriendPanel.SetActive(false);
            DialogForFriend.SetActive(true);
            friendLikedText.text = "I Don't Like This Friend";
            petAnimator.SetBool("Laydown", false);
            //petTransform.localPosition = new Vector3(-2.06f, -2.62f, 0f);
            petAnimator.SetBool("Sad", true);
            happiness.decreaseHappiness(20);
            happiness.SavePetData();
            NewFriendButton.SetActive(true);
        }
    }

    private bool IsLikedFriend(string category)
    {
        return category == "Cobays" || category == "Potato" || category == "Jade" || category == "Coffee";
    }

    public void StressEventTriggerTwo()
    {

        stressPanelTwo.SetActive(false);
        ChooseRelieveStressPanel.SetActive(true);
    }

    public void ChooseTravel()
    {
        Debug.Log(happiness.happiness_current);
        ChooseRelieveStressPanel.SetActive(false);
        TravelPanel.SetActive(true);
        happiness.increaseHappiness(20);
        Debug.Log(happiness.happiness_current);
        happiness.SavePetData();
    }

    public void ChooseResign()
    {
        ChooseRelieveStressPanel.SetActive(false);
        ResignPanel.SetActive(true);
        happiness.decreaseHappiness(20);
        happiness.SavePetData();
    }

    public void ChooseMovie()
    {
        ChooseRelieveStressPanel.SetActive(false);
        MoviePanel.SetActive(true);
        happiness.increaseHappiness(20);
        happiness.SavePetData();
    }
}
