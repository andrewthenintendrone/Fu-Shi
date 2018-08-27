using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class fade : MonoBehaviour
{
    private Image fadeImage;
    private bool fadingOut;
    private bool fadingIn;
    private Color currentColor = Color.black;
    private string sceneName;

    private void Start()
    {
        if(GetComponent<Image>())
        {
            fadeImage = GetComponent<Image>();
        }
    }

    private void Update()
    {
        if(fadingOut)
        {
            currentColor.a = Mathf.Min(currentColor.a + Time.deltaTime, 1.0f);

            fadeImage.color = currentColor;

            if (currentColor.a == 1.0f)
            {
                SceneManager.LoadScene(sceneName);
                currentColor.a = 0.0f;
                fadingOut = false;
                fadingIn = true;
            }
        }
        else if(fadingIn)
        {
            currentColor.a = Mathf.Min(currentColor.a + Time.deltaTime, 1.0f);

            if(currentColor.a == 1.0f)
            {
                fadingIn = false;
            }
        }
    }

    public void loadScene(string sceneName)
    {
        this.sceneName = sceneName;

        currentColor.a = 0.0f;

        // fade out
        fadingOut = true;
    }
}
