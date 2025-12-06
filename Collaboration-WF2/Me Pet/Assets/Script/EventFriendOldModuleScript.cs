using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class EventTrigger : MonoBehaviour
{
    public string categoryFriendName;

    public TextMeshProUGUI lonelyText;
    public TextMeshProUGUI friendLikedText;
    public GameObject lonelyPanel;
    public GameObject ChooseFriendPanel;
    public GameObject DialogForFriend;
    public Animator petAnimator;
    public Transform petTransform;
    public PetStatus happiness;
    public GameObject NewFriendButton;
    public GameObject BackHallButton;

    void Start()
    {
        string petName = PlayerPrefs.GetString("PetName", "Pet");
        lonelyText.text = $"{petName}'s Felt Lonely is time to find him a friend";
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
            NewFriendButton.SetActive(true);
        }
    }

    private bool IsLikedFriend(string category)
    {
        return category == "Cobays" || category == "Potato" || category == "Jade" || category == "Coffee";
    }
}