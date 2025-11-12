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


    private void Start()
    {
        water.SetActive(false);
        if (FindFirstObjectByType<Energy_Bar>().currentStage == Energy_Bar.PetStage.Kid)
        {
            maxBubbles = 4;
        }
        else if (FindFirstObjectByType<Energy_Bar>().currentStage == Energy_Bar.PetStage.Teen)
        {
            maxBubbles = 6;
        }
        else if (FindFirstObjectByType<Energy_Bar>().currentStage == Energy_Bar.PetStage.Adult)
        {
            maxBubbles = 8;
        }
        else if (FindFirstObjectByType<Energy_Bar>().currentStage == Energy_Bar.PetStage.Old)
        {
            maxBubbles = 10;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Pet")){
            CatDirtyManager catManager = FindAnyObjectByType<CatDirtyManager>();

            if (catManager == null)
            {
                return;
            }
            if (catManager.dirty >= 20)
            {
                audio.Play();
            }
           
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Pet"))
        {
            audio.Stop();
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Pet"))
        {

            CatDirtyManager catManager = FindAnyObjectByType<CatDirtyManager>();

            if (catManager == null)
                return;

            // ❌ Show message if not dirty
            if (catManager.dirty < 20)
            {
                catManager.ShowCloudMessage("I still not so dirty yet >_<", 2f);
                return;
            }

            // ✅ Dirty enough: apply soap
            if (Time.time - lastBubbleTime > bubbleDelay && activeBubbles.Count < maxBubbles)
            {
                isBathing = true;
                PlayerPrefs.SetInt("IsBathing", isBathing ? 1 : 0);
                PlayerPrefs.Save();
                rightButton.interactable = false;
                leftButton.interactable = false;
                Vector3 spawnPos = other.ClosestPoint(transform.position);

                bool tooClose = false;
                foreach (GameObject bubbleObj in activeBubbles)
                {
                    if (bubbleObj != null && Vector3.Distance(bubbleObj.transform.position, spawnPos) < minDistanceBetweenBubbles)
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

                    catManager.OnSoapUsed();

                }
                if (activeBubbles.Count >= maxBubbles)
                {
                    water.SetActive(true);
                }
            }
        }
    }

    void Update()
    {
        activeBubbles.RemoveAll(b => b == null);
        CatDirtyManager catManager = FindAnyObjectByType<CatDirtyManager>();

        if (activeBubbles.Count > maxBubbles/2 && activeBubbles.Count != maxBubbles && !hasShownHalfCleanMessage)
        {
            catManager.ShowCloudMessage("Almost there! Keep scrubbing to make your pet shine!", 2.5f);
            Debug.Log("Message shown: Almost there!");
            
        }
    }
}
