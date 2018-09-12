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

    [Tooltip("this is the switch determining wether the player can use the ink spray")]
    public bool InkAbility = false;
    [Tooltip("this is the switch determining wether the player can use the time warp")]
    public bool timeAbility = false;

    private Material effectMaterial;

	void Start ()
    {
        filter.layerMask = 1 << LayerMask.NameToLayer("Solid");
        if(Camera.main.gameObject.GetComponent<PostProcessing>() != null)
        {
            effectMaterial = Camera.main.gameObject.GetComponent<PostProcessing>().effectMaterial;
        }
    }
	
	void Update ()
    {
        float inkAxis = Input.GetAxis("Ink");
        float timeAxis = Input.GetAxis("Time");

        if (inkAxis > 0.5f)
        {
            if (!inkHeld)
            {
                if (InkAbility)
                {


                    //create a gameobject InkWave
                    GameObject CurrentInkwave = Instantiate(inkwaveprefab, transform.position + new Vector3(0, 0.7f), Quaternion.identity);

                    //if player has R stick input use it
                    //else use player facing
                    Vector2 RstickDir = new Vector2(Input.GetAxis("RstickX"), Input.GetAxis("RstickY")).normalized;
                    Vector2 LstickDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

                    if (RstickDir.sqrMagnitude == 0)
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
            }
            inkHeld = true;
        }
        else
        {
            inkHeld = false;
        }

        if (timeAxis > 0.5f)
        {
            if (timeAbility)
            {
                if (!timeHeld)
                {
                    int numHits = Physics2D.CircleCast(transform.position, timeRadius, Vector2.zero, new ContactFilter2D(), hits, Mathf.Infinity);
                    bool hasReversed = false;
                    for (int i = 0; i < numHits && !hasReversed; i++)
                    {
                        if (hits[i].collider.gameObject.GetComponentInParent<patrolmove>() != null)
                        {
                            hits[i].collider.gameObject.GetComponentInParent<patrolmove>().reverse();
                            hasReversed = true;
                        }
                        if (hits[i].collider.gameObject.GetComponentInParent<enemyProjectile>() != null)
                        {
                            hits[i].collider.gameObject.GetComponentInParent<enemyProjectile>().Reverse();
                            hasReversed = true;
                        }
                        if (hits[i].collider.gameObject.GetComponent<Door>() != null)
                        {
                            if (hits[i].collider.gameObject.GetComponent<Door>().hasBeenOpened)
                            {
                                hits[i].collider.gameObject.GetComponent<Door>().isOpen = true;
                                hits[i].collider.gameObject.GetComponent<Door>().stuckOpen = true;
                                hasReversed = true;
                            }
                        }
                    }
                }
                timeHeld = true;
            }
        }
        else
        {
            timeHeld = false;
        }

        // set _TimeWarpRadius in the shader
        if(effectMaterial != null)
        {
            if(timeAxis > 0.5f)
            {
                effectMaterial.SetFloat("_TimeWarpRadius", 1.0f / 16.0f);
            }
            else
            {
                effectMaterial.SetFloat("_TimeWarpRadius", 0.0f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "inkGiver")
        {
            InkAbility = true;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.name == "timeGiver")
        {
            timeAbility = true;
            Destroy(collision.gameObject);
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
