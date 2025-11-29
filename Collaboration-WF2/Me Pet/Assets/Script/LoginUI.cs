using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    private void OnEnable()
    {
        //LoginManager.OnLoginSuccess += LoadGameUI;
    }

    private void OnDisable()
    {
        //LoginManager.OnLoginSuccess -= LoadGameUI;
    }

    private void LoadGameUI()
    {
        Debug.Log("Login success → Returning to StartScene");
        SceneManager.LoadScene("StartScene");
    }

    public void OnLoginDone()
    {
        SceneManager.LoadScene("KidScene");
    }
}
