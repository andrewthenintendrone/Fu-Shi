using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class DisableObjectOnContact : MonoBehaviour
{
    [SerializeField]
    private GameObject wisp;
    [SerializeField]
    private GameObject wispPatrolPoints;

   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == wisp)
        {
            wisp.SetActive(false);
            wispPatrolPoints.SetActive(false);
        }
        
    }

}
