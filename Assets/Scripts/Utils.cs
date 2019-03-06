﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public static class Utils 
{
    #region variables

    //the position to reset the player to when they die
    public static Vector2 resetPos;

    // is the game paused
    public static bool gamePaused = false;

    // reference to health ui image
    private static Image healthImage1;
    private static Image healthImage2;

    // current player health value
    private static int health = 1;

    public static int Health
    {
        get { return health; }
        // update UI health sprite whenever health is set
        set { health = Mathf.Min(value, maxHealth); updateHealthSprite(); }
    }

    // time that the game was started
    public static int startTime = 0;

    // maximum player health
    public static int maxHealth;

    private static Sprite[] healthImages = new Sprite[9];

    // player reference
    private static GameObject player = null;

    // dev mode bool
    private static bool devMode = false;
    public static bool DEVMODE
    {
        get { return devMode; }
    }

    #endregion

    // Use this for initialization
    public static void Init()
    {
        if (GameObject.Find("Health1") != null)
        {
            healthImage1 = GameObject.Find("Health1").GetComponent<Image>();
        }
        if (GameObject.Find("Health2") != null)
        {
            healthImage2 = GameObject.Find("Health2").GetComponent<Image>();
        }

        // load the health sprites
        healthImages = Resources.LoadAll<Sprite>("HealthUI");

        // maximum health is determined y how many health sprites there are
        maxHealth = healthImages.Length - 1;

        //set default health
        Health = maxHealth;

        if (!SaveLoad.Load())
        {
            resetPos = getPlayer().transform.position;
        }
    }

    public static IEnumerator KillPlayer()
    {
        // make sure the player is a fox not an ink blot
        if (getPlayer().GetComponent<InkBlot>() != null)
        {
            Debug.Log(player.name);
            player.GetComponent<InkBlot>().launch();
        }
        else
        {
            getPlayer().GetComponent<Animator>().SetTrigger("death");

            // trigger a fade out
            GameObject.FindObjectOfType<Fade>().triggerFadeOut();
        }

        yield return null;
    }

    // returns the player to the last checkpoint immediately
    public static void ResetPlayer()
    {
        // reset the players position and velocity to the last checkpoint
        player.transform.position = resetPos;
        player.GetComponent<Player>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody2D>().velocity = Vector3.zero;

        // restore health
        Health = maxHealth;

        // teleport camera as well
        GameObject.FindObjectOfType<DualForwardFocusCamera>().TeleportToPlayer();

        // trigger the fade in
        GameObject.FindObjectOfType<Fade>().triggerFadeIn();
    }

    // updates the checkpoint to the specified position
    public static void updateCheckpoint(Vector2 position)
    {
        resetPos = position;
    }

    public static void Exit()
    {
        DiscordRpc.Shutdown();
        Application.Quit();
    }

    public static void toggleDevMode()
    {
        devMode = !devMode;
    }

    private static void updateHealthSprite()
    {
        if (healthImage1 != null && healthImage2 != null)
        {
            // adjust if the player has the extra health
            if(maxHealth > 3)
            {
                healthImage1.GetComponent<RectTransform>().anchoredPosition = Vector3.left * 128;
                healthImage2.enabled = true;
            }
            else
            {
                healthImage1.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                healthImage2.enabled = false;
            }

            if (Health >= 0 && Health < 3)
            {
                healthImage1.sprite = healthImages[Health];
            }
            else if(Health >= 3 && Health <= maxHealth)
            {
                healthImage1.sprite = healthImages[3];
                healthImage2.sprite = healthImages[health - 3];
            }
            else
            {
                Debug.Log("Tried to set health to " + Health.ToString() + ". Was this a mistake?");
            }

            if (Health <= 0)
            {
                GameObject.FindObjectOfType<MonoBehaviour>().StartCoroutine(KillPlayer());
            }
        }
    }

    // load scene (in Utils)
    public static void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // returns the player
    public static GameObject getPlayer()
    {
        // find the current player gameobject
        if (GameObject.Find("inkblot") != null)
        {
            player = GameObject.Find("inkblot");
        }
        else if (GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player");
        }

        return player;
    }

    // shows a notification box with the specified strings
    public static void showNotification(string messageText, string confirmText)
    {
        GameObject.FindObjectOfType<UIController>().showNotification(messageText, confirmText);
    }

    // update discord rich presence
    public static void UpdateDiscordPresence()
    {
        DiscordRpc.RichPresence discordPresence = new DiscordRpc.RichPresence();
        discordPresence.startTimestamp = startTime;
        discordPresence.largeImageKey = "fox";
        
        // figure out image key and message
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            discordPresence.smallImageKey = "menu";
            discordPresence.details = "Is on the menu";
        }
        else if(SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (Utils.maxHealth == 6)
            {
                discordPresence.smallImageKey = "health";
                discordPresence.details = "Has obtained Inari's vitality";
                discordPresence.state = "Currently has " + health.ToString() + " health";
            }
            else if (player.GetComponent<Abilityactivator>().hasTimeAbility)
            {
                discordPresence.smallImageKey = "time";
                discordPresence.details = "Has obtained Daikokuten's drum";
                discordPresence.state = "Currently has " + health.ToString() + " health";
            }
            else if (player.GetComponent<Abilityactivator>().hasInkAbility)
            {
                discordPresence.smallImageKey = "ink";
                discordPresence.details = "Has obtained Jurojin's brush";
                discordPresence.state = "Currently has " + health.ToString() + " health";
            }
            else
            {
                discordPresence.smallImageKey = "";
                discordPresence.details = "Has had soul stolen";
                discordPresence.state = "Currently has " + health.ToString() + " health";
            }
        }
        else if(SceneManager.GetActiveScene().buildIndex == 2)
        {
            discordPresence.smallImageKey = "ending";
            discordPresence.details = "Has completed the game";
        }

        DiscordRpc.UpdatePresence(discordPresence);
    }
}
