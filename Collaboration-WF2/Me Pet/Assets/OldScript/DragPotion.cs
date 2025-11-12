using UnityEngine;
using System.Collections;

public class DragPotion : MonoBehaviour
{
    private Vector3 offset;
    private bool dragging = false;
    private Vector3 originalPosition;
    private bool isDrinking = false;
    private bool isOverMouth = false;

    public bool isEating;

    public Animator catAnimator; 


    public GameObject mouthPointObject;
    public GameObject MedicationDialogue;
    public GameObject HealthFullDialogue;
    public GameObject FeedSuccessDialogue;
    public Energy_Bar energyBarScript;

    public AudioSource eatingSound;
    public AudioSource audioSource;
    public AudioClip healthWarningClip;
    private bool hasPlayedHealthSound = false;
    void Start()
    {

        originalPosition = transform.position;

        if (mouthPointObject == null)
        {
            mouthPointObject = GameObject.Find("MouthPoint");
        }

        if (energyBarScript == null)
        {
            energyBarScript = GameObject.Find("EnergyBarManager").GetComponent<Energy_Bar>(); // replace with actual GameObject name
        }
    }

    void Update()
    {
        if (energyBarScript != null)
        {
            if (energyBarScript.health_current < 30)
            {
                if(!MedicationDialogue.activeSelf)
                {
                    MedicationDialogue.SetActive(true);
                }

                if(!hasPlayedHealthSound && audioSource != null && healthWarningClip != null)
                {
                    audioSource.PlayOneShot(healthWarningClip);
                    hasPlayedHealthSound = true;
                }
            }
            else 
            {
                if (MedicationDialogue.activeSelf)
                {
                    MedicationDialogue.SetActive(false);
                }
                hasPlayedHealthSound = false;
            }
            if (energyBarScript.health_current < 100)
            {
                if (HealthFullDialogue != null && HealthFullDialogue.activeSelf)
                {
                    HealthFullDialogue.SetActive(false);
                }
            }

        }
    }

    void OnMouseDown()
    {
        if (!isDrinking)
        {
            dragging = true;
            offset = transform.position - GetMouseWorldPos();
        }
    }

    void OnMouseDrag()
    {
        if (dragging)
        {
            Vector3 targetPos = GetMouseWorldPos() + offset;
            transform.position = new Vector3(targetPos.x, targetPos.y, -1f); // bring to front
        }
    }

    void OnMouseUp()
    {
        if (!isDrinking)
        {
            dragging = false;

            if (isOverMouth)
            {
                if (energyBarScript.health_current >= 100)
                {                  
                    HealthFullDialogue.SetActive(true);
                    // Return potion to original position
                    transform.position = originalPosition;
                }
                else
                {
                    isEating = true;
                    PlayerPrefs.SetInt("IsEating", isEating ? 1 : 0);
                    PlayerPrefs.Save();
                    StartCoroutine(DrinkPotion());
                }
            }
            else
            {
                transform.position = originalPosition;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("CatMouth"))
        {
            isOverMouth = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("CatMouth"))
        {
            isOverMouth = false;
        }
    }

    IEnumerator DrinkPotion()
    {
        isDrinking = true;

        // Snap potion to center of mouth
        if (mouthPointObject != null)
        {
            transform.position = mouthPointObject.transform.position;
        }

        // Start animation
        if (catAnimator != null)
            catAnimator.SetBool("Eating", true);
        if (eatingSound != null)
            eatingSound.Play();

        // Wait 2 seconds visually at mouth
        yield return new WaitForSeconds(1f);

        // Increase health by 10%
        energyBarScript.IncreaseHealth();

        // Return potion to original position
        transform.position = originalPosition;

        yield return new WaitForSeconds(0.4f);


        if (eatingSound != null)
            eatingSound.Stop();
        // Stop animation
        if (catAnimator != null)
            catAnimator.SetBool("Eating", false);

        if (FeedSuccessDialogue != null)
            FeedSuccessDialogue.SetActive(true);
        
        yield return new WaitForSeconds(1f);

        if (FeedSuccessDialogue != null)
            FeedSuccessDialogue.SetActive(false);
        isDrinking = false;
        isEating = false;
        PlayerPrefs.SetInt("IsEating", isEating ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void EndEatingAnimation()
    {
        catAnimator.SetBool("Eating", false);
        catAnimator.SetBool("CatIdleEatingPose", true);

    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
