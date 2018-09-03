using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Tooltip("number of discrete hits it takes to kill the enemy")]
    public int health;

    public GameObject EnemyProjectile;
   

    [Tooltip("amount of damage contacting this enemy will do")]
    public int damageAmt;

    public bool shoot = false;
  

    // Use this for initialization
    void Start ()
    {
        
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


    void shootProjectile()
    {
        GameObject projectileInstance = Instantiate(EnemyProjectile, transform.position, Quaternion.identity);



    }

}
