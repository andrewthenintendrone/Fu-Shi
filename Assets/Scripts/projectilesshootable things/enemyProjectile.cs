using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyProjectile : MonoBehaviour {

    public float speed;

    [HideInInspector]
    public Vector3 direction;

    public float lifetime;

    private float countdown;


	// Use this for initialization
	void Start ()
    {
        countdown = lifetime;
        Destroy(gameObject, lifetime);
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.Translate(direction * speed * Time.fixedDeltaTime);
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            Utils.Health--;
        }
        if (collision.gameObject.GetComponent<inkWave>() != null)
        {

        }
        else
        {
            Destroy(gameObject);
        }
        
    }
}
