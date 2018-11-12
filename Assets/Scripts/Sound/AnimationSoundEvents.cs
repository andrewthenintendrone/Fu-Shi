using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundEvents : MonoBehaviour {

    private AudioClip[] walkSounds;
    private AudioClip[] jumpSounds;
    private AudioClip deathSound;

    [SerializeField]
    private AudioClip jumpPiece;

   
    void Start ()
    {
        walkSounds = Resources.LoadAll<AudioClip>("walk");
        jumpSounds = Resources.LoadAll<AudioClip>("jump");
        deathSound = Resources.Load<AudioClip>("FoxDeath");
	}
	
    public void playWalkSound()
    {
        SoundManager.instance.PlayRandomSFX(walkSounds);
    }

    public void playJumpSound()
    {
        SoundManager.instance.PlayRandomSFX(jumpSounds);
    }

    public void playDeathSound()
    {
        SoundManager.instance.playSingle(deathSound);
    }
}
