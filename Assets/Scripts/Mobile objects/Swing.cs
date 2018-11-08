using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float actualTime = 0;

	void FixedUpdate ()
    {
        if(!Utils.gamePaused)
        {
            actualTime += Time.fixedDeltaTime;
            transform.eulerAngles = Vector3.forward * Mathf.Sin(actualTime * swingSpeed) * maxSwingAngle * (flip ? -1 : 1);
        }
	}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Vector3 linePos = transform.position;
        Vector3 prevLinePos = transform.position;

        for (float i = -32; i < 33; i++)
        {
            prevLinePos = linePos;
            linePos = transform.position - new Vector3(Mathf.Sin(Mathf.Deg2Rad * i / 32 * maxSwingAngle), Mathf.Cos(Mathf.Deg2Rad * i / 32 * maxSwingAngle), 0) * 32.0f;
            Gizmos.DrawLine(prevLinePos, linePos);
        }
        Gizmos.DrawLine(linePos, transform.position);
    }
}
