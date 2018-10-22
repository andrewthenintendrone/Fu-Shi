using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundEvents : MonoBehaviour {

    private AudioClip[] walkSounds;
    private AudioClip[] jumpSounds;
    

    [SerializeField]
    private AudioClip jumpPiece;

    // Use this for initialization
    void Start ()
    {
        walkSounds = Resources.LoadAll<AudioClip>("walk");
        jumpSounds = Resources.LoadAll<AudioClip>("jump");
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void playWalkSound()
    {
        SoundManager.instance.PlayRandomSFX(walkSounds);
    }

    public void playJumpSound()
    {
        SoundManager.instance.PlayRandomSFX(jumpSounds);
    }
}
