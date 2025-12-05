using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class DraggableFood : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;
    public Animator catAnimator; 

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector3 originalPosition;
    public GameObject platePosition;
    public GameObject FoodFullDialog;
    public GameObject FeedSuccessDialogue;
    public PetStatus energyBarScript;
    public AudioSource eatingSound;
    private Transform originalParent;
    private Vector2 originalAnchoredPosition;

    public bool isEating;


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        originalParent = transform.parent;
        originalAnchoredPosition = rectTransform.anchoredPosition;
    }



    void Awake()
    {
        originalParent = transform.parent;

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        GameObject hovered = eventData.pointerCurrentRaycast.gameObject;

        if (hovered != null && hovered.CompareTag("Plate"))
        {
            if (energyBarScript.hunger_current >= 100)
            {
                FoodFullDialog.SetActive(true);
                // Return potion to original position
                transform.position = originalPosition;
            }
            else
            {
                isEating = true;
                PlayerPrefs.SetInt("IsEating", isEating ? 1 : 0);
                PlayerPrefs.Save();
                // Temporarily set parent to plate
                transform.SetParent(hovered.transform);
                rectTransform.anchoredPosition = new Vector2(0, 35);


                // Start eating process
                StartCoroutine(HandleEating());
                return;
            }
        }

        // Invalid drop — reset
        ResetToOriginal();
    }

    private void ResetToOriginal()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalAnchoredPosition;
    }


    IEnumerator HandleEating()
    {
        canvasGroup.blocksRaycasts = false;

        // Start animation
        if (catAnimator != null)
            catAnimator.SetBool("Eating", true);

        if (eatingSound != null)
            eatingSound.Play();

        // Stay on plate
        yield return new WaitForSeconds(0.5f);

        // Increase food level
        if (energyBarScript != null)
            //energyBarScript.IncreaseFood();

        yield return new WaitForSeconds(0.5f);

        if (eatingSound != null)
            eatingSound.Stop();

        // Reset food
        ResetToOriginal();

        // Re-enable dragging
        canvasGroup.blocksRaycasts = true;

        // Stop animation
        if (catAnimator != null)
            catAnimator.SetBool("Eating", false);

        if (FeedSuccessDialogue != null)
            FeedSuccessDialogue.SetActive(true);

        yield return new WaitForSeconds(1f);

        if (FeedSuccessDialogue != null)
            FeedSuccessDialogue.SetActive(false);

        isEating = false;
        PlayerPrefs.SetInt("IsEating", isEating ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void EndEatingAnimation()
    {
        catAnimator.SetBool("Eating", false);
        catAnimator.SetBool("CatIdleEatingPose", true);

    }

}
