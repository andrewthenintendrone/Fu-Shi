using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Tooltip("the source for playing effects on the player")]
    private AudioSource efxSource;   
    [Tooltip("the source for playing music")]
    private AudioSource MusicSource;
    [Tooltip("lowest pitch the effects will be modulated to")]
    [SerializeField]
    private float lowPitchRange = .95f;
    [Tooltip("highest pitch the effects will be modulated to")]

    public static SoundManager instance = null;

    [SerializeField]
    private float highPitchRange = 1.05f;
    [SerializeField]
    private float MusicLoopPoint = 5.142f;
    
    public bool playMusic = false;

    public AudioClip music;



    public void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad(gameObject);

        efxSource = gameObject.GetComponents<AudioSource>()[0];
        MusicSource = gameObject.GetComponents<AudioSource>()[1];

        if (!MusicSource.isPlaying && playMusic)
        {
            MusicSource.Play();
        }

    }

    //play one audioclip at normal pitch once
    public void playSingle(AudioClip clip)
    {
        efxSource.clip = clip;

        efxSource.Play();
    }

    public void PlayRandomSFX(params AudioClip[] clips)
    {

        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        efxSource.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        efxSource.clip = clips[randomIndex];

        //Play the clip.
        efxSource.Play();
    }

    public void Update()
    {

        if(!MusicSource.isPlaying && playMusic)
        {
            MusicSource.time = MusicLoopPoint;
            MusicSource.Play();
        }
    }



}
