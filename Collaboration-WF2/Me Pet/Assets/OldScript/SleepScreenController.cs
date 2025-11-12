using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LightToggle : MonoBehaviour
{
    public Button musicToggleButton;
    public GameObject HallLightScreen;
    public GameObject HallDarkScreen;
    public Animator petAnimator;
    public Energy_Bar energyBar;
    private Coroutine regenEnergyCoroutine;
    private Coroutine stopEnergyCoroutine = null;
    private bool isLightOn = true;
    public bool isSleeping = true;

    public AudioSource audio;

    public bool isWaitingToRegen = false;  // Prevents rapid restart of coroutine

    public void ToggleLight()
    {
        isLightOn = !isLightOn;

        HallLightScreen.SetActive(isLightOn);
        HallDarkScreen.SetActive(!isLightOn);

        if (isLightOn)
        {
            petAnimator.SetBool("Laydown", true);
            petAnimator.SetBool("Sleep", false);
            musicToggleButton.gameObject.SetActive(true);

            if (regenEnergyCoroutine != null && stopEnergyCoroutine == null)
            {
                stopEnergyCoroutine = StartCoroutine(DelayedStopEnergy());
            }

            isSleeping = false;
            PlayerPrefs.SetInt("IsSleeping", isSleeping ? 1 : 0);
            PlayerPrefs.Save();
            energyBar.ResumeEnergyDeduction();
        }
        else
        {
            petAnimator.SetBool("Laydown", false);
            petAnimator.SetBool("Sleep", true);
            musicToggleButton.gameObject.SetActive(false);

            if (!isWaitingToRegen && regenEnergyCoroutine == null)
            {
                regenEnergyCoroutine = StartCoroutine(DelayedRegenerateEnergy());
            }

            isSleeping = true;
            PlayerPrefs.SetInt("IsSleeping", isSleeping ? 1 : 0);
            PlayerPrefs.Save();
            energyBar.PauseEnergyDeduction();
        }
    }

    public void OpenBackLight()
    {
        if (isLightOn == false)
        {
            HallLightScreen.SetActive(isLightOn);
            HallDarkScreen.SetActive(!isLightOn);
        }
    }
    private IEnumerator DelayedRegenerateEnergy()
    {
        isWaitingToRegen = true;
        yield return new WaitForSeconds(1f); // Delay before starting regen to prevent spamming

        regenEnergyCoroutine = StartCoroutine(RegenerateEnergy());
        isWaitingToRegen = false;
        stopEnergyCoroutine = null;
    }

    private IEnumerator DelayedStopEnergy()
    {
        yield return new WaitForSeconds(1f);
        StopCoroutine(regenEnergyCoroutine);
        regenEnergyCoroutine = null;
        isWaitingToRegen = false;
    }

    private IEnumerator RegenerateEnergy()
    {
        while (energyBar.energy_current < energyBar.energy_max && !isLightOn)
        {
            energyBar.energy_current += 1;
            if (energyBar.energy_current > energyBar.energy_max)
                energyBar.energy_current = energyBar.energy_max;

            energyBar.energy_Slider.value = (float)energyBar.energy_current / energyBar.energy_max;
            energyBar.energyDetail_Slider.value = (float)energyBar.energy_current / energyBar.energy_max;

            yield return new WaitForSeconds(2f);
        }

        regenEnergyCoroutine = null;
    }

    public void playAudio()
    {
        audio.Play();
    }
}