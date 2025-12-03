using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PetStatus;

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

    public List<PetDataID> allPetList;
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

    //void PopulatePetList()
    //{

    //    foreach (Transform child in petListContent)
    //        Destroy(child.gameObject);

    //    foreach (var pet in allPetList)
    //    {

    //        //custom UI layout showing a pet's image and name
    //        GameObject petGO = Instantiate(petItemPrefab, petListContent);

    //        //Sets the displayed text to the pet's name
    //        petGO.transform.Find("PetNameText").GetComponent<TextMeshProUGUI>().text = pet.name;

    //        //pre-chosen image names for Kid/Teen/Adult/Old stages
    //        PetAlbumData albumData = petAlbumDataDict[pet.id];

    //        //Chooses the most appropriate available stage image for preview
    //        string stageFolder = GetPreviewStage(albumData, out string imageName);

    //        //Loads the image
    //        Sprite sprite = Resources.Load<Sprite>($"DigitalAlbum/{stageFolder}/{imageName}");

    //        //If the sprite was loaded, sets it to the Image UI component named PetImage inside the prefab
    //        if (sprite != null)
    //            petGO.transform.Find("PetImage").GetComponent<Image>().sprite = sprite;

    //        //switches to the album panel and shows that pet's stage images

    //        string capturedId = pet.id;
    //        string capturedName = pet.name;
    //        petGO.GetComponent<Button>().onClick.AddListener(() => ShowPetAlbum(capturedId, capturedName));
    //    }
    //}

    void PopulatePetList()
    {
        // Clear old UI
        foreach (Transform child in petListContent)
            Destroy(child.gameObject);

        // No dead pets
        if (allPetList == null || allPetList.Count == 0)
        {
            Debug.Log("No dead pets — show empty album.");
            EmptyDigitalAlbum.SetActive(true);
            return;
        }

        EmptyDigitalAlbum.SetActive(false);

        foreach (var pet in allPetList)
        {
            GameObject petGO = Instantiate(petItemPrefab, petListContent);

            // 1. Display Pet Name
            petGO.transform.Find("PetNameText")
                           .GetComponent<TextMeshProUGUI>()
                           .text = pet.name;

            // 2. Get album data (safer)
            if (!petAlbumDataDict.TryGetValue(pet.id, out PetAlbumData albumData))
            {
                Debug.LogWarning($"No album data for petId={pet.id}, generating...");
                CacheRandomImagesForPet(pet.id, pet.name, pet.stagePassed);
                albumData = petAlbumDataDict[pet.id];
            }

            // 3. Pick a preview image (Kid/Teen/Adult/Old)
            string stageFolder = GetPreviewStage(albumData, out string imageName);

            Sprite preview = null;
            if (!string.IsNullOrEmpty(imageName))
                preview = Resources.Load<Sprite>($"DigitalAlbum/{stageFolder}/{imageName}");

            // 4. Apply sprite or fallback
            Image img = petGO.transform.Find("PetImage").GetComponent<Image>();

            if (preview != null)
            {
                img.sprite = preview;
            }
            else
            {
                Debug.LogWarning($"Missing preview image for {pet.name}. Using placeholder.");
                img.sprite = Resources.Load<Sprite>("DigitalAlbum/Placeholder/defaultPet");
            }

            // 5. Add click to open album
            string capturedId = pet.id;
            string capturedName = pet.name;

            petGO.GetComponent<Button>().onClick.AddListener(() =>
            {
                ShowPetAlbum(capturedId, capturedName);
            });
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
    public void CacheRandomImagesForPet(string petId, string petName, string stagePassed)
    {
        if (string.IsNullOrEmpty(petId))
        {
            Debug.LogError("CacheRandomImagesForPet called with null/empty petId. petName: " + petName);
            return;
        }

        if (petAlbumDataDict == null)
            petAlbumDataDict = new Dictionary<string, PetAlbumData>();

        PetAlbumData data = new PetAlbumData(petId)
        {
            kidImageName = GetRandomImageName("KidStage"),
            gratitudeIndex = Random.Range(0, gratitudeMessages.Length)
        };

        if (stagePassed == "Teen" || stagePassed == "Adult" || stagePassed == "Old")
            data.teenImageName = GetRandomImageName("TeenStage");

        if (stagePassed == "Adult" || stagePassed == "Old")
            data.adultImageName = GetRandomImageName("AdultStage");

        if (stagePassed == "Old")
            data.oldImageName = GetRandomImageName("OldStage");

        petAlbumDataDict[petId] = data; // safe now

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

    public List<PetDataID> LoadDeadPets()
    {
        List<PetDataID> allPetList = new List<PetDataID>();

        if (PlayerPrefs.HasKey("DeadPets"))
        {
            string json = PlayerPrefs.GetString("DeadPets");
            PetDataList petList = JsonUtility.FromJson<PetDataList>(json);
            allPetList = petList.pets;
        }

        return allPetList;
    }

    public void SaveDeadPet(string petId, string petName, string stagePassed)
    {
        if (string.IsNullOrEmpty(petId))
        {
            Debug.LogError("SaveDeadPet called with empty petId. petName: " + petName);
            return;
        }

        PetDataList petList;
        if (PlayerPrefs.HasKey("DeadPets"))
        {
            string json = PlayerPrefs.GetString("DeadPets");
            petList = JsonUtility.FromJson<PetDataList>(json) ?? new PetDataList();
        }
        else
        {
            petList = new PetDataList();
        }

        petList.pets.Add(new PetDataID { id = petId, name = petName, stagePassed = stagePassed });

        // Save updated list to PlayerPrefs first so LoadDeadPets() will see it
        string updatedJson = JsonUtility.ToJson(petList);
        PlayerPrefs.SetString("DeadPets", updatedJson);
        PlayerPrefs.Save();

        Debug.Log(updatedJson);

        // Ensure dict exists before caching images
        if (petAlbumDataDict == null)
            petAlbumDataDict = new Dictionary<string, PetAlbumData>();

        // Cache images (now safe)
        CacheRandomImagesForPet(petId, petName, stagePassed);

        // call reset/save (this awaits cloud save inside)
        ResetPetAfterDeath();
    }


    public async void ResetPetAfterDeath()
    {
        // Load existing data
        PetData data;
        if (PlayerPrefs.HasKey("PetData"))
        {
            string oldJson = PlayerPrefs.GetString("PetData");
            data = JsonUtility.FromJson<PetData>(oldJson) ?? new PetData();
        }
        else
        {
            data = new PetData();
        }

        var albumManager = this; // since ResetPetAfterDeath is inside DigitalAlbumManager
        if (albumManager == null)
        {
            albumManager = FindObjectOfType<DigitalAlbumManager>();
        }

        if (albumManager != null)
        {
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

        // Reset all gameplay stats
        data.energy = 0;
        data.hunger = 0;
        data.happiness = 0;
        data.health = 0;
        data.progress = 0;
        data.stage = 0;
        data.moneyValue = 0;
        data.dirty = 0;
        data.ownedItems.Clear();
        data.firstTime = true; // so StartScene knows to create new pet
        data.lastEnergySecond = 0;
        data.lastHappinessSecond = 0;
        data.lastHealthSecond = 0;
        data.lastProgressSecond = 0;
        data.lastHungerSecond = 0;
        data.lastSavedTime = System.DateTime.Now.ToString();

        // Save to PlayerPrefs & Cloud
        string json = JsonUtility.ToJson(data);
        DataManager.savePetDataToLocal(json);
        await DataManager.SavePetDataToCloud(json);

        Debug.Log("Pet data reset after death. Dead pets and album saved.");
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

    public void LoadAlbumFromCloud(PetStatus.PetData data)
    {
        // Load dead pets
        allPetList = new List<PetDataID>();
        foreach (var p in data.deadPets)
        {
            allPetList.Add(new PetDataID
            {
                id = p.id,
                name = p.name,
                stagePassed = p.stagePassed
            });
        }

        // Load album stage images
        petAlbumDataDict = new Dictionary<string, PetAlbumData>();

        foreach (var record in data.albumDataList)
        {
            PetAlbumData album = new PetAlbumData(record.petId, record.petName)
            {
                kidImageName = record.kidImageName,
                teenImageName = record.teenImageName,
                adultImageName = record.adultImageName,
                oldImageName = record.oldImageName,
                gratitudeIndex = record.gratitudeIndex
            };

            petAlbumDataDict[record.petId] = album;
        }

        Debug.Log("Digital album loaded from cloud.");
    }
}

[System.Serializable]
public class PetDataID
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
    public List<PetDataID> pets = new List<PetDataID>();
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



