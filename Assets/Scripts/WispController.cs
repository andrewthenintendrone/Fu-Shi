using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispController : MonoBehaviour
{
    [SerializeField]
    private Transform[] patrolPoints = new Transform[0];

    [SerializeField]
    [Tooltip("speed of the unit as it moves on patrol")]
    private float moveSpd;

    [SerializeField]
    [Tooltip("default patrol point that object will move towards on start if moving")]
    private int currPatrolPoint = 0;

    [SerializeField]
    private float activationDist = 3.0f;

    [SerializeField]
    private float lootRadius;

    [SerializeField]
    private bool lootActive;

    [SerializeField]
    private bool isActive = false;

    [HideInInspector]
    public bool hasPlayed = false;

    [SerializeField]
    private GameObject Loot;

    [SerializeField]
    private float distanceCutoff;



    public void Update()
    {
        if(Vector3.SqrMagnitude(Utils.getPlayer().transform.position - transform.position) <= Mathf.Pow(activationDist, 2.0f))
        {
            isActive = true;
        }

        if (Vector3.SqrMagnitude(Utils.getPlayer().transform.position - transform.position) <= Mathf.Pow(lootRadius, 2.0f))
        {
            if (!lootActive)
            {
                Utils.showNotification("I have your soul!", "Press B to Continue");
            }

            lootActive = true;
        }

        if (Loot != null)
        {
            
            if (lootActive)
            {
                Loot.SetActive(true);
            }
            else
            {
                Loot.SetActive(false);
            }

        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Utils.gamePaused)
        {
            patrol();
        }
    }

    private void patrol()
    {

        Vector3 target;

        target = patrolPoints[currPatrolPoint].position;



        //check distance to nearest patrol point
        float distCheck = (transform.position - target).magnitude;
        if (distCheck <= distanceCutoff)
        {
            
            // update target to next position

            findNextNode();

        }
        

        //move to next position
            if (isActive)
            {
                float step = moveSpd * Time.fixedDeltaTime;
                transform.position = Vector3.MoveTowards(transform.position, target, step);
            }
        
    }

    
    void findNextNode()
    {
            if (currPatrolPoint == patrolPoints.Length - 1)
            {
                hasPlayed = true;
                gameObject.SetActive(false);
            }
            else
            {
                currPatrolPoint++;
            }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        if (patrolPoints.Length > 1)
        {
            for (int i = 1; i < patrolPoints.Length; i++)
            {
                Gizmos.DrawLine(patrolPoints[i - 1].position, patrolPoints[i].position);
            }
           
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        
        
        // draw detect radius
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, activationDist);

        if (lootRadius > 0)
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, lootRadius);
        }

    }

#endif

}
