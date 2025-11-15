//using System;
//using System.Collections.Generic;
//using Unity.VisualScripting.Antlr3.Runtime;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using static Energy_Bar;

////[Serializable] <--- Duplicate If need put look like need delete the file from other side , Status bar or Digital album 
//public class PetData
//{
//    public float dirty;
//    public int energy;
//    public int hunger;
//    public int happiness;
//    public int health;
//    public int progress;
//    public string stage;
//    public string lastSavedTime;
//    public bool firstTime;
//    public float lastEnergySecond;
//    public float lastHungerSecond;
//    public float lastHappinessSecond;
//    public float lastHealthSecond;
//    public float lastProgressSecond;
//    public string petName;
//}
//public class PetStatus : MonoBehaviour
//{
//    [Header("Pet Attributes")]
//    public float dirty;
//    public int energy;
//    public int hunger;
//    public int happiness;
//    public int health;
//    public int progress;
//    public string stage;
//    public string petName;

//    [Header("Internal")]
//    public string lastSavedTime;
//    public bool firstTime = false;

//    [Header("Rates (seconds per deduction/increase)")]
//    public float energyDeductTime = 60f;
//    public float hungerDeductTime = 60f;
//    public float happinessDeductTime = 60f;
//    public float healthDeductTime = 60f;
//    public float progressIncreaseTime = 60f;

//    private float lastEnergyTime;
//    private float lastHungerTime;
//    private float lastHappinessTime;
//    private float lastHealthTime;
//    private float lastProgressTime;

//    private Coroutine energyRoutine;
//    private Coroutine hungerRoutine;
//    private Coroutine happinessRoutine;
//    private Coroutine progressRoutine;
//    private Coroutine healthRoutine;


//    //public Dictionary<> eventCompleted;
//    //public Dictionary<> musicPreferences;
//    //public List<FoodItem> foodInventory;

//    void Start()
//    {
//        LoadPetStatus();

//        if (!firstTime)
//        {
//            // Resume natural decay
//            energyRoutine = StartCoroutine(DeductEnergyOverTime());
//            hungerRoutine = StartCoroutine(DeductHungerOverTime());
//            happinessRoutine = StartCoroutine(DeductHappinessOverTime());
//            progressRoutine = StartCoroutine(IncreaseProgressOverTime());

//            if (stage == "Old")
//                healthRoutine = StartCoroutine(DeductHealthOverTime());
//        }
//    }



//    private System.Collections.IEnumerator DeductEnergyOverTime()
//    {
//        while (energy > 0)
//        {
//            yield return new WaitForSeconds(energyDeductTime);
//            energy = Mathf.Max(0, energy - 1);
//        }
//    }

//    private System.Collections.IEnumerator DeductHungerOverTime()
//    {
//        while (hunger > 0)
//        {
//            yield return new WaitForSeconds(hungerDeductTime);
//            hunger = Mathf.Max(0, hunger - 1);
//        }
//    }

//    private System.Collections.IEnumerator DeductHappinessOverTime()
//    {
//        while (happiness > 0)
//        {
//            yield return new WaitForSeconds(happinessDeductTime);
//            happiness = Mathf.Max(0, happiness - 1);
//        }
//    }

//    private System.Collections.IEnumerator DeductHealthOverTime()
//    {
//        while (health > 0)
//        {
//            yield return new WaitForSeconds(healthDeductTime);
//            health = Mathf.Max(0, health - 1);
//        }
//    }

//    private System.Collections.IEnumerator IncreaseProgressOverTime()
//    {
//        while (progress < 100)
//        {
//            yield return new WaitForSeconds(progressIncreaseTime);
//            progress = Mathf.Min(100, progress + 1);
//        }
//    }
//    // Update is called once per frame



//    public void createPetStatus(string name)
//    {
//        PetData data = new PetData
//        {
//            petName = name,
//            dirty = 0,
//            energy = 100,
//            hunger = 100,
//            happiness = 100,
//            health = 100,
//            progress = 0,
//            stage = "Kid",
//            lastSavedTime = DateTime.Now.ToString(),
//            firstTime = true,
//            lastEnergySecond = 0,
//            lastHappinessSecond = 0,
//            lastHealthSecond = 0,
//            lastProgressSecond = 0,
//            lastHungerSecond = 0
//        };

//        string json = JsonUtility.ToJson(data);
//        PlayerPrefs.SetString("PetData", json);
//        PlayerPrefs.Save();
//        Debug.Log("Created new pet profile.");
//    }
//    void Update()
//    {

//    }

//    public void updatePetStatus()
//    {
//        PetData data = new PetData
//        {
//            petName = petName,
//            dirty = dirty,
//            energy = energy,
//            hunger = hunger,
//            happiness = happiness,
//            health = health,
//            progress = progress,
//            stage = stage,
//            lastSavedTime = DateTime.Now.ToString(),
//            firstTime = false,
//            lastEnergySecond = lastEnergyTime,
//            lastHappinessSecond = lastHappinessTime,
//            lastHealthSecond = lastHealthTime,
//            lastProgressSecond = lastProgressTime,
//            lastHungerSecond = lastHungerTime
//        };

//        string json = JsonUtility.ToJson(data);
//        PlayerPrefs.SetString("PetData", json);
//        PlayerPrefs.Save();
//        Debug.Log("Pet status updated and saved.");
//    }

//    public void LoadPetStatus()
//    {
//        if (PlayerPrefs.HasKey("PetData"))
//        {
//            string json = PlayerPrefs.GetString("PetData");
//            PetData data = JsonUtility.FromJson<PetData>(json);

//            dirty = data.dirty;
//            energy = data.energy;
//            hunger = data.hunger;
//            happiness = data.happiness;
//            health = data.health;
//            progress = data.progress;
//            stage = data.stage;
//            lastSavedTime = data.lastSavedTime;
//            firstTime = data.firstTime;
//            petName = data.petName;

//            // handle time difference for passive decay
//            DateTime lastTime = DateTime.Parse(data.lastSavedTime);
//            TimeSpan diff = DateTime.Now - lastTime;
//            double secondsPassed = diff.TotalSeconds;

//            ApplyOfflineDecay(secondsPassed, data);
//        }
//        else
//        {
//            Debug.LogWarning("No saved PetData found.");
//            firstTime = true;
//        }
//    }

//    private void ApplyOfflineDecay(double secondsPassed, PetData data)
//    {
//        int energyLost = Mathf.FloorToInt((float)((secondsPassed + data.lastEnergySecond) / energyDeductTime));
//        energy = Mathf.Max(0, data.energy - energyLost);

//        int hungerLost = Mathf.FloorToInt((float)((secondsPassed + data.lastHungerSecond) / hungerDeductTime));
//        hunger = Mathf.Max(0, data.hunger - hungerLost);

//        int happinessLost = Mathf.FloorToInt((float)((secondsPassed + data.lastHappinessSecond) / happinessDeductTime));
//        happiness = Mathf.Max(0, data.happiness - happinessLost);

//        int progressGain = Mathf.FloorToInt((float)((secondsPassed + data.lastProgressSecond) / progressIncreaseTime));
//        progress = Mathf.Min(100, data.progress + progressGain);

//        if (stage == "Old")
//        {
//            int healthLost = Mathf.FloorToInt((float)((secondsPassed + data.lastHealthSecond) / healthDeductTime));
//            health = Mathf.Max(0, data.health - healthLost);
//        }
//        else
//        {
//            health = data.health;
//        }
//    }



//    public int RetrieveValue(string key)
//    {
//        return key switch
//        {
//            "energy" => energy,
//            "hunger" => hunger,
//            "happiness" => happiness,
//            "health" => health,
//            "progress" => progress,
//            _ => -1
//        };
//    }

//    public void updateFoodStatus()
//    {

//    }

//    public void updateMedicineStatus()
//    {

//    }

//    public void updateProductandMoney()
//    {

//    }

//    public void updateLikeMusicStatus()
//    {

//    }

//    public void updateUnlikeMusicStatus()
//    {

//    }

//    public void resetStatusRate()
//    {

//    }

//    public void updateSleepStatus()
//    {

//    }

//    public void updateBathStatus()
//    {

//    }

//    public void updateEventStatus()
//    {

//    }

//    void OnApplicationQuit()
//    {
//        updatePetStatus();
//    }

//    public void OnApplicationPause(bool paused) //Class Diagram is OnApplicationStop()
//    {
//        if (paused)
//            updatePetStatus();
//    }

//    public void onConnectionReload()
//    {



//    }

//    public void connectionLost()
//    {

//    }

//}
