using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundEvents : MonoBehaviour {

    [SerializeField]
    private AudioClip walkPiece1;
    [SerializeField]
    private AudioClip walkPiece2;
    [SerializeField]
    private AudioClip walkPiece3;
    [SerializeField]
    private AudioClip walkPiece4;

    [SerializeField]
    private AudioClip jumpPiece;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void playWalkSound()
    {
        if (walkPiece1 != null)
        {
            SoundManager.instance.PlayRandomSFX(walkPiece1, walkPiece2,walkPiece3,walkPiece4);
        }
        
    }

    public void playJumpSound()
    {
        SoundManager.instance.PlayRandomSFX(jumpPiece);
    }
}
