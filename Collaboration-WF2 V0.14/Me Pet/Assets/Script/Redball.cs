using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class RedBall : MonoBehaviour
{
    public int pointValue = 10;
    public Animator cat;
    
    void Start()
{
    if (cat == null)
        cat = GameObject.FindGameObjectWithTag("Cat").GetComponent<Animator>();
}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Cat"))
        {

            if (MiniGameController.instance != null)
            {
                MiniGameController.instance.AddScore(pointValue);
                MiniGameController.instance.PlayCatchAnimation();
            }

            Destroy(gameObject);
            Debug.Log("Cat collided with a ball!");
        }

        if (collision.gameObject.CompareTag("Floor"))
        {
            if (MiniGameController.instance != null)
            {
                MiniGameController.instance.MissScore();
            }

            Destroy(gameObject);
            Debug.Log("Ball Miss!");
        }
    }
}
