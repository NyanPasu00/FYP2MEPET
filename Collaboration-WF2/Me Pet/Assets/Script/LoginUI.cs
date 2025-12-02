using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{

    private void LoadGameUI()
    {
        Debug.Log("Login success → Returning to StartScene");
        SceneManager.LoadScene("StartScene");
    }

    public void OnLoginDone()
    {
        SceneManager.LoadScene("KidScene");
    }

    public void displayLoginMessage(string message)
    {
        
    }
}
