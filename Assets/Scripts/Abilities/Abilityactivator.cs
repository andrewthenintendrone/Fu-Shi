using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Abilityactivator : MonoBehaviour
{
 
    public float timeRadius;
    private RaycastHit2D[] hits = new RaycastHit2D[100];
    private ContactFilter2D filter;
    public GameObject inkwaveprefab;

    private bool inkHeld = false;
    private bool timeHeld = false;

	void Start ()
    {
        filter.layerMask = 1 << LayerMask.NameToLayer("Solid");
	}
	
	void Update ()
    {
        float inkAxis = Input.GetAxis("Ink");
        float timeAxis = Input.GetAxis("Time");

        if (inkAxis > 0.5f)
        {
            if(!inkHeld)
            {





                //create a gameobject InkWave
                GameObject CurrentInkwave = Instantiate(inkwaveprefab, transform.position, Quaternion.identity);

                //if player has R stick input use it
                //else use player facing
                Vector2 RstickDir = new Vector2(Input.GetAxis("RstickX"), Input.GetAxis("RstickY")).normalized;
                Vector2 LstickDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
                

                if (RstickDir.sqrMagnitude == 0 )
                {
                    if (LstickDir.sqrMagnitude == 0)
                    {
                        if (gameObject.GetComponent<Player>().facingRight)
                        {
                            CurrentInkwave.GetComponent<inkWave>().direction = Vector2.right;
                        }
                        else
                        {
                            CurrentInkwave.GetComponent<inkWave>().direction = Vector2.left;
                        }
                    }
                    else
                    {
                        CurrentInkwave.GetComponent<inkWave>().direction = LstickDir;
                    }
                }
                else
                {

                    CurrentInkwave.GetComponent<inkWave>().direction = RstickDir;

                }
                

            }
            inkHeld = true;
        }
        else
        {
            inkHeld = false;
        }
        if (timeAxis > 0.5f)
        {
            if(!timeHeld)
            {
                int numHits = Physics2D.CircleCast(transform.position, timeRadius, Vector2.zero, filter, hits, Mathf.Infinity);
                bool hasReversed = false;
                for (int i = 0; i < numHits && !hasReversed; i++)
                {
                    if (hits[i].collider.gameObject.GetComponent<patrolmove>() != null)
                    {
                        hits[i].collider.gameObject.GetComponent<patrolmove>().reverse();
                        hasReversed = true;
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
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, timeRadius);
    }
#endif
}
