using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Player))]
public class AnimationController : MonoBehaviour {

    

    Animator animator;
    Player player;

	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        statecheck();
	}

    void statecheck()
    {
        animator.SetInteger("state", (int)player.animationState);

        if(player.animationState == Player.AnimationState.DASH)
        {
            player.animationState = Player.AnimationState.IDLE;
            animator.SetTrigger("dash");
            Debug.Log("dash triggered");
        }
    }

}
