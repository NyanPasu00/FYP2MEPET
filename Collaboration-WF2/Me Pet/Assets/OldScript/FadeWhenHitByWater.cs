using UnityEngine;

public class FadeWhenHitByWater : MonoBehaviour
{
    public float floatSpeed = 0.2f;
    public float fadeTime = 3f;

    private float lifeTimer = 0f;
    public bool isFading = false;
    private SpriteRenderer sr;

    private CatDirtyManager catDirtyManager;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>(); 
        catDirtyManager = FindAnyObjectByType<CatDirtyManager>(); // Use the new method here

    }

    void Update()
    {
        if (isFading)
        {
            lifeTimer += Time.deltaTime;

            transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            if (sr != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, lifeTimer / fadeTime);
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            }

            if (lifeTimer >= fadeTime)
            {
                if (catDirtyManager != null)
                {
                    catDirtyManager.dirty = 0f;
                    catDirtyManager.HandleFullyCleaned();
                }

                Destroy(gameObject);
            }

            if (catDirtyManager != null)
            {
                catDirtyManager.DecreaseDirtGradually(Time.deltaTime);
            }

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
