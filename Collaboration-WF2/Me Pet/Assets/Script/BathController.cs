using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BathController : MonoBehaviour
{
    [Header("Cat & Dirt")]
    public GameObject dirtySpotPrefab;
    public GameObject cat;
    public GameObject cloud;
    public Button leftButton;
    public Button rightButton;
    public TMP_Text messageText;

    public float maxDirty = 100f;
    [Range(0, 100)] public float dirty = 0f;

    public bool hasUsedSoap = false;
    private bool hasShownHalfCleanMessage = false;
    public bool firstTime = true;

    private float dirtyTimer = 0f;
    public float dirtyInterval = 60f;   // seconds
    private float lastMilestone = 0f;

    public Animator catAnimator;
    public bool isBathing;

    public PetStatus energy;

    [Header("Dirty Spawn Spot")]
    [Header("Hall")]
    public BoxCollider2D hallLyingSpawnArea;   // pet lying / sleeping / idle
    public BoxCollider2D hallDanceSpawnArea;   // pet dancing (likes music)
    public BoxCollider2D hallSadSpawnArea;     // pet sad (dislikes music)
    [Header("Kitchen")]
    public BoxCollider2D kitchenSpawnArea;
    [Header("Game Room")]
    public BoxCollider2D gameroomSpawnArea;
    [Header("Bathroom")]
    public BoxCollider2D bathroomSpawnArea;
    [Header("Medication")]
    public BoxCollider2D medicSpawnArea;
    private List<GameObject> activeSpots = new List<GameObject>();

    public enum HallState
    {
        Lying,
        Dance,
        Sad
    }

    [Header("Current Hall State")]
    public HallState hallState = HallState.Lying;

    [Header("Hall Pose State")]
    [Tooltip("True = pet standing/dancing. False = pet lying (default).")]
    public bool hallStanding = false;

    private bool lastHallStandingState = false;

    [Header("Shower")]
    public ParticleSystem showerEffect;    // was in ShowerController
    public GameObject showerHead;
    public Transform petTarget;            // pet position to follow
    public GameObject waterDrop;           // the draggable drop icon
    public Transform waterDropStartPos;    // where waterDrop returns to
    public AudioSource audioSource;
    public bool isShowering = false;
    private float showerTimer = 0f;
    public float showerDuration = 5f;

    private Renderer showerWaterRenderer;
    private SpriteRenderer showerHeadRenderer;

    // store original cat position so it doesn't "teleport"
    private Vector3 originalCatPosition;

    private bool hasGivenBathReward = false;
    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (showerEffect != null)
            showerWaterRenderer = showerEffect.GetComponent<Renderer>();

        if (showerHead != null)
            showerHeadRenderer = showerHead.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (cat != null)
            originalCatPosition = cat.transform.position;

        // Make sure shower visuals start hidden
        if (showerHead != null)
            showerHead.SetActive(false);
        if (showerEffect != null)
            showerEffect.Stop();

        CheckMilestoneAndSpawn();
    }

    void Update()
    {
        // --------- Dirt grows over time ----------
        dirtyTimer += Time.deltaTime;
        if (dirtyTimer >= dirtyInterval)
        {
            dirtyTimer = 0f;
            IncreaseDirt();
        }

        // --------- Shower timer ----------
        if (isShowering)
        {
            showerTimer += Time.deltaTime;
            if (showerTimer >= showerDuration)
            {
                StopShower();
            }
        }

        // --------- Fully clean while showering ----------
        //if (dirty <= 0 && isShowering)
        //{
        //    HandleFullyCleaned();
        //}
    }

    /* ===================== PUBLIC POSE API ===================== */

    // Call this from UIController / music controller when pose changes
    public void SetHallState(HallState newState)
    {
        if (hallState == newState)
            return; // no change, no refresh

        hallState = newState;

        // Wait a frame so pet + colliders finish moving, then respawn dirt
        StartCoroutine(RefreshDirtySpotsNextFrame());
    }

    private IEnumerator RefreshDirtySpotsNextFrame()
    {
        // Let Update/LateUpdate/Animator finish this frame
        yield return null;

        RefreshDirtySpots();
    }

    // Optional: keep old API working
    public void SetHallStanding(bool standing)
    {
        // standing → use Dance as default; lying → Lying
        SetHallState(standing ? HallState.Dance : HallState.Lying);
    }

    /* ===================== SHOWER CONTROL ===================== */

    public void StartShower()
    {
        if (isShowering) return;

        Debug.Log("[BathController] StartShower");

        isShowering = true;
        showerTimer = 0f;
        hasGivenBathReward = false;

        if (showerHead != null)
        {
            showerHead.SetActive(true);
            if (showerHeadRenderer != null)
                showerHeadRenderer.sortingOrder = 200;
        }

        if (showerEffect != null)
        {
            showerEffect.gameObject.SetActive(true);
            showerEffect.Play();
            if (showerWaterRenderer != null)
                showerWaterRenderer.sortingOrder = 201;
        }

        if (audioSource != null)
            audioSource.Play();
    }

    public void StopShower()
    {
        if (!isShowering) return;

        Debug.Log("[BathController] StopShower");

        isShowering = false;

        if (showerEffect != null)
            showerEffect.Stop();

        if (audioSource != null)
            audioSource.Stop();

        if (showerHead != null)
            showerHead.SetActive(false);

        if (waterDrop != null)
            waterDrop.SetActive(false);
    }

    /* ===================== CLEAN FINISH LOGIC ===================== */

    public void HandleFullyCleaned()
    {
        // 1. force dirty to 0
        dirty = 0f;

        // 2. remove all spawned dirty spots
        foreach (var spot in activeSpots)
        {
            if (spot != null) Destroy(spot);
        }
        activeSpots.Clear();

        // 3. stop shower visuals / sound
        StopShower();

        hasShownHalfCleanMessage = false;
        firstTime = true;

        // 4. animation + happiness
        if (catAnimator != null && hasUsedSoap)
        {
            if (cat != null)
                cat.transform.position = originalCatPosition;

            catAnimator.SetBool("isClean", true);
            

            ShowCloudMessage("All clean! Great job!", 2.5f);

            Invoke(nameof(ResetToIdle), 2f);
        }

        // 5. sync to PetData (optional but good)
        if (energy != null)
        {
            energy.SavePetData();   // this will store dirty = 0
        }
    }

    private void ResetToIdle()
    {
        if (catAnimator == null) return;

        if (cat != null)
            cat.transform.position = originalCatPosition;

        catAnimator.SetBool("isClean", false);
        leftButton.interactable = true;
        rightButton.interactable = true;
        hasUsedSoap = false;
        isBathing = false;
        if (!hasGivenBathReward && dirty == 0f)
        {
            hasGivenBathReward = true;
            Debug.Log("[BathController] Bath reward given (+20 happiness?)");

            if (energy != null)
            {
                energy.updateBathStatus();  // your +20
                energy.updateWhenDirty();   // stop dirty penalty
            }
        }
        CheckMilestoneAndSpawn();
        PlayerPrefs.SetInt("IsBathing", isBathing ? 1 : 0);
        PlayerPrefs.Save();
    }

    /* ===================== DIRT GROW / SPAWN ===================== */

    void CheckMilestoneAndSpawn()
    {
        int spotsPerArea = 0;

        if (dirty >= 100 && lastMilestone < 100)
        {
            lastMilestone = 100;
            spotsPerArea = 10;
            if (cloud != null) cloud.SetActive(true);
        }
        else if (dirty >= 80 && lastMilestone < 80)
        {
            lastMilestone = 80;
            spotsPerArea = 8;
        }
        else if (dirty >= 60 && lastMilestone < 60)
        {
            lastMilestone = 60;
            spotsPerArea = 6;
        }
        else if (dirty >= 40 && lastMilestone < 40)
        {
            lastMilestone = 40;
            spotsPerArea = 4;
        }
        else if (dirty >= 20 && lastMilestone < 20)
        {
            lastMilestone = 20;
            spotsPerArea = 2;
        }

        if (spotsPerArea > 0)
        {
            SpawnDirtySpotsPerArea(spotsPerArea);
        }
    }

    void IncreaseDirt()
    {
        if (dirty >= maxDirty) return;

        dirty++;

        CheckMilestoneAndSpawn();

        if (dirty >= 60)
        {
            ShowCloudMessage("I need to bath !!! :(", 2.5f);
        }

        if (energy != null)
        {
            energy.updateWhenDirty();
        }
    }

    void SpawnDirtySpotsPerArea(int spotsPerArea)
    {
        List<BoxCollider2D> areas = new List<BoxCollider2D>();

        // -------- HALL: choose ONE collider based on hallState --------
        BoxCollider2D hallArea = null;
        switch (hallState)
        {
            case HallState.Lying:
                hallArea = hallLyingSpawnArea;
                break;

            case HallState.Dance:
                hallArea = hallDanceSpawnArea;
                break;

            case HallState.Sad:
                hallArea = hallSadSpawnArea;
                break;
        }

        if (hallArea != null)
            areas.Add(hallArea);

        // -------- Other rooms --------
        if (kitchenSpawnArea != null) areas.Add(kitchenSpawnArea);
        if (gameroomSpawnArea != null) areas.Add(gameroomSpawnArea);
        if (bathroomSpawnArea != null) areas.Add(bathroomSpawnArea);
        if (medicSpawnArea != null) areas.Add(medicSpawnArea);

        if (areas.Count == 0)
        {
            Debug.LogWarning("[BathController] No spawn areas assigned.");
            return;
        }

        foreach (BoxCollider2D area in areas)
        {
            for (int i = 0; i < spotsPerArea; i++)
            {
                SpawnSingleSpotInArea(area);
            }
        }
    }

    void SpawnSingleSpotInArea(BoxCollider2D area)
    {
        Bounds bounds = area.bounds;

        GameObject spot = Instantiate(dirtySpotPrefab);

        if (cat != null)
            spot.transform.SetParent(cat.transform);

        Vector3 randomWorldPos = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            0
        );

        spot.transform.position = randomWorldPos;

        float randomScale = Random.Range(0.01f, 0.03f);
        spot.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

        activeSpots.Add(spot);
    }


    /* ===================== UI / SOAP ===================== */

    public void ShowCloudMessage(string msg, float duration = 2f)
    {
        if (cloud != null) cloud.SetActive(true);
        if (messageText != null) messageText.text = msg;

        CancelInvoke(nameof(HideCloudMessage));
        Invoke(nameof(HideCloudMessage), duration);
    }

    public void HideCloudMessage()
    {
        if (cloud != null) cloud.SetActive(false);
        hasShownHalfCleanMessage = true;
    }

    public void DecreaseDirtGradually(float amount)
    {
        if (dirty <= 0) return;

        dirty -= amount;
        dirty = Mathf.Clamp(dirty, 0, maxDirty);

        if (dirty <= 0 && isShowering)
            HandleFullyCleaned();

        if (energy != null)
        {
            energy.updateWhenDirty();
        }
    }

    public void OnSoapUsed()
    {
        hasUsedSoap = true;
        hasShownHalfCleanMessage = false;

        if (firstTime && dirty >= maxDirty)
        {
            HideCloudMessage();
            firstTime = false;
        }
    }

    public void RefreshDirtySpots()
    {
        // Clear old spots
        foreach (var spot in activeSpots)
        {
            if (spot != null) Destroy(spot);
        }
        activeSpots.Clear();

        // Reset milestone so current dirt level re-triggers spawn
        lastMilestone = 0;
        CheckMilestoneAndSpawn();
    }
}