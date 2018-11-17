using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class ControllerTest : MonoBehaviour
{
    private GameObject stickSprite;

    public float deadZone = 0.19f;


	// Use this for initialization
	void Start ()
    {
        stickSprite = GetComponentsInChildren<Transform>()[1].gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 stickPosition = new Vector3(xAxis, yAxis, 0);

        if(stickPosition.magnitude >= deadZone)
        {
            stickSprite.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
        }
        else
        {
            stickSprite.GetComponent<Renderer>().material.SetColor("_Color", Color.white);
            GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
        }

        stickSprite.transform.localPosition = stickPosition * stickSprite.transform.localScale.x;
	}
}
