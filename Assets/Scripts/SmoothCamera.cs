using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SmoothCamera : MonoBehaviour
{
    private Transform target; // object to target (Player)
    public float marginX = 0.0f; // follow margin x
    public float marginY = 0.0f; // follow margin y
    public float smoothTime = 0.125f; // how much to smooth motion
    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate ()
    {
        Vector3 point = cam.WorldToViewportPoint(target.position);
        Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));

        Vector3 desiredPosition = transform.position + delta;
        
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
