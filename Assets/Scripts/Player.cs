﻿using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public struct MovementSettings
{
    public float runSpeed;

    public float jumpHeight;

    public int jumpCount;

    public float extraJumpForce;

    public float extraJumpTime;

    public float gravityScale;
}

public class Player : MonoBehaviour
{
    private CharacterController2D character;
    public Vector3 velocity;

    public int currentJumps;
    public float extraJumpTimer;
    bool jumpHeld = false;

    private float xAxis;
    private float yAxis;
    private int jumpAxis;

    public MovementSettings movementSettings;

    public enum AnimationState
    {
        IDLE,
        DASH,
        RUN,
        JUMP
    }

    public AnimationState animationState = AnimationState.IDLE;

    public GameObject InkBlotPrefab;

    private void Start()
    {
        Utils.Init();
        character = GetComponent<CharacterController2D>();
        character.onTriggerEnterEvent += triggerFunction;
        character.onControllerCollidedEvent += collisionFunction;
        currentJumps = movementSettings.jumpCount;
        extraJumpTimer = movementSettings.extraJumpTime;
    }

    void FixedUpdate ()
    {
        #region get inputs

        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");
        jumpAxis = (int)Input.GetAxisRaw("Jump");

        #endregion

        velocity.x = xAxis * movementSettings.runSpeed;

        // development movement
        if (Utils.DEVMODE)
        {
            velocity = new Vector3(xAxis, yAxis, 0).normalized * movementSettings.runSpeed * 2;

            transform.position += velocity * Time.deltaTime;

            return;
        }

        // flip the player model to match the direction of the players velocity
        if (Mathf.Abs(velocity.x) != 0)
        {
            transform.eulerAngles = (velocity.x > 0 ? Vector3.zero : Vector3.up * 180.0f);
        }

        // reset jump count if the player becomes grounded
        if(!character.isGrounded)
        {
            velocity.y += Physics.gravity.y * movementSettings.gravityScale * Time.fixedDeltaTime;

            if(jumpAxis == 1 && extraJumpTimer > 0 && velocity.y > 0)
            {
                velocity.y += movementSettings.extraJumpForce * Time.deltaTime;
                extraJumpTimer = Mathf.Max(0.0f, extraJumpTimer - Time.fixedDeltaTime);
            }
        }
        else
        {
            velocity.y = -0.1f;
            currentJumps = movementSettings.jumpCount;
            extraJumpTimer = movementSettings.extraJumpTime;
        }

        if (yAxis < 0)
        {
            character.ignoreOneWayPlatformsThisFrame = true;
            Invoke("enableOneWayPlatforms", 0.25f);
        }

        // kill velocity when hitting a roof
        if (character.collisionState.above)
        {
            velocity.y = 0;
        }

        // update jump held
        if (!jumpHeld && jumpAxis == 1)
        {
            jumpHeld = true;

            if(currentJumps > 0)
            {
                velocity.y = movementSettings.jumpHeight;
                currentJumps--;
            }
        }
        if(jumpHeld && jumpAxis == 0)
        {
            jumpHeld = false;
        }

        character.move(velocity * Time.fixedDeltaTime);

        changeColor(character.isGrounded ? Color.red : Color.white);
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

    public void collisionFunction(RaycastHit2D hitInfo)
    {
        // inkable surface
        if (hitInfo.collider.gameObject.GetComponentInChildren<inkableSurface>() != null)
        {
            // inked
            if (hitInfo.collider.gameObject.GetComponentInChildren<inkableSurface>().Inked)
            {
                GameObject inkBlot = Instantiate(InkBlotPrefab);
                inkBlot.name = "inkblot";
                inkBlot.transform.position = transform.position;
                inkBlot.transform.parent = hitInfo.collider.gameObject.transform;
                inkBlot.GetComponent<InkBlot>().player = this.gameObject;
                this.gameObject.SetActive(false);
            }
        }
    }

    public void triggerFunction(Collider2D col)
    {
        if(col.tag == "reset")
        {
            Utils.resetPlayer();
        }
        else if (col.tag == "checkpoint")
        {
            Utils.updateCheckpoint(col.transform.position);
        }
    }
}
