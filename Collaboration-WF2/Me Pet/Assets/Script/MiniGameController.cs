using JetBrains.Annotations;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MiniGameController : MonoBehaviour
{
    public static MiniGameController instance;

    // ==========================
    //  SCORE
    // ==========================
    [Header("Score")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private AudioClip catchSound;
    [SerializeField] private AudioSource catchAudioSource;
    private int score = 0;

    // ==========================
    //  MISSED / GAME OVER
    // ==========================
    [Header("Miss / Game Over")]
    [SerializeField] private TextMeshProUGUI missedText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private AudioClip missSound;
    [SerializeField] private AudioSource missAudioSource;

    [Header("Miss Limit")]
    [SerializeField] private int maxMisses = 8;
    private int missCount = 0;
    private int highScore = 0;

    // ==========================
    //  BALL SPAWNER
    // ==========================
    [Header("Ball Spawner")]
    [SerializeField] private GameObject redBallPrefab;
    [SerializeField] private GameObject orangeBallPrefab;
    [SerializeField] private float spawnInterval = 1.2f;
    [SerializeField] private float spawnXRange = 2.0f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.2f;
    [SerializeField] private float gravityScale = 0.4f;

    private float timeElapsed = 0f; // for increasing difficulty

    // ==========================
    //  CAT MOVEMENT
    // ==========================
    [Header("Cat Movement")]
    [SerializeField] private Transform catTransform;
    [SerializeField] private float moveSpeed = 5f;

    private float moveDirection = 0f;  // -1 = left, 1 = right
    private SpriteRenderer catRenderer;
    private Animator catAnimator;

    [Header("Money / This Match")]
    [SerializeField] private int maxMoneyPerMatch = 300;
    private int moneyEarnedThisMatch = 0;
    [SerializeField] private TextMeshProUGUI matchMoneyText;


    [Header("Scene Navigation")]
    [SerializeField] private string returnSceneName = "KidScene"; // <--- change to your actual scene name

    private BGMScript bgm;
    public PetStatus PetStatus;  
    private PetStatus.PetStage currentStage;



    private void Awake()
    {
        if (instance == null && instance != this)
        {
            instance = this;
        }

    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Start()
    {
        moneyEarnedThisMatch = 0;

        

        // Stop global BGM when entering mini game
        bgm = BGMScript.Instance;
        if (bgm != null)
        {
            bgm.StopMusic();
        }

       

        // High score
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // Init UI
        UpdateScoreUI();
        UpdateMissUI();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);

        // Cat components
        if (catTransform != null)
        {
            catRenderer = catTransform.GetComponent<SpriteRenderer>();
            catAnimator = catTransform.GetComponent<Animator>();
        }

        // Start spawning balls
        InvokeRepeating(nameof(SpawnBall), 1f, spawnInterval);
    }

    private void Update()
    {
        // Move cat
        if (catTransform != null && moveDirection != 0f)
        {
            catTransform.Translate(Vector2.right * moveDirection * moveSpeed * Time.deltaTime);
        }

        // Difficulty scaling
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= 30f) // every 30 seconds
        {
            gravityScale += 0.1f;   // balls fall faster
            timeElapsed = 0f;
        }
    }

    // ==========================
    //  BALL SPAWN LOGIC
    // ==========================
    private void SpawnBall()
    {
        if (redBallPrefab == null && orangeBallPrefab == null)
            return;

        GameObject chosenPrefab;

        if (redBallPrefab != null && orangeBallPrefab != null)
        {
            chosenPrefab = Random.value < 0.5f ? redBallPrefab : orangeBallPrefab;
        }
        else
        {
            // fallback if one is missing
            chosenPrefab = redBallPrefab != null ? redBallPrefab : orangeBallPrefab;
        }

        float spawnX = Random.Range(-spawnXRange, spawnXRange);
        Vector2 spawnPosition = new Vector2(spawnX, transform.position.y);

        GameObject ball = Instantiate(chosenPrefab, spawnPosition, Quaternion.identity);

        // Random size
        float scale = Random.Range(minScale, maxScale);
        ball.transform.localScale = new Vector3(scale, scale, 1f);

        // Gravity scale
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = gravityScale;
        }
    }

    // ==========================
    //  SCORE METHODS
    // ==========================
    public void AddScore(int points)
    {
        score += points;

        if (catchAudioSource != null && catchSound != null)
        {
            catchAudioSource.PlayOneShot(catchSound);
        }

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
  
    public int GetScore()
    {
        return score;
    }

    // ==========================
    //  MISS METHODS
    // ==========================
    public void MissScore()
    {
        missCount++;
        UpdateMissUI();

        if (missAudioSource != null && missSound != null)
        {
            missAudioSource.PlayOneShot(missSound);
        }

        if (missCount >= maxMisses)
        {
            TriggerGameOver();
        }
    }

    private void UpdateMissUI()
    {
        if (missedText != null)
        {
            missedText.text = $"Missed: {missCount}/{maxMisses}";
        }
    }

    private void TriggerGameOver()
    {
        // Try to find PetStatus automatically if not assigned in Inspector
        if (PetStatus == null)
        {
            PetStatus = FindFirstObjectByType<PetStatus>();
        }

        if (PetStatus != null)
        {
            currentStage = PetStatus.currentStage;
            Debug.Log("MiniGame current stage = " + currentStage);
        }
        else
        {
            Debug.LogWarning("MiniGameController: PetStatus reference not found. Defaulting to Kid.");
            currentStage = PetStatus.PetStage.Kid; // fallback
        }

        Time.timeScale = 0f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        int currentScore = score;

        if (currentScoreText != null)
            currentScoreText.text = currentScore.ToString();

        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (currentScore > savedHighScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", currentScore);
            PlayerPrefs.Save();
        }
        else
        {
            highScore = savedHighScore;
        }

        if (highScoreText != null)
            highScoreText.text = highScore.ToString();

        // NEW: money for this match (only on loss)
        moneyEarnedThisMatch = CalculateMoneyForMatch(currentScore);

        // (Optional) show it on game over UI if you add a TMP text
        if (matchMoneyText != null)
            matchMoneyText.text = $"+{moneyEarnedThisMatch}";

        // Add to wallet in PetData
        AddMatchMoneyToPetData(moneyEarnedThisMatch);

        Debug.Log($"Game over. Score = {currentScore}, money this match = {moneyEarnedThisMatch}");
    }

    // ==========================
    //  PAUSE / RESTART / QUIT
    // ==========================
    public void PauseGame()
    {
        // Try to find PetStatus automatically if not assigned in Inspector
        if (PetStatus == null)
        {
            PetStatus = FindFirstObjectByType<PetStatus>();
        }

        if (PetStatus != null)
        {
            currentStage = PetStatus.currentStage;
            Debug.Log("MiniGame current stage = " + currentStage);
        }
        else
        {
            Debug.LogWarning("MiniGameController: PetStatus reference not found. Defaulting to Kid.");
            currentStage = PetStatus.PetStage.Kid; // fallback
        }

        Time.timeScale = 0f;
        if (pausePanel != null)
            pausePanel.SetActive(!pausePanel);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    public void RestartGame()
    {
        OnDestroy();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        OnDestroy(); // if you want to keep this, otherwise you can remove this line
        GameUI gameUI = FindFirstObjectByType<GameUI>();


        if (bgm != null)
        {
            bgm.PlayMusic();
        }

            // When the main scene finishes loading, we will jump to Hall
            SceneManager.sceneLoaded += OnMainSceneLoaded;

        switch (currentStage)
        {
            case PetStatus.PetStage.Kid:
                gameUI.PlayAndLoad("KidScene");
                //SceneManager.LoadScene("KidScene");
                break;

            case PetStatus.PetStage.Teen:
                gameUI.PlayAndLoad("TeenScene");
                break;

            case PetStatus.PetStage.Adult:
                gameUI.PlayAndLoad("AdultScene");
                break;

            case PetStatus.PetStage.Old:
                gameUI.PlayAndLoad("OldScene");
                break;

            default:
                Debug.LogWarning("Unknown stage, defaulting to KidScene.");
                gameUI.PlayAndLoad("KidScene");
                break;
        }
    }


    private void OnMainSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // We only want this to run once
        SceneManager.sceneLoaded -= OnMainSceneLoaded;

        // Find GameUI in the newly loaded scene and tell it to go to Hall
        GameUI gameUI = FindFirstObjectByType<GameUI>();
        if (gameUI != null)
        {
            gameUI.gameBackScene();
        }
        else
        {
            Debug.LogWarning("GameUI not found in main scene after quitting mini game.");
        }
    }

    // ==========================
    //  CAT MOVEMENT API
    //  (Hook these to UI buttons)
    // ==========================
    public void MoveLeft()
    {
        moveDirection = -1f;

        if (catRenderer != null)
            catRenderer.flipX = false;

        if (catAnimator != null)
            catAnimator.SetBool("Catch", false);
    }

    public void MoveRight()
    {
        moveDirection = 1f;

        if (catRenderer != null)
            catRenderer.flipX = true;

        if (catAnimator != null)
            catAnimator.SetBool("Catch", false);
    }

    public void StopMoving()
    {
        moveDirection = 0f;
    }

    public void PlayCatchAnimation()
    {
        if (catAnimator != null)
            StartCoroutine(CatchRoutine());
    }

    private IEnumerator CatchRoutine()
    {
        catAnimator.SetBool("Catch", true);
        yield return new WaitForSeconds(0.333f); // duration of your catch anim
        catAnimator.SetBool("Catch", false);
    }

    private int CalculateMoneyForMatch(int finalScore)
    {
        int hundreds = finalScore / 100;   // integer division

        if (hundreds <= 0)
            return 0;

        int reward = 20;                   // first 100 points

        if (hundreds > 1)
        {
            reward += (hundreds - 1) * 10; // extra 10 for each additional 100
        }

        if (reward > maxMoneyPerMatch)
            reward = maxMoneyPerMatch;

        return reward;
    }

    private void AddMatchMoneyToPetData(int reward)
    {
        if (reward <= 0)
            return;

        string json = PlayerPrefs.GetString("PetData", string.Empty);
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("PetData not found when trying to add money.");
            return;
        }

        PetStatus.PetData data = JsonUtility.FromJson<PetStatus.PetData>(json);
        if (data == null)
        {
            Debug.LogWarning("Failed to parse PetData JSON when adding money.");
            return;
        }

        data.moneyValue += reward;

        string updatedJson = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PetData", updatedJson);
        PlayerPrefs.Save();

        Debug.Log($"Added {reward} money to wallet. New total = {data.moneyValue}");
    }
}
