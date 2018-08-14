using UnityEngine;
using System;
using System.Collections.Generic;

public class Player2 : MonoBehaviour
{
    public float moveSpeed;
    public float jumpHeight;
    public float dashSpeed;
    public float gravityScale;
    private CharacterController2D character;
    private Vector3 velocity;

    private int jumpCount = 2;
    bool jumpHeld = false;

    private void Start()
    {
        Utils.Init();
        character = GetComponent<CharacterController2D>();
        character.onTriggerEnterEvent += triggerFunction;
    }

    void FixedUpdate ()
    {
        float xAxis = Input.GetAxis("Horizontal");
        int jumpAxis = (int)Input.GetAxisRaw("Fire1");
        int dashAxis = (int)Input.GetAxisRaw("Fire2");

        if (Utils.DEVMODE)
        {
            float yAxis = Input.GetAxis("Vertical");
            velocity = new Vector3(xAxis, yAxis, 0).normalized * moveSpeed;

            transform.position += velocity * Time.deltaTime;

            return;
        }

        velocity.x = xAxis * moveSpeed;

        // rotate model to match direction
        if(Mathf.Abs(velocity.x) != 0)
        {
            transform.eulerAngles = (velocity.x > 0 ? Vector3.zero : Vector3.up * 180.0f);
        }

        if(!character.isGrounded)
        {
            velocity.y += Physics.gravity.y * gravityScale * Time.fixedDeltaTime;
        }
        else
        {
            jumpCount = 2;
        }

        // update jump held
        if(!jumpHeld && jumpAxis == 1)
        {
            jumpHeld = true;

            if(jumpCount > 0)
            {
                velocity.y = jumpHeight;
                //jumpCount--;
            }
        }
        if(jumpHeld && jumpAxis == 0)
        {
            jumpHeld = false;
        }

        if (dashAxis == 1)
        {
            velocity.x = xAxis * dashSpeed;
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
        GetComponent<Renderer>().material.color = color;
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
