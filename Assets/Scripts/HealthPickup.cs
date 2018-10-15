using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField]
    [Tooltip("how fast the health pickup moves towards the player")]
    private float moveSpeed;

    [SerializeField]
    [Tooltip("how close to the player the health pickup has to be to start moving")]
    private float movementDistance;
	
	void Update()
    {
        Vector3 playerOffset = Utils.getPlayer().transform.position - transform.position;

        if(Vector3.Magnitude(playerOffset) <= movementDistance)
        {
            transform.Translate(playerOffset.normalized * moveSpeed * Time.deltaTime);
        }
	}
}
