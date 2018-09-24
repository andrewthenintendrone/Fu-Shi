using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public struct MovementSettings
{
    [Tooltip("maximum horizontal speed while on the ground")]
    public float maxGroundSpeed;

    [Tooltip("maximum horizontal speed while in the air")]
    public float maxAirSpeed;

    [Tooltip("speed at which to reach maximum horizontal speed")]
    public float acceleration;

    [Tooltip("speed at which to reach zero horizontal speed from maximum horizontal speed")]
    public float deceleration;

    [Tooltip("speed at which to reach zero horizontal speed from maximum horizontal speed while on a slippery surface")]
    public float slowDeceleration;

    [Tooltip("force to apply when jumping")]
    public float jumpForce;

    [Tooltip("number of jumps that the player can make (2 is double jump)")]
    public int jumpCount;

    [Tooltip("regular strength of gravity")]
    public float gravityScale;

    [Tooltip("fall speed multiplier")]
    public float fallSpeedMultiplier;

    [Tooltip("low jump fall speed multiplier")]
    public float lowJumpFallSpeedMultiplier;

    [Tooltip("terminal velocity")]
    public float maxFallSpeed;

    [Tooltip("how long to stay invulnerable after taking damage")]
    public float invulnerabilityTime;
}

public class Player : MonoBehaviour
{
    [HideInInspector]
    public CharacterController2D character;

    [HideInInspector]
    public Vector3 velocity;

    [HideInInspector]
    public int currentJumps;

    [HideInInspector]
    public bool jumpHeld = false;

    // current x axis value
    private float xAxis;

    // current y axis value
    private float yAxis;

    // current jump axis value
    private int jumpAxis;

    [Tooltip("settings related to character movement")]
    public MovementSettings movementSettings;

    [Tooltip("ink blot prefab")]
    public GameObject InkBlotPrefab;

    // the current deceleration value to use (regular or slippery)
    private float currentDeceleration;

    [HideInInspector]
    // is the player launching
    public bool isLaunching = false;

    // can the player turn into an ink blot
    [HideInInspector]
    public bool canTurnIntoInkBlot = true;

    // which way the player is facing
    [HideInInspector]
    public bool facingRight = true;

    private Animator animator;

    // is the player invulnerable
    private bool isInvulnerable;

    private void Start()
    {
        Utils.Init();
        character = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
        character.onTriggerEnterEvent += triggerEnterFunction;
        character.onTriggerStayEvent += triggerStayFunction;
        character.onControllerCollidedEvent += collisionFunction;
        currentJumps = movementSettings.jumpCount;
        currentDeceleration = movementSettings.deceleration;
    }

    void FixedUpdate ()
    {
        #region get inputs

        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");
        jumpAxis = (int)Input.GetAxisRaw("Jump");

        #endregion

        // development movement overides everything else
        if (Utils.DEVMODE)
        {
            velocity = new Vector3(xAxis, yAxis, 0).normalized * movementSettings.maxAirSpeed * 2;
            transform.position += velocity * Time.deltaTime;

            return;
        }

        if(!isLaunching)
        {
            float maxSpeed = movementSettings.maxGroundSpeed;

            if(!character.isGrounded)
            {
                maxSpeed = movementSettings.maxAirSpeed;
            }

            // accelerate up to run speed
            if (velocity.x < maxSpeed && xAxis > 0)
            {
                velocity.x += xAxis * movementSettings.acceleration * Time.fixedDeltaTime;
            }
            else if (velocity.x > -maxSpeed && xAxis < 0)
            {
                velocity.x += xAxis * movementSettings.acceleration * Time.fixedDeltaTime;
            }
            // decelerate
            if (xAxis == 0 && velocity.x != 0)
            {
                velocity.x = Mathf.Sign(velocity.x) * Mathf.Max(Mathf.Abs(velocity.x) - currentDeceleration, 0.0f);
            }
        }

        // the player is on the ground
        if (character.isGrounded)
        {
            // reset current number of jumps
            currentJumps = movementSettings.jumpCount;

            // set y velocity to a small negative value to keep grounded
            if(!isLaunching)
            {
                velocity.y = -0.1f;
            }

            // finish launch
            if (velocity.y < 0)
            {
                isLaunching = false;
                velocity.x = Mathf.Sign(velocity.x) * Mathf.Min(Mathf.Abs(velocity.x), movementSettings.maxGroundSpeed);
            }
        }
        else
        {
            // only add gravity force until terminal velocity is reached
            if (velocity.y > -movementSettings.maxFallSpeed)
            {
                // regular gravity
                velocity.y += Physics.gravity.y * movementSettings.gravityScale * Time.fixedDeltaTime;

                // fall speed multiplied gravity
                if(velocity.y < 0)
                {
                    velocity.y += Physics2D.gravity.y * movementSettings.fallSpeedMultiplier * Time.fixedDeltaTime;
                }

                if(!jumpHeld && velocity.y > 0)
                {
                    velocity.y += Physics2D.gravity.y * movementSettings.lowJumpFallSpeedMultiplier * Time.fixedDeltaTime;
                }
            }
        }

        // update jump held
        if (!jumpHeld && jumpAxis == 1)
        {
            jumpHeld = true;

            if (yAxis <= -0.5f)
            {
                character.ignoreOneWayPlatformsThisFrame = true;
                Invoke("enableOneWayPlatforms", 0.25f);
            }
            else
            {
                if (currentJumps > 0 && !isLaunching)
                {
                    velocity.y = movementSettings.jumpForce;
                    currentJumps--;
                }
            }
        }
        if (jumpHeld && jumpAxis == 0)
        {
            jumpHeld = false;
        }

        character.move(velocity * Time.fixedDeltaTime);

        UpdateAppearance();
    }

    void Update()
    {
        // toggle devmode
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Utils.toggleDevMode();
        }
    }

    void changeColor(Color color)
    {
        GetComponentInChildren<Renderer>().material.color = color;
    }

    void enableOneWayPlatforms()
    {
        character.ignoreOneWayPlatformsThisFrame = false;
    }

    void cancelLaunch()
    {
        isLaunching = false;
    }

    void enableCanTurnIntoInkBlot()
    {
        canTurnIntoInkBlot = true;
    }

    public void collisionFunction(RaycastHit2D hitInfo)
    {
        // inkable surface
        if (hitInfo.collider.gameObject.GetComponentInChildren<inkableSurface>() != null)
        {
            // inked
            if (hitInfo.collider.gameObject.GetComponentInChildren<inkableSurface>().Inked)
            {
                if(canTurnIntoInkBlot)
                {
                    // ensure only one ink blot at a time
                    if (GameObject.Find("inkblot") == null)
                    {
                        // disable this gameObject
                        gameObject.SetActive(false);
                        GameObject newInkBlot = Instantiate(InkBlotPrefab);
                        newInkBlot.name = "inkblot";
                        newInkBlot.transform.position = transform.position + new Vector3(character.boxCollider.offset.x, character.boxCollider.offset.y, 0);
                        newInkBlot.transform.parent = hitInfo.transform;
                        newInkBlot.GetComponent<InkBlot>().player = gameObject;
                        newInkBlot.GetComponent<InkBlot>().jumpHeld = jumpHeld;
                    }
                }
            }
        }

        // slippery surface
        if(hitInfo.collider.gameObject.tag == "slippery")
        {
            currentDeceleration = movementSettings.slowDeceleration;
        }
        else
        {
            currentDeceleration = movementSettings.deceleration;
        }
    }

    public void triggerEnterFunction(Collider2D col)
    {
        if(col.tag == "reset")
        {
            Utils.resetPlayer();
        }
        else if(col.tag == "enemy")
        {
            Utils.Health = Mathf.Max(Utils.Health - 1, 0);
        }
        // sets the checkpoint
        else if (col.tag == "checkpoint")
        {
            Utils.updateCheckpoint(col.transform.position);
        }
        else if(col.tag == "savepoint")
        {
            Utils.updateCheckpoint(col.transform.position);
            SaveLoad.Save();
        }
        else if(col.tag == "collectable")
        {
            Utils.Health = Mathf.Min(Utils.Health + 1, Utils.maxHealth);
            Utils.numberOfCollectables++;
            Utils.updateCollectableText();
            col.gameObject.SetActive(false);
        }
    }

    public void triggerStayFunction(Collider2D col)
    {
        if (col.tag == "spikes")
        {
            if (!isInvulnerable)
            {
                Utils.Health = Mathf.Max(Utils.Health - 1, 0);
                isInvulnerable = true;

                Invoke("becomeVulnerable", movementSettings.invulnerabilityTime);
            }
        }
    }

    public void UpdateAppearance()
    {
        // color red if on the ground
        // changeColor(character.isGrounded ? Color.red : Color.white);

        if(gameObject.activeSelf)
        {
            animator.SetBool("isGrounded", character.isGrounded);
            animator.SetFloat("absoluteXVelocity", Mathf.Abs(velocity.x));
            animator.SetFloat("yVelocity", velocity.y);
            animator.SetInteger("currentJumps", currentJumps);

            // landing
            if(character.collisionState.becameGroundedThisFrame)
            {
                animator.SetTrigger("land");
            }
            else
            {
                animator.ResetTrigger("land");
            }
        }

        // scale the player model to match the direction of the players velocity
        if (Mathf.Abs(velocity.x) != 0)
        {
            facingRight = velocity.x > 0;
            transform.localScale = (facingRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1));
        }
    }

    private void becomeVulnerable()
    {
        isInvulnerable = false;
    }
}
