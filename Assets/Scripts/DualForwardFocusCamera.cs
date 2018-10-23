using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualForwardFocusCamera : MonoBehaviour
{
    #region variables

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

    [Tooltip("height of outer lines")]
    [Range(0f, 60f)]
    public float YThresholdExtents = 10f;

    public float normalizedClampedDistance;


    // which side is being focused on
    private RectTransform.Edge currentFocus = RectTransform.Edge.Left;

    [Tooltip("the lag or delay to smooth the camera on the X axis")]
    [SerializeField]
    private float smoothX;

    // current x velocity
    private float velocityX;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        // store component references
        cam = GetComponent<Camera>();
        targetCollider = Utils.getPlayer().GetComponent<Collider2D>();
    }

    // called every physics step
    void FixedUpdate()
    {
        if(!Utils.gamePaused)
        {
            Bounds targetBounds = targetCollider.bounds;

            #region xAxisLogic

            // offset from the bounds on the x axis
            float deltaPositionFromBoundsX = 0;

            // did the x edge focus change this frame
            bool didLastXEdgeContactChange = false;

            // worldspace position of the edges of the focus area
            float leftEdge, rightEdge;

            // calculate the left and right edges of the focus area
            if (currentFocus == RectTransform.Edge.Left)
            {
                rightEdge = transform.position.x - width * 0.5f;
                leftEdge = rightEdge - XThresholdExtents * 0.5f;
            }
            else
            {
                leftEdge = transform.position.x + width * 0.5f;
                rightEdge = leftEdge + XThresholdExtents * 0.5f;
            }

            // the player has left the focus area on the left
            if (targetBounds.center.x < leftEdge)
            {
                deltaPositionFromBoundsX = targetBounds.center.x - leftEdge;

                // change focus side
                if (currentFocus == RectTransform.Edge.Left)
                {
                    didLastXEdgeContactChange = true;
                    currentFocus = RectTransform.Edge.Right;
                }
            }
            // the player has left the focus area on the right
            else if (targetBounds.center.x > rightEdge)
            {
                deltaPositionFromBoundsX = targetBounds.center.x - rightEdge;

                // change focus side
                if (currentFocus == RectTransform.Edge.Right)
                {
                    didLastXEdgeContactChange = true;
                    currentFocus = RectTransform.Edge.Left;
                }
            }

            // calculate the desired x position
            float desiredX = (currentFocus == RectTransform.Edge.Left ? rightEdge : leftEdge);

            // if we didnt change focus this frame update the desired X
            if (!didLastXEdgeContactChange)
            {
                desiredX = transform.position.x + deltaPositionFromBoundsX;
            }

            // smooth the desiredX using smoothdamp
            desiredX = Mathf.SmoothDamp(transform.position.x, desiredX, ref velocityX, smoothX);

            #endregion

            #region yAxisLogic

            // calculate player y distance from center
            float distanceFromCenter = Mathf.Abs(targetBounds.center.y - transform.position.y);

            // normalizedClampedDistance is a value from 0 to 1 used to lerp on the y axis
            normalizedClampedDistance = Mathf.Clamp01((distanceFromCenter - (height * 0.5f)) / (YThresholdExtents * 0.5f));

            // smooth the desiredY using lerp
            float desiredY = Mathf.Lerp(transform.position.y, targetBounds.center.y, normalizedClampedDistance);

            #endregion

            // move to the desired position
            transform.position = new Vector3(desiredX, desiredY, transform.position.z);
        }
    }

    // teleports to the player when it dies
    public void TeleportToPlayer()
    {
        transform.position = targetCollider.transform.position;
        transform.position += Vector3.back * 10;
    }

    #region editor

#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        // calculate the center of the camera
        Vector3 positionInFrontOfCamera = transform.position;
        positionInFrontOfCamera.z -= 1;
        
        Gizmos.color = Color.cyan;

        // create inner bounds at center
        Bounds bounds = new Bounds(positionInFrontOfCamera, new Vector3(width, height));

        // draw inner bounds
        Gizmos.DrawLine(bounds.min, bounds.min + Vector3.up * bounds.size.y);
        Gizmos.DrawLine(bounds.max, bounds.max + Vector3.down * bounds.size.y);
        Gizmos.DrawLine(bounds.min + Vector3.up * bounds.size.y, bounds.max);
        Gizmos.DrawLine(bounds.max + Vector3.down * bounds.size.y, bounds.min);

        // expand inner bounds to get outer bounds
        bounds.Expand(new Vector3(XThresholdExtents, YThresholdExtents));

        // draw outer bounds x axis lines
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(bounds.min, bounds.min + Vector3.up * bounds.size.y);
        Gizmos.DrawLine(bounds.max, bounds.max + Vector3.down * bounds.size.y);

        // draw outer bounds y axis lines
        Gizmos.color = Color.red;
        Gizmos.DrawLine(bounds.min + Vector3.up * bounds.size.y, bounds.max);
        Gizmos.DrawLine(bounds.max + Vector3.down * bounds.size.y, bounds.min);
    }

#endif

    #endregion

    #endregion
}
