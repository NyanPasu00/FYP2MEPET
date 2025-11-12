using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallMissed : MonoBehaviour
{

    public static BallMissed instance;

    public TextMeshProUGUI missed;
    public GameObject gameOverPanel; // Drag this in the Inspector
    public GameObject pausePanel;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;
    public AudioClip missSound;       // Assign in Inspector
    public AudioSource audioSource;
    private int misscount = 0;
    private int highScore = 0;
   
    void Awake()
    {
        

        if (instance == null)
            instance = this;


    }

    private void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void MissScore()
    {
        misscount += 1;
        missed.text = "Missed: " + misscount.ToString() + "/8";

        audioSource.PlayOneShot(missSound);

        if (misscount >= 8)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        Time.timeScale = 0f; //Pause the game
        gameOverPanel.SetActive(true);

        int currentScore = ScoreManager.instance.GetScore(); // Add GetScore() method in ScoreManager

        currentScoreText.text = "" + currentScore;

        if (currentScore > PlayerPrefs.GetInt("HighScore", 0))
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
        }

        highScoreText.text = "" + highScore;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }
    public void RestartGame()
    {
        Time.timeScale = 1f; // Unpause the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // Just in case
        FindFirstObjectByType<BGMScript>().PlayMusic();

        SceneLoader.Instance.gameBackScene();

    }

  
}