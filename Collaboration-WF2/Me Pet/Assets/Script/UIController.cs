using NUnit.Framework.Interfaces;
using System.Collections;
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
    void Start()
    {
        dragCanvasGroup = draggableFood.GetComponent<CanvasGroup>();
        originalPos = draggableFood.anchoredPosition;
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


}
