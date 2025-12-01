using UnityEngine;
using UnityEngine.UI;

public class FridgeClick : MonoBehaviour
{
    public GameUI gameUI;

    void OnMouseDown()
    {
        gameUI.openFridge();
        gameUI.PlayUIClick();
    }
}
