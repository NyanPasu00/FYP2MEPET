using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CatDirtyManager : MonoBehaviour
{
    public GameObject dirtySpotPrefab;   // Assign a prefab in the Inspector
    public GameObject cat;               // Reference to the cat GameObject
    public GameObject cloud;
    public Button leftButton;
    public Button rightButton;
    public TMP_Text messageText;
    public float maxDirty = 100;
    public float dirty = 0;
    public bool hasUsedSoap = false;
    private bool hasShownHalfCleanMessage = false;
    public bool firstTime = true;
    private float dirtyTimer = 0f;
    public float dirtyInterval = 50f; // seconds
    private float lastMilestone = 0;
    public ShowerController currentShower;
    public Animator catAnimator;
    public bool isBathing;

    public Energy_Bar energy;
    public BoxCollider2D spawnArea;
    public BoxCollider2D spawnAreaLying;
    private List<GameObject> activeSpots = new List<GameObject>();
    void Start()
    {
        //cat.transform.position = new Vector3(0.62f, -1.5f, 0f);
        dirty = dirty;
        CheckMilestoneAndSpawn();
        
    }

    void CheckMilestoneAndSpawn()
    {
        if (dirty == 100)
        {
            lastMilestone = 100;
            SpawnDirtySpots(10);
            cloud.SetActive(true);
        }
        else if (dirty >= 80)
        {
            lastMilestone = 80;
            SpawnDirtySpots(8);
        }
        else if (dirty >= 60)
        {
            lastMilestone = 60;
            SpawnDirtySpots(6);
        }
        else if (dirty >= 40)
        {
            lastMilestone = 40;
            SpawnDirtySpots(4);
        }
        else if (dirty >= 20)
        {
            lastMilestone = 20;
            SpawnDirtySpots(2);
        }
    }

    public void HandleFullyCleaned()
    {
        if (currentShower != null)
        {
            currentShower.StopShower();
            currentShower = null;
        }

        hasShownHalfCleanMessage = false;
        firstTime = true;

        if (catAnimator != null && hasUsedSoap)
        {
            cat.transform.position = new Vector3(-0.04f, -1.5f, 0f);
            catAnimator.SetBool("isClean", true);
            energy.increaseHappiness(10);
            Debug.Log("clean cat");
            ShowCloudMessage("All clean! Great job!", 2.5f);

            Invoke(nameof(ResetToIdle), 2f);
        }
    }

    void Update()
    {
        dirtyTimer += Time.deltaTime;

        if (dirtyTimer >= dirtyInterval)
        {
            dirtyTimer = 0f;
            IncreaseDirt();
        }

        if (dirty <= 0 && currentShower != null)
        {
            currentShower.StopShower();
            currentShower = null;

            
            hasShownHalfCleanMessage = false; // Reset for future use
            firstTime = true;

            if (catAnimator != null && hasUsedSoap)
            {
                
                cat.transform.position = new Vector3(-0.04f, -1.5f, 0f);
                catAnimator.SetBool("isClean", true);
                energy.increaseHappiness(10);
                Debug.Log("clean cat");
                ShowCloudMessage("All clean! Great job!", 2.5f);  // Clean message

                // 🆕 After 2 seconds, set isClean back to false
                Invoke(nameof(ResetToIdle), 2f);
            }
                
            

        }
    }

    void ResetToIdle()
    {
        if (catAnimator != null)
        {
            
            cat.transform.position = new Vector3(0.197f, -1.5f, 0f);
            catAnimator.SetBool("isClean", false);
            leftButton.interactable = true;
            rightButton.interactable = true;
            hasUsedSoap = false;
            isBathing = false;
            PlayerPrefs.SetInt("IsBathing", isBathing ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    void IncreaseDirt()
    {
        if (dirty < maxDirty)
        {
            dirty++;
            Debug.Log("Dirt increased: " + dirty);

            if (dirty % 20 == 0 && dirty != lastMilestone)
            {
                lastMilestone = dirty;
                SpawnDirtySpots(2);
            }

            //if (dirty >= maxDirty)
            //{
            //    cloud.SetActive(true);
            //    Debug.Log("The cat needs a bath!");
            //}
            if (dirty >= 60)
            {
                ShowCloudMessage("I need to bath !!! :(", 2.5f);
                Debug.Log("The cat needs a bath!");
            }

        }
    }

    void SpawnDirtySpots(int amount)
    {
       

        // Choose correct spawn area
        BoxCollider2D areaToUse = spawnArea;


        Bounds bounds = areaToUse.bounds;

        for (int i = 0; i < amount; i++)
        {
            GameObject spot = Instantiate(dirtySpotPrefab);

            // Set parent to the cat
            spot.transform.SetParent(cat.transform);

            // Random world position within bounds
            Vector3 randomWorldPos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                0
            );

            spot.transform.position = randomWorldPos;

            // Random scale
            float randomScale = Random.Range(0.01f, 0.03f);
            spot.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            activeSpots.Add(spot);

            Debug.Log("Spawned dirty spot at: " + spot.transform.position);
        }
    }


    public void ShowCloudMessage(string msg, float duration = 2f)
    {
        if (cloud != null)
            cloud.SetActive(true);

        if (messageText != null)
            messageText.text = msg;

        CancelInvoke("HideCloudMessage");
        Invoke(nameof(HideCloudMessage), duration);
    }

    public void HideCloudMessage()
    {
        if (cloud != null)
            cloud.SetActive(false);
        hasShownHalfCleanMessage = true;
    }

    public void DecreaseDirtGradually(float amount)
    {
        if (dirty > 0)
        {
            dirty -= amount;
            dirty = Mathf.Clamp(dirty, 0, maxDirty); // Prevent going below 0
            Debug.Log("Dirt decreased to: " + dirty);
        }
    }

    // Handle the soap being used (called from SoapBubbleSpawner)
    public void OnSoapUsed()
    {
        hasUsedSoap = true; // Set soap flag to true
        hasShownHalfCleanMessage = false; // Reset for future use

        if (firstTime == true && dirty >= maxDirty)
        {
            // Hide the message because soap is being used
            HideCloudMessage();
            firstTime = false;
        }
    }
}
