using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class FoodSpriteEntry
{
    public string foodName;
    public Sprite foodSprite;
}
public class GameUI : MonoBehaviour
{
    [SerializeField]
    [Header("Status Panel")]
    public GameObject statusPanel;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI statusTitle;
    public TextMeshProUGUI stageText;

    private bool justOpened = false;
    public bool panelOpen = false;

    public PetStatus stats;


    [SerializeField]
    [Header("Transition")]
    public Animator transition;
    public Transform HallCameraPosition;
    public Transform KitchenCameraPosition;
    public Transform GameRoomCameraPosition;
    public Transform BathroomCameraPosition;
    public Transform MainCameraPosition;
    public float TransitionSpeed = 4f;
    public TextMeshProUGUI Title;

    //Kitchen Shop and Cart Design
    [SerializeField]
    [Header("Shop and Cart")]
    private bool isCartOpen = false;
    private bool isShopOpen = false;
    private bool isPaymentOpen = false;
    public GameObject ShopPage;
    public GameObject CartPage;
    public GameObject PaymentPage;
    public Transform cartContentParent;
    public GameObject cartItemPrefab;
    public TMP_Text totalCostText;
    public TMP_Text paymentCostText;
    public TMP_Text currentMoneyText;
    public TMP_Text remainMoneyText;
    public TMP_Text coinValueInShop;
    public TMP_Text coinValueInCart;

    //Display Food and SelectedFood
    private bool isFridgeOpen = false;
    public GameObject FridgePage;
    public Transform availableFoodParent;     
    public GameObject foodItemPrefab;
    public GameObject FoodSelection;
    public Image selectedFoodIcon;
    public TMP_Text petMessage;
    public TMP_Text petFoodMessage;


    private int currentRoomIndex = 2;
    private bool isMoving = false;

    //Changing Scene
    public bool isEating;
    public bool isSleeping;
    public bool isDancing;
    public bool isBathing;
    public bool isAlbumOpen;

    public List<FoodSpriteEntry> foodSpriteList;
    [HideInInspector]
    public Dictionary<string, Sprite> foodSprites = new Dictionary<string, Sprite>();

    [Header("Song Panel")]
    public GameObject SongMenuPanel;

    [Header("Music Screen")]
    public GameObject HallPanel;
    public GameObject MusicScreenPanel;
    public UnityEngine.UI.Button lightToggleButton; // the lamp / sleep toggle button

    public AudioSource audioClip; //click sound
    [SerializeField] private AudioClip uiClickClip;

    [Header("Light Switch Sound")]
    public AudioSource audioLightClip; //click sound
    [SerializeField] private AudioClip uiLightClip;

    [Header("Toast Message")]
    public GameObject toastPanel;              // panel background
    public TextMeshProUGUI toastText;         // text "Item added to cart"
    private Coroutine toastCoroutine;
   
    void Start()
    {
        if (statusPanel != null)
        {
            statusPanel.SetActive(false);
        }
        
    }

    public void PlayLightUIClick()
    {
        if (audioLightClip != null && uiLightClip != null)
        {
            // important: PlayOneShot, NOT .Play on a shared music source
            audioLightClip.PlayOneShot(uiLightClip);
        }
    }

    public void PlayUIClick()
    {
        if (audioClip != null && uiClickClip != null)
        {
            // important: PlayOneShot, NOT .Play on a shared music source
            audioClip.PlayOneShot(uiClickClip);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //------------------------------------------------------------------STATUS PANEL START-------------------------------------------------------------------
        // Only update if panel is open
        if (panelOpen)
        {
            UpdateStatusText();

            // Skip outside-click detection on the first frame
            if (!justOpened && Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
            {
                ClosePanel();
            }

            // Clear the flag after one frame
            if (justOpened)
            {
                justOpened = false;
            }
        }
        //-------------------------------------------------------------------STATUS PANEL END--------------------------------------------------------------------
    }

    //--------------------------------------------------------------------STATUS PANEL START-------------------------------------------------------------------
    public void ToggleStatusPanel()
    {
        panelOpen = !panelOpen;
        statusPanel.SetActive(panelOpen);

        if (panelOpen)
        {
            justOpened = true;          // <-- mark that we just opened this frame
            UpdateStatusText();
        }
    }

    void ClosePanel()
    {
        statusPanel.SetActive(false);
        panelOpen = false;
    }

    void UpdateStatusText()
    {
        float progressPercent = (float)stats.progress_current / stats.progress_max * 100f;

        string json = PlayerPrefs.GetString("PetData", string.Empty);

    string petName = "Pet";
    if (!string.IsNullOrEmpty(json))
    {
            PetStatus.PetData data = JsonUtility.FromJson<PetStatus.PetData>(json);
            if (data != null && !string.IsNullOrEmpty(data.petName))
        {
            petName = data.petName;
        }
    }

        statusTitle.text = $"{petName}'s Conditions";

        stageText.text = $"Stage: {stats.currentStage}";

        statusText.text = $"Progress: {progressPercent:F0}%\n" +
                          $"Energy: {stats.energy_current}%\n" +
                          $"Hunger: {stats.hunger_current}%\n" +
                          $"Happiness: {stats.happiness_current}%\n" +
                          $"Health: {stats.health_current}%";

    }

    bool IsPointerOverUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    //---------------------------------------------------------------STATUS PANEL END--------------------------------------------------------------------------------
    public void displayGameplay()
    {
        PlayAndLoad("KidScene");
    }

    public void displayNewGame()
    {
        PlayAndLoad("PetNameScene");
    }


    public void openFridge()
    {
        foodSprites = new Dictionary<string, Sprite>();
        for (int i = 0; i < foodSpriteList.Count; i++)
        {
            foodSprites[foodSpriteList[i].foodName.ToLower()] = foodSpriteList[i].foodSprite;
        }

        // Toggle fridge UI first
        isFridgeOpen = !isFridgeOpen;

        displayAvailableFood(stats.ownedItems, foodSprites);

        FridgePage.SetActive(isFridgeOpen);

    }

    public void displayAvailableFood(Dictionary<string, int> ownedItems, Dictionary<string, Sprite> foodSprites)
    {
        Transform realParent = GetRealParent();
        foreach (Transform t in realParent)
            Destroy(t.gameObject);
            Debug.Log("Destroy");

        foreach (var kvp in ownedItems)
        {
            string foodName = kvp.Key;
            int quantity = kvp.Value;

            GameObject go = Instantiate(foodItemPrefab, availableFoodParent);
            go.transform.localScale = Vector3.one;

            FoodItemUI ui = go.GetComponent<FoodItemUI>();

            string key = foodName.Trim().ToLower();
            if (foodSprites.ContainsKey(key))
            {
                ui.foodIcon.sprite = foodSprites[key];
            }
            else
            {
                Debug.LogWarning($"Food sprite for '{key}' not found!");
                ui.foodIcon.sprite = null; // fallback
            }
            ui.quantityText.text = "x" + quantity.ToString();
            ui.foodNameText.text = foodName;

            ui.selectButton.onClick.RemoveAllListeners();
            ui.selectButton.onClick.AddListener(() =>
            {
                UIController.instance.SelectFood(foodName);
                PlayUIClick();
            });
        }
    }

    private Transform GetRealParent()
    {
        // If this parent has no LayoutGroup, check children for one
        if (availableFoodParent.GetComponent<LayoutGroup>() == null)
        {
            foreach (Transform child in availableFoodParent)
            {
                if (child.GetComponent<LayoutGroup>() != null)
                    return child; // This is the actual grid/content
            }
        }
        return availableFoodParent;
    }

    public void displaySelectedFood(string foodName, Sprite sprite)
    {
        
        if (selectedFoodIcon != null && sprite != null)
        {
            selectedFoodIcon.sprite = sprite;
            selectedFoodIcon.enabled = true; // show image
            selectedFoodIcon.color = Color.white; // ensure visible
        }
        else
        {
            Debug.LogWarning("SelectedFoodIcon or sprite is null!");
        }
    }

    public void clearSelectedFood()
    {
        selectedFoodIcon.sprite = null;
        selectedFoodIcon.color = new Color(1, 1, 1, 0); // fully transparent
    }

    public void resetFoodLocation()
    {
        UIController ui = UIController.instance;

        // return to original parent
        ui.draggableFood.SetParent(ui.originalParent);

        // reset anchored position
        ui.draggableFood.anchoredPosition = ui.originalAnchoredPosition;

        // ensure raycast works again
        ui.draggableFood.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void displayPetMessage(string message , bool status)
    {
        if (status)
        {

            petMessage.text = message;
        }
        else
        {
            petFoodMessage.text = message;
        }
    }



    //Display Kitchen Shop Product
    public void displayProduct()
    {
        coinValueInShop.text = stats.RetrieveValue("moneyValue").ToString();
        isShopOpen = !isShopOpen;
        ShopPage.SetActive(isShopOpen);
    }

    public void displayCart(Cart cart)
    {
        if (cart == null)
        {
            Debug.LogError("Cart is NULL in displayCart");
            return;
        }

        // Recalculate total
        cart.calculateTotalCost();
        coinValueInCart.text = stats.RetrieveValue("moneyValue").ToString();
        totalCostText.text = cart.totalCost.ToString();

        // Clear all existing rows
        foreach (Transform child in cartContentParent)
            Destroy(child.gameObject);

        if (cartItemPrefab == null)
        {
            Debug.LogError("cartItemPrefab is not assigned!");
            return;
        }

        // Rebuild rows from cart.items
        foreach (CartItem item in cart.items)
        {
            GameObject go = Instantiate(cartItemPrefab, cartContentParent);
            go.transform.localScale = Vector3.one;

            CartItemUI ui = go.GetComponent<CartItemUI>();
            if (ui == null)
            {
                Debug.LogError("CartItemUI component not found on prefab!");
                continue;
            }

            ui.priceText.text = item.price.ToString();
            ui.quantityText.text = item.quantity.ToString();
            ui.totalProductPrice.text = (item.quantity * item.price).ToString();
            ui.iconImage.sprite = item.icon;

            // VERY IMPORTANT: capture local copy for the lambda
            string thisItemName = item.itemName;

            // + button
            ui.addButton.onClick.RemoveAllListeners();
            ui.addButton.onClick.AddListener(() =>
            {
                cart.updateCart(thisItemName, +1);
                displayCart(cart);           // <-- refresh UI
                PlayUIClick();
            });
            

            // - button
            ui.minusButton.onClick.RemoveAllListeners();
            ui.minusButton.onClick.AddListener(() =>
            {
                cart.updateCart(thisItemName, -1);
                displayCart(cart);           // <-- refresh UI
                PlayUIClick();
            });



            // Remove button
            ui.removeButton.onClick.RemoveAllListeners();
            ui.removeButton.onClick.AddListener(() =>
            {
                cart.removeItem(thisItemName);
                displayCart(cart);           // <-- refresh UI
                PlayUIClick();
            });
        }
    }

    public void displayConfirmationPayment(int current, int payment)
    {
        currentMoneyText.text = current.ToString();
        paymentCostText.text = payment.ToString();
        remainMoneyText.text = (current - payment).ToString();
        openPaymentPage();
    }

    public void openPaymentPage()
    {
        isPaymentOpen = !isPaymentOpen;
        PaymentPage.SetActive(isPaymentOpen);
    }

    public void openCart(Cart cart)
    {
        isCartOpen = !isCartOpen;
        CartPage.SetActive(isCartOpen);

        displayCart(cart);
    }

    public void closeCartandShop()
    {
        ShopPage.SetActive(false);
        PaymentPage.SetActive(false);
        CartPage.SetActive(false);
    }


    public void displaySelectedGame()
    {
        PlayAndLoad("PlayBallScene");

    }

    public void gameBackScene()
    {
        // Force room index to Gameroom
        currentRoomIndex = 1; 

        if (GameRoomCameraPosition != null && MainCameraPosition != null)
        {
            // Snap the camera to Gameroom
            MainCameraPosition.position = new Vector3(
                GameRoomCameraPosition.position.x,
                GameRoomCameraPosition.position.y,
                -10f
            );
        }

        // Update title text
        UpdateRoomTitle();
    }
    public void displayMusicCategory()
    {
        Debug.Log("Button Clicked! Toggling Song Menu");
        SongMenuPanel.SetActive(!SongMenuPanel.activeSelf);
    }

    // Called by UIController when a category is chosen
    public void ShowMusicScreen()
    {
        SongMenuPanel.SetActive(false);

        if (HallPanel != null)
            HallPanel.SetActive(false);

        if (MusicScreenPanel != null)
            MusicScreenPanel.SetActive(true);

        if (lightToggleButton != null)
            lightToggleButton.gameObject.SetActive(false);
    }

    // Hook this to your "Back" button on the music screen
    public void HideMusicScreen()
    {
        if (MusicScreenPanel != null)
            MusicScreenPanel.SetActive(false);

        SongMenuPanel.SetActive(false);

        if (HallPanel != null)
            HallPanel.SetActive(true);

        if (lightToggleButton != null)
            lightToggleButton.gameObject.SetActive(true);

        // Tell controller to stop music and reset pet state
        if (UIController.instance != null)
        {
            UIController.instance.OnExitMusicScreen();
        }
    }

    public void OnClickMusicCategory(string categoryName)
    {
        // Open the music screen UI
        ShowMusicScreen();

        // Ask UIController to handle the actual music logic
        if (UIController.instance != null)
        {
            UIController.instance.RequestPlayCategorySong(categoryName);
        }
        else
        {
            Debug.LogWarning("UIController.instance not found in scene!");
        }
    }

    public void transitionToRight()
    {
        // If already moving, ignore extra clicks
        if (isMoving) return;

        StartCoroutine(backgroundTransition());

        Transform target = null;

        if (currentRoomIndex == 0)     
        {
            currentRoomIndex = 1;
            target = GameRoomCameraPosition;
        }
        else if (currentRoomIndex == 1) 
        {
            currentRoomIndex = 2;
            target = HallCameraPosition;
        }
        else if (currentRoomIndex == 2) 
        {
            currentRoomIndex = 3;
            target = KitchenCameraPosition;
        }
        else
        {

            StartCoroutine(EdgeShake());
            return;
        }

        UpdateRoomTitle();

        // Move camera smoothly to next room
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, -10);
        StartCoroutine(MoveCamera(targetPos));
    }

    public void transitionToLeft()
    {
        // If already moving, ignore extra clicks
        if (isMoving) return;

        StartCoroutine(backgroundTransition());

        Transform target = null;

        if (currentRoomIndex == 3)
        {
            currentRoomIndex = 2;
            target = HallCameraPosition;
        }
        else if (currentRoomIndex == 2)
        {
            currentRoomIndex = 1;
            target = GameRoomCameraPosition;
        }
        else if (currentRoomIndex == 1)
        {
            currentRoomIndex = 0;
            target = BathroomCameraPosition;
        }
        else
        {

            StartCoroutine(EdgeShake());
            return;
        }

        UpdateRoomTitle();

        // Move camera smoothly to next room
        Vector3 targetPos = new Vector3(target.position.x, target.position.y, -10);
        StartCoroutine(MoveCamera(targetPos));
    }

    private IEnumerator MoveCamera(Vector3 targetPos)
    {
        isMoving = true;

        Vector3 startPos = MainCameraPosition.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * TransitionSpeed;
            MainCameraPosition.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        MainCameraPosition.position = targetPos;
        isMoving = false;
    }

    public IEnumerator backgroundTransition()
    {
        transition.SetBool("Run", true);
        yield return new WaitForSeconds(0.4f);
        transition.SetBool("Run", false);
    }

    private IEnumerator EdgeShake()
    {
        Vector3 originalPos;
        if (currentRoomIndex == 0)
        {
            originalPos = BathroomCameraPosition.position;
        }
        else 
        {
            originalPos = KitchenCameraPosition.position; 
        }

        float duration = 0.2f;   // how long the shake lasts
        float magnitude = 0.2f;  // how strong the shake is
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // left-right only
            float offsetX = Mathf.Sin(elapsed * 50f) * magnitude;

            MainCameraPosition.position = new Vector3(
                originalPos.x + offsetX,
                originalPos.y,
                originalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        // reset back to original
        MainCameraPosition.position = originalPos;
    }
    private void UpdateRoomTitle()
    {
        switch (currentRoomIndex)
        {
            case 0:
                Title.text = $"Bathroom";
                break;
            case 1:
                Title.text = $"Game Room";
                break;
            case 2:
                Title.text = $"Hall";
                break;
            case 3:
                Title.text = $"Kitchen";
                break;
            case 4:
                Title.text = $"Medic";
                break;
        }
    }

    public void PlayAndLoad(string sceneName)
    {
        StartCoroutine(PlayTransitionAndLoad(sceneName));      
    }

    private IEnumerator PlayTransitionAndLoad(string sceneName)
    {
        // reset flags
        isBathing = false;
        PlayerPrefs.SetInt("IsBathing", 0);
        isDancing = false;
        PlayerPrefs.SetInt("IsDancing", 0);
        isEating = false;
        PlayerPrefs.SetInt("IsEating", 0);
        isAlbumOpen = false;
        PlayerPrefs.SetInt("IsAlbumOpen", 0);
        isSleeping = false;
        PlayerPrefs.SetInt("IsSleeping", 0);
        PlayerPrefs.Save();

        // play click sound if available (optional)
        if (audioClip != null && uiClickClip != null)
        {
            audioClip.PlayOneShot(uiClickClip);
            // small delay if you want to hear more of the sound
            yield return new WaitForSeconds(0.2f);
        }

        // play transition if Animator is valid
        if (transition != null && transition.runtimeAnimatorController != null)
        {
            transition.SetBool("Run", true);
            yield return new WaitForSeconds(0.4f);
        }   

        // now change scene
        SceneManager.LoadScene(sceneName);
    }
    public void ShowToast(string message, float duration = 1.5f)
    {
        // If a toast is already running, stop it and restart
        if (toastCoroutine != null)
        {
            StopCoroutine(toastCoroutine);
        }

        toastCoroutine = StartCoroutine(ShowToastRoutine(message, duration));
    }

    private IEnumerator ShowToastRoutine(string message, float duration)
    {
        if (toastPanel != null && toastText != null)
        {
            toastText.text = message;
            toastPanel.SetActive(true);

            yield return new WaitForSeconds(duration);

            toastPanel.SetActive(false);
        }

        toastCoroutine = null;
    }

    public void spawnBubbleOnPet()
    {

    }

    public void displayShower()
    {

    }

    public void displayCleanPet()
    {

    }

    public void resetShowerLocation()
    {

    }

    public void displayGameSelection()
    {

    }

}

