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

    [Tooltip("the player stops when they are moving slower than this speed")]
    public float movementCutoff;

    [Tooltip("force to apply for jumps")]
    public float jumpForce;

    [Tooltip("extra force to apply for held jumps")]
    public float extraJumpForce;

    [Tooltip("extra time for held jumps")]
    public float extraJumpTime;
}

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    // stored RigidBody 2D
    private Rigidbody2D rb;

    // timer for extra jump height
    private float extraJumpTimer;

    // is the jump axis being held
    private bool jumpHeld;

    // movement settings
    public MovementSettings movementSettings;

    private void Start()
    {
        // initialise Utils
        Utils.Init();

        // store RigidBody
        rb = GetComponent<Rigidbody2D>();
    }

    // physics step
    void FixedUpdate ()
    {
        // read the Horizontal input
        float xAxis = Input.GetAxisRaw("Horizontal");

        // if absolute x velocity is lower than the maximum run speed
        if(Mathf.Abs(rb.velocity.x) < movementSettings.maxRunSpeed)
        {
            // add force in the direction of movement
            rb.AddForce(Vector2.right * xAxis * movementSettings.acceleration);
        }

        // if we are on the ground reset the extra jump timer
        if(isGrounded())
        {
            extraJumpTimer = movementSettings.extraJumpTime;
        }

        // if the jump button is pressed
        if(Input.GetAxisRaw("Fire1") == 1)
        {
            // if we are on the ground
            // don't jump multiple times for a hold
            // ensure y velocity is 0 to prevent "super jump"
            if(isGrounded() && !jumpHeld && rb.velocity.y == 0)
            {
                jumpHeld = true;
                // add initial impulse jump force
                rb.AddForce(Vector3.up * movementSettings.jumpForce, ForceMode2D.Impulse);
            }
            // if we are in the air and still have extra jump time
            // don't allow extra jump when falling
            else if(extraJumpTimer > 0.0f && rb.velocity.y > 0)
            {
                // add extra jump force
                rb.AddForce(Vector3.up * movementSettings.extraJumpForce, ForceMode2D.Force);

                // decrement extra jump timer
                extraJumpTimer -= Time.fixedDeltaTime;
            }
        }

        // if the jump axis is 0 a hold is over
        if(Input.GetAxisRaw("Fire1") == 0)
        {
            jumpHeld = false;
        }

        // cut off movement when it gets too slow
        if (Mathf.Abs(rb.velocity.x) < movementSettings.movementCutoff)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void Update()
    {

    }

    // check if the player is currently on the ground
    public bool isGrounded()
    {
        Debug.Log("Current velocity is " + rb.velocity);

        int layerMask = ~(1 << 8);

        // calculate corners of rectangle
        Vector3 bottomLeft = transform.position + Vector3.left * transform.localScale.x / 2 + Vector3.down * transform.localScale.y / 2;
        Vector3 bottomRight = transform.position + Vector3.right * transform.localScale.x / 2 + Vector3.down * transform.localScale.y / 2;

        // draw in red or green based on whether we are grounded
        if(Physics2D.OverlapArea(bottomLeft, bottomRight, layerMask))
        {
            Debug.DrawLine(bottomLeft, bottomRight, Color.green);
            return true;
        }
        else
        {
            Debug.DrawLine(bottomLeft, bottomRight, Color.red);
            return false;
        }
    }

    // called by unity when this object enters a trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // tell Utils to update the checkpoint
        if(collision.tag == "checkpoint")
        {
            Utils.updateCheckpoint(collision.gameObject.transform.position);
        }
        // sell Utils to reset the player to the last checkpoint
        if(collision.tag == "reset")
        {
            Utils.resetPlayer();
        }
    }
}
