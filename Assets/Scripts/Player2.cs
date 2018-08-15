﻿using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public struct MovementSettings2
{
    public float runSpeed;

    public float dashSpeed;

    public float jumpHeight;

    public int jumpCount;

    public float extraJumpForce;

    public float extraJumpTime;

    public float gravityScale;
}

public class Player2 : MonoBehaviour
{
    private CharacterController2D character;
    private Vector3 velocity;

    private int currentJumps;
    public float extraJumpTimer;
    bool jumpHeld = false;

    public MovementSettings2 movementSettings;

    private void Start()
    {
        Utils.Init();
        character = GetComponent<CharacterController2D>();
        character.onTriggerEnterEvent += triggerFunction;
        currentJumps = movementSettings.jumpCount;
        extraJumpTimer = movementSettings.extraJumpTime;
    }

    void FixedUpdate ()
    {
        #region get inputs

        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        int jumpAxis = (int)Input.GetAxisRaw("Jump");
        int dashAxis = (int)Input.GetAxisRaw("Dash");

        #endregion

        if (dashAxis == 1)
        {
            velocity.x = xAxis * movementSettings.dashSpeed;
        }
        else
        {
            velocity.x = xAxis * movementSettings.runSpeed;
        }

        // development movement
        if (Utils.DEVMODE)
        {
            velocity = new Vector3(xAxis, yAxis, 0).normalized * movementSettings.runSpeed;

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
        }
        else
        {
            velocity.y = -0.1f;
            currentJumps = movementSettings.jumpCount;
            extraJumpTimer = movementSettings.extraJumpTime;
        }

        // kill velocity when hitting a roof
        if(character.collisionState.above)
        {
            velocity.y = 0;
        }

        // update jump held
        if(!jumpHeld && jumpAxis == 1)
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
