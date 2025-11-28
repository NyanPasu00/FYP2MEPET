using UnityEngine;



public class DragObject : MonoBehaviour

{

    private Vector3 offset;

    private bool dragging = false;



    private Vector3 originalPosition; // 🆕 To store the original starting position



    void Start()

    {

        // Save the object's original position when the scene starts

        originalPosition = transform.position;

    }



    void OnMouseDown()

    {

        Debug.Log("Dragging Start");

        dragging = true;

        offset = transform.position - GetMouseWorldPos();

    }



    void OnMouseDrag()

    {

        if (dragging)

        {

            transform.position = GetMouseWorldPos() + offset;

        }

    }



    void OnMouseUp()

    {

        dragging = false;



        // 🆕 Move the object back to its original position when drag ends

        transform.position = originalPosition;

    }



    Vector3 GetMouseWorldPos()

    {

        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = Camera.main.WorldToScreenPoint(transform.position).z;

        return Camera.main.ScreenToWorldPoint(mousePoint);

    }

}