using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildtoTrigger : MonoBehaviour
{

	// Use this for initialization
	void Start ()
    {
		
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.parent = transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.parent = null;
        }
    }


}
