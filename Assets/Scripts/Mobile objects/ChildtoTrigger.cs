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

        if (other.gameObject.tag == "Player")
        {
            if (!insideWall)
            {
                other.gameObject.transform.parent = transform;
            }

            Vector3 fakeMotion = Vector3.zero;

            if (GetComponentInParent<patrolmove>().getNextPatrolPoint().x > transform.position.x)
            {
                fakeMotion.x = Mathf.Epsilon;
            }
            else
            {
                fakeMotion.x = -Mathf.Epsilon;
            }

            other.gameObject.GetComponent<CharacterController2D>().move(fakeMotion);
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
