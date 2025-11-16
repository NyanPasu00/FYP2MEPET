using UnityEngine;
using TMPro;

public class WaterDrop : MonoBehaviour
{
    public BathController bathController;
    public Transform waterDropStartPos;
    public TMP_Text messageText;
    public GameObject cloud;

    [HideInInspector] public Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Pet"))
            return;

        // Make sure we have a BathController
        if (bathController == null)
        {
            bathController = FindAnyObjectByType<BathController>();
            if (bathController == null)
            {
                Debug.LogError("WaterDrop: No BathController found in scene!");
                return;
            }
        }

        // Too clean -> block shower
        if (bathController.dirty <= 20f)
        {
            bathController.ShowCloudMessage("I still not so dirty yet >_<", 2f);
            return;
        }

        // No soap yet -> remind player
        if (!bathController.hasUsedSoap)
        {
            bathController.ShowCloudMessage("Oops! I look so dirty. Try using soap!", 2f);
            return;
        }

        // Soap was used — allow shower
        Debug.Log("Water drop touched the pet, starting shower!");

        // Tell BathController which pet / drop we are using
        bathController.petTarget = other.transform;
        bathController.waterDrop = gameObject;
        bathController.waterDropStartPos = waterDropStartPos;

        // Move the shower object above the pet
        bathController.transform.position = other.transform.position + new Vector3(0f, 1f, 0f);

        gameObject.transform.position = waterDropStartPos.position;

        // Start shower
        bathController.StartShower();

   
    }

    // Optional: if you still want to clear text from this script
    void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }
}


