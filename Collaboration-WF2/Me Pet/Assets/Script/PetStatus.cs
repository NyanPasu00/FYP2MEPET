using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class PetStatus : MonoBehaviour
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
    public class DeadPetRecord
    {
        public string id;
        public string name;
        public string stagePassed;
    }

    [System.Serializable]
    public class PetAlbumRecord
    {
        public string petId;
        public string petName;
        public string kidImageName;
        public string teenImageName;
        public string adultImageName;
        public string oldImageName;
        public int gratitudeIndex;
    }

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
        public bool hasReachedTeenHalf;
        public bool hasReachedAdultHalf;
        public bool hasReachedOldHalf;
        public float lastEnergySecond;
        public float lastHealthSecond;
        public float lastProgressSecond;
        public float lastHappinessSecond;
        public float lastHungerSecond;
        public int moneyValue;
        public List<FoodEntry> ownedItems = new List<FoodEntry>();
        public List<DeadPetRecord> deadPets = new List<DeadPetRecord>();
        public List<PetAlbumRecord> albumDataList = new List<PetAlbumRecord>();
    }


    public string petName;
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
    private Coroutine sleepEnergyCoroutine;
    private Coroutine sleepHungerCoroutine;
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

    public GameUI GameUI;

    private Coroutine musicHappinessCoroutine;
    private Coroutine dirtyHappinessCoroutine;


    void Start()
    {
        if (GameUI == null)
        {
            GameUI = FindFirstObjectByType<GameUI>();
        }

        if (GameUI == null)
        {
            Debug.LogWarning("PetStatus: GameUI reference is missing. backgroundTransition() will not run.");
        }

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

    public void IncreaseFood(int num)
    {
        hunger_current = hunger_current + num;
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
        Debug.Log(happiness_current);
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
        if(energy_Slider != null)
        {
            energy_Slider.value = (float)energy_current / energy_max;
            energyDetail_Slider.value = (float)energy_current / energy_max;

        }
    }
    public void GetHungerFill()
    {
        if(hunger_Slider != null)
        {
            hunger_Slider.value = (float)hunger_current / hunger_max;
            hungerDetail_Slider.value = (float)hunger_current / hunger_max;

        }
    }
    public void GetHappinessFill()
    {
        if (happiness_Slider != null)
        {
            happiness_Slider.value = (float)happiness_current / happiness_max;
            happinessDetail_Slider.value = (float)happiness_current / happiness_max;
        }
    }
    public void GetHealthFill()
    {
        if (health_Slider != null)
        {
            health_Slider.value = (float)health_current / health_max;
            healthDetail_Slider.value = (float)health_current / health_max;
        }
    }

    public void GetProgressFill()
    {
        if(progress_Image != null)
        {
        progress_Image.fillAmount = (float)progress_current / progress_max;
        progressDetail_Slider.value = (float)progress_current / progress_max;

        }
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
            GameUI.PlayAndLoad("TeenStressEvent");
        }
        else if (currentStage == PetStage.Adult && !hasReachedAdultHalf)
        {
            progress_increase_time = 60f;
            hasReachedAdultHalf = true;
            PlayerPrefs.SetInt("hasReachedAdultHalf", hasReachedAdultHalf ? 1 : 0);
            PlayerPrefs.Save();
            HandleHalfProgressEvent();
            GameUI.PlayAndLoad("AdultStressEvent");
        }
        else if (currentStage == PetStage.Old && !hasReachedOldHalf)
        {
            progress_increase_time = 60f;
            hasReachedOldHalf = true;
            PlayerPrefs.SetInt("hasReachedOldHalf", hasReachedOldHalf ? 1 : 0);
            PlayerPrefs.Save();
            HandleHalfProgressEvent();
            GameUI.PlayAndLoad("FindFriendSceneTest");
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
            GameUI.PlayAndLoad("KidToTeen");


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
            GameUI.PlayAndLoad("TeenToAdult");
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
            GameUI.PlayAndLoad("AdultToOld");
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

    public void eventBackScene()
    {
        if (currentStage == PetStage.Teen)
        {
            //FindFirstObjectByType<PetStatus>()?.SavePetData();
            GameUI.PlayAndLoad("TeenScene");
        }
        else if (currentStage == PetStage.Adult)
        {
            FindFirstObjectByType<PetStatus>()?.SavePetData();
            GameUI.PlayAndLoad("AdultScene");
        }
        else if (currentStage == PetStage.Old)
        {
            FindFirstObjectByType<PetStatus>()?.SavePetData();
            GameUI.PlayAndLoad("OldScene");
        }

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


    public async void createPetData()
    {
        string petNewName = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(petNewName))
        {
            Debug.LogWarning("Pet name is empty!");
            return;
        }

        // ============================
        // 1. Load existing cloud data
        // ============================
        PetData data = new PetData();

        string cloudJson = await DataManager.LoadPetDataFromCloud();
        if (!string.IsNullOrEmpty(cloudJson))
        {
            try
            {
                data = JsonUtility.FromJson<PetData>(cloudJson);
            }
            catch
            {
                data = new PetData();
            }
        }

        // If no lists exist, create them
        if (data.deadPets == null)
            data.deadPets = new List<DeadPetRecord>();

        if (data.albumDataList == null)
            data.albumDataList = new List<PetAlbumRecord>();

        // ==========================================================
        // 2. Now overwrite ONLY the alive-pet stats (fresh new pet)
        // ==========================================================
        data.petName = petNewName;
        data.dirty = 0;
        data.energy = 100;
        data.hunger = 100;
        data.happiness = 100;
        data.health = 100;
        data.progress = 0;
        data.stage = PetStage.Kid;
        data.lastSavedTime = System.DateTime.Now.ToString();
        data.firstTime = false;   // NEW pet created → not first time
        data.hasReachedTeenHalf = false;
        data.hasReachedAdultHalf = false;
        data.hasReachedOldHalf = false;
        data.lastEnergySecond = 0;
        data.lastHappinessSecond = 0;
        data.lastHealthSecond = 0;
        data.lastProgressSecond = 0;
        data.lastHungerSecond = 0;
        data.moneyValue = 150;

        // =====================================
        // 3. Save to PlayerPrefs + Cloud
        // =====================================
        string json = JsonUtility.ToJson(data);
        DataManager.savePetDataToLocal(json);

        await DataManager.SavePetDataToCloud(json);

        // Pet is alive again
        petDead = false;
        PlayerPrefs.SetInt("PetDead", 0);
        PlayerPrefs.Save();

        Debug.Log("New pet created, old album preserved.");

        GameUI.PlayAndLoad("KidScene");
    }


    public async void SavePetData()
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

        // Save album + dead pets
        DigitalAlbumManager albumManager = FindFirstObjectByType<DigitalAlbumManager>();
        if (albumManager != null)
        {
            // Save Dead Pets
            data.deadPets = new List<DeadPetRecord>();
            foreach (var p in albumManager.LoadDeadPets())
            {
                data.deadPets.Add(new DeadPetRecord
                {
                    id = p.id,
                    name = p.name,
                    stagePassed = p.stagePassed
                });
            }

            // Save Album Data
            data.albumDataList = new List<PetAlbumRecord>();

            foreach (var entry in albumManager.petAlbumDataDict.Values)
            {
                data.albumDataList.Add(new PetAlbumRecord
                {
                    petId = entry.petId,
                    petName = entry.petName,
                    kidImageName = entry.kidImageName,
                    teenImageName = entry.teenImageName,
                    adultImageName = entry.adultImageName,
                    oldImageName = entry.oldImageName,
                    gratitudeIndex = entry.gratitudeIndex
                });
            }
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
        data.hasReachedTeenHalf = hasReachedTeenHalf;
        data.hasReachedAdultHalf = hasReachedAdultHalf;
        data.hasReachedOldHalf = hasReachedOldHalf;
        data.lastEnergySecond = lastEnergyTime;
        data.lastHappinessSecond = lastHappinessTime;
        data.lastHealthSecond = lastHealthTime;
        data.lastProgressSecond = lastProgressTime;
        data.lastHungerSecond = lastHungerTime;

        string json = JsonUtility.ToJson(data);
        DataManager.savePetDataToLocal(json);
        await DataManager.SavePetDataToCloud(json);
        Debug.Log("Save folder: " + Application.persistentDataPath);
        Debug.Log("dirty : " + data.dirty);
    }

    async void LoadPetData()
    {
        if (PlayerPrefs.HasKey("PetData"))
        {
            if (bathController == null)
                bathController = FindAnyObjectByType<BathController>();
            string json = PlayerPrefs.GetString("PetData");
            PetData data = JsonUtility.FromJson<PetData>(json);

            petName = data.petName;

            hasReachedTeenHalf = data.hasReachedTeenHalf;
            hasReachedAdultHalf = data.hasReachedAdultHalf;
            hasReachedOldHalf = data.hasReachedOldHalf;
            firstTimePlay = data.firstTime;

            if (firstTimePlay == false)
            {
                DigitalAlbumManager albumManager = FindFirstObjectByType<DigitalAlbumManager>();
                if (albumManager != null)
                {
                    // ---------------------------
                    // RESTORE DEAD PET LIST
                    // ---------------------------
                    albumManager.allPetList = new List<PetDataID>();
                    foreach (var dead in data.deadPets)
                    {
                        albumManager.allPetList.Add(new PetDataID
                        {
                            id = dead.id,
                            name = dead.name,
                            stagePassed = dead.stagePassed
                        });
                    }

                    // ---------------------------
                    // RESTORE ALBUM DATA
                    // ---------------------------
                    albumManager.petAlbumDataDict = new Dictionary<string, PetAlbumData>();
                    foreach (var album in data.albumDataList)
                    {
                        albumManager.petAlbumDataDict[album.petId] = new PetAlbumData(album.petId)
                        {
                            petId = album.petId,
                            petName = album.petName,
                            kidImageName = album.kidImageName,
                            teenImageName = album.teenImageName,
                            adultImageName = album.adultImageName,
                            oldImageName = album.oldImageName,
                            gratitudeIndex = album.gratitudeIndex
                        };
                    }
                }

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

                    bathController.RefreshDirtySpots();
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
                    if(stageRepresent != null)
                    {

                        stageRepresent.text = $"{PetStageRepresent.K}\n";
                    }

                }
                else if (currentStage == PetStage.Teen)
                {
                    if (stageRepresent != null)
                    {
                        stageRepresent.text = $"{PetStageRepresent.T}\n";
                        if (progress_current >= 50 && !hasReachedTeenHalf)
                        {
                            progress_current = 49;
                        }
                    }

                }
                else if (currentStage == PetStage.Adult)
                {
                    if (stageRepresent != null)
                    {
                        stageRepresent.text = $"{PetStageRepresent.A}\n";
                        if (progress_current >= 50 && !hasReachedAdultHalf)
                        {
                            progress_current = 49;
                        }
                    }

                }
                else if (currentStage == PetStage.Old)
                {
                    if (stageRepresent != null)
                    {
                        stageRepresent.text = $"{PetStageRepresent.O}\n";
                        if (progress_current >= 50 && !hasReachedOldHalf)
                        {
                            progress_current = 49;
                        }
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

        // dancing = standing; everything else (idle/sleep/etc.) = lying
        if (bathController != null)
        {
            // dancing = standing; everything else (idle/sleep/etc.) = lying
            bathController.SetHallStanding(isDancing);
        }

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

                if(FindFirstObjectByType<BGMScript>() != null)
            {
                FindFirstObjectByType<BGMScript>().StopMusic();
            }
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
            Destroy(this.gameObject);
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
            Destroy(this.gameObject);
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
            Destroy(this.gameObject);
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
            Destroy(this.gameObject);
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
        if (currentFoodName == "milk" || currentFoodName == "water" || currentFoodName == "apple" || currentFoodName == "orange")
        {
            IncreaseFood(5);

        } else if (currentFoodName == "porridge" || currentFoodName == "fried chicken" || currentFoodName == "noodle" || currentFoodName == "rice")
        {
            IncreaseFood(15);

        } else if (currentFoodName == "fried egg" || currentFoodName == "avacado" || currentFoodName == "cola")
        {
            IncreaseFood(8);

        } else
        {
            IncreaseFood(30);
        }
        // save pet data
        SavePetData();  // internally converts dictionary to List<FoodEntry> before saving
        Debug.Log("Products updated & saved.");
    }

    public void updateMedicineStatus(string currentPotionName)
    {
        Debug.Log(currentPotionName);

        if (ownedItems.ContainsKey(currentPotionName))
        {
            // decrease quantity
            ownedItems[currentPotionName]--;

            // remove if zero
            if (ownedItems[currentPotionName] <= 0)
            {
                ownedItems.Remove(currentPotionName);
                // optional: clear UI if last item
                if (UIController.instance != null)
                {
                    UIController.instance.gameUI.clearSelectedFood();
                }
            }
        }

        // apply food effect (increase hunger)
        IncreaseHealth();

        // save pet data
        SavePetData();  // internally converts dictionary to List<FoodEntry> before saving
        Debug.Log("Products updated & saved.");
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
        // Stop any previous music regen coroutine
        if (musicHappinessCoroutine != null)
        {
            StopCoroutine(musicHappinessCoroutine);
            musicHappinessCoroutine = null;
        }

        // While pet is enjoying music, normal happiness deduction should pause
        PauseHappinessDeduction();

        // Start a gentle happiness regen while music is liked
        musicHappinessCoroutine = StartCoroutine(RegenerateHappinessFromMusic());
    }

    public void updateUnlikeMusicStatus()
    {
        // If there was a special music regen running, stop it
        if (musicHappinessCoroutine != null)
        {
            StopCoroutine(musicHappinessCoroutine);
            musicHappinessCoroutine = null;
        }

        // Small penalty when pet dislikes music
        decreaseHappiness(5);

        // Resume normal deduction behaviour
        ResumeHappinessDeduction();
    }

    public void resetStatusRate()
    {
        if (musicHappinessCoroutine != null)
        {
            StopCoroutine(musicHappinessCoroutine);
            musicHappinessCoroutine = null;
        }

        // Make sure normal happiness deduction is running again
        ResumeHappinessDeduction();
    }

    private IEnumerator RegenerateHappinessFromMusic()
    {
        while (happiness_current < happiness_max)
        {
            happiness_current += 1;
            if (happiness_current > happiness_max)
                happiness_current = happiness_max;

            GetHappinessFill();    // update sliders

            yield return new WaitForSeconds(5f);
        }

        musicHappinessCoroutine = null;
    }

    public void updateSleepStatus()
    {
        Debug.Log(isSleeping);
        // Only run this logic when the pet is actually marked sleeping
        if (!isSleeping)
            return;

        // Start energy regen while sleeping (if not already running)
        if (sleepEnergyCoroutine == null)
        {
            sleepEnergyCoroutine = StartCoroutine(SleepEnergyRoutine());
        }

        // Start extra hunger deduction while sleeping (if not already running)
        if (sleepHungerCoroutine == null)
        {
            sleepHungerCoroutine = StartCoroutine(SleepHungerRoutine());
        }
    }

    public void stopSleepStatus()
    {
        isSleeping = false;

        if (sleepEnergyCoroutine != null)
        {
            StopCoroutine(sleepEnergyCoroutine);
            sleepEnergyCoroutine = null;
        }

        if (sleepHungerCoroutine != null)
        {
            StopCoroutine(sleepHungerCoroutine);
            sleepHungerCoroutine = null;
        }

        
    }

    private IEnumerator SleepEnergyRoutine()
    {
        while (isSleeping)
        {
            Debug.Log("+1");
            // +1 energy every 5 seconds
            energy_current = Mathf.Min(energy_max, energy_current + 1);
            GetEnergyFill();

            yield return new WaitForSeconds(5f);

            // If pet died / object destroyed, break safely
            if (!this) yield break;
        }

        sleepEnergyCoroutine = null;
    }

    private IEnumerator SleepHungerRoutine()
    {
        while (isSleeping)
        {
            // Wait 30 seconds, but allow early exit if wake happens
            float elapsed = 0f;
            while (elapsed < 30f && isSleeping)
            {
                elapsed += Time.deltaTime;
                yield return null;

                if (!this) yield break;
            }

            if (!isSleeping) break;

            // -1 hunger every 30 seconds (1% of max)
            DeductHunger(1);

            // If hunger reached 0, your Update() will handle death anyway
            if (hunger_current <= 0)
            {
                sleepHungerCoroutine = null;
                yield break;
            }
        }

        sleepHungerCoroutine = null;
    }

    public void updateBathStatus()
    {
        increaseHappiness(20);   // adjust value if you want

        // Save new status so it persists
        SavePetData();
    }

    public void updateWhenDirty()
    {
        if (bathController == null)
            return;

        // If dirty >= 60 and the penalty coroutine is not running, start it
        if (bathController.dirty >= 60f)
        {
            if (dirtyHappinessCoroutine == null)
            {
                dirtyHappinessCoroutine = StartCoroutine(DirtyHappinessPenaltyRoutine());
            }
        }
        else
        {
            // Dirty below 60 → stop the penalty coroutine if it is running
            if (dirtyHappinessCoroutine != null)
            {
                StopCoroutine(dirtyHappinessCoroutine);
                dirtyHappinessCoroutine = null;
            }
        }
    }

    private IEnumerator DirtyHappinessPenaltyRoutine()
    {
        // Loop as long as pet is very dirty
        while (bathController != null && bathController.dirty >= 60f)
        {
            float elapsed = 0f;

            // Wait 25 seconds BUT allow early exit if cleaned during this period
            while (elapsed < 25f)
            {
                if (bathController == null || bathController.dirty < 60f)
                {
                    dirtyHappinessCoroutine = null;
                    yield break;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // 25 seconds passed with dirty still >= 60 → lose 1 happiness
            decreaseHappiness(1);

            // Optional safety: if happiness reaches 0, stop this penalty loop
            if (happiness_current <= 0)
            {
                dirtyHappinessCoroutine = null;
                yield break;
            }
        }

        dirtyHappinessCoroutine = null;
    }
    public void updateEventStatus()
    {

    }

    //public void onConnectionReload()
    //{



    //}

    //public void connectionLost()
    //{

    //}

}