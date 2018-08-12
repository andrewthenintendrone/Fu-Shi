using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
//[RequireComponent(typeof(Rigidbody2D))]
public class Player2 : MonoBehaviour
{
    #region fields

    // earth gravity constant (9.81 m/s^2)
    private const float gravityConstant = -9.81f;

    // whether the player is grounded or not
    private bool grounded = false;

    // whether the player is above a slope or not
    private bool aboveSlope = false;

    // BoxCollider2D reference
    private BoxCollider2D col;

    // RigidBody2D reference
    //private Rigidbody2D rb;

    // x axis movement speed
    public float moveSpeed;

    // lasyer mask to interact with
    public LayerMask layerMask;

    // player velocity
    private Vector3 velocity;

    #endregion

    private void Awake()
    {
        // store component references
        col = GetComponent<BoxCollider2D>();
        //rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        checkGrounded();
        checkSlope();

        float xAxis = Input.GetAxis("Horizontal");

        velocity.x = xAxis * moveSpeed;

        if(!grounded)
        {
            velocity.y += gravityConstant * Time.fixedDeltaTime;
        }
        else
        {
            velocity.y = 0;
        }

        changeColor(Color.red);

        //if(aboveSlope)
        //{
        //    changeColor(Color.red);
        //}
        //else
        //{
        //    changeColor(Color.white);
        //}

        transform.position += velocity * Time.fixedDeltaTime;
    }

    // updates whether the player is grounded
    private void checkGrounded()
    {
        RaycastHit2D hitInfo;

        hitInfo = (Physics2D.Raycast(col.bounds.center, Vector2.down, layerMask));

        // ray hit something
        if(hitInfo.collider != null)
        {
            if(hitInfo.distance < col.bounds.extents.y)
            {
                grounded = true;
                transform.position += Vector3.up * (hitInfo.distance - hitInfo.distance);
                return;
            }
        }

        grounded = false;
    }

    private void checkSlope()
    {
        RaycastHit2D hitInfo;
        RaycastHit2D leftHitInfo;
        RaycastHit2D rightHitInfo;

        hitInfo = Physics2D.Raycast(col.bounds.center, Vector2.down, layerMask);
        leftHitInfo = Physics2D.Raycast(new Vector2(col.bounds.min.x, col.bounds.center.y), Vector2.down, layerMask);
        rightHitInfo = Physics2D.Raycast(new Vector2(col.bounds.max.x, col.bounds.center.y), Vector2.down, layerMask);

        float hitDistanceLeft = 0;
        float hitDistanceRight = 0;

        // we are on a slope in the following conditions
        // the hit distances of the two raycasts are different
        // the normal direction of at least one slope is not straight up
        if(leftHitInfo.collider != null)
        {
            hitDistanceLeft = leftHitInfo.distance;
        }
        if(rightHitInfo.collider != null)
        {
            hitDistanceRight = rightHitInfo.distance;
        }

        if(hitDistanceLeft != hitDistanceRight)
        {
            if(Vector3.Angle(leftHitInfo.normal, Vector3.up) > 5)
            {
                aboveSlope = true;
                return;
            }
            if(Vector3.Angle(rightHitInfo.normal, Vector3.up) > 5)
            {
                aboveSlope = true;
                return;
            }
        }

        aboveSlope = false;
    }

    void changeColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }
}
