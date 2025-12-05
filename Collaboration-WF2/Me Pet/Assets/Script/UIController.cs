using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
public class UIController : MonoBehaviour , IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public LoginManager loginManager;
    public Internet internetChecker;
    public PetStatus petStatus;
    public bool isLogin = false;
    public bool isOnPet = false;
    public bool isSleep = true;
    public bool isMoneyEnough = false;
    public bool connectionStatus = false;
    public string loginType;
    public string loginStatus;
    public int likePercentage;
    public int energyLevel;
    public float dirtyLevel;
    public int bubbleNum;
    public int eventOptionSelected;
    public bool isLikeMusic = false;
    public bool isLikeCategory = false;


    public Button musicToggleButton;

    //Hall Sleep and Awake
    [Header ("Hall Sleep and Awake")]
    private bool isLightOn = true;
    public GameObject HallLightScreen;
    public GameObject HallDarkScreen;
    public Animator petAnimator;
    public bool isWaitingToRegen = false;

    public BathController bathController;

    //Kitchen Cart
    public Cart cart;
    public GameUI gameUI;
  

    public TMP_InputField nameInputField;

    //Draggable Food
    [Header("Kitchen")]
    public Animator kitchenPetAnimator;
    public RectTransform draggableFood;
    public Canvas canvas;
    public GameObject FoodFullDialog;
    public GameObject FeedSuccessDialogue;
    public AudioSource eatingSound;
    public static UIController instance;
    public bool isEating;
    public Transform originalParent;
    public Vector2 originalAnchoredPosition;
    private CanvasGroup dragCanvasGroup;
    private Vector3 originalPosition;
    public string currentFoodName;
    

    [System.Serializable]
    public class MusicCategory
    {
        public string categoryName;       // Rock, Rap, Reggae, etc.
        public List<AudioClip> songs;     // 5–10 clips per category
    }

    [Header("Music")]
    public BGMScript bgm;                    // background BGM controller
    public AudioSource musicPlayer;          // AudioSource used to play songs
    public Transform petPosition;            // pet transform
    public TextMeshProUGUI reactionText;     // text "I love this song" / "I hate this song"
    public List<MusicCategory> musicCategories = new List<MusicCategory>();
    private Vector3 originalPetPosition;

    private HashSet<string> likedCategories = new HashSet<string>();
    private string lastCategoryName = null;
    private string currentCategoryPlaying = null;
    private int sameCategoryCount = 0;
    private int boredomThreshold = 4;
 

    private Coroutine musicRewardCoroutine;
    private int moneyEarnedThisPlay = 0;

    [Header("Money Popup")]
    public MoneyPopup moneyPopupPrefab;  // assign prefab in Inspector
    public Transform coinIcon;       // assign your coin icon RectTransform
    public Canvas Songcanvas;

    public GameObject Cloud;
    public GameObject LowEnergyCloud;


    void Start()
    {
        dragCanvasGroup = draggableFood.GetComponent<CanvasGroup>();
        originalAnchoredPosition = draggableFood.anchoredPosition;

        originalParent = draggableFood.parent;

        originalPetPosition = petPosition.position;

        InitMusicPreferences();

        // NEW: initialise hall spawn based on sleep state
        if (bathController != null)
        {
            // default: isSleep = true ⇒ lying spawn
            bathController.SetHallStanding(false);
        }
    }

    void Update()
    {
        if (petStatus != null)
        {
            if (petStatus.health_current <= 60 && petStatus.currentStage == PetStatus.PetStage.Old)
            {
                isHealth();
            }
            else if (petStatus.hunger_current <= 50)
            {
                isHunger();
            }
            else
            {
                HideMessage();
            }


        }
    }

    void Awake()
    {
        instance = this;

        bgm = BGMScript.Instance;

        originalParent = draggableFood.parent;
        dragCanvasGroup = draggableFood.GetComponent<CanvasGroup>();
        originalAnchoredPosition = draggableFood.anchoredPosition;

    }

    public void SelectFood(string foodName)
    {
        currentFoodName = foodName;
        UIController.instance.currentFoodName = foodName;
        // get sprite
        Sprite sprite = gameUI.foodSprites[foodName];

        // update selected UI
        gameUI.displaySelectedFood(foodName, sprite);

        draggableFood.GetComponent<Image>().sprite = sprite;

        gameUI.openFridge();

        // reset position
        gameUI.resetFoodLocation();
    }

    public void connect()
    {
        if (!internetChecker.isConnect())
        {
            Debug.Log("No internet");
            return;
        }
        Debug.Log("Validating");
        isValidateConnectionStatus();
    }

    public void isValidateConnectionStatus()
    {
        if (!internetChecker.isValidateConnectionStatus())
        {
            Debug.Log("Connection unstable");
            return;
        }
        Debug.Log("Check is Login?");
        IsLogin();
    }

    public async void IsLogin()
    {
        if (loginManager != null)
        {
            await loginManager.CheckLoginAsync();
        }
        else
        {
            Debug.LogError("LoginManager reference not set!");
        }
    }


    public void createPet()
    {
        string petName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(petName))
        {
            Debug.LogWarning("Pet name is empty!");
            return;
        }

        SceneManager.LoadScene("HallScene");
    }

    public void OnBeginDrag(PointerEventData e)
    {
        dragCanvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData e)
    {
        draggableFood.anchoredPosition += e.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData e)
    {
        dragCanvasGroup.blocksRaycasts = true;

        GameObject hovered = e.pointerCurrentRaycast.gameObject;

        if (hovered != null && hovered.CompareTag("Plate"))
        {
            if (petStatus.hunger_current >= 100)
            {
                FoodFullDialog.SetActive(true);
                // Return potion to original position
                transform.position = originalPosition;
            }
            else
            {
                isEating = true;
                PlayerPrefs.SetInt("IsEating", isEating ? 1 : 0);
                PlayerPrefs.Save();

                transform.SetParent(hovered.transform);
                draggableFood.anchoredPosition = new Vector2(0, 35);

                HandleEating();
                return;
            }
        }

        // Invalid drop — reset
        gameUI.resetFoodLocation();
    }

    public bool IsOnPlate(PointerEventData e)
    {
        GameObject hovered = e.pointerCurrentRaycast.gameObject;
        if (hovered == null) return false;

        return hovered.CompareTag("Plate");
    }

    public void HandleEating()
    {
        string foodName = UIController.instance.currentFoodName;
        dragCanvasGroup.blocksRaycasts = false;

        if (kitchenPetAnimator != null)
            kitchenPetAnimator.SetBool("Eating", true);

        if (eatingSound != null)
            eatingSound.Play();

        if (foodName == null)
        {
            Debug.Log("Name is Null");
        }

        

        if (FeedSuccessDialogue != null)
            FeedSuccessDialogue.SetActive(true);

        Invoke(nameof(FinishEating), 0.8f);
    }

    private void FinishEating()
    {
        if (petAnimator != null)
            kitchenPetAnimator.SetBool("Eating", false);

        if (eatingSound != null)
            eatingSound.Stop();

        if (FeedSuccessDialogue != null)
            FeedSuccessDialogue.SetActive(false);

        gameUI.resetFoodLocation();


        string foodName = UIController.instance.currentFoodName;
        if(foodName == "potion")
        {
            petStatus.updateMedicineStatus(foodName);
        }
        petStatus.updateFoodStatus(foodName);

        if (!petStatus.ownedItems.ContainsKey(foodName) || petStatus.ownedItems[foodName] <= 0)
        {
            gameUI.clearSelectedFood();
            UIController.instance.currentFoodName = null;
        }

        dragCanvasGroup.blocksRaycasts = true;

        isEating = false;
        PlayerPrefs.SetInt("IsEating", 0);
        PlayerPrefs.Save();
    }

    public void isHunger()
    {

        if (Cloud != null)
        {
            Cloud.SetActive(true);
        }

        if (gameUI != null)
        {
            gameUI.displayPetMessage("I am Hungry , Feed Me Please");
        }
            
        
    }

    public void HideMessage()
    {
        if (Cloud != null) 
        { 
            Cloud.SetActive(false);
        }
        
    }

    public void isHealth()
    {

        if (Cloud != null)
        {
            Cloud.SetActive(true);
        }

        if (gameUI != null)
        {
            gameUI.displayPetMessage("Not Feeling Good , Feed Me Medicine");
        }
            
        
    }

    public void addItemToCart(ItemData item)
    {
        cart.addItem(item);
        gameUI.displayCart(cart);

        if (gameUI != null)
        {
            gameUI.ShowToast("Item added to cart");
        }

        Debug.Log("UIController passed item to Cart");
    }

    public void removeItemFromCart(string itemName)
    {
        cart.removeItem(itemName);
        gameUI.displayCart(cart);
    }

    public void modifyItemQuantity(string itemName, int change)
    {
        cart.updateCart(itemName, change);
        gameUI.displayCart(cart);
    }

    public void requestCheckout()
    {
        int moneyValue = petStatus.RetrieveValue("moneyValue");

        cart.calculateTotalCost();
        int totalCost = cart.totalCost;
        if (isValidateMoney(moneyValue, totalCost))
        {
            gameUI.displayConfirmationPayment(moneyValue, totalCost);
        }

        gameUI.displayPetMessage("unsuccessful");
    }

    public bool isValidateMoney(int moneyValue, int totalCost)
    {
        if (moneyValue >= totalCost)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void paymentConfirm()
    {
        int totalCost = cart.totalCost;

        petStatus.updateProductandMoney(cart.items, totalCost);

        cart.items.Clear();
        cart.calculateTotalCost();

        gameUI.displayCart(cart);
        gameUI.closeCartandShop();

        Debug.Log("Payment successful.");
    }

    public void validateSelectedGame()
    {
        if (petStatus == null)
        {
            Debug.LogWarning("petStatus is not assigned on UIController.");
            return;
        }

        // 1. Check energy first
        if (petStatus.energy_current <= 30)
        {
            // Not enough energy → show cloud & message, block the game
            isEnergyLow();
            return;
        }

        // 2. Enough energy → allow game, consume energy and add happiness
        //    (assumes stats are 0–100, adjust if your range is different)
        petStatus.energy_current = Mathf.Max(0, petStatus.energy_current - 10);

        // If your variable name is different (e.g. mood_current), change her
        petStatus.happiness_current = Mathf.Clamp(petStatus.happiness_current + 20, 0, 100);

        // Refresh UI bars (if you have these methods)
        petStatus.GetEnergyFill();
        petStatus.GetHappinessFill();   // rename if needed

        // Save pet data
        petStatus.SavePetData();

        Debug.Log("Game validated: -10 energy, +20 happiness, data saved.");

        if(gameUI != null)
        {
            gameUI.displaySelectedGame();
        }
    }

    public void validateSelectedCategory()
    {

    }

    public void IsLikeCategory()
    {

    }

    public void setLikePercentage(int percentage)
    {

    }

    public void randomPlayMusic()
    {

    }

    public void IsLikeMusic()
    {

    }

    public bool IsSleep()
    {
        return isSleep;
    }

    public void requestSleepOrAwake()
    {
        if (IsSleep())
        {
            WakeUpPet();

            if (gameUI != null)
            {
                gameUI.displayPetMessage("wakeup");
            }
            return;
        }

        int energyValue = (petStatus != null)
            ? petStatus.energy_current
            : 0;

        if (energyValue >= 95)
        {
            if (gameUI != null)
            {
                gameUI.displayPetMessage("rejectsleep");
            }
            return;
        }

        GoToSleep();

        if (gameUI != null)
        {
            gameUI.displayPetMessage("sleep");
        }
    }

    private void WakeUpPet()
    {
        isLightOn = true;
        isSleep = false;

        if (HallLightScreen != null) HallLightScreen.SetActive(true);
        if (HallDarkScreen != null) HallDarkScreen.SetActive(false);

        if (petAnimator != null)
        {
            petAnimator.SetBool("Laydown", true);
            petAnimator.SetBool("Sleep", false);
        }

        if (musicToggleButton != null)
            musicToggleButton.gameObject.SetActive(true);

        // NEW: stop sleep stat effects
        if (petStatus != null)
        {
            petStatus.stopSleepStatus();
            petStatus.ResumeEnergyDeduction();
        }

        PlayerPrefs.SetInt("IsSleeping", 0);
        PlayerPrefs.Save();
    }

    private void GoToSleep()
    {
        isLightOn = false;
        isSleep = true;

        if (HallLightScreen != null) HallLightScreen.SetActive(false);
        if (HallDarkScreen != null) HallDarkScreen.SetActive(true);

        if (petAnimator != null)
        {
            petAnimator.SetBool("Laydown", false);
            petAnimator.SetBool("Sleep", true);
        }

        // NEW: sleeping/lying ⇒ use lying hall spawn
        if (bathController != null)
        {
            bathController.SetHallStanding(false);
        }

        if (musicToggleButton != null)
            musicToggleButton.gameObject.SetActive(false);

        if (petStatus != null)
        {
            petStatus.isSleeping = true;
            // Stop normal energy deduction while sleeping…
            petStatus.PauseEnergyDeduction();

            // …and let PetStatus handle sleep energy/hunger logic
            petStatus.updateSleepStatus();
        }

        PlayerPrefs.SetInt("IsSleeping", 1);
        PlayerPrefs.Save();
    }



    public void isEnergyLow()
    {
        if (petStatus.energy_current <= 30)
        {
            if (LowEnergyCloud != null)
            {
                LowEnergyCloud.SetActive(true);

                CancelInvoke(nameof(HideLowEnergyCloud));
                Invoke(nameof(HideLowEnergyCloud), 3f);
            }      

        }
        else 
            return;
    }

    private void HideLowEnergyCloud()
    {
        if (LowEnergyCloud != null)
            LowEnergyCloud.SetActive(false);

        // if you have a way to clear the message, do it here
        // gameUI.displayLowEnergyMessage("");
    }

    public void isValidateDirty()
    {

    }

    public void requestShower()
    {

    }

    public void isValidateBubble()
    {

    }

    public void IsOnPet()
    {

    }

    public void validateEventOption()
    {

    }

    public void isValidateEventOption()
    {

    }

    private void InitMusicPreferences()
    {
        boredomThreshold = Random.Range(4, 7);
        ShuffleLikedCategories();
    }

    private void ShuffleLikedCategories()
    {
        likedCategories.Clear();

        if (musicCategories == null || musicCategories.Count == 0)
            return;

        List<int> idx = new List<int>();
        for (int i = 0; i < musicCategories.Count; i++)
            idx.Add(i);

        for (int i = 0; i < idx.Count; i++)
        {
            int j = Random.Range(i, idx.Count);
            int temp = idx[i];
            idx[i] = idx[j];
            idx[j] = temp;
        }

        int likeCount = Mathf.Min(3, idx.Count);
        for (int i = 0; i < likeCount; i++)
        {
            likedCategories.Add(musicCategories[idx[i]].categoryName);
        }

        Debug.Log("New liked categories: " + string.Join(", ", likedCategories));
    }

    private void PlayCategorySong(string categoryName)
    {
        BGMScript globalBgm = BGMScript.Instance;
        if (globalBgm != null)
        {
            globalBgm.StopMusic();
        }

        MusicCategory category = musicCategories.Find(c => c.categoryName == categoryName);
        if (category == null || category.songs == null || category.songs.Count == 0)
        {
            Debug.LogWarning("No songs configured for category " + categoryName);
            return;
        }

        int songIndex = Random.Range(0, category.songs.Count);
        musicPlayer.clip = category.songs[songIndex];
        musicPlayer.Play();

        bool categoryLiked = likedCategories.Contains(categoryName);
        isLikeCategory = categoryLiked;

        int roll = Random.Range(1, 11);
        bool songDisliked;

        if (categoryLiked)
        {
            songDisliked = (roll == 1);
        }
        else
        {
            songDisliked = (roll <= 8);
        }

        bool songLiked = !songDisliked;
        isLikeMusic = songLiked;

        UpdatePetReaction(songLiked);

        if (lastCategoryName == categoryName)
        {
            sameCategoryCount++;
        }
        else
        {
            lastCategoryName = categoryName;
            sameCategoryCount = 1;
        }

        if (sameCategoryCount >= boredomThreshold)
        {
            ShuffleLikedCategories();
            sameCategoryCount = 0;
            boredomThreshold = Random.Range(4, 7);

            if (reactionText != null)
            {
                reactionText.text = "I'm bored... let's change music.";
            }
        }
    }

    private void UpdatePetReaction(bool songLiked)
    {
        // Reset pet world position when entering music
        petPosition.position = originalPetPosition;

        // Stop money reward coroutine (still handled in UIController)
        if (musicRewardCoroutine != null)
        {
            StopCoroutine(musicRewardCoroutine);
            musicRewardCoroutine = null;
        }

        // Make sure PetStatus clears any previous music effect first
        if (petStatus != null)
        {
            petStatus.resetStatusRate();
        }

        if (songLiked)
        {
            // ------------------
            // Animations / visuals
            // ------------------
            petAnimator.SetBool("Sad", false);
            petAnimator.SetBool("Laydown", false);
            petAnimator.SetBool("Dance", true);

            if (petPosition != null && petStatus != null)
            {
                switch (petStatus.currentStage)
                {
                    case PetStatus.PetStage.Kid:
                        petPosition.localPosition = new Vector3(-1.21f, -2.332834f, 0f);
                        break;
                    case PetStatus.PetStage.Teen:
                        petPosition.localPosition = new Vector3(-0.75f, -2.332834f, 0f);
                        break;
                    case PetStatus.PetStage.Adult:
                        petPosition.localPosition = new Vector3(-0.75f, -2.332834f, 0f);
                        break;
                    case PetStatus.PetStage.Old:
                        petPosition.localPosition = new Vector3(-0.75f, -2.332834f, 0f);
                        break;
                }

                if (bathController != null)
                {
                    bathController.SetHallState(BathController.HallState.Dance);
                }
            }

            if (reactionText != null)
            {
                reactionText.text = "I love this song!";
            }

            // ------------------
            // STAT changes → PetStatus
            // ------------------
            if (petStatus != null)
            {
                petStatus.updateLikeMusicStatus();
            }

            PlayerPrefs.SetInt("IsDancing", 1);
            PlayerPrefs.Save();

            // Money reward still managed here
            musicRewardCoroutine = StartCoroutine(MusicRewardRoutine());
        }
        else
        {
            // ------------------
            // Animations / visuals
            // ------------------
            petAnimator.SetBool("Dance", false);
            petAnimator.SetBool("Laydown", false);
            petAnimator.SetBool("Sad", true);

            if (bathController != null)
            {
                bathController.SetHallState(BathController.HallState.Sad);
            }

            if (reactionText != null)
            {
                reactionText.text = "I hate this song...";
            }

            PlayerPrefs.SetInt("IsDancing", 0);
            PlayerPrefs.Save();

            // ------------------
            // STAT changes → PetStatus
            // ------------------
            if (petStatus != null)
            {
                petStatus.updateUnlikeMusicStatus();
            }
        }
    }


    private IEnumerator MusicRewardRoutine()
    {
        moneyEarnedThisPlay = 0;

        while (musicPlayer != null && musicPlayer.isPlaying && isLikeMusic)
        {
            if (moneyEarnedThisPlay >= 20)
                break;

            yield return new WaitForSeconds(10f);

            if (musicPlayer == null || !musicPlayer.isPlaying || !isLikeMusic)
                break;

            if (moneyEarnedThisPlay >= 20)
                break;

            int reward = 5;
            petStatus.AddMoney(reward);
            moneyEarnedThisPlay += reward;

            //ShowMoneyPopup(reward);

            MoneyPopup.Create(moneyPopupPrefab, Songcanvas.transform, coinIcon.position, reward);
            Debug.Log("money Created");

            petStatus.energy_current = Mathf.Max(0, petStatus.energy_current - 3);

            petStatus.GetEnergyFill();

            petStatus.SavePetData();
        }

        musicRewardCoroutine = null;
    }

    public void OnExitMusicScreen()
    {
        if (petPosition != null)
        {
            petPosition.position = originalPetPosition;
        }
        if (musicPlayer != null && musicPlayer.isPlaying)
        {
            musicPlayer.Stop();
        }

        BGMScript globalBgm = BGMScript.Instance;
        if (globalBgm != null)
        {
            globalBgm.PlayMusic();
        }

        PlayerPrefs.SetInt("IsDancing", 0);
        PlayerPrefs.Save();

        petAnimator.SetBool("Dance", false);
        petAnimator.SetBool("Sad", false);
        petAnimator.SetBool("Laydown", true);

        if (bathController != null)
        {
            bathController.SetHallState(BathController.HallState.Lying);
        }

        if (musicRewardCoroutine != null)
        {
            StopCoroutine(musicRewardCoroutine);
            musicRewardCoroutine = null;
        }

        if (petStatus != null)
        {
            petStatus.resetStatusRate();
        }
    }

    public void RequestPlayCategorySong(string categoryName)
    {
        PlayCategorySong(categoryName);
    }


}