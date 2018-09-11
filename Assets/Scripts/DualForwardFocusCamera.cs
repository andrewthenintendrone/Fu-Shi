using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualForwardFocusCamera : MonoBehaviour
{
    // camera component
    [System.NonSerialized]
    [HideInInspector]
    public Camera cam;

    // collider to track to
    private Collider2D targetCollider;

    [Tooltip("width of the inner box for camera motion")]
    [Range(0f, 20f)]
    [SerializeField]
    private float width = 3f;

    [Tooltip("height of the inner box for camera motion")]
    [Range(0f, 20f)]
    [SerializeField]
    public float height = 3f;

    [Tooltip("width of the detector / outer lines for vertical switching")]
    [Range(0f, 20f)]
    public float XThresholdExtents = 0.5f;

    [Tooltip("width of the detector / outer lines for vertical switching")]
    [Range(0f, 20f)]
    public float YThresholdExtents = 0.5f;

    // which sides are being focused on on the x and y axis
    private RectTransform.Edge XSideFocus = RectTransform.Edge.Left;
    private RectTransform.Edge YSideFocus = RectTransform.Edge.Bottom;

    [Tooltip("the lag or delay to smooth the camera")]
    [SerializeField]
    private float smoothTime;

    // current velocity
    private Vector3 velocity;
    
    #region MonoBehaviour

    void Awake()
    {
        // store component references
        cam = GetComponent<Camera>();
        targetCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>();
    }

    void FixedUpdate()
    {
        Bounds targetBounds = targetCollider.bounds;
        Vector3 desiredOffset = Vector3.zero;
        Vector3 basePosition = getNormalizedCameraPosition();

        // offset from the bounds
        Vector3 deltaPositionFromBounds = Vector3.zero;

        // did the x or y edge focus change this frame
        bool didLastXEdgeContactChange = false;
        bool didLastYEdgeContactChange = false;

        // worldspace position of rect edges
        float leftEdge, rightEdge, topEdge, bottomEdge;

        // calculate the positions of the relevent edges on the x axis
        if (XSideFocus == RectTransform.Edge.Left)
        {
            rightEdge = basePosition.x - width * 0.5f;
            leftEdge = rightEdge - XThresholdExtents * 0.5f;
        }
        else
        {
            leftEdge = basePosition.x + width * 0.5f;
            rightEdge = leftEdge + XThresholdExtents * 0.5f;
        }

        // calculate the positions of the relevent edges on the y axis
        if (YSideFocus == RectTransform.Edge.Top)
        {
            bottomEdge = basePosition.y + height * 0.5f;
            topEdge = bottomEdge + YThresholdExtents * 0.5f;
        }
        else
        {
            topEdge = basePosition.y - height * 0.5f;
            bottomEdge = topEdge - YThresholdExtents * 0.5f;
        }

        // the player has left the focus area on the x axis
        if (targetBounds.center.x < leftEdge)
        {
            deltaPositionFromBounds.x = targetBounds.center.x - leftEdge;

            // change focus edge
            if (XSideFocus == RectTransform.Edge.Left)
            {
                didLastXEdgeContactChange = true;
                XSideFocus = RectTransform.Edge.Right;
            }
        }
        else if (targetBounds.center.x > rightEdge)
        {
            deltaPositionFromBounds.x = targetBounds.center.x - rightEdge;

            // change focus edge
            if (XSideFocus == RectTransform.Edge.Right)
            {
                didLastXEdgeContactChange = true;
                XSideFocus = RectTransform.Edge.Left;
            }
        }

        // // the player has left the focus area on the y axis
        if (targetBounds.center.y < bottomEdge)
        {
            deltaPositionFromBounds.y = targetBounds.center.y - bottomEdge;

            // change focus edge
            if (YSideFocus == RectTransform.Edge.Bottom)
            {
                didLastYEdgeContactChange = true;
                YSideFocus = RectTransform.Edge.Top;
            }
        }
        else if (targetBounds.center.y > topEdge)
        {
            deltaPositionFromBounds.y = targetBounds.center.y - topEdge;

            // change focus edge
            if (YSideFocus == RectTransform.Edge.Top)
            {
                didLastYEdgeContactChange = true;
                YSideFocus = RectTransform.Edge.Bottom;
            }
        }

        // calculate the desired position
        float desiredX = (XSideFocus == RectTransform.Edge.Left ? rightEdge : leftEdge);
        desiredOffset.x = targetBounds.center.x - desiredX;

        float desiredY = (YSideFocus == RectTransform.Edge.Top ? bottomEdge : topEdge);
        desiredOffset.y = targetBounds.center.y - desiredY;

        // if we didnt change focus this frame update the desired offset
        if (!didLastXEdgeContactChange)
        {
            desiredOffset.x = deltaPositionFromBounds.x;
        }

        if(!didLastYEdgeContactChange)
        {
            desiredOffset.y = deltaPositionFromBounds.y;
        }

        // update the target position
        Vector3 targetPosition = transform.position + desiredOffset;

        // smooth the movement to the target position
        targetPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // finally set position
        transform.position = targetPosition;
    }

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        // calculate positions
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
        bounds.Expand(new Vector3(XThresholdExtents, YThresholdExtents));
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
        return GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
    }
}
