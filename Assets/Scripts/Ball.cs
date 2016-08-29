using UnityEngine;
using System.Collections;

public class Ball : Photon.PunBehaviour
{
    public float speed = 40f;

    public GameController gc;

    void Start()
    {
        // Offline movement
        float r = Random.value;

        if (r >= 0.5f)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
        }
    }

    void FixedUpdate()
    {
        if (gc.paused)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    public void StartBall()
    {
        transform.position = new Vector2(0, 0);

        float r = Random.value;

        if (r >= 0.5f)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.right * speed;
        }
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.left * speed;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("LeftRacket"))
        {
            float y = hitFactor(transform.position,
                                col.transform.position,
                                col.collider.bounds.size.y);

            Vector2 dir = new Vector2(1, y).normalized;

            GetComponent<Rigidbody2D>().velocity = dir * speed;
        }

        if (col.gameObject.CompareTag("RightRacket"))
        {
            float y = hitFactor(transform.position,
                                col.transform.position,
                                col.collider.bounds.size.y);

            Vector2 dir = new Vector2(-1, y).normalized;

            GetComponent<Rigidbody2D>().velocity = dir * speed;
        }

        if (col.gameObject.CompareTag("LeftWall"))
        {
            gc.scoreRight();
            transform.position = new Vector2(0, 0);
        }

        if (col.gameObject.CompareTag("RightWall"))
        {
            gc.scoreLeft();
            transform.position = new Vector2(0, 0);
        }
    }

    float hitFactor(Vector2 ballPos, Vector2 racketPos, float racketHeight)
    {
        return (ballPos.y - racketPos.y) / racketHeight;
    }
}
