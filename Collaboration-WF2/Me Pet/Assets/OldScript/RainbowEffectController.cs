using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RainbowLightController : MonoBehaviour
{
    public Light2D light2D; // Assign your Light2D here
    public float colorChangeSpeed = 1f;

    private void Update()
    {
        float hue = Mathf.PingPong(Time.time * colorChangeSpeed, 1f);
        Color rainbowColor = Color.HSVToRGB(hue, 1f, 1f);
        light2D.color = rainbowColor;
    }
}
