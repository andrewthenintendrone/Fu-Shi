using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class Utils
{
    //the position to reset the player to when they die
    public static Vector3 resetPos;

    // reference to health ui image
    private static Image healthImage;

    // current player health value
    private static int health;

    public static int Health
    {
        get { return health; }
        // update UI health sprite whenever health is set
        set { health = value; updateHealthSprite(); }
    }

    // maximum player health
    public static int maxHealth;

    private static Sprite[] healthImages = new Sprite[9];

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
        //if(!SaveLoad.Load())
        //{
            resetPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        //}

        if(GameObject.Find("collectableText") != null)
        {
            collectableText = GameObject.Find("collectableText").GetComponent<Text>();
        }
        if (GameObject.Find("Health") != null)
        {
            healthImage = GameObject.Find("Health").GetComponent<Image>();
        }

        // load the health sprites
        healthImages = Resources.LoadAll<Sprite>("health_strip");

        // maximum health is determined y how many health sprites there are
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
            if(Health <= 0)
            {
                resetPlayer();
                Health = maxHealth;
                return;
            }

            if (Health > 0 && Health <= maxHealth)
            {
                healthImage.sprite = healthImages[Health];
            }
            else
            {
                Debug.Log("Tried to set health to " + Health.ToString() + ". Was this a mistake?");
            }
        }
    }

    // load scene (in Utils)
    public static void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
