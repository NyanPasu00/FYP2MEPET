using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TextMeshProUGUI scoreText;
    private int score = 0;
    public AudioClip catchSound;
    public AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void AddScore(int points)
    {
        audioSource.PlayOneShot(catchSound);
        score += points;
        scoreText.text = "Score: " + score.ToString();
    }

    public int GetScore()
    {
        return score;
    }
}
