using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using TMPro;
using System.Collections;
public class LoginUI : MonoBehaviour
{

    public GameObject loginSuccessObject;
    public GameObject loginErrorObject;
    public TMP_Text loginSuccessText;
    public TMP_Text loginErrorText;
    public GameUI gameUI;
    private void LoadGameUI()
    {
        Debug.Log("Login success → Returning to StartScene");
        SceneManager.LoadScene("StartScene");
    }

    public void displayLoginMessage(string message , bool status)
    {
        
        if (status)
        {
            Debug.Log("Display Message Please");
            loginSuccessText.text = message;

            loginSuccessObject.SetActive(true);
        }else
        {
            loginErrorText.text = message;
            loginErrorObject.SetActive(true);

            StartCoroutine(HideLoginErrorAfterDelay());
        }
       
        

    }

    public void displayLoginPage()
    {
        gameUI.PlayAndLoad("LoginScene");
    }

    private IEnumerator HideLoginErrorAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        loginErrorObject.SetActive(false);
    }
}
