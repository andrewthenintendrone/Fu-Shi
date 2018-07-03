using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Tooltip("number of discrete hits it takes to kill the enemy")]
    public int health;

    public Vector3[] patrolPoints = new Vector3[2];
    [Tooltip("wether the unit will patrol or not")]
    public bool willPatrol;

    [Tooltip("amount of damage contacting this enemy will do")]
    public int damageAmt;
    [Tooltip("speed of the unit as it moves on patrol")]
    public float moveSpd;


	// Use this for initialization
	void Start ()
    {
        if (health == 0)
        {
            health = 3;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		

	}

    private void patrol()
    {
        //do you have a patrol point selected?


        //if yes follow it


        //else pick the closest one


    }
}
