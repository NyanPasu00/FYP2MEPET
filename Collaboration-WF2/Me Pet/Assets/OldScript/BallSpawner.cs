using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject redBallPrefab;
    public GameObject orangeBallPrefab;
    public float spawnInterval = 1.2f;
    public float spawnXRange = 2.0f;
    public float minScale = 0.5f;
    public float maxScale = 1.2f;

    private float timeElapsed = 0f;
    public float gravityScale = 0.4f;

    void Start()
    {
        InvokeRepeating("SpawnBall", 1f, spawnInterval);
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        // Increase difficulty every 60 seconds
        if (timeElapsed >= 30f)
        {
            gravityScale += 0.1f; // balls fall faster
            timeElapsed = 0f;
        }
    }

    void SpawnBall()
    {
        // Randomly choose red or orange
        GameObject chosenPrefab = Random.value < 0.5f ? redBallPrefab : orangeBallPrefab;

        // Random position
        float spawnX = Random.Range(-spawnXRange, spawnXRange);
        Vector2 spawnPosition = new Vector2(spawnX, transform.position.y);

        GameObject ball = Instantiate(chosenPrefab, spawnPosition, Quaternion.identity);

        // Random size
        float scale = Random.Range(minScale, maxScale);
        ball.transform.localScale = new Vector3(scale, scale, 1f);

        // Set gravity scale based on difficulty
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = gravityScale;
        }
    }
}