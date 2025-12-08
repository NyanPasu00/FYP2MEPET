using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class EvolutionManager : MonoBehaviour
{
    public Animator petAnimator;
    public GameObject oldStagePet;      // The child pet GameObject
    public GameObject newStagePet;     // The teen pet GameObject (initially hidden)
    public GameObject backButton;  // Button to go back to game (initially hidden
    public TextMeshProUGUI stageText;
    public int loopCount = 3;

    void Start()
    {
        backButton.SetActive(false);
        newStagePet.SetActive(false);
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "KidToTeen")
        {
            stageText.text = "Your cat has grown up and is ready for the next stage and challenge. From Kid To Teen Stage"; 

        }
        if (currentScene == "TeenToAdult")
        {
            stageText.text = "Your cat has grown up and is ready for the next stage and challenge. From Teen To Adult Stage";

        }
        if (currentScene == "AdultToOld")
        {
            stageText.text = "Your cat has grown up and is ready for the next stage and challenge. From Adult To Old Stage";

        }
        StartCoroutine(PlayEvolutionAnimation());
    }

    IEnumerator PlayEvolutionAnimation()
    {
        for (int i = 0; i < loopCount; i++)
        {
            petAnimator.SetTrigger("StartEvolution");
            // Wait until animation finishes (replace 1.5f with your animation length)
            yield return new WaitForSeconds(1.5f);
        }

        // Switch to teen pet
        oldStagePet.SetActive(false);
        newStagePet.SetActive(true);

        // Show back to game button
        backButton.SetActive(true);
    }

    public void BackToTeenStageGame()
    {
        
        SceneManager.LoadScene("TeenHallScene");
       
    }
    public void BackToAdultStageGame()
    {

        SceneManager.LoadScene("AdultHallScene");

    }
    public void BackToOldStageGame()
    {

        SceneManager.LoadScene("OldHallScene");

    }
}
