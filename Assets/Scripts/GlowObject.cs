using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowObject : MonoBehaviour
{
    [SerializeField]
    private Texture2D sprite1;

    [SerializeField]
    private Texture2D sprite2;

    private float fade = 0.0f;

    [SerializeField]
    [Tooltip("how many seconds it takes to fade in")]
    private float fadeInSpeed;

    [SerializeField]
    [Tooltip("how many seconds it takes to fade out")]
    private float fadeOutSpeed;

    [HideInInspector]
    public bool nearPlayer = false;

    private ParticleSystem Particles;

    private void Start()
    {
        GetComponent<SpriteRenderer>().material.SetTexture("_MainTex", sprite1);
        GetComponent<SpriteRenderer>().material.SetTexture("_MainTex2", sprite2);
        Particles = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if(nearPlayer)
        {
            if (!Particles.isPlaying)
            {
                Particles.Play();
            }

            fade = Mathf.Min(1.0f, fade + Time.deltaTime / fadeInSpeed);
            
        }
        else
        {
            fade = Mathf.Max(0.0f, fade - Time.deltaTime / fadeOutSpeed);
        }

        GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
    }
}
