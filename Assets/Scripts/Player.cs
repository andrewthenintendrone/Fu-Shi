using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // movement
    [Tooltip("maximum speed that the player can run")]
    public float maxRunSpeed;

    // current speed that the player is running at
    private float currentRunSpeed = 0;

    [Tooltip("how fast the player will accelerate")]
    public float acceleration;

    [Tooltip("how fast the player will decelerate")]
    public float deceleration;

    [Tooltip("the player stops when they are moving slower than this speed")]
    public float movementCutoff;

    // current movement direction
    private Vector2 movement = new Vector2(0, 0);

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
            // decelerate to 0
            currentRunSpeed -= Mathf.Sign(currentRunSpeed) * deceleration * Time.deltaTime;
        }
        else
        {
            currentRunSpeed += xAxis * acceleration;
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
