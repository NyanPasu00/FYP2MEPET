using System.Collections;
using UnityEngine;
using TMPro;

public class MoneyPopup : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float floatDistance = 50f;
    public float duration = 1f;

    private void Awake()
    {
        if (text == null)
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (text == null)
        {
            Debug.LogError("MoneyPopup: No TextMeshProUGUI found. Please assign it in the prefab.");
        }
    }

    public void Init(int amount)
    {
        if (text == null)
        {
            Debug.LogError("MoneyPopup: text is null in Init().");
            return;
        }

        text.text = $"+{amount} Coins";
        text.fontSize = 60;              // big and obvious
        text.color = Color.yellow;       // bright
        Debug.Log("MoneyPopup.Init called: " + text.text);

        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        if (text == null)
            yield break;

        RectTransform rt = (RectTransform)transform;
        Vector2 startPos = rt.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0f, floatDistance);

        Color startColor = text.color;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, lerp);

            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, lerp);
            text.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }
}
