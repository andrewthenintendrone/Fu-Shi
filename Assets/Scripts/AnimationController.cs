using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Player))]
public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private Player player;

	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
	}
	
	// FixedUpdate is called once per physics step
	void FixedUpdate ()
    {
        statecheck();
	}

    // check state and set up animator to match
    void statecheck()
    {
        animator.SetBool("run", (Mathf.Abs(player.velocity.x) > 0.01f));
    }
}
