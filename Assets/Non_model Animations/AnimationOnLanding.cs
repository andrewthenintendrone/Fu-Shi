using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOnLanding : MonoBehaviour
{

    [SerializeField]
    private Animator animator;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.position.y >= GetComponent<Collider2D>().bounds.center.y)
        {
            animator.SetTrigger("JumpedOn");
        }
    }
}
