using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    Vector3 velocity = Vector3.zero;

    public Transform targetTransform;

    public float smoothTime;

    public enum TrackType
    {
        Snap,
        Lerp,
        SmoothDamp
    }

    public TrackType trackType;

    void Update()
    {
        switch (trackType)
        {
            case TrackType.Snap:
                Snap();
                break;
            case TrackType.Lerp:
                Lerp();
                break;
            case TrackType.SmoothDamp:
                SmoothDamp();
                break;
            default:
                Snap();
                break;
        }
    }

    void Snap()
    {
        transform.position = targetTransform.position;
    }

    void Lerp()
    {
        transform.position = Vector3.Lerp(transform.position, targetTransform.position, smoothTime);
    }

    void SmoothDamp()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetTransform.position, ref velocity, smoothTime);
    }
}
