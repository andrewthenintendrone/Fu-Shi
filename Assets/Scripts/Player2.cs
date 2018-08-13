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

    // jump force
    public float jumpForce;

    // gravity multiplier
    public float gravityMultiplier;

    // lasyer mask to interact with
    public LayerMask layerMask;

    public float minDistanceToFloor;

    // player velocity
    public Vector3 velocity;

    #endregion

    private void Awake()
    {
        // store component references
        col = GetComponent<BoxCollider2D>();
        //rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        checkGround();

        float xAxis = Input.GetAxis("Horizontal");

        if(xAxis > 0)
        {
            checkRight();
        }
        if(xAxis < 0)
        {
            checkLeft();
        }

        float jumpAxis = (int)Input.GetAxisRaw("Fire1");

        velocity.x = xAxis * moveSpeed;

        if (!grounded)
        {
            velocity.y += gravityConstant * gravityMultiplier * Time.fixedDeltaTime;
        }
        else
        {
            velocity.y = 0;

            transform.position += Vector3.up * (col.bounds.extents.y - minDistanceToFloor);

            if (jumpAxis == 1)
            {
                velocity.y = jumpForce;
            }
        }

        if (aboveSlope)
        {
            changeColor(Color.red);
        }
        else
        {
            changeColor(Color.white);
        }

        transform.position += velocity * Time.fixedDeltaTime;
    }

    // moves the player out of wall geometry to the left
    private void checkLeft()
    {
        RaycastHit2D rayLeft = Physics2D.Raycast(col.bounds.center, Vector2.left, layerMask);

        if(rayLeft.collider != null)
        {
            if (rayLeft.distance < col.bounds.extents.x)
            {
                transform.position += Vector3.right * (col.bounds.extents.x - rayLeft.distance);
            }
        }
    }

    // moves the player out of wall geometry to the right
    private void checkRight()
    {
        RaycastHit2D rayRight = Physics2D.Raycast(col.bounds.center, Vector2.right, layerMask);

        if(rayRight.collider != null)
        {
            if (rayRight.distance < col.bounds.extents.x)
            {
                transform.position += Vector3.left * (col.bounds.extents.x - rayRight.distance);
            }
        }
    }

    // updates whether the player is grounded and on a slope
    private void checkGround()
    {
        minDistanceToFloor = col.bounds.extents.y + 1;

        RaycastHit2D hitInfo;
        RaycastHit2D hitInfoLeft;
        RaycastHit2D hitInfoRight;

        hitInfo = (Physics2D.Raycast(col.bounds.center, Vector2.down, layerMask));
        hitInfoLeft = (Physics2D.Raycast(new Vector2(col.bounds.min.x, col.bounds.center.y), Vector2.down, layerMask));
        hitInfoRight = (Physics2D.Raycast(new Vector2(col.bounds.max.x, col.bounds.center.y), Vector2.down, layerMask));

        // ray hit something
        if (hitInfo.collider != null)
        {
            //Debug.DrawLine(col.bounds.center, hitInfo.point, Color.green);
            minDistanceToFloor = Mathf.Min(minDistanceToFloor, hitInfo.distance);
        }
        if (hitInfoLeft.collider != null)
        {
            //Debug.DrawLine(new Vector3(col.bounds.min.x, col.bounds.center.y, 0), hitInfoLeft.point, Color.red);
            minDistanceToFloor = Mathf.Min(minDistanceToFloor, hitInfoLeft.distance);
        }
        if (hitInfoRight.collider != null)
        {
            //Debug.DrawLine(new Vector3(col.bounds.max.x, col.bounds.center.y, 0), hitInfoRight.point, Color.blue);
            minDistanceToFloor = Mathf.Min(minDistanceToFloor, hitInfoRight.distance);
        }

        Debug.DrawLine(col.bounds.center, col.bounds.center + Vector3.down * minDistanceToFloor, Color.yellow);

        grounded = minDistanceToFloor <= col.bounds.extents.y;

        bool slopeLeft = Mathf.Abs(Vector3.Angle(hitInfoLeft.normal, Vector3.up)) >= 5;
        bool slopeRight = Mathf.Abs(Vector3.Angle(hitInfoRight.normal, Vector3.up)) >= 5;

        aboveSlope = (slopeLeft && slopeRight) ||
                       (slopeLeft && !slopeRight && hitInfoLeft.point.y >= hitInfoRight.point.y) ||
                       (!slopeLeft && slopeRight && hitInfoRight.point.y >= hitInfoLeft.point.y);
    }

    void changeColor(Color color)
    {
        GetComponent<Renderer>().material.color = color;
    }
}
