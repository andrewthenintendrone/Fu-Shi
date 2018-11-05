using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inkBullet: MonoBehaviour
{
    [HideInInspector]
    public Vector2 direction;

    public float speed;

    public float lifetime;

    private void Start()
    {
        float rotation = Vector2.SignedAngle(Vector2.left, direction);
        transform.eulerAngles = Vector3.forward * rotation;
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if(!Utils.gamePaused)
        {
            Vector3 traveldir = new Vector3(direction.x, direction.y) * speed * Time.fixedDeltaTime;

            // travel along the direction vector at a speed float
            transform.Translate(traveldir, Space.World);
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // check if object has inkable surface
        if (collision.gameObject.GetComponentInChildren<inkableSurface>() != null)
        {
            collision.gameObject.GetComponentInChildren<inkableSurface>().Inked = true;
        }

        if (collision.gameObject.GetComponent<enemyProjectile>() == null && collision.gameObject.GetComponent<inkBullet>() == null && collision.gameObject.GetComponent<Player>() == null
            && collision.gameObject.GetComponent<Inkmeleeslash>() == null)
        {
        
         Destroy(gameObject);
          
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            collision.gameObject.GetComponent<Enemy>().health--;
            Destroy(gameObject);
        }

        // check if object has inkable surface
        if (collision.gameObject.GetComponentInChildren<inkableSurface>() != null)
        {
            collision.gameObject.GetComponentInChildren<inkableSurface>().Inked = true;
        }
    }
}
