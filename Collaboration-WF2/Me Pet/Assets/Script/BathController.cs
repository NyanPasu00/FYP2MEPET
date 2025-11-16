using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

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
    public float dirtyInterval = 50f;   // seconds
    private float lastMilestone = 0f;

    public Animator catAnimator;
    public bool isBathing;

    public Energy_Bar energy;

    [Header("Dirty Spawn Spot")]
    [Header("Hall")]
    public BoxCollider2D spawnArea;
    public BoxCollider2D spawnAreaLying;
    [Header("Kitchen")]
    public BoxCollider2D kitchenSpawnArea;
    [Header("Game Room")]
    public BoxCollider2D gameroomSpawnArea;
    [Header("Bathroom")]
    public BoxCollider2D bathroomSpawnArea;
    private List<GameObject> activeSpots = new List<GameObject>();

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
        if (dirty <= 0 && isShowering)
        {
            HandleFullyCleaned();
        }
    }

    /* ===================== SHOWER CONTROL ===================== */

    public void StartShower()
    {

        if (isShowering) return;

        Debug.Log("[BathController] StartShower");

        isShowering = true;
        showerTimer = 0f;

        // We do NOT move the shower. You place the shower head + water
        // at the correct position in the scene manually.
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

        waterDrop.SetActive(false);

    }

    /* ===================== CLEAN FINISH LOGIC ===================== */

    public void HandleFullyCleaned()
    {
        StopShower();
        hasShownHalfCleanMessage = false;
        firstTime = true;

        if (catAnimator != null && hasUsedSoap)
        {
            if (cat != null)
                cat.transform.position = originalCatPosition;

            catAnimator.SetBool("isClean", true);
            energy.increaseHappiness(10);
            ShowCloudMessage("All clean! Great job!", 2.5f);

            Invoke(nameof(ResetToIdle), 2f);
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
        PlayerPrefs.SetInt("IsBathing", isBathing ? 1 : 0);
        PlayerPrefs.Save();
    }

    /* ===================== DIRT GROW / SPAWN (same as before) ===================== */

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

    void IncreaseDirt()
    {
        if (dirty >= maxDirty) return;

        dirty++;

        if (dirty % 20 == 0 && dirty != lastMilestone)
        {
            lastMilestone = dirty;
            SpawnDirtySpots(2);
        }

        if (dirty >= 60)
        {
            ShowCloudMessage("I need to bath !!! :(", 2.5f);
        }
    }

    void SpawnDirtySpots(int amount)
    {
        List<BoxCollider2D> areas = new List<BoxCollider2D>();
        if (spawnArea != null) areas.Add(spawnArea);
        if (spawnAreaLying != null) areas.Add(spawnAreaLying);
        if (kitchenSpawnArea != null) areas.Add(kitchenSpawnArea);
        if (gameroomSpawnArea != null) areas.Add(gameroomSpawnArea);
        if (bathroomSpawnArea != null) areas.Add(bathroomSpawnArea);

        if (areas.Count == 0) return;

        for (int i = 0; i < amount; i++)
        {
            BoxCollider2D areaToUse = areas[Random.Range(0, areas.Count)];
            Bounds bounds = areaToUse.bounds;

            GameObject spot = Instantiate(dirtySpotPrefab);
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
}