using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Abilityactivator : MonoBehaviour
{
    [Tooltip("range of the time reverse ability")]
    [SerializeField]
    private float timeRadius;

    // list of the objects that have currently been hit by the time reverse ability
    private RaycastHit2D[] hits = new RaycastHit2D[100];

    private ContactFilter2D filter;

    [Tooltip("ink wave prefab gameobject")]
    [SerializeField]
    private GameObject inkSlashPrefab;

    // is the ink axis held
    private bool inkHeld = false;

    // is the time axis held
    private bool timeHeld = false;

    [Tooltip("this is the switch determining whether the player can use the ink spray ability")]
    public bool hasInkAbility = false;

    [Tooltip("this is the switch determining whether the player can use the time reverse ability")]
    public bool hasTimeAbility = false;

    // reference to the post processing effect
    private Material effectMaterial;
    [SerializeField]
    [Tooltip("the distance away from the fox's centre that the ink ability starts")]
    private float offsetdistance = 0f;

	void Start ()
    {
        // only hit solid objects with the time reverse ability
        filter.layerMask = 1 << LayerMask.NameToLayer("Solid");

        // get reference to screen space effect
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
                if (hasInkAbility)
                {
                    //create a gameobject InkWave
                    GameObject CurrentInkwave = Instantiate(inkSlashPrefab, transform.position + new Vector3(0, 0.7f), Quaternion.identity);

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
                                CurrentInkwave.GetComponent<Inkmeleeslash>().direction = Vector2.right;
                            }
                            else
                            {
                                CurrentInkwave.GetComponent<Inkmeleeslash>().direction = Vector2.left;
                            }

                            // ternary
                            // CurrentInkwave.GetComponent<Inkmeleeslash>().direction = (gameObject.GetComponent<Player>().facingRight ? Vector2.right : Vector2.left);
                        }
                        else
                        {
                            CurrentInkwave.GetComponent<Inkmeleeslash>().direction = LstickDir;
                        }
                    }
                    else
                    {
                        CurrentInkwave.GetComponent<Inkmeleeslash>().direction = RstickDir;
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
            if (hasTimeAbility)
            {
                if (!timeHeld)
                {
                    int numHits = Physics2D.CircleCast(transform.position, timeRadius, Vector2.zero, new ContactFilter2D(), hits, Mathf.Infinity);
                    for (int i = 0; i < numHits; i++)
                    {
                        if (hits[i].collider.gameObject.GetComponentInParent<patrolmove>() != null)
                        {
                            hits[i].collider.gameObject.GetComponentInParent<patrolmove>().reverse();
                        }
                        if (hits[i].collider.gameObject.GetComponentInParent<enemyProjectile>() != null)
                        {
                            hits[i].collider.gameObject.GetComponentInParent<enemyProjectile>().Reverse();
                        }
                        if (hits[i].collider.gameObject.GetComponent<Door>() != null)
                        {
                            if (hits[i].collider.gameObject.GetComponent<Door>().hasBeenOpened)
                            {
                                hits[i].collider.gameObject.GetComponent<Door>().isOpen = true;
                                hits[i].collider.gameObject.GetComponent<Door>().stuckOpen = true;
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
            if(timeAxis > 0.5f && hasTimeAbility)
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
            hasInkAbility = true;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.name == "timeGiver")
        {
            hasTimeAbility = true;
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
