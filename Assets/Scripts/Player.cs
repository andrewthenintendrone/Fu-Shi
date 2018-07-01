using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // movement
    [Tooltip("maximum speed that the player can run")]
    public float maxRunSpeed;

    [Tooltip("current speed that the player is running at")]
    public float currentRunSpeed = 0;

    [Tooltip("the player gains this much speed per second when accelerating")]
    public float accelerationFactor;

    [Tooltip("the player loses this much speed per second when deccelerating")]
    public float deccelerationFactor;

    [Tooltip("the player comes to a stop when they are moving slower than this speed")]
    public float movementCutoff;

    [Tooltip("current movement direction")]
    public Vector2 movement = new Vector2(0, 0);

    // gravity
    [Tooltip("power of gravity")]
    public float gravity = -9.81f;

	void Update ()
    {
        // check the Horizontal input
        float xAxis = Input.GetAxisRaw("Horizontal");

        // no input
        if(xAxis == 0 && currentRunSpeed != 0)
        {
            // deccelerate to 0
            currentRunSpeed -= Mathf.Sign(currentRunSpeed) * deccelerationFactor * Time.deltaTime;
        }
        else
        {
            currentRunSpeed += xAxis * accelerationFactor;
            if(Mathf.Abs(currentRunSpeed) > maxRunSpeed)
            {
                currentRunSpeed = maxRunSpeed * Mathf.Sign(currentRunSpeed);
            }
        }

        // cut off movement
        if(Mathf.Abs(currentRunSpeed) < movementCutoff)
        {
            currentRunSpeed = 0;
        }

        movement.Set(currentRunSpeed, 0);

        transform.Translate(movement * Time.deltaTime);
	}
}
