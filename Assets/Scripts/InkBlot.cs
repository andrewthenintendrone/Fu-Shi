﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkBlot : MonoBehaviour
{
    [HideInInspector]
    // reference to the player gameobject
    public GameObject player;

    [Tooltip("force to launch the player at")]
    public float launchForce;

    [Tooltip("how long it takes for the player to regain control after launch")]
    public float launchTime;

    // debug line renderer
    private LineRenderer lr;

    // debug launch direction
    private Vector3 direction;


    private void Start()
    {
        // get debug line renderer
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        // get inputs
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        // get normalized launch direction
        direction = (Vector3.right * xAxis + Vector3.up * yAxis).normalized;

        // reset line points and draw direction line
        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + direction * 3);
        Debug.DrawLine(transform.position, transform.position + direction, Color.red);

        // the jump button launches
        if ((int)Input.GetAxisRaw("Jump") == 1)
        {
            launch();
        }
    }

    public void launch()
    {
        // reactivate the player gameobject and set isLaunching to true
        player.SetActive(true);
        player.GetComponent<Player>().isLaunching = true;

        // set the players velocity to the launch force
        player.GetComponent<Player>().velocity = direction * launchForce;

        // stop the launch after launchTime
        player.GetComponent<Player>().Invoke("cancelLaunch", launchTime);

        // destroy this gameobject
        Destroy(gameObject);
    }
}
