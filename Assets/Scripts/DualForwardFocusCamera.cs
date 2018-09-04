using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualForwardFocusCamera : MonoBehaviour
{

    [System.NonSerialized]
    [HideInInspector]
    public new Camera camera;
    public Collider2D targetCollider;

    [Range(0f, 20f)]
    public float width = 3f;

    private Vector3 basePosition;

    [Tooltip("width of the detector / outer lines of the dual forward docus system")]
    [Range(0.5f, 5f)]
    public float dualForwardFocusThresholdExtents = 0.5f;

    [Range(0.5f, 5f)]
    public float dualVerticalFocusThresholdExtents = 0.5f;

    public RectTransform.Edge _currentEdgeFocusHoriz;
    public RectTransform.Edge _currentEdgeFocusVert;

    Transform _transform;
    private static DualForwardFocusCamera _instance;

    public static DualForwardFocusCamera instance
    {
        get
        {
            if (System.Object.Equals(_instance, null))
            {
                _instance = FindObjectOfType(typeof(DualForwardFocusCamera)) as DualForwardFocusCamera;

                if (System.Object.Equals(_instance, null))
                    throw new UnityException("cameraScript does not appear to exist");
            }

            return _instance;
        }
    }

    #region MonoBehaviour

    void Awake()
    {
        _instance = this;
        _transform = GetComponent<Transform>();
        camera = GetComponent<Camera>();

    }


    void FixedUpdate()
    {
        var targetBounds = targetCollider.bounds;

        // we keep track of the target's velocity since some camera behaviors need to know about it
        //TODO use player velocity value
        //var velocity = (targetBounds.center - _targetPositionLastFrame) / Time.deltaTime;
        //velocity.z = 0f;


        // fetch the average velocity for use in our camera behaviors
        //TODO not sure whether we need this :confirm
        //var targetAvgVelocity = _averageVelocityQueue.average();


        // we use the transform.position plus the offset when passing the base position to our camera behaviors
        //var basePosition = getNormalizedCameraPosition();
        //var accumulatedDeltaOffset = Vector3.zero;


        //this is the position the camera wants to sit at
        //position cameraBehavior                     




        //Smoothing section





        // reset Z just in case one of the other scripts messed with it
        //desiredPosition.z = _transform.position.z;



        //pretty gizmo stuff
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        var positionInFrontOfCamera = getNormalizedCameraPosition();
        positionInFrontOfCamera.z = 1f;

        
        Gizmos.color = new Color(0f, 0.5f, 0.6f);

        var bounds = new Bounds(basePosition, new Vector3(width, 10f));
        var lineWidth = Camera.main.orthographicSize;

        bounds.center = new Vector3(bounds.center.x, basePosition.y, bounds.center.z);
        bounds.Expand(new Vector3(0f, lineWidth - bounds.size.y));

        Gizmos.DrawLine(bounds.min, bounds.min + new Vector3(0f, bounds.size.y));
        Gizmos.DrawLine(bounds.max, bounds.max - new Vector3(0f, bounds.size.y));

        bounds.Expand(new Vector3(dualForwardFocusThresholdExtents, 1f));
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(bounds.min, bounds.min + new Vector3(0f, bounds.size.y));
        Gizmos.DrawLine(bounds.max, bounds.max - new Vector3(0f, bounds.size.y));
    }
#endif


    void OnApplicationQuit()
    {
        _instance = null;
    }

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
