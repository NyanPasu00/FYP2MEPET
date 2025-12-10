using fbg;
using System.Collections;
using TMPro;
using UnityEngine;

public class MainUI : MonoBehaviour
{
    public GameObject connectionPanel;
    public TMP_Text connectionMessage;

    public void displayConnectionError(string message)
    {
        connectionPanel.SetActive(true);
        connectionMessage.text = message;

        StartCoroutine(HideConnectionMessage());
    }

    private IEnumerator HideConnectionMessage()
    {
        yield return new WaitForSeconds(2f);
        connectionPanel.SetActive(false);
    }
}
