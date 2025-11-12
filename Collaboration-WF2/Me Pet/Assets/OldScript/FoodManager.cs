using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class FoodManager : MonoBehaviour
{
    public GameObject FoodSelection;
    public GameObject FoodDialog;
    public GameObject FoodFullDialog;
    public Image selectedFoodImage;
    public Energy_Bar energyBarScript;
    public AudioSource audioSource;
    public AudioClip hungerWarningClip;
    private bool hasPlayedHungerSound = false;


    [System.Serializable]
    public class FoodData
    {
        public string foodName;
        public Sprite foodSprite;
    }

    public List<FoodData> foodList = new List<FoodData>();
    private int currentFoodIndex = 0;
    private Dictionary<string, Sprite> foodDict;
    //private bool isFoodChanged = false;

    void Start()
    {
        // Initialize the food dictionary
        foodDict = new Dictionary<string, Sprite>();
        for (int i = 0; i < foodList.Count; i++)
        {
            foodDict[foodList[i].foodName.ToLower()] = foodList[i].foodSprite;
        }

        // Show default food (apple or first item)
        if (foodList.Count > 0)
        {
            SetFoodByIndex(0);
        }
    }


    void Update()
    {
        if (energyBarScript != null)
        {
            if (energyBarScript.hunger_current < 30)
            {
                if (!FoodDialog.activeSelf)
                {
                    FoodDialog.SetActive(true);
                }

                if (!hasPlayedHungerSound && audioSource != null && hungerWarningClip != null)
                {
                    audioSource.PlayOneShot(hungerWarningClip);
                    hasPlayedHungerSound = true;
                }
            }
            else
            {
                if (FoodDialog.activeSelf)
                {
                    FoodDialog.SetActive(false);
                }
                hasPlayedHungerSound = false; // reset when hunger goes back up
            }
            if (energyBarScript.hunger_current < 100)
            {
                if (FoodFullDialog != null && FoodFullDialog.activeSelf)
                {
                    FoodFullDialog.SetActive(false);
                }
            }
        }
    }


    public void SelectFood(string foodName)
    {
        for (int i = 0; i < foodList.Count; i++)
        {
            if (foodList[i].foodName.ToLower() == foodName.ToLower())
            {
                currentFoodIndex = i;
                SetFoodByIndex(i);
                break;
            }
        }

        FoodSelection.SetActive(false);
    }

    public void ClosePopup()
    {
        FoodSelection.SetActive(false);
        SetFoodByIndex(currentFoodIndex);
    }

    public void NextFood()
    {
        if (foodList.Count == 0) return;

        currentFoodIndex = (currentFoodIndex + 1) % foodList.Count;
        SetFoodByIndex(currentFoodIndex);
    }

    public void PrevFood()
    {
        if (foodList.Count == 0) return;

        currentFoodIndex = (currentFoodIndex - 1 + foodList.Count) % foodList.Count;
        SetFoodByIndex(currentFoodIndex);
    }

    private void SetFoodByIndex(int index)
    {
        selectedFoodImage.sprite = foodList[index].foodSprite;
        selectedFoodImage.color = Color.white;
    }

}
