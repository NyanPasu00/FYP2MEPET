using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DigitalAlbumManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject EmptyDigitalAlbum;
    public GameObject PetList;
    public GameObject PetAlbum;

    [Header("Containers")]
    public RectTransform petListContent;
    public RectTransform stagesContent;

    [Header("Prefabs")]
    public GameObject petItemPrefab;
    public GameObject stageItemPrefab;

    [Header("UI")]
    public TextMeshProUGUI petAlbumTitle;
    public TextMeshProUGUI gratitudeText;

    private List<PetData> allPetList;
    public Dictionary<string, PetAlbumData> petAlbumDataDict;
    private bool isAlbumOpen = false;

    private readonly string[] gratitudeMessages = new string[]
    {
        "From my first steps to my last breath, your love was everything to me.",
        "You made my short life feel full. Now, live yours with the same love you gave me.",
        "You took care of me with love. Now, take care of yourself the same way—gently, patiently, and every day.",
        "Every time you comforted me, you were practicing how to heal your own heart.",
        "You gave me peace in every moment. Now, promise to seek that peace for yourself.",
        "I grew because you cared. Now it’s your turn—grow for yourself."
    };

    void Start()
    {
        //PlayerPrefs.DeleteKey("DeadPets");
        //PlayerPrefs.DeleteKey("PetAlbumData");
        //PlayerPrefs.Save();

        allPetList = LoadDeadPets();
        petAlbumDataDict = new Dictionary<string, PetAlbumData>();
        LoadPetAlbumData();

        if (allPetList == null || allPetList.Count == 0)
        {
            Debug.Log("No dead pets found. Skipping album setup.");
            return;
        }

        // Only cache album if not already saved
        foreach (var pet in allPetList)
        {
            if (!petAlbumDataDict.ContainsKey(pet.id))
            {
                CacheRandomImagesForPet(pet.id, pet.name, pet.stagePassed);
            }
        }
    }

    public void ToggleDigitalAlbum()
    {
        isAlbumOpen = !isAlbumOpen;
        gameObject.SetActive(isAlbumOpen);
        PlayerPrefs.SetInt("IsAlbumOpen", isAlbumOpen ? 1 : 0);
        PlayerPrefs.Save();

        if (isAlbumOpen) ShowDigitalAlbum();
    }

    public void ShowDigitalAlbum()
    {
        PetList.SetActive(true);
        PetAlbum.SetActive(false);

        EmptyDigitalAlbum.SetActive(allPetList.Count == 0);
        PetList.SetActive(allPetList.Count > 0);

        PopulatePetList();
    }

    void PopulatePetList()
    {

        foreach (Transform child in petListContent)
            Destroy(child.gameObject);

        foreach (var pet in allPetList)
        {
           
            //custom UI layout showing a pet's image and name
            GameObject petGO = Instantiate(petItemPrefab, petListContent);

            //Sets the displayed text to the pet's name
            petGO.transform.Find("PetNameText").GetComponent<TextMeshProUGUI>().text = pet.name;

            //pre-chosen image names for Kid/Teen/Adult/Old stages
            PetAlbumData albumData = petAlbumDataDict[pet.id];

            //Chooses the most appropriate available stage image for preview
            string stageFolder = GetPreviewStage(albumData, out string imageName);

            //Loads the image
            Sprite sprite = Resources.Load<Sprite>($"DigitalAlbum/{stageFolder}/{imageName}");

            //If the sprite was loaded, sets it to the Image UI component named PetImage inside the prefab
            if (sprite != null)
                petGO.transform.Find("PetImage").GetComponent<Image>().sprite = sprite;

            //switches to the album panel and shows that pet's stage images

            string capturedId = pet.id;
            string capturedName = pet.name;
            petGO.GetComponent<Button>().onClick.AddListener(() => ShowPetAlbum(capturedId, capturedName));
        }
    }

    public void ShowPetAlbum(string petId,string petName)
    {

        PetList.SetActive(false);
        PetAlbum.SetActive(true);

        // Display pet name
        if (petAlbumTitle != null)
            petAlbumTitle.text = petName;

        // Checks if the selected pet already has saved (cached) stage image selections
        if (!petAlbumDataDict.ContainsKey(petId))
        {
            var pet = allPetList.Find(p => p.id == petId);
            if (pet != null)
            {
                CacheRandomImagesForPet(pet.id, pet.name, pet.stagePassed);
            }else
            {
                Debug.LogError($"Pet not found in list: {petId}");
                return;
            }
        }

        // Remove any previously displayed images from stagesContent
        foreach (Transform child in stagesContent)
            Destroy(child.gameObject);

        // Retrieve the pet’s saved image names
        PetAlbumData data = petAlbumDataDict[petId];

        AddStageImageIfExists("KidStage", data.kidImageName);
        AddStageImageIfExists("TeenStage", data.teenImageName);
        AddStageImageIfExists("AdultStage", data.adultImageName);
        AddStageImageIfExists("OldStage", data.oldImageName);

        if (gratitudeText != null)
        {
            int index = data.gratitudeIndex;
            if (index >= 0 && index < gratitudeMessages.Length)
                gratitudeText.text = gratitudeMessages[index];
            else
                gratitudeText.text = "";
        }
    }

    void AddStageImageIfExists(string folder, string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
        {
            Debug.LogWarning($"Skipped loading empty image from {folder}");
            return;
        }

        Sprite sprite = Resources.Load<Sprite>($"DigitalAlbum/{folder}/{imageName}");
        if (sprite == null)
        {
            Debug.LogError($"Sprite not found: DigitalAlbum/{folder}/{imageName}");
            return;
        }

        Debug.Log($"Loaded: DigitalAlbum/{folder}/{imageName}");

        GameObject item = Instantiate(stageItemPrefab, stagesContent);
        Image img = item.GetComponent<Image>();
        if (img != null)
            img.sprite = sprite;
        else
            Debug.LogError("StageItemPrefab has no Image component!");
    }


    //Selects 1 image per eligible stage and stores in dictionary
    public void CacheRandomImagesForPet(string petId,string petName, string stagePassed)
    {
        PetAlbumData data = new PetAlbumData(petId);
        data.kidImageName = GetRandomImageName("KidStage");

        if (stagePassed == "Teen" || stagePassed == "Adult" || stagePassed == "Old")
            data.teenImageName = GetRandomImageName("TeenStage");

        if (stagePassed == "Adult" || stagePassed == "Old")
            data.adultImageName = GetRandomImageName("AdultStage");

        if (stagePassed == "Old")
            data.oldImageName = GetRandomImageName("OldStage");

        data.gratitudeIndex = Random.Range(0, gratitudeMessages.Length);

        petAlbumDataDict[petId] = data;

        // Save the updated album data
        SavePetAlbumData();
    }

    string GetPreviewStage(PetAlbumData data, out string imageName)
    {
        if (!string.IsNullOrEmpty(data.oldImageName)) { imageName = data.oldImageName; return "OldStage"; }
        if (!string.IsNullOrEmpty(data.adultImageName)) { imageName = data.adultImageName; return "AdultStage"; }
        if (!string.IsNullOrEmpty(data.teenImageName)) { imageName = data.teenImageName; return "TeenStage"; }
        imageName = data.kidImageName; return "KidStage";
    }

    string GetRandomImageName(string folder)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>($"DigitalAlbum/{folder}");
        if (sprites.Length == 0) return null;
        return sprites[Random.Range(0, sprites.Length)].name;
    }

    public List<PetData> LoadDeadPets()
    {
        List<PetData> allPetList = new List<PetData>();

        if (PlayerPrefs.HasKey("DeadPets"))
        {
            string json = PlayerPrefs.GetString("DeadPets");
            PetDataList petList = JsonUtility.FromJson<PetDataList>(json);
            allPetList = petList.pets;
        }

        return allPetList;
    }

    public void SaveDeadPet(string petId,string petName, string stagePassed)
    {
        PetDataList petList;

        // Load existing list from PlayerPrefs
        if (PlayerPrefs.HasKey("DeadPets"))
        {
            string json = PlayerPrefs.GetString("DeadPets");
            petList = JsonUtility.FromJson<PetDataList>(json);
        }
        else
        {
            petList = new PetDataList();
        }

        // Add new pet data
        petList.pets.Add(new PetData { id = petId , name = petName, stagePassed = stagePassed });

        // Save updated list
        string updatedJson = JsonUtility.ToJson(petList);
        PlayerPrefs.SetString("DeadPets", updatedJson);
        PlayerPrefs.Save();
    }

    public void SavePetAlbumData()
    {
        // Create a list to hold all PetAlbumData items
        List<PetAlbumData> albumDataList = new List<PetAlbumData>(petAlbumDataDict.Values);

        // Convert the list to JSON
        string json = JsonUtility.ToJson(new PetAlbumDataListWrapper(albumDataList));

        // Save the JSON to PlayerPrefs
        PlayerPrefs.SetString("PetAlbumData", json);
        PlayerPrefs.Save();
    }

    public void LoadPetAlbumData()
    {
        // Check if the data exists in PlayerPrefs
        if (PlayerPrefs.HasKey("PetAlbumData"))
        {
            string json = PlayerPrefs.GetString("PetAlbumData");

            // Deserialize the JSON to a list of PetAlbumData
            PetAlbumDataListWrapper wrapper = JsonUtility.FromJson<PetAlbumDataListWrapper>(json);

            // Populate the petAlbumDataDict from the loaded list
            foreach (var data in wrapper.albumDataList)
            {
                petAlbumDataDict[data.petId] = data;
            }
        }
    }
}

[System.Serializable]
public class PetData
{
    public string id;
    public string name;
    public string stagePassed;
}

[System.Serializable]
public class PetAlbumData
{
    public string petId;
    public string petName;
    public string kidImageName;
    public string teenImageName;
    public string adultImageName;
    public string oldImageName;
    public int gratitudeIndex = -1;

    public PetAlbumData(string id)
    {
        petId = id;
    }

    public PetAlbumData(string id, string name)
    {
        petId = id;
        petName = name;
    }
}

[System.Serializable]
public class PetDataList
{
    public List<PetData> pets = new List<PetData>();
}

[System.Serializable]
public class PetAlbumDataListWrapper
{
    public List<PetAlbumData> albumDataList;

    public PetAlbumDataListWrapper(List<PetAlbumData> list)
    {
        albumDataList = list;
    }
}

