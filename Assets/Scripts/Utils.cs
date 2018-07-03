using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    

    //the position to reset to (updated via function)
    public static Vector3 resetPos;
    

    // Use this for initialization
    public static void Init ()
    {
        resetPos = GameObject.FindGameObjectWithTag("Player").transform.position;
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
}
