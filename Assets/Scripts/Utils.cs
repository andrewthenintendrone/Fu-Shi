﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{

    //the position to reset to (updated via function)
    public static Vector3 resetPos;
    
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
        collectableText = GameObject.Find("collectableText").GetComponent<Text>();
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
        collectableText.text = "Collectables: " + numberOfCollectables.ToString();
    }
}
