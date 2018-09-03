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
}

public class Player : MonoBehaviour
{
    private CharacterController2D character;
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
    public bool isLaunching = false;

    // can the player turn into an ink blot
    public bool canTurnIntoInkBlot = true;

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
            velocity = new Vector3(xAxis, yAxis, 0).normalized * movementSettings.runSpeed * 2;

            transform.position += velocity * Time.deltaTime;

            return;
        }

        if(!isLaunching)
        {
            // accelerate up to run speed
            if (Mathf.Abs(velocity.x) < movementSettings.runSpeed)
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
            transform.localScale = (velocity.x > 0 ? new Vector3(1, 1, 1) : new Vector3(-1, 1, 1));
        }
    }
}
