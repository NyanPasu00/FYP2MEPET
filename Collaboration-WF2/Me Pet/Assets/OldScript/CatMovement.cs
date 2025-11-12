using System.Collections;
using UnityEngine;

public class CatMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float moveDirection = 0f;
    private SpriteRenderer cat;
    private Animator catanim;

    private void Start()
    {
        // Get SpriteRenderer so we can flip the sprite
        cat = GetComponent<SpriteRenderer>();
        catanim = GetComponent<Animator>();
    }

    void Update()
    {
        transform.Translate(Vector2.right * moveDirection * moveSpeed * Time.deltaTime);
    }

    public void MoveLeft()
    {
        moveDirection = -1f;
        if (cat != null)
            cat.flipX = false;
        catanim.SetBool("Catch", false);

    }

    public void MoveRight()
    {
        moveDirection = 1f;
        if (cat != null)
            cat.flipX = true;
        catanim.SetBool("Catch", false);
    }

    public void StopMoving()
    {
        moveDirection = 0f;
    }

    public void PlayCatchAnimation()
    {
        StartCoroutine(CatchRoutine());
    }

    private IEnumerator CatchRoutine()
    {
        catanim.SetBool("Catch", true);
        yield return new WaitForSeconds(0.333f); // Animation duration
        catanim.SetBool("Catch", false);
    }
}
