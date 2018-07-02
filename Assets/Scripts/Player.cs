using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MovementSettings
{
    [Tooltip("maximum speed that the player can run")]
    public float maxRunSpeed;

    [Tooltip("how fast the player will accelerate")]
    public float acceleration;

    [Tooltip("how fast the player will decelerate")]
    public float deceleration;

    [Tooltip("the player stops when they are moving slower than this speed")]
    public float movementCutoff;
}

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    // movement
    public MovementSettings movementSettings;

    private void Start()
    {
        // store RigidBody
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate ()
    {
        // check the Horizontal input
        float xAxis = Input.GetAxisRaw("Horizontal");

        // accelerate
        if(Mathf.Abs(rb.velocity.x) < movementSettings.maxRunSpeed)
        {
            rb.AddForce(Vector2.right * xAxis * movementSettings.acceleration);
        }

        // cut off movement
        if (Mathf.Abs(rb.velocity.x) < movementSettings.movementCutoff)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
	}
}
