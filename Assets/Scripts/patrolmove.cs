using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class patrolmove : MonoBehaviour
{
    public Transform[] patrolPoints = new Transform[0];
    [Tooltip("wether the unit will patrol or not")]
    public bool willPatrol = false;

   
    [Tooltip("speed of the unit as it moves on patrol")]
    public float moveSpd;
    [Tooltip("time unit waits at each patrol point position")]
    public float hangtime = 1.0f;
    [Tooltip ("defines how the platform will return to the start position, reversing or looping")]
    public bool willCycle = false;

    
    //default patrol point that object will move towards on start if moving
  
    public int currPatrolPoint = 0;
    //what point is the platform trying to reach
    //private int endpoint;
    private bool freeze = false;
    private float hangcount;
    private bool goingForward = true;
    // Use this for initialization
    void Start()
    {
        hangcount = hangtime;
        if (willPatrol && patrolPoints[0] == null)
        {
            
            Debug.Log(this.gameObject.name + " this object wants to move but has not enough patrol points");
        }

    }


    // Update is called once per frame
    void Update()
    {

        if (willPatrol && patrolPoints[0])
        {
            patrol();
        }
        
        
        countdown();
    }

    private void patrol()
    {

        Vector3 target;
        float distanceCutoff = 0.05f;

        target = patrolPoints[currPatrolPoint].position;



        //check distance to nearest patrol point
        float distCheck = (transform.position - target).magnitude;
        if (distCheck <= distanceCutoff)
        {
         
            //need it to pause here for a short delay

            freeze = true;
            

            // update target to next position

            findNextNode();

        }


        
        //move to next position
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

    [ContextMenu("reverse")]

    public void reverse()
    {
        goingForward = !goingForward;

        findNextNode();
    }

    void findNextNode()
    {

        if(willCycle)
        {
            if (goingForward)//looping down the list
            {
                if(currPatrolPoint == patrolPoints.Length - 1)
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
                if(currPatrolPoint == 0)
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
            if(goingForward)//not looping down the list
            {
                if(currPatrolPoint == patrolPoints.Length - 1)
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
                if(currPatrolPoint == 0)
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

}
