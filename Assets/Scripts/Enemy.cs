using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [Tooltip("number of discrete hits it takes to kill the enemy")]
    public int health;

    [Tooltip("prefab of the projectile or bullet to shoot at the player goes here")]
    public GameObject EnemyProjectile;

    [Tooltip("squared distance to check around the enemy for the player")]
    public float detectDistance;
    [Tooltip("debugging / utility to show the range of detection")]
    public bool drawDetectRadius = false;

    [Tooltip("amount of damage contacting this enemy will do")]
    public int damageAmt;

    [Tooltip("wether the enemy will shoot at the player")]
    public bool shoot = false;


    [Tooltip("how long between shots from this enemy")]
    public float shootInterval;


    private float countdown;
    // Use this for initialization
    void Start ()
    {
        countdown = shootInterval;
	}
	
	// Update is called once per frame
	void Update ()
    {
        checkDead();

        if (countdown <= 0)
        {
            checkPlayerDist();
            countdown = shootInterval;
        }

        countdown -= Time.deltaTime;
	}

    public void checkDead()
    {
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }


   private void shootProjectile(Vector3 target)
   {
        GameObject projectileInstance = Instantiate(EnemyProjectile, transform.position, Quaternion.identity);

        target -= gameObject.transform.position;
        projectileInstance.GetComponent<enemyProjectile>().direction = target;

   }

    private void checkPlayerDist()
    {
        Vector3 targetpos = GameObject.FindGameObjectWithTag("Player").transform.position;


        if (Vector3.SqrMagnitude(targetpos - gameObject.transform.position) <= Mathf.Pow(detectDistance, 2.0f))
        {
            shootProjectile(targetpos);
        }
        
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        UnityEditor.Handles.color = Color.red;
        if (drawDetectRadius)
        {
            UnityEditor.Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, detectDistance);
        }
        
    }
#endif
}
