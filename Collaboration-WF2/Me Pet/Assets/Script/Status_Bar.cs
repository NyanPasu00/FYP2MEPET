using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class Energy_Bar : MonoBehaviour
{
    [Header("Player First Time? / Pet Die?")]
    public bool firstTimePlay = true;
    public bool petDead;

    [Header("Is the player having movement?")]
    public bool isEating;
    public bool isDancing;
    public bool isSleeping;
    public bool isBathing;
    public bool isAlbumOpen;

    public bool progressStop = false;

    [Header("Which Stage Player Reach?")]
    public static bool hasReachedTeenHalf = false;
    public static bool hasReachedAdultHalf = false;
    public static bool hasReachedOldHalf = false;


    //New Game Enter Name
    [SerializeField]
    [Header("Input Name Field")]
    public TMP_InputField nameInputField;

    [System.Serializable]
    public class FoodEntry
    {
        public string foodName;
        public int quantity;

        public FoodEntry(string name, int qty)
        {
            foodName = name;
            quantity = qty;
        }
    }

    [System.Serializable]
    public class PetData
    {
        public string petName;
        public float dirty;
        public int energy;
        public int hunger;
        public int happiness;
        public int health;
        public int progress;
        public PetStage stage;
        public PetStage represent;
        public string lastSavedTime; // Store as string to serialize easily
        public bool firstTime;
        public float lastEnergySecond;
        public float lastHealthSecond;
        public float lastProgressSecond;
        public float lastHappinessSecond;
        public float lastHungerSecond;
        public int moneyValue;
        public List<FoodEntry> ownedItems = new List<FoodEntry>();
    }

    public int moneyValue;
    public Dictionary<string, int> ownedItems = new Dictionary<string, int>();

    [Header("Bath / Dirty Controller")]
    public BathController bathController;

    [SerializeField]
    [Header("Energy")]
    public int energy_max = 100;
    public int energy_current;
    public Slider energy_Slider;
    public Slider energyDetail_Slider;
    public float energy_deduct_time = 60f;

    [Header("Hunger")]
    public int hunger_max = 100;
    public int hunger_current;
    public Slider hunger_Slider;
    public Slider hungerDetail_Slider;
    public float hunger_deduct_time = 60f;

    [Header("Happiness")]
    public int happiness_max = 100;
    public int happiness_current;
    public Slider happiness_Slider;
    public Slider happinessDetail_Slider;
    public float happiness_deduct_time = 60f;

    [Header("Health")]
    public int health_max = 100;
    public int health_current;
    public Slider health_Slider;
    public Slider healthDetail_Slider;
    public float health_deduct_time = 60f;

    [Header("Progress")]
    public int progress_max = 100;
    public int progress_current;
    public Image progress_Image;
    public Slider progressDetail_Slider;
    public float progress_increase_time = 60f;

    private Coroutine energyDeductCoroutine;
    private Coroutine happinessDeductCoroutine;
    public enum PetStage
    {
        Kid,
        Teen,
        Adult,
        Old
    }

    public enum PetStageRepresent
    {
        K,
        T,
        A,
        O
    }

    public PetStage currentStage = PetStage.Kid;
    public TextMeshProUGUI stageRepresent;
    public TMP_Text moneyText;

    private float lastEnergyTime = 0f;
    private float lastHungerTime = 0f;
    private float lastHappinessTime = 0f;
    private float lastHealthTime = 0f;
    private float lastProgressTime = 0f;

    //public Dictionary<> eventCompleted;
    //public Dictionary<> musicPreferences;
    
    void Start()
    {
        LoadPetData();
        UpdateAllUI();


        energyDeductCoroutine = StartCoroutine(DeductEnergyOverTime());
        StartCoroutine(DeductHungerOverTime());
        happinessDeductCoroutine = StartCoroutine(DeductHappinessOverTime());
        StartCoroutine(IncreaseProgressOverTime());

        if (currentStage == PetStage.Old)
        {
            StartCoroutine(DeductHealthOverTime());
        }


    }

    IEnumerator DeductEnergyOverTime()
    {
        while (energy_current > 0)
        {
            while (lastEnergyTime < energy_deduct_time)
            {
                lastEnergyTime += Time.deltaTime;
                yield return null;
            }

            DeductEnergy(1);
            lastEnergyTime = 0f;
        }
    }

    IEnumerator DeductHungerOverTime()
    {
        while (hunger_current > 0)
        {
            while (lastHungerTime < hunger_deduct_time)
            {
                lastHungerTime += Time.deltaTime;
                yield return null;
            }

            DeductHunger(1);
            lastHungerTime = 0f;
        }
    }

    IEnumerator DeductHappinessOverTime()
    {

        while (happiness_current > 0)
        {
            while (lastHappinessTime < happiness_deduct_time)
            {
                lastHappinessTime += Time.deltaTime;
                yield return null;
            }

            DeductHappiness(1);
            lastHappinessTime = 0f;
        }
    }

    IEnumerator DeductHealthOverTime()
    {

        while (health_current > 0)
        {
            while (lastHealthTime < health_deduct_time)
            {
                lastHealthTime += Time.deltaTime;
                yield return null;
            }

            DeductHealth(1);
            lastHealthTime = 0f;
        }
    }

    IEnumerator IncreaseProgressOverTime()
    {

        while (progress_current <= 100)
        {
            while (lastProgressTime < progress_increase_time)
            {
                lastProgressTime += Time.deltaTime;
                yield return null;
            }

            IncreaseProgress(1);
            lastProgressTime = 0f;
        }
    }

    public void IncreaseHealth()
    {
        health_current = health_current + 10;
        if (health_current >= 100)
        {
            health_current = 100;
        }
        GetHealthFill();
    }

    public void IncreaseFood()
    {
        hunger_current = hunger_current + 10;
        if (hunger_current >= 100)
        {
            hunger_current = 100;
        }
        GetHungerFill();
    }

    public void GamePlayEnergyNeed()
    {

        if (energy_current <= 30)
        {
            GetEnergyFill();
        }
        else
        {
            energy_current = energy_current - 15;
            GetEnergyFill();
        }
    }

    public void increaseHappiness(int happiness)
    {
        happiness_current = happiness_current + happiness;
        if (happiness_current >= 100)
        {
            happiness_current = 100;
        }
        GetHappinessFill();
    }

    public void decreaseHappiness(int happiness)
    {
        happiness_current = happiness_current - happiness;
        if (happiness_current <= 0)
        {
            happiness_current = 0;
        }
        GetHappinessFill();
    }

    void DeductEnergy(int percent)
    {

        int amountToDeduct = Mathf.CeilToInt((percent / 100f) * energy_max);
        energy_current = Mathf.Max(0, energy_current - amountToDeduct);
        GetEnergyFill();

    }

    void DeductHunger(int percent)
    {
        int amountToDeduct = Mathf.CeilToInt((percent / 100f) * hunger_max);
        hunger_current = Mathf.Max(0, hunger_current - amountToDeduct);
        GetHungerFill();


    }

    void DeductHappiness(int percent)
    {
        int amountToDeduct = Mathf.CeilToInt((percent / 100f) * happiness_max);
        happiness_current = Mathf.Max(0, happiness_current - amountToDeduct);
        GetHappinessFill();


    }
    void DeductHealth(int percent)
    {
        int amountToDeduct = Mathf.CeilToInt((percent / 100f) * health_max);
        health_current = Mathf.Max(0, health_current - amountToDeduct);
        GetHealthFill();


    }

    public void GetEnergyFill()
    {
        energy_Slider.value = (float)energy_current / energy_max;
        energyDetail_Slider.value = (float)energy_current / energy_max;
    }
    void GetHungerFill()
    {
        hunger_Slider.value = (float)hunger_current / hunger_max;
        hungerDetail_Slider.value = (float)hunger_current / hunger_max;
    }
    void GetHappinessFill()
    {
        happiness_Slider.value = (float)happiness_current / happiness_max;
        happinessDetail_Slider.value = (float)happiness_current / happiness_max;
    }
    void GetHealthFill()
    {
        health_Slider.value = (float)health_current / health_max;
        healthDetail_Slider.value = (float)health_current / health_max;
    }

    void GetProgressFill()
    {
        progress_Image.fillAmount = (float)progress_current / progress_max;
        progressDetail_Slider.value = (float)progress_current / progress_max;
    }
    public void IncreaseProgress(int value)
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "TeenStressEvent")
        {
            StopAllCoroutines();

        }
        if (currentScene == "AdultStressEvent")
        {
            StopAllCoroutines();

        }
        if (currentScene == "FindFriendSceneTest")
        {
            StopAllCoroutines();

        }


        progress_current += value;
        GetProgressFill();
        if (currentStage == PetStage.Kid)
            stageRepresent.text = $"{PetStageRepresent.K}\n";

        if (progress_current >= 50)
        {
            if (isBathing == false && isAlbumOpen == false && isDancing == false && isSleeping == false && isEating == false)
            {
                changeEventScene();
            }
            else if (isBathing == true || isAlbumOpen == true || isDancing == true || isSleeping == true || isEating == true)
            {
                progress_increase_time = 1;
                if (currentStage == PetStage.Teen && !hasReachedTeenHalf)
                {
                    progress_current = 50;
                }
                else if (currentStage == PetStage.Adult && !hasReachedAdultHalf)
                {
                    progress_current = 50;
                }
                else if (currentStage == PetStage.Old && !hasReachedOldHalf)
                {
                    progress_current = 50;
                }

                progressStop = true;
            }

        }

        if (progress_current >= progress_max)
        {
            progress_current = progress_max;
            if (isBathing == false && isAlbumOpen == false && isDancing == false && isSleeping == false && isEating == false)
            {
                AdvanceStage();
            }
            else
            {
                progress_increase_time = 1;
                progressStop = true;
            }
        }

    }

    void changeEventScene()
    {
        if (currentStage == PetStage.Teen && !hasReachedTeenHalf)
        {
            progress_increase_time = 60f;
            hasReachedTeenHalf = true;
            PlayerPrefs.SetInt("hasReachedTeenHalf", hasReachedTeenHalf ? 1 : 0);
            PlayerPrefs.Save();
            HandleHalfProgressEvent();
            UnityEngine.SceneManagement.SceneManager.LoadScene("TeenStressEvent");
        }
        else if (currentStage == PetStage.Adult && !hasReachedAdultHalf)
        {
            progress_increase_time = 60f;
            hasReachedAdultHalf = true;
            PlayerPrefs.SetInt("hasReachedAdultHalf", hasReachedAdultHalf ? 1 : 0);
            PlayerPrefs.Save();
            HandleHalfProgressEvent();
            UnityEngine.SceneManagement.SceneManager.LoadScene("AdultStressEvent");
        }
        else if (currentStage == PetStage.Old && !hasReachedOldHalf)
        {
            progress_increase_time = 60f;
            hasReachedOldHalf = true;
            PlayerPrefs.SetInt("hasReachedOldHalf", hasReachedOldHalf ? 1 : 0);
            PlayerPrefs.Save();
            HandleHalfProgressEvent();
            UnityEngine.SceneManagement.SceneManager.LoadScene("FindFriendSceneTest");
        }
    }
    void AdvanceStage()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        progress_increase_time = 60f;
        if (currentStage == PetStage.Kid)
        {
            progressStop = false;
            currentStage = PetStage.Teen;
            stageRepresent.text = $"{PetStageRepresent.T}\n";
            progress_current = 0;
            progress_Image.fillAmount = 0f;
            progressDetail_Slider.value = 0f;
            SavePetData();

            UnityEngine.SceneManagement.SceneManager.LoadScene("KidToTeen");

        }
        else if (currentStage == PetStage.Teen)
        {
            progressStop = false;
            currentStage = PetStage.Adult;
            stageRepresent.text = $"{PetStageRepresent.A}\n";
            progress_current = 0;
            progress_Image.fillAmount = 0f;
            progressDetail_Slider.value = 0f;

            SavePetData();

            UnityEngine.SceneManagement.SceneManager.LoadScene("TeenToAdult");
        }
        else if (currentStage == PetStage.Adult)
        {
            progressStop = false;
            currentStage = PetStage.Old;
            stageRepresent.text = $"{PetStageRepresent.O}\n";
            progress_current = 0;
            progress_Image.fillAmount = 0f;
            progressDetail_Slider.value = 0f;
            StartCoroutine(DeductHealthOverTime());
            SavePetData();

            UnityEngine.SceneManagement.SceneManager.LoadScene("AdultToOld");
        }
        else
        {
            progressStop = false;
            PlayerPrefs.SetInt("CauseOfDeath", 4); // 5 = Energy death (you can define it)
            PlayerPrefs.SetString("CurrentStage", currentStage.ToString()); // assume you have this variable
            PlayerPrefs.Save();

            // Load the 1LastWord scene
            petDead = true;
            hasReachedTeenHalf = false;
            hasReachedAdultHalf = false;
            hasReachedTeenHalf = false;
            PlayerPrefs.SetInt("hasReachedTeenHalf", hasReachedTeenHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("hasReachedAdultHalf", hasReachedAdultHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("hasReachedOldHalf", hasReachedTeenHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("PetDead", petDead ? 1 : 0);
            PlayerPrefs.Save();
            FindFirstObjectByType<BGMScript>().StopMusic();
            SceneManager.LoadScene("1LastWord");

        }

        // If already Old, you can decide whether to do nothing or show "Passed Away"
    }

    void OnApplicationQuit()
    {
        SavePetData();
        isBathing = false;
        PlayerPrefs.SetInt("IsBathing", isBathing ? 1 : 0);
        isDancing = false;
        PlayerPrefs.SetInt("IsDancing", isDancing ? 1 : 0);
        isEating = false;
        PlayerPrefs.SetInt("IsEating", isEating ? 1 : 0);
        isAlbumOpen = false;
        PlayerPrefs.SetInt("IsAlbumOpen", isAlbumOpen ? 1 : 0);
        isSleeping = false;
        PlayerPrefs.SetInt("IsSleeping", isSleeping ? 1 : 0);

        PlayerPrefs.Save();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SavePetData();
            isBathing = false;
            PlayerPrefs.SetInt("IsBathing", isBathing ? 1 : 0);
            isDancing = false;
            PlayerPrefs.SetInt("IsDancing", isDancing ? 1 : 0);
            isEating = false;
            PlayerPrefs.SetInt("IsEating", isEating ? 1 : 0);
            isAlbumOpen = false;
            PlayerPrefs.SetInt("IsAlbumOpen", isAlbumOpen ? 1 : 0);
            isSleeping = false;
            PlayerPrefs.SetInt("IsSleeping", isSleeping ? 1 : 0);

            PlayerPrefs.Save();
        }
    }

    public void createPetData()
    {
        string petNewName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(petNewName))
        {
            Debug.LogWarning("Pet name is empty!");
            return;
        }

        PetData data = new PetData();
        data.petName = petNewName;
        data.dirty = 0;
        data.energy = 100;
        data.hunger = 100;
        data.happiness = 100;
        data.health = 100;
        data.progress = 0;
        data.stage = PetStage.Kid;
        data.lastSavedTime = System.DateTime.Now.ToString();
        data.firstTime = true;
        data.lastEnergySecond = 0f;
        data.lastHappinessSecond = 0f;
        data.lastHealthSecond = 0f;
        data.lastProgressSecond = 0f;
        data.lastHungerSecond = 0f;
        data.moneyValue = 0;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PetData", json);
        PlayerPrefs.Save();

        petDead = false;
        PlayerPrefs.SetInt("PetDead", petDead ? 1 : 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene("KidScene");
    }

    public void SavePetData()
    {
        PetData data;

        // Load existing data first so we keep petName
        if (PlayerPrefs.HasKey("PetData"))
        {
            string oldJson = PlayerPrefs.GetString("PetData");
            data = JsonUtility.FromJson<PetData>(oldJson) ?? new PetData();
        }
        else
        {
            data = new PetData();
        }


        if (bathController == null)
            bathController = FindAnyObjectByType<BathController>();

        data.ownedItems.Clear();
        foreach (var kvp in ownedItems)  // your current dictionary
        {
            data.ownedItems.Add(new FoodEntry(kvp.Key, kvp.Value));
        }

        data.dirty = bathController != null ? bathController.dirty : 0f;
        data.energy = energy_current;
        data.hunger = hunger_current;
        data.happiness = happiness_current;
        data.health = health_current;
        data.progress = progress_current;
        data.stage = currentStage;
        data.moneyValue = moneyValue;
        data.lastSavedTime = System.DateTime.Now.ToString();
        data.firstTime = false;
        data.lastEnergySecond = lastEnergyTime;
        data.lastHappinessSecond = lastHappinessTime;
        data.lastHealthSecond = lastHealthTime;
        data.lastProgressSecond = lastProgressTime;
        data.lastHungerSecond = lastHungerTime;

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PetData", json);
        PlayerPrefs.Save();
        Debug.Log("Save folder: " + Application.persistentDataPath);
        Debug.Log("dirty : " + data.dirty);
    }

    void LoadPetData()
    {
        if (PlayerPrefs.HasKey("PetData"))
        {
            if (bathController == null)
                bathController = FindAnyObjectByType<BathController>();
            string json = PlayerPrefs.GetString("PetData");
            PetData data = JsonUtility.FromJson<PetData>(json);

            hasReachedTeenHalf = PlayerPrefs.GetInt("hasReachedTeenHalf", 0) == 1;
            hasReachedAdultHalf = PlayerPrefs.GetInt("hasReachedAdultHalf", 0) == 1;
            hasReachedOldHalf = PlayerPrefs.GetInt("hasReachedOldHalf", 0) == 1;
            firstTimePlay = data.firstTime;

            if (firstTimePlay == false)
            {
                //Money
                moneyValue = data.moneyValue;
                ownedItems = new Dictionary<string, int>();
                foreach (var entry in data.ownedItems)
                {
                    ownedItems[entry.foodName] = entry.quantity;
                }

                // Calculate time difference
                System.DateTime lastTime = System.DateTime.Parse(data.lastSavedTime);
                System.TimeSpan timeDiff = System.DateTime.Now - lastTime;
                double secondsPassed = timeDiff.TotalSeconds;

                // ENERGY
                int energyLost = Mathf.FloorToInt((float)((secondsPassed + data.lastEnergySecond) / energy_deduct_time * energy_max * 0.01f));
                float fullEnergyCycles = Mathf.Floor((float)((secondsPassed + data.lastEnergySecond) / energy_deduct_time));
                lastEnergyTime = (float)((secondsPassed + data.lastEnergySecond) - fullEnergyCycles * energy_deduct_time);
                energy_current = Mathf.Max(0, data.energy - energyLost);

                // HUNGER
                int hungerLost = Mathf.FloorToInt((float)((secondsPassed + data.lastHungerSecond) / hunger_deduct_time * hunger_max * 0.01f));
                float fullHungerCycles = Mathf.Floor((float)((secondsPassed + data.lastHungerSecond) / hunger_deduct_time));
                lastHungerTime = (float)((secondsPassed + data.lastHungerSecond) - fullHungerCycles * hunger_deduct_time);
                hunger_current = Mathf.Max(0, data.hunger - hungerLost);

                // HAPPINESS
                int happinessLost = Mathf.FloorToInt((float)((secondsPassed + data.lastHappinessSecond) / happiness_deduct_time * happiness_max * 0.01f));
                float fullHappinessCycles = Mathf.Floor((float)((secondsPassed + data.lastHappinessSecond) / happiness_deduct_time));
                lastHappinessTime = (float)((secondsPassed + data.lastHappinessSecond) - fullHappinessCycles * happiness_deduct_time);
                happiness_current = Mathf.Max(0, data.happiness - happinessLost);

                // PROGRESS
                int progressIncrease = Mathf.FloorToInt((float)((secondsPassed + data.lastProgressSecond) / progress_increase_time * progress_max * 0.01f));
                float fullProgressCycles = Mathf.Floor((float)((secondsPassed + data.lastProgressSecond) / progress_increase_time));
                lastProgressTime = (float)((secondsPassed + data.lastProgressSecond) - fullProgressCycles * progress_increase_time);
                progress_current = Mathf.Max(0, data.progress + progressIncrease);
                currentStage = data.stage;

                // DIRTY (no leftover logic needed, always starts at 0 per session)
                int dirtyIncrease = Mathf.FloorToInt((float)(secondsPassed / 60f * 100 * 0.01f));
                if (bathController != null)
                {
                    bathController.dirty = Mathf.Max(0, data.dirty + dirtyIncrease);
                }

                // HEALTH (Only if Old stage)
                if (currentStage == PetStage.Old)
                {
                    int healthLost = Mathf.FloorToInt((float)((secondsPassed + data.lastHealthSecond) / health_deduct_time * health_max * 0.01f));
                    float fullHealthCycles = Mathf.Floor((float)((secondsPassed + data.lastHealthSecond) / health_deduct_time));
                    lastHealthTime = (float)((secondsPassed + data.lastHealthSecond) - fullHealthCycles * health_deduct_time);
                    health_current = Mathf.Max(0, data.health - healthLost);
                }
                else
                {
                    health_current = data.health;
                }

                if (progress_current > 99)
                {
                    progress_current = 99;
                }

                if (currentStage == PetStage.Kid)
                {
                    stageRepresent.text = $"{PetStageRepresent.K}\n";

                }
                else if (currentStage == PetStage.Teen)
                {

                    stageRepresent.text = $"{PetStageRepresent.T}\n";
                    if (progress_current >= 50 && !hasReachedTeenHalf)
                    {
                        progress_current = 49;
                    }

                }
                else if (currentStage == PetStage.Adult)
                {

                    stageRepresent.text = $"{PetStageRepresent.A}\n";
                    if (progress_current >= 50 && !hasReachedAdultHalf)
                    {
                        progress_current = 49;
                    }

                }
                else if (currentStage == PetStage.Old)
                {

                    stageRepresent.text = $"{PetStageRepresent.O}\n";
                    if (progress_current >= 50 && !hasReachedOldHalf)
                    {
                        progress_current = 49;
                    }

                }

            }
            else if (firstTimePlay == true)
            {
                energy_current = data.energy;
                hunger_current = data.hunger;
                happiness_current = data.happiness;
                health_current = data.health;
                progress_current = data.progress;
                currentStage = data.stage;
                if (bathController != null)
                {
                    bathController.dirty = data.dirty;
                }

            }

        }
        else
        {
            Debug.Log("No saved pet data found.");
        }
    }

    void UpdateAllUI()
    {
        GetProgressFill();
        GetHealthFill();
        GetEnergyFill();
        GetHungerFill();
        GetHappinessFill();
    }

    public void PauseEnergyDeduction()
    {
        if (energyDeductCoroutine != null)
        {
            StopCoroutine(energyDeductCoroutine);
            energyDeductCoroutine = null;
        }
    }

    public void ResumeEnergyDeduction()
    {
        if (energyDeductCoroutine == null)
        {
            energyDeductCoroutine = StartCoroutine(DeductEnergyOverTime());
        }
    }

    public void PauseHappinessDeduction()
    {
        if (happinessDeductCoroutine != null)
        {
            StopCoroutine(happinessDeductCoroutine);
            happinessDeductCoroutine = null;
        }
    }

    public void ResumeHappinessDeduction()
    {
        if (happinessDeductCoroutine == null)
        {
            happinessDeductCoroutine = StartCoroutine(DeductHappinessOverTime());
        }
    }


    private void HandleHalfProgressEvent()
    {
        progressStop = false;
        Debug.Log("Progress reached 50%, switching scene.");
        StopAllCoroutines();
        // Optional: Save before switching
        SavePetData();

        // Prevent GameObject from being destroyed so you can do work in next scene
        DontDestroyOnLoad(this.gameObject);

        // Load next scene (you can also use a loading scene or fade-out effect)

    }


    private void Update()
    {
        isBathing = PlayerPrefs.GetInt("IsBathing", 0) == 1;
        isDancing = PlayerPrefs.GetInt("IsDancing", 0) == 1;
        isEating = PlayerPrefs.GetInt("IsEating", 0) == 1;
        isAlbumOpen = PlayerPrefs.GetInt("IsAlbumOpen", 0) == 1;
        isSleeping = PlayerPrefs.GetInt("IsSleeping", 0) == 1;

        //Debug.Log("isSleeping" + isSleeping);
        //Debug.Log("isDancing" + isDancing);
        //Debug.Log("isEating" + isEating);
        //Debug.Log("isAlbumOpen" + isAlbumOpen);
        //Debug.Log("isBathing" + isBathing);
        //Debug.Log("progressStop" + progressStop);

        if (progressStop == true)
        {
            if (progress_current >= 50)
            {
                if (isBathing == false && isAlbumOpen == false && isDancing == false && isSleeping == false && isEating == false)
                {
                    changeEventScene();
                }
            }
            else if (progress_current >= progress_max)
            {
                if (isBathing == false && isAlbumOpen == false && isDancing == false && isSleeping == false && isEating == false)
                {
                    StartCoroutine(IncreaseProgressOverTime());
                    AdvanceStage();
                }
            }
        }

        if (energy_current == 0)
        {
            PlayerPrefs.SetInt("CauseOfDeath", 2); // 5 = Energy death (you can define it)
            PlayerPrefs.SetString("CurrentStage", currentStage.ToString()); // assume you have this variable
            PlayerPrefs.Save();

            FindFirstObjectByType<BGMScript>().StopMusic();
            // Load the 1LastWord scene
            petDead = true;
            hasReachedTeenHalf = false;
            hasReachedAdultHalf = false;
            hasReachedTeenHalf = false;
            PlayerPrefs.SetInt("hasReachedTeenHalf", hasReachedTeenHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("hasReachedAdultHalf", hasReachedAdultHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("hasReachedOldHalf", hasReachedTeenHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("PetDead", petDead ? 1 : 0);
            PlayerPrefs.Save();
            SceneManager.LoadScene("1LastWord");
        }

        if (hunger_current == 0)
        {
            PlayerPrefs.SetInt("CauseOfDeath", 0); // 5 = Energy death (you can define it)
            PlayerPrefs.SetString("CurrentStage", currentStage.ToString()); // assume you have this variable
            PlayerPrefs.Save();

            FindFirstObjectByType<BGMScript>().StopMusic();
            // Load the 1LastWord scene
            petDead = true;
            hasReachedTeenHalf = false;
            hasReachedAdultHalf = false;
            hasReachedTeenHalf = false;
            PlayerPrefs.SetInt("hasReachedTeenHalf", hasReachedTeenHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("hasReachedAdultHalf", hasReachedAdultHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("hasReachedOldHalf", hasReachedTeenHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("PetDead", petDead ? 1 : 0);
            PlayerPrefs.Save();
            SceneManager.LoadScene("1LastWord");
        }

        if (happiness_current == 0)
        {
            PlayerPrefs.SetInt("CauseOfDeath", 1); // 5 = Energy death (you can define it)
            PlayerPrefs.SetString("CurrentStage", currentStage.ToString()); // assume you have this variable
            PlayerPrefs.Save();

            FindFirstObjectByType<BGMScript>().StopMusic();
            // Load the 1LastWord scene
            petDead = true;
            hasReachedTeenHalf = false;
            hasReachedAdultHalf = false;
            hasReachedTeenHalf = false;
            PlayerPrefs.SetInt("hasReachedTeenHalf", hasReachedTeenHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("hasReachedAdultHalf", hasReachedAdultHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("hasReachedOldHalf", hasReachedTeenHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("PetDead", petDead ? 1 : 0);
            PlayerPrefs.Save();
            SceneManager.LoadScene("1LastWord");
        }

        if (health_current == 0)
        {
            PlayerPrefs.SetInt("CauseOfDeath", 3); // 5 = Energy death (you can define it)
            PlayerPrefs.SetString("CurrentStage", currentStage.ToString()); // assume you have this variable
            PlayerPrefs.Save();

            FindFirstObjectByType<BGMScript>().StopMusic();
            // Load the 1LastWord scene
            petDead = true;
            hasReachedTeenHalf = false;
            hasReachedAdultHalf = false;
            hasReachedTeenHalf = false;
            PlayerPrefs.SetInt("hasReachedTeenHalf", hasReachedTeenHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("hasReachedAdultHalf", hasReachedAdultHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("hasReachedOldHalf", hasReachedTeenHalf ? 1 : 0);
            PlayerPrefs.Save();
            PlayerPrefs.SetInt("PetDead", petDead ? 1 : 0);
            PlayerPrefs.Save();
            SceneManager.LoadScene("1LastWord");
        }
    }

    public int RetrieveValue(string key)
    {
        return key switch
        {
            "energy" => energy_current,
            "hunger" => hunger_current,
            "happiness" => happiness_current,
            "health" => health_current,
            "progress" => progress_current,
            "moneyValue" => moneyValue,
            _ => -1
        };
    }

    public void AddMoney(int amount)
    {
        moneyValue += amount;
        if (moneyValue < 0)
            moneyValue = 0;

        // update UI text if assigned
        if (moneyText != null)
        {
            moneyText.text = moneyValue.ToString();
        }

        SavePetData();
    }

    public void updateFoodStatus(string currentFoodName)
    {
        Debug.Log(currentFoodName);

        if (ownedItems.ContainsKey(currentFoodName))
        {
            // decrease quantity
            ownedItems[currentFoodName]--;

            // remove if zero
            if (ownedItems[currentFoodName] <= 0)
            {
                ownedItems.Remove(currentFoodName);
                // optional: clear UI if last item
                if (UIController.instance != null)
                {
                    UIController.instance.gameUI.clearSelectedFood();
                }
            }
        }

        // apply food effect (increase hunger)
        IncreaseFood();

        // save pet data
        SavePetData();  // internally converts dictionary to List<FoodEntry> before saving
        Debug.Log("Products updated & saved.");
    }

    public void updateMedicineStatus()
    {

    }

    public void updateProductandMoney(List<CartItem> purchasedItems, int totalCost)
    {
        // 1. Deduct money
        moneyValue -= totalCost;

        // 2. Add items to inventory
        foreach (CartItem item in purchasedItems)
        {
            if (ownedItems.ContainsKey(item.itemName))
                ownedItems[item.itemName] += item.quantity;
            else
                ownedItems.Add(item.itemName, item.quantity);
        }

        // 3. Save updated data
        SavePetData();

        Debug.Log("Products and money updated & saved.");
    }

    public void updateLikeMusicStatus()
    {

    }

    public void updateUnlikeMusicStatus()
    {

    }

    public void resetStatusRate()
    {

    }

    public void updateSleepStatus()
    {

    }

    public void updateBathStatus()
    {

    }

    public void updateEventStatus()
    {

    }

    public void onConnectionReload()
    {



    }

    public void connectionLost()
    {

    }

}