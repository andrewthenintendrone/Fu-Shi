using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// movement settings struct
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

// player requires a RigidBody2D component
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

    // list of previous positions for debugging
    private List<Vector3> positions = new List<Vector3>();

    [Tooltip("maximum health that the player can have")]
    public int maxHealth;

    [Tooltip("current health of the player")]
    public int currentHealth;

    // movement settings
    public MovementSettings movementSettings;

    [Tooltip("enable debugging functions")]
    public bool enableDebug = false;

    // AnimationState enum
    public enum AnimationState
    {
        IDLE,
        RUN,
        DASH,
        JUMP,
        DOUBLEJUMP
    }

    [Tooltip("current animation state")]
    public AnimationState animationState = AnimationState.IDLE;

    // called once on the first frame this script exists
    private void Start()
    {
        // initialise Utils
        Utils.Init();

        // store a reference to the RigidBody2D component and set its velocity to zero
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = Vector2.zero;

        // set the current health and current jums to thier maximum values
        currentHealth = maxHealth;
        currentJumps = movementSettings.jumpCount;
    }

    // called once per physics step (faster than frame rate)
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

        // flip the player model to match the direction of the players velocity
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

        // reset extra jump timer and current number of jumps when on the ground
        if(isGrounded())
        {
            extraJumpTimer = movementSettings.extraJumpTime;
            currentJumps = movementSettings.jumpCount;
        }

        // if the jump axis is 0 jumpHeld becomes false
        if (Input.GetAxisRaw("Fire1") == 0)
        {
            jumpHeld = false;
        }

        // jump input
        if (Input.GetAxisRaw("Fire1") == 1 && !jumpHeld)
        {
            // jump is now being held
            jumpHeld = true;

            // if the player still has jumps left
            if(currentJumps > 0)
            {
                // jump / double jump
                if (currentJumps == movementSettings.jumpCount)
                {
                    animationState = AnimationState.JUMP;
                }
                else
                {
                    animationState = AnimationState.DOUBLEJUMP;
                }

                // subtract a jump
                currentJumps--;

                // cancel y momentum to ensure all jumps are the same height
                rb.velocity = new Vector2(rb.velocity.x, 0);

                // add impulse force upwards
                rb.AddForce(Vector3.up * movementSettings.jumpForce, ForceMode2D.Impulse);
            }
        }

        // decrement dash cooldown timer to 0
        dashCooldownTimer = Mathf.Max(0, dashCooldownTimer - Time.fixedDeltaTime);
    }

    // called once per frame
    private void Update()
    {
        // show debug info (if enabled)
        if (enableDebug)
        {
            // change color to match state
            switch(animationState)
            {
                case AnimationState.IDLE:
                    setColor(Color.white);
                    break;
                case AnimationState.RUN:
                    setColor(Color.green);
                    break;
                case AnimationState.DASH:
                    setColor(Color.blue);
                    break;
                case AnimationState.JUMP:
                    setColor(Color.yellow);
                    break;
                case AnimationState.DOUBLEJUMP:
                    setColor(Color.cyan);
                    break;
                default:
                    // this should never occur
                    setColor(Color.magenta);
                    break;
            }

            // add the current position to the list
            positions.Add(transform.position);

            // draw a debug line between all previous positions
            for (int i = 0; i < positions.Count - 1; i++)
            {
                Debug.DrawLine(positions[i], positions[i + 1], Color.red);
            }

            // use B to break
            if (Input.GetKeyDown(KeyCode.B))
            {
                Debug.Break();
            }
        }

        // use escape key to exit game
        if (Input.GetKey(KeyCode.Escape))
        {
            Utils.Exit();
        }
    }

    // the player is Idle
    private void Idle()
    {
        // horizontal velocity
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
            if(isGrounded())
            {
                animationState = AnimationState.IDLE;
            }
            else if (currentJumps == movementSettings.jumpCount - 1)
            {
                animationState = AnimationState.JUMP;
            }
            else
            {
                animationState = AnimationState.DOUBLEJUMP;
            }
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
