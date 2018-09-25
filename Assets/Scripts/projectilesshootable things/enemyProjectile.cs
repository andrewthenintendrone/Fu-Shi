using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyProjectile : MonoBehaviour
{
    public float speed;

    [HideInInspector]
    public Vector3 direction;

    public float lifetime;

    public bool reversed = false;


	void Start ()
    {
        Destroy(gameObject, lifetime);
	}

    void FixedUpdate()
    {
        if(!Utils.gamePaused)
        {
            gameObject.transform.Translate(direction * speed * Time.fixedDeltaTime);
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Utils.Health = Mathf.Max(Utils.Health - 1, 0);
        }
        if (collision.gameObject.GetComponent<inkBullet>() == null && collision.gameObject.GetComponent<enemyProjectile>() == null && collision.gameObject.GetComponent<Inkmeleeslash>() == null)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() != null && reversed)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            enemy.health = Mathf.Max(enemy.health - 1, 0);
            enemy.checkDead();
            Destroy(gameObject);
        }
    }

    public void Reverse()
    {
        if(!reversed)
        {
            reversed = true;
            direction = -direction;
        }
    }
}
