using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

public class LoadingScene : MonoBehaviour
{
    public class PetData
    {
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
    }
    public enum PetStage
    {
        Kid,
        Teen,
        Adult,
        Old
    }
    public PetStage currentStage;

    public bool petDead;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string json = PlayerPrefs.GetString("PetData");
        PetData data = JsonUtility.FromJson<PetData>(json);

        currentStage = data.stage;

        Debug.Log(currentStage);
        petDead = PlayerPrefs.GetInt("PetDead", 0) == 1;

        if (petDead == true)
        {
            SceneManager.LoadScene("StartScene");
        }
        else if(petDead == false)
        {
            if(currentStage == PetStage.Kid)
            {
                SceneManager.LoadScene("HallScene");
            }
            else if (currentStage == PetStage.Teen)
            {
                SceneManager.LoadScene("TeenHallScene");
            }
            else if (currentStage == PetStage.Adult)
            {
                SceneManager.LoadScene("AdultHallScene");
            }
            else if (currentStage == PetStage.Old)
            {
                SceneManager.LoadScene("OldHallScene");
            }
        }
    }


}
