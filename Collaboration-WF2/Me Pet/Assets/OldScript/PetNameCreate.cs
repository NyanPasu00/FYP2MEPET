using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PetNameManager : MonoBehaviour
{
    public TMP_InputField nameInputField; // or use InputField if you're not using TextMeshPro

    public void SavePetNameAndContinue()
    {
        string petName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(petName))
        {
            Debug.LogWarning("Pet name is empty!");
            return;
        }

        PlayerPrefs.SetString("PetName", petName); // Save pet name
        PlayerPrefs.Save();

        // Load your main game scene
        SceneManager.LoadScene("HallScene");
    }
}

