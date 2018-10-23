using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationInkBlot : MonoBehaviour {

    [SerializeField]
    private Animator animator;

    public void PlayAnimation()
    {
        animator.SetTrigger("Leaf");
    }

}
