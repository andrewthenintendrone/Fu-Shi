using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : MonoBehaviour
{
    // current velocity
    private Vector3 velocity = Vector3.zero;

    private BoxCollider2D mCollider;

    // movement speed from stick
    public float movementSpeed;

    // height that is jumped to
    public float jumpHeight;

    // ground layer mask
    public LayerMask groundLayer;

    // Use this for initialization
    void Start()
    {
        mCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // most basic left / right movement
        float xAxis = Input.GetAxis("Horizontal");
        velocity.x = xAxis * movementSpeed;

        // most basic gravity
        if (!isGrounded())
        {
            velocity += Physics.gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }

        // most basic jump
        int jumpAxis = (int)Input.GetAxisRaw("Fire1");
        if (jumpAxis == 1)
        {
            Jump();
        }

        unClip();

        // translation
        transform.Translate(velocity * Time.deltaTime);
    }

    bool isGrounded()
    {
        Debug.DrawRay(mCollider.bounds.center, Vector3.down * mCollider.bounds.extents.y, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(mCollider.bounds.center, Vector3.down, mCollider.bounds.extents.y, groundLayer);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    void unClip()
    {
        RaycastHit2D hit = Physics2D.Raycast(mCollider.bounds.center, Vector3.down, mCollider.bounds.extents.y, groundLayer);
        if(hit.collider != null)
        {
            if(hit.distance < mCollider.bounds.extents.y)
            {
                transform.position += Vector3.up * hit.distance;
            }
        }

        hit = Physics2D.Raycast(mCollider.bounds.center, Vector3.up, mCollider.bounds.extents.y, groundLayer);
        if (hit.collider != null)
        {
            if (hit.distance < mCollider.bounds.extents.y)
            {
                transform.position += Vector3.down * hit.distance;
            }
        }

        hit = Physics2D.Raycast(mCollider.bounds.center, Vector3.left, mCollider.bounds.extents.x, groundLayer);
        if (hit.collider != null)
        {
            if (hit.distance < mCollider.bounds.extents.x)
            {
                transform.position += Vector3.right * hit.distance;
            }
        }

        hit = Physics2D.Raycast(mCollider.bounds.center, Vector3.right, mCollider.bounds.extents.x, groundLayer);
        if (hit.collider != null)
        {
            if (hit.distance < mCollider.bounds.extents.x)
            {
                transform.position += Vector3.left * hit.distance;
            }
        }
    }

    // returns true if the Player is above a slope
    bool isAboveSlope()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(mCollider.bounds.center + Vector3.left * mCollider.bounds.extents.x, Vector2.down, mCollider.bounds.extents.y);
        RaycastHit2D rightHit = Physics2D.Raycast(mCollider.bounds.center + Vector3.right * mCollider.bounds.extents.x, Vector2.down, mCollider.bounds.extents.y);

        Debug.DrawRay(mCollider.bounds.center + Vector3.left * mCollider.bounds.extents.x, Vector3.down * mCollider.bounds.extents.y, Color.red);
        Debug.DrawRay(mCollider.bounds.center + Vector3.right * mCollider.bounds.extents.x, Vector3.down * mCollider.bounds.extents.y, Color.red);

        return false;
    }

    void Jump()
    {
        if(isGrounded())
        {
            return;
        }
        else
        {
            // Jump
        }
    }
}
