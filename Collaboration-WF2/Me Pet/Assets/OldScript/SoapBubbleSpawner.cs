using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SoapBubbleSpawner : MonoBehaviour
{
    public GameObject bubble;
    public GameObject water;
    public Button leftButton;
    public Button rightButton;
    public float bubbleDelay = 0.7f;
    public int maxBubbles = 5;
    public float minDistanceBetweenBubbles = 20.5f;
    private bool hasShownHalfCleanMessage = false;
    private bool isBathing;

    private float lastBubbleTime = 0f;
    private List<GameObject> activeBubbles = new List<GameObject>();
    public AudioSource audio;

    public BathController bathController;


    private void Start()
    {
        water.SetActive(false);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Pet"))
            return;

        if (bathController == null)
            bathController = FindAnyObjectByType<BathController>();

        if (bathController == null)
            return;

        if (bathController.dirty >= 20f && audio != null)
        {
            audio.Play();
        }
        if (FindFirstObjectByType<PetStatus>().currentStage == PetStatus.PetStage.Kid)
        {
            Debug.Log("Bubble = maxbubble");
            maxBubbles = 4;
        }
        else if (FindFirstObjectByType<PetStatus>().currentStage == PetStatus.PetStage.Teen)
        {
            Debug.Log("Bubble = maxbubble");
            maxBubbles = 6;
        }
        else if (FindFirstObjectByType<PetStatus>().currentStage == PetStatus.PetStage.Adult)
        {
            Debug.Log("Bubble = maxbubble");
            maxBubbles = 8;
        }
        else if (FindFirstObjectByType<PetStatus>().currentStage == PetStatus.PetStage.Old)
        {
            Debug.Log("Bubble = maxbubble");
            maxBubbles = 10;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Pet"))
            return;

        if (audio != null)
            audio.Stop();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Pet"))
            return;

        if (bathController == null)
            bathController = FindAnyObjectByType<BathController>();

        if (bathController == null)
            return;

        // Not dirty enough
        if (bathController.dirty < 20f)
        {
            bathController.ShowCloudMessage("I still not so dirty yet >_<", 2f);
            return;
        }

        // ✅ Dirty enough: spawn bubbles / apply soap
        if (Time.time - lastBubbleTime > bubbleDelay && activeBubbles.Count < maxBubbles)
        {
            isBathing = true;
            PlayerPrefs.SetInt("IsBathing", isBathing ? 1 : 0);
            PlayerPrefs.Save();

            if (rightButton != null) rightButton.interactable = false;
            if (leftButton != null) leftButton.interactable = false;

            Vector3 spawnPos = other.ClosestPoint(transform.position);

            bool tooClose = false;
            foreach (GameObject bubbleObj in activeBubbles)
            {
                if (bubbleObj != null &&
                    Vector3.Distance(bubbleObj.transform.position, spawnPos) < minDistanceBetweenBubbles)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                GameObject newBubble = Instantiate(bubble, spawnPos, Quaternion.identity);
                activeBubbles.Add(newBubble);
                lastBubbleTime = Time.time;

                // Tell BathController that soap has been used
                bathController.OnSoapUsed();
            }

            // Enough bubbles -> show water drop
            if (activeBubbles.Count >= maxBubbles && water != null)
            {
                water.SetActive(true);
            }
        }
    }

    private void Update()
    {
        // Clean up destroyed bubbles
        activeBubbles.RemoveAll(b => b == null);

        if (bathController == null)
            bathController = FindAnyObjectByType<BathController>();

        if (bathController == null)
            return;

        // Show "almost there" message once
        if (!hasShownHalfCleanMessage &&
            activeBubbles.Count > maxBubbles / 2 &&
            activeBubbles.Count != maxBubbles)
        {
            bathController.ShowCloudMessage(
                "Almost there! Keep scrubbing to make your pet shine!",
                2.5f
            );
            Debug.Log("Message shown: Almost there!");
            hasShownHalfCleanMessage = true;
        }
    }
}
