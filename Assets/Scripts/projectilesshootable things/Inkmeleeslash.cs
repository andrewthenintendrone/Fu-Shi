using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inkmeleeslash : MonoBehaviour {

    [SerializeField]
    private GameObject inkbulletprefab;
    [HideInInspector]
    public Vector2 direction;
	//spawn the smaller inkprojectiles here
	void Start ()
    {
       GameObject inkbullet1 = Instantiate(inkbulletprefab, transform.position, transform.rotation);
        inkbullet1.GetComponent<inkBullet>().direction = transform.forward;
	}
	
	
	void Update ()
    {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // check if object has inkable surface
        if (collision.gameObject.GetComponentInChildren<inkableSurface>() != null)
        {
            collision.gameObject.GetComponentInChildren<inkableSurface>().Inked = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            collision.gameObject.GetComponent<Enemy>().health--;
        }
    }
}
