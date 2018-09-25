using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("number of discrete hits it takes to kill the enemy")]
    [SerializeField]
    public int maxHealth;

    [HideInInspector]
    // current health
    public int health;

    [Tooltip("prefab of the projectile or bullet to shoot at the player goes here")]
    public GameObject EnemyProjectile;

    [Tooltip("squared distance to check around the enemy for the player")]
    public float detectDistance;

    [Tooltip("debugging / utility to show the range of detection")]
    public bool drawDetectRadius = false;

    [Tooltip("whether the enemy will shoot at the player")]
    public bool shoot = false;

    [Tooltip("how long between shots from this enemy")]
    public float shootInterval;

    [SerializeField]
    [Tooltip("how far away from the enemy the projectile starts at")]
    private float projectileStartDistance;


    void Start ()
    {
        health = maxHealth;
        if(shoot)
        {
            InvokeRepeating("checkPlayerDist", shootInterval, shootInterval);
        }
	}
	
	void Update ()
    {
        if(!Utils.gamePaused)
        {
            checkDead();
        }
	}

    public void checkDead()
    {
        if(!Utils.gamePaused)
        {
            if (health <= 0)
            {
                gameObject.GetComponent<Renderer>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().enabled = false;
                this.enabled = false;
                CancelInvoke("checkPlayerDist");
            }
        }
    }

   private void shootProjectile(Vector3 direction)
   {
        GameObject projectileInstance = Instantiate(EnemyProjectile, transform.position + direction * projectileStartDistance, Quaternion.identity);

        projectileInstance.GetComponent<enemyProjectile>().direction = direction;
   }

    private void checkPlayerDist()
    {
        if(!Utils.gamePaused)
        {
            if (!gameObject.activeSelf)
            {
                CancelInvoke();
                return;
            }

            Vector3 direction = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>().bounds.center - transform.position;

            if (Vector3.SqrMagnitude(direction) <= Mathf.Pow(detectDistance, 2.0f))
            {
                shootProjectile(direction.normalized);
            }
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (drawDetectRadius)
        {
            // draw detect radius
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, detectDistance);

            // draw projectile start distance
            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, projectileStartDistance);
        }
    }

#endif
}
