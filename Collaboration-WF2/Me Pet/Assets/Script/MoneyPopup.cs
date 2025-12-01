using System.Collections;
using UnityEngine;
using TMPro;

public class MoneyPopup : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float floatDistance = 50f;   // in UI pixels
    public float duration = 1f;

    private RectTransform rt;
    private Vector2 startPos;
    private float timer;

    private void Awake()
    {
        rt = transform as RectTransform;

        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>();

        if (text == null)
            Debug.LogError("MoneyPopup: No TextMeshProUGUI found. Please assign it in the prefab.");
    }

    // STATIC CREATE – uses prefab + parent + position
    public static MoneyPopup Create(MoneyPopup prefab, Transform parent, Vector3 screenPosition, int amount)
    {
        // spawn under the canvas
        MoneyPopup popup = Instantiate(prefab, parent);

        RectTransform popupRT = popup.transform as RectTransform;
        popupRT.position = screenPosition;   // same position as coin icon

        popup.Setup(amount);
        return popup;
    }

    public void Setup(int moneyAmount)
    {
        if (text != null)
            text.SetText("+" + moneyAmount.ToString() + " Coins");

        // reset animation state
        rt = rt ?? (transform as RectTransform);
        startPos = rt.anchoredPosition;
        timer = 0f;
    }

    private void Update()
    {
        if (rt == null) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);

        // move up in UI space
        rt.anchoredPosition = startPos + new Vector2(0, floatDistance * t);

        // fade / destroy at end (optional)
        if (t >= 1f)
            Destroy(gameObject);
    }
}
