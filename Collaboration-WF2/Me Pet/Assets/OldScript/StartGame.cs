using UnityEngine;
using UnityEngine.SceneManagement; // very important

public class StartGame : MonoBehaviour
{
    public PetStatus energyBarCheck;
    public GameObject EnergyNotEnough;

    void Start()
    {
        EnergyNotEnough.SetActive(false);
    }
    public void LoadPlayBallScene()
    {
        if (energyBarCheck.energy_current <= 30)
        {
            EnergyNotEnough.SetActive(true);
        }
        else
        {
            energyBarCheck.GamePlayEnergyNeed();
            energyBarCheck.happiness_current += 30;
            if(energyBarCheck.happiness_current >= 100)
            {
                energyBarCheck.happiness_current = 100;
            }
            energyBarCheck.SavePetData();
            FindFirstObjectByType<BGMScript>().StopMusic();
            FindFirstObjectByType<SceneLoader>().LoadPlayBallScene();
        }
    }
}
