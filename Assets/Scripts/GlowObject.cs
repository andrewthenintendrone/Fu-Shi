using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowObject : MonoBehaviour
{
    [SerializeField]
    [Tooltip("sprite without glow")]
    private Texture2D sprite1;

    [SerializeField]
    [Tooltip("sprite with glow")]
    private Texture2D sprite2;

    // amount of glow
    private float fade = 0.0f;

    [SerializeField]
    [Tooltip("how many seconds it takes to fade in")]
    private float fadeInSpeed;

    [SerializeField]
    [Tooltip("how many seconds it takes to fade out")]
    private float fadeOutSpeed;

    [HideInInspector]
    // set to true by the player when within range
    public bool nearPlayer = false;

    private ParticleSystem Particles = null;

    private void Start()
    {
        // set textures within the shader
        GetComponent<SpriteRenderer>().material.SetTexture("_MainTex", sprite1);
        GetComponent<SpriteRenderer>().material.SetTexture("_MainTex2", sprite2);

        // store reference to the particle system
        if (GetComponent<ParticleSystem>() != null)
        {
            Particles = GetComponent<ParticleSystem>();
        }
    }

    private void Update()
    {
        // increase or decrease fade over time based on whether the player is near
        if(nearPlayer)
        {
            // play particle effect if it exists
            if (Particles != null && !Particles.isPlaying)
            {
                Particles.Play();
            }

            fade = Mathf.Min(1.0f, fade + Time.deltaTime / fadeInSpeed);
        }
        else
        {
            fade = Mathf.Max(0.0f, fade - Time.deltaTime / fadeOutSpeed);
        }

        // set fade amount in the shader
        GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
    }
}
