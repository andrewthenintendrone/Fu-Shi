using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patrolmove : MonoBehaviour
{
    //public Transform[] patrolPoints = new Transform[2];
    public List<Vector2> patrolPoints = new List<Vector2>();
    //[Tooltip("wether the unit will patrol or not")]
    //public bool willPatrol = false;
    private bool willPatrol;
   
    [Tooltip("speed of the unit as it moves on patrol")]
    public float moveSpd;
    [Tooltip("time unit waits at each patrol point position")]
    public float hangtime = 1.0f;

    //default patrol point that enemy will move towards on start if moving
    private int currPatrolPoint = 0;
    private bool freeze = false;
    private float hangcount;

    // Use this for initialization
    void Start()
    {
        hangcount = hangtime;

        // attempt to find patrol points (parented to the same object as this)
        foreach (Transform currentTransform in transform.parent.GetComponentsInChildren<Transform>())
        {
            // the platform itself is not a waypoint add it to the list
            if(currentTransform != transform.parent && currentTransform != this.transform && currentTransform.parent != this.transform)
            {
                patrolPoints.Add(currentTransform.position);
            }
        }

        // the object will patrol if there are patrol points
        willPatrol = (patrolPoints.Count > 0);

        if (!willPatrol)
        {
            Debug.Log(this.gameObject.name + " this object wants to move but has not enough patrol points");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (willPatrol)
        {
            patrol();
        }
        
        
        countdown();
    }

    private void patrol()
    {
        Vector3 target;
        float distanceCutoff = 0.05f;

        if (currPatrolPoint == 0)
        {
            target = patrolPoints[0];
        }
        else
        {
            target = patrolPoints[1];
        }

        //check distance to nearest patrol point
        float distCheck = (transform.position - target).magnitude;
        if (distCheck <= distanceCutoff)
        {
            //need it to pause here for a short delay

            freeze = true;

            if (currPatrolPoint == 0)
            {
                currPatrolPoint = 1;
            }
            else
            {
                currPatrolPoint = 0;
            }
        }



        //if within threshold change target point to other one

        if (!freeze)
        {
            float step = moveSpd * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }



    }

    private void countdown()
    {
        if (freeze)
        {
            hangcount -= 1 * Time.deltaTime;
        }
        if (hangcount <= 0)
        {
            freeze = false;
            hangcount = hangtime;
        }
    }
}
