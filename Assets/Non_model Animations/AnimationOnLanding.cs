using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOnLanding : MonoBehaviour
{
    private Animator animator;

    private void OnAwake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.position.y >= GetComponent<Collider2D>().bounds.center.y && collision.gameObject == Utils.getPlayer())
        {
            animator.SetTrigger("JumpedOn");
        }
    }
}
