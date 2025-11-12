using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ClearDigitalAlbumManager : MonoBehaviour
{
    

    public void ClearDigitalAlbum()
    {
        PlayerPrefs.DeleteKey("DeadPets");
        PlayerPrefs.DeleteKey("PetAlbumData");
        PlayerPrefs.Save();
    }

}