using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Player))]
public class AnimationController : MonoBehaviour
{
    //private Animator animator;
    //private Player player;

	// Use this for initialization
	void Start ()
    {
        //animator = GetComponent<Animator>();
        //player = GetComponent<Player>();
	}
	
	// FixedUpdate is called once per physics step
	void FixedUpdate ()
    {
        statecheck();
	}

    // update the animation state based on the AnimationState enum in Player
    void statecheck()
    {
        //animator.SetInteger("state", (int)player.animationState);

        //if(player.animationState == Player.AnimationState.DASH)
        //{
        //    player.animationState = Player.AnimationState.IDLE;
        //    animator.SetTrigger("dash");
        //}
    }
}
