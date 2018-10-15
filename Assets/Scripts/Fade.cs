using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fade : MonoBehaviour
{
    private Image fadeImage;
    private bool fadingOut = false;
    private bool fadingIn = true;
    private Color currentColor = Color.black;

    private void Start()
    {
        if(GetComponent<Image>())
        {
            fadeImage = GetComponent<Image>();
        }

        triggerFade();
    }

    private void Update()
    {
        if (fadingOut)
        {
            currentColor.a = Mathf.Min(currentColor.a + Time.deltaTime, 1.0f);

            if (currentColor.a == 1.0f)
            {
                fadingOut = false;
                fadingIn = true;
            }
        }
        else if(fadingIn)
        {
            currentColor.a = Mathf.Max(currentColor.a - Time.deltaTime, 0.0f);

            if(currentColor.a == 0.0f)
            {
                fadingIn = false;
                fadingOut = true;
            }
        }

        fadeImage.color = currentColor;
    }

    public void triggerFade()
    {
        fadingOut = true;
    }
}
