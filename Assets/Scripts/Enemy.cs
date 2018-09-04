using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
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

    [Tooltip("whether the enemy will shoot at the player")]
    public bool shoot = false;

    [Tooltip("how long between shots from this enemy")]
    public float shootInterval;


    void Start ()
    {
        InvokeRepeating("checkPlayerDist", shootInterval, shootInterval);
	}
	
	void Update ()
    {
        checkDead();
	}

    public void checkDead()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

   private void shootProjectile(Vector3 direction)
   {
        GameObject projectileInstance = Instantiate(EnemyProjectile, transform.position, Quaternion.identity);

        projectileInstance.GetComponent<enemyProjectile>().direction = direction.normalized;
   }

    private void checkPlayerDist()
    {
        if(gameObject == null)
        {
            CancelInvoke();
            return;
        }

        Vector3 direction = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;

        if (Vector3.SqrMagnitude(direction) <= Mathf.Pow(detectDistance, 2.0f))
        {
            shootProjectile(direction);
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.color = Color.red;

        if (drawDetectRadius)
        {
            UnityEditor.Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, detectDistance);
        }
    }

#endif
}
