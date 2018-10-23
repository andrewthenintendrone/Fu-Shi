using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Swing : MonoBehaviour
{
    [SerializeField]
    [Tooltip("flip the direction of the swing")]
    private bool flip = false;

    [SerializeField]
    [Tooltip("how fast to swing")]
    private float swingSpeed;

    [SerializeField]
    [Tooltip("maximum angle to swing")]
    private float maxSwingAngle;

	void FixedUpdate ()
    {
        transform.eulerAngles = Vector3.forward * Mathf.Sin(Time.fixedTime * swingSpeed) * maxSwingAngle * (flip ? -1 : 1);
	}

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 linePos = Vector3.zero;
        Vector3 prevLinePos = Vector3.zero;

        for (float i = -maxSwingAngle; i < maxSwingAngle; i++)
        {
            prevLinePos = linePos;
            linePos = new Vector3(Mathf.Sin(i), Mathf.Cos(i), 0) * 100.0f;
            Gizmos.DrawLine(prevLinePos, linePos);
        }
    }

#endif
}
