using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinMove : MonoBehaviour
{

    [SerializeField]
    private float shakeAmt = 0.03f;
    [SerializeField]
    private float shakeSpd = 0.5f;

    private float actualTime = 0;
	
	void Update ()
    {
        if (!Utils.gamePaused)
        {
            actualTime += Time.deltaTime;
            transform.position += Vector3.right * Mathf.Sin(actualTime * shakeSpd) * shakeAmt;
        }   
	}
}
