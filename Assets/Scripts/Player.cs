using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public struct MovementSettings
{
    public float runSpeed;

    public float acceleration;

    public float deceleration;

    public float slowDeceleration;

    public float jumpHeight;

    public int jumpCount;

    public float extraJumpForce;

    public float extraJumpTime;

    public float gravityScale;

    [Tooltip("terminal velocity")]
    public float maxFallSpeed;

    public AnimationCurve jumpCurve;
}

public class Player : MonoBehaviour
{
    [HideInInspector]
    public CharacterController2D character;
    public Vector3 velocity;

    public int currentJumps;
    public float extraJumpTimer;

    [HideInInspector]
    public bool jumpHeld = false;

    private float xAxis;
    private float yAxis;
    private int jumpAxis;

    public MovementSettings movementSettings;

    public GameObject InkBlotPrefab;

    private float currentDeceleration;

    // is the player launching
    [HideInInspector]
    public bool isLaunching = false;

    // can the player turn into an ink blot
    [HideInInspector]
    public bool canTurnIntoInkBlot = true;

    // which way the player is facing
    [HideInInspector]
    public bool facingRight = true;

    // dev mode jump curve time
    private float devModeTime = 0.0f;

    private void Start()
    {
        Utils.Init();
        character = GetComponent<CharacterController2D>();
        character.onTriggerEnterEvent += triggerFunction;
        character.onControllerCollidedEvent += collisionFunction;
        currentJumps = movementSettings.jumpCount;
        extraJumpTimer = movementSettings.extraJumpTime;
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
            //velocity = new Vector3(xAxis, yAxis, 0).normalized * movementSettings.runSpeed * 2;
            //transform.position += velocity * Time.deltaTime;
            devModeTime += Time.fixedDeltaTime;
            if (devModeTime >= movementSettings.jumpCurve.keys[movementSettings.jumpCurve.length - 1].time)
            {
                devModeTime = 0.0f;
            }

            transform.position = new Vector3(0, movementSettings.jumpCurve.Evaluate(devModeTime) * movementSettings.jumpHeight, 0);

            return;
        }

        if(!isLaunching)
        {
            // accelerate up to run speed
            if (velocity.x < movementSettings.runSpeed && xAxis > 0)
            {
                velocity.x += xAxis * movementSettings.acceleration * Time.fixedDeltaTime;
            }
            else if (velocity.x > -movementSettings.runSpeed && xAxis < 0)
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

            // reset extra jumps
            extraJumpTimer = movementSettings.extraJumpTime;

            // set y velocity to a small negative value to keep grounded
            if(!isLaunching)
            {
                velocity.y = -0.1f;
            }

            // finish launch
            if (velocity.y < 0)
            {
                isLaunching = false;
                velocity.x = Mathf.Sign(velocity.x) * Mathf.Min(Mathf.Abs(velocity.x), movementSettings.runSpeed);
            }
        }
        else
        {
            // add gravity force until terminal velocity is reached
            if (velocity.y > -movementSettings.maxFallSpeed)
            {
                velocity.y += Physics.gravity.y * movementSettings.gravityScale * Time.fixedDeltaTime;
            }

            // apply extra jump force
            if (jumpAxis == 1 && extraJumpTimer > 0 && velocity.y > 0)
            {
                velocity.y += movementSettings.extraJumpForce * Time.fixedDeltaTime;
                extraJumpTimer = Mathf.Max(0.0f, extraJumpTimer - Time.fixedDeltaTime);
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
                    velocity.y = movementSettings.jumpHeight;
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

    public void triggerFunction(Collider2D col)
    {
        if(col.tag == "reset")
        {
            Utils.resetPlayer();
        }
        else if(col.tag == "enemy")
        {
            Utils.Health = Mathf.Max(Utils.Health - 1, 0);
        }
        else if (col.tag == "checkpoint")
        {
            Utils.updateCheckpoint(col.transform.position);
        }
        else if(col.tag == "collectable")
        {
            Utils.Health = Mathf.Min(Utils.Health + 1, Utils.maxHealth);
            Utils.numberOfCollectables++;
            Utils.updateCollectableText();
            Destroy(col.gameObject);
        }
        else if (col.tag == "levelDoor")
        {
            Utils.loadScene(col.name);
        }
    }

    // eventually handle animation?
    public void UpdateAppearance()
    {
        // color red if on the ground
        // changeColor(character.isGrounded ? Color.red : Color.white);

        // scale the player model to match the direction of the players velocity
        if (Mathf.Abs(velocity.x) != 0)
        {
            facingRight = velocity.x > 0;
            transform.localScale = (facingRight ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1));
        }
    }
}
