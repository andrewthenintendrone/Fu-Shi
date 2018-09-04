using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualForwardFocusCamera : MonoBehaviour
{

    [System.NonSerialized]
    [HideInInspector]
    public new Camera camera;
    public Collider2D targetCollider;


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
                    throw new UnityException("CameraKit2D does not appear to exist");
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

        // var allCameraBehaviors = GetComponents<ICameraBaseBehavior>();
        //handle gizmo drawing here
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
