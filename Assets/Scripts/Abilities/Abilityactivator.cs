using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Abilityactivator : MonoBehaviour
{
    public float inkRadius;
    public float timeRadius;
    private RaycastHit2D[] hits = new RaycastHit2D[100];
    private ContactFilter2D filter;

    bool inkHeld = false;
    bool timeHeld = false;

	void Start ()
    {
        filter.layerMask = 1 << LayerMask.NameToLayer("Solid");
	}
	
	void Update ()
    {
        float inkAxis = Input.GetAxis("Ink");
        float timeAxis = Input.GetAxis("Time");

        if (inkAxis != 0)
        {
            if(!inkHeld)
            {
                int numHits = Physics2D.CircleCast(transform.position, inkRadius, Vector2.zero, filter, hits, Mathf.Infinity);

                for (int i = 0; i < numHits; i++)
                {
                    if (hits[i].collider.gameObject.GetComponent<inkableSurface>() != null)
                    {
                        hits[i].collider.gameObject.GetComponent<inkableSurface>().Inked = true;
                    }
                }
            }
            inkHeld = true;
        }
        else
        {
            inkHeld = false;
        }
        if (timeAxis != 0)
        {
            if(!timeHeld)
            {
                int numHits = Physics2D.CircleCast(transform.position, timeRadius, Vector2.zero, filter, hits, Mathf.Infinity);

                for (int i = 0; i < numHits; i++)
                {
                    if (hits[i].collider.gameObject.GetComponent<patrolmove>() != null)
                    {
                        hits[i].collider.gameObject.GetComponent<patrolmove>().reverse();
                    }
                }
            }
            timeHeld = true;
        }
        else
        {
            timeHeld = false;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, inkRadius);
    }
#endif
}
