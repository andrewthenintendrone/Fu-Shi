using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patrolmove : MonoBehaviour
{
    [SerializeField]
    private Transform[] patrolPoints = new Transform[0];

    [SerializeField]
    [Tooltip("wether the unit will patrol or not")]
    private bool willPatrol = false;

    [SerializeField]
    [Tooltip("speed of the unit as it moves on patrol")]
    private float moveSpd;

    [SerializeField]
    [Tooltip("time unit waits at each patrol point position")]
    private float hangtime = 1.0f;

    [SerializeField]
    [Tooltip("defines how the platform will return to the start position, reversing or looping")]
    private bool willCycle = false;

    [SerializeField]
    [Tooltip("default patrol point that object will move towards on start if moving")]
    private int currPatrolPoint = 0;

    [SerializeField]
    private int prevPatrolPoint;

    //what point is the platform trying to reach
    //private int endpoint;
    private bool freeze = false;
    private float hangcount;
    private bool goingForward = true;

    private float t = 0.0f;

    float InOutQuadBlend(float t)
    {
        if (t <= 0.5f)
        {
            return 2.0f * Mathf.Pow(t, 2.0f);
        }
        t -= 0.5f;
        return 2.0f * t * (1.0f - t) + 0.5f;
    }

    // Use this for initialization
    void Start()
    {
        hangcount = hangtime;
        if (willPatrol == true && patrolPoints.Length < 2)
        {
            Debug.Log(this.gameObject.name + " this object wants to move but has not enough patrol points");
            willPatrol = false;
        }

        if (willPatrol)
        {
            findNextNode();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!Utils.gamePaused)
        {
            if (willPatrol)
            {
                patrol();
            }


            countdown();
        }
    }

    private void patrol()
    {

        //check distance to nearest patrol point
        if (t >= 1.0f)
        {

            //need it to pause here for a short delay

            freeze = true;


            // update target to next position

            findNextNode();

            t = 0;
        }



        //move to next position
        if (!freeze)
        {
            float distance = Vector3.Distance(patrolPoints[prevPatrolPoint].position, patrolPoints[currPatrolPoint].position);

            t += 1.0f / (distance / moveSpd) * Time.fixedDeltaTime;

            transform.position = Vector3.Lerp(patrolPoints[prevPatrolPoint].position, patrolPoints[currPatrolPoint].position, InOutQuadBlend(t));
        }



    }

    private void countdown()
    {
        if (freeze)
        {
            hangcount -= Time.fixedDeltaTime;
        }
        if (hangcount <= 0)
        {
            freeze = false;
            hangcount = hangtime;
        }
    }

    [ContextMenu("reverse")]

    public void reverse()
    {
        if (transform.parent == null)
        {
            hangcount = hangtime - hangcount;

            goingForward = !goingForward;

            findNextNode();

            t = 1.0f - t;

            flash(Color.blue);
        }
        else
        {
            foreach (Transform Child in transform.parent)
            {
                if (Child.gameObject.GetComponent<patrolmove>() != null && Child.gameObject.GetComponent<patrolmove>().willPatrol)
                {
                    patrolmove currScript = Child.gameObject.GetComponent<patrolmove>();
                    currScript.hangcount = currScript.hangtime - currScript.hangcount;
                    currScript.goingForward = !currScript.goingForward;
                    currScript.findNextNode();

                    currScript.flash(Color.blue);
                }

            }
        }
    }

    void findNextNode()
    {
        prevPatrolPoint = currPatrolPoint;

        if (willCycle)
        {
            if (goingForward)//looping down the list
            {
                if (currPatrolPoint == patrolPoints.Length - 1)
                {
                    currPatrolPoint = 0;
                }
                else
                {
                    currPatrolPoint++;
                }
            }
            else//looping up the list
            {
                if (currPatrolPoint == 0)
                {
                    currPatrolPoint = patrolPoints.Length - 1;
                }
                else
                {
                    currPatrolPoint--;
                }
            }
        }
        else
        {
            if (goingForward)//not looping down the list
            {
                if (currPatrolPoint == patrolPoints.Length - 1)
                {
                    goingForward = !goingForward;
                    currPatrolPoint--;
                }
                else
                {
                    currPatrolPoint++;
                }
            }
            else//not looping up the list
            {
                if (currPatrolPoint == 0)
                {
                    goingForward = !goingForward;
                    currPatrolPoint++;
                }
                else
                {
                    currPatrolPoint--;
                }
            }
        }
    }

    private void flash(Color color)
    {
        GetComponentInChildren<Renderer>().material.color = color;

        Invoke("endFlash", 0.5f);
    }

    private void endFlash()
    {
        GetComponentInChildren<Renderer>().material.color = Color.white;
    }

    void FindPrevNode()
    {
        if (goingForward)
        {
            if (currPatrolPoint == 0)
            {
                if (willCycle)
                {
                    prevPatrolPoint = patrolPoints.Length - 1;
                }
                else
                {
                    prevPatrolPoint = 1;
                }
            }
            else
            {
                prevPatrolPoint = currPatrolPoint - 1;
            }
        }
        else
        {
            if (currPatrolPoint == patrolPoints.Length - 1)
            {
                if (willCycle)
                {
                    prevPatrolPoint = 0;
                }
                else
                {
                    prevPatrolPoint = patrolPoints.Length - 2;
                }
            }
            else
            {
                prevPatrolPoint = currPatrolPoint + 1;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        if (patrolPoints.Length > 1 && willPatrol)
        {
            for (int i = 1; i < patrolPoints.Length; i++)
            {
                Gizmos.DrawLine(patrolPoints[i - 1].position, patrolPoints[i].position);
            }
            if (willCycle)
            {
                Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1].position, patrolPoints[0].position);
            }
        }
    }
}
