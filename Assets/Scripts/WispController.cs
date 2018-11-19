using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WispController : MonoBehaviour
{
    #region variables

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

    [SerializeField]
    [Tooltip("the particle effect that appears on death")]
    private GameObject deathParticles;

    #endregion

    public void Update()
    {
        float sqrDistToPlayer = Vector2.SqrMagnitude(Utils.getPlayer().transform.position - transform.position);

        if(sqrDistToPlayer <= Mathf.Pow(activationDist, 2.0f))
        {
            if (!isActive)
            {
                isActive = true;
                SoundManager.instance.playwispLaugh();
            }
        }

        if (sqrDistToPlayer <= Mathf.Pow(lootRadius, 2.0f))
        {
            if (!lootActive)
            {
                Utils.showNotification("I have your soul!", "Press B to Continue");
                SoundManager.instance.playwispLaugh();
                lootActive = true;
            }
        }

        if (Loot != null)
        {
            Loot.SetActive(lootActive);
        }

        if (!Utils.gamePaused)
        {
            patrol();
        }
    }

    private void patrol()
    {
        if (isActive)
        {
            Vector3 target = patrolPoints[currPatrolPoint].position;

            // check distance to nearest patrol point
            float distCheck = Vector2.SqrMagnitude(transform.position - target);

            if (distCheck <= Mathf.Pow(distanceCutoff, 2.0f))
            {
                // update target to next position
                findNextNode();
            }

            // move to next position
            float step = moveSpd * Time.fixedDeltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }
    }

    
    void findNextNode()
    {       
        // run to the end of its path
        if (currPatrolPoint == patrolPoints.Length - 1)
        {
            hasPlayed = true;
            Instantiate(deathParticles, transform.position, Quaternion.identity);
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
