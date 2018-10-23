using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField]
    private Texture2D sprite1;

    [SerializeField]
    private Texture2D sprite2;

    private float fade = 0.0f;

    [SerializeField]
    private float fadeSpeed;

    [HideInInspector]
    public bool nearPlayer = false;

    private void Start()
    {
        GetComponent<SpriteRenderer>().material.SetTexture("_MainTex", sprite1);
        GetComponent<SpriteRenderer>().material.SetTexture("_MainTex2", sprite2);
    }

    private void Update()
    {
        if(nearPlayer)
        {
            fade = Mathf.Min(1.0f, fade + Time.deltaTime * fadeSpeed);
        }
        else
        {
            fade = Mathf.Max(0.0f, fade - Time.deltaTime * fadeSpeed);
        }

        GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
    }
}
