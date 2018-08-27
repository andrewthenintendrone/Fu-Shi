using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public static Sprite[] healthImages = new Sprite[3];

    private static bool devMode = false;
    public static bool DEVMODE
    {
        get { return devMode; }
    }

    public static int numberOfCollectables = 0;
    private static Text collectableText;

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

        //set default health
        Health = 3;

        //load the health sprites
        healthImages[0] = Resources.Load<Sprite>("Health_Full");
        healthImages[1] = Resources.Load<Sprite>("Health_Medium");
        healthImages[2] = Resources.Load<Sprite>("Health_Dangerous");

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
        if(healthImage != null)
        {
            if (Health >= 0 && Health < healthImages.Length)
            {
                GameObject.Find("Health").GetComponent<Image>().sprite = healthImages[Health];
            }
            else
            {
                Debug.Log("Tried to set health to " + Health.ToString() + ". Was this a mistake?");
            }
        }
    }
}
