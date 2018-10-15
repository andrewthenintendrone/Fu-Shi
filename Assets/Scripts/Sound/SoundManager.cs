using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    [Tooltip("the source for playing effects on the player")]
    public static AudioSource efxSource;   
    [Tooltip("the source for playing music")]
    public static AudioSource MusicSource;
    [Tooltip("lowest pitch the effects will be modulated to")]
    [SerializeField]
    private static float lowPitchRange = .95f;
    [Tooltip("highest pitch the effects will be modulated to")]
    [SerializeField]
    private static float highPitchRange = 1.05f;
    [SerializeField]
    private static float MusicLoopPoint = 5.142f;
    
    public static bool playMusic = false;

    public static AudioClip music;



    private static void Init()
    {

        efxSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[0];
        MusicSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponents<AudioSource>()[1];

        if (!MusicSource.isPlaying && playMusic)
        {
            MusicSource.Play();
        }

    }

    //play one audioclip at normal pitch once
    public static void playSingle(AudioClip clip)
    {
        efxSource.clip = clip;

        efxSource.Play();
    }

    public static void PlayRandomSFX(params AudioClip[] clips)
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

    public static void Update()
    {

        if(!MusicSource.isPlaying && playMusic)
        {
            MusicSource.time = MusicLoopPoint;
            MusicSource.Play();
        }
    }



}
