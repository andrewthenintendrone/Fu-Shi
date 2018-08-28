using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class Utils
{

    //the position to reset to (updated via function)
    public static Vector3 resetPos;

    // health image
    private static Image healthImage;

    //player health value
    private static int health;

    public static int Health
    {
        get { return health; }
        set { health = value; updateHealthSprite(); }
    }

    public static int maxHealth;

    private static Sprite[] healthImages = new Sprite[9];

    private static bool devMode = false;
    public static bool DEVMODE
    {
        get { return devMode; }
    }

    public static int numberOfCollectables = 0;
    private static Text collectableText;

    // fade script
    private static fade fadeScript;

    // Use this for initialization
    public static void Init()
    {
        resetPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        if(GameObject.Find("collectableText") != null)
        {
            collectableText = GameObject.Find("collectableText").GetComponent<Text>();
        }
        if (GameObject.Find("Health") != null)
        {
            healthImage = GameObject.Find("Health").GetComponent<Image>();
        }
        else
        {

        }

        // create UI fade effect
        GameObject fadeObject = new GameObject("Fade");
        fadeScript = fadeObject.AddComponent<fade>();

        //in the same manner as the fade object above create the health sprite
        GameObject HPsymbol = new GameObject("Heath");

        //load the health sprites
        healthImages = Resources.LoadAll<Sprite>("dummy_healthbar");

        maxHealth = healthImages.Length - 1;

        //set default health
        Health = maxHealth;
    }

    public static void resetPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = resetPos;
        player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }

    public static void updateCheckpoint(Vector3 position)
    {
        resetPos = position;
    }

    public static void Exit()
    {
        Application.Quit();
    }

    public static T GetSafeComponent<T>(this GameObject obj)
    {
        T component = obj.GetComponent<T>();

        if (component == null)
        {
            Debug.LogError("Expected to find component of type "
               + typeof(T) + " but found none", obj);
        }

        return component;
    }

    public static void toggleDevMode()
    {
        devMode = !devMode;
    }

    public static void updateCollectableText()
    {
        if(collectableText != null)
        {
            collectableText.text = "Collectables: " + numberOfCollectables.ToString();
        }
    }

    private static void updateHealthSprite()
    {
        if (healthImage != null)
        {
            if (Health >= 0 && Health <= maxHealth)
            {
                healthImage.sprite = healthImages[Health];
            }
            else
            {
                Debug.Log("Tried to set health to " + Health.ToString() + ". Was this a mistake?");
            }
        }
    }

    // fade out while loading scene
    public static void loadScene(string sceneName)
    {
        fadeScript.loadScene(sceneName);
    }
}
