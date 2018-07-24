using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Tooltip("number of discrete hits it takes to kill the enemy")]
    public int health;

   

    [Tooltip("amount of damage contacting this enemy will do")]
    public int damageAmt;
   

  

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
        checkDead();
        
	}

    public void checkDead()
    {
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }


}
