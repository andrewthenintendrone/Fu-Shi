using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildtoTrigger : MonoBehaviour
{
    public bool insideWall = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !insideWall)
        {
            other.gameObject.transform.parent = transform;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Rigidbody2D>() != null && other.gameObject.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Kinematic)
        {
            insideWall = true;
        }

        if (other.gameObject.tag == "Player" && !insideWall)
        {
            other.gameObject.transform.parent = transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Rigidbody2D>() != null && other.gameObject.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Kinematic)
        {
            insideWall = false;
        }
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.parent = null;
        }
    }
}
