using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
public class GameUI : MonoBehaviour
{
    public GameObject statusPanel;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI statusTitle;
    public TextMeshProUGUI stageText;

    public Energy_Bar stats;

    [SerializeField]
    private bool panelOpen = false;

    void Start()
    {
        statusPanel.SetActive(false);
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
}
