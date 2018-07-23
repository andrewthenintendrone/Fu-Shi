using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MovementSettings
{
    [Tooltip("maximum speed that the player can run")]
    public float maxRunSpeed;

    [Tooltip("friction while running")]
    public float runFriction;

    [Tooltip("how fast the player will accelerate")]
    public float acceleration;

    [Tooltip("force to apply for jumps")]
    public float jumpForce;

    [Tooltip("extra force to apply for held jumps")]
    public float extraJumpForce;

    [Tooltip("extra time for held jumps")]
    public float extraJumpTime;

    [Tooltip("force to apply for wall jumps")]
    public float wallJumpForce;

    [Tooltip("angle for wall jumps")]
    public float wallJumpAngle;

    [Tooltip("dash force")]
    public float dashForce;

    [Tooltip("friction while dashing")]
    public float dashFriction;

    [Tooltip("how long to use dash friction for after dashing")]
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

    [Tooltip("maximum health that the player can have")]
    public int maxHealth;

    [Tooltip("current health of the player")]
    public int currentHealth;

    // movement settings
    public MovementSettings movementSettings;

    private void Start()
    {
        // initialise Utils
        Utils.Init();

        // store RigidBody
        rb = GetComponent<Rigidbody2D>();
        rb.drag = movementSettings.runFriction;
    }

    // physics step
    void FixedUpdate ()
    {
        // read the Horizontal input
        float xAxis = Input.GetAxisRaw("Horizontal");

        // if absolute x velocity is lower than the maximum run speed
        if(Mathf.Abs(rb.velocity.x) < movementSettings.maxRunSpeed)
        {
            rb.AddForce(Vector2.right * xAxis * movementSettings.acceleration);
        }

        // if the jump axis is 0 a hold is over
        if (Input.GetAxisRaw("Fire1") == 0)
        {
            jumpHeld = false;
        }

        // if we are on the ground reset the extra jump timer
        // probably can be improved
        if (!isGrounded())
        {
            // apply fake drag
            //rb.velocity -= new Vector2(Mathf.Clamp(rb.velocity.x, -1, 1) * Mathf.Sign(rb.velocity.x) * rb.drag * Time.fixedDeltaTime, 0);
        }

        // jump if the jump button is pressed
        if (Input.GetAxisRaw("Fire1") == 1)
        {
            Jump();
            wallJump();
        }

        // Dash if the dash button is pressed and the dash cooldown is 0
        if(Input.GetAxisRaw("Fire2") == 1 && dashCooldownTimer == 0)
        {
            // reset dash cooldown timer
            dashCooldownTimer = movementSettings.dashCooldown;

            // set dash friction
            rb.drag = movementSettings.dashFriction;

            Invoke("setRunFriction", movementSettings.dashTime);

            Vector3 dashDirection = Vector3.right * -Mathf.Sign(transform.right.x);

            rb.AddForce(dashDirection * movementSettings.dashForce, ForceMode2D.Impulse);
        }

        // flip model to match direction
        if(rb.velocity.x > 0)
        {
            transform.eulerAngles = Vector3.up * 180;
        }
        else if(rb.velocity.x < 0)
        {
            transform.eulerAngles = Vector3.zero;
        }
    }

    private void Update()
    {
        // change color to match state
        if(isGrounded())
        {
            setColor(Color.red);
        }
        else if(wallCheck() != 0)
        {
            setColor(Color.yellow);
        }
        else
        {
            setColor(Color.white);
        }

        // decrement dash cooldown timer to 0
        dashCooldownTimer = Mathf.Max(0, dashCooldownTimer - Time.deltaTime);
    }

    // check if the player is currently on the ground
    public bool isGrounded()
    {
        int layerMask = ~(1 << 8);

        // calculate corners of rectangle
        Vector3 bottomLeft = GetComponent<BoxCollider2D>().bounds.min + Vector3.right * 0.1f;
        Vector3 bottomRight = GetComponent<BoxCollider2D>().bounds.max + Vector3.left * 0.1f;
        bottomRight.y = bottomLeft.y;

        // draw in red or green based on whether we are grounded
        if(Physics2D.OverlapArea(bottomLeft, bottomRight, layerMask))
        {
            Debug.DrawLine(bottomLeft, bottomRight, Color.green);

            // reset extra jump timer
            extraJumpTimer = movementSettings.extraJumpTime;
            return true;
        }
        else
        {
            Debug.DrawLine(bottomLeft, bottomRight, Color.red);
            return false;
        }
    }

    // checks if the player is touching a wall
    // no wall = 0
    // left wall = 1
    // right wall = 2
    public int wallCheck()
    {
        int layerMask = ~(1 << 8);

        // calculate bottom corners of rectangle (move up to avoid detecting floor)
        Vector3 bottomLeft = GetComponent<BoxCollider2D>().bounds.min + Vector3.up * 0.1f;
        Vector3 bottomRight = GetComponent<BoxCollider2D>().bounds.max;
        bottomRight.y = bottomLeft.y;

        // offset the corners outwards (we want to detect walls that we arn't inside)
        bottomLeft.x -= 0.1f;
        bottomRight.x += 0.1f;

        // height of collider
        float height = GetComponent<BoxCollider2D>().bounds.size.y;

        int collision = 0;

        // left collision
        if(Physics2D.OverlapArea(bottomLeft, bottomLeft + Vector3.up * height, layerMask))
        {
            Debug.DrawLine(bottomLeft, bottomLeft + Vector3.up * height, Color.green);
            collision = 1;
        }
        else
        {
            Debug.DrawLine(bottomLeft, bottomLeft + Vector3.up * height, Color.red);
        }

        // right collision
        if (Physics2D.OverlapArea(bottomRight, bottomRight + Vector3.up * height, layerMask))
        {
            Debug.DrawLine(bottomRight, bottomRight + Vector3.up * height, Color.green);
            collision = 2;
        }
        else
        {
            Debug.DrawLine(bottomRight, bottomRight + Vector3.up * height, Color.red);
        }

        return collision;
    }

    // handle jumping
    void Jump()
    {
        // to make a jump the following conditions must be met
        // the player must be on the ground
        // the player must not be holding the jump button from a previous jump
        // the players y velocity must be 0 (to prevent "super jumps")
        if (isGrounded() && !jumpHeld && rb.velocity.y == 0)
        {
            // jump is now being held
            jumpHeld = true;

            // add the initial impulse jump force
            rb.AddForce(Vector3.up * movementSettings.jumpForce, ForceMode2D.Impulse);
        }

        // if a jump can't be started it may be because a jump is already in progress
        // in this case extra jump height should be applied until extra jump timer reaches 0
        else if (extraJumpTimer > 0.0f && rb.velocity.y > 0)
        {
            // add extra jump force
            rb.AddForce(Vector3.up * movementSettings.extraJumpForce, ForceMode2D.Force);

            // decrement extra jump timer
            extraJumpTimer -= Time.fixedDeltaTime;
        }
    }

    // handle wall jump
    void wallJump()
    {
        // to make a wall jump the following conditions must be met
        // the player must be on the wall
        // the player must not be holding the jump button from a previous jump
        if (wallCheck() != 0 && !jumpHeld && rb.velocity.x == 0)
        {
            // jump is now being held
            jumpHeld = true;

            // calculate impulse force using wall jump angle
            Vector3 wallJumpForce = new Vector3(Mathf.Sin(-movementSettings.wallJumpAngle * Mathf.Rad2Deg), Mathf.Cos(-movementSettings.wallJumpAngle * Mathf.Deg2Rad), 0).normalized;

            // if the wall is to the left flip the x
            if(wallCheck() == 1)
            {
                wallJumpForce.x = -wallJumpForce.x;
            }

            // add the initial impulse jump force
            rb.AddForce(wallJumpForce * movementSettings.wallJumpForce, ForceMode2D.Impulse);

            // draw wall jump angle line
            Debug.DrawLine(transform.position, transform.position + wallJumpForce, Color.magenta);
        }
    }

    void setRunFriction()
    {
        rb.drag = movementSettings.runFriction;
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
        if (collision.name == "Tengu_Enemy")
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
                setColor(Color.blue);

                // take damage
                currentHealth--;

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

    // shortcut for setting material color
    void setColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }
}
