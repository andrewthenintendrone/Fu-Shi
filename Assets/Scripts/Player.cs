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
    private Rigidbody2D rb;

    private float extraJumpTimer;

    // movement
    public MovementSettings movementSettings;

    private void Start()
    {
        Utils.Init();

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

        // reset extra jump timer when grounded
        if(isGrounded())
        {
            extraJumpTimer = movementSettings.extraJumpTime;
        }

        // jump
        if(Input.GetAxisRaw("Fire1") == 1)
        {
            if(isGrounded())
            {
                Debug.Log("jumped");
                rb.AddForce(Vector3.up * movementSettings.jumpForce, ForceMode2D.Impulse);
            }
            else if(extraJumpTimer > 0.0f && rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.up * movementSettings.extraJumpForce, ForceMode2D.Force);
                extraJumpTimer -= Time.fixedDeltaTime;
            }
        }

        // cut off movement
        if (Mathf.Abs(rb.velocity.x) < movementSettings.movementCutoff)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void Update()
    {
        Debug.DrawRay(transform.position + Vector3.down * 0.5f, Vector3.down * 0.1f, Color.red);
    }

    // is the player on the ground
    public bool isGrounded()
    {
        // just raycast for now
        int layerMask = ~(1 << 8);
        return Physics2D.Raycast(transform.position + Vector3.down * 0.5f, Vector3.down, 0.1f, layerMask);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "checkpoint")
        {
            Utils.updateCheckpoint(collision.gameObject.transform.position);
        }
        if(collision.tag == "reset")
        {
            Utils.resetPlayer();
        }
    }
}
