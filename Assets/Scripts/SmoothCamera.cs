using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SmoothCamera : MonoBehaviour
{
    private Transform target; // object to target (Player)
    
    [Tooltip("x axis margin")]
    public float marginX = 0.0f;

    [Tooltip("y axis margin")]
    public float marginY = 0.0f;

    [Tooltip("how low the default position of the camera rests relative to the player")]
    public float offsetY = 0.0f;
    [Tooltip("how much to smooth motion")]
    public float smoothTime;
    private Vector3 velocity = Vector3.zero;

    // Camera component
    private Camera cam;

    private void Start()
    {
        // get reference to camera component
        cam = GetComponent<Camera>();

        // get reference to player
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate ()
    {
        Vector3 point = cam.WorldToViewportPoint(target.position);
        Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        Vector3 offset = new Vector3(0, offsetY);

        Vector3 desiredPosition = transform.position + delta + offset;

        if(Mathf.Abs(delta.x) > marginX)
        {
            desiredPosition.x += delta.x;
        }
        if (Mathf.Abs(delta.y) > marginY)
        {
            desiredPosition.y += delta.y;
        }

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }
}
