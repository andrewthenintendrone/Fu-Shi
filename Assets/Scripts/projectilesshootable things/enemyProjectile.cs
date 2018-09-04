using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyProjectile : MonoBehaviour
{
    public float speed;

    [HideInInspector]
    public Vector3 direction;

    public float lifetime;


	void Start ()
    {
        Destroy(gameObject, lifetime);
	}

    void FixedUpdate()
    {
        gameObject.transform.Translate(direction * speed * Time.fixedDeltaTime);
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            Utils.Health = Mathf.Max(Utils.Health - 1, 0);
        }
        if (collision.gameObject.GetComponent<inkWave>() == null && collision.gameObject.GetComponent<enemyProjectile>() == null)
        {
            Destroy(gameObject);
        }
    }
}
