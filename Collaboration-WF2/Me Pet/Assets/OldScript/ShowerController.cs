
using UnityEngine;

public class ShowerController : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem showerEffect;   // Assigned in Inspector
    public Transform petTarget;           // Assigned in Inspector
    public GameObject waterDrop;           // Reference to the original water drop
    public Transform waterDropStartPos;    // Where the drop should return
    public AudioSource audioSource;
    public bool isShowering = false;
    private float showerTimer = 0f;
    public float showerDuration = 5f;

    void Start()
    {
        // Safely assign AudioSource if not set via Inspector
        audioSource = GetComponent<AudioSource>();

        if (showerEffect == null)
            Debug.LogWarning("⚠ ShowerController: showerEffect is not assigned!");

        if (petTarget == null)
            Debug.LogWarning("⚠ ShowerController: petTarget is not assigned!");

        if (audioSource == null)
            Debug.LogWarning("⚠ ShowerController: AudioSource component is missing!");
    }

    void Update()
    {

        if (isShowering && petTarget != null)
        {

            transform.position = petTarget.position + new Vector3(0, 1f, 0);
            showerTimer += Time.deltaTime;

            if (showerTimer >= showerDuration)
            {
                StopShower();
            }
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pet"))
        {
            StartShower();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Pet"))
        {
            StopShower();
        }
    }

    public void StartShower()
    {
        if (!isShowering)
        {
            isShowering = true;
            showerTimer = 0f;

            if (showerEffect != null)
                showerEffect.Play();

            if (audioSource != null)
            {
                audioSource.Play();
                Debug.Log("Audio Source is playing");
            }
            else
            {
                Debug.LogError("AudioSource is NULL");
            }
        }
    }

    public void StopShower()
    {
        if (isShowering)
        {
            if (!isShowering) return;

            isShowering = false;

            if (showerEffect != null)
                showerEffect.Stop();

            if (audioSource != null)
                audioSource.Stop();

            // ✅ Bring back the water drop
            if (waterDrop != null)
            {
                waterDrop.transform.position = new Vector3(-1.59f, -4.22f, 0);
                waterDrop.SetActive(true);
            }

            // ✅ Destroy this shower
            Destroy(gameObject);
        }
    }



}