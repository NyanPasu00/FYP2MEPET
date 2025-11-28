using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class StatusDetail : MonoBehaviour
{
    public GameObject statusPanel;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI statusTitle;
    public TextMeshProUGUI stageText;

    // These should reference your values from the other script
    public PetStatus stats;
    public SceneLoader loader;

    [SerializeField]
    private bool panelOpen = false;

    private void Start()
    {
        statusPanel.SetActive(false);
    }

    void Update()
    {
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
    }

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
        loader.playAudio();
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
}
