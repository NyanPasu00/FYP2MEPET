using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIController : MonoBehaviour
{
    public Energy_Bar petStatus;
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
    private bool isLightOn = true;
    public GameObject HallLightScreen;
    public GameObject HallDarkScreen;
    public Animator petAnimator;

    //Kitchen Cart
    public Cart cart;
    public GameUI gameUI;

    public TMP_InputField nameInputField;

    //Draggable Food
    public RectTransform draggableFood;
    public Canvas canvas;
    //public Transform foodOriginalParent;
    public Transform plateArea;
    private Vector2 originalPos;
    private CanvasGroup dragCanvasGroup;
    public static UIController instance;

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

    private HashSet<string> likedCategories = new HashSet<string>();
    private string lastCategoryName = null;
    private int sameCategoryCount = 0;
    private int boredomThreshold = 4;
    private Coroutine regenHappinessCoroutine;

    private Coroutine musicRewardCoroutine;
    private int moneyEarnedThisPlay = 0;


    void Start()
    {
        dragCanvasGroup = draggableFood.GetComponent<CanvasGroup>();
        originalPos = draggableFood.anchoredPosition;

        InitMusicPreferences();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        instance = this;
    }

    public string currentFoodName;

    public void SelectFood(string foodName)
    {
        currentFoodName = foodName;

        // get sprite
        Sprite sprite = gameUI.foodSprites[foodName];

        // update selected UI
        gameUI.displaySelectedFood(foodName, sprite);

        
        //draggableFood.SetActive(true);
        draggableFood.GetComponent<Image>().sprite = sprite;

        // reset position
        gameUI.resetFoodLocation();
    }

    public void connect()
    {

    }

    public void isValidateConnectionStatus()
    {

    }

    public void IsLogin()
    {

    }

    public void isValidateNewUser()
    {

    }

    public void onClick()
    {

    }

    public void isValidateLoginType()
    {

    }

    public void isValidateLoginStatus(string status)
    {

    }

    public void createPet()
    {
        string petName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(petName))
        {
            Debug.LogWarning("Pet name is empty!");
            return;
        }

        //FindFirstObjectByType<PetStatus>()?.createPetStatus(petName);

        // Load your main game scene
        SceneManager.LoadScene("HallScene");
    
    }

    public void OnBeginDrag()
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

        if (IsOnPlate(e))
            HandleEating();
        else
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
        // decrease food quantity
        petStatus.ownedItems[currentFoodName]--;

        if (petStatus.ownedItems[currentFoodName] <= 0)
            petStatus.ownedItems.Remove(currentFoodName);

        // give the pet food
        petStatus.IncreaseFood();

        // save data
        petStatus.SavePetData();

        // update UI
        gameUI.displayAvailableFood(petStatus.ownedItems, gameUI.foodSprites);
        gameUI.resetFoodLocation();
    }

    public void isHunger()
    {

    }

    public void isHealth()
    {

    }

    public void addItemToCart(ItemData item)
    {
        cart.addItem(item);
        Debug.Log("UIController passed item to Cart");
    }

    public void removeItemFromCart(string itemName)
    {
        cart.removeItem(itemName);
        gameUI.displayCart(cart); // refresh the UI
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


        // Process payment
        petStatus.updateProductandMoney(cart.items, totalCost);

        // Clear cart after purchase
        cart.items.Clear();
        cart.calculateTotalCost();

        // Update UI again (refresh)
        gameUI.displayCart(cart);
        gameUI.closeCartandShop();


        Debug.Log("Payment successful.");
    }

    public void validateSelectedGame()
    {

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

    public void requestSleepOrAwake()
    {
        isLightOn = !isLightOn;

        HallLightScreen.SetActive(isLightOn);
        HallDarkScreen.SetActive(!isLightOn);

        if (isLightOn)
        {
            petAnimator.SetBool("Laydown", true);
            petAnimator.SetBool("Sleep", false);
            musicToggleButton.gameObject.SetActive(true);

            //if (regenEnergyCoroutine != null && stopEnergyCoroutine == null)
            //{
            //    stopEnergyCoroutine = StartCoroutine(DelayedStopEnergy());
            //}

            isSleep = false;
            //PlayerPrefs.SetInt("IsSleeping", isSleeping ? 1 : 0);
            //PlayerPrefs.Save();
            //energyBar.ResumeEnergyDeduction();
        }
        else
        {
            petAnimator.SetBool("Laydown", false);
            petAnimator.SetBool("Sleep", true);
            musicToggleButton.gameObject.SetActive(false);

            //if (!isWaitingToRegen && regenEnergyCoroutine == null)
            //{
            //    regenEnergyCoroutine = StartCoroutine(DelayedRegenerateEnergy());
            //}

            isSleep = true;
            //PlayerPrefs.SetInt("IsSleeping", isSleep ? 1 : 0);
            //PlayerPrefs.Save();
            //energyBar.PauseEnergyDeduction();
        }
    }

    public void IsSleep()
    {

    }

    public void isEnergyLow()
    {

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
        boredomThreshold = Random.Range(4, 7);   // 4–6 clicks
        ShuffleLikedCategories();
    }

    private void ShuffleLikedCategories()
    {
        likedCategories.Clear();

        if (musicCategories == null || musicCategories.Count == 0)
            return;

        // indices 0..N-1
        List<int> idx = new List<int>();
        for (int i = 0; i < musicCategories.Count; i++)
            idx.Add(i);

        // Fisher–Yates shuffle
        for (int i = 0; i < idx.Count; i++)
        {
            int j = Random.Range(i, idx.Count);
            int temp = idx[i];
            idx[i] = idx[j];
            idx[j] = temp;
        }

        // Pick first 3 as liked
        int likeCount = Mathf.Min(3, idx.Count);
        for (int i = 0; i < likeCount; i++)
        {
            likedCategories.Add(musicCategories[idx[i]].categoryName);
        }

        Debug.Log("New liked categories: " + string.Join(", ", likedCategories));
    }

    private void PlayCategorySong(string categoryName)
    {
        // Stop background BGM
        if (bgm != null)
        {
            bgm.StopMusic();
        }

        // Find the category
        MusicCategory category = musicCategories.Find(c => c.categoryName == categoryName);
        if (category == null || category.songs == null || category.songs.Count == 0)
        {
            Debug.LogWarning("No songs configured for category " + categoryName);
            return;
        }

        // Randomly pick one song from this category
        int songIndex = Random.Range(0, category.songs.Count);
        musicPlayer.clip = category.songs[songIndex];
        musicPlayer.Play();

        // Decide if pet likes this CATEGORY
        bool categoryLiked = likedCategories.Contains(categoryName);
        isLikeCategory = categoryLiked;

        // Decide if this particular song is liked
        // Roll 1–10
        int roll = Random.Range(1, 11);
        bool songDisliked;

        if (categoryLiked)
        {
            // 10% dislike
            songDisliked = (roll == 1);
        }
        else
        {
            // 80% dislike
            songDisliked = (roll <= 8);
        }

        bool songLiked = !songDisliked;
        isLikeMusic = songLiked;

        UpdatePetReaction(songLiked);

        // Boredom logic: same category clicked too many times
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
            // Pet is bored; randomise liked categories again
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
        // Stop previous regen if any
        if (regenHappinessCoroutine != null)
        {
            StopCoroutine(regenHappinessCoroutine);
            regenHappinessCoroutine = null;
        }

        // Also stop previous music reward if any
        if (musicRewardCoroutine != null)
        {
            StopCoroutine(musicRewardCoroutine);
            musicRewardCoroutine = null;
        }

        if (songLiked)
        {
            // Happy / dancing
            petAnimator.SetBool("Sad", false);
            petAnimator.SetBool("Laydown", false);
            petAnimator.SetBool("Dance", true);

            if (petPosition != null)
            {
                petPosition.localPosition = new Vector3(-0.41f, -3.93f, 0f);
            }

            if (reactionText != null)
            {
                reactionText.text = "I love this song!";
            }

            // Regen happiness over time
            regenHappinessCoroutine = StartCoroutine(RegenerateHappiness());
            petStatus.PauseHappinessDeduction();

            PlayerPrefs.SetInt("IsDancing", 1);
            PlayerPrefs.Save();

            // Start reward coroutine: +money, -energy every 10 seconds (max +20 money per play)
            musicRewardCoroutine = StartCoroutine(MusicRewardRoutine());
        }
        else
        {
            // Sad / dislike
            petAnimator.SetBool("Dance", false);
            petAnimator.SetBool("Laydown", false);
            petAnimator.SetBool("Sad", true);

            if (petPosition != null)
            {
                switch (petStatus.currentStage)
                {
                    case Energy_Bar.PetStage.Kid:
                        petPosition.localPosition = new Vector3(-1.14f, -3.78f, 0f);
                        break;
                    case Energy_Bar.PetStage.Teen:
                        petPosition.localPosition = new Vector3(-1.68f, -3.82f, 0f);
                        break;
                    case Energy_Bar.PetStage.Adult:
                        petPosition.localPosition = new Vector3(-2.08f, -3.91f, 0f);
                        break;
                    case Energy_Bar.PetStage.Old:
                        petPosition.localPosition = new Vector3(-2.08f, -3.91f, 0f);
                        break;
                }
            }

            if (reactionText != null)
            {
                reactionText.text = "I hate this song...";
            }

            PlayerPrefs.SetInt("IsDancing", 0);
            PlayerPrefs.Save();

            // Punish happiness a bit
            petStatus.decreaseHappiness(5);
            petStatus.ResumeHappinessDeduction();
        }
    }

    private IEnumerator RegenerateHappiness()
    {
        while (petStatus.happiness_current < petStatus.happiness_max)
        {
            petStatus.happiness_current += 1;
            if (petStatus.happiness_current > petStatus.happiness_max)
                petStatus.happiness_current = petStatus.happiness_max;

            petStatus.happiness_Slider.value =
                (float)petStatus.happiness_current / petStatus.happiness_max;
            petStatus.happinessDetail_Slider.value =
                (float)petStatus.happiness_current / petStatus.happiness_max;

            yield return new WaitForSeconds(1f);
        }

        regenHappinessCoroutine = null;
    }

    private IEnumerator MusicRewardRoutine()
    {
        moneyEarnedThisPlay = 0;

        // reward only while music is playing AND song is liked
        while (musicPlayer != null && musicPlayer.isPlaying && isLikeMusic)
        {
            if (moneyEarnedThisPlay >= 20)
                break; // reached max reward for this play

            // wait 10 seconds of continuous listening
            yield return new WaitForSeconds(10f);

            // check again in case music stopped during the wait
            if (musicPlayer == null || !musicPlayer.isPlaying || !isLikeMusic)
                break;

            if (moneyEarnedThisPlay >= 20)
                break;

            // +5 money
            petStatus.AddMoney(5);          // uses the new method in Energy_Bar
            moneyEarnedThisPlay += 5;

            // - energy (for example, 3 points per 10s; adjust as you like)
            petStatus.energy_current = Mathf.Max(0, petStatus.energy_current - 3);

            petStatus.GetEnergyFill();

            // save data after change
            petStatus.SavePetData();
        }

        musicRewardCoroutine = null;
    }

    public void OnExitMusicScreen()
    {
        // Stop song, resume BGM
        if (musicPlayer != null && musicPlayer.isPlaying)
        {
            musicPlayer.Stop();
        }

        if (bgm != null)
        {
            bgm.PlayMusic();
        }

        PlayerPrefs.SetInt("IsDancing", 0);
        PlayerPrefs.Save();

        petAnimator.SetBool("Dance", false);
        petAnimator.SetBool("Sad", false);
        petAnimator.SetBool("Laydown", true);

        if (petPosition != null)
        {
            switch (petStatus.currentStage)
            {
                case Energy_Bar.PetStage.Kid:
                    petPosition.localPosition = new Vector3(-1.25f, -2.4f, 0f);
                    break;
                case Energy_Bar.PetStage.Teen:
                    petPosition.localPosition = new Vector3(-1.83f, -2.65f, 0f);
                    break;
                case Energy_Bar.PetStage.Adult:
                    petPosition.localPosition = new Vector3(-2.38f, -2.7f, 0f);
                    break;
                case Energy_Bar.PetStage.Old:
                    petPosition.localPosition = new Vector3(-2.42f, -2.75f, 0f);
                    break;
            }
        }

        if (regenHappinessCoroutine != null)
        {
            StopCoroutine(regenHappinessCoroutine);
            regenHappinessCoroutine = null;
        }

        if (musicRewardCoroutine != null)
        {
            StopCoroutine(musicRewardCoroutine);
            musicRewardCoroutine = null;
        }

        petStatus.ResumeHappinessDeduction();
    }

    public void RequestPlayCategorySong(string categoryName)
    {
        PlayCategorySong(categoryName);   // this is the method we wrote earlier with probabilities etc.
    }

}
