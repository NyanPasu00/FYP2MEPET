using UnityEngine;

public class FadeWhenHitByWater : MonoBehaviour
{
    public float floatSpeed = 0.2f;
    public float fadeTime = 3f;

    private float lifeTimer = 0f;
    public bool isFading = false;
    private SpriteRenderer sr;

    private BathController bathController;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        bathController = FindAnyObjectByType<BathController>();
    }

    void Update()
    {
        if (!isFading) return;

        lifeTimer += Time.deltaTime;

        // bubble float up
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // fade alpha
        if (sr != null)
        {
            float alpha = Mathf.Lerp(1f, 0f, lifeTimer / fadeTime);
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
        }

        // when finished fading
        if (lifeTimer >= fadeTime)
        {
            if (bathController != null)
            {
                // fully clean at the end of fade
                bathController.dirty = 0f;
                bathController.HandleFullyCleaned();
            }

            Destroy(gameObject);
        }

        // gradually reduce dirt while fading
        if (bathController != null)
        {
            bathController.DecreaseDirtGradually(Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Water") && !isFading)
        {
            isFading = true;
        }
    }
}