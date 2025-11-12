using UnityEngine;
using System.Collections;

public class HappinessManager : MonoBehaviour
{
    public Energy_Bar happinessBar;
    private Coroutine regenHappinessCoroutine;

    public void StartHappinessRegen()
    {
        if (regenHappinessCoroutine != null)
        {
            StopCoroutine(regenHappinessCoroutine);
            regenHappinessCoroutine = null;
        }

        regenHappinessCoroutine = StartCoroutine(RegenerateHappiness());
    }

    private IEnumerator RegenerateHappiness()
    {
        Debug.Log("Happiness Regen Started!");

        while (happinessBar.happiness_current < happinessBar.happiness_max)
        {
            happinessBar.happiness_current += 1;
            yield return new WaitForSeconds(2f);
        }

        Debug.Log("Happiness Regen Done!");
    }
}
