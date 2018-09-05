﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualForwardFocusCamera : MonoBehaviour
{

    [System.NonSerialized]
    [HideInInspector]
    public new Camera camera;


    private Collider2D targetCollider;

    [Range(0f, 20f)]
    [SerializeField]
    private float width = 3f;

    [Range(0f, 20f)]
    [SerializeField]
    public float height = 3f;

    

    [Tooltip("width of the detector / outer lines of the dual forward focus system")]
    [Range(0f, 20f)]
    public float dualForwardFocusThresholdExtents = 0.5f;

    [Range(0f, 20f)]
    public float dualVerticalFocusThresholdExtents = 0.5f;

    private RectTransform.Edge XSideFocus;
    private RectTransform.Edge YSideFocus;

    [SerializeField]
    private float smoothTime;

    // current velocity
    private Vector3 velocity;



    
    #region MonoBehaviour

    void Awake()
    {
        
        camera = GetComponent<Camera>();
        targetCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
    }


    void FixedUpdate()
    {
        Bounds targetBounds = targetCollider.bounds;


        Vector3 desiredOffset = Vector3.zero;

        Vector3 basePosition = getNormalizedCameraPosition();


        Vector3 deltaPositionFromBounds = Vector3.zero;
        bool didLastEdgeContactChange = false;
        //worldspace position of rect edges
        float leftEdge, rightEdge, topEdge, bottomEdge;

        //if the camera focus is a certain side generate the two bound lines on X 
        if (XSideFocus == RectTransform.Edge.Left)
        {
            rightEdge = basePosition.x - width * 0.5f;
            leftEdge = rightEdge - dualForwardFocusThresholdExtents * 0.5f;
        }
        else
        {
            leftEdge = basePosition.x + width * 0.5f;
            rightEdge = leftEdge + dualForwardFocusThresholdExtents * 0.5f;
        }

        //as above but generating the Y component
        if (YSideFocus == RectTransform.Edge.Top)
        {
            bottomEdge = basePosition.y + height * 0.5f;
            topEdge = bottomEdge + dualVerticalFocusThresholdExtents * 0.5f;
        }
        else
        {
            topEdge = basePosition.y - height * 0.5f;
            bottomEdge = topEdge - dualVerticalFocusThresholdExtents * 0.5f;
        }

        
        //if the player bounds is beyond the outside edge
        if (leftEdge > targetBounds.center.x)
        {
            //how far the player has passed outside the bounds
            deltaPositionFromBounds.x = targetBounds.center.x - leftEdge;
            //if the player is on the left side of the screen
            //swap the focus
            if (XSideFocus == RectTransform.Edge.Left)
            {
                didLastEdgeContactChange = true;
                XSideFocus = RectTransform.Edge.Right;
            }
        }
        else if (rightEdge < targetBounds.center.x)
        {
            deltaPositionFromBounds.x = targetBounds.center.x - rightEdge;

            if (XSideFocus == RectTransform.Edge.Right)
            {
                didLastEdgeContactChange = true;
                XSideFocus = RectTransform.Edge.Left;
            }
        }

        //if the player has gone beyond a lower edge
        if (bottomEdge > targetBounds.center.y)
        {
            //how far the player has passed outside the bounds
            deltaPositionFromBounds.y = targetBounds.center.y - bottomEdge;
            //if the player is on the bottom side of the screen
            //swap the focus
            if (YSideFocus == RectTransform.Edge.Bottom)
            {
                didLastEdgeContactChange = true;
                YSideFocus = RectTransform.Edge.Top;
            }
        }
        else if (topEdge < targetBounds.center.x)
        {
            deltaPositionFromBounds.x = targetBounds.center.x - rightEdge;

            if (YSideFocus == RectTransform.Edge.Top)
            {
                didLastEdgeContactChange = true;
                YSideFocus = RectTransform.Edge.Bottom;
            }
        }


        float desiredY = (YSideFocus == RectTransform.Edge.Top ? bottomEdge : topEdge);
        desiredOffset.y = targetBounds.center.y - desiredY;

        float desiredX = (XSideFocus == RectTransform.Edge.Left ? rightEdge : leftEdge);
        desiredOffset.x = targetBounds.center.x - desiredX;

        // if we didnt switch direction this works much like a normal camera window
        if (!didLastEdgeContactChange)
        {
            desiredOffset.x = deltaPositionFromBounds.x;
            desiredOffset.y = deltaPositionFromBounds.y;
        }
           








        Vector3 targetPosition = transform.position + desiredOffset;


        targetPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // bad hack
        //targetPosition.y = targetBounds.center.y;


        transform.position = targetPosition;

    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Vector3 positionInFrontOfCamera = getNormalizedCameraPosition();
        positionInFrontOfCamera.z = transform.position.z - 1;

        
        Gizmos.color = new Color(0f, 0.5f, 0.6f);

        Bounds bounds = new Bounds(positionInFrontOfCamera, new Vector3(width, height));
        float lineWidth = Camera.main.orthographicSize;

        bounds.center = new Vector3(bounds.center.x, positionInFrontOfCamera.y, bounds.center.z);

        //draw inner bounding lines
        Gizmos.DrawLine(bounds.min, bounds.min + Vector3.up * bounds.size.y);
        Gizmos.DrawLine(bounds.max, bounds.max + Vector3.down * bounds.size.y);
        Gizmos.DrawLine(bounds.min + Vector3.up * bounds.size.y, bounds.max);
        Gizmos.DrawLine(bounds.max + Vector3.down * bounds.size.y, bounds.min);


        //draw outer detection bounds
        bounds.Expand(new Vector3(dualForwardFocusThresholdExtents, dualVerticalFocusThresholdExtents));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(bounds.min, bounds.min + Vector3.up * bounds.size.y);
        Gizmos.DrawLine(bounds.max, bounds.max + Vector3.down * bounds.size.y);
        Gizmos.DrawLine(bounds.min + Vector3.up * bounds.size.y, bounds.max);
        Gizmos.DrawLine(bounds.max + Vector3.down * bounds.size.y, bounds.min);


    }
#endif

    #endregion

    Vector3 getNormalizedCameraPosition()
    {
        //Camera.main.ViewportToWorldPoint()
#if UNITY_EDITOR
        return GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
#else
		return camera.ViewportToWorldPoint( new Vector3( 0.5f + horizontalOffset, 0.5f + verticalOffset, 0f ) );
#endif


    }
}
