using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    private Image fadeImage;
    private Color currentColor = Color.black;

    enum FADETYPE
    {
        IN,
        OUT,
        NONE
    }

    private FADETYPE currentFadeType = FADETYPE.IN;

    [SerializeField]
    [Tooltip("time it takes to fade in / out in seconds")]
    private float fadeTime = 1.0f;

    private void Start()
    {
        if(GetComponent<Image>())
        {
            fadeImage = GetComponent<Image>();
        }

        triggerFadeIn();
    }

    private void Update()
    {
        switch (currentFadeType)
        {
            case FADETYPE.IN:
                currentColor.a = Mathf.Max(currentColor.a - Time.deltaTime / fadeTime, 0.0f);

                if (currentColor.a == 0.0f)
                {
                    currentFadeType = FADETYPE.NONE;
                }
                break;
            case FADETYPE.OUT:
                currentColor.a = Mathf.Min(currentColor.a + Time.deltaTime / fadeTime, 1.0f);

                if (currentColor.a == 1.0f)
                {
                    currentFadeType = FADETYPE.NONE;

                    // after fade out is finished reset the player
                    Utils.ResetPlayer();
                }
                break;
            case FADETYPE.NONE:
                break;
            default:
                break;
        }

        fadeImage.color = currentColor;
    }

    public void triggerFadeOut()
    {
        currentColor.a = 0.0f;

        currentFadeType = FADETYPE.OUT;
    }

    public void triggerFadeIn()
    {
        currentColor.a = 1.0f;

        currentFadeType = FADETYPE.IN;
    }
}
