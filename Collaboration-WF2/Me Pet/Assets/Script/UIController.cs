using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class UIController : MonoBehaviour
{
    //public PetStatus petStatus;
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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    }

    public void onDrag()
    {

    }

    public void onEndDrag()
    {

    }

    public void isOnPlate()
    {

    }

    public void isHunger()
    {

    }

    public void isHealth()
    {

    }

    public void addItemToCart()
    {

    }

    public void removeItemFromCart()
    {

    }

    public void modifyItemQuantity()
    {

    }

    public void requestCheckout()
    {

    }

    public void isValidateMoney()
    {

    }

    public void paymentConfirm()
    {

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
