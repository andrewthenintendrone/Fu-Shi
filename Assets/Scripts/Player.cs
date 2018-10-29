using UnityEngine;
using System;
using System.Collections.Generic;
using XInputDotNetPure;

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

    [Tooltip("how much knockback to apply when hitting things")]
    public float knockBack;
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

    [SerializeField]
    [Range(0, 1)]
    [Tooltip("how strong to rumble when taking damage")]
    private float rumblePower = 1.0f;

    [SerializeField]
    [Tooltip("how long to rumble for when taking damage")]
    private float rumbleTime = 0.2f;

    private void Start()
    {
        character = GetComponent<CharacterController2D>();
        animator = GetComponent<Animator>();
        character.onTriggerEnterEvent += triggerEnterFunction;
        character.onTriggerStayEvent += triggerStayFunction;
        character.onTriggerExitEvent += triggerExitFunction;
        currentJumps = movementSettings.jumpCount;

        Utils.Init();
    }

    void FixedUpdate ()
    {
        if(!Utils.gamePaused && Utils.Health > 0)
        {
            animator.SetFloat("playSpeed", 1.0f);
            GetComponent<DynamicBone>().enabled = true;

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

            if (!isLaunching)
            {
                float maxSpeed = movementSettings.maxGroundSpeed;

                if (!character.isGrounded)
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
                    velocity.x = Mathf.Sign(velocity.x) * Mathf.Max(Mathf.Abs(velocity.x) - movementSettings.deceleration, 0.0f);
                }
            }

            // the player is on the ground
            if (character.isGrounded)
            {
                // reset current number of jumps
                currentJumps = movementSettings.jumpCount;

                // set y velocity to a small negative value to keep grounded
                if (!isLaunching)
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
                    if (velocity.y < 0)
                    {
                        velocity.y += Physics2D.gravity.y * movementSettings.fallSpeedMultiplier * Time.fixedDeltaTime;
                    }

                    if (!jumpHeld && velocity.y > 0)
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
        else
        {
            // disable animation and tail physics
            animator.SetFloat("playSpeed", 0.0f);

            GetComponent<DynamicBone>().enabled = false;
        }
    }

    void Update()
    {
        // toggle devmode
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Utils.toggleDevMode();
        }

        if(Utils.DEVMODE)
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                Utils.Health = Mathf.Min(Utils.Health + 1, Utils.maxHealth);
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                Utils.Health = Mathf.Max(Utils.Health - 1, 0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GetComponent<Abilityactivator>().hasInkAbility = !GetComponent<Abilityactivator>().hasInkAbility;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GetComponent<Abilityactivator>().hasTimeAbility = !GetComponent<Abilityactivator>().hasTimeAbility;
            }

            changeColor(new Color(UnityEngine.Random.Range(0, 255) / 255.0f, UnityEngine.Random.Range(0, 255) / 255.0f, UnityEngine.Random.Range(0, 255) / 255.0f));
        }
        else
        {
            changeColor(Color.white);
        }
    }

    void changeColor(Color color)
    {
        GetComponentInChildren<SkinnedMeshRenderer>().material.color = color;
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

    public void OnCollisionEnter2D(Collision2D hitInfo)
    {
        if(!Utils.gamePaused && Utils.Health > 0)
        {
            // inkable surface
            if (hitInfo.collider.gameObject.GetComponentInChildren<inkableSurface>() != null)
            {
                // inked
                if (hitInfo.collider.gameObject.GetComponentInChildren<inkableSurface>().Inked)
                {
                    if (canTurnIntoInkBlot)
                    {
                        // ensure only one ink blot at a time
                        if (GameObject.Find("inkblot") == null)
                        {
                            GameObject newInkBlot = Instantiate(InkBlotPrefab);
                            newInkBlot.name = "inkblot";
                            newInkBlot.transform.position = transform.position + new Vector3(character.boxCollider.offset.x, character.boxCollider.offset.y, 0);
                            newInkBlot.transform.parent = hitInfo.transform;
                            newInkBlot.GetComponent<InkBlot>().player = gameObject;
                            newInkBlot.GetComponent<InkBlot>().jumpHeld = jumpHeld;

                            // disable this gameObject
                            animator.enabled = false;
                            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                            GetComponent<Abilityactivator>().enabled = false;
                            GetComponent<Collider2D>().enabled = false;
                            this.enabled = false;
                        }
                    }
                }
            }
        }
    }

    public void triggerEnterFunction(Collider2D col)
    {
        if(!Utils.gamePaused && Utils.Health > 0)
        {
            if (col.tag == "reset")
            {
                Utils.ResetPlayer();
            }
            else if (col.tag == "enemy")
            {
                // damage
                if (!isInvulnerable)
                {
                    Utils.Health = Mathf.Max(Utils.Health - 1, 0);
                    isInvulnerable = true;
                    Invoke("becomeVulnerable", movementSettings.invulnerabilityTime);
                }

                // rumble
                GamePad.SetVibration(PlayerIndex.One, rumblePower, rumblePower);
                Invoke("stopRumble", rumbleTime);

                // knockback
                Vector3 directionToEnemy = (col.gameObject.transform.position - transform.position).normalized;
                RaycastHit2D hitInfo = Physics2D.Raycast(col.bounds.center, directionToEnemy, 10.0f, 1 << LayerMask.NameToLayer("Trigger"));
                if(hitInfo)
                {
                    velocity = hitInfo.normal * movementSettings.knockBack;
                }
            }
            else if (col.tag == "spikes")
            {
                // damage
                if(!isInvulnerable)
                {
                    Utils.Health = Mathf.Max(Utils.Health - 1, 0);
                    isInvulnerable = true;
                    Invoke("becomeVulnerable", movementSettings.invulnerabilityTime);
                }

                // rumble
                GamePad.SetVibration(PlayerIndex.One, rumblePower, rumblePower);
                Invoke("stopRumble", rumbleTime);

                // knockback
                Vector3 directionToSpikes = (col.gameObject.transform.position - character.boxCollider.bounds.center).normalized;
                RaycastHit2D hitInfo = Physics2D.Raycast(character.boxCollider.bounds.center, directionToSpikes, 10.0f, 1 << LayerMask.NameToLayer("Trigger"));
                if (hitInfo)
                {
                    velocity = hitInfo.normal * movementSettings.knockBack;
                }

                // add an extra jump for fairness
                currentJumps = Mathf.Min(currentJumps + 1, movementSettings.jumpCount);
            }
            // sets the checkpoint
            else if (col.tag == "checkpoint")
            {
                Utils.updateCheckpoint(col.transform.position);
            }
            else if (col.tag == "savepoint")
            {
                Utils.updateCheckpoint(col.transform.position);
                Utils.Health = Utils.maxHealth;
                SaveLoad.Save();
            }
            // collectable
            else if (col.tag == "collectable")
            {
                Utils.numberOfCollectables++;
                Utils.updateCollectableText();
                col.gameObject.SetActive(false);
            }
            // health pickup
            else if(col.tag == "health")
            {
                if(col.name == "extraHealth")
                {
                    Utils.maxHealth = 6;
                    Utils.Health = 6;
                    Destroy(col.gameObject);
                }
                else
                {
                    Utils.Health++;
                    Destroy(col.gameObject);
                }
            }
        }
    }

    public void triggerStayFunction(Collider2D col)
    {
        if(col.tag == "savepoint")
        {
            col.GetComponent<SavePoint>().nearPlayer = true;
        }
    }

    public void triggerExitFunction(Collider2D col)
    {
        if (col.tag == "savepoint")
        {
            col.gameObject.GetComponent<SavePoint>().nearPlayer = false;
        }
    }

    public void UpdateAppearance()
    {
        if(gameObject.activeSelf)
        {
            // rotate to match slope
            GameObject joints = GameObject.Find("JOINTS");
            RaycastHit2D slopeHitInfo = Physics2D.Raycast(transform.position, Vector2.down, 1.0f, character.platformMask);
            if (slopeHitInfo && character.isGrounded)
            {
                float angle = Vector2.SignedAngle(slopeHitInfo.normal, Vector2.up);
                joints.transform.localEulerAngles = Vector3.forward * angle * (facingRight ? 1 : -1);
            }
            else
            {
                joints.transform.localEulerAngles = Vector3.zero;
            }

            animator.SetBool("isGrounded", character.isGrounded);
            animator.SetFloat("absoluteXVelocity", Mathf.Abs(velocity.x));
            animator.SetFloat("yVelocity", velocity.y);
            animator.SetInteger("currentJumps", currentJumps);
        }

        // scale the player model to match the direction of the players velocity
        if (Mathf.Abs(velocity.x) != 0)
        {
            facingRight = velocity.x > 0;
            transform.localScale = (facingRight ? new Vector3(-1, 1, 1) : new Vector3(1, 1, 1));
        }
    }

    private void becomeVulnerable()
    {
        isInvulnerable = false;
    }

    private void stopRumble()
    {
        GamePad.SetVibration(PlayerIndex.One, 0, 0);
    }
}
