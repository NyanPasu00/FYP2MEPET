using System.Collections;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
public class GameUI : MonoBehaviour
{
    [SerializeField]
    [Header("Status Panel")]
    public GameObject statusPanel;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI statusTitle;
    public TextMeshProUGUI stageText;

    private bool panelOpen = false;

    public Energy_Bar stats;

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

    private int currentRoomIndex = 2;
    private bool isMoving = false;


    void Start()
    {
        statusPanel.SetActive(false);

        MainCameraPosition.position = new Vector3(HallCameraPosition.position.x, HallCameraPosition.position.y, -10);
        currentRoomIndex = 2;
    }

    // Update is called once per frame
    void Update()
    {
        //------------------------------------------------------------------STATUS PANEL START-------------------------------------------------------------------
        // Only update if panel is open
        if (panelOpen == true)
        {
            UpdateStatusText();

            // Hide if clicking outside
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIElement())
            {
                ClosePanel();
            }
        }
        //-------------------------------------------------------------------STATUS PANEL END--------------------------------------------------------------------
    }

    //--------------------------------------------------------------------STATUS PANEL START-------------------------------------------------------------------
    public void ToggleStatusPanel()
    {
        Debug.Log("Correct");
        panelOpen = !panelOpen;
        statusPanel.SetActive(panelOpen);
        if (panelOpen == true)
        {
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

        string petName = PlayerPrefs.GetString("PetName", "Pet");

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

    }

    public void displayNewGame()
    {

    }

    public void displayAvailableFood()
    {

    }

    public void displaySelectedFood()
    {

    }

    public void displayPetMessage(string message)
    {

    }

    public void resetFoodLocation()
    {

    }

    public void resetMedicineLocation()
    {

    }

    public void displayProduct()
    {

    }

    public void displayConfirmationPayment()
    {

    }

    public void displayGameSelection()
    {

    }

    public void displaySelectedGame()
    {

    }

    public void displayMusicCategory()
    {

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

    public void displayCart()
    {

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
        }
    }
}
