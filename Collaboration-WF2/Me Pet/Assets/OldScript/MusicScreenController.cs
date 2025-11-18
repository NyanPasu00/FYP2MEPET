using UnityEngine;

public class SongCategoryButton : MonoBehaviour
{
    [Tooltip("Rock, Rap, Reggae, Country, Jazz, Metal")]
    public string categoryName;

    public void OnClick()
    {
        if (UIController.instance != null)
        {
            //UIController.instance.OnClickMusicCategory(categoryName);
        }
        else
        {
            Debug.LogWarning("UIController.instance not found in scene!");
        }
    }
}