using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInkBlot : MonoBehaviour {

 
    private Animator animator;

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        animator.SetTrigger("Leaf");
    }

}
