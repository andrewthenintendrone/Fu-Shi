﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Tooltip("number of discrete hits it takes to kill the enemy")]
    public int health;

    public Transform[] patrolPoints = new Transform[2];
    [Tooltip("wether the unit will patrol or not")]
    public bool willPatrol;

    [Tooltip("amount of damage contacting this enemy will do")]
    public int damageAmt;
    [Tooltip("speed of the unit as it moves on patrol")]
    public float moveSpd;
    [Tooltip("time unit waits at each patrol point position")]
    public float hangtime = 1.0f;

    //default patrol point that enemy will move towards on start if moving
    private int currPatrolPoint = 0;
    private bool freeze = false;
    private float hangcount;

    // Use this for initialization
    void Start ()
    {
        if (health == 0)
        {
            health = 3;
        }
        hangcount = hangtime;
	}
	
	// Update is called once per frame
	void Update ()
    {
        patrol();
        countdown();
	}

    private void patrol()
    {
        
        Vector3 target;
        float distanceCutoff = 0.05f;
        if (currPatrolPoint == 0 )
        {
            target = patrolPoints[0].position;
        }
        else
        {
            target = patrolPoints[1].position;
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
