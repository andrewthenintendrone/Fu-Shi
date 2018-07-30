﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MovementSettings
{
    [Tooltip("maximum speed that the player can run")]
    public float maxRunSpeed;

    [Tooltip("how fast the player will accelerate per second")]
    public float acceleration;

    [Tooltip("force to apply for jumps")]
    public float jumpForce;

    [Tooltip("extra force to apply for held jumps")]
    public float extraJumpForce;

    [Tooltip("extra time for held jumps")]
    public float extraJumpTime;

    [Tooltip("number of jumps the player can make before touching the ground")]
    public int jumpCount;

    [Tooltip("dash force")]
    public float dashForce;

    [Tooltip("how long to dash for")]
    public float dashTime;

    [Tooltip("dash cooldown")]
    public float dashCooldown;

    [Tooltip("how much enemies will knock the player back")]
    public float knockbackFromEnemies;

    [Tooltip("How much knockback will be done to enemies")]
    public float knockbackToEnemies;
}

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    // stored RigidBody 2D
    private Rigidbody2D rb;

    // timer for extra jump height
    private float extraJumpTimer;

    // dash cooldown timer
    private float dashCooldownTimer;

    // is the jump axis being held
    private bool jumpHeld;

    // number of jumps the player can still make
    public int currentJumps;

    private List<Vector3> positions = new List<Vector3>();

    [Tooltip("maximum health that the player can have")]
    public int maxHealth;

    [Tooltip("current health of the player")]
    public int currentHealth;

    // movement settings
    public MovementSettings movementSettings;

    public bool enableDebug = false;

    public enum AnimationState
    {
        IDLE,
        RUN,
        DASH,
        JUMP,
        DOUBLEJUMP
    }

    public AnimationState animationState = AnimationState.IDLE;

    private void Start()
    {
        // initialise Utils
        Utils.Init();

        // store RigidBody
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;

        currentHealth = maxHealth;
        currentJumps = movementSettings.jumpCount;
    }

    // physics step
    void FixedUpdate()
    {
        // branch based on current state (state machine)
        switch (animationState)
        {
            case AnimationState.IDLE:
                Idle();
                break;
            case AnimationState.RUN:
                Run();
                break;
            case AnimationState.DASH:
                Dash();
                break;
            case AnimationState.JUMP:
                Jump();
                break;
            case AnimationState.DOUBLEJUMP:
                DoubleJump();
                break;
            default:
                break;
        }

        // acclerate up to the maximum speed
        if (Mathf.Abs(rb.velocity.x) < movementSettings.maxRunSpeed)
        {
            rb.AddForce(Vector2.right * Input.GetAxisRaw("Horizontal") * movementSettings.acceleration);
        }

        // flip model to match direction
        if (rb.velocity.x > 0.1f)
        {
            transform.eulerAngles = Vector3.up * 180;
        }
        else if(rb.velocity.x < -0.1f)
        {
            transform.eulerAngles = Vector3.zero;
        }

        // dash input
        if (Input.GetAxisRaw("Fire2") == 1 && dashCooldownTimer == 0)
        {
            // reset dash cooldown timer
            dashCooldownTimer = movementSettings.dashCooldown;

            // cancel this dash after dash time
            Invoke("cancelDash", movementSettings.dashTime);

            // determine dash direction
            Vector3 dashDirection = Vector3.right * -Mathf.Sign(transform.right.x);

            // add dash force
            rb.AddForce(dashDirection * movementSettings.dashForce, ForceMode2D.Impulse);

            animationState = AnimationState.DASH;
        }

        if(isGrounded())
        {
            extraJumpTimer = movementSettings.extraJumpTime;
            currentJumps = movementSettings.jumpCount;
        }

        // if the jump axis is 0 a hold is over
        if (Input.GetAxisRaw("Fire1") == 0)
        {
            jumpHeld = false;
        }

        // jump input when jumps are more than 0
        if (Input.GetAxisRaw("Fire1") == 1 && !jumpHeld)
        {
            jumpHeld = true;

            if(currentJumps > 0)
            {
                // jump
                if (currentJumps == movementSettings.jumpCount)
                {
                    animationState = AnimationState.JUMP;
                }
                else
                {
                    animationState = AnimationState.DOUBLEJUMP;
                }

                currentJumps--;
                // cancel y momentum
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector3.up * movementSettings.jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    private void Update()
    {
        // decrement dash cooldown timer to 0
        dashCooldownTimer = Mathf.Max(0, dashCooldownTimer - Time.deltaTime);

        if (enableDebug)
        {
            // change color to match state
            if (isGrounded())
            {
                setColor(Color.red);
            }
            else
            {
                setColor(Color.white);
            }

            positions.Add(transform.position);

            for (int i = 0; i < positions.Count - 1; i++)
            {
                Debug.DrawLine(positions[i], positions[i + 1], Color.red);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Break();
            }
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Utils.Exit();
        }
    }

    // the player is Idle
    private void Idle()
    {
        // left / right input
        if(Mathf.Abs(rb.velocity.x) > 0)
        {
            // transition to run state
            animationState = AnimationState.RUN;
            return;
        }
    }

    // the player is running
    private void Run()
    {
        // transition back to an appropriate state when velocity gets low enough
        if (Mathf.Abs(rb.velocity.x) < 0.1f)
        {
            animationState = AnimationState.IDLE;
        }
    }

    // the player is dashing
    private void Dash()
    {
        
    }

    // the player is jumping
    void Jump()
    {
        // apply extra jump force
        if (extraJumpTimer > 0.0f && rb.velocity.y > 0)
        {
            rb.AddForce(Vector3.up * movementSettings.extraJumpForce, ForceMode2D.Force);

            // decrement extra jump timer
            extraJumpTimer = Mathf.Max(0.0f, extraJumpTimer - Time.fixedDeltaTime);
        }

        // transition back to an appropriate state
        if (isGrounded())
        {
            if(Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                animationState = AnimationState.RUN;
            }
            else
            {
                animationState = AnimationState.IDLE;
            }
        }
    }

    // the player is double jumping
    void DoubleJump()
    {
        // apply extra jump force
        if (extraJumpTimer > 0 && rb.velocity.y > 0)
        {
            rb.AddForce(Vector3.up * movementSettings.extraJumpForce, ForceMode2D.Force);

            // decrement extra jump timer
            extraJumpTimer = Mathf.Max(0.0f, extraJumpTimer - Time.fixedDeltaTime);
        }

        // transition back to an appropriate state
        if (isGrounded())
        {
            if (Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                animationState = AnimationState.RUN;
            }
            else
            {
                animationState = AnimationState.IDLE;
            }
        }
    }

    // check if the player is currently on the ground
    public bool isGrounded()
    {
        // we are not grounded if we are moving up
        if(rb.velocity.y > 0)
        {
            return false;
        }

        int layerMask = ~(1 << 8);

        // calculate corners of rectangle
        Vector3 bottomLeft = GetComponent<BoxCollider2D>().bounds.min + Vector3.right * 0.1f;
        Vector3 bottomRight = GetComponent<BoxCollider2D>().bounds.max + Vector3.left * 0.1f;
        bottomRight.y = bottomLeft.y;

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
        // tell Utils to reset the player to the last checkpoint
        if(collision.tag == "reset")
        {
            Utils.resetPlayer();
        }
        // enemy
        if (collision.tag == "enemy")
        {
            // dashing
            if(dashCooldownTimer > 0)
            {
                collision.gameObject.GetComponent<Rigidbody2D>().simulated = true;
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.ClampMagnitude(rb.velocity, 1.0f) * 10.0f, ForceMode2D.Impulse);

                // damage enemy
                collision.GetComponent<Enemy>().health--;

                // enemy dies if health hits 0
                collision.GetComponent<Enemy>().checkDead();
            }
            else
            {
                if(enableDebug)
                {
                    setColor(Color.blue);
                }

                // take damage from enemy
                currentHealth -= collision.GetComponent<Enemy>().damageAmt;

                // knockback
                rb.AddForce(new Vector2(-Mathf.Sign(rb.velocity.x) * movementSettings.knockbackFromEnemies, 0), ForceMode2D.Impulse);

                // are we dead
                if (currentHealth <= 0)
                {
                    // reset health and respawn
                    currentHealth = maxHealth;
                    Utils.resetPlayer();
                }
            }
        }
    }

    // kill dash by applying an opposite force
    public void cancelDash()
    {
        // kill x momentum
        rb.velocity = new Vector2(rb.velocity.x * 0.25f, rb.velocity.y);

        // transition back to an appropriate state
        if(Mathf.Abs(rb.velocity.x) > 0.1f)
        {
            animationState = AnimationState.RUN;
        }
        else
        {
            animationState = AnimationState.IDLE;
        }
    }

    // shortcut for setting material color
    void setColor(Color color)
    {
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = color;
    }
}
