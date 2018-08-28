using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class fade : MonoBehaviour
{
    private Image fadeImage;
    private bool fadingOut = false;
    private bool fadingIn = true;
    private Color currentColor = Color.black;
    private string sceneName;

    private void Start()
    {

        // attach to canvas
        if (GameObject.FindObjectOfType<Canvas>() != null)
        {
            fadeImage = gameObject.AddComponent<Image>();
            fadeImage.rectTransform.SetParent(GameObject.FindObjectOfType<Canvas>().transform);

            // scale to fill canvas
            fadeImage.rectTransform.anchorMin = new Vector2(0, 0);
            fadeImage.rectTransform.anchorMax = new Vector2(1, 1);
            fadeImage.rectTransform.offsetMin = Vector3.zero;
            fadeImage.rectTransform.offsetMax = Vector3.zero;
            fadeImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            fadeImage.rectTransform.localScale = new Vector3(1, 1, 1);

            fadeImage.rectTransform.localPosition = Vector3.zero;
        }
    }

    private void Update()
    {
        fadeImage.color = currentColor;
        if (fadingOut)
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
        if(fadingIn)
        {
            currentColor.a = Mathf.Max(currentColor.a - Time.deltaTime, 0.0f);

            if(currentColor.a == 0.0f)
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
