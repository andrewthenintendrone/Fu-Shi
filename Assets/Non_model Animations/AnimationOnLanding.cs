using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOnLanding : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    private void OnCollisionEnter2D(Collision2D collision)
    {
       //Debug.Log("stepped");
        animator.SetTrigger("JumpedOn");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        animator.SetTrigger("JumpedOn");
    }
}
