using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using static PetStatus;

public class SceneLoader : MonoBehaviour
{
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
    }
    public enum PetStage
    {
        Kid,
        Teen,
        Adult,
        Old
    }
    public static SceneLoader Instance;
    public AudioSource audioClip; // click sound
    public PetStatus energy;
    public bool hall = false;
    public bool kitchen = false;
    public bool bathroom = false;
    public bool gameroom = false;
    public PetStage currentStage;


    public bool isEating;
    public bool isSleeping;
    public bool isDancing;
    public bool isBathing;
    public bool isAlbumOpen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioClip = GetComponent<AudioSource>();
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject); // destroy old one
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioClip = GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Reconnect Energy_Bar after scene changes
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //energy = FindObjectOfType<Energy_Bar>();
    }

    // Public function to use in button OnClick
    public void PlayAndLoad(string sceneName)
    {
        if (audioClip != null)
        {
            StartCoroutine(PlaySoundAndLoad(sceneName));
        }
        else
        {
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
            SceneManager.LoadScene(sceneName); // fallback
        }
    }

    private System.Collections.IEnumerator PlaySoundAndLoad(string sceneName)
    {
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
        audioClip.Play();
        yield return new WaitForSeconds(0.4f); // wait short delay (or audioClip.clip.length)
        SceneManager.LoadScene(sceneName);
    }

    // Optional: Scene-specific wrappers for buttons
    public void LoadPetNameScene()
    {
        PlayAndLoad("PetNameScene");
    }

    public void BackToHallScene()
    {
            
        FindFirstObjectByType<PetStatus>()?.SavePetData();

        hall = true;
        kitchen = false;
        gameroom = false;
        bathroom = false;

        SaveCurrentRoom();

        if (energy.currentStage == PetStatus.PetStage.Kid)
        {
            PlayAndLoad("HallScene");
        }
        else if (energy.currentStage == PetStatus.PetStage.Teen)
        {
            PlayAndLoad("TeenHallScene");
        }
        else if (energy.currentStage == PetStatus.PetStage.Adult)
        {
            PlayAndLoad("AdultHallScene");
        }
        else if (energy.currentStage == PetStatus.PetStage.Old)
        {
            PlayAndLoad("OldHallScene");
        }
    }

    public void BackToKitchenScene()
    {
        FindFirstObjectByType<PetStatus>()?.SavePetData();

        hall = false;
        kitchen = true;
        gameroom = false;
        bathroom = false;

        SaveCurrentRoom();

        if (energy.currentStage == PetStatus.PetStage.Kid)
        {
            PlayAndLoad("KitchenScene");
        }
        else if (energy.currentStage == PetStatus.PetStage.Teen)
        {
            PlayAndLoad("TeenKitchenScene");
        }
        else if (energy.currentStage == PetStatus.PetStage.Adult)
        {
            PlayAndLoad("AdultKitchenScene");
        }
        else if (energy.currentStage == PetStatus.PetStage.Old)
        {
            PlayAndLoad("OldKitchenScene");
        }
    }

    public void BackToMedicationScene()
    {
        FindFirstObjectByType<PetStatus>()?.SavePetData();
        PlayAndLoad("MedicationScene");
    }

    public void BackToBathRoomScene()
    {
        FindFirstObjectByType<PetStatus>()?.SavePetData();

        hall = false;
        kitchen = false;
        gameroom = false;
        bathroom = true;

        SaveCurrentRoom();

        if (energy.currentStage == PetStatus.PetStage.Kid)
        {
            PlayAndLoad("BathRoomScene");
        }
        else if (energy.currentStage == PetStatus.PetStage.Teen)
        {
            PlayAndLoad("TeenBathRoomScene");
        }
        else if (energy.currentStage == PetStatus.PetStage.Adult)
        {
            PlayAndLoad("AdultBathRoomScene");
        }
        else if (energy.currentStage == PetStatus.PetStage.Old)
        {
            PlayAndLoad("OldBathRoomScene");
        }
    }

    public void BackToGameRoomScene()
    {
        FindFirstObjectByType<PetStatus>()?.SavePetData();

        hall = false;
        kitchen = false;
        gameroom = true;
        bathroom = false;

        SaveCurrentRoom();

        if (energy.currentStage == PetStatus.PetStage.Kid)
        {
            PlayAndLoad("GameRoomScene");
        }
        else if (energy.currentStage == PetStatus.PetStage.Teen)
        {
            PlayAndLoad("TeenGameRoomScene");
        }
        else if (energy.currentStage == PetStatus.PetStage.Adult)
        {
            PlayAndLoad("AdultGameRoomScene");
        }
        else if (energy.currentStage == PetStatus.PetStage.Old)
        {
            PlayAndLoad("OldGameRoomScene");
        }
    }

    public void LoadPlayBallScene()
    {
        PlayAndLoad("PlayBallScene");
    }

    public void playAudio()
    {
        if (audioClip != null)
        {
            audioClip.Play();
        }
    }

    public void eventBackScene()
    {
        LoadCurrentStage();
        LoadCurrentRoom();
        Debug.Log(hall);
        Debug.Log(kitchen);
        if (hall == true)
        {
            stageToHallScene();
        }
        else if (kitchen == true)
        {
            stageToKitchenScene();
        }
        else if (gameroom == true)
        {
            stageToGameRoomScene();
        }
        else if (bathroom == true)
        {
            stageToBathRoomScene();
        }
    }

    public void gameBackScene()
    {
        LoadCurrentStage();
        stageToGameRoomScene();

    }

    private void SaveCurrentRoom()
    {
        PlayerPrefs.SetInt("hall", hall ? 1 : 0);
        PlayerPrefs.SetInt("kitchen", kitchen ? 1 : 0);
        PlayerPrefs.SetInt("gameroom", gameroom ? 1 : 0);
        PlayerPrefs.SetInt("bathroom", bathroom ? 1 : 0);
        PlayerPrefs.Save(); // Make sure it's written
    }

    private void LoadCurrentRoom()
    {
        hall = PlayerPrefs.GetInt("hall", 0) == 1;
        kitchen = PlayerPrefs.GetInt("kitchen", 0) == 1;
        gameroom = PlayerPrefs.GetInt("gameroom", 0) == 1;
        bathroom = PlayerPrefs.GetInt("bathroom", 0) == 1;
    }

    public void LoadCurrentStage()
    {
        string json = PlayerPrefs.GetString("PetData");
        PetData data = JsonUtility.FromJson<PetData>(json);

        currentStage = data.stage;

        Debug.Log(currentStage);
    }

    public void stageToHallScene()
    {

        FindFirstObjectByType<PetStatus>()?.SavePetData();

        hall = true;
        kitchen = false;
        gameroom = false;
        bathroom = false;

        SaveCurrentRoom();

        if (currentStage == PetStage.Kid)
        {
            PlayAndLoad("HallScene");
        }
        else if (currentStage == PetStage.Teen)
        {
            PlayAndLoad("TeenHallScene");
        }
        else if (currentStage == PetStage.Adult)
        {
            PlayAndLoad("AdultHallScene");
        }
        else if (currentStage == PetStage.Old)
        {
            PlayAndLoad("OldHallScene");
        }
    }

    public void stageToKitchenScene()
    {
        FindFirstObjectByType<PetStatus>()?.SavePetData();

        hall = false;
        kitchen = true;
        gameroom = false;
        bathroom = false;

        SaveCurrentRoom();

        if (currentStage == PetStage.Kid)
        {
            PlayAndLoad("KitchenScene");
        }
        else if (currentStage == PetStage.Teen)
        {
            PlayAndLoad("TeenKitchenScene");
        }
        else if (currentStage == PetStage.Adult)
        {
            PlayAndLoad("AdultKitchenScene");
        }
        else if (currentStage == PetStage.Old)
        {
            PlayAndLoad("OldKitchenScene");
        }
    }

    public void stageToBathRoomScene()
    {
        FindFirstObjectByType<PetStatus>()?.SavePetData();

        hall = false;
        kitchen = false;
        gameroom = false;
        bathroom = true;

        SaveCurrentRoom();

        if (currentStage == PetStage.Kid)
        {
            PlayAndLoad("BathRoomScene");
        }
        else if (currentStage == PetStage.Teen)
        {
            PlayAndLoad("TeenBathRoomScene");
        }
        else if (currentStage == PetStage.Adult)
        {
            PlayAndLoad("AdultBathRoomScene");
        }
        else if (currentStage == PetStage.Old)
        {
            PlayAndLoad("OldBathRoomScene");
        }
    }

    public void stageToGameRoomScene()
    {
        FindFirstObjectByType<PetStatus>()?.SavePetData();

        hall = false;
        kitchen = false;
        gameroom = true;
        bathroom = false;

        SaveCurrentRoom();

        if (currentStage == PetStage.Kid)
        {
            PlayAndLoad("KidScene");
        }
        else if (currentStage == PetStage.Teen)
        {
            PlayAndLoad("TeenGameRoomScene");
        }
        else if (currentStage == PetStage.Adult)
        {
            PlayAndLoad("AdultGameRoomScene");
        }
        else if (currentStage == PetStage.Old)
        {
            PlayAndLoad("OldGameRoomScene");
        }
    }
}