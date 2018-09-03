using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyProjectile : MonoBehaviour {

    public float speed;

    [HideInInspector]
    Vector2 direction;

    public float lifetime;

    private float countdown;


	// Use this for initialization
	void Start ()
    {
        countdown = lifetime;
	}

    private void Update()
    {
        if (countdown <= 0)
        {
            Destroy(gameObject);
        }
        countdown -= Time.deltaTime;

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.Translate(new Vector3(direction.x, direction.y) * speed * Time.fixedDeltaTime);
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            Utils.Health--;
        }
    }
}
