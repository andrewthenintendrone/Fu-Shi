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


    [Tooltip("height at which the camera enters panic mode and attempts to catch up to the player")]
    [Range(0f,60f)]
    public float panicModeThreshHold = 10f;

    public float normalizedClampedDistance;


    // which side is being focused on
    private RectTransform.Edge currentFocus = RectTransform.Edge.Left;

    [Tooltip("the lag or delay to smooth the camera on the X axis")]
    [SerializeField]
    private float smoothX;

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

        #region dualForwardFocusLogic

        // offset from the bounds
        Vector3 deltaPositionFromBounds = Vector3.zero;

        // did the x edge focus change this frame
        bool didLastXEdgeContactChange = false;

        // worldspace position of rect edges
        float leftEdge, rightEdge;


        // calculate the positions of the relevent edges on the x axis
        if (currentFocus == RectTransform.Edge.Left)
        {
            rightEdge = basePosition.x - width * 0.5f;
            leftEdge = rightEdge - XThresholdExtents * 0.5f;
        }
        else
        {
            leftEdge = basePosition.x + width * 0.5f;
            rightEdge = leftEdge + XThresholdExtents * 0.5f;
        }

        // the player has left the focus area on the x axis
        if (targetBounds.center.x < leftEdge)
        {
            deltaPositionFromBounds.x = targetBounds.center.x - leftEdge;

            // change focus edge
            if (currentFocus == RectTransform.Edge.Left)
            {
                didLastXEdgeContactChange = true;
                currentFocus = RectTransform.Edge.Right;
            }
        }
        else if (targetBounds.center.x > rightEdge)
        {
            deltaPositionFromBounds.x = targetBounds.center.x - rightEdge;

            // change focus edge
            if (currentFocus == RectTransform.Edge.Right)
            {
                didLastXEdgeContactChange = true;
                currentFocus = RectTransform.Edge.Left;
            }
        }
        
        // calculate the desired position
        float desiredX = (currentFocus == RectTransform.Edge.Left ? rightEdge : leftEdge);
        desiredOffset.x = targetBounds.center.x - desiredX;

        // if we didnt change focus this frame update the desired offset
        if (!didLastXEdgeContactChange)
        {
            desiredOffset.x = deltaPositionFromBounds.x;
        }

        #endregion

        // calculate player distance from center
        float distanceFromCenter = Mathf.Abs(targetBounds.center.y - basePosition.y);
        normalizedClampedDistance = Mathf.Clamp01((distanceFromCenter - (height * 0.5f)) / (panicModeThreshHold * 0.5f));

        // update the target position
        Vector3 targetPosition = transform.position + desiredOffset;

        // smooth the movement to the target position
        targetPosition.x = Mathf.SmoothDamp(transform.position.x, targetPosition.x, ref velocity.x, smoothX);
        // finally set position
        transform.position = targetPosition;

        float desiredY = Mathf.Lerp(basePosition.y, targetBounds.center.y, normalizedClampedDistance);
        Vector3 finalPosition = transform.position;
        finalPosition.y = desiredY;
        transform.position = finalPosition;
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
        bounds.Expand(new Vector3(XThresholdExtents, panicModeThreshHold));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(bounds.min, bounds.min + Vector3.up * bounds.size.y);
        Gizmos.DrawLine(bounds.max, bounds.max + Vector3.down * bounds.size.y);
        Gizmos.color = Color.red;
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
